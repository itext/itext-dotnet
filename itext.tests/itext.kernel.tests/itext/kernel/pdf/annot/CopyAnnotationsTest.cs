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
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CopyAnnotationsTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/annot" + "/CopyAnnotationsTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/annot/CopyAnnotationsTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CopyGoToRDestinationTest() {
            String outFile = DESTINATION_FOLDER + "CopiedGoToRAnnotation.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "GoToRAnnotation.pdf"))) {
                    input.CopyPagesTo(1, input.GetNumberOfPages(), @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 1);
            NUnit.Framework.Assert.AreEqual(1, annotations.Count, "Destination is not copied");
        }

        [NUnit.Framework.Test]
        public virtual void CopyMultipleGoToRDestinationTest() {
            String outFile = DESTINATION_FOLDER + "CopiedMultiGoToRAnnotation.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "MultiDestinations.pdf"))) {
                    input.CopyPagesTo(1, input.GetNumberOfPages(), @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 1);
            NUnit.Framework.Assert.AreEqual(2, annotations.Count, "Not all destinations are copied");
        }

        [NUnit.Framework.Test]
        public virtual void CopyGoToRWithoutTargetTest() {
            String outFile = DESTINATION_FOLDER + "CopiedGoToRNoTarget.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "namedDest.pdf"))) {
                    input.CopyPagesTo(2, 6, @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 5);
            NUnit.Framework.Assert.IsTrue(annotations.IsEmpty(), "Destinations are copied but should not");
        }

        [NUnit.Framework.Test]
        public virtual void CopyGoToRNamedDestinationTest() {
            String outFile = DESTINATION_FOLDER + "CopiedGoToRNamedDest.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "namedDest.pdf"))) {
                    input.CopyPagesTo(1, 6, @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 6);
            NUnit.Framework.Assert.IsFalse(annotations.IsEmpty(), "Annotation is copied");
            String destination = (annotations[0]).GetPdfObject().Get(PdfName.Dest).ToString();
            NUnit.Framework.Assert.AreEqual("Destination_1", destination, "Destination is different from expected");
        }

        [NUnit.Framework.Test]
        public virtual void FileAttachmentTargetTest() {
            String outFile = DESTINATION_FOLDER + "CopiedFileAttachmentTarget.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "fileAttachmentTargetTest.pdf"))) {
                    input.CopyPagesTo(1, input.GetNumberOfPages(), @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 2);
            NUnit.Framework.Assert.IsFalse(annotations.IsEmpty(), "Annotation is not copied");
            String nm = annotations[0].GetPdfObject().GetAsString(PdfName.NM).ToString();
            NUnit.Framework.Assert.AreEqual("FileAttachmentAnnotation1", nm, "File attachment name is different from expected"
                );
        }

        [NUnit.Framework.Test]
        public virtual void CopyLinkWidgetTest() {
            String outFile = DESTINATION_FOLDER + "CopiedLinkWidget.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "LinkWidgetExplicitDestination.pdf"
                    ))) {
                    input.CopyPagesTo(1, input.GetNumberOfPages(), @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 1);
            NUnit.Framework.Assert.IsFalse(annotations.IsEmpty(), "Annotation is not copied");
            NUnit.Framework.Assert.AreEqual(PdfName.Widget, annotations[0].GetSubtype(), "Annotation is of a different subtype"
                );
        }

        [NUnit.Framework.Test]
        public virtual void NoPdfNameATest() {
            String outFile = DESTINATION_FOLDER + "CopiedNoPdfNameA.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "GoToRAnnotation.pdf"))) {
                    PdfAnnotation pdfAnnotation = input.GetPage(1).GetAnnotations()[0];
                    pdfAnnotation.GetPdfObject().Remove(PdfName.A);
                    input.CopyPagesTo(1, input.GetNumberOfPages(), @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 1);
            NUnit.Framework.Assert.IsFalse(annotations.IsEmpty(), "Annotation is not copied");
        }

        [NUnit.Framework.Test]
        public virtual void NoPdfNameDTest() {
            String outFile = DESTINATION_FOLDER + "CopiedNoPdfNameD.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "GoToRAnnotation.pdf"))) {
                    PdfAnnotation pdfAnnotation = input.GetPage(1).GetAnnotations()[0];
                    pdfAnnotation.GetPdfObject().GetAsDictionary(PdfName.A).Remove(PdfName.D);
                    input.CopyPagesTo(1, input.GetNumberOfPages(), @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 1);
            NUnit.Framework.Assert.IsFalse(annotations.IsEmpty(), "Annotation is not copied");
        }

        [NUnit.Framework.Test]
        public virtual void NoPdfNameSTest() {
            String outFile = DESTINATION_FOLDER + "CopiedNoPdfNameS.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "GoToRAnnotation.pdf"))) {
                    PdfAnnotation pdfAnnotation = input.GetPage(1).GetAnnotations()[0];
                    pdfAnnotation.GetPdfObject().GetAsDictionary(PdfName.A).Remove(PdfName.S);
                    input.CopyPagesTo(1, input.GetNumberOfPages(), @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 1);
            NUnit.Framework.Assert.IsTrue(annotations.IsEmpty(), "Annotation is copied");
        }

        [NUnit.Framework.Test]
        public virtual void NoPdfNameDWithGoToRTest() {
            String outFile = DESTINATION_FOLDER + "CopiedNoPdfNameDGoToR.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "GoToRAnnotation.pdf"))) {
                    PdfAnnotation pdfAnnotation = input.GetPage(1).GetAnnotations()[0];
                    PdfDictionary aDictionary = pdfAnnotation.GetPdfObject().GetAsDictionary(PdfName.A);
                    aDictionary.Remove(PdfName.D);
                    aDictionary.Remove(PdfName.S);
                    aDictionary.Put(PdfName.S, PdfName.GoToR);
                    input.CopyPagesTo(1, input.GetNumberOfPages(), @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 1);
            NUnit.Framework.Assert.IsFalse(annotations.IsEmpty(), "Annotation is not copied");
        }

        [NUnit.Framework.Test]
        public virtual void LinkInsideArray() {
            String outFile = DESTINATION_FOLDER + "CopiedLinkInArray.pdf";
            using (PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile))) {
                using (PdfDocument input = new PdfDocument(new PdfReader(SOURCE_FOLDER + "LinkInArray.pdf"))) {
                    input.CopyPagesTo(1, 1, @out);
                }
            }
            IList<PdfAnnotation> annotations = GetAnnotationsFromPdf(outFile, 1);
            NUnit.Framework.Assert.IsTrue(annotations.IsEmpty(), "Annotation is copied");
        }

        private IList<PdfAnnotation> GetAnnotationsFromPdf(String outFilePath, int pageNumber) {
            IList<PdfAnnotation> annotations;
            using (PdfDocument result = new PdfDocument(CompareTool.CreateOutputReader(outFilePath))) {
                annotations = result.GetPage(pageNumber).GetAnnotations();
            }
            return annotations;
        }
    }
}
