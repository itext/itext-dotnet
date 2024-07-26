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
    public class ListItemPositionAlignmentTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ListItemPositionAlignmentTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ListItemPositionAlignmentTest/";

        private const String PARAMETERS_NAME_PATTERN = "{index}: list-base-direction: {0}; list-item-base-direction: {1};"
             + " list-symbol-alignment: {2}; list-symbol-position: {3};";

        private const String RESULTANT_FILE_NAME_PATTERN = "list-dir-{0}_item-dir-{1}_symbol-align-{2}_symbol-position-{3}";

        private const String HTML_PATTERN = "<ul style=\"background-color: green; direction: {3}\">" + "  <li style=\"background-color: blue;\">Usual item</li>"
             + "  <li style=\"background-color: yellow; direction: {2}; symbol-alignment:{1}; symbol-position: {0}\">Specific item</li>"
             + "</ul>";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IEnumerable<Object[]> BaseDirectionAndSymbolAlignmentProperties() {
            BaseDirection?[] directionTestValues = new BaseDirection?[] { BaseDirection.LEFT_TO_RIGHT, BaseDirection.RIGHT_TO_LEFT
                 };
            ListSymbolAlignment[] listSymbolAlignmentTestValues = new ListSymbolAlignment[] { ListSymbolAlignment.LEFT
                , ListSymbolAlignment.RIGHT };
            ListSymbolPosition[] listSymbolPositionTestValues = new ListSymbolPosition[] { ListSymbolPosition.OUTSIDE, 
                ListSymbolPosition.INSIDE };
            IList<Object[]> objectList = new List<Object[]>();
            int count = 0;
            foreach (BaseDirection? listBA in directionTestValues) {
                foreach (BaseDirection? itemBA in directionTestValues) {
                    foreach (ListSymbolAlignment listSA in listSymbolAlignmentTestValues) {
                        foreach (ListSymbolPosition listSP in listSymbolPositionTestValues) {
                            objectList.Add(new Object[] { listBA, itemBA, listSA, listSP, count });
                            count++;
                        }
                    }
                }
            }
            return objectList;
        }

        [NUnit.Framework.TestCaseSource("BaseDirectionAndSymbolAlignmentProperties")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TYPOGRAPHY_NOT_FOUND, Count = 8)]
        public virtual void DefaultListIemPositionAlignmentTest(BaseDirection? listBaseDirection, BaseDirection? listItemBaseDirection
            , ListSymbolAlignment listSymbolAlignment, ListSymbolPosition listSymbolPosition, int? comparisonPdfId
            ) {
            // Create an HTML for this test
            CreateHtml(listBaseDirection, listItemBaseDirection, listSymbolAlignment, listSymbolPosition);
            String fileName = MessageFormatUtil.Format(RESULTANT_FILE_NAME_PATTERN, FormatSymbolPosition(listSymbolPosition
                ), FormatSymbolAlignment(listSymbolAlignment), FormatBaseDirection(listItemBaseDirection), FormatBaseDirection
                (listBaseDirection));
            String outFileName = DESTINATION_FOLDER + "defaultListItemTest" + comparisonPdfId + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_defaultListItemTest" + comparisonPdfId + ".pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            List list = CreateTestList(listBaseDirection, listItemBaseDirection, listSymbolAlignment, listSymbolPosition
                );
            document.Add(list);
            document.Close();
            System.Console.Out.WriteLine("HTML: " + UrlUtil.GetNormalizedFileUriString(DESTINATION_FOLDER + fileName +
                 ".html") + "\n");
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private List CreateTestList(BaseDirection? listBaseDirection, BaseDirection? listItemBaseDirection, ListSymbolAlignment
             listSymbolAlignment, ListSymbolPosition listSymbolPosition) {
            List list = new List();
            list.SetSymbolIndent(20);
            list.SetListSymbol("\u2022");
            list.SetBackgroundColor(ColorConstants.GREEN);
            ListItem listItem1 = new ListItem();
            listItem1.Add(new Paragraph("Usual item"));
            listItem1.SetBackgroundColor(ColorConstants.BLUE);
            list.Add(listItem1);
            ListItem listItem2 = new ListItem();
            listItem2.Add(new Paragraph("Specific item"));
            listItem2.SetBackgroundColor(ColorConstants.YELLOW);
            listItem2.SetProperty(Property.BASE_DIRECTION, listItemBaseDirection);
            listItem2.SetProperty(Property.LIST_SYMBOL_ALIGNMENT, listSymbolAlignment);
            listItem2.SetProperty(Property.LIST_SYMBOL_POSITION, listSymbolPosition);
            list.Add(listItem2);
            list.SetProperty(Property.BASE_DIRECTION, listBaseDirection);
            return list;
        }

        private void CreateHtml(BaseDirection? listBaseDirection, BaseDirection? listItemBaseDirection, ListSymbolAlignment
             listSymbolAlignment, ListSymbolPosition listSymbolPosition) {
            String fileName = MessageFormatUtil.Format(RESULTANT_FILE_NAME_PATTERN, FormatSymbolPosition(listSymbolPosition
                ), FormatSymbolAlignment(listSymbolAlignment), FormatBaseDirection(listItemBaseDirection), FormatBaseDirection
                (listBaseDirection));
            String htmlString = MessageFormatUtil.Format(HTML_PATTERN, FormatSymbolPosition(listSymbolPosition), FormatSymbolAlignment
                (listSymbolAlignment), FormatBaseDirection(listItemBaseDirection), FormatBaseDirection(listBaseDirection
                ));
            using (Stream htmlFile = FileUtil.GetFileOutputStream(DESTINATION_FOLDER + fileName + ".html")) {
                byte[] htmlBytes = htmlString.GetBytes(System.Text.Encoding.UTF8);
                htmlFile.Write(htmlBytes, 0, htmlBytes.Length);
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

        private static String FormatSymbolAlignment(ListSymbolAlignment alignment) {
            switch (alignment) {
                case ListSymbolAlignment.LEFT: {
                    return "left";
                }

                case ListSymbolAlignment.RIGHT: {
                    return "right";
                }

                default: {
                    NUnit.Framework.Assert.Fail("Unexpected symbol alignment");
                    return null;
                }
            }
        }

        private static String FormatSymbolPosition(ListSymbolPosition position) {
            switch (position) {
                case ListSymbolPosition.OUTSIDE: {
                    return "outside";
                }

                case ListSymbolPosition.INSIDE: {
                    return "inside";
                }

                default: {
                    NUnit.Framework.Assert.Fail("Unexpected symbol position");
                    return null;
                }
            }
        }
    }
}
