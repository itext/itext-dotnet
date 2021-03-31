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
using System;
using System.Collections.Generic;
using iText.Kernel;
using iText.Kernel.Actions.Session;
using iText.Test;

namespace iText.Kernel.Actions.Processors {
    public class DefaultITextProductEventProcessorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ConstructorWithNullProductNameTest() {
            NUnit.Framework.Assert.That(() =>  {
                new DefaultITextProductEventProcessor(null);
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>().With.Message.EqualTo(PdfException.ProductNameCannotBeNull))
;
        }

        [NUnit.Framework.Test]
        public virtual void BuildFirstLineOfProducerTest() {
            DefaultITextProductEventProcessor processor = new DefaultITextProductEventProcessor("test-product");
            ClosingSession session = new ClosingSession(null);
            processor.AggregationOnClose(session);
            NUnit.Framework.Assert.IsNotNull(session.GetProducer());
            NUnit.Framework.Assert.AreEqual(1, session.GetProducer().Count);
            NUnit.Framework.Assert.AreEqual("test-product", session.GetProducer()[0]);
            processor.CompletionOnClose(session);
            NUnit.Framework.Assert.IsNull(session.GetProducer());
        }

        [NUnit.Framework.Test]
        public virtual void BuildSecondLineOfProducerTest() {
            DefaultITextProductEventProcessor processor = new DefaultITextProductEventProcessor("test-product");
            ClosingSession session = new ClosingSession(null);
            IList<String> producer = new List<String>();
            producer.Add("some producer");
            session.SetProducer(producer);
            processor.AggregationOnClose(session);
            NUnit.Framework.Assert.IsNotNull(session.GetProducer());
            NUnit.Framework.Assert.AreEqual(2, session.GetProducer().Count);
            NUnit.Framework.Assert.AreEqual("some producer", session.GetProducer()[0]);
            NUnit.Framework.Assert.AreEqual("test-product", session.GetProducer()[1]);
            processor.CompletionOnClose(session);
            NUnit.Framework.Assert.IsNull(session.GetProducer());
        }
    }
}
