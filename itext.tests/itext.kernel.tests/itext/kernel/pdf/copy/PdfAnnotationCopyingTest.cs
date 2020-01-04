/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using NUnit.Framework;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Test;

namespace iText.Kernel.Pdf.Copy {
    public class PdfAnnotationCopyingTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfAnnotationCopyingTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfAnnotationCopyingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TestCopyingPageWithAnnotationContainingPopupKey() {
            NUnit.Framework.Assert.That(() =>  {
                // TODO remove expected exception and thus enable assertions when DEVSIX-3585 is implemented
                String inFilePath = sourceFolder + "annotation-with-popup.pdf";
                String outFilePath = destinationFolder + "copy-annotation-with-popup.pdf";
                PdfDocument originalDocument = new PdfDocument(new PdfReader(inFilePath));
                PdfDocument outDocument = new PdfDocument(new PdfWriter(outFilePath));
                originalDocument.CopyPagesTo(1, 1, outDocument);
                // During the second copy call we have to rebuild/preserve all the annotation relationship (Popup in this case),
                // so that we don't end up with annotation on one page referring to an annotation on another page as its popup
                // or as its parent
                originalDocument.CopyPagesTo(1, 1, outDocument);
                originalDocument.Close();
                outDocument.Close();
                outDocument = new PdfDocument(new PdfReader(outFilePath));
                for (int pageNum = 1; pageNum <= outDocument.GetNumberOfPages(); pageNum++) {
                    PdfPage page = outDocument.GetPage(pageNum);
                    NUnit.Framework.Assert.AreEqual(2, page.GetAnnotsSize());
                    NUnit.Framework.Assert.AreEqual(2, page.GetAnnotations().Count);
                    bool foundMarkupAnnotation = false;
                    foreach (PdfAnnotation annotation in page.GetAnnotations()) {
                        PdfDictionary annotationPageDict = annotation.GetPageObject();
                        if (annotationPageDict != null) {
                            NUnit.Framework.Assert.AreSame(page.GetPdfObject(), annotationPageDict);
                        }
                        if (annotation is PdfMarkupAnnotation) {
                            foundMarkupAnnotation = true;
                            PdfPopupAnnotation popup = ((PdfMarkupAnnotation)annotation).GetPopup();
                            NUnit.Framework.Assert.IsTrue(page.ContainsAnnotation(popup), MessageFormatUtil.Format("Popup reference must point to annotation present on the same page (# {0})"
                                , pageNum));
                            PdfDictionary parentAnnotation = popup.GetParentObject();
                            NUnit.Framework.Assert.AreSame(annotation.GetPdfObject(), parentAnnotation, "Popup annotation parent must point to the annotation that specified it as Popup"
                                );
                        }
                    }
                    NUnit.Framework.Assert.IsTrue(foundMarkupAnnotation, "Markup annotation expected to be present but not found"
                        );
                }
                outDocument.Close();
            }
            , NUnit.Framework.Throws.InstanceOf<AssertionException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void TestCopyingPageWithAnnotationContainingIrtKey() {
            NUnit.Framework.Assert.That(() =>  {
                // TODO remove expected exception and thus enable assertions when DEVSIX-3585 is implemented
                String inFilePath = sourceFolder + "annotation-with-irt.pdf";
                String outFilePath = destinationFolder + "copy-annotation-with-irt.pdf";
                PdfDocument originalDocument = new PdfDocument(new PdfReader(inFilePath));
                PdfDocument outDocument = new PdfDocument(new PdfWriter(outFilePath));
                originalDocument.CopyPagesTo(1, 1, outDocument);
                // During the second copy call we have to rebuild/preserve all the annotation relationship (IRT in this case),
                // so that we don't end up with annotation on one page referring to an annotation on another page as its IRT
                // or as its parent
                originalDocument.CopyPagesTo(1, 1, outDocument);
                originalDocument.Close();
                outDocument.Close();
                outDocument = new PdfDocument(new PdfReader(outFilePath));
                for (int pageNum = 1; pageNum <= outDocument.GetNumberOfPages(); pageNum++) {
                    PdfPage page = outDocument.GetPage(pageNum);
                    NUnit.Framework.Assert.AreEqual(4, page.GetAnnotsSize());
                    NUnit.Framework.Assert.AreEqual(4, page.GetAnnotations().Count);
                    bool foundMarkupAnnotation = false;
                    foreach (PdfAnnotation annotation in page.GetAnnotations()) {
                        PdfDictionary annotationPageDict = annotation.GetPageObject();
                        if (annotationPageDict != null) {
                            NUnit.Framework.Assert.AreSame(page.GetPdfObject(), annotationPageDict);
                        }
                        if (annotation is PdfMarkupAnnotation) {
                            foundMarkupAnnotation = true;
                            PdfDictionary inReplyTo = ((PdfMarkupAnnotation)annotation).GetInReplyToObject();
                            NUnit.Framework.Assert.IsTrue(page.ContainsAnnotation(PdfAnnotation.MakeAnnotation(inReplyTo)), "IRT reference must point to annotation present on the same page"
                                );
                        }
                    }
                    NUnit.Framework.Assert.IsTrue(foundMarkupAnnotation, "Markup annotation expected to be present but not found"
                        );
                }
                outDocument.Close();
            }
            , NUnit.Framework.Throws.InstanceOf<AssertionException>())
;
        }
    }
}
