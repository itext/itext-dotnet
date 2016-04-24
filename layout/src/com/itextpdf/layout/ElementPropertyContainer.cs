/*
$Id: dbf0ced56f9bdf633ca061583cf7c45b5d941038 $

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
using System.Collections;
using System.Collections.Generic;
using com.itextpdf.kernel.color;
using com.itextpdf.kernel.font;
using com.itextpdf.kernel.pdf.canvas;
using com.itextpdf.layout.border;
using com.itextpdf.layout.hyphenation;
using com.itextpdf.layout.layout;
using com.itextpdf.layout.splitting;
using java.lang;

namespace com.itextpdf.layout
{
	/// <summary>A generic abstract element that fits in a PDF layout object hierarchy.</summary>
	/// <remarks>
	/// A generic abstract element that fits in a PDF layout object hierarchy.
	/// A superclass of all
	/// <see cref="com.itextpdf.layout.element.IElement">layout object</see>
	/// implementations.
	/// </remarks>
	/// <?/>
	public abstract class ElementPropertyContainer<T> : IPropertyContainer<T>
	{
		protected internal IDictionary<Property, Object> properties = new EnumMap<Property
			, Object>(typeof(Property));

		public virtual T1 SetProperty<T1>(Property property, Object value)
			where T1 : T
		{
			properties[property] = value;
			return (T1)this;
		}

		public virtual bool HasProperty(Property property)
		{
			return HasOwnProperty(property);
		}

		public virtual bool HasOwnProperty(Property property)
		{
			return properties.ContainsKey(property);
		}

		public virtual void DeleteOwnProperty(Property property)
		{
			properties.Remove(property);
		}

		public virtual T1 GetProperty<T1>(Property property)
		{
			return this.GetOwnProperty<T1>(property);
		}

		public virtual T1 GetOwnProperty<T1>(Property property)
		{
			return (T1)properties[property];
		}

		public virtual T1 GetDefaultProperty<T1>(Property property)
		{
			switch (property)
			{
				case Property.MARGIN_TOP:
				case Property.MARGIN_RIGHT:
				case Property.MARGIN_BOTTOM:
				case Property.MARGIN_LEFT:
				case Property.PADDING_TOP:
				case Property.PADDING_RIGHT:
				case Property.PADDING_BOTTOM:
				case Property.PADDING_LEFT:
				{
					return (T1)(Object)0f;
				}

				case Property.POSITION:
				{
					return (T1)(Object)LayoutPosition.STATIC;
				}

				case Property.FORCED_PLACEMENT:
				{
					return (T1)(Object)false;
				}

				default:
				{
					return (T1)(Object)null;
				}
			}
		}

		/// <summary>Gets the width property of the Element.</summary>
		/// <returns>the width of the element, with a value and a measurement unit.</returns>
		/// <seealso>
		/// 
		/// <see cref="UnitValue"/>
		/// </seealso>
		public virtual Property.UnitValue GetWidth()
		{
			return this.GetProperty<Property.UnitValue>(Property.WIDTH);
		}

		/// <summary>Sets the width property of the Element, measured in points.</summary>
		/// <param name="width">a value measured in points.</param>
		/// <returns>this Element.</returns>
		public virtual T SetWidth(float width)
		{
			return this.SetProperty<T>(Property.WIDTH, Property.UnitValue.CreatePointValue(width
				));
		}

		/// <summary>Sets the width property of the Element, measured in percentage.</summary>
		/// <param name="widthPercent">a value measured in percentage.</param>
		/// <returns>this Element.</returns>
		public virtual T SetWidthPercent(float widthPercent)
		{
			return this.SetProperty<T>(Property.WIDTH, Property.UnitValue.CreatePercentValue(
				widthPercent));
		}

		/// <summary>
		/// Sets the width property of the Element with a
		/// <see cref="UnitValue"/>
		/// .
		/// </summary>
		/// <param name="width">
		/// a
		/// <see cref="UnitValue"/>
		/// object
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetWidth(Property.UnitValue width)
		{
			return this.SetProperty<T>(Property.WIDTH, width);
		}

		/// <summary>Gets the height property of the Element.</summary>
		/// <returns>the height of the element, as a floating point value.</returns>
		public virtual float GetHeight()
		{
			return this.GetProperty<float>(Property.HEIGHT);
		}

		/// <summary>Sets the height property of the Element.</summary>
		/// <param name="height">a floating point value for the new height</param>
		/// <returns>this Element.</returns>
		public virtual T SetHeight(float height)
		{
			return this.SetProperty<T>(Property.HEIGHT, height);
		}

		/// <summary>Sets values for a relative repositioning of the Element.</summary>
		/// <remarks>
		/// Sets values for a relative repositioning of the Element. Also has as a
		/// side effect that the Element's
		/// <see cref="Property.POSITION"/>
		/// is changed to
		/// <see cref="com.itextpdf.layout.layout.LayoutPosition.RELATIVE">relative</see>
		/// .
		/// The default implementation in
		/// <see cref="com.itextpdf.layout.renderer.AbstractRenderer"/>
		/// treats
		/// <code>left</code> and <code>top</code> as the most important values. Only
		/// if <code>left == 0</code> will <code>right</code> be used for the
		/// calculation; ditto for top vs. bottom.
		/// </remarks>
		/// <param name="left">movement to the left</param>
		/// <param name="top">movement upwards on the page</param>
		/// <param name="right">movement to the right</param>
		/// <param name="bottom">movement downwards on the page</param>
		/// <returns>this Element.</returns>
		/// <seealso cref="com.itextpdf.layout.layout.LayoutPosition.RELATIVE"/>
		public virtual T SetRelativePosition(float left, float top, float right, float bottom
			)
		{
			this.SetProperty<T>(Property.POSITION, LayoutPosition.RELATIVE);
			this.SetProperty<T>(Property.LEFT, left);
			this.SetProperty<T>(Property.RIGHT, right);
			this.SetProperty<T>(Property.TOP, top);
			this.SetProperty<T>(Property.BOTTOM, bottom);
			return (T)(Object)this;
		}

		/// <summary>Sets values for a absolute repositioning of the Element.</summary>
		/// <remarks>
		/// Sets values for a absolute repositioning of the Element. Also has as a
		/// side effect that the Element's
		/// <see cref="Property.POSITION"/>
		/// is changed to
		/// <see cref="com.itextpdf.layout.layout.LayoutPosition.FIXED">fixed</see>
		/// .
		/// </remarks>
		/// <param name="x">horizontal position on the page</param>
		/// <param name="y">vertical position on the page</param>
		/// <param name="width">a floating point value measured in points.</param>
		/// <returns>this Element.</returns>
		public virtual T SetFixedPosition(float x, float y, float width)
		{
			return SetFixedPosition(x, y, Property.UnitValue.CreatePointValue(width));
		}

		/// <summary>Sets values for a absolute repositioning of the Element.</summary>
		/// <remarks>
		/// Sets values for a absolute repositioning of the Element. Also has as a
		/// side effect that the Element's
		/// <see cref="Property.POSITION"/>
		/// is changed to
		/// <see cref="com.itextpdf.layout.layout.LayoutPosition.FIXED">fixed</see>
		/// .
		/// </remarks>
		/// <param name="x">horizontal position on the page</param>
		/// <param name="y">vertical position on the page</param>
		/// <param name="width">
		/// a
		/// <see cref="UnitValue"/>
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetFixedPosition(float x, float y, Property.UnitValue width)
		{
			this.SetProperty<T>(Property.POSITION, LayoutPosition.FIXED);
			this.SetProperty<T>(Property.X, x);
			this.SetProperty<T>(Property.Y, y);
			this.SetProperty<T>(Property.WIDTH, width);
			return (T)(Object)this;
		}

		/// <summary>
		/// Sets values for a absolute repositioning of the Element, on a specific
		/// page.
		/// </summary>
		/// <remarks>
		/// Sets values for a absolute repositioning of the Element, on a specific
		/// page. Also has as a side effect that the Element's
		/// <see cref="Property.POSITION"/>
		/// is changed to
		/// <see cref="com.itextpdf.layout.layout.LayoutPosition.FIXED">fixed</see>
		/// .
		/// </remarks>
		/// <param name="pageNumber">the page where the element must be positioned</param>
		/// <param name="x">horizontal position on the page</param>
		/// <param name="y">vertical position on the page</param>
		/// <param name="width">a floating point value measured in points.</param>
		/// <returns>this Element.</returns>
		public virtual T SetFixedPosition(int pageNumber, float x, float y, float width)
		{
			this.SetFixedPosition(x, y, width);
			this.SetProperty<T>(Property.PAGE_NUMBER, pageNumber);
			return (T)(Object)this;
		}

		/// <summary>
		/// Sets values for a absolute repositioning of the Element, on a specific
		/// page.
		/// </summary>
		/// <remarks>
		/// Sets values for a absolute repositioning of the Element, on a specific
		/// page. Also has as a side effect that the Element's
		/// <see cref="Property.POSITION"/>
		/// is changed to
		/// <see cref="com.itextpdf.layout.layout.LayoutPosition.FIXED">fixed</see>
		/// .
		/// </remarks>
		/// <param name="pageNumber">the page where the element must be positioned</param>
		/// <param name="x">horizontal position on the page</param>
		/// <param name="y">vertical position on the page</param>
		/// <param name="width">a floating point value measured in points.</param>
		/// <returns>this Element.</returns>
		public virtual T SetFixedPosition(int pageNumber, float x, float y, Property.UnitValue
			 width)
		{
			this.SetFixedPosition(x, y, width);
			this.SetProperty<T>(Property.PAGE_NUMBER, pageNumber);
			return (T)(Object)this;
		}

		//    public T setAbsolutePosition(float x, float y) {
		//        return (T) setProperty(Property.POSITION, LayoutPosition.ABSOLUTE).
		//            setProperty(Property.X, x).
		//            setProperty(Property.Y, y);
		//    }
		/// <summary>Sets the horizontal alignment of this Element.</summary>
		/// <param name="horizontalAlignment">
		/// an enum value of type
		/// <see cref="HorizontalAlignment"/>
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetHorizontalAlignment(Property.HorizontalAlignment horizontalAlignment
			)
		{
			return this.SetProperty<T>(Property.HORIZONTAL_ALIGNMENT, horizontalAlignment);
		}

		/// <summary>Sets the font of this Element.</summary>
		/// <param name="font">
		/// a
		/// <see cref="com.itextpdf.kernel.font.PdfFont">font program</see>
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetFont(PdfFont font)
		{
			return this.SetProperty<T>(Property.FONT, font);
		}

		/// <summary>Sets the font color of this Element.</summary>
		/// <param name="fontColor">
		/// a
		/// <see cref="com.itextpdf.kernel.color.Color"/>
		/// for the text in this Element.
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetFontColor(Color fontColor)
		{
			return this.SetProperty<T>(Property.FONT_COLOR, fontColor);
		}

		/// <summary>Sets the font size of this Element.</summary>
		/// <param name="fontSize">a floating point value</param>
		/// <returns>this Element.</returns>
		public virtual T SetFontSize(float fontSize)
		{
			return this.SetProperty<T>(Property.FONT_SIZE, fontSize);
		}

		/// <summary>Sets the font size of this Element.</summary>
		/// <param name="alignment">
		/// an enum value of type
		/// <see cref="TextAlignment"/>
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetTextAlignment(Property.TextAlignment alignment)
		{
			return this.SetProperty<T>(Property.TEXT_ALIGNMENT, alignment);
		}

		/// <summary>Defines a custom spacing distance between all characters of a textual element.
		/// 	</summary>
		/// <remarks>
		/// Defines a custom spacing distance between all characters of a textual element.
		/// The character-spacing parameter is added to the glyph???s horizontal or vertical displacement (depending on the writing mode).
		/// </remarks>
		/// <param name="charSpacing">a floating point value</param>
		/// <returns>this Element.</returns>
		public virtual T SetCharacterSpacing(float charSpacing)
		{
			return this.SetProperty<T>(Property.CHARACTER_SPACING, charSpacing);
		}

		/// <summary>Defines a custom spacing distance between words of a textual element.</summary>
		/// <remarks>
		/// Defines a custom spacing distance between words of a textual element.
		/// This value works exactly like the character spacing, but only kicks in at word boundaries.
		/// </remarks>
		/// <param name="wordSpacing">a floating point value</param>
		/// <returns>this Element.</returns>
		public virtual T SetWordSpacing(float wordSpacing)
		{
			return this.SetProperty<T>(Property.WORD_SPACING, wordSpacing);
		}

		/// <summary>Enable or disable kerning.</summary>
		/// <remarks>
		/// Enable or disable kerning.
		/// Some fonts may specify kern pairs, i.e. pair of glyphs, between which the amount of horizontal space is adjusted.
		/// This adjustment is typically negative, e.g. in "AV" pair the glyphs will typically be moved closer to each other.
		/// </remarks>
		/// <param name="fontKerning">an enum value as a boolean wrapper specifying whether or not to apply kerning
		/// 	</param>
		/// <returns>this Element.</returns>
		public virtual T SetFontKerning(Property.FontKerning fontKerning)
		{
			return this.SetProperty<T>(Property.FONT_KERNING, fontKerning);
		}

		/// <summary>Specifies a background color for the Element.</summary>
		/// <param name="backgroundColor">the background color</param>
		/// <returns>this Element.</returns>
		public virtual T SetBackgroundColor(Color backgroundColor)
		{
			return SetBackgroundColor(backgroundColor, 0, 0, 0, 0);
		}

		/// <summary>
		/// Specifies a background color for the Element, and extra space that
		/// must be counted as part of the background and therefore colored.
		/// </summary>
		/// <param name="backgroundColor">the background color</param>
		/// <param name="extraLeft">extra coloring to the left side</param>
		/// <param name="extraTop">extra coloring at the top</param>
		/// <param name="extraRight">extra coloring to the right side</param>
		/// <param name="extraBottom">extra coloring at the bottom</param>
		/// <returns>this Element.</returns>
		public virtual T SetBackgroundColor(Color backgroundColor, float extraLeft, float
			 extraTop, float extraRight, float extraBottom)
		{
			return this.SetProperty<T>(Property.BACKGROUND, new Property.Background(backgroundColor
				, extraLeft, extraTop, extraRight, extraBottom));
		}

		/// <summary>Sets a border for all four edges of this Element with customizable color, width, pattern type.
		/// 	</summary>
		/// <param name="border">
		/// a customized
		/// <see cref="com.itextpdf.layout.border.Border"/>
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetBorder(Border border)
		{
			return this.SetProperty<T>(Property.BORDER, border);
		}

		/// <summary>Sets a border for the upper limit of this Element with customizable color, width, pattern type.
		/// 	</summary>
		/// <param name="border">
		/// a customized
		/// <see cref="com.itextpdf.layout.border.Border"/>
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetBorderTop(Border border)
		{
			return this.SetProperty<T>(Property.BORDER_TOP, border);
		}

		/// <summary>Sets a border for the right limit of this Element with customizable color, width, pattern type.
		/// 	</summary>
		/// <param name="border">
		/// a customized
		/// <see cref="com.itextpdf.layout.border.Border"/>
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetBorderRight(Border border)
		{
			return this.SetProperty<T>(Property.BORDER_RIGHT, border);
		}

		/// <summary>Sets a border for the bottom limit of this Element with customizable color, width, pattern type.
		/// 	</summary>
		/// <param name="border">
		/// a customized
		/// <see cref="com.itextpdf.layout.border.Border"/>
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetBorderBottom(Border border)
		{
			return this.SetProperty<T>(Property.BORDER_BOTTOM, border);
		}

		/// <summary>Sets a border for the left limit of this Element with customizable color, width, pattern type.
		/// 	</summary>
		/// <param name="border">
		/// a customized
		/// <see cref="com.itextpdf.layout.border.Border"/>
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetBorderLeft(Border border)
		{
			return this.SetProperty<T>(Property.BORDER_LEFT, border);
		}

		/// <summary>Sets a rule for splitting strings when they don't fit into one line.</summary>
		/// <remarks>
		/// Sets a rule for splitting strings when they don't fit into one line.
		/// The default implementation is
		/// <see cref="com.itextpdf.layout.splitting.DefaultSplitCharacters"/>
		/// </remarks>
		/// <param name="splitCharacters">
		/// an implementation of
		/// <see cref="com.itextpdf.layout.splitting.ISplitCharacters"/>
		/// </param>
		/// <returns>this Element.</returns>
		public virtual T SetSplitCharacters(ISplitCharacters splitCharacters)
		{
			return this.SetProperty<T>(Property.SPLIT_CHARACTERS, splitCharacters);
		}

		/// <summary>Gets a rule for splitting strings when they don't fit into one line.</summary>
		/// <returns>
		/// the current string splitting rule, an implementation of
		/// <see cref="com.itextpdf.layout.splitting.ISplitCharacters"/>
		/// </returns>
		public virtual ISplitCharacters GetSplitCharacters()
		{
			return this.GetProperty<ISplitCharacters>(Property.SPLIT_CHARACTERS);
		}

		/// <summary>
		/// Gets the text rendering mode, a variable that determines whether showing
		/// text causes glyph outlines to be stroked, filled, used as a clipping
		/// boundary, or some combination of the three.
		/// </summary>
		/// <returns>the current text rendering mode</returns>
		/// <seealso cref="com.itextpdf.kernel.pdf.canvas.PdfCanvasConstants.TextRenderingMode
		/// 	"/>
		public virtual int GetTextRenderingMode()
		{
			return this.GetProperty<int>(Property.TEXT_RENDERING_MODE);
		}

		/// <summary>
		/// Sets the text rendering mode, a variable that determines whether showing
		/// text causes glyph outlines to be stroked, filled, used as a clipping
		/// boundary, or some combination of the three.
		/// </summary>
		/// <param name="textRenderingMode">an <code>int</code> value</param>
		/// <returns>this Element.</returns>
		/// <seealso cref="com.itextpdf.kernel.pdf.canvas.PdfCanvasConstants.TextRenderingMode
		/// 	"/>
		public virtual T SetTextRenderingMode(int textRenderingMode)
		{
			return this.SetProperty<T>(Property.TEXT_RENDERING_MODE, textRenderingMode);
		}

		/// <summary>Gets the stroke color for the current element.</summary>
		/// <remarks>
		/// Gets the stroke color for the current element.
		/// The stroke color is the color of the outlines or edges of a shape.
		/// </remarks>
		/// <returns>the current stroke color</returns>
		public virtual Color GetStrokeColor()
		{
			return this.GetProperty<Color>(Property.STROKE_COLOR);
		}

		/// <summary>Sets the stroke color for the current element.</summary>
		/// <remarks>
		/// Sets the stroke color for the current element.
		/// The stroke color is the color of the outlines or edges of a shape.
		/// </remarks>
		/// <param name="strokeColor">a new stroke color</param>
		/// <returns>this Element.</returns>
		public virtual T SetStrokeColor(Color strokeColor)
		{
			return this.SetProperty<T>(Property.STROKE_COLOR, strokeColor);
		}

		/// <summary>Gets the stroke width for the current element.</summary>
		/// <remarks>
		/// Gets the stroke width for the current element.
		/// The stroke width is the width of the outlines or edges of a shape.
		/// </remarks>
		/// <returns>the current stroke width</returns>
		public virtual float GetStrokeWidth()
		{
			return this.GetProperty<float>(Property.STROKE_WIDTH);
		}

		/// <summary>Sets the stroke width for the current element.</summary>
		/// <remarks>
		/// Sets the stroke width for the current element.
		/// The stroke width is the width of the outlines or edges of a shape.
		/// </remarks>
		/// <param name="strokeWidth">a new stroke width</param>
		/// <returns>this Element.</returns>
		public virtual T SetStrokeWidth(float strokeWidth)
		{
			return this.SetProperty<T>(Property.STROKE_WIDTH, strokeWidth);
		}

		/// <summary>Switch on the simulation of bold style for a font.</summary>
		/// <remarks>
		/// Switch on the simulation of bold style for a font.
		/// Be aware that using correct bold font is highly preferred over this option.
		/// </remarks>
		/// <returns>this element</returns>
		public virtual T SetBold()
		{
			return this.SetProperty<T>(Property.BOLD_SIMULATION, true);
		}

		/// <summary>Switch on the simulation of italic style for a font.</summary>
		/// <remarks>
		/// Switch on the simulation of italic style for a font.
		/// Be aware that using correct italic (oblique) font is highly preferred over this option.
		/// </remarks>
		/// <returns>this element</returns>
		public virtual T SetItalic()
		{
			return this.SetProperty<T>(Property.ITALIC_SIMULATION, true);
		}

		/// <summary>Sets default line-through attributes for text.</summary>
		/// <remarks>
		/// Sets default line-through attributes for text.
		/// See
		/// <see cref="ElementPropertyContainer{T}.SetUnderline(com.itextpdf.kernel.color.Color, float, float, float, float, int)
		/// 	"/>
		/// for more fine tuning.
		/// </remarks>
		/// <returns>this element</returns>
		public virtual T SetLineThrough()
		{
			// 7/24 is the average between default browser behavior(1/4) and iText5 behavior(1/3)
			return SetUnderline(null, .75f, 0, 0, 7 / 24f, PdfCanvasConstants.LineCapStyle.BUTT
				);
		}

		/// <summary>Sets default underline attributes for text.</summary>
		/// <remarks>
		/// Sets default underline attributes for text.
		/// See other overloads for more fine tuning.
		/// </remarks>
		/// <returns>this element</returns>
		public virtual T SetUnderline()
		{
			return SetUnderline(null, .75f, 0, 0, -1 / 8f, PdfCanvasConstants.LineCapStyle.BUTT
				);
		}

		/// <summary>Sets an horizontal line that can be an underline or a strikethrough.</summary>
		/// <remarks>
		/// Sets an horizontal line that can be an underline or a strikethrough.
		/// Actually, the line can be anywhere vertically and has always the text width.
		/// Multiple call to this method will produce multiple lines.
		/// </remarks>
		/// <param name="thickness">the absolute thickness of the line</param>
		/// <param name="yPosition">the absolute y position relative to the baseline</param>
		/// <returns>this element</returns>
		public virtual T SetUnderline(float thickness, float yPosition)
		{
			return SetUnderline(null, thickness, 0, yPosition, 0, PdfCanvasConstants.LineCapStyle
				.BUTT);
		}

		/// <summary>Sets an horizontal line that can be an underline or a strikethrough.</summary>
		/// <remarks>
		/// Sets an horizontal line that can be an underline or a strikethrough.
		/// Actually, the line can be anywhere vertically due to position parameter.
		/// Multiple call to this method will produce multiple lines.
		/// The thickness of the line will be
		/// <c>thickness + thicknessMul * fontSize</c>
		/// .
		/// The position of the line will be
		/// <c>baseLine + yPosition + yPositionMul * fontSize</c>
		/// .
		/// </remarks>
		/// <param name="color">
		/// the color of the line or <CODE>null</CODE> to follow the
		/// text color
		/// </param>
		/// <param name="thickness">the absolute thickness of the line</param>
		/// <param name="thicknessMul">the thickness multiplication factor with the font size
		/// 	</param>
		/// <param name="yPosition">the absolute y position relative to the baseline</param>
		/// <param name="yPositionMul">the position multiplication factor with the font size</param>
		/// <param name="lineCapStyle">
		/// the end line cap style. Allowed values are enumerated in
		/// <see cref="com.itextpdf.kernel.pdf.canvas.PdfCanvasConstants.LineCapStyle"/>
		/// </param>
		/// <returns>this element</returns>
		public virtual T SetUnderline(Color color, float thickness, float thicknessMul, float
			 yPosition, float yPositionMul, int lineCapStyle)
		{
			Property.Underline newUnderline = new Property.Underline(color, thickness, thicknessMul
				, yPosition, yPositionMul, lineCapStyle);
			Object currentProperty = this.GetProperty<T>(Property.UNDERLINE);
			if (currentProperty is IList)
			{
				((IList)currentProperty).Add(newUnderline);
			}
			else
			{
				if (currentProperty is Property.Underline)
				{
					this.SetProperty<T>(Property.UNDERLINE, com.itextpdf.io.util.JavaUtil.ArraysAsList
						((Property.Underline)currentProperty, newUnderline));
				}
				else
				{
					this.SetProperty<T>(Property.UNDERLINE, newUnderline);
				}
			}
			return (T)(Object)this;
		}

		/// <summary>
		/// This attribute specifies the base direction of directionally neutral text
		/// (i.e., text that doesn't have inherent directionality as defined in Unicode)
		/// in an element's content and attribute values.
		/// </summary>
		/// <param name="baseDirection">base direction</param>
		/// <returns>this element</returns>
		public virtual T SetBaseDirection(Property.BaseDirection baseDirection)
		{
			return this.SetProperty<T>(Property.BASE_DIRECTION, baseDirection);
		}

		/// <summary>
		/// Sets a custom hyphenation configuration which will hyphenate words automatically accordingly to the
		/// language and country.
		/// </summary>
		/// <param name="hyphenationConfig">
		/// 
		/// <seealso/>
		/// 
		/// <see cref="com.itextpdf.layout.hyphenation.HyphenationConfig"/>
		/// }
		/// </param>
		/// <returns>this element</returns>
		public virtual T SetHyphenation(HyphenationConfig hyphenationConfig)
		{
			return this.SetProperty<T>(Property.HYPHENATION, hyphenationConfig);
		}

		/// <summary>Sets the writing system for this text element.</summary>
		/// <param name="script">a new script type</param>
		/// <returns>this Element.</returns>
		public virtual T SetFontScript(Character.UnicodeScript script)
		{
			return this.SetProperty<T>(Property.FONT_SCRIPT, script);
		}

		/// <summary>Sets a destination name that will be created when this element is drawn to content.
		/// 	</summary>
		/// <param name="destination">the destination name to be created</param>
		/// <returns>this Element.</returns>
		public virtual T SetDestination(String destination)
		{
			return this.SetProperty<T>(Property.DESTINATION, destination);
		}
	}
}
