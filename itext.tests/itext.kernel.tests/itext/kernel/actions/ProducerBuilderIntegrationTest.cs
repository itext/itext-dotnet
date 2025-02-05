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
using System.IO;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Actions {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ProducerBuilderIntegrationTest : ExtendedITextTest {
        private static String ITEXT_PRODUCER;

        private const String MODIFIED_USING = "; modified using ";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    doc.AddNewPage();
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                ITEXT_PRODUCER = docReopen.GetDocumentInfo().GetProducer();
            }
        }

        [NUnit.Framework.Test]
        public virtual void ModifiedByItextTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    doc.GetDocumentInfo().SetProducer("someProducer");
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                NUnit.Framework.Assert.AreEqual("someProducer" + MODIFIED_USING + ITEXT_PRODUCER, docReopen.GetDocumentInfo
                    ().GetProducer());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ModifiedSecondTimeModifiedByItextTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    doc.GetDocumentInfo().SetProducer("someProducer; modified using anotherProducer");
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                NUnit.Framework.Assert.AreEqual("someProducer; modified using anotherProducer" + MODIFIED_USING + ITEXT_PRODUCER
                    , docReopen.GetDocumentInfo().GetProducer());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreatedByItextModifiedByItextTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    doc.GetDocumentInfo().SetProducer(ITEXT_PRODUCER);
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                NUnit.Framework.Assert.AreEqual(ITEXT_PRODUCER, docReopen.GetDocumentInfo().GetProducer());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ModifiedByItextSecondTimeModifiedByItextTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    doc.GetDocumentInfo().SetProducer("someProducer" + MODIFIED_USING + ITEXT_PRODUCER);
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                NUnit.Framework.Assert.AreEqual("someProducer" + MODIFIED_USING + ITEXT_PRODUCER, docReopen.GetDocumentInfo
                    ().GetProducer());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ModifiedByItextSecondTimeModifiedThirdTimeModifiedByItextTest() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument doc = new PdfDocument(new PdfWriter(outputStream))) {
                    doc.GetDocumentInfo().SetProducer("someProducer" + MODIFIED_USING + ITEXT_PRODUCER + MODIFIED_USING + "thirdProducer"
                        );
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument docReopen = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                NUnit.Framework.Assert.AreEqual("someProducer" + MODIFIED_USING + ITEXT_PRODUCER + MODIFIED_USING + "thirdProducer"
                     + MODIFIED_USING + ITEXT_PRODUCER, docReopen.GetDocumentInfo().GetProducer());
            }
        }
    }
}
