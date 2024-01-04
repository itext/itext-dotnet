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
using System;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Validate.Impl;
using iText.Test;

namespace iText.StyledXmlParser.Css.Validate {
    [NUnit.Framework.Category("UnitTest")]
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
                .BACKGROUND_POSITION_X, "initial")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION_Y, "-0")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION_Y, "5px")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION_X, "5em")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION_Y, "5px, 5%, bottom")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION_X, "left 5%, right")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION_X, "5")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION_Y, "ja")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION_X, "initial 5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION_Y, "ja, 5px")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .BACKGROUND_POSITION_X, "initial, 5px")));
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

        [NUnit.Framework.Test]
        public virtual void OverflowWrapTest() {
            String[] overflowWrapOrWordWrap = new String[] { CommonCssConstants.OVERFLOW_WRAP, CommonCssConstants.WORDWRAP
                 };
            foreach (String overflowWrap in overflowWrapOrWordWrap) {
                NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(overflowWrap
                    , "normal")));
                NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(overflowWrap
                    , "anywhere")));
                NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(overflowWrap
                    , "break-word")));
                NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(overflowWrap
                    , "inherit")));
                NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(overflowWrap
                    , "unset")));
                NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(overflowWrap
                    , "initial")));
                NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(overflowWrap
                    , "auto")));
                NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(overflowWrap
                    , "norm")));
            }
        }

        [NUnit.Framework.Test]
        public virtual void WordWrapTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_BREAK, "normal")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_BREAK, "break-all")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_BREAK, "keep-all")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_BREAK, "break-word")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_BREAK, "inherit")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_BREAK, "unset")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_BREAK, "initial")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_BREAK, "auto")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .WORD_BREAK, "norm")));
        }

        [NUnit.Framework.Test]
        public virtual void JustifyContentTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "inherit")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "right")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "normal")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "space-between")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "self-end")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "unsafe self-end")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "stretch")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "space-evenly")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "flex-start")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "baseline")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "safe right")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "unsafe normal")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "unsafe space-between")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "self-center")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "self-end unsafe")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "safe stretch")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "space_evenly")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .JUSTIFY_CONTENT, "flex-start left")));
        }

        [NUnit.Framework.Test]
        public virtual void MulticolValidationTest() {
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_COUNT, "auto")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_COUNT, "3")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_COUNT, "-3")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_WIDTH, "auto")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_WIDTH, "30px")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_WIDTH, "20%")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_WIDTH, "5em")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_WIDTH, "5rem")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_WIDTH, "-5rem")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_WIDTH, "10")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_GAP, "normal")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_GAP, "30px")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_GAP, "15%")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_GAP, "2em")));
            NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_GAP, "3rem")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_GAP, "-5em")));
            NUnit.Framework.Assert.IsFalse(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                .COLUMN_GAP, "10")));
        }

        [NUnit.Framework.Test]
        public virtual void ChangeValidatorTest() {
            try {
                CssDeclarationValidationMaster.SetValidator(new CssDeviceCmykAwareValidator());
                NUnit.Framework.Assert.IsTrue(CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(CommonCssConstants
                    .COLOR, "device-cmyk(0, 100%, 70%, 0)")));
            }
            finally {
                CssDeclarationValidationMaster.SetValidator(new CssDefaultValidator());
            }
        }
    }
}
