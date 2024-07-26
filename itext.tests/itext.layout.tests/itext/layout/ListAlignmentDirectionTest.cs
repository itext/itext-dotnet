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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ListAlignmentDirectionTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ListAlignmentDirectionTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ListAlignmentDirectionTest/";

        private const String PARAMETERS_NAME_PATTERN = "item-text-align: {0}; item-direction: {1}, " + "list-text-align: {2}; list-direction: {3}";

        private const String RESULTANT_FILE_NAME_PATTERN = "item-text-align-{0}_item-dir-{1}_list-text-align-{2}_list-dir-{3}";

        private const String HTML_PATTERN = "<ul style=\"background-color: green; width: 300pt; margin-left: 150pt; text-align: {2}; direction: {3}\">"
             + "  <li style=\"background-color: blue;\">Usual line</li>" + "  <li style=\"background-color: yellow; text-align: {0}; direction: {1}\">Specific line</li>"
             + "</ul>";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IEnumerable<Object[]> AlignItemsAndJustifyContentProperties() {
            TextAlignment?[] alignmentTestValues = new TextAlignment?[] { TextAlignment.LEFT, TextAlignment.CENTER, TextAlignment
                .RIGHT, TextAlignment.JUSTIFIED, TextAlignment.JUSTIFIED_ALL };
            BaseDirection?[] directionTestValues = new BaseDirection?[] { BaseDirection.LEFT_TO_RIGHT, BaseDirection.RIGHT_TO_LEFT
                 };
            IList<Object[]> objectList = new List<Object[]>();
            foreach (TextAlignment? itemTA in alignmentTestValues) {
                foreach (BaseDirection? itemBA in directionTestValues) {
                    foreach (TextAlignment? listTA in alignmentTestValues) {
                        foreach (BaseDirection? listBA in directionTestValues) {
                            objectList.Add(new Object[] { itemTA, itemBA, listTA, listBA });
                        }
                    }
                }
            }
            return objectList;
        }

        [NUnit.Framework.TestCaseSource("AlignItemsAndJustifyContentProperties")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TYPOGRAPHY_NOT_FOUND, Count = 8)]
        public virtual void AlignmentDirectionTest(TextAlignment? itemTextAlignment, BaseDirection? itemBaseDirection
            , TextAlignment? listTextAlignment, BaseDirection? listBaseDirection) {
            // TODO DEVSIX-5727 direction of the first list-item should define the symbol indent's side. Once the issue
            // is fixed, the corresponding cmps should be updated.
            // Create an HTML for this test
            CreateHtml(itemTextAlignment, itemBaseDirection, listTextAlignment, listBaseDirection);
            String fileName = MessageFormatUtil.Format(RESULTANT_FILE_NAME_PATTERN, FormatTextAlignment(itemTextAlignment
                ), FormatBaseDirection(itemBaseDirection), FormatTextAlignment(listTextAlignment), FormatBaseDirection
                (listBaseDirection));
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            Style style = new Style().SetTextAlignment(itemTextAlignment).SetBaseDirection(itemBaseDirection);
            List list = CreateTestList(style);
            list.SetTextAlignment(listTextAlignment);
            list.SetBaseDirection(listBaseDirection);
            document.Add(list);
            document.Close();
            System.Console.Out.WriteLine("HTML: " + UrlUtil.GetNormalizedFileUriString(DESTINATION_FOLDER + fileName +
                 ".html") + "\n");
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff_"));
        }

        private static List CreateTestList(Style secondItemStyle) {
            List list = new List();
            list.SetSymbolIndent(20);
            list.SetListSymbol("\u2022");
            list.SetBackgroundColor(ColorConstants.GREEN);
            list.SetWidth(300);
            list.SetMarginLeft(150);
            ListItem listItem1 = new ListItem();
            listItem1.Add(new Paragraph("Usual item"));
            listItem1.SetBackgroundColor(ColorConstants.BLUE);
            list.Add(listItem1);
            ListItem listItem2 = new ListItem();
            listItem2.AddStyle(secondItemStyle);
            listItem2.Add(new Paragraph("Specific item"));
            listItem2.SetBackgroundColor(ColorConstants.YELLOW);
            list.Add(listItem2);
            return list;
        }

        private void CreateHtml(TextAlignment? itemTextAlignment, BaseDirection? itemBaseDirection, TextAlignment?
             listTextAlignment, BaseDirection? listBaseDirection) {
            String fileName = MessageFormatUtil.Format(RESULTANT_FILE_NAME_PATTERN, FormatTextAlignment(itemTextAlignment
                ), FormatBaseDirection(itemBaseDirection), FormatTextAlignment(listTextAlignment), FormatBaseDirection
                (listBaseDirection));
            String htmlString = MessageFormatUtil.Format(HTML_PATTERN, FormatTextAlignment(itemTextAlignment, true), FormatBaseDirection
                (itemBaseDirection), FormatTextAlignment(listTextAlignment, true), FormatBaseDirection(listBaseDirection
                ));
            using (Stream htmlFile = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + fileName + ".html")) {
                byte[] htmlBytes = htmlString.GetBytes(System.Text.Encoding.UTF8);
                htmlFile.Write(htmlBytes, 0, htmlBytes.Length);
            }
        }

        private static String FormatTextAlignment(TextAlignment? alignment) {
            return FormatTextAlignment(alignment, false);
        }

        private static String FormatTextAlignment(TextAlignment? alignment, bool isHtml) {
            switch (alignment) {
                case TextAlignment.LEFT: {
                    return "left";
                }

                case TextAlignment.RIGHT: {
                    return "right";
                }

                case TextAlignment.CENTER: {
                    return "center";
                }

                case TextAlignment.JUSTIFIED: {
                    return "justify";
                }

                case TextAlignment.JUSTIFIED_ALL: {
                    return isHtml ? "justify" : "justify-all";
                }

                default: {
                    NUnit.Framework.Assert.Fail("Unexpected text alignment");
                    return null;
                }
            }
        }

        private static String FormatBaseDirection(BaseDirection? direction) {
            switch (direction) {
                case BaseDirection.LEFT_TO_RIGHT: {
                    return "ltr";
                }

                case BaseDirection.RIGHT_TO_LEFT: {
                    return "rtl";
                }

                default: {
                    NUnit.Framework.Assert.Fail("Unexpected base direction");
                    return null;
                }
            }
        }
    }
}
