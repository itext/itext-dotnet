/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
