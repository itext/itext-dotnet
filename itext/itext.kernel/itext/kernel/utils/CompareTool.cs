/*

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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using iText.IO.Font;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;

namespace iText.Kernel.Utils {
    /// <summary>
    /// This class provides means to compare two PDF files both by content and visually
    /// and gives the report of their differences.
    /// </summary>
    /// <remarks>
    /// This class provides means to compare two PDF files both by content and visually
    /// and gives the report of their differences.
    /// <br/><br/>
    /// For visual comparison it uses external tools: Ghostscript and ImageMagick, which
    /// should be installed on your machine. To allow CompareTool to use them, you need
    /// to pass either java properties or environment variables with names "gsExec" and
    /// "compareExec", which would contain the paths to the executables of correspondingly
    /// Ghostscript and ImageMagick tools.
    /// <br/><br/>
    /// CompareTool class was mainly designed for the testing purposes of iText in order to
    /// ensure that the same code produces the same PDF document. For this reason you will
    /// often encounter such parameter names as "outDoc" and "cmpDoc" which stand for output
    /// document and document-for-comparison. The first one is viewed as the current result,
    /// and the second one is referred as normal or ideal result. OutDoc is compared to the
    /// ideal cmpDoc. Therefore all reports of the comparison are in the form: "Expected ...,
    /// but was ...". This should be interpreted in the following way: "expected" part stands
    /// for the content of the cmpDoc and "but was" part stands for the content of the outDoc.
    /// </remarks>
    public class CompareTool {
        private const String cannotOpenOutputDirectory = "Cannot open output directory for <filename>.";

        private const String gsFailed = "GhostScript failed for <filename>.";

        private const String unexpectedNumberOfPages = "Unexpected number of pages for <filename>.";

        private const String differentPages = "File <filename> differs on page <pagenumber>.";

        private const String undefinedGsPath = "Path to GhostScript is not specified. Please use -DgsExec=<path_to_ghostscript> (e.g. -DgsExec=\"C:/Program Files/gs/gs9.14/bin/gswin32c.exe\")";

        private const String ignoredAreasPrefix = "ignored_areas_";

        private const String gsParams = " -dNOPAUSE -dBATCH -sDEVICE=png16m -r150 -sOutputFile=<outputfile> <inputfile>";

        private const String compareParams = " \"<image1>\" \"<image2>\" \"<difference>\"";

        private String gsExec;

        private String compareExec;

        private String cmpPdf;

        private String cmpPdfName;

        private String cmpImage;

        private String outPdf;

        private String outPdfName;

        private String outImage;

        private ReaderProperties outProps;

        private ReaderProperties cmpProps;

        private IList<PdfIndirectReference> outPagesRef;

        private IList<PdfIndirectReference> cmpPagesRef;

        private int compareByContentErrorsLimit = 1;

        private bool generateCompareByContentXmlReport = false;

        private bool encryptionCompareEnabled = false;

        private bool useCachedPagesForComparison = true;

        /// <summary>Creates an instance of the CompareTool.</summary>
        public CompareTool() {
            gsExec = iText.IO.Util.SystemUtil.GetEnvironmentVariable("gsExec");
            compareExec = iText.IO.Util.SystemUtil.GetEnvironmentVariable("compareExec");
        }

        /// <summary>
        /// Compares two PDF documents by content starting from Catalog dictionary and then recursively comparing
        /// corresponding objects which are referenced from it.
        /// </summary>
        /// <remarks>
        /// Compares two PDF documents by content starting from Catalog dictionary and then recursively comparing
        /// corresponding objects which are referenced from it. You can roughly imagine it as depth-first traversal
        /// of the two trees that represent pdf objects structure of the documents.
        /// <br/><br/>
        /// The main difference between this method and the
        /// <see cref="CompareByContent(System.String, System.String, System.String, System.String)"/>
        /// methods is the return value. This method returns a
        /// <see cref="CompareResult"/>
        /// class instance, which could be used
        /// in code, however compareByContent methods in case of the differences simply return String value, which could
        /// only be printed. Also, keep in mind that this method doesn't perform visual comparison of the documents.
        /// <br/><br/>
        /// For more explanations about what is outDoc and cmpDoc see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outDocument">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpDocument">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <returns>
        /// the report of comparison of two files in the form of the custom class instance.
        /// See
        /// <see cref="CompareResult"/>
        /// for more info.
        /// </returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual CompareTool.CompareResult CompareByCatalog(PdfDocument outDocument, PdfDocument cmpDocument
            ) {
            CompareTool.CompareResult compareResult = null;
            compareResult = new CompareTool.CompareResult(this, compareByContentErrorsLimit);
            CompareTool.ObjectPath catalogPath = new CompareTool.ObjectPath(cmpDocument.GetCatalog().GetPdfObject().GetIndirectReference
                (), outDocument.GetCatalog().GetPdfObject().GetIndirectReference());
            ICollection<PdfName> ignoredCatalogEntries = new LinkedHashSet<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList
                (PdfName.Metadata));
            CompareDictionariesExtended(outDocument.GetCatalog().GetPdfObject(), cmpDocument.GetCatalog().GetPdfObject
                (), catalogPath, compareResult, ignoredCatalogEntries);
            return compareResult;
        }

        // TODO to document
        public virtual iText.Kernel.Utils.CompareTool DisableCachedPagesComparison() {
            this.useCachedPagesForComparison = false;
            return this;
        }

        /// <summary>Sets the maximum errors count which will be returned as the result of the comparison.</summary>
        /// <param name="compareByContentMaxErrorCount">the errors count.</param>
        /// <returns>this CompareTool instance.</returns>
        public virtual iText.Kernel.Utils.CompareTool SetCompareByContentErrorsLimit(int compareByContentMaxErrorCount
            ) {
            this.compareByContentErrorsLimit = compareByContentMaxErrorCount;
            return this;
        }

        /// <summary>Enables or disables the generation of the comparison report in the form of the xml document.</summary>
        /// <remarks>
        /// Enables or disables the generation of the comparison report in the form of the xml document.
        /// <br/>
        /// IMPORTANT NOTE: this flag affect only the comparison made by compareByContent methods!
        /// </remarks>
        /// <param name="generateCompareByContentXmlReport">true to enable xml report generation, false - to disable.</param>
        /// <returns>this CompareTool instance.</returns>
        public virtual iText.Kernel.Utils.CompareTool SetGenerateCompareByContentXmlReport(bool generateCompareByContentXmlReport
            ) {
            this.generateCompareByContentXmlReport = generateCompareByContentXmlReport;
            return this;
        }

        /// <summary>Enables the comparison of the encryption properties of the documents.</summary>
        /// <remarks>
        /// Enables the comparison of the encryption properties of the documents. Encryption properties comparison
        /// results are returned along with all other comparison results.
        /// <br/>
        /// IMPORTANT NOTE: this flag affect only the comparison made by compareByContent methods!
        /// </remarks>
        /// <returns>this CompareTool instance.</returns>
        public virtual iText.Kernel.Utils.CompareTool EnableEncryptionCompare() {
            this.encryptionCompareEnabled = true;
            return this;
        }

        public virtual ReaderProperties GetOutReaderProperties() {
            if (outProps == null) {
                outProps = new ReaderProperties();
            }
            return outProps;
        }

        public virtual ReaderProperties GetCmpReaderProperties() {
            if (cmpProps == null) {
                cmpProps = new ReaderProperties();
            }
            return cmpProps;
        }

        /// <summary>Compares two documents visually.</summary>
        /// <remarks>
        /// Compares two documents visually. For the comparison two external tools are used: Ghostscript and ImageMagick.
        /// For more info about needed configuration for visual comparison process see
        /// <see cref="CompareTool"/>
        /// class description.
        /// <br/>
        /// During comparison for every page of two documents an image file will be created in the folder specified by
        /// outPath absolute path. Then those page images will be compared and if there are any differences for some pages,
        /// another image file will be created with marked differences on it.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <param name="differenceImagePrefix">file name prefix for image files with marked differences if there is any.
        ///     </param>
        /// <returns>string containing list of the pages that are visually different, or null if there are no visual differences.
        ///     </returns>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual String CompareVisually(String outPdf, String cmpPdf, String outPath, String differenceImagePrefix
            ) {
            return CompareVisually(outPdf, cmpPdf, outPath, differenceImagePrefix, null);
        }

        /// <summary>Compares two documents visually.</summary>
        /// <remarks>
        /// Compares two documents visually. For the comparison two external tools are used: Ghostscript and ImageMagick.
        /// For more info about needed configuration for visual comparison process see
        /// <see cref="CompareTool"/>
        /// class description.
        /// <br/>
        /// During comparison for every page of two documents an image file will be created in the folder specified by
        /// outPath absolute path. Then those page images will be compared and if there are any differences for some pages,
        /// another image file will be created with marked differences on it.
        /// <br/>
        /// It is possible to ignore certain areas of the document pages during visual comparison. This is useful for example
        /// in case if documents should be the same except certain page area with date on it. In this case, in the folder
        /// specified by the outPath, new pdf documents will be created with the black rectangles at the specified ignored
        /// areas, and visual comparison will be performed on these new documents.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <param name="differenceImagePrefix">file name prefix for image files with marked differences if there is any.
        ///     </param>
        /// <param name="ignoredAreas">a map with one-based page numbers as keys and lists of ignored rectangles as values.
        ///     </param>
        /// <returns>string containing list of the pages that are visually different, or null if there are no visual differences.
        ///     </returns>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual String CompareVisually(String outPdf, String cmpPdf, String outPath, String differenceImagePrefix
            , IDictionary<int, IList<Rectangle>> ignoredAreas) {
            Init(outPdf, cmpPdf);
            return CompareVisually(outPath, differenceImagePrefix, ignoredAreas);
        }

        /// <summary>
        /// Compares two PDF documents by content starting from page dictionaries and then recursively comparing
        /// corresponding objects which are referenced from them.
        /// </summary>
        /// <remarks>
        /// Compares two PDF documents by content starting from page dictionaries and then recursively comparing
        /// corresponding objects which are referenced from them. You can roughly imagine it as depth-first traversal
        /// of the two trees that represent pdf objects structure of the documents.
        /// <br/><br/>
        /// Unlike
        /// <see cref="CompareByCatalog(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Pdf.PdfDocument)"/>
        /// this method performs content comparison page by page
        /// and doesn't compare the tag structure, acroforms and all other things that doesn't belong to specific pages.
        /// <br/>
        /// When comparison by content is finished, if any differences were found, visual comparison is automatically started.
        /// For more info see
        /// <see cref="CompareVisually(System.String, System.String, System.String, System.String)"/>
        /// .
        /// <br/><br/>
        /// For more explanations about what is outPdf and cmpPdf see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <param name="differenceImagePrefix">file name prefix for image files with marked visual differences if there is any.
        ///     </param>
        /// <returns>
        /// string containing text report of the encountered content differences and also list of the pages that are
        /// visually different, or null if there are no content and therefore no visual differences.
        /// </returns>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath, String differenceImagePrefix
            ) {
            return CompareByContent(outPdf, cmpPdf, outPath, differenceImagePrefix, null, null, null);
        }

        /// <summary>This method overload is used to compare two encrypted PDF documents.</summary>
        /// <remarks>
        /// This method overload is used to compare two encrypted PDF documents. Document passwords are passed with
        /// outPass and cmpPass parameters.
        /// <br/><br/>
        /// Compares two PDF documents by content starting from page dictionaries and then recursively comparing
        /// corresponding objects which are referenced from them. You can roughly imagine it as depth-first traversal
        /// of the two trees that represent pdf objects structure of the documents.
        /// <br/><br/>
        /// Unlike
        /// <see cref="CompareByCatalog(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Pdf.PdfDocument)"/>
        /// this method performs content comparison page by page
        /// and doesn't compare the tag structure, acroforms and all other things that doesn't belong to specific pages.
        /// <br/>
        /// When comparison by content is finished, if any differences were found, visual comparison is automatically started.
        /// For more info see
        /// <see cref="CompareVisually(System.String, System.String, System.String, System.String)"/>
        /// .
        /// <br/><br/>
        /// For more explanations about what is outPdf and cmpPdf see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <param name="differenceImagePrefix">file name prefix for image files with marked visual differences if there is any.
        ///     </param>
        /// <param name="outPass">password for the encrypted document specified by the outPdf absolute path.</param>
        /// <param name="cmpPass">password for the encrypted document specified by the cmpPdf absolute path.</param>
        /// <returns>
        /// string containing text report of the encountered content differences and also list of the pages that are
        /// visually different, or null if there are no content and therefore no visual differences.
        /// </returns>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath, String differenceImagePrefix
            , byte[] outPass, byte[] cmpPass) {
            return CompareByContent(outPdf, cmpPdf, outPath, differenceImagePrefix, null, outPass, cmpPass);
        }

        /// <summary>
        /// Compares two PDF documents by content starting from page dictionaries and then recursively comparing
        /// corresponding objects which are referenced from them.
        /// </summary>
        /// <remarks>
        /// Compares two PDF documents by content starting from page dictionaries and then recursively comparing
        /// corresponding objects which are referenced from them. You can roughly imagine it as depth-first traversal
        /// of the two trees that represent pdf objects structure of the documents.
        /// <br/><br/>
        /// Unlike
        /// <see cref="CompareByCatalog(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Pdf.PdfDocument)"/>
        /// this method performs content comparison page by page
        /// and doesn't compare the tag structure, acroforms and all other things that doesn't belong to specific pages.
        /// <br/>
        /// When comparison by content is finished, if any differences were found, visual comparison is automatically started.
        /// For more info see
        /// <see cref="CompareVisually(System.String, System.String, System.String, System.String, System.Collections.Generic.IDictionary{K, V})
        ///     "/>
        /// .
        /// <br/><br/>
        /// For more explanations about what is outPdf and cmpPdf see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <param name="differenceImagePrefix">file name prefix for image files with marked visual differences if there is any.
        ///     </param>
        /// <param name="ignoredAreas">a map with one-based page numbers as keys and lists of ignored rectangles as values.
        ///     </param>
        /// <returns>
        /// string containing text report of the encountered content differences and also list of the pages that are
        /// visually different, or null if there are no content and therefore no visual differences.
        /// </returns>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath, String differenceImagePrefix
            , IDictionary<int, IList<Rectangle>> ignoredAreas) {
            Init(outPdf, cmpPdf);
            return CompareByContent(outPath, differenceImagePrefix, ignoredAreas);
        }

        /// <summary>This method overload is used to compare two encrypted PDF documents.</summary>
        /// <remarks>
        /// This method overload is used to compare two encrypted PDF documents. Document passwords are passed with
        /// outPass and cmpPass parameters.
        /// <br/><br/>
        /// Compares two PDF documents by content starting from page dictionaries and then recursively comparing
        /// corresponding objects which are referenced from them. You can roughly imagine it as depth-first traversal
        /// of the two trees that represent pdf objects structure of the documents.
        /// <br/><br/>
        /// Unlike
        /// <see cref="CompareByCatalog(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Pdf.PdfDocument)"/>
        /// this method performs content comparison page by page
        /// and doesn't compare the tag structure, acroforms and all other things that doesn't belong to specific pages.
        /// <br/>
        /// When comparison by content is finished, if any differences were found, visual comparison is automatically started.
        /// For more info see
        /// <see cref="CompareVisually(System.String, System.String, System.String, System.String, System.Collections.Generic.IDictionary{K, V})
        ///     "/>
        /// .
        /// <br/><br/>
        /// For more explanations about what is outPdf and cmpPdf see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <param name="differenceImagePrefix">file name prefix for image files with marked visual differences if there is any.
        ///     </param>
        /// <param name="ignoredAreas">a map with one-based page numbers as keys and lists of ignored rectangles as values.
        ///     </param>
        /// <param name="outPass">password for the encrypted document specified by the outPdf absolute path.</param>
        /// <param name="cmpPass">password for the encrypted document specified by the cmpPdf absolute path.</param>
        /// <returns>
        /// string containing text report of the encountered content differences and also list of the pages that are
        /// visually different, or null if there are no content and therefore no visual differences.
        /// </returns>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath, String differenceImagePrefix
            , IDictionary<int, IList<Rectangle>> ignoredAreas, byte[] outPass, byte[] cmpPass) {
            Init(outPdf, cmpPdf);
            SetPassword(outPass, cmpPass);
            return CompareByContent(outPath, differenceImagePrefix, ignoredAreas);
        }

        /// <summary>Simple method that compares two given PdfDictionaries by content.</summary>
        /// <remarks>
        /// Simple method that compares two given PdfDictionaries by content. This is "deep" comparing, which means that all
        /// nested objects are also compared by content.
        /// </remarks>
        /// <param name="outDict">dictionary to compare.</param>
        /// <param name="cmpDict">dictionary to compare.</param>
        /// <returns>true if dictionaries are equal by content, otherwise false.</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual bool CompareDictionaries(PdfDictionary outDict, PdfDictionary cmpDict) {
            return CompareDictionariesExtended(outDict, cmpDict, null, null);
        }

        /// <summary>Simple method that compares two given PdfStreams by content.</summary>
        /// <remarks>
        /// Simple method that compares two given PdfStreams by content. This is "deep" comparing, which means that all
        /// nested objects are also compared by content.
        /// </remarks>
        /// <param name="outStream">stream to compare.</param>
        /// <param name="cmpStream">stream to compare.</param>
        /// <returns>true if stream are equal by content, otherwise false.</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual bool CompareStreams(PdfStream outStream, PdfStream cmpStream) {
            return CompareStreamsExtended(outStream, cmpStream, null, null);
        }

        /// <summary>Simple method that compares two given PdfArrays by content.</summary>
        /// <remarks>
        /// Simple method that compares two given PdfArrays by content. This is "deep" comparing, which means that all
        /// nested objects are also compared by content.
        /// </remarks>
        /// <param name="outArray">array to compare.</param>
        /// <param name="cmpArray">array to compare.</param>
        /// <returns>true if arrays are equal by content, otherwise false.</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual bool CompareArrays(PdfArray outArray, PdfArray cmpArray) {
            return CompareArraysExtended(outArray, cmpArray, null, null);
        }

        /// <summary>Simple method that compares two given PdfNames.</summary>
        /// <param name="outName">name to compare.</param>
        /// <param name="cmpName">name to compare.</param>
        /// <returns>true if names are equal, otherwise false.</returns>
        public virtual bool CompareNames(PdfName outName, PdfName cmpName) {
            return cmpName.Equals(outName);
        }

        /// <summary>Simple method that compares two given PdfNumbers.</summary>
        /// <param name="outNumber">number to compare.</param>
        /// <param name="cmpNumber">number to compare.</param>
        /// <returns>true if numbers are equal, otherwise false.</returns>
        public virtual bool CompareNumbers(PdfNumber outNumber, PdfNumber cmpNumber) {
            return cmpNumber.GetValue() == outNumber.GetValue();
        }

        /// <summary>Simple method that compares two given PdfStrings.</summary>
        /// <param name="outString">string to compare.</param>
        /// <param name="cmpString">string to compare.</param>
        /// <returns>true if strings are equal, otherwise false.</returns>
        public virtual bool CompareStrings(PdfString outString, PdfString cmpString) {
            return cmpString.GetValue().Equals(outString.GetValue());
        }

        /// <summary>Simple method that compares two given PdfBooleans.</summary>
        /// <param name="outBoolean">boolean to compare.</param>
        /// <param name="cmpBoolean">boolean to compare.</param>
        /// <returns>true if booleans are equal, otherwise false.</returns>
        public virtual bool CompareBooleans(PdfBoolean outBoolean, PdfBoolean cmpBoolean) {
            return cmpBoolean.GetValue() == outBoolean.GetValue();
        }

        /// <summary>Compares xmp metadata of the two given PDF documents.</summary>
        /// <param name="outPdf">the absolute path to the output file, which xmp is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which xmp is to be compared to output file.</param>
        /// <returns>text report of the xmp differences, or null if there are no differences.</returns>
        public virtual String CompareXmp(String outPdf, String cmpPdf) {
            return CompareXmp(outPdf, cmpPdf, false);
        }

        /// <summary>Compares xmp metadata of the two given PDF documents.</summary>
        /// <param name="outPdf">the absolute path to the output file, which xmp is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which xmp is to be compared to output file.</param>
        /// <param name="ignoreDateAndProducerProperties">
        /// true, if to ignore differences in date or producer xmp metadata
        /// properties.
        /// </param>
        /// <returns>text report of the xmp differences, or null if there are no differences.</returns>
        public virtual String CompareXmp(String outPdf, String cmpPdf, bool ignoreDateAndProducerProperties) {
            Init(outPdf, cmpPdf);
            PdfDocument cmpDocument = null;
            PdfDocument outDocument = null;
            try {
                cmpDocument = new PdfDocument(new PdfReader(this.cmpPdf));
                outDocument = new PdfDocument(new PdfReader(this.outPdf));
                byte[] cmpBytes = cmpDocument.GetXmpMetadata();
                byte[] outBytes = outDocument.GetXmpMetadata();
                if (ignoreDateAndProducerProperties) {
                    XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(cmpBytes);
                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.CreateDate, true, true);
                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.ModifyDate, true, true);
                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.MetadataDate, true, true);
                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_PDF, PdfConst.Producer, true, true);
                    cmpBytes = XMPMetaFactory.SerializeToBuffer(xmpMeta, new SerializeOptions(SerializeOptions.SORT));
                    xmpMeta = XMPMetaFactory.ParseFromBuffer(outBytes);
                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.CreateDate, true, true);
                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.ModifyDate, true, true);
                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.MetadataDate, true, true);
                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_PDF, PdfConst.Producer, true, true);
                    outBytes = XMPMetaFactory.SerializeToBuffer(xmpMeta, new SerializeOptions(SerializeOptions.SORT));
                }
                if (!CompareXmls(cmpBytes, outBytes)) {
                    return "The XMP packages different!";
                }
            }
            catch (Exception) {
                return "XMP parsing failure!";
            }
            finally {
                if (cmpDocument != null) {
                    cmpDocument.Close();
                }
                if (outDocument != null) {
                    outDocument.Close();
                }
            }
            return null;
        }

        /// <summary>Utility method that provides simple comparison of the two xml files stored in byte arrays.</summary>
        /// <param name="xml1">first xml file data to compare.</param>
        /// <param name="xml2">second xml file data to compare.</param>
        /// <returns>true if xml structures are identical, false otherwise.</returns>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual bool CompareXmls(byte[] xml1, byte[] xml2) {
            return XmlUtils.CompareXmls(new MemoryStream(xml1), new MemoryStream(xml2));
        }

        /// <summary>Utility method that provides simple comparison of the two xml files.</summary>
        /// <param name="xmlFilePath1">absolute path to the first xml file to compare.</param>
        /// <param name="xmlFilePath2">absolute path to the second xml file to compare.</param>
        /// <returns>true if xml structures are identical, false otherwise.</returns>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual bool CompareXmls(String xmlFilePath1, String xmlFilePath2) {
            return XmlUtils.CompareXmls(new FileStream(xmlFilePath1, FileMode.Open, FileAccess.Read), new FileStream(xmlFilePath2
                , FileMode.Open, FileAccess.Read));
        }

        /// <summary>This method overload is used to compare two encrypted PDF documents.</summary>
        /// <remarks>
        /// This method overload is used to compare two encrypted PDF documents. Document passwords are passed with
        /// outPass and cmpPass parameters.
        /// <br/><br/>
        /// Compares document info dictionaries of two pdf documents.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which info is to be compared to cmp-file info.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which info is to be compared to output file info.</param>
        /// <param name="outPass">password for the encrypted document specified by the outPdf absolute path.</param>
        /// <param name="cmpPass">password for the encrypted document specified by the cmpPdf absolute path.</param>
        /// <returns>text report of the differences in documents infos.</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual String CompareDocumentInfo(String outPdf, String cmpPdf, byte[] outPass, byte[] cmpPass) {
            System.Console.Out.Write("[itext] INFO  Comparing document info.......");
            String message = null;
            SetPassword(outPass, cmpPass);
            PdfDocument outDocument = new PdfDocument(new PdfReader(outPdf, GetOutReaderProperties()));
            PdfDocument cmpDocument = new PdfDocument(new PdfReader(cmpPdf, GetCmpReaderProperties()));
            String[] cmpInfo = ConvertInfo(cmpDocument.GetDocumentInfo());
            String[] outInfo = ConvertInfo(outDocument.GetDocumentInfo());
            for (int i = 0; i < cmpInfo.Length; ++i) {
                if (!cmpInfo[i].Equals(outInfo[i])) {
                    message = "Document info fail";
                    break;
                }
            }
            outDocument.Close();
            cmpDocument.Close();
            if (message == null) {
                System.Console.Out.WriteLine("OK");
            }
            else {
                System.Console.Out.WriteLine("Fail");
            }
            System.Console.Out.Flush();
            return message;
        }

        /// <summary>Compares document info dictionaries of two pdf documents.</summary>
        /// <param name="outPdf">the absolute path to the output file, which info is to be compared to cmp-file info.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which info is to be compared to output file info.</param>
        /// <returns>text report of the differences in documents infos.</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual String CompareDocumentInfo(String outPdf, String cmpPdf) {
            return CompareDocumentInfo(outPdf, cmpPdf, null, null);
        }

        /// <summary>Compares if two documents has identical link annotations on corresponding pages.</summary>
        /// <param name="outPdf">the absolute path to the output file, which links are to be compared to cmp-file links.
        ///     </param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which links are to be compared to output file links.
        ///     </param>
        /// <returns>text report of the differences in documents links.</returns>
        /// <exception cref="System.IO.IOException"/>
        public virtual String CompareLinkAnnotations(String outPdf, String cmpPdf) {
            System.Console.Out.Write("[itext] INFO  Comparing link annotations....");
            String message = null;
            PdfDocument outDocument = new PdfDocument(new PdfReader(outPdf));
            PdfDocument cmpDocument = new PdfDocument(new PdfReader(cmpPdf));
            for (int i = 0; i < outDocument.GetNumberOfPages() && i < cmpDocument.GetNumberOfPages(); i++) {
                IList<PdfLinkAnnotation> outLinks = GetLinkAnnotations(i + 1, outDocument);
                IList<PdfLinkAnnotation> cmpLinks = GetLinkAnnotations(i + 1, cmpDocument);
                if (cmpLinks.Count != outLinks.Count) {
                    message = String.Format("Different number of links on page {0}.", i + 1);
                    break;
                }
                for (int j = 0; j < cmpLinks.Count; j++) {
                    if (!CompareLinkAnnotations(cmpLinks[j], outLinks[j], cmpDocument, outDocument)) {
                        message = String.Format("Different links on page {0}.\n{1}\n{2}", i + 1, cmpLinks[j].ToString(), outLinks[
                            j].ToString());
                        break;
                    }
                }
            }
            outDocument.Close();
            cmpDocument.Close();
            if (message == null) {
                System.Console.Out.WriteLine("OK");
            }
            else {
                System.Console.Out.WriteLine("Fail");
            }
            System.Console.Out.Flush();
            return message;
        }

        /// <summary>Compares tag structures of the two PDF documents.</summary>
        /// <remarks>
        /// Compares tag structures of the two PDF documents.
        /// <br/>
        /// This method creates xml files in the same folder with outPdf file. These xml files contain documents tag structures
        /// converted into the xml structure. These xml files are compared if they are equal.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which tags are to be compared to cmp-file tags.
        ///     </param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which tags are to be compared to output file tags.
        ///     </param>
        /// <returns>text report of the differences in documents tags.</returns>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        public virtual String CompareTagStructures(String outPdf, String cmpPdf) {
            System.Console.Out.Write("[itext] INFO  Comparing tag structures......");
            String outXmlPath = outPdf.Replace(".pdf", ".xml");
            String cmpXmlPath = outPdf.Replace(".pdf", ".cmp.xml");
            String message = null;
            PdfReader readerOut = new PdfReader(outPdf);
            PdfDocument docOut = new PdfDocument(readerOut);
            FileStream xmlOut = new FileStream(outXmlPath, FileMode.Create);
            new TaggedPdfReaderTool(docOut).SetRootTag("root").ConvertToXml(xmlOut);
            docOut.Close();
            xmlOut.Close();
            PdfReader readerCmp = new PdfReader(cmpPdf);
            PdfDocument docCmp = new PdfDocument(readerCmp);
            FileStream xmlCmp = new FileStream(cmpXmlPath, FileMode.Create);
            new TaggedPdfReaderTool(docCmp).SetRootTag("root").ConvertToXml(xmlCmp);
            docCmp.Close();
            xmlCmp.Close();
            if (!CompareXmls(outXmlPath, cmpXmlPath)) {
                message = "The tag structures are different.";
            }
            if (message == null) {
                System.Console.Out.WriteLine("OK");
            }
            else {
                System.Console.Out.WriteLine("Fail");
            }
            System.Console.Out.Flush();
            return message;
        }

        private void Init(String outPdf, String cmpPdf) {
            this.outPdf = outPdf;
            this.cmpPdf = cmpPdf;
            outPdfName = new FileInfo(outPdf).Name;
            cmpPdfName = new FileInfo(cmpPdf).Name;
            outImage = outPdfName + "-%03d.png";
            if (cmpPdfName.StartsWith("cmp_")) {
                cmpImage = cmpPdfName + "-%03d.png";
            }
            else {
                cmpImage = "cmp_" + cmpPdfName + "-%03d.png";
            }
        }

        private void SetPassword(byte[] outPass, byte[] cmpPass) {
            if (outPass != null) {
                GetOutReaderProperties().SetPassword(outPass);
            }
            if (cmpPass != null) {
                GetCmpReaderProperties().SetPassword(outPass);
            }
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        private String CompareVisually(String outPath, String differenceImagePrefix, IDictionary<int, IList<Rectangle
            >> ignoredAreas) {
            return CompareVisually(outPath, differenceImagePrefix, ignoredAreas, null);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private String CompareVisually(String outPath, String differenceImagePrefix, IDictionary<int, IList<Rectangle
            >> ignoredAreas, IList<int> equalPages) {
            if (gsExec == null) {
                return undefinedGsPath;
            }
            if (!(new FileInfo(gsExec).Exists)) {
                return new FileInfo(gsExec).FullName + " does not exist";
            }
            if (!outPath.EndsWith("/")) {
                outPath = outPath + "/";
            }
            PrepareOutputDirs(outPath, differenceImagePrefix);
            if (ignoredAreas != null && !ignoredAreas.IsEmpty()) {
                CreateIgnoredAreasPdfs(outPath, ignoredAreas);
            }
            String imagesGenerationResult = RunGhostScriptImageGeneration(outPath);
            if (imagesGenerationResult != null) {
                return imagesGenerationResult;
            }
            return CompareImagesOfPdfs(outPath, differenceImagePrefix, equalPages);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private String CompareImagesOfPdfs(String outPath, String differenceImagePrefix, IList<int> equalPages) {
            FileInfo[] imageFiles = FileUtil.ListFilesInDirectoryByFilter(outPath, new CompareTool.PngFileFilter(this)
                );
            FileInfo[] cmpImageFiles = FileUtil.ListFilesInDirectoryByFilter(outPath, new CompareTool.CmpPngFileFilter
                (this));
            bool bUnexpectedNumberOfPages = false;
            if (imageFiles.Length != cmpImageFiles.Length) {
                bUnexpectedNumberOfPages = true;
            }
            int cnt = Math.Min(imageFiles.Length, cmpImageFiles.Length);
            if (cnt < 1) {
                return "No files for comparing.\nThe result or sample pdf file is not processed by GhostScript.";
            }
            iText.IO.Util.JavaUtil.Sort(imageFiles, new CompareTool.ImageNameComparator(this));
            iText.IO.Util.JavaUtil.Sort(cmpImageFiles, new CompareTool.ImageNameComparator(this));
            String differentPagesFail = null;
            bool compareExecIsOk = compareExec != null && new FileInfo(compareExec).Exists;
            IList<int> diffPages = new List<int>();
            for (int i = 0; i < cnt; i++) {
                if (equalPages != null && equalPages.Contains(i)) {
                    continue;
                }
                System.Console.Out.Write("Comparing page " + iText.IO.Util.JavaUtil.IntegerToString(i + 1) + " (" + imageFiles
                    [i].FullName + ")...");
                FileStream is1 = new FileStream(imageFiles[i].FullName, FileMode.Open, FileAccess.Read);
                FileStream is2 = new FileStream(cmpImageFiles[i].FullName, FileMode.Open, FileAccess.Read);
                bool cmpResult = CompareStreams(is1, is2);
                is1.Close();
                is2.Close();
                if (!cmpResult) {
                    differentPagesFail = " Page is different!";
                    diffPages.Add(i + 1);
                    if (compareExecIsOk) {
                        String currCompareParams = compareParams.Replace("<image1>", imageFiles[i].FullName).Replace("<image2>", cmpImageFiles
                            [i].FullName).Replace("<difference>", outPath + differenceImagePrefix + iText.IO.Util.JavaUtil.IntegerToString
                            (i + 1) + ".png");
                        if (SystemUtil.RunProcessAndWait(compareExec, currCompareParams)) {
                            differentPagesFail += "\nPlease, examine " + outPath + differenceImagePrefix + iText.IO.Util.JavaUtil.IntegerToString
                                (i + 1) + ".png for more details.";
                        }
                    }
                    System.Console.Out.WriteLine(differentPagesFail);
                }
                else {
                    System.Console.Out.WriteLine(" done.");
                }
            }
            if (differentPagesFail != null) {
                String errorMessage = differentPages.Replace("<filename>", outPdf).Replace("<pagenumber>", ListDiffPagesAsString
                    (diffPages));
                if (!compareExecIsOk) {
                    errorMessage += "\nYou can optionally specify path to ImageMagick compare tool (e.g. -DcompareExec=\"C:/Program Files/ImageMagick-6.5.4-2/compare.exe\") to visualize differences.";
                }
                return errorMessage;
            }
            else {
                if (bUnexpectedNumberOfPages) {
                    return unexpectedNumberOfPages.Replace("<filename>", outPdf);
                }
            }
            return null;
        }

        private String ListDiffPagesAsString(IList<int> diffPages) {
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < diffPages.Count; i++) {
                sb.Append(diffPages[i]);
                if (i < diffPages.Count - 1) {
                    sb.Append(", ");
                }
            }
            sb.Append("]");
            return diffPages.ToString();
        }

        /// <exception cref="System.IO.IOException"/>
        private void CreateIgnoredAreasPdfs(String outPath, IDictionary<int, IList<Rectangle>> ignoredAreas) {
            PdfWriter outWriter = new PdfWriter(outPath + ignoredAreasPrefix + outPdfName);
            PdfWriter cmpWriter = new PdfWriter(outPath + ignoredAreasPrefix + cmpPdfName);
            PdfDocument pdfOutDoc = new PdfDocument(new PdfReader(outPdf), outWriter);
            PdfDocument pdfCmpDoc = new PdfDocument(new PdfReader(cmpPdf), cmpWriter);
            foreach (KeyValuePair<int, IList<Rectangle>> entry in ignoredAreas) {
                int pageNumber = entry.Key;
                IList<Rectangle> rectangles = entry.Value;
                if (rectangles != null && !rectangles.IsEmpty()) {
                    //drawing rectangles manually, because we don't want to create dependency on itextpdf.canvas module for itextpdf.kernel
                    PdfStream outStream = GetPageContentStream(pdfOutDoc.GetPage(pageNumber));
                    PdfStream cmpStream = GetPageContentStream(pdfCmpDoc.GetPage(pageNumber));
                    outStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("q\n"));
                    outStream.GetOutputStream().WriteFloats(new float[] { 0.0f, 0.0f, 0.0f }).WriteSpace().WriteBytes(ByteUtils
                        .GetIsoBytes("rg\n"));
                    cmpStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("q\n"));
                    cmpStream.GetOutputStream().WriteFloats(new float[] { 0.0f, 0.0f, 0.0f }).WriteSpace().WriteBytes(ByteUtils
                        .GetIsoBytes("rg\n"));
                    foreach (Rectangle rect in rectangles) {
                        outStream.GetOutputStream().WriteFloats(new float[] { rect.GetX(), rect.GetY(), rect.GetWidth(), rect.GetHeight
                            () }).WriteSpace().WriteBytes(ByteUtils.GetIsoBytes("re\n")).WriteBytes(ByteUtils.GetIsoBytes("f\n"));
                        cmpStream.GetOutputStream().WriteFloats(new float[] { rect.GetX(), rect.GetY(), rect.GetWidth(), rect.GetHeight
                            () }).WriteSpace().WriteBytes(ByteUtils.GetIsoBytes("re\n")).WriteBytes(ByteUtils.GetIsoBytes("f\n"));
                    }
                    outStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("Q\n"));
                    cmpStream.GetOutputStream().WriteBytes(ByteUtils.GetIsoBytes("Q\n"));
                }
            }
            pdfOutDoc.Close();
            pdfCmpDoc.Close();
            Init(outPath + ignoredAreasPrefix + outPdfName, outPath + ignoredAreasPrefix + cmpPdfName);
        }

        private PdfStream GetPageContentStream(PdfPage page) {
            PdfStream stream = page.GetContentStream(page.GetContentStreamCount() - 1);
            return stream.GetOutputStream() == null ? page.NewContentStreamAfter() : stream;
        }

        private void PrepareOutputDirs(String outPath, String differenceImagePrefix) {
            FileInfo[] imageFiles;
            FileInfo[] cmpImageFiles;
            FileInfo[] diffFiles;
            if (!FileUtil.DirectoryExists(outPath)) {
                FileUtil.CreateDirectories(outPath);
            }
            else {
                imageFiles = FileUtil.ListFilesInDirectoryByFilter(outPath, new CompareTool.PngFileFilter(this));
                foreach (FileInfo file in imageFiles) {
                    file.Delete();
                }
                cmpImageFiles = FileUtil.ListFilesInDirectoryByFilter(outPath, new CompareTool.CmpPngFileFilter(this));
                foreach (FileInfo file_1 in cmpImageFiles) {
                    file_1.Delete();
                }
                diffFiles = FileUtil.ListFilesInDirectoryByFilter(outPath, new CompareTool.DiffPngFileFilter(this, differenceImagePrefix
                    ));
                foreach (FileInfo file_2 in diffFiles) {
                    file_2.Delete();
                }
            }
        }

        /// <summary>Runs ghostscript to create images of pdfs.</summary>
        /// <param name="outPath">Path to the output folder.</param>
        /// <returns>Returns null if result is successful, else returns error message.</returns>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private String RunGhostScriptImageGeneration(String outPath) {
            if (!FileUtil.DirectoryExists(outPath)) {
                return cannotOpenOutputDirectory.Replace("<filename>", outPdf);
            }
            String currGsParams = gsParams.Replace("<outputfile>", outPath + cmpImage).Replace("<inputfile>", cmpPdf);
            if (!SystemUtil.RunProcessAndWait(gsExec, currGsParams)) {
                return gsFailed.Replace("<filename>", cmpPdf);
            }
            currGsParams = gsParams.Replace("<outputfile>", outPath + outImage).Replace("<inputfile>", outPdf);
            if (!SystemUtil.RunProcessAndWait(gsExec, currGsParams)) {
                return gsFailed.Replace("<filename>", outPdf);
            }
            return null;
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        private String CompareByContent(String outPath, String differenceImagePrefix, IDictionary<int, IList<Rectangle
            >> ignoredAreas) {
            System.Console.Out.Write("[itext] INFO  Comparing by content..........");
            PdfDocument outDocument;
            try {
                outDocument = new PdfDocument(new PdfReader(outPdf, GetOutReaderProperties()));
            }
            catch (System.IO.IOException e) {
                throw new System.IO.IOException("File \"" + outPdf + "\" not found", e);
            }
            IList<PdfDictionary> outPages = new List<PdfDictionary>();
            outPagesRef = new List<PdfIndirectReference>();
            LoadPagesFromReader(outDocument, outPages, outPagesRef);
            PdfDocument cmpDocument;
            try {
                cmpDocument = new PdfDocument(new PdfReader(cmpPdf, GetCmpReaderProperties()));
            }
            catch (System.IO.IOException e) {
                throw new System.IO.IOException("File \"" + cmpPdf + "\" not found", e);
            }
            IList<PdfDictionary> cmpPages = new List<PdfDictionary>();
            cmpPagesRef = new List<PdfIndirectReference>();
            LoadPagesFromReader(cmpDocument, cmpPages, cmpPagesRef);
            if (outPages.Count != cmpPages.Count) {
                return CompareVisually(outPath, differenceImagePrefix, ignoredAreas);
            }
            CompareTool.CompareResult compareResult = new CompareTool.CompareResult(this, compareByContentErrorsLimit);
            IList<int> equalPages = new List<int>(cmpPages.Count);
            for (int i = 0; i < cmpPages.Count; i++) {
                CompareTool.ObjectPath currentPath = new CompareTool.ObjectPath(cmpPagesRef[i], outPagesRef[i]);
                if (CompareDictionariesExtended(outPages[i], cmpPages[i], currentPath, compareResult)) {
                    equalPages.Add(i);
                }
            }
            CompareTool.ObjectPath catalogPath = new CompareTool.ObjectPath(cmpDocument.GetCatalog().GetPdfObject().GetIndirectReference
                (), outDocument.GetCatalog().GetPdfObject().GetIndirectReference());
            ICollection<PdfName> ignoredCatalogEntries = new LinkedHashSet<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList
                (PdfName.Pages, PdfName.Metadata));
            CompareDictionariesExtended(outDocument.GetCatalog().GetPdfObject(), cmpDocument.GetCatalog().GetPdfObject
                (), catalogPath, compareResult, ignoredCatalogEntries);
            if (encryptionCompareEnabled) {
                CompareDocumentsEncryption(outDocument, cmpDocument, compareResult);
            }
            outDocument.Close();
            cmpDocument.Close();
            if (generateCompareByContentXmlReport) {
                String outPdfName = new FileInfo(outPdf).Name;
                FileStream xml = new FileStream(outPath + "/" + outPdfName.JSubstring(0, outPdfName.Length - 3) + "report.xml"
                    , FileMode.Create);
                try {
                    compareResult.WriteReportToXml(xml);
                }
                catch (Exception e) {
                    throw new Exception(e.Message, e);
                }
                finally {
                    xml.Close();
                }
            }
            if (equalPages.Count == cmpPages.Count && compareResult.IsOk()) {
                System.Console.Out.WriteLine("OK");
                System.Console.Out.Flush();
                return null;
            }
            else {
                System.Console.Out.WriteLine("Fail");
                System.Console.Out.Flush();
                String compareByContentReport = "Compare by content report:\n" + compareResult.GetReport();
                System.Console.Out.WriteLine(compareByContentReport);
                System.Console.Out.Flush();
                String message = CompareVisually(outPath, differenceImagePrefix, ignoredAreas, equalPages);
                if (message == null || message.Length == 0) {
                    return "Compare by content fails. No visual differences";
                }
                return message;
            }
        }

        private void LoadPagesFromReader(PdfDocument doc, IList<PdfDictionary> pages, IList<PdfIndirectReference> 
            pagesRef) {
            int numOfPages = doc.GetNumberOfPages();
            for (int i = 0; i < numOfPages; ++i) {
                pages.Add(doc.GetPage(i + 1).GetPdfObject());
                pagesRef.Add(pages[i].GetIndirectReference());
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private void CompareDocumentsEncryption(PdfDocument outDocument, PdfDocument cmpDocument, CompareTool.CompareResult
             compareResult) {
            PdfDictionary outEncrypt = outDocument.GetTrailer().GetAsDictionary(PdfName.Encrypt);
            PdfDictionary cmpEncrypt = cmpDocument.GetTrailer().GetAsDictionary(PdfName.Encrypt);
            if (outEncrypt == null && cmpEncrypt == null) {
                return;
            }
            CompareTool.TrailerPath trailerPath = new CompareTool.TrailerPath(cmpDocument, outDocument);
            if (outEncrypt == null) {
                compareResult.AddError(trailerPath, "Expected encrypted document.");
                return;
            }
            if (cmpEncrypt == null) {
                compareResult.AddError(trailerPath, "Expected not encrypted document.");
                return;
            }
            ICollection<PdfName> ignoredEncryptEntries = new LinkedHashSet<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList
                (PdfName.O, PdfName.U, PdfName.OE, PdfName.UE, PdfName.Perms, PdfName.CF, PdfName.Recipients));
            CompareTool.ObjectPath objectPath = new CompareTool.ObjectPath(outEncrypt.GetIndirectReference(), cmpEncrypt
                .GetIndirectReference());
            CompareDictionariesExtended(outEncrypt, cmpEncrypt, objectPath, compareResult, ignoredEncryptEntries);
            PdfDictionary outCfDict = outEncrypt.GetAsDictionary(PdfName.CF);
            PdfDictionary cmpCfDict = cmpEncrypt.GetAsDictionary(PdfName.CF);
            if (cmpCfDict != null || outCfDict != null) {
                if (cmpCfDict != null && outCfDict == null || cmpCfDict == null) {
                    compareResult.AddError(objectPath, "One of the dictionaries is null, the other is not.");
                }
                else {
                    ICollection<PdfName> mergedKeys = new SortedSet<PdfName>(outCfDict.KeySet());
                    mergedKeys.AddAll(cmpCfDict.KeySet());
                    foreach (PdfName key in mergedKeys) {
                        objectPath.PushDictItemToPath(key);
                        LinkedHashSet<PdfName> excludedKeys = new LinkedHashSet<PdfName>(iText.IO.Util.JavaUtil.ArraysAsList(PdfName
                            .Recipients));
                        CompareDictionariesExtended(outCfDict.GetAsDictionary(key), cmpCfDict.GetAsDictionary(key), objectPath, compareResult
                            , excludedKeys);
                        objectPath.Pop();
                    }
                }
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private bool CompareStreams(Stream is1, Stream is2) {
            byte[] buffer1 = new byte[64 * 1024];
            byte[] buffer2 = new byte[64 * 1024];
            int len1;
            int len2;
            for (; ; ) {
                len1 = is1.Read(buffer1);
                len2 = is2.Read(buffer2);
                if (len1 != len2) {
                    return false;
                }
                if (!iText.IO.Util.JavaUtil.ArraysEquals(buffer1, buffer2)) {
                    return false;
                }
                if (len1 == -1) {
                    break;
                }
            }
            return true;
        }

        /// <exception cref="System.IO.IOException"/>
        private bool CompareDictionariesExtended(PdfDictionary outDict, PdfDictionary cmpDict, CompareTool.ObjectPath
             currentPath, CompareTool.CompareResult compareResult) {
            return CompareDictionariesExtended(outDict, cmpDict, currentPath, compareResult, null);
        }

        /// <exception cref="System.IO.IOException"/>
        private bool CompareDictionariesExtended(PdfDictionary outDict, PdfDictionary cmpDict, CompareTool.ObjectPath
             currentPath, CompareTool.CompareResult compareResult, ICollection<PdfName> excludedKeys) {
            if (cmpDict != null && outDict == null || outDict != null && cmpDict == null) {
                compareResult.AddError(currentPath, "One of the dictionaries is null, the other is not.");
                return false;
            }
            bool dictsAreSame = true;
            // Iterate through the union of the keys of the cmp and out dictionaries
            ICollection<PdfName> mergedKeys = new SortedSet<PdfName>(cmpDict.KeySet());
            mergedKeys.AddAll(outDict.KeySet());
            foreach (PdfName key in mergedKeys) {
                if (excludedKeys != null && excludedKeys.Contains(key)) {
                    continue;
                }
                if (key.Equals(PdfName.Parent) || key.Equals(PdfName.P) || key.Equals(PdfName.ModDate)) {
                    continue;
                }
                if (outDict.IsStream() && cmpDict.IsStream() && (key.Equals(PdfName.Filter) || key.Equals(PdfName.Length))
                    ) {
                    continue;
                }
                if (key.Equals(PdfName.BaseFont) || key.Equals(PdfName.FontName)) {
                    PdfObject cmpObj = cmpDict.Get(key);
                    if (cmpObj.IsName() && cmpObj.ToString().IndexOf('+') > 0) {
                        PdfObject outObj = outDict.Get(key);
                        if (!outObj.IsName() || outObj.ToString().IndexOf('+') == -1) {
                            if (compareResult != null && currentPath != null) {
                                compareResult.AddError(currentPath, String.Format("PdfDictionary {0} entry: Expected: {1}. Found: {2}", key
                                    .ToString(), cmpObj.ToString(), outObj.ToString()));
                            }
                            dictsAreSame = false;
                        }
                        else {
                            String cmpName = cmpObj.ToString().Substring(cmpObj.ToString().IndexOf('+'));
                            String outName = outObj.ToString().Substring(outObj.ToString().IndexOf('+'));
                            if (!cmpName.Equals(outName)) {
                                if (compareResult != null && currentPath != null) {
                                    compareResult.AddError(currentPath, String.Format("PdfDictionary {0} entry: Expected: {1}. Found: {2}", key
                                        .ToString(), cmpObj.ToString(), outObj.ToString()));
                                }
                                dictsAreSame = false;
                            }
                        }
                        continue;
                    }
                }
                if (currentPath != null) {
                    currentPath.PushDictItemToPath(key);
                }
                dictsAreSame = CompareObjects(outDict.Get(key, false), cmpDict.Get(key, false), currentPath, compareResult
                    ) && dictsAreSame;
                if (currentPath != null) {
                    currentPath.Pop();
                }
                if (!dictsAreSame && (currentPath == null || compareResult == null || compareResult.IsMessageLimitReached(
                    ))) {
                    return false;
                }
            }
            return dictsAreSame;
        }

        /// <exception cref="System.IO.IOException"/>
        private bool CompareObjects(PdfObject outObj, PdfObject cmpObj, CompareTool.ObjectPath currentPath, CompareTool.CompareResult
             compareResult) {
            PdfObject outDirectObj = null;
            PdfObject cmpDirectObj = null;
            if (outObj != null) {
                outDirectObj = outObj.IsIndirectReference() ? ((PdfIndirectReference)outObj).GetRefersTo(false) : outObj;
            }
            if (cmpObj != null) {
                cmpDirectObj = cmpObj.IsIndirectReference() ? ((PdfIndirectReference)cmpObj).GetRefersTo(false) : cmpObj;
            }
            if (cmpDirectObj == null && outDirectObj == null) {
                return true;
            }
            if (outDirectObj == null) {
                compareResult.AddError(currentPath, "Expected object was not found.");
                return false;
            }
            else {
                if (cmpDirectObj == null) {
                    compareResult.AddError(currentPath, "Found object which was not expected to be found.");
                    return false;
                }
                else {
                    if (cmpDirectObj.GetObjectType() != outDirectObj.GetObjectType()) {
                        compareResult.AddError(currentPath, String.Format("Types do not match. Expected: {0}. Found: {1}.", cmpDirectObj
                            .GetType().Name, outDirectObj.GetType().Name));
                        return false;
                    }
                    else {
                        if (cmpObj.IsIndirectReference() && !outObj.IsIndirectReference()) {
                            compareResult.AddError(currentPath, "Expected indirect object.");
                            return false;
                        }
                        else {
                            if (!cmpObj.IsIndirectReference() && outObj.IsIndirectReference()) {
                                compareResult.AddError(currentPath, "Expected direct object.");
                                return false;
                            }
                        }
                    }
                }
            }
            if (currentPath != null && cmpObj.IsIndirectReference() && outObj.IsIndirectReference()) {
                if (currentPath.IsComparing((PdfIndirectReference)cmpObj, (PdfIndirectReference)outObj)) {
                    return true;
                }
                currentPath = currentPath.ResetDirectPath((PdfIndirectReference)cmpObj, (PdfIndirectReference)outObj);
            }
            if (cmpDirectObj.IsDictionary() && PdfName.Page.Equals(((PdfDictionary)cmpDirectObj).GetAsName(PdfName.Type
                )) && useCachedPagesForComparison) {
                if (!outDirectObj.IsDictionary() || !PdfName.Page.Equals(((PdfDictionary)outDirectObj).GetAsName(PdfName.Type
                    ))) {
                    if (compareResult != null && currentPath != null) {
                        compareResult.AddError(currentPath, "Expected a page. Found not a page.");
                    }
                    return false;
                }
                PdfIndirectReference cmpRefKey = cmpObj.IsIndirectReference() ? (PdfIndirectReference)cmpObj : cmpObj.GetIndirectReference
                    ();
                PdfIndirectReference outRefKey = outObj.IsIndirectReference() ? (PdfIndirectReference)outObj : outObj.GetIndirectReference
                    ();
                // References to the same page
                if (cmpPagesRef == null) {
                    cmpPagesRef = new List<PdfIndirectReference>();
                    for (int i = 1; i <= cmpObj.GetIndirectReference().GetDocument().GetNumberOfPages(); ++i) {
                        cmpPagesRef.Add(cmpObj.GetIndirectReference().GetDocument().GetPage(i).GetPdfObject().GetIndirectReference
                            ());
                    }
                }
                if (outPagesRef == null) {
                    outPagesRef = new List<PdfIndirectReference>();
                    for (int i = 1; i <= outObj.GetIndirectReference().GetDocument().GetNumberOfPages(); ++i) {
                        outPagesRef.Add(outObj.GetIndirectReference().GetDocument().GetPage(i).GetPdfObject().GetIndirectReference
                            ());
                    }
                }
                if (cmpPagesRef.Contains(cmpRefKey) && cmpPagesRef.IndexOf(cmpRefKey) == outPagesRef.IndexOf(outRefKey)) {
                    return true;
                }
                if (compareResult != null && currentPath != null) {
                    compareResult.AddError(currentPath, String.Format("The dictionaries refer to different pages. Expected page number: {0}. Found: {1}"
                        , cmpPagesRef.IndexOf(cmpRefKey), outPagesRef.IndexOf(outRefKey)));
                }
                return false;
            }
            if (cmpDirectObj.IsDictionary()) {
                if (!CompareDictionariesExtended((PdfDictionary)outDirectObj, (PdfDictionary)cmpDirectObj, currentPath, compareResult
                    )) {
                    return false;
                }
            }
            else {
                if (cmpDirectObj.IsStream()) {
                    if (!CompareStreamsExtended((PdfStream)outDirectObj, (PdfStream)cmpDirectObj, currentPath, compareResult)) {
                        return false;
                    }
                }
                else {
                    if (cmpDirectObj.IsArray()) {
                        if (!CompareArraysExtended((PdfArray)outDirectObj, (PdfArray)cmpDirectObj, currentPath, compareResult)) {
                            return false;
                        }
                    }
                    else {
                        if (cmpDirectObj.IsName()) {
                            if (!CompareNamesExtended((PdfName)outDirectObj, (PdfName)cmpDirectObj, currentPath, compareResult)) {
                                return false;
                            }
                        }
                        else {
                            if (cmpDirectObj.IsNumber()) {
                                if (!CompareNumbersExtended((PdfNumber)outDirectObj, (PdfNumber)cmpDirectObj, currentPath, compareResult)) {
                                    return false;
                                }
                            }
                            else {
                                if (cmpDirectObj.IsString()) {
                                    if (!CompareStringsExtended((PdfString)outDirectObj, (PdfString)cmpDirectObj, currentPath, compareResult)) {
                                        return false;
                                    }
                                }
                                else {
                                    if (cmpDirectObj.IsBoolean()) {
                                        if (!CompareBooleansExtended((PdfBoolean)outDirectObj, (PdfBoolean)cmpDirectObj, currentPath, compareResult
                                            )) {
                                            return false;
                                        }
                                    }
                                    else {
                                        if (outDirectObj.IsNull() && cmpDirectObj.IsNull()) {
                                        }
                                        else {
                                            throw new NotSupportedException();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <exception cref="System.IO.IOException"/>
        private bool CompareStreamsExtended(PdfStream outStream, PdfStream cmpStream, CompareTool.ObjectPath currentPath
            , CompareTool.CompareResult compareResult) {
            bool toDecode = PdfName.FlateDecode.Equals(outStream.Get(PdfName.Filter));
            byte[] outStreamBytes = outStream.GetBytes(toDecode);
            byte[] cmpStreamBytes = cmpStream.GetBytes(toDecode);
            if (iText.IO.Util.JavaUtil.ArraysEquals(outStreamBytes, cmpStreamBytes)) {
                return CompareDictionariesExtended(outStream, cmpStream, currentPath, compareResult);
            }
            else {
                String errorMessage = "";
                if (cmpStreamBytes.Length != outStreamBytes.Length) {
                    errorMessage += String.Format("PdfStream. Lengths are different. Expected: {0}. Found: {1}", cmpStreamBytes
                        .Length, outStreamBytes.Length) + "\n";
                }
                else {
                    errorMessage += "PdfStream. Bytes are different.\n";
                }
                String bytesDifference = FindBytesDifference(outStreamBytes, cmpStreamBytes);
                if (bytesDifference != null) {
                    errorMessage += bytesDifference;
                }
                if (compareResult != null && currentPath != null) {
                    //            currentPath.pushOffsetToPath(firstDifferenceOffset);
                    compareResult.AddError(currentPath, errorMessage);
                }
                //            currentPath.pop();
                return false;
            }
        }

        private String FindBytesDifference(byte[] outStreamBytes, byte[] cmpStreamBytes) {
            int numberOfDifferentBytes = 0;
            int firstDifferenceOffset = 0;
            int minLength = Math.Min(cmpStreamBytes.Length, outStreamBytes.Length);
            for (int i = 0; i < minLength; i++) {
                if (cmpStreamBytes[i] != outStreamBytes[i]) {
                    ++numberOfDifferentBytes;
                    if (numberOfDifferentBytes == 1) {
                        firstDifferenceOffset = i;
                    }
                }
            }
            String errorMessage = null;
            if (numberOfDifferentBytes > 0) {
                int diffBytesAreaL = 10;
                int diffBytesAreaR = 10;
                int lCmp = Math.Max(0, firstDifferenceOffset - diffBytesAreaL);
                int rCmp = Math.Min(cmpStreamBytes.Length, firstDifferenceOffset + diffBytesAreaR);
                int lOut = Math.Max(0, firstDifferenceOffset - diffBytesAreaL);
                int rOut = Math.Min(outStreamBytes.Length, firstDifferenceOffset + diffBytesAreaR);
                String cmpByte = iText.IO.Util.JavaUtil.GetStringForBytes(new byte[] { cmpStreamBytes[firstDifferenceOffset
                    ] });
                String cmpByteNeighbours = iText.IO.Util.StringUtil.ReplaceAll(iText.IO.Util.JavaUtil.GetStringForBytes(cmpStreamBytes
                    , lCmp, rCmp - lCmp), "\\r|\\n", " ");
                String outByte = iText.IO.Util.JavaUtil.GetStringForBytes(new byte[] { outStreamBytes[firstDifferenceOffset
                    ] });
                String outBytesNeighbours = iText.IO.Util.StringUtil.ReplaceAll(iText.IO.Util.JavaUtil.GetStringForBytes(outStreamBytes
                    , lOut, rOut - lOut), "\\r|\\n", " ");
                errorMessage = String.Format("First bytes difference is encountered at index {0}. Expected: {1} ({2}). Found: {3} ({4}). Total number of different bytes: {5}"
                    , iText.IO.Util.JavaUtil.IntegerToString(System.Convert.ToInt32(firstDifferenceOffset)), cmpByte, cmpByteNeighbours
                    , outByte, outBytesNeighbours, numberOfDifferentBytes);
            }
            else {
                // lengths are different
                errorMessage = String.Format("Bytes of the shorter array are the same as the first {0} bytes of the longer one."
                    , minLength);
            }
            return errorMessage;
        }

        /// <exception cref="System.IO.IOException"/>
        private bool CompareArraysExtended(PdfArray outArray, PdfArray cmpArray, CompareTool.ObjectPath currentPath
            , CompareTool.CompareResult compareResult) {
            if (outArray == null) {
                if (compareResult != null && currentPath != null) {
                    compareResult.AddError(currentPath, "Found null. Expected PdfArray.");
                }
                return false;
            }
            else {
                if (outArray.Size() != cmpArray.Size()) {
                    if (compareResult != null && currentPath != null) {
                        compareResult.AddError(currentPath, String.Format("PdfArrays. Lengths are different. Expected: {0}. Found: {1}."
                            , cmpArray.Size(), outArray.Size()));
                    }
                    return false;
                }
            }
            bool arraysAreEqual = true;
            for (int i = 0; i < cmpArray.Size(); i++) {
                if (currentPath != null) {
                    currentPath.PushArrayItemToPath(i);
                }
                arraysAreEqual = CompareObjects(outArray.Get(i, false), cmpArray.Get(i, false), currentPath, compareResult
                    ) && arraysAreEqual;
                if (currentPath != null) {
                    currentPath.Pop();
                }
                if (!arraysAreEqual && (currentPath == null || compareResult == null || compareResult.IsMessageLimitReached
                    ())) {
                    return false;
                }
            }
            return arraysAreEqual;
        }

        private bool CompareNamesExtended(PdfName outName, PdfName cmpName, CompareTool.ObjectPath currentPath, CompareTool.CompareResult
             compareResult) {
            if (cmpName.Equals(outName)) {
                return true;
            }
            else {
                if (compareResult != null && currentPath != null) {
                    compareResult.AddError(currentPath, String.Format("PdfName. Expected: {0}. Found: {1}", cmpName.ToString()
                        , outName.ToString()));
                }
                return false;
            }
        }

        private bool CompareNumbersExtended(PdfNumber outNumber, PdfNumber cmpNumber, CompareTool.ObjectPath currentPath
            , CompareTool.CompareResult compareResult) {
            if (cmpNumber.GetValue() == outNumber.GetValue()) {
                return true;
            }
            else {
                if (compareResult != null && currentPath != null) {
                    compareResult.AddError(currentPath, String.Format("PdfNumber. Expected: {0}. Found: {1}", cmpNumber, outNumber
                        ));
                }
                return false;
            }
        }

        private bool CompareStringsExtended(PdfString outString, PdfString cmpString, CompareTool.ObjectPath currentPath
            , CompareTool.CompareResult compareResult) {
            if (iText.IO.Util.JavaUtil.ArraysEquals(ConvertPdfStringToBytes(cmpString), ConvertPdfStringToBytes(outString
                ))) {
                return true;
            }
            else {
                String cmpStr = cmpString.ToUnicodeString();
                String outStr = outString.ToUnicodeString();
                if (cmpStr.Length != outStr.Length) {
                    if (compareResult != null && currentPath != null) {
                        compareResult.AddError(currentPath, String.Format("PdfString. Lengths are different. Expected: {0}. Found: {1}"
                            , cmpStr.Length, outStr.Length));
                    }
                }
                else {
                    for (int i = 0; i < cmpStr.Length; i++) {
                        if (cmpStr[i] != outStr[i]) {
                            int l = Math.Max(0, i - 10);
                            int r = Math.Min(cmpStr.Length, i + 10);
                            if (compareResult != null && currentPath != null) {
                                currentPath.PushOffsetToPath(i);
                                compareResult.AddError(currentPath, String.Format("PdfString. Characters differ at position {0}. Expected: {1} ({2}). Found: {3} ({4})."
                                    , i, char.ToString(cmpStr[i]), cmpStr.JSubstring(l, r).Replace("\n", "\\n"), char.ToString(outStr[i]), 
                                    outStr.JSubstring(l, r).Replace("\n", "\\n")));
                                currentPath.Pop();
                            }
                            break;
                        }
                    }
                }
                return false;
            }
        }

        private byte[] ConvertPdfStringToBytes(PdfString pdfString) {
            byte[] bytes;
            String value = pdfString.GetValue();
            String encoding = pdfString.GetEncoding();
            if (encoding != null && encoding.Equals(PdfEncodings.UNICODE_BIG) && PdfEncodings.IsPdfDocEncoding(value)) {
                bytes = PdfEncodings.ConvertToBytes(value, PdfEncodings.PDF_DOC_ENCODING);
            }
            else {
                bytes = PdfEncodings.ConvertToBytes(value, encoding);
            }
            return bytes;
        }

        private bool CompareBooleansExtended(PdfBoolean outBoolean, PdfBoolean cmpBoolean, CompareTool.ObjectPath 
            currentPath, CompareTool.CompareResult compareResult) {
            if (cmpBoolean.GetValue() == outBoolean.GetValue()) {
                return true;
            }
            else {
                if (compareResult != null && currentPath != null) {
                    compareResult.AddError(currentPath, String.Format("PdfBoolean. Expected: {0}. Found: {1}.", cmpBoolean.GetValue
                        (), outBoolean.GetValue()));
                }
                return false;
            }
        }

        private IList<PdfLinkAnnotation> GetLinkAnnotations(int pageNum, PdfDocument document) {
            IList<PdfLinkAnnotation> linkAnnotations = new List<PdfLinkAnnotation>();
            IList<PdfAnnotation> annotations = document.GetPage(pageNum).GetAnnotations();
            foreach (PdfAnnotation annotation in annotations) {
                if (PdfName.Link.Equals(annotation.GetSubtype())) {
                    linkAnnotations.Add((PdfLinkAnnotation)annotation);
                }
            }
            return linkAnnotations;
        }

        private bool CompareLinkAnnotations(PdfLinkAnnotation cmpLink, PdfLinkAnnotation outLink, PdfDocument cmpDocument
            , PdfDocument outDocument) {
            // Compare link rectangles, page numbers the links refer to, and simple parameters (non-indirect, non-arrays, non-dictionaries)
            PdfObject cmpDestObject = cmpLink.GetDestinationObject();
            PdfObject outDestObject = outLink.GetDestinationObject();
            if (cmpDestObject != null && outDestObject != null) {
                if (cmpDestObject.GetObjectType() != outDestObject.GetObjectType()) {
                    return false;
                }
                else {
                    PdfArray explicitCmpDest = null;
                    PdfArray explicitOutDest = null;
                    IDictionary<String, PdfObject> cmpNamedDestinations = cmpDocument.GetCatalog().GetNameTree(PdfName.Dests).
                        GetNames();
                    IDictionary<String, PdfObject> outNamedDestinations = outDocument.GetCatalog().GetNameTree(PdfName.Dests).
                        GetNames();
                    switch (cmpDestObject.GetObjectType()) {
                        case PdfObject.ARRAY: {
                            explicitCmpDest = (PdfArray)cmpDestObject;
                            explicitOutDest = (PdfArray)outDestObject;
                            break;
                        }

                        case PdfObject.NAME: {
                            explicitCmpDest = (PdfArray)cmpNamedDestinations.Get(((PdfName)cmpDestObject).GetValue());
                            explicitOutDest = (PdfArray)outNamedDestinations.Get(((PdfName)outDestObject).GetValue());
                            break;
                        }

                        case PdfObject.STRING: {
                            explicitCmpDest = (PdfArray)cmpNamedDestinations.Get(((PdfString)cmpDestObject).ToUnicodeString());
                            explicitOutDest = (PdfArray)outNamedDestinations.Get(((PdfString)outDestObject).ToUnicodeString());
                            break;
                        }

                        default: {
                            break;
                        }
                    }
                    if (GetExplicitDestinationPageNum(explicitCmpDest) != GetExplicitDestinationPageNum(explicitOutDest)) {
                        return false;
                    }
                }
            }
            PdfDictionary cmpDict = cmpLink.GetPdfObject();
            PdfDictionary outDict = outLink.GetPdfObject();
            if (cmpDict.Size() != outDict.Size()) {
                return false;
            }
            Rectangle cmpRect = cmpDict.GetAsRectangle(PdfName.Rect);
            Rectangle outRect = outDict.GetAsRectangle(PdfName.Rect);
            if (cmpRect.GetHeight() != outRect.GetHeight() || cmpRect.GetWidth() != outRect.GetWidth() || cmpRect.GetX
                () != outRect.GetX() || cmpRect.GetY() != outRect.GetY()) {
                return false;
            }
            foreach (KeyValuePair<PdfName, PdfObject> cmpEntry in cmpDict.EntrySet()) {
                PdfObject cmpObj = cmpEntry.Value;
                if (!outDict.ContainsKey(cmpEntry.Key)) {
                    return false;
                }
                PdfObject outObj = outDict.Get(cmpEntry.Key);
                if (cmpObj.GetObjectType() != outObj.GetObjectType()) {
                    return false;
                }
                switch (cmpObj.GetObjectType()) {
                    case PdfObject.NULL:
                    case PdfObject.BOOLEAN:
                    case PdfObject.NUMBER:
                    case PdfObject.STRING:
                    case PdfObject.NAME: {
                        if (!cmpObj.ToString().Equals(outObj.ToString())) {
                            return false;
                        }
                        break;
                    }
                }
            }
            return true;
        }

        private int GetExplicitDestinationPageNum(PdfArray explicitDest) {
            PdfIndirectReference pageReference = (PdfIndirectReference)explicitDest.Get(0, false);
            PdfDocument doc = pageReference.GetDocument();
            for (int i = 1; i <= doc.GetNumberOfPages(); ++i) {
                if (doc.GetPage(i).GetPdfObject().GetIndirectReference().Equals(pageReference)) {
                    return i;
                }
            }
            throw new ArgumentException("PdfLinkAnnotation comparison: Page not found.");
        }

        private String[] ConvertInfo(PdfDocumentInfo info) {
            String[] convertedInfo = new String[] { "", "", "", "" };
            String infoValue = info.GetTitle();
            if (infoValue != null) {
                convertedInfo[0] = infoValue;
            }
            infoValue = info.GetAuthor();
            if (infoValue != null) {
                convertedInfo[1] = infoValue;
            }
            infoValue = info.GetSubject();
            if (infoValue != null) {
                convertedInfo[2] = infoValue;
            }
            infoValue = info.GetKeywords();
            if (infoValue != null) {
                convertedInfo[3] = infoValue;
            }
            return convertedInfo;
        }

        private class PngFileFilter : iText.IO.Util.FileUtil.IFileFilter {
            public virtual bool Accept(FileInfo pathname) {
                String ap = pathname.Name;
                bool b1 = ap.EndsWith(".png");
                bool b2 = ap.Contains("cmp_");
                return b1 && !b2 && ap.Contains(this._enclosing.outPdfName);
            }

            internal PngFileFilter(CompareTool _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly CompareTool _enclosing;
        }

        private class CmpPngFileFilter : iText.IO.Util.FileUtil.IFileFilter {
            public virtual bool Accept(FileInfo pathname) {
                String ap = pathname.Name;
                bool b1 = ap.EndsWith(".png");
                bool b2 = ap.Contains("cmp_");
                return b1 && b2 && ap.Contains(this._enclosing.cmpPdfName);
            }

            internal CmpPngFileFilter(CompareTool _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly CompareTool _enclosing;
        }

        private class DiffPngFileFilter : iText.IO.Util.FileUtil.IFileFilter {
            private String differenceImagePrefix;

            public DiffPngFileFilter(CompareTool _enclosing, String differenceImagePrefix) {
                this._enclosing = _enclosing;
                this.differenceImagePrefix = differenceImagePrefix;
            }

            public virtual bool Accept(FileInfo pathname) {
                String ap = pathname.Name;
                bool b1 = ap.EndsWith(".png");
                bool b2 = ap.StartsWith(this.differenceImagePrefix);
                return b1 && b2;
            }

            private readonly CompareTool _enclosing;
        }

        private class ImageNameComparator : IComparer<FileInfo> {
            public virtual int Compare(FileInfo f1, FileInfo f2) {
                String f1Name = f1.Name;
                String f2Name = f2.Name;
                return string.CompareOrdinal(f1Name, f2Name);
            }

            internal ImageNameComparator(CompareTool _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly CompareTool _enclosing;
        }

        /// <summary>Class containing results of the comparison of two documents.</summary>
        public class CompareResult {
            protected internal IDictionary<CompareTool.ObjectPath, String> differences = new LinkedDictionary<CompareTool.ObjectPath
                , String>();

            protected internal int messageLimit = 1;

            /// <summary>Creates new empty instance of CompareResult with given limit of difference messages.</summary>
            /// <param name="messageLimit">maximum number of difference messages handled by this CompareResult.</param>
            public CompareResult(CompareTool _enclosing, int messageLimit) {
                this._enclosing = _enclosing;
                // LinkedHashMap to retain order. HashMap has different order in Java6/7 and Java8
                this.messageLimit = messageLimit;
            }

            /// <summary>Is used to define if documents are considered equal after comparison.</summary>
            /// <returns>true if documents are equal, false otherwise.</returns>
            public virtual bool IsOk() {
                return this.differences.Count == 0;
            }

            /// <summary>Returns number of differences between two documents met during comparison.</summary>
            /// <returns>number of differences.</returns>
            public virtual int GetErrorCount() {
                return this.differences.Count;
            }

            /// <summary>Converts this CompareResult into text form.</summary>
            /// <returns>text report of the differences between two documents.</returns>
            public virtual String GetReport() {
                StringBuilder sb = new StringBuilder();
                bool firstEntry = true;
                foreach (KeyValuePair<CompareTool.ObjectPath, String> entry in this.differences) {
                    if (!firstEntry) {
                        sb.Append("-----------------------------").Append("\n");
                    }
                    CompareTool.ObjectPath diffPath = entry.Key;
                    sb.Append(entry.Value).Append("\n").Append(diffPath.ToString()).Append("\n");
                    firstEntry = false;
                }
                return sb.ToString();
            }

            /// <summary>
            /// Returns map with
            /// <see cref="ObjectPath"/>
            /// as keys and difference descriptions as values.
            /// </summary>
            /// <returns>differences map which could be used to find in the document objects that are different.</returns>
            public virtual IDictionary<CompareTool.ObjectPath, String> GetDifferences() {
                return this.differences;
            }

            /// <summary>Converts this CompareResult into xml form.</summary>
            /// <param name="stream">output stream to which xml report will be written.</param>
            /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
            /// <exception cref="Javax.Xml.Transform.TransformerException"/>
            public virtual void WriteReportToXml(Stream stream) {
                XmlDocument xmlReport = XmlUtils.InitNewXmlDocument();
                XmlElement root = xmlReport.CreateElement("report");
                XmlElement errors = xmlReport.CreateElement("errors");
                errors.SetAttribute("count", this.differences.Count.ToString());
                root.AppendChild(errors);
                foreach (KeyValuePair<CompareTool.ObjectPath, String> entry in this.differences) {
                    XmlElement errorNode = xmlReport.CreateElement("error");
                    XmlElement message = xmlReport.CreateElement("message");
                    message.AppendChild(xmlReport.CreateTextNode(entry.Value));
                    XmlElement path = entry.Key.ToXmlNode(xmlReport);
                    errorNode.AppendChild(message);
                    errorNode.AppendChild(path);
                    errors.AppendChild(errorNode);
                }
                xmlReport.AppendChild(root);
                XmlUtils.WriteXmlDocToStream(xmlReport, stream);
            }

            protected internal virtual bool IsMessageLimitReached() {
                return this.differences.Count >= this.messageLimit;
            }

            protected internal virtual void AddError(CompareTool.ObjectPath path, String message) {
                if (this.differences.Count < this.messageLimit) {
                    this.differences[((CompareTool.ObjectPath)path.Clone())] = message;
                }
            }

            private readonly CompareTool _enclosing;
        }

        /// <summary>Class that encapsulates information about paths to the objects from the certain base or root object.
        ///     </summary>
        public class ObjectPath {
            protected internal PdfIndirectReference baseCmpObject;

            protected internal PdfIndirectReference baseOutObject;

            protected internal Stack<CompareTool.ObjectPath.LocalPathItem> path = new Stack<CompareTool.ObjectPath.LocalPathItem
                >();

            protected internal Stack<CompareTool.ObjectPath.IndirectPathItem> indirects = new Stack<CompareTool.ObjectPath.IndirectPathItem
                >();

            /// <summary>Creates empty ObjectPath.</summary>
            public ObjectPath() {
            }

            /// <summary>Creates ObjectPath with corresponding root objects in two documents.</summary>
            /// <param name="baseCmpObject">root object in cmp document.</param>
            /// <param name="baseOutObject">root object in out document.</param>
            protected internal ObjectPath(PdfIndirectReference baseCmpObject, PdfIndirectReference baseOutObject) {
                this.baseCmpObject = baseCmpObject;
                this.baseOutObject = baseOutObject;
            }

            private ObjectPath(PdfIndirectReference baseCmpObject, PdfIndirectReference baseOutObject, Stack<CompareTool.ObjectPath.LocalPathItem
                > path, Stack<CompareTool.ObjectPath.IndirectPathItem> indirects) {
                this.baseCmpObject = baseCmpObject;
                this.baseOutObject = baseOutObject;
                this.path = path;
                this.indirects = indirects;
            }

            public virtual CompareTool.ObjectPath ResetDirectPath(PdfIndirectReference baseCmpObject, PdfIndirectReference
                 baseOutObject) {
                CompareTool.ObjectPath newPath = new CompareTool.ObjectPath(baseCmpObject, baseOutObject);
                newPath.indirects = (Stack<CompareTool.ObjectPath.IndirectPathItem>)indirects.Clone();
                newPath.indirects.Push(new CompareTool.ObjectPath.IndirectPathItem(this, baseCmpObject, baseOutObject));
                return newPath;
            }

            public virtual bool IsComparing(PdfIndirectReference baseCmpObject, PdfIndirectReference baseOutObject) {
                return indirects.Contains(new CompareTool.ObjectPath.IndirectPathItem(this, baseCmpObject, baseOutObject));
            }

            public virtual void PushArrayItemToPath(int index) {
                path.Push(new CompareTool.ObjectPath.ArrayPathItem(index));
            }

            public virtual void PushDictItemToPath(PdfName key) {
                path.Push(new CompareTool.ObjectPath.DictPathItem(key));
            }

            public virtual void PushOffsetToPath(int offset) {
                path.Push(new CompareTool.ObjectPath.OffsetPathItem(offset));
            }

            public virtual void Pop() {
                path.Pop();
            }

            public virtual Stack<CompareTool.ObjectPath.LocalPathItem> GetLocalPath() {
                return path;
            }

            public virtual Stack<CompareTool.ObjectPath.IndirectPathItem> GetIndirectPath() {
                return indirects;
            }

            public virtual PdfIndirectReference GetBaseCmpObject() {
                return baseCmpObject;
            }

            public virtual PdfIndirectReference GetBaseOutObject() {
                return baseOutObject;
            }

            public virtual XmlElement ToXmlNode(XmlDocument document) {
                XmlElement element = document.CreateElement("path");
                XmlElement baseNode = document.CreateElement("base");
                baseNode.SetAttribute("cmp", String.Format("{0} {1} obj", baseCmpObject.GetObjNumber(), baseCmpObject.GetGenNumber
                    ()));
                baseNode.SetAttribute("out", String.Format("{0} {1} obj", baseOutObject.GetObjNumber(), baseOutObject.GetGenNumber
                    ()));
                element.AppendChild(baseNode);
                foreach (CompareTool.ObjectPath.LocalPathItem pathItem in path) {
                    element.AppendChild(pathItem.ToXmlNode(document));
                }
                return element;
            }

            public override String ToString() {
                StringBuilder sb = new StringBuilder();
                sb.Append(String.Format("Base cmp object: {0} obj. Base out object: {1} obj", baseCmpObject, baseOutObject
                    ));
                foreach (CompareTool.ObjectPath.LocalPathItem pathItem in path) {
                    sb.Append("\n");
                    sb.Append(pathItem.ToString());
                }
                return sb.ToString();
            }

            public override int GetHashCode() {
                int hashCode = (baseCmpObject != null ? baseCmpObject.GetHashCode() : 0) * 31 + (baseOutObject != null ? baseOutObject
                    .GetHashCode() : 0);
                foreach (CompareTool.ObjectPath.LocalPathItem pathItem in path) {
                    hashCode *= 31;
                    hashCode += pathItem.GetHashCode();
                }
                return hashCode;
            }

            public override bool Equals(Object obj) {
                return obj is CompareTool.ObjectPath && baseCmpObject.Equals(((CompareTool.ObjectPath)obj).baseCmpObject) 
                    && baseOutObject.Equals(((CompareTool.ObjectPath)obj).baseOutObject) && System.Linq.Enumerable.SequenceEqual
                    (path, ((CompareTool.ObjectPath)obj).path);
            }

            protected internal virtual Object Clone() {
                return new CompareTool.ObjectPath(baseCmpObject, baseOutObject, (Stack<CompareTool.ObjectPath.LocalPathItem
                    >)path.Clone(), (Stack<CompareTool.ObjectPath.IndirectPathItem>)indirects.Clone());
            }

            public class IndirectPathItem {
                private PdfIndirectReference cmpObject;

                private PdfIndirectReference outObject;

                public IndirectPathItem(ObjectPath _enclosing, PdfIndirectReference cmpObject, PdfIndirectReference outObject
                    ) {
                    this._enclosing = _enclosing;
                    this.cmpObject = cmpObject;
                    this.outObject = outObject;
                }

                public virtual PdfIndirectReference GetCmpObject() {
                    return this.cmpObject;
                }

                public virtual PdfIndirectReference GetOutObject() {
                    return this.outObject;
                }

                public override int GetHashCode() {
                    return this.cmpObject.GetHashCode() * 31 + this.outObject.GetHashCode();
                }

                public override bool Equals(Object obj) {
                    return (obj is CompareTool.ObjectPath.IndirectPathItem && this.cmpObject.Equals(((CompareTool.ObjectPath.IndirectPathItem
                        )obj).cmpObject) && this.outObject.Equals(((CompareTool.ObjectPath.IndirectPathItem)obj).outObject));
                }

                private readonly ObjectPath _enclosing;
            }

            public abstract class LocalPathItem {
                protected internal abstract XmlElement ToXmlNode(XmlDocument document);
            }

            public class DictPathItem : CompareTool.ObjectPath.LocalPathItem {
                internal PdfName key;

                public DictPathItem(PdfName key) {
                    this.key = key;
                }

                public override String ToString() {
                    return "Dict key: " + key;
                }

                public override int GetHashCode() {
                    return key.GetHashCode();
                }

                public override bool Equals(Object obj) {
                    return obj is CompareTool.ObjectPath.DictPathItem && key.Equals(((CompareTool.ObjectPath.DictPathItem)obj)
                        .key);
                }

                protected internal override XmlElement ToXmlNode(XmlDocument document) {
                    XmlElement element = document.CreateElement("dictKey");
                    element.AppendChild(document.CreateTextNode(key.ToString()));
                    return element;
                }

                public virtual PdfName GetKey() {
                    return key;
                }
            }

            public class ArrayPathItem : CompareTool.ObjectPath.LocalPathItem {
                internal int index;

                public ArrayPathItem(int index) {
                    this.index = index;
                }

                public override String ToString() {
                    return "Array index: " + index.ToString();
                }

                public override int GetHashCode() {
                    return index;
                }

                public override bool Equals(Object obj) {
                    return obj is CompareTool.ObjectPath.ArrayPathItem && index == ((CompareTool.ObjectPath.ArrayPathItem)obj)
                        .index;
                }

                protected internal override XmlElement ToXmlNode(XmlDocument document) {
                    XmlElement element = document.CreateElement("arrayIndex");
                    element.AppendChild(document.CreateTextNode(index.ToString()));
                    return element;
                }

                public virtual int GetIndex() {
                    return index;
                }
            }

            public class OffsetPathItem : CompareTool.ObjectPath.LocalPathItem {
                internal int offset;

                public OffsetPathItem(int offset) {
                    this.offset = offset;
                }

                public virtual int GetOffset() {
                    return offset;
                }

                public override String ToString() {
                    return "Offset: " + offset.ToString();
                }

                public override int GetHashCode() {
                    return offset;
                }

                public override bool Equals(Object obj) {
                    return obj is CompareTool.ObjectPath.OffsetPathItem && offset == ((CompareTool.ObjectPath.OffsetPathItem)obj
                        ).offset;
                }

                protected internal override XmlElement ToXmlNode(XmlDocument document) {
                    XmlElement element = document.CreateElement("offset");
                    element.AppendChild(document.CreateTextNode(offset.ToString()));
                    return element;
                }
            }
        }

        private class TrailerPath : CompareTool.ObjectPath {
            private PdfDocument outDocument;

            private PdfDocument cmpDocument;

            public TrailerPath(PdfDocument cmpDoc, PdfDocument outDoc) {
                outDocument = outDoc;
                cmpDocument = cmpDoc;
            }

            public TrailerPath(PdfDocument cmpDoc, PdfDocument outDoc, Stack<CompareTool.ObjectPath.LocalPathItem> path
                ) {
                this.outDocument = outDoc;
                this.cmpDocument = cmpDoc;
                this.path = path;
            }

            public override XmlElement ToXmlNode(XmlDocument document) {
                XmlElement element = document.CreateElement("path");
                XmlElement baseNode = document.CreateElement("base");
                baseNode.SetAttribute("cmp", "trailer");
                baseNode.SetAttribute("out", "trailer");
                element.AppendChild(baseNode);
                foreach (CompareTool.ObjectPath.LocalPathItem pathItem in path) {
                    element.AppendChild(pathItem.ToXmlNode(document));
                }
                return element;
            }

            public override String ToString() {
                StringBuilder sb = new StringBuilder();
                sb.Append("Base cmp object: trailer. Base out object: trailer");
                foreach (CompareTool.ObjectPath.LocalPathItem pathItem in path) {
                    sb.Append("\n");
                    sb.Append(pathItem.ToString());
                }
                return sb.ToString();
            }

            public override int GetHashCode() {
                int hashCode = outDocument.GetHashCode() * 31 + cmpDocument.GetHashCode();
                foreach (CompareTool.ObjectPath.LocalPathItem pathItem in path) {
                    hashCode *= 31;
                    hashCode += pathItem.GetHashCode();
                }
                return hashCode;
            }

            public override bool Equals(Object obj) {
                return obj is CompareTool.TrailerPath && outDocument.Equals(((CompareTool.TrailerPath)obj).outDocument) &&
                     cmpDocument.Equals(((CompareTool.TrailerPath)obj).cmpDocument) && System.Linq.Enumerable.SequenceEqual
                    (path, ((CompareTool.ObjectPath)obj).path);
            }

            protected internal override Object Clone() {
                return new CompareTool.TrailerPath(cmpDocument, outDocument, (Stack<CompareTool.ObjectPath.LocalPathItem>)
                    path.Clone());
            }
        }
    }
}
