/*
$Id: 2b7aabd502739397ab634690460b360304934c25 $

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
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Pdf.Colorspace
{
	public abstract class PdfPattern : PdfObjectWrapper<PdfDictionary>
	{
		private const long serialVersionUID = -6771280634868639993L;

		protected internal PdfPattern(PdfDictionary pdfObject)
			: base(pdfObject)
		{
		}

		public static iTextSharp.Kernel.Pdf.Colorspace.PdfPattern GetPatternInstance(PdfDictionary
			 pdfObject)
		{
			PdfNumber type = pdfObject.GetAsNumber(PdfName.PatternType);
			if (type.IntValue() == 1 && pdfObject is PdfStream)
			{
				return new PdfPattern.Tiling((PdfStream)pdfObject);
			}
			else
			{
				if (type.IntValue() == 2)
				{
					return new PdfPattern.Shading(pdfObject);
				}
			}
			throw new ArgumentException("pdfObject");
		}

		public virtual PdfArray GetMatrix()
		{
			return GetPdfObject().GetAsArray(PdfName.Matrix);
		}

		public virtual void SetMatrix(PdfArray matrix)
		{
			GetPdfObject().Put(PdfName.Matrix, matrix);
			SetModified();
		}

		protected internal override bool IsWrappedObjectMustBeIndirect()
		{
			return true;
		}

		public class Tiling : PdfPattern
		{
			private const long serialVersionUID = 1450379837955897673L;

			private PdfResources resources = null;

			public class PaintType
			{
				public const int COLORED = 1;

				public const int UNCOLORED = 2;
			}

			public class TilingType
			{
				public const int CONSTANT_SPACING = 1;

				public const int NO_DISTORTION = 2;

				public const int CONSTANT_SPACING_AND_FASTER_TILING = 3;
			}

			public Tiling(PdfStream pdfObject)
				: base(pdfObject)
			{
			}

			public Tiling(float width, float height)
				: this(width, height, true)
			{
			}

			public Tiling(float width, float height, bool colored)
				: this(new Rectangle(width, height), colored)
			{
			}

			public Tiling(Rectangle bbox)
				: this(bbox, true)
			{
			}

			public Tiling(Rectangle bbox, bool colored)
				: this(bbox, bbox.GetWidth(), bbox.GetHeight(), colored)
			{
			}

			public Tiling(float width, float height, float xStep, float yStep)
				: this(width, height, xStep, yStep, true)
			{
			}

			public Tiling(float width, float height, float xStep, float yStep, bool colored)
				: this(new Rectangle(width, height), xStep, yStep, colored)
			{
			}

			public Tiling(Rectangle bbox, float xStep, float yStep)
				: this(bbox, xStep, yStep, true)
			{
			}

			public Tiling(Rectangle bbox, float xStep, float yStep, bool colored)
				: base(new PdfStream())
			{
				GetPdfObject().Put(PdfName.Type, PdfName.Pattern);
				GetPdfObject().Put(PdfName.PatternType, new PdfNumber(1));
				GetPdfObject().Put(PdfName.PaintType, new PdfNumber(colored ? PdfPattern.Tiling.PaintType
					.COLORED : PdfPattern.Tiling.PaintType.UNCOLORED));
				GetPdfObject().Put(PdfName.TilingType, new PdfNumber(PdfPattern.Tiling.TilingType
					.CONSTANT_SPACING));
				GetPdfObject().Put(PdfName.BBox, new PdfArray(bbox));
				GetPdfObject().Put(PdfName.XStep, new PdfNumber(xStep));
				GetPdfObject().Put(PdfName.YStep, new PdfNumber(yStep));
				resources = new PdfResources();
				GetPdfObject().Put(PdfName.Resources, resources.GetPdfObject());
			}

			public virtual bool IsColored()
			{
				return GetPdfObject().GetAsNumber(PdfName.PaintType).IntValue() == PdfPattern.Tiling.PaintType
					.COLORED;
			}

			public virtual void SetColored(bool colored)
			{
				GetPdfObject().Put(PdfName.PaintType, new PdfNumber(colored ? PdfPattern.Tiling.PaintType
					.COLORED : PdfPattern.Tiling.PaintType.UNCOLORED));
				SetModified();
			}

			public virtual int GetTilingType()
			{
				return GetPdfObject().GetAsNumber(PdfName.TilingType).IntValue();
			}

			public virtual void SetTilingType(int tilingType)
			{
				if (tilingType != PdfPattern.Tiling.TilingType.CONSTANT_SPACING && tilingType != 
					PdfPattern.Tiling.TilingType.NO_DISTORTION && tilingType != PdfPattern.Tiling.TilingType
					.CONSTANT_SPACING_AND_FASTER_TILING)
				{
					throw new ArgumentException("tilingType");
				}
				GetPdfObject().Put(PdfName.TilingType, new PdfNumber(tilingType));
				SetModified();
			}

			public virtual Rectangle GetBBox()
			{
				return GetPdfObject().GetAsArray(PdfName.BBox).ToRectangle();
			}

			public virtual void SetBBox(Rectangle bbox)
			{
				GetPdfObject().Put(PdfName.BBox, new PdfArray(bbox));
				SetModified();
			}

			public virtual float GetXStep()
			{
				return GetPdfObject().GetAsNumber(PdfName.XStep).FloatValue();
			}

			public virtual void SetXStep(float xStep)
			{
				GetPdfObject().Put(PdfName.XStep, new PdfNumber(xStep));
				SetModified();
			}

			public virtual float GetYStep()
			{
				return GetPdfObject().GetAsNumber(PdfName.YStep).FloatValue();
			}

			public virtual void SetYStep(float yStep)
			{
				GetPdfObject().Put(PdfName.YStep, new PdfNumber(yStep));
				SetModified();
			}

			public virtual PdfResources GetResources()
			{
				if (this.resources == null)
				{
					PdfDictionary resources = GetPdfObject().GetAsDictionary(PdfName.Resources);
					if (resources == null)
					{
						resources = new PdfDictionary();
						GetPdfObject().Put(PdfName.Resources, resources);
					}
					this.resources = new PdfResources(resources);
				}
				return resources;
			}

			public override void Flush()
			{
				resources = null;
				base.Flush();
			}
		}

		public class Shading : PdfPattern
		{
			private const long serialVersionUID = -4289411438737403786L;

			public Shading(PdfDictionary pdfObject)
				: base(pdfObject)
			{
			}

			public Shading(PdfShading shading)
				: base(new PdfDictionary())
			{
				GetPdfObject().Put(PdfName.Type, PdfName.Pattern);
				GetPdfObject().Put(PdfName.PatternType, new PdfNumber(2));
				GetPdfObject().Put(PdfName.Shading, shading.GetPdfObject());
			}

			public virtual PdfDictionary GetShading()
			{
				return (PdfDictionary)GetPdfObject().Get(PdfName.Shading);
			}

			public virtual void SetShading(PdfShading shading)
			{
				GetPdfObject().Put(PdfName.Shading, shading.GetPdfObject());
				SetModified();
			}

			public virtual void SetShading(PdfDictionary shading)
			{
				GetPdfObject().Put(PdfName.Shading, shading);
				SetModified();
			}
		}
	}
}
