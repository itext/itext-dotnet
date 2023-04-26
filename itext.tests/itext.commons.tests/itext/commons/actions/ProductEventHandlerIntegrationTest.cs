/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using System.IO;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Sequence;
using iText.Commons.Ecosystem;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Commons.Actions {
    public class ProductEventHandlerIntegrationTest : ExtendedITextTest { 
        private TextWriter outBackup;

        [NUnit.Framework.SetUp]
        public virtual void InitTest() {
            outBackup = System.Console.Out;
            ProductEventHandler.INSTANCE.ClearProcessors();
        }

        [NUnit.Framework.TearDown]
        public virtual void AfterEach() {
            System.Console.SetOut(outBackup);
            ProductProcessorFactoryKeeper.RestoreDefaultProductProcessorFactory();
            ProductEventHandler.INSTANCE.ClearProcessors();
        }

        [NUnit.Framework.Test]
        public virtual void RemoveAGPLLoggingTest() {
            MemoryStream testOut = new MemoryStream();
            System.Console.SetOut(new FormattingStreamWriter(testOut));
            
            EventManager.AcknowledgeAgplUsageDisableWarningMessage();
            for (int i = 0; i < 10001; i++) {
                
                ProductEventHandler handler = ProductEventHandler.INSTANCE;
                
                SequenceId sequenceId = new SequenceId();
                
                NUnit.Framework.Assert.IsTrue(handler.GetEvents(sequenceId).IsEmpty());
                ITextTestEvent @event = new ITextTestEvent(sequenceId, null, "test-event",
                    ProductNameConstant.ITEXT_CORE);
                EventManager.GetInstance().OnEvent(@event);
                
                ConfirmEvent confirmEvent = new ConfirmEvent(sequenceId, @event);
                EventManager.GetInstance().OnEvent(confirmEvent);
                
                NUnit.Framework.Assert.AreEqual(1, handler.GetEvents(sequenceId).Count);
                NUnit.Framework.Assert.IsTrue(handler.GetEvents(sequenceId)[0] is ConfirmedEventWrapper);
                NUnit.Framework.Assert.AreEqual(@event, ((ConfirmedEventWrapper)handler.GetEvents(sequenceId)[0]).GetEvent
                ());
            }
            using (var reader = new StreamReader(testOut))
            {
                NUnit.Framework.Assert.AreEqual("", reader.ReadToEnd());
            }
        }
    }
}
