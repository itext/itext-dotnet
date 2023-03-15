/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfCopyTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfCopyTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfCopyTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.MAKE_COPY_OF_CATALOG_DICTIONARY_IS_FORBIDDEN)]
        public virtual void CopySignedDocuments() {
            PdfDocument pdfDoc1 = new PdfDocument(new PdfReader(sourceFolder + "hello_signed.pdf"));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(destinationFolder + "copySignedDocuments.pdf"));
            pdfDoc1.CopyPagesTo(1, 1, pdfDoc2);
            pdfDoc2.Close();
            pdfDoc1.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(destinationFolder + "copySignedDocuments.pdf"));
            PdfDictionary sig = (PdfDictionary)pdfDocument.GetPdfObject(13);
            PdfDictionary sigRef = sig.GetAsArray(PdfName.Reference).GetAsDictionary(0);
            NUnit.Framework.Assert.IsTrue(PdfName.SigRef.Equals(sigRef.GetAsName(PdfName.Type)));
            NUnit.Framework.Assert.IsTrue(sigRef.Get(PdfName.Data).IsNull());
        }

        [NUnit.Framework.Test]
        public virtual void Copying1() {
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(destinationFolder + "copying1_1.pdf"));
            pdfDoc1.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").SetTitle("Empty iText 6 Document"
                );
            pdfDoc1.GetCatalog().Put(new PdfName("a"), new PdfName("b").MakeIndirect(pdfDoc1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.Flush();
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(new PdfReader(destinationFolder + "copying1_1.pdf"));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(destinationFolder + "copying1_2.pdf"));
            pdfDoc2.AddNewPage();
            pdfDoc2.GetDocumentInfo().GetPdfObject().Put(new PdfName("a"), pdfDoc1.GetCatalog().GetPdfObject().Get(new 
                PdfName("a")).CopyTo(pdfDoc2));
            pdfDoc2.Close();
            pdfDoc1.Close();
            PdfReader reader = new PdfReader(destinationFolder + "copying1_2.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary trailer = pdfDocument.GetTrailer();
            PdfDictionary info = trailer.GetAsDictionary(PdfName.Info);
            PdfName b = info.GetAsName(new PdfName("a"));
            NUnit.Framework.Assert.AreEqual("/b", b.ToString());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Copying2() {
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(destinationFolder + "copying2_1.pdf"));
            for (int i = 0; i < 10; i++) {
                PdfPage page1 = pdfDoc1.AddNewPage();
                page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + (i + 1).ToString() + "\n"
                    ));
                page1.Flush();
            }
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(new PdfReader(destinationFolder + "copying2_1.pdf"));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(destinationFolder + "copying2_2.pdf"));
            for (int i = 0; i < 10; i++) {
                if (i % 2 == 0) {
                    pdfDoc2.AddPage(pdfDoc1.GetPage(i + 1).CopyTo(pdfDoc2));
                }
            }
            pdfDoc2.Close();
            pdfDoc1.Close();
            PdfReader reader = new PdfReader(destinationFolder + "copying2_2.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 0; i < 5; i++) {
                byte[] bytes = pdfDocument.GetPage(i + 1).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + (i * 2 + 1).ToString() + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes
                    (bytes));
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Copying3() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "copying3_1.pdf"));
            PdfDictionary helloWorld = (PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc);
            PdfDictionary helloWorld1 = (PdfDictionary)new PdfDictionary().MakeIndirect(pdfDoc);
            helloWorld.Put(new PdfName("Hello"), new PdfString("World"));
            helloWorld.Put(new PdfName("HelloWrld"), helloWorld);
            helloWorld.Put(new PdfName("HelloWrld1"), helloWorld1);
            PdfPage page = pdfDoc.AddNewPage();
            page.GetPdfObject().Put(new PdfName("HelloWorld"), helloWorld);
            page.GetPdfObject().Put(new PdfName("HelloWorldClone"), (PdfObject)helloWorld.Clone());
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + "copying3_1.pdf");
            pdfDoc = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary dic0 = pdfDoc.GetPage(1).GetPdfObject().GetAsDictionary(new PdfName("HelloWorld"));
            NUnit.Framework.Assert.AreEqual(4, dic0.GetIndirectReference().GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, dic0.GetIndirectReference().GetGenNumber());
            PdfDictionary dic1 = pdfDoc.GetPage(1).GetPdfObject().GetAsDictionary(new PdfName("HelloWorldClone"));
            NUnit.Framework.Assert.AreEqual(8, dic1.GetIndirectReference().GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, dic1.GetIndirectReference().GetGenNumber());
            PdfString str0 = dic0.GetAsString(new PdfName("Hello"));
            PdfString str1 = dic1.GetAsString(new PdfName("Hello"));
            NUnit.Framework.Assert.AreEqual(str0.GetValue(), str1.GetValue());
            NUnit.Framework.Assert.AreEqual(str0.GetValue(), "World");
            PdfDictionary dic01 = dic0.GetAsDictionary(new PdfName("HelloWrld"));
            PdfDictionary dic11 = dic1.GetAsDictionary(new PdfName("HelloWrld"));
            NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetObjNumber(), dic11.GetIndirectReference().
                GetObjNumber());
            NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetGenNumber(), dic11.GetIndirectReference().
                GetGenNumber());
            NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetObjNumber(), 4);
            NUnit.Framework.Assert.AreEqual(dic01.GetIndirectReference().GetGenNumber(), 0);
            PdfDictionary dic02 = dic0.GetAsDictionary(new PdfName("HelloWrld1"));
            PdfDictionary dic12 = dic1.GetAsDictionary(new PdfName("HelloWrld1"));
            NUnit.Framework.Assert.AreEqual(dic02.GetIndirectReference().GetObjNumber(), dic12.GetIndirectReference().
                GetObjNumber());
            NUnit.Framework.Assert.AreEqual(dic02.GetIndirectReference().GetGenNumber(), dic12.GetIndirectReference().
                GetGenNumber());
            NUnit.Framework.Assert.AreEqual(dic12.GetIndirectReference().GetObjNumber(), 5);
            NUnit.Framework.Assert.AreEqual(dic12.GetIndirectReference().GetGenNumber(), 0);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void CopyDocumentsWithFormFieldsTest() {
            String filename = sourceFolder + "fieldsOn2-sPage.pdf";
            PdfDocument sourceDoc = new PdfDocument(new PdfReader(filename));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "copyDocumentsWithFormFields.pdf"));
            sourceDoc.InitializeOutlines();
            sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), pdfDoc);
            sourceDoc.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "copyDocumentsWithFormFields.pdf"
                , sourceFolder + "cmp_copyDocumentsWithFormFields.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopySamePageWithAnnotationsSeveralTimes() {
            String filename = sourceFolder + "rotated_annotation.pdf";
            PdfDocument sourceDoc = new PdfDocument(new PdfReader(filename));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "copySamePageWithAnnotationsSeveralTimes.pdf"
                ));
            sourceDoc.InitializeOutlines();
            sourceDoc.CopyPagesTo(JavaUtil.ArraysAsList(1, 1, 1), pdfDoc);
            sourceDoc.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "copySamePageWithAnnotationsSeveralTimes.pdf"
                , sourceFolder + "cmp_copySamePageWithAnnotationsSeveralTimes.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyIndirectInheritablePageEntriesTest01() {
            String src = sourceFolder + "indirectPageProps.pdf";
            String filename = "copyIndirectInheritablePageEntriesTest01.pdf";
            String dest = destinationFolder + filename;
            String cmp = sourceFolder + "cmp_" + filename;
            PdfDocument outputDoc = new PdfDocument(new PdfWriter(dest));
            PdfDocument sourceDoc = new PdfDocument(new PdfReader(src));
            sourceDoc.CopyPagesTo(1, 1, outputDoc);
            sourceDoc.Close();
            outputDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyPageNoRotationToDocWithRotationInKidsPageTest() {
            String src = sourceFolder + "srcFileWithSetRotation.pdf";
            String dest = destinationFolder + "copyPageNoRotationToDocWithRotationInKidsPage.pdf";
            String cmp = sourceFolder + "cmp_copyPageNoRotationToDocWithRotationInKidsPage.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfDocument sourceDoc = new PdfDocument(new PdfReader(sourceFolder + "noRotationProp.pdf"));
            sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), pdfDoc);
            sourceDoc.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void CopyPageNoRotationToDocWithRotationInPagesDictTest() {
            //TODO: update cmp-files when DEVSIX-3635 will be fixed
            String src = sourceFolder + "indirectPageProps.pdf";
            String dest = destinationFolder + "copyPageNoRotationToDocWithRotationInPagesDict.pdf";
            String cmp = sourceFolder + "cmp_copyPageNoRotationToDocWithRotationInPagesDict.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfDocument sourceDoc = new PdfDocument(new PdfReader(sourceFolder + "noRotationProp.pdf"));
            sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), pdfDoc);
            sourceDoc.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void CopyPageWithRotationInPageToDocWithRotationInPagesDictTest() {
            String src = sourceFolder + "indirectPageProps.pdf";
            String dest = destinationFolder + "copyPageWithRotationInPageToDocWithRotationInPagesDict.pdf";
            String cmp = sourceFolder + "cmp_copyPageWithRotationInPageToDocWithRotationInPagesDict.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            PdfDocument sourceDoc = new PdfDocument(new PdfReader(sourceFolder + "srcFileCopyPageWithSetRotationValueInKids.pdf"
                ));
            sourceDoc.CopyPagesTo(1, sourceDoc.GetNumberOfPages(), pdfDoc);
            sourceDoc.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void CopySelfContainedObject() {
            ByteArrayOutputStream inputBytes = new ByteArrayOutputStream();
            PdfDocument prepInputDoc = new PdfDocument(new PdfWriter(inputBytes));
            PdfDictionary selfContainedDict = new PdfDictionary();
            PdfName randDictName = PdfName.Sound;
            PdfName randEntry1 = PdfName.R;
            PdfName randEntry2 = PdfName.S;
            selfContainedDict.Put(randEntry1, selfContainedDict);
            selfContainedDict.Put(randEntry2, selfContainedDict);
            prepInputDoc.AddNewPage().Put(randDictName, selfContainedDict.MakeIndirect(prepInputDoc));
            prepInputDoc.Close();
            PdfDocument srcDoc = new PdfDocument(new PdfReader(new MemoryStream(inputBytes.ToArray())));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destinationFolder + "copySelfContainedObject.pdf"));
            srcDoc.CopyPagesTo(1, 1, destDoc);
            PdfDictionary destPageObj = destDoc.GetFirstPage().GetPdfObject();
            PdfDictionary destSelfContainedDict = destPageObj.GetAsDictionary(randDictName);
            PdfDictionary destSelfContainedDictR = destSelfContainedDict.GetAsDictionary(randEntry1);
            PdfDictionary destSelfContainedDictS = destSelfContainedDict.GetAsDictionary(randEntry2);
            NUnit.Framework.Assert.AreEqual(destSelfContainedDict.GetIndirectReference(), destSelfContainedDictR.GetIndirectReference
                ());
            NUnit.Framework.Assert.AreEqual(destSelfContainedDict.GetIndirectReference(), destSelfContainedDictS.GetIndirectReference
                ());
            destDoc.Close();
            srcDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CopyDifferentRangesOfPagesWithBookmarksTest() {
            String outFileName = destinationFolder + "copyDifferentRangesOfPagesWithBookmarksTest.pdf";
            String cmpFileName = sourceFolder + "cmp_copyDifferentRangesOfPagesWithBookmarksTest.pdf";
            PdfDocument targetPdf = new PdfDocument(new PdfWriter(outFileName));
            targetPdf.InitializeOutlines();
            PdfDocument sourcePdf = new PdfDocument(new PdfReader(sourceFolder + "sameDocWithBookmarksPdf.pdf"));
            sourcePdf.InitializeOutlines();
            int sourcePdfLength = sourcePdf.GetNumberOfPages();
            int sourcePdfOutlines = sourcePdf.GetOutlines(false).GetAllChildren().Count;
            sourcePdf.CopyPagesTo(3, sourcePdfLength, targetPdf);
            sourcePdf.CopyPagesTo(1, 2, targetPdf);
            int targetOutlines = targetPdf.GetOutlines(false).GetAllChildren().Count;
            NUnit.Framework.Assert.AreEqual(sourcePdfOutlines, targetOutlines);
            sourcePdf.Close();
            targetPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesLinkAnnotationTest() {
            // TODO DEVSIX-577. Update cmp
            String outFileName = destinationFolder + "copyPagesLinkAnnotationTest.pdf";
            String cmpFileName = sourceFolder + "cmp_copyPagesLinkAnnotationTest.pdf";
            PdfDocument targetPdf = new PdfDocument(new PdfWriter(outFileName));
            PdfDocument linkAnotPdf = new PdfDocument(new PdfReader(sourceFolder + "pdfLinkAnnotationTest.pdf"));
            int linkPdfLength = linkAnotPdf.GetNumberOfPages();
            linkAnotPdf.CopyPagesTo(3, linkPdfLength, targetPdf);
            linkAnotPdf.CopyPagesTo(1, 2, targetPdf);
            IList<PdfAnnotation> annotations = GetPdfAnnotations(targetPdf);
            NUnit.Framework.Assert.AreEqual(0, annotations.Count, "The number of merged annotations are not the same."
                );
            linkAnotPdf.Close();
            targetPdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        private IList<PdfAnnotation> GetPdfAnnotations(PdfDocument pdfDoc) {
            int number = pdfDoc.GetNumberOfPages();
            List<PdfAnnotation> annotations = new List<PdfAnnotation>();
            for (int i = 1; i <= number; i++) {
                annotations.AddAll(pdfDoc.GetPage(i).GetAnnotations());
            }
            return annotations;
        }
    }
}
