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
