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
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUACanvasTextTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUACanvasTextTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/iTextFreeSansWithE001Glyph.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<String> TextRepresentation() {
            return JavaUtil.ArraysAsList("text", "array", "glyphs");
        }

        [NUnit.Framework.Test]
        public virtual void PuaValueInLayoutTest() {
            String filename = "puaValueInLayoutTest";
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            framework.AddSuppliers(new _Generator_73());
            framework.AssertBothFail(filename, PdfUAExceptionMessageConstants.PUA_CONTENT_WITHOUT_ALT, PdfUAConformance
                .PDF_UA_2);
        }

        private sealed class _Generator_73 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_73() {
            }

            public IBlockElement Generate() {
                Paragraph paragraph = new Paragraph("hello_" + "\uE001");
                paragraph.SetFont(PdfUACanvasTextTest.LoadFont());
                return paragraph;
            }
        }

        [NUnit.Framework.TestCaseSource("TextRepresentation")]
        public virtual void PuaValueWithoutAttributesTest(String textRepresentation) {
            String filename = "puaValueWithoutAttributesTest_" + textRepresentation;
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            framework.AddBeforeGenerationHook((document) => {
                PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
                TagTreePointer pointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                pointer.AddTag(StandardRoles.P);
                pointer.SetPageForTagging(document.GetFirstPage());
                canvas.BeginText();
                PdfFont font = LoadFont();
                canvas.SetFontAndSize(font, 24);
                canvas.OpenTag(pointer.GetTagReference());
                AddPuaTextToCanvas(canvas, textRepresentation, font);
                canvas.CloseTag();
                canvas.EndText();
            }
            );
            AssertResult(false, textRepresentation, filename, framework);
        }

        [NUnit.Framework.TestCaseSource("TextRepresentation")]
        public virtual void PuaValueWithAltOnTagTest(String textRepresentation) {
            String filename = "puaValueWithAltOnTagTest_" + textRepresentation;
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            framework.AddBeforeGenerationHook((document) => {
                PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
                TagTreePointer pointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                pointer.AddTag(StandardRoles.P);
                pointer.SetPageForTagging(document.GetFirstPage());
                pointer.ApplyProperties(new DefaultAccessibilityProperties(StandardRoles.P).SetAlternateDescription("alt description"
                    ));
                canvas.BeginText();
                PdfFont font = LoadFont();
                canvas.SetFontAndSize(font, 24);
                CanvasTag canvasTag = new CanvasTag(pointer.GetTagReference().GetRole(), pointer.GetTagReference().CreateNextMcid
                    ());
                canvas.OpenTag(canvasTag);
                AddPuaTextToCanvas(canvas, textRepresentation, font);
                canvas.CloseTag();
                canvas.EndText();
            }
            );
            AssertResult(true, textRepresentation, filename, framework);
        }

        [NUnit.Framework.TestCaseSource("TextRepresentation")]
        public virtual void PuaValueWithActualTextOnTagTest(String textRepresentation) {
            String filename = "puaValueWithActualTextOnTagTest_" + textRepresentation;
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            framework.AddBeforeGenerationHook((document) => {
                PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
                TagTreePointer pointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                pointer.AddTag(StandardRoles.P);
                pointer.SetPageForTagging(document.GetFirstPage());
                pointer.ApplyProperties(new DefaultAccessibilityProperties(StandardRoles.P).SetActualText("alt description"
                    ));
                canvas.BeginText();
                PdfFont font = LoadFont();
                canvas.SetFontAndSize(font, 24);
                CanvasTag canvasTag = new CanvasTag(pointer.GetTagReference().GetRole(), pointer.GetTagReference().CreateNextMcid
                    ());
                canvas.OpenTag(canvasTag);
                AddPuaTextToCanvas(canvas, textRepresentation, font);
                canvas.CloseTag();
                canvas.EndText();
            }
            );
            AssertResult(true, textRepresentation, filename, framework);
        }

        [NUnit.Framework.TestCaseSource("TextRepresentation")]
        public virtual void PuaValueWithAltOnCanvasTest(String textRepresentation) {
            String filename = "puaValueWithAltOnCanvasTest_" + textRepresentation;
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            framework.AddBeforeGenerationHook((document) => {
                PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
                TagTreePointer pointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                pointer.AddTag(StandardRoles.P);
                pointer.SetPageForTagging(document.GetFirstPage());
                canvas.BeginText();
                PdfFont font = LoadFont();
                canvas.SetFontAndSize(font, 24);
                CanvasTag canvasTag = new CanvasTag(pointer.GetTagReference().GetRole(), pointer.GetTagReference().CreateNextMcid
                    ());
                canvasTag.AddProperty(PdfName.Alt, new PdfString("alt description"));
                canvas.OpenTag(canvasTag);
                AddPuaTextToCanvas(canvas, textRepresentation, font);
                canvas.CloseTag();
                canvas.EndText();
            }
            );
            AssertResult(true, textRepresentation, filename, framework);
        }

        [NUnit.Framework.TestCaseSource("TextRepresentation")]
        public virtual void PuaValueWithActualTextOnCanvasTest(String textRepresentation) {
            String filename = "puaValueWithActualTextOnCanvasTest_" + textRepresentation;
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            framework.AddBeforeGenerationHook((document) => {
                PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
                TagTreePointer pointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                pointer.AddTag(StandardRoles.P);
                pointer.SetPageForTagging(document.GetFirstPage());
                canvas.BeginText();
                PdfFont font = LoadFont();
                canvas.SetFontAndSize(font, 24);
                CanvasTag canvasTag = new CanvasTag(pointer.GetTagReference().GetRole(), pointer.GetTagReference().CreateNextMcid
                    ());
                canvasTag.AddProperty(PdfName.ActualText, new PdfString("alt description"));
                canvas.OpenTag(canvasTag);
                AddPuaTextToCanvas(canvas, textRepresentation, font);
                canvas.CloseTag();
                canvas.EndText();
            }
            );
            AssertResult(true, textRepresentation, filename, framework);
        }

        [NUnit.Framework.TestCaseSource("TextRepresentation")]
        public virtual void PuaValueOnTwoPagesTest(String textRepresentation) {
            String filename = "puaValueOnTwoPagesTest_" + textRepresentation;
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, false);
            framework.AddBeforeGenerationHook((document) => {
                // Text on page 1 contains PUA and alt, which is valid.
                PdfCanvas canvasOnPageOne = new PdfCanvas(document.AddNewPage());
                TagTreePointer pointer1 = document.GetTagStructureContext().GetAutoTaggingPointer();
                pointer1.AddTag(StandardRoles.P);
                pointer1.SetPageForTagging(document.GetFirstPage());
                pointer1.ApplyProperties(new DefaultAccessibilityProperties(StandardRoles.P).SetAlternateDescription("alt description"
                    ));
                canvasOnPageOne.BeginText();
                PdfFont font = LoadFont();
                canvasOnPageOne.SetFontAndSize(font, 24);
                CanvasTag canvasTag = new CanvasTag(pointer1.GetTagReference().GetRole(), pointer1.GetTagReference().CreateNextMcid
                    ());
                canvasOnPageOne.OpenTag(canvasTag);
                AddPuaTextToCanvas(canvasOnPageOne, textRepresentation, font);
                canvasOnPageOne.CloseTag();
                canvasOnPageOne.EndText();
                pointer1.MoveToParent();
                // Text on page two contains PUA, but doesn't contain alt, which is invalid.
                PdfCanvas canvasOnPageTwo = new PdfCanvas(document.AddNewPage());
                TagTreePointer pointer2 = document.GetTagStructureContext().GetAutoTaggingPointer();
                pointer2.AddTag(StandardRoles.P);
                pointer2.SetPageForTagging(document.GetPage(2));
                canvasOnPageTwo.BeginText();
                canvasOnPageTwo.SetFontAndSize(font, 24);
                canvasOnPageTwo.OpenTag(pointer2.GetTagReference());
                AddPuaTextToCanvas(canvasOnPageTwo, textRepresentation, font);
                canvasOnPageTwo.CloseTag();
                canvasOnPageTwo.EndText();
            }
            );
            AssertResult(false, textRepresentation, filename, framework);
        }

        private void AssertResult(bool expectedValid, String textRepresentation, String filename, UaValidationTestFramework
             framework) {
            if (expectedValid) {
                framework.AssertBothValid(filename, PdfUAConformance.PDF_UA_2);
            }
            else {
                if ("array".Equals(textRepresentation)) {
                    // In case of "array" PdfCanvas#showText(PdfArray) is used. In this method we don't have this check, because
                    // of the complications regarding not symbolic fonts.
                    framework.AssertOnlyVeraPdfFail(filename, PdfUAConformance.PDF_UA_2);
                }
                else {
                    framework.AssertBothFail(filename, PdfUAExceptionMessageConstants.PUA_CONTENT_WITHOUT_ALT, PdfUAConformance
                        .PDF_UA_2);
                }
            }
        }

        private void AddPuaTextToCanvas(PdfCanvas canvas, String textRepresentation, PdfFont font) {
            String stringWithPua = "hello_" + "\uE001";
            switch (textRepresentation) {
                case "text": {
                    canvas.ShowText(stringWithPua);
                    break;
                }

                case "array": {
                    PdfArray array = new PdfArray();
                    array.Add(new PdfString(font.ConvertToBytes(stringWithPua)));
                    canvas.ShowText(array);
                    break;
                }

                case "glyphs": {
                    GlyphLine glyphLine = font.CreateGlyphLine(stringWithPua);
                    canvas.ShowText(glyphLine);
                    break;
                }
            }
        }

        private static PdfFont LoadFont() {
            try {
                return PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message);
            }
        }
    }
}
