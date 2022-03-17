using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    public class SignatureFormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        private static readonly Rectangle DUMMY_RECTANGLE = new Rectangle(7, 11, 13, 17);

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            SignatureFormFieldBuilder builder = new SignatureFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void CreateSignatureWithWidgetTest() {
            PdfSignatureFormField signatureFormField = new SignatureFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).CreateSignature();
            CompareSignatures(signatureFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSignatureWithoutWidgetTest() {
            PdfSignatureFormField signatureFormField = new SignatureFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreateSignature
                ();
            CompareSignatures(signatureFormField, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSignatureWithConformanceLevelTest() {
            PdfSignatureFormField signatureFormField = new SignatureFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).SetConformanceLevel(PdfAConformanceLevel.PDF_A_1A).CreateSignature();
            CompareSignatures(signatureFormField, true);
        }

        private static void CompareSignatures(PdfSignatureFormField signatureFormField, bool widgetExpected) {
            PdfDictionary expectedDictionary = new PdfDictionary();
            IList<PdfWidgetAnnotation> widgets = signatureFormField.GetWidgets();
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
            PutIfAbsent(expectedDictionary, PdfName.FT, PdfName.Sig);
            PutIfAbsent(expectedDictionary, PdfName.T, new PdfString(DUMMY_NAME));
            expectedDictionary.MakeIndirect(DUMMY_DOCUMENT);
            signatureFormField.MakeIndirect(DUMMY_DOCUMENT);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(expectedDictionary, signatureFormField
                .GetPdfObject()));
        }

        private static void PutIfAbsent(PdfDictionary dictionary, PdfName name, PdfObject value) {
            if (!dictionary.ContainsKey(name)) {
                dictionary.Put(name, value);
            }
        }
    }
}
