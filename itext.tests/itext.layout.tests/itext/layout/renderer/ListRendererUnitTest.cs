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
using System.Linq;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test.Attributes;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("UnitTest")]
    public class ListRendererUnitTest : RendererUnitTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.GET_NEXT_RENDERER_SHOULD_BE_OVERRIDDEN)]
        public virtual void GetNextRendererShouldBeOverriddenTest() {
            ListRenderer listRenderer = new _ListRenderer_56(new List());
            // Nothing is overridden
            NUnit.Framework.Assert.AreEqual(typeof(ListRenderer), listRenderer.GetNextRenderer().GetType());
        }

        private sealed class _ListRenderer_56 : ListRenderer {
            public _ListRenderer_56(List baseArg1)
                : base(baseArg1) {
            }
        }

        [NUnit.Framework.Test]
        public virtual void SymbolPositioningInsideDrawnOnceTest() {
            ListRendererUnitTest.InvocationsCounter invocationsCounter = new ListRendererUnitTest.InvocationsCounter();
            List modelElement = new List();
            modelElement.SetNextRenderer(new ListRendererUnitTest.ListRendererCreatingNotifyingListSymbols(modelElement
                , invocationsCounter));
            modelElement.Add((ListItem)new ListItem().Add(new Paragraph("ListItem1")).Add(new Paragraph("ListItem2")));
            modelElement.SetProperty(Property.LIST_SYMBOL_POSITION, ListSymbolPosition.INSIDE);
            modelElement.SetFontSize(30);
            // A hack for a test in order to layout the list at the very left border of the parent area.
            // List symbols are not drawn outside the parent area, so we want to make sure that symbol renderer
            // won't be drawn twice even if there's enough space around the list.
            modelElement.SetMarginLeft(500);
            IRenderer listRenderer = modelElement.CreateRendererSubTree();
            Document document = CreateDummyDocument();
            listRenderer.SetParent(document.GetRenderer());
            PdfPage pdfPage = document.GetPdfDocument().AddNewPage();
            // should be enough to fit a single list-item, but not both
            int height = 80;
            // we don't want to impose any width restrictions
            int width = 1000;
            LayoutResult result = listRenderer.Layout(CreateLayoutContext(width, height));
            System.Diagnostics.Debug.Assert(result.GetStatus() == LayoutResult.PARTIAL);
            result.GetSplitRenderer().Draw(new DrawContext(document.GetPdfDocument(), new PdfCanvas(pdfPage)));
            // only split part is drawn, list symbol is expected to be drawn only once.
            NUnit.Framework.Assert.AreEqual(1, invocationsCounter.GetInvocationsCount());
        }

        [NUnit.Framework.Test]
        public virtual void SymbolPositioningInsideAfterPageBreakTest() {
            List modelElement = new List();
            modelElement.SetNextRenderer(new ListRenderer(modelElement));
            for (int i = 0; i < 25; i++) {
                String s = "listitem " + i;
                ListItem listItem = (ListItem)new ListItem().Add(new Paragraph(s));
                modelElement.Add(listItem);
            }
            modelElement.SetProperty(Property.LIST_SYMBOL_POSITION, ListSymbolPosition.INSIDE);
            modelElement.SetFontSize(30);
            IRenderer listRenderer = modelElement.CreateRendererSubTree();
            Document document = CreateDummyDocument();
            listRenderer.SetParent(document.GetRenderer());
            LayoutContext layoutContext = CreateLayoutContext(595, 842);
            LayoutResult result = listRenderer.Layout(layoutContext);
            result.GetOverflowRenderer().Layout(layoutContext);
            Regex regex = iText.Commons.Utils.StringUtil.RegexCompile("^.-.*?-.*$");
            IList<IRenderer> childRenderers = listRenderer.GetChildRenderers();
            NUnit.Framework.Assert.AreEqual(0, childRenderers.Where((listitem) => iText.Commons.Utils.Matcher.Match(regex
                , listitem.ToString()).Matches()).Count());
        }

        private class ListRendererCreatingNotifyingListSymbols : ListRenderer {
            private ListRendererUnitTest.InvocationsCounter counter;

            public ListRendererCreatingNotifyingListSymbols(List modelElement, ListRendererUnitTest.InvocationsCounter
                 counter)
                : base(modelElement) {
                this.counter = counter;
            }

            protected internal override IRenderer MakeListSymbolRenderer(int index, IRenderer renderer) {
                return new ListRendererUnitTest.NotifyingListSymbolRenderer(new iText.Layout.Element.Text("-"), counter);
            }

            public override IRenderer GetNextRenderer() {
                return new ListRendererUnitTest.ListRendererCreatingNotifyingListSymbols((List)GetModelElement(), counter);
            }
        }

        private class NotifyingListSymbolRenderer : TextRenderer {
            private ListRendererUnitTest.InvocationsCounter counter;

            public NotifyingListSymbolRenderer(iText.Layout.Element.Text textElement, ListRendererUnitTest.InvocationsCounter
                 counter)
                : base(textElement) {
                this.counter = counter;
            }

            public override void Draw(DrawContext drawContext) {
                counter.RegisterInvocation();
                base.Draw(drawContext);
            }
        }

        private class InvocationsCounter {
            private int counter = 0;

//\cond DO_NOT_DOCUMENT
            internal virtual void RegisterInvocation() {
                ++counter;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
            internal virtual int GetInvocationsCount() {
                return counter;
            }
//\endcond
        }
    }
}
