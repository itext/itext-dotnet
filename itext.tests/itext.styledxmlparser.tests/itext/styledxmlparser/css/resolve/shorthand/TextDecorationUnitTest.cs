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
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;
using iText.Test;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand {
    [NUnit.Framework.Category("UnitTest")]
    public class TextDecorationUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ResolveShorthandLineEmptyTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-line"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandLineNoneTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("none");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            NUnit.Framework.Assert.AreEqual("none", resultMap.Get("text-decoration-line"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandLineNoneAndUnderlineTogetherTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("none underline");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            String line = resultMap.Get("text-decoration-line");
            NUnit.Framework.Assert.IsTrue(line != null && line.Contains("underline") && line.Contains("none"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandLineOnePropertyTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("underline");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            NUnit.Framework.Assert.AreEqual("underline", resultMap.Get("text-decoration-line"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandLineTwoPropertiesTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("underline overline");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            String line = resultMap.Get("text-decoration-line");
            NUnit.Framework.Assert.IsTrue(line != null && line.Contains("underline") && line.Contains("overline"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandColorNamedTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("underline red");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            NUnit.Framework.Assert.AreEqual("underline", resultMap.Get("text-decoration-line"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("red", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandColorRgbTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("underline rgb(255, 255, 0)");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            NUnit.Framework.Assert.AreEqual("underline", resultMap.Get("text-decoration-line"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("rgb(255,255,0)", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandColorRgbWithOpacityTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("underline rgb(255, 255, 0, 0.5)");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            NUnit.Framework.Assert.AreEqual("underline", resultMap.Get("text-decoration-line"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("rgb(255,255,0,0.5)", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandColorHslTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("underline hsl(300, 76%, 72%)");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            NUnit.Framework.Assert.AreEqual("underline", resultMap.Get("text-decoration-line"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("hsl(300,76%,72%)", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandColorHexTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("underline #DDAA55");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            NUnit.Framework.Assert.AreEqual("underline", resultMap.Get("text-decoration-line"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("#ddaa55", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandStyleOnePropertyTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("underline wavy");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            NUnit.Framework.Assert.AreEqual("underline", resultMap.Get("text-decoration-line"));
            NUnit.Framework.Assert.AreEqual("wavy", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-color"));
        }

        [NUnit.Framework.Test]
        public virtual void ResolveShorthandStyleTwoPropertiesTest() {
            TextDecorationShorthandResolver resolver = new TextDecorationShorthandResolver();
            IList<CssDeclaration> result = resolver.ResolveShorthand("underline wavy dotted");
            IDictionary<String, String> resultMap = ConvertCssDeclarationsToMap(result);
            NUnit.Framework.Assert.AreEqual(3, resultMap.Count);
            NUnit.Framework.Assert.AreEqual("underline", resultMap.Get("text-decoration-line"));
            NUnit.Framework.Assert.AreEqual("dotted", resultMap.Get("text-decoration-style"));
            NUnit.Framework.Assert.AreEqual("initial", resultMap.Get("text-decoration-color"));
        }

        private IDictionary<String, String> ConvertCssDeclarationsToMap(IList<CssDeclaration> declarations) {
            IDictionary<String, String> result = new Dictionary<String, String>();
            foreach (CssDeclaration decl in declarations) {
                result.Put(decl.GetProperty(), decl.GetExpression());
            }
            return result;
        }
    }
}
