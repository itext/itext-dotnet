/*
$Id: d14f89840eb0e051f7bd38409930e905e6a9fbf4 $

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
using com.itextpdf.kernel.color;
using com.itextpdf.layout.element;

namespace com.itextpdf.layout
{
	/// <summary>
	/// An enum of property names that are used for graphical properties of layout
	/// elements.
	/// </summary>
	/// <remarks>
	/// An enum of property names that are used for graphical properties of layout
	/// elements. The
	/// <see cref="IPropertyContainer{T}"/>
	/// performs the same function as an
	/// <see cref="EnumMap"/>
	/// , with the values of
	/// <see cref="Property"/>
	/// as its potential keys.
	/// </remarks>
	public sealed class Property
	{
		public static readonly com.itextpdf.layout.Property ACTION = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property AREA_BREAK_TYPE = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property AUTO_SCALE = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property AUTO_SCALE_HEIGHT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property AUTO_SCALE_WIDTH = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property BACKGROUND = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property BASE_DIRECTION = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property BOLD_SIMULATION = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property BORDER = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property BORDER_BOTTOM = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property BORDER_LEFT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property BORDER_RIGHT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property BORDER_TOP = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property BOTTOM = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property CHARACTER_SPACING = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property COLSPAN = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property DESTINATION = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property FIRST_LINE_INDENT = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property FLUSH_ON_DRAW = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property FONT = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property FONT_COLOR = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property FONT_KERNING = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property FONT_SCRIPT = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property FONT_SIZE = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property FULL = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property FORCED_PLACEMENT = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property HEIGHT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property HORIZONTAL_ALIGNMENT = new com.itextpdf.layout.Property
			();

		/// <summary>Value of 1 is equivalent to no scaling</summary>
		public static readonly com.itextpdf.layout.Property HORIZONTAL_SCALING = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property HYPHENATION = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property ITALIC_SIMULATION = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property KEEP_TOGETHER = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property LEADING = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property LEFT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property LINE_DRAWER = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property LIST_START = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property LIST_SYMBOL = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property LIST_SYMBOL_ALIGNMENT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property LIST_SYMBOL_INDENT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property LIST_SYMBOL_PRE_TEXT = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property LIST_SYMBOL_POST_TEXT = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property MARGIN_BOTTOM = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property MARGIN_LEFT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property MARGIN_RIGHT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property MARGIN_TOP = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property PADDING_BOTTOM = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property PADDING_LEFT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property PADDING_RIGHT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property PADDING_TOP = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property PAGE_NUMBER = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property POSITION = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property REVERSED = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property RIGHT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property ROTATION_ANGLE = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property ROTATION_INITIAL_HEIGHT = new 
			com.itextpdf.layout.Property();

		public static readonly com.itextpdf.layout.Property ROTATION_INITIAL_WIDTH = new 
			com.itextpdf.layout.Property();

		public static readonly com.itextpdf.layout.Property ROTATION_POINT_X = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property ROTATION_POINT_Y = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property ROWSPAN = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property SPACING_RATIO = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property SPLIT_CHARACTERS = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property STROKE_COLOR = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property STROKE_WIDTH = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property SKEW = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property TAB_ANCHOR = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property TAB_DEFAULT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property TAB_LEADER = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property TAB_STOPS = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property TEXT_ALIGNMENT = new com.itextpdf.layout.Property
			(true);

		/// <summary>Use values from .</summary>
		public static readonly com.itextpdf.layout.Property TEXT_RENDERING_MODE = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property TEXT_RISE = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property TOP = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property UNDERLINE = new com.itextpdf.layout.Property
			(true);

		/// <summary>Value of 1 is equivalent to no scaling</summary>
		public static readonly com.itextpdf.layout.Property VERTICAL_ALIGNMENT = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property VERTICAL_SCALING = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property WIDTH = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property WORD_SPACING = new com.itextpdf.layout.Property
			(true);

		public static readonly com.itextpdf.layout.Property X = new com.itextpdf.layout.Property
			();

		public static readonly com.itextpdf.layout.Property Y = new com.itextpdf.layout.Property
			();

		private bool inherited;

		internal Property()
		{
			this.inherited = false;
		}

		internal Property(bool inherited)
		{
			this.inherited = inherited;
		}

		/// <summary>
		/// Some properties must be passed to
		/// <see cref="IPropertyContainer{T}"/>
		/// objects that
		/// are lower in the document's hierarchy. Most inherited properties are
		/// related to textual operations.
		/// </summary>
		/// <returns>whether or not this type of property is inheritable.</returns>
		public bool IsInherited()
		{
			return com.itextpdf.layout.Property.inherited;
		}

		/// <summary>
		/// A specialized enum containing potential property values for
		/// <see cref="HORIZONTAL_ALIGNMENT"/>
		/// .
		/// </summary>
		public enum HorizontalAlignment
		{
			LEFT,
			CENTER,
			RIGHT
		}

		/// <summary>
		/// A specialized enum containing potential property values for
		/// <see cref="VERTICAL_ALIGNMENT"/>
		/// .
		/// </summary>
		public enum VerticalAlignment
		{
			TOP,
			MIDDLE,
			BOTTOM
		}

		/// <summary>
		/// A specialized enum containing potential property values for
		/// <see cref="TEXT_ALIGNMENT"/>
		/// .
		/// </summary>
		public enum TextAlignment
		{
			LEFT,
			CENTER,
			RIGHT,
			JUSTIFIED,
			JUSTIFIED_ALL
		}

		/// <summary>A specialized enum containing alignment properties for list symbols.</summary>
		/// <remarks>
		/// A specialized enum containing alignment properties for list symbols.
		/// <see cref="ListSymbolAlignment.LEFT"/>
		/// means that the items will be aligned as follows:
		/// 9.  Item 9
		/// 10. Item 10
		/// Whereas
		/// <see cref="ListSymbolAlignment.RIGHT"/>
		/// means the items will be aligned as follows:
		/// 9. Item 9
		/// 10. Item 10
		/// </remarks>
		public enum ListSymbolAlignment
		{
			RIGHT,
			LEFT
		}

		/// <summary>
		/// A specialized class holding configurable properties related to an
		/// <see cref="Element"/>
		/// 's background. This class is meant to be used as the value for the
		/// <see cref="Property.BACKGROUND"/>
		/// key in an
		/// <see cref="IPropertyContainer{T}"/>
		/// . Allows
		/// to define a background color, and positive or negative changes to the
		/// location of the edges of the background coloring.
		/// </summary>
		public class Background
		{
			protected internal Color color;

			protected internal float extraLeft;

			protected internal float extraRight;

			protected internal float extraTop;

			protected internal float extraBottom;

			/// <summary>Creates a background with a specified color.</summary>
			/// <param name="color">the background color</param>
			public Background(Color color)
				: this(color, 0, 0, 0, 0)
			{
			}

			/// <summary>
			/// Creates a background with a specified color, and extra space that
			/// must be counted as part of the background and therefore colored.
			/// </summary>
			/// <remarks>
			/// Creates a background with a specified color, and extra space that
			/// must be counted as part of the background and therefore colored.
			/// These values are allowed to be negative.
			/// </remarks>
			/// <param name="color">the background color</param>
			/// <param name="extraLeft">extra coloring to the left side</param>
			/// <param name="extraTop">extra coloring at the top</param>
			/// <param name="extraRight">extra coloring to the right side</param>
			/// <param name="extraBottom">extra coloring at the bottom</param>
			public Background(Color color, float extraLeft, float extraTop, float extraRight, 
				float extraBottom)
			{
				this.color = color;
				this.extraLeft = extraLeft;
				this.extraRight = extraRight;
				this.extraTop = extraTop;
				this.extraBottom = extraBottom;
			}

			/// <summary>Gets the background's color.</summary>
			/// <returns>
			/// a
			/// <see cref="com.itextpdf.kernel.color.Color"/>
			/// of any supported kind
			/// </returns>
			public virtual Color GetColor()
			{
				return color;
			}

			/// <summary>Gets the extra space that must be filled to the left of the Element.</summary>
			/// <returns>a float value</returns>
			public virtual float GetExtraLeft()
			{
				return extraLeft;
			}

			/// <summary>Gets the extra space that must be filled to the right of the Element.</summary>
			/// <returns>a float value</returns>
			public virtual float GetExtraRight()
			{
				return extraRight;
			}

			/// <summary>Gets the extra space that must be filled at the top of the Element.</summary>
			/// <returns>a float value</returns>
			public virtual float GetExtraTop()
			{
				return extraTop;
			}

			/// <summary>Gets the extra space that must be filled at the bottom of the Element.</summary>
			/// <returns>a float value</returns>
			public virtual float GetExtraBottom()
			{
				return extraBottom;
			}
		}

		/// <summary>
		/// A specialized class that specifies the leading, "the vertical distance between
		/// the baselines of adjacent lines of text" (ISO-32000-1, section 9.3.5).
		/// </summary>
		/// <remarks>
		/// A specialized class that specifies the leading, "the vertical distance between
		/// the baselines of adjacent lines of text" (ISO-32000-1, section 9.3.5).
		/// Allows to use either an absolute (constant) leading value, or one
		/// determined by font size. Pronounce as 'ledding' (cfr. Led Zeppelin).
		/// This class is meant to be used as the value for the
		/// <see cref="Property.LEADING"/>
		/// key in an
		/// <see cref="IPropertyContainer{T}"/>
		/// .
		/// </remarks>
		public class Leading
		{
			/// <summary>A leading type independent of font size.</summary>
			public const int FIXED = 1;

			/// <summary>A leading type related to the font size and the resulting bounding box.</summary>
			public const int MULTIPLIED = 2;

			protected internal int type;

			protected internal float value;

			/// <summary>Creates a Leading object.</summary>
			/// <param name="type">
			/// a constant type that defines the calculation of actual
			/// leading distance. Either
			/// <see cref="FIXED"/>
			/// or
			/// <see cref="MULTIPLIED"/>
			/// </param>
			/// <param name="value">to be used as a basis for the leading calculation.</param>
			public Leading(int type, float value)
			{
				this.type = type;
				this.value = value;
			}

			/// <summary>Gets the calculation type of the Leading object.</summary>
			/// <returns>
			/// the calculation type. Either
			/// <see cref="FIXED"/>
			/// or
			/// <see cref="MULTIPLIED"/>
			/// </returns>
			public virtual int GetType()
			{
				return type;
			}

			/// <summary>Gets the value to be used as the basis for the leading calculation.</summary>
			/// <returns>a calculation value</returns>
			public virtual float GetValue()
			{
				return value;
			}
		}

		public class Underline
		{
			protected internal Color color;

			protected internal float thickness;

			protected internal float thicknessMul;

			protected internal float yPosition;

			protected internal float yPositionMul;

			protected internal int lineCapStyle;

			public Underline(Color color, float thickness, float thicknessMul, float yPosition
				, float yPositionMul, int lineCapStyle)
			{
				this.color = color;
				this.thickness = thickness;
				this.thicknessMul = thicknessMul;
				this.yPosition = yPosition;
				this.yPositionMul = yPositionMul;
				this.lineCapStyle = lineCapStyle;
			}

			public virtual Color GetColor()
			{
				return color;
			}

			public virtual float GetThickness(float fontSize)
			{
				return thickness + thicknessMul * fontSize;
			}

			public virtual float GetYPosition(float fontSize)
			{
				return yPosition + yPositionMul * fontSize;
			}

			public virtual float GetYPositionMul()
			{
				return yPositionMul;
			}
		}

		/// <summary>A specialized class that holds a value and the unit it is measured in.</summary>
		public class UnitValue
		{
			public const int POINT = 1;

			public const int PERCENT = 2;

			protected internal int unitType;

			protected internal float value;

			/// <summary>Creates a UnitValue object with a specified type and value.</summary>
			/// <param name="unitType">
			/// either
			/// <see cref="POINT"/>
			/// or a
			/// <see cref="PERCENT"/>
			/// </param>
			/// <param name="value">the value to be stored.</param>
			public UnitValue(int unitType, float value)
			{
				this.unitType = unitType;
				this.value = value;
			}

			/// <summary>Creates a UnitValue POINT object with a specified value.</summary>
			/// <param name="value">the value to be stored.</param>
			/// <returns>
			/// a new
			/// <see cref="POINT"/>
			/// 
			/// <see cref="UnitValue"/>
			/// </returns>
			public static Property.UnitValue CreatePointValue(float value)
			{
				return new Property.UnitValue(POINT, value);
			}

			/// <summary>Creates a UnitValue PERCENT object with a specified value.</summary>
			/// <param name="value">the value to be stored.</param>
			/// <returns>
			/// a new
			/// <see cref="PERCENT"/>
			/// 
			/// <see cref="UnitValue"/>
			/// </returns>
			public static Property.UnitValue CreatePercentValue(float value)
			{
				return new Property.UnitValue(PERCENT, value);
			}

			public virtual int GetUnitType()
			{
				return unitType;
			}

			public virtual void SetUnitType(int unitType)
			{
				this.unitType = unitType;
			}

			public virtual float GetValue()
			{
				return value;
			}

			public virtual void SetValue(float value)
			{
				this.value = value;
			}

			public virtual bool IsPointValue()
			{
				return unitType == POINT;
			}

			public virtual bool IsPercentValue()
			{
				return unitType == PERCENT;
			}

			public override bool Equals(Object obj)
			{
				if (!(obj is Property.UnitValue))
				{
					return false;
				}
				Property.UnitValue other = (Property.UnitValue)obj;
				return int.Compare(unitType, other.unitType) == 0 && float.Compare(value, other.value
					) == 0;
			}

			public override int GetHashCode()
			{
				int hash = 7;
				hash = 71 * hash + this.unitType;
				hash = 71 * hash + com.itextpdf.io.util.JavaUtil.FloatToIntBits(this.value);
				return hash;
			}
		}

		/// <summary>
		/// A specialized enum holding the possible values for a list
		/// <see cref="List"/>
		/// 's entry prefix. This class is meant to
		/// be used as the value for the
		/// <see cref="LIST_SYMBOL"/>
		/// key in an
		/// <see cref="IPropertyContainer{T}"/>
		/// .
		/// </summary>
		public enum ListNumberingType
		{
			DECIMAL,
			ROMAN_LOWER,
			ROMAN_UPPER,
			ENGLISH_LOWER,
			ENGLISH_UPPER,
			GREEK_LOWER,
			GREEK_UPPER,
			ZAPF_DINGBATS_1,
			ZAPF_DINGBATS_2,
			ZAPF_DINGBATS_3,
			ZAPF_DINGBATS_4
		}

		/// <summary>
		/// A specialized enum holding the possible values for a
		/// <see cref="List">List</see>
		/// 's entry prefix. This class is meant
		/// to be used as the value for the
		/// <see cref="LIST_SYMBOL"/>
		/// key in an
		/// <see cref="IPropertyContainer{T}"/>
		/// .
		/// </summary>
		public enum TabAlignment
		{
			LEFT,
			RIGHT,
			CENTER,
			ANCHOR
		}

		/// <summary>
		/// A specialized enum holding the possible values for a text
		/// <see cref="Element"/>
		/// 's kerning property. This class is meant to
		/// be used as the value for the
		/// <see cref="FONT_KERNING"/>
		/// key in an
		/// <see cref="IPropertyContainer{T}"/>
		/// .
		/// </summary>
		public enum FontKerning
		{
			YES,
			NO
		}

		/// <summary>
		/// A specialized enum holding the possible values for a text
		/// <see cref="Element"/>
		/// 's base direction. This class is meant to
		/// be used as the value for the
		/// <see cref="BASE_DIRECTION"/>
		/// key in an
		/// <see cref="IPropertyContainer{T}"/>
		/// .
		/// </summary>
		public enum BaseDirection
		{
			NO_BIDI,
			DEFAULT_BIDI,
			LEFT_TO_RIGHT,
			RIGHT_TO_LEFT
		}

		public enum AreaBreakType
		{
			NEW_AREA,
			NEW_PAGE,
			LAST_PAGE
		}
	}
}
