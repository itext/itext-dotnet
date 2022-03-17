using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    public class NonTerminalFormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            NonTerminalFormFieldBuilder builder = new NonTerminalFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void CreateNonTerminalFormField() {
            PdfFormField nonTerminalFormField = new NonTerminalFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateNonTerminalFormField
                ();
            CompareNonTerminalFormFields(nonTerminalFormField);
        }

        private static void CompareNonTerminalFormFields(PdfFormField nonTerminalFormField) {
            PdfDictionary expectedDictionary = new PdfDictionary();
            IList<PdfWidgetAnnotation> widgets = nonTerminalFormField.GetWidgets();
            NUnit.Framework.Assert.AreEqual(0, widgets.Count);
            PutIfAbsent(expectedDictionary, PdfName.T, new PdfString(DUMMY_NAME));
            expectedDictionary.MakeIndirect(DUMMY_DOCUMENT);
            nonTerminalFormField.MakeIndirect(DUMMY_DOCUMENT);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(expectedDictionary, nonTerminalFormField
                .GetPdfObject()));
        }

        private static void PutIfAbsent(PdfDictionary dictionary, PdfName name, PdfObject value) {
            if (!dictionary.ContainsKey(name)) {
                dictionary.Put(name, value);
            }
        }
    }
}
