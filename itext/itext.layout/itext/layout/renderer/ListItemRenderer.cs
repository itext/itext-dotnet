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
using iText.IO.Log;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

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
                SetProperty(Property.MIN_HEIGHT, minHeight);
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
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.ListItemRenderer));
                logger.Error(iText.IO.LogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED);
                return;
            }
            bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
            TagTreePointer tagPointer = null;
            if (isTagged) {
                tagPointer = drawContext.GetDocument().GetTagStructureContext().GetAutoTaggingPointer();
                IAccessibleElement modelElement = (IAccessibleElement)GetModelElement();
                PdfName role = modelElement.GetRole();
                if (role != null && !PdfName.Artifact.Equals(role)) {
                    bool lBodyTagIsCreated = tagPointer.IsElementConnectedToTag(modelElement);
                    if (!lBodyTagIsCreated) {
                        tagPointer.AddTag(PdfName.LI);
                    }
                    else {
                        tagPointer.MoveToTag(modelElement).MoveToParent();
                    }
                }
                else {
                    isTagged = false;
                }
            }
            base.Draw(drawContext);
            // It will be null in case of overflow (only the "split" part will contain symbol renderer.
            if (symbolRenderer != null && !symbolAddedInside) {
                symbolRenderer.SetParent(parent);
                float x = occupiedArea.GetBBox().GetX();
                ListSymbolPosition symbolPosition = (ListSymbolPosition)this.GetProperty<Object>(Property.LIST_SYMBOL_POSITION
                    );
                if (symbolPosition != ListSymbolPosition.DEFAULT) {
                    float? symbolIndent = this.GetPropertyAsFloat(Property.LIST_SYMBOL_INDENT);
                    x -= symbolAreaWidth + (float)(symbolIndent == null ? 0 : symbolIndent);
                    if (symbolPosition == ListSymbolPosition.OUTSIDE) {
                        x += (float)this.GetPropertyAsFloat(Property.MARGIN_LEFT);
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
                        if (symbolRenderer is TextRenderer) {
                            ((TextRenderer)symbolRenderer).MoveYLineTo((float)yLine);
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
                    .LIST_SYMBOL_ALIGNMENT, ListSymbolAlignment.RIGHT);
                float xPosition = x - symbolRenderer.GetOccupiedArea().GetBBox().GetX();
                if (listSymbolAlignment == ListSymbolAlignment.RIGHT) {
                    xPosition += symbolAreaWidth - symbolRenderer.GetOccupiedArea().GetBBox().GetWidth();
                }
                symbolRenderer.Move(xPosition, 0);
                if (symbolRenderer.GetOccupiedArea().GetBBox().GetRight() > parent.GetOccupiedArea().GetBBox().GetLeft()) {
                    if (isTagged) {
                        tagPointer.AddTag(0, PdfName.Lbl);
                    }
                    BeginElementOpacityApplying(drawContext);
                    symbolRenderer.Draw(drawContext);
                    EndElementOpacityApplying(drawContext);
                    if (isTagged) {
                        tagPointer.MoveToParent();
                    }
                }
            }
            if (isTagged) {
                tagPointer.MoveToParent();
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
            if (layoutResult == LayoutResult.PARTIAL) {
                splitRenderer.symbolRenderer = symbolRenderer;
                splitRenderer.symbolAreaWidth = symbolAreaWidth;
            }
            // TODO retain all the properties ?
            splitRenderer.SetProperty(Property.MARGIN_LEFT, this.GetProperty<Object>(Property.MARGIN_LEFT));
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
            // TODO retain all the properties ?
            overflowRenderer.SetProperty(Property.MARGIN_LEFT, this.GetProperty<Object>(Property.MARGIN_LEFT));
            return overflowRenderer;
        }

        private void ApplyListSymbolPosition() {
            if (symbolRenderer != null) {
                ListSymbolPosition symbolPosition = (ListSymbolPosition)this.GetProperty<Object>(Property.LIST_SYMBOL_POSITION
                    );
                if (symbolPosition == ListSymbolPosition.INSIDE) {
                    if (childRenderers.Count > 0 && childRenderers[0] is ParagraphRenderer) {
                        ParagraphRenderer paragraphRenderer = (ParagraphRenderer)childRenderers[0];
                        float? symbolIndent = this.GetPropertyAsFloat(Property.LIST_SYMBOL_INDENT);
                        if (symbolIndent != null) {
                            symbolRenderer.SetProperty(Property.MARGIN_RIGHT, symbolIndent);
                        }
                        paragraphRenderer.childRenderers.Add(0, symbolRenderer);
                        symbolAddedInside = true;
                    }
                    else {
                        if (childRenderers.Count > 0 && childRenderers[0] is ImageRenderer) {
                            IRenderer paragraphRenderer = new Paragraph().SetMargin(0).CreateRendererSubTree();
                            float? symbolIndent = this.GetPropertyAsFloat(Property.LIST_SYMBOL_INDENT);
                            if (symbolIndent != null) {
                                symbolRenderer.SetProperty(Property.MARGIN_RIGHT, symbolIndent);
                            }
                            paragraphRenderer.AddChild(symbolRenderer);
                            paragraphRenderer.AddChild(childRenderers[0]);
                            childRenderers[0] = paragraphRenderer;
                            symbolAddedInside = true;
                        }
                    }
                    if (!symbolAddedInside) {
                        IRenderer paragraphRenderer = new Paragraph().SetMargin(0).CreateRendererSubTree();
                        float? symbolIndent = this.GetPropertyAsFloat(Property.LIST_SYMBOL_INDENT);
                        if (symbolIndent != null) {
                            symbolRenderer.SetProperty(Property.MARGIN_RIGHT, symbolIndent);
                        }
                        paragraphRenderer.AddChild(symbolRenderer);
                        childRenderers.Add(0, paragraphRenderer);
                        symbolAddedInside = true;
                    }
                }
            }
        }

        private bool IsListSymbolEmpty(IRenderer listSymbolRenderer) {
            return listSymbolRenderer is TextRenderer && ((TextRenderer)listSymbolRenderer).GetText().ToString().Length
                 == 0;
        }

        private float[] CalculateAscenderDescender() {
            PdfFont listItemFont = ResolveFirstPdfFont();
            float? fontSize = this.GetPropertyAsFloat(Property.FONT_SIZE);
            if (listItemFont != null && fontSize != null) {
                float[] ascenderDescender = TextRenderer.CalculateAscenderDescender(listItemFont);
                return new float[] { (float)fontSize * ascenderDescender[0] / TextRenderer.TEXT_SPACE_COEFF, (float)fontSize
                     * ascenderDescender[1] / TextRenderer.TEXT_SPACE_COEFF };
            }
            return new float[] { 0, 0 };
        }
    }
}
