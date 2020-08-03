/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
