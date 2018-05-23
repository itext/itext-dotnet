/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
Authors: iText Software.

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
using System.IO;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Converter {
    /// <summary>
    /// This is the main container class for static methods that do high-level
    /// conversion operations from input to PDF, either by drawing on a canvas or by
    /// returning an XObject, which can then be used by the calling class for further
    /// processing and drawing operations.
    /// </summary>
    public sealed class SvgConverter {
        private SvgConverter() {
        }

        private static void CheckNull(Object o) {
            if (o == null) {
                throw new SvgProcessingException(SvgLogMessageConstant.PARAMETER_CANNOT_BE_NULL);
            }
        }

        /// <summary>
        /// Draws a String containing valid SVG to a document, on a given page
        /// number.
        /// </summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        public static void DrawOnDocument(String content, PdfDocument document, int pageNo) {
            DrawOnDocument(content, document, pageNo, 0, 0);
        }

        /// <summary>
        /// Draws a String containing valid SVG to a document, on a given page
        /// number.
        /// </summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        public static void DrawOnDocument(String content, PdfDocument document, int pageNo, float x, float y) {
            CheckNull(document);
            DrawOnPage(content, document.GetPage(pageNo), x, y);
        }

        /// <summary>
        /// Draws a String containing valid SVG to a document, on a given page
        /// number.
        /// </summary>
        /// <param name="content">the Stream object containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnDocument(String content, PdfDocument document, int pageNo, ISvgConverterProperties
             props) {
            DrawOnDocument(content, document, pageNo, 0, 0, props);
        }

        /// <summary>
        /// Draws a String containing valid SVG to a document, on a given page
        /// number.
        /// </summary>
        /// <param name="content">the Stream object containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnDocument(String content, PdfDocument document, int pageNo, float x, float y, ISvgConverterProperties
             props) {
            CheckNull(document);
            DrawOnPage(content, document.GetPage(pageNo), x, y, props);
        }

        /// <summary>
        /// Draws a Stream containing valid SVG to a document, on a given page
        /// number.
        /// </summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnDocument(Stream stream, PdfDocument document, int pageNo) {
            DrawOnDocument(stream, document, pageNo, 0, 0);
        }

        /// <summary>
        /// Draws a Stream containing valid SVG to a document, on a given page
        /// number.
        /// </summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnDocument(Stream stream, PdfDocument document, int pageNo, float x, float y) {
            CheckNull(document);
            DrawOnPage(stream, document.GetPage(pageNo), x, y);
        }

        /// <summary>
        /// Draws a Stream containing valid SVG to a document, on a given page
        /// number.
        /// </summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnDocument(Stream stream, PdfDocument document, int pageNo, ISvgConverterProperties
             props) {
            DrawOnDocument(stream, document, pageNo, 0, 0, props);
        }

        /// <summary>
        /// Draws a Stream containing valid SVG to a document, on a given page
        /// number.
        /// </summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnDocument(Stream stream, PdfDocument document, int pageNo, float x, float y, ISvgConverterProperties
             props) {
            CheckNull(document);
            DrawOnPage(stream, document.GetPage(pageNo), x, y, props);
        }

        /// <summary>Draws a String containing valid SVG to a given page</summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        public static void DrawOnPage(String content, PdfPage page) {
            DrawOnPage(content, page, 0, 0);
        }

        /// <summary>Draws a String containing valid SVG to a given page</summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        public static void DrawOnPage(String content, PdfPage page, float x, float y) {
            CheckNull(page);
            DrawOnCanvas(content, new PdfCanvas(page), x, y);
        }

        /// <summary>Draws a String containing valid SVG to a given page</summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnPage(String content, PdfPage page, ISvgConverterProperties props) {
            DrawOnPage(content, page, 0, 0, props);
        }

        /// <summary>Draws a String containing valid SVG to a given page</summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnPage(String content, PdfPage page, float x, float y, ISvgConverterProperties props
            ) {
            CheckNull(page);
            DrawOnCanvas(content, new PdfCanvas(page), x, y, props);
        }

        /// <summary>Draws a Stream containing valid SVG to a given page</summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnPage(Stream stream, PdfPage page) {
            DrawOnPage(stream, page, 0, 0);
        }

        /// <summary>Draws a Stream containing valid SVG to a given page, at a given location</summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnPage(Stream stream, PdfPage page, float x, float y) {
            CheckNull(page);
            DrawOnCanvas(stream, new PdfCanvas(page), x, y);
        }

        /// <summary>Draws a Stream containing valid SVG to a given page</summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnPage(Stream stream, PdfPage page, ISvgConverterProperties props) {
            DrawOnPage(stream, page, 0, 0, props);
        }

        /// <summary>Draws a Stream containing valid SVG to a given page</summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnPage(Stream stream, PdfPage page, float x, float y, ISvgConverterProperties props
            ) {
            CheckNull(page);
            DrawOnCanvas(stream, new PdfCanvas(page), x, y, props);
        }

        /// <summary>Draws a String containing valid SVG to a pre-made canvas object</summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        public static void DrawOnCanvas(String content, PdfCanvas canvas) {
            DrawOnCanvas(content, canvas, 0, 0);
        }

        /// <summary>Draws a String containing valid SVG to a pre-made canvas object</summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        public static void DrawOnCanvas(String content, PdfCanvas canvas, float x, float y) {
            CheckNull(canvas);
            Draw(ConvertToXObject(content, canvas.GetDocument()), canvas, x, y);
        }

        /// <summary>Draws a String containing valid SVG to a pre-made canvas object</summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnCanvas(String content, PdfCanvas canvas, ISvgConverterProperties props) {
            DrawOnCanvas(content, canvas, 0, 0, props);
        }

        /// <summary>draws a String containing valid SVG to a pre-made canvas object, at a specified location</summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnCanvas(String content, PdfCanvas canvas, float x, float y, ISvgConverterProperties
             props) {
            CheckNull(canvas);
            Draw(ConvertToXObject(content, canvas.GetDocument(), props), canvas, x, y);
        }

        /// <summary>Draws a Stream containing valid SVG to a pre-made canvas object</summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnCanvas(Stream stream, PdfCanvas canvas) {
            DrawOnCanvas(stream, canvas, 0, 0);
        }

        /// <summary>Draws a Stream containing valid SVG to a pre-made canvas object, to a specified location</summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnCanvas(Stream stream, PdfCanvas canvas, float x, float y) {
            CheckNull(canvas);
            Draw(ConvertToXObject(stream, canvas.GetDocument()), canvas, x, y);
        }

        /// <summary>Draws a Stream containing valid SVG to a pre-made canvas object</summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnCanvas(Stream stream, PdfCanvas canvas, ISvgConverterProperties props) {
            DrawOnCanvas(stream, canvas, 0, 0, props);
        }

        /// <summary>Draws a String containing valid SVG to a pre-made canvas object, at a specified position on the canvas
        ///     </summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static void DrawOnCanvas(Stream stream, PdfCanvas canvas, float x, float y, ISvgConverterProperties
             props) {
            CheckNull(canvas);
            Draw(ConvertToXObject(stream, canvas.GetDocument(), props), canvas, x, y);
        }

        /// <summary>Create a single page pdf containing the SVG on its page using the default processing and drawing logic
        ///     </summary>
        /// <param name="svgStream">inputstream containing the SVG</param>
        /// <param name="pdfDest">PDF destination outputStream</param>
        /// <exception cref="System.IO.IOException">when the one of the streams cannot be read correctly</exception>
        public static void CreatePdf(Stream svgStream, Stream pdfDest) {
            CreatePdf(svgStream, null, pdfDest, null);
        }

        /// <summary>Create a single page pdf containing the SVG on its page using the default processing and drawing logic
        ///     </summary>
        /// <param name="svgStream">inputstream containing the SVG</param>
        /// <param name="pdfDest">PDF destination outputStream</param>
        /// <param name="writerprops">writerproperties for the pdf document</param>
        /// <exception cref="System.IO.IOException">when the one of the streams cannot be read correctly</exception>
        public static void CreatePdf(Stream svgStream, Stream pdfDest, WriterProperties writerprops) {
            CreatePdf(svgStream, null, pdfDest, writerprops);
        }

        /// <summary>Create a single page pdf containing the SVG on its page using the default processing and drawing logic
        ///     </summary>
        /// <param name="svgStream">inputstream containing the SVG</param>
        /// <param name="props">Svg Converter properties to change default behaviour</param>
        /// <param name="pdfDest">PDF destination outputStream</param>
        /// <param name="svgStream">inputstream containing the SVG</param>
        /// <param name="props">Svg Converter properties to change default behaviour</param>
        /// <param name="pdfDest">PDF destination outputStream</param>
        /// <param name="writerprops">writerproperties for the pdf document</param>
        /// <exception cref="System.IO.IOException">
        /// when the one of the streams cannot be read correctly
        /// public static void createPdf(InputStream svgStream,ISvgConverterProperties props, OutputStream pdfDest) throws IOException {
        /// createPdf(svgStream,props,pdfDest,null);
        /// }
        /// <p>
        /// /
        /// Create a single page pdf containing the SVG on its page using the default processing and drawing logic
        /// </exception>
        /// <exception cref="System.IO.IOException">when the one of the streams cannot be read correctly</exception>
        public static void CreatePdf(Stream svgStream, ISvgConverterProperties props, Stream pdfDest, WriterProperties
             writerprops) {
            //create doc
            PdfDocument pdfDocument;
            if (writerprops != null) {
                pdfDocument = new PdfDocument(new PdfWriter(pdfDest, writerprops));
            }
            else {
                pdfDocument = new PdfDocument(new PdfWriter(pdfDest));
            }
            //process
            ISvgNodeRenderer topSvgRenderer = Process(Parse(svgStream, props), props).GetRootRenderer();
            //Extract topmost dimensions
            CheckNull(topSvgRenderer);
            CheckNull(pdfDocument);
            float width = CssUtils.ParseAbsoluteLength(topSvgRenderer.GetAttribute(AttributeConstants.WIDTH));
            float height = CssUtils.ParseAbsoluteLength(topSvgRenderer.GetAttribute(AttributeConstants.HEIGHT));
            //adjust pagesize and create new page
            pdfDocument.SetDefaultPageSize(new PageSize(width, height));
            PdfPage page = pdfDocument.AddNewPage();
            PdfCanvas pageCanvas = new PdfCanvas(page);
            //Add to the first page
            PdfFormXObject xObject = ConvertToXObject(topSvgRenderer, pdfDocument, props);
            //Draw
            Draw(xObject, pageCanvas);
            pdfDocument.Close();
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <p>
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// .
        /// <p>
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// </summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// containing the PDF instructions
        /// corresponding to the passed SVG content
        /// </returns>
        public static PdfFormXObject ConvertToXObject(String content, PdfDocument document) {
            return ConvertToXObject(Process(Parse(content)).GetRootRenderer(), document);
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <p>
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// .
        /// <p>
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// </summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// containing the PDF instructions
        /// corresponding to the passed SVG content
        /// </returns>
        public static PdfFormXObject ConvertToXObject(String content, PdfDocument document, ISvgConverterProperties
             props) {
            return ConvertToXObject(Process(Parse(content), props).GetRootRenderer(), document);
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <p>
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// .
        /// <p>
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// </summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// containing the PDF instructions
        /// corresponding to the passed SVG content
        /// </returns>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static PdfFormXObject ConvertToXObject(Stream stream, PdfDocument document) {
            return ConvertToXObject(Process(Parse(stream)).GetRootRenderer(), document);
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <p>
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// .
        /// <p>
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// </summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// containing the PDF instructions
        /// corresponding to the passed SVG content
        /// </returns>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static PdfFormXObject ConvertToXObject(Stream stream, PdfDocument document, ISvgConverterProperties
             props) {
            return ConvertToXObject(Process(Parse(stream, props), props).GetRootRenderer(), document, props);
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <p>
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// .
        /// <p>
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// </summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Layout.Element.Image">Image</see>
        /// containing the PDF instructions
        /// corresponding to the passed SVG content
        /// </returns>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static Image ConvertToImage(Stream stream, PdfDocument document) {
            return new Image(ConvertToXObject(stream, document));
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Layout.Element.Image">image</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <p>
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// .
        /// <p>
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// </summary>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <returns>
        /// a
        /// <see cref="iText.Layout.Element.Image">Image</see>
        /// containing the PDF instructions
        /// corresponding to the passed SVG content
        /// </returns>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static Image ConvertToImage(Stream stream, PdfDocument document, ISvgConverterProperties props) {
            return new Image(ConvertToXObject(stream, document, props));
        }

        /*
        * This method is kept private, because there is little purpose in exposing it.
        */
        private static void Draw(PdfFormXObject pdfForm, PdfCanvas canvas) {
            canvas.AddXObject(pdfForm, 0, 0);
        }

        /*
        * This method is kept private, because there is little purpose in exposing it.
        */
        private static void Draw(PdfFormXObject pdfForm, PdfCanvas canvas, float x, float y) {
            canvas.AddXObject(pdfForm, x, y);
        }

        /// <summary>
        /// This method draws a NodeRenderer tree to a canvas that is tied to the
        /// passed document.
        /// </summary>
        /// <remarks>
        /// This method draws a NodeRenderer tree to a canvas that is tied to the
        /// passed document.
        /// <p>
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// .
        /// <p>
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// this method, or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// </remarks>
        /// <param name="topSvgRenderer">
        /// the
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// instance that contains
        /// the renderer tree
        /// </param>
        /// <param name="document">
        /// the document that the returned
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// can be drawn on (on any given page
        /// coordinates)
        /// </param>
        /// <returns>
        /// an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// containing the PDF instructions
        /// corresponding to the passed node renderer tree.
        /// </returns>
        public static PdfFormXObject ConvertToXObject(ISvgNodeRenderer topSvgRenderer, PdfDocument document) {
            return ConvertToXObject(topSvgRenderer, document, new DefaultSvgConverterProperties());
        }

        /// <summary>
        /// This method draws a NodeRenderer tree to a canvas that is tied to the
        /// passed document.
        /// </summary>
        /// <remarks>
        /// This method draws a NodeRenderer tree to a canvas that is tied to the
        /// passed document.
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// .
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// this method, or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>
        /// .
        /// </remarks>
        /// <param name="topSvgRenderer">
        /// the
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// instance that contains
        /// the renderer tree
        /// </param>
        /// <param name="document">the document that the returned</param>
        /// <param name="properties">
        /// the converter properties
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// can be drawn on (on any given page
        /// coordinates)
        /// </param>
        /// <returns>
        /// an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// containing the PDF instructions
        /// corresponding to the passed node renderer tree.
        /// </returns>
        public static PdfFormXObject ConvertToXObject(ISvgNodeRenderer topSvgRenderer, PdfDocument document, ISvgConverterProperties
             properties) {
            CheckNull(topSvgRenderer);
            CheckNull(document);
            float width = CssUtils.ParseAbsoluteLength(topSvgRenderer.GetAttribute(AttributeConstants.WIDTH));
            float height = CssUtils.ParseAbsoluteLength(topSvgRenderer.GetAttribute(AttributeConstants.HEIGHT));
            PdfFormXObject pdfForm = new PdfFormXObject(new Rectangle(0, 0, width, height));
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            SvgDrawContext context = new SvgDrawContext();
            if (properties == null) {
                properties = new DefaultSvgConverterProperties();
            }
            context.SetResourceResolver(properties.GetResourceResolver());
            context.PushCanvas(canvas);
            ISvgNodeRenderer root = new PdfRootSvgNodeRenderer(topSvgRenderer);
            root.Draw(context);
            return pdfForm;
        }

        /// <summary>
        /// Use the default implementation of
        /// <see cref="iText.Svg.Processors.ISvgProcessor"/>
        /// to convert an XML
        /// DOM tree to a node renderer tree.
        /// </summary>
        /// <param name="root">the XML DOM tree</param>
        /// <returns>a node renderer tree corresponding to the passed XML DOM tree</returns>
        public static ISvgProcessorResult Process(INode root) {
            CheckNull(root);
            ISvgProcessor processor = new DefaultSvgProcessor();
            return processor.Process(root);
        }

        /// <summary>
        /// Use the default implementation of
        /// <see cref="iText.Svg.Processors.ISvgProcessor"/>
        /// to convert an XML
        /// DOM tree to a node renderer tree. The passed properties can modify the default behaviour
        /// </summary>
        /// <param name="root">the XML DOM tree</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <returns>a node renderer tree corresponding to the passed XML DOM tree</returns>
        public static ISvgProcessorResult Process(INode root, ISvgConverterProperties props) {
            CheckNull(root);
            ISvgProcessor processor = new DefaultSvgProcessor();
            if (props == null) {
                return processor.Process(root);
            }
            else {
                return processor.Process(root, props);
            }
        }

        /// <summary>
        /// Parse a String containing valid SVG into an XML DOM node, using the
        /// default JSoup XML parser.
        /// </summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <returns>an XML DOM tree corresponding to the passed String input</returns>
        public static INode Parse(String content) {
            CheckNull(content);
            IHtmlParser xmlParser = new JsoupXmlParser();
            return xmlParser.Parse(content);
        }

        /// <summary>
        /// Parse a Stream containing valid SVG into an XML DOM node, using the
        /// default JSoup XML parser.
        /// </summary>
        /// <remarks>
        /// Parse a Stream containing valid SVG into an XML DOM node, using the
        /// default JSoup XML parser. This method will assume that the encoding of
        /// the Stream is
        /// <c>UTF-8</c>
        /// .
        /// </remarks>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <returns>an XML DOM tree corresponding to the passed String input</returns>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static INode Parse(Stream stream) {
            CheckNull(stream);
            return Parse(stream, null);
        }

        /// <summary>
        /// Parse a Stream containing valid SVG into an XML DOM node, using the
        /// default JSoup XML parser.
        /// </summary>
        /// <remarks>
        /// Parse a Stream containing valid SVG into an XML DOM node, using the
        /// default JSoup XML parser. This method will assume that the encoding of
        /// the Stream is
        /// <c>UTF-8</c>
        /// , unless specified otherwise by the method
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties.GetCharset()"/>
        /// of the
        /// <paramref name="props"/>
        /// parameter.
        /// </remarks>
        /// <param name="stream">the Stream object containing valid SVG content</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        /// <returns>an XML DOM tree corresponding to the passed String input</returns>
        /// <exception cref="System.IO.IOException">when the Stream cannot be read correctly</exception>
        public static INode Parse(Stream stream, ISvgConverterProperties props) {
            CheckNull(stream);
            // props is allowed to be null
            IHtmlParser xmlParser = new JsoupXmlParser();
            return xmlParser.Parse(stream, props != null ? props.GetCharset() : null);
        }
    }
}
