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
using System.Collections.Generic;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Test;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TaggedSigningFieldTest : ExtendedITextTest {
        // TODO DEVSIX-5438: Change assertions after implementing signature field tagging
        [NUnit.Framework.Test]
        public virtual void CheckSigningFieldTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                pdfDoc.SetTagged();
                Rectangle rect = new Rectangle(36, 648, 200, 100);
                PdfFormField signField = new SignatureFormFieldBuilder(pdfDoc, "signature").SetWidgetRectangle(rect).CreateSignature
                    ();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                form.AddField(signField);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                NUnit.Framework.Assert.IsNotNull(tagPointer.MoveToKid(StandardRoles.FORM));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckSigningFieldZeroSizeRectangleTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                pdfDoc.SetTagged();
                Rectangle rect = new Rectangle(36, 648, 0, 0);
                PdfFormField signField = new SignatureFormFieldBuilder(pdfDoc, "signature").SetWidgetRectangle(rect).CreateSignature
                    ();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                form.AddField(signField);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                NUnit.Framework.Assert.IsNotNull(tagPointer.MoveToKid(StandardRoles.FORM));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckSigningFieldPrintFlagTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfPage page = pdfDoc.AddNewPage();
                pdfDoc.SetTagged();
                Rectangle rect = new Rectangle(36, 648, 200, 100);
                PdfFormField signField = new SignatureFormFieldBuilder(pdfDoc, "signature").SetWidgetRectangle(rect).CreateSignature
                    ();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                form.AddField(signField);
                IList<PdfAnnotation> annotations = page.GetAnnotations();
                annotations[0].SetFlag(PdfAnnotation.PRINT);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                NUnit.Framework.Assert.IsNotNull(tagPointer.MoveToKid(StandardRoles.FORM));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckSigningFieldHiddenFlagTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfPage page = pdfDoc.AddNewPage();
                pdfDoc.SetTagged();
                Rectangle rect = new Rectangle(36, 648, 200, 100);
                PdfFormField signField = new SignatureFormFieldBuilder(pdfDoc, "signature").SetWidgetRectangle(rect).CreateSignature
                    ();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                form.AddField(signField);
                IList<PdfAnnotation> annotations = page.GetAnnotations();
                annotations[0].SetFlag(PdfAnnotation.HIDDEN);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                NUnit.Framework.Assert.IsNotNull(tagPointer.MoveToKid(StandardRoles.FORM));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckSigningFieldNoViewFlagTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfPage page = pdfDoc.AddNewPage();
                pdfDoc.SetTagged();
                Rectangle rect = new Rectangle(36, 648, 200, 100);
                PdfFormField signField = new SignatureFormFieldBuilder(pdfDoc, "signature").SetWidgetRectangle(rect).CreateSignature
                    ();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                form.AddField(signField);
                IList<PdfAnnotation> annotations = page.GetAnnotations();
                annotations[0].SetFlag(PdfAnnotation.NO_VIEW);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                NUnit.Framework.Assert.IsNotNull(tagPointer.MoveToKid(StandardRoles.FORM));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckSigningFieldInvisibleFlagTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfPage page = pdfDoc.AddNewPage();
                pdfDoc.SetTagged();
                Rectangle rect = new Rectangle(36, 648, 200, 100);
                PdfFormField signField = new SignatureFormFieldBuilder(pdfDoc, "signature").SetWidgetRectangle(rect).CreateSignature
                    ();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                form.AddField(signField);
                IList<PdfAnnotation> annotations = page.GetAnnotations();
                annotations[0].SetFlag(PdfAnnotation.INVISIBLE);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                NUnit.Framework.Assert.IsNotNull(tagPointer.MoveToKid(StandardRoles.FORM));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckSigningFieldOutsidePageTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                pdfDoc.SetTagged();
                Rectangle rect = new Rectangle(-150, -150, 100, 100);
                PdfFormField signField = new SignatureFormFieldBuilder(pdfDoc, "signature").SetWidgetRectangle(rect).CreateSignature
                    ();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                form.AddField(signField);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                NUnit.Framework.Assert.IsNotNull(tagPointer.MoveToKid(StandardRoles.FORM));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckSigningFieldOutsidePageAndHiddenTest() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfPage page = pdfDoc.AddNewPage();
                pdfDoc.SetTagged();
                Rectangle rect = new Rectangle(-150, -150, 200, 100);
                PdfFormField signField = new SignatureFormFieldBuilder(pdfDoc, "signature").SetWidgetRectangle(rect).CreateSignature
                    ();
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                form.AddField(signField);
                IList<PdfAnnotation> annotations = page.GetAnnotations();
                annotations[0].SetFlag(PdfAnnotation.HIDDEN);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                NUnit.Framework.Assert.IsNotNull(tagPointer.MoveToKid(StandardRoles.FORM));
            }
        }
    }
}
