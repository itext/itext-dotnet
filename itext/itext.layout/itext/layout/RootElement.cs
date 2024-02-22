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
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Splitting;
using iText.Layout.Tagging;

namespace iText.Layout {
    /// <summary>A generic abstract root element for a PDF layout object hierarchy.</summary>
    /// <typeparam name="T">this type</typeparam>
    public abstract class RootElement<T> : ElementPropertyContainer<T>, IDisposable
        where T : IPropertyContainer {
        protected internal bool immediateFlush = true;

        protected internal PdfDocument pdfDocument;

        protected internal IList<IElement> childElements = new List<IElement>();

        protected internal PdfFont defaultFont;

        protected internal FontProvider defaultFontProvider;

        protected internal ISplitCharacters defaultSplitCharacters;

        protected internal RootRenderer rootRenderer;

        private LayoutTaggingHelper defaultLayoutTaggingHelper;

        /// <summary>Adds an element to the root.</summary>
        /// <remarks>Adds an element to the root. The element is immediately placed in the contents.</remarks>
        /// <param name="element">an element with spacial margins, tabbing, and alignment</param>
        /// <returns>this element</returns>
        /// <seealso cref="iText.Layout.Element.BlockElement{T}"/>
        public virtual T Add(IBlockElement element) {
            return AddElement(element);
        }

        /// <summary>Adds an image to the root.</summary>
        /// <remarks>Adds an image to the root. The element is immediately placed in the contents.</remarks>
        /// <param name="image">a graphical image element</param>
        /// <returns>this element</returns>
        /// <seealso cref="iText.Layout.Element.Image"/>
        public virtual T Add(Image image) {
            return AddElement(image);
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Layout.Font.FontProvider"/>
        /// if presents.
        /// </summary>
        /// <returns>
        /// instance of
        /// <see cref="iText.Layout.Font.FontProvider"/>
        /// if exists, otherwise null.
        /// </returns>
        public virtual FontProvider GetFontProvider() {
            Object fontProvider = this.GetProperty<Object>(Property.FONT_PROVIDER);
            if (fontProvider is FontProvider) {
                return (FontProvider)fontProvider;
            }
            return null;
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Layout.Font.FontProvider"/>.
        /// </summary>
        /// <remarks>
        /// Sets
        /// <see cref="iText.Layout.Font.FontProvider"/>.
        /// Note, font provider is inherited property.
        /// </remarks>
        /// <param name="fontProvider">
        /// instance of
        /// <see cref="iText.Layout.Font.FontProvider"/>.
        /// </param>
        public virtual void SetFontProvider(FontProvider fontProvider) {
            SetProperty(Property.FONT_PROVIDER, fontProvider);
        }

        public override bool HasProperty(int property) {
            return HasOwnProperty(property);
        }

        public override bool HasOwnProperty(int property) {
            return properties.ContainsKey(property);
        }

        public override T1 GetProperty<T1>(int property) {
            return this.GetOwnProperty<T1>(property);
        }

        public override T1 GetOwnProperty<T1>(int property) {
            return (T1)properties.Get(property);
        }

        public override T1 GetDefaultProperty<T1>(int property) {
            try {
                switch (property) {
                    case Property.FONT: {
                        if (defaultFont == null) {
                            defaultFont = PdfFontFactory.CreateFont();
                        }
                        return (T1)(Object)defaultFont;
                    }

                    case Property.FONT_PROVIDER: {
                        if (defaultFontProvider == null) {
                            defaultFontProvider = new FontProvider();
                        }
                        return (T1)(Object)defaultFontProvider;
                    }

                    case Property.SPLIT_CHARACTERS: {
                        if (defaultSplitCharacters == null) {
                            defaultSplitCharacters = new DefaultSplitCharacters();
                        }
                        return (T1)(Object)defaultSplitCharacters;
                    }

                    case Property.FONT_SIZE: {
                        return (T1)(Object)UnitValue.CreatePointValue(12);
                    }

                    case Property.TAGGING_HELPER: {
                        return (T1)(Object)InitTaggingHelperIfNeeded();
                    }

                    case Property.TEXT_RENDERING_MODE: {
                        return (T1)(Object)PdfCanvasConstants.TextRenderingMode.FILL;
                    }

                    case Property.TEXT_RISE: {
                        return (T1)(Object)0f;
                    }

                    case Property.SPACING_RATIO: {
                        return (T1)(Object)0.75f;
                    }

                    default: {
                        return (T1)(Object)null;
                    }
                }
            }
            catch (System.IO.IOException exc) {
                throw new Exception(exc.ToString(), exc);
            }
        }

        public override void DeleteOwnProperty(int property) {
            properties.JRemove(property);
        }

        public override void SetProperty(int property, Object value) {
            properties.Put(property, value);
        }

        /// <summary>
        /// Gets the rootRenderer attribute, a specialized
        /// <see cref="iText.Layout.Renderer.IRenderer"/>
        /// that
        /// acts as the root object that other
        /// <see cref="iText.Layout.Renderer.IRenderer">renderers</see>
        /// descend
        /// from.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Layout.Renderer.RootRenderer"/>
        /// attribute
        /// </returns>
        public virtual RootRenderer GetRenderer() {
            return EnsureRootRendererNotNull();
        }

        /// <summary>Convenience method to write a text aligned about the specified point</summary>
        /// <param name="text">text to be placed to the page</param>
        /// <param name="x">the point about which the text will be aligned and rotated</param>
        /// <param name="y">the point about which the text will be aligned and rotated</param>
        /// <param name="textAlign">horizontal alignment about the specified point</param>
        /// <returns>this object</returns>
        public virtual T ShowTextAligned(String text, float x, float y, TextAlignment? textAlign) {
            return ShowTextAligned(text, x, y, textAlign, 0);
        }

        /// <summary>Convenience method to write a text aligned about the specified point</summary>
        /// <param name="text">text to be placed to the page</param>
        /// <param name="x">the point about which the text will be aligned and rotated</param>
        /// <param name="y">the point about which the text will be aligned and rotated</param>
        /// <param name="textAlign">horizontal alignment about the specified point</param>
        /// <param name="angle">the angle of rotation applied to the text, in radians</param>
        /// <returns>this object</returns>
        public virtual T ShowTextAligned(String text, float x, float y, TextAlignment? textAlign, float angle) {
            return ShowTextAligned(text, x, y, textAlign, VerticalAlignment.BOTTOM, angle);
        }

        /// <summary>Convenience method to write a text aligned about the specified point</summary>
        /// <param name="text">text to be placed to the page</param>
        /// <param name="x">the point about which the text will be aligned and rotated</param>
        /// <param name="y">the point about which the text will be aligned and rotated</param>
        /// <param name="textAlign">horizontal alignment about the specified point</param>
        /// <param name="vertAlign">vertical alignment about the specified point</param>
        /// <param name="angle">the angle of rotation applied to the text, in radians</param>
        /// <returns>this object</returns>
        public virtual T ShowTextAligned(String text, float x, float y, TextAlignment? textAlign, VerticalAlignment?
             vertAlign, float angle) {
            Paragraph p = new Paragraph(text).SetMultipliedLeading(1).SetMargin(0);
            return ShowTextAligned(p, x, y, pdfDocument.GetNumberOfPages(), textAlign, vertAlign, angle);
        }

        /// <summary>Convenience method to write a kerned text aligned about the specified point</summary>
        /// <param name="text">text to be placed to the page</param>
        /// <param name="x">the point about which the text will be aligned and rotated</param>
        /// <param name="y">the point about which the text will be aligned and rotated</param>
        /// <param name="textAlign">horizontal alignment about the specified point</param>
        /// <param name="vertAlign">vertical alignment about the specified point</param>
        /// <param name="radAngle">the angle of rotation applied to the text, in radians</param>
        /// <returns>this object</returns>
        public virtual T ShowTextAlignedKerned(String text, float x, float y, TextAlignment? textAlign, VerticalAlignment?
             vertAlign, float radAngle) {
            Paragraph p = new Paragraph(text).SetMultipliedLeading(1).SetMargin(0).SetFontKerning(FontKerning.YES);
            return ShowTextAligned(p, x, y, pdfDocument.GetNumberOfPages(), textAlign, vertAlign, radAngle);
        }

        /// <summary>Convenience method to write a text aligned about the specified point</summary>
        /// <param name="p">
        /// paragraph of text to be placed to the page. By default it has no leading and is written in single line.
        /// Set width to write multiline text.
        /// </param>
        /// <param name="x">the point about which the text will be aligned and rotated</param>
        /// <param name="y">the point about which the text will be aligned and rotated</param>
        /// <param name="textAlign">horizontal alignment about the specified point</param>
        /// <returns>this object</returns>
        public virtual T ShowTextAligned(Paragraph p, float x, float y, TextAlignment? textAlign) {
            return ShowTextAligned(p, x, y, pdfDocument.GetNumberOfPages(), textAlign, VerticalAlignment.BOTTOM, 0);
        }

        /// <summary>Convenience method to write a text aligned about the specified point</summary>
        /// <param name="p">
        /// paragraph of text to be placed to the page. By default it has no leading and is written in single line.
        /// Set width to write multiline text.
        /// </param>
        /// <param name="x">the point about which the text will be aligned and rotated</param>
        /// <param name="y">the point about which the text will be aligned and rotated</param>
        /// <param name="textAlign">horizontal alignment about the specified point</param>
        /// <param name="vertAlign">vertical alignment about the specified point</param>
        /// <returns>this object</returns>
        public virtual T ShowTextAligned(Paragraph p, float x, float y, TextAlignment? textAlign, VerticalAlignment?
             vertAlign) {
            return ShowTextAligned(p, x, y, pdfDocument.GetNumberOfPages(), textAlign, vertAlign, 0);
        }

        /// <summary>Convenience method to write a text aligned about the specified point</summary>
        /// <param name="p">
        /// paragraph of text to be placed to the page. By default it has no leading and is written in single line.
        /// Set width to write multiline text.
        /// </param>
        /// <param name="x">the point about which the text will be aligned and rotated</param>
        /// <param name="y">the point about which the text will be aligned and rotated</param>
        /// <param name="pageNumber">the page number to write the text</param>
        /// <param name="textAlign">horizontal alignment about the specified point</param>
        /// <param name="vertAlign">vertical alignment about the specified point</param>
        /// <param name="radAngle">the angle of rotation applied to the text, in radians</param>
        /// <returns>this object</returns>
        public virtual T ShowTextAligned(Paragraph p, float x, float y, int pageNumber, TextAlignment? textAlign, 
            VerticalAlignment? vertAlign, float radAngle) {
            Div div = new Div();
            div.SetTextAlignment(textAlign).SetVerticalAlignment(vertAlign);
            if (radAngle != 0) {
                div.SetRotationAngle(radAngle);
            }
            div.SetProperty(Property.ROTATION_POINT_X, x);
            div.SetProperty(Property.ROTATION_POINT_Y, y);
            float divSize = 5e3f;
            float divX = x;
            float divY = y;
            if (textAlign == TextAlignment.CENTER) {
                divX = x - divSize / 2;
                p.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            }
            else {
                if (textAlign == TextAlignment.RIGHT) {
                    divX = x - divSize;
                    p.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                }
            }
            if (vertAlign == VerticalAlignment.MIDDLE) {
                divY = y - divSize / 2;
            }
            else {
                if (vertAlign == VerticalAlignment.TOP) {
                    divY = y - divSize;
                }
            }
            if (pageNumber == 0) {
                pageNumber = 1;
            }
            div.SetFixedPosition(pageNumber, divX, divY, divSize).SetMinHeight(divSize);
            if (p.GetProperty<Leading>(Property.LEADING) == null) {
                p.SetMultipliedLeading(1);
            }
            div.Add(p.SetMargins(0, 0, 0, 0));
            div.GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
            this.Add(div);
            return (T)(Object)this;
        }

        protected internal abstract RootRenderer EnsureRootRendererNotNull();

        protected internal virtual void CreateAndAddRendererSubTree(IElement element) {
            IRenderer rendererSubTreeRoot = element.CreateRendererSubTree();
            LayoutTaggingHelper taggingHelper = InitTaggingHelperIfNeeded();
            if (taggingHelper != null) {
                taggingHelper.AddKidsHint(pdfDocument.GetTagStructureContext().GetAutoTaggingPointer(), JavaCollectionsUtil
                    .SingletonList<IRenderer>(rendererSubTreeRoot));
            }
            EnsureRootRendererNotNull().AddChild(rendererSubTreeRoot);
            TraverseAndCallIso(pdfDocument, rendererSubTreeRoot);
        }

        private LayoutTaggingHelper InitTaggingHelperIfNeeded() {
            return defaultLayoutTaggingHelper == null && pdfDocument.IsTagged() ? defaultLayoutTaggingHelper = new LayoutTaggingHelper
                (pdfDocument, immediateFlush) : defaultLayoutTaggingHelper;
        }

        private T AddElement(IElement element) {
            childElements.Add(element);
            CreateAndAddRendererSubTree(element);
            if (immediateFlush) {
                childElements.JRemoveAt(childElements.Count - 1);
            }
            return (T)(Object)this;
        }

        private static void TraverseAndCallIso(PdfDocument pdfDocument, IRenderer renderer) {
            if (renderer == null) {
                return;
            }
            pdfDocument.CheckIsoConformance(renderer, IsoKey.LAYOUT);
            IList<IRenderer> renderers = renderer.GetChildRenderers();
            if (renderers == null) {
                return;
            }
            foreach (IRenderer childRenderer in renderers) {
                TraverseAndCallIso(pdfDocument, childRenderer);
            }
        }

        public abstract void Close();

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
