/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Parser {
    [NUnit.Framework.Category("UnitTest")]
    public class MultiLocaleTest : ExtendedITextTest {
        private readonly CultureInfo defaultLocale = System.Threading.Thread.CurrentThread.CurrentUICulture;

        public static ICollection<CultureInfo> Locales() {
            return JavaUtil.ArraysAsList(System.Globalization.CultureInfo.InvariantCulture, new CultureInfo("tr", false
                ));
        }

        [NUnit.Framework.TearDown]
        public virtual void SetDefaultLocale() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = defaultLocale;
        }

        [NUnit.Framework.TestCaseSource("Locales")]
        public virtual void CaseSupport(CultureInfo locale) {
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

        [NUnit.Framework.TestCaseSource("Locales")]
        public virtual void AttributeCaseNormalization(CultureInfo locale) {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            ParseSettings parseSettings = new ParseSettings(false, false);
            String normalizedAttribute = parseSettings.NormalizeAttribute("HIDDEN");
            NUnit.Framework.Assert.AreEqual("hidden", normalizedAttribute);
        }

        [NUnit.Framework.TestCaseSource("Locales")]
        public virtual void AttributesCaseNormalization(CultureInfo locale) {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            ParseSettings parseSettings = new ParseSettings(false, false);
            Attributes attributes = new Attributes();
            attributes.Put("ITEM", "1");
            Attributes normalizedAttributes = parseSettings.NormalizeAttributes(attributes);
            NUnit.Framework.Assert.AreEqual("item", normalizedAttributes.AsList()[0].Key);
        }

        [NUnit.Framework.TestCaseSource("Locales")]
        public virtual void CanBeInsensitive(CultureInfo locale) {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            iText.StyledXmlParser.Jsoup.Parser.Tag script1 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("script", 
                ParseSettings.htmlDefault);
            iText.StyledXmlParser.Jsoup.Parser.Tag script2 = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf("SCRIPT", 
                ParseSettings.htmlDefault);
            NUnit.Framework.Assert.AreSame(script1, script2);
        }
    }
}
