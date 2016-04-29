/*
$Id: b6cd09a03ccd873499dfcac2d7bcbf2b62e29c36 $

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
using com.itextpdf.kernel.geom;
using com.itextpdf.kernel.pdf.canvas;

namespace com.itextpdf.kernel.pdf.canvas.parser.clipper
{
	/// <summary>
	/// This class contains variety of methods allowing to convert iText
	/// abstractions into the abstractions of the Clipper library and vise versa.
	/// </summary>
	/// <remarks>
	/// This class contains variety of methods allowing to convert iText
	/// abstractions into the abstractions of the Clipper library and vise versa.
	/// <p>
	/// For example:
	/// <ul>
	/// <li>
	/// <see cref="PolyTree"/>
	/// to
	/// <see cref="com.itextpdf.kernel.geom.Path"/>
	/// </li>
	/// <li>
	/// <see cref="com.itextpdf.kernel.geom.Point"/>
	/// to
	/// <see cref="IntPoint"/>
	/// </li>
	/// <li>
	/// <see cref="IntPoint"/>
	/// to
	/// <see cref="com.itextpdf.kernel.geom.Point"/>
	/// </li>
	/// </ul>
	/// </p>
	/// </remarks>
	public class ClipperBridge
	{
		/// <summary>
		/// Since the clipper library uses integer coordinates, we should convert
		/// our floating point numbers into fixed point numbers by multiplying by
		/// this coefficient.
		/// </summary>
		/// <remarks>
		/// Since the clipper library uses integer coordinates, we should convert
		/// our floating point numbers into fixed point numbers by multiplying by
		/// this coefficient. Vary it to adjust the preciseness of the calculations.
		/// </remarks>
		public static double floatMultiplier = Math.Pow(10, 14);

		/// <summary>
		/// Converts Clipper library
		/// <see cref="PolyTree"/>
		/// abstraction into iText
		/// <see cref="com.itextpdf.kernel.geom.Path"/>
		/// object.
		/// </summary>
		public static com.itextpdf.kernel.geom.Path ConvertToPath(PolyTree result)
		{
			com.itextpdf.kernel.geom.Path path = new com.itextpdf.kernel.geom.Path();
			PolyNode node = result.GetFirst();
			while (node != null)
			{
				AddContour(path, node.Contour, !node.IsOpen);
				node = node.GetNext();
			}
			return path;
		}

		/// <summary>
		/// Adds iText
		/// <see cref="List<IntPoint>"/>
		/// to the given
		/// <see cref="Clipper"/>
		/// object.
		/// </summary>
		/// <param name="clipper">
		/// The
		/// <see cref="Clipper"/>
		/// object.
		/// </param>
		/// <param name="path">
		/// The
		/// <see cref="com.itextpdf.kernel.geom.Path"/>
		/// object to be added to the
		/// <see cref="Clipper"/>
		/// .
		/// </param>
		/// <param name="polyType">
		/// See
		/// <see cref="PolyType"/>
		/// .
		/// </param>
		public static void AddPath(Clipper clipper, com.itextpdf.kernel.geom.Path path, PolyType
			 polyType)
		{
			foreach (Subpath subpath in path.GetSubpaths())
			{
				if (!subpath.IsSinglePointClosed() && !subpath.IsSinglePointOpen())
				{
					IList<Point> linearApproxPoints = subpath.GetPiecewiseLinearApproximation();
					clipper.AddPath(new List<IntPoint>(ConvertToLongPoints(linearApproxPoints)), polyType
						, subpath.IsClosed());
				}
			}
		}

		/// <summary>
		/// Adds all iText
		/// <see cref="com.itextpdf.kernel.geom.Subpath"/>
		/// s of the iText
		/// <see cref="List<IntPoint>"/>
		/// to the
		/// <see cref="ClipperOffset"/>
		/// object with one
		/// note: it doesn't add degenerate subpaths.
		/// </summary>
		/// <returns>
		/// 
		/// <see cref="System.Collections.IList{E}"/>
		/// consisting of all degenerate iText
		/// <see cref="com.itextpdf.kernel.geom.Subpath"/>
		/// s of the path.
		/// </returns>
		public static IList<Subpath> AddPath(ClipperOffset offset, com.itextpdf.kernel.geom.Path
			 path, JoinType joinType, EndType endType)
		{
			IList<Subpath> degenerateSubpaths = new List<Subpath>();
			foreach (Subpath subpath in path.GetSubpaths())
			{
				if (subpath.IsDegenerate())
				{
					degenerateSubpaths.Add(subpath);
					continue;
				}
				if (!subpath.IsSinglePointClosed() && !subpath.IsSinglePointOpen())
				{
					EndType et;
					if (subpath.IsClosed())
					{
						// Offsetting is never used for path being filled
						et = EndType.CLOSED_LINE;
					}
					else
					{
						et = endType;
					}
					IList<Point> linearApproxPoints = subpath.GetPiecewiseLinearApproximation();
					offset.AddPath((List<IntPoint>)ConvertToLongPoints(linearApproxPoints), joinType, 
						et);
				}
			}
			return degenerateSubpaths;
		}

		/// <summary>
		/// Converts list of
		/// <see cref="IntPoint"/>
		/// objects into list of
		/// <see cref="com.itextpdf.kernel.geom.Point"/>
		/// objects.
		/// </summary>
		public static IList<Point> ConvertToFloatPoints(IList<IntPoint> points)
		{
			IList<Point> convertedPoints = new List<Point>(points.Count);
			foreach (IntPoint point in points)
			{
				convertedPoints.Add(new Point(point.X / floatMultiplier, point.Y / floatMultiplier
					));
			}
			return convertedPoints;
		}

		/// <summary>
		/// Converts list of
		/// <see cref="com.itextpdf.kernel.geom.Point"/>
		/// objects into list of
		/// <see cref="IntPoint"/>
		/// objects.
		/// </summary>
		public static IList<IntPoint> ConvertToLongPoints(IList<Point> points)
		{
			IList<IntPoint> convertedPoints = new List<IntPoint>(points.Count);
			foreach (Point point in points)
			{
				convertedPoints.Add(new IntPoint(floatMultiplier * point.GetX(), floatMultiplier 
					* point.GetY()));
			}
			return convertedPoints;
		}

		/// <summary>
		/// Converts iText line join style constant into the corresponding constant
		/// of the Clipper library.
		/// </summary>
		/// <param name="lineJoinStyle">
		/// iText line join style constant. See
		/// <see cref="com.itextpdf.kernel.pdf.canvas.PdfCanvasConstants"/>
		/// </param>
		/// <returns>Clipper line join style constant.</returns>
		public static JoinType GetJoinType(int lineJoinStyle)
		{
			switch (lineJoinStyle)
			{
				case PdfCanvasConstants.LineJoinStyle.BEVEL:
				{
					return JoinType.BEVEL;
				}

				case PdfCanvasConstants.LineJoinStyle.MITER:
				{
					return JoinType.MITER;
				}
			}
			return JoinType.ROUND;
		}

		/// <summary>
		/// Converts iText line cap style constant into the corresponding constant
		/// of the Clipper library.
		/// </summary>
		/// <param name="lineCapStyle">
		/// iText line cap style constant. See
		/// <see cref="com.itextpdf.kernel.pdf.canvas.PdfCanvasConstants"/>
		/// </param>
		/// <returns>Clipper line cap (end type) style constant.</returns>
		public static EndType GetEndType(int lineCapStyle)
		{
			switch (lineCapStyle)
			{
				case PdfCanvasConstants.LineCapStyle.BUTT:
				{
					return EndType.OPEN_BUTT;
				}

				case PdfCanvasConstants.LineCapStyle.PROJECTING_SQUARE:
				{
					return EndType.OPEN_SQUARE;
				}
			}
			return EndType.OPEN_ROUND;
		}

		/// <summary>
		/// Converts iText filling rule constant into the corresponding constant
		/// of the Clipper library .
		/// </summary>
		/// <param name="fillingRule">
		/// Either
		/// <see cref="com.itextpdf.kernel.pdf.canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING
		/// 	"/>
		/// or
		/// <see cref="com.itextpdf.kernel.pdf.canvas.PdfCanvasConstants.FillingRule.EVEN_ODD
		/// 	"/>
		/// .
		/// </param>
		/// <returns>Clipper fill type constant.</returns>
		public static PolyFillType GetFillType(int fillingRule)
		{
			PolyFillType fillType = PolyFillType.NON_ZERO;
			if (fillingRule == PdfCanvasConstants.FillingRule.EVEN_ODD)
			{
				fillType = PolyFillType.EVEN_ODD;
			}
			return fillType;
		}

		public static void AddContour(com.itextpdf.kernel.geom.Path path, IList<IntPoint>
			 contour, bool close)
		{
			IList<Point> floatContour = ConvertToFloatPoints(contour);
			IEnumerator<Point> iter = floatContour.GetEnumerator();
			Point point = iter.Current;
			path.MoveTo((float)point.GetX(), (float)point.GetY());
			while (iter.MoveNext())
			{
				point = iter.Current;
				path.LineTo((float)point.GetX(), (float)point.GetY());
			}
			if (close)
			{
				path.CloseSubpath();
			}
		}

		public static void AddRectToClipper(Clipper clipper, Point[] rectVertices, PolyType
			 polyType)
		{
			clipper.AddPath(new List<IntPoint>(ConvertToLongPoints(new List<Point>(com.itextpdf.io.util.JavaUtil.ArraysAsList
				(rectVertices)))), polyType, true);
		}
	}
}
