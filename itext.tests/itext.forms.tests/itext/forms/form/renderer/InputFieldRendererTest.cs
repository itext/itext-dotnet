using iText.Forms.Form;
using iText.Forms.Form.Element;
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
        public virtual void CreateParagraphRendererTest() {
            InputFieldRenderer inputFieldRendererWithoutPlaceholder = new InputFieldRenderer(new InputField(""));
            IRenderer paragraphRender = inputFieldRendererWithoutPlaceholder.CreateParagraphRenderer("");
            NUnit.Framework.Assert.IsTrue(paragraphRender is ParagraphRenderer);
            InputField inputFieldWithEmptyPlaceholder = new InputField("");
            inputFieldWithEmptyPlaceholder.SetPlaceholder(new _Paragraph_82());
            InputFieldRenderer inputFieldRendererWithEmptyPlaceholder = new InputFieldRenderer(inputFieldWithEmptyPlaceholder
                );
            paragraphRender = inputFieldRendererWithEmptyPlaceholder.CreateParagraphRenderer("");
            NUnit.Framework.Assert.IsTrue(paragraphRender is ParagraphRenderer);
            NUnit.Framework.Assert.IsFalse(paragraphRender is InputFieldRendererTest.CustomParagraphRenderer);
            InputField inputFieldWithPlaceholder = new InputField("");
            inputFieldWithPlaceholder.SetPlaceholder(new _Paragraph_95());
            InputFieldRenderer inputFieldRendererWithPlaceholder = new InputFieldRenderer(inputFieldWithPlaceholder);
            paragraphRender = inputFieldRendererWithPlaceholder.CreateParagraphRenderer("");
            NUnit.Framework.Assert.IsTrue(paragraphRender is InputFieldRendererTest.CustomParagraphRenderer);
        }

        private sealed class _Paragraph_82 : Paragraph {
            public _Paragraph_82() {
            }

            public override IRenderer CreateRendererSubTree() {
                return new InputFieldRendererTest.CustomParagraphRenderer(this);
            }
        }

        private sealed class _Paragraph_95 : Paragraph {
            public _Paragraph_95() {
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
