/*
$Id$

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
using iTextSharp.Layout.Property;

namespace iTextSharp.Layout
{
	/// <summary>Container object for style properties of an element.</summary>
	/// <remarks>
	/// Container object for style properties of an element. A style can be used as
	/// an effective way to define multiple equal properties to several elements.
	/// Used in
	/// <see cref="iTextSharp.Layout.Element.AbstractElement{T}"/>
	/// </remarks>
	public class Style : ElementPropertyContainer<Style>
	{
		/// <summary>Gets the current left margin width of the element.</summary>
		/// <returns>the left margin width, as a <code>float</code></returns>
		public virtual float? GetMarginLeft()
		{
			return this.GetProperty<float>(iTextSharp.Layout.Property.Property.MARGIN_LEFT);
		}

		/// <summary>Sets the left margin width of the element.</summary>
		/// <param name="value">the new left margin width</param>
		/// <returns>this element</returns>
		public virtual Style SetMarginLeft(float value)
		{
			SetProperty(iTextSharp.Layout.Property.Property.MARGIN_LEFT, value);
			return (Style)(Object)this;
		}

		/// <summary>Gets the current right margin width of the element.</summary>
		/// <returns>the right margin width, as a <code>float</code></returns>
		public virtual float? GetMarginRight()
		{
			return this.GetProperty<float>(iTextSharp.Layout.Property.Property.MARGIN_RIGHT);
		}

		/// <summary>Sets the right margin width of the element.</summary>
		/// <param name="value">the new right margin width</param>
		/// <returns>this element</returns>
		public virtual Style SetMarginRight(float value)
		{
			SetProperty(iTextSharp.Layout.Property.Property.MARGIN_RIGHT, value);
			return (Style)(Object)this;
		}

		/// <summary>Gets the current top margin width of the element.</summary>
		/// <returns>the top margin width, as a <code>float</code></returns>
		public virtual float? GetMarginTop()
		{
			return this.GetProperty<float>(iTextSharp.Layout.Property.Property.MARGIN_TOP);
		}

		/// <summary>Sets the top margin width of the element.</summary>
		/// <param name="value">the new top margin width</param>
		/// <returns>this element</returns>
		public virtual Style SetMarginTop(float value)
		{
			SetProperty(iTextSharp.Layout.Property.Property.MARGIN_TOP, value);
			return (Style)(Object)this;
		}

		/// <summary>Gets the current bottom margin width of the element.</summary>
		/// <returns>the bottom margin width, as a <code>float</code></returns>
		public virtual float? GetMarginBottom()
		{
			return this.GetProperty<float>(iTextSharp.Layout.Property.Property.MARGIN_BOTTOM);
		}

		/// <summary>Sets the bottom margin width of the element.</summary>
		/// <param name="value">the new bottom margin width</param>
		/// <returns>this element</returns>
		public virtual Style SetMarginBottom(float value)
		{
			SetProperty(iTextSharp.Layout.Property.Property.MARGIN_BOTTOM, value);
			return (Style)(Object)this;
		}

		/// <summary>Sets all margins around the element to the same width.</summary>
		/// <param name="commonMargin">the new margin width</param>
		/// <returns>this element</returns>
		public virtual Style SetMargin(float commonMargin)
		{
			return SetMargins(commonMargin, commonMargin, commonMargin, commonMargin);
		}

		/// <summary>Sets the margins around the element to a series of new widths.</summary>
		/// <param name="marginTop">the new margin top width</param>
		/// <param name="marginRight">the new margin right width</param>
		/// <param name="marginBottom">the new margin bottom width</param>
		/// <param name="marginLeft">the new margin left width</param>
		/// <returns>this element</returns>
		public virtual Style SetMargins(float marginTop, float marginRight, float marginBottom
			, float marginLeft)
		{
			SetMarginTop(marginTop);
			SetMarginRight(marginRight);
			SetMarginBottom(marginBottom);
			SetMarginLeft(marginLeft);
			return (Style)(Object)this;
		}

		/// <summary>Gets the current left padding width of the element.</summary>
		/// <returns>the left padding width, as a <code>float</code></returns>
		public virtual float? GetPaddingLeft()
		{
			return this.GetProperty<float>(iTextSharp.Layout.Property.Property.PADDING_LEFT);
		}

		/// <summary>Sets the left padding width of the element.</summary>
		/// <param name="value">the new left padding width</param>
		/// <returns>this element</returns>
		public virtual Style SetPaddingLeft(float value)
		{
			SetProperty(iTextSharp.Layout.Property.Property.PADDING_LEFT, value);
			return (Style)(Object)this;
		}

		/// <summary>Gets the current right padding width of the element.</summary>
		/// <returns>the right padding width, as a <code>float</code></returns>
		public virtual float? GetPaddingRight()
		{
			return this.GetProperty<float>(iTextSharp.Layout.Property.Property.PADDING_RIGHT);
		}

		/// <summary>Sets the right padding width of the element.</summary>
		/// <param name="value">the new right padding width</param>
		/// <returns>this element</returns>
		public virtual Style SetPaddingRight(float value)
		{
			SetProperty(iTextSharp.Layout.Property.Property.PADDING_RIGHT, value);
			return (Style)(Object)this;
		}

		/// <summary>Gets the current top padding width of the element.</summary>
		/// <returns>the top padding width, as a <code>float</code></returns>
		public virtual float? GetPaddingTop()
		{
			return this.GetProperty<float>(iTextSharp.Layout.Property.Property.PADDING_TOP);
		}

		/// <summary>Sets the top padding width of the element.</summary>
		/// <param name="value">the new top padding width</param>
		/// <returns>this element</returns>
		public virtual Style SetPaddingTop(float value)
		{
			SetProperty(iTextSharp.Layout.Property.Property.PADDING_TOP, value);
			return (Style)(Object)this;
		}

		/// <summary>Gets the current bottom padding width of the element.</summary>
		/// <returns>the bottom padding width, as a <code>float</code></returns>
		public virtual float? GetPaddingBottom()
		{
			return this.GetProperty<float>(iTextSharp.Layout.Property.Property.PADDING_BOTTOM
				);
		}

		/// <summary>Sets the bottom padding width of the element.</summary>
		/// <param name="value">the new bottom padding width</param>
		/// <returns>this element</returns>
		public virtual Style SetPaddingBottom(float value)
		{
			SetProperty(iTextSharp.Layout.Property.Property.PADDING_BOTTOM, value);
			return (Style)(Object)this;
		}

		/// <summary>Sets all paddings around the element to the same width.</summary>
		/// <param name="commonPadding">the new padding width</param>
		/// <returns>this element</returns>
		public virtual Style SetPadding(float commonPadding)
		{
			return (Style)(Object)SetPaddings(commonPadding, commonPadding, commonPadding, commonPadding
				);
		}

		/// <summary>Sets the paddings around the element to a series of new widths.</summary>
		/// <param name="paddingTop">the new padding top width</param>
		/// <param name="paddingRight">the new padding right width</param>
		/// <param name="paddingBottom">the new padding bottom width</param>
		/// <param name="paddingLeft">the new padding left width</param>
		/// <returns>this element</returns>
		public virtual Style SetPaddings(float paddingTop, float paddingRight, float paddingBottom
			, float paddingLeft)
		{
			SetPaddingTop(paddingTop);
			SetPaddingRight(paddingRight);
			SetPaddingBottom(paddingBottom);
			SetPaddingLeft(paddingLeft);
			return (Style)(Object)this;
		}

		/// <summary>Sets the vertical alignment of the element.</summary>
		/// <param name="verticalAlignment">the vertical alignment setting</param>
		/// <returns>this element</returns>
		public virtual Style SetVerticalAlignment(VerticalAlignment verticalAlignment)
		{
			SetProperty(iTextSharp.Layout.Property.Property.VERTICAL_ALIGNMENT, verticalAlignment
				);
			return (Style)(Object)this;
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
		public virtual Style SetSpacingRatio(float ratio)
		{
			SetProperty(iTextSharp.Layout.Property.Property.SPACING_RATIO, ratio);
			return (Style)(Object)this;
		}

		/// <summary>
		/// Returns whether the
		/// <see cref="iTextSharp.Layout.Element.BlockElement{T}"/>
		/// should be kept together as much
		/// as possible.
		/// </summary>
		/// <returns>
		/// the current value of the
		/// <see cref="iTextSharp.Layout.Property.Property.KEEP_TOGETHER"/>
		/// property
		/// </returns>
		public virtual bool? IsKeepTogether()
		{
			return this.GetProperty<bool>(iTextSharp.Layout.Property.Property.KEEP_TOGETHER);
		}

		/// <summary>
		/// Sets whether the
		/// <see cref="iTextSharp.Layout.Element.BlockElement{T}"/>
		/// should be kept together as much
		/// as possible.
		/// </summary>
		/// <param name="keepTogether">
		/// the new value of the
		/// <see cref="iTextSharp.Layout.Property.Property.KEEP_TOGETHER"/>
		/// property
		/// </param>
		/// <returns>this element</returns>
		public virtual Style SetKeepTogether(bool keepTogether)
		{
			SetProperty(iTextSharp.Layout.Property.Property.KEEP_TOGETHER, keepTogether);
			return (Style)(Object)this;
		}

		/// <summary>Sets the rotation radAngle.</summary>
		/// <param name="radAngle">the new rotation radAngle, as a <code>float</code></param>
		/// <returns>this element</returns>
		public virtual Style SetRotationAngle(float radAngle)
		{
			SetProperty(iTextSharp.Layout.Property.Property.ROTATION_ANGLE, radAngle);
			return (Style)(Object)this;
		}

		/// <summary>Sets the rotation angle.</summary>
		/// <param name="angle">the new rotation angle, as a <code>double</code></param>
		/// <returns>this element</returns>
		public virtual Style SetRotationAngle(double angle)
		{
			SetProperty(iTextSharp.Layout.Property.Property.ROTATION_ANGLE, (float)angle);
			return (Style)(Object)this;
		}
	}
}
