/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Layout.Properties;

namespace iText.Layout {
    /// <summary>Container object for style properties of an element.</summary>
    /// <remarks>
    /// Container object for style properties of an element. A style can be used as
    /// an effective way to define multiple equal properties to several elements.
    /// Used in
    /// <see cref="iText.Layout.Element.AbstractElement{T}"/>.
    /// The properties set via Style have a lower priority than directly set properties.
    /// For example, if the same property is set directly and added via Style, then,
    /// no matter in which order they are set, the one set directly will be chosen.
    /// </remarks>
    public class Style : ElementPropertyContainer<iText.Layout.Style> {
        public Style() {
        }

        public Style(iText.Layout.Style style) {
            properties.AddAll(style.properties);
        }

        /// <summary>Gets the current left margin width of the element.</summary>
        /// <returns>
        /// the left margin width, as a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </returns>
        public virtual UnitValue GetMarginLeft() {
            return this.GetProperty<UnitValue>(Property.MARGIN_LEFT);
        }

        /// <summary>Sets the left margin width of the element.</summary>
        /// <param name="value">the new left margin width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetMarginLeft(float value) {
            UnitValue marginUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.MARGIN_LEFT, marginUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Gets the current right margin width of the element.</summary>
        /// <returns>
        /// the right margin width, as a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </returns>
        public virtual UnitValue GetMarginRight() {
            return this.GetProperty<UnitValue>(Property.MARGIN_RIGHT);
        }

        /// <summary>Sets the right margin width of the element.</summary>
        /// <param name="value">the new right margin width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetMarginRight(float value) {
            UnitValue marginUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.MARGIN_RIGHT, marginUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Gets the current top margin width of the element.</summary>
        /// <returns>
        /// the top margin width, as a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </returns>
        public virtual UnitValue GetMarginTop() {
            return this.GetProperty<UnitValue>(Property.MARGIN_TOP);
        }

        /// <summary>Sets the top margin width of the element.</summary>
        /// <param name="value">the new top margin width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetMarginTop(float value) {
            UnitValue marginUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.MARGIN_TOP, marginUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Gets the current bottom margin width of the element.</summary>
        /// <returns>
        /// the bottom margin width, as a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </returns>
        public virtual UnitValue GetMarginBottom() {
            return this.GetProperty<UnitValue>(Property.MARGIN_BOTTOM);
        }

        /// <summary>Sets the bottom margin width of the element.</summary>
        /// <param name="value">the new bottom margin width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetMarginBottom(float value) {
            UnitValue marginUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.MARGIN_BOTTOM, marginUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Sets all margins around the element to the same width.</summary>
        /// <param name="commonMargin">the new margin width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetMargin(float commonMargin) {
            return SetMargins(commonMargin, commonMargin, commonMargin, commonMargin);
        }

        /// <summary>Sets the margins around the element to a series of new widths.</summary>
        /// <param name="marginTop">the new margin top width</param>
        /// <param name="marginRight">the new margin right width</param>
        /// <param name="marginBottom">the new margin bottom width</param>
        /// <param name="marginLeft">the new margin left width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetMargins(float marginTop, float marginRight, float marginBottom, float
             marginLeft) {
            SetMarginTop(marginTop);
            SetMarginRight(marginRight);
            SetMarginBottom(marginBottom);
            SetMarginLeft(marginLeft);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Gets the current left padding width of the element.</summary>
        /// <returns>
        /// the left padding width, as a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </returns>
        public virtual UnitValue GetPaddingLeft() {
            return this.GetProperty<UnitValue>(Property.PADDING_LEFT);
        }

        /// <summary>Sets the left padding width of the element.</summary>
        /// <param name="value">the new left padding width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetPaddingLeft(float value) {
            UnitValue paddingUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.PADDING_LEFT, paddingUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Gets the current right padding width of the element.</summary>
        /// <returns>
        /// the right padding width, as a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </returns>
        public virtual UnitValue GetPaddingRight() {
            return this.GetProperty<UnitValue>(Property.PADDING_RIGHT);
        }

        /// <summary>Sets the right padding width of the element.</summary>
        /// <param name="value">the new right padding width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetPaddingRight(float value) {
            UnitValue paddingUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.PADDING_RIGHT, paddingUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Gets the current top padding width of the element.</summary>
        /// <returns>
        /// the top padding width, as a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </returns>
        public virtual UnitValue GetPaddingTop() {
            return this.GetProperty<UnitValue>(Property.PADDING_TOP);
        }

        /// <summary>Sets the top padding width of the element.</summary>
        /// <param name="value">the new top padding width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetPaddingTop(float value) {
            UnitValue paddingUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.PADDING_TOP, paddingUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Gets the current bottom padding width of the element.</summary>
        /// <returns>
        /// the bottom padding width, as a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </returns>
        public virtual UnitValue GetPaddingBottom() {
            return this.GetProperty<UnitValue>(Property.PADDING_BOTTOM);
        }

        /// <summary>Sets the bottom padding width of the element.</summary>
        /// <param name="value">the new bottom padding width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetPaddingBottom(float value) {
            UnitValue paddingUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.PADDING_BOTTOM, paddingUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Sets all paddings around the element to the same width.</summary>
        /// <param name="commonPadding">the new padding width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetPadding(float commonPadding) {
            return (iText.Layout.Style)(Object)SetPaddings(commonPadding, commonPadding, commonPadding, commonPadding);
        }

        /// <summary>Sets the paddings around the element to a series of new widths.</summary>
        /// <param name="paddingTop">the new padding top width</param>
        /// <param name="paddingRight">the new padding right width</param>
        /// <param name="paddingBottom">the new padding bottom width</param>
        /// <param name="paddingLeft">the new padding left width</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetPaddings(float paddingTop, float paddingRight, float paddingBottom, float
             paddingLeft) {
            SetPaddingTop(paddingTop);
            SetPaddingRight(paddingRight);
            SetPaddingBottom(paddingBottom);
            SetPaddingLeft(paddingLeft);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Sets the vertical alignment of the element.</summary>
        /// <param name="verticalAlignment">the vertical alignment setting</param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetVerticalAlignment(VerticalAlignment? verticalAlignment) {
            SetProperty(Property.VERTICAL_ALIGNMENT, verticalAlignment);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>
        /// Sets a ratio which determines in which proportion will word spacing and character spacing
        /// be applied when horizontal alignment is justified.
        /// </summary>
        /// <param name="ratio">
        /// the ratio coefficient. It must be between 0 and 1, inclusive.
        /// It means that <strong>ratio</strong> part of the free space will
        /// be compensated by word spacing, and <strong>1-ratio</strong> part of the free space will
        /// be compensated by character spacing.
        /// If <strong>ratio</strong> is 1, additional character spacing will not be applied.
        /// If <strong>ratio</strong> is 0, additional word spacing will not be applied.
        /// </param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetSpacingRatio(float ratio) {
            SetProperty(Property.SPACING_RATIO, ratio);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>
        /// Returns whether the
        /// <see cref="iText.Layout.Element.BlockElement{T}"/>
        /// should be kept together as much
        /// as possible.
        /// </summary>
        /// <returns>
        /// the current value of the
        /// <see cref="iText.Layout.Properties.Property.KEEP_TOGETHER"/>
        /// property
        /// </returns>
        public virtual bool? IsKeepTogether() {
            return this.GetProperty<bool?>(Property.KEEP_TOGETHER);
        }

        /// <summary>
        /// Sets whether the
        /// <see cref="iText.Layout.Element.BlockElement{T}"/>
        /// should be kept together as much
        /// as possible.
        /// </summary>
        /// <param name="keepTogether">
        /// the new value of the
        /// <see cref="iText.Layout.Properties.Property.KEEP_TOGETHER"/>
        /// property
        /// </param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetKeepTogether(bool keepTogether) {
            SetProperty(Property.KEEP_TOGETHER, keepTogether);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Sets the rotation radAngle.</summary>
        /// <param name="radAngle">the new rotation radAngle, as a <c>float</c></param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetRotationAngle(float radAngle) {
            SetProperty(Property.ROTATION_ANGLE, radAngle);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Sets the rotation angle.</summary>
        /// <param name="angle">the new rotation angle, as a <c>double</c></param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Style SetRotationAngle(double angle) {
            SetProperty(Property.ROTATION_ANGLE, (float)angle);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Sets the width property of the element, measured in points.</summary>
        /// <param name="width">a value measured in points.</param>
        /// <returns>this Element.</returns>
        public virtual iText.Layout.Style SetWidth(float width) {
            SetProperty(Property.WIDTH, UnitValue.CreatePointValue(width));
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>
        /// Sets the width property of the element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="width">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>this Element.</returns>
        public virtual iText.Layout.Style SetWidth(UnitValue width) {
            SetProperty(Property.WIDTH, width);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Gets the width property of the element.</summary>
        /// <returns>the width of the element, with a value and a measurement unit.</returns>
        /// <seealso cref="iText.Layout.Properties.UnitValue"/>
        public virtual UnitValue GetWidth() {
            return (UnitValue)this.GetProperty<UnitValue>(Property.WIDTH);
        }

        /// <summary>
        /// Sets the height property of the element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="height">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>this Element.</returns>
        public virtual iText.Layout.Style SetHeight(UnitValue height) {
            SetProperty(Property.HEIGHT, height);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Sets the height property the element as a point-value.</summary>
        /// <param name="height">a floating point value for the new height</param>
        /// <returns>the block element itself.</returns>
        public virtual iText.Layout.Style SetHeight(float height) {
            UnitValue heightAsUV = UnitValue.CreatePointValue(height);
            SetProperty(Property.HEIGHT, heightAsUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Gets the height property of the element.</summary>
        /// <returns>the height of the element, as a floating point value. Null if the property is not present</returns>
        public virtual UnitValue GetHeight() {
            return (UnitValue)this.GetProperty<UnitValue>(Property.HEIGHT);
        }

        /// <summary>Sets the max-height of the element as point-unit value.</summary>
        /// <param name="maxHeight">a floating point value for the new max-height</param>
        /// <returns>the block element itself</returns>
        public virtual iText.Layout.Style SetMaxHeight(float maxHeight) {
            UnitValue maxHeightAsUV = UnitValue.CreatePointValue(maxHeight);
            SetProperty(Property.MAX_HEIGHT, maxHeightAsUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>
        /// Sets the max-height property of the element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="maxHeight">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>the block element itself</returns>
        public virtual iText.Layout.Style SetMaxHeight(UnitValue maxHeight) {
            SetProperty(Property.MAX_HEIGHT, maxHeight);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>
        /// Sets the min-height property of the element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="minHeight">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>the block element itself</returns>
        public virtual iText.Layout.Style SetMinHeight(UnitValue minHeight) {
            SetProperty(Property.MIN_HEIGHT, minHeight);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Sets the min-height of the element as point-unit value.</summary>
        /// <param name="minHeight">a floating point value for the new min-height</param>
        /// <returns>the block element itself</returns>
        public virtual iText.Layout.Style SetMinHeight(float minHeight) {
            UnitValue minHeightAsUV = UnitValue.CreatePointValue(minHeight);
            SetProperty(Property.MIN_HEIGHT, minHeightAsUV);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>
        /// Sets the max-width property of the element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="maxWidth">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>the block element itself</returns>
        public virtual iText.Layout.Style SetMaxWidth(UnitValue maxWidth) {
            SetProperty(Property.MAX_WIDTH, maxWidth);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Sets the max-width of the element as point-unit value.</summary>
        /// <param name="maxWidth">a floating point value for the new max-width</param>
        /// <returns>the block element itself</returns>
        public virtual iText.Layout.Style SetMaxWidth(float maxWidth) {
            SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(maxWidth));
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>
        /// Sets the min-width property of the element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="minWidth">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>the block element itself</returns>
        public virtual iText.Layout.Style SetMinWidth(UnitValue minWidth) {
            SetProperty(Property.MIN_WIDTH, minWidth);
            return (iText.Layout.Style)(Object)this;
        }

        /// <summary>Sets the min-width of the element as point-unit value.</summary>
        /// <param name="minWidth">a floating point value for the new min-width</param>
        /// <returns>the block element itself</returns>
        public virtual iText.Layout.Style SetMinWidth(float minWidth) {
            SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(minWidth));
            return (iText.Layout.Style)(Object)this;
        }
    }
}
