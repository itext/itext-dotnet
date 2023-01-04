/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Commons.Actions.Contexts;
using iText.Forms.Exceptions;
using iText.IO.Source;
using iText.Kernel.Exceptions;
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
            PdfDictionary pdfDictionary = new PdfDictionary();
            PdfFormField pdfFormField = new PdfFormField(pdfDocument);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfFormField.GetRect(pdfDictionary
                ));
            NUnit.Framework.Assert.AreEqual(FormsExceptionMessageConstant.WRONG_FORM_FIELD_ADD_ANNOTATION_TO_THE_FIELD
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SetMetaInfoToCanvasMetaInfoUsedTest() {
            Canvas canvas = CreateCanvas();
            MetaInfoContainer metaInfoContainer = new MetaInfoContainer(new _IMetaInfo_62());
            FormsMetaInfoStaticContainer.UseMetaInfoDuringTheAction(metaInfoContainer, () => PdfFormField.SetMetaInfoToCanvas
                (canvas));
            NUnit.Framework.Assert.AreSame(metaInfoContainer, canvas.GetProperty<MetaInfoContainer>(Property.META_INFO
                ));
        }

        private sealed class _IMetaInfo_62 : IMetaInfo {
            public _IMetaInfo_62() {
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetMetaInfoToCanvasMetaInfoNotUsedTest() {
            Canvas canvas = CreateCanvas();
            PdfFormField.SetMetaInfoToCanvas(canvas);
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
