using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Kernel.Pdf.Tagutils {
    [NUnit.Framework.Category("Unit test")]
    public class AccessibilityPropertiesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetAccessibilityPropertiesTest() {
            AccessibilityProperties properties = new _AccessibilityProperties_21();
            NUnit.Framework.Assert.IsNotNull(properties.SetRole(StandardRoles.DIV));
            NUnit.Framework.Assert.IsNotNull(properties.SetLanguage("EN-GB"));
            NUnit.Framework.Assert.IsNotNull(properties.SetActualText("actualText"));
            NUnit.Framework.Assert.IsNotNull(properties.SetAlternateDescription("Description"));
            NUnit.Framework.Assert.IsNotNull(properties.SetExpansion("expansion"));
            NUnit.Framework.Assert.IsNotNull(properties.SetPhoneme("phoneme"));
            NUnit.Framework.Assert.IsNotNull(properties.SetPhoneticAlphabet("Phonetic Alphabet"));
            NUnit.Framework.Assert.IsNotNull(properties.SetNamespace(new PdfNamespace("Namespace")));
            NUnit.Framework.Assert.IsNotNull(properties.GetRefsList());
            NUnit.Framework.Assert.IsNotNull(properties.ClearRefs());
            NUnit.Framework.Assert.IsNotNull(properties.AddAttributes(new PdfStructureAttributes("attributes")));
            NUnit.Framework.Assert.IsNotNull(properties.AddAttributes(0, new PdfStructureAttributes("attributes")));
            NUnit.Framework.Assert.IsNotNull(properties.ClearAttributes());
            NUnit.Framework.Assert.IsNotNull(properties.GetAttributesList());
            NUnit.Framework.Assert.IsNotNull(properties.AddRef(new TagTreePointer(CreateTestDocument())));
        }

        private sealed class _AccessibilityProperties_21 : AccessibilityProperties {
            public _AccessibilityProperties_21() {
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetAccessibilityPropertiesTest() {
            AccessibilityProperties properties = new _AccessibilityProperties_42();
            NUnit.Framework.Assert.IsNull(properties.GetRole());
            NUnit.Framework.Assert.IsNull(properties.GetLanguage());
            NUnit.Framework.Assert.IsNull(properties.GetActualText());
            NUnit.Framework.Assert.IsNull(properties.GetAlternateDescription());
            NUnit.Framework.Assert.IsNull(properties.GetExpansion());
            NUnit.Framework.Assert.IsNull(properties.GetPhoneme());
            NUnit.Framework.Assert.IsNull(properties.GetPhoneticAlphabet());
            NUnit.Framework.Assert.IsNull(properties.GetNamespace());
        }

        private sealed class _AccessibilityProperties_42 : AccessibilityProperties {
            public _AccessibilityProperties_42() {
            }
        }

        private static PdfDocument CreateTestDocument() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.SetTagged();
            return pdfDoc;
        }
    }
}
