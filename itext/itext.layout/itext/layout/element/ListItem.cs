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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A list item is a layout element that is one entry in a
    /// <see cref="List"/>.
    /// </summary>
    /// <remarks>
    /// A list item is a layout element that is one entry in a
    /// <see cref="List"/>
    /// . The list
    /// object controls the prefix, postfix, and numbering of the list items.
    /// </remarks>
    public class ListItem : Div {
        /// <summary>Creates a ListItem.</summary>
        public ListItem()
            : base() {
        }

        /// <summary>Creates a list item with text.</summary>
        /// <param name="text">the textual contents of the list item</param>
        public ListItem(String text)
            : this() {
            Add(new Paragraph(text).SetMarginTop(0).SetMarginBottom(0));
        }

        /// <summary>Customizes the index of the item in the list.</summary>
        /// <param name="ordinalValue">the custom value property of an ordered list's list item.</param>
        /// <returns>this listItem.</returns>
        public virtual iText.Layout.Element.ListItem SetListSymbolOrdinalValue(int ordinalValue) {
            SetProperty(Property.LIST_SYMBOL_ORDINAL_VALUE, ordinalValue);
            return this;
        }

        /// <summary>Creates a list item with an image.</summary>
        /// <param name="image">the graphical contents of the list item</param>
        public ListItem(Image image)
            : this() {
            Add(image);
        }

        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case Property.LIST_SYMBOL_POSITION: {
                    return (T1)(Object)ListSymbolPosition.DEFAULT;
                }

                default: {
                    return base.GetDefaultProperty<T1>(property);
                }
            }
        }

        /// <summary>Sets the list item symbol to be used.</summary>
        /// <param name="symbol">the textual symbol to be used for the item.</param>
        /// <returns>this list item.</returns>
        public virtual iText.Layout.Element.ListItem SetListSymbol(String symbol) {
            return SetListSymbol(new Text(symbol));
        }

        /// <summary>Sets the list item symbol to be used.</summary>
        /// <param name="text">
        /// the
        /// <see cref="Text"/>
        /// object to be used for the item.
        /// </param>
        /// <returns>this list item.</returns>
        public virtual iText.Layout.Element.ListItem SetListSymbol(Text text) {
            SetProperty(Property.LIST_SYMBOL, text);
            return this;
        }

        /// <summary>Sets the list item symbol to be used.</summary>
        /// <param name="image">
        /// the
        /// <see cref="Image"/>
        /// object to be used for the item.
        /// </param>
        /// <returns>this list.</returns>
        public virtual iText.Layout.Element.ListItem SetListSymbol(Image image) {
            SetProperty(Property.LIST_SYMBOL, image);
            return this;
        }

        /// <summary>Sets the list item numbering type to be used.</summary>
        /// <param name="listNumberingType">
        /// the
        /// <see cref="iText.Layout.Properties.ListNumberingType"/>
        /// that will generate appropriate prefixes for the
        /// <see cref="ListItem"/>.
        /// </param>
        /// <returns>this list item.</returns>
        public virtual iText.Layout.Element.ListItem SetListSymbol(ListNumberingType listNumberingType) {
            // Do not draw any points after ZapfDingbats special number symbol
            if (listNumberingType == ListNumberingType.ZAPF_DINGBATS_1 || listNumberingType == ListNumberingType.ZAPF_DINGBATS_2
                 || listNumberingType == ListNumberingType.ZAPF_DINGBATS_3 || listNumberingType == ListNumberingType.ZAPF_DINGBATS_4
                ) {
                SetProperty(Property.LIST_SYMBOL_POST_TEXT, " ");
            }
            SetProperty(Property.LIST_SYMBOL, listNumberingType);
            return this;
        }

        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.LBODY);
            }
            return tagProperties;
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new ListItemRenderer(this);
        }
    }
}
