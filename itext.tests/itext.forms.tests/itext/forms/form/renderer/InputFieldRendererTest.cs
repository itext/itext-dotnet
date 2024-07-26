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
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Forms.Form.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class InputFieldRendererTest : ExtendedITextTest {
        private const double EPS = 0.0001;

        [NUnit.Framework.Test]
        public virtual void NullPasswordTest() {
            InputFieldRenderer inputFieldRenderer = new InputFieldRenderer(new InputField(""));
            inputFieldRenderer.SetProperty(FormProperty.FORM_FIELD_PASSWORD_FLAG, null);
            NUnit.Framework.Assert.IsFalse(inputFieldRenderer.IsPassword());
        }

        [NUnit.Framework.Test]
        public virtual void NullSizeTest() {
            InputFieldRenderer inputFieldRenderer = new InputFieldRenderer(new InputField(""));
            inputFieldRenderer.SetProperty(FormProperty.FORM_FIELD_SIZE, null);
            NUnit.Framework.Assert.AreEqual(20, inputFieldRenderer.GetSize());
        }

        [NUnit.Framework.Test]
        public virtual void SetMinMaxWidthBasedOnFixedWidthWithAbsoluteWidthTest() {
            InputFieldRendererTest.CustomInputFieldRenderer areaRenderer = new InputFieldRendererTest.CustomInputFieldRenderer
                (new InputField(""));
            areaRenderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(10));
            MinMaxWidth minMaxWidth = new MinMaxWidth();
            NUnit.Framework.Assert.IsTrue(areaRenderer.CallSetMinMaxWidthBasedOnFixedWidth(minMaxWidth));
            NUnit.Framework.Assert.AreEqual(122, minMaxWidth.GetChildrenMaxWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(122, minMaxWidth.GetChildrenMinWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SetMinMaxWidthBasedOnFixedWidthWithoutAbsoluteWidthTest() {
            InputFieldRendererTest.CustomInputFieldRenderer areaRenderer = new InputFieldRendererTest.CustomInputFieldRenderer
                (new InputField(""));
            areaRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePercentValue(10));
            areaRenderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(10));
            MinMaxWidth minMaxWidth = new MinMaxWidth();
            NUnit.Framework.Assert.IsTrue(areaRenderer.CallSetMinMaxWidthBasedOnFixedWidth(minMaxWidth));
            NUnit.Framework.Assert.AreEqual(122, minMaxWidth.GetChildrenMaxWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(0, minMaxWidth.GetChildrenMinWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SetMinMaxWidthBasedOnFixedWidthWithoutAbsoluteWidthOnElementTest() {
            InputFieldRendererTest.CustomInputFieldRenderer areaRenderer = new InputFieldRendererTest.CustomInputFieldRenderer
                (new InputField(""));
            areaRenderer.GetModelElement().SetProperty(Property.WIDTH, UnitValue.CreatePercentValue(10));
            areaRenderer.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(10));
            MinMaxWidth minMaxWidth = new MinMaxWidth();
            NUnit.Framework.Assert.IsTrue(areaRenderer.CallSetMinMaxWidthBasedOnFixedWidth(minMaxWidth));
            NUnit.Framework.Assert.AreEqual(122, minMaxWidth.GetChildrenMaxWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(0, minMaxWidth.GetChildrenMinWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void PdfAConformanceLevelTest() {
            InputFieldRenderer inputFieldRenderer = new InputFieldRenderer(new InputField(""));
            NUnit.Framework.Assert.IsNull(inputFieldRenderer.GetGenericConformanceLevel(null));
        }

        [NUnit.Framework.Test]
        public virtual void PdfAConformanceLevelWithDocumentTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            InputFieldRenderer inputFieldRenderer = new InputFieldRenderer(new InputField(""));
            NUnit.Framework.Assert.IsNull(inputFieldRenderer.GetGenericConformanceLevel(pdfDocument));
        }

        [NUnit.Framework.Test]
        public virtual void PdfAConformanceLevelWithConformanceLevelTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            InputFieldRenderer inputFieldRenderer = new InputFieldRenderer(new InputField(""));
            inputFieldRenderer.SetProperty(FormProperty.FORM_CONFORMANCE_LEVEL, PdfAConformanceLevel.PDF_A_1B);
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_1B, inputFieldRenderer.GetGenericConformanceLevel
                (pdfDocument));
        }

        [NUnit.Framework.Test]
        public virtual void CreateParagraphRendererTest() {
            InputFieldRenderer inputFieldRendererWithoutPlaceholder = new InputFieldRenderer(new InputField(""));
            IRenderer paragraphRender = inputFieldRendererWithoutPlaceholder.CreateParagraphRenderer("");
            NUnit.Framework.Assert.IsTrue(paragraphRender is ParagraphRenderer);
            InputField inputFieldWithEmptyPlaceholder = new InputField("");
            inputFieldWithEmptyPlaceholder.SetPlaceholder(new _Paragraph_129());
            InputFieldRenderer inputFieldRendererWithEmptyPlaceholder = new InputFieldRenderer(inputFieldWithEmptyPlaceholder
                );
            paragraphRender = inputFieldRendererWithEmptyPlaceholder.CreateParagraphRenderer("");
            NUnit.Framework.Assert.IsTrue(paragraphRender is ParagraphRenderer);
            NUnit.Framework.Assert.IsFalse(paragraphRender is InputFieldRendererTest.CustomParagraphRenderer);
            InputField inputFieldWithPlaceholder = new InputField("");
            inputFieldWithPlaceholder.SetPlaceholder(new _Paragraph_142());
            InputFieldRenderer inputFieldRendererWithPlaceholder = new InputFieldRenderer(inputFieldWithPlaceholder);
            paragraphRender = inputFieldRendererWithPlaceholder.CreateParagraphRenderer("");
            NUnit.Framework.Assert.IsTrue(paragraphRender is InputFieldRendererTest.CustomParagraphRenderer);
        }

        private sealed class _Paragraph_129 : Paragraph {
            public _Paragraph_129() {
            }

            public override IRenderer CreateRendererSubTree() {
                return new InputFieldRendererTest.CustomParagraphRenderer(this);
            }
        }

        private sealed class _Paragraph_142 : Paragraph {
            public _Paragraph_142() {
            }

            public override bool IsEmpty() {
                return false;
            }

            public override IRenderer CreateRendererSubTree() {
                return new InputFieldRendererTest.CustomParagraphRenderer(this);
            }
        }

        private class CustomParagraphRenderer : ParagraphRenderer {
            public CustomParagraphRenderer(Paragraph modelElement)
                : base(modelElement) {
            }
        }

        private class CustomInputFieldRenderer : InputFieldRenderer {
            public CustomInputFieldRenderer(InputField modelElement)
                : base(modelElement) {
            }

            public virtual bool CallSetMinMaxWidthBasedOnFixedWidth(MinMaxWidth minMaxWidth) {
                return this.SetMinMaxWidthBasedOnFixedWidth(minMaxWidth);
            }
        }
    }
}
