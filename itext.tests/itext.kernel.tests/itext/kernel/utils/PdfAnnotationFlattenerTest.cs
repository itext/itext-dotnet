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
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/flatteningTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/utils/flatteningTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void TestNullAnnotations() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                IList<PdfAnnotation> annotations = null;
                NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                    flattener.Flatten(annotations);
                }
                );
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestNullDocument() {
            PdfDocument pdfDoc = null;
            PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                flattener.Flatten(pdfDoc);
            }
            );
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
                flattener.Flatten(new List<PdfAnnotation>());
                NUnit.Framework.Assert.AreEqual(0, pdfDoc.GetFirstPage().GetAnnotsSize());
            }
        }

        [NUnit.Framework.Test]
        public virtual void DefaultAppearanceGetsRendered() {
            String resultFile = DESTINATION_FOLDER + "default_annotations_app.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(resultFile))) {
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_default_annotations_app.pdf"
                , DESTINATION_FOLDER, "diff_"));
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
                flattener.Flatten(JavaCollectionsUtil.SingletonList(unknownAnnotation));
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
                flattener.Flatten(JavaCollectionsUtil.SingletonList(unknownAnnotation));
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
                flattener.Flatten(pdfDoc.GetFirstPage().GetAnnotations());
                NUnit.Framework.Assert.AreEqual(0, pdfDoc.GetFirstPage().GetAnnotsSize());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RemoveQuadPoints() {
            String fileToFlatten = DESTINATION_FOLDER + "file_to_quadpoints.pdf";
            String resultFile = DESTINATION_FOLDER + "flattened_quadpoints.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(fileToFlatten))) {
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
            using (PdfDocument pdfDoc_1 = new PdfDocument(CompareTool.CreateOutputReader(fileToFlatten), CompareTool.CreateTestPdfWriter
                (resultFile))) {
                new PdfAnnotationFlattener().Flatten(pdfDoc_1.GetFirstPage().GetAnnotations());
            }
            //it is expected that the line is the middle of the page because the annotation whole rectangle is the
            // size of the page, it's also expected that underline will not show up as it is at the bottom of the page
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_text_quadpoints.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidQuadPoints() {
            String fileToFlatten = DESTINATION_FOLDER + "file_to_invalid_quadpoints.pdf";
            String resultFile = DESTINATION_FOLDER + "flattened_invalid_quadpoints.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(fileToFlatten))) {
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
            using (PdfDocument pdfDoc_1 = new PdfDocument(CompareTool.CreateOutputReader(fileToFlatten), CompareTool.CreateTestPdfWriter
                (resultFile))) {
                new PdfAnnotationFlattener().Flatten(pdfDoc_1.GetFirstPage().GetAnnotations());
            }
            //it is expected that the line is the middle of the page because the annotation whole rectangle is the
            // size of the page, it's also expected that underline will not show up as it is at the bottom of the page
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_invalid_quadpoints.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestEmptyParamListDoesntDeleteAnyAnnots() {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                pdfDoc.AddNewPage();
                int[] borders = new int[] { 0, 0, 1 };
                pdfDoc.GetFirstPage().AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 540, 300, 25)).SetAction(PdfAction
                    .CreateURI("http://itextpdf.com/node")).SetBorder(new PdfArray(borders)));
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(new List<PdfAnnotation>());
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
                flattener.Flatten(pdfDoc.GetPage(2).GetAnnotations());
                NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetFirstPage().GetAnnotsSize());
                NUnit.Framework.Assert.AreEqual(1, pdfDoc.GetPage(2).GetAnnotsSize());
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FLATTENING_IS_NOT_YET_SUPPORTED)]
        public virtual void FlattenPdfLink() {
            String resultFile = DESTINATION_FOLDER + "flattened_pdf_link.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "simple_link_annotation.pdf"), CompareTool
                .CreateTestPdfWriter(resultFile))) {
                new PdfAnnotationFlattener().Flatten(pdfDoc);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattened_pdf_link.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenPdfLinkWithDefaultAppearance() {
            String resultFile = DESTINATION_FOLDER + "flattened_DA_pdf_link.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "simple_link_annotation.pdf"), CompareTool
                .CreateTestPdfWriter(resultFile))) {
                PdfAnnotation annot = pdfDoc.GetFirstPage().GetAnnotations()[0];
                annot.SetNormalAppearance(new PdfDictionary());
                PdfFormXObject formN = new PdfFormXObject(new Rectangle(179, 530, 122, 21));
                PdfCanvas canvasN = new PdfCanvas(formN, pdfDoc);
                canvasN.SaveState().SetColor(ColorConstants.RED, true).SetLineWidth(1.5f).Rectangle(180, 531, 120, 48).Fill
                    ().RestoreState();
                annot.SetNormalAppearance(formN.GetPdfObject());
                new PdfAnnotationFlattener().Flatten(pdfDoc);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattened_DA_pdf_link.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenTextMarkupAnnotations() {
            String fileToFlatten = DESTINATION_FOLDER + "file_to_flatten_markup_text.pdf";
            String resultFile = DESTINATION_FOLDER + "flattened_markup_text.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(fileToFlatten))) {
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
            using (PdfDocument pdfDoc_1 = new PdfDocument(CompareTool.CreateOutputReader(fileToFlatten), CompareTool.CreateTestPdfWriter
                (resultFile))) {
                new PdfAnnotationFlattener().Flatten(pdfDoc_1.GetFirstPage().GetAnnotations());
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_text_markup_flatten.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FLATTENING_IS_NOT_YET_SUPPORTED)]
        public virtual void FlattenLinkAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenLinkAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenLinkAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(1, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenLinkAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FORMFIELD_ANNOTATION_WILL_NOT_BE_FLATTENED)]
        public virtual void FlattenWidgetAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenWidgetAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenWidgetAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                IList<PdfAnnotation> annot = flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(1, annot.Count);
                NUnit.Framework.Assert.AreEqual(1, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenWidgetAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenScreenAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenScreenAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenScreenAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenScreenAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void Flatten3DAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flatten3DAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flatten3DAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flatten3DAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenHighlightAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenHighlightAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenHighlightAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenHighlightAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenUnderlineAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenUnderlineAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenUnderlineAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenUnderlineAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenSquigglyAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenSquigglyAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenSquigglyAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenSquigglyAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenStrikeOutAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenStrikeOutAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenStrikeOutAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenStrikeOutAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenCaretAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenCaretAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenCaretAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenCaretAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenTextAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenTextAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenTextAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenTextAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenSoundAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenSoundAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenSoundAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenSoundAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenStampAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenStampAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenStampAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                NUnit.Framework.Assert.AreEqual(0, flattener.Flatten(document).Count);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenStampAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenFileAttachmentAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenFileAttachmentAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenFileAttachmentAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenFileAttachmentAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenInkAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenInkAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenInkAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenInkAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenPrinterMarkAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenPrinterMarkAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenPrinterMarkAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenPrinterMarkAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenTrapNetAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenTrapNetAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenTrapNetAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenTrapNetAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestWarnAnnotationFlattenerAnnotNull() {
            WarnFormfieldFlattener warnFormfieldFlattener = new WarnFormfieldFlattener();
            PdfPage page = null;
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                warnFormfieldFlattener.Flatten(null, page);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestWarnAnnotationFlattenerPageNull() {
            WarnFormfieldFlattener warnFormfieldFlattener = new WarnFormfieldFlattener();
            PdfAnnotation annot = new PdfCircleAnnotation(new Rectangle(100, 100, 100, 100));
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                warnFormfieldFlattener.Flatten(annot, null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void RemoveWithoutDrawingFormfieldFlattenerNull() {
            RemoveWithoutDrawingFlattener flattener = new RemoveWithoutDrawingFlattener();
            PdfPage page = null;
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                flattener.Flatten(null, page);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void RemoveWithoutDrawingAnnotationFlattenerPageNull() {
            RemoveWithoutDrawingFlattener warnFormfieldFlattener = new RemoveWithoutDrawingFlattener();
            PdfAnnotation annot = new PdfCircleAnnotation(new Rectangle(100, 100, 100, 100));
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                warnFormfieldFlattener.Flatten(annot, null);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void FlattenFreeTextAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenFreeTextAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenFreeTextAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenFreeTextAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenSquareAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenSquareAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenSquareAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenSquareAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenCircleAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenCircleAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenCircleAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenCircleAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlatteningOfAnnotationListWithNullAnnotationContinues() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                document.AddNewPage();
                List<PdfAnnotation> annots = new List<PdfAnnotation>();
                annots.Add(null);
                flattener.Flatten(annots);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FlatteningOfAnnotationListWithNoPageAttachedAnnotationContinues() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                document.AddNewPage();
                List<PdfAnnotation> annots = new List<PdfAnnotation>();
                annots.Add(new PdfCircleAnnotation(new Rectangle(100, 100, 100, 100)));
                flattener.Flatten(annots);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FlattenLineAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenLineAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenLineAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenLineAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenPolygonAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenPolygonAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenPolygonAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenPolygonAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenPolyLineAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenPolyLineAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenPolyLineAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenPolyLineAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenRedactAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenRedactAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenRedactAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenRedactAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenWatermarkAnnotationTest() {
            String sourceFile = SOURCE_FOLDER + "flattenWatermarkAnnotationTest.pdf";
            String resultFile = DESTINATION_FOLDER + "flattenWatermarkAnnotationTest.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFile), CompareTool.CreateTestPdfWriter(resultFile
                ))) {
                PdfAnnotationFlattener flattener = new PdfAnnotationFlattener();
                flattener.Flatten(document);
                NUnit.Framework.Assert.AreEqual(0, document.GetFirstPage().GetAnnotations().Count);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, SOURCE_FOLDER + "cmp_flattenWatermarkAnnotationTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
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

//\cond DO_NOT_DOCUMENT
        internal class CustomPdfAnnotationFlattenFactory : PdfAnnotationFlattenFactory {
            public override IAnnotationFlattener GetAnnotationFlattenWorker(PdfName name) {
                if (PdfName.Link.Equals(name)) {
                    return new _IAnnotationFlattener_872();
                }
                return base.GetAnnotationFlattenWorker(name);
            }

            private sealed class _IAnnotationFlattener_872 : IAnnotationFlattener {
                public _IAnnotationFlattener_872() {
                }

                public bool Flatten(PdfAnnotation annotation, PdfPage page) {
                    page.RemoveAnnotation(annotation);
                    return true;
                }
            }
        }
//\endcond
    }
}
