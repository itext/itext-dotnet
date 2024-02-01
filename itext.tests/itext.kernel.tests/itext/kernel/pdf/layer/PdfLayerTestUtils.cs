/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Layer {
    internal class PdfLayerTestUtils {
        internal static void AddTextInsideLayer(IPdfOCG layer, PdfCanvas canvas, String text, float x, float y) {
            if (layer != null) {
                canvas.BeginLayer(layer);
            }
            canvas.BeginText().MoveText(x, y).ShowText(text).EndText();
            if (layer != null) {
                canvas.EndLayer();
            }
        }

        internal static PdfLayer PrepareNewLayer() {
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            return new PdfLayer("layer1", dummyDoc);
        }

        internal static PdfLayer PrepareLayerDesignIntent() {
            PdfLayer pdfLayer = PrepareNewLayer();
            pdfLayer.SetIntents(JavaCollectionsUtil.SingletonList(PdfName.Design));
            return pdfLayer;
        }

        internal static PdfLayer PrepareLayerDesignAndCustomIntent(PdfName custom) {
            PdfLayer pdfLayer = PrepareNewLayer();
            pdfLayer.SetIntents(JavaUtil.ArraysAsList(PdfName.Design, custom));
            return pdfLayer;
        }

        internal static void CompareLayers(String outPdf, String cmpPdf) {
            ITextTest.PrintOutCmpPdfNameAndDir(outPdf, cmpPdf);
            System.Console.Out.WriteLine();
            using (PdfDocument outDoc = new PdfDocument(CompareTool.CreateOutputReader(outPdf))) {
                using (PdfDocument cmpDoc = new PdfDocument(CompareTool.CreateOutputReader(cmpPdf))) {
                    PdfDictionary outOCP = outDoc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OCProperties);
                    PdfDictionary cmpOCP = cmpDoc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OCProperties);
                    NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(outOCP, cmpOCP));
                }
            }
        }
    }
}
