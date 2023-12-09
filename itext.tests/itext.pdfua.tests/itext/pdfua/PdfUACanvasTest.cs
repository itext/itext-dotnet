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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfua {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfUACanvasTest : ExtendedITextTest {
        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUACanvasTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUACanvasTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextContentIsNotTagged() {
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument
                .CreateWriterProperties()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.ShowText("Hello World!");
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING, 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextContentIsCorrectlyTaggedAsContent() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextContentIsCorrectlyTaggedAsContent.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag(StandardRoles.P);
            canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200
                , 200).ShowText("Hello World!").EndText().RestoreState().CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_TextContentIsCorrectlyTaggedAsContent.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextContentIsNotInTagTree() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextContentIsNotInTagTree.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.OpenTag(new CanvasTag(PdfName.P)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200, 200
                );
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.ShowText("Hello World!");
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextArtifactIsNotInTagTree() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextArtifactIsNotInTagTree.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.OpenTag(new CanvasTag(PdfName.Artifact)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(
                200, 200).ShowText("Hello World!").EndText().RestoreState().CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_TextArtifactIsNotInTagTree.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextContentWithMCIDButNotInTagTree() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextContentWithMCIDButNotInTagTree.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.OpenTag(new CanvasTag(PdfName.P, 99)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200
                , 200);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.ShowText("Hello World!");
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsTaggedButNotInTagTree() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextGlyphLineContentIsTagged.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.H1)).SetFontAndSize(font, 12).BeginText().MoveText(200, 200
                ).SetColor(ColorConstants.RED, true);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.ShowText(glyphLine);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextGlyphLineContentIsArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFontAndSize(font, 12).BeginText().MoveText(
                200, 200).SetColor(ColorConstants.RED, true).ShowText(glyphLine).EndText().CloseTag().RestoreState();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_TextGlyphLineContentIsArtifact.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsContentCorrect() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextGlyphLineContentIsContentCorrect.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                .H1);
            canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetFontAndSize(font, 12).BeginText().MoveText(200
                , 200).SetColor(ColorConstants.RED, true).ShowText(glyphLine).EndText().CloseTag().RestoreState();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_TextGlyphLineContentIsContentCorrect.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_allowPureBmcInArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_005_allowPureBmcInArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFontAndSize(font, 12).BeginMarkedContent(PdfName
                .P).BeginText().MoveText(200, 200).SetColor(ColorConstants.RED, true).ShowText(glyphLine).EndMarkedContent
                ().EndText().CloseTag().RestoreState();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_allowPureBmcInArtifact.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_allowNestedPureBmcInArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_005_allowNestedPureBmcInArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFontAndSize(font, 12).BeginMarkedContent(PdfName
                .P).OpenTag(new CanvasTag(PdfName.Artifact)).BeginText().MoveText(200, 200).SetColor(ColorConstants.RED
                , true).ShowText(glyphLine).CloseTag().EndMarkedContent().EndText().CloseTag().RestoreState();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_allowNestedPureBmcInArtifact.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsNotTagged() {
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument
                .CreateWriterProperties()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.LineTo(200, 200);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING, 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsTaggedButIsNotAnArtifact() {
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument
                .CreateWriterProperties()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.OpenTag(new CanvasTag(PdfName.H1)).SetColor(ColorConstants.RED, true).SetLineWidth(2);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.LineTo(200, 200);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_LineContentThatIsMarkedAsArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_005_LineContentThatIsMarkedAsArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                .H);
            canvas.OpenTag(tagPointer.GetTagReference()).SaveState().SetStrokeColor(ColorConstants.MAGENTA).MoveTo(300
                , 300).LineTo(400, 350).Stroke().RestoreState().CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_LineContentThatIsMarkedAsArtifact.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleNotMarked() {
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument
                .CreateWriterProperties()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING, 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleMarkedArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_005_RectangleMarkedArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFillColor(ColorConstants.RED).Rectangle(new 
                Rectangle(200, 200, 100, 100)).Fill().CloseTag().RestoreState();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_RectangleMarkedArtifact.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleMarkedContentWithoutMcid() {
            String outPdf = DESTINATION_FOLDER + "01_005_RectangleMarkedContentWithoutMcid.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.Rect)).SetFillColor(ColorConstants.RED);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleMarkedContent() {
            String outPdf = DESTINATION_FOLDER + "01_005_RectangleMarkedContent.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                .H);
            canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetFillColor(ColorConstants.RED).Rectangle(new Rectangle
                (200, 200, 100, 100)).Fill().CloseTag().RestoreState();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_RectangleMarkedContent.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_004_bezierMarkedAsContent() {
            String outPdf = DESTINATION_FOLDER + "01_004_bezierCurveShouldBeTagged.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                .DIV);
            canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetColor(ColorConstants.RED, true).SetLineWidth(5
                ).SetStrokeColor(ColorConstants.RED).Arc(400, 400, 500, 500, 30, 50).Stroke().CloseTag().RestoreState(
                );
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_004_bezierCurveShouldBeTagged.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_004_bezierMarkedAsArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_004_bezierMarkedAsArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetColor(ColorConstants.RED, true).SetLineWidth
                (5).SetStrokeColor(ColorConstants.RED).Arc(400, 400, 500, 500, 30, 50).Stroke().CloseTag().RestoreState
                ();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_004_bezierMarkedAsArtifact.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_004_bezierCurveInvalidMCID() {
            String outPdf = DESTINATION_FOLDER + "01_004_bezierCurveInvalidMCID.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.P, 420)).SetColor(ColorConstants.RED, true).SetLineWidth(
                5).SetStrokeColor(ColorConstants.RED);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.Arc(400, 400, 500, 500, 30, 50);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RandomOperationsWithoutActuallyAddingContent() {
            String outPdf = DESTINATION_FOLDER + "01_005_RandomOperationsWithoutActuallyAddingContent.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetColor(ColorConstants.RED, true).SetLineCapStyle(1).SetTextMatrix(20, 2).SetLineWidth(2);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_RandomOperationsWithoutActuallyAddingContent.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_003_ContentMarkedAsArtifactsPresentInsideTaggedContent() {
            String outPdf = DESTINATION_FOLDER + "01_003_ContentMarkedAsArtifactsPresentInsideTaggedContent.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag(StandardRoles.P);
            canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200
                , 200).ShowText("Hello World!").EndText();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.OpenTag(new CanvasTag(PdfName.Artifact));
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.ARTIFACT_CANT_BE_INSIDE_REAL_CONTENT, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContent() {
            String outPdf = DESTINATION_FOLDER + "validRoleAddedInsideMarkedContent.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Have to use low level tagging otherwise it throws error earlier
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P, page1));
            PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
            canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginMarkedContent(PdfName.P).BeginText().SetFontAndSize(font
                , 12).MoveText(200, 200).ShowText("Hello World!").EndText().EndMarkedContent().RestoreState().CloseTag
                ();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_validRoleAddedInsideMarkedContent.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContentMultiple() {
            String outPdf = DESTINATION_FOLDER + "validRoleAddedInsideMarkedContentMultiple.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Have to use low level tagging otherwise it throws error earlier
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P, page1));
            PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
            canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginMarkedContent(PdfName.P).BeginText().SetFontAndSize(font
                , 12).MoveText(200, 200).ShowText("Hello World!").EndText().EndMarkedContent().BeginMarkedContent(PdfName
                .H1).BeginText().ShowText("Hello but nested").EndText().EndMarkedContent().RestoreState().CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_validRoleAddedInsideMarkedContentMultiple.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContentMCR_IN_MCR() {
            String outPdf = DESTINATION_FOLDER + "validRoleAddedInsideMarkedContentMCR_IN_MCR.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P, page1));
            PdfStructElem paragraph2 = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P, page1));
            PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
            PdfMcr mcr1 = paragraph2.AddKid(new PdfMcrNumber(page1, paragraph2));
            canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginMarkedContent(PdfName.P).BeginText().SetFontAndSize(font
                , 12).MoveText(200, 200).ShowText("Hello World!").EndText().EndMarkedContent().OpenTag(new CanvasTag(mcr1
                )).BeginMarkedContent(PdfName.H1).BeginText().ShowText("Hello but nested").EndText().EndMarkedContent(
                ).CloseTag().RestoreState().CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_validRoleAddedInsideMarkedContentMCR_IN_MCR.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_004_TaggedContentShouldNotBeInsideArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_004_TaggedContentShouldNotBeInsideArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag(StandardRoles.P);
            canvas.OpenTag(new CanvasTag(PdfName.Artifact)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(
                200, 200).ShowText("Hello World!").EndText();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.OpenTag(tagPointer.GetTagReference());
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.REAL_CONTENT_CANT_BE_INSIDE_ARTIFACT, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_31_009_FontIsNotEmbedded() {
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), PdfUATestPdfDocument
                .CreateWriterProperties()));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
            tagPointer.SetPageForTagging(pdfDoc.GetFirstPage());
            tagPointer.AddTag(StandardRoles.P);
            canvas.BeginText().OpenTag(tagPointer.GetTagReference()).SetFontAndSize(font, 12).ShowText("Please crash on close, tyvm"
                ).EndText().CloseTag();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                pdfDoc.Close();
            }
            );
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.FONT_SHOULD_BE_EMBEDDED
                , "Courier"), e.Message);
        }
    }
}
