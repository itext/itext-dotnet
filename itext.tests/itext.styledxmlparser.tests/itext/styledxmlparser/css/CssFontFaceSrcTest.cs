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
using System.IO;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Font;
using iText.StyledXmlParser.Css.Parse;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Css {
    [NUnit.Framework.Category("UnitTest")]
    public class CssFontFaceSrcTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/css/CssFontFaceSrcTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
        }

        [NUnit.Framework.Test]
        public virtual void SrcPropertyTest() {
            String fontSrc = "web-fonts/droid-serif-invalid.";
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(sourceFolder + "srcs.css", FileMode.Open
                , FileAccess.Read));
            CssFontFaceRule fontFaceRule = (CssFontFaceRule)styleSheet.GetStatements()[0];
            CssDeclaration src = fontFaceRule.GetProperties()[0];
            NUnit.Framework.Assert.AreEqual("src", src.GetProperty(), "src expected");
            String[] sources = iText.Commons.Utils.StringUtil.Split(src.GetExpression(), ",");
            NUnit.Framework.Assert.AreEqual(27, sources.Length, "27 sources expected");
            for (int i = 0; i < sources.Length; i++) {
                Matcher m = iText.Commons.Utils.Matcher.Match(CssFontFace.CssFontFaceSrc.UrlPattern, sources[i]);
                NUnit.Framework.Assert.IsTrue(m.Matches(), "Expression doesn't match pattern: " + sources[i]);
                String format = m.Group(CssFontFace.CssFontFaceSrc.FormatGroup);
                String source2 = MessageFormatUtil.Format("{0}({1}){2}", m.Group(CssFontFace.CssFontFaceSrc.TypeGroup), m.
                    Group(CssFontFace.CssFontFaceSrc.UrlGroup), format != null ? MessageFormatUtil.Format(" format({0})", 
                    format) : "");
                String url = CssFontFace.CssFontFaceSrc.Unquote(m.Group(CssFontFace.CssFontFaceSrc.UrlGroup));
                NUnit.Framework.Assert.IsTrue(url.StartsWith(fontSrc), "Invalid url: " + url);
                NUnit.Framework.Assert.IsTrue(format == null || CssFontFace.CssFontFaceSrc.ParseFormat(format) != CssFontFace.FontFormat
                    .None, "Invalid format: " + format);
                NUnit.Framework.Assert.AreEqual(sources[i], source2, "Group check fails: ");
                CssFontFace.CssFontFaceSrc fontFaceSrc = CssFontFace.CssFontFaceSrc.Create(sources[i]);
                NUnit.Framework.Assert.IsTrue(fontFaceSrc.GetSrc().StartsWith(fontSrc), "Invalid url: " + fontSrc);
                String type = "url";
                if (fontFaceSrc.IsLocal()) {
                    type = "local";
                }
                NUnit.Framework.Assert.IsTrue(sources[i].StartsWith(type), "Type '" + type + "' expected: " + sources[i]);
                switch (fontFaceSrc.GetFormat()) {
                    case CssFontFace.FontFormat.OpenType: {
                        NUnit.Framework.Assert.IsTrue(sources[i].Contains("opentype"), "Format " + fontFaceSrc.GetFormat() + " expected: "
                             + sources[i]);
                        break;
                    }

                    case CssFontFace.FontFormat.TrueType: {
                        NUnit.Framework.Assert.IsTrue(sources[i].Contains("truetype"), "Format " + fontFaceSrc.GetFormat() + " expected: "
                             + sources[i]);
                        break;
                    }

                    case CssFontFace.FontFormat.SVG: {
                        NUnit.Framework.Assert.IsTrue(sources[i].Contains("svg"), "Format " + fontFaceSrc.GetFormat() + " expected: "
                             + sources[i]);
                        break;
                    }

                    case CssFontFace.FontFormat.None: {
                        NUnit.Framework.Assert.IsFalse(sources[i].Contains("format("), "Format " + fontFaceSrc.GetFormat() + " expected: "
                             + sources[i]);
                        break;
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.QUOTE_IS_NOT_CLOSED_IN_CSS_EXPRESSION
            )]
        public virtual void ParseBase64SrcTest() {
            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(new FileStream(sourceFolder + "srcs2.css", FileMode.Open
                , FileAccess.Read));
            CssFontFaceRule fontFaceRule = (CssFontFaceRule)styleSheet.GetStatements()[0];
            CssDeclaration src = fontFaceRule.GetProperties()[0];
            NUnit.Framework.Assert.AreEqual("src", src.GetProperty(), "src expected");
            String[] sources = CssFontFace.SplitSourcesSequence(src.GetExpression());
            NUnit.Framework.Assert.AreEqual(8, sources.Length, "8 sources expected");
            for (int i = 0; i < 6; i++) {
                Matcher m = iText.Commons.Utils.Matcher.Match(CssFontFace.CssFontFaceSrc.UrlPattern, sources[i]);
                NUnit.Framework.Assert.IsTrue(m.Matches(), "Expression doesn't match pattern: " + sources[i]);
            }
            for (int i = 6; i < sources.Length; i++) {
                Matcher m = iText.Commons.Utils.Matcher.Match(CssFontFace.CssFontFaceSrc.UrlPattern, sources[i]);
                NUnit.Framework.Assert.IsFalse(m.Matches(), "Expression matches pattern (though it shouldn't!): " + sources
                    [i]);
            }
        }
    }
}
