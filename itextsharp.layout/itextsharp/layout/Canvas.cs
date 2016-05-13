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
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Pdf.Xobject;
using iTextSharp.Layout.Element;
using iTextSharp.Layout.Renderer;

namespace iTextSharp.Layout
{
	/// <summary>
	/// This class is used for adding content directly onto a specified
	/// <see cref="iTextSharp.Kernel.Pdf.Canvas.PdfCanvas"/>
	/// .
	/// <see cref="Canvas"/>
	/// does not know the concept of a page, so it can't reflow to a 'next'
	/// <see cref="Canvas"/>
	/// .
	/// This class effectively acts as a bridge between the high-level <em>layout</em>
	/// API and the low-level <em>kernel</em> API.
	/// </summary>
	public class Canvas : RootElement<iTextSharp.Layout.Canvas>
	{
		protected internal PdfCanvas pdfCanvas;

		protected internal Rectangle rootArea;

		/// <summary>
		/// Is initialized and used only when Canvas element autotagging is enabled, see
		/// <see cref="EnableAutoTagging(iTextSharp.Kernel.Pdf.PdfPage)"/>
		/// .
		/// It is also used to determine if autotagging is enabled.
		/// </summary>
		protected internal PdfPage page;

		/// <summary>Creates a new Canvas to manipulate a specific document and page.</summary>
		/// <param name="pdfCanvas">the low-level content stream writer</param>
		/// <param name="pdfDocument">the document that the resulting content stream will be written to
		/// 	</param>
		/// <param name="rootArea">the maximum area that the Canvas may write upon</param>
		public Canvas(PdfCanvas pdfCanvas, PdfDocument pdfDocument, Rectangle rootArea)
			: base()
		{
			this.pdfDocument = pdfDocument;
			this.pdfCanvas = pdfCanvas;
			this.rootArea = rootArea;
		}

		/// <summary>Creates a new Canvas to manipulate a specific document and page.</summary>
		/// <param name="pdfCanvas">the low-level content stream writer</param>
		/// <param name="pdfDocument">the document that the resulting content stream will be written to
		/// 	</param>
		/// <param name="rootArea">the maximum area that the Canvas may write upon</param>
		public Canvas(PdfCanvas pdfCanvas, PdfDocument pdfDocument, Rectangle rootArea, bool
			 immediateFlush)
			: this(pdfCanvas, pdfDocument, rootArea)
		{
			this.immediateFlush = immediateFlush;
		}

		/// <summary>
		/// Creates a new Canvas to manipulate a specific
		/// <see cref="iTextSharp.Kernel.Pdf.Xobject.PdfFormXObject"/>
		/// .
		/// </summary>
		/// <param name="formXObject">the form</param>
		/// <param name="pdfDocument">the document that the resulting content stream will be written to
		/// 	</param>
		public Canvas(PdfFormXObject formXObject, PdfDocument pdfDocument)
			: this(new PdfCanvas(formXObject, pdfDocument), pdfDocument, formXObject.GetBBox(
				).ToRectangle())
		{
		}

		/// <summary>
		/// Gets the
		/// <see cref="iTextSharp.Kernel.Pdf.PdfDocument"/>
		/// for this canvas.
		/// </summary>
		/// <returns>the document that the resulting content stream will be written to</returns>
		public virtual PdfDocument GetPdfDocument()
		{
			return pdfDocument;
		}

		/// <summary>Gets the root area rectangle.</summary>
		/// <returns>the maximum area that the Canvas may write upon</returns>
		public virtual Rectangle GetRootArea()
		{
			return rootArea;
		}

		/// <summary>
		/// Gets the
		/// <see cref="iTextSharp.Kernel.Pdf.Canvas.PdfCanvas"/>
		/// .
		/// </summary>
		/// <returns>the low-level content stream writer</returns>
		public virtual PdfCanvas GetPdfCanvas()
		{
			return pdfCanvas;
		}

		/// <summary>
		/// Sets the
		/// <see cref="iTextSharp.Layout.Renderer.IRenderer"/>
		/// for this Canvas.
		/// </summary>
		/// <param name="canvasRenderer">a renderer specific for canvas operations</param>
		public virtual void SetRenderer(CanvasRenderer canvasRenderer)
		{
			this.rootRenderer = canvasRenderer;
		}

		/// <summary>Returned value is not null only in case when autotagging is enabled.</summary>
		/// <returns>the page, on which this canvas will be rendered, or null if autotagging is not enabled.
		/// 	</returns>
		public virtual PdfPage GetPage()
		{
			return page;
		}

		/// <summary>Enables canvas content autotagging.</summary>
		/// <remarks>Enables canvas content autotagging. By default it is disabled.</remarks>
		/// <param name="page">the page, on which this canvas will be rendered.</param>
		public virtual void EnableAutoTagging(PdfPage page)
		{
			this.page = page;
		}

		/// <returns>true if autotagging of canvas content is enabled. Default value - false.
		/// 	</returns>
		public virtual bool IsAutoTaggingEnabled()
		{
			return page != null;
		}

		public virtual void Relayout()
		{
			if (immediateFlush)
			{
				throw new InvalidOperationException("Operation not supported with immediate flush"
					);
			}
			rootRenderer = new CanvasRenderer(this, immediateFlush);
			foreach (IElement element in childElements)
			{
				rootRenderer.AddChild(element.CreateRendererSubTree());
			}
		}

		protected internal override RootRenderer EnsureRootRendererNotNull()
		{
			if (rootRenderer == null)
			{
				rootRenderer = new CanvasRenderer(this, immediateFlush);
			}
			return rootRenderer;
		}
	}
}
