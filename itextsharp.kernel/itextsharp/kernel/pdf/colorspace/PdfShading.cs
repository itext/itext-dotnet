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
using iTextSharp.Kernel;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Function;

namespace iTextSharp.Kernel.Pdf.Colorspace
{
	public abstract class PdfShading : PdfObjectWrapper<PdfDictionary>
	{
		private class ShadingType
		{
			public const int FUNCTION_BASED = 1;

			public const int AXIAL = 2;

			public const int RADIAL = 3;

			public const int FREE_FORM_GOURAUD_SHADED_TRIANGLE_MESH = 4;

			public const int LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH = 5;

			public const int COONS_PATCH_MESH = 6;

			public const int TENSOR_PRODUCT_PATCH_MESH = 7;
		}

		public static PdfShading MakeShading(PdfDictionary shadingDictionary)
		{
			if (!shadingDictionary.ContainsKey(PdfName.ShadingType))
			{
				throw new PdfException(PdfException.UnexpectedShadingType);
			}
			PdfShading shading;
			switch (shadingDictionary.GetAsNumber(PdfName.ShadingType).IntValue())
			{
				case PdfShading.ShadingType.FUNCTION_BASED:
				{
					shading = new PdfShading.FunctionBased(shadingDictionary);
					break;
				}

				case PdfShading.ShadingType.AXIAL:
				{
					shading = new PdfShading.Axial(shadingDictionary);
					break;
				}

				case PdfShading.ShadingType.RADIAL:
				{
					shading = new PdfShading.Radial(shadingDictionary);
					break;
				}

				case PdfShading.ShadingType.FREE_FORM_GOURAUD_SHADED_TRIANGLE_MESH:
				{
					if (!shadingDictionary.IsStream())
					{
						throw new PdfException(PdfException.UnexpectedShadingType);
					}
					shading = new PdfShading.FreeFormGouraudShadedTriangleMesh((PdfStream)shadingDictionary
						);
					break;
				}

				case PdfShading.ShadingType.LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH:
				{
					if (!shadingDictionary.IsStream())
					{
						throw new PdfException(PdfException.UnexpectedShadingType);
					}
					shading = new PdfShading.LatticeFormGouraudShadedTriangleMesh((PdfStream)shadingDictionary
						);
					break;
				}

				case PdfShading.ShadingType.COONS_PATCH_MESH:
				{
					if (!shadingDictionary.IsStream())
					{
						throw new PdfException(PdfException.UnexpectedShadingType);
					}
					shading = new PdfShading.CoonsPatchMesh((PdfStream)shadingDictionary);
					break;
				}

				case PdfShading.ShadingType.TENSOR_PRODUCT_PATCH_MESH:
				{
					if (!shadingDictionary.IsStream())
					{
						throw new PdfException(PdfException.UnexpectedShadingType);
					}
					shading = new PdfShading.TensorProductPatchMesh((PdfStream)shadingDictionary);
					break;
				}

				default:
				{
					throw new PdfException(PdfException.UnexpectedShadingType);
				}
			}
			return shading;
		}

		protected internal PdfShading(PdfDictionary pdfObject)
			: base(pdfObject)
		{
		}

		protected internal PdfShading(PdfDictionary pdfObject, int type, PdfObject colorSpace
			)
			: base(pdfObject)
		{
			GetPdfObject().Put(PdfName.ShadingType, new PdfNumber(type));
			GetPdfObject().Put(PdfName.ColorSpace, colorSpace);
		}

		public virtual int GetShadingType()
		{
			return (int)GetPdfObject().GetAsInt(PdfName.ShadingType);
		}

		public virtual PdfObject GetColorSpace()
		{
			return GetPdfObject().Get(PdfName.ColorSpace);
		}

		public virtual PdfObject GetFunction()
		{
			return GetPdfObject().Get(PdfName.Function);
		}

		public virtual void SetFunction(PdfFunction function)
		{
			GetPdfObject().Put(PdfName.Function, function.GetPdfObject());
			SetModified();
		}

		public virtual void SetFunction(PdfFunction[] functions)
		{
			PdfArray arr = new PdfArray();
			foreach (PdfFunction func in functions)
			{
				arr.Add(func.GetPdfObject());
			}
			GetPdfObject().Put(PdfName.Function, arr);
			SetModified();
		}

		protected internal override bool IsWrappedObjectMustBeIndirect()
		{
			return true;
		}

		public class FunctionBased : PdfShading
		{
			public FunctionBased(PdfDictionary pdfObject)
				: base(pdfObject)
			{
			}

			public FunctionBased(PdfColorSpace colorSpace, PdfFunction function)
				: this(colorSpace.GetPdfObject(), function)
			{
			}

			public FunctionBased(PdfObject colorSpace, PdfFunction function)
				: base(new PdfDictionary(), PdfShading.ShadingType.FUNCTION_BASED, colorSpace)
			{
				SetFunction(function);
			}

			public virtual PdfArray GetDomain()
			{
				return GetPdfObject().GetAsArray(PdfName.Domain);
			}

			public virtual void SetDomain(float xmin, float xmax, float ymin, float ymax)
			{
				GetPdfObject().Put(PdfName.Domain, new PdfArray(new float[] { xmin, xmax, ymin, ymax
					 }));
				SetModified();
			}

			public virtual void SetDomain(PdfArray domain)
			{
				GetPdfObject().Put(PdfName.Domain, domain);
				SetModified();
			}

			public virtual float[] GetMatrix()
			{
				PdfArray matrix = GetPdfObject().GetAsArray(PdfName.Matrix);
				if (matrix == null)
				{
					return new float[] { 1, 0, 0, 1, 0, 0 };
				}
				float[] result = new float[6];
				for (int i = 0; i < 6; i++)
				{
					result[i] = matrix.GetAsNumber(i).FloatValue();
				}
				return result;
			}

			public virtual void SetMatrix(float[] matrix)
			{
				SetMatrix(new PdfArray(matrix));
			}

			public virtual void SetMatrix(PdfArray matrix)
			{
				GetPdfObject().Put(PdfName.Matrix, matrix);
				SetModified();
			}
		}

		public class Axial : PdfShading
		{
			public Axial(PdfDictionary pdfDictionary)
				: base(pdfDictionary)
			{
			}

			public Axial(PdfColorSpace cs, float x0, float y0, float[] color0, float x1, float
				 y1, float[] color1)
				: base(new PdfDictionary(), PdfShading.ShadingType.AXIAL, cs.GetPdfObject())
			{
				if (cs is PdfSpecialCs.Pattern)
				{
					throw new ArgumentException("colorSpace");
				}
				SetCoords(x0, y0, x1, y1);
				PdfFunction func = new PdfFunction.Type2(new PdfArray(new float[] { 0, 1 }), null
					, new PdfArray(color0), new PdfArray(color1), new PdfNumber(1));
				SetFunction(func);
			}

			public Axial(PdfColorSpace cs, float x0, float y0, float[] color0, float x1, float
				 y1, float[] color1, bool[] extend)
				: this(cs, x0, y0, color0, x1, y1, color1)
			{
				if (extend != null)
				{
					SetExtend(extend[0], extend[1]);
				}
			}

			public Axial(PdfColorSpace cs, PdfArray coords, PdfFunction function)
				: base(new PdfDictionary(), PdfShading.ShadingType.AXIAL, cs.GetPdfObject())
			{
				SetCoords(coords);
				SetFunction(function);
			}

			public virtual PdfArray GetCoords()
			{
				return GetPdfObject().GetAsArray(PdfName.Coords);
			}

			public virtual void SetCoords(float x0, float y0, float x1, float y1)
			{
				SetCoords(new PdfArray(new float[] { x0, y0, x1, y1 }));
			}

			public virtual void SetCoords(PdfArray coords)
			{
				GetPdfObject().Put(PdfName.Coords, coords);
				SetModified();
			}

			public virtual float[] GetDomain()
			{
				PdfArray domain = GetPdfObject().GetAsArray(PdfName.Domain);
				if (domain == null)
				{
					return new float[] { 0, 1 };
				}
				return new float[] { domain.GetAsNumber(0).FloatValue(), domain.GetAsNumber(1).FloatValue
					() };
			}

			public virtual void SetDomain(float t0, float t1)
			{
				GetPdfObject().Put(PdfName.Domain, new PdfArray(new float[] { t0, t1 }));
				SetModified();
			}

			public virtual bool[] GetExtend()
			{
				PdfArray extend = GetPdfObject().GetAsArray(PdfName.Extend);
				if (extend == null)
				{
					return new bool[] { true, true };
				}
				return new bool[] { extend.GetAsBoolean(0).GetValue(), extend.GetAsBoolean(1).GetValue
					() };
			}

			public virtual void SetExtend(bool extendStart, bool extendEnd)
			{
				GetPdfObject().Put(PdfName.Extend, new PdfArray(new bool[] { extendStart, extendEnd
					 }));
				SetModified();
			}
		}

		public class Radial : PdfShading
		{
			public Radial(PdfDictionary pdfDictionary)
				: base(pdfDictionary)
			{
			}

			public Radial(PdfColorSpace cs, float x0, float y0, float r0, float[] color0, float
				 x1, float y1, float r1, float[] color1)
				: base(new PdfDictionary(), PdfShading.ShadingType.RADIAL, cs.GetPdfObject())
			{
				SetCoords(x0, y0, r0, x1, y1, r1);
				PdfFunction func = new PdfFunction.Type2(new PdfArray(new float[] { 0, 1 }), null
					, new PdfArray(color0), new PdfArray(color1), new PdfNumber(1));
				SetFunction(func);
			}

			public Radial(PdfColorSpace cs, float x0, float y0, float r0, float[] color0, float
				 x1, float y1, float r1, float[] color1, bool[] extend)
				: this(cs, x0, y0, r0, color0, x1, y1, r1, color1)
			{
				if (extend != null)
				{
					SetExtend(extend[0], extend[1]);
				}
			}

			public Radial(PdfColorSpace cs, PdfArray coords, PdfFunction function)
				: base(new PdfDictionary(), PdfShading.ShadingType.RADIAL, cs.GetPdfObject())
			{
				SetCoords(coords);
				SetFunction(function);
			}

			public virtual PdfArray GetCoords()
			{
				return GetPdfObject().GetAsArray(PdfName.Coords);
			}

			public virtual void SetCoords(float x0, float y0, float r0, float x1, float y1, float
				 r1)
			{
				SetCoords(new PdfArray(new float[] { x0, y0, r0, x1, y1, r1 }));
			}

			public virtual void SetCoords(PdfArray coords)
			{
				GetPdfObject().Put(PdfName.Coords, coords);
				SetModified();
			}

			public virtual float[] GetDomain()
			{
				PdfArray domain = GetPdfObject().GetAsArray(PdfName.Domain);
				if (domain == null)
				{
					return new float[] { 0, 1 };
				}
				return new float[] { domain.GetAsNumber(0).FloatValue(), domain.GetAsNumber(1).FloatValue
					() };
			}

			public virtual void SetDomain(float t0, float t1)
			{
				GetPdfObject().Put(PdfName.Domain, new PdfArray(new float[] { t0, t1 }));
				SetModified();
			}

			public virtual bool[] GetExtend()
			{
				PdfArray extend = GetPdfObject().GetAsArray(PdfName.Extend);
				if (extend == null)
				{
					return new bool[] { true, true };
				}
				return new bool[] { extend.GetAsBoolean(0).GetValue(), extend.GetAsBoolean(1).GetValue
					() };
			}

			public virtual void SetExtend(bool extendStart, bool extendEnd)
			{
				GetPdfObject().Put(PdfName.Extend, new PdfArray(new bool[] { extendStart, extendEnd
					 }));
				SetModified();
			}
		}

		public class FreeFormGouraudShadedTriangleMesh : PdfShading
		{
			public FreeFormGouraudShadedTriangleMesh(PdfStream pdfStream)
				: base(pdfStream)
			{
			}

			public FreeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate, 
				int bitsPerComponent, int bitsPerFlag, float[] decode)
				: this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)
					)
			{
			}

			public FreeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate, 
				int bitsPerComponent, int bitsPerFlag, PdfArray decode)
				: base(new PdfStream(), PdfShading.ShadingType.FREE_FORM_GOURAUD_SHADED_TRIANGLE_MESH
					, cs.GetPdfObject())
			{
				SetBitsPerCoordinate(bitsPerCoordinate);
				SetBitsPerComponent(bitsPerComponent);
				SetBitsPerFlag(bitsPerFlag);
				SetDecode(decode);
			}

			public virtual int GetBitsPerCoordinate()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
			}

			public virtual void SetBitsPerCoordinate(int bitsPerCoordinate)
			{
				GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
				SetModified();
			}

			public virtual int GetBitsPerComponent()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
			}

			public virtual void SetBitsPerComponent(int bitsPerComponent)
			{
				GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
				SetModified();
			}

			public virtual int GetBitsPerFlag()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerFlag);
			}

			public virtual void SetBitsPerFlag(int bitsPerFlag)
			{
				GetPdfObject().Put(PdfName.BitsPerFlag, new PdfNumber(bitsPerFlag));
				SetModified();
			}

			public virtual PdfArray GetDecode()
			{
				return GetPdfObject().GetAsArray(PdfName.Decode);
			}

			public virtual void SetDecode(float[] decode)
			{
				GetPdfObject().Put(PdfName.Decode, new PdfArray(decode));
			}

			public virtual void SetDecode(PdfArray decode)
			{
				GetPdfObject().Put(PdfName.Decode, decode);
			}
		}

		public class LatticeFormGouraudShadedTriangleMesh : PdfShading
		{
			public LatticeFormGouraudShadedTriangleMesh(PdfStream pdfStream)
				: base(pdfStream)
			{
			}

			public LatticeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate
				, int bitsPerComponent, int verticesPerRow, float[] decode)
				: this(cs, bitsPerCoordinate, bitsPerComponent, verticesPerRow, new PdfArray(decode
					))
			{
			}

			public LatticeFormGouraudShadedTriangleMesh(PdfColorSpace cs, int bitsPerCoordinate
				, int bitsPerComponent, int verticesPerRow, PdfArray decode)
				: base(new PdfStream(), PdfShading.ShadingType.LATTICE_FORM_GOURAUD_SHADED_TRIANGLE_MESH
					, cs.GetPdfObject())
			{
				SetBitsPerCoordinate(bitsPerCoordinate);
				SetBitsPerComponent(bitsPerComponent);
				SetVerticesPerRow(verticesPerRow);
				SetDecode(decode);
			}

			public virtual int GetBitsPerCoordinate()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
			}

			public virtual void SetBitsPerCoordinate(int bitsPerCoordinate)
			{
				GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
				SetModified();
			}

			public virtual int GetBitsPerComponent()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
			}

			public virtual void SetBitsPerComponent(int bitsPerComponent)
			{
				GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
				SetModified();
			}

			public virtual int GetVerticesPerRow()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.VerticesPerRow);
			}

			public virtual void SetVerticesPerRow(int verticesPerRow)
			{
				GetPdfObject().Put(PdfName.VerticesPerRow, new PdfNumber(verticesPerRow));
				SetModified();
			}

			public virtual PdfArray GetDecode()
			{
				return GetPdfObject().GetAsArray(PdfName.Decode);
			}

			public virtual void SetDecode(float[] decode)
			{
				GetPdfObject().Put(PdfName.Decode, new PdfArray(decode));
			}

			public virtual void SetDecode(PdfArray decode)
			{
				GetPdfObject().Put(PdfName.Decode, decode);
			}
		}

		public class CoonsPatchMesh : PdfShading
		{
			public CoonsPatchMesh(PdfStream pdfStream)
				: base(pdfStream)
			{
			}

			public CoonsPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent
				, int bitsPerFlag, float[] decode)
				: this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)
					)
			{
			}

			public CoonsPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent
				, int bitsPerFlag, PdfArray decode)
				: base(new PdfStream(), PdfShading.ShadingType.COONS_PATCH_MESH, cs.GetPdfObject(
					))
			{
				SetBitsPerCoordinate(bitsPerCoordinate);
				SetBitsPerComponent(bitsPerComponent);
				SetBitsPerFlag(bitsPerFlag);
				SetDecode(decode);
			}

			public virtual int GetBitsPerCoordinate()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
			}

			public virtual void SetBitsPerCoordinate(int bitsPerCoordinate)
			{
				GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
				SetModified();
			}

			public virtual int GetBitsPerComponent()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
			}

			public virtual void SetBitsPerComponent(int bitsPerComponent)
			{
				GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
				SetModified();
			}

			public virtual int GetBitsPerFlag()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerFlag);
			}

			public virtual void SetBitsPerFlag(int bitsPerFlag)
			{
				GetPdfObject().Put(PdfName.BitsPerFlag, new PdfNumber(bitsPerFlag));
				SetModified();
			}

			public virtual PdfArray GetDecode()
			{
				return GetPdfObject().GetAsArray(PdfName.Decode);
			}

			public virtual void SetDecode(float[] decode)
			{
				GetPdfObject().Put(PdfName.Decode, new PdfArray(decode));
			}

			public virtual void SetDecode(PdfArray decode)
			{
				GetPdfObject().Put(PdfName.Decode, decode);
			}
		}

		public class TensorProductPatchMesh : PdfShading
		{
			public TensorProductPatchMesh(PdfStream pdfStream)
				: base(pdfStream)
			{
			}

			public TensorProductPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent
				, int bitsPerFlag, float[] decode)
				: this(cs, bitsPerCoordinate, bitsPerComponent, bitsPerFlag, new PdfArray(decode)
					)
			{
			}

			public TensorProductPatchMesh(PdfColorSpace cs, int bitsPerCoordinate, int bitsPerComponent
				, int bitsPerFlag, PdfArray decode)
				: base(new PdfStream(), PdfShading.ShadingType.TENSOR_PRODUCT_PATCH_MESH, cs.GetPdfObject
					())
			{
				SetBitsPerCoordinate(bitsPerCoordinate);
				SetBitsPerComponent(bitsPerComponent);
				SetBitsPerFlag(bitsPerFlag);
				SetDecode(decode);
			}

			public virtual int GetBitsPerCoordinate()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerCoordinate);
			}

			public virtual void SetBitsPerCoordinate(int bitsPerCoordinate)
			{
				GetPdfObject().Put(PdfName.BitsPerCoordinate, new PdfNumber(bitsPerCoordinate));
				SetModified();
			}

			public virtual int GetBitsPerComponent()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerComponent);
			}

			public virtual void SetBitsPerComponent(int bitsPerComponent)
			{
				GetPdfObject().Put(PdfName.BitsPerComponent, new PdfNumber(bitsPerComponent));
				SetModified();
			}

			public virtual int GetBitsPerFlag()
			{
				return (int)GetPdfObject().GetAsInt(PdfName.BitsPerFlag);
			}

			public virtual void SetBitsPerFlag(int bitsPerFlag)
			{
				GetPdfObject().Put(PdfName.BitsPerFlag, new PdfNumber(bitsPerFlag));
				SetModified();
			}

			public virtual PdfArray GetDecode()
			{
				return GetPdfObject().GetAsArray(PdfName.Decode);
			}

			public virtual void SetDecode(float[] decode)
			{
				GetPdfObject().Put(PdfName.Decode, new PdfArray(decode));
			}

			public virtual void SetDecode(PdfArray decode)
			{
				GetPdfObject().Put(PdfName.Decode, decode);
			}
		}
	}
}
