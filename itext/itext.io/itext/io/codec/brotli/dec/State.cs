/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/
namespace iText.IO.Codec.Brotli.Dec
{
	internal sealed class State
	{
		internal int runningState = iText.IO.Codec.Brotli.Dec.RunningState.Uninitialized;

		internal int nextRunningState;

		internal readonly iText.IO.Codec.Brotli.Dec.BitReader br = new iText.IO.Codec.Brotli.Dec.BitReader();

		internal byte[] ringBuffer;

		internal readonly int[] blockTypeTrees = new int[3 * iText.IO.Codec.Brotli.Dec.Huffman.HuffmanMaxTableSize];

		internal readonly int[] blockLenTrees = new int[3 * iText.IO.Codec.Brotli.Dec.Huffman.HuffmanMaxTableSize];

		internal int metaBlockLength;

		internal bool inputEnd;

		internal bool isUncompressed;

		internal bool isMetadata;

		internal readonly iText.IO.Codec.Brotli.Dec.HuffmanTreeGroup hGroup0 = new iText.IO.Codec.Brotli.Dec.HuffmanTreeGroup();

		internal readonly iText.IO.Codec.Brotli.Dec.HuffmanTreeGroup hGroup1 = new iText.IO.Codec.Brotli.Dec.HuffmanTreeGroup();

		internal readonly iText.IO.Codec.Brotli.Dec.HuffmanTreeGroup hGroup2 = new iText.IO.Codec.Brotli.Dec.HuffmanTreeGroup();

		internal readonly int[] blockLength = new int[3];

		internal readonly int[] numBlockTypes = new int[3];

		internal readonly int[] blockTypeRb = new int[6];

		internal readonly int[] distRb = new int[] { 16, 15, 11, 4 };

		internal int pos = 0;

		internal int maxDistance = 0;

		internal int distRbIdx = 0;

		internal bool trivialLiteralContext = false;

		internal int literalTreeIndex = 0;

		internal int literalTree;

		internal int j;

		internal int insertLength;

		internal byte[] contextModes;

		internal byte[] contextMap;

		internal int contextMapSlice;

		internal int distContextMapSlice;

		internal int contextLookupOffset1;

		internal int contextLookupOffset2;

		internal int treeCommandOffset;

		internal int distanceCode;

		internal byte[] distContextMap;

		internal int numDirectDistanceCodes;

		internal int distancePostfixMask;

		internal int distancePostfixBits;

		internal int distance;

		internal int copyLength;

		internal int copyDst;

		internal int maxBackwardDistance;

		internal int maxRingBufferSize;

		internal int ringBufferSize = 0;

		internal long expectedTotalSize = 0;

		internal byte[] customDictionary = new byte[0];

		internal int bytesToIgnore = 0;

		internal int outputOffset;

		internal int outputLength;

		internal int outputUsed;

		internal int bytesWritten;

		internal int bytesToWrite;

		internal byte[] output;

		// Current meta-block header information.
		// TODO: Update to current spec.
		private static int DecodeWindowBits(iText.IO.Codec.Brotli.Dec.BitReader br)
		{
			if (iText.IO.Codec.Brotli.Dec.BitReader.ReadBits(br, 1) == 0)
			{
				return 16;
			}
			int n = iText.IO.Codec.Brotli.Dec.BitReader.ReadBits(br, 3);
			if (n != 0)
			{
				return 17 + n;
			}
			n = iText.IO.Codec.Brotli.Dec.BitReader.ReadBits(br, 3);
			if (n != 0)
			{
				return 8 + n;
			}
			return 17;
		}

		/// <summary>Associate input with decoder state.</summary>
		/// <param name="state">uninitialized state without associated input</param>
		/// <param name="input">compressed data source</param>
		internal static void SetInput(iText.IO.Codec.Brotli.Dec.State state, System.IO.Stream input)
		{
			if (state.runningState != iText.IO.Codec.Brotli.Dec.RunningState.Uninitialized)
			{
				throw new System.InvalidOperationException("State MUST be uninitialized");
			}
			iText.IO.Codec.Brotli.Dec.BitReader.Init(state.br, input);
			int windowBits = DecodeWindowBits(state.br);
			if (windowBits == 9)
			{
				/* Reserved case for future expansion. */
				throw new iText.IO.Codec.Brotli.Dec.BrotliRuntimeException("Invalid 'windowBits' code");
			}
			state.maxRingBufferSize = 1 << windowBits;
			state.maxBackwardDistance = state.maxRingBufferSize - 16;
			state.runningState = iText.IO.Codec.Brotli.Dec.RunningState.BlockStart;
		}

		internal static void Close(iText.IO.Codec.Brotli.Dec.State state)
		{
			if (state.runningState == iText.IO.Codec.Brotli.Dec.RunningState.Uninitialized)
			{
				throw new System.InvalidOperationException("State MUST be initialized");
			}
			if (state.runningState == iText.IO.Codec.Brotli.Dec.RunningState.Closed)
			{
				return;
			}
			state.runningState = iText.IO.Codec.Brotli.Dec.RunningState.Closed;
			iText.IO.Codec.Brotli.Dec.BitReader.Close(state.br);
		}
	}
}
