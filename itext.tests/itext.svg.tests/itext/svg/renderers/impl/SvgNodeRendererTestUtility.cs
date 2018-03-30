using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Svg.Converter;

namespace iText.Svg.Renderers.Impl {
    public class SvgNodeRendererTestUtility {
        /// <exception cref="System.IO.IOException"/>
        public static void Convert(Stream svg, Stream pdfOutputStream) {
            PdfDocument doc = new PdfDocument(new PdfWriter(pdfOutputStream, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            SvgConverter.DrawOnDocument(svg, doc, 1);
            doc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        public static void ConvertAndCompare(String src, String dest, String fileName) {
            Convert(new FileStream(src + fileName + ".svg", FileMode.Open, FileAccess.Read), new FileStream(dest + fileName
                 + ".pdf", FileMode.Create));
            CompareTool compareTool = new CompareTool();
            String compareResult = compareTool.CompareByContent(dest + fileName + ".pdf", src + "cmp_" + fileName + ".pdf"
                , dest, "diff_");
            NUnit.Framework.Assert.IsNull(compareResult);
        }
    }
}
