using iText.IO.Util;
using iText.Kernel;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfStreamDecodeTest : ExtendedITextTest {
        private static readonly byte[] BYTES = new byte[] { (byte)0x78, (byte)0xda, (byte)0x01, (byte)0x28, (byte)
            0x00, (byte)0xd7, (byte)0xff, (byte)0x78, (byte)0xda, (byte)0xab, (byte)0xb8, (byte)0xf5, (byte)0xf6, 
            (byte)0x60, (byte)0x23, (byte)0x03, (byte)0x10, (byte)0x1c, (byte)0x56, (byte)0x58, (byte)0xf1, (byte)
            0x73, (byte)0xb7, (byte)0xec, (byte)0x93, (byte)0x50, (byte)0x46, (byte)0x86, (byte)0x51, (byte)0x30, 
            (byte)0x0a, (byte)0x46, (byte)0xc1, (byte)0x90, (byte)0x07, (byte)0xeb, (byte)0xd9, (byte)0x96, (byte)
            0x87, (byte)0x26, (byte)0x84, (byte)0x03, (byte)0x00, (byte)0x27, (byte)0xef, (byte)0x0a, (byte)0x80, 
            (byte)0x91, (byte)0x9d, (byte)0x12, (byte)0x0e };

        private static readonly byte[] FLATE_DECODED_BYTES = new byte[] { (byte)0x78, (byte)0x9c, (byte)0x01, (byte
            )0x33, (byte)0x00, (byte)0xcc, (byte)0xff, (byte)0x78, (byte)0xda, (byte)0x01, (byte)0x28, (byte)0x00, 
            (byte)0xd7, (byte)0xff, (byte)0x78, (byte)0xda, (byte)0xab, (byte)0xb8, (byte)0xf5, (byte)0xf6, (byte)
            0x60, (byte)0x23, (byte)0x03, (byte)0x10, (byte)0x1c, (byte)0x56, (byte)0x58, (byte)0xf1, (byte)0x73, 
            (byte)0xb7, (byte)0xec, (byte)0x93, (byte)0x50, (byte)0x46, (byte)0x86, (byte)0x51, (byte)0x30, (byte)
            0x0a, (byte)0x46, (byte)0xc1, (byte)0x90, (byte)0x07, (byte)0xeb, (byte)0xd9, (byte)0x96, (byte)0x87, 
            (byte)0x26, (byte)0x84, (byte)0x03, (byte)0x00, (byte)0x27, (byte)0xef, (byte)0x0a, (byte)0x80, (byte)
            0x91, (byte)0x9d, (byte)0x12, (byte)0x0e, (byte)0x7b, (byte)0xda, (byte)0x16, (byte)0xad };

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.DCTDECODE_FILTER_DECODING, LogLevel = LogLevelConstants.INFO)]
        public virtual void TestDCTDecodeFilter() {
            PdfStream pdfStream = new PdfStream(FLATE_DECODED_BYTES);
            pdfStream.Put(PdfName.Filter, new PdfArray(JavaUtil.ArraysAsList((PdfObject)PdfName.FlateDecode, (PdfObject
                )PdfName.DCTDecode)));
            NUnit.Framework.Assert.AreEqual(BYTES, pdfStream.GetBytes());
        }

        [NUnit.Framework.Test]
        public virtual void TestJBIG2DecodeFilter() {
            PdfStream pdfStream = new PdfStream(FLATE_DECODED_BYTES);
            pdfStream.Put(PdfName.Filter, new PdfArray(JavaUtil.ArraysAsList((PdfObject)PdfName.FlateDecode, (PdfObject
                )PdfName.JBIG2Decode)));
            NUnit.Framework.Assert.That(() =>  {
                pdfStream.GetBytes(true);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(MessageFormatUtil.Format(PdfException.Filter1IsNotSupported, PdfName.JBIG2Decode)))
;
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.JPXDECODE_FILTER_DECODING, LogLevel = LogLevelConstants.INFO)]
        public virtual void TestJPXDecodeFilter() {
            PdfStream pdfStream = new PdfStream(FLATE_DECODED_BYTES);
            pdfStream.Put(PdfName.Filter, new PdfArray(JavaUtil.ArraysAsList((PdfObject)PdfName.FlateDecode, (PdfObject
                )PdfName.JPXDecode)));
            NUnit.Framework.Assert.AreEqual(BYTES, pdfStream.GetBytes());
        }
    }
}
