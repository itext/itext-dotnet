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
using System.IO;
using iText.Commons.Utils;
using iText.Forms.Exceptions;
using iText.IO.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class ChoiceFormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        private static readonly Rectangle DUMMY_RECTANGLE = new Rectangle(7, 11, 13, 17);

        private static readonly PdfArray DUMMY_OPTIONS = new PdfArray(JavaUtil.ArraysAsList("option1", "option2", 
            "option3"), false);

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            ChoiceFormFieldBuilder builder = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void SetGetOptionsAsPdfArrayTest() {
            ChoiceFormFieldBuilder builder = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            builder.SetOptions(DUMMY_OPTIONS);
            NUnit.Framework.Assert.AreSame(DUMMY_OPTIONS, builder.GetOptions());
        }

        [NUnit.Framework.Test]
        public virtual void SetGetOptionsAsStringArrayTest() {
            ChoiceFormFieldBuilder builder = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            String[] options = new String[] { "option1", "option2", "option3" };
            builder.SetOptions(options);
            for (int i = 0; i < options.Length; ++i) {
                NUnit.Framework.Assert.AreEqual(new PdfString(options[i], PdfEncodings.UNICODE_BIG), builder.GetOptions().
                    GetAsString(i));
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetGetOptionsAsTwoDimensionalStringArrayTest() {
            ChoiceFormFieldBuilder builder = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            String[][] options = new String[][] { new String[] { "option1", "option2" }, new String[] { "option3", "option4"
                 } };
            builder.SetOptions(options);
            for (int i = 0; i < options.Length; ++i) {
                for (int j = 0; j < options[i].Length; ++j) {
                    NUnit.Framework.Assert.AreEqual(new PdfString(options[i][j], PdfEncodings.UNICODE_BIG), builder.GetOptions
                        ().GetAsArray(i).GetAsString(j));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetGetOptionsAsIllegalTwoDimensionalStringArrayTest() {
            ChoiceFormFieldBuilder builder = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            String[][] options = new String[][] { new String[] { "option1", "option2", "option3" } };
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => builder.SetOptions(options
                ));
            NUnit.Framework.Assert.AreEqual(FormsExceptionMessageConstant.INNER_ARRAY_SHALL_HAVE_TWO_ELEMENTS, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void CreateComboBoxWithWidgetTest() {
            PdfChoiceFormField choiceFormField = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).CreateComboBox();
            CompareChoices(new PdfDictionary(), choiceFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateComboBoxWithoutWidgetTest() {
            PdfChoiceFormField choiceFormField = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateComboBox
                ();
            CompareChoices(new PdfDictionary(), choiceFormField, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateComboBoxWithConformanceLevelTest() {
            PdfChoiceFormField choiceFormField = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).SetGenericConformanceLevel(PdfAConformanceLevel.PDF_A_1A).CreateComboBox();
            CompareChoices(new PdfDictionary(), choiceFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateComboBoxWithOptionsTest() {
            PdfChoiceFormField choiceFormField = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).SetOptions(DUMMY_OPTIONS).CreateComboBox();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Opt, DUMMY_OPTIONS);
            CompareChoices(expectedDictionary, choiceFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateComboBoxWithoutOptionsTest() {
            PdfChoiceFormField choiceFormField = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).CreateComboBox();
            CompareChoices(new PdfDictionary(), choiceFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateListWithWidgetTest() {
            PdfChoiceFormField choiceFormField = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).CreateList();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            CompareChoices(expectedDictionary, choiceFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateListWithoutWidgetTest() {
            PdfChoiceFormField choiceFormField = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateList();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            CompareChoices(expectedDictionary, choiceFormField, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateListWithConformanceLevelTest() {
            PdfChoiceFormField choiceFormField = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).SetGenericConformanceLevel(PdfAConformanceLevel.PDF_A_1A).CreateList();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            CompareChoices(expectedDictionary, choiceFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateListWithOptionsTest() {
            PdfChoiceFormField choiceFormField = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).SetOptions(DUMMY_OPTIONS).CreateList();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            expectedDictionary.Put(PdfName.Opt, DUMMY_OPTIONS);
            CompareChoices(expectedDictionary, choiceFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateListWithoutOptionsTest() {
            PdfChoiceFormField choiceFormField = new ChoiceFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).CreateList();
            PdfDictionary expectedDictionary = new PdfDictionary();
            expectedDictionary.Put(PdfName.Ff, new PdfNumber(0));
            CompareChoices(expectedDictionary, choiceFormField, true);
        }

        private static void CompareChoices(PdfDictionary expectedDictionary, PdfChoiceFormField choiceFormField, bool
             widgetExpected) {
            IList<PdfWidgetAnnotation> widgets = choiceFormField.GetWidgets();
            if (widgetExpected) {
                NUnit.Framework.Assert.AreEqual(1, widgets.Count);
                PdfWidgetAnnotation annotation = widgets[0];
                NUnit.Framework.Assert.IsTrue(DUMMY_RECTANGLE.EqualsWithEpsilon(annotation.GetRectangle().ToRectangle()));
                PdfArray kids = new PdfArray();
                kids.Add(annotation.GetPdfObject());
                PutIfAbsent(expectedDictionary, PdfName.Kids, kids);
            }
            else {
                NUnit.Framework.Assert.AreEqual(0, widgets.Count);
            }
            PutIfAbsent(expectedDictionary, PdfName.FT, PdfName.Ch);
            PutIfAbsent(expectedDictionary, PdfName.Ff, new PdfNumber(PdfChoiceFormField.FF_COMBO));
            PutIfAbsent(expectedDictionary, PdfName.Opt, new PdfArray());
            PutIfAbsent(expectedDictionary, PdfName.T, new PdfString(DUMMY_NAME));
            PutIfAbsent(expectedDictionary, PdfName.V, new PdfArray());
            PutIfAbsent(expectedDictionary, PdfName.DA, choiceFormField.GetPdfObject().Get(PdfName.DA));
            expectedDictionary.MakeIndirect(DUMMY_DOCUMENT);
            choiceFormField.MakeIndirect(DUMMY_DOCUMENT);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(expectedDictionary, choiceFormField
                .GetPdfObject()));
        }

        private static void PutIfAbsent(PdfDictionary dictionary, PdfName name, PdfObject value) {
            if (!dictionary.ContainsKey(name)) {
                dictionary.Put(name, value);
            }
        }
    }
}
