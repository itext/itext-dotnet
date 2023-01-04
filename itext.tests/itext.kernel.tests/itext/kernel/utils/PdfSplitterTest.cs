/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfSplitterTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/PdfSplitterTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/utils/PdfSplitterTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY, Count = 3)]
        public virtual void SplitDocumentTest01() {
            String inputFileName = sourceFolder + "iphone_user_guide.pdf";
            PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
            IList<int> pageNumbers = JavaUtil.ArraysAsList(30, 100);
            IList<PdfDocument> splitDocuments = new _PdfSplitter_85(inputPdfDoc).SplitByPageNumbers(pageNumbers);
            foreach (PdfDocument doc in splitDocuments) {
                doc.Close();
            }
            for (int i = 1; i <= 3; i++) {
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "splitDocument1_" + i
                    .ToString() + ".pdf", sourceFolder + "cmp/" + "cmp_splitDocument1_" + i.ToString() + ".pdf", destinationFolder
                    , "diff_"));
            }
        }

        private sealed class _PdfSplitter_85 : PdfSplitter {
            public _PdfSplitter_85(PdfDocument baseArg1)
                : base(baseArg1) {
                this.partNumber = 1;
            }

            internal int partNumber;

            protected internal override PdfWriter GetNextPdfWriter(PageRange documentPageRange) {
                try {
                    return new PdfWriter(PdfSplitterTest.destinationFolder + "splitDocument1_" + (this.partNumber++).ToString(
                        ) + ".pdf");
                }
                catch (FileNotFoundException) {
                    throw new Exception();
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY, Count = 3)]
        public virtual void SplitDocumentTest02() {
            String inputFileName = sourceFolder + "iphone_user_guide.pdf";
            PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
            new _PdfSplitter_115(inputPdfDoc).SplitByPageCount(60, new _IDocumentReadyListener_126());
            for (int i = 1; i <= 3; i++) {
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "splitDocument2_" + i
                    .ToString() + ".pdf", sourceFolder + "cmp/" + "cmp_splitDocument2_" + i.ToString() + ".pdf", destinationFolder
                    , "diff_"));
            }
        }

        private sealed class _PdfSplitter_115 : PdfSplitter {
            public _PdfSplitter_115(PdfDocument baseArg1)
                : base(baseArg1) {
                this.partNumber = 1;
            }

            internal int partNumber;

            protected internal override PdfWriter GetNextPdfWriter(PageRange documentPageRange) {
                try {
                    return new PdfWriter(PdfSplitterTest.destinationFolder + "splitDocument2_" + (this.partNumber++).ToString(
                        ) + ".pdf");
                }
                catch (FileNotFoundException) {
                    throw new Exception();
                }
            }
        }

        private sealed class _IDocumentReadyListener_126 : PdfSplitter.IDocumentReadyListener {
            public _IDocumentReadyListener_126() {
            }

            public void DocumentReady(PdfDocument pdfDocument, PageRange pageRange) {
                if (new PageRange("61-120").Equals(pageRange)) {
                    pdfDocument.GetDocumentInfo().SetAuthor("Modified Author");
                }
                pdfDocument.Close();
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY, Count = 2)]
        public virtual void SplitDocumentTest03() {
            String inputFileName = sourceFolder + "iphone_user_guide.pdf";
            PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
            PageRange pageRange1 = new PageRange().AddPageSequence(4, 15).AddSinglePage(18).AddPageSequence(1, 2);
            PageRange pageRange2 = new PageRange().AddSinglePage(99).AddSinglePage(98).AddPageSequence(70, 99);
            IList<PdfDocument> splitDocuments = new _PdfSplitter_154(inputPdfDoc).ExtractPageRanges(JavaUtil.ArraysAsList
                (pageRange1, pageRange2));
            foreach (PdfDocument pdfDocument in splitDocuments) {
                pdfDocument.Close();
            }
            for (int i = 1; i <= 2; i++) {
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "splitDocument3_" + i
                     + ".pdf", sourceFolder + "cmp/" + "cmp_splitDocument3_" + i.ToString() + ".pdf", destinationFolder, "diff_"
                    ));
            }
        }

        private sealed class _PdfSplitter_154 : PdfSplitter {
            public _PdfSplitter_154(PdfDocument baseArg1)
                : base(baseArg1) {
                this.partNumber = 1;
            }

            internal int partNumber;

            protected internal override PdfWriter GetNextPdfWriter(PageRange documentPageRange) {
                try {
                    return new PdfWriter(PdfSplitterTest.destinationFolder + "splitDocument3_" + (this.partNumber++).ToString(
                        ) + ".pdf");
                }
                catch (FileNotFoundException) {
                    throw new Exception();
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY, Count = 2)]
        public virtual void SplitDocumentTest04() {
            String inputFileName = sourceFolder + "iphone_user_guide.pdf";
            PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
            PageRange pageRange1 = new PageRange("even & 80-").AddPageSequence(4, 15).AddSinglePage(18).AddPageSequence
                (1, 2);
            PageRange pageRange2 = new PageRange("99,98").AddPageSequence(70, 99);
            IList<PdfDocument> splitDocuments = new _PdfSplitter_188(inputPdfDoc).ExtractPageRanges(JavaUtil.ArraysAsList
                (pageRange1, pageRange2));
            foreach (PdfDocument pdfDocument in splitDocuments) {
                pdfDocument.Close();
            }
            for (int i = 1; i <= 2; i++) {
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "splitDocument4_" + i
                     + ".pdf", sourceFolder + "cmp/" + "cmp_splitDocument4_" + i.ToString() + ".pdf", destinationFolder, "diff_"
                    ));
            }
        }

        private sealed class _PdfSplitter_188 : PdfSplitter {
            public _PdfSplitter_188(PdfDocument baseArg1)
                : base(baseArg1) {
                this.partNumber = 1;
            }

            internal int partNumber;

            protected internal override PdfWriter GetNextPdfWriter(PageRange documentPageRange) {
                try {
                    return new PdfWriter(PdfSplitterTest.destinationFolder + "splitDocument4_" + (this.partNumber++).ToString(
                        ) + ".pdf");
                }
                catch (FileNotFoundException) {
                    throw new Exception();
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY, Count = 2)]
        public virtual void SplitDocumentByOutlineTest() {
            String inputFileName = sourceFolder + "iphone_user_guide.pdf";
            PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
            PdfSplitter splitter = new PdfSplitter(inputPdfDoc);
            IList<String> listTitles = new List<String>();
            listTitles.Add("Syncing iPod Content from Your iTunes Library");
            listTitles.Add("Restoring or Transferring Your iPhone Settings");
            IList<PdfDocument> list = splitter.SplitByOutlines(listTitles);
            NUnit.Framework.Assert.AreEqual(1, list[0].GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(2, list[1].GetNumberOfPages());
            list[0].Close();
            list[1].Close();
        }

        [NUnit.Framework.Test]
        public virtual void SplitDocumentBySize() {
            String inputFileName = sourceFolder + "splitBySize.pdf";
            PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName));
            PdfSplitter splitter = new _PdfSplitter_234(inputPdfDoc);
            IList<PdfDocument> documents = splitter.SplitBySize(100000);
            foreach (PdfDocument doc in documents) {
                doc.Close();
            }
            for (int i = 1; i <= 4; ++i) {
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "splitBySize_part" + 
                    i + ".pdf", sourceFolder + "cmp/" + "cmp_splitBySize_part" + i + ".pdf", destinationFolder, "diff_"));
            }
        }

        private sealed class _PdfSplitter_234 : PdfSplitter {
            public _PdfSplitter_234(PdfDocument baseArg1)
                : base(baseArg1) {
                this.partNumber = 1;
            }

            internal int partNumber;

            protected internal override PdfWriter GetNextPdfWriter(PageRange documentPageRange) {
                try {
                    return new PdfWriter(PdfSplitterTest.destinationFolder + "splitBySize_part" + (this.partNumber++).ToString
                        () + ".pdf");
                }
                catch (FileNotFoundException) {
                    throw new Exception();
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY, Count = 10)]
        public virtual void SplitByPageCountTest() {
            String inputFileName = sourceFolder + "iphone_user_guide.pdf";
            using (PdfDocument inputPdfDoc = new PdfDocument(new PdfReader(inputFileName))) {
                PdfSplitter splitter = new PdfSplitter(inputPdfDoc);
                int pagesCount = inputPdfDoc.GetNumberOfPages();
                int pagesCountInSplitDoc = 13;
                IList<PdfDocument> splitDocuments = splitter.SplitByPageCount(pagesCountInSplitDoc);
                foreach (PdfDocument doc in splitDocuments) {
                    doc.Close();
                }
                NUnit.Framework.Assert.AreEqual(pagesCount / pagesCountInSplitDoc, splitDocuments.Count);
            }
        }
    }
}
