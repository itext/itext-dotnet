/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Xobject;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa.Checker {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfA2CheckerTransparencyTest : ExtendedITextTest {
        private PdfA2Checker pdfA2Checker;

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            pdfA2Checker = new PdfA2Checker(PdfAConformanceLevel.PDF_A_2B);
            pdfA2Checker.SetFullCheckMode(true);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPatternWithFormResourceCycle() {
            using (MemoryStream bos = new MemoryStream()) {
                using (PdfWriter writer = new PdfWriter(bos)) {
                    using (PdfDocument document = new PdfDocument(writer)) {
                        PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0f, 0f));
                        formXObject.GetResources().AddForm(formXObject);
                        PdfPattern.Tiling tillingPattern = new PdfPattern.Tiling(0f, 0f);
                        tillingPattern.GetResources().AddForm(formXObject);
                        PdfPage pageToCheck = document.AddNewPage();
                        PdfResources pageResources = pageToCheck.GetResources();
                        pageResources.AddPattern(new PdfPattern.Shading(new PdfDictionary()));
                        pageResources.AddPattern(tillingPattern);
                        EnsureTransparencyObjectsNotEmpty();
                        // no assertions as we want to check that no exceptions would be thrown
                        pdfA2Checker.CheckSinglePage(pageToCheck);
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckAppearanceStreamsWithCycle() {
            using (MemoryStream bos = new MemoryStream()) {
                using (PdfWriter writer = new PdfWriter(bos)) {
                    using (PdfDocument document = new PdfDocument(writer)) {
                        PdfDictionary normalAppearance = new PdfDictionary();
                        normalAppearance.Put(PdfName.ON, normalAppearance);
                        normalAppearance.MakeIndirect(document);
                        PdfAnnotation annotation = new PdfPopupAnnotation(new Rectangle(0f, 0f));
                        annotation.SetAppearance(PdfName.N, normalAppearance);
                        PdfPage pageToCheck = document.AddNewPage();
                        pageToCheck.AddAnnotation(annotation);
                        EnsureTransparencyObjectsNotEmpty();
                        // no assertions as we want to check that no exceptions would be thrown
                        pdfA2Checker.CheckPageTransparency(pageToCheck.GetPdfObject(), pageToCheck.GetResources().GetPdfObject());
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckPatternWithTransparentFormResource() {
            using (MemoryStream bos = new MemoryStream()) {
                using (PdfWriter writer = new PdfWriter(bos)) {
                    using (PdfDocument document = new PdfDocument(writer)) {
                        PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0f, 0f));
                        formXObject.SetGroup(new PdfTransparencyGroup());
                        PdfPattern.Tiling tillingPattern = new PdfPattern.Tiling(0f, 0f);
                        tillingPattern.GetResources().AddForm(formXObject);
                        PdfPage pageToCheck = document.AddNewPage();
                        PdfResources pageResources = pageToCheck.GetResources();
                        pageResources.AddPattern(new PdfPattern.Shading(new PdfDictionary()));
                        pageResources.AddPattern(tillingPattern);
                        Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckSinglePage
                            (pageToCheck));
                        NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
                            , e.Message);
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckPatternWithoutTransparentFormResource() {
            using (MemoryStream bos = new MemoryStream()) {
                using (PdfWriter writer = new PdfWriter(bos)) {
                    using (PdfDocument document = new PdfDocument(writer)) {
                        PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0f, 0f));
                        PdfPattern.Tiling tillingPattern = new PdfPattern.Tiling(0f, 0f);
                        tillingPattern.GetResources().AddForm(formXObject);
                        PdfPage pageToCheck = document.AddNewPage();
                        PdfResources pageResources = pageToCheck.GetResources();
                        pageResources.AddPattern(new PdfPattern.Shading(new PdfDictionary()));
                        pageResources.AddPattern(tillingPattern);
                        EnsureTransparencyObjectsNotEmpty();
                        // no assertions as we want to check that no exceptions would be thrown
                        pdfA2Checker.CheckSinglePage(pageToCheck);
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckAppearanceStreamWithTransparencyGroup() {
            using (MemoryStream bos = new MemoryStream()) {
                using (PdfWriter writer = new PdfWriter(bos)) {
                    using (PdfDocument document = new PdfDocument(writer)) {
                        PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0f, 0f));
                        formXObject.SetGroup(new PdfTransparencyGroup());
                        PdfAnnotation annotation = new PdfPopupAnnotation(new Rectangle(0f, 0f));
                        annotation.SetNormalAppearance(formXObject.GetPdfObject());
                        PdfPage pageToCheck = document.AddNewPage();
                        pageToCheck.AddAnnotation(new PdfPopupAnnotation(new Rectangle(0f, 0f)));
                        pageToCheck.AddAnnotation(annotation);
                        Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckSinglePage
                            (pageToCheck));
                        NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
                            , e.Message);
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckAppearanceStreamWithTransparencyGroup2() {
            using (MemoryStream bos = new MemoryStream()) {
                using (PdfWriter writer = new PdfWriter(bos)) {
                    using (PdfDocument document = new PdfDocument(writer)) {
                        PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0f, 0f));
                        formXObject.SetGroup(new PdfTransparencyGroup());
                        PdfFormXObject formStream = new PdfFormXObject(new Rectangle(0f, 0f));
                        formStream.GetResources().AddForm(formXObject);
                        PdfAnnotation annotation = new PdfPopupAnnotation(new Rectangle(0f, 0f));
                        annotation.SetNormalAppearance(formStream.GetPdfObject());
                        PdfPage pageToCheck = document.AddNewPage();
                        pageToCheck.AddAnnotation(new PdfPopupAnnotation(new Rectangle(0f, 0f)));
                        pageToCheck.AddAnnotation(annotation);
                        Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => pdfA2Checker.CheckSinglePage
                            (pageToCheck));
                        NUnit.Framework.Assert.AreEqual(PdfaExceptionMessageConstant.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
                            , e.Message);
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckAppearanceStreamWithoutTransparencyGroup() {
            using (MemoryStream bos = new MemoryStream()) {
                using (PdfWriter writer = new PdfWriter(bos)) {
                    using (PdfDocument document = new PdfDocument(writer)) {
                        PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0f, 0f));
                        PdfAnnotation annotation = new PdfPopupAnnotation(new Rectangle(0f, 0f));
                        annotation.SetNormalAppearance(formXObject.GetPdfObject());
                        PdfPage pageToCheck = document.AddNewPage();
                        pageToCheck.AddAnnotation(new PdfPopupAnnotation(new Rectangle(0f, 0f)));
                        pageToCheck.AddAnnotation(annotation);
                        EnsureTransparencyObjectsNotEmpty();
                        // no assertions as we want to check that no exceptions would be thrown
                        pdfA2Checker.CheckSinglePage(pageToCheck);
                    }
                }
            }
        }

        private void EnsureTransparencyObjectsNotEmpty() {
            PdfFormXObject formXObject = new PdfFormXObject(new Rectangle(0f, 0f));
            formXObject.SetGroup(new PdfTransparencyGroup());
            pdfA2Checker.CheckFormXObject(formXObject.GetPdfObject());
        }
    }
}
