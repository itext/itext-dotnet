using System;
using System.Collections.Generic;
using System.Text;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class FreeReferencesTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/FreeReferencesTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/FreeReferencesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest01() {
            String src = "freeRefsGapsAndMaxGen.pdf";
            String @out = "freeReferencesTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 4\n" + "0000000000 65535 f \n" + "0000000269 00000 n \n" 
                + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "12 3\n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" };
            // TODO first xref shall have no subsections
            NUnit.Framework.Assert.AreEqual(expected, xrefString);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest02() {
            String src = "freeRefsGapsAndMaxGen.pdf";
            String @out = "freeReferencesTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 5\n" + "0000000010 65535 f \n" + "0000000269 00000 n \n" 
                + "0000000569 00000 n \n" + "0000000314 00000 n \n" + "0000000000 65535 f \n" + "10 5\n" + "0000000011 00000 f \n"
                 + "0000000000 00001 f \n" + "0000000133 00000 n \n" + "0000000015 00000 n \n" + "0000000480 00000 n \n"
                , "xref\n" + "3 1\n" + "0000000995 00000 n \n" };
            // Append mode, no possibility to fix subsections in first xref
            NUnit.Framework.Assert.AreEqual(expected, xrefString);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest03() {
            String src = "freeRefsDeletedObj.pdf";
            String @out = "freeReferencesTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 7\n" + "0000000000 65535 f \n" + "0000000265 00000 n \n" 
                + "0000000564 00000 n \n" + "0000000310 00000 n \n" + "0000000132 00000 n \n" + "0000000015 00001 n \n"
                 + "0000000476 00000 n \n", "xref\n" + "0 1\n" + "0000000005 65535 n \n" + "3 3\n" + "0000000923 00000 n \n"
                 + "0000001170 00000 n \n" + "0000000000 00002 f \n" + "7 1\n" + "0000001303 00000 n \n", "xref\n" + "1 3\n"
                 + "0000001706 00000 n \n" + "0000001998 00000 n \n" + "0000001751 00000 n \n" + "5 1\n" + "0000002055 00002 n \n"
                 + "8 1\n" + "0000002156 00000 n \n" };
            // TODO fifth object is no longer free, however zero obj not updated
            NUnit.Framework.Assert.AreEqual(expected, xrefString);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest04() {
            String src = "simpleDoc.pdf";
            String @out = "freeReferencesTest04.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            PdfObject contentsObj = pdfDocument.GetPage(1).GetPdfObject().Remove(PdfName.Contents);
            NUnit.Framework.Assert.IsTrue(contentsObj is PdfIndirectReference);
            PdfIndirectReference contentsRef = (PdfIndirectReference)contentsObj;
            contentsRef.SetFree();
            PdfObject freedContentsRefRefersTo = contentsRef.GetRefersTo();
            NUnit.Framework.Assert.IsNull(freedContentsRefRefersTo);
            pdfDocument.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest05() {
            String src = "simpleDocWithSubsections.pdf";
            String @out = "freeReferencesTest05.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out));
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 4\n" + "0000000000 65535 f \n" + "0000000269 00000 n \n" 
                + "0000000561 00000 n \n" + "0000000314 00000 n \n" + "11 3\n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000613 00000 n \n" };
            // TODO first xref shall have no subsections
            NUnit.Framework.Assert.AreEqual(expected, xrefString);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest06() {
            String src = "simpleDocWithSubsections.pdf";
            String @out = "freeReferencesTest06.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + src), new PdfWriter(destinationFolder
                 + @out), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 4\n" + "0000000000 65535 f \n" + "0000000269 00000 n \n" 
                + "0000000569 00000 n \n" + "0000000314 00000 n \n" + "11 3\n" + "0000000133 00000 n \n" + "0000000015 00000 n \n"
                 + "0000000480 00000 n \n", "xref\n" + "3 1\n" + "0000000935 00000 n \n" };
            // Append mode, no possibility to fix subsections in first xref
            NUnit.Framework.Assert.AreEqual(expected, xrefString);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void FreeReferencesTest07() {
            String @out = "freeReferencesTest07.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + @out));
            pdfDocument.CreateNextIndirectReference();
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            String[] xrefString = ExtractXrefTableAsStrings(@out);
            String[] expected = new String[] { "xref\n" + "0 4\n" + "0000000000 65535 f \n" + "0000000203 00000 n \n" 
                + "0000000414 00000 n \n" + "0000000248 00000 n \n" + "5 2\n" + "0000000088 00000 n \n" + "0000000015 00000 n \n"
                 };
            // TODO first xref shall have no subsections
            NUnit.Framework.Assert.AreEqual(expected, xrefString);
        }

        /// <exception cref="System.IO.IOException"/>
        private String[] ExtractXrefTableAsStrings(String @out) {
            byte[] outPdfBytes = ReadFile(destinationFolder + @out);
            String outPdfContent = iText.IO.Util.JavaUtil.GetStringForBytes(outPdfBytes, Encoding.ASCII);
            String xrefStr = "\nxref";
            String trailerStr = "trailer";
            int xrefInd = outPdfContent.IndexOf(xrefStr, StringComparison.Ordinal);
            int trailerInd = outPdfContent.IndexOf(trailerStr, StringComparison.Ordinal);
            int lastXrefInd = outPdfContent.LastIndexOf(xrefStr);
            IList<String> xrefs = new List<String>();
            while (true) {
                xrefs.Add(outPdfContent.JSubstring(xrefInd + 1, trailerInd));
                if (xrefInd == lastXrefInd) {
                    break;
                }
                xrefInd = outPdfContent.IndexOf(xrefStr, xrefInd + 1);
                trailerInd = outPdfContent.IndexOf(trailerStr, trailerInd + 1);
            }
            return xrefs.ToArray(new String[xrefs.Count]);
        }
    }
}
