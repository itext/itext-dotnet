/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;
using iText.Test;

namespace iText.StyledXmlParser.Jsoup.Safety {
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
        public virtual void SafeListedProtocolShouldBeRetained() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            Whitelist safelist = (Whitelist)((Whitelist)Whitelist.None()).AddTags("a").AddAttributes("a", "href").AddProtocols
                ("a", "href", "something");
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean("<a href=\"SOMETHING://x\"></a>", safelist);
            NUnit.Framework.Assert.AreEqual("<a href=\"something://x/\"></a>", TextUtil.StripNewlines(cleanHtml));
        }

        [NUnit.Framework.Test]
        public virtual void CleanerSafeListedProtocolShouldBeRetained() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            Safelist safelist = Safelist.None().AddTags("a").AddAttributes("a", "href").AddProtocols("a", "href", "something"
                );
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean("<a href=\"SOMETHING://x\"></a>", safelist);
            NUnit.Framework.Assert.AreEqual("<a href=\"something://x/\"></a>", TextUtil.StripNewlines(cleanHtml));
        }

        [NUnit.Framework.Test]
        public virtual void CompatibilitySafeListedProtocolShouldBeRetained() {
            System.Threading.Thread.CurrentThread.CurrentUICulture = locale;
            Whitelist safelist = (Whitelist)((Whitelist)Whitelist.None()).AddTags("a").AddAttributes("a", "href").AddProtocols
                ("a", "href", "something");
            String cleanHtml = iText.StyledXmlParser.Jsoup.Jsoup.Clean("<a href=\"SOMETHING://x\"></a>", safelist);
            NUnit.Framework.Assert.AreEqual("<a href=\"something://x/\"></a>", TextUtil.StripNewlines(cleanHtml));
        }
    }
}
