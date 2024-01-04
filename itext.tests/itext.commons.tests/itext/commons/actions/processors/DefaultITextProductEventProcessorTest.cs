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
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Sequence;
using iText.Commons.Ecosystem;
using iText.Commons.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Commons.Actions.Processors {
    [NUnit.Framework.Category("UnitTest")]
    public class DefaultITextProductEventProcessorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorWithNullProductNameTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new DefaultITextProductEventProcessor
                (null));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.PRODUCT_NAME_CAN_NOT_BE_NULL, e.Message);
        }

        [NUnit.Framework.Test]
        [LogMessage("{0} you are probably {1}", LogLevel = LogLevelConstants.INFO)]
        public virtual void MessageIsLoggedTest() {
            DefaultITextProductEventProcessorTest.TestDefaultITextProductEventProcessor testProcessor = new DefaultITextProductEventProcessorTest.TestDefaultITextProductEventProcessor
                ();
            ITextTestEvent e = new ITextTestEvent(new SequenceId(), CommonsProductData.GetInstance(), null, "test event"
                );
            NUnit.Framework.Assert.DoesNotThrow(() => testProcessor.OnEvent(new ConfirmEvent(e)));
        }

        [NUnit.Framework.Test]
        [LogMessage("{0} you are probably {1}", LogLevel = LogLevelConstants.INFO, Count = 4)]
        public virtual void MessageIsLoggedThreeTimesTest() {
            int iterationsNumber = 15;
            // "1" correspond to expected iterations with log messages:
            // 1 0 0 0 0
            // 0 1 0 0 0
            // 1 0 0 0 1
            DefaultITextProductEventProcessorTest.TestDefaultITextProductEventProcessor testProcessor = new DefaultITextProductEventProcessorTest.TestDefaultITextProductEventProcessor
                ();
            ITextTestEvent e = new ITextTestEvent(new SequenceId(), CommonsProductData.GetInstance(), null, "test event"
                );
            for (int i = 0; i < iterationsNumber; ++i) {
                NUnit.Framework.Assert.DoesNotThrow(() => testProcessor.OnEvent(new ConfirmEvent(e)));
            }
        }

        private class TestDefaultITextProductEventProcessor : DefaultITextProductEventProcessor {
            public TestDefaultITextProductEventProcessor()
                : base("test product") {
            }

            internal override long AcquireRepeatLevel(int lvl) {
                switch (lvl) {
                    case 0: {
                        return 0;
                    }

                    case 1: {
                        return 5;
                    }

                    case 2: {
                        return 3;
                    }
                }
                return 0;
            }
        }
    }
}
