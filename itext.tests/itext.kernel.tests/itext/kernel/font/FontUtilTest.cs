using iText.IO.Font.Cmap;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class FontUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseUniversalNotExistedCMapTest() {
            NUnit.Framework.Assert.IsNull(FontUtil.ParseUniversalToUnicodeCMap("NotExisted"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.UNKNOWN_ERROR_WHILE_PROCESSING_CMAP, LogLevel = LogLevelConstants
            .ERROR)]
        public virtual void ProcessInvalidToUnicodeTest() {
            PdfStream toUnicode = new PdfStream();
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                toUnicode.MakeIndirect(pdfDocument);
                toUnicode.Flush();
                CMapToUnicode cmap = FontUtil.ProcessToUnicode(toUnicode);
                NUnit.Framework.Assert.IsNotNull(cmap);
                NUnit.Framework.Assert.IsFalse(cmap.HasByteMappings());
            }
        }
    }
}
