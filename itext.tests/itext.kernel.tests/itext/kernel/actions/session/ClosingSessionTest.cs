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
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Actions.Session {
    public class ClosingSessionTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/actions/";

        [NUnit.Framework.Test]
        public virtual void ProducerTest() {
            ClosingSession session = new ClosingSession(null);
            IList<String> producer = new List<String>();
            producer.Add("producer0");
            producer.Add("producer1");
            NUnit.Framework.Assert.IsNull(session.GetProducer());
            session.SetProducer(producer);
            NUnit.Framework.Assert.AreEqual(producer, session.GetProducer());
        }

        [NUnit.Framework.Test]
        public virtual void PropertiesTest() {
            ClosingSession session = new ClosingSession(null);
            NUnit.Framework.Assert.IsNull(session.GetProperty("test"));
            NUnit.Framework.Assert.IsNull(session.GetProperty("test-map"));
            session.SetProperty("test", "test-value");
            IDictionary<String, Object> testMap = new Dictionary<String, Object>();
            testMap.Put("key", "value");
            testMap.Put("int-key", 0);
            session.SetProperty("test-map", testMap);
            NUnit.Framework.Assert.AreEqual("test-value", session.GetProperty("test"));
            NUnit.Framework.Assert.AreEqual(testMap, session.GetProperty("test-map"));
        }

        [NUnit.Framework.Test]
        public virtual void NullConstructorTest() {
            ClosingSession session = new ClosingSession(null);
            NUnit.Framework.Assert.IsNull(session.GetDocument());
        }

        [NUnit.Framework.Test]
        public virtual void DocumentConstructorTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hello.pdf"))) {
                ClosingSession sessionWithDocument = new ClosingSession(document);
                NUnit.Framework.Assert.AreEqual(document, sessionWithDocument.GetDocument());
            }
        }
    }
}
