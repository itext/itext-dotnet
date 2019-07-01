using System;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Svg.Converter;

namespace iText.Svg.Utils {
    public class TestUtils {
        /// <exception cref="System.IO.IOException"/>
        public static void ConvertSVGtoPDF(String pdfFilePath, String svgFilePath, int PageNo, PageSize pageSize) {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(pdfFilePath, FileMode.Create), new 
                WriterProperties().SetCompressionLevel(0)));
            PageSize format = new PageSize(pageSize);
            pdfDocument.AddNewPage(format.Rotate());
            SvgConverter.DrawOnDocument(new FileStream(svgFilePath, FileMode.Open, FileAccess.Read), pdfDocument, PageNo
                );
            pdfDocument.Close();
        }
    }
}
