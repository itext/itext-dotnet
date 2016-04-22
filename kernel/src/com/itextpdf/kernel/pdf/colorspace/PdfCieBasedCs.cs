/*
$Id: 71a20edc3a67ddfa6209d79a47e1aa9727a957f1 $

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
using System.Collections.Generic;
using System.IO;
using com.itextpdf.io.color;
using com.itextpdf.kernel;
using com.itextpdf.kernel.pdf;

namespace com.itextpdf.kernel.pdf.colorspace
{
	public abstract class PdfCieBasedCs : PdfColorSpace
	{
		private const long serialVersionUID = 7803780450619297557L;

		protected internal override bool IsWrappedObjectMustBeIndirect()
		{
			return true;
		}

		public PdfCieBasedCs(PdfArray pdfObject)
			: base(pdfObject)
		{
		}

		public class CalGray : PdfCieBasedCs
		{
			private const long serialVersionUID = -3974274460820215173L;

			public CalGray(PdfArray pdfObject)
				: base(pdfObject)
			{
			}

			public CalGray(float[] whitePoint)
				: this(new PdfArray(new _List_81()))
			{
				if (whitePoint == null || whitePoint.Length != 3)
				{
					throw new PdfException(PdfException.WhitePointIsIncorrectlySpecified, this);
				}
				PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
				d.Put(PdfName.WhitePoint, new PdfArray(whitePoint));
			}

			private sealed class _List_81 : List<PdfObject>
			{
				public _List_81()
				{
					{
						this.Add(PdfName.CalGray);
						this.Add(new PdfDictionary());
					}
				}
			}

			public CalGray(float[] whitePoint, float[] blackPoint, float gamma)
				: this(whitePoint)
			{
				PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
				if (blackPoint != null)
				{
					d.Put(PdfName.BlackPoint, new PdfArray(blackPoint));
				}
				if (gamma != float.NaN)
				{
					d.Put(PdfName.Gamma, new PdfNumber(gamma));
				}
			}

			public override int GetNumberOfComponents()
			{
				return 1;
			}

			public override float[] GetDefaultColorants()
			{
				return new float[GetNumberOfComponents()];
			}
		}

		public class CalRgb : PdfCieBasedCs
		{
			private const long serialVersionUID = -2926074370411556426L;

			public CalRgb(PdfArray pdfObject)
				: base(pdfObject)
			{
			}

			public CalRgb(float[] whitePoint)
				: this(new PdfArray(new _List_120()))
			{
				if (whitePoint == null || whitePoint.Length != 3)
				{
					throw new PdfException(PdfException.WhitePointIsIncorrectlySpecified, this);
				}
				PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
				d.Put(PdfName.WhitePoint, new PdfArray(whitePoint));
			}

			private sealed class _List_120 : List<PdfObject>
			{
				public _List_120()
				{
					{
						this.Add(PdfName.CalRGB);
						this.Add(new PdfDictionary());
					}
				}
			}

			public CalRgb(float[] whitePoint, float[] blackPoint, float[] gamma, float[] matrix
				)
				: this(whitePoint)
			{
				PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
				if (blackPoint != null)
				{
					d.Put(PdfName.BlackPoint, new PdfArray(blackPoint));
				}
				if (gamma != null)
				{
					d.Put(PdfName.Gamma, new PdfArray(gamma));
				}
				if (matrix != null)
				{
					d.Put(PdfName.Matrix, new PdfArray(matrix));
				}
			}

			public override int GetNumberOfComponents()
			{
				return 3;
			}

			public override float[] GetDefaultColorants()
			{
				return new float[GetNumberOfComponents()];
			}
		}

		public class Lab : PdfCieBasedCs
		{
			private const long serialVersionUID = 7067722970343880433L;

			public Lab(PdfArray pdfObject)
				: base(pdfObject)
			{
			}

			public Lab(float[] whitePoint)
				: this(new PdfArray(new _List_161()))
			{
				if (whitePoint == null || whitePoint.Length != 3)
				{
					throw new PdfException(PdfException.WhitePointIsIncorrectlySpecified, this);
				}
				PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
				d.Put(PdfName.WhitePoint, new PdfArray(whitePoint));
			}

			private sealed class _List_161 : List<PdfObject>
			{
				public _List_161()
				{
					{
						this.Add(PdfName.Lab);
						this.Add(new PdfDictionary());
					}
				}
			}

			public Lab(float[] whitePoint, float[] blackPoint, float[] range)
				: this(whitePoint)
			{
				PdfDictionary d = ((PdfArray)GetPdfObject()).GetAsDictionary(1);
				if (blackPoint != null)
				{
					d.Put(PdfName.BlackPoint, new PdfArray(blackPoint));
				}
				if (range != null)
				{
					d.Put(PdfName.Range, new PdfArray(range));
				}
			}

			public override int GetNumberOfComponents()
			{
				return 3;
			}

			public override float[] GetDefaultColorants()
			{
				return new float[GetNumberOfComponents()];
			}
		}

		public class IccBased : PdfCieBasedCs
		{
			private const long serialVersionUID = 3265273715107224067L;

			public IccBased(PdfArray pdfObject)
				: base(pdfObject)
			{
			}

			public IccBased(Stream iccStream)
				: this(new PdfArray(new _List_200(iccStream)))
			{
			}

			private sealed class _List_200 : List<PdfObject>
			{
				public _List_200(Stream iccStream)
				{
					this.iccStream = iccStream;
					{
						this.Add(PdfName.ICCBased);
						this.Add(PdfCieBasedCs.IccBased.GetIccProfileStream(iccStream));
					}
				}

				private readonly Stream iccStream;
			}

			public IccBased(Stream iccStream, float[] range)
				: this(new PdfArray(new _List_207(iccStream, range)))
			{
			}

			private sealed class _List_207 : List<PdfObject>
			{
				public _List_207(Stream iccStream, float[] range)
				{
					this.iccStream = iccStream;
					this.range = range;
					{
						this.Add(PdfName.ICCBased);
						this.Add(PdfCieBasedCs.IccBased.GetIccProfileStream(iccStream, range));
					}
				}

				private readonly Stream iccStream;

				private readonly float[] range;
			}

			public override int GetNumberOfComponents()
			{
				return ((PdfArray)GetPdfObject()).GetAsStream(1).GetAsInt(PdfName.N);
			}

			public override float[] GetDefaultColorants()
			{
				return new float[GetNumberOfComponents()];
			}

			public static PdfStream GetIccProfileStream(Stream iccStream)
			{
				IccProfile iccProfile = IccProfile.GetInstance(iccStream);
				PdfStream stream = new PdfStream(iccProfile.GetData());
				stream.Put(PdfName.N, new PdfNumber(iccProfile.GetNumComponents()));
				switch (iccProfile.GetNumComponents())
				{
					case 1:
					{
						stream.Put(PdfName.Alternate, PdfName.DeviceGray);
						break;
					}

					case 3:
					{
						stream.Put(PdfName.Alternate, PdfName.DeviceRGB);
						break;
					}

					case 4:
					{
						stream.Put(PdfName.Alternate, PdfName.DeviceCMYK);
						break;
					}

					default:
					{
						break;
					}
				}
				return stream;
			}

			public static PdfStream GetIccProfileStream(Stream iccStream, float[] range)
			{
				PdfStream stream = GetIccProfileStream(iccStream);
				stream.Put(PdfName.Range, new PdfArray(range));
				return stream;
			}
		}
	}
}
