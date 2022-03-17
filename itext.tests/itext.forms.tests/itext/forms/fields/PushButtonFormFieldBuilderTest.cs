using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Fields {
    public class PushButtonFormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        private static readonly Rectangle DUMMY_RECTANGLE = new Rectangle(7, 11, 13, 17);

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            PushButtonFormFieldBuilder builder = new PushButtonFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void SetGetCaptionType() {
            PushButtonFormFieldBuilder builder = new PushButtonFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME);
            builder.SetCaption("Caption");
            NUnit.Framework.Assert.AreEqual("Caption", builder.GetCaption());
        }

        [NUnit.Framework.Test]
        public virtual void CreatePushButtonWithWidgetTest() {
            PdfButtonFormField pushButtonFormField = new PushButtonFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).CreatePushButton();
            ComparePushButtons(pushButtonFormField, true);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePushButtonWithoutWidgetTest() {
            PdfButtonFormField pushButtonFormField = new PushButtonFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).CreatePushButton
                ();
            ComparePushButtons(pushButtonFormField, false);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePushButtonWithConformanceLevelTest() {
            PdfButtonFormField pushButtonFormField = new PushButtonFormFieldBuilder(DUMMY_DOCUMENT, DUMMY_NAME).SetWidgetRectangle
                (DUMMY_RECTANGLE).SetConformanceLevel(PdfAConformanceLevel.PDF_A_1A).CreatePushButton();
            ComparePushButtons(pushButtonFormField, true);
        }

        private static void ComparePushButtons(PdfButtonFormField pushButtonFormField, bool widgetExpected) {
            PdfDictionary expectedDictionary = new PdfDictionary();
            IList<PdfWidgetAnnotation> widgets = pushButtonFormField.GetWidgets();
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
            PutIfAbsent(expectedDictionary, PdfName.FT, PdfName.Btn);
            PutIfAbsent(expectedDictionary, PdfName.Ff, new PdfNumber(PdfButtonFormField.FF_PUSH_BUTTON));
            PutIfAbsent(expectedDictionary, PdfName.T, new PdfString(DUMMY_NAME));
            expectedDictionary.MakeIndirect(DUMMY_DOCUMENT);
            pushButtonFormField.MakeIndirect(DUMMY_DOCUMENT);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareDictionariesStructure(expectedDictionary, pushButtonFormField
                .GetPdfObject()));
        }

        private static void PutIfAbsent(PdfDictionary dictionary, PdfName name, PdfObject value) {
            if (!dictionary.ContainsKey(name)) {
                dictionary.Put(name, value);
            }
        }
    }
}
