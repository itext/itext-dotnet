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
using System.Collections.Generic;
using iText.Forms.Form;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Tagging;
using iText.Test;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class AccessibleElementTest : ExtendedITextTest {
        public static ICollection<Object[]> GetDataTestFixtureData() {
            int amountOfEntries = 8;
            IList<Object[]> data = new List<Object[]>();
            for (int i = 0; i < amountOfEntries; i++) {
                data.Add(new Object[] { new AccessibleElementTest.TestContainer(i) });
            }
            return data;
        }

        private Func<IFormField> GetDataToTest(int index) {
            switch (index) {
                case 0: {
                    return () => new InputField("inputField");
                }

                case 1: {
                    return () => new TextArea("textArea");
                }

                case 2: {
                    return () => new Radio("radioButton", "group");
                }

                case 3: {
                    ComboBoxField field = new ComboBoxField("comboBox");
                    field.AddOption(new SelectFieldItem("option1"));
                    field.AddOption(new SelectFieldItem("option2"));
                    return () => field;
                }

                case 4: {
                    ListBoxField field2 = new ListBoxField("listBox", 4, false);
                    field2.AddOption(new SelectFieldItem("option1"));
                    field2.AddOption(new SelectFieldItem("option2"));
                    return () => field2;
                }

                case 5: {
                    return () => new SignatureFieldAppearance("signatureField");
                }

                case 6: {
                    return () => new Button("button");
                }

                case 7: {
                    return () => new CheckBox("checkBox");
                }

                default: {
                    throw new ArgumentException("Invalid index");
                }
            }
        }

        [NUnit.Framework.TestCaseSource("GetDataTestFixtureData")]
        public virtual void TestInteractive(AccessibleElementTest.TestContainer testContainer) {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            IFormField element = GetDataToTest(testContainer.index)();
            element.SetInteractive(true);
            IAccessibleElement accessibleElement = (IAccessibleElement)element;
            accessibleElement.GetAccessibilityProperties().SetLanguage("en");
            document.Add((IBlockElement)element);
            PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
            IStructureNode documentStruct = root.GetKids()[0];
            IStructureNode kid = documentStruct.GetKids()[0];
            PdfStructElem elem = (PdfStructElem)kid;
            PdfDictionary obj = elem.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.Form, elem.GetRole());
            NUnit.Framework.Assert.IsTrue(obj.ContainsKey(PdfName.Lang));
            NUnit.Framework.Assert.AreEqual("en", obj.GetAsString(PdfName.Lang).GetValue());
            document.Close();
            pdfDocument.Close();
        }

        [NUnit.Framework.TestCaseSource("GetDataTestFixtureData")]
        public virtual void TestNonInteractive(AccessibleElementTest.TestContainer testContainer) {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            IFormField element = GetDataToTest(testContainer.index)();
            IAccessibleElement accessibleElement = (IAccessibleElement)element;
            accessibleElement.GetAccessibilityProperties().SetLanguage("en");
            document.Add((IBlockElement)element);
            PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
            IStructureNode documentStruct = root.GetKids()[0];
            IStructureNode kid = documentStruct.GetKids()[0];
            PdfStructElem elem = (PdfStructElem)kid;
            PdfDictionary obj = elem.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.Form, elem.GetRole());
            NUnit.Framework.Assert.IsTrue(obj.ContainsKey(PdfName.Lang));
            NUnit.Framework.Assert.AreEqual("en", obj.GetAsString(PdfName.Lang).GetValue());
            document.Close();
            pdfDocument.Close();
        }

        [NUnit.Framework.TestCaseSource("GetDataTestFixtureData")]
        public virtual void TestInteractiveProperty(AccessibleElementTest.TestContainer testContainer) {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos));
            pdfDocument.SetTagged();
            Document document = new Document(pdfDocument);
            IFormField element = GetDataToTest(testContainer.index)();
            element.SetProperty(FormProperty.FORM_ACCESSIBILITY_LANGUAGE, "en");
            IFormField formField = (IFormField)element;
            formField.SetInteractive(true);
            document.Add((IBlockElement)element);
            PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
            IStructureNode documentStruct = root.GetKids()[0];
            IStructureNode kid = documentStruct.GetKids()[0];
            PdfStructElem elem = (PdfStructElem)kid;
            PdfDictionary obj = elem.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.Form, elem.GetRole());
            NUnit.Framework.Assert.IsTrue(obj.ContainsKey(PdfName.Lang));
            NUnit.Framework.Assert.AreEqual("en", obj.GetAsString(PdfName.Lang).GetValue());
            document.Close();
            pdfDocument.Close();
        }

        public class TestContainer {
            public readonly int index;

            public TestContainer(int index) {
                this.index = index;
            }

            public override String ToString() {
                switch (index) {
                    case 0: {
                        return "InputField";
                    }

                    case 1: {
                        return "TextArea";
                    }

                    case 2: {
                        return "Radio";
                    }

                    case 3: {
                        return "ComboBox";
                    }

                    case 4: {
                        return "ListBox";
                    }

                    case 5: {
                        return "SignatureField";
                    }

                    case 6: {
                        return "Button";
                    }

                    case 7: {
                        return "CheckBox";
                    }

                    default: {
                        throw new ArgumentException("Invalid index");
                    }
                }
            }
        }
    }
}
