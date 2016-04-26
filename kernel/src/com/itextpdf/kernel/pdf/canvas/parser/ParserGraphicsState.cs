/*
$Id: 3167623e7381b28b7370099fb4b8ef2e68febfcd $

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
using com.itextpdf.kernel.pdf.canvas.parser.clipper;

namespace com.itextpdf.kernel.pdf.canvas.parser
{
	/// <summary>
	/// Internal class which is essentially a
	/// <see cref="com.itextpdf.kernel.pdf.canvas.CanvasGraphicsState"/>
	/// which supports tracking of
	/// clipping path state and changes.
	/// </summary>
	public class ParserGraphicsState : CanvasGraphicsState
	{
		private com.itextpdf.kernel.geom.Path clippingPath;

		/// <summary>Internal empty & default constructor.</summary>
		internal ParserGraphicsState()
		{
		}

		/// <summary>Copy constructor.</summary>
		/// <param name="source">the Graphics State to copy from</param>
		internal ParserGraphicsState(com.itextpdf.kernel.pdf.canvas.parser.ParserGraphicsState
			 source)
			: base(source)
		{
			// NOTE: From the spec default value of this field should be the boundary of the entire imageable portion of the output page.
			if (source.clippingPath != null)
			{
				clippingPath = new com.itextpdf.kernel.geom.Path(source.clippingPath);
			}
		}

		/// <summary>Sets the current clipping path to the specified path.</summary>
		/// <remarks>
		/// Sets the current clipping path to the specified path.
		/// <br/>
		/// <strong>Note:</strong>This method doesn't modify existing clipping path,
		/// it simply replaces it with the new one instead.
		/// </remarks>
		/// <param name="clippingPath">New clipping path.</param>
		public virtual void SetClippingPath(com.itextpdf.kernel.geom.Path clippingPath)
		{
			com.itextpdf.kernel.geom.Path pathCopy = new com.itextpdf.kernel.geom.Path(clippingPath
				);
			pathCopy.CloseAllSubpaths();
			this.clippingPath = pathCopy;
		}

		public override void UpdateCtm(Matrix newCtm)
		{
			base.UpdateCtm(newCtm);
			if (clippingPath != null)
			{
				TransformClippingPath(newCtm);
			}
		}

		/// <summary>Intersects the current clipping path with the given path.</summary>
		/// <remarks>
		/// Intersects the current clipping path with the given path.
		/// <br/>
		/// <strong>Note:</strong> Coordinates of the given path should be in
		/// the transformed user space.
		/// </remarks>
		/// <param name="path">The path to be intersected with the current clipping path.</param>
		/// <param name="fillingRule">
		/// The filling rule which should be applied to the given path.
		/// It should be either
		/// <see cref="com.itextpdf.kernel.pdf.canvas.PdfCanvasConstants.FillingRule.EVEN_ODD
		/// 	"/>
		/// or
		/// <see cref="com.itextpdf.kernel.pdf.canvas.PdfCanvasConstants.FillingRule.NONZERO_WINDING
		/// 	"/>
		/// </param>
		public virtual void Clip(com.itextpdf.kernel.geom.Path path, int fillingRule)
		{
			if (clippingPath == null || clippingPath.IsEmpty())
			{
				return;
			}
			com.itextpdf.kernel.geom.Path pathCopy = new com.itextpdf.kernel.geom.Path(path);
			pathCopy.CloseAllSubpaths();
			IClipper clipper = new DefaultClipper();
			ClipperBridge.AddPath(clipper, clippingPath, IClipper.PolyType.SUBJECT);
			ClipperBridge.AddPath(clipper, pathCopy, IClipper.PolyType.CLIP);
			PolyTree resultTree = new PolyTree();
			clipper.Execute(IClipper.ClipType.INTERSECTION, resultTree, IClipper.PolyFillType
				.NON_ZERO, ClipperBridge.GetFillType(fillingRule));
			clippingPath = ClipperBridge.ConvertToPath(resultTree);
		}

		/// <summary>Getter for the current clipping path.</summary>
		/// <remarks>
		/// Getter for the current clipping path.
		/// <br/>
		/// <strong>Note:</strong> The returned clipping path is in the transformed user space, so
		/// if you want to get it in default user space, apply transformation matrix (
		/// <see cref="com.itextpdf.kernel.pdf.canvas.CanvasGraphicsState.GetCtm()"/>
		/// ).
		/// </remarks>
		/// <returns>The current clipping path.</returns>
		public virtual com.itextpdf.kernel.geom.Path GetClippingPath()
		{
			return clippingPath;
		}

		private void TransformClippingPath(Matrix newCtm)
		{
			com.itextpdf.kernel.geom.Path path = new com.itextpdf.kernel.geom.Path();
			foreach (Subpath subpath in clippingPath.GetSubpaths())
			{
				Subpath transformedSubpath = TransformSubpath(subpath, newCtm);
				path.AddSubpath(transformedSubpath);
			}
			clippingPath = path;
		}

		private Subpath TransformSubpath(Subpath subpath, Matrix newCtm)
		{
			Subpath newSubpath = new Subpath();
			newSubpath.SetClosed(subpath.IsClosed());
			foreach (IShape segment in subpath.GetSegments())
			{
				IShape transformedSegment = TransformSegment(segment, newCtm);
				newSubpath.AddSegment(transformedSegment);
			}
			return newSubpath;
		}

		private IShape TransformSegment(IShape segment, Matrix newCtm)
		{
			IShape newSegment;
			IList<Point> segBasePts = segment.GetBasePoints();
			Point[] transformedPoints = TransformPoints(newCtm, segBasePts.ToArray(new Point[
				segBasePts.Count]));
			if (segment is BezierCurve)
			{
				newSegment = new BezierCurve(com.itextpdf.io.util.JavaUtil.ArraysAsList(transformedPoints
					));
			}
			else
			{
				newSegment = new Line(transformedPoints[0], transformedPoints[1]);
			}
			return newSegment;
		}

		private Point[] TransformPoints(Matrix transformationMatrix, params Point[] points
			)
		{
			try
			{
				AffineTransform t = new AffineTransform(transformationMatrix.Get(Matrix.I11), transformationMatrix
					.Get(Matrix.I12), transformationMatrix.Get(Matrix.I21), transformationMatrix.Get
					(Matrix.I22), transformationMatrix.Get(Matrix.I31), transformationMatrix.Get(Matrix
					.I32));
				t = t.CreateInverse();
				Point[] transformed = new Point[points.Length];
				t.Transform(points, 0, transformed, 0, points.Length);
				return transformed;
			}
			catch (NoninvertibleTransformException e)
			{
				throw new Exception(e);
			}
		}
	}
}
