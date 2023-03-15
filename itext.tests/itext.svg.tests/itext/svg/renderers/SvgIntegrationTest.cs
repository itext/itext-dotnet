/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Svg.Converter;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Test;

namespace iText.Svg.Renderers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SvgIntegrationTest : ExtendedITextTest {
        public virtual void Convert(Stream svg, Stream pdfOutputStream) {
            PdfDocument doc = new PdfDocument(new PdfWriter(pdfOutputStream, new WriterProperties().SetCompressionLevel
                (0)));
            doc.AddNewPage();
            SvgConverter.DrawOnDocument(svg, doc, 1);
            doc.Close();
        }

        public virtual void Convert(String svg, String output) {
            Convert(svg, output, PageSize.DEFAULT);
        }

        public virtual void Convert(String svg, String output, PageSize size) {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(output, new WriterProperties().SetCompressionLevel(
                0)))) {
                doc.AddNewPage(size);
                ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(svg);
                SvgConverter.DrawOnDocument(new FileStream(svg, FileMode.Open, FileAccess.Read), doc, 1, properties);
            }
        }

        public static PdfDocument ConvertWithResult(String svg, String output) {
            PdfDocument doc = new PdfDocument(new PdfWriter(output, new WriterProperties().SetCompressionLevel(0)));
            doc.AddNewPage();
            ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(svg);
            SvgConverter.DrawOnDocument(new FileStream(svg, FileMode.Open, FileAccess.Read), doc, 1, properties);
            return doc;
        }

        public virtual void ConvertToSinglePage(Stream svg, Stream pdfOutputStream) {
            WriterProperties writerprops = new WriterProperties().SetCompressionLevel(0);
            SvgConverter.CreatePdf(svg, pdfOutputStream, writerprops);
        }

        public virtual void ConvertToSinglePage(FileInfo svg, FileInfo pdf) {
            SvgConverter.CreatePdf(svg, pdf);
        }

        public virtual void ConvertToSinglePage(FileInfo svg, FileInfo pdf, ISvgConverterProperties properties) {
            SvgConverter.CreatePdf(svg, pdf, properties);
        }

        public virtual void ConvertToSinglePage(FileInfo svg, FileInfo pdf, ISvgConverterProperties properties, WriterProperties
             writerProperties) {
            SvgConverter.CreatePdf(svg, pdf, properties, writerProperties);
        }

        public virtual void ConvertToSinglePage(FileInfo svg, FileInfo pdf, WriterProperties writerProperties) {
            SvgConverter.CreatePdf(svg, pdf, writerProperties);
        }

        public virtual void ConvertToSinglePage(Stream svg, Stream pdfOutputStream, ISvgConverterProperties properties
            ) {
            SvgConverter.CreatePdf(svg, pdfOutputStream, properties);
        }

        public virtual void ConvertToSinglePage(Stream svg, Stream pdfOutputStream, ISvgConverterProperties properties
            , WriterProperties writerprops) {
            SvgConverter.CreatePdf(svg, pdfOutputStream, properties, writerprops);
        }

        public virtual void ConvertAndCompare(String src, String dest, String fileName) {
            ConvertAndCompare(src, dest, fileName, PageSize.DEFAULT);
        }

        public virtual void ConvertAndCompare(String src, String dest, String fileName, PageSize size) {
            Convert(src + fileName + ".svg", dest + fileName + ".pdf", size);
            Compare(fileName, src, dest);
        }

        public virtual void ConvertAndCompareSinglePage(String src, String dest, String fileName) {
            ConvertToSinglePage(new FileStream(src + fileName + ".svg", FileMode.Open, FileAccess.Read), new FileStream
                (dest + fileName + ".pdf", FileMode.Create));
            Compare(fileName, src, dest);
        }

        public virtual void ConvertAndCompareSinglePage(String src, String dest, String fileName, ISvgConverterProperties
             properties) {
            ConvertToSinglePage(new FileStream(src + fileName + ".svg", FileMode.Open, FileAccess.Read), new FileStream
                (dest + fileName + ".pdf", FileMode.Create), properties);
            Compare(fileName, src, dest);
        }

        protected internal virtual void Compare(String filename, String sourceFolder, String destinationFolder) {
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename + ".pdf", sourceFolder
                 + "cmp_" + filename + ".pdf", destinationFolder, "diff_"));
        }
    }
}
