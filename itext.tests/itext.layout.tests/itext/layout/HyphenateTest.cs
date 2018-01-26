/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.Layout.Hyphenation;
using iText.Test;
using NUnit.Framework;

namespace iText.Layout {
    public class HyphenateTest : ExtendedITextTest {

        [TestCase("af")]
        [TestCase("as")]
        [TestCase("bg")]
        [TestCase("bn")]
        [TestCase("ca")]
        [TestCase("cop")]
        [TestCase("cs")]
        [TestCase("cy")]
        [TestCase("da")]
        [TestCase("de")]
        [TestCase("de_1901")]
        [TestCase("de_CH")]
        [TestCase("de_DR")]
        [TestCase("el")]
        [TestCase("el_Polyton")]
        [TestCase("en")]
        [TestCase("en_GB")]
        [TestCase("en_US")]
        [TestCase("eo")]
        [TestCase("es")]
        [TestCase("et")]
        [TestCase("eu")]
        [TestCase("fi")]
        [TestCase("fr")]
        [TestCase("ga")]
        [TestCase("gl")]
        [TestCase("grc")]
        [TestCase("gu")]
        [TestCase("hi")]
        [TestCase("hr")]
        [TestCase("hsb")]
        [TestCase("hu")]
        [TestCase("hy")]
        [TestCase("ia")]
        [TestCase("id")]
        [TestCase("is")]
        [TestCase("it")]
        [TestCase("kmr")]
        [TestCase("kn")]
        [TestCase("la")]
        [TestCase("lo")]
        [TestCase("lt")]
        [TestCase("lv")]
        [TestCase("ml")]
        [TestCase("mn")]
        [TestCase("mr")]
        [TestCase("nb")]
        [TestCase("nl")]
        [TestCase("nn")]
        [TestCase("no")]
        [TestCase("or")]
        [TestCase("pa")]
        [TestCase("pl")]
        [TestCase("pt")]
        [TestCase("ro")]
        [TestCase("ru")]
        [TestCase("sa")]
        [TestCase("sk")]
        [TestCase("sl")]
        [TestCase("sr_Cyrl")]
        [TestCase("sr_Latn")]
        [TestCase("sv")]
        [TestCase("ta")]
        [TestCase("te")]
        [TestCase("tk")]
        [TestCase("tr")]
        [TestCase("uk")]
        [TestCase("zh_Latn")]
        public virtual void LoadConfigTest(String lang) {
            String[] parts = iText.IO.Util.StringUtil.Split(lang, "_");
            lang = parts[0];
            String country = (parts.Length == 2) ? parts[1] : null;
            HyphenationConfig config = new HyphenationConfig(lang, country, 3, 3);
            Assert.IsNotNull(config.Hyphenate("country"), "Language: " + lang + ": hyphenate() returned null");
        }
    }
}
