using System;
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Forms.Fields {
    public class TerminalFormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        private static readonly Rectangle DUMMY_RECTANGLE = new Rectangle(7, 11, 13, 17);

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            TerminalFormFieldBuilderTest.TestBuilder builder = new TerminalFormFieldBuilderTest.TestBuilder(DUMMY_DOCUMENT
                , DUMMY_NAME);
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void GetSetWidgetTest() {
            TerminalFormFieldBuilderTest.TestBuilder builder = new TerminalFormFieldBuilderTest.TestBuilder(DUMMY_DOCUMENT
                , DUMMY_NAME);
            builder.SetWidgetRectangle(DUMMY_RECTANGLE);
            NUnit.Framework.Assert.AreSame(DUMMY_RECTANGLE, builder.GetWidgetRectangle());
        }

        [NUnit.Framework.Test]
        public virtual void GetSetPageTest() {
            TerminalFormFieldBuilderTest.TestBuilder builder = new TerminalFormFieldBuilderTest.TestBuilder(DUMMY_DOCUMENT
                , DUMMY_NAME);
            PdfPage page = DUMMY_DOCUMENT.AddNewPage();
            builder.SetPage(page);
            NUnit.Framework.Assert.AreEqual(1, builder.GetPage());
            builder.SetPage(5);
            NUnit.Framework.Assert.AreEqual(5, builder.GetPage());
        }

        [NUnit.Framework.Test]
        public virtual void SetPageToFieldTest() {
            TerminalFormFieldBuilderTest.TestBuilder builder = new TerminalFormFieldBuilderTest.TestBuilder(DUMMY_DOCUMENT
                , DUMMY_NAME);
            builder.SetPage(5);
            PdfFormField formField = new _PdfFormField_56(DUMMY_DOCUMENT);
            builder.SetPageToField(formField);
        }

        private sealed class _PdfFormField_56 : PdfFormField {
            public _PdfFormField_56(PdfDocument baseArg1)
                : base(baseArg1) {
            }

            public override PdfFormField SetPage(int pageNum) {
                NUnit.Framework.Assert.AreEqual(5, pageNum);
                return this;
            }
        }

        private class TestBuilder : TerminalFormFieldBuilder<TerminalFormFieldBuilderTest.TestBuilder> {
            protected internal TestBuilder(PdfDocument document, String formFieldName)
                : base(document, formFieldName) {
            }

            protected internal override TerminalFormFieldBuilderTest.TestBuilder GetThis() {
                return this;
            }
        }
    }
}
