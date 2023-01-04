/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System;
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Commons.Actions.Sequence {
    [NUnit.Framework.Category("UnitTest")]
    public class SequenceIdManagerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetIdentifier() {
            SequenceIdManagerTest.IdentifiableElement element = new SequenceIdManagerTest.IdentifiableElement();
            NUnit.Framework.Assert.IsNull(SequenceIdManager.GetSequenceId(element));
            SequenceId sequenceId = new SequenceId();
            SequenceIdManager.SetSequenceId(element, sequenceId);
            NUnit.Framework.Assert.AreEqual(sequenceId, SequenceIdManager.GetSequenceId(element));
        }

        [NUnit.Framework.Test]
        public virtual void OverrideIdentifierTest() {
            SequenceIdManagerTest.IdentifiableElement element = new SequenceIdManagerTest.IdentifiableElement();
            SequenceId sequenceId1 = new SequenceId();
            SequenceId sequenceId2 = new SequenceId();
            SequenceIdManager.SetSequenceId(element, sequenceId1);
            Exception e = NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => SequenceIdManager.SetSequenceId
                (element, sequenceId2));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.ELEMENT_ALREADY_HAS_IDENTIFIER
                , sequenceId1.GetId(), sequenceId2.GetId()), e.Message);
        }

        private class IdentifiableElement : AbstractIdentifiableElement {
        }
    }
}
