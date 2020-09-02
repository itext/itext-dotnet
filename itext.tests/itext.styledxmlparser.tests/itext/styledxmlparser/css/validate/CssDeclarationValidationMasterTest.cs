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

        [NUnit.Framework.Test]
        public virtual void BackgroundRepeatValidationTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "initial")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "no-repeat")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "repeat no-repeat")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "space")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "round")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "space repeat")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "no-repeat round")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "no-repeat,repeat")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "no-repeat repeat,repeat")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "repeat-x, repeat no-repeat")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "ja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "repeat-x repeat")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "initial repeat")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "repeat, repeat-x repeat")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "ja, repeat")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_REPEAT, "initial, repeat")));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundPositionValidationTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "initial")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "0")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "5px")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "5em")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "5px 5%")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "top")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "left top")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "top left")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "50px top")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "left 50px")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "left 50px bottom 20px")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "left 50px bottom")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "left 10% center")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "5px, 5%, left")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "5px 5%, top")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "5")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "ja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "5px ja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "initial 5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "ja, 5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "initial, 5px")));
            // TODO DEVSIX-1457 change the assertions below when background-position will be fully supported.
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "top top")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "left left")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "top 50px")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "50px left")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION, "left 50px center 20px")));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundSizeTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "initial")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "0")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "10px")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "10%")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "10% 10px")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "10px 10em")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "auto")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "auto 10px")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "10px auto")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "cover")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "contain")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "5px, 10%, auto")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "5px 10%, 20em")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "ja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "10")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "cover 10px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "initial 10px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "10px contain")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "ja, 5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "5px, ja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_SIZE, "initial, 10px")));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundOriginTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_ORIGIN, "initial")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_ORIGIN, "border-box")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_ORIGIN, "padding-box")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_ORIGIN, "content-box")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_ORIGIN, "content-box, border-box")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_ORIGIN, "5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_ORIGIN, "ja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_ORIGIN, "border-box border-box")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_ORIGIN, "content-box padding-box")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_ORIGIN, "ja, padding-box")));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundClipTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_CLIP, "initial")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_CLIP, "border-box")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_CLIP, "padding-box")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_CLIP, "content-box")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_CLIP, "content-box, border-box")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_CLIP, "5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_CLIP, "ja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_CLIP, "border-box border-box")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_CLIP, "content-box padding-box")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_CLIP, "ja, padding-box")));
        }

        [NUnit.Framework.Test]
        public virtual void BackgroundImageTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "initial")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "url(rock_texture.jpg)")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "linear-gradient(red,green,blue)")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "url()")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "none")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "url(img.jpg),url(img2.jpg)")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "none,url(img2.jpg)")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "linear-gradient(red,green,blue),url(img2.jpg)")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "ja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "url(url(rock_texture.jpg)")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "true-linear-gradient(red,green,blue)")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "url(img.jpg) url(img2.jpg)")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_IMAGE, "initial,url(img.jpg)")));
        }
    }
}
