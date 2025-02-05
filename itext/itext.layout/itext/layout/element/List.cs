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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A List is a layout element representing a series of objects that are vertically
    /// outlined with the same or very similar layout properties, giving it a sense
    /// of unity.
    /// </summary>
    /// <remarks>
    /// A List is a layout element representing a series of objects that are vertically
    /// outlined with the same or very similar layout properties, giving it a sense
    /// of unity. It contains
    /// <see cref="ListItem"/>
    /// objects that can optionally be prefixed
    /// with a symbol and/or numbered.
    /// </remarks>
    public class List : BlockElement<iText.Layout.Element.List> {
        public const String DEFAULT_LIST_SYMBOL = "- ";

        protected internal DefaultAccessibilityProperties tagProperties;

        /// <summary>
        /// Creates a List with the
        /// <see cref="DEFAULT_LIST_SYMBOL"/>
        /// as a prefix.
        /// </summary>
        public List()
            : base() {
        }

        /// <summary>Creates a List with a custom numbering type.</summary>
        /// <param name="listNumberingType">a prefix style</param>
        public List(ListNumberingType listNumberingType)
            : base() {
            SetListSymbol(listNumberingType);
        }

        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case Property.LIST_SYMBOL: {
                    return (T1)(Object)new Text(DEFAULT_LIST_SYMBOL);
                }

                case Property.LIST_SYMBOL_PRE_TEXT: {
                    return (T1)(Object)"";
                }

                case Property.LIST_SYMBOL_POST_TEXT: {
                    return (T1)(Object)". ";
                }

                case Property.LIST_SYMBOL_POSITION: {
                    return (T1)(Object)ListSymbolPosition.DEFAULT;
                }

                default: {
                    return base.GetDefaultProperty<T1>(property);
                }
            }
        }

        /// <summary>
        /// Adds a new
        /// <see cref="ListItem"/>
        /// to the bottom of the List.
        /// </summary>
        /// <param name="listItem">a new list item</param>
        /// <returns>this list.</returns>
        public virtual iText.Layout.Element.List Add(ListItem listItem) {
            childElements.Add(listItem);
            return this;
        }

        /// <summary>
        /// Adds a new
        /// <see cref="ListItem"/>
        /// to the bottom of the List.
        /// </summary>
        /// <param name="text">textual contents of the new list item</param>
        /// <returns>this list.</returns>
        public virtual iText.Layout.Element.List Add(String text) {
            return Add(new ListItem(text));
        }

        /// <summary>Customizes the index of the first item in the list.</summary>
        /// <param name="start">the custom index, as an <c>int</c></param>
        /// <returns>this list.</returns>
        public virtual iText.Layout.Element.List SetItemStartIndex(int start) {
            SetProperty(Property.LIST_START, start);
            return this;
        }

        /// <summary>Sets the list symbol to be used.</summary>
        /// <remarks>
        /// Sets the list symbol to be used. This will create an unordered list, i.e.
        /// all
        /// <see cref="ListItem">list items</see>
        /// will be shown with the same prefix.
        /// </remarks>
        /// <param name="symbol">the textual symbol to be used for all items.</param>
        /// <returns>this list.</returns>
        public virtual iText.Layout.Element.List SetListSymbol(String symbol) {
            return SetListSymbol(new Text(symbol));
        }

        /// <summary>Sets the list symbol to be used.</summary>
        /// <remarks>
        /// Sets the list symbol to be used. This will create an unordered list, i.e.
        /// all
        /// <see cref="ListItem">list items</see>
        /// will be shown with the same prefix.
        /// </remarks>
        /// <param name="text">
        /// the
        /// <see cref="Text"/>
        /// object to be used for all items.
        /// </param>
        /// <returns>this list.</returns>
        public virtual iText.Layout.Element.List SetListSymbol(Text text) {
            SetProperty(Property.LIST_SYMBOL, text);
            return this;
        }

        /// <summary>Sets the list symbol to be used.</summary>
        /// <remarks>
        /// Sets the list symbol to be used. This will create an unordered list, i.e.
        /// all
        /// <see cref="ListItem">list items</see>
        /// will be shown with the same prefix.
        /// </remarks>
        /// <param name="image">
        /// the
        /// <see cref="Image"/>
        /// object to be used for all items.
        /// </param>
        /// <returns>this list.</returns>
        public virtual iText.Layout.Element.List SetListSymbol(Image image) {
            SetProperty(Property.LIST_SYMBOL, image);
            return this;
        }

        /// <summary>Sets the list numbering type to be used.</summary>
        /// <remarks>
        /// Sets the list numbering type to be used. This will create an ordered list,
        /// i.e. every
        /// <see cref="ListItem"/>
        /// will have a unique prefix.
        /// </remarks>
        /// <param name="listNumberingType">
        /// the
        /// <see cref="iText.Layout.Properties.ListNumberingType"/>
        /// that will generate appropriate prefixes for the
        /// <see cref="ListItem"/>
        /// s.
        /// </param>
        /// <returns>this list.</returns>
        public virtual iText.Layout.Element.List SetListSymbol(ListNumberingType listNumberingType) {
            // Do not draw any points after ZapfDingbats special number symbol
            if (listNumberingType == ListNumberingType.ZAPF_DINGBATS_1 || listNumberingType == ListNumberingType.ZAPF_DINGBATS_2
                 || listNumberingType == ListNumberingType.ZAPF_DINGBATS_3 || listNumberingType == ListNumberingType.ZAPF_DINGBATS_4
                ) {
                SetPostSymbolText(" ");
            }
            SetProperty(Property.LIST_SYMBOL, listNumberingType);
            return this;
        }

        /// <summary>A specialized enum containing alignment properties for list symbols.</summary>
        /// <remarks>
        /// A specialized enum containing alignment properties for list symbols.
        /// <see cref="iText.Layout.Properties.ListSymbolAlignment.LEFT"/>
        /// means that the items will be aligned as follows:
        /// 9.  Item 9
        /// 10. Item 10
        /// <para />
        /// Whereas
        /// <see cref="iText.Layout.Properties.ListSymbolAlignment.RIGHT"/>
        /// means the items will be aligned as follows:
        /// 9. Item 9
        /// 10. Item 10
        /// </remarks>
        /// <param name="alignment">the alignment of the list symbols</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Element.List SetListSymbolAlignment(ListSymbolAlignment alignment) {
            SetProperty(Property.LIST_SYMBOL_ALIGNMENT, alignment);
            return this;
        }

        /// <summary>
        /// Gets the indent offset of the
        /// <see cref="ListItem"/>
        /// symbols.
        /// </summary>
        /// <returns>the indent offset as a <c>float</c>.</returns>
        public virtual float? GetSymbolIndent() {
            return this.GetProperty<float?>(Property.LIST_SYMBOL_INDENT);
        }

        /// <summary>
        /// Sets the indent offset of the
        /// <see cref="ListItem"/>
        /// symbols.
        /// </summary>
        /// <param name="symbolIndent">the new indent offset.</param>
        /// <returns>this list.</returns>
        public virtual iText.Layout.Element.List SetSymbolIndent(float symbolIndent) {
            SetProperty(Property.LIST_SYMBOL_INDENT, symbolIndent);
            return this;
        }

        /// <summary>
        /// Gets the piece of text that is added after the
        /// <see cref="ListItem"/>
        /// symbol.
        /// </summary>
        /// <returns>the post symbol text</returns>
        public virtual String GetPostSymbolText() {
            return this.GetProperty<String>(Property.LIST_SYMBOL_POST_TEXT);
        }

        /// <summary>
        /// Sets a piece of text that should be added after the
        /// <see cref="ListItem"/>
        /// symbol.
        /// </summary>
        /// <param name="postSymbolText">the post symbol text</param>
        public virtual void SetPostSymbolText(String postSymbolText) {
            SetProperty(Property.LIST_SYMBOL_POST_TEXT, postSymbolText);
        }

        /// <summary>
        /// Gets the piece of text that is added before the
        /// <see cref="ListItem"/>
        /// symbol.
        /// </summary>
        /// <returns>the pre symbol text</returns>
        public virtual String GetPreSymbolText() {
            return this.GetProperty<String>(Property.LIST_SYMBOL_PRE_TEXT);
        }

        /// <summary>
        /// Sets a piece of text that should be added before the
        /// <see cref="ListItem"/>
        /// symbol.
        /// </summary>
        /// <param name="preSymbolText">the pre symbol text</param>
        public virtual void SetPreSymbolText(String preSymbolText) {
            SetProperty(Property.LIST_SYMBOL_PRE_TEXT, preSymbolText);
        }

        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.L);
            }
            return tagProperties;
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new ListRenderer(this);
        }
    }
}
