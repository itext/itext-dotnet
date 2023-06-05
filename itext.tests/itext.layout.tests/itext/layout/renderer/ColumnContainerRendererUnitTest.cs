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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Splitting;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class ColumnContainerRendererUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SimpleTest() {
            Div columnContainer = new ColumnContainer();
            Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor "
                 + "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation " 
                + "ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in "
                 + "voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non "
                 + "proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");
            columnContainer.Add(FillTextProperties(paragraph));
            columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
            ColumnContainerRenderer renderer = (ColumnContainerRenderer)columnContainer.CreateRendererSubTree();
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(600f, 200.0f))));
            NUnit.Framework.Assert.IsTrue(result.GetSplitRenderer() is ColumnContainerRenderer);
            NUnit.Framework.Assert.AreEqual(3, result.GetSplitRenderer().GetChildRenderers().Count);
            NUnit.Framework.Assert.AreEqual(9, result.GetSplitRenderer().GetChildRenderers()[0].GetChildRenderers().Count
                );
        }

        [NUnit.Framework.Test]
        public virtual void KeepTogetherParagraphTest() {
            Div columnContainer = new ColumnContainer();
            Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor "
                 + "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation " 
                + "ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in "
                 + "voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non "
                 + "proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");
            paragraph.SetProperty(Property.KEEP_TOGETHER, true);
            columnContainer.Add(FillTextProperties(paragraph));
            columnContainer.SetProperty(Property.COLUMN_COUNT, 3);
            ColumnContainerRenderer renderer = (ColumnContainerRenderer)columnContainer.CreateRendererSubTree();
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => renderer.Layout(new LayoutContext(new 
                LayoutArea(1, new Rectangle(200f, 20f)))));
        }

        [NUnit.Framework.Test]
        public virtual void DivWithNoHeightTest() {
            Div div = new ColumnContainer();
            Paragraph paragraph = new Paragraph("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor "
                 + "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation " 
                + "ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in "
                 + "voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non "
                 + "proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");
            div.Add(FillTextProperties(paragraph));
            div.SetProperty(Property.COLUMN_COUNT, 3);
            ColumnContainerRenderer renderer = (ColumnContainerRenderer)div.CreateRendererSubTree();
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(20.0f, 20.0f))));
            NUnit.Framework.Assert.AreEqual(LayoutResult.NOTHING, result.GetStatus());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleParagraphsTest() {
            Div div = new ColumnContainer();
            Div child = new Div();
            Paragraph firstParagraph = new Paragraph("Lorem ipsum dolor sit");
            Paragraph secondParagraph = new Paragraph("consectetur adipiscing elit");
            Paragraph thirdParagraph = new Paragraph("sed do eiusmod tempor incididunt");
            child.Add(FillTextProperties(firstParagraph));
            child.Add(FillTextProperties(secondParagraph));
            child.Add(FillTextProperties(thirdParagraph));
            div.Add(child);
            div.SetProperty(Property.COLUMN_COUNT, 3);
            ColumnContainerRenderer renderer = (ColumnContainerRenderer)div.CreateRendererSubTree();
            LayoutResult result = renderer.Layout(new LayoutContext(new LayoutArea(1, new Rectangle(600f, 30.0f))));
            NUnit.Framework.Assert.IsTrue(result.GetSplitRenderer() is ColumnContainerRenderer);
            NUnit.Framework.Assert.AreEqual(3, result.GetSplitRenderer().GetChildRenderers().Count);
            NUnit.Framework.Assert.AreEqual(1, result.GetSplitRenderer().GetChildRenderers()[0].GetChildRenderers().Count
                );
            NUnit.Framework.Assert.AreEqual(2, ((ParagraphRenderer)result.GetSplitRenderer().GetChildRenderers()[0].GetChildRenderers
                ()[0]).GetLines().Count);
        }

        private static IBlockElement FillTextProperties(IBlockElement container) {
            container.SetProperty(Property.TEXT_RISE, 5.0f);
            container.SetProperty(Property.CHARACTER_SPACING, 5.0f);
            container.SetProperty(Property.WORD_SPACING, 5.0f);
            container.SetProperty(Property.FONT, PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            container.SetProperty(Property.FONT_SIZE, new UnitValue(UnitValue.POINT, 10));
            container.SetProperty(Property.SPLIT_CHARACTERS, new DefaultSplitCharacters());
            return container;
        }
    }
}
