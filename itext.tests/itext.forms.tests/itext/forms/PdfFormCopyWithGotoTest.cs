/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfFormCopyWithGotoTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfFormCopyWithGotoTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfFormCopyWithGotoTest/";

        public static readonly String TARGET_DOC = SOURCE_FOLDER + "Target.pdf";

        public static readonly String SOURCE_WIDGET_ACTION_WITH_DESTINATION_EXPLICIT = SOURCE_FOLDER + "LinkWidgetExplicitDestination.pdf";

        public static readonly String SOURCE_WIDGET_ACTION_WITH_DESTINATION_NAMED = SOURCE_FOLDER + "LinkWidgetNamedDestination.pdf";

        public static readonly String SOURCE_WIDGET_ADDITIONAL_ACTION_DOWN_DESTINATION_EXPLICIT = SOURCE_FOLDER + 
            "LinkWidgetAAMouseDownExplicitDestination.pdf";

        public static readonly String SOURCE_WIDGET_ADDITIONAL_ACTION_DOWN_DESTINATION_NAMED = SOURCE_FOLDER + "LinkWidgetAAMouseDownNamedDestination.pdf";

        public static readonly String SOURCE_WIDGET_ADDITIONAL_ACTION_UP_DESTINATION_EXPLICIT = SOURCE_FOLDER + "LinkWidgetAAMouseUpExplicitDestination.pdf";

        public static readonly String SOURCE_WIDGET_ADDITIONAL_ACTION_UP_DESTINATION_NAMED = SOURCE_FOLDER + "LinkWidgetAAMouseUpNamedDestination.pdf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetNamedDestinationMissing() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ACTION_WITH_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetNamedDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNull(((PdfWidgetAnnotation)annot).GetAction());
            // verify wether name is removed
            NUnit.Framework.Assert.IsTrue(resultDoc.GetCatalog().GetNameTree(PdfName.Dests).GetNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetNamedDestinationTargetBecomesPage5() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ACTION_WITH_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetNamedDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfWidgetAnnotation)annot).GetAction().Get(PdfName.
                D));
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetExplicitDestinationMissing() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ACTION_WITH_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetExplicitDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNull(((PdfWidgetAnnotation)annot).GetAction());
            // verify wether name is removed
            NUnit.Framework.Assert.IsTrue(resultDoc.GetCatalog().GetNameTree(PdfName.Dests).GetNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetExplicitDestinationTargetBecomesPage5() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ACTION_WITH_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetExplicitDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfWidgetAnnotation)annot).GetAction().Get(PdfName.
                D));
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetAAUpNamedDestinationMissing() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ADDITIONAL_ACTION_UP_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetAAUpNamedDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNull(((PdfWidgetAnnotation)annot).GetAction());
            // verify wether name is removed
            NUnit.Framework.Assert.IsTrue(resultDoc.GetCatalog().GetNameTree(PdfName.Dests).GetNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetAAUpNamedDestinationTargetBecomesPage5() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ADDITIONAL_ACTION_UP_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetAAUpNamedDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfWidgetAnnotation)annot).GetAdditionalAction().GetAsDictionary
                (PdfName.U).Get(PdfName.D));
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetAAUpExplicitDestinationMissing() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ADDITIONAL_ACTION_UP_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetAAUpExplicitDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNull(((PdfWidgetAnnotation)annot).GetAction());
            // verify wether name is removed
            NUnit.Framework.Assert.IsTrue(resultDoc.GetCatalog().GetNameTree(PdfName.Dests).GetNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetAAUpExplicitDestinationTargetBecomesPage5() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ADDITIONAL_ACTION_UP_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetAAUpExplicitDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfWidgetAnnotation)annot).GetAdditionalAction().GetAsDictionary
                (PdfName.U).Get(PdfName.D));
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetAADownNamedDestinationMissing() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ADDITIONAL_ACTION_DOWN_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetAADownNamedDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNull(((PdfWidgetAnnotation)annot).GetAction());
            // verify wether name is removed
            NUnit.Framework.Assert.IsTrue(resultDoc.GetCatalog().GetNameTree(PdfName.Dests).GetNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetAADownNamedDestinationTargetBecomesPage5() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ADDITIONAL_ACTION_DOWN_DESTINATION_NAMED);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetAADownNamedDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfWidgetAnnotation)annot).GetAdditionalAction().GetAsDictionary
                (PdfName.D).Get(PdfName.D));
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetAADownExplicitDestinationMissing() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ADDITIONAL_ACTION_DOWN_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetAADownExplicitDestinationMissing.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 2, targetDoc, 2, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 2 and verify the annotation is not copied
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(2).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNull(((PdfWidgetAnnotation)annot).GetAction());
            // verify wether name is removed
            NUnit.Framework.Assert.IsTrue(resultDoc.GetCatalog().GetNameTree(PdfName.Dests).GetNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestLinkWidgetAADownExplicitDestinationTargetBecomesPage5() {
            PdfReader sourceReader = new PdfReader(SOURCE_WIDGET_ADDITIONAL_ACTION_DOWN_DESTINATION_EXPLICIT);
            PdfDocument copySource = new PdfDocument(sourceReader);
            PdfReader targetReader = new PdfReader(TARGET_DOC);
            String outputPath = System.IO.Path.Combine(DESTINATION_FOLDER, "TestLinkWidgetAADownExplicitDestinationTargetBecomesPage5.pdf"
                ).ToString();
            PdfDocument targetDoc = new PdfDocument(targetReader, new PdfWriter(outputPath));
            copySource.CopyPagesTo(1, 3, targetDoc, 3, new PdfPageFormCopier());
            copySource.Close();
            targetDoc.Close();
            PdfReader resultReader = new PdfReader(outputPath);
            PdfDocument resultDoc = new PdfDocument(resultReader);
            // get annotation on page 3 and verify it points to page 5
            PdfAnnotation annot = null;
            foreach (PdfAnnotation item in resultDoc.GetPage(3).GetAnnotations()) {
                if (item.GetSubtype() == PdfName.Widget) {
                    annot = item;
                    break;
                }
            }
            NUnit.Framework.Assert.IsNotNull(annot);
            PdfDestination dest = PdfDestination.MakeDestination(((PdfWidgetAnnotation)annot).GetAdditionalAction().GetAsDictionary
                (PdfName.D).Get(PdfName.D));
            NUnit.Framework.Assert.AreEqual(resultDoc.GetPage(5).GetPdfObject(), dest.GetDestinationPage(resultDoc.GetCatalog
                ().GetNameTree(PdfName.Dests)));
        }
    }
}
