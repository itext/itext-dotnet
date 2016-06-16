using System;

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <remarks>
    /// A queue for bytes.
    /// <p>
    /// This file could be more optimized.
    /// </p>
    /// </remarks>
    public class ByteQueue
    {
        /// <returns>The smallest number which can be written as 2^x which is bigger than i.</returns>
        public static int NextTwoPow(
            int i)
        {
            /*
            * This code is based of a lot of code I found on the Internet
            * which mostly referenced a book called "Hacking delight".
            *
            */
            i |= (i >> 1);
            i |= (i >> 2);
            i |= (i >> 4);
            i |= (i >> 8);
            i |= (i >> 16);
            return i + 1;
        }

        /**
         * The initial size for our buffer.
         */
        private const int DefaultCapacity = 1024;

        /**
         * The buffer where we store our data.
         */
        private byte[] databuf;

        /**
         * How many bytes at the beginning of the buffer are skipped.
         */
        private int skipped = 0;

        /**
         * How many bytes in the buffer are valid data.
         */
        private int available = 0;

        public ByteQueue()
            : this(DefaultCapacity)
        {
        }

        public ByteQueue(int capacity)
        {
            this.databuf = new byte[capacity];
        }

        /// <summary>Read data from the buffer.</summary>
        /// <param name="buf">The buffer where the read data will be copied to.</param>
        /// <param name="offset">How many bytes to skip at the beginning of buf.</param>
        /// <param name="len">How many bytes to read at all.</param>
        /// <param name="skip">How many bytes from our data to skip.</param>
        public void Read(
            byte[]	buf,
            int		offset,
            int		len,
            int		skip)
        {
            if ((buf.Length - offset) < len)
            {
                throw new ArgumentException("Buffer size of " + buf.Length + " is too small for a read of " + len + " bytes");
            }
            if ((available - skip) < len)
            {
                throw new InvalidOperationException("Not enough data to read");
            }
            Array.Copy(databuf, skipped + skip, buf, offset, len);
        }

        /// <summary>Add some data to our buffer.</summary>
        /// <param name="data">A byte-array to read data from.</param>
        /// <param name="offset">How many bytes to skip at the beginning of the array.</param>
        /// <param name="len">How many bytes to read from the array.</param>
        public void AddData(
            byte[]	data,
            int		offset,
            int		len)
        {
            if ((skipped + available + len) > databuf.Length)
            {
                int desiredSize = ByteQueue.NextTwoPow(available + len);
                if (desiredSize > databuf.Length)
                {
                    byte[] tmp = new byte[desiredSize];
                    Array.Copy(databuf, skipped, tmp, 0, available);
                    databuf = tmp;
                }
                else
                {
                    Array.Copy(databuf, skipped, databuf, 0, available);
                }
                skipped = 0;
            }

            Array.Copy(data, offset, databuf, skipped + available, len);
            available += len;
        }

        /// <summary>Remove some bytes from our data from the beginning.</summary>
        /// <param name="i">How many bytes to remove.</param>
        public void RemoveData(
            int i)
        {
            if (i > available)
            {
                throw new InvalidOperationException("Cannot remove " + i + " bytes, only got " + available);
            }

            /*
            * Skip the data.
            */
            available -= i;
            skipped += i;
        }

        public void RemoveData(byte[] buf, int off, int len, int skip)
        {
            Read(buf, off, len, skip);
            RemoveData(skip + len);
        }

        public byte[] RemoveData(int len, int skip)
        {
            byte[] buf = new byte[len];
            RemoveData(buf, 0, len, skip);
            return buf;
        }

        /// <summary>The number of bytes which are available in this buffer.</summary>
        public int Available
        {
            get { return available; }
        }
    }
}
