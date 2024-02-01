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
using iText.Test;

namespace iText.StyledXmlParser.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class WhiteSpaceUtilUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesTest() {
            String toCollapse = "A   B";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = "A B";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesTrailingWhiteSpaceTest() {
            String toCollapse = "A   B   ";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = "A B ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesPrecedingWhiteSpaceTest() {
            String toCollapse = "   A B";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = " A B";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesPrecedingAndTrailingWhiteSpaceTest() {
            String toCollapse = "   A   B   ";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = " A B ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesNewLineWhiteSpaceTest() {
            String toCollapse = "\n   A B  \n";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = " A B ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void CollapseConsecutiveWhiteSpacesTabWhiteSpaceTest() {
            String toCollapse = "\t  A B  \t";
            String actual = WhiteSpaceUtil.CollapseConsecutiveSpaces(toCollapse);
            String expected = " A B ";
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
