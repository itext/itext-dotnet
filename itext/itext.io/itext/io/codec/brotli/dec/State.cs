/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
using System.IO;

namespace iText.IO.Codec.Brotli.Dec {
//\cond DO_NOT_DOCUMENT
    internal sealed class State {
//\cond DO_NOT_DOCUMENT
        internal byte[] ringBuffer;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal byte[] contextModes;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal byte[] contextMap;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal byte[] distContextMap;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal byte[] distExtraBits;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal byte[] output;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal byte[] byteBuffer;
//\endcond

//\cond DO_NOT_DOCUMENT
        // BitReader
        internal short[] shortBuffer;
//\endcond

//\cond DO_NOT_DOCUMENT
        // BitReader
        internal int[] intBuffer;
//\endcond

//\cond DO_NOT_DOCUMENT
        // BitReader
        internal int[] rings;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int[] blockTrees;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int[] literalTreeGroup;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int[] commandTreeGroup;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int[] distanceTreeGroup;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int[] distOffset;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal long accumulator64;
//\endcond

//\cond DO_NOT_DOCUMENT
        // BitReader: pre-fetched bits.
        internal int runningState;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Default value is 0 == Decode.UNINITIALIZED
        internal int nextRunningState;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int accumulator32;
//\endcond

//\cond DO_NOT_DOCUMENT
        // BitReader: pre-fetched bits.
        internal int bitOffset;
//\endcond

//\cond DO_NOT_DOCUMENT
        // BitReader: bit-reading position in accumulator.
        internal int halfOffset;
//\endcond

//\cond DO_NOT_DOCUMENT
        // BitReader: offset of next item in intBuffer/shortBuffer.
        internal int tailBytes;
//\endcond

//\cond DO_NOT_DOCUMENT
        // BitReader: number of bytes in unfinished half.
        internal int endOfStreamReached;
//\endcond

//\cond DO_NOT_DOCUMENT
        // BitReader: input stream is finished.
        internal int metaBlockLength;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int inputEnd;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int isUncompressed;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int isMetadata;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int literalBlockLength;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int numLiteralBlockTypes;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int commandBlockLength;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int numCommandBlockTypes;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int distanceBlockLength;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int numDistanceBlockTypes;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int pos;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int maxDistance;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int distRbIdx;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int trivialLiteralContext;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int literalTreeIdx;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int commandTreeIdx;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int j;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int insertLength;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int contextMapSlice;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int distContextMapSlice;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int contextLookupOffset1;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int contextLookupOffset2;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int distanceCode;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int numDirectDistanceCodes;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int distancePostfixBits;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int distance;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int copyLength;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int maxBackwardDistance;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int maxRingBufferSize;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int ringBufferSize;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int expectedTotalSize;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int outputOffset;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int outputLength;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int outputUsed;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int ringBufferBytesWritten;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int ringBufferBytesReady;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int isEager;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int isLargeWindow;
//\endcond

//\cond DO_NOT_DOCUMENT
        // Compound dictionary
        internal int cdNumChunks;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int cdTotalSize;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int cdBrIndex;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int cdBrOffset;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int cdBrLength;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int cdBrCopied;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal byte[][] cdChunks;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int[] cdChunkOffsets;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal int cdBlockBits;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal byte[] cdBlockMap;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal Stream input = Utils.MakeEmptyInput();
//\endcond

//\cond DO_NOT_DOCUMENT
        // BitReader
        internal State() {
            this.ringBuffer = new byte[0];
            this.rings = new int[10];
            this.rings[0] = 16;
            this.rings[1] = 15;
            this.rings[2] = 11;
            this.rings[3] = 4;
        }
//\endcond
    }
//\endcond
}
