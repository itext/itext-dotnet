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
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.IO.Source;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils.Annotationsflattening;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAnnotationFlattenerTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/flatteningTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/utils/flatteningTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TestNullAnnotations() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                PdfPage page = pdfDoc.GetFirstPage();
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    flattener.Flatten(null, page);
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestNullPage() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                IList<PdfAnnotation> annotations = new List<PdfAnnotation>();
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    flattener.Flatten(annotations, null);
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestNullPageFlatten() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    flattener.Flatten(null);
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestNullPageDrawAppearanceWorker() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                IAnnotationFlattener flattener = new DefaultAnnotationFlattener();
                PdfPage page = pdfDoc.GetFirstPage();
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    flattener.Flatten(null, page);
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestNullAnnotationDrawAppearanceWorker() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                IAnnotationFlattener flattener = new DefaultAnnotationFlattener();
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    flattener.Flatten(new PdfLinkAnnotation(new Rectangle(20, 20)), null);
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyAnnotations() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(new List<PdfAnnotation>(), pdfDoc.GetFirstPage());
                NUnit.Framework.Assert.AreEqual(0, pdfDoc.GetFirstPage().GetAnnotsSize());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DefaultAppearanceGetsRendered() {
            String resultFile = destinationFolder + "default_annotations_app.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(resultFile))) {
                PdfFormXObject formN = new PdfFormXObject(new Rectangle(179, 530, 122, 21));
                PdfCanvas canvasN = new PdfCanvas(formN, pdfDoc);
                PdfAnnotation annotation = new PdfLinkAnnotation(new Rectangle(100, 540, 300, 50)).SetAction(PdfAction.CreateURI
                    ("http://itextpdf.com/node"));
                canvasN.SaveState().SetColor(ColorConstants.RED, true).SetLineWidth(1.5f).Rectangle(180, 531, 120, 48).Fill
                    ().RestoreState();
                canvasN.SaveState().BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 12).SetColor
                    (ColorConstants.YELLOW, true).MoveText(180, 531).ShowText("Hello appearance").EndText().RestoreState();
                annotation.SetNormalAppearance(formN.GetPdfObject());
                pdfDoc.AddNewPage();
                pdfDoc.GetFirstPage().AddAnnotation(annotation);
                DefaultAnnotationFlattener worker = new DefaultAnnotationFlattener();
                worker.Flatten(annotation, pdfDoc.GetFirstPage());
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_default_annotations_app.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FLATTENING_IS_NOT_YET_SUPPORTED)]
        public virtual void UnknownAnnotationsDefaultImplementation() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfDictionary unknownAnnot = new PdfDictionary();
                unknownAnnot.Put(PdfName.Subtype, new PdfName("Unknown"));
                unknownAnnot.Put(PdfName.Rect, new PdfArray(new int[] { 100, 100, 200, 200 }));
                PdfAnnotation unknownAnnotation = PdfAnnotation.MakeAnnotation(unknownAnnot);
                pdfDoc.AddNewPage();
                pdfDoc.GetFirstPage().AddAnnotation(unknownAnnotation);
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(JavaCollectionsUtil.SingletonList(unknownAnnotation), pdfDoc.GetFirstPage());
                //Annotation is not removed in default implementation
                NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetFirstPage().GetAnnotsSize());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FLATTENING_IS_NOT_YET_SUPPORTED)]
        public virtual void NullTypeAnnotationsDefaultImplementation() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfDictionary unknownAnnot = new PdfDictionary();
                unknownAnnot.Put(PdfName.Rect, new PdfArray(new int[] { 100, 100, 200, 200 }));
                PdfAnnotation unknownAnnotation = PdfAnnotation.MakeAnnotation(unknownAnnot);
                pdfDoc.AddNewPage();
                pdfDoc.GetFirstPage().AddAnnotation(unknownAnnotation);
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(JavaCollectionsUtil.SingletonList(unknownAnnotation), pdfDoc.GetFirstPage());
                //Annotation is not removed in default implementation
                NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetFirstPage().GetAnnotsSize());
            }
        }

        [NUnit.Framework.Test]
        public virtual void OverwriteDefaultImplementation() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                int[] borders = new int[] { 0, 0, 1 };
                pdfDoc.AddNewPage();
                pdfDoc.GetFirstPage().AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 540, 300, 25)).SetAction(PdfAction
                    .CreateURI("http://itextpdf.com/node")).SetBorder(new PdfArray(borders)));
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener(new PdfAnnotationFlattenerTest.CustomPdfAnnotationFlattenFactory
                    ());
                flattener.Flatten(pdfDoc.GetFirstPage().GetAnnotations(), pdfDoc.GetFirstPage());
                NUnit.Framework.Assert.AreEqual(0, pdfDoc.GetFirstPage().GetAnnotsSize());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RemoveQuadPoints() {
            String fileToFlatten = destinationFolder + "file_to_quadpoints.pdf";
            String resultFile = destinationFolder + "flattened_quadpoints.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(fileToFlatten))) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                float x = 50;
                float y = 750;
                float textLength = 350;
                float[] points = new float[] { x, y + 15, x + textLength, y + 15, x, y - 4, x + textLength, y - 4 };
                PdfAnnotation annot = CreateTextAnnotation(canvas, x, y, points, PdfName.StrikeOut, ColorConstants.RED);
                annot.GetPdfObject().Remove(PdfName.QuadPoints);
                page.AddAnnotation(annot);
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(fileToFlatten), new PdfWriter(resultFile))) {
                new PdfAnnotationFlattener().Flatten(pdfDoc_1.GetFirstPage().GetAnnotations(), pdfDoc_1.GetFirstPage());
            }
            //it is expected that the line is the middle of the page because the annotation whole rectangle is the
            // size of the page, it's also expected that underline will not show up as it is at the bottom of the page
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_text_quadpoints.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidQuadPoints() {
            String fileToFlatten = destinationFolder + "file_to_invalid_quadpoints.pdf";
            String resultFile = destinationFolder + "flattened_invalid_quadpoints.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(fileToFlatten))) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                float x = 50;
                float y = 750;
                float textLength = 350;
                float[] points = new float[] { x, y + 15, x + textLength, y + 15, x, y - 4, x + textLength, y - 4 };
                PdfAnnotation annot = CreateTextAnnotation(canvas, x, y, points, PdfName.StrikeOut, ColorConstants.RED);
                annot.GetPdfObject().Put(PdfName.QuadPoints, new PdfArray(new float[] { 0, 0, 0, 0, 0, 0 }));
                page.AddAnnotation(annot);
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(fileToFlatten), new PdfWriter(resultFile))) {
                new PdfAnnotationFlattener().Flatten(pdfDoc_1.GetFirstPage().GetAnnotations(), pdfDoc_1.GetFirstPage());
            }
            //it is expected that the line is the middle of the page because the annotation whole rectangle is the
            // size of the page, it's also expected that underline will not show up as it is at the bottom of the page
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_invalid_quadpoints.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyParamListDoesntDeleteAnyAnnots() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                int[] borders = new int[] { 0, 0, 1 };
                pdfDoc.GetFirstPage().AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 540, 300, 25)).SetAction(PdfAction
                    .CreateURI("http://itextpdf.com/node")).SetBorder(new PdfArray(borders)));
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(new List<PdfAnnotation>(), pdfDoc.GetFirstPage());
                NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetFirstPage().GetAnnotsSize());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FLATTENING_IS_NOT_YET_SUPPORTED)]
        public virtual void TestListFromDifferentPageDoesntDeleteAnyAnnotsButWarnsUser() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                pdfDoc.AddNewPage();
                int[] borders = new int[] { 0, 0, 1 };
                pdfDoc.GetPage(1).AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 540, 300, 25)).SetAction(PdfAction
                    .CreateURI("http://itextpdf.com/node")).SetBorder(new PdfArray(borders)));
                pdfDoc.GetPage(2).AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 540, 300, 25)).SetAction(PdfAction
                    .CreateURI("http://itextpdf.com/node")).SetBorder(new PdfArray(borders)));
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(pdfDoc.GetPage(2).GetAnnotations(), pdfDoc.GetFirstPage());
                NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetFirstPage().GetAnnotsSize());
                NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetPage(2).GetAnnotsSize());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FLATTENING_IS_NOT_YET_SUPPORTED)]
        public virtual void FlattenPdfLink() {
            String resultFile = destinationFolder + "flattened_pdf_link.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "simple_link_annotation.pdf"), new 
                PdfWriter(resultFile))) {
                new PdfAnnotationFlattener().Flatten(pdfDoc.GetFirstPage());
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_flattened_pdf_link.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenPdfLinkWithDefaultAppearance() {
            String resultFile = destinationFolder + "flattened_DA_pdf_link.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "simple_link_annotation.pdf"), new 
                PdfWriter(resultFile))) {
                PdfAnnotation annot = pdfDoc.GetFirstPage().GetAnnotations()[0];
                annot.SetNormalAppearance(new PdfDictionary());
                PdfFormXObject formN = new PdfFormXObject(new Rectangle(179, 530, 122, 21));
                PdfCanvas canvasN = new PdfCanvas(formN, pdfDoc);
                canvasN.SaveState().SetColor(ColorConstants.RED, true).SetLineWidth(1.5f).Rectangle(180, 531, 120, 48).Fill
                    ().RestoreState();
                annot.SetNormalAppearance(formN.GetPdfObject());
                new PdfAnnotationFlattener().Flatten(pdfDoc.GetFirstPage());
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_flattened_DA_pdf_link.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenTextMarkupAnnotations() {
            String fileToFlatten = destinationFolder + "file_to_flatten_markup_text.pdf";
            String resultFile = destinationFolder + "flattened_markup_text.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(fileToFlatten))) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                float x = 50;
                float y = 750;
                float textLength = 350;
                float[] points = new float[] { x, y + 15, x + textLength, y + 15, x, y - 4, x + textLength, y - 4 };
                page.AddAnnotation(CreateTextAnnotation(canvas, x, y, points, PdfName.Underline, ColorConstants.RED));
                y -= 50;
                float[] points2 = new float[] { x, y + 15, x + textLength, y + 15, x, y - 4, x + textLength, y - 4 };
                page.AddAnnotation(CreateTextAnnotation(canvas, x, y, points2, PdfName.StrikeOut, ColorConstants.BLUE));
                y -= 50;
                float[] points3 = new float[] { x, y + 15, x + textLength, y + 15, x, y - 4, x + textLength, y - 4 };
                page.AddAnnotation(CreateTextAnnotation(canvas, x, y, points3, PdfName.Squiggly, ColorConstants.RED));
                y -= 50;
                float[] points4 = new float[] { x, y + 15, x + textLength, y + 15, x, y - 4, x + textLength, y - 4 };
                page.AddAnnotation(CreateTextAnnotation(canvas, x, y, points4, PdfName.Highlight, ColorConstants.YELLOW));
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(fileToFlatten), new PdfWriter(resultFile))) {
                new PdfAnnotationFlattener().Flatten(pdfDoc_1.GetFirstPage().GetAnnotations(), pdfDoc_1.GetFirstPage());
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_text_markup_flatten.pdf"
                , destinationFolder, "diff_"));
        }

        private PdfTextMarkupAnnotation CreateTextAnnotation(PdfCanvas canvas, float x, float y, float[] quadPoints
            , PdfName type, Color color) {
            canvas.SaveState().BeginText().MoveText(x, y).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText(PlaceHolderTextUtil.GetPlaceHolderText(PlaceHolderTextUtil.PlaceHolderTextBy.WORDS, 5)
                ).EndText().RestoreState();
            PdfTextMarkupAnnotation markup = null;
            if (PdfName.Underline.Equals(type)) {
                markup = PdfTextMarkupAnnotation.CreateUnderline(PageSize.A4, quadPoints);
            }
            if (PdfName.StrikeOut.Equals(type)) {
                markup = PdfTextMarkupAnnotation.CreateStrikeout(PageSize.A4, quadPoints);
            }
            if (PdfName.Highlight.Equals(type)) {
                markup = PdfTextMarkupAnnotation.CreateHighLight(PageSize.A4, quadPoints);
            }
            if (PdfName.Squiggly.Equals(type)) {
                markup = PdfTextMarkupAnnotation.CreateSquiggly(PageSize.A4, quadPoints);
            }
            if (markup == null) {
                throw new ArgumentException();
            }
            markup.SetContents(new PdfString("TextMarkup"));
            markup.SetColor(color.GetColorValue());
            return markup;
        }

        internal class CustomPdfAnnotationFlattenFactory : PdfAnnotationFlattenFactory {
            public override IAnnotationFlattener GetAnnotationFlattenWorker(PdfName name) {
                if (PdfName.Link.Equals(name)) {
                    return new _IAnnotationFlattener_432();
                }
                return base.GetAnnotationFlattenWorker(name);
            }

            private sealed class _IAnnotationFlattener_432 : IAnnotationFlattener {
                public _IAnnotationFlattener_432() {
                }

                public bool Flatten(PdfAnnotation annotation, PdfPage page) {
                    page.RemoveAnnotation(annotation);
                    return true;
                }
            }
        }
    }
}
