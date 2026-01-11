/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CompressionStrategyTest : ExtendedITextTest {
        private static readonly byte[] TEST_CONTENT_STREAM_DATA = ("q\n" + "0 0 0 RG\n" + "10 w\n" + "1 j\n" + "2 J\n"
             + "100 100 m\n" + "300 100 l\n" + "200 300 l\n" + "s\n" + "1 0 0 RG\n" + "200 50 m\n" + "200 350 l\n"
             + "S\n" + "Q\n").GetBytes(System.Text.Encoding.ASCII);

        [NUnit.Framework.Test]
        public virtual void Ascii85DecodeTest() {
            DoStrategyTest(new ASCII85CompressionStrategy());
        }

        [NUnit.Framework.Test]
        public virtual void AsciiHexDecodeTest() {
            DoStrategyTest(new ASCIIHexCompressionStrategy());
        }

        [NUnit.Framework.Test]
        public virtual void FlateDecodeTest() {
            DoStrategyTest(new FlateCompressionStrategy());
        }

        [NUnit.Framework.Test]
        public virtual void RunLengthDecodeTest() {
            DoStrategyTest(new RunLengthCompressionStrategy());
        }

        private static void DoStrategyTest(IStreamCompressionStrategy strategy) {
            long writePlainTime = SystemUtil.CurrentTimeMillis();
            ByteArrayOutputStream plainPdfBytes = new ByteArrayOutputStream();
            WriteTestDocument(plainPdfBytes, null, CompressionConstants.NO_COMPRESSION);
            long plainSize = plainPdfBytes.Length;
            writePlainTime = SystemUtil.CurrentTimeMillis() - writePlainTime;
            System.Console.Out.WriteLine("Generated PDF size without compression: " + plainSize + " bytes in " + writePlainTime
                 + "ms");
            long writeCompressedTime = SystemUtil.CurrentTimeMillis();
            ByteArrayOutputStream compressedPdfBytes = new ByteArrayOutputStream();
            WriteTestDocument(compressedPdfBytes, strategy, CompressionConstants.DEFAULT_COMPRESSION);
            long compressedSize = compressedPdfBytes.Length;
            writeCompressedTime = SystemUtil.CurrentTimeMillis() - writeCompressedTime;
            System.Console.Out.WriteLine("Generated PDF with `" + strategy.GetFilterName() + "` compression: " + compressedSize
                 + " bytes in " + writeCompressedTime + "ms");
            System.Console.Out.WriteLine("Compression ratio: " + ((double)compressedSize / plainSize));
            PdfDocument plainDoc = new PdfDocument(new PdfReader(new MemoryStream(compressedPdfBytes.ToArray())));
            PdfDocument compressedDoc = new PdfDocument(new PdfReader(new MemoryStream(plainPdfBytes.ToArray())));
            int numberOfPdfObjects = plainDoc.GetNumberOfPdfObjects();
            NUnit.Framework.Assert.AreEqual(numberOfPdfObjects, compressedDoc.GetNumberOfPdfObjects(), "Number of PDF objects should be the same in both documents"
                );
            for (int objNum = 1; objNum <= numberOfPdfObjects; ++objNum) {
                PdfObject plainObj = plainDoc.GetPdfObject(objNum);
                PdfObject compressedObj = compressedDoc.GetPdfObject(objNum);
                NUnit.Framework.Assert.AreEqual(GetType(plainObj), GetType(compressedObj), "PDF object type should be identical for object number "
                     + objNum);
                if ((plainObj is PdfStream) && (compressedObj is PdfStream)) {
                    byte[] plainStreamBytes = ((PdfStream)plainObj).GetBytes();
                    byte[] compressedStreamBytes = ((PdfStream)compressedObj).GetBytes();
                    NUnit.Framework.Assert.AreEqual(plainStreamBytes, compressedStreamBytes, "PDF stream bytes should be identical for object number "
                         + objNum);
                }
            }
        }

        private static void WriteTestDocument(ByteArrayOutputStream os, IStreamCompressionStrategy strategy, int compressionLevel
            ) {
            DocumentProperties docProps = new DocumentProperties();
            if (strategy != null) {
                docProps.RegisterDependency(typeof(IStreamCompressionStrategy), strategy);
            }
            WriterProperties writerProps = new WriterProperties().SetCompressionLevel(compressionLevel);
            using (PdfDocument doc = new PdfDocument(new PdfWriter(os, writerProps), docProps)) {
                PdfPage page = doc.AddNewPage();
                page.GetFirstContentStream().SetData(TEST_CONTENT_STREAM_DATA);
            }
        }

        private static byte GetType(PdfObject obj) {
            if (obj == null) {
                return -1;
            }
            return obj.GetObjectType();
        }
    }
}
