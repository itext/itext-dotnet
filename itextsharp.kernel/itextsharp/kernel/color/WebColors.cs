/*
$Id: 3f62c5971f6b74fd77e7640e30242d1fc7282b82 $

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
using System.Collections.Generic;
using com.itextpdf.io.util;
using com.itextpdf.kernel;

namespace com.itextpdf.kernel.color
{
	/// <summary>
	/// This class is a HashMap that contains the names of colors as a key and the
	/// corresponding BaseColor as value.
	/// </summary>
	/// <remarks>
	/// This class is a HashMap that contains the names of colors as a key and the
	/// corresponding BaseColor as value. (Source: Wikipedia
	/// http://en.wikipedia.org/wiki/Web_colors )
	/// </remarks>
	public class WebColors : Dictionary<String, int[]>
	{
		private const long serialVersionUID = 3542523100813372896L;

		/// <summary>HashMap containing all the names and corresponding color values.</summary>
		public static readonly WebColors NAMES = new WebColors();

		static WebColors()
		{
			NAMES["aliceblue"] = new int[] { unchecked((int)(0xf0)), unchecked((int)(0xf8)), 
				unchecked((int)(0xff)), unchecked((int)(0xff)) };
			NAMES["antiquewhite"] = new int[] { unchecked((int)(0xfa)), unchecked((int)(0xeb)
				), 0xd7, unchecked((int)(0xff)) };
			NAMES["aqua"] = new int[] { 0x00, unchecked((int)(0xff)), unchecked((int)(0xff)), 
				unchecked((int)(0xff)) };
			NAMES["aquamarine"] = new int[] { 0x7f, unchecked((int)(0xff)), 0xd4, unchecked((
				int)(0xff)) };
			NAMES["azure"] = new int[] { unchecked((int)(0xf0)), unchecked((int)(0xff)), unchecked(
				(int)(0xff)), unchecked((int)(0xff)) };
			NAMES["beige"] = new int[] { unchecked((int)(0xf5)), unchecked((int)(0xf5)), 0xdc
				, unchecked((int)(0xff)) };
			NAMES["bisque"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xe4)), unchecked(
				(int)(0xc4)), unchecked((int)(0xff)) };
			NAMES["black"] = new int[] { 0x00, 0x00, 0x00, unchecked((int)(0xff)) };
			NAMES["blanchedalmond"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xeb
				)), unchecked((int)(0xcd)), unchecked((int)(0xff)) };
			NAMES["blue"] = new int[] { 0x00, 0x00, unchecked((int)(0xff)), unchecked((int)(0xff
				)) };
			NAMES["blueviolet"] = new int[] { unchecked((int)(0x8a)), 0x2b, unchecked((int)(0xe2
				)), unchecked((int)(0xff)) };
			NAMES["brown"] = new int[] { 0xa5, 0x2a, 0x2a, unchecked((int)(0xff)) };
			NAMES["burlywood"] = new int[] { 0xde, 0xb8, unchecked((int)(0x87)), unchecked((int
				)(0xff)) };
			NAMES["cadetblue"] = new int[] { 0x5f, 0x9e, 0xa0, unchecked((int)(0xff)) };
			NAMES["chartreuse"] = new int[] { 0x7f, unchecked((int)(0xff)), 0x00, unchecked((
				int)(0xff)) };
			NAMES["chocolate"] = new int[] { 0xd2, 0x69, 0x1e, unchecked((int)(0xff)) };
			NAMES["coral"] = new int[] { unchecked((int)(0xff)), 0x7f, 0x50, unchecked((int)(
				0xff)) };
			NAMES["cornflowerblue"] = new int[] { 0x64, 0x95, unchecked((int)(0xed)), unchecked(
				(int)(0xff)) };
			NAMES["cornsilk"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xf8)), 0xdc
				, unchecked((int)(0xff)) };
			NAMES["crimson"] = new int[] { 0xdc, 0x14, 0x3c, unchecked((int)(0xff)) };
			NAMES["cyan"] = new int[] { 0x00, unchecked((int)(0xff)), unchecked((int)(0xff)), 
				unchecked((int)(0xff)) };
			NAMES["darkblue"] = new int[] { 0x00, 0x00, unchecked((int)(0x8b)), unchecked((int
				)(0xff)) };
			NAMES["darkcyan"] = new int[] { 0x00, unchecked((int)(0x8b)), unchecked((int)(0x8b
				)), unchecked((int)(0xff)) };
			NAMES["darkgoldenrod"] = new int[] { 0xb8, unchecked((int)(0x86)), 0x0b, unchecked(
				(int)(0xff)) };
			NAMES["darkgray"] = new int[] { 0xa9, 0xa9, 0xa9, unchecked((int)(0xff)) };
			NAMES["darkgreen"] = new int[] { 0x00, 0x64, 0x00, unchecked((int)(0xff)) };
			NAMES["darkkhaki"] = new int[] { 0xbd, 0xb7, 0x6b, unchecked((int)(0xff)) };
			NAMES["darkmagenta"] = new int[] { unchecked((int)(0x8b)), 0x00, unchecked((int)(
				0x8b)), unchecked((int)(0xff)) };
			NAMES["darkolivegreen"] = new int[] { 0x55, 0x6b, 0x2f, unchecked((int)(0xff)) };
			NAMES["darkorange"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0x8c)), 
				0x00, unchecked((int)(0xff)) };
			NAMES["darkorchid"] = new int[] { 0x99, 0x32, unchecked((int)(0xcc)), unchecked((
				int)(0xff)) };
			NAMES["darkred"] = new int[] { unchecked((int)(0x8b)), 0x00, 0x00, unchecked((int
				)(0xff)) };
			NAMES["darksalmon"] = new int[] { unchecked((int)(0xe9)), 0x96, 0x7a, unchecked((
				int)(0xff)) };
			NAMES["darkseagreen"] = new int[] { unchecked((int)(0x8f)), 0xbc, unchecked((int)
				(0x8f)), unchecked((int)(0xff)) };
			NAMES["darkslateblue"] = new int[] { 0x48, 0x3d, unchecked((int)(0x8b)), unchecked(
				(int)(0xff)) };
			NAMES["darkslategray"] = new int[] { 0x2f, 0x4f, 0x4f, unchecked((int)(0xff)) };
			NAMES["darkturquoise"] = new int[] { 0x00, unchecked((int)(0xce)), 0xd1, unchecked(
				(int)(0xff)) };
			NAMES["darkviolet"] = new int[] { 0x94, 0x00, 0xd3, unchecked((int)(0xff)) };
			NAMES["deeppink"] = new int[] { unchecked((int)(0xff)), 0x14, 0x93, unchecked((int
				)(0xff)) };
			NAMES["deepskyblue"] = new int[] { 0x00, 0xbf, unchecked((int)(0xff)), unchecked(
				(int)(0xff)) };
			NAMES["dimgray"] = new int[] { 0x69, 0x69, 0x69, unchecked((int)(0xff)) };
			NAMES["dodgerblue"] = new int[] { 0x1e, 0x90, unchecked((int)(0xff)), unchecked((
				int)(0xff)) };
			NAMES["firebrick"] = new int[] { 0xb2, 0x22, 0x22, unchecked((int)(0xff)) };
			NAMES["floralwhite"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xfa))
				, unchecked((int)(0xf0)), unchecked((int)(0xff)) };
			NAMES["forestgreen"] = new int[] { 0x22, unchecked((int)(0x8b)), 0x22, unchecked(
				(int)(0xff)) };
			NAMES["fuchsia"] = new int[] { unchecked((int)(0xff)), 0x00, unchecked((int)(0xff
				)), unchecked((int)(0xff)) };
			NAMES["gainsboro"] = new int[] { 0xdc, 0xdc, 0xdc, unchecked((int)(0xff)) };
			NAMES["ghostwhite"] = new int[] { unchecked((int)(0xf8)), unchecked((int)(0xf8)), 
				unchecked((int)(0xff)), unchecked((int)(0xff)) };
			NAMES["gold"] = new int[] { unchecked((int)(0xff)), 0xd7, 0x00, unchecked((int)(0xff
				)) };
			NAMES["goldenrod"] = new int[] { 0xda, 0xa5, 0x20, unchecked((int)(0xff)) };
			NAMES["gray"] = new int[] { unchecked((int)(0x80)), unchecked((int)(0x80)), unchecked(
				(int)(0x80)), unchecked((int)(0xff)) };
			NAMES["green"] = new int[] { 0x00, unchecked((int)(0x80)), 0x00, unchecked((int)(
				0xff)) };
			NAMES["greenyellow"] = new int[] { 0xad, unchecked((int)(0xff)), 0x2f, unchecked(
				(int)(0xff)) };
			NAMES["honeydew"] = new int[] { unchecked((int)(0xf0)), unchecked((int)(0xff)), unchecked(
				(int)(0xf0)), unchecked((int)(0xff)) };
			NAMES["hotpink"] = new int[] { unchecked((int)(0xff)), 0x69, 0xb4, unchecked((int
				)(0xff)) };
			NAMES["indianred"] = new int[] { unchecked((int)(0xcd)), 0x5c, 0x5c, unchecked((int
				)(0xff)) };
			NAMES["indigo"] = new int[] { 0x4b, 0x00, unchecked((int)(0x82)), unchecked((int)
				(0xff)) };
			NAMES["ivory"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked(
				(int)(0xf0)), unchecked((int)(0xff)) };
			NAMES["khaki"] = new int[] { unchecked((int)(0xf0)), unchecked((int)(0xe6)), unchecked(
				(int)(0x8c)), unchecked((int)(0xff)) };
			NAMES["lavender"] = new int[] { unchecked((int)(0xe6)), unchecked((int)(0xe6)), unchecked(
				(int)(0xfa)), unchecked((int)(0xff)) };
			NAMES["lavenderblush"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xf0
				)), unchecked((int)(0xf5)), unchecked((int)(0xff)) };
			NAMES["lawngreen"] = new int[] { 0x7c, unchecked((int)(0xfc)), 0x00, unchecked((int
				)(0xff)) };
			NAMES["lemonchiffon"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xfa)
				), unchecked((int)(0xcd)), unchecked((int)(0xff)) };
			NAMES["lightblue"] = new int[] { 0xad, 0xd8, unchecked((int)(0xe6)), unchecked((int
				)(0xff)) };
			NAMES["lightcoral"] = new int[] { unchecked((int)(0xf0)), unchecked((int)(0x80)), 
				unchecked((int)(0x80)), unchecked((int)(0xff)) };
			NAMES["lightcyan"] = new int[] { unchecked((int)(0xe0)), unchecked((int)(0xff)), 
				unchecked((int)(0xff)), unchecked((int)(0xff)) };
			NAMES["lightgoldenrodyellow"] = new int[] { unchecked((int)(0xfa)), unchecked((int
				)(0xfa)), 0xd2, unchecked((int)(0xff)) };
			NAMES["lightgreen"] = new int[] { 0x90, unchecked((int)(0xee)), 0x90, unchecked((
				int)(0xff)) };
			NAMES["lightgrey"] = new int[] { 0xd3, 0xd3, 0xd3, unchecked((int)(0xff)) };
			NAMES["lightpink"] = new int[] { unchecked((int)(0xff)), 0xb6, unchecked((int)(0xc1
				)), unchecked((int)(0xff)) };
			NAMES["lightsalmon"] = new int[] { unchecked((int)(0xff)), 0xa0, 0x7a, unchecked(
				(int)(0xff)) };
			NAMES["lightseagreen"] = new int[] { 0x20, 0xb2, 0xaa, unchecked((int)(0xff)) };
			NAMES["lightskyblue"] = new int[] { unchecked((int)(0x87)), unchecked((int)(0xce)
				), unchecked((int)(0xfa)), unchecked((int)(0xff)) };
			NAMES["lightslategray"] = new int[] { 0x77, unchecked((int)(0x88)), 0x99, unchecked(
				(int)(0xff)) };
			NAMES["lightsteelblue"] = new int[] { 0xb0, unchecked((int)(0xc4)), 0xde, unchecked(
				(int)(0xff)) };
			NAMES["lightyellow"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xff))
				, unchecked((int)(0xe0)), unchecked((int)(0xff)) };
			NAMES["lime"] = new int[] { 0x00, unchecked((int)(0xff)), 0x00, unchecked((int)(0xff
				)) };
			NAMES["limegreen"] = new int[] { 0x32, unchecked((int)(0xcd)), 0x32, unchecked((int
				)(0xff)) };
			NAMES["linen"] = new int[] { unchecked((int)(0xfa)), unchecked((int)(0xf0)), unchecked(
				(int)(0xe6)), unchecked((int)(0xff)) };
			NAMES["magenta"] = new int[] { unchecked((int)(0xff)), 0x00, unchecked((int)(0xff
				)), unchecked((int)(0xff)) };
			NAMES["maroon"] = new int[] { unchecked((int)(0x80)), 0x00, 0x00, unchecked((int)
				(0xff)) };
			NAMES["mediumaquamarine"] = new int[] { 0x66, unchecked((int)(0xcd)), 0xaa, unchecked(
				(int)(0xff)) };
			NAMES["mediumblue"] = new int[] { 0x00, 0x00, unchecked((int)(0xcd)), unchecked((
				int)(0xff)) };
			NAMES["mediumorchid"] = new int[] { 0xba, 0x55, 0xd3, unchecked((int)(0xff)) };
			NAMES["mediumpurple"] = new int[] { 0x93, 0x70, 0xdb, unchecked((int)(0xff)) };
			NAMES["mediumseagreen"] = new int[] { 0x3c, 0xb3, 0x71, unchecked((int)(0xff)) };
			NAMES["mediumslateblue"] = new int[] { 0x7b, 0x68, unchecked((int)(0xee)), unchecked(
				(int)(0xff)) };
			NAMES["mediumspringgreen"] = new int[] { 0x00, unchecked((int)(0xfa)), 0x9a, unchecked(
				(int)(0xff)) };
			NAMES["mediumturquoise"] = new int[] { 0x48, 0xd1, unchecked((int)(0xcc)), unchecked(
				(int)(0xff)) };
			NAMES["mediumvioletred"] = new int[] { unchecked((int)(0xc7)), 0x15, unchecked((int
				)(0x85)), unchecked((int)(0xff)) };
			NAMES["midnightblue"] = new int[] { 0x19, 0x19, 0x70, unchecked((int)(0xff)) };
			NAMES["mintcream"] = new int[] { unchecked((int)(0xf5)), unchecked((int)(0xff)), 
				unchecked((int)(0xfa)), unchecked((int)(0xff)) };
			NAMES["mistyrose"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xe4)), 
				unchecked((int)(0xe1)), unchecked((int)(0xff)) };
			NAMES["moccasin"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xe4)), 0xb5
				, unchecked((int)(0xff)) };
			NAMES["navajowhite"] = new int[] { unchecked((int)(0xff)), 0xde, 0xad, unchecked(
				(int)(0xff)) };
			NAMES["navy"] = new int[] { 0x00, 0x00, unchecked((int)(0x80)), unchecked((int)(0xff
				)) };
			NAMES["oldlace"] = new int[] { unchecked((int)(0xfd)), unchecked((int)(0xf5)), unchecked(
				(int)(0xe6)), unchecked((int)(0xff)) };
			NAMES["olive"] = new int[] { unchecked((int)(0x80)), unchecked((int)(0x80)), 0x00
				, unchecked((int)(0xff)) };
			NAMES["olivedrab"] = new int[] { 0x6b, unchecked((int)(0x8e)), 0x23, unchecked((int
				)(0xff)) };
			NAMES["orange"] = new int[] { unchecked((int)(0xff)), 0xa5, 0x00, unchecked((int)
				(0xff)) };
			NAMES["orangered"] = new int[] { unchecked((int)(0xff)), 0x45, 0x00, unchecked((int
				)(0xff)) };
			NAMES["orchid"] = new int[] { 0xda, 0x70, 0xd6, unchecked((int)(0xff)) };
			NAMES["palegoldenrod"] = new int[] { unchecked((int)(0xee)), unchecked((int)(0xe8
				)), 0xaa, unchecked((int)(0xff)) };
			NAMES["palegreen"] = new int[] { 0x98, unchecked((int)(0xfb)), 0x98, unchecked((int
				)(0xff)) };
			NAMES["paleturquoise"] = new int[] { 0xaf, unchecked((int)(0xee)), unchecked((int
				)(0xee)), unchecked((int)(0xff)) };
			NAMES["palevioletred"] = new int[] { 0xdb, 0x70, 0x93, unchecked((int)(0xff)) };
			NAMES["papayawhip"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xef)), 
				0xd5, unchecked((int)(0xff)) };
			NAMES["peachpuff"] = new int[] { unchecked((int)(0xff)), 0xda, 0xb9, unchecked((int
				)(0xff)) };
			NAMES["peru"] = new int[] { unchecked((int)(0xcd)), unchecked((int)(0x85)), 0x3f, 
				unchecked((int)(0xff)) };
			NAMES["pink"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xc0)), unchecked(
				(int)(0xcb)), unchecked((int)(0xff)) };
			NAMES["plum"] = new int[] { 0xdd, 0xa0, 0xdd, unchecked((int)(0xff)) };
			NAMES["powderblue"] = new int[] { 0xb0, unchecked((int)(0xe0)), unchecked((int)(0xe6
				)), unchecked((int)(0xff)) };
			NAMES["purple"] = new int[] { unchecked((int)(0x80)), 0x00, unchecked((int)(0x80)
				), unchecked((int)(0xff)) };
			NAMES["red"] = new int[] { unchecked((int)(0xff)), 0x00, 0x00, unchecked((int)(0xff
				)) };
			NAMES["rosybrown"] = new int[] { 0xbc, unchecked((int)(0x8f)), unchecked((int)(0x8f
				)), unchecked((int)(0xff)) };
			NAMES["royalblue"] = new int[] { 0x41, 0x69, unchecked((int)(0xe1)), unchecked((int
				)(0xff)) };
			NAMES["saddlebrown"] = new int[] { unchecked((int)(0x8b)), 0x45, 0x13, unchecked(
				(int)(0xff)) };
			NAMES["salmon"] = new int[] { unchecked((int)(0xfa)), unchecked((int)(0x80)), 0x72
				, unchecked((int)(0xff)) };
			NAMES["sandybrown"] = new int[] { unchecked((int)(0xf4)), 0xa4, 0x60, unchecked((
				int)(0xff)) };
			NAMES["seagreen"] = new int[] { 0x2e, unchecked((int)(0x8b)), 0x57, unchecked((int
				)(0xff)) };
			NAMES["seashell"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xf5)), unchecked(
				(int)(0xee)), unchecked((int)(0xff)) };
			NAMES["sienna"] = new int[] { 0xa0, 0x52, 0x2d, unchecked((int)(0xff)) };
			NAMES["silver"] = new int[] { unchecked((int)(0xc0)), unchecked((int)(0xc0)), unchecked(
				(int)(0xc0)), unchecked((int)(0xff)) };
			NAMES["skyblue"] = new int[] { unchecked((int)(0x87)), unchecked((int)(0xce)), unchecked(
				(int)(0xeb)), unchecked((int)(0xff)) };
			NAMES["slateblue"] = new int[] { 0x6a, 0x5a, unchecked((int)(0xcd)), unchecked((int
				)(0xff)) };
			NAMES["slategray"] = new int[] { 0x70, unchecked((int)(0x80)), 0x90, unchecked((int
				)(0xff)) };
			NAMES["snow"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xfa)), unchecked(
				(int)(0xfa)), unchecked((int)(0xff)) };
			NAMES["springgreen"] = new int[] { 0x00, unchecked((int)(0xff)), 0x7f, unchecked(
				(int)(0xff)) };
			NAMES["steelblue"] = new int[] { 0x46, unchecked((int)(0x82)), 0xb4, unchecked((int
				)(0xff)) };
			NAMES["tan"] = new int[] { 0xd2, 0xb4, unchecked((int)(0x8c)), unchecked((int)(0xff
				)) };
			NAMES["teal"] = new int[] { 0x00, unchecked((int)(0x80)), unchecked((int)(0x80)), 
				unchecked((int)(0xff)) };
			NAMES["thistle"] = new int[] { 0xd8, 0xbf, 0xd8, unchecked((int)(0xff)) };
			NAMES["tomato"] = new int[] { unchecked((int)(0xff)), 0x63, 0x47, unchecked((int)
				(0xff)) };
			NAMES["transparent"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xff))
				, unchecked((int)(0xff)), 0x00 };
			NAMES["turquoise"] = new int[] { 0x40, unchecked((int)(0xe0)), 0xd0, unchecked((int
				)(0xff)) };
			NAMES["violet"] = new int[] { unchecked((int)(0xee)), unchecked((int)(0x82)), unchecked(
				(int)(0xee)), unchecked((int)(0xff)) };
			NAMES["wheat"] = new int[] { unchecked((int)(0xf5)), 0xde, 0xb3, unchecked((int)(
				0xff)) };
			NAMES["white"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xff)), unchecked(
				(int)(0xff)), unchecked((int)(0xff)) };
			NAMES["whitesmoke"] = new int[] { unchecked((int)(0xf5)), unchecked((int)(0xf5)), 
				unchecked((int)(0xf5)), unchecked((int)(0xff)) };
			NAMES["yellow"] = new int[] { unchecked((int)(0xff)), unchecked((int)(0xff)), 0x00
				, unchecked((int)(0xff)) };
			NAMES["yellowgreen"] = new int[] { 0x9a, unchecked((int)(0xcd)), 0x32, unchecked(
				(int)(0xff)) };
		}

		/// <summary>
		/// A web color string without the leading # will be 3 or 6 characters long
		/// and all those characters will be hex digits.
		/// </summary>
		/// <remarks>
		/// A web color string without the leading # will be 3 or 6 characters long
		/// and all those characters will be hex digits. NOTE: colStr must be all
		/// lower case or the current hex letter test will fail.
		/// </remarks>
		/// <param name="colStr">
		/// A non-null, lower case string that might describe an RGB color
		/// in hex.
		/// </param>
		/// <returns>Is this a web color hex string without the leading #?</returns>
		private static bool MissingHashColorFormat(String colStr)
		{
			int len = colStr.Length;
			if (len == 3 || len == 6)
			{
				// and it just contains hex chars 0-9, a-f, A-F
				String match = "[0-9a-f]{" + len + "}";
				return colStr.Matches(match);
			}
			return false;
		}

		/// <summary>Gives you a BaseColor based on a name.</summary>
		/// <param name="name">
		/// a name such as black, violet, cornflowerblue or #RGB or
		/// #RRGGBB or RGB or RRGGBB or rgb(R,G,B)
		/// </param>
		/// <returns>the corresponding BaseColor object. Never returns null.</returns>
		/// <exception cref="System.ArgumentException">if the String isn't a know representation of a color.
		/// 	</exception>
		public static DeviceRgb GetRGBColor(String name)
		{
			int[] color = new int[] { 0, 0, 0, 255 };
			String colorName = name.ToLower();
			bool colorStrWithoutHash = MissingHashColorFormat(colorName);
			if (colorName.StartsWith("#") || colorStrWithoutHash)
			{
				if (!colorStrWithoutHash)
				{
					// lop off the # to unify hex parsing.
					colorName = colorName.Substring(1);
				}
				if (colorName.Length == 3)
				{
					String red = colorName.JSubstring(0, 1);
					color[0] = System.Convert.ToInt32(red + red, 16);
					String green = colorName.JSubstring(1, 2);
					color[1] = System.Convert.ToInt32(green + green, 16);
					String blue = colorName.Substring(2);
					color[2] = System.Convert.ToInt32(blue + blue, 16);
					return new DeviceRgb(color[0], color[1], color[2]);
				}
				if (colorName.Length == 6)
				{
					color[0] = System.Convert.ToInt32(colorName.JSubstring(0, 2), 16);
					color[1] = System.Convert.ToInt32(colorName.JSubstring(2, 4), 16);
					color[2] = System.Convert.ToInt32(colorName.Substring(4), 16);
					return new DeviceRgb(color[0], color[1], color[2]);
				}
				throw new PdfException(PdfException.UnknownColorFormatMustBeRGBorRRGGBB);
			}
			if (colorName.StartsWith("rgb("))
			{
				String delim = "rgb(), \t\r\n\f";
				StringTokenizer tok = new StringTokenizer(colorName, delim);
				for (int k = 0; k < 3; ++k)
				{
					if (tok.HasMoreTokens())
					{
						color[k] = GetRGBChannelValue(tok.NextToken());
						color[k] = Math.Max(0, color[k]);
						color[k] = Math.Min(255, color[k]);
					}
				}
				return new DeviceRgb(color[0], color[1], color[2]);
			}
			if (colorName.StartsWith("rgba("))
			{
				String delim = "rgba(), \t\r\n\f";
				StringTokenizer tok = new StringTokenizer(colorName, delim);
				for (int k = 0; k < 3; ++k)
				{
					if (tok.HasMoreTokens())
					{
						color[k] = GetRGBChannelValue(tok.NextToken());
						color[k] = Math.Max(0, color[k]);
						color[k] = Math.Min(255, color[k]);
					}
				}
				return new DeviceRgb(color[0], color[1], color[2]);
			}
			if (!NAMES.ContainsKey(colorName))
			{
				throw new PdfException(PdfException.ColorNotFound).SetMessageParams(colorName);
			}
			color = NAMES[colorName];
			return new DeviceRgb(color[0], color[1], color[2]);
		}

		private static int GetRGBChannelValue(String rgbChannel)
		{
			if (rgbChannel.EndsWith("%"))
			{
				return System.Convert.ToInt32(rgbChannel.JSubstring(0, rgbChannel.Length - 1)) * 
					255 / 100;
			}
			else
			{
				return System.Convert.ToInt32(rgbChannel);
			}
		}
	}
}
