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
using iTextSharp.IO;
using iTextSharp.IO.Log;
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Log;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Pdf.Tagutils;
using iTextSharp.Kernel.Xmp;
using iTextSharp.Pdfa.Checker;

namespace iTextSharp.Pdfa
{
	public class PdfADocument : PdfDocument
	{
		protected internal PdfAChecker checker;

		public PdfADocument(PdfWriter writer, PdfAConformanceLevel conformanceLevel, PdfOutputIntent
			 outputIntent)
			: base(writer)
		{
			SetChecker(conformanceLevel);
			AddOutputIntent(outputIntent);
		}

		public PdfADocument(PdfReader reader, PdfWriter writer)
			: this(reader, writer, new StampingProperties())
		{
		}

		public PdfADocument(PdfReader reader, PdfWriter writer, StampingProperties properties
			)
			: base(reader, writer, properties)
		{
			byte[] existingXmpMetadata = GetXmpMetadata();
			if (existingXmpMetadata == null)
			{
				throw new PdfAConformanceException(PdfAConformanceException.DocumentToReadFromShallBeAPdfAConformantFileWithValidXmpMetadata
					);
			}
			XmpMeta meta;
			try
			{
				meta = XmpMetaFactory.ParseFromBuffer(existingXmpMetadata);
			}
			catch (XmpException)
			{
				throw new PdfAConformanceException(PdfAConformanceException.DocumentToReadFromShallBeAPdfAConformantFileWithValidXmpMetadata
					);
			}
			PdfAConformanceLevel conformanceLevel = PdfAConformanceLevel.GetConformanceLevel(
				meta);
			if (conformanceLevel == null)
			{
				throw new PdfAConformanceException(PdfAConformanceException.DocumentToReadFromShallBeAPdfAConformantFileWithValidXmpMetadata
					);
			}
			SetChecker(conformanceLevel);
		}

		public override void CheckIsoConformance(Object obj, IsoKey key)
		{
			CheckIsoConformance(obj, key, null);
		}

		public override void CheckShowTextIsoConformance(Object obj, PdfResources resources
			)
		{
			CanvasGraphicsState gState = (CanvasGraphicsState)obj;
			bool fill = false;
			bool stroke = false;
			switch (gState.GetTextRenderingMode())
			{
				case PdfCanvasConstants.TextRenderingMode.STROKE:
				case PdfCanvasConstants.TextRenderingMode.STROKE_CLIP:
				{
					stroke = true;
					break;
				}

				case PdfCanvasConstants.TextRenderingMode.FILL:
				case PdfCanvasConstants.TextRenderingMode.FILL_CLIP:
				{
					fill = true;
					break;
				}

				case PdfCanvasConstants.TextRenderingMode.FILL_STROKE:
				case PdfCanvasConstants.TextRenderingMode.FILL_STROKE_CLIP:
				{
					stroke = true;
					fill = true;
					break;
				}
			}
			IsoKey drawMode = IsoKey.DRAWMODE_FILL;
			if (fill && stroke)
			{
				drawMode = IsoKey.DRAWMODE_FILL_STROKE;
			}
			else
			{
				if (fill)
				{
					drawMode = IsoKey.DRAWMODE_FILL;
				}
				else
				{
					if (stroke)
					{
						drawMode = IsoKey.DRAWMODE_STROKE;
					}
				}
			}
			if (fill || stroke)
			{
				CheckIsoConformance(gState, drawMode, resources);
			}
		}

		public override void CheckIsoConformance(Object obj, IsoKey key, PdfResources resources
			)
		{
			CanvasGraphicsState gState;
			PdfDictionary currentColorSpaces = null;
			if (resources != null)
			{
				currentColorSpaces = resources.GetPdfObject().GetAsDictionary(PdfName.ColorSpace);
			}
			switch (key)
			{
				case IsoKey.CANVAS_STACK:
				{
					checker.CheckCanvasStack((char)(char)obj);
					break;
				}

				case IsoKey.PDF_OBJECT:
				{
					checker.CheckPdfObject((PdfObject)obj);
					break;
				}

				case IsoKey.RENDERING_INTENT:
				{
					checker.CheckRenderingIntent((PdfName)obj);
					break;
				}

				case IsoKey.INLINE_IMAGE:
				{
					checker.CheckInlineImage((PdfStream)obj, currentColorSpaces);
					break;
				}

				case IsoKey.GRAPHIC_STATE_ONLY:
				{
					gState = (CanvasGraphicsState)obj;
					checker.CheckExtGState(gState);
					break;
				}

				case IsoKey.DRAWMODE_FILL:
				{
					gState = (CanvasGraphicsState)obj;
					checker.CheckColor(gState.GetFillColor(), currentColorSpaces, true);
					checker.CheckExtGState(gState);
					break;
				}

				case IsoKey.DRAWMODE_STROKE:
				{
					gState = (CanvasGraphicsState)obj;
					checker.CheckColor(gState.GetStrokeColor(), currentColorSpaces, false);
					checker.CheckExtGState(gState);
					break;
				}

				case IsoKey.DRAWMODE_FILL_STROKE:
				{
					gState = (CanvasGraphicsState)obj;
					checker.CheckColor(gState.GetFillColor(), currentColorSpaces, true);
					checker.CheckColor(gState.GetStrokeColor(), currentColorSpaces, false);
					checker.CheckExtGState(gState);
					break;
				}

				case IsoKey.PAGE:
				{
					checker.CheckSinglePage((PdfPage)obj);
					break;
				}
			}
		}

		public virtual PdfAConformanceLevel GetConformanceLevel()
		{
			return checker.GetConformanceLevel();
		}

		protected override void UpdateXmpMetadata()
		{
			try
			{
				XmpMeta xmpMeta = UpdateDefaultXmpMetadata();
				xmpMeta.SetProperty(XmpConst.NS_PDFA_ID, XmpConst.PART, checker.GetConformanceLevel
					().GetPart());
				xmpMeta.SetProperty(XmpConst.NS_PDFA_ID, XmpConst.CONFORMANCE, checker.GetConformanceLevel
					().GetConformance());
				if (this.IsTagged())
				{
					XmpMeta taggedExtensionMeta = XmpMetaFactory.ParseFromString(PdfAXMPUtil.PDF_UA_EXTENSION
						);
					XmpUtils.AppendProperties(taggedExtensionMeta, xmpMeta, true, false);
				}
				SetXmpMetadata(xmpMeta);
			}
			catch (XmpException e)
			{
				ILogger logger = LoggerFactory.GetLogger(typeof(iTextSharp.Pdfa.PdfADocument));
				logger.Error(LogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA, e);
			}
		}

		protected override void CheckIsoConformance()
		{
			checker.CheckDocument(catalog);
		}

		/// <exception cref="System.IO.IOException"/>
		protected override void FlushObject(PdfObject pdfObject, bool canBeInObjStm)
		{
			MarkObjectAsMustBeFlushed(pdfObject);
			if (isClosing || checker.ObjectIsChecked(pdfObject))
			{
				base.FlushObject(pdfObject, canBeInObjStm);
			}
		}

		//suppress the call
		//TODO log unsuccessful call
		protected override void FlushFonts()
		{
			foreach (PdfFont pdfFont in GetDocumentFonts())
			{
				if (!pdfFont.IsEmbedded())
				{
					throw new PdfAConformanceException(PdfAConformanceException.AllFontsMustBeEmbeddedThisOneIsnt1
						).SetMessageParams(pdfFont.GetFontProgram().GetFontNames().GetFontName());
				}
			}
			base.FlushFonts();
		}

		protected internal virtual void SetChecker(PdfAConformanceLevel conformanceLevel)
		{
			switch (conformanceLevel.GetPart())
			{
				case "1":
				{
					checker = new PdfA1Checker(conformanceLevel);
					break;
				}

				case "2":
				{
					checker = new PdfA2Checker(conformanceLevel);
					break;
				}

				case "3":
				{
					checker = new PdfA3Checker(conformanceLevel);
					break;
				}
			}
		}

		protected override void InitTagStructureContext()
		{
			tagStructureContext = new TagStructureContext(this, GetPdfVersionForPdfA(checker.
				GetConformanceLevel()));
		}

		protected override Counter GetCounter()
		{
			return CounterFactory.GetCounter(typeof(iTextSharp.Pdfa.PdfADocument));
		}

		private static PdfVersion GetPdfVersionForPdfA(PdfAConformanceLevel conformanceLevel
			)
		{
			PdfVersion version;
			switch (conformanceLevel.GetPart())
			{
				case "1":
				{
					version = PdfVersion.PDF_1_4;
					break;
				}

				case "2":
				{
					version = PdfVersion.PDF_1_7;
					break;
				}

				case "3":
				{
					version = PdfVersion.PDF_1_7;
					break;
				}

				default:
				{
					version = PdfVersion.PDF_1_4;
					break;
				}
			}
			return version;
		}
	}
}
