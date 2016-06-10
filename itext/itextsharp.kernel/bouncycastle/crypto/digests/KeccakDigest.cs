using System;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Digests
{
    /// <summary>
    /// Implementation of Keccak based on following KeccakNISTInterface.c from http://keccak.noekeon.org/
    /// </summary>
    /// <remarks>
    /// Following the naming conventions used in the C source code to enable easy review of the implementation.
    /// </remarks>
    public class KeccakDigest
        : IDigest, IMemoable
    {
        private static readonly ulong[] KeccakRoundConstants = KeccakInitializeRoundConstants();

        private static readonly int[] KeccakRhoOffsets = KeccakInitializeRhoOffsets();

        private static ulong[] KeccakInitializeRoundConstants()
        {
            ulong[] keccakRoundConstants = new ulong[24];
            byte LFSRState = 0x01;

            for (int i = 0; i < 24; i++)
            {
                keccakRoundConstants[i] = 0;
                for (int j = 0; j < 7; j++)
                {
                    int bitPosition = (1 << j) - 1;

                    // LFSR86540

                    bool loBit = (LFSRState & 0x01) != 0;
                    if (loBit)
                    {
                        keccakRoundConstants[i] ^= 1UL << bitPosition;
                    }

                    bool hiBit = (LFSRState & 0x80) != 0;
                    LFSRState <<= 1;
                    if (hiBit)
                    {
                        LFSRState ^= 0x71;
                    }

                }
            }

            return keccakRoundConstants;
        }

        private static int[] KeccakInitializeRhoOffsets()
        {
            int[] keccakRhoOffsets = new int[25];
            int x, y, t, newX, newY;

            int rhoOffset = 0;
            keccakRhoOffsets[(((0) % 5) + 5 * ((0) % 5))] = rhoOffset;
            x = 1;
            y = 0;
            for (t = 1; t < 25; t++)
            {
                //rhoOffset = ((t + 1) * (t + 2) / 2) % 64;
                rhoOffset = (rhoOffset + t) & 63;
                keccakRhoOffsets[(((x) % 5) + 5 * ((y) % 5))] = rhoOffset;
                newX = (0 * x + 1 * y) % 5;
                newY = (2 * x + 3 * y) % 5;
                x = newX;
                y = newY;
            }

            return keccakRhoOffsets;
        }

        protected byte[] state = new byte[(1600 / 8)];
        protected byte[] dataQueue = new byte[(1536 / 8)];
        protected int rate;
        protected int bitsInQueue;
        protected int fixedOutputLength;
        protected bool squeezing;
        protected int bitsAvailableForSqueezing;
        protected byte[] chunk;
        protected byte[] oneByte;

        private void ClearDataQueueSection(int off, int len)
        {
            for (int i = off; i != off + len; i++)
            {
                dataQueue[i] = 0;
            }
        }

        public KeccakDigest()
            : this(288)
        {
        }

        public KeccakDigest(int bitLength)
        {
            Init(bitLength);
        }

        public KeccakDigest(KeccakDigest source)
        {
            CopyIn(source);
        }

        private void CopyIn(KeccakDigest source)
        {
            Array.Copy(source.state, 0, this.state, 0, source.state.Length);
            Array.Copy(source.dataQueue, 0, this.dataQueue, 0, source.dataQueue.Length);
            this.rate = source.rate;
            this.bitsInQueue = source.bitsInQueue;
            this.fixedOutputLength = source.fixedOutputLength;
            this.squeezing = source.squeezing;
            this.bitsAvailableForSqueezing = source.bitsAvailableForSqueezing;
            this.chunk = Arrays.Clone(source.chunk);
            this.oneByte = Arrays.Clone(source.oneByte);
        }

        public virtual string AlgorithmName
        {
            get { return "Keccak-" + fixedOutputLength; }
        }

        public virtual int GetDigestSize()
        {
            return fixedOutputLength / 8;
        }

        public virtual void Update(byte input)
        {
            oneByte[0] = input;

            Absorb(oneByte, 0, 8L);
        }

        public virtual void BlockUpdate(byte[] input, int inOff, int len)
        {
            Absorb(input, inOff, len * 8L);
        }

        public virtual int DoFinal(byte[] output, int outOff)
        {
            Squeeze(output, outOff, fixedOutputLength);

            Reset();

            return GetDigestSize();
        }

        /*
         * TODO Possible API change to support partial-byte suffixes.
         */
        protected virtual int DoFinal(byte[] output, int outOff, byte partialByte, int partialBits)
        {
            if (partialBits > 0)
            {
                oneByte[0] = partialByte;
                Absorb(oneByte, 0, partialBits);
            }

            Squeeze(output, outOff, fixedOutputLength);

            Reset();

            return GetDigestSize();
        }

        public virtual void Reset()
        {
            Init(fixedOutputLength);
        }

        /**
         * Return the size of block that the compression function is applied to in bytes.
         *
         * @return internal byte length of a block.
         */
        public virtual int GetByteLength()
        {
            return rate / 8;
        }

        private void Init(int bitLength)
        {
            switch (bitLength)
            {
                case 128:
                    InitSponge(1344, 256);
                    break;
                case 224:
                    InitSponge(1152, 448);
                    break;
                case 256:
                    InitSponge(1088, 512);
                    break;
                case 288:
                    InitSponge(1024, 576);
                    break;
                case 384:
                    InitSponge(832, 768);
                    break;
                case 512:
                    InitSponge(576, 1024);
                    break;
                default:
                    throw new ArgumentException("must be one of 128, 224, 256, 288, 384, or 512.", "bitLength");
            }
        }

        private void InitSponge(int rate, int capacity)
        {
            if (rate + capacity != 1600)
            {
                throw new InvalidOperationException("rate + capacity != 1600");
            }
            if ((rate <= 0) || (rate >= 1600) || ((rate % 64) != 0))
            {
                throw new InvalidOperationException("invalid rate value");
            }

            this.rate = rate;
            // this is never read, need to check to see why we want to save it
            //  this.capacity = capacity;
            this.fixedOutputLength = 0;
            Arrays.Fill(this.state, (byte)0);
            Arrays.Fill(this.dataQueue, (byte)0);
            this.bitsInQueue = 0;
            this.squeezing = false;
            this.bitsAvailableForSqueezing = 0;
            this.fixedOutputLength = capacity / 2;
            this.chunk = new byte[rate / 8];
            this.oneByte = new byte[1];
        }

        private void AbsorbQueue()
        {
            KeccakAbsorb(state, dataQueue, rate / 8);

            bitsInQueue = 0;
        }

        protected virtual void Absorb(byte[] data, int off, long databitlen)
        {
            long i, j, wholeBlocks;

            if ((bitsInQueue % 8) != 0)
            {
                throw new InvalidOperationException("attempt to absorb with odd length queue");
            }
            if (squeezing)
            {
                throw new InvalidOperationException("attempt to absorb while squeezing");
            }

            i = 0;
            while (i < databitlen)
            {
                if ((bitsInQueue == 0) && (databitlen >= rate) && (i <= (databitlen - rate)))
                {
                    wholeBlocks = (databitlen - i) / rate;

                    for (j = 0; j < wholeBlocks; j++)
                    {
                        Array.Copy(data, (int)(off + (i / 8) + (j * chunk.Length)), chunk, 0, chunk.Length);

                        KeccakAbsorb(state, chunk, chunk.Length);
                    }

                    i += wholeBlocks * rate;
                }
                else
                {
                    int partialBlock = (int)(databitlen - i);
                    if (partialBlock + bitsInQueue > rate)
                    {
                        partialBlock = rate - bitsInQueue;
                    }
                    int partialByte = partialBlock % 8;
                    partialBlock -= partialByte;
                    Array.Copy(data, off + (int)(i / 8), dataQueue, bitsInQueue / 8, partialBlock / 8);

                    bitsInQueue += partialBlock;
                    i += partialBlock;
                    if (bitsInQueue == rate)
                    {
                        AbsorbQueue();
                    }
                    if (partialByte > 0)
                    {
                        int mask = (1 << partialByte) - 1;
                        dataQueue[bitsInQueue / 8] = (byte)(data[off + ((int)(i / 8))] & mask);
                        bitsInQueue += partialByte;
                        i += partialByte;
                    }
                }
            }
        }

        private void PadAndSwitchToSqueezingPhase()
        {
            if (bitsInQueue + 1 == rate)
            {
                dataQueue[bitsInQueue / 8] |= (byte)(1U << (bitsInQueue % 8));
                AbsorbQueue();
                ClearDataQueueSection(0, rate / 8);
            }
            else
            {
                ClearDataQueueSection((bitsInQueue + 7) / 8, rate / 8 - (bitsInQueue + 7) / 8);
                dataQueue[bitsInQueue / 8] |= (byte)(1U << (bitsInQueue % 8));
            }
            dataQueue[(rate - 1) / 8] |= (byte)(1U << ((rate - 1) % 8));
            AbsorbQueue();

            if (rate == 1024)
            {
                KeccakExtract1024bits(state, dataQueue);
                bitsAvailableForSqueezing = 1024;
            }
            else
            {
                KeccakExtract(state, dataQueue, rate / 64);
                bitsAvailableForSqueezing = rate;
            }

            squeezing = true;
        }

        protected virtual void Squeeze(byte[] output, int offset, long outputLength)
        {
            long i;
            int partialBlock;

            if (!squeezing)
            {
                PadAndSwitchToSqueezingPhase();
            }
            if ((outputLength % 8) != 0)
            {
                throw new InvalidOperationException("outputLength not a multiple of 8");
            }

            i = 0;
            while (i < outputLength)
            {
                if (bitsAvailableForSqueezing == 0)
                {
                    KeccakPermutation(state);

                    if (rate == 1024)
                    {
                        KeccakExtract1024bits(state, dataQueue);
                        bitsAvailableForSqueezing = 1024;
                    }
                    else
                    {
                        KeccakExtract(state, dataQueue, rate / 64);
                        bitsAvailableForSqueezing = rate;
                    }
                }
                partialBlock = bitsAvailableForSqueezing;
                if ((long)partialBlock > outputLength - i)
                {
                    partialBlock = (int)(outputLength - i);
                }

                Array.Copy(dataQueue, (rate - bitsAvailableForSqueezing) / 8, output, offset + (int)(i / 8), partialBlock / 8);
                bitsAvailableForSqueezing -= partialBlock;
                i += partialBlock;
            }
        }

        private static void FromBytesToWords(ulong[] stateAsWords, byte[] state)
        {
            for (int i = 0; i < (1600 / 64); i++)
            {
                stateAsWords[i] = 0;
                int index = i * (64 / 8);
                for (int j = 0; j < (64 / 8); j++)
                {
                    stateAsWords[i] |= ((ulong)state[index + j] & 0xff) << ((8 * j));
                }
            }
        }

        private static void FromWordsToBytes(byte[] state, ulong[] stateAsWords)
        {
            for (int i = 0; i < (1600 / 64); i++)
            {
                int index = i * (64 / 8);
                for (int j = 0; j < (64 / 8); j++)
                {
                    state[index + j] = (byte)(stateAsWords[i] >> (8 * j));
                }
            }
        }

        private void KeccakPermutation(byte[] state)
        {
            ulong[] longState = new ulong[state.Length / 8];

            FromBytesToWords(longState, state);

            KeccakPermutationOnWords(longState);

            FromWordsToBytes(state, longState);
        }

        private void KeccakPermutationAfterXor(byte[] state, byte[] data, int dataLengthInBytes)
        {
            for (int i = 0; i < dataLengthInBytes; i++)
            {
                state[i] ^= data[i];
            }

            KeccakPermutation(state);
        }

        private void KeccakPermutationOnWords(ulong[] state)
        {
            int i;

            for (i = 0; i < 24; i++)
            {
                Theta(state);
                Rho(state);
                Pi(state);
                Chi(state);
                Iota(state, i);
            }
        }

        ulong[] C = new ulong[5];

        private void Theta(ulong[] A)
        {
            for (int x = 0; x < 5; x++)
            {
                C[x] = 0;
                for (int y = 0; y < 5; y++)
                {
                    C[x] ^= A[x + 5 * y];
                }
            }
            for (int x = 0; x < 5; x++)
            {
                ulong dX = ((((C[(x + 1) % 5]) << 1) ^ ((C[(x + 1) % 5]) >> (64 - 1)))) ^ C[(x + 4) % 5];
                for (int y = 0; y < 5; y++)
                {
                    A[x + 5 * y] ^= dX;
                }
            }
        }

        private void Rho(ulong[] A)
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    int index = x + 5 * y;
                    A[index] = ((KeccakRhoOffsets[index] != 0) ? (((A[index]) << KeccakRhoOffsets[index]) ^ ((A[index]) >> (64 - KeccakRhoOffsets[index]))) : A[index]);
                }
            }
        }

        ulong[] tempA = new ulong[25];

        private void Pi(ulong[] A)
        {
            Array.Copy(A, 0, tempA, 0, tempA.Length);

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    A[y + 5 * ((2 * x + 3 * y) % 5)] = tempA[x + 5 * y];
                }
            }
        }

        ulong[] chiC = new ulong[5];

        private void Chi(ulong[] A)
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    chiC[x] = A[x + 5 * y] ^ ((~A[(((x + 1) % 5) + 5 * y)]) & A[(((x + 2) % 5) + 5 * y)]);
                }
                for (int x = 0; x < 5; x++)
                {
                    A[x + 5 * y] = chiC[x];
                }
            }
        }

        private static void Iota(ulong[] A, int indexRound)
        {
            A[(((0) % 5) + 5 * ((0) % 5))] ^= KeccakRoundConstants[indexRound];
        }

        private void KeccakAbsorb(byte[] byteState, byte[] data, int dataInBytes)
        {
            KeccakPermutationAfterXor(byteState, data, dataInBytes);
        }

        private void KeccakExtract1024bits(byte[] byteState, byte[] data)
        {
            Array.Copy(byteState, 0, data, 0, 128);
        }

        private void KeccakExtract(byte[] byteState, byte[] data, int laneCount)
        {
            Array.Copy(byteState, 0, data, 0, laneCount * 8);
        }

        public virtual IMemoable Copy()
        {
            return new KeccakDigest(this);
        }

        public virtual void Reset(IMemoable other)
        {
            KeccakDigest d = (KeccakDigest)other;

            CopyIn(d);
        }
    }
}
