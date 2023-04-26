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
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Navigation;
using iText.Test;

namespace iText.Kernel.Pdf.Copy {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfDestinationCopyingTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfDestinationCopyingTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf" + "/PdfDestinationCopyingTest/";

        public static readonly String TARGET_DOC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfDestinationCopyingTest" + "/Target.pdf";

        public static readonly String SOURCE_ANNOTATION_WITH_DESTINATION_EXPLICIT = SOURCE_FOLDER + "LinkAnnotationViaDestExplicitDestination.pdf";

        public static readonly String SOURCE_ANNOTATION_WITH_DESTINATION_NAMED = SOURCE_FOLDER + "LinkAnnotationViaDestNamedDestination.pdf";

        public static readonly String SOURCE_ANNOTATION_VIA_ACTION_WITH_DESTINATION_EXPLICIT = SOURCE_FOLDER + "LinkAnnotationViaActionExplicitDestination.pdf";

        public static readonly String SOURCE_ANNOTATION_VIA_ACTION_WITH_DESTINATION_NAMED = SOURCE_FOLDER + "LinkAnnotationViaActionNamedDestination.pdf";

        public static readonly String SOURCE_ANNOTATION_VIA_NEXT_ACTION_WITH_DESTINATION_EXPLICIT = SOURCE_FOLDER 
            + "LinkAnnotationViaActionWithNextActionExplicitDestination.pdf";

        public static readonly String SOURCE_ANNOTATION_VIA_NEXT_ACTION_WITH_DESTINATION_NAMED = SOURCE_FOLDER + "LinkAnnotationViaActionWithNextActionNamedDestination.pdf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationExplicitDestinationMissingTest() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_WITH_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "LinkAnnotationExplicitDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNull(annot);
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationExplicitDestinationTargetBecomesPage5Test() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_WITH_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "LinkAnnotationExplicitDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfLinkAnnotation)annot).GetDestinationObject());
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationNamedDestinationMissingTest() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_WITH_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "LinkAnnotationNamedDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNull(annot);
            // verify wether name is removed
            NUnit.Framework.Assert.IsTrue(resultDoc.GetCatalog().GetNameTree(PdfName.Dests).GetNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationNamedDestinationTargetBecomesPage5Test() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_WITH_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "LinkAnnotationNamedDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfLinkAnnotation)annot).GetDestinationObject());
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationViaActionExplicitDestinationMissingTest() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_VIA_ACTION_WITH_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkAnnotationViaActionExplicitDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNull(annot);
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationViaActionExplicitDestinationTargetBecomesPage5Test() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_VIA_ACTION_WITH_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "LinkAnnotationViaActionExplicitDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfLinkAnnotation)annot).GetAction().Get(PdfName.D)
                );
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationViaActionNamedDestinationMissingTest() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_VIA_ACTION_WITH_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "LinkAnnotationViaActionNamedDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNull(annot);
            // verify wether name is removed
            NUnit.Framework.Assert.IsTrue(resultDoc.GetCatalog().GetNameTree(PdfName.Dests).GetNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationViaActionNamedDestinationTargetBecomesPage5Test() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_VIA_ACTION_WITH_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "LinkAnnotationViaActionNamedDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfLinkAnnotation)annot).GetAction().Get(PdfName.D)
                );
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationViaNextActionExplicitDestinationMissingTest() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_VIA_NEXT_ACTION_WITH_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkAnnotationViaNextActionExplicitDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            NUnit.Framework.Assert.IsNull(((PdfLinkAnnotation)annot).GetAction().Get(PdfName.Next));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationViaNextActionExplicitDestinationTargetBecomesPage5Test() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_VIA_NEXT_ACTION_WITH_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "LinkAnnotationViaNextActionExplicitDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfLinkAnnotation)annot).GetAction().GetAsDictionary
                (PdfName.Next).Get(PdfName.D));
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationViaNextActionNamedDestinationMissingTest() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_VIA_NEXT_ACTION_WITH_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "LinkAnnotationViaNextActionNamedDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            NUnit.Framework.Assert.IsNull(((PdfLinkAnnotation)annot).GetAction().Get(PdfName.Next));
            // verify whether name is removed
            NUnit.Framework.Assert.IsTrue(resultDoc.GetCatalog().GetNameTree(PdfName.Dests).GetNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotationViaNextActionNamedDestinationTargetBecomesPage5Test() {
            PdfReader sourceReader = new PdfReader(SOURCE_ANNOTATION_VIA_NEXT_ACTION_WITH_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "LinkAnnotationViaNextActionNamedDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3);
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Link) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfLinkAnnotation)annot).GetAction().GetAsDictionary
                (PdfName.Next).Get(PdfName.D));
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }
    }
}
