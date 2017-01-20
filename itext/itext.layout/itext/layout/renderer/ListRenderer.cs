/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Numbering;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class ListRenderer : BlockRenderer {
        /// <summary>Creates a ListRenderer from its corresponding layout object.</summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.List"/>
        /// which this object should manage
        /// </param>
        public ListRenderer(List modelElement)
            : base(modelElement) {
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            OverrideHeightProperties();
            if (!HasOwnProperty(Property.LIST_SYMBOLS_INITIALIZED)) {
                IList<IRenderer> symbolRenderers = new List<IRenderer>();
                int listItemNum = (int)this.GetProperty<int?>(Property.LIST_START, 1);
                for (int i = 0; i < childRenderers.Count; i++) {
                    childRenderers[i].SetParent(this);
                    IRenderer currentSymbolRenderer = MakeListSymbolRenderer(listItemNum++, childRenderers[i]);
                    childRenderers[i].SetParent(null);
                    currentSymbolRenderer.SetParent(this);
                    // Workaround for the case when font is specified as string
                    if (currentSymbolRenderer is AbstractRenderer && currentSymbolRenderer.GetProperty<Object>(Property.FONT) 
                        is String) {
                        PdfFont actualPdfFont = ((AbstractRenderer)currentSymbolRenderer).ResolveFirstPdfFont();
                        currentSymbolRenderer.SetProperty(Property.FONT, actualPdfFont);
                    }
                    symbolRenderers.Add(currentSymbolRenderer);
                    LayoutResult listSymbolLayoutResult = currentSymbolRenderer.Layout(layoutContext);
                    currentSymbolRenderer.SetParent(null);
                    if (listSymbolLayoutResult.GetStatus() != LayoutResult.FULL) {
                        return new LayoutResult(LayoutResult.NOTHING, null, null, this, listSymbolLayoutResult.GetCauseOfNothing()
                            );
                    }
                }
                float maxSymbolWidth = 0;
                for (int i = 0; i < childRenderers.Count; i++) {
                    IRenderer symbolRenderer = symbolRenderers[i];
                    IRenderer listItemRenderer = childRenderers[i];
                    if ((ListSymbolPosition)listItemRenderer.GetProperty<Object>(Property.LIST_SYMBOL_POSITION) != ListSymbolPosition
                        .INSIDE) {
                        maxSymbolWidth = Math.Max(maxSymbolWidth, symbolRenderer.GetOccupiedArea().GetBBox().GetWidth());
                    }
                }
                float? symbolIndent = this.GetPropertyAsFloat(Property.LIST_SYMBOL_INDENT);
                listItemNum = 0;
                foreach (IRenderer childRenderer in childRenderers) {
                    childRenderer.SetParent(this);
                    childRenderer.DeleteOwnProperty(Property.MARGIN_LEFT);
                    float calculatedMargin = (float)childRenderer.GetProperty(Property.MARGIN_LEFT, (float?)0f);
                    if ((ListSymbolPosition)childRenderer.GetProperty<Object>(Property.LIST_SYMBOL_POSITION) == ListSymbolPosition
                        .DEFAULT) {
                        calculatedMargin += maxSymbolWidth + (float)(symbolIndent != null ? symbolIndent : 0f);
                    }
                    childRenderer.SetProperty(Property.MARGIN_LEFT, calculatedMargin);
                    IRenderer symbolRenderer = symbolRenderers[listItemNum++];
                    ((ListItemRenderer)childRenderer).AddSymbolRenderer(symbolRenderer, maxSymbolWidth);
                }
            }
            LayoutResult result = base.Layout(layoutContext);
            // cannot place even the first ListItemRenderer
            if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT)) && null != result.GetCauseOfNothing()) {
                if (LayoutResult.FULL == result.GetStatus()) {
                    result = CorrectListSplitting(this, null, result.GetCauseOfNothing(), result.GetOccupiedArea());
                }
                else {
                    if (LayoutResult.PARTIAL == result.GetStatus()) {
                        result = CorrectListSplitting(result.GetSplitRenderer(), result.GetOverflowRenderer(), result.GetCauseOfNothing
                            (), result.GetOccupiedArea());
                    }
                }
            }
            return result;
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.ListRenderer((List)modelElement);
        }

        protected internal override AbstractRenderer CreateSplitRenderer(int layoutResult) {
            AbstractRenderer splitRenderer = base.CreateSplitRenderer(layoutResult);
            splitRenderer.SetProperty(Property.LIST_SYMBOLS_INITIALIZED, true);
            return splitRenderer;
        }

        protected internal override AbstractRenderer CreateOverflowRenderer(int layoutResult) {
            AbstractRenderer overflowRenderer = base.CreateOverflowRenderer(layoutResult);
            overflowRenderer.SetProperty(Property.LIST_SYMBOLS_INITIALIZED, true);
            return overflowRenderer;
        }

        protected internal virtual IRenderer MakeListSymbolRenderer(int index, IRenderer renderer) {
            IRenderer symbolRenderer = CreateListSymbolRenderer(index, renderer);
            // underlying should not be applied
            if (symbolRenderer != null) {
                symbolRenderer.SetProperty(Property.UNDERLINE, false);
            }
            return symbolRenderer;
        }

        private IRenderer CreateListSymbolRenderer(int index, IRenderer renderer) {
            Object defaultListSymbol = renderer.GetProperty<Object>(Property.LIST_SYMBOL);
            if (defaultListSymbol is Text) {
                return new TextRenderer((Text)defaultListSymbol);
            }
            else {
                if (defaultListSymbol is Image) {
                    return new ImageRenderer((Image)defaultListSymbol);
                }
                else {
                    if (defaultListSymbol is ListNumberingType) {
                        ListNumberingType numberingType = (ListNumberingType)defaultListSymbol;
                        String numberText;
                        switch (numberingType) {
                            case ListNumberingType.DECIMAL: {
                                numberText = index.ToString();
                                break;
                            }

                            case ListNumberingType.DECIMAL_LEADING_ZERO: {
                                numberText = (index < 10 ? "0" : "") + index.ToString();
                                break;
                            }

                            case ListNumberingType.ROMAN_LOWER: {
                                numberText = RomanNumbering.ToRomanLowerCase(index);
                                break;
                            }

                            case ListNumberingType.ROMAN_UPPER: {
                                numberText = RomanNumbering.ToRomanUpperCase(index);
                                break;
                            }

                            case ListNumberingType.ENGLISH_LOWER: {
                                numberText = EnglishAlphabetNumbering.ToLatinAlphabetNumberLowerCase(index);
                                break;
                            }

                            case ListNumberingType.ENGLISH_UPPER: {
                                numberText = EnglishAlphabetNumbering.ToLatinAlphabetNumberUpperCase(index);
                                break;
                            }

                            case ListNumberingType.GREEK_LOWER: {
                                numberText = GreekAlphabetNumbering.ToGreekAlphabetNumberLowerCase(index);
                                break;
                            }

                            case ListNumberingType.GREEK_UPPER: {
                                numberText = GreekAlphabetNumbering.ToGreekAlphabetNumberUpperCase(index);
                                break;
                            }

                            case ListNumberingType.ZAPF_DINGBATS_1: {
                                numberText = iText.IO.Util.JavaUtil.CharToString((char)(index + 171));
                                break;
                            }

                            case ListNumberingType.ZAPF_DINGBATS_2: {
                                numberText = iText.IO.Util.JavaUtil.CharToString((char)(index + 181));
                                break;
                            }

                            case ListNumberingType.ZAPF_DINGBATS_3: {
                                numberText = iText.IO.Util.JavaUtil.CharToString((char)(index + 191));
                                break;
                            }

                            case ListNumberingType.ZAPF_DINGBATS_4: {
                                numberText = iText.IO.Util.JavaUtil.CharToString((char)(index + 201));
                                break;
                            }

                            default: {
                                throw new InvalidOperationException();
                            }
                        }
                        Text textElement = new Text(renderer.GetProperty<String>(Property.LIST_SYMBOL_PRE_TEXT) + numberText + renderer
                            .GetProperty<String>(Property.LIST_SYMBOL_POST_TEXT));
                        IRenderer textRenderer;
                        // Be careful. There is a workaround here. For Greek symbols we first set a dummy font with document=null
                        // in order for the metrics to be taken into account correctly during layout.
                        // Then on draw we set the correct font with actual document in order for the font objects to be created.
                        if (numberingType == ListNumberingType.GREEK_LOWER || numberingType == ListNumberingType.GREEK_UPPER || numberingType
                             == ListNumberingType.ZAPF_DINGBATS_1 || numberingType == ListNumberingType.ZAPF_DINGBATS_2 || numberingType
                             == ListNumberingType.ZAPF_DINGBATS_3 || numberingType == ListNumberingType.ZAPF_DINGBATS_4) {
                            String constantFont = (numberingType == ListNumberingType.GREEK_LOWER || numberingType == ListNumberingType
                                .GREEK_UPPER) ? FontConstants.SYMBOL : FontConstants.ZAPFDINGBATS;
                            textRenderer = new _TextRenderer_227(constantFont, textElement);
                            try {
                                textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(constantFont));
                            }
                            catch (System.IO.IOException) {
                            }
                        }
                        else {
                            textRenderer = new TextRenderer(textElement);
                        }
                        return textRenderer;
                    }
                    else {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        private sealed class _TextRenderer_227 : TextRenderer {
            public _TextRenderer_227(String constantFont, Text baseArg1)
                : base(baseArg1) {
                this.constantFont = constantFont;
            }

            public override void Draw(DrawContext drawContext) {
                try {
                    this.SetProperty(Property.FONT, PdfFontFactory.CreateFont(constantFont));
                }
                catch (System.IO.IOException) {
                }
                base.Draw(drawContext);
            }

            private readonly String constantFont;
        }

        /// <summary>
        /// <p>
        /// Corrects split and overflow renderers when
        /// <see cref="iText.Layout.Properties.Property.FORCED_PLACEMENT"/>
        /// is applied.
        /// We assume that
        /// <see cref="iText.Layout.Properties.Property.FORCED_PLACEMENT"/>
        /// is applied when the first
        /// <see cref="ListItemRenderer"/>
        /// cannot be fully layouted.
        /// This means that the problem has occurred in one of first list item renderer's child.
        /// We consider the right solution to force placement of all first item renderer's childs before the one,
        /// which was the cause of
        /// <see cref="iText.Layout.Layout.LayoutResult.NOTHING"/>
        /// , including this child.
        /// </p>
        /// <p>
        /// Notice that we do not expect
        /// <see cref="iText.Layout.Properties.Property.FORCED_PLACEMENT"/>
        /// to be applied
        /// if we can render the first item renderer and strongly recommend not to set
        /// <see cref="iText.Layout.Properties.Property.FORCED_PLACEMENT"/>
        /// manually.
        /// </p>
        /// </summary>
        /// <param name="splitRenderer">
        /// the
        /// <see cref="IRenderer">split renderer</see>
        /// before correction
        /// </param>
        /// <param name="overflowRenderer">
        /// the
        /// <see cref="IRenderer">overflow renderer</see>
        /// before correction
        /// </param>
        /// <param name="causeOfNothing">
        /// the
        /// <see cref="com.itextpdf.layout.layout.LayoutResult#causeOfNothing">cause of nothing renderer</see>
        /// </param>
        /// <param name="occupiedArea">the area occupied by layouting before correction</param>
        /// <returns>
        /// corrected
        /// <see cref="iText.Layout.Layout.LayoutResult">layout result</see>
        /// </returns>
        private LayoutResult CorrectListSplitting(IRenderer splitRenderer, IRenderer overflowRenderer, IRenderer causeOfNothing
            , LayoutArea occupiedArea) {
            // the first not rendered child
            int firstNotRendered = splitRenderer.GetChildRenderers()[0].GetChildRenderers().IndexOf(causeOfNothing);
            if (-1 == firstNotRendered) {
                return new LayoutResult(null == overflowRenderer ? LayoutResult.FULL : LayoutResult.PARTIAL, occupiedArea, 
                    splitRenderer, overflowRenderer, this);
            }
            // Notice that placed item is a son of the first ListItemRenderer (otherwise there would be now FORCED_PLACEMENT applied)
            IRenderer firstListItemRenderer = splitRenderer.GetChildRenderers()[0];
            iText.Layout.Renderer.ListRenderer newOverflowRenderer = (iText.Layout.Renderer.ListRenderer)CreateOverflowRenderer
                (LayoutResult.PARTIAL);
            newOverflowRenderer.DeleteOwnProperty(Property.FORCED_PLACEMENT);
            // ListItemRenderer for not rendered children of firstListItemRenderer
            newOverflowRenderer.childRenderers.Add(new ListItemRenderer((ListItem)firstListItemRenderer.GetModelElement
                ()));
            newOverflowRenderer.childRenderers.AddAll(splitRenderer.GetChildRenderers().SubList(1, splitRenderer.GetChildRenderers
                ().Count));
            IList<IRenderer> childrenStillRemainingToRender = new List<IRenderer>(firstListItemRenderer.GetChildRenderers
                ().SubList(firstNotRendered + 1, firstListItemRenderer.GetChildRenderers().Count));
            // 'this' renderer will become split renderer
            splitRenderer.GetChildRenderers().RemoveAll(splitRenderer.GetChildRenderers().SubList(1, splitRenderer.GetChildRenderers
                ().Count));
            if (0 != childrenStillRemainingToRender.Count) {
                newOverflowRenderer.GetChildRenderers()[0].GetChildRenderers().AddAll(childrenStillRemainingToRender);
                splitRenderer.GetChildRenderers()[0].GetChildRenderers().RemoveAll(childrenStillRemainingToRender);
                newOverflowRenderer.GetChildRenderers()[0].SetProperty(Property.MARGIN_LEFT, splitRenderer.GetChildRenderers
                    ()[0].GetProperty<float?>(Property.MARGIN_LEFT));
            }
            else {
                newOverflowRenderer.childRenderers.JRemoveAt(0);
            }
            if (null != overflowRenderer) {
                newOverflowRenderer.childRenderers.AddAll(overflowRenderer.GetChildRenderers());
            }
            if (0 != newOverflowRenderer.childRenderers.Count) {
                return new LayoutResult(LayoutResult.PARTIAL, occupiedArea, splitRenderer, newOverflowRenderer, this);
            }
            else {
                return new LayoutResult(LayoutResult.FULL, occupiedArea, null, null, this);
            }
        }
    }
}
