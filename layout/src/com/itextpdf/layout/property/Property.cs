/*
$Id: caebae720a2acf9ab64b3e1d93c39c60cbe354fc $

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
namespace com.itextpdf.layout.property
{
	/// <summary>
	/// An enum of property names that are used for graphical properties of layout
	/// elements.
	/// </summary>
	/// <remarks>
	/// An enum of property names that are used for graphical properties of layout
	/// elements. The
	/// <see cref="com.itextpdf.layout.IPropertyContainer"/>
	/// performs the same function as an
	/// <see cref="System.Collections.IDictionary{K, V}"/>
	/// , with the values of
	/// <see cref="Property"/>
	/// as its potential keys.
	/// </remarks>
	public sealed class Property
	{
		public static readonly com.itextpdf.layout.property.Property ACTION = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property AREA_BREAK_TYPE = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property AUTO_SCALE = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property AUTO_SCALE_HEIGHT = 
			new com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property AUTO_SCALE_WIDTH = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property BACKGROUND = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property BASE_DIRECTION = new 
			com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property BOLD_SIMULATION = new 
			com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property BORDER = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property BORDER_BOTTOM = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property BORDER_LEFT = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property BORDER_RIGHT = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property BORDER_TOP = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property BOTTOM = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property CHARACTER_SPACING = 
			new com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property COLSPAN = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property DESTINATION = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property FIRST_LINE_INDENT = 
			new com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property FLUSH_ON_DRAW = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property FONT = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property FONT_COLOR = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property FONT_KERNING = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property FONT_SCRIPT = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property FONT_SIZE = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property FULL = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property FORCED_PLACEMENT = new 
			com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property HEIGHT = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property HORIZONTAL_ALIGNMENT
			 = new com.itextpdf.layout.property.Property();

		/// <summary>Value of 1 is equivalent to no scaling</summary>
		public static readonly com.itextpdf.layout.property.Property HORIZONTAL_SCALING = 
			new com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property HYPHENATION = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property ITALIC_SIMULATION = 
			new com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property KEEP_TOGETHER = new 
			com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property LEADING = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property LEFT = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property LINE_DRAWER = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property LIST_START = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property LIST_SYMBOL = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property LIST_SYMBOL_ALIGNMENT
			 = new com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property LIST_SYMBOL_INDENT = 
			new com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property LIST_SYMBOL_PRE_TEXT
			 = new com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property LIST_SYMBOL_POST_TEXT
			 = new com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property MARGIN_BOTTOM = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property MARGIN_LEFT = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property MARGIN_RIGHT = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property MARGIN_TOP = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property PADDING_BOTTOM = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property PADDING_LEFT = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property PADDING_RIGHT = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property PADDING_TOP = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property PAGE_NUMBER = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property POSITION = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property REVERSED = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property RIGHT = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property ROTATION_ANGLE = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property ROTATION_INITIAL_HEIGHT
			 = new com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property ROTATION_INITIAL_WIDTH
			 = new com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property ROTATION_POINT_X = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property ROTATION_POINT_Y = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property ROWSPAN = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property SPACING_RATIO = new 
			com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property SPLIT_CHARACTERS = new 
			com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property STROKE_COLOR = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property STROKE_WIDTH = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property SKEW = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property TAB_ANCHOR = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property TAB_DEFAULT = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property TAB_LEADER = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property TAB_STOPS = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property TEXT_ALIGNMENT = new 
			com.itextpdf.layout.property.Property(true);

		/// <summary>Use values from .</summary>
		public static readonly com.itextpdf.layout.property.Property TEXT_RENDERING_MODE = 
			new com.itextpdf.layout.property.Property(true);

		public static readonly com.itextpdf.layout.property.Property TEXT_RISE = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property TOP = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property UNDERLINE = new com.itextpdf.layout.property.Property
			(true);

		/// <summary>Value of 1 is equivalent to no scaling</summary>
		public static readonly com.itextpdf.layout.property.Property VERTICAL_ALIGNMENT = 
			new com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property VERTICAL_SCALING = new 
			com.itextpdf.layout.property.Property();

		public static readonly com.itextpdf.layout.property.Property WIDTH = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property WORD_SPACING = new com.itextpdf.layout.property.Property
			(true);

		public static readonly com.itextpdf.layout.property.Property X = new com.itextpdf.layout.property.Property
			();

		public static readonly com.itextpdf.layout.property.Property Y = new com.itextpdf.layout.property.Property
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
		/// <see cref="com.itextpdf.layout.IPropertyContainer"/>
		/// objects that
		/// are lower in the document's hierarchy. Most inherited properties are
		/// related to textual operations.
		/// </summary>
		/// <returns>whether or not this type of property is inheritable.</returns>
		public bool IsInherited()
		{
			return com.itextpdf.layout.property.Property.inherited;
		}
	}
}
