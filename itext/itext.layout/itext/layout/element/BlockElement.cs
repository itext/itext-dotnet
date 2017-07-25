/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="BlockElement{T}"/>
    /// will try to take up as much horizontal space as
    /// available to it on the canvas or page. The concept is comparable to the block
    /// element in HTML. Also like in HTML, the visual representation of the object
    /// can be delimited by padding, a border, and/or a margin.
    /// </summary>
    /// 
    public abstract class BlockElement<T> : AbstractElement<T>, IAccessibleElement, IBlockElement
        where T : IElement {
        /// <summary>Creates a BlockElement.</summary>
        protected internal BlockElement() {
        }

        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case Property.OVERFLOW:
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
        /// <returns>the left margin width, as a <code>float</code></returns>
        public virtual float? GetMarginLeft() {
            return this.GetProperty<float?>(Property.MARGIN_LEFT);
        }

        /// <summary>Sets the left margin width of the element.</summary>
        /// <param name="value">the new left margin width</param>
        /// <returns>this element</returns>
        public virtual T SetMarginLeft(float value) {
            SetProperty(Property.MARGIN_LEFT, value);
            return (T)(Object)this;
        }

        /// <summary>Gets the current right margin width of the element.</summary>
        /// <returns>the right margin width, as a <code>float</code></returns>
        public virtual float? GetMarginRight() {
            return this.GetProperty<float?>(Property.MARGIN_RIGHT);
        }

        /// <summary>Sets the right margin width of the element.</summary>
        /// <param name="value">the new right margin width</param>
        /// <returns>this element</returns>
        public virtual T SetMarginRight(float value) {
            SetProperty(Property.MARGIN_RIGHT, value);
            return (T)(Object)this;
        }

        /// <summary>Gets the current top margin width of the element.</summary>
        /// <returns>the top margin width, as a <code>float</code></returns>
        public virtual float? GetMarginTop() {
            return this.GetProperty<float?>(Property.MARGIN_TOP);
        }

        /// <summary>Sets the top margin width of the element.</summary>
        /// <param name="value">the new top margin width</param>
        /// <returns>this element</returns>
        public virtual T SetMarginTop(float value) {
            SetProperty(Property.MARGIN_TOP, value);
            return (T)(Object)this;
        }

        /// <summary>Gets the current bottom margin width of the element.</summary>
        /// <returns>the bottom margin width, as a <code>float</code></returns>
        public virtual float? GetMarginBottom() {
            return this.GetProperty<float?>(Property.MARGIN_BOTTOM);
        }

        /// <summary>Sets the bottom margin width of the element.</summary>
        /// <param name="value">the new bottom margin width</param>
        /// <returns>this element</returns>
        public virtual T SetMarginBottom(float value) {
            SetProperty(Property.MARGIN_BOTTOM, value);
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
        /// <returns>the left padding width, as a <code>float</code></returns>
        public virtual float? GetPaddingLeft() {
            return this.GetProperty<float?>(Property.PADDING_LEFT);
        }

        /// <summary>Sets the left padding width of the element.</summary>
        /// <param name="value">the new left padding width</param>
        /// <returns>this element</returns>
        public virtual T SetPaddingLeft(float value) {
            SetProperty(Property.PADDING_LEFT, value);
            return (T)(Object)this;
        }

        /// <summary>Gets the current right padding width of the element.</summary>
        /// <returns>the right padding width, as a <code>float</code></returns>
        public virtual float? GetPaddingRight() {
            return this.GetProperty<float?>(Property.PADDING_RIGHT);
        }

        /// <summary>Sets the right padding width of the element.</summary>
        /// <param name="value">the new right padding width</param>
        /// <returns>this element</returns>
        public virtual T SetPaddingRight(float value) {
            SetProperty(Property.PADDING_RIGHT, value);
            return (T)(Object)this;
        }

        /// <summary>Gets the current top padding width of the element.</summary>
        /// <returns>the top padding width, as a <code>float</code></returns>
        public virtual float? GetPaddingTop() {
            return this.GetProperty<float?>(Property.PADDING_TOP);
        }

        /// <summary>Sets the top padding width of the element.</summary>
        /// <param name="value">the new top padding width</param>
        /// <returns>this element</returns>
        public virtual T SetPaddingTop(float value) {
            SetProperty(Property.PADDING_TOP, value);
            return (T)(Object)this;
        }

        /// <summary>Gets the current bottom padding width of the element.</summary>
        /// <returns>the bottom padding width, as a <code>float</code></returns>
        public virtual float? GetPaddingBottom() {
            return this.GetProperty<float?>(Property.PADDING_BOTTOM);
        }

        /// <summary>Sets the bottom padding width of the element.</summary>
        /// <param name="value">the new bottom padding width</param>
        /// <returns>this element</returns>
        public virtual T SetPaddingBottom(float value) {
            SetProperty(Property.PADDING_BOTTOM, value);
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
        /// It means that <b>ratio</b> part of the free space will
        /// be compensated by word spacing, and <b>1-ratio</b> part of the free space will
        /// be compensated by character spacing.
        /// If <b>ratio</b> is 1, additional character spacing will not be applied.
        /// If <b>ratio</b> is 0, additional word spacing will not be applied.
        /// </param>
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
        /// Note that this will only work for high-level elements, i.e. elements added to the
        /// <see cref="iText.Layout.RootElement{T}"/>
        /// .
        /// </summary>
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
        /// <param name="angleInRadians">the new rotation radAngle, as a <code>float</code>, in radians</param>
        /// <returns>this element</returns>
        public virtual T SetRotationAngle(float angleInRadians) {
            SetProperty(Property.ROTATION_ANGLE, angleInRadians);
            return (T)(Object)this;
        }

        /// <summary>Sets the rotation angle.</summary>
        /// <param name="angleInRadians">the new rotation angle, as a <code>double</code>, in radians</param>
        /// <returns>this element</returns>
        public virtual T SetRotationAngle(double angleInRadians) {
            SetProperty(Property.ROTATION_ANGLE, (float)angleInRadians);
            return (T)(Object)this;
        }

        public override T SetHeight(float height) {
            base.SetHeight(height);
            return (T)(Object)this;
        }

        public virtual T SetMaxHeight(float maxHeight) {
            SetProperty(Property.MAX_HEIGHT, maxHeight);
            return (T)(Object)this;
        }

        public virtual T SetMinHeight(float minHeight) {
            SetProperty(Property.MIN_HEIGHT, minHeight);
            return (T)(Object)this;
        }

        public virtual T SetMaxWidth(float maxWidth) {
            SetProperty(Property.MAX_WIDTH, maxWidth);
            return (T)(Object)this;
        }

        public virtual T SetMinWidth(float minWidth) {
            SetProperty(Property.MIN_WIDTH, minWidth);
            return (T)(Object)this;
        }

        public abstract AccessibilityProperties GetAccessibilityProperties();

        public abstract PdfName GetRole();

        public abstract void SetRole(PdfName arg1);
    }
}
