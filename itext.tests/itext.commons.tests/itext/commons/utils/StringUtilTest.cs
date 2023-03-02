/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Text.RegularExpressions;
using iText.Test;

namespace iText.Commons.Utils {
    /// <summary>At the moment there is no StringUtil class in Java, but there is one in C# and we are testing</summary>
    [NUnit.Framework.Category("UnitTest")]
    public class StringUtilTest : ExtendedITextTest {
        private const char SPLIT_PERIOD = '.';

        [NUnit.Framework.Test]
        public virtual void PatternSplitTest01() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6457 fix different behavior of Pattern.split method)
            // Pattern.split in Java works differently compared to Regex.Split in C#
            // In C#, empty strings are possible at the beginning of the resultant array for non-capturing groups in
            // split regex
            // Thus, in C# we use a separate utility for splitting to align the implementation with Java
            // This test verifies that the resultant behavior is the same
            Regex pattern = iText.Commons.Utils.StringUtil.RegexCompile("(?=[ab])");
            String source = "a01aa78ab89b";
            String[] expected = new String[] { "a01", "a", "a78", "a", "b89", "b" };
            String[] result = iText.Commons.Utils.StringUtil.Split(pattern, source);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void PatternSplitTest02() {
            Regex pattern = iText.Commons.Utils.StringUtil.RegexCompile("(?=[ab])");
            String source = "";
            String[] expected = new String[] { "" };
            String[] result = iText.Commons.Utils.StringUtil.Split(pattern, source);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void StringSplitTest01() {
            // Android-Conversion-Ignore-Test (TODO DEVSIX-6457 fix different behavior of Pattern.split method)
            String source = "a01aa78ab89b";
            String[] expected = new String[] { "a01", "a", "a78", "a", "b89", "b" };
            String[] result = iText.Commons.Utils.StringUtil.Split(source, "(?=[ab])");
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void StringSplitTest02() {
            String source = "";
            String[] expected = new String[] { "" };
            String[] result = iText.Commons.Utils.StringUtil.Split(source, "(?=[ab])");
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts01() {
            String source = "";
            String[] expected = new String[] { "" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(iText.Commons.Utils.StringUtil.Split(source, SPLIT_PERIOD.ToString()), result
                );
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts02() {
            String source = null;
            NUnit.Framework.Assert.Catch(typeof(Exception), () => StringSplitUtil.SplitKeepTrailingWhiteSpace(source, 
                SPLIT_PERIOD));
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts03() {
            String source = "test.test1";
            String[] expected = new String[] { "test", "test1" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts04() {
            String source = "test..test1";
            String[] expected = new String[] { "test", "", "test1" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts05() {
            String source = "test...test1";
            String[] expected = new String[] { "test", "", "", "test1" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyParts06() {
            String source = ".test1";
            String[] expected = new String[] { "", "test1" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyPartsDifferentBehaviour01() {
            String source = "test.";
            String[] expected = new String[] { "test", "" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void SplitKeepEmptyPartsDifferentBehaviour02() {
            String source = "test..";
            String[] expected = new String[] { "test", "", "" };
            String[] result = StringSplitUtil.SplitKeepTrailingWhiteSpace(source, SPLIT_PERIOD);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }
    }
}
