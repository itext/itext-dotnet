/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    [NUnit.Framework.Category("UnitTest")]
    [NUnit.Framework.TestFixtureSource("LocalesTestFixtureData")]
    public class MultiLocaleTest : ExtendedITextTest {
        private readonly CultureInfo defaultLocale = System.Threading.Thread.CurrentThread.CurrentUICulture;

        public static ICollection<CultureInfo> Locales() {
            return JavaUtil.ArraysAsList(System.Globalization.CultureInfo.InvariantCulture, new CultureInfo("tr", false
                ));
        }

        public static ICollection<NUnit.Framework.TestFixtureData> LocalesTestFixtureData() {
            return Locales().Select(array => new NUnit.Framework.TestFixtureData(array)).ToList();
        }

        [NUnit.Framework.TearDown]
        public virtual void SetDefaultLocale() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = defaultLocale;
        }

        private CultureInfo locale;

        public MultiLocaleTest(CultureInfo locale) {
            this.locale = locale;
        }

        public MultiLocaleTest(CultureInfo[] array)
            : this(array[0]) {
        }

        [NUnit.Framework.Test]
        public virtual void CaseSupport() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            ParseSettings bothOn = new ParseSettings(true, true);
            ParseSettings bothOff = new ParseSettings(false, false);
            ParseSettings tagOn = new ParseSettings(true, false);
            ParseSettings attrOn = new ParseSettings(false, true);
            NUnit.Framework.Assert.AreEqual("IMG", bothOn.NormalizeTag("IMG"));
            NUnit.Framework.Assert.AreEqual("ID", bothOn.NormalizeAttribute("ID"));
            NUnit.Framework.Assert.AreEqual("img", bothOff.NormalizeTag("IMG"));
            NUnit.Framework.Assert.AreEqual("id", bothOff.NormalizeAttribute("ID"));
            NUnit.Framework.Assert.AreEqual("IMG", tagOn.NormalizeTag("IMG"));
            NUnit.Framework.Assert.AreEqual("id", tagOn.NormalizeAttribute("ID"));
            NUnit.Framework.Assert.AreEqual("img", attrOn.NormalizeTag("IMG"));
            NUnit.Framework.Assert.AreEqual("ID", attrOn.NormalizeAttribute("ID"));
        }

        [NUnit.Framework.Test]
        public virtual void AttributeCaseNormalization() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            ParseSettings parseSettings = new ParseSettings(false, false);
            String normalizedAttribute = parseSettings.NormalizeAttribute("HIDDEN");
            NUnit.Framework.Assert.AreEqual("hidden", normalizedAttribute);
        }

        [NUnit.Framework.Test]
        public virtual void AttributesCaseNormalization() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            ParseSettings parseSettings = new ParseSettings(false, false);
            Attributes attributes = new Attributes();
            attributes.Put("ITEM", "1");
            Attributes normalizedAttributes = parseSettings.NormalizeAttributes(attributes);
            NUnit.Framework.Assert.AreEqual("item", normalizedAttributes.AsList()[0].Key);
        }

        [NUnit.Framework.Test]
        public virtual void CanBeInsensitive() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            iText.StyledXmlParser.Jsoup.Parser.Tag script1 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("script", 
                ParseSettings.htmlDefault);
            iText.StyledXmlParser.Jsoup.Parser.Tag script2 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("SCRIPT", 
                ParseSettings.htmlDefault);
            NUnit.Framework.Assert.AreSame(script1, script2);
        }
    }
}
