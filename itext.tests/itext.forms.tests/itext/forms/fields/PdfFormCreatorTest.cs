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
            PdfFormFactory customFactory = new _PdfFormFactory_21();
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

        private sealed class _PdfFormFactory_21 : PdfFormFactory {
            public _PdfFormFactory_21() {
            }

            public override PdfAcroForm GetAcroForm(PdfDocument document, bool createIfNotExist) {
                return base.GetAcroForm(document, false);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreateTextFormFieldTest() {
            PdfFormFactory customFactory = new _PdfFormFactory_41();
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

        private sealed class _PdfFormFactory_41 : PdfFormFactory {
            public _PdfFormFactory_41() {
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
