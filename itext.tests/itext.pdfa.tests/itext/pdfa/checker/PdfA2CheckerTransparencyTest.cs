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
                        NUnit.Framework.Assert.AreEqual(PdfAConformanceException.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
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
                        NUnit.Framework.Assert.AreEqual(PdfAConformanceException.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
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
                        NUnit.Framework.Assert.AreEqual(PdfAConformanceException.THE_DOCUMENT_DOES_NOT_CONTAIN_A_PDFA_OUTPUTINTENT_BUT_PAGE_CONTAINS_TRANSPARENCY_AND_DOES_NOT_CONTAIN_BLENDING_COLOR_SPACE
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
