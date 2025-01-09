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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ListTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/ListTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/ListTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void NestedListTest01() {
            String outFileName = destinationFolder + "nestedListTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_nestedListTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            List romanList2 = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One")
                .Add("Two").Add("Three");
            List romanList = new List(ListNumberingType.ROMAN_LOWER).SetSymbolIndent(20).SetMarginLeft(25).Add("One").
                Add("Two").Add((ListItem)new ListItem("Three").Add(romanList2));
            List list = new List(ListNumberingType.DECIMAL).SetSymbolIndent(20).Add("One").Add("Two").Add("Three").Add
                ("Four").Add((ListItem)new ListItem("Roman List").Add(romanList)).Add("Five").Add("Six").Add((ListItem
                )new ListItem().Add(romanList2));
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void NestedListTest02() {
            String outFileName = destinationFolder + "nestedListTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_nestedListTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            List nestedList = new List().SetSymbolIndent(20).SetMarginLeft(25).Add("One").Add("Two").Add("Three");
            List list = new List(ListNumberingType.DECIMAL).SetSymbolIndent(20).Add("One").Add("Two").Add("Three").Add
                ("Four").Add((ListItem)new ListItem().Add(nestedList));
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListNestedInTableTest01() {
            String outFileName = destinationFolder + "listNestedInTableTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_listNestedInTableTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument, PageSize.A9.Rotate());
            List list = new List(ListNumberingType.DECIMAL).Add("first string").Add("second string").Add("third string"
                ).Add("fourth string");
            Table table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth();
            table.AddCell(new Cell().Add(list).SetVerticalAlignment(VerticalAlignment.BOTTOM).SetFontSize(10));
            document.Add(table);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListNumberingTest01() {
            String outFileName = destinationFolder + "listNumberingTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_listNumberingTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            IList<List> lists = new List<List>();
            lists.Add(new List(ListNumberingType.DECIMAL));
            lists.Add(new List(ListNumberingType.ROMAN_LOWER));
            lists.Add(new List(ListNumberingType.ROMAN_UPPER));
            lists.Add(new List(ListNumberingType.ENGLISH_LOWER));
            lists.Add(new List(ListNumberingType.ENGLISH_UPPER));
            lists.Add(new List(ListNumberingType.GREEK_LOWER));
            lists.Add(new List(ListNumberingType.GREEK_UPPER));
            for (int i = 1; i <= 30; i++) {
                foreach (List list in lists) {
                    list.Add("Item #" + i);
                }
            }
            foreach (List list in lists) {
                document.Add(list).Add(new AreaBreak());
            }
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 8)]
        public virtual void AddListOnShortPage1() {
            String outFileName = destinationFolder + "addListOnShortPage1.pdf";
            String cmpFileName = sourceFolder + "cmp_addListOnShortPage1.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc, new PageSize(500, 60));
            ListItem item = new ListItem();
            ListItem nestedItem = new ListItem();
            List list = new List(ListNumberingType.DECIMAL);
            List nestedList = new List(ListNumberingType.ENGLISH_UPPER);
            List nestedNestedList = new List(ListNumberingType.GREEK_LOWER);
            nestedNestedList.Add("Hello");
            nestedNestedList.Add("World");
            nestedItem.Add(nestedNestedList);
            nestedList.Add(nestedItem);
            nestedList.Add(nestedItem);
            item.Add(nestedList);
            list.Add(item);
            list.Add(item);
            doc.Add(list);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 3)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_TO_CREATE_A_TAG_FOR_FINISHED_HINT, Count = 6)]
        public virtual void AddListOnShortPage2() {
            String outFileName = destinationFolder + "addListOnShortPage2.pdf";
            String cmpFileName = sourceFolder + "cmp_addListOnShortPage2.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName)).SetTagged();
            Document doc = new Document(pdfDoc, new PageSize(500, 130));
            List list = new List(ListNumberingType.DECIMAL);
            ListItem item = new ListItem();
            item.Add(new Paragraph("Red"));
            item.Add(new Paragraph("Is"));
            item.Add(new Paragraph("The"));
            item.Add(new Paragraph("Color"));
            item.Add(new Image(ImageDataFactory.Create(sourceFolder + "red.png")));
            List nestedList = new List(ListNumberingType.ENGLISH_UPPER);
            nestedList.Add("Hello");
            nestedList.Add("World");
            item.Add(nestedList);
            for (int i = 0; i < 3; i++) {
                list.Add(item);
            }
            doc.Add(list);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void DivInListItemTest01() {
            String outFileName = destinationFolder + "divInListItemTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_divInListItemTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            ListItem item = new ListItem();
            item.Add(new Div().Add(new Paragraph("text")));
            document.Add(new List().Add(item));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListOverflowTest01() {
            String outFileName = destinationFolder + "listOverflowTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_listOverflowTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph p = new Paragraph("Test String");
            List list = new List(ListNumberingType.DECIMAL).Add("first string").Add("second string").Add("third string"
                ).Add("fourth string");
            for (int i = 0; i < 28; i++) {
                document.Add(p);
            }
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListOverflowTest02() {
            String outFileName = destinationFolder + "listOverflowTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_listOverflowTest02.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph p = new Paragraph("Test String");
            List list = new List(ListNumberingType.DECIMAL).Add("first string");
            ListItem item = (ListItem)new ListItem("second string").Add(new Paragraph("third string"));
            list.Add(item).Add("fourth item");
            for (int i = 0; i < 28; i++) {
                document.Add(p);
            }
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListOverflowTest03() {
            String outFileName = destinationFolder + "listOverflowTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_listOverflowTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            Paragraph p = new Paragraph("Test String");
            List list = new List(ListNumberingType.DECIMAL).SetItemStartIndex(10).Add("first string").Add("second string"
                ).Add("third string").Add("fourth string");
            for (int i = 0; i < 28; i++) {
                document.Add(p);
            }
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListEmptyItemTest01() {
            String outFileName = destinationFolder + "listEmptyItemTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_listEmptyItemTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName)).SetTagged();
            Document document = new Document(pdfDocument);
            List list = new List(ListNumberingType.GREEK_LOWER);
            list.Add(new ListItem()).Add(new ListItem()).Add(new ListItem()).Add("123").Add((ListItem)new ListItem().Add
                (new Div()));
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageInListTest01() {
            String outFileName = destinationFolder + "imageInListTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_imageInListTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            List list = new List(ListNumberingType.GREEK_LOWER);
            PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 100);
            list.Add(new ListItem()).Add(new ListItem(image)).Add(new ListItem()).Add("123").Add((ListItem)new ListItem
                ().Add(new Div().SetHeight(70).SetBackgroundColor(ColorConstants.RED)));
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListItemAlignmentTest01() {
            String outFileName = destinationFolder + "listItemAlignmentTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_listItemAlignmentTest01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDocument);
            List list = new List(ListNumberingType.DECIMAL).SetListSymbolAlignment(ListSymbolAlignment.LEFT);
            for (int i = 1; i <= 30; i++) {
                list.Add("Item #" + i);
            }
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListItemTest01() {
            String outFileName = destinationFolder + "listItemTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_listItemTest01.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            List list = new List();
            list.Add(new ListItem("The quick brown").SetListSymbol(ListNumberingType.ZAPF_DINGBATS_1)).Add(new ListItem
                ("fox").SetListSymbol(ListNumberingType.ZAPF_DINGBATS_2)).Add(new ListItem("jumps over the lazy").SetListSymbol
                (ListNumberingType.ZAPF_DINGBATS_3)).Add(new ListItem("dog").SetListSymbol(ListNumberingType.ZAPF_DINGBATS_4
                ));
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListItemTest02() {
            String outFileName = destinationFolder + "listItemTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_listItemTest02.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            document.SetFontColor(ColorConstants.WHITE);
            List list = new List();
            Style liStyle = new Style().SetMargins(20, 0, 20, 0).SetBackgroundColor(ColorConstants.BLACK);
            list.Add((ListItem)new ListItem("").AddStyle(liStyle)).Add((ListItem)new ListItem("fox").AddStyle(liStyle)
                ).Add((ListItem)new ListItem("").AddStyle(liStyle)).Add((ListItem)new ListItem("dog").AddStyle(liStyle
                ));
            document.Add(list.SetBackgroundColor(ColorConstants.BLUE));
            document.Add(new Paragraph("separation between lists"));
            liStyle.SetMargin(0);
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListItemWithoutMarginsTest() {
            String outFileName = destinationFolder + "listItemWithoutMarginsTest.pdf";
            String cmpFileName = sourceFolder + "cmp_listItemWithoutMarginsTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            document.SetMargins(0, 0, 0, 0);
            List list = new List();
            list.SetListSymbol(ListNumberingType.DECIMAL);
            list.Add(new ListItem("list item 1"));
            list.Add(new ListItem("list item 2"));
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListItemBigMarginsTest() {
            String outFileName = destinationFolder + "listItemBigMarginsTest.pdf";
            String cmpFileName = sourceFolder + "cmp_listItemBigMarginsTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            int margin = 100;
            document.SetMargins(margin, margin, margin, margin);
            List list = new List();
            list.SetListSymbol(ListNumberingType.DECIMAL);
            list.Add(new ListItem("list item 1"));
            list.Add(new ListItem("list item 2"));
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void MaxMarginWidthWhereTheBulletIsNotDrawnTest() {
            String outFileName = destinationFolder + "maxMarginWidthWhereTheBulletIsNotDrawn.pdf";
            String cmpFileName = sourceFolder + "cmp_maxMarginWidthWhereTheBulletIsNotDrawn.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            int margin = 50;
            document.SetMargins(margin, margin, margin, margin);
            List list = new List();
            list.SetListSymbol(ListNumberingType.DECIMAL);
            list.Add(new ListItem("list item 1"));
            list.Add(new ListItem("list item 2"));
            float? marginLeft = document.GetDefaultProperty<float>(Property.MARGIN_LEFT);
            list.SetFixedPosition((float)marginLeft, 780, 200);
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void InitialMarginWidthWhereTheBulletIsDrawnTest() {
            String outFileName = destinationFolder + "initialMarginWidthWhereTheBulletIsDrawn.pdf";
            String cmpFileName = sourceFolder + "cmp_initialMarginWidthWhereTheBulletIsDrawn.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            int margin = 49;
            document.SetMargins(margin, margin, margin, margin);
            List list = new List();
            list.SetListSymbol(ListNumberingType.DECIMAL);
            list.Add(new ListItem("list item 1"));
            list.Add(new ListItem("list item 2"));
            float? marginLeft = document.GetDefaultProperty<float>(Property.MARGIN_LEFT);
            list.SetFixedPosition((float)marginLeft, 780, 200);
            document.Add(list);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 4)]
        [NUnit.Framework.Test]
        public virtual void ListWithSetHeightProperties01() {
            String outFileName = destinationFolder + "listWithSetHeightProperties01.pdf";
            String cmpFileName = sourceFolder + "cmp_listWithSetHeightProperties01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("Default layout:"));
            ListItem item = new ListItem();
            ListItem nestedItem = new ListItem();
            List list = new List(ListNumberingType.DECIMAL);
            List nestedList = new List(ListNumberingType.ENGLISH_UPPER);
            List nestedNestedList = new List(ListNumberingType.GREEK_LOWER);
            nestedNestedList.Add("Hello");
            nestedNestedList.Add("World");
            nestedItem.Add(nestedNestedList);
            nestedList.Add(nestedItem);
            nestedList.Add(nestedItem);
            item.Add(nestedList);
            list.Add(item);
            list.Add(item);
            list.SetBorder(new SolidBorder(ColorConstants.RED, 3));
            doc.Add(list);
            doc.Add(new AreaBreak());
            doc.Add(new Paragraph("List's height is set shorter than needed:"));
            list.SetHeight(50);
            doc.Add(list);
            doc.Add(new AreaBreak());
            list.DeleteOwnProperty(Property.HEIGHT);
            list.DeleteOwnProperty(Property.MIN_HEIGHT);
            list.DeleteOwnProperty(Property.MAX_HEIGHT);
            doc.Add(new Paragraph("List's min height is set shorter than needed:"));
            list.SetMinHeight(50);
            doc.Add(list);
            doc.Add(new AreaBreak());
            list.DeleteOwnProperty(Property.HEIGHT);
            list.DeleteOwnProperty(Property.MIN_HEIGHT);
            list.DeleteOwnProperty(Property.MAX_HEIGHT);
            doc.Add(new Paragraph("List's max height is set shorter than needed:"));
            list.SetMaxHeight(50);
            doc.Add(list);
            doc.Add(new AreaBreak());
            list.DeleteOwnProperty(Property.HEIGHT);
            list.DeleteOwnProperty(Property.MIN_HEIGHT);
            list.DeleteOwnProperty(Property.MAX_HEIGHT);
            doc.Add(new Paragraph("List's height is set bigger than needed:"));
            list.SetHeight(1300);
            doc.Add(list);
            doc.Add(new AreaBreak());
            list.DeleteOwnProperty(Property.HEIGHT);
            list.DeleteOwnProperty(Property.MIN_HEIGHT);
            list.DeleteOwnProperty(Property.MAX_HEIGHT);
            doc.Add(new Paragraph("List's min height is set bigger than needed:"));
            list.SetMinHeight(1300);
            doc.Add(list);
            doc.Add(new AreaBreak());
            list.DeleteOwnProperty(Property.HEIGHT);
            list.DeleteOwnProperty(Property.MIN_HEIGHT);
            list.DeleteOwnProperty(Property.MAX_HEIGHT);
            doc.Add(new Paragraph("List's max height is set bigger than needed:"));
            list.SetMaxHeight(1300);
            doc.Add(list);
            doc.Add(new AreaBreak());
            list.DeleteOwnProperty(Property.HEIGHT);
            list.DeleteOwnProperty(Property.MIN_HEIGHT);
            list.DeleteOwnProperty(Property.MAX_HEIGHT);
            doc.Add(new Paragraph("Some list items' and nested lists' heights are set bigger or shorter than needed:")
                );
            nestedList.SetHeight(400);
            nestedItem.SetHeight(300);
            doc.Add(list);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ListSetSymbol() {
            List list = new List();
            NUnit.Framework.Assert.IsNull(list.GetProperty<Object>(Property.LIST_SYMBOL));
            list.SetListSymbol("* ");
            NUnit.Framework.Assert.AreEqual("* ", ((Text)list.GetProperty<Object>(Property.LIST_SYMBOL)).GetText());
            list = new List();
            Style style = new Style();
            style.SetProperty(Property.LIST_SYMBOL, new Text("* "));
            list.AddStyle(style);
            NUnit.Framework.Assert.AreEqual("* ", ((Text)list.GetProperty<Object>(Property.LIST_SYMBOL)).GetText());
        }

        [NUnit.Framework.Test]
        public virtual void ListItemNullSymbol() {
            String outFileName = destinationFolder + "listItemNullSymbol.pdf";
            String cmpFileName = sourceFolder + "cmp_listItemNullSymbol.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            List list = new List();
            list.Add(new ListItem("List item 1"));
            Text listSymbolText = null;
            ListItem listItem2 = new ListItem("List item 2").SetListSymbol(listSymbolText);
            list.Add(listItem2);
            list.Add(new ListItem("List item 3"));
            doc.Add(list);
            doc.Add(new LineSeparator(new DashedLine()));
            list.SetListSymbol(ListNumberingType.ENGLISH_LOWER);
            doc.Add(list);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA)]
        public virtual void ListSymbolForcedPlacement01() {
            String outFileName = destinationFolder + "listSymbolForcedPlacement01.pdf";
            String cmpFileName = sourceFolder + "cmp_listSymbolForcedPlacement01.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            // This may seem like a contrived example, but in real life, this happened
            // with a two-column layout. The key is that the label is wider than the column.
            pdf.SetDefaultPageSize(PageSize.A7);
            document.Add(new Paragraph("Before list."));
            List l = new List();
            ListItem li = new ListItem().SetListSymbol("Aircraft of comparable role, configuration and era");
            l.Add(li);
            document.Add(l);
            document.Add(new Paragraph("After list."));
            document.Close();
            // TODO DEVSIX-1655: partially not fitting list symbol not shown at all, however this might be improved
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void BothSymbolIndentAndMarginAreSetTest() {
            // There is no symbol indent in html: one uses margins for such a purpose.
            String outFileName = destinationFolder + "bothSymbolIndentAndMarginAreSetTest.pdf";
            String cmpFileName = sourceFolder + "cmp_bothSymbolIndentAndMarginAreSetTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            List l = CreateTestList();
            ListItem li = new ListItem("Only symbol indent: 50pt");
            li.SetBackgroundColor(ColorConstants.BLUE);
            l.Add(li);
            li = new ListItem("Symbol indent: 50pt and margin-left: 50pt = 100pt");
            li.SetMarginLeft(50);
            li.SetBackgroundColor(ColorConstants.YELLOW);
            l.Add(li);
            document.Add(l);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED)]
        public virtual void ListItemMarginInPercentTest() {
            String outFileName = destinationFolder + "listItemMarginInPercentTest.pdf";
            String cmpFileName = sourceFolder + "cmp_listItemMarginInPercentTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            List l = CreateTestList();
            ListItem li = new ListItem("Left margin in percent: 50%");
            li.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePercentValue(50));
            li.SetBackgroundColor(ColorConstants.BLUE);
            l.Add(li);
            document.Add(l);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ListItemWithImageSymbolPositionTest() {
            String outFileName = destinationFolder + "listItemWithImageSymbolPositionTest.pdf";
            String cmpFileName = sourceFolder + "cmp_listItemWithImageSymbolPositionTest.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            List l = new List();
            l.SetMarginLeft(50);
            l.SetSymbolIndent(20);
            l.SetListSymbol("\u2022");
            l.SetBackgroundColor(ColorConstants.GREEN);
            ImageData img = ImageDataFactory.Create(sourceFolder + "red.png");
            PdfImageXObject xObject = new PdfImageXObject(img);
            ListItem listItemImg1 = new ListItem();
            listItemImg1.Add(new iText.Layout.Element.Image(xObject).SetHeight(30));
            listItemImg1.SetProperty(Property.LIST_SYMBOL_POSITION, ListSymbolPosition.INSIDE);
            l.Add(listItemImg1);
            ListItem listItemImg2 = new ListItem();
            listItemImg2.Add(new iText.Layout.Element.Image(xObject).SetHeight(30));
            listItemImg2.SetProperty(Property.LIST_SYMBOL_POSITION, ListSymbolPosition.OUTSIDE);
            l.Add(listItemImg2);
            document.Add(l);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }

        // TODO DEVSIX-6877 wrapping list item content in a div causes the bullet to be misaligned
        [NUnit.Framework.Test]
        public virtual void ListItemWrappedDivSymbolInside() {
            String outFileName = destinationFolder + "listItemWrappedDivSymbolInside.pdf";
            String cmpFileName = sourceFolder + "cmp_listItemWrappedDivSymbolInside.pdf";
            PdfDocument pdf = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdf);
            List l = new List();
            l.SetMarginLeft(50);
            l.SetListSymbol("\u2022");
            l.SetBackgroundColor(ColorConstants.GREEN);
            l.Add("Regular item 1");
            ListItem listItem = new ListItem();
            listItem.Add(new Div().Add(new Paragraph("Wrapped text")));
            listItem.SetProperty(Property.LIST_SYMBOL_POSITION, ListSymbolPosition.INSIDE);
            l.Add(listItem);
            l.Add("Regular item 2");
            document.Add(l);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff_"));
        }

        private static List CreateTestList() {
            List l = new List();
            l.SetWidth(300);
            l.SetBackgroundColor(ColorConstants.RED);
            l.SetSymbolIndent(50);
            l.SetListSymbol("\u2022");
            return l;
        }
    }
}
