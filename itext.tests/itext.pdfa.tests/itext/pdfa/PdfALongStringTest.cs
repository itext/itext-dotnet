using System;
using System.IO;
using System.Text;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;

namespace iText.Pdfa {
    public class PdfALongStringTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfALongStringTest/";

        private const String LOREM_IPSUM = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis condimentum, tortor sit amet fermentum pharetra, sem felis finibus enim, vel consectetur nunc justo at nisi. In hac habitasse platea dictumst. Donec quis suscipit eros. Nam urna purus, scelerisque in placerat in, convallis vel sapien. Suspendisse sed lacus sit amet orci ornare vulputate. In hac habitasse platea dictumst. Ut eu aliquet felis, at consectetur neque.";

        private const int STRING_LENGTH_LIMIT = 32767;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void RunTest() {
            //TODO(DEVSIX-2978): Produces non-conforming PDF/A document
            String file = "pdfALongString.pdf";
            String filename = destinationFolder + file;
            using (Stream icm = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read
                )) {
                using (PdfADocument pdf = new PdfADocument(new PdfWriter(new FileStream(filename, FileMode.Create)), PdfAConformanceLevel
                    .PDF_A_3U, new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB ICC preference", icm))) {
                    using (Document document = new Document(pdf)) {
                        StringBuilder stringBuilder = new StringBuilder(LOREM_IPSUM);
                        while (stringBuilder.Length < STRING_LENGTH_LIMIT) {
                            stringBuilder.Append(stringBuilder.ToString());
                        }
                        PdfFontFactory.Register(sourceFolder + "FreeSans.ttf", sourceFolder + "FreeSans.ttf");
                        PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", true);
                        Paragraph p = new Paragraph(stringBuilder.ToString());
                        p.SetMinWidth(1e6f);
                        p.SetFont(font);
                        document.Add(p);
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_" + file, destinationFolder
                , "diff_"));
        }
    }
}
