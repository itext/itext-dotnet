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

namespace iText.IO.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class GhostScriptHelperUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void VerifyEmptyPageList() {
            String testPageList = "";
            NUnit.Framework.Assert.IsFalse(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyNullPageList() {
            String testPageList = null;
            NUnit.Framework.Assert.IsTrue(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyPageListWithLeadingSpaces() {
            String testPageList = "     1";
            NUnit.Framework.Assert.IsFalse(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyPageListWithTrailingSpaces() {
            String testPageList = "1     ";
            NUnit.Framework.Assert.IsFalse(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyValidPageListWithSeveralPages() {
            String testPageList = "1,2,3";
            NUnit.Framework.Assert.IsTrue(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyValidPageListOfOnePage() {
            String testPageList = "2";
            NUnit.Framework.Assert.IsTrue(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyPageListWithNegativePages() {
            // It's worth mentioning that gs allows negative arguments: if one of the passed list numbers is negative,
            // then all the pages are processed. However, if "0" is passed, then no pages are processed.
            // Having said that, at iText level we're strict and do not allow such values.
            String testPageList = "-2";
            NUnit.Framework.Assert.IsFalse(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyPageListWithSomeNegativePagesInTheMiddle() {
            // It's worth mentioning that gs allows negative arguments: if one of the passed list numbers is negative,
            // then all the pages are processed. However, if "0" is passed, then no pages are processed.
            // Having said that, at iText level we're strict and do not allow such values.
            String testPageList = "1,-2,3";
            NUnit.Framework.Assert.IsFalse(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyPageListWithSomeNegativePagesAtTheEnd() {
            // It's worth mentioning that gs allows negative arguments: if one of the passed list numbers is negative,
            // then all the pages are processed. However, if "0" is passed, then no pages are processed.
            // Having said that, at iText level we're strict and do not allow such values.
            String testPageList = "1,-2";
            NUnit.Framework.Assert.IsFalse(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyPageListWithOnlyPageZero() {
            String testPageList = "0";
            NUnit.Framework.Assert.IsTrue(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyPageListWithOneOfPagesBeingZero() {
            String testPageList = "3,0,2";
            NUnit.Framework.Assert.IsTrue(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyValidPageListWithDescendingOrder() {
            // For gs the order doesn't play any role
            String testPageList = "3,2,1";
            NUnit.Framework.Assert.IsTrue(GhostscriptHelper.ValidatePageList(testPageList));
        }

        [NUnit.Framework.Test]
        public virtual void VerifyTextInPageList() {
            String testPageList = "1,hello,2";
            NUnit.Framework.Assert.IsFalse(GhostscriptHelper.ValidatePageList(testPageList));
        }
    }
}
