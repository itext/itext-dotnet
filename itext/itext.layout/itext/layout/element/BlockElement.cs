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
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;
using iText.Layout.Tagging;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="BlockElement{T}"/>
    /// will try to take up as much horizontal space as
    /// available to it on the canvas or page.
    /// </summary>
    /// <remarks>
    /// A
    /// <see cref="BlockElement{T}"/>
    /// will try to take up as much horizontal space as
    /// available to it on the canvas or page. The concept is comparable to the block
    /// element in HTML. Also like in HTML, the visual representation of the object
    /// can be delimited by padding, a border, and/or a margin.
    /// </remarks>
    /// <typeparam name="T">the type of the implementation</typeparam>
    public abstract class BlockElement<T> : AbstractElement<T>, IAccessibleElement, IBlockElement
        where T : IElement {
        /// <summary>Creates a BlockElement.</summary>
        protected internal BlockElement() {
        }

        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case Property.OVERFLOW_X:
                case Property.OVERFLOW_Y: {
                    return (T1)(Object)OverflowPropertyValue.FIT;
                }

                default: {
                    return base.GetDefaultProperty<T1>(property);
                }
            }
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
        public virtual T SetMarginLeft(float value) {
            UnitValue marginUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.MARGIN_LEFT, marginUV);
            return (T)(Object)this;
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
        public virtual T SetMarginRight(float value) {
            UnitValue marginUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.MARGIN_RIGHT, marginUV);
            return (T)(Object)this;
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
        public virtual T SetMarginTop(float value) {
            UnitValue marginUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.MARGIN_TOP, marginUV);
            return (T)(Object)this;
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
        public virtual T SetMarginBottom(float value) {
            UnitValue marginUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.MARGIN_BOTTOM, marginUV);
            return (T)(Object)this;
        }

        /// <summary>Sets all margins around the element to the same width.</summary>
        /// <param name="commonMargin">the new margin width</param>
        /// <returns>this element</returns>
        public virtual T SetMargin(float commonMargin) {
            return SetMargins(commonMargin, commonMargin, commonMargin, commonMargin);
        }

        /// <summary>Sets the margins around the element to a series of new widths.</summary>
        /// <param name="marginTop">the new margin top width</param>
        /// <param name="marginRight">the new margin right width</param>
        /// <param name="marginBottom">the new margin bottom width</param>
        /// <param name="marginLeft">the new margin left width</param>
        /// <returns>this element</returns>
        public virtual T SetMargins(float marginTop, float marginRight, float marginBottom, float marginLeft) {
            SetMarginTop(marginTop);
            SetMarginRight(marginRight);
            SetMarginBottom(marginBottom);
            SetMarginLeft(marginLeft);
            return (T)(Object)this;
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
        public virtual T SetPaddingLeft(float value) {
            UnitValue paddingUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.PADDING_LEFT, paddingUV);
            return (T)(Object)this;
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
        public virtual T SetPaddingRight(float value) {
            UnitValue paddingUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.PADDING_RIGHT, paddingUV);
            return (T)(Object)this;
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
        public virtual T SetPaddingTop(float value) {
            UnitValue paddingUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.PADDING_TOP, paddingUV);
            return (T)(Object)this;
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
        public virtual T SetPaddingBottom(float value) {
            UnitValue paddingUV = UnitValue.CreatePointValue(value);
            SetProperty(Property.PADDING_BOTTOM, paddingUV);
            return (T)(Object)this;
        }

        /// <summary>Sets all paddings around the element to the same width.</summary>
        /// <param name="commonPadding">the new padding width</param>
        /// <returns>this element</returns>
        public virtual T SetPadding(float commonPadding) {
            return SetPaddings(commonPadding, commonPadding, commonPadding, commonPadding);
        }

        /// <summary>Sets the paddings around the element to a series of new widths.</summary>
        /// <param name="paddingTop">the new padding top width</param>
        /// <param name="paddingRight">the new padding right width</param>
        /// <param name="paddingBottom">the new padding bottom width</param>
        /// <param name="paddingLeft">the new padding left width</param>
        /// <returns>this element</returns>
        public virtual T SetPaddings(float paddingTop, float paddingRight, float paddingBottom, float paddingLeft) {
            SetPaddingTop(paddingTop);
            SetPaddingRight(paddingRight);
            SetPaddingBottom(paddingBottom);
            SetPaddingLeft(paddingLeft);
            return (T)(Object)this;
        }

        /// <summary>Sets the vertical alignment of the element.</summary>
        /// <param name="verticalAlignment">the vertical alignment setting</param>
        /// <returns>this element</returns>
        public virtual T SetVerticalAlignment(VerticalAlignment? verticalAlignment) {
            SetProperty(Property.VERTICAL_ALIGNMENT, verticalAlignment);
            return (T)(Object)this;
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
        public virtual T SetSpacingRatio(float ratio) {
            SetProperty(Property.SPACING_RATIO, ratio);
            return (T)(Object)this;
        }

        /// <summary>
        /// Returns whether the
        /// <see cref="BlockElement{T}"/>
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
        /// <see cref="BlockElement{T}"/>
        /// should be kept together as much
        /// as possible.
        /// </summary>
        /// <param name="keepTogether">
        /// the new value of the
        /// <see cref="iText.Layout.Properties.Property.KEEP_TOGETHER"/>
        /// property
        /// </param>
        /// <returns>this element</returns>
        public virtual T SetKeepTogether(bool keepTogether) {
            SetProperty(Property.KEEP_TOGETHER, keepTogether);
            return (T)(Object)this;
        }

        /// <summary>
        /// Returns whether the end of this
        /// <see cref="BlockElement{T}"/>
        /// and the start of the next sibling of this element
        /// should be placed in the same area.
        /// </summary>
        /// <returns>
        /// the current value of the
        /// <see cref="iText.Layout.Properties.Property.KEEP_WITH_NEXT"/>
        /// property
        /// </returns>
        public virtual bool? IsKeepWithNext() {
            return this.GetProperty<bool?>(Property.KEEP_WITH_NEXT);
        }

        /// <summary>
        /// Sets whether the end of this
        /// <see cref="BlockElement{T}"/>
        /// and the start of the next sibling of this element
        /// should be placed in the same area.
        /// </summary>
        /// <remarks>
        /// Sets whether the end of this
        /// <see cref="BlockElement{T}"/>
        /// and the start of the next sibling of this element
        /// should be placed in the same area.
        /// Note that this will only work for high-level elements, i.e. elements added to the
        /// <see cref="iText.Layout.RootElement{T}"/>.
        /// </remarks>
        /// <param name="keepWithNext">
        /// the new value of the
        /// <see cref="iText.Layout.Properties.Property.KEEP_WITH_NEXT"/>
        /// property
        /// </param>
        /// <returns>this element</returns>
        public virtual T SetKeepWithNext(bool keepWithNext) {
            SetProperty(Property.KEEP_WITH_NEXT, keepWithNext);
            return (T)(Object)this;
        }

        /// <summary>Sets the rotation radAngle.</summary>
        /// <param name="angleInRadians">the new rotation radAngle, as a <c>float</c>, in radians</param>
        /// <returns>this element</returns>
        public virtual T SetRotationAngle(float angleInRadians) {
            SetProperty(Property.ROTATION_ANGLE, angleInRadians);
            return (T)(Object)this;
        }

        /// <summary>Sets the rotation angle.</summary>
        /// <param name="angleInRadians">the new rotation angle, as a <c>double</c>, in radians</param>
        /// <returns>this element</returns>
        public virtual T SetRotationAngle(double angleInRadians) {
            SetProperty(Property.ROTATION_ANGLE, (float)angleInRadians);
            return (T)(Object)this;
        }

        /// <summary>Sets the width property of a block element, measured in points.</summary>
        /// <param name="width">a value measured in points.</param>
        /// <returns>this Element.</returns>
        public virtual T SetWidth(float width) {
            SetProperty(Property.WIDTH, UnitValue.CreatePointValue(width));
            return (T)(Object)this;
        }

        /// <summary>
        /// Sets the width property of a block element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="width">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>this Element.</returns>
        public virtual T SetWidth(UnitValue width) {
            SetProperty(Property.WIDTH, width);
            return (T)(Object)this;
        }

        /// <summary>Gets the width property of a block element.</summary>
        /// <returns>the width of the element, with a value and a measurement unit.</returns>
        /// <seealso cref="iText.Layout.Properties.UnitValue"/>
        public virtual UnitValue GetWidth() {
            return (UnitValue)this.GetProperty<UnitValue>(Property.WIDTH);
        }

        /// <summary>
        /// Sets the height property of a block element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="height">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>this Element.</returns>
        public virtual T SetHeight(UnitValue height) {
            SetProperty(Property.HEIGHT, height);
            return (T)(Object)this;
        }

        /// <summary>Sets the height property a block element as a point-value.</summary>
        /// <param name="height">a floating point value for the new height</param>
        /// <returns>the block element itself.</returns>
        public virtual T SetHeight(float height) {
            UnitValue heightAsUV = UnitValue.CreatePointValue(height);
            SetProperty(Property.HEIGHT, heightAsUV);
            return (T)(Object)this;
        }

        /// <summary>Gets the height property of a block element.</summary>
        /// <returns>the height of the element, as a floating point value. Null if the property is not present</returns>
        public virtual UnitValue GetHeight() {
            return (UnitValue)this.GetProperty<UnitValue>(Property.HEIGHT);
        }

        /// <summary>Sets the max-height of a block element as point-unit value.</summary>
        /// <param name="maxHeight">a floating point value for the new max-height</param>
        /// <returns>the block element itself</returns>
        public virtual T SetMaxHeight(float maxHeight) {
            UnitValue maxHeightAsUV = UnitValue.CreatePointValue(maxHeight);
            SetProperty(Property.MAX_HEIGHT, maxHeightAsUV);
            return (T)(Object)this;
        }

        /// <summary>
        /// Sets the max-height property of a block element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="maxHeight">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>the block element itself</returns>
        public virtual T SetMaxHeight(UnitValue maxHeight) {
            SetProperty(Property.MAX_HEIGHT, maxHeight);
            return (T)(Object)this;
        }

        /// <summary>
        /// Sets the min-height property of a block element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="minHeight">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>the block element itself</returns>
        public virtual T SetMinHeight(UnitValue minHeight) {
            SetProperty(Property.MIN_HEIGHT, minHeight);
            return (T)(Object)this;
        }

        /// <summary>Sets the min-height of a block element as point-unit value.</summary>
        /// <param name="minHeight">a floating point value for the new min-height</param>
        /// <returns>the block element itself</returns>
        public virtual T SetMinHeight(float minHeight) {
            UnitValue minHeightAsUV = UnitValue.CreatePointValue(minHeight);
            SetProperty(Property.MIN_HEIGHT, minHeightAsUV);
            return (T)(Object)this;
        }

        /// <summary>
        /// Sets the max-width property of a block element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="maxWidth">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>the block element itself</returns>
        public virtual T SetMaxWidth(UnitValue maxWidth) {
            SetProperty(Property.MAX_WIDTH, maxWidth);
            return (T)(Object)this;
        }

        /// <summary>Sets the max-width of a block element as point-unit value.</summary>
        /// <param name="maxWidth">a floating point value for the new max-width</param>
        /// <returns>the block element itself</returns>
        public virtual T SetMaxWidth(float maxWidth) {
            SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(maxWidth));
            return (T)(Object)this;
        }

        /// <summary>
        /// Sets the min-width property of a block element with a
        /// <see cref="iText.Layout.Properties.UnitValue"/>.
        /// </summary>
        /// <param name="minWidth">
        /// a
        /// <see cref="iText.Layout.Properties.UnitValue"/>
        /// object
        /// </param>
        /// <returns>the block element itself</returns>
        public virtual T SetMinWidth(UnitValue minWidth) {
            SetProperty(Property.MIN_WIDTH, minWidth);
            return (T)(Object)this;
        }

        /// <summary>Sets the min-width of a block element as point-unit value.</summary>
        /// <param name="minWidth">a floating point value for the new min-width</param>
        /// <returns>the block element itself</returns>
        public virtual T SetMinWidth(float minWidth) {
            SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(minWidth));
            return (T)(Object)this;
        }

        /// <summary>Give this element a neutral role.</summary>
        /// <remarks>
        /// Give this element a neutral role. See also
        /// <see cref="iText.Kernel.Pdf.Tagutils.AccessibilityProperties.SetRole(System.String)"/>.
        /// </remarks>
        /// <returns>this Element</returns>
        public virtual T SetNeutralRole() {
            this.GetAccessibilityProperties().SetRole(null);
            // cast to Object is necessary for autoporting reasons
            return (T)(Object)this;
        }

        public abstract AccessibilityProperties GetAccessibilityProperties();
    }
}
