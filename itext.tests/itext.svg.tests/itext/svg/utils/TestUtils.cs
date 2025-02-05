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
using System;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Svg.Converter;

namespace iText.Svg.Utils {
    public class TestUtils {
        public static void ConvertSVGtoPDF(String pdfFilePath, String svgFilePath, int PageNo, PageSize pageSize) {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(FileUtil.GetFileOutputStream(pdfFilePath), new WriterProperties
                ().SetCompressionLevel(0)));
            PageSize format = new PageSize(pageSize);
            pdfDocument.AddNewPage(format.Rotate());
            SvgConverter.DrawOnDocument(FileUtil.GetInputStreamForFile(svgFilePath), pdfDocument, PageNo);
            pdfDocument.Close();
        }
    }
}
