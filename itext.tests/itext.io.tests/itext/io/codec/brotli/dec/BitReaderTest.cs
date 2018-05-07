/* Copyright 2015 Google Inc. All Rights Reserved.

Distributed under MIT license.
See file LICENSE for detail or copy at https://opensource.org/licenses/MIT
*/

using iText.Test;

namespace iText.IO.Codec.Brotli.Dec
{
	/// <summary>
	/// Tests for
	/// <see cref="BitReader"/>
	/// .
	/// </summary>
	public class BitReaderTest : ExtendedITextTest
	{
		[NUnit.Framework.Test]
		public virtual void TestReadAfterEos()
		{
			iText.IO.Codec.Brotli.Dec.BitReader reader = new iText.IO.Codec.Brotli.Dec.BitReader();
			iText.IO.Codec.Brotli.Dec.BitReader.Init(reader, new System.IO.MemoryStream(new byte[1]));
			iText.IO.Codec.Brotli.Dec.BitReader.ReadBits(reader, 9);
			try
			{
				iText.IO.Codec.Brotli.Dec.BitReader.CheckHealth(reader, false);
			}
			catch (iText.IO.Codec.Brotli.Dec.BrotliRuntimeException)
			{
				// This exception is expected.
				return;
			}
			NUnit.Framework.Assert.Fail("BrotliRuntimeException should have been thrown by BitReader.checkHealth");
		}
	}
}
