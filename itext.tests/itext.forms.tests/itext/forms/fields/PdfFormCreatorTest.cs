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
using iText.Forms;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfFormCreatorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetAcroFormTest() {
            PdfFormFactory customFactory = new _PdfFormFactory_43();
            // Never create new acroform.
            PdfFormCreator.SetFactory(customFactory);
            try {
                using (PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                    PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(document, true);
                    NUnit.Framework.Assert.IsNull(acroForm);
                }
            }
            finally {
                PdfFormCreator.SetFactory(new PdfFormFactory());
            }
        }

        private sealed class _PdfFormFactory_43 : PdfFormFactory {
            public _PdfFormFactory_43() {
            }

            public override PdfAcroForm GetAcroForm(PdfDocument document, bool createIfNotExist) {
                return base.GetAcroForm(document, false);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreateTextFormFieldTest() {
            PdfFormFactory customFactory = new _PdfFormFactory_63();
            // All text is read by default.
            PdfFormCreator.SetFactory(customFactory);
            try {
                using (PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                    PdfFormField text = new TextFormFieldBuilder(document, "name").SetWidgetRectangle(new Rectangle(100, 100))
                        .CreateText();
                    NUnit.Framework.Assert.AreEqual(ColorConstants.RED, text.GetColor());
                }
            }
            finally {
                PdfFormCreator.SetFactory(new PdfFormFactory());
            }
        }

        private sealed class _PdfFormFactory_63 : PdfFormFactory {
            public _PdfFormFactory_63() {
            }

            public override PdfTextFormField CreateTextFormField(PdfWidgetAnnotation widgetAnnotation, PdfDocument document
                ) {
                PdfTextFormField formField = base.CreateTextFormField(widgetAnnotation, document);
                formField.SetColor(ColorConstants.RED);
                return formField;
            }
        }
    }
}
