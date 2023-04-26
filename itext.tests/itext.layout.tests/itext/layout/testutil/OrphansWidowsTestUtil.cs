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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Testutil {
    public class OrphansWidowsTestUtil {
        public static String PARA_TEXT = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod "
             + "tempor incididunt ut labore et dolore magna aliqua. Nulla at volutpat diam ut " + "venenatis tellus in. Orci porta non pulvinar neque laoreet suspendisse interdum "
             + "consectetur. Ipsum dolor sit amet consectetur adipiscing. Id porta nibh venenatis" + " cras sed felis eget velit. Sapien nec sagittis aliquam malesuada. Cras sed felis"
             + " eget velit aliquet sagittis. Leo a diam sollicitudin tempor id eu nisl nunc." + " Faucibus a pellentesque sit amet porttitor eget dolor morbi. Nisl vel pretium"
             + " lectus quam id leo in vitae. Vehicula ipsum a arcu cursus vitae. Tincidunt praesent" + " semper feugiat nibh sed pulvinar proin gravida hendrerit. Nisl vel pretium lectus"
             + " quam id leo in vitae turpis. Quis hendrerit dolor magna eget est lorem. Diam sit" + " amet nisl suscipit adipiscing bibendum est ultricies. Ultricies mi eget mauris pharetra."
             + " Etiam dignissim diam quis enim. Felis bibendum ut tristique et egestas quis.";

        public const float LINES_SPACE_EPS = 5;

        public static void ProduceOrphansWidowsTestCase(String outPdf, int linesLeft, bool orphans, Paragraph testPara
            , bool applyMarginsOnTestPara) {
            PageSize pageSize = new PageSize(PageSize.A4.GetWidth(), PageSize.A5.GetHeight());
            Document doc = new Document(new PdfDocument(new PdfWriter(outPdf)), pageSize);
            Rectangle[] columns = InitUniformColumns(pageSize, 2);
            doc.SetRenderer(new ColumnDocumentRenderer(doc, columns));
            SetParagraphStylingProperties(testPara, applyMarginsOnTestPara);
            testPara.Add(PARA_TEXT);
            float linesHeight = CalculateHeightForLinesNum(doc, testPara, columns[0].GetWidth(), linesLeft, orphans);
            String descriptionIntro = "Test " + (orphans ? "orphans" : "widows") + ". ";
            String descriptionBeg = "This block height is adjusted in such way as to leave ";
            String descriptionEnd = " line(s) on area break. Reference example without orphans/widows control can be found on the next page.";
            float adjustmentHeight = columns[0].GetHeight() - linesHeight - LINES_SPACE_EPS;
            doc.Add(new Paragraph().Add(new Text(descriptionIntro).SetFontColor(ColorConstants.RED)).Add(new Text(descriptionBeg
                ).SetFontSize(8)).Add(new Text(linesLeft.ToString()).SetFontColor(ColorConstants.RED)).Add(new Text(descriptionEnd
                ).SetFontSize(8)).SetMargin(0).SetBorder(new SolidBorder(1)).SetHeight(adjustmentHeight));
            if (!applyMarginsOnTestPara) {
                doc.Add(testPara);
            }
            else {
                Div div = new Div().Add(testPara).SetMarginTop(15);
                div.SetProperty(Property.COLLAPSING_MARGINS, true);
                doc.Add(div);
            }
            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            doc.Add(new Paragraph("Reference example without orphans/widows control.").SetMargin(0).SetBorder(new SolidBorder
                (1)).SetHeight(adjustmentHeight));
            if (!applyMarginsOnTestPara) {
                doc.Add(SetParagraphStylingProperties(new Paragraph(PARA_TEXT), false));
            }
            else {
                Div div = new Div().Add(SetParagraphStylingProperties(new Paragraph(PARA_TEXT), true)).SetMarginTop(15);
                div.SetProperty(Property.COLLAPSING_MARGINS, true);
                doc.Add(div);
            }
            doc.Close();
        }

        public static void ProduceOrphansWidowsAndMaxHeightLimitTestCase(String outPdf, bool orphans) {
            Document document = new Document(new PdfDocument(new PdfWriter(outPdf)));
            SingleMaxHeightCase(document, orphans, false);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            SingleMaxHeightCase(document, orphans, true);
            document.Close();
        }

        public static void ProduceOrphansWidowsOnCanvasOfLimitedSizeTestCase(String outPdf, bool orphans) {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outPdf));
            Document document = new Document(pdfDocument);
            String orphansOrWidows = orphans ? "orphans" : "widows";
            Paragraph paraOnCanvas = SetParagraphStylingProperties(new Paragraph(PARA_TEXT), false);
            Rectangle effectiveArea = document.GetPageEffectiveArea(pdfDocument.GetDefaultPageSize());
            int minOrphansOrWidows = 3;
            float linesHeight = CalculateHeightForLinesNum(document, paraOnCanvas, effectiveArea.GetWidth(), minOrphansOrWidows
                 - 1, orphans);
            if (orphans) {
                paraOnCanvas.SetOrphansControl(new ParagraphOrphansControl(minOrphansOrWidows));
            }
            else {
                paraOnCanvas.SetWidowsControl(new ParagraphWidowsControl(minOrphansOrWidows, 1, false));
            }
            String description = "The paragraph beneath has property " + orphansOrWidows.ToUpperInvariant() + "_CONTROL,"
                 + " limiting the number of allowed " + orphansOrWidows + " to 3. " + "The size of canvas is limited so that the lines that"
                 + (orphans ? " " : " don't ") + "fit in the canvas cause " + orphansOrWidows + " violation. " + "The entire canvas area is filled in magenta.";
            SingleLimitedCanvasSizeCase(document, paraOnCanvas, description, linesHeight, 1);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            paraOnCanvas.DeleteOwnProperty(orphans ? Property.ORPHANS_CONTROL : Property.WIDOWS_CONTROL);
            description = "The paragraph beneath has no " + orphansOrWidows.ToUpperInvariant() + "_CONTROL, property.";
            SingleLimitedCanvasSizeCase(document, paraOnCanvas, description, linesHeight, 2);
            document.Close();
        }

        public static void ProduceOrphansWidowsWithinDivOfLimitedSizeTestCase(String outPdf, bool orphans) {
            Document document = new Document(new PdfDocument(new PdfWriter(outPdf)));
            String orphansOrWidows = orphans ? "orphans" : "widows";
            Paragraph testDescription = new Paragraph().SetBorder(new SolidBorder(ColorConstants.RED, 1));
            testDescription.Add("The paragraph beneath has property " + orphansOrWidows.ToUpperInvariant() + "_CONTROL,"
                 + " limiting the number of allowed " + orphansOrWidows + " to 3. " + "The size of div-wrapper of the paragraph is limited so that the lines that"
                 + (orphans ? " " : " don't ") + "fit in the canvas cause " + orphansOrWidows + " violation. ");
            document.Add(testDescription);
            Paragraph paragraph = SetParagraphStylingProperties(new Paragraph(PARA_TEXT), false);
            Rectangle effectiveArea = document.GetPageEffectiveArea(document.GetPdfDocument().GetDefaultPageSize());
            int minOrphansOrWidows = 3;
            float linesHeight = CalculateHeightForLinesNum(document, paragraph, effectiveArea.GetWidth(), minOrphansOrWidows
                 - 1, orphans);
            if (orphans) {
                paragraph.SetOrphansControl(new ParagraphOrphansControl(minOrphansOrWidows));
            }
            else {
                paragraph.SetWidowsControl(new ParagraphWidowsControl(minOrphansOrWidows, 1, false));
            }
            Div divOfLimitedSize = new Div().Add(paragraph);
            divOfLimitedSize.SetHeight(linesHeight + LINES_SPACE_EPS).SetBackgroundColor(ColorConstants.MAGENTA);
            document.Add(divOfLimitedSize);
            document.Close();
        }

        public static void ProduceOrphansWidowsKeepTogetherTestCase(String outPdf, bool orphans, bool large) {
            Document document = new Document(new PdfDocument(new PdfWriter(outPdf)));
            Paragraph paragraph = new Paragraph(PARA_TEXT).SetMargin(0).SetBackgroundColor(new DeviceRgb(232, 232, 232
                ));
            if (large) {
                paragraph.Add(PARA_TEXT).Add(PARA_TEXT).Add(PARA_TEXT).Add(PARA_TEXT);
            }
            Rectangle effectiveArea = document.GetPageEffectiveArea(document.GetPdfDocument().GetDefaultPageSize());
            int minOrphansOrWidows = 3;
            float linesHeight;
            if (!large || orphans) {
                linesHeight = CalculateHeightForLinesNum(document, paragraph, effectiveArea.GetWidth(), minOrphansOrWidows
                     - 1, orphans);
            }
            else {
                linesHeight = CalculateHeightForLinesNumKeepTogetherCaseSpecific(document, paragraph, effectiveArea.GetWidth
                    (), effectiveArea.GetHeight(), minOrphansOrWidows - 1);
            }
            if (orphans) {
                paragraph.SetOrphansControl(new ParagraphOrphansControl(minOrphansOrWidows));
            }
            else {
                paragraph.SetWidowsControl(new ParagraphWidowsControl(minOrphansOrWidows, 1, true));
            }
            paragraph.SetKeepTogether(true);
            String orphansOrWidows = orphans ? "orphans" : "widows";
            Paragraph testDescription = new Paragraph().SetMargin(0).SetBorder(new SolidBorder(ColorConstants.RED, 1))
                .SetHeight(effectiveArea.GetHeight() - linesHeight - LINES_SPACE_EPS);
            testDescription.Add("The paragraph beneath has property " + orphansOrWidows.ToUpperInvariant() + "_CONTROL,"
                 + " limiting the number of allowed " + orphansOrWidows + " to 3. " + "The paragraph has also KEEP_TOGETHER property. The size of this description-paragraph is defined so"
                 + " that " + orphansOrWidows + " violation " + (large ? "occurs." : "should have occurred if not for KEEP_TOGETHER."
                ));
            document.Add(testDescription);
            document.Add(paragraph);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            String referencePagesDescription;
            if (large) {
                referencePagesDescription = "The paragraph beneath has KEEP_TOGETHER property " + "and no " + orphansOrWidows
                    .ToUpperInvariant() + "_CONTROL property.";
            }
            else {
                referencePagesDescription = "The paragraph beneath has neither KEEP_TOGETHER property nor " + orphansOrWidows
                    .ToUpperInvariant() + "_CONTROL property.";
            }
            document.Add(new Paragraph(referencePagesDescription).SetMargin(0).SetBorder(new SolidBorder(ColorConstants
                .RED, 1)).SetHeight(effectiveArea.GetHeight() - linesHeight - LINES_SPACE_EPS));
            paragraph.DeleteOwnProperty(orphans ? Property.ORPHANS_CONTROL : Property.WIDOWS_CONTROL);
            if (!large) {
                paragraph.DeleteOwnProperty(Property.KEEP_TOGETHER);
            }
            document.Add(paragraph);
            document.Close();
        }

        public static void ProduceOrphansWidowsInlineImageTestCase(String outPdf, String imagePath, bool orphans) {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outPdf));
            Document document = new Document(pdfDocument);
            iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(imagePath));
            SingleInlineImageCase(document, img, orphans, true);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            SingleInlineImageCase(document, img, orphans, false);
            document.Close();
        }

        public static void ProduceOrphansWidowsHugeInlineImageTestCase(String outPdf, String imagePath, bool orphans
            ) {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outPdf));
            Document document = new Document(pdfDocument);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(imagePath));
            String text = "Just two lines\nJust two lines\n";
            Paragraph paragraph = new Paragraph().SetMargin(0).SetBackgroundColor(new DeviceRgb(232, 232, 232));
            if (orphans) {
                paragraph.Add(text).Add(img);
            }
            else {
                paragraph.Add(img).Add(text);
            }
            Rectangle effectiveArea = document.GetPageEffectiveArea(document.GetPdfDocument().GetDefaultPageSize());
            int minOrphansOrWidows = 3;
            if (orphans) {
                paragraph.SetOrphansControl(new ParagraphOrphansControl(minOrphansOrWidows));
            }
            else {
                paragraph.SetWidowsControl(new ParagraphWidowsControl(minOrphansOrWidows, 1, false));
            }
            String orphansOrWidows = orphans ? "orphans" : "widows";
            Paragraph testDescription = new Paragraph("The paragraph beneath has " + orphansOrWidows.ToUpperInvariant(
                ) + "_CONTROL property, limiting the number of allowed " + orphansOrWidows + " to 3. Huge image is part of the paragraph."
                ).SetMargin(0).SetBorder(new SolidBorder(ColorConstants.RED, 1)).SetHeight(effectiveArea.GetHeight() /
                 2);
            document.Add(testDescription);
            document.Add(paragraph);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            testDescription = new Paragraph("The paragraph beneath has no " + orphansOrWidows.ToUpperInvariant() + "_CONTROL property. Huge image is part of the paragraph."
                ).SetMargin(0).SetBorder(new SolidBorder(ColorConstants.RED, 1)).SetHeight(effectiveArea.GetHeight() /
                 2);
            document.Add(testDescription);
            paragraph.DeleteOwnProperty(orphans ? Property.ORPHANS_CONTROL : Property.WIDOWS_CONTROL);
            document.Add(paragraph);
            document.Close();
        }

        public static void ProduceOrphansWidowsInlineBlockTestCase(String outPdf, bool orphans) {
            Document document = new Document(new PdfDocument(new PdfWriter(outPdf)));
            Paragraph inlineBlockParagraph = SetParagraphStylingProperties(new Paragraph(OrphansWidowsTestUtil.PARA_TEXT
                ), false);
            Rectangle effectiveArea = document.GetPageEffectiveArea(document.GetPdfDocument().GetDefaultPageSize());
            int minOrphansOrWidows = 3;
            float linesHeight = CalculateHeightForLinesNum(document, inlineBlockParagraph, effectiveArea.GetWidth(), minOrphansOrWidows
                 - 1, orphans);
            String orphansOrWidows = orphans ? "orphans" : "widows";
            document.Add(new Paragraph("The paragraph beneath has property " + orphansOrWidows + "_CONTROL, limiting the number of allowed "
                 + orphansOrWidows + " to 3.").SetMargin(0).SetHeight(effectiveArea.GetHeight() - linesHeight - LINES_SPACE_EPS
                ).SetBorder(new SolidBorder(1)));
            Paragraph outerParagraph = new Paragraph().SetMargin(0).SetBorder(new SolidBorder(ColorConstants.RED, 1));
            outerParagraph.Add(inlineBlockParagraph);
            if (orphans) {
                inlineBlockParagraph.SetOrphansControl(new ParagraphOrphansControl(minOrphansOrWidows));
            }
            else {
                inlineBlockParagraph.SetWidowsControl(new ParagraphWidowsControl(minOrphansOrWidows, 1, false));
            }
            document.Add(outerParagraph);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            document.Add(new Paragraph("The paragraph beneath has no " + orphansOrWidows + "_CONTROL property, limiting the number of allowed "
                 + orphansOrWidows + " to 3.").SetMargin(0).SetHeight(effectiveArea.GetHeight() - linesHeight - LINES_SPACE_EPS
                ).SetBorder(new SolidBorder(1)));
            inlineBlockParagraph.DeleteOwnProperty(orphans ? Property.ORPHANS_CONTROL : Property.WIDOWS_CONTROL);
            document.Add(outerParagraph);
            document.Close();
        }

        public static void ProduceOrphansWidowsInlineFloatTestCase(String outPdf, bool orphans) {
            Document document = new Document(new PdfDocument(new PdfWriter(outPdf)));
            Paragraph inlineFloatParagraph = SetParagraphStylingProperties(new Paragraph(OrphansWidowsTestUtil.PARA_TEXT
                ), false);
            float floatingParaWidth = 200;
            Rectangle effectiveArea = document.GetPageEffectiveArea(document.GetPdfDocument().GetDefaultPageSize());
            int minOrphansOrWidows = 3;
            float linesHeight = CalculateHeightForLinesNum(document, inlineFloatParagraph, floatingParaWidth, minOrphansOrWidows
                 - 1, orphans);
            String orphansOrWidows = orphans ? "orphans" : "widows";
            document.Add(new Paragraph("The paragraph on the right has property " + orphansOrWidows + "_CONTROL, " + "limiting the number of allowed "
                 + orphansOrWidows + " to 3. " + "It's also floating to the right and has fixed width.").SetMargin(0).
                SetHeight(effectiveArea.GetHeight() - linesHeight - LINES_SPACE_EPS).SetBorder(new SolidBorder(1)));
            if (orphans) {
                inlineFloatParagraph.SetOrphansControl(new ParagraphOrphansControl(minOrphansOrWidows));
            }
            else {
                inlineFloatParagraph.SetWidowsControl(new ParagraphWidowsControl(minOrphansOrWidows, 1, false));
            }
            inlineFloatParagraph.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            inlineFloatParagraph.SetWidth(floatingParaWidth - 2);
            Paragraph placeholder = new Paragraph("This is just a placeholder").SetMargin(0).SetBorder(new SolidBorder
                (ColorConstants.RED, 1));
            document.Add(inlineFloatParagraph);
            document.Add(placeholder);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            document.Add(new Paragraph("The paragraph on the right has no " + orphansOrWidows + "_CONTROL,  property "
                 + "limiting the number of allowed " + orphansOrWidows + " to 3. " + "It's also floating to the right and has fixed width."
                ).SetMargin(0).SetHeight(effectiveArea.GetHeight() - linesHeight - LINES_SPACE_EPS).SetBorder(new SolidBorder
                (1)));
            inlineFloatParagraph.DeleteOwnProperty(orphans ? Property.ORPHANS_CONTROL : Property.WIDOWS_CONTROL);
            document.Add(inlineFloatParagraph);
            document.Add(placeholder);
            document.Close();
        }

        public static void ProduceOrphansWidowsFloatingDivTestCase(String outPdf, bool orphans) {
            Document document = new Document(new PdfDocument(new PdfWriter(outPdf)));
            Paragraph paraInFloatingDiv = SetParagraphStylingProperties(new Paragraph(OrphansWidowsTestUtil.PARA_TEXT)
                , false);
            float floatingDivWidth = 200;
            Rectangle effectiveArea = document.GetPageEffectiveArea(document.GetPdfDocument().GetDefaultPageSize());
            int minOrphansOrWidows = 3;
            float linesHeight = CalculateHeightForLinesNum(document, paraInFloatingDiv, floatingDivWidth, minOrphansOrWidows
                 - 1, orphans);
            String orphansOrWidows = orphans ? "orphans" : "widows";
            document.Add(new Paragraph("The div on the right has a paragraph child, that has property " + orphansOrWidows
                 + "_CONTROL, limiting the number of allowed " + orphansOrWidows + " to 3. " + "The div is floating to the right and has fixed width."
                ).SetMargin(0).SetHeight(effectiveArea.GetHeight() - linesHeight - LINES_SPACE_EPS).SetBorder(new SolidBorder
                (1)));
            if (orphans) {
                paraInFloatingDiv.SetOrphansControl(new ParagraphOrphansControl(minOrphansOrWidows));
            }
            else {
                paraInFloatingDiv.SetWidowsControl(new ParagraphWidowsControl(minOrphansOrWidows, 1, false));
            }
            Div floatingDiv = new Div().Add(paraInFloatingDiv).SetMargin(0).SetWidth(floatingDivWidth - 2);
            floatingDiv.SetProperty(Property.FLOAT, FloatPropertyValue.RIGHT);
            Paragraph placeholder = new Paragraph("This is just a placeholder").SetMargin(0).SetBorder(new SolidBorder
                (ColorConstants.RED, 1));
            document.Add(floatingDiv);
            document.Add(placeholder);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            document.Add(new Paragraph("The div on the right has a paragraph child, that has property, that has no " +
                 orphansOrWidows + "_CONTROL, limiting the number of allowed " + orphansOrWidows + " to 3." + "The div is floating to the right and has fixed width."
                ).SetMargin(0).SetHeight(effectiveArea.GetHeight() - linesHeight - LINES_SPACE_EPS).SetBorder(new SolidBorder
                (1)));
            paraInFloatingDiv.DeleteOwnProperty(orphans ? Property.ORPHANS_CONTROL : Property.WIDOWS_CONTROL);
            document.Add(floatingDiv);
            document.Add(placeholder);
            document.Close();
        }

        public static void ProduceOrphansWidowsBiggerThanLinesCountTestCase(String outPdf, bool orphans, bool singleLine
            ) {
            Document document = new Document(new PdfDocument(new PdfWriter(outPdf)));
            Paragraph smallParagraph = SetParagraphStylingProperties(new Paragraph(), false);
            if (singleLine) {
                smallParagraph.Add("Single line!");
            }
            else {
                smallParagraph.Add("First line!\nSecond line!");
            }
            Rectangle effectiveArea = document.GetPageEffectiveArea(document.GetPdfDocument().GetDefaultPageSize());
            int minOrphansOrWidows = 3;
            float linesHeight = CalculateHeightForLinesNum(document, smallParagraph, effectiveArea.GetWidth(), minOrphansOrWidows
                 - 2, orphans);
            String orphansOrWidows = orphans ? "orphans" : "widows";
            document.Add(new Paragraph("The paragraph beneath has only " + (singleLine ? "one line" : "two lines") + " and property "
                 + orphansOrWidows + "_CONTROL," + " limiting the number of allowed " + orphansOrWidows + " to 3.").SetMargin
                (0).SetHeight(effectiveArea.GetHeight() - linesHeight - LINES_SPACE_EPS).SetBorder(new SolidBorder(1))
                );
            if (orphans) {
                smallParagraph.SetOrphansControl(new ParagraphOrphansControl(minOrphansOrWidows));
            }
            else {
                smallParagraph.SetWidowsControl(new ParagraphWidowsControl(minOrphansOrWidows, 2, false));
            }
            document.Add(smallParagraph);
            document.Close();
        }

        public static void ProduceOrphansWidowsUnexpectedWidthOfNextAreaTestCase(String outPdf, bool widerNextPage
            ) {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outPdf));
            Document document = new Document(pdfDocument);
            pdfDocument.AddNewPage();
            pdfDocument.AddNewPage(widerNextPage ? PageSize.A2 : PageSize.A6);
            pdfDocument.AddNewPage();
            Paragraph smallParagraph = SetParagraphStylingProperties(new Paragraph(PARA_TEXT), false);
            Rectangle effectiveArea = document.GetPageEffectiveArea(document.GetPdfDocument().GetDefaultPageSize());
            float linesHeight = CalculateHeightForLinesNum(document, smallParagraph, effectiveArea.GetWidth(), 3, false
                );
            String descriptionPara = "The paragraph beneath has property WIDOWS_CONTROL set to (6, 3, false)." + " The widows are resolved as if the next page had the same effective area as this one."
                 + (widerNextPage ? " In fact the following page is wider, which is why the widows aren't fixed," + " some lines are moved for no reason and no violation report is logged."
                 : " In fact" + " the following page is narrower, which is why the widows shouldn't be fixed in the first place. "
                 + "As a result some lines are moved for no reason.");
            document.Add(new Paragraph(descriptionPara).SetMargin(0).SetHeight(effectiveArea.GetHeight() - linesHeight
                 - LINES_SPACE_EPS).SetBorder(new SolidBorder(1)));
            smallParagraph.SetWidowsControl(new ParagraphWidowsControl(6, 3, false));
            document.Add(smallParagraph);
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            String referencePagePara = "This is a reference page illustrating how" + (widerNextPage ? " widows are" + 
                " resolved when the page's effective area remains the same" : " it would look like" + " if widows \"violation\" hadn't been resolved: there's actually no violation."
                );
            document.Add(new Paragraph(referencePagePara).SetMargin(0).SetHeight(effectiveArea.GetHeight() - linesHeight
                 - LINES_SPACE_EPS).SetBorder(new SolidBorder(1)));
            if (!widerNextPage) {
                smallParagraph.DeleteOwnProperty(Property.WIDOWS_CONTROL);
                pdfDocument.AddNewPage(PageSize.A6);
            }
            document.Add(smallParagraph);
            document.Close();
        }

        public static void ProduceOrphansOrWidowsTestCase(String outPdf, int linesLeft, bool orphans, Paragraph testPara
            ) {
            Document doc = new Document(new PdfDocument(new PdfWriter(outPdf)));
            PageSize pageSize = new PageSize(PageSize.A4.GetWidth(), PageSize.A5.GetHeight());
            doc.GetPdfDocument().SetDefaultPageSize(pageSize);
            testPara.SetMargin(0).SetBackgroundColor(new DeviceRgb(232, 232, 232));
            testPara.Add(PARA_TEXT);
            String orphansOrWidows = orphans ? "orphans" : "widows";
            String description = "Test " + orphansOrWidows + ".\n" + " This block height is adjusted in" + " such way as to leave "
                 + (linesLeft.ToString()) + " line(s) on area break.\n" + " Configuration is identified by the file name.\n Reference example"
                 + " without " + orphansOrWidows + " control can be found on the next page.";
            float effectiveWidth;
            float effectiveHeight;
            doc.SetRenderer(new DocumentRenderer(doc));
            Rectangle effectiveArea = doc.GetPageEffectiveArea(pageSize);
            effectiveWidth = effectiveArea.GetWidth();
            effectiveHeight = effectiveArea.GetHeight();
            float linesHeight = CalculateHeightForLinesNum(doc, testPara, effectiveWidth, linesLeft, orphans);
            float adjustmentHeight = effectiveHeight - linesHeight - LINES_SPACE_EPS;
            doc.Add(new Paragraph().Add(new Text(description)).SetMargin(0).SetBorder(new SolidBorder(1)).SetHeight(adjustmentHeight
                ));
            doc.Add(testPara);
            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            doc.Add(new Paragraph("Reference example without orphans/widows control.").SetMargin(0).SetBorder(new SolidBorder
                (1)).SetHeight(adjustmentHeight));
            doc.Add(new Paragraph(PARA_TEXT).SetMargin(0).SetBackgroundColor(new DeviceRgb(232, 232, 232)));
            doc.Close();
        }

        public static void ProduceOrphansAndWidowsTestCase(String outPdf, Paragraph testPara) {
            Document doc = new Document(new PdfDocument(new PdfWriter(outPdf)));
            PageSize pageSize = new PageSize(PageSize.A4.GetWidth(), PageSize.A5.GetHeight());
            doc.GetPdfDocument().SetDefaultPageSize(pageSize);
            Rectangle[] columns = InitUniformColumns(pageSize, 2);
            doc.SetRenderer(new ColumnDocumentRenderer(doc, columns));
            String paraText = "A one line string\n";
            testPara.SetMargin(0).SetBackgroundColor(new DeviceRgb(232, 232, 232));
            testPara.Add(paraText);
            float linesHeight = CalculateHeightForLinesNum(doc, testPara, columns[1].GetWidth(), 1, true);
            float adjustmentHeight = columns[0].GetHeight() - linesHeight - LINES_SPACE_EPS;
            String description = "Test orphans and widows case at once. This block height" + " is adjusted in such way that both orphans and widows cases occur.\n "
                 + "The following paragraph contains as many fitting in one line text strings as needed" + " to reproduce the case with both orphans and widows\n"
                 + "Reference example without orphans and widows" + " control can be found on the next page";
            doc.Add(new Paragraph(description).SetMargin(0).SetBorder(new SolidBorder(1)).SetHeight(adjustmentHeight));
            Paragraph tempPara = new Paragraph().SetMargin(0);
            for (int i = 0; i < 50; i++) {
                tempPara.Add(paraText);
            }
            ParagraphRenderer renderer = (ParagraphRenderer)tempPara.CreateRendererSubTree().SetParent(doc.GetRenderer
                ());
            LayoutResult layoutRes = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(columns[1].GetWidth
                (), columns[1].GetHeight()))));
            int numberOfLines = ((ParagraphRenderer)layoutRes.GetSplitRenderer()).GetLines().Count;
            for (int i = 0; i <= numberOfLines; i++) {
                testPara.Add(paraText);
            }
            doc.Add(testPara);
            doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            doc.Add(new Paragraph("Reference example without orphans and widows control.").SetMargin(0).SetBorder(new 
                SolidBorder(1)).SetHeight(adjustmentHeight));
            Paragraph paragraph = new Paragraph();
            for (int i = 0; i <= numberOfLines + 1; i++) {
                paragraph.Add(paraText);
            }
            paragraph.SetMargin(0).SetBackgroundColor(new DeviceRgb(232, 232, 232));
            doc.Add(paragraph);
            doc.Add(new Paragraph(paraText).SetMargin(0).SetBackgroundColor(new DeviceRgb(232, 232, 232)));
            doc.Close();
        }

        public static float CalculateHeightForLinesNum(Document doc, Paragraph p, float width, float linesNum, bool
             orphans) {
            ParagraphRenderer renderer = (ParagraphRenderer)p.CreateRendererSubTree().SetParent(doc.GetRenderer());
            LayoutResult layoutRes = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(width, 100000))
                ));
            float lineHeight = layoutRes.GetOccupiedArea().GetBBox().GetHeight() / renderer.GetLines().Count;
            float height = lineHeight * linesNum;
            if (orphans) {
                return height;
            }
            else {
                return layoutRes.GetOccupiedArea().GetBBox().GetHeight() - height;
            }
        }

        public static float CalculateHeightForLinesNumKeepTogetherCaseSpecific(Document doc, Paragraph p, float width
            , float height, float linesNum) {
            ParagraphRenderer renderer = (ParagraphRenderer)p.CreateRendererSubTree().SetParent(doc.GetRenderer());
            LayoutResult layoutRes = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(width, 10000)))
                );
            int allLinesCount = renderer.GetLines().Count;
            float lineHeight = layoutRes.GetOccupiedArea().GetBBox().GetHeight() / allLinesCount;
            renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(width, height))));
            int linesWithinOnePageCount = renderer.GetLines().Count;
            return (allLinesCount - linesWithinOnePageCount - linesNum) * lineHeight;
        }

        private static void SingleLimitedCanvasSizeCase(Document document, Paragraph paraOnCanvas, String description
            , float canvasHeight, int pageNum) {
            PdfDocument pdfDocument = document.GetPdfDocument();
            document.Add(new Paragraph(description).SetBorder(new SolidBorder(ColorConstants.RED, 1)));
            PdfCanvas pdfCanvas = new PdfCanvas(pdfDocument.GetPage(pageNum));
            Rectangle effectiveArea = document.GetPageEffectiveArea(pdfDocument.GetDefaultPageSize());
            Rectangle rectangle = new Rectangle(36, 550, effectiveArea.GetWidth(), canvasHeight + LINES_SPACE_EPS);
            pdfCanvas.SaveState().SetFillColor(ColorConstants.MAGENTA).Rectangle(rectangle).Fill().RestoreState();
            iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, rectangle);
            canvas.Add(paraOnCanvas);
            canvas.Close();
        }

        private static void SingleMaxHeightCase(Document document, bool orphans, bool overflowVisible) {
            String orphansOrWidows = orphans ? "orphans" : "widows";
            Paragraph testDescription = new Paragraph().SetBorder(new SolidBorder(ColorConstants.RED, 1));
            testDescription.Add("The paragraph beneath has property " + orphansOrWidows.ToUpperInvariant() + "_CONTROL, limiting the number of allowed "
                 + orphansOrWidows + " to 3. " + "The paragraph also has property MAX_HEIGHT, whose value is defined so that the lines that"
                 + (orphans ? " " : " don't ") + "fit in the area limited by MAX_HEIGHT value cause " + orphansOrWidows
                 + " violation.\n");
            if (overflowVisible) {
                testDescription.Add("On this page the paragraph has also Property.OVERFLOW_Y set to VISIBLE " + "in order to visualize the "
                     + orphansOrWidows + " violation.");
            }
            document.Add(testDescription);
            Paragraph paragraph = SetParagraphStylingProperties(new Paragraph(PARA_TEXT), false);
            Rectangle effectiveArea = document.GetPageEffectiveArea(document.GetPdfDocument().GetDefaultPageSize());
            int minOrphansOrWidows = 3;
            float linesHeight = CalculateHeightForLinesNum(document, paragraph, effectiveArea.GetWidth(), minOrphansOrWidows
                 - 1, orphans);
            if (orphans) {
                paragraph.SetOrphansControl(new ParagraphOrphansControl(minOrphansOrWidows));
            }
            else {
                paragraph.SetWidowsControl(new ParagraphWidowsControl(minOrphansOrWidows, 1, false));
            }
            paragraph.SetProperty(Property.MAX_HEIGHT, new UnitValue(UnitValue.POINT, linesHeight + LINES_SPACE_EPS));
            if (overflowVisible) {
                paragraph.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            }
            document.Add(paragraph);
        }

        private static void SingleInlineImageCase(Document document, iText.Layout.Element.Image img, bool orphans, 
            bool orphansWidowsEnabled) {
            Paragraph paragraph = SetParagraphStylingProperties(new Paragraph(), false);
            for (int i = 0; i < 100; i++) {
                paragraph.Add(img).Add("inline image");
            }
            Rectangle effectiveArea = document.GetPageEffectiveArea(document.GetPdfDocument().GetDefaultPageSize());
            int minOrphansOrWidows = 3;
            float linesHeight = CalculateHeightForLinesNum(document, paragraph, effectiveArea.GetWidth(), minOrphansOrWidows
                 - 1, orphans);
            if (orphansWidowsEnabled) {
                if (orphans) {
                    paragraph.SetOrphansControl(new ParagraphOrphansControl(minOrphansOrWidows));
                }
                else {
                    paragraph.SetWidowsControl(new ParagraphWidowsControl(minOrphansOrWidows, 1, false));
                }
            }
            String orphansOrWidows = orphans ? "orphans" : "widows";
            Paragraph testDescription = new Paragraph().SetMargin(0).SetBorder(new SolidBorder(ColorConstants.RED, 1))
                .SetHeight(effectiveArea.GetHeight() - linesHeight - LINES_SPACE_EPS);
            testDescription.Add("The paragraph beneath has" + (orphansWidowsEnabled ? " " : " no ") + orphansOrWidows.
                ToUpperInvariant() + "_CONTROL property" + (orphansWidowsEnabled ? ", limiting the number of allowed "
                 + orphansOrWidows + " to 3. " : ".") + "The size of this description-paragraph is defined so that " +
                 orphansOrWidows + " violation occurs.");
            document.Add(testDescription);
            document.Add(paragraph);
        }

        private static Paragraph SetParagraphStylingProperties(Paragraph paragraph, bool increasedTopMargin) {
            return paragraph.SetMargins(increasedTopMargin ? 30 : 0, 0, 0, 0).SetBackgroundColor(new DeviceRgb(232, 232
                , 232)).SetBorder(new SolidBorder(1));
        }

        private static Rectangle[] InitUniformColumns(PageSize pageSize, int columnsNum) {
            Rectangle[] columns = new Rectangle[columnsNum];
            float columnWidth = (pageSize.GetWidth() - 72) / columnsNum;
            for (int i = 0; i < columnsNum; ++i) {
                columns[i] = new Rectangle(36 + i * columnWidth, 36, columnWidth - 36, pageSize.GetHeight() - 72);
            }
            return columns;
        }
    }
}
