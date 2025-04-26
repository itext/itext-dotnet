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
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers.Utils.Ua2 {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfUA2AnnotationCheckerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BasicAnnotationBadParent() {
            PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(new Rectangle(0, 0, 100, 100), new float[] { 2, 3
                 });
            PdfStructElem parent = new PdfStructElem(null, PdfName.Div);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                PdfUA2AnnotationChecker.CheckAnnotation(lineAnnotation.GetPdfObject(), parent);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void BasicLineAnnotation() {
            PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(new Rectangle(0, 0, 100, 100), new float[] { 2, 3
                 });
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(lineAnnotation.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicSquareAnnotation() {
            PdfSquareAnnotation squareAnnotation = new PdfSquareAnnotation(new Rectangle(0, 0, 100, 100));
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(squareAnnotation.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicCircleAnnotation() {
            PdfCircleAnnotation circleAnnotation = new PdfCircleAnnotation(new Rectangle(0, 0, 100, 100));
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(circleAnnotation.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicPolygonAnnotation() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.Polygon);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicPolyLineAnnotation() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.PolyLine);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicHighlightAnnotation() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.Highlight);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicUnderlineAnnotation() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.Underline);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicSquigglyAnnotation() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.Squiggly);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicStrikeOutAnnotation() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.StrikeOut);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicCaretAnnotation() {
            PdfCaretAnnotation annotation = new PdfCaretAnnotation(new Rectangle(2, 2, 100, 100));
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annotation.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicInkAnnotationThrowsOnNoContents() {
            PdfInkAnnotation annotation = new PdfInkAnnotation(new Rectangle(2, 2, 100, 100));
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                PdfUA2AnnotationChecker.CheckAnnotation(annotation.GetPdfObject(), parent);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.ANNOT_CONTENTS_IS_NULL_OR_EMPTY, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BasicInkAnnotationThrowsOnEmptyContents() {
            PdfInkAnnotation annotation = new PdfInkAnnotation(new Rectangle(2, 2, 100, 100));
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            annotation.SetContents("");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                PdfUA2AnnotationChecker.CheckAnnotation(annotation.GetPdfObject(), parent);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.ANNOT_CONTENTS_IS_NULL_OR_EMPTY, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BasicInkAnnotation() {
            PdfInkAnnotation annotation = new PdfInkAnnotation(new Rectangle(2, 2, 100, 100));
            annotation.SetContents("Test");
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annotation.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicPopup() {
            PdfPopupAnnotation annotation = new PdfPopupAnnotation(new Rectangle(2, 2, 100, 100));
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                PdfUA2AnnotationChecker.CheckAnnotation(annotation.GetPdfObject(), parent);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.POPUP_ANNOTATIONS_ARE_NOT_ALLOWED, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void BasicFileAttachment() {
            PdfFileAttachmentAnnotation annotation = new PdfFileAttachmentAnnotation(new Rectangle(2, 2, 100, 100));
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annotation.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicSoundNotAllowed() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.Sound);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.DEPRECATED_ANNOTATIONS_ARE_NOT_ALLOWED
                , PdfName.Sound.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BasicMovieNotAllowed() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.Movie);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.DEPRECATED_ANNOTATIONS_ARE_NOT_ALLOWED
                , PdfName.Movie.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BasicPrinterMark() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.PrinterMark);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.PRINTER_MARK_SHALL_BE_AN_ARTIFACT, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void BasicTrapNet() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.TrapNet);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.DEPRECATED_ANNOTATIONS_ARE_NOT_ALLOWED
                , PdfName.TrapNet.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BasicWatermark() {
            PdfWatermarkAnnotation annot = new PdfWatermarkAnnotation(new Rectangle(0, 0, 100, 100));
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicRedaction() {
            PdfRedactAnnotation annot = new PdfRedactAnnotation(new Rectangle(0, 0, 100, 100));
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void BasicProjection() {
            PdfDictionary annotation = new PdfDictionary();
            annotation.Put(PdfName.Subtype, PdfName.Projection);
            PdfAnnotation annot = PdfAnnotation.MakeAnnotation(annotation);
            PdfStructElem parent = new PdfStructElem(null, PdfName.Annot);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                PdfUA2AnnotationChecker.CheckAnnotation(annot.GetPdfObject(), parent);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void PdfUAWithEmbeddedFilesWithoutAFRTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfUADocument doc = new PdfUADocument(writer, new PdfUAConfig(PdfUAConformance.PDF_UA_2, "hello", "en-US")
                );
            doc.AddNewPage();
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(doc, "file".GetBytes(), "description", "file.txt", null
                , null, null);
            PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
            fsDict.Remove(PdfName.AFRelationship);
            PdfFileAttachmentAnnotation annotation = new PdfFileAttachmentAnnotation(new Rectangle(2, 2, 100, 100), fs
                );
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => doc.GetPage(1).AddAnnotation
                (annotation));
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.FILE_SPEC_SHALL_CONTAIN_AFRELATIONSHIP, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void PdfUAWithEmbeddedFilesWithoutAFROnClosingTest() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0));
            PdfUADocument doc = new PdfUADocument(writer, new PdfUAConfig(PdfUAConformance.PDF_UA_2, "hello", "en-US")
                );
            doc.AddNewPage();
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(doc, "file".GetBytes(), "description", "file.txt", null
                , null, null);
            PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
            fsDict.Remove(PdfName.AFRelationship);
            PdfFileAttachmentAnnotation annotation = new PdfFileAttachmentAnnotation(new Rectangle(2, 2, 100, 100), fs
                );
            PdfPage page = doc.GetPage(1);
            page.GetPdfObject().Put(PdfName.Annots, new PdfArray(annotation.GetPdfObject()));
            TagTreePointer tagPointer = doc.GetTagStructureContext().GetAutoTaggingPointer();
            tagPointer.AddTag(StandardRoles.ANNOT);
            tagPointer.SetPageForTagging(page).AddAnnotationTag(annotation);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.FILE_SPEC_SHALL_CONTAIN_AFRELATIONSHIP, e.Message
                );
        }
    }
}
