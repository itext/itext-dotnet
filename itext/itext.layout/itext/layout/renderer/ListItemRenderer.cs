/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Tagging;

namespace iText.Layout.Renderer {
    public class ListItemRenderer : DivRenderer {
        protected internal IRenderer symbolRenderer;

        protected internal float symbolAreaWidth;

        private bool symbolAddedInside;

        /// <summary>Creates a ListItemRenderer from its corresponding layout object.</summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.ListItem"/>
        /// which this object should manage
        /// </param>
        public ListItemRenderer(ListItem modelElement)
            : base(modelElement) {
        }

        public virtual void AddSymbolRenderer(IRenderer symbolRenderer, float symbolAreaWidth) {
            this.symbolRenderer = symbolRenderer;
            this.symbolAreaWidth = symbolAreaWidth;
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            if (symbolRenderer != null && this.GetProperty<Object>(Property.HEIGHT) == null && !IsListSymbolEmpty(symbolRenderer
                )) {
                float[] ascenderDescender = CalculateAscenderDescender();
                float minHeight = Math.Max(symbolRenderer.GetOccupiedArea().GetBBox().GetHeight(), ascenderDescender[0] - 
                    ascenderDescender[1]);
                UpdateMinHeight(UnitValue.CreatePointValue(minHeight));
            }
            ApplyListSymbolPosition();
            LayoutResult result = base.Layout(layoutContext);
            if (LayoutResult.PARTIAL == result.GetStatus()) {
                result.GetOverflowRenderer().DeleteOwnProperty(Property.MIN_HEIGHT);
            }
            return result;
        }

        public override void Draw(DrawContext drawContext) {
            if (occupiedArea == null) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.ListItemRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                    , "Drawing won't be performed."));
                return;
            }
            if (drawContext.IsTaggingEnabled()) {
                LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                if (taggingHelper != null) {
                    if (symbolRenderer != null) {
                        LayoutTaggingHelper.AddTreeHints(taggingHelper, symbolRenderer);
                    }
                    if (taggingHelper.IsArtifact(this)) {
                        taggingHelper.MarkArtifactHint(symbolRenderer);
                    }
                    else {
                        TaggingHintKey hintKey = LayoutTaggingHelper.GetHintKey(this);
                        TaggingHintKey parentHint = taggingHelper.GetAccessibleParentHint(hintKey);
                        if (parentHint != null && !(StandardRoles.LI.Equals(parentHint.GetAccessibleElement().GetAccessibilityProperties
                            ().GetRole()))) {
                            TaggingDummyElement listItemIntermediate = new TaggingDummyElement(StandardRoles.LI);
                            IList<TaggingHintKey> intermediateKid = JavaCollectionsUtil.SingletonList<TaggingHintKey>(LayoutTaggingHelper
                                .GetOrCreateHintKey(listItemIntermediate));
                            taggingHelper.ReplaceKidHint(hintKey, intermediateKid);
                            if (symbolRenderer != null) {
                                taggingHelper.AddKidsHint(listItemIntermediate, JavaCollectionsUtil.SingletonList<IRenderer>(symbolRenderer
                                    ));
                            }
                            taggingHelper.AddKidsHint(listItemIntermediate, JavaCollectionsUtil.SingletonList<IRenderer>(this));
                        }
                    }
                }
            }
            base.Draw(drawContext);
            // It will be null in case of overflow (only the "split" part will contain symbol renderer.
            if (symbolRenderer != null && !symbolAddedInside) {
                bool isRtl = BaseDirection.RIGHT_TO_LEFT == this.GetProperty<BaseDirection?>(Property.BASE_DIRECTION);
                symbolRenderer.SetParent(this);
                float x = isRtl ? occupiedArea.GetBBox().GetRight() : occupiedArea.GetBBox().GetLeft();
                ListSymbolPosition symbolPosition = (ListSymbolPosition)ListRenderer.GetListItemOrListProperty(this, parent
                    , Property.LIST_SYMBOL_POSITION);
                if (symbolPosition != ListSymbolPosition.DEFAULT) {
                    float? symbolIndent = this.GetPropertyAsFloat(Property.LIST_SYMBOL_INDENT);
                    if (isRtl) {
                        x += (symbolAreaWidth + (float)(symbolIndent == null ? 0 : symbolIndent));
                    }
                    else {
                        x -= (symbolAreaWidth + (float)(symbolIndent == null ? 0 : symbolIndent));
                    }
                    if (symbolPosition == ListSymbolPosition.OUTSIDE) {
                        if (isRtl) {
                            UnitValue marginRightUV = this.GetPropertyAsUnitValue(Property.MARGIN_RIGHT);
                            if (!marginRightUV.IsPointValue()) {
                                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.ListItemRenderer));
                                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                                    , Property.MARGIN_RIGHT));
                            }
                            x -= marginRightUV.GetValue();
                        }
                        else {
                            UnitValue marginLeftUV = this.GetPropertyAsUnitValue(Property.MARGIN_LEFT);
                            if (!marginLeftUV.IsPointValue()) {
                                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.ListItemRenderer));
                                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                                    , Property.MARGIN_LEFT));
                            }
                            x += marginLeftUV.GetValue();
                        }
                    }
                }
                ApplyMargins(occupiedArea.GetBBox(), false);
                ApplyBorderBox(occupiedArea.GetBBox(), false);
                if (childRenderers.Count > 0) {
                    float? yLine = null;
                    for (int i = 0; i < childRenderers.Count; i++) {
                        if (childRenderers[i].GetOccupiedArea().GetBBox().GetHeight() > 0) {
                            yLine = ((AbstractRenderer)childRenderers[i]).GetFirstYLineRecursively();
                            if (yLine != null) {
                                break;
                            }
                        }
                    }
                    if (yLine != null) {
                        if (symbolRenderer is LineRenderer) {
                            symbolRenderer.Move(0, (float)yLine - ((LineRenderer)symbolRenderer).GetYLine());
                        }
                        else {
                            symbolRenderer.Move(0, (float)yLine - symbolRenderer.GetOccupiedArea().GetBBox().GetY());
                        }
                    }
                    else {
                        symbolRenderer.Move(0, occupiedArea.GetBBox().GetY() + occupiedArea.GetBBox().GetHeight() - (symbolRenderer
                            .GetOccupiedArea().GetBBox().GetY() + symbolRenderer.GetOccupiedArea().GetBBox().GetHeight()));
                    }
                }
                else {
                    if (symbolRenderer is TextRenderer) {
                        ((TextRenderer)symbolRenderer).MoveYLineTo(occupiedArea.GetBBox().GetY() + occupiedArea.GetBBox().GetHeight
                            () - CalculateAscenderDescender()[0]);
                    }
                    else {
                        symbolRenderer.Move(0, occupiedArea.GetBBox().GetY() + occupiedArea.GetBBox().GetHeight() - symbolRenderer
                            .GetOccupiedArea().GetBBox().GetHeight() - symbolRenderer.GetOccupiedArea().GetBBox().GetY());
                    }
                }
                ApplyBorderBox(occupiedArea.GetBBox(), true);
                ApplyMargins(occupiedArea.GetBBox(), true);
                ListSymbolAlignment listSymbolAlignment = (ListSymbolAlignment)parent.GetProperty<ListSymbolAlignment?>(Property
                    .LIST_SYMBOL_ALIGNMENT, isRtl ? ListSymbolAlignment.LEFT : ListSymbolAlignment.RIGHT);
                float dxPosition = x - symbolRenderer.GetOccupiedArea().GetBBox().GetX();
                if (listSymbolAlignment == ListSymbolAlignment.RIGHT) {
                    if (!isRtl) {
                        dxPosition += symbolAreaWidth - symbolRenderer.GetOccupiedArea().GetBBox().GetWidth();
                    }
                }
                else {
                    if (listSymbolAlignment == ListSymbolAlignment.LEFT) {
                        if (isRtl) {
                            dxPosition -= (symbolAreaWidth - symbolRenderer.GetOccupiedArea().GetBBox().GetWidth());
                        }
                    }
                }
                if (symbolRenderer is LineRenderer) {
                    if (isRtl) {
                        symbolRenderer.Move(dxPosition - symbolRenderer.GetOccupiedArea().GetBBox().GetWidth(), 0);
                    }
                    else {
                        symbolRenderer.Move(dxPosition, 0);
                    }
                }
                else {
                    symbolRenderer.Move(dxPosition, 0);
                }
                // consider page area without margins
                RootRenderer root = GetRootRenderer();
                Rectangle effectiveArea = root.GetCurrentArea().GetBBox();
                // symbols are not drawn here, because they are in page margins
                if (!isRtl && symbolRenderer.GetOccupiedArea().GetBBox().GetRight() > effectiveArea.GetLeft() || isRtl && 
                    symbolRenderer.GetOccupiedArea().GetBBox().GetLeft() < effectiveArea.GetRight()) {
                    BeginElementOpacityApplying(drawContext);
                    symbolRenderer.Draw(drawContext);
                    EndElementOpacityApplying(drawContext);
                }
            }
        }

        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.ListItemRenderer((ListItem)modelElement);
        }

        protected internal override AbstractRenderer CreateSplitRenderer(int layoutResult) {
            iText.Layout.Renderer.ListItemRenderer splitRenderer = (iText.Layout.Renderer.ListItemRenderer)GetNextRenderer
                ();
            splitRenderer.parent = parent;
            splitRenderer.modelElement = modelElement;
            splitRenderer.occupiedArea = occupiedArea;
            splitRenderer.symbolAddedInside = symbolAddedInside;
            splitRenderer.isLastRendererForModelElement = false;
            if (layoutResult == LayoutResult.PARTIAL) {
                splitRenderer.symbolRenderer = symbolRenderer;
                splitRenderer.symbolAreaWidth = symbolAreaWidth;
            }
            splitRenderer.AddAllProperties(GetOwnProperties());
            return splitRenderer;
        }

        protected internal override AbstractRenderer CreateOverflowRenderer(int layoutResult) {
            iText.Layout.Renderer.ListItemRenderer overflowRenderer = (iText.Layout.Renderer.ListItemRenderer)GetNextRenderer
                ();
            overflowRenderer.parent = parent;
            overflowRenderer.modelElement = modelElement;
            if (layoutResult == LayoutResult.NOTHING) {
                overflowRenderer.symbolRenderer = symbolRenderer;
                overflowRenderer.symbolAreaWidth = symbolAreaWidth;
            }
            overflowRenderer.AddAllProperties(GetOwnProperties());
            return overflowRenderer;
        }

        private void ApplyListSymbolPosition() {
            if (symbolRenderer != null) {
                ListSymbolPosition symbolPosition = (ListSymbolPosition)ListRenderer.GetListItemOrListProperty(this, parent
                    , Property.LIST_SYMBOL_POSITION);
                if (symbolPosition == ListSymbolPosition.INSIDE) {
                    bool isRtl = BaseDirection.RIGHT_TO_LEFT.Equals(this.GetProperty<BaseDirection?>(Property.BASE_DIRECTION));
                    if (childRenderers.Count > 0 && childRenderers[0] is ParagraphRenderer) {
                        ParagraphRenderer paragraphRenderer = (ParagraphRenderer)childRenderers[0];
                        // TODO DEVSIX-6876 LIST_SYMBOL_INDENT is not inherited
                        float? symbolIndent = this.GetPropertyAsFloat(Property.LIST_SYMBOL_INDENT);
                        if (symbolRenderer is LineRenderer) {
                            if (symbolIndent != null) {
                                symbolRenderer.GetChildRenderers()[1].SetProperty(isRtl ? Property.MARGIN_LEFT : Property.MARGIN_RIGHT, UnitValue
                                    .CreatePointValue((float)symbolIndent));
                            }
                            foreach (IRenderer childRenderer in symbolRenderer.GetChildRenderers()) {
                                paragraphRenderer.childRenderers.Add(0, childRenderer);
                            }
                        }
                        else {
                            if (symbolIndent != null) {
                                symbolRenderer.SetProperty(isRtl ? Property.MARGIN_LEFT : Property.MARGIN_RIGHT, UnitValue.CreatePointValue
                                    ((float)symbolIndent));
                            }
                            paragraphRenderer.childRenderers.Add(0, symbolRenderer);
                        }
                        symbolAddedInside = true;
                    }
                    else {
                        if (childRenderers.Count > 0 && childRenderers[0] is ImageRenderer) {
                            IRenderer paragraphRenderer = RenderSymbolInNeutralParagraph();
                            paragraphRenderer.AddChild(childRenderers[0]);
                            childRenderers[0] = paragraphRenderer;
                            symbolAddedInside = true;
                        }
                    }
                    if (!symbolAddedInside) {
                        IRenderer paragraphRenderer = RenderSymbolInNeutralParagraph();
                        childRenderers.Add(0, paragraphRenderer);
                        symbolAddedInside = true;
                    }
                }
            }
        }

        private IRenderer RenderSymbolInNeutralParagraph() {
            Paragraph p = new Paragraph().SetNeutralRole();
            IRenderer paragraphRenderer = p.SetMargin(0).CreateRendererSubTree();
            float? symbolIndent = (float?)ListRenderer.GetListItemOrListProperty(this, parent, Property.LIST_SYMBOL_INDENT
                );
            if (symbolIndent != null) {
                // cast to float is necessary for autoporting reasons
                symbolRenderer.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePointValue((float)symbolIndent));
            }
            paragraphRenderer.AddChild(symbolRenderer);
            return paragraphRenderer;
        }

        private bool IsListSymbolEmpty(IRenderer listSymbolRenderer) {
            if (listSymbolRenderer is TextRenderer) {
                return ((TextRenderer)listSymbolRenderer).GetText().ToString().Length == 0;
            }
            else {
                if (listSymbolRenderer is LineRenderer) {
                    return ((TextRenderer)listSymbolRenderer.GetChildRenderers()[1]).GetText().ToString().Length == 0;
                }
            }
            return false;
        }

        private float[] CalculateAscenderDescender() {
            PdfFont listItemFont = ResolveFirstPdfFont();
            UnitValue fontSize = this.GetPropertyAsUnitValue(Property.FONT_SIZE);
            if (listItemFont != null && fontSize != null) {
                if (!fontSize.IsPointValue()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.ListItemRenderer));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                        , Property.FONT_SIZE));
                }
                float[] ascenderDescender = TextRenderer.CalculateAscenderDescender(listItemFont);
                return new float[] { fontSize.GetValue() * FontProgram.ConvertTextSpaceToGlyphSpace(ascenderDescender[0]), 
                    fontSize.GetValue() * FontProgram.ConvertTextSpaceToGlyphSpace(ascenderDescender[1]) };
            }
            return new float[] { 0, 0 };
        }
    }
}
