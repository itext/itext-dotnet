/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions.Contexts;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils.Objectpathitems;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;

namespace iText.Kernel.Utils {
    /// <summary>
    /// This class provides means to compare two PDF files both by content and visually
    /// and gives the report on their differences.
    /// </summary>
    /// <remarks>
    /// This class provides means to compare two PDF files both by content and visually
    /// and gives the report on their differences.
    /// <para />
    /// For visual comparison it uses external tools: Ghostscript and ImageMagick, which
    /// should be installed on your machine. To allow CompareTool to use them, you need
    /// to pass either java properties or environment variables with names "ITEXT_GS_EXEC" and
    /// "ITEXT_MAGICK_COMPARE_EXEC", which would contain the commands to execute the
    /// Ghostscript and ImageMagick tools.
    /// <para />
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
        private const String FILE_PROTOCOL = "file://";

        private const String UNEXPECTED_NUMBER_OF_PAGES = "Unexpected number of pages for <filename>.";

        private const String DIFFERENT_PAGES = "File " + FILE_PROTOCOL + "<filename> differs on page <pagenumber>.";

        private const String IGNORED_AREAS_PREFIX = "ignored_areas_";

        private const String VERSION_REGEXP = "(\\d+\\.)+\\d+(-SNAPSHOT)?";

        private const String VERSION_REPLACEMENT = "<version>";

        private const String COPYRIGHT_REGEXP = "\u00a9\\d+-\\d+ (?:iText Group NV|Apryse Group NV)";

        private const String COPYRIGHT_REPLACEMENT = "\u00a9<copyright years> Apryse Group NV";

        private const String NEW_LINES = "\\r|\\n";

        private String cmpPdfName;

        private String outPdfName;

        private String cmpPdf;

        private String cmpImage;

        private String outPdf;

        private String outImage;

        private ReaderProperties outProps;

        private ReaderProperties cmpProps;

        private IList<PdfIndirectReference> outPagesRef;

        private IList<PdfIndirectReference> cmpPagesRef;

        private int compareByContentErrorsLimit = 1000;

        private bool generateCompareByContentXmlReport = false;

        private bool encryptionCompareEnabled = false;

        private bool kdfSaltCompareEnabled = true;

        private bool useCachedPagesForComparison = true;

        private IMetaInfo metaInfo;

        private String gsExec;

        private String compareExec;

        /// <summary>
        /// Create new
        /// <see cref="CompareTool"/>
        /// instance.
        /// </summary>
        public CompareTool() {
        }

//\cond DO_NOT_DOCUMENT
        internal CompareTool(String gsExec, String compareExec) {
            this.gsExec = gsExec;
            this.compareExec = compareExec;
        }
//\endcond

        /// <summary>
        /// Create
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// optimized for tests.
        /// </summary>
        /// <param name="filename">File to write to when necessary.</param>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// to be used in tests.
        /// </returns>
        public static PdfWriter CreateTestPdfWriter(String filename) {
            return CreateTestPdfWriter(filename, new WriterProperties());
        }

        /// <summary>
        /// Create
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// optimized for tests.
        /// </summary>
        /// <param name="filename">File to write to when necessary.</param>
        /// <param name="properties">
        /// 
        /// <see cref="iText.Kernel.Pdf.WriterProperties"/>
        /// to use.
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// to be used in tests.
        /// </returns>
        public static PdfWriter CreateTestPdfWriter(String filename, WriterProperties properties) {
            return new MemoryFirstPdfWriter(filename, properties);
        }

        // Android-Conversion-Replace return new PdfWriter(filename, properties);
        /// <summary>
        /// Create
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// out of the data created recently or read from disk.
        /// </summary>
        /// <param name="filename">File to read the data from when necessary.</param>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// to be used in tests.
        /// </returns>
        public static PdfReader CreateOutputReader(String filename) {
            return iText.Kernel.Utils.CompareTool.CreateOutputReader(filename, new ReaderProperties());
        }

        /// <summary>
        /// Create
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// out of the data created recently or read from disk.
        /// </summary>
        /// <param name="filename">File to read the data from when necessary.</param>
        /// <param name="properties">
        /// 
        /// <see cref="iText.Kernel.Pdf.ReaderProperties"/>
        /// to use.
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// to be used in tests.
        /// </returns>
        public static PdfReader CreateOutputReader(String filename, ReaderProperties properties) {
            MemoryFirstPdfWriter outWriter = MemoryFirstPdfWriter.Get(filename);
            if (outWriter != null) {
                return new PdfReader(new MemoryStream(outWriter.GetBAOutputStream().ToArray()), properties);
            }
            else {
                return new PdfReader(filename, properties);
            }
        }

        /// <summary>Clean up memory occupied for the tests.</summary>
        /// <param name="path">Path to clean up memory for.</param>
        public static void Cleanup(String path) {
            MemoryFirstPdfWriter.Cleanup(path);
        }

        /// <summary>
        /// Compares two PDF documents by content starting from Catalog dictionary and then recursively comparing
        /// corresponding objects which are referenced from it.
        /// </summary>
        /// <remarks>
        /// Compares two PDF documents by content starting from Catalog dictionary and then recursively comparing
        /// corresponding objects which are referenced from it. You can roughly imagine it as depth-first traversal
        /// of the two trees that represent pdf objects structure of the documents.
        /// <para />
        /// The main difference between this method and the
        /// <see cref="CompareByContent(System.String, System.String, System.String, System.String)"/>
        /// methods is the return value. This method returns a
        /// <see cref="CompareResult"/>
        /// class instance, which could be used
        /// in code, whilst compareByContent methods in case of the differences simply return String value, which could
        /// only be printed. Also, keep in mind that this method doesn't perform visual comparison of the documents.
        /// <para />
        /// For more explanations about what outDoc and cmpDoc are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outDocument">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// corresponding to the output file, which is to be compared with cmp-file.
        /// </param>
        /// <param name="cmpDocument">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// corresponding to the cmp-file, which is to be compared with output file.
        /// </param>
        /// <returns>
        /// the report on comparison of two files in the form of the custom class
        /// <see cref="CompareResult"/>
        /// instance.
        /// </returns>
        /// <seealso cref="CompareResult"/>
        public virtual CompareTool.CompareResult CompareByCatalog(PdfDocument outDocument, PdfDocument cmpDocument
            ) {
            CompareTool.CompareResult compareResult = null;
            compareResult = new CompareTool.CompareResult(compareByContentErrorsLimit);
            ObjectPath catalogPath = new ObjectPath(cmpDocument.GetCatalog().GetPdfObject().GetIndirectReference(), outDocument
                .GetCatalog().GetPdfObject().GetIndirectReference());
            ICollection<PdfName> ignoredCatalogEntries = new LinkedHashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Metadata
                ));
            CompareDictionariesExtended(outDocument.GetCatalog().GetPdfObject(), cmpDocument.GetCatalog().GetPdfObject
                (), catalogPath, compareResult, ignoredCatalogEntries);
            // Method compareDictionariesExtended eventually calls compareObjects method which doesn't compare page objects.
            // At least for now compare page dictionaries explicitly here like this.
            if (cmpPagesRef == null || outPagesRef == null) {
                return compareResult;
            }
            if (outPagesRef.Count != cmpPagesRef.Count && !compareResult.IsMessageLimitReached()) {
                compareResult.AddError(catalogPath, "Documents have different numbers of pages.");
            }
            for (int i = 0; i < Math.Min(cmpPagesRef.Count, outPagesRef.Count); i++) {
                if (compareResult.IsMessageLimitReached()) {
                    break;
                }
                ObjectPath currentPath = new ObjectPath(cmpPagesRef[i], outPagesRef[i]);
                PdfDictionary outPageDict = (PdfDictionary)outPagesRef[i].GetRefersTo();
                PdfDictionary cmpPageDict = (PdfDictionary)cmpPagesRef[i].GetRefersTo();
                CompareDictionariesExtended(outPageDict, cmpPageDict, currentPath, compareResult);
            }
            return compareResult;
        }

        /// <summary>Disables the default logic of pages comparison.</summary>
        /// <remarks>
        /// Disables the default logic of pages comparison.
        /// This option makes sense only for
        /// <see cref="CompareByCatalog(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Pdf.PdfDocument)"/>
        /// method.
        /// <para />
        /// By default, pages are treated as special objects and if they are met in the process of comparison, then they are
        /// not checked as objects, but rather simply checked that they have same page numbers in both documents.
        /// This behaviour is intended for the
        /// <see cref="CompareByContent(System.String, System.String, System.String)"/>
        /// set of methods, because in them documents are compared in page by page basis. Thus, we don't need to check if pages
        /// are of the same content when they are met in comparison process, we are sure that we will compare their content or
        /// we have already compared them.
        /// <para />
        /// However, if you would use
        /// <see cref="CompareByCatalog(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Pdf.PdfDocument)"/>
        /// with default behaviour
        /// of pages comparison, pages won't be checked at all, every time when reference to the page dictionary is met,
        /// only page numbers will be compared for both documents. You can say that in this case, comparison will be performed
        /// for all document's catalog entries except /Pages (However in fact, document's page tree structures will be compared,
        /// but pages themselves - won't).
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="CompareTool"/>
        /// instance.
        /// </returns>
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

        /// <summary>Enables or disables the generation of the comparison report in the form of an xml document.</summary>
        /// <remarks>
        /// Enables or disables the generation of the comparison report in the form of an xml document.
        /// <para />
        /// IMPORTANT NOTE: this flag affects only the comparison performed by compareByContent methods!
        /// </remarks>
        /// <param name="generateCompareByContentXmlReport">true to enable xml report generation, false - to disable.</param>
        /// <returns>this CompareTool instance.</returns>
        public virtual iText.Kernel.Utils.CompareTool SetGenerateCompareByContentXmlReport(bool generateCompareByContentXmlReport
            ) {
            this.generateCompareByContentXmlReport = generateCompareByContentXmlReport;
            return this;
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Commons.Actions.Contexts.IMetaInfo"/>
        /// info that will be used for both read and written documents creation.
        /// </summary>
        /// <param name="metaInfo">meta info to set</param>
        public virtual void SetEventCountingMetaInfo(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
        }

        /// <summary>Enables the comparison of the encryption properties of the documents.</summary>
        /// <remarks>
        /// Enables the comparison of the encryption properties of the documents. Encryption properties comparison
        /// results are returned along with all other comparison results.
        /// <para />
        /// IMPORTANT NOTE: this flag affects only the comparison performed by compareByContent methods!
        /// <see cref="CompareByCatalog(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Pdf.PdfDocument)"/>
        /// doesn't compare encryption properties
        /// because encryption properties aren't part of the document's Catalog.
        /// </remarks>
        /// <returns>this CompareTool instance.</returns>
        public virtual iText.Kernel.Utils.CompareTool EnableEncryptionCompare() {
            return EnableEncryptionCompare(true);
        }

        /// <summary>Enables the comparison of the encryption properties of the documents.</summary>
        /// <remarks>
        /// Enables the comparison of the encryption properties of the documents. Encryption properties comparison
        /// results are returned along with all other comparison results.
        /// <para />
        /// IMPORTANT NOTE: this flag affects only the comparison performed by compareByContent methods!
        /// <see cref="CompareByCatalog(iText.Kernel.Pdf.PdfDocument, iText.Kernel.Pdf.PdfDocument)"/>
        /// doesn't compare encryption properties
        /// because encryption properties aren't part of the document's Catalog.
        /// </remarks>
        /// <param name="kdfSaltCompareEnabled">
        /// set to
        /// <see langword="true"/>
        /// if
        /// <see cref="iText.Kernel.Pdf.PdfName.KDFSalt"/>
        /// entry must be compared,
        /// {code false} otherwise
        /// </param>
        /// <returns>this CompareTool instance.</returns>
        public virtual iText.Kernel.Utils.CompareTool EnableEncryptionCompare(bool kdfSaltCompareEnabled) {
            this.encryptionCompareEnabled = true;
            this.kdfSaltCompareEnabled = kdfSaltCompareEnabled;
            return this;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.ReaderProperties"/>
        /// to be passed later to the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// of the output document.
        /// </summary>
        /// <remarks>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.ReaderProperties"/>
        /// to be passed later to the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// of the output document.
        /// <para />
        /// Documents for comparison are opened in reader mode. This method is intended to alter
        /// <see cref="iText.Kernel.Pdf.ReaderProperties"/>
        /// which are used to open the output document. This is particularly useful for comparison of encrypted documents.
        /// <para />
        /// For more explanations about what outDoc and cmpDoc are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.ReaderProperties"/>
        /// instance to be passed later to the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// of the output document.
        /// </returns>
        public virtual ReaderProperties GetOutReaderProperties() {
            if (outProps == null) {
                outProps = new ReaderProperties();
            }
            return outProps;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.ReaderProperties"/>
        /// to be passed later to the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// of the cmp document.
        /// </summary>
        /// <remarks>
        /// Gets
        /// <see cref="iText.Kernel.Pdf.ReaderProperties"/>
        /// to be passed later to the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// of the cmp document.
        /// <para />
        /// Documents for comparison are opened in reader mode. This method is intended to alter
        /// <see cref="iText.Kernel.Pdf.ReaderProperties"/>
        /// which are used to open the cmp document. This is particularly useful for comparison of encrypted documents.
        /// <para />
        /// For more explanations about what outDoc and cmpDoc are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.ReaderProperties"/>
        /// instance to be passed later to the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// of the cmp document.
        /// </returns>
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
        /// <para />
        /// Note, that this method uses
        /// <see cref="iText.IO.Util.ImageMagickHelper"/>
        /// and
        /// <see cref="iText.IO.Util.GhostscriptHelper"/>
        /// classes and therefore may
        /// create temporary files and directories.
        /// <para />
        /// During comparison for every page of the two documents an image file will be created in the folder specified by
        /// outPath parameter. Then those page images will be compared and if there are any differences for some pages,
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
        /// <para />
        /// Note, that this method uses
        /// <see cref="iText.IO.Util.ImageMagickHelper"/>
        /// and
        /// <see cref="iText.IO.Util.GhostscriptHelper"/>
        /// classes and therefore may
        /// create temporary files and directories.
        /// <para />
        /// During comparison for every page of two documents an image file will be created in the folder specified by
        /// outPath parameter. Then those page images will be compared and if there are any differences for some pages,
        /// another image file will be created with marked differences on it.
        /// <para />
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
        public virtual String CompareVisually(String outPdf, String cmpPdf, String outPath, String differenceImagePrefix
            , IDictionary<int, IList<Rectangle>> ignoredAreas) {
            Init(outPdf, cmpPdf);
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outPdf));
            System.Console.Out.WriteLine("Cmp pdf: " + UrlUtil.GetNormalizedFileUriString(cmpPdf) + "\n");
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
        /// <para />
        /// When comparison by content is finished, if any differences were found, visual comparison is automatically started.
        /// For this overload, differenceImagePrefix value is generated using diff_%outPdfFileName%_ format.
        /// <para />
        /// For more explanations about what outPdf and cmpPdf are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <returns>
        /// string containing text report on the encountered content differences and also list of the pages that are
        /// visually different, or null if there are no content and therefore no visual differences.
        /// </returns>
        /// <seealso cref="CompareVisually(System.String, System.String, System.String, System.String)"/>
        public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath) {
            return CompareByContent(outPdf, cmpPdf, outPath, null, null, null, null);
        }

        /// <summary>
        /// Compares two PDF documents by content starting from page dictionaries and then recursively comparing
        /// corresponding objects which are referenced from them.
        /// </summary>
        /// <remarks>
        /// Compares two PDF documents by content starting from page dictionaries and then recursively comparing
        /// corresponding objects which are referenced from them. You can roughly imagine it as depth-first traversal
        /// of the two trees that represent pdf objects structure of the documents.
        /// <para />
        /// When comparison by content is finished, if any differences were found, visual comparison is automatically started.
        /// <para />
        /// For more explanations about what outPdf and cmpPdf are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <param name="differenceImagePrefix">
        /// file name prefix for image files with marked visual differences if there are any;
        /// if it's set to null the prefix defaults to diff_%outPdfFileName%_ format.
        /// </param>
        /// <returns>
        /// string containing text report on the encountered content differences and also list of the pages that are
        /// visually different, or null if there are no content and therefore no visual differences.
        /// </returns>
        /// <seealso cref="CompareVisually(System.String, System.String, System.String, System.String)"/>
        public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath, String differenceImagePrefix
            ) {
            return CompareByContent(outPdf, cmpPdf, outPath, differenceImagePrefix, null, null, null);
        }

        /// <summary>This method overload is used to compare two encrypted PDF documents.</summary>
        /// <remarks>
        /// This method overload is used to compare two encrypted PDF documents. Document passwords are passed with
        /// outPass and cmpPass parameters.
        /// <para />
        /// Compares two PDF documents by content starting from page dictionaries and then recursively comparing
        /// corresponding objects which are referenced from them. You can roughly imagine it as depth-first traversal
        /// of the two trees that represent pdf objects structure of the documents.
        /// <para />
        /// When comparison by content is finished, if any differences were found, visual comparison is automatically started.
        /// For more info see
        /// <see cref="CompareVisually(System.String, System.String, System.String, System.String)"/>.
        /// <para />
        /// For more explanations about what outPdf and cmpPdf are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <param name="differenceImagePrefix">
        /// file name prefix for image files with marked visual differences if there is any;
        /// if it's set to null the prefix defaults to diff_%outPdfFileName%_ format.
        /// </param>
        /// <param name="outPass">password for the encrypted document specified by the outPdf absolute path.</param>
        /// <param name="cmpPass">password for the encrypted document specified by the cmpPdf absolute path.</param>
        /// <returns>
        /// string containing text report on the encountered content differences and also list of the pages that are
        /// visually different, or null if there are no content and therefore no visual differences.
        /// </returns>
        /// <seealso cref="CompareVisually(System.String, System.String, System.String, System.String)"/>
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
        /// <para />
        /// When comparison by content is finished, if any differences were found, visual comparison is automatically started.
        /// <para />
        /// For more explanations about what outPdf and cmpPdf are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <param name="differenceImagePrefix">
        /// file name prefix for image files with marked visual differences if there are any;
        /// if it's set to null the prefix defaults to diff_%outPdfFileName%_ format.
        /// </param>
        /// <param name="ignoredAreas">a map with one-based page numbers as keys and lists of ignored rectangles as values.
        ///     </param>
        /// <returns>
        /// string containing text report on the encountered content differences and also list of the pages that are
        /// visually different, or null if there are no content and therefore no visual differences.
        /// </returns>
        /// <seealso cref="CompareVisually(System.String, System.String, System.String, System.String)"/>
        public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath, String differenceImagePrefix
            , IDictionary<int, IList<Rectangle>> ignoredAreas) {
            return CompareByContent(outPdf, cmpPdf, outPath, differenceImagePrefix, ignoredAreas, null, null);
        }

        /// <summary>This method overload is used to compare two encrypted PDF documents.</summary>
        /// <remarks>
        /// This method overload is used to compare two encrypted PDF documents. Document passwords are passed with
        /// outPass and cmpPass parameters.
        /// <para />
        /// Compares two PDF documents by content starting from page dictionaries and then recursively comparing
        /// corresponding objects which are referenced from them. You can roughly imagine it as depth-first traversal
        /// of the two trees that represent pdf objects structure of the documents.
        /// <para />
        /// When comparison by content is finished, if any differences were found, visual comparison is automatically started.
        /// <para />
        /// For more explanations about what outPdf and cmpPdf are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which is to be compared to cmp-file.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which is to be compared to output file.</param>
        /// <param name="outPath">the absolute path to the folder, which will be used to store image files for visual comparison.
        ///     </param>
        /// <param name="differenceImagePrefix">
        /// file name prefix for image files with marked visual differences if there are any;
        /// if it's set to null the prefix defaults to diff_%outPdfFileName%_ format.
        /// </param>
        /// <param name="ignoredAreas">a map with one-based page numbers as keys and lists of ignored rectangles as values.
        ///     </param>
        /// <param name="outPass">password for the encrypted document specified by the outPdf absolute path.</param>
        /// <param name="cmpPass">password for the encrypted document specified by the cmpPdf absolute path.</param>
        /// <returns>
        /// string containing text report on the encountered content differences and also list of the pages that are
        /// visually different, or null if there are no content and therefore no visual differences.
        /// </returns>
        /// <seealso cref="CompareVisually(System.String, System.String, System.String, System.String)"/>
        public virtual String CompareByContent(String outPdf, String cmpPdf, String outPath, String differenceImagePrefix
            , IDictionary<int, IList<Rectangle>> ignoredAreas, byte[] outPass, byte[] cmpPass) {
            Init(outPdf, cmpPdf);
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(outPdf));
            System.Console.Out.WriteLine("Cmp pdf: " + UrlUtil.GetNormalizedFileUriString(cmpPdf) + "\n");
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
        public virtual bool CompareDictionaries(PdfDictionary outDict, PdfDictionary cmpDict) {
            return CompareDictionariesExtended(outDict, cmpDict, null, null);
        }

        /// <summary>Recursively compares structures of two corresponding dictionaries from out and cmp PDF documents.
        ///     </summary>
        /// <remarks>
        /// Recursively compares structures of two corresponding dictionaries from out and cmp PDF documents. You can roughly
        /// imagine it as depth-first traversal of the two trees that represent pdf objects structure of the documents.
        /// <para />
        /// Both out and cmp
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// shall have indirect references.
        /// <para />
        /// By default page dictionaries are excluded from the comparison when met and are instead compared in a special manner,
        /// simply comparing their page numbers. This behavior can be disabled by calling
        /// <see cref="DisableCachedPagesComparison()"/>.
        /// <para />
        /// For more explanations about what outPdf and cmpPdf are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outDict">
        /// an indirect
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from the output file, which is to be compared to cmp-file dictionary.
        /// </param>
        /// <param name="cmpDict">
        /// an indirect
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from the cmp-file file, which is to be compared to output file dictionary.
        /// </param>
        /// <returns>
        /// 
        /// <see cref="CompareResult"/>
        /// instance containing differences between the two dictionaries,
        /// or
        /// <see langword="null"/>
        /// if dictionaries are equal.
        /// </returns>
        public virtual CompareTool.CompareResult CompareDictionariesStructure(PdfDictionary outDict, PdfDictionary
             cmpDict) {
            return CompareDictionariesStructure(outDict, cmpDict, null);
        }

        /// <summary>Recursively compares structures of two corresponding dictionaries from out and cmp PDF documents.
        ///     </summary>
        /// <remarks>
        /// Recursively compares structures of two corresponding dictionaries from out and cmp PDF documents. You can roughly
        /// imagine it as depth-first traversal of the two trees that represent pdf objects structure of the documents.
        /// <para />
        /// Both out and cmp
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// shall have indirect references.
        /// <para />
        /// By default page dictionaries are excluded from the comparison when met and are instead compared in a special manner,
        /// simply comparing their page numbers. This behavior can be disabled by calling
        /// <see cref="DisableCachedPagesComparison()"/>.
        /// <para />
        /// For more explanations about what outPdf and cmpPdf are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outDict">
        /// an indirect
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from the output file, which is to be compared to cmp-file dictionary.
        /// </param>
        /// <param name="cmpDict">
        /// an indirect
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// from the cmp-file file, which is to be compared to output file dictionary.
        /// </param>
        /// <param name="excludedKeys">
        /// a
        /// <see cref="Java.Util.Set{E}"/>
        /// of names that designate entries from
        /// <paramref name="outDict"/>
        /// and
        /// <paramref name="cmpDict"/>
        /// dictionaries
        /// which are to be skipped during comparison.
        /// </param>
        /// <returns>
        /// 
        /// <see cref="CompareResult"/>
        /// instance containing differences between the two dictionaries,
        /// or
        /// <see langword="null"/>
        /// if dictionaries are equal.
        /// </returns>
        public virtual CompareTool.CompareResult CompareDictionariesStructure(PdfDictionary outDict, PdfDictionary
             cmpDict, ICollection<PdfName> excludedKeys) {
            if (outDict.GetIndirectReference() == null || cmpDict.GetIndirectReference() == null) {
                throw new ArgumentException("The 'outDict' and 'cmpDict' objects shall have indirect references.");
            }
            CompareTool.CompareResult compareResult = new CompareTool.CompareResult(compareByContentErrorsLimit);
            ObjectPath currentPath = new ObjectPath(cmpDict.GetIndirectReference(), outDict.GetIndirectReference());
            if (!CompareDictionariesExtended(outDict, cmpDict, currentPath, compareResult, excludedKeys)) {
                System.Diagnostics.Debug.Assert(!compareResult.IsOk());
                System.Console.Out.WriteLine(compareResult.GetReport());
                return compareResult;
            }
            System.Diagnostics.Debug.Assert(compareResult.IsOk());
            return null;
        }

        /// <summary>Compares structures of two corresponding streams from out and cmp PDF documents.</summary>
        /// <remarks>
        /// Compares structures of two corresponding streams from out and cmp PDF documents. You can roughly
        /// imagine it as depth-first traversal of the two trees that represent pdf objects structure of the documents.
        /// <para />
        /// For more explanations about what outPdf and cmpPdf are see last paragraph of the
        /// <see cref="CompareTool"/>
        /// class description.
        /// </remarks>
        /// <param name="outStream">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// from the output file, which is to be compared to cmp-file stream.
        /// </param>
        /// <param name="cmpStream">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// from the cmp-file file, which is to be compared to output file stream.
        /// </param>
        /// <returns>
        /// 
        /// <see cref="CompareResult"/>
        /// instance containing differences between the two streams,
        /// or
        /// <see langword="null"/>
        /// if streams are equal.
        /// </returns>
        public virtual CompareTool.CompareResult CompareStreamsStructure(PdfStream outStream, PdfStream cmpStream) {
            CompareTool.CompareResult compareResult = new CompareTool.CompareResult(compareByContentErrorsLimit);
            ObjectPath currentPath = new ObjectPath(cmpStream.GetIndirectReference(), outStream.GetIndirectReference()
                );
            if (!CompareStreamsExtended(outStream, cmpStream, currentPath, compareResult)) {
                System.Diagnostics.Debug.Assert(!compareResult.IsOk());
                System.Console.Out.WriteLine(compareResult.GetReport());
                return compareResult;
            }
            System.Diagnostics.Debug.Assert(compareResult.IsOk());
            return null;
        }

        /// <summary>Simple method that compares two given PdfStreams by content.</summary>
        /// <remarks>
        /// Simple method that compares two given PdfStreams by content. This is "deep" comparing, which means that all
        /// nested objects are also compared by content.
        /// </remarks>
        /// <param name="outStream">stream to compare.</param>
        /// <param name="cmpStream">stream to compare.</param>
        /// <returns>true if stream are equal by content, otherwise false.</returns>
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
        /// <returns>text report on the xmp differences, or null if there are no differences.</returns>
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
        /// <returns>text report on the xmp differences, or null if there are no differences.</returns>
        public virtual String CompareXmp(String outPdf, String cmpPdf, bool ignoreDateAndProducerProperties) {
            Init(outPdf, cmpPdf);
            try {
                using (PdfReader readerCmp = iText.Kernel.Utils.CompareTool.CreateOutputReader(this.cmpPdf)) {
                    using (PdfDocument cmpDocument = new PdfDocument(readerCmp, new DocumentProperties().SetEventCountingMetaInfo
                        (metaInfo))) {
                        using (PdfReader readerOut = iText.Kernel.Utils.CompareTool.CreateOutputReader(this.outPdf)) {
                            using (PdfDocument outDocument = new PdfDocument(readerOut, new DocumentProperties().SetEventCountingMetaInfo
                                (metaInfo))) {
                                byte[] cmpBytes = cmpDocument.GetXmpMetadataBytes();
                                byte[] outBytes = outDocument.GetXmpMetadataBytes();
                                if (ignoreDateAndProducerProperties) {
                                    XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(cmpBytes, new ParseOptions().SetOmitNormalization(true));
                                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.CreateDate, true, true);
                                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.ModifyDate, true, true);
                                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.MetadataDate, true, true);
                                    XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_PDF, PdfConst.Producer, true, true);
                                    cmpBytes = XMPMetaFactory.SerializeToBuffer(xmpMeta, new SerializeOptions(SerializeOptions.SORT));
                                    xmpMeta = XMPMetaFactory.ParseFromBuffer(outBytes, new ParseOptions().SetOmitNormalization(true));
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
                        }
                    }
                }
            }
            catch (Exception) {
                return "XMP parsing failure!";
            }
            return null;
        }

        /// <summary>Utility method that provides simple comparison of the two xml files stored in byte arrays.</summary>
        /// <param name="xml1">first xml file data to compare.</param>
        /// <param name="xml2">second xml file data to compare.</param>
        /// <returns>true if xml structures are identical, false otherwise.</returns>
        public virtual bool CompareXmls(byte[] xml1, byte[] xml2) {
            return XmlUtils.CompareXmls(new MemoryStream(xml1), new MemoryStream(xml2));
        }

        /// <summary>Utility method that provides simple comparison of the two xml files.</summary>
        /// <param name="outXmlFile">absolute path to the out xml file to compare.</param>
        /// <param name="cmpXmlFile">absolute path to the cmp xml file to compare.</param>
        /// <returns>true if xml structures are identical, false otherwise.</returns>
        public virtual bool CompareXmls(String outXmlFile, String cmpXmlFile) {
            System.Console.Out.WriteLine("Out xml: " + UrlUtil.GetNormalizedFileUriString(outXmlFile));
            System.Console.Out.WriteLine("Cmp xml: " + UrlUtil.GetNormalizedFileUriString(cmpXmlFile) + "\n");
            using (Stream outXmlStream = FileUtil.GetInputStreamForFile(outXmlFile)) {
                using (Stream cmpXmlStream = FileUtil.GetInputStreamForFile(cmpXmlFile)) {
                    return XmlUtils.CompareXmls(outXmlStream, cmpXmlStream);
                }
            }
        }

        /// <summary>Compares document info dictionaries of two pdf documents.</summary>
        /// <remarks>
        /// Compares document info dictionaries of two pdf documents.
        /// <para />
        /// This method overload is used to compare two encrypted PDF documents. Document passwords are passed with
        /// outPass and cmpPass parameters.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which info is to be compared to cmp-file info.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which info is to be compared to output file info.</param>
        /// <param name="outPass">password for the encrypted document specified by the outPdf absolute path.</param>
        /// <param name="cmpPass">password for the encrypted document specified by the cmpPdf absolute path.</param>
        /// <returns>text report on the differences in documents infos.</returns>
        public virtual String CompareDocumentInfo(String outPdf, String cmpPdf, byte[] outPass, byte[] cmpPass) {
            System.Console.Out.Write("[itext] INFO  Comparing document info.......");
            String message = null;
            SetPassword(outPass, cmpPass);
            using (PdfReader readerOut = iText.Kernel.Utils.CompareTool.CreateOutputReader(outPdf, GetOutReaderProperties
                ())) {
                using (PdfDocument outDocument = new PdfDocument(readerOut, new DocumentProperties().SetEventCountingMetaInfo
                    (metaInfo))) {
                    using (PdfReader readerCmp = iText.Kernel.Utils.CompareTool.CreateOutputReader(cmpPdf, GetCmpReaderProperties
                        ())) {
                        using (PdfDocument cmpDocument = new PdfDocument(readerCmp, new DocumentProperties().SetEventCountingMetaInfo
                            (metaInfo))) {
                            String[] cmpInfo = ConvertDocInfoToStrings(cmpDocument.GetDocumentInfo());
                            String[] outInfo = ConvertDocInfoToStrings(outDocument.GetDocumentInfo());
                            for (int i = 0; i < cmpInfo.Length; ++i) {
                                if (!cmpInfo[i].Equals(outInfo[i])) {
                                    message = MessageFormatUtil.Format("Document info fail. Expected: \"{0}\", actual: \"{1}\"", cmpInfo[i], outInfo
                                        [i]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (message == null) {
                System.Console.Out.WriteLine("OK");
            }
            else {
                iText.Kernel.Utils.CompareTool.WriteOnDisk(outPdf);
                iText.Kernel.Utils.CompareTool.WriteOnDiskIfNotExists(cmpPdf);
                System.Console.Out.WriteLine("Fail");
            }
            System.Console.Out.Flush();
            return message;
        }

        /// <summary>Compares document info dictionaries of two pdf documents.</summary>
        /// <param name="outPdf">the absolute path to the output file, which info is to be compared to cmp-file info.</param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which info is to be compared to output file info.</param>
        /// <returns>text report on the differences in documents infos.</returns>
        public virtual String CompareDocumentInfo(String outPdf, String cmpPdf) {
            return CompareDocumentInfo(outPdf, cmpPdf, null, null);
        }

        /// <summary>Checks if two documents have identical link annotations on corresponding pages.</summary>
        /// <param name="outPdf">the absolute path to the output file, which links are to be compared to cmp-file links.
        ///     </param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which links are to be compared to output file links.
        ///     </param>
        /// <returns>text report on the differences in documents links.</returns>
        public virtual String CompareLinkAnnotations(String outPdf, String cmpPdf) {
            System.Console.Out.Write("[itext] INFO  Comparing link annotations....");
            String message = null;
            using (PdfReader readerOut = iText.Kernel.Utils.CompareTool.CreateOutputReader(outPdf)) {
                using (PdfDocument outDocument = new PdfDocument(readerOut, new DocumentProperties().SetEventCountingMetaInfo
                    (metaInfo))) {
                    using (PdfReader readerCmp = iText.Kernel.Utils.CompareTool.CreateOutputReader(cmpPdf)) {
                        using (PdfDocument cmpDocument = new PdfDocument(readerCmp, new DocumentProperties().SetEventCountingMetaInfo
                            (metaInfo))) {
                            for (int i = 0; i < outDocument.GetNumberOfPages() && i < cmpDocument.GetNumberOfPages(); i++) {
                                IList<PdfLinkAnnotation> outLinks = GetLinkAnnotations(i + 1, outDocument);
                                IList<PdfLinkAnnotation> cmpLinks = GetLinkAnnotations(i + 1, cmpDocument);
                                if (cmpLinks.Count != outLinks.Count) {
                                    message = MessageFormatUtil.Format("Different number of links on page {0}.", i + 1);
                                    break;
                                }
                                for (int j = 0; j < cmpLinks.Count; j++) {
                                    if (!CompareLinkAnnotations(cmpLinks[j], outLinks[j], cmpDocument, outDocument)) {
                                        message = MessageFormatUtil.Format("Different links on page {0}.\n{1}\n{2}", i + 1, cmpLinks[j].ToString()
                                            , outLinks[j].ToString());
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (message == null) {
                System.Console.Out.WriteLine("OK");
            }
            else {
                iText.Kernel.Utils.CompareTool.WriteOnDisk(outPdf);
                iText.Kernel.Utils.CompareTool.WriteOnDiskIfNotExists(cmpPdf);
                System.Console.Out.WriteLine("Fail");
            }
            System.Console.Out.Flush();
            return message;
        }

        /// <summary>Compares tag structures of the two PDF documents.</summary>
        /// <remarks>
        /// Compares tag structures of the two PDF documents.
        /// <para />
        /// This method creates xml files in the same folder with outPdf file. These xml files contain documents tag structures
        /// converted into the xml structure. These xml files are compared if they are equal.
        /// </remarks>
        /// <param name="outPdf">the absolute path to the output file, which tags are to be compared to cmp-file tags.
        ///     </param>
        /// <param name="cmpPdf">the absolute path to the cmp-file, which tags are to be compared to output file tags.
        ///     </param>
        /// <returns>text report of the differences in documents tags.</returns>
        public virtual String CompareTagStructures(String outPdf, String cmpPdf) {
            System.Console.Out.Write("[itext] INFO  Comparing tag structures......");
            String outXmlPath = outPdf.Replace(".pdf", ".xml");
            String cmpXmlPath = outPdf.Replace(".pdf", ".cmp.xml");
            String message = null;
            using (PdfReader readerOut = iText.Kernel.Utils.CompareTool.CreateOutputReader(outPdf)) {
                using (PdfDocument docOut = new PdfDocument(readerOut, new DocumentProperties().SetEventCountingMetaInfo(metaInfo
                    ))) {
                    using (Stream xmlOut = FileUtil.GetFileOutputStream(outXmlPath)) {
                        new TaggedPdfReaderTool(docOut).SetRootTag("root").ConvertToXml(xmlOut);
                    }
                }
            }
            using (PdfReader readerCmp = iText.Kernel.Utils.CompareTool.CreateOutputReader(cmpPdf)) {
                using (PdfDocument docCmp = new PdfDocument(readerCmp, new DocumentProperties().SetEventCountingMetaInfo(metaInfo
                    ))) {
                    using (Stream xmlCmp = FileUtil.GetFileOutputStream(cmpXmlPath)) {
                        new TaggedPdfReaderTool(docCmp).SetRootTag("root").ConvertToXml(xmlCmp);
                    }
                }
            }
            if (!CompareXmls(outXmlPath, cmpXmlPath)) {
                message = "The tag structures are different.";
            }
            if (message == null) {
                System.Console.Out.WriteLine("OK");
            }
            else {
                iText.Kernel.Utils.CompareTool.WriteOnDisk(outPdf);
                iText.Kernel.Utils.CompareTool.WriteOnDiskIfNotExists(cmpPdf);
                System.Console.Out.WriteLine("Fail");
            }
            System.Console.Out.Flush();
            return message;
        }

        /// <summary>Converts document info into a string array.</summary>
        /// <remarks>
        /// Converts document info into a string array.
        /// <para />
        /// Converts document info into a string array. It can be used to compare PdfDocumentInfo later on.
        /// Default implementation retrieves title, author, subject, keywords and producer.
        /// </remarks>
        /// <param name="info">an instance of PdfDocumentInfo to be converted.</param>
        /// <returns>String array with all the document info tester is interested in.</returns>
        protected internal virtual String[] ConvertDocInfoToStrings(PdfDocumentInfo info) {
            String[] convertedInfo = new String[] { "", "", "", "", "" };
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
            infoValue = info.GetProducer();
            if (infoValue != null) {
                convertedInfo[4] = ConvertProducerLine(infoValue);
            }
            return convertedInfo;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual String ConvertProducerLine(String producer) {
            return iText.Commons.Utils.StringUtil.ReplaceAll(iText.Commons.Utils.StringUtil.ReplaceAll(producer, VERSION_REGEXP
                , VERSION_REPLACEMENT), COPYRIGHT_REGEXP, COPYRIGHT_REPLACEMENT);
        }
//\endcond

        private void Init(String outPdf, String cmpPdf) {
            this.outPdf = outPdf;
            this.cmpPdf = cmpPdf;
            outPdfName = new FileInfo(outPdf).Name;
            cmpPdfName = new FileInfo(cmpPdf).Name;
            outImage = outPdfName;
            if (cmpPdfName.StartsWith("cmp_")) {
                cmpImage = cmpPdfName;
            }
            else {
                cmpImage = "cmp_" + cmpPdfName;
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

        private String CompareVisually(String outPath, String differenceImagePrefix, IDictionary<int, IList<Rectangle
            >> ignoredAreas) {
            return CompareVisually(outPath, differenceImagePrefix, ignoredAreas, null);
        }

        private String CompareVisually(String outPath, String differenceImagePrefix, IDictionary<int, IList<Rectangle
            >> ignoredAreas, IList<int> equalPages) {
            if (!outPath.EndsWith("/")) {
                outPath = outPath + "/";
            }
            if (differenceImagePrefix == null) {
                String fileBasedPrefix = "";
                if (outPdfName != null) {
                    // should always be initialized by this moment
                    fileBasedPrefix = outPdfName + "_";
                }
                differenceImagePrefix = "diff_" + fileBasedPrefix;
            }
            PrepareOutputDirs(outPath, differenceImagePrefix);
            System.Console.Out.WriteLine("Comparing visually..........");
            if (ignoredAreas != null && !ignoredAreas.IsEmpty()) {
                CreateIgnoredAreasPdfs(outPath, ignoredAreas);
            }
            GhostscriptHelper ghostscriptHelper = null;
            try {
                ghostscriptHelper = new GhostscriptHelper(gsExec);
            }
            catch (ArgumentException e) {
                throw new CompareTool.CompareToolExecutionException(e.Message);
            }
            ghostscriptHelper.RunGhostScriptImageGeneration(outPdf, outPath, outImage);
            ghostscriptHelper.RunGhostScriptImageGeneration(cmpPdf, outPath, cmpImage);
            return CompareImagesOfPdfs(outPath, differenceImagePrefix, equalPages);
        }

        private String CompareImagesOfPdfs(String outPath, String differenceImagePrefix, IList<int> equalPages) {
            FileInfo[] imageFiles = FileUtil.ListFilesInDirectoryByFilter(outPath, new CompareTool.PngFileFilter(outPdfName
                ));
            FileInfo[] cmpImageFiles = FileUtil.ListFilesInDirectoryByFilter(outPath, new CompareTool.CmpPngFileFilter
                (cmpPdfName));
            bool bUnexpectedNumberOfPages = false;
            if (imageFiles.Length != cmpImageFiles.Length) {
                bUnexpectedNumberOfPages = true;
            }
            int cnt = Math.Min(imageFiles.Length, cmpImageFiles.Length);
            if (cnt < 1) {
                throw new CompareTool.CompareToolExecutionException("No files for comparing. The result or sample pdf file is not processed by GhostScript."
                    );
            }
            JavaUtil.Sort(imageFiles, new CompareTool.ImageNameComparator());
            JavaUtil.Sort(cmpImageFiles, new CompareTool.ImageNameComparator());
            bool compareExecIsOk;
            String imageMagickInitError = null;
            ImageMagickHelper imageMagickHelper = null;
            try {
                imageMagickHelper = new ImageMagickHelper(compareExec);
                compareExecIsOk = true;
            }
            catch (ArgumentException e) {
                compareExecIsOk = false;
                imageMagickInitError = e.Message;
                ITextLogManager.GetLogger(typeof(iText.Kernel.Utils.CompareTool)).LogWarning(e.Message);
            }
            IList<int> diffPages = new List<int>();
            String differentPagesFail = null;
            for (int i = 0; i < cnt; i++) {
                if (equalPages != null && equalPages.Contains(i)) {
                    continue;
                }
                System.Console.Out.WriteLine("Comparing page " + JavaUtil.IntegerToString(i + 1) + ": " + UrlUtil.GetNormalizedFileUriString
                    (imageFiles[i].Name) + " ...");
                System.Console.Out.WriteLine("Comparing page " + JavaUtil.IntegerToString(i + 1) + ": " + UrlUtil.GetNormalizedFileUriString
                    (imageFiles[i].Name) + " ...");
                Stream is1 = FileUtil.GetInputStreamForFile(imageFiles[i].FullName);
                Stream is2 = FileUtil.GetInputStreamForFile(cmpImageFiles[i].FullName);
                bool cmpResult = CompareStreams(is1, is2);
                is1.Dispose();
                is2.Dispose();
                if (!cmpResult) {
                    differentPagesFail = "Page is different!";
                    diffPages.Add(i + 1);
                    if (compareExecIsOk) {
                        String diffName = outPath + differenceImagePrefix + JavaUtil.IntegerToString(i + 1) + ".png";
                        if (!imageMagickHelper.RunImageMagickImageCompare(imageFiles[i].FullName, cmpImageFiles[i].FullName, diffName
                            )) {
                            FileInfo diffFile = new FileInfo(diffName);
                            differentPagesFail += "\nPlease, examine " + FILE_PROTOCOL + UrlUtil.ToNormalizedURI(diffFile).AbsolutePath
                                 + " for more details.";
                        }
                    }
                    System.Console.Out.WriteLine(differentPagesFail);
                }
                else {
                    System.Console.Out.WriteLine(" done.");
                }
            }
            if (differentPagesFail != null) {
                String errorMessage = DIFFERENT_PAGES.Replace("<filename>", UrlUtil.ToNormalizedURI(outPdf).AbsolutePath).
                    Replace("<pagenumber>", ListDiffPagesAsString(diffPages));
                if (!compareExecIsOk) {
                    errorMessage += "\n" + imageMagickInitError;
                }
                return errorMessage;
            }
            else {
                if (bUnexpectedNumberOfPages) {
                    return UNEXPECTED_NUMBER_OF_PAGES.Replace("<filename>", outPdf);
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
            return sb.ToString();
        }

        private void CreateIgnoredAreasPdfs(String outPath, IDictionary<int, IList<Rectangle>> ignoredAreas) {
            StampingProperties properties = new StampingProperties();
            properties.SetEventCountingMetaInfo(metaInfo);
            using (PdfWriter outWriter = new PdfWriter(outPath + IGNORED_AREAS_PREFIX + outPdfName)) {
                using (PdfReader readerOut = iText.Kernel.Utils.CompareTool.CreateOutputReader(outPdf)) {
                    using (PdfDocument pdfOutDoc = new PdfDocument(readerOut, outWriter, properties)) {
                        using (PdfWriter cmpWriter = new PdfWriter(outPath + IGNORED_AREAS_PREFIX + cmpPdfName)) {
                            using (PdfReader readerCmp = iText.Kernel.Utils.CompareTool.CreateOutputReader(cmpPdf)) {
                                using (PdfDocument pdfCmpDoc = new PdfDocument(readerCmp, cmpWriter, properties)) {
                                    foreach (KeyValuePair<int, IList<Rectangle>> entry in ignoredAreas) {
                                        int pageNumber = entry.Key;
                                        IList<Rectangle> rectangles = entry.Value;
                                        if (rectangles != null && !rectangles.IsEmpty()) {
                                            PdfCanvas outCanvas = new PdfCanvas(pdfOutDoc.GetPage(pageNumber));
                                            PdfCanvas cmpCanvas = new PdfCanvas(pdfCmpDoc.GetPage(pageNumber));
                                            outCanvas.SaveState();
                                            cmpCanvas.SaveState();
                                            foreach (Rectangle rect in rectangles) {
                                                outCanvas.Rectangle(rect).Fill();
                                                cmpCanvas.Rectangle(rect).Fill();
                                            }
                                            outCanvas.RestoreState();
                                            cmpCanvas.RestoreState();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Init(outPath + IGNORED_AREAS_PREFIX + outPdfName, outPath + IGNORED_AREAS_PREFIX + cmpPdfName);
        }

        private void PrepareOutputDirs(String outPath, String differenceImagePrefix) {
            FileInfo[] imageFiles;
            FileInfo[] cmpImageFiles;
            FileInfo[] diffFiles;
            if (!FileUtil.DirectoryExists(outPath)) {
                FileUtil.CreateDirectories(outPath);
            }
            else {
                imageFiles = FileUtil.ListFilesInDirectoryByFilter(outPath, new CompareTool.PngFileFilter(cmpPdfName));
                foreach (FileInfo file in imageFiles) {
                    file.Delete();
                }
                cmpImageFiles = FileUtil.ListFilesInDirectoryByFilter(outPath, new CompareTool.CmpPngFileFilter(cmpPdfName
                    ));
                foreach (FileInfo file in cmpImageFiles) {
                    file.Delete();
                }
                diffFiles = FileUtil.ListFilesInDirectoryByFilter(outPath, new CompareTool.DiffPngFileFilter(differenceImagePrefix
                    ));
                foreach (FileInfo file in diffFiles) {
                    file.Delete();
                }
            }
        }

        private void PrintOutCmpDirectories() {
            System.Console.Out.WriteLine("Out file folder: " + FILE_PROTOCOL + UrlUtil.ToNormalizedURI(new FileInfo(outPdf
                ).DirectoryName).AbsolutePath);
            System.Console.Out.WriteLine("Cmp file folder: " + FILE_PROTOCOL + UrlUtil.ToNormalizedURI(new FileInfo(cmpPdf
                ).DirectoryName).AbsolutePath);
        }

        private String CompareByContent(String outPath, String differenceImagePrefix, IDictionary<int, IList<Rectangle
            >> ignoredAreas) {
            PrintOutCmpDirectories();
            System.Console.Out.Write("Comparing by content..........");
            using (PdfReader readerOut = iText.Kernel.Utils.CompareTool.CreateOutputReader(outPdf, GetOutReaderProperties
                ())) {
                using (PdfDocument outDocument = new PdfDocument(readerOut, new DocumentProperties().SetEventCountingMetaInfo
                    (metaInfo))) {
                    using (PdfReader readerCmp = iText.Kernel.Utils.CompareTool.CreateOutputReader(cmpPdf, GetCmpReaderProperties
                        ())) {
                        using (PdfDocument cmpDocument = new PdfDocument(readerCmp, new DocumentProperties().SetEventCountingMetaInfo
                            (metaInfo))) {
                            IList<PdfDictionary> outPages = new List<PdfDictionary>();
                            outPagesRef = new List<PdfIndirectReference>();
                            LoadPagesFromReader(outDocument, outPages, outPagesRef);
                            IList<PdfDictionary> cmpPages = new List<PdfDictionary>();
                            cmpPagesRef = new List<PdfIndirectReference>();
                            LoadPagesFromReader(cmpDocument, cmpPages, cmpPagesRef);
                            if (outPages.Count != cmpPages.Count) {
                                iText.Kernel.Utils.CompareTool.WriteOnDisk(outPdf);
                                iText.Kernel.Utils.CompareTool.WriteOnDiskIfNotExists(cmpPdf);
                                return CompareVisuallyAndCombineReports("Documents have different numbers of pages.", outPath, differenceImagePrefix
                                    , ignoredAreas, null);
                            }
                            CompareTool.CompareResult compareResult = new CompareTool.CompareResult(compareByContentErrorsLimit);
                            IList<int> equalPages = new List<int>(cmpPages.Count);
                            for (int i = 0; i < cmpPages.Count; i++) {
                                ObjectPath currentPath = new ObjectPath(cmpPagesRef[i], outPagesRef[i]);
                                if (CompareDictionariesExtended(outPages[i], cmpPages[i], currentPath, compareResult)) {
                                    equalPages.Add(i);
                                }
                            }
                            ObjectPath catalogPath = new ObjectPath(cmpDocument.GetCatalog().GetPdfObject().GetIndirectReference(), outDocument
                                .GetCatalog().GetPdfObject().GetIndirectReference());
                            ICollection<PdfName> ignoredCatalogEntries = new LinkedHashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Pages
                                , PdfName.Metadata));
                            CompareDictionariesExtended(outDocument.GetCatalog().GetPdfObject(), cmpDocument.GetCatalog().GetPdfObject
                                (), catalogPath, compareResult, ignoredCatalogEntries);
                            if (encryptionCompareEnabled) {
                                CompareDocumentsEncryption(outDocument, cmpDocument, compareResult);
                                CompareDocumentsMac(outDocument, cmpDocument, compareResult);
                            }
                            if (generateCompareByContentXmlReport) {
                                String outPdfName = new FileInfo(outPdf).Name;
                                Stream xml = FileUtil.GetFileOutputStream(outPath + "/" + outPdfName.JSubstring(0, outPdfName.Length - 3) 
                                    + "report.xml");
                                try {
                                    compareResult.WriteReportToXml(xml);
                                }
                                catch (Exception e) {
                                    throw new Exception(e.Message, e);
                                }
                                finally {
                                    xml.Dispose();
                                }
                            }
                            if (equalPages.Count == cmpPages.Count && compareResult.IsOk()) {
                                System.Console.Out.WriteLine("OK");
                                System.Console.Out.Flush();
                                return null;
                            }
                            else {
                                iText.Kernel.Utils.CompareTool.WriteOnDisk(outPdf);
                                iText.Kernel.Utils.CompareTool.WriteOnDiskIfNotExists(cmpPdf);
                                return CompareVisuallyAndCombineReports(compareResult.GetReport(), outPath, differenceImagePrefix, ignoredAreas
                                    , equalPages);
                            }
                        }
                    }
                }
            }
        }

        private static void WriteOnDisk(String filename) {
            MemoryFirstPdfWriter outWriter = MemoryFirstPdfWriter.Get(filename);
            if (outWriter != null) {
                outWriter.Dump();
            }
        }

        private static void WriteOnDiskIfNotExists(String filename) {
            if (!new FileInfo(filename).Exists) {
                iText.Kernel.Utils.CompareTool.WriteOnDisk(filename);
            }
        }

        private String CompareVisuallyAndCombineReports(String compareByFailContentReason, String outPath, String 
            differenceImagePrefix, IDictionary<int, IList<Rectangle>> ignoredAreas, IList<int> equalPages) {
            System.Console.Out.WriteLine("Fail");
            System.Console.Out.Flush();
            String compareByContentReport = "Compare by content report:\n" + compareByFailContentReason;
            System.Console.Out.WriteLine(compareByContentReport);
            System.Console.Out.Flush();
            String message = CompareVisually(outPath, differenceImagePrefix, ignoredAreas, equalPages);
            if (message == null || message.Length == 0) {
                return "Compare by content fails. No visual differences";
            }
            return message;
        }

        private void LoadPagesFromReader(PdfDocument doc, IList<PdfDictionary> pages, IList<PdfIndirectReference> 
            pagesRef) {
            int numOfPages = doc.GetNumberOfPages();
            for (int i = 0; i < numOfPages; ++i) {
                pages.Add(doc.GetPage(i + 1).GetPdfObject());
                pagesRef.Add(pages[i].GetIndirectReference());
            }
        }

        private void CompareDocumentsEncryption(PdfDocument outDocument, PdfDocument cmpDocument, CompareTool.CompareResult
             compareResult) {
            PdfDictionary outEncrypt = outDocument.GetTrailer().GetAsDictionary(PdfName.Encrypt);
            PdfDictionary cmpEncrypt = cmpDocument.GetTrailer().GetAsDictionary(PdfName.Encrypt);
            if (outEncrypt == null && cmpEncrypt == null) {
                return;
            }
            TrailerPath trailerPath = new TrailerPath(cmpDocument, outDocument);
            if (outEncrypt == null) {
                compareResult.AddError(trailerPath, "Expected encrypted document.");
                return;
            }
            if (cmpEncrypt == null) {
                compareResult.AddError(trailerPath, "Expected not encrypted document.");
                return;
            }
            ICollection<PdfName> ignoredEncryptEntries = new LinkedHashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.O, PdfName
                .U, PdfName.OE, PdfName.UE, PdfName.Perms, PdfName.CF, PdfName.Recipients));
            ObjectPath objectPath = new ObjectPath(outEncrypt.GetIndirectReference(), cmpEncrypt.GetIndirectReference(
                ));
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
                        LinkedHashSet<PdfName> excludedKeys = new LinkedHashSet<PdfName>(JavaUtil.ArraysAsList(PdfName.Recipients)
                            );
                        CompareDictionariesExtended(outCfDict.GetAsDictionary(key), cmpCfDict.GetAsDictionary(key), objectPath, compareResult
                            , excludedKeys);
                        objectPath.Pop();
                    }
                }
            }
        }

        private void CompareDocumentsMac(PdfDocument outDocument, PdfDocument cmpDocument, CompareTool.CompareResult
             compareResult) {
            PdfDictionary outAuthCode = outDocument.GetTrailer().GetAsDictionary(PdfName.AuthCode);
            PdfDictionary cmpAuthCode = cmpDocument.GetTrailer().GetAsDictionary(PdfName.AuthCode);
            if (outAuthCode == null && cmpAuthCode == null) {
                return;
            }
            ObjectPath trailerPath = new TrailerPath(cmpDocument, outDocument);
            if (outAuthCode == null) {
                compareResult.AddError(trailerPath, "Output document does not contain MAC.");
                return;
            }
            if (cmpAuthCode == null) {
                compareResult.AddError(trailerPath, "Output document contains MAC which is not expected.");
                return;
            }
            CompareDictionariesExtended(outAuthCode, cmpAuthCode, trailerPath, compareResult, new HashSet<PdfName>(JavaUtil.ArraysAsList
                (PdfName.ByteRange, PdfName.MAC)));
        }

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
                if (!JavaUtil.ArraysEquals(buffer1, buffer2)) {
                    return false;
                }
                if (len1 == -1) {
                    break;
                }
            }
            return true;
        }

        private bool CompareDictionariesExtended(PdfDictionary outDict, PdfDictionary cmpDict, ObjectPath currentPath
            , CompareTool.CompareResult compareResult) {
            return CompareDictionariesExtended(outDict, cmpDict, currentPath, compareResult, null);
        }

        private bool CompareDictionariesExtended(PdfDictionary outDict, PdfDictionary cmpDict, ObjectPath currentPath
            , CompareTool.CompareResult compareResult, ICollection<PdfName> excludedKeys) {
            if (cmpDict != null && outDict == null || outDict != null && cmpDict == null) {
                compareResult.AddError(currentPath, "One of the dictionaries is null, the other is not.");
                return false;
            }
            bool dictsAreSame = true;
            // Iterate through the union of the keys of the cmp and out dictionaries
            ICollection<PdfName> mergedKeys = new SortedSet<PdfName>(cmpDict.KeySet());
            mergedKeys.AddAll(outDict.KeySet());
            foreach (PdfName key in mergedKeys) {
                if (!dictsAreSame && (currentPath == null || compareResult == null || compareResult.IsMessageLimitReached(
                    ))) {
                    return false;
                }
                if (excludedKeys != null && excludedKeys.Contains(key)) {
                    continue;
                }
                if (key.Equals(PdfName.Parent) || key.Equals(PdfName.P) || key.Equals(PdfName.ModDate) || (key.Equals(PdfName
                    .KDFSalt) && !kdfSaltCompareEnabled)) {
                    continue;
                }
                if (outDict.IsStream() && cmpDict.IsStream() && (key.Equals(PdfName.Filter) || key.Equals(PdfName.Length))
                    ) {
                    continue;
                }
                if (key.Equals(PdfName.BaseFont) || key.Equals(PdfName.FontName)) {
                    PdfObject cmpObj = cmpDict.Get(key);
                    if (cmpObj != null && cmpObj.IsName() && cmpObj.ToString().IndexOf('+') > 0) {
                        PdfObject outObj = outDict.Get(key);
                        if (!outObj.IsName() || outObj.ToString().IndexOf('+') == -1) {
                            if (compareResult != null && currentPath != null) {
                                compareResult.AddError(currentPath, MessageFormatUtil.Format("PdfDictionary {0} entry: Expected: {1}. Found: {2}"
                                    , key.ToString(), cmpObj.ToString(), outObj.ToString()));
                            }
                            dictsAreSame = false;
                        }
                        else {
                            String cmpName = cmpObj.ToString().Substring(cmpObj.ToString().IndexOf('+'));
                            String outName = outObj.ToString().Substring(outObj.ToString().IndexOf('+'));
                            if (!cmpName.Equals(outName)) {
                                if (compareResult != null && currentPath != null) {
                                    compareResult.AddError(currentPath, MessageFormatUtil.Format("PdfDictionary {0} entry: Expected: {1}. Found: {2}"
                                        , key.ToString(), cmpObj.ToString(), outObj.ToString()));
                                }
                                dictsAreSame = false;
                            }
                        }
                        continue;
                    }
                }
                // A number tree can be stored in multiple, semantically equivalent ways.
                // Flatten to a single array, in order to get a canonical representation.
                if (key.Equals(PdfName.ParentTree) || key.Equals(PdfName.PageLabels)) {
                    if (currentPath != null) {
                        currentPath.PushDictItemToPath(key);
                    }
                    PdfDictionary outNumTree = outDict.GetAsDictionary(key);
                    PdfDictionary cmpNumTree = cmpDict.GetAsDictionary(key);
                    LinkedList<PdfObject> outItems = new LinkedList<PdfObject>();
                    LinkedList<PdfObject> cmpItems = new LinkedList<PdfObject>();
                    PdfNumber outLeftover = FlattenNumTree(outNumTree, null, outItems);
                    PdfNumber cmpLeftover = FlattenNumTree(cmpNumTree, null, cmpItems);
                    if (outLeftover != null) {
                        ITextLogManager.GetLogger(typeof(iText.Kernel.Utils.CompareTool)).LogWarning(iText.IO.Logs.IoLogMessageConstant
                            .NUM_TREE_SHALL_NOT_END_WITH_KEY);
                        if (cmpLeftover == null) {
                            if (compareResult != null && currentPath != null) {
                                compareResult.AddError(currentPath, "Number tree unexpectedly ends with a key");
                            }
                            dictsAreSame = false;
                        }
                    }
                    if (cmpLeftover != null) {
                        ITextLogManager.GetLogger(typeof(iText.Kernel.Utils.CompareTool)).LogWarning(iText.IO.Logs.IoLogMessageConstant
                            .NUM_TREE_SHALL_NOT_END_WITH_KEY);
                        if (outLeftover == null) {
                            if (compareResult != null && currentPath != null) {
                                compareResult.AddError(currentPath, "Number tree was expected to end with a key (although it is invalid according to the specification), but ended with a value"
                                    );
                            }
                            dictsAreSame = false;
                        }
                    }
                    if (outLeftover != null && cmpLeftover != null && !CompareNumbers(outLeftover, cmpLeftover)) {
                        if (compareResult != null && currentPath != null) {
                            compareResult.AddError(currentPath, "Number tree was expected to end with a different key (although it is invalid according to the specification)"
                                );
                        }
                        dictsAreSame = false;
                    }
                    PdfArray outArray = new PdfArray(outItems, outItems.Count);
                    PdfArray cmpArray = new PdfArray(cmpItems, cmpItems.Count);
                    if (!CompareArraysExtended(outArray, cmpArray, currentPath, compareResult)) {
                        if (compareResult != null && currentPath != null) {
                            compareResult.AddError(currentPath, "Number trees were flattened, compared and found to be different.");
                        }
                        dictsAreSame = false;
                    }
                    if (currentPath != null) {
                        currentPath.Pop();
                    }
                    continue;
                }
                if (currentPath != null) {
                    currentPath.PushDictItemToPath(key);
                }
                dictsAreSame = CompareObjects(outDict.Get(key, false), cmpDict.Get(key, false), currentPath, compareResult
                    ) && dictsAreSame;
                if (currentPath != null) {
                    currentPath.Pop();
                }
            }
            return dictsAreSame;
        }

        private PdfNumber FlattenNumTree(PdfDictionary dictionary, PdfNumber leftOver, LinkedList<PdfObject> items
            ) {
            /*Map<PdfNumber, PdfObject> items*/
            PdfArray nums = dictionary.GetAsArray(PdfName.Nums);
            if (nums != null) {
                for (int k = 0; k < nums.Size(); k++) {
                    PdfNumber number;
                    if (leftOver == null) {
                        number = nums.GetAsNumber(k++);
                    }
                    else {
                        number = leftOver;
                        leftOver = null;
                    }
                    if (k < nums.Size()) {
                        items.AddLast(number);
                        items.AddLast(nums.Get(k, false));
                    }
                    else {
                        return number;
                    }
                }
            }
            else {
                if ((nums = dictionary.GetAsArray(PdfName.Kids)) != null) {
                    for (int k = 0; k < nums.Size(); k++) {
                        PdfDictionary kid = nums.GetAsDictionary(k);
                        leftOver = FlattenNumTree(kid, leftOver, items);
                    }
                }
            }
            return null;
        }

        /// <summary>Compare PDF objects.</summary>
        /// <param name="outObj">out object corresponding to the output file, which is to be compared with cmp object</param>
        /// <param name="cmpObj">cmp object corresponding to the cmp-file, which is to be compared with out object</param>
        /// <param name="currentPath">
        /// current objects
        /// <see cref="iText.Kernel.Utils.Objectpathitems.ObjectPath"/>
        /// path
        /// </param>
        /// <param name="compareResult">
        /// 
        /// <see cref="CompareResult"/>
        /// for the results of the comparison of the two documents
        /// </param>
        /// <returns>true if objects are equal, false otherwise.</returns>
        protected internal virtual bool CompareObjects(PdfObject outObj, PdfObject cmpObj, ObjectPath currentPath, 
            CompareTool.CompareResult compareResult) {
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
                        compareResult.AddError(currentPath, MessageFormatUtil.Format("Types do not match. Expected: {0}. Found: {1}."
                            , cmpDirectObj.GetType().Name, outDirectObj.GetType().Name));
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
                    for (int i = 1; i <= cmpRefKey.GetDocument().GetNumberOfPages(); ++i) {
                        cmpPagesRef.Add(cmpRefKey.GetDocument().GetPage(i).GetPdfObject().GetIndirectReference());
                    }
                }
                if (outPagesRef == null) {
                    outPagesRef = new List<PdfIndirectReference>();
                    for (int i = 1; i <= outRefKey.GetDocument().GetNumberOfPages(); ++i) {
                        outPagesRef.Add(outRefKey.GetDocument().GetPage(i).GetPdfObject().GetIndirectReference());
                    }
                }
                // If at least one of the page dictionaries is in the document's page tree, we don't proceed with deep comparison,
                // because pages are compared at different level, so we compare only their index.
                // However only if both page dictionaries are not in the document's page trees, we continue to comparing them as normal dictionaries.
                if (cmpPagesRef.Contains(cmpRefKey) || outPagesRef.Contains(outRefKey)) {
                    if (cmpPagesRef.Contains(cmpRefKey) && cmpPagesRef.IndexOf(cmpRefKey) == outPagesRef.IndexOf(outRefKey)) {
                        return true;
                    }
                    if (compareResult != null && currentPath != null) {
                        compareResult.AddError(currentPath, MessageFormatUtil.Format("The dictionaries refer to different pages. Expected page number: {0}. Found: {1}"
                            , cmpPagesRef.IndexOf(cmpRefKey) + 1, outPagesRef.IndexOf(outRefKey) + 1));
                    }
                    return false;
                }
            }
            if (cmpDirectObj.IsDictionary()) {
                return CompareDictionariesExtended((PdfDictionary)outDirectObj, (PdfDictionary)cmpDirectObj, currentPath, 
                    compareResult);
            }
            else {
                if (cmpDirectObj.IsStream()) {
                    return CompareStreamsExtended((PdfStream)outDirectObj, (PdfStream)cmpDirectObj, currentPath, compareResult
                        );
                }
                else {
                    if (cmpDirectObj.IsArray()) {
                        return CompareArraysExtended((PdfArray)outDirectObj, (PdfArray)cmpDirectObj, currentPath, compareResult);
                    }
                    else {
                        if (cmpDirectObj.IsName()) {
                            return CompareNamesExtended((PdfName)outDirectObj, (PdfName)cmpDirectObj, currentPath, compareResult);
                        }
                        else {
                            if (cmpDirectObj.IsNumber()) {
                                return CompareNumbersExtended((PdfNumber)outDirectObj, (PdfNumber)cmpDirectObj, currentPath, compareResult
                                    );
                            }
                            else {
                                if (cmpDirectObj.IsString()) {
                                    return CompareStringsExtended((PdfString)outDirectObj, (PdfString)cmpDirectObj, currentPath, compareResult
                                        );
                                }
                                else {
                                    if (cmpDirectObj.IsBoolean()) {
                                        return CompareBooleansExtended((PdfBoolean)outDirectObj, (PdfBoolean)cmpDirectObj, currentPath, compareResult
                                            );
                                    }
                                    else {
                                        if (outDirectObj.IsNull() && cmpDirectObj.IsNull()) {
                                            return true;
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
        }

        private bool CompareStreamsExtended(PdfStream outStream, PdfStream cmpStream, ObjectPath currentPath, CompareTool.CompareResult
             compareResult) {
            bool toDecode = PdfName.FlateDecode.Equals(outStream.Get(PdfName.Filter));
            byte[] outStreamBytes = outStream.GetBytes(toDecode);
            byte[] cmpStreamBytes = cmpStream.GetBytes(toDecode);
            if (JavaUtil.ArraysEquals(outStreamBytes, cmpStreamBytes)) {
                return CompareDictionariesExtended(outStream, cmpStream, currentPath, compareResult);
            }
            else {
                StringBuilder errorMessage = new StringBuilder();
                if (cmpStreamBytes.Length != outStreamBytes.Length) {
                    errorMessage.Append(MessageFormatUtil.Format("PdfStream. Lengths are different. Expected: {0}. Found: {1}\n"
                        , cmpStreamBytes.Length, outStreamBytes.Length));
                }
                else {
                    errorMessage.Append("PdfStream. Bytes are different.\n");
                }
                int firstDifferenceOffset = FindBytesDifference(outStreamBytes, cmpStreamBytes, errorMessage);
                if (compareResult != null && currentPath != null) {
                    currentPath.PushOffsetToPath(firstDifferenceOffset);
                    compareResult.AddError(currentPath, errorMessage.ToString());
                    currentPath.Pop();
                }
                return false;
            }
        }

        /// <returns>first difference offset</returns>
        private int FindBytesDifference(byte[] outStreamBytes, byte[] cmpStreamBytes, StringBuilder errorMessage) {
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
            String bytesDifference = null;
            if (numberOfDifferentBytes > 0) {
                int diffBytesAreaL = 10;
                int diffBytesAreaR = 10;
                int lCmp = Math.Max(0, firstDifferenceOffset - diffBytesAreaL);
                int rCmp = Math.Min(cmpStreamBytes.Length, firstDifferenceOffset + diffBytesAreaR);
                int lOut = Math.Max(0, firstDifferenceOffset - diffBytesAreaL);
                int rOut = Math.Min(outStreamBytes.Length, firstDifferenceOffset + diffBytesAreaR);
                String cmpByte = iText.Commons.Utils.JavaUtil.GetStringForBytes(new byte[] { cmpStreamBytes[firstDifferenceOffset
                    ] }, iText.Commons.Utils.EncodingUtil.ISO_8859_1);
                String cmpByteNeighbours = iText.Commons.Utils.StringUtil.ReplaceAll(iText.Commons.Utils.JavaUtil.GetStringForBytes
                    (cmpStreamBytes, lCmp, rCmp - lCmp, iText.Commons.Utils.EncodingUtil.ISO_8859_1), NEW_LINES, " ");
                String outByte = iText.Commons.Utils.JavaUtil.GetStringForBytes(new byte[] { outStreamBytes[firstDifferenceOffset
                    ] }, iText.Commons.Utils.EncodingUtil.ISO_8859_1);
                String outBytesNeighbours = iText.Commons.Utils.StringUtil.ReplaceAll(iText.Commons.Utils.JavaUtil.GetStringForBytes
                    (outStreamBytes, lOut, rOut - lOut, iText.Commons.Utils.EncodingUtil.ISO_8859_1), NEW_LINES, " ");
                bytesDifference = MessageFormatUtil.Format("First bytes difference is encountered at index {0}. Expected: {1} ({2}). Found: {3} ({4}). Total number of different bytes: {5}"
                    , JavaUtil.IntegerToString(Convert.ToInt32(firstDifferenceOffset)), cmpByte, cmpByteNeighbours, outByte
                    , outBytesNeighbours, numberOfDifferentBytes);
            }
            else {
                // lengths are different
                firstDifferenceOffset = minLength;
                bytesDifference = MessageFormatUtil.Format("Bytes of the shorter array are the same as the first {0} bytes of the longer one."
                    , minLength);
            }
            errorMessage.Append(bytesDifference);
            return firstDifferenceOffset;
        }

        private bool CompareArraysExtended(PdfArray outArray, PdfArray cmpArray, ObjectPath currentPath, CompareTool.CompareResult
             compareResult) {
            if (outArray == null) {
                if (compareResult != null && currentPath != null) {
                    compareResult.AddError(currentPath, "Found null. Expected PdfArray.");
                }
                return false;
            }
            else {
                if (outArray.Size() != cmpArray.Size()) {
                    if (compareResult != null && currentPath != null) {
                        compareResult.AddError(currentPath, MessageFormatUtil.Format("PdfArrays. Lengths are different. Expected: {0}. Found: {1}."
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

        private bool CompareNamesExtended(PdfName outName, PdfName cmpName, ObjectPath currentPath, CompareTool.CompareResult
             compareResult) {
            if (cmpName.Equals(outName)) {
                return true;
            }
            else {
                if (compareResult != null && currentPath != null) {
                    compareResult.AddError(currentPath, MessageFormatUtil.Format("PdfName. Expected: {0}. Found: {1}", cmpName
                        .ToString(), outName.ToString()));
                }
                return false;
            }
        }

        private bool CompareNumbersExtended(PdfNumber outNumber, PdfNumber cmpNumber, ObjectPath currentPath, CompareTool.CompareResult
             compareResult) {
            if (cmpNumber.GetValue() == outNumber.GetValue()) {
                return true;
            }
            else {
                if (compareResult != null && currentPath != null) {
                    compareResult.AddError(currentPath, MessageFormatUtil.Format("PdfNumber. Expected: {0}. Found: {1}", cmpNumber
                        , outNumber));
                }
                return false;
            }
        }

        private bool CompareStringsExtended(PdfString outString, PdfString cmpString, ObjectPath currentPath, CompareTool.CompareResult
             compareResult) {
            if (JavaUtil.ArraysEquals(ConvertPdfStringToBytes(cmpString), ConvertPdfStringToBytes(outString))) {
                return true;
            }
            else {
                String cmpStr = cmpString.ToUnicodeString();
                String outStr = outString.ToUnicodeString();
                StringBuilder errorMessage = new StringBuilder();
                if (cmpStr.Length != outStr.Length) {
                    errorMessage.Append(MessageFormatUtil.Format("PdfString. Lengths are different. Expected: {0}. Found: {1}\n"
                        , cmpStr.Length, outStr.Length));
                }
                else {
                    errorMessage.Append("PdfString. Characters are different.\n");
                }
                int firstDifferenceOffset = FindStringDifference(outStr, cmpStr, errorMessage);
                if (compareResult != null && currentPath != null) {
                    currentPath.PushOffsetToPath(firstDifferenceOffset);
                    compareResult.AddError(currentPath, errorMessage.ToString());
                    currentPath.Pop();
                }
                return false;
            }
        }

        private int FindStringDifference(String outString, String cmpString, StringBuilder errorMessage) {
            int numberOfDifferentChars = 0;
            int firstDifferenceOffset = 0;
            int minLength = Math.Min(cmpString.Length, outString.Length);
            for (int i = 0; i < minLength; i++) {
                if (cmpString[i] != outString[i]) {
                    ++numberOfDifferentChars;
                    if (numberOfDifferentChars == 1) {
                        firstDifferenceOffset = i;
                    }
                }
            }
            String stringDifference = null;
            if (numberOfDifferentChars > 0) {
                int diffBytesAreaL = 15;
                int diffBytesAreaR = 15;
                int lCmp = Math.Max(0, firstDifferenceOffset - diffBytesAreaL);
                int rCmp = Math.Min(cmpString.Length, firstDifferenceOffset + diffBytesAreaR);
                int lOut = Math.Max(0, firstDifferenceOffset - diffBytesAreaL);
                int rOut = Math.Min(outString.Length, firstDifferenceOffset + diffBytesAreaR);
                String cmpByte = cmpString[firstDifferenceOffset].ToString();
                String cmpByteNeighbours = iText.Commons.Utils.StringUtil.ReplaceAll(cmpString.JSubstring(lCmp, rCmp), NEW_LINES
                    , " ");
                String outByte = outString[firstDifferenceOffset].ToString();
                String outBytesNeighbours = iText.Commons.Utils.StringUtil.ReplaceAll(outString.JSubstring(lOut, rOut), NEW_LINES
                    , " ");
                stringDifference = MessageFormatUtil.Format("First characters difference is encountered at index {0}.\nExpected: {1} ({2}).\nFound: {3} ({4}).\nTotal number of different characters: {5}"
                    , JavaUtil.IntegerToString(Convert.ToInt32(firstDifferenceOffset)), cmpByte, cmpByteNeighbours, outByte
                    , outBytesNeighbours, numberOfDifferentChars);
            }
            else {
                // lengths are different
                firstDifferenceOffset = minLength;
                stringDifference = MessageFormatUtil.Format("All characters of the shorter string are the same as the first {0} characters of the longer one."
                    , minLength);
            }
            errorMessage.Append(stringDifference);
            return firstDifferenceOffset;
        }

        private byte[] ConvertPdfStringToBytes(PdfString pdfString) {
            byte[] bytes;
            String value = pdfString.GetValue();
            String encoding = pdfString.GetEncoding();
            if (encoding != null && PdfEncodings.UNICODE_BIG.Equals(encoding) && PdfEncodings.IsPdfDocEncoding(value)) {
                bytes = PdfEncodings.ConvertToBytes(value, PdfEncodings.PDF_DOC_ENCODING);
            }
            else {
                bytes = PdfEncodings.ConvertToBytes(value, encoding);
            }
            return bytes;
        }

        private bool CompareBooleansExtended(PdfBoolean outBoolean, PdfBoolean cmpBoolean, ObjectPath currentPath, 
            CompareTool.CompareResult compareResult) {
            if (cmpBoolean.GetValue() == outBoolean.GetValue()) {
                return true;
            }
            else {
                if (compareResult != null && currentPath != null) {
                    compareResult.AddError(currentPath, MessageFormatUtil.Format("PdfBoolean. Expected: {0}. Found: {1}.", cmpBoolean
                        .GetValue(), outBoolean.GetValue()));
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
                    PdfNameTree cmpNamedDestinations = cmpDocument.GetCatalog().GetNameTree(PdfName.Dests);
                    PdfNameTree outNamedDestinations = outDocument.GetCatalog().GetNameTree(PdfName.Dests);
                    switch (cmpDestObject.GetObjectType()) {
                        case PdfObject.ARRAY: {
                            explicitCmpDest = (PdfArray)cmpDestObject;
                            explicitOutDest = (PdfArray)outDestObject;
                            break;
                        }

                        case PdfObject.NAME: {
                            String cmpDestName = ((PdfName)cmpDestObject).GetValue();
                            explicitCmpDest = (PdfArray)cmpNamedDestinations.GetEntry(cmpDestName);
                            String outDestName = ((PdfName)outDestObject).GetValue();
                            explicitOutDest = (PdfArray)outNamedDestinations.GetEntry(outDestName);
                            break;
                        }

                        case PdfObject.STRING: {
                            explicitCmpDest = (PdfArray)cmpNamedDestinations.GetEntry((PdfString)cmpDestObject);
                            explicitOutDest = (PdfArray)outNamedDestinations.GetEntry((PdfString)outDestObject);
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

        private class PngFileFilter : iText.Commons.Utils.FileUtil.IFileFilter {
            private String currentOutPdfName;

            public PngFileFilter(String currentOutPdfName) {
                this.currentOutPdfName = currentOutPdfName;
            }

            public virtual bool Accept(FileInfo pathname) {
                String ap = pathname.Name;
                bool b1 = ap.EndsWith(".png");
                bool b2 = ap.Contains("cmp_");
                return b1 && !b2 && ap.Contains(currentOutPdfName);
            }
        }

        private class CmpPngFileFilter : iText.Commons.Utils.FileUtil.IFileFilter {
            private String currentCmpPdfName;

            public CmpPngFileFilter(String currentCmpPdfName) {
                this.currentCmpPdfName = currentCmpPdfName;
            }

            public virtual bool Accept(FileInfo pathname) {
                String ap = pathname.Name;
                bool b1 = ap.EndsWith(".png");
                bool b2 = ap.Contains("cmp_");
                return b1 && b2 && ap.Contains(currentCmpPdfName);
            }
        }

        private class DiffPngFileFilter : iText.Commons.Utils.FileUtil.IFileFilter {
            private String differenceImagePrefix;

            public DiffPngFileFilter(String differenceImagePrefix) {
                this.differenceImagePrefix = differenceImagePrefix;
            }

            public virtual bool Accept(FileInfo pathname) {
                String ap = pathname.Name;
                bool b1 = ap.EndsWith(".png");
                bool b2 = ap.StartsWith(differenceImagePrefix);
                return b1 && b2;
            }
        }

        private class ImageNameComparator : IComparer<FileInfo> {
            public virtual int Compare(FileInfo f1, FileInfo f2) {
                String f1Name = f1.Name;
                String f2Name = f2.Name;
                return string.CompareOrdinal(f1Name, f2Name);
            }
        }

        /// <summary>Class containing results of the comparison of two documents.</summary>
        public class CompareResult {
            // LinkedHashMap to retain order. HashMap has different order in Java6/7 and Java8
            protected internal IDictionary<ObjectPath, String> differences = new LinkedDictionary<ObjectPath, String>(
                );

            protected internal int messageLimit = 1;

            /// <summary>Creates new empty instance of CompareResult with given limit of difference messages.</summary>
            /// <param name="messageLimit">maximum number of difference messages to be handled by this CompareResult.</param>
            public CompareResult(int messageLimit) {
                this.messageLimit = messageLimit;
            }

            /// <summary>Verifies if documents are considered equal after comparison.</summary>
            /// <returns>true if documents are equal, false otherwise.</returns>
            public virtual bool IsOk() {
                return differences.Count == 0;
            }

            /// <summary>Returns number of differences between two documents detected during comparison.</summary>
            /// <returns>number of differences.</returns>
            public virtual int GetErrorCount() {
                return differences.Count;
            }

            /// <summary>Converts this CompareResult into text form.</summary>
            /// <returns>text report on the differences between two documents.</returns>
            public virtual String GetReport() {
                StringBuilder sb = new StringBuilder();
                bool firstEntry = true;
                foreach (KeyValuePair<ObjectPath, String> entry in differences) {
                    if (!firstEntry) {
                        sb.Append("-----------------------------").Append("\n");
                    }
                    ObjectPath diffPath = entry.Key;
                    sb.Append(entry.Value).Append("\n").Append(diffPath.ToString()).Append("\n");
                    firstEntry = false;
                }
                return sb.ToString();
            }

            /// <summary>
            /// Returns map with
            /// <see cref="iText.Kernel.Utils.Objectpathitems.ObjectPath"/>
            /// as keys and difference descriptions as values.
            /// </summary>
            /// <returns>differences map which could be used to find in the document the objects that are different.</returns>
            public virtual IDictionary<ObjectPath, String> GetDifferences() {
                return differences;
            }

            /// <summary>Converts this CompareResult into xml form.</summary>
            /// <param name="stream">output stream to which xml report will be written.</param>
            public virtual void WriteReportToXml(Stream stream) {
                XmlDocument xmlReport = XmlUtil.InitNewXmlDocument();
                XmlElement root = xmlReport.CreateElement("report");
                XmlElement errors = xmlReport.CreateElement("errors");
                errors.SetAttribute("count", differences.Count.ToString());
                root.AppendChild(errors);
                foreach (KeyValuePair<ObjectPath, String> entry in differences) {
                    XmlNode errorNode = xmlReport.CreateElement("error");
                    XmlNode message = xmlReport.CreateElement("message");
                    message.AppendChild(xmlReport.CreateTextNode(entry.Value));
                    XmlNode path = entry.Key.ToXmlNode(xmlReport);
                    errorNode.AppendChild(message);
                    errorNode.AppendChild(path);
                    errors.AppendChild(errorNode);
                }
                xmlReport.AppendChild(root);
                XmlUtils.WriteXmlDocToStream(xmlReport, stream);
            }

            /// <summary>Checks whether maximum number of difference messages to be handled by this CompareResult is reached.
            ///     </summary>
            /// <returns>true if limit of difference messages is reached, false otherwise.</returns>
            protected internal virtual bool IsMessageLimitReached() {
                return differences.Count >= messageLimit;
            }

            /// <summary>
            /// Adds an error message for the
            /// <see cref="iText.Kernel.Utils.Objectpathitems.ObjectPath"/>.
            /// </summary>
            /// <param name="path">
            /// 
            /// <see cref="iText.Kernel.Utils.Objectpathitems.ObjectPath"/>
            /// for the two corresponding objects in the compared documents
            /// </param>
            /// <param name="message">an error message</param>
            protected internal virtual void AddError(ObjectPath path, String message) {
                if (differences.Count < messageLimit) {
                    differences.Put(new ObjectPath(path), message);
                }
            }
        }

        /// <summary>
        /// Exceptions thrown when errors occur during generation and comparison of images obtained on the basis of pdf
        /// files.
        /// </summary>
        public class CompareToolExecutionException : Exception {
            /// <summary>
            /// Creates a new
            /// <see cref="CompareToolExecutionException"/>.
            /// </summary>
            /// <param name="msg">the detail message.</param>
            public CompareToolExecutionException(String msg)
                : base(msg) {
            }
        }
    }
}
