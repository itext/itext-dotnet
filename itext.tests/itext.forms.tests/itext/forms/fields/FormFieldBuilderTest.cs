using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Forms.Fields {
    public class FormFieldBuilderTest : ExtendedITextTest {
        private static readonly PdfDocument DUMMY_DOCUMENT = new PdfDocument(new PdfWriter(new MemoryStream()));

        private const String DUMMY_NAME = "dummy name";

        [NUnit.Framework.Test]
        public virtual void ConstructorTest() {
            FormFieldBuilderTest.TestBuilder builder = new FormFieldBuilderTest.TestBuilder(DUMMY_DOCUMENT, DUMMY_NAME
                );
            NUnit.Framework.Assert.AreSame(DUMMY_DOCUMENT, builder.GetDocument());
            NUnit.Framework.Assert.AreSame(DUMMY_NAME, builder.GetFormFieldName());
        }

        [NUnit.Framework.Test]
        public virtual void GetSetConformanceLevelTest() {
            FormFieldBuilderTest.TestBuilder builder = new FormFieldBuilderTest.TestBuilder(DUMMY_DOCUMENT, DUMMY_NAME
                );
            builder.SetConformanceLevel(PdfAConformanceLevel.PDF_A_1A);
            NUnit.Framework.Assert.AreSame(PdfAConformanceLevel.PDF_A_1A, builder.GetConformanceLevel());
        }

        private class TestBuilder : FormFieldBuilder<FormFieldBuilderTest.TestBuilder> {
            protected internal TestBuilder(PdfDocument document, String formFieldName)
                : base(document, formFieldName) {
            }

            protected internal override FormFieldBuilderTest.TestBuilder GetThis() {
                return this;
            }
        }
    }
}
