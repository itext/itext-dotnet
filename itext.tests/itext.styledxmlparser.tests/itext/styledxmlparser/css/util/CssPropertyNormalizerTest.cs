/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using iText.Test;

namespace iText.StyledXmlParser.Css.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class CssPropertyNormalizerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CheckUrlNormalizationSimpleTest() {
            Test("url('data:image/png;base64,iVBORw0K')", "url('data:image/png;base64,iVBORw0K')");
        }

        [NUnit.Framework.Test]
        public virtual void CheckUrlNormalizationUppercaseTest() {
            Test("URL('data:image/png;base64,iVBORw0K')", "url('data:image/png;base64,iVBORw0K')");
            Test("uRL('data:image/png;base64,iVBORw0K')", "url('data:image/png;base64,iVBORw0K')");
            Test("urL('data:image/png;base64,iVBORw0K')", "url('data:image/png;base64,iVBORw0K')");
        }

        [NUnit.Framework.Test]
        public virtual void CheckUrlNormalizationWhitespacesTest() {
            Test("  url(  'data:image/png;base64,iVBORw0K' )", "url('data:image/png;base64,iVBORw0K')");
        }

        [NUnit.Framework.Test]
        public virtual void CheckUrlNormalizationLineTerminatorsTest() {
            // Test is initially added to ensure equal behavior between Java and C#.
            // The behavior itself might be reconsidered in the future. Browsers do not forgive newlines in base64 expressions
            Test("url(data:image/png;base64,iVBOR\nw0K)", "url(data:image/png;base64,iVBOR\nw0K)");
            Test("url(data:image/png;base64,iVBOR\rw0K)", "url(data:image/png;base64,iVBOR\rw0K)");
            Test("url(data:image/png;base64,iVBOR\r\nw0K)", "url(data:image/png;base64,iVBOR\r\nw0K)");
        }

        private void Test(String input, String expectedOutput) {
            String result = CssPropertyNormalizer.Normalize(input);
            NUnit.Framework.Assert.AreEqual(expectedOutput, result);
        }
    }
}
