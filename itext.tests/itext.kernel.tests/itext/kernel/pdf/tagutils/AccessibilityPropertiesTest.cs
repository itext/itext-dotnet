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
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Kernel.Pdf.Tagutils {
    [NUnit.Framework.Category("UnitTest")]
    public class AccessibilityPropertiesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetAccessibilityPropertiesTest() {
            AccessibilityProperties properties = new _AccessibilityProperties_43();
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

        private sealed class _AccessibilityProperties_43 : AccessibilityProperties {
            public _AccessibilityProperties_43() {
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetAccessibilityPropertiesTest() {
            AccessibilityProperties properties = new _AccessibilityProperties_64();
            NUnit.Framework.Assert.IsNull(properties.GetRole());
            NUnit.Framework.Assert.IsNull(properties.GetLanguage());
            NUnit.Framework.Assert.IsNull(properties.GetActualText());
            NUnit.Framework.Assert.IsNull(properties.GetAlternateDescription());
            NUnit.Framework.Assert.IsNull(properties.GetExpansion());
            NUnit.Framework.Assert.IsNull(properties.GetPhoneme());
            NUnit.Framework.Assert.IsNull(properties.GetPhoneticAlphabet());
            NUnit.Framework.Assert.IsNull(properties.GetNamespace());
        }

        private sealed class _AccessibilityProperties_64 : AccessibilityProperties {
            public _AccessibilityProperties_64() {
            }
        }

        private static PdfDocument CreateTestDocument() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.SetTagged();
            return pdfDoc;
        }
    }
}
