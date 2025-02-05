/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Exceptions;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;
using iText.Svg.Utils;

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

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Svg.Converter.SvgConverter
            ));

        private static void CheckNull(Object o) {
            if (o == null) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.PARAMETER_CANNOT_BE_NULL);
            }
        }

        /// <summary>
        /// Draws a String containing valid SVG to a document, on a given page
        /// number at the origin of the page.
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
        /// number on the provided x and y coordinate.
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
        /// number on the provided x and y coordinate.
        /// </summary>
        /// <param name="content">the String value containing valid SVG content</param>
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
        /// number on the provided x and y coordinate.
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
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnDocument(String content, PdfDocument document, int pageNo, float x, float y, ISvgConverterProperties
             props) {
            CheckNull(document);
            DrawOnPage(content, document.GetPage(pageNo), x, y, props);
        }

        /// <summary>
        /// Draws a Stream containing valid SVG to a document, on a given page
        /// number ate the origni of the page.
        /// </summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing valid SVG content
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        public static void DrawOnDocument(Stream stream, PdfDocument document, int pageNo) {
            DrawOnDocument(stream, document, pageNo, 0, 0);
        }

        /// <summary>
        /// Draws a Stream containing valid SVG to a document, on a given page
        /// number on the provided x and y coordinate.
        /// </summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing valid SVG content
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        public static void DrawOnDocument(Stream stream, PdfDocument document, int pageNo, float x, float y) {
            CheckNull(document);
            DrawOnPage(stream, document.GetPage(pageNo), x, y);
        }

        /// <summary>
        /// Draws a Stream containing valid SVG to a document, on a given page
        /// number on the provided x and y coordinate.
        /// </summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing valid SVG content
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnDocument(Stream stream, PdfDocument document, int pageNo, ISvgConverterProperties
             props) {
            DrawOnDocument(stream, document, pageNo, 0, 0, props);
        }

        /// <summary>
        /// Draws a Stream containing valid SVG to a document, on a given page
        /// number on the provided x and y coordinate.
        /// </summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing valid SVG content
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="pageNo">the page to draw on</param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnDocument(Stream stream, PdfDocument document, int pageNo, float x, float y, ISvgConverterProperties
             props) {
            CheckNull(document);
            DrawOnPage(stream, document.GetPage(pageNo), x, y, props);
        }

        /// <summary>Draws a String containing valid SVG to a given page at the origin of the page.</summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        public static void DrawOnPage(String content, PdfPage page) {
            DrawOnPage(content, page, 0, 0);
        }

        /// <summary>Draws a String containing valid SVG to a given page on the provided x and y coordinate.</summary>
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

        /// <summary>Draws a String containing valid SVG to a given page on the provided x and y coordinate.</summary>
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

        /// <summary>Draws a String containing valid SVG to a given page on the provided x and y coordinate.</summary>
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

        /// <summary>Draws a Stream containing valid SVG to a given page at coordinate 0,0.</summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// object containing valid SVG content
        /// </param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        public static void DrawOnPage(Stream stream, PdfPage page) {
            DrawOnPage(stream, page, 0, 0);
        }

        /// <summary>Draws a Stream containing valid SVG to a given page, at a given location.</summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// object containing valid SVG content
        /// </param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        public static void DrawOnPage(Stream stream, PdfPage page, float x, float y) {
            CheckNull(page);
            DrawOnCanvas(stream, new PdfCanvas(page), x, y);
        }

        /// <summary>Draws a Stream containing valid SVG to a given page at a given location.</summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// object containing valid SVG content
        /// </param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnPage(Stream stream, PdfPage page, ISvgConverterProperties props) {
            DrawOnPage(stream, page, 0, 0, props);
        }

        /// <summary>Draws a Stream containing valid SVG to a given page at a given location.</summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// object containing valid SVG content
        /// </param>
        /// <param name="page">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnPage(Stream stream, PdfPage page, float x, float y, ISvgConverterProperties props
            ) {
            CheckNull(page);
            if (props is SvgConverterProperties && ((SvgConverterProperties)props).GetCustomViewport() == null) {
                ((SvgConverterProperties)props).SetCustomViewport(page.GetMediaBox());
            }
            DrawOnCanvas(stream, new PdfCanvas(page), x, y, props);
        }

        /// <summary>Draws a String containing valid SVG to a pre-made canvas object.</summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        public static void DrawOnCanvas(String content, PdfCanvas canvas) {
            DrawOnCanvas(content, canvas, 0, 0);
        }

        /// <summary>Draws a String containing valid SVG to a pre-made canvas object.</summary>
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

        /// <summary>Draws a String containing valid SVG to a pre-made canvas object.</summary>
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

        /// <summary>draws a String containing valid SVG to a pre-made canvas object, at a specified location.</summary>
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

        /// <summary>Draws a Stream containing valid SVG to a pre-made canvas object.</summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// object containing valid SVG content
        /// </param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        public static void DrawOnCanvas(Stream stream, PdfCanvas canvas) {
            DrawOnCanvas(stream, canvas, 0, 0);
        }

        /// <summary>Draws a Stream containing valid SVG to a pre-made canvas object, to a specified location.</summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// object containing valid SVG content
        /// </param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        public static void DrawOnCanvas(Stream stream, PdfCanvas canvas, float x, float y) {
            CheckNull(canvas);
            Draw(ConvertToXObject(stream, canvas.GetDocument()), canvas, x, y);
        }

        /// <summary>Draws a Stream containing valid SVG to a pre-made canvas object.</summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// object containing valid SVG content
        /// </param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnCanvas(Stream stream, PdfCanvas canvas, ISvgConverterProperties props) {
            DrawOnCanvas(stream, canvas, 0, 0, props);
        }

        /// <summary>Draws a String containing valid SVG to a pre-made canvas object, at a specified position on the canvas.
        ///     </summary>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// object containing valid SVG content
        /// </param>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>
        /// instance to draw on
        /// </param>
        /// <param name="x">x-coordinate of the location to draw at</param>
        /// <param name="y">y-coordinate of the location to draw at</param>
        /// <param name="props">a container for extra properties that customize the behavior</param>
        public static void DrawOnCanvas(Stream stream, PdfCanvas canvas, float x, float y, ISvgConverterProperties
             props) {
            CheckNull(canvas);
            Draw(ConvertToXObject(stream, canvas.GetDocument(), props), canvas, x, y);
        }

        /// <summary>
        /// Converts SVG stored in a
        /// <see cref="System.IO.FileInfo"/>
        /// to a PDF
        /// <see cref="System.IO.FileInfo"/>.
        /// </summary>
        /// <param name="svgFile">
        /// the
        /// <see cref="System.IO.FileInfo"/>
        /// containing the source SVG
        /// </param>
        /// <param name="pdfFile">
        /// the
        /// <see cref="System.IO.FileInfo"/>
        /// containing the resulting PDF
        /// </param>
        public static void CreatePdf(FileInfo svgFile, FileInfo pdfFile) {
            CreatePdf(svgFile, pdfFile, null, null);
        }

        /// <summary>
        /// Converts SVG stored in a
        /// <see cref="System.IO.FileInfo"/>
        /// to a PDF
        /// <see cref="System.IO.FileInfo"/>
        /// ,
        /// using specific
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>.
        /// </summary>
        /// <param name="svgFile">
        /// the
        /// <see cref="System.IO.FileInfo"/>
        /// containing the source SVG
        /// </param>
        /// <param name="pdfFile">
        /// the
        /// <see cref="System.IO.FileInfo"/>
        /// containing the resulting PDF
        /// </param>
        /// <param name="props">
        /// a
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        public static void CreatePdf(FileInfo svgFile, FileInfo pdfFile, ISvgConverterProperties props) {
            CreatePdf(svgFile, pdfFile, props, null);
        }

        /// <summary>
        /// Converts SVG stored in a
        /// <see cref="System.IO.FileInfo"/>
        /// to a PDF
        /// <see cref="System.IO.FileInfo"/>
        /// ,
        /// using
        /// <see cref="iText.Kernel.Pdf.WriterProperties"/>
        /// </summary>
        /// <param name="svgFile">
        /// the
        /// <see cref="System.IO.FileInfo"/>
        /// containing the source SVG
        /// </param>
        /// <param name="pdfFile">
        /// the
        /// <see cref="System.IO.FileInfo"/>
        /// containing the resulting PDF
        /// </param>
        /// <param name="writerProps">
        /// the
        /// <see cref="iText.Kernel.Pdf.WriterProperties"/>
        /// for the pdf document
        /// </param>
        public static void CreatePdf(FileInfo svgFile, FileInfo pdfFile, WriterProperties writerProps) {
            CreatePdf(svgFile, pdfFile, null, writerProps);
        }

        /// <summary>
        /// Converts SVG stored in a
        /// <see cref="System.IO.FileInfo"/>
        /// to a PDF
        /// <see cref="System.IO.FileInfo"/>
        /// ,
        /// using specific
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.WriterProperties"/>.
        /// </summary>
        /// <param name="svgFile">
        /// the
        /// <see cref="System.IO.FileInfo"/>
        /// containing the source SVG
        /// </param>
        /// <param name="pdfFile">
        /// the
        /// <see cref="System.IO.FileInfo"/>
        /// containing the resulting PDF
        /// </param>
        /// <param name="props">
        /// a
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        /// <param name="writerProps">
        /// a
        /// <see cref="iText.Kernel.Pdf.WriterProperties"/>
        /// for the pdf document
        /// </param>
        public static void CreatePdf(FileInfo svgFile, FileInfo pdfFile, ISvgConverterProperties props, WriterProperties
             writerProps) {
            if (props == null) {
                props = new SvgConverterProperties().SetBaseUri(FileUtil.GetParentDirectoryUri(svgFile));
            }
            else {
                if (props.GetBaseUri() == null || String.IsNullOrEmpty(props.GetBaseUri())) {
                    String baseUri = FileUtil.GetParentDirectoryUri(svgFile);
                    props = ConvertToSvgConverterProps(props, baseUri);
                }
            }
            using (Stream fileInputStream = FileUtil.GetInputStreamForFile(svgFile.FullName)) {
                using (Stream fileOutputStream = FileUtil.GetFileOutputStream(pdfFile.FullName)) {
                    CreatePdf(fileInputStream, fileOutputStream, props, writerProps);
                }
            }
        }

        /// <summary>Copies properties from custom ISvgConverterProperties into new SvgConverterProperties.</summary>
        /// <remarks>
        /// Copies properties from custom ISvgConverterProperties into new SvgConverterProperties.
        /// Since ISvgConverterProperties itself is immutable we have to do it.
        /// </remarks>
        /// <param name="props">
        /// 
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        /// <param name="baseUri">the directory of new SvgConverterProperties</param>
        /// <returns>new SvgConverterProperties.</returns>
        private static SvgConverterProperties ConvertToSvgConverterProps(ISvgConverterProperties props, String baseUri
            ) {
            return new SvgConverterProperties().SetBaseUri(baseUri).SetMediaDeviceDescription(props.GetMediaDeviceDescription
                ()).SetFontProvider(props.GetFontProvider()).SetCharset(props.GetCharset()).SetRendererFactory(props.GetRendererFactory
                ());
        }

        /// <summary>Create a single page pdf containing the SVG on its page using the default processing and drawing logic
        ///     </summary>
        /// <param name="svgStream">
        /// 
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing the SVG
        /// </param>
        /// <param name="pdfDest">PDF destination outputStream</param>
        public static void CreatePdf(Stream svgStream, Stream pdfDest) {
            CreatePdf(svgStream, pdfDest, null, null);
        }

        /// <summary>Create a single page pdf containing the SVG on its page using the default processing and drawing logic
        ///     </summary>
        /// <param name="svgStream">
        /// 
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing the SVG
        /// </param>
        /// <param name="pdfDest">PDF destination outputStream</param>
        /// <param name="writerProps">writer properties for the pdf document</param>
        public static void CreatePdf(Stream svgStream, Stream pdfDest, WriterProperties writerProps) {
            CreatePdf(svgStream, pdfDest, null, writerProps);
        }

        /// <summary>Create a single page pdf containing the SVG on its page using the default processing and drawing logic
        ///     </summary>
        /// <param name="svgStream">
        /// 
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing the SVG
        /// </param>
        /// <param name="pdfDest">PDF destination outputStream</param>
        /// <param name="props">
        /// 
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        public static void CreatePdf(Stream svgStream, Stream pdfDest, ISvgConverterProperties props) {
            CreatePdf(svgStream, pdfDest, props, null);
        }

        /// <summary>Create a single page pdf containing the SVG on its page using the default processing and drawing logic
        ///     </summary>
        /// <param name="svgStream">
        /// 
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing the SVG
        /// </param>
        /// <param name="pdfDest">PDF destination outputStream</param>
        /// <param name="props">
        /// 
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        /// <param name="writerProps">
        /// 
        /// <see cref="iText.Kernel.Pdf.WriterProperties"/>
        /// for the pdf document
        /// </param>
        public static void CreatePdf(Stream svgStream, Stream pdfDest, ISvgConverterProperties props, WriterProperties
             writerProps) {
            // Create doc
            if (writerProps == null) {
                writerProps = new WriterProperties();
            }
            using (PdfWriter writer = new PdfWriter(pdfDest, writerProps)) {
                using (PdfDocument pdfDocument = new PdfDocument(writer)) {
                    // Process
                    ISvgProcessorResult processorResult = Process(Parse(svgStream, props), props);
                    ResourceResolver resourceResolver = iText.Svg.Converter.SvgConverter.GetResourceResolver(processorResult, 
                        props);
                    SvgDrawContext drawContext = new SvgDrawContext(resourceResolver, processorResult.GetFontProvider());
                    if (processorResult is SvgProcessorResult) {
                        drawContext.SetCssContext(((SvgProcessorResult)processorResult).GetContext().GetCssContext());
                    }
                    drawContext.AddNamedObjects(processorResult.GetNamedObjects());
                    // Add temp fonts
                    drawContext.SetTempFonts(processorResult.GetTempFonts());
                    ISvgNodeRenderer topSvgRenderer = processorResult.GetRootRenderer();
                    // Extract topmost dimensions
                    CheckNull(topSvgRenderer);
                    CheckNull(pdfDocument);
                    // Since svg is a single object in the document, em = rem
                    float em = drawContext.GetCssContext().GetRootFontSize();
                    Rectangle wh = SvgCssUtils.ExtractWidthAndHeight(topSvgRenderer, em, drawContext);
                    // Adjust pagesize and create new page
                    pdfDocument.SetDefaultPageSize(new PageSize(wh.GetWidth(), wh.GetHeight()));
                    PdfPage page = pdfDocument.AddNewPage();
                    PdfCanvas pageCanvas = new PdfCanvas(page);
                    // Add to the first page
                    PdfFormXObject xObject = ConvertToXObject(topSvgRenderer, pdfDocument, drawContext);
                    // Draw
                    Draw(xObject, pageCanvas);
                }
            }
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <para />
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// <para />
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode, iText.Svg.Processors.ISvgConverterProperties)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>.
        /// </remarks>
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
            return ConvertToXObject(content, document, null);
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <para />
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// <para />
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode, iText.Svg.Processors.ISvgConverterProperties)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>.
        /// </remarks>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">
        /// 
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// containing the PDF instructions
        /// corresponding to the passed SVG content
        /// </returns>
        public static PdfFormXObject ConvertToXObject(String content, PdfDocument document, ISvgConverterProperties
             props) {
            CheckNull(content);
            CheckNull(document);
            return ConvertToXObject(Process(Parse(content), props), document, props);
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <para />
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// <para />
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode, iText.Svg.Processors.ISvgConverterProperties)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>.
        /// </remarks>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing valid SVG content
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">
        /// 
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// containing the PDF instructions
        /// corresponding to the passed SVG content
        /// </returns>
        public static PdfFormXObject ConvertToXObject(Stream stream, PdfDocument document, ISvgConverterProperties
             props) {
            CheckNull(stream);
            CheckNull(document);
            return ConvertToXObject(Process(Parse(stream, props), props), document, props);
        }

        //Private converter for unification
        private static PdfFormXObject ConvertToXObject(ISvgProcessorResult processorResult, PdfDocument document, 
            ISvgConverterProperties props) {
            ResourceResolver resourceResolver = iText.Svg.Converter.SvgConverter.GetResourceResolver(processorResult, 
                props);
            SvgDrawContext drawContext = new SvgDrawContext(resourceResolver, processorResult.GetFontProvider());
            if (processorResult is SvgProcessorResult) {
                drawContext.SetCssContext(((SvgProcessorResult)processorResult).GetContext().GetCssContext());
            }
            if (props is SvgConverterProperties) {
                drawContext.SetCustomViewport(((SvgConverterProperties)props).GetCustomViewport());
            }
            drawContext.SetTempFonts(processorResult.GetTempFonts());
            drawContext.AddNamedObjects(processorResult.GetNamedObjects());
            return ConvertToXObject(processorResult.GetRootRenderer(), document, drawContext);
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <para />
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// <para />
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode, iText.Svg.Processors.ISvgConverterProperties)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>.
        /// </remarks>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing valid SVG content
        /// </param>
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
        public static PdfFormXObject ConvertToXObject(Stream stream, PdfDocument document) {
            return ConvertToXObject(stream, document, null);
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <para />
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// <para />
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode, iText.Svg.Processors.ISvgConverterProperties)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>.
        /// </remarks>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing valid SVG content
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Layout.Element.Image">Image</see>
        /// containing the PDF instructions corresponding to the passed SVG content
        /// </returns>
        public static Image ConvertToImage(Stream stream, PdfDocument document) {
            return new Image(ConvertToXObject(stream, document));
        }

        /// <summary>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Layout.Element.Image">image</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Converts a String containing valid SVG content to an
        /// <see cref="iText.Layout.Element.Image">image</see>
        /// that can then be used on the passed
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . This method does NOT manipulate the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// in any way.
        /// <para />
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// <para />
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode, iText.Svg.Processors.ISvgConverterProperties)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// <see cref="ConvertToXObject(iText.Svg.Renderers.ISvgNodeRenderer, iText.Kernel.Pdf.PdfDocument)"/>
        /// , or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>.
        /// </remarks>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing valid SVG content
        /// </param>
        /// <param name="document">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance to draw on
        /// </param>
        /// <param name="props">
        /// 
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Layout.Element.Image">Image</see>
        /// containing the PDF instructions corresponding to the passed SVG content
        /// </returns>
        public static Image ConvertToImage(Stream stream, PdfDocument document, ISvgConverterProperties props) {
            return new Image(ConvertToXObject(stream, document, props));
        }

        /*
        * This method is kept private, because there is little purpose in exposing it.
        */
        private static void Draw(PdfFormXObject pdfForm, PdfCanvas canvas) {
            Draw(pdfForm, canvas, 0, 0);
        }

//\cond DO_NOT_DOCUMENT
        /*
        * This method is kept private, because there is little purpose in exposing it.
        */
        internal static void Draw(PdfFormXObject pdfForm, PdfCanvas canvas, float x, float y) {
            canvas.AddXObjectAt(pdfForm, x + (pdfForm.GetBBox() == null ? 0 : pdfForm.GetBBox().GetAsNumber(0).FloatValue
                ()), y + (pdfForm.GetBBox() == null ? 0 : pdfForm.GetBBox().GetAsNumber(1).FloatValue()));
        }
//\endcond

        /// <summary>
        /// This method draws a NodeRenderer tree to a canvas that is tied to the
        /// passed document.
        /// </summary>
        /// <remarks>
        /// This method draws a NodeRenderer tree to a canvas that is tied to the
        /// passed document.
        /// <para />
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// <para />
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode, iText.Svg.Processors.ISvgConverterProperties)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// this method, or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>.
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
            return ConvertToXObject(topSvgRenderer, document, new SvgDrawContext(null, null));
        }

        /// <summary>
        /// This method draws a NodeRenderer tree to a canvas that is tied to the
        /// passed document.
        /// </summary>
        /// <remarks>
        /// This method draws a NodeRenderer tree to a canvas that is tied to the
        /// passed document.
        /// <para />
        /// This method (or its overloads) is the best method to use if you want to
        /// reuse the same SVG image multiple times on the same
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// <para />
        /// If you want to reuse this object on other
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instances,
        /// please either use any of the
        /// <see cref="Process(iText.StyledXmlParser.Node.INode, iText.Svg.Processors.ISvgConverterProperties)"/>
        /// overloads in this same
        /// class and convert its result to an XObject with
        /// this method, or look into
        /// using
        /// <see cref="iText.Kernel.Pdf.PdfObject.CopyTo(iText.Kernel.Pdf.PdfDocument)"/>.
        /// </remarks>
        /// <param name="topSvgRenderer">
        /// the
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// instance that contains
        /// the renderer tree
        /// </param>
        /// <param name="document">the document that the returned</param>
        /// <param name="context">the SvgDrawContext</param>
        /// <returns>
        /// an
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject">XObject</see>
        /// containing the PDF instructions
        /// corresponding to the passed node renderer tree.
        /// </returns>
        private static PdfFormXObject ConvertToXObject(ISvgNodeRenderer topSvgRenderer, PdfDocument document, SvgDrawContext
             context) {
            CheckNull(topSvgRenderer);
            CheckNull(document);
            CheckNull(context);
            // Can't determine em value here, so em=rem
            float em = context.GetCssContext().GetRootFontSize();
            Rectangle bbox = SvgCssUtils.ExtractWidthAndHeight(topSvgRenderer, em, context);
            PdfFormXObject pdfForm = new PdfFormXObject(bbox);
            PdfCanvas canvas = new PdfCanvas(pdfForm, document);
            context.PushCanvas(canvas);
            ISvgNodeRenderer root = new PdfRootSvgNodeRenderer(topSvgRenderer);
            root.Draw(context);
            return pdfForm;
        }

        /// <summary>
        /// Parse and process an Inputstream containing an SVG, using the default Svg processor (
        /// <see cref="iText.Svg.Processors.Impl.DefaultSvgProcessor"/>
        /// )
        /// The parsing of the stream is done using UTF-8 as the default charset.
        /// </summary>
        /// <remarks>
        /// Parse and process an Inputstream containing an SVG, using the default Svg processor (
        /// <see cref="iText.Svg.Processors.Impl.DefaultSvgProcessor"/>
        /// )
        /// The parsing of the stream is done using UTF-8 as the default charset.
        /// The properties used by the processor are the
        /// <see cref="iText.Svg.Processors.Impl.SvgConverterProperties"/>
        /// </remarks>
        /// <param name="svgStream">
        /// 
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing the SVG to parse and process
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Svg.Processors.ISvgProcessorResult"/>
        /// containing the root renderer and metadata of the svg
        /// </returns>
        public static ISvgProcessorResult ParseAndProcess(Stream svgStream) {
            return ParseAndProcess(svgStream, null);
        }

        /// <summary>
        /// Parse and process an Inputstream containing an SVG, using the default Svg processor (
        /// <see cref="iText.Svg.Processors.Impl.DefaultSvgProcessor"/>
        /// )
        /// </summary>
        /// <param name="svgStream">
        /// 
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing the SVG to parse and process
        /// </param>
        /// <param name="props">
        /// 
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Svg.Processors.ISvgProcessorResult"/>
        /// containing the root renderer and metadata of the svg
        /// </returns>
        public static ISvgProcessorResult ParseAndProcess(Stream svgStream, ISvgConverterProperties props) {
            IXmlParser parser = new JsoupXmlParser();
            String charset = iText.Svg.Converter.SvgConverter.TryToExtractCharset(props);
            INode nodeTree;
            try {
                nodeTree = parser.Parse(svgStream, charset);
            }
            catch (Exception e) {
                throw new SvgProcessingException(SvgExceptionMessageConstant.FAILED_TO_PARSE_INPUTSTREAM, e);
            }
            return new DefaultSvgProcessor().Process(nodeTree, props);
        }

        /// <summary>
        /// Use the default implementation of
        /// <see cref="iText.Svg.Processors.ISvgProcessor"/>
        /// to convert an XML
        /// DOM tree to a node renderer tree.
        /// </summary>
        /// <remarks>
        /// Use the default implementation of
        /// <see cref="iText.Svg.Processors.ISvgProcessor"/>
        /// to convert an XML
        /// DOM tree to a node renderer tree. The passed properties can modify the default behaviour
        /// </remarks>
        /// <param name="root">the XML DOM tree</param>
        /// <param name="props">
        /// 
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        /// <returns>a node renderer tree corresponding to the passed XML DOM tree</returns>
        public static ISvgProcessorResult Process(INode root, ISvgConverterProperties props) {
            CheckNull(root);
            return new DefaultSvgProcessor().Process(root, props);
        }

        /// <summary>
        /// Parse a String containing valid SVG into an XML DOM node, using the
        /// default JSoup XML parser.
        /// </summary>
        /// <param name="content">the String value containing valid SVG content</param>
        /// <returns>an XML DOM tree corresponding to the passed String input</returns>
        public static INode Parse(String content) {
            CheckNull(content);
            return new JsoupXmlParser().Parse(content);
        }

        /// <summary>
        /// Parse a Stream containing valid SVG into an XML DOM node, using the
        /// default JSoup XML parser.
        /// </summary>
        /// <remarks>
        /// Parse a Stream containing valid SVG into an XML DOM node, using the
        /// default JSoup XML parser. This method will assume that the encoding of
        /// the Stream is
        /// <c>UTF-8</c>.
        /// </remarks>
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing valid SVG content
        /// </param>
        /// <returns>an XML DOM tree corresponding to the passed String input</returns>
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
        /// <param name="stream">
        /// the
        /// <see cref="System.IO.Stream">Stream</see>
        /// containing valid SVG content
        /// </param>
        /// <param name="props">
        /// 
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        /// <returns>an XML DOM tree corresponding to the passed String input</returns>
        public static INode Parse(Stream stream, ISvgConverterProperties props) {
            CheckNull(stream);
            // props is allowed to be null
            IXmlParser xmlParser = new JsoupXmlParser();
            return xmlParser.Parse(stream, iText.Svg.Converter.SvgConverter.TryToExtractCharset(props));
        }

        /// <summary>
        /// Extract width and height of the passed SVGNodeRenderer,
        /// defaulting to respective viewbox values if either one is not present or
        /// to browser default if viewbox is missing as well
        /// </summary>
        /// <remarks>
        /// Extract width and height of the passed SVGNodeRenderer,
        /// defaulting to respective viewbox values if either one is not present or
        /// to browser default if viewbox is missing as well
        /// <para />
        /// Deprecated in favour of
        /// <see cref="iText.Svg.Utils.SvgCssUtils.ExtractWidthAndHeight(iText.Svg.Renderers.ISvgNodeRenderer, float, iText.Svg.Renderers.SvgDrawContext)
        ///     "/>
        /// </remarks>
        /// <param name="topSvgRenderer">
        /// the
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// instance that contains
        /// the renderer tree
        /// </param>
        /// <returns>float[2], width is in position 0, height in position 1</returns>
        [Obsolete]
        public static float[] ExtractWidthAndHeight(ISvgNodeRenderer topSvgRenderer) {
            SvgDrawContext context = new SvgDrawContext(null, null);
            float em = context.GetCssContext().GetRootFontSize();
            Rectangle rectangle = SvgCssUtils.ExtractWidthAndHeight(topSvgRenderer, em, context);
            return new float[] { rectangle.GetX(), rectangle.GetY(), rectangle.GetWidth(), rectangle.GetHeight() };
        }

//\cond DO_NOT_DOCUMENT
        internal static ResourceResolver GetResourceResolver(ISvgProcessorResult processorResult, ISvgConverterProperties
             props) {
            if (processorResult is SvgProcessorResult) {
                return ((SvgProcessorResult)processorResult).GetContext().GetResourceResolver();
            }
            return CreateResourceResolver(props);
        }
//\endcond

        /// <summary>
        /// Tries to extract charset from
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>.
        /// </summary>
        /// <param name="props">
        /// 
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// an instance for extra properties to customize the behavior
        /// </param>
        /// <returns>charset | null</returns>
        private static String TryToExtractCharset(ISvgConverterProperties props) {
            return props != null ? props.GetCharset() : null;
        }

        private static ResourceResolver CreateResourceResolver(ISvgConverterProperties props) {
            if (props == null) {
                return new ResourceResolver(null);
            }
            return new ResourceResolver(props.GetBaseUri(), props.GetResourceRetriever());
        }
    }
}
