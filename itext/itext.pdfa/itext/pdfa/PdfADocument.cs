/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.XMP;
using iText.Pdfa.Checker;
using iText.Pdfa.Exceptions;
using iText.Pdfa.Logs;

namespace iText.Pdfa {
    /// <summary>
    /// This class extends
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// and is in charge of creating files
    /// that comply with the PDF/A standard.
    /// </summary>
    /// <remarks>
    /// This class extends
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// and is in charge of creating files
    /// that comply with the PDF/A standard.
    /// Client code is still responsible for making sure the file is actually PDF/A
    /// compliant: multiple steps must be undertaken (depending on the
    /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
    /// ) to ensure that the PDF/A standard is followed.
    /// This class will throw exceptions, mostly
    /// <see cref="iText.Pdfa.Exceptions.PdfAConformanceException"/>
    /// ,
    /// and thus refuse to output a PDF/A file if at any point the document does not
    /// adhere to the PDF/A guidelines specified by the
    /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>.
    /// </remarks>
    public class PdfADocument : PdfDocument {
        private static IPdfPageFactory pdfAPageFactory = new PdfAPageFactory();

        protected internal PdfAChecker checker;

        private bool alreadyLoggedThatObjectFlushingWasNotPerformed = false;

        private bool alreadyLoggedThatPageFlushingWasNotPerformed = false;

        private bool isPdfADocument = true;

        /// <summary>Constructs a new PdfADocument for writing purposes, i.e. from scratch.</summary>
        /// <remarks>
        /// Constructs a new PdfADocument for writing purposes, i.e. from scratch. A
        /// PDF/A file has a conformance level, and must have an explicit output
        /// intent.
        /// </remarks>
        /// <param name="writer">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object to write to
        /// </param>
        /// <param name="conformanceLevel">the generation and strictness level of the PDF/A that must be followed.</param>
        /// <param name="outputIntent">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// </param>
        public PdfADocument(PdfWriter writer, PdfAConformanceLevel conformanceLevel, PdfOutputIntent outputIntent)
            : this(writer, conformanceLevel, outputIntent, new DocumentProperties()) {
        }

        /// <summary>Constructs a new PdfADocument for writing purposes, i.e. from scratch.</summary>
        /// <remarks>
        /// Constructs a new PdfADocument for writing purposes, i.e. from scratch. A
        /// PDF/A file has a conformance level, and must have an explicit output
        /// intent.
        /// </remarks>
        /// <param name="writer">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object to write to
        /// </param>
        /// <param name="conformanceLevel">the generation and strictness level of the PDF/A that must be followed.</param>
        /// <param name="outputIntent">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// </param>
        /// <param name="properties">
        /// a
        /// <see cref="iText.Kernel.Pdf.DocumentProperties"/>
        /// </param>
        public PdfADocument(PdfWriter writer, PdfAConformanceLevel conformanceLevel, PdfOutputIntent outputIntent, 
            DocumentProperties properties)
            : base(writer, properties) {
            SetChecker(conformanceLevel);
            AddOutputIntent(outputIntent);
        }

        /// <summary>Opens a PDF/A document in the stamping mode.</summary>
        /// <param name="reader">PDF reader.</param>
        /// <param name="writer">PDF writer.</param>
        public PdfADocument(PdfReader reader, PdfWriter writer)
            : this(reader, writer, new StampingProperties()) {
        }

        /// <summary>Opens a PDF/A document in stamping mode.</summary>
        /// <param name="reader">PDF reader.</param>
        /// <param name="writer">PDF writer.</param>
        /// <param name="properties">properties of the stamping process</param>
        public PdfADocument(PdfReader reader, PdfWriter writer, StampingProperties properties)
            : this(reader, writer, properties, false) {
        }

        internal PdfADocument(PdfReader reader, PdfWriter writer, StampingProperties properties, bool tolerant)
            : base(reader, writer, properties) {
            PdfAConformanceLevel conformanceLevel = reader.GetPdfAConformanceLevel();
            if (conformanceLevel == null) {
                if (tolerant) {
                    isPdfADocument = false;
                }
                else {
                    throw new PdfAConformanceException(PdfAConformanceException.DOCUMENT_TO_READ_FROM_SHALL_BE_A_PDFA_CONFORMANT_FILE_WITH_VALID_XMP_METADATA
                        );
                }
            }
            SetChecker(conformanceLevel);
        }

        public override void CheckIsoConformance(Object obj, IsoKey key) {
            CheckIsoConformance(obj, key, null, null);
        }

        public override void CheckIsoConformance(Object obj, IsoKey key, PdfResources resources, PdfStream contentStream
            ) {
            if (!isPdfADocument) {
                base.CheckIsoConformance(obj, key, resources, contentStream);
                return;
            }
            CanvasGraphicsState gState;
            PdfDictionary currentColorSpaces = null;
            if (resources != null) {
                currentColorSpaces = resources.GetPdfObject().GetAsDictionary(PdfName.ColorSpace);
            }
            switch (key) {
                case IsoKey.CANVAS_STACK: {
                    checker.CheckCanvasStack((char)obj);
                    break;
                }

                case IsoKey.PDF_OBJECT: {
                    checker.CheckPdfObject((PdfObject)obj);
                    break;
                }

                case IsoKey.RENDERING_INTENT: {
                    checker.CheckRenderingIntent((PdfName)obj);
                    break;
                }

                case IsoKey.INLINE_IMAGE: {
                    checker.CheckInlineImage((PdfStream)obj, currentColorSpaces);
                    break;
                }

                case IsoKey.EXTENDED_GRAPHICS_STATE: {
                    gState = (CanvasGraphicsState)obj;
                    checker.CheckExtGState(gState, contentStream);
                    break;
                }

                case IsoKey.FILL_COLOR: {
                    gState = (CanvasGraphicsState)obj;
                    checker.CheckColor(gState.GetFillColor(), currentColorSpaces, true, contentStream);
                    break;
                }

                case IsoKey.PAGE: {
                    checker.CheckSinglePage((PdfPage)obj);
                    break;
                }

                case IsoKey.STROKE_COLOR: {
                    gState = (CanvasGraphicsState)obj;
                    checker.CheckColor(gState.GetStrokeColor(), currentColorSpaces, false, contentStream);
                    break;
                }

                case IsoKey.TAG_STRUCTURE_ELEMENT: {
                    checker.CheckTagStructureElement((PdfObject)obj);
                    break;
                }

                case IsoKey.FONT_GLYPHS: {
                    checker.CheckFontGlyphs(((CanvasGraphicsState)obj).GetFont(), contentStream);
                    break;
                }

                case IsoKey.XREF_TABLE: {
                    checker.CheckXrefTable((PdfXrefTable)obj);
                    break;
                }

                case IsoKey.SIGNATURE: {
                    checker.CheckSignature((PdfDictionary)obj);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the PdfAConformanceLevel set in the constructor or in the metadata
        /// of the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// </returns>
        public virtual PdfAConformanceLevel GetConformanceLevel() {
            if (isPdfADocument) {
                return checker.GetConformanceLevel();
            }
            else {
                return null;
            }
        }

        internal virtual void LogThatPdfAPageFlushingWasNotPerformed() {
            if (!alreadyLoggedThatPageFlushingWasNotPerformed) {
                alreadyLoggedThatPageFlushingWasNotPerformed = true;
                // This log message will be printed once for one instance of the document.
                ITextLogManager.GetLogger(typeof(iText.Pdfa.PdfADocument)).LogWarning(PdfALogMessageConstant.PDFA_PAGE_FLUSHING_WAS_NOT_PERFORMED
                    );
            }
        }

        protected override void AddCustomMetadataExtensions(XMPMeta xmpMeta) {
            if (!isPdfADocument) {
                base.AddCustomMetadataExtensions(xmpMeta);
                return;
            }
            if (this.IsTagged()) {
                try {
                    if (xmpMeta.GetPropertyInteger(XMPConst.NS_PDFUA_ID, XMPConst.PART) != null) {
                        XMPMeta taggedExtensionMeta = XMPMetaFactory.ParseFromString(PdfAXMPUtil.PDF_UA_EXTENSION);
                        XMPUtils.AppendProperties(taggedExtensionMeta, xmpMeta, true, false);
                    }
                }
                catch (XMPException exc) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Pdfa.PdfADocument));
                    logger.LogError(exc, iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA);
                }
            }
        }

        protected override void UpdateXmpMetadata() {
            if (!isPdfADocument) {
                base.UpdateXmpMetadata();
                return;
            }
            try {
                XMPMeta xmpMeta = UpdateDefaultXmpMetadata();
                xmpMeta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART, checker.GetConformanceLevel().GetPart());
                xmpMeta.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE, checker.GetConformanceLevel().GetConformance
                    ());
                AddCustomMetadataExtensions(xmpMeta);
                SetXmpMetadata(xmpMeta);
            }
            catch (XMPException e) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Pdfa.PdfADocument));
                logger.LogError(e, iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA);
            }
        }

        protected override void CheckIsoConformance() {
            if (isPdfADocument) {
                checker.CheckDocument(catalog);
            }
            else {
                base.CheckIsoConformance();
            }
        }

        protected override void FlushObject(PdfObject pdfObject, bool canBeInObjStm) {
            if (!isPdfADocument) {
                base.FlushObject(pdfObject, canBeInObjStm);
                return;
            }
            MarkObjectAsMustBeFlushed(pdfObject);
            if (isClosing || checker.ObjectIsChecked(pdfObject)) {
                base.FlushObject(pdfObject, canBeInObjStm);
            }
            else {
                if (!alreadyLoggedThatObjectFlushingWasNotPerformed) {
                    alreadyLoggedThatObjectFlushingWasNotPerformed = true;
                    // This log message will be printed once for one instance of the document.
                    ITextLogManager.GetLogger(typeof(iText.Pdfa.PdfADocument)).LogWarning(PdfALogMessageConstant.PDFA_OBJECT_FLUSHING_WAS_NOT_PERFORMED
                        );
                }
            }
        }

        protected override void FlushFonts() {
            if (isPdfADocument) {
                foreach (PdfFont pdfFont in GetDocumentFonts()) {
                    checker.CheckFont(pdfFont);
                }
            }
            base.FlushFonts();
        }

        /// <summary>
        /// Sets the checker that defines the requirements of the PDF/A standard
        /// depending on conformance level.
        /// </summary>
        /// <param name="conformanceLevel">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfAConformanceLevel"/>
        /// </param>
        protected internal virtual void SetChecker(PdfAConformanceLevel conformanceLevel) {
            if (!isPdfADocument) {
                return;
            }
            switch (conformanceLevel.GetPart()) {
                case "1": {
                    checker = new PdfA1Checker(conformanceLevel);
                    break;
                }

                case "2": {
                    checker = new PdfA2Checker(conformanceLevel);
                    break;
                }

                case "3": {
                    checker = new PdfA3Checker(conformanceLevel);
                    break;
                }
            }
        }

        /// <summary>Initializes tagStructureContext to track necessary information of document's tag structure.</summary>
        protected override void InitTagStructureContext() {
            if (isPdfADocument) {
                tagStructureContext = new TagStructureContext(this, GetPdfVersionForPdfA(checker.GetConformanceLevel()));
            }
            else {
                base.InitTagStructureContext();
            }
        }

        protected override IPdfPageFactory GetPageFactory() {
            if (isPdfADocument) {
                return pdfAPageFactory;
            }
            else {
                return base.GetPageFactory();
            }
        }

        internal virtual bool IsClosing() {
            return isClosing;
        }

        private static PdfVersion GetPdfVersionForPdfA(PdfAConformanceLevel conformanceLevel) {
            PdfVersion version;
            switch (conformanceLevel.GetPart()) {
                case "1": {
                    version = PdfVersion.PDF_1_4;
                    break;
                }

                case "2": {
                    version = PdfVersion.PDF_1_7;
                    break;
                }

                case "3": {
                    version = PdfVersion.PDF_1_7;
                    break;
                }

                default: {
                    version = PdfVersion.PDF_1_4;
                    break;
                }
            }
            return version;
        }
    }
}
