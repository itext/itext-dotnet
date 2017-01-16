using System;
using iText.Kernel.Pdf.Xobject;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfImageXObjectTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/parser/PdfImageXObjectTest/";

        /// <exception cref="System.Exception"/>
        private void TestFile(String filename, int page, String objectid) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + filename));
            try {
                PdfResources resources = pdfDocument.GetPage(page).GetResources();
                PdfDictionary xobjets = resources.GetResource(PdfName.XObject);
                PdfObject obj = xobjets.Get(new PdfName(objectid));
                if (obj == null) {
                    throw new ArgumentNullException("Reference " + objectid + " not found - Available keys are " + xobjets.KeySet
                        ());
                }
                PdfImageXObject img = new PdfImageXObject((PdfStream)(obj.IsIndirectReference() ? ((PdfIndirectReference)obj
                    ).GetRefersTo() : obj));
                byte[] result = img.GetImageBytes(true);
                NUnit.Framework.Assert.IsNotNull(result);
                int zeroCount = 0;
                foreach (byte b in result) {
                    if (b == 0) {
                        zeroCount++;
                    }
                }
                NUnit.Framework.Assert.IsTrue(zeroCount > 0);
            }
            finally {
                pdfDocument.Close();
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestMultiStageFilters() {
            TestFile("multistagefilter1.pdf", 1, "Obj13");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestAscii85Filters() {
            TestFile("ASCII85_RunLengthDecode.pdf", 1, "Im9");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestCcittFilters() {
            TestFile("ccittfaxdecode.pdf", 1, "background0");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestFlateDecodeFilters() {
            TestFile("flatedecode_runlengthdecode.pdf", 1, "Im9");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestDctDecodeFilters() {
            TestFile("dctdecode.pdf", 1, "im1");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void Testjbig2Filters() {
            TestFile("jbig2decode.pdf", 1, "2");
        }
    }
}
