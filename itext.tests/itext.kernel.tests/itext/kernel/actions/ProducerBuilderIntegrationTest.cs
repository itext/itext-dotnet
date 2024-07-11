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
