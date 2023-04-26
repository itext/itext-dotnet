/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Layout.Properties;
using iText.StyledXmlParser.Css;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class CssBackgroundUtilsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseBackgroundRepeatTest() {
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, CssBackgroundUtils.ParseBackgroundRepeat
                ("repeat"));
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, CssBackgroundUtils.ParseBackgroundRepeat
                ("RePeAt"));
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.NO_REPEAT, CssBackgroundUtils.ParseBackgroundRepeat
                ("no-repeat"));
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, CssBackgroundUtils.ParseBackgroundRepeat
                ("no- repeat"));
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.ROUND, CssBackgroundUtils.ParseBackgroundRepeat
                ("round"));
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, CssBackgroundUtils.ParseBackgroundRepeat
                ("ro!und"));
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.SPACE, CssBackgroundUtils.ParseBackgroundRepeat
                ("space"));
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, CssBackgroundUtils.ParseBackgroundRepeat
                (" space "));
            NUnit.Framework.Assert.AreEqual(BackgroundRepeat.BackgroundRepeatValue.REPEAT, CssBackgroundUtils.ParseBackgroundRepeat
                ("something"));
        }

        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.URL_IS_NOT_CLOSED_IN_CSS_EXPRESSION
            )]
        [NUnit.Framework.Test]
        public virtual void ResolveBackgroundPropertyTypeTest() {
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("jaja"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("ul(rock_texture.jpg)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("url(rock_texture.jpg"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("url(rock(_texture.jpg)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("url(rock_t(ext)ure.jpg)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.UNDEFINED, CssBackgroundUtils.ResolveBackgroundPropertyType
                ("url(url(rock_texture.jpg)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_IMAGE, CssBackgroundUtils
                .ResolveBackgroundPropertyType("url(rock_texture.jpg)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_IMAGE, CssBackgroundUtils
                .ResolveBackgroundPropertyType("linear-gradient(#e66465, #9198e5)"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_IMAGE, CssBackgroundUtils
                .ResolveBackgroundPropertyType("none"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_REPEAT, CssBackgroundUtils
                .ResolveBackgroundPropertyType("repeat-x"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_X, CssBackgroundUtils
                .ResolveBackgroundPropertyType("left"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_Y, CssBackgroundUtils
                .ResolveBackgroundPropertyType("bottom"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION, CssBackgroundUtils
                .ResolveBackgroundPropertyType("center"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION_OR_SIZE, CssBackgroundUtils
                .ResolveBackgroundPropertyType("10%"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_SIZE, CssBackgroundUtils
                .ResolveBackgroundPropertyType("contain"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ORIGIN_OR_CLIP, CssBackgroundUtils
                .ResolveBackgroundPropertyType("padding-box"));
            NUnit.Framework.Assert.AreEqual(CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ATTACHMENT, CssBackgroundUtils
                .ResolveBackgroundPropertyType("fixed"));
        }

        [NUnit.Framework.Test]
        public virtual void GetBackgroundPropertyNameFromType() {
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_COLOR, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_COLOR));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_IMAGE, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_IMAGE));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_CLIP, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_CLIP));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_ORIGIN, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ORIGIN));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_POSITION, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_POSITION));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_REPEAT, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_REPEAT));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_SIZE, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_SIZE));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.BACKGROUND_ATTACHMENT, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.BACKGROUND_ATTACHMENT));
            NUnit.Framework.Assert.AreEqual(CommonCssConstants.UNDEFINED_NAME, CssBackgroundUtils.GetBackgroundPropertyNameFromType
                (CssBackgroundUtils.BackgroundPropertyType.UNDEFINED));
        }
    }
}
