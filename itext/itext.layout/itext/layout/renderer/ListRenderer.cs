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
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Numbering;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Tagging;

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
            LayoutResult errorResult = InitializeListSymbols(layoutContext);
            if (errorResult != null) {
                return errorResult;
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

        /// <summary>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// </summary>
        /// <remarks>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// <para />
        /// If a renderer overflows to the next area, iText uses this method to create a renderer
        /// for the overflow part. So if one wants to extend
        /// <see cref="ListRenderer"/>
        /// , one should override
        /// this method: otherwise the default method will be used and thus the default rather than the custom
        /// renderer will be created.
        /// </remarks>
        /// <returns>new renderer instance</returns>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.ListRenderer), this.GetType());
            return new iText.Layout.Renderer.ListRenderer((List)modelElement);
        }

        protected internal override AbstractRenderer CreateSplitRenderer(int layoutResult) {
            AbstractRenderer splitRenderer = base.CreateSplitRenderer(layoutResult);
            splitRenderer.AddAllProperties(GetOwnProperties());
            splitRenderer.SetProperty(Property.LIST_SYMBOLS_INITIALIZED, true);
            return splitRenderer;
        }

        protected internal override AbstractRenderer CreateOverflowRenderer(int layoutResult) {
            AbstractRenderer overflowRenderer = base.CreateOverflowRenderer(layoutResult);
            overflowRenderer.AddAllProperties(GetOwnProperties());
            overflowRenderer.SetProperty(Property.LIST_SYMBOLS_INITIALIZED, true);
            return overflowRenderer;
        }

        public override MinMaxWidth GetMinMaxWidth() {
            LayoutResult errorResult = InitializeListSymbols(new LayoutContext(new LayoutArea(1, new Rectangle(MinMaxWidthUtils
                .GetInfWidth(), AbstractRenderer.INF))));
            if (errorResult != null) {
                return MinMaxWidthUtils.CountDefaultMinMaxWidth(this);
            }
            return base.GetMinMaxWidth();
        }

        protected internal virtual IRenderer MakeListSymbolRenderer(int index, IRenderer renderer) {
            IRenderer symbolRenderer = CreateListSymbolRenderer(index, renderer);
            // underlying should not be applied
            if (symbolRenderer != null) {
                symbolRenderer.SetProperty(Property.UNDERLINE, false);
            }
            return symbolRenderer;
        }

        internal static Object GetListItemOrListProperty(IRenderer listItem, IRenderer list, int propertyId) {
            return listItem.HasProperty(propertyId) ? listItem.GetProperty<Object>(propertyId) : list.GetProperty<Object
                >(propertyId);
        }

        private IRenderer CreateListSymbolRenderer(int index, IRenderer renderer) {
            Object defaultListSymbol = GetListItemOrListProperty(renderer, this, Property.LIST_SYMBOL);
            if (defaultListSymbol is Text) {
                return SurroundTextBullet(new TextRenderer((Text)defaultListSymbol));
            }
            else {
                if (defaultListSymbol is Image) {
                    return ((Image)defaultListSymbol).GetRenderer();
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
                                numberText = GreekAlphabetNumbering.ToGreekAlphabetNumber(index, false, true);
                                break;
                            }

                            case ListNumberingType.GREEK_UPPER: {
                                numberText = GreekAlphabetNumbering.ToGreekAlphabetNumber(index, true, true);
                                break;
                            }

                            case ListNumberingType.ZAPF_DINGBATS_1: {
                                numberText = JavaUtil.CharToString((char)(index + 171));
                                break;
                            }

                            case ListNumberingType.ZAPF_DINGBATS_2: {
                                numberText = JavaUtil.CharToString((char)(index + 181));
                                break;
                            }

                            case ListNumberingType.ZAPF_DINGBATS_3: {
                                numberText = JavaUtil.CharToString((char)(index + 191));
                                break;
                            }

                            case ListNumberingType.ZAPF_DINGBATS_4: {
                                numberText = JavaUtil.CharToString((char)(index + 201));
                                break;
                            }

                            default: {
                                throw new InvalidOperationException();
                            }
                        }
                        Text textElement = new Text(GetListItemOrListProperty(renderer, this, Property.LIST_SYMBOL_PRE_TEXT) + numberText
                             + GetListItemOrListProperty(renderer, this, Property.LIST_SYMBOL_POST_TEXT));
                        IRenderer textRenderer;
                        // Be careful. There is a workaround here. For Greek symbols we first set a dummy font with document=null
                        // in order for the metrics to be taken into account correctly during layout.
                        // Then on draw we set the correct font with actual document in order for the font objects to be created.
                        if (numberingType == ListNumberingType.GREEK_LOWER || numberingType == ListNumberingType.GREEK_UPPER || numberingType
                             == ListNumberingType.ZAPF_DINGBATS_1 || numberingType == ListNumberingType.ZAPF_DINGBATS_2 || numberingType
                             == ListNumberingType.ZAPF_DINGBATS_3 || numberingType == ListNumberingType.ZAPF_DINGBATS_4) {
                            String constantFont = (numberingType == ListNumberingType.GREEK_LOWER || numberingType == ListNumberingType
                                .GREEK_UPPER) ? StandardFonts.SYMBOL : StandardFonts.ZAPFDINGBATS;
                            textRenderer = new ListRenderer.ConstantFontTextRenderer(textElement, constantFont);
                            try {
                                textRenderer.SetProperty(Property.FONT, PdfFontFactory.CreateFont(constantFont));
                            }
                            catch (System.IO.IOException) {
                            }
                        }
                        else {
                            textRenderer = new TextRenderer(textElement);
                        }
                        return SurroundTextBullet(textRenderer);
                    }
                    else {
                        if (defaultListSymbol is IListSymbolFactory) {
                            return SurroundTextBullet(((IListSymbolFactory)defaultListSymbol).CreateSymbol(index, this, renderer).CreateRendererSubTree
                                ());
                        }
                        else {
                            if (defaultListSymbol == null) {
                                return null;
                            }
                            else {
                                throw new InvalidOperationException();
                            }
                        }
                    }
                }
            }
        }

        // Wrap the bullet with a line because the direction (f.e. RTL) is processed on the LineRenderer level.
        private LineRenderer SurroundTextBullet(IRenderer bulletRenderer) {
            LineRenderer lineRenderer = new LineRenderer();
            Text zeroWidthJoiner = new Text("\u200D");
            zeroWidthJoiner.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
            TextRenderer zeroWidthJoinerRenderer = new TextRenderer(zeroWidthJoiner);
            lineRenderer.AddChild(zeroWidthJoinerRenderer);
            lineRenderer.AddChild(bulletRenderer);
            lineRenderer.AddChild(zeroWidthJoinerRenderer);
            return lineRenderer;
        }

        /// <summary>
        /// Corrects split and overflow renderers when
        /// <see cref="iText.Layout.Properties.Property.FORCED_PLACEMENT"/>
        /// is applied.
        /// </summary>
        /// <remarks>
        /// Corrects split and overflow renderers when
        /// <see cref="iText.Layout.Properties.Property.FORCED_PLACEMENT"/>
        /// is applied.
        /// <para />
        /// We assume that
        /// <see cref="iText.Layout.Properties.Property.FORCED_PLACEMENT"/>
        /// is applied when the first
        /// <see cref="ListItemRenderer"/>
        /// cannot be fully layouted.
        /// This means that the problem has occurred in one of the first list item renderer's children.
        /// In that case we force the placement of all first item renderer's children before the one,
        /// which was the cause of
        /// <see cref="iText.Layout.Layout.LayoutResult.NOTHING"/>
        /// , including this child.
        /// <para />
        /// Notice that we do not expect
        /// <see cref="iText.Layout.Properties.Property.FORCED_PLACEMENT"/>
        /// to be applied
        /// if we can render the first item renderer and strongly recommend not to set
        /// <see cref="iText.Layout.Properties.Property.FORCED_PLACEMENT"/>
        /// manually.
        /// </remarks>
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
        /// the renderer which has produced
        /// <see cref="iText.Layout.Layout.LayoutResult.NOTHING"/>
        /// </param>
        /// <param name="occupiedArea">the area occupied by layout before correction</param>
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
            // Notice that placed item is a son of the first ListItemRenderer (otherwise there would be now
            // FORCED_PLACEMENT applied)
            IRenderer firstListItemRenderer = splitRenderer.GetChildRenderers()[0];
            iText.Layout.Renderer.ListRenderer newOverflowRenderer = (iText.Layout.Renderer.ListRenderer)CreateOverflowRenderer
                (LayoutResult.PARTIAL);
            newOverflowRenderer.DeleteOwnProperty(Property.FORCED_PLACEMENT);
            // ListItemRenderer for not rendered children of firstListItemRenderer
            newOverflowRenderer.childRenderers.Add(((ListItemRenderer)firstListItemRenderer).CreateOverflowRenderer(LayoutResult
                .PARTIAL));
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
                    ()[0].GetProperty<UnitValue>(Property.MARGIN_LEFT));
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

        private LayoutResult InitializeListSymbols(LayoutContext layoutContext) {
            if (!HasOwnProperty(Property.LIST_SYMBOLS_INITIALIZED)) {
                IList<IRenderer> symbolRenderers = new List<IRenderer>();
                int listItemNum = (int)this.GetProperty<int?>(Property.LIST_START, 1);
                for (int i = 0; i < childRenderers.Count; i++) {
                    childRenderers[i].SetParent(this);
                    listItemNum = (childRenderers[i].GetProperty<int?>(Property.LIST_SYMBOL_ORDINAL_VALUE) != null) ? (int)childRenderers
                        [i].GetProperty<int?>(Property.LIST_SYMBOL_ORDINAL_VALUE) : listItemNum;
                    IRenderer currentSymbolRenderer = MakeListSymbolRenderer(listItemNum, childRenderers[i]);
                    if (currentSymbolRenderer != null && BaseDirection.RIGHT_TO_LEFT == this.GetProperty<BaseDirection?>(Property
                        .BASE_DIRECTION)) {
                        currentSymbolRenderer.SetProperty(Property.BASE_DIRECTION, BaseDirection.RIGHT_TO_LEFT);
                    }
                    LayoutResult listSymbolLayoutResult = null;
                    if (currentSymbolRenderer != null) {
                        ++listItemNum;
                        currentSymbolRenderer.SetParent(childRenderers[i]);
                        listSymbolLayoutResult = currentSymbolRenderer.Layout(layoutContext);
                        currentSymbolRenderer.SetParent(null);
                    }
                    bool isForcedPlacement = true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT));
                    bool listSymbolNotFit = listSymbolLayoutResult != null && listSymbolLayoutResult.GetStatus() != LayoutResult
                        .FULL;
                    // TODO DEVSIX-1655: partially not fitting list symbol not shown at all, however this might be improved
                    if (listSymbolNotFit && isForcedPlacement) {
                        currentSymbolRenderer = null;
                    }
                    symbolRenderers.Add(currentSymbolRenderer);
                    if (listSymbolNotFit && !isForcedPlacement) {
                        return new LayoutResult(LayoutResult.NOTHING, null, null, this, listSymbolLayoutResult.GetCauseOfNothing()
                            );
                    }
                }
                float maxSymbolWidth = 0;
                for (int i = 0; i < childRenderers.Count; i++) {
                    IRenderer symbolRenderer = symbolRenderers[i];
                    if (symbolRenderer != null) {
                        IRenderer listItemRenderer = childRenderers[i];
                        if ((ListSymbolPosition)GetListItemOrListProperty(listItemRenderer, this, Property.LIST_SYMBOL_POSITION) !=
                             ListSymbolPosition.INSIDE) {
                            maxSymbolWidth = Math.Max(maxSymbolWidth, symbolRenderer.GetOccupiedArea().GetBBox().GetWidth());
                        }
                    }
                }
                float? symbolIndent = this.GetPropertyAsFloat(Property.LIST_SYMBOL_INDENT);
                listItemNum = 0;
                foreach (IRenderer childRenderer in childRenderers) {
                    // Symbol indent's value should be summed with the margin's value
                    bool isRtl = BaseDirection.RIGHT_TO_LEFT == childRenderer.GetProperty<BaseDirection?>(Property.BASE_DIRECTION
                        );
                    int marginToSet = isRtl ? Property.MARGIN_RIGHT : Property.MARGIN_LEFT;
                    childRenderer.DeleteOwnProperty(marginToSet);
                    UnitValue marginToSetUV = childRenderer.GetProperty<UnitValue>(marginToSet, UnitValue.CreatePointValue(0f)
                        );
                    if (!marginToSetUV.IsPointValue()) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.ListRenderer));
                        logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                            , marginToSet));
                    }
                    float calculatedMargin = marginToSetUV.GetValue();
                    if ((ListSymbolPosition)GetListItemOrListProperty(childRenderer, this, Property.LIST_SYMBOL_POSITION) == ListSymbolPosition
                        .DEFAULT) {
                        calculatedMargin += maxSymbolWidth + (float)(symbolIndent != null ? symbolIndent : 0f);
                    }
                    childRenderer.SetProperty(marginToSet, UnitValue.CreatePointValue(calculatedMargin));
                    IRenderer symbolRenderer = symbolRenderers[listItemNum++];
                    ((ListItemRenderer)childRenderer).AddSymbolRenderer(symbolRenderer, maxSymbolWidth);
                    if (symbolRenderer != null) {
                        LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                        if (taggingHelper != null) {
                            if (symbolRenderer is LineRenderer) {
                                taggingHelper.SetRoleHint(symbolRenderer.GetChildRenderers()[1], StandardRoles.LBL);
                            }
                            else {
                                taggingHelper.SetRoleHint(symbolRenderer, StandardRoles.LBL);
                            }
                        }
                    }
                }
            }
            return null;
        }

        private sealed class ConstantFontTextRenderer : TextRenderer {
            private String constantFontName;

            public ConstantFontTextRenderer(Text textElement, String font)
                : base(textElement) {
                constantFontName = font;
            }

            public override void Draw(DrawContext drawContext) {
                try {
                    SetProperty(Property.FONT, PdfFontFactory.CreateFont(constantFontName));
                }
                catch (System.IO.IOException) {
                }
                // Do nothing
                base.Draw(drawContext);
            }
        }
    }
}
