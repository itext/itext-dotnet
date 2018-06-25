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
using iText.StyledXmlParser.Css;
using iText.Test;

namespace iText.StyledXmlParser.Css.Util {
    public class CssUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ValidateMetricValue() {
            NUnit.Framework.Assert.AreEqual(true, CssUtils.IsMetricValue("1px"));
            NUnit.Framework.Assert.AreEqual(true, CssUtils.IsMetricValue("1in"));
            NUnit.Framework.Assert.AreEqual(true, CssUtils.IsMetricValue("1cm"));
            NUnit.Framework.Assert.AreEqual(true, CssUtils.IsMetricValue("1mm"));
            NUnit.Framework.Assert.AreEqual(true, CssUtils.IsMetricValue("1pc"));
            NUnit.Framework.Assert.AreEqual(false, CssUtils.IsMetricValue("1em"));
            NUnit.Framework.Assert.AreEqual(false, CssUtils.IsMetricValue("1rem"));
            NUnit.Framework.Assert.AreEqual(false, CssUtils.IsMetricValue("1ex"));
            NUnit.Framework.Assert.AreEqual(true, CssUtils.IsMetricValue("1pt"));
            NUnit.Framework.Assert.AreEqual(false, CssUtils.IsMetricValue("1inch"));
            NUnit.Framework.Assert.AreEqual(false, CssUtils.IsMetricValue("+1m"));
        }

        [NUnit.Framework.Test]
        public virtual void ValidateNumericValue() {
            NUnit.Framework.Assert.AreEqual(true, CssUtils.IsNumericValue("1"));
            NUnit.Framework.Assert.AreEqual(true, CssUtils.IsNumericValue("12"));
            NUnit.Framework.Assert.AreEqual(true, CssUtils.IsNumericValue("1.2"));
            NUnit.Framework.Assert.AreEqual(true, CssUtils.IsNumericValue(".12"));
            NUnit.Framework.Assert.AreEqual(false, CssUtils.IsNumericValue("12f"));
            NUnit.Framework.Assert.AreEqual(false, CssUtils.IsNumericValue("f1.2"));
            NUnit.Framework.Assert.AreEqual(false, CssUtils.IsNumericValue(".12f"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseLength() {
            NUnit.Framework.Assert.AreEqual(9, CssUtils.ParseAbsoluteLength("12"), 0);
            NUnit.Framework.Assert.AreEqual(576, CssUtils.ParseAbsoluteLength("8inch"), 0);
            NUnit.Framework.Assert.AreEqual(576, CssUtils.ParseAbsoluteLength("8", CommonCssConstants.IN), 0);
        }

        [NUnit.Framework.Test]
        public virtual void NormalizeProperty() {
            NUnit.Framework.Assert.AreEqual("part1 part2", CssUtils.NormalizeCssProperty("   part1   part2  "));
            NUnit.Framework.Assert.AreEqual("\" the next quote is ESCAPED \\\\\\\" still  IN string \"", CssUtils.NormalizeCssProperty
                ("\" the next quote is ESCAPED \\\\\\\" still  IN string \""));
            NUnit.Framework.Assert.AreEqual("\" the next quote is NOT ESCAPED \\\\\" not in the string", CssUtils.NormalizeCssProperty
                ("\" the next quote is NOT ESCAPED \\\\\" NOT in   THE string"));
            NUnit.Framework.Assert.AreEqual("\" You CAN put 'Single  Quotes' in double quotes WITHOUT escaping\"", CssUtils
                .NormalizeCssProperty("\" You CAN put 'Single  Quotes' in double quotes WITHOUT escaping\""));
            NUnit.Framework.Assert.AreEqual("' You CAN put \"DOUBLE  Quotes\" in double quotes WITHOUT escaping'", CssUtils
                .NormalizeCssProperty("' You CAN put \"DOUBLE  Quotes\" in double quotes WITHOUT escaping'"));
            NUnit.Framework.Assert.AreEqual("\" ( BLA \" attr(href)\" BLA )  \"", CssUtils.NormalizeCssProperty("\" ( BLA \"      AttR( Href  )\" BLA )  \""
                ));
            NUnit.Framework.Assert.AreEqual("\" (  \"attr(href) \"  )  \"", CssUtils.NormalizeCssProperty("\" (  \"aTTr( hREf  )   \"  )  \""
                ));
            NUnit.Framework.Assert.AreEqual("rgba(255,255,255,0.2)", CssUtils.NormalizeCssProperty("rgba(  255,  255 ,  255 ,0.2   )"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void NormalizeUrlTest() {
            NUnit.Framework.Assert.AreEqual("url(data:application/font-woff;base64,2CBPCRXmgywtV1t4oWwjBju0kqkvfhPs0cYdMgFtDSY5uL7MIGT5wiGs078HrvBHekp0Yf=)"
                , CssUtils.NormalizeCssProperty("url(data:application/font-woff;base64,2CBPCRXmgywtV1t4oWwjBju0kqkvfhPs0cYdMgFtDSY5uL7MIGT5wiGs078HrvBHekp0Yf=)"
                ));
            NUnit.Framework.Assert.AreEqual("url(\"quoted  Url\")", CssUtils.NormalizeCssProperty("  url(  \"quoted  Url\")"
                ));
            NUnit.Framework.Assert.AreEqual("url('quoted  Url')", CssUtils.NormalizeCssProperty("  url(  'quoted  Url')"
                ));
            NUnit.Framework.Assert.AreEqual("url(haveEscapedEndBracket\\))", CssUtils.NormalizeCssProperty("url(  haveEscapedEndBracket\\) )"
                ));
        }
    }
}
