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
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfObjectReleaseTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfObjectReleaseTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfObjectReleaseTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FORBID_RELEASE_IS_SET, Count = 108)]
        public virtual void ReleaseObjectsInDocWithStructTreeRootTest() {
            SinglePdfObjectReleaseTest("releaseObjectsInDocWithStructTreeRoot.pdf", "releaseObjectsInDocWithStructTreeRoot_stamping.pdf"
                , "releaseObjectsInDocWithStructTreeRoot_stamping_release.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FORBID_RELEASE_IS_SET, Count = 5)]
        public virtual void ReleaseObjectsInDocWithXfaTest() {
            SinglePdfObjectReleaseTest("releaseObjectsInDocWithXfa.pdf", "releaseObjectsInDocWithXfa_stamping.pdf", "releaseObjectsInDocWithXfa_stamping_release.pdf"
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FORBID_RELEASE_IS_SET, Count = 3)]
        public virtual void ReleaseObjectsInSimpleDocTest() {
            SinglePdfObjectReleaseTest("releaseObjectsInSimpleDoc.pdf", "releaseObjectsInSimpleDoc_stamping.pdf", "releaseObjectsInSimpleDoc_stamping_release.pdf"
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FORBID_RELEASE_IS_SET)]
        public virtual void ReleaseCatalogTest() {
            String srcFile = SOURCE_FOLDER + "releaseObjectsInSimpleDoc.pdf";
            String release = DESTINATION_FOLDER + "outReleaseObjectsInSimpleDoc.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(release))) {
                doc.GetCatalog().GetPdfObject().Release();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(release, srcFile, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FORBID_RELEASE_IS_SET)]
        public virtual void ReleasePagesTest() {
            String srcFile = SOURCE_FOLDER + "releaseObjectsInSimpleDoc.pdf";
            String release = DESTINATION_FOLDER + "outReleaseObjectsInSimpleDoc.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(release))) {
                doc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Pages).Release();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(release, srcFile, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FORBID_RELEASE_IS_SET)]
        public virtual void ReleaseStructTreeRootTest() {
            String srcFile = SOURCE_FOLDER + "releaseObjectsInDocWithStructTreeRoot.pdf";
            String release = DESTINATION_FOLDER + "outReleaseObjectsInDocWithStructTreeRoot.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(release))) {
                doc.GetStructTreeRoot().GetPdfObject().Release();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(release, srcFile, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FORBID_RELEASE_IS_SET)]
        public virtual void ReleaseModifiedObjectTest() {
            String srcFile = SOURCE_FOLDER + "releaseModifiedObject.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_releaseModifiedObject.pdf";
            String outFile = DESTINATION_FOLDER + "releaseModifiedObject.pdf";
            using (PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(outFile))) {
                PdfAnnotation annots = doc.GetPage(1).GetAnnotations()[0];
                annots.SetRectangle(new PdfArray(new Rectangle(100, 100, 80, 50)));
                annots.GetPdfObject().Release();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void AddingReleasedObjectToDocumentTest() {
            String srcFile = SOURCE_FOLDER + "releaseObjectsInSimpleDoc.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(DESTINATION_FOLDER + "addingReleasedObjectToDocument.pdf"
                ));
            try {
                PdfObject releasedObj = doc.GetPdfObject(1);
                releasedObj.Release();
                doc.GetCatalog().Put(PdfName.Outlines, releasedObj);
            }
            finally {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => doc.Close());
                NUnit.Framework.Assert.AreEqual("Cannot write object after it was released." + " In normal situation the object must be read once again before being written."
                    , e.Message);
            }
        }

        private void SinglePdfObjectReleaseTest(String inputFilename, String outStampingFilename, String outStampingReleaseFilename
            ) {
            String srcFile = SOURCE_FOLDER + inputFilename;
            String outPureStamping = DESTINATION_FOLDER + outStampingFilename;
            String outStampingRelease = DESTINATION_FOLDER + outStampingReleaseFilename;
            PdfDocument doc = new PdfDocument(new PdfReader(srcFile), new PdfWriter(outPureStamping));
            // We open/close document to make sure that the results of release logic and simple overwriting coincide.
            doc.Close();
            PdfDocument stamperRelease = new PdfDocument(new PdfReader(srcFile), new PdfWriter(outStampingRelease));
            for (int i = 0; i < stamperRelease.GetNumberOfPdfObjects(); i++) {
                PdfObject pdfObject = stamperRelease.GetPdfObject(i);
                if (pdfObject != null) {
                    stamperRelease.GetPdfObject(i).Release();
                }
            }
            stamperRelease.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outStampingRelease, outPureStamping, DESTINATION_FOLDER
                ));
        }
    }
}
