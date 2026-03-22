/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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

        public static IList<PdfConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextContentIsNotTagged(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().BeginText().SetFontAndSize(GetPdfFont(), 10).ShowText("Hello World!");
            }
            );
            framework.AssertBothFail("textContentIsNotTagged", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextNoContentIsNotTagged(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().BeginText().SetFontAndSize(GetPdfFont(), 10).EndText();
            }
            );
            framework.AssertBothValid("textNoContentIsNotTagged");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextContentIsCorrectlyTaggedAsContent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(page1);
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag(StandardRoles.P);
                canvas.OpenTag(tagPointer.GetTagReference()).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200
                    , 200).ShowText("Hello World!").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothValid("01_005_TextContentIsCorrectlyTaggedAsContent");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextContentIsNotInTagTree(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextArtifactIsNotInTagTree(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(page1);
                canvas.OpenTag(new CanvasTag(PdfName.Artifact)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(
                    200, 200).ShowText("Hello World!").EndText().RestoreState().CloseTag();
            }
            );
            framework.AssertBothValid("01_005_TextArtifactIsNotInTagTree");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextContentWithMCIDButNotInTagTree(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsTaggedButNotInTagTree(PdfConformance conformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextGlyphLineInBadStructure(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new _PdfCanvas_197(pdfDoc.AddNewPage());
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
                , false);
        }

        private sealed class _PdfCanvas_197 : PdfCanvas {
            public _PdfCanvas_197(PdfPage baseArg1)
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
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsArtifact(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFontAndSize(font, 12).BeginText().MoveText(
                    200, 200).SetColor(ColorConstants.BLUE, true).ShowText(glyphLine).EndText().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_TextGlyphLineContentIsArtifact");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_TextGlyphLineContentIsContentCorrect(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetFontAndSize(font, 12).BeginText().MoveText(200
                    , 200).SetColor(ColorConstants.BLUE, true).ShowText(glyphLine).EndText().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_TextGlyphLineContentIsContentCorrect");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_allowPureBmcInArtifact(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFontAndSize(font, 12).BeginMarkedContent(PdfName
                    .P).BeginText().MoveText(200, 200).SetColor(ColorConstants.BLUE, true).ShowText(glyphLine).EndMarkedContent
                    ().EndText().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_allowPureBmcInArtifact");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_allowNestedPureBmcInArtifact(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                GlyphLine glyphLine = font.CreateGlyphLine("Hello World!");
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFontAndSize(font, 12).BeginMarkedContent(PdfName
                    .P).OpenTag(new CanvasTag(PdfName.Artifact)).BeginText().MoveText(200, 200).SetColor(ColorConstants.BLUE
                    , true).ShowText(glyphLine).CloseTag().EndMarkedContent().EndText().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_allowNestedPureBmcInArtifact");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsNotTagged(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.LineTo(200, 200).Fill();
            }
            );
            framework.AssertBothFail("lineContentThatIsContentIsNotTagged", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsNotTagged_noContent(PdfConformance conformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.MoveTo(0, 0);
                canvas.LineTo(200, 200);
            }
            );
            framework.AssertBothValid("lineContentThatIsContentIsNotTagged_noContent");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsTaggedButIsNotAnArtifact(PdfConformance conformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                canvas.OpenTag(new CanvasTag(PdfName.P)).SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.LineTo(200, 200).Fill();
            }
            );
            framework.AssertBothFail("lineContentThatIsContentIsTaggedButIsNotAnArtifact", PdfUAExceptionMessageConstants
                .CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT, false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_LineContentThatIsContentIsTaggedButIsNotAnArtifact_no_drawing(PdfConformance
             conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                canvas.OpenTag(new CanvasTag(PdfName.P)).SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.MoveTo(0, 0);
                canvas.LineTo(200, 200);
                canvas.LineTo(300, 200);
            }
            );
            framework.AssertBothValid("lineContentThatIsContentIsTaggedButIsNotAnArtifactNoDrawing");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_LineContentThatIsMarkedAsArtifact(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.OpenTag(tagPointer.GetTagReference()).SaveState().SetStrokeColor(ColorConstants.MAGENTA).MoveTo(300
                    , 300).LineTo(400, 350).Stroke().RestoreState().CloseTag();
            }
            );
            framework.AssertBothValid("01_005_LineContentThatIsMarkedAsArtifact");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleNotMarked(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleNoContent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
            }
            );
            framework.AssertBothValid("checkPoint_01_005_RectangleNoContent");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleClip(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineWidth(2);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
                canvas.Clip();
            }
            );
            framework.AssertBothValid("checkPoint_01_005_RectangleClip");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleClosePathStroke(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_Rectangle_EOFIllStroke(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_Rectangle_FillStroke(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_Rectangle_eoFill(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_Rectangle_eoFillStroke(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleMarkedArtifact(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetFillColor(ColorConstants.RED).Rectangle(new 
                    Rectangle(200, 200, 100, 100)).Fill().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_RectangleMarkedArtifact");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleMarkedContentWithoutMcid(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P)).SetFillColor(ColorConstants.RED);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100)).Fill();
            }
            );
            framework.AssertBothFail("rectangleMarkedContentWithoutMcid", PdfUAExceptionMessageConstants.CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleMarkedContentWithoutMcid_NoContent(PdfConformance conformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P)).SetFillColor(ColorConstants.RED);
                canvas.Rectangle(new Rectangle(200, 200, 100, 100));
            }
            );
            framework.AssertBothValid("checkPoint_01_005_RectangleMarkedContentWithoutMcid_NoContent");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RectangleMarkedContent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .H1);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetFillColor(ColorConstants.RED).Rectangle(new Rectangle
                    (200, 200, 100, 100)).Fill().CloseTag().RestoreState();
            }
            );
            framework.AssertBothValid("01_005_RectangleMarkedContent");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_004_bezierMarkedAsContent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(pdfDoc.GetFirstPage()).AddTag(StandardRoles
                    .DIV);
                canvas.SaveState().OpenTag(tagPointer.GetTagReference()).SetColor(ColorConstants.RED, true).SetLineWidth(5
                    ).SetStrokeColor(ColorConstants.RED).Arc(400, 400, 500, 500, 30, 50).Stroke().CloseTag().RestoreState(
                    );
            }
            );
            if (conformance == PdfConformance.PDF_UA_1) {
                framework.AssertBothValid("01_004_bezierCurveShouldBeTagged");
            }
            else {
                if (conformance == PdfConformance.PDF_UA_2) {
                    framework.AssertBothFail("01_004_bezierCurveShouldBeTagged", MessageFormatUtil.Format(KernelExceptionMessageConstant
                        .PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED, "Div", "CONTENT"));
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_004_bezierMarkedAsArtifact(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.Artifact)).SetColor(ColorConstants.RED, true).SetLineWidth
                    (5).SetStrokeColor(ColorConstants.RED).Arc(400, 400, 500, 500, 30, 50).Stroke().CloseTag().RestoreState
                    ();
            }
            );
            framework.AssertBothValid("01_004_bezierMarkedAsArtifact");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_004_bezierCurveInvalidMCID(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P, 420)).SetColor(ColorConstants.RED, true).SetLineWidth(
                    5).MoveTo(20, 20).LineTo(300, 300).SetStrokeColor(ColorConstants.RED).Fill();
            }
            );
            framework.AssertBothFail("checkPoint_01_004_bezierCurveInvalidMCID", PdfUAExceptionMessageConstants.CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_004_bezierCurveInvalidMCID_NoContent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SaveState().OpenTag(new CanvasTag(PdfName.P, 420)).SetColor(ColorConstants.RED, true).SetLineWidth(
                    5).MoveTo(20, 20).LineTo(300, 300).SetStrokeColor(ColorConstants.RED);
            }
            );
            framework.AssertBothValid("checkPoint_01_004_bezierCurveInvalidMCID_NoContent");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_005_RandomOperationsWithoutActuallyAddingContent(PdfConformance conformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetColor(ColorConstants.RED, true).SetLineCapStyle(1).SetTextMatrix(20, 2).SetLineWidth(2);
            }
            );
            framework.AssertBothValid("01_005_RandomOperationsWithoutActuallyAddingContent");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_003_ContentMarkedAsArtifactsPresentInsideTaggedContent(PdfConformance conformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                // Have to use low level tagging, otherwise it throws error earlier.
                pdfDoc.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem paragraph = conformance == PdfConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem
                    (pdfDoc, PdfName.P, page1)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(new PdfStructElem
                    (pdfDoc, PdfName.P, page1));
                PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
                canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginMarkedContent(PdfName.P).BeginText().SetFontAndSize(font
                    , 12).MoveText(200, 200).ShowText("Hello World!").EndText().EndMarkedContent().RestoreState().CloseTag
                    ();
            }
            );
            framework.AssertBothValid("validRoleAddedInsideMarkedContent");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContentMultiple(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                // Have to use low level tagging, otherwise it throws error earlier.
                pdfDoc.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem paragraph = conformance == PdfConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem
                    (pdfDoc, PdfName.P, page1)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(new PdfStructElem
                    (pdfDoc, PdfName.P, page1));
                PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
                canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginMarkedContent(PdfName.P).BeginText().SetFontAndSize(font
                    , 12).MoveText(200, 200).ShowText("Hello World!").EndText().EndMarkedContent().BeginMarkedContent(PdfName
                    .H1).BeginText().ShowText("Hello but nested").EndText().EndMarkedContent().RestoreState().CloseTag();
            }
            );
            framework.AssertBothValid("validRoleAddedInsideMarkedContentMultiple");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_validRoleAddedInsideMarkedContentMCR_IN_MCR(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                pdfDoc.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem paragraph = conformance == PdfConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem
                    (pdfDoc, PdfName.P, page1)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid(new PdfStructElem
                    (pdfDoc, PdfName.P, page1));
                PdfStructElem paragraph2 = conformance == PdfConformance.PDF_UA_1 ? pdfDoc.GetStructTreeRoot().AddKid(new 
                    PdfStructElem(pdfDoc, PdfName.P, page1)) : ((PdfStructElem)pdfDoc.GetStructTreeRoot().GetKids()[0]).AddKid
                    (new PdfStructElem(pdfDoc, PdfName.P, page1));
                PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
                PdfMcr mcr1 = paragraph2.AddKid(new PdfMcrNumber(page1, paragraph2));
                canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginMarkedContent(PdfName.P).BeginText().SetFontAndSize(font
                    , 12).MoveText(200, 200).ShowText("Hello World!").EndText().EndMarkedContent().OpenTag(new CanvasTag(mcr1
                    )).BeginMarkedContent(PdfName.H1).BeginText().ShowText("Hello but nested").EndText().EndMarkedContent(
                    ).CloseTag().RestoreState().CloseTag();
            }
            );
            framework.AssertOnlyVeraPdfFail("validRoleAddedInsideMarkedContentMCR_IN_MCR");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_01_004_TaggedContentShouldNotBeInsideArtifact(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_31_009_FontIsNotEmbedded(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(StandardFonts.COURIER);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
                tagPointer.SetPageForTagging(pdfDoc.GetFirstPage());
                tagPointer.AddTag(StandardRoles.P);
                canvas.BeginText().OpenTag(tagPointer.GetTagReference()).SetFontAndSize(font, 12).ShowText("Please crash on close, tyvm"
                    ).EndText().CloseTag();
            }
            );
            framework.AssertBothFail("31_009_FontIsNotEmbedded", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .FONT_SHOULD_BE_EMBEDDED, "Courier"), false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_19_003_iDEntryInNoteTagIsNotPresent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            if (conformance == PdfConformance.PDF_UA_1) {
                framework.AssertBothFail("invalidNoteTag02", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY);
            }
            else {
                if (conformance == PdfConformance.PDF_UA_2) {
                    framework.AssertBothFail("invalidNoteTag02", PdfUAExceptionMessageConstants.DOCUMENT_USES_NOTE_TAG);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CheckPoint_19_003_validNoteTagIsPresent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfFont font = GetPdfFont();
                PdfPage page1 = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                pdfDocument.GetTagStructureContext().NormalizeDocumentRootTag();
                PdfStructElem paragraph = conformance == PdfConformance.PDF_UA_1 ? pdfDocument.GetStructTreeRoot().AddKid(
                    new PdfStructElem(pdfDocument, PdfName.P, page1)) : ((PdfStructElem)pdfDocument.GetStructTreeRoot().GetKids
                    ()[0]).AddKid(new PdfStructElem(pdfDocument, PdfName.P, page1));
                PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
                PdfStructElem note = conformance == PdfConformance.PDF_UA_1 ? pdfDocument.GetStructTreeRoot().AddKid(new PdfStructElem
                    (pdfDocument, PdfName.Note, page1)) : ((PdfStructElem)pdfDocument.GetStructTreeRoot().GetKids()[0]).AddKid
                    (new PdfStructElem(pdfDocument, PdfName.Note, page1));
                note.Put(PdfName.ID, new PdfString("1"));
                canvas.OpenTag(new CanvasTag(mcr)).SaveState().BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                    ("Hello World!").EndText().RestoreState().CloseTag();
            }
            );
            if (conformance == PdfConformance.PDF_UA_1) {
                framework.AssertBothValid("validNoteTagPresent");
            }
            else {
                if (conformance == PdfConformance.PDF_UA_2) {
                    framework.AssertBothFail("validNoteTagPresent", PdfUAExceptionMessageConstants.DOCUMENT_USES_NOTE_TAG);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void UsingCharacterWithoutUnicodeMappingTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(FontProgramFactory.CreateType1Font(FONT_FOLDER + "cmr10.afm", FONT_FOLDER
                         + "cmr10.pfb"), FontEncoding.FONT_SPECIFIC, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
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

        private static PdfFont GetPdfFont() {
            PdfFont font = null;
            try {
                font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
            return font;
        }
    }
}
