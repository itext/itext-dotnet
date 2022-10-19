/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Collections.Generic;
using iText.Commons.Actions.Processors;
using iText.Commons.Actions.Sequence;
using iText.Commons.Ecosystem;
using iText.Commons.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Commons.Actions {
    [NUnit.Framework.Category("UnitTest")]
    public class EventManagerTest : ExtendedITextTest {
        [NUnit.Framework.TearDown]
        public virtual void AfterEach() {
            ProductProcessorFactoryKeeper.RestoreDefaultProductProcessorFactory();
        }

        [NUnit.Framework.Test]
        [LogMessage(TestConfigurationEvent.MESSAGE)]
        public virtual void ConfigurationEventTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => EventManager.GetInstance().OnEvent(new TestConfigurationEvent())
                );
        }

        [NUnit.Framework.Test]
        public virtual void ThrowSomeExceptionsTest() {
            EventManager eventManager = EventManager.GetInstance();
            IEventHandler handler1 = new EventManagerTest.ThrowArithmeticExpHandler();
            IEventHandler handler2 = new EventManagerTest.ThrowIllegalArgumentExpHandler();
            eventManager.Register(handler1);
            eventManager.Register(handler2);
            SequenceId sequenceId = new SequenceId();
            try {
                eventManager.OnEvent(new ITextTestEvent(sequenceId, null, "test-event", ProductNameConstant.ITEXT_CORE));
            }
            catch (AggregatedException e) {
                NUnit.Framework.Assert.AreEqual("Error during event processing:\n" + "0) ThrowArithmeticExpHandler\n" + "1) ThrowIllegalArgumentExpHandler\n"
                    , e.Message);
                IList<Exception> aggregatedExceptions = e.GetAggregatedExceptions();
                NUnit.Framework.Assert.AreEqual(2, aggregatedExceptions.Count);
                NUnit.Framework.Assert.AreEqual("ThrowArithmeticExpHandler", aggregatedExceptions[0].Message);
                NUnit.Framework.Assert.AreEqual("ThrowIllegalArgumentExpHandler", aggregatedExceptions[1].Message);
            }
            eventManager.Unregister(handler1);
            eventManager.Unregister(handler2);
        }

        [NUnit.Framework.Test]
        public virtual void ThrowOneUncheckedExceptionsTest() {
            EventManager eventManager = EventManager.GetInstance();
            IEventHandler handler1 = new EventManagerTest.ThrowArithmeticExpHandler();
            eventManager.Register(handler1);
            try {
                SequenceId sequenceId = new SequenceId();
                Exception exception = NUnit.Framework.Assert.Catch(typeof(ArithmeticException), () => eventManager.OnEvent
                    (new ITextTestEvent(sequenceId, null, "test-event", ProductNameConstant.ITEXT_CORE)));
                NUnit.Framework.Assert.AreEqual("ThrowArithmeticExpHandler", exception.Message);
            }
            finally {
                eventManager.Unregister(handler1);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ConfigureHandlersTest() {
            EventManager eventManager = EventManager.GetInstance();
            IEventHandler handler = new EventManagerTest.ThrowArithmeticExpHandler();
            NUnit.Framework.Assert.IsFalse(eventManager.IsRegistered(handler));
            eventManager.Register(handler);
            NUnit.Framework.Assert.IsTrue(eventManager.IsRegistered(handler));
            NUnit.Framework.Assert.IsTrue(eventManager.Unregister(handler));
            NUnit.Framework.Assert.IsFalse(eventManager.IsRegistered(handler));
            NUnit.Framework.Assert.IsFalse(eventManager.Unregister(handler));
        }

        [NUnit.Framework.Test]
        public virtual void TurningOffAgplTest() {
            IProductProcessorFactory defaultProductProcessorFactory = ProductProcessorFactoryKeeper.GetProductProcessorFactory
                ();
            NUnit.Framework.Assert.IsTrue(defaultProductProcessorFactory is DefaultProductProcessorFactory);
            EventManager.AcknowledgeAgplUsageDisableWarningMessage();
            IProductProcessorFactory underAgplProductProcessorFactory1 = ProductProcessorFactoryKeeper.GetProductProcessorFactory
                ();
            NUnit.Framework.Assert.IsTrue(underAgplProductProcessorFactory1 is UnderAgplProductProcessorFactory);
        }

        private class ThrowArithmeticExpHandler : IEventHandler {
            public virtual void OnEvent(IEvent @event) {
                throw new ArithmeticException("ThrowArithmeticExpHandler");
            }
        }

        private class ThrowIllegalArgumentExpHandler : IEventHandler {
            public virtual void OnEvent(IEvent @event) {
                throw new ArgumentException("ThrowIllegalArgumentExpHandler");
            }
        }
    }
}
