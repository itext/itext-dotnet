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
