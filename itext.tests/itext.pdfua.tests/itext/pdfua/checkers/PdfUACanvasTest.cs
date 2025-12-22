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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Font.Otf;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfUACanvasTest : ExtendedITextTest {
        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly String FONT_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUACanvasTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextContentIsNotTagged(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().BeginText().SetFontAndSize(GetPdfFont(), 10).ShowText("Hello World!");
            }
            );
            framework.AssertBothFail("textContentIsNotTagged", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextNoContentIsNotTagged(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().BeginText().SetFontAndSize(GetPdfFont(), 10).EndText();
            }
            );
            framework.AssertBothValid("textNoContentIsNotTagged", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextContentIsCorrectlyTaggedAsContent(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(page1);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag(StandardRoles.P);
                canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200
                    , 200).ShowText("Hello World!").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothValid("01_005_TextContentIsCorrectlyTaggedAsContent", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextContentIsNotInTagTree(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                canvas.OpenTag(new CanvasTag(PdfName.P)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200, 200
                    );
                canvas.ShowText("Hello World!");
            }
            );
            framework.AssertBothFail("01_005_TextContentIsNotInTagTree", PdfUAExceptionMessageConstants.CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextArtifactIsNotInTagTree(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(page1);
                canvas.OpenTag(new CanvasTag(PdfName.Artifact)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(
                    200, 200).ShowText("Hello World!").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothValid("01_005_TextArtifactIsNotInTagTree", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextContentWithMCIDButNotInTagTree(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                canvas.OpenTag(new CanvasTag(PdfName.P, 99)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200
                    , 200);
                canvas.ShowText("Hello World!");
            }
            );
            framework.AssertBothFail("textContentWithMCIDButNotInTagTree", PdfUAExceptionMessageConstants.CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsTaggedButNotInTagTree(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.H1)).SetFontAndSize(font, 12).BeginText().MoveText(200, 200
                    ).SetColor(ColorConstants.RED, true);
                canvas.ShowText(glyphLine);
            }
            );
            framework.AssertBothFail("textGlyphLineContentIsTaggedButNotInTagTree", PdfUAExceptionMessageConstants.CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextGlyphLineInBadStructure(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new _PdfCanvas_235(pdfDoc.AddNewPage());
                // Disable the checkIsoConformance call check by simulating generating not tagged content
                // same as in annotations of form fields.
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
                canvas.ShowText(glyphLine);
            }
            );
            framework.AssertBothFail("textGlyphLineInBadStructure", PdfUAExceptionMessageConstants.REAL_CONTENT_INSIDE_ARTIFACT_OR_VICE_VERSA
                , false, pdfUAConformance);
        }

        private sealed class _PdfCanvas_235 : PdfCanvas {
            public _PdfCanvas_235(PdfPage baseArg1)
                : base(baseArg1) {
            }

            public override PdfCanvas OpenTag(CanvasTag tag) {
                this.SetDrawingOnPage(false);
                base.OpenTag(tag);
                this.SetDrawingOnPage(true);
                return this;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsArtifact(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFontAndSize(font, 12).BeginText().MoveText(
                    200, 200).SetColor(ColorConstants.RED, true).ShowText(glyphLine).EndText().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_TextGlyphLineContentIsArtifact", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsContentCorrect(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetFontAndSize(font, 12).BeginText().MoveText(200
                    , 200).SetColor(ColorConstants.RED, true).ShowText(glyphLine).EndText().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_TextGlyphLineContentIsContentCorrect", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_allowPureBmcInArtifact(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFontAndSize(font, 12).BeginMarkedContent(PdfName
                    .P).BeginText().MoveText(200, 200).SetColor(ColorConstants.RED, true).ShowText(glyphLine).EndMarkedContent
                    ().EndText().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_allowPureBmcInArtifact", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_allowNestedPureBmcInArtifact(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFontAndSize(font, 12).BeginMarkedContent(PdfName
                    .P).OpenTag(new CanvasTag(PdfName.Artifact)).BeginText().MoveText(200, 200).SetColor(ColorConstants.RED
                    , true).ShowText(glyphLine).CloseTag().EndMarkedContent().EndText().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_allowNestedPureBmcInArtifact", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsNotTagged(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.LineTo(200, 200).Fill();
            }
            );
            framework.AssertBothFail("lineContentThatIsContentIsNotTagged", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsNotTagged_noContent(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.LineTo(200, 200);
            }
            );
            framework.AssertBothValid("lineContentThatIsContentIsNotTagged_noContent", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsTaggedButIsNotAnArtifact(PdfUAConformance 
            pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                canvas.OpenTag(new CanvasTag(PdfName.P)).SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.LineTo(200, 200).Fill();
            }
            );
            framework.AssertBothFail("lineContentThatIsContentIsTaggedButIsNotAnArtifact", PdfUAExceptionMessageConstants
                .CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT, false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsTaggedButIsNotAnArtifact_no_drawing(PdfUAConformance
             pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                canvas.OpenTag(new CanvasTag(PdfName.P)).SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.LineTo(200, 200);
                canvas.LineTo(300, 200);
            }
            );
            framework.AssertBothValid("lineContentThatIsContentIsTaggedButIsNotAnArtifactNoDrawing", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_LineContentThatIsMarkedAsArtifact(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.OpenTag(tagPointer.GetTagReference()).SaveState().SetStrokeColor(ColorConstants.MAGENTA).MoveTo(300
                    , 300).LineTo(400, 350).Stroke().RestoreState().CloseTag();
            }
            );
            framework.AssertBothValid("01_005_LineContentThatIsMarkedAsArtifact", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleNotMarked(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.Fill();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_RectangleNotMarked", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleNoContent(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
            }
            );
            framework.AssertBothValid("checkPoint_01_005_RectangleNoContent", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleClip(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.Clip();
            }
            );
            framework.AssertBothValid("checkPoint_01_005_RectangleClip", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleClosePathStroke(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.ClosePathStroke();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_RectangleClosePathStroke", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_Rectangle_EOFIllStroke(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.ClosePathEoFillStroke();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_Rectangle_ClosPathEOFIllStroke", PdfUAExceptionMessageConstants
                .TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING, false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_Rectangle_FillStroke(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.FillStroke();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_Rectangle_FillStroke", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_Rectangle_eoFill(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.EoFill();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_Rectangle_eoFill", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_Rectangle_eoFillStroke(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.EoFillStroke();
            }
            );
            framework.AssertBothFail("checkPoint_01_005_Rectangle_eoFillStroke", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleMarkedArtifact(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFillColor(ColorConstants.RED).Rectangle(new 
                    Rectangle(200, 200, 100, 100)).Fill().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_RectangleMarkedArtifact", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleMarkedContentWithoutMcid(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P)).SetFillColor(ColorConstants.RED);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100)).Fill();
            }
            );
            framework.AssertBothFail("rectangleMarkedContentWithoutMcid", PdfUAExceptionMessageConstants.CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleMarkedContentWithoutMcid_NoContent(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P)).SetFillColor(ColorConstants.RED);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
            }
            );
            framework.AssertBothValid("checkPoint_01_005_RectangleMarkedContentWithoutMcid_NoContent", pdfUAConformance
                );
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleMarkedContent(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetFillColor(ColorConstants.RED).Rectangle(new Rectangle
                    (200, 200, 100, 100)).Fill().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_RectangleMarkedContent", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_004_bezierMarkedAsContent(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .DIV);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetColor(ColorConstants.RED, true).SetLineWidth(5
                    ).SetStrokeColor(ColorConstants.RED).Arc(400, 400, 500, 500, 30, 50).Stroke().CloseTag().RestoreState(
                    );
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("01_004_bezierCurveShouldBeTagged", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("01_004_bezierCurveShouldBeTagged", MessageFormatUtil.Format(KernelExceptionMessageConstant
                        .PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED, "Div", "CONTENT"), pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_004_bezierMarkedAsArtifact(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetColor(ColorConstants.RED, true).SetLineWidth
                    (5).SetStrokeColor(ColorConstants.RED).Arc(400, 400, 500, 500, 30, 50).Stroke().CloseTag().RestoreState
                    ();
            }
            );
            framework.AssertBothValid("01_004_bezierMarkedAsArtifact", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_004_bezierCurveInvalidMCID(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P, 420)).SetColor(ColorConstants.RED, true).SetLineWidth(
                    5).MoveTo(20, 20).LineTo(300, 300).SetStrokeColor(ColorConstants.RED).Fill();
            }
            );
            framework.AssertBothFail("checkPoint_01_004_bezierCurveInvalidMCID", PdfUAExceptionMessageConstants.CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_004_bezierCurveInvalidMCID_NoContent(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P, 420)).SetColor(ColorConstants.RED, true).SetLineWidth(
                    5).MoveTo(20, 20).LineTo(300, 300).SetStrokeColor(ColorConstants.RED);
            }
            );
            framework.AssertBothValid("checkPoint_01_004_bezierCurveInvalidMCID_NoContent", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RandomOperationsWithoutActuallyAddingContent(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineCapStyle(1).SetTextMatrix(20, 2).SetLineWidth(2);
            }
            );
            framework.AssertBothValid("01_005_RandomOperationsWithoutActuallyAddingContent", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_003_ContentMarkedAsArtifactsPresentInsideTaggedContent(PdfUAConformance 
            pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag(StandardRoles.P);
                canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200
                    , 200).ShowText("Hello World!").EndText();
                canvas.OpenTag(new CanvasTag(PdfName.Artifact));
            }
            );
            framework.AssertBothFail("contentMarkedAsArtifactsInsideTaggedContent", PdfUAExceptionMessageConstants.ARTIFACT_CANT_BE_INSIDE_REAL_CONTENT
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContent(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                // Have to use low level tagging, otherwise it throws error earlier.
                pdfDoc.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem paragraph = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid
                    (new PdfStructElem(pdfDoc, PdfName.P, page1)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0
                    ]).AddKid(new PdfStructElem(pdfDoc, PdfName.P, page1));
                PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
                canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginMarkedContent(PdfName.P).BeginText().SetFontAndSize(font
                    , 12).MoveText(200, 200).ShowText("Hello World!").EndText().EndMarkedContent().RestoreState().CloseTag
                    ();
            }
            );
            framework.AssertBothValid("validRoleAddedInsideMarkedContent", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContentMultiple(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                // Have to use low level tagging, otherwise it throws error earlier.
                pdfDoc.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem paragraph = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid
                    (new PdfStructElem(pdfDoc, PdfName.P, page1)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0
                    ]).AddKid(new PdfStructElem(pdfDoc, PdfName.P, page1));
                PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
                canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginMarkedContent(PdfName.P).BeginText().SetFontAndSize(font
                    , 12).MoveText(200, 200).ShowText("Hello World!").EndText().EndMarkedContent().BeginMarkedContent(PdfName
                    .H1).BeginText().ShowText("Hello but nested").EndText().EndMarkedContent().RestoreState().CloseTag();
            }
            );
            framework.AssertBothValid("validRoleAddedInsideMarkedContentMultiple", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContentMCR_IN_MCR(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                pdfDoc.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem paragraph = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid
                    (new PdfStructElem(pdfDoc, PdfName.P, page1)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0
                    ]).AddKid(new PdfStructElem(pdfDoc, PdfName.P, page1));
                PdfStructElem paragraph2 = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid
                    (new PdfStructElem(pdfDoc, PdfName.P, page1)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0
                    ]).AddKid(new PdfStructElem(pdfDoc, PdfName.P, page1));
                PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
                PdfMcr mcr1 = paragraph2.AddKid(new PdfMcrNumber(page1, paragraph2));
                canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginMarkedContent(PdfName.P).BeginText().SetFontAndSize(font
                    , 12).MoveText(200, 200).ShowText("Hello World!").EndText().EndMarkedContent().OpenTag(new CanvasTag(mcr1
                    )).BeginMarkedContent(PdfName.H1).BeginText().ShowText("Hello but nested").EndText().EndMarkedContent(
                    ).CloseTag().RestoreState().CloseTag();
            }
            );
            framework.AssertOnlyVeraPdfFail("validRoleAddedInsideMarkedContentMCR_IN_MCR", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_004_TaggedContentShouldNotBeInsideArtifact(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag(StandardRoles.P);
                canvas.OpenTag(new CanvasTag(PdfName.Artifact)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(
                    200, 200).ShowText("Hello World!").EndText();
                canvas.OpenTag(tagPointer.GetTagReference());
            }
            );
            framework.AssertBothFail("taggedContentShouldNotBeInsideArtifact", PdfUAExceptionMessageConstants.REAL_CONTENT_CANT_BE_INSIDE_ARTIFACT
                , false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_31_009_FontIsNotEmbedded(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                tagPointer.SetPageForTagging(pdfDoc.GetFirstPage());
                tagPointer.AddTag(StandardRoles.P);
                canvas.BeginText().OpenTag(tagPointer.GetTagReference()).SetFontAndSize(font, 12).ShowText("Please crash on close, tyvm"
                    ).EndText().CloseTag();
            }
            );
            framework.AssertBothFail("31_009_FontIsNotEmbedded", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .FONT_SHOULD_BE_EMBEDDED, "Courier"), false, pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_19_003_iDEntryInNoteTagIsNotPresent(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
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
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("invalidNoteTag02", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY, 
                    pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("invalidNoteTag02", PdfUAExceptionMessageConstants.DOCUMENT_USES_NOTE_TAG, pdfUAConformance
                        );
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_19_003_validNoteTagIsPresent(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                pdfDocument.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem paragraph = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? pdfDocument.GetStructTreeRoot().
                    AddKid(new PdfStructElem(pdfDocument, PdfName.P, page1)) : ((PdfStructElem)pdfDocument.GetStructTreeRoot
                    ().GetKids()[0]).AddKid(new PdfStructElem(pdfDocument, PdfName.P, page1));
                PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
                PdfStructElem note = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? pdfDocument.GetStructTreeRoot().AddKid
                    (new PdfStructElem(pdfDocument, PdfName.Note, page1)) : ((PdfStructElem)pdfDocument.GetStructTreeRoot(
                    ).GetKids()[0]).AddKid(new PdfStructElem(pdfDocument, PdfName.Note, page1));
                note.Put(PdfName.ID, new PdfString("1"));
                canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                    ("Hello World!").EndText().RestoreState().CloseTag();
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("validNoteTagPresent", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("validNoteTagPresent", PdfUAExceptionMessageConstants.DOCUMENT_USES_NOTE_TAG, pdfUAConformance
                        );
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void UsingCharacterWithoutUnicodeMappingTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
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
                .GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE, " "), false, pdfUAConformance);
        }

        private static PdfFont GetPdfFont() {
            PdfFont font = null;
            try {
                font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException) {
                throw new Exception();
            }
            return font;
        }
    }
}
