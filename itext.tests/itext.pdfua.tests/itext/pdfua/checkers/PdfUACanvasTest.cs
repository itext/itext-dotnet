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
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfua.Checkers {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua
    // validation on Android)
    [NUnit.Framework.Category("UnitTest")]
    public class PdfUACanvasTest : ExtendedITextTest {
        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly String FONT_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUACanvasTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUACanvasTest/";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextContentIsNotTagged() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().BeginText().SetFontAndSize(GetFont(), 10).ShowText("Hello World!");
            }
            );
            framework.AssertBothFail("checkPoint_01_005_TextContentIsNotTagged", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextNoContentIsNotTagged() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().BeginText().SetFontAndSize(GetFont(), 10).EndText();
            }
            );
            framework.AssertBothValid("checkPoint_01_005_TextNoContentIsNotTagged");
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextContentIsCorrectlyTaggedAsContent() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextContentIsCorrectlyTaggedAsContent.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextContentIsNotInTagTree() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextContentIsNotInTagTree.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.OpenTag(new CanvasTag(PdfName.Artifact)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(
                200, 200).ShowText("Hello World!").EndText().RestoreState().CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_TextArtifactIsNotInTagTree.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextContentWithMCIDButNotInTagTree() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextContentWithMCIDButNotInTagTree.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
        public virtual void CheckPoint_01_005_TextGlyphLineInBadStructure() {
            String outPdf = DESTINATION_FOLDER + "checkPoint_01_005_TextGlyphLineInBadStructure.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfCanvas canvas = new _PdfCanvas_249(pdfDoc.AddNewPage());
            // disable the checkIsoConformance call check by simulating  generating not tagged content
            // same as in annotations of formfields.
            GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
            TagTreePointer pointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
            pointer.AddTag(StandardRoles.DIV);
            pointer.SetPageForTagging(pdfDoc.GetFirstPage());
            canvas.SaveState();
            canvas.OpenTag(pointer.GetTagReference());
            canvas.OpenTag(new CanvasArtifact());
            pointer.AddTag(StandardRoles.P);
            canvas.OpenTag(pointer.GetTagReference());
            canvas.SetFontAndSize(font, 12);
            canvas.BeginText();
            canvas.MoveText(200, 200);
            canvas.SetColor(ColorConstants.RED, true);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                canvas.ShowText(glyphLine);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.REAL_CONTENT_INSIDE_ARTIFACT_OR_VICE_VERSA, 
                e.Message);
        }

        private sealed class _PdfCanvas_249 : PdfCanvas {
            public _PdfCanvas_249(PdfPage baseArg1)
                : base(baseArg1) {
            }

            public override PdfCanvas OpenTag(CanvasTag tag) {
                this.SetDrawingOnPage(false);
                base.OpenTag(tag);
                this.SetDrawingOnPage(true);
                return this;
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextGlyphLineContentIsArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFontAndSize(font, 12).BeginText().MoveText(
                200, 200).SetColor(ColorConstants.RED, true).ShowText(glyphLine).EndText().CloseTag().RestoreState();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_TextGlyphLineContentIsArtifact.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsContentCorrect() {
            String outPdf = DESTINATION_FOLDER + "01_005_TextGlyphLineContentIsContentCorrect.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_allowPureBmcInArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_005_allowPureBmcInArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_allowNestedPureBmcInArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_005_allowNestedPureBmcInArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsNotTagged() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.LineTo(200, 200).Fill();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_LineContentThatIsContentIsNotTagged", PdfUAExceptionMessageConstants
                .TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING, false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsNotTagged_noContent() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.LineTo(200, 200);
            }
            );
            framework.AssertBothValid("checkPoint_01_005_LineContentThatIsContentIsNotTagged_noContent");
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsTaggedButIsNotAnArtifact() {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                canvas.OpenTag(new CanvasTag(PdfName.P)).SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.LineTo(200, 200).Fill();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_LineContentThatIsContentIsTaggedButIsNotAnArtifact", PdfUAExceptionMessageConstants
                .CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT, false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsTaggedButIsNotAnArtifact_no_drawing() {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                canvas.OpenTag(new CanvasTag(PdfName.P)).SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.LineTo(200, 200);
                canvas.LineTo(300, 200);
            }
            );
            framework.AssertBothValid("checkPoint_01_005_LineContentThatIsContentIsTaggedButIsNotAnArtifact_no_drawing"
                );
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_LineContentThatIsMarkedAsArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_005_LineContentThatIsMarkedAsArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                .H);
            canvas.OpenTag(tagPointer.GetTagReference()).SaveState().SetStrokeColor(ColorConstants.MAGENTA).MoveTo(300
                , 300).LineTo(400, 350).Stroke().RestoreState().CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_LineContentThatIsMarkedAsArtifact.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleNotMarked() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.Fill();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_RectangleNotMarked", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleNoContent() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
            }
            );
            framework.AssertBothValid("checkPoint_01_005_RectangleNoContent");
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleClip() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.Clip();
            }
            );
            framework.AssertBothValid("checkPoint_01_005_RectangleNoContent");
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleClosePathStroke() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.ClosePathStroke();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_RectangleClosePathStroke", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_Rectangle_EOFIllStroke() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.ClosePathEoFillStroke();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_Rectangle_ClosPathEOFIllStroke", PdfUAExceptionMessageConstants
                .TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING, false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_Rectangle_FillStroke() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.FillStroke();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_Rectangle_FillStroke", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_Rectangle_eoFill() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.EoFill();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_Rectangle_eoFill", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_Rectangle_eoFillStroke() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.EoFillStroke();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_Rectangle_eoFillStroke", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleMarkedArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_005_RectangleMarkedArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFillColor(ColorConstants.RED).Rectangle(new 
                Rectangle(200, 200, 100, 100)).Fill().CloseTag().RestoreState();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_RectangleMarkedArtifact.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleMarkedContentWithoutMcid() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P)).SetFillColor(ColorConstants.RED);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100)).Fill();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_RectangleMarkedContentWithoutMcid", PdfUAExceptionMessageConstants
                .CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT, false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleMarkedContentWithoutMcid_NoContent() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P)).SetFillColor(ColorConstants.RED);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
            }
            );
            framework.AssertBothValid("checkPoint_01_005_RectangleMarkedContentWithoutMcid_NoContent");
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RectangleMarkedContent() {
            String outPdf = DESTINATION_FOLDER + "01_005_RectangleMarkedContent.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                .H);
            canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetFillColor(ColorConstants.RED).Rectangle(new Rectangle
                (200, 200, 100, 100)).Fill().CloseTag().RestoreState();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_RectangleMarkedContent.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_004_bezierMarkedAsContent() {
            String outPdf = DESTINATION_FOLDER + "01_004_bezierCurveShouldBeTagged.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                .DIV);
            canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetColor(ColorConstants.RED, true).SetLineWidth(5
                ).SetStrokeColor(ColorConstants.RED).Arc(400, 400, 500, 500, 30, 50).Stroke().CloseTag().RestoreState(
                );
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_004_bezierCurveShouldBeTagged.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_004_bezierMarkedAsArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_004_bezierMarkedAsArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetColor(ColorConstants.RED, true).SetLineWidth
                (5).SetStrokeColor(ColorConstants.RED).Arc(400, 400, 500, 500, 30, 50).Stroke().CloseTag().RestoreState
                ();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_004_bezierMarkedAsArtifact.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_004_bezierCurveInvalidMCID() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P, 420)).SetColor(ColorConstants.RED, true).SetLineWidth(
                    5).MoveTo(20, 20).LineTo(300, 300).SetStrokeColor(ColorConstants.RED).Fill();
            }
            );
            framework.AssertBothFail("checkPoint_01_004_bezierCurveInvalidMCID", PdfUAExceptionMessageConstants.CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_004_bezierCurveInvalidMCID_NoContent() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P, 420)).SetColor(ColorConstants.RED, true).SetLineWidth(
                    5).MoveTo(20, 20).LineTo(300, 300).SetStrokeColor(ColorConstants.RED);
            }
            );
            framework.AssertBothValid("checkPoint_01_004_bezierCurveInvalidMCID_NoContent");
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_005_RandomOperationsWithoutActuallyAddingContent() {
            String outPdf = DESTINATION_FOLDER + "01_005_RandomOperationsWithoutActuallyAddingContent.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.SetColor(ColorConstants.RED, true).SetLineCapStyle(1).SetTextMatrix(20, 2).SetLineWidth(2);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_01_005_RandomOperationsWithoutActuallyAddingContent.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_003_ContentMarkedAsArtifactsPresentInsideTaggedContent() {
            String outPdf = DESTINATION_FOLDER + "01_003_ContentMarkedAsArtifactsPresentInsideTaggedContent.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContentMultiple() {
            String outPdf = DESTINATION_FOLDER + "validRoleAddedInsideMarkedContentMultiple.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_validRoleAddedInsideMarkedContentMultiple.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContentMCR_IN_MCR() {
            String outPdf = DESTINATION_FOLDER + "validRoleAddedInsideMarkedContentMCR_IN_MCR.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_validRoleAddedInsideMarkedContentMCR_IN_MCR.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_01_004_TaggedContentShouldNotBeInsideArtifact() {
            String outPdf = DESTINATION_FOLDER + "01_004_TaggedContentShouldNotBeInsideArtifact.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream()));
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

        [NUnit.Framework.Test]
        public virtual void CheckPoint_19_003_iDEntryInNoteTagIsNotPresent() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
                PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P, page1));
                PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
                doc.AddKid(new PdfStructElem(pdfDoc, PdfName.Note, page1));
                canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                    ("Hello World!").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothFail("invalidNoteTag02", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY);
        }

        [NUnit.Framework.Test]
        public virtual void CheckPoint_19_003_validNoteTagIsPresent() {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                PdfPage page1 = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                PdfStructElem doc = pdfDocument.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDocument, PdfName.Document
                    ));
                PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDocument, PdfName.P, page1));
                PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
                PdfStructElem note = doc.AddKid(new PdfStructElem(pdfDocument, PdfName.Note, page1));
                note.Put(PdfName.ID, new PdfString("1"));
                canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                    ("Hello World!").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothValid("validNoteTagPresent");
            String outPdf = DESTINATION_FOLDER + "layout_validNoteTagPresent.pdf";
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_validNoteTagPresent.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void UsingCharacterWithoutUnicodeMappingTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(FONT_FOLDER + "cmr10.afm", FONT_FOLDER
                         + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                PdfPage page = pdfDoc.AddNewPage();
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page).AddTag(StandardRoles.P);
                new PdfCanvas(page).OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().MoveText(36, 700).SetFontAndSize
                    (font, 72)
                                // space symbol isn't defined in the font
                                .ShowText("Hello world").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothFail("usingCharacterWithoutUnicodeMappingTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE, " "), false);
        }

        private PdfFont GetFont() {
            try {
                return PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException) {
                throw new Exception();
            }
        }
    }
}
