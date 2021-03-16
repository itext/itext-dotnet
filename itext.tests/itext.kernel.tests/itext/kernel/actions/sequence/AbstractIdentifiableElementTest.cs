/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.Kernel;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Actions.Sequence {
    public class AbstractIdentifiableElementTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetFirstIdTest() {
            AbstractIdentifiableElementTest.TestElement element = new AbstractIdentifiableElementTest.TestElement();
            SequenceId sequenceId = new SequenceId();
            element.SetSequenceId(sequenceId);
            NUnit.Framework.Assert.AreEqual(sequenceId, element.GetSequenceId());
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.ELEMENT_ALREADY_HAS_AN_IDENTIFIER)]
        public virtual void SetSecondIdTest() {
            AbstractIdentifiableElementTest.TestElement element = new AbstractIdentifiableElementTest.TestElement();
            SequenceId firstSequenceId = new SequenceId();
            SequenceId secondSequenceId = new SequenceId();
            element.SetSequenceId(firstSequenceId);
            element.SetSequenceId(secondSequenceId);
            NUnit.Framework.Assert.AreEqual(firstSequenceId, element.GetSequenceId());
            NUnit.Framework.Assert.AreNotEqual(secondSequenceId, element.GetSequenceId());
        }

        private class TestElement : AbstractIdentifiableElement {
        }
    }
}
