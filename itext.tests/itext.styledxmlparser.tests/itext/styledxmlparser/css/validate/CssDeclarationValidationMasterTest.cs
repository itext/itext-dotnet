using iText.StyledXmlParser.Css;
using iText.Test;

namespace iText.StyledXmlParser.Css.Validate {
    public class CssDeclarationValidationMasterTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void FontSizeEnumValidationTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "larger")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "smaller")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "xx-small")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "x-small")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "small")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "medium")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "large")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "x-large")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "xx-large")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "smaler")));
        }

        [NUnit.Framework.Test]
        public virtual void FontSizeNumericValidationTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "5jaja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "normal")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .FONT_SIZE, "5%")));
        }

        [NUnit.Framework.Test]
        public virtual void WordSpacingValidationTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_SPACING, "5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_SPACING, "5jaja")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_SPACING, "normal")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_SPACING, "5%")));
        }

        [NUnit.Framework.Test]
        public virtual void LetterSpacingValidationTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .LETTER_SPACING, "5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .LETTER_SPACING, "5jaja")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .LETTER_SPACING, "normal")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .LETTER_SPACING, "5%")));
        }

        [NUnit.Framework.Test]
        public virtual void TextIndentValidationTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .TEXT_INDENT, "5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .TEXT_INDENT, "5jaja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .TEXT_INDENT, "normal")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .TEXT_INDENT, "5%")));
        }

        [NUnit.Framework.Test]
        public virtual void LineHeightValidationTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .LINE_HEIGHT, "5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .LINE_HEIGHT, "5jaja")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .LINE_HEIGHT, "normal")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .LINE_HEIGHT, "5%")));
        }
    }
}
