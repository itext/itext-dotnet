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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Sequence;
using iText.Kernel.Actions.Data;
using iText.Kernel.Actions.Ecosystem;
using iText.Test;

namespace iText.Kernel.Actions.Events {
    [NUnit.Framework.Category("UnitTest")]
    public class ITextCoreProductEventTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void OpenDocumentEventTest() {
            SequenceId sequenceId = new SequenceId();
            ITextCoreProductEvent @event = ITextCoreProductEvent.CreateProcessPdfEvent(sequenceId, new TestMetaInfo("meta data"
                ), EventConfirmationType.ON_CLOSE);
            NUnit.Framework.Assert.AreEqual(ITextCoreProductEvent.PROCESS_PDF, @event.GetEventType());
            NUnit.Framework.Assert.AreEqual(ProductNameConstant.ITEXT_CORE, @event.GetProductName());
            NUnit.Framework.Assert.AreEqual(EventConfirmationType.ON_CLOSE, @event.GetConfirmationType());
            NUnit.Framework.Assert.AreEqual(sequenceId, @event.GetSequenceId());
            NUnit.Framework.Assert.AreEqual(ITextCoreProductData.GetInstance().GetPublicProductName(), @event.GetProductData
                ().GetPublicProductName());
            NUnit.Framework.Assert.AreEqual(ITextCoreProductData.GetInstance().GetProductName(), @event.GetProductData
                ().GetProductName());
            NUnit.Framework.Assert.AreEqual(ITextCoreProductData.GetInstance().GetVersion(), @event.GetProductData().GetVersion
                ());
            NUnit.Framework.Assert.AreEqual(ITextCoreProductData.GetInstance().GetSinceCopyrightYear(), @event.GetProductData
                ().GetSinceCopyrightYear());
            NUnit.Framework.Assert.AreEqual(ITextCoreProductData.GetInstance().GetToCopyrightYear(), @event.GetProductData
                ().GetToCopyrightYear());
        }
    }
}
