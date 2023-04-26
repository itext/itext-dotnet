/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Actions.Contexts;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfFormFieldUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CannotGetRectangleIfKidsIsNullTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfFormField pdfFormField = new PdfFormField(pdfDocument);
            NUnit.Framework.Assert.IsNull(pdfFormField.GetFirstFormAnnotation());
        }

        [NUnit.Framework.Test]
        public virtual void SetMetaInfoToCanvasMetaInfoUsedTest() {
            Canvas canvas = CreateCanvas();
            MetaInfoContainer metaInfoContainer = new MetaInfoContainer(new _IMetaInfo_56());
            FormsMetaInfoStaticContainer.UseMetaInfoDuringTheAction(metaInfoContainer, () => PdfFormAnnotation.SetMetaInfoToCanvas
                (canvas));
            NUnit.Framework.Assert.AreSame(metaInfoContainer, canvas.GetProperty<MetaInfoContainer>(Property.META_INFO
                ));
        }

        private sealed class _IMetaInfo_56 : IMetaInfo {
            public _IMetaInfo_56() {
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetMetaInfoToCanvasMetaInfoNotUsedTest() {
            Canvas canvas = CreateCanvas();
            PdfFormAnnotation.SetMetaInfoToCanvas(canvas);
            NUnit.Framework.Assert.IsNull(canvas.GetProperty<MetaInfoContainer>(Property.META_INFO));
        }

        private static Canvas CreateCanvas() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfStream stream = (PdfStream)new PdfStream().MakeIndirect(document);
                PdfResources resources = new PdfResources();
                PdfCanvas pdfCanvas = new PdfCanvas(stream, resources, document);
                return new iText.Layout.Canvas(pdfCanvas, new Rectangle(100, 100));
            }
        }
    }
}
