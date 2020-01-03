/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Utils {
    public class PdfMergerTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/PdfMergerTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/utils/PdfMergerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentTest01() {
            String filename = sourceFolder + "courierTest.pdf";
            String filename1 = sourceFolder + "helveticaTest.pdf";
            String filename2 = sourceFolder + "timesRomanTest.pdf";
            String resultFile = destinationFolder + "mergedResult01.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfReader reader1 = new PdfReader(filename1);
            PdfReader reader2 = new PdfReader(filename2);
            FileStream fos1 = new FileStream(resultFile, FileMode.Create);
            PdfWriter writer1 = new PdfWriter(fos1);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDocument pdfDoc1 = new PdfDocument(reader1);
            PdfDocument pdfDoc2 = new PdfDocument(reader2);
            PdfDocument pdfDoc3 = new PdfDocument(writer1);
            PdfMerger merger = new PdfMerger(pdfDoc3).SetCloseSourceDocuments(true);
            merger.Merge(pdfDoc, 1, 1);
            merger.Merge(pdfDoc1, 1, 1);
            merger.Merge(pdfDoc2, 1, 1);
            pdfDoc3.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(resultFile, sourceFolder + "cmp_mergedResult01.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void MergeDocumentOutlinesWithNullDestinationTest01() {
            String resultFile = destinationFolder + "mergeDocumentOutlinesWithNullDestinationTest01.pdf";
            String filename = sourceFolder + "null_dest_outline.pdf";
            PdfDocument sourceDocument = new PdfDocument(new PdfReader(filename));
            PdfMerger resultDocument = new PdfMerger(new PdfDocument(new PdfWriter(resultFile)));
            resultDocument.Merge(sourceDocument, 1, 1);
            resultDocument.Close();
            sourceDocument.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(resultFile, sourceFolder + "cmp_mergeDocumentOutlinesWithNullDestinationTest01.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentTest02() {
            String filename = sourceFolder + "doc1.pdf";
            String filename1 = sourceFolder + "doc2.pdf";
            String filename2 = sourceFolder + "doc3.pdf";
            String resultFile = destinationFolder + "mergedResult02.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfReader reader1 = new PdfReader(filename1);
            PdfReader reader2 = new PdfReader(filename2);
            FileStream fos1 = new FileStream(resultFile, FileMode.Create);
            PdfWriter writer1 = new PdfWriter(fos1);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDocument pdfDoc1 = new PdfDocument(reader1);
            PdfDocument pdfDoc2 = new PdfDocument(reader2);
            PdfDocument pdfDoc3 = new PdfDocument(writer1);
            PdfMerger merger = new PdfMerger(pdfDoc3).SetCloseSourceDocuments(true);
            merger.Merge(pdfDoc, 1, 1).Merge(pdfDoc1, 1, 1).Merge(pdfDoc2, 1, 1).Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(resultFile, sourceFolder + "cmp_mergedResult02.pdf", destinationFolder
                , "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void MergeDocumentTest03() {
            String filename = sourceFolder + "pdf_open_parameters.pdf";
            String filename1 = sourceFolder + "iphone_user_guide.pdf";
            String resultFile = destinationFolder + "mergedResult03.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfReader reader1 = new PdfReader(filename1);
            FileStream fos1 = new FileStream(resultFile, FileMode.Create);
            PdfWriter writer1 = new PdfWriter(fos1);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDocument pdfDoc1 = new PdfDocument(reader1);
            PdfDocument pdfDoc3 = new PdfDocument(writer1);
            pdfDoc3.SetTagged();
            new PdfMerger(pdfDoc3).Merge(pdfDoc, 2, 2).Merge(pdfDoc1, 7, 8).Close();
            pdfDoc.Close();
            pdfDoc1.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = "";
            String contentErrorMessage = compareTool.CompareByContent(resultFile, sourceFolder + "cmp_mergedResult03.pdf"
                , destinationFolder, "diff_");
            String tagStructErrorMessage = compareTool.CompareTagStructures(resultFile, sourceFolder + "cmp_mergedResult03.pdf"
                );
            errorMessage += tagStructErrorMessage == null ? "" : tagStructErrorMessage + "\n";
            errorMessage += contentErrorMessage == null ? "" : contentErrorMessage;
            if (!String.IsNullOrEmpty(errorMessage)) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        [LogMessage(iText.IO.LogMessageConstant.CREATED_ROOT_TAG_HAS_MAPPING, Count = 2)]
        public virtual void MergeDocumentTest04() {
            String filename = sourceFolder + "pdf_open_parameters.pdf";
            String filename1 = sourceFolder + "iphone_user_guide.pdf";
            String resultFile = destinationFolder + "mergedResult04.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfReader reader1 = new PdfReader(filename1);
            FileStream fos1 = new FileStream(resultFile, FileMode.Create);
            PdfWriter writer1 = new PdfWriter(fos1);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDocument pdfDoc1 = new PdfDocument(reader1);
            PdfDocument pdfDoc3 = new PdfDocument(writer1);
            pdfDoc3.SetTagged();
            PdfMerger merger = new PdfMerger(pdfDoc3).SetCloseSourceDocuments(true);
            IList<int> pages = new List<int>();
            pages.Add(3);
            pages.Add(2);
            pages.Add(1);
            merger.Merge(pdfDoc, pages);
            IList<int> pages1 = new List<int>();
            pages1.Add(5);
            pages1.Add(9);
            pages1.Add(4);
            pages1.Add(3);
            merger.Merge(pdfDoc1, pages1);
            merger.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = "";
            String contentErrorMessage = compareTool.CompareByContent(resultFile, sourceFolder + "cmp_mergedResult04.pdf"
                , destinationFolder, "diff_");
            String tagStructErrorMessage = compareTool.CompareTagStructures(resultFile, sourceFolder + "cmp_mergedResult04.pdf"
                );
            errorMessage += tagStructErrorMessage == null ? "" : tagStructErrorMessage + "\n";
            errorMessage += contentErrorMessage == null ? "" : contentErrorMessage;
            if (!String.IsNullOrEmpty(errorMessage)) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MergeTableWithEmptyTdTest() {
            String filename = sourceFolder + "tableWithEmptyTd.pdf";
            String resultFile = destinationFolder + "tableWithEmptyTdResult.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument sourceDoc = new PdfDocument(reader);
            PdfDocument output = new PdfDocument(new PdfWriter(resultFile));
            output.SetTagged();
            PdfMerger merger = new PdfMerger(output).SetCloseSourceDocuments(true);
            merger.Merge(sourceDoc, 1, sourceDoc.GetNumberOfPages());
            sourceDoc.Close();
            reader.Close();
            merger.Close();
            output.Close();
            CompareTool compareTool = new CompareTool();
            String tagStructErrorMessage = compareTool.CompareTagStructures(resultFile, sourceFolder + "cmp_tableWithEmptyTd.pdf"
                );
            String errorMessage = tagStructErrorMessage == null ? "" : tagStructErrorMessage + "\n";
            if (!String.IsNullOrEmpty(errorMessage)) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.NAME_ALREADY_EXISTS_IN_THE_NAME_TREE, Count = 2)]
        public virtual void MergeOutlinesNamedDestinations() {
            String filename = sourceFolder + "outlinesNamedDestinations.pdf";
            String resultFile = destinationFolder + "mergeOutlinesNamedDestinations.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument sourceDoc = new PdfDocument(reader);
            PdfDocument output = new PdfDocument(new PdfWriter(resultFile));
            PdfMerger merger = new PdfMerger(output).SetCloseSourceDocuments(false);
            merger.Merge(sourceDoc, 2, 3);
            merger.Merge(sourceDoc, 2, 3);
            sourceDoc.Close();
            reader.Close();
            merger.Close();
            output.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(resultFile, sourceFolder + "cmp_mergeOutlinesNamedDestinations.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
