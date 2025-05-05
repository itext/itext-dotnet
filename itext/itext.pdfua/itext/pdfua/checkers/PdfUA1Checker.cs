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
using System.Collections.Generic;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils.Checkers;
using iText.Kernel.Validation;
using iText.Kernel.Validation.Context;
using iText.Kernel.XMP;
using iText.Layout.Validation.Context;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Checkers.Utils.Tables;
using iText.Pdfua.Checkers.Utils.Ua1;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers {
    /// <summary>
    /// The class defines the requirements of the PDF/UA-1 standard and contains
    /// method implementations from the abstract
    /// <see cref="PdfUAChecker"/>
    /// class.
    /// </summary>
    /// <remarks>
    /// The class defines the requirements of the PDF/UA-1 standard and contains
    /// method implementations from the abstract
    /// <see cref="PdfUAChecker"/>
    /// class.
    /// <para />
    /// The specification implemented by this class is ISO 14289-1.
    /// </remarks>
    public class PdfUA1Checker : PdfUAChecker {
        private readonly PdfDocument pdfDocument;

        private readonly TagStructureContext tagStructureContext;

        private readonly PdfUA1HeadingsChecker headingsChecker;

        private readonly PdfUAValidationContext context;

        /// <summary>Creates PdfUA1Checker instance with PDF document which will be validated against PDF/UA-1 standard.
        ///     </summary>
        /// <param name="pdfDocument">the document to validate</param>
        public PdfUA1Checker(PdfDocument pdfDocument)
            : base() {
            this.pdfDocument = pdfDocument;
            this.tagStructureContext = new TagStructureContext(pdfDocument);
            this.context = new PdfUAValidationContext(pdfDocument);
            this.headingsChecker = new PdfUA1HeadingsChecker(context);
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public override void Validate(IValidationContext context) {
            switch (context.GetType()) {
                case ValidationType.PDF_DOCUMENT: {
                    PdfDocumentValidationContext pdfDocContext = (PdfDocumentValidationContext)context;
                    CheckCatalog(pdfDocContext.GetPdfDocument().GetCatalog());
                    CheckStructureTreeRoot(pdfDocContext.GetPdfDocument().GetStructTreeRoot());
                    CheckFonts(pdfDocContext.GetDocumentFonts());
                    PdfUA1XfaCheckUtil.Check(pdfDocContext.GetPdfDocument());
                    break;
                }

                case ValidationType.PDF_OBJECT: {
                    PdfObjectValidationContext objContext = (PdfObjectValidationContext)context;
                    CheckPdfObject(objContext.GetObject());
                    break;
                }

                case ValidationType.CRYPTO: {
                    CryptoValidationContext cryptoContext = (CryptoValidationContext)context;
                    CheckCrypto((PdfDictionary)cryptoContext.GetCrypto());
                    break;
                }

                case ValidationType.FONT: {
                    FontValidationContext fontContext = (FontValidationContext)context;
                    CheckText(fontContext.GetText(), fontContext.GetFont());
                    break;
                }

                case ValidationType.CANVAS_BEGIN_MARKED_CONTENT: {
                    CanvasBmcValidationContext bmcContext = (CanvasBmcValidationContext)context;
                    CheckLogicalStructureInBMC(bmcContext.GetTagStructureStack(), bmcContext.GetCurrentBmc(), this.pdfDocument
                        );
                    break;
                }

                case ValidationType.CANVAS_WRITING_CONTENT: {
                    CanvasWritingContentValidationContext writingContext = (CanvasWritingContentValidationContext)context;
                    CheckContentInCanvas(writingContext.GetTagStructureStack(), this.pdfDocument);
                    break;
                }

                case ValidationType.LAYOUT: {
                    LayoutValidationContext layoutContext = (LayoutValidationContext)context;
                    new LayoutCheckUtil(this.context).CheckRenderer(layoutContext.GetRenderer());
                    headingsChecker.CheckLayoutElement(layoutContext.GetRenderer());
                    break;
                }

                case ValidationType.DUPLICATE_ID_ENTRY: {
                    DuplicateIdEntryValidationContext idContext = (DuplicateIdEntryValidationContext)context;
                    throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.NON_UNIQUE_ID_ENTRY_IN_STRUCT_TREE_ROOT
                        , idContext.GetId()));
                }
            }
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public override bool IsPdfObjectReadyToFlush(PdfObject @object) {
            return false;
        }

        /// <summary>Verify the conformity of the file specification dictionary.</summary>
        /// <param name="fileSpec">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// containing file specification to be checked
        /// </param>
        protected internal virtual void CheckFileSpec(PdfDictionary fileSpec) {
            if (fileSpec.ContainsKey(PdfName.EF)) {
                if (!fileSpec.ContainsKey(PdfName.F) || !fileSpec.ContainsKey(PdfName.UF)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.FILE_SPECIFICATION_DICTIONARY_SHALL_CONTAIN_F_KEY_AND_UF_KEY
                        );
                }
            }
        }

        /// <summary>
        /// Checks that the
        /// <c>Catalog</c>
        /// dictionary of a conforming file (the version number of a file may be any value
        /// from 1.0 to 1.7) contains the
        /// <c>Metadata</c>
        /// key whose value is a metadata stream.
        /// </summary>
        /// <remarks>
        /// Checks that the
        /// <c>Catalog</c>
        /// dictionary of a conforming file (the version number of a file may be any value
        /// from 1.0 to 1.7) contains the
        /// <c>Metadata</c>
        /// key whose value is a metadata stream. Also checks that the value
        /// of
        /// <c>pdfuaid:part</c>
        /// is 1 for conforming PDF files.
        /// <para />
        /// Checks that the
        /// <c>Metadata</c>
        /// stream in the document catalog dictionary includes a
        /// <c>dc:title</c>
        /// entry
        /// reflecting the title of the document.
        /// </remarks>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary
        /// </param>
        protected internal virtual void CheckMetadata(PdfCatalog catalog) {
            if (catalog.GetDocument().GetPdfVersion().CompareTo(PdfVersion.PDF_1_7) > 0) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.INVALID_PDF_VERSION);
            }
            try {
                XMPMeta metadata = catalog.GetDocument().GetXmpMetadata();
                if (metadata == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DOCUMENT_SHALL_CONTAIN_XMP_METADATA_STREAM
                        );
                }
                int? part = metadata.GetPropertyInteger(XMPConst.NS_PDFUA_ID, XMPConst.PART);
                if (!Convert.ToInt32(1).Equals(part)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_UA_VERSION_IDENTIFIER
                        );
                }
                if (metadata.GetProperty(XMPConst.NS_DC, XMPConst.TITLE) == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_DC_TITLE_ENTRY);
                }
            }
            catch (XMPException e) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DOCUMENT_SHALL_CONTAIN_XMP_METADATA_STREAM
                    , e);
            }
        }

//\cond DO_NOT_DOCUMENT
        internal override void CheckOCProperties(PdfDictionary ocProperties) {
            if (ocProperties != null && !(ocProperties.Get(PdfName.Configs) is PdfArray)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.OCG_PROPERTIES_CONFIG_SHALL_BE_AN_ARRAY
                    );
            }
            base.CheckOCProperties(ocProperties);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override void CheckLogicalStructureInBMC(Stack<Tuple2<PdfName, PdfDictionary>> stack, Tuple2<PdfName
            , PdfDictionary> currentBmc, PdfDocument document) {
            CheckStandardRoleMapping(currentBmc);
            base.CheckLogicalStructureInBMC(stack, currentBmc, document);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// For all non-symbolic TrueType fonts used for rendering, the embedded TrueType font program shall contain one or
        /// several non-symbolic cmap entries such that all necessary glyph lookups can be carried out.
        /// </summary>
        /// <param name="fontProgram">the embedded TrueType font program to check</param>
        internal override void CheckNonSymbolicCmapSubtable(TrueTypeFont fontProgram) {
            if ((fontProgram.IsCmapPresent(3, 0) && fontProgram.GetNumberOfCmaps() == 1) || fontProgram.GetNumberOfCmaps
                () == 0) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.NON_SYMBOLIC_TTF_SHALL_CONTAIN_NON_SYMBOLIC_CMAP
                    );
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks cmap entries present in the embedded TrueType font program of the symbolic TrueType font.</summary>
        /// <remarks>
        /// Checks cmap entries present in the embedded TrueType font program of the symbolic TrueType font.
        /// <para />
        /// The “cmap” table in the embedded font program shall either contain exactly one encoding or it shall contain,
        /// at least, the Microsoft Symbol (3,0 – Platform ID = 3, Encoding ID = 0) encoding.
        /// </remarks>
        /// <param name="fontProgram">the embedded TrueType font program to check</param>
        internal override void CheckSymbolicCmapSubtable(TrueTypeFont fontProgram) {
            if (!fontProgram.IsCmapPresent(3, 0) && fontProgram.GetNumberOfCmaps() != 1) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.SYMBOLIC_TTF_SHALL_CONTAIN_EXACTLY_ONE_OR_AT_LEAST_MICROSOFT_SYMBOL_CMAP
                    );
            }
        }
//\endcond

        private void CheckStandardRoleMapping(Tuple2<PdfName, PdfDictionary> tag) {
            PdfNamespace @namespace = tagStructureContext.GetDocumentDefaultNamespace();
            String role = tag.GetFirst().GetValue();
            if (!StandardRoles.ARTIFACT.Equals(role) && !tagStructureContext.CheckIfRoleShallBeMappedToStandardRole(role
                , @namespace)) {
                throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.TAG_MAPPING_DOESNT_TERMINATE_WITH_STANDARD_TYPE
                    , role));
            }
        }

        private void CheckCatalog(PdfCatalog catalog) {
            PdfDictionary catalogDict = catalog.GetPdfObject();
            if (!catalogDict.ContainsKey(PdfName.Metadata)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.METADATA_SHALL_BE_PRESENT_IN_THE_CATALOG_DICTIONARY
                    );
            }
            CheckLang(catalog);
            PdfCheckersUtil.ValidateLang(catalogDict, EXCEPTION_SUPPLIER);
            PdfDictionary markInfo = catalogDict.GetAsDictionary(PdfName.MarkInfo);
            if (markInfo != null && markInfo.ContainsKey(PdfName.Suspects)) {
                PdfBoolean markInfoSuspects = markInfo.GetAsBoolean(PdfName.Suspects);
                if (markInfoSuspects != null && markInfoSuspects.GetValue()) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.SUSPECTS_ENTRY_IN_MARK_INFO_DICTIONARY_SHALL_NOT_HAVE_A_VALUE_OF_TRUE
                        );
                }
            }
            CheckViewerPreferences(catalog);
            CheckMetadata(catalog);
            CheckOCProperties(catalogDict.GetAsDictionary(PdfName.OCProperties));
        }

        private void CheckStructureTreeRoot(PdfStructTreeRoot structTreeRoot) {
            PdfDictionary roleMap = structTreeRoot.GetRoleMap();
            foreach (KeyValuePair<PdfName, PdfObject> entry in roleMap.EntrySet()) {
                String role = entry.Key.GetValue();
                IRoleMappingResolver roleMappingResolver = pdfDocument.GetTagStructureContext().GetRoleMappingResolver(role
                    );
                if (roleMappingResolver.CurrentRoleIsStandard()) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.ONE_OR_MORE_STANDARD_ROLE_REMAPPED);
                }
            }
            TagTreeIterator tagTreeIterator = new TagTreeIterator(structTreeRoot);
            tagTreeIterator.AddHandler(new GraphicsCheckUtil.GraphicsHandler(context));
            tagTreeIterator.AddHandler(new PdfUA1FormulaChecker.PdfUA1FormulaTagHandler(context));
            tagTreeIterator.AddHandler(new PdfUA1NotesChecker.PdfUA1NotesTagHandler(context));
            tagTreeIterator.AddHandler(new PdfUA1HeadingsChecker.PdfUA1HeadingHandler(context));
            tagTreeIterator.AddHandler(new TableCheckUtil.TableHandler(context));
            tagTreeIterator.AddHandler(new PdfUA1AnnotationChecker.PdfUA1AnnotationHandler(context));
            tagTreeIterator.AddHandler(new PdfUA1FormChecker.PdfUA1FormTagHandler(context));
            tagTreeIterator.AddHandler(new PdfUA1ListChecker.PdfUA1ListHandler(context));
            tagTreeIterator.Traverse();
        }

        private void CheckCrypto(PdfDictionary encryptionDictionary) {
            if (encryptionDictionary != null) {
                if (!(encryptionDictionary.Get(PdfName.P) is PdfNumber)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.P_VALUE_IS_ABSENT_IN_ENCRYPTION_DICTIONARY
                        );
                }
                int permissions = (int)((PdfNumber)encryptionDictionary.Get(PdfName.P)).LongValue();
                if ((EncryptionConstants.ALLOW_SCREENREADERS & permissions) == 0) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.TENTH_BIT_OF_P_VALUE_IN_ENCRYPTION_SHOULD_BE_NON_ZERO
                        );
                }
            }
        }

        /// <summary>
        /// This method checks the requirements that must be fulfilled by a COS
        /// object in a PDF/UA document.
        /// </summary>
        /// <param name="obj">the COS object that must be checked</param>
        private void CheckPdfObject(PdfObject obj) {
            if (obj.GetObjectType() == PdfObject.DICTIONARY) {
                PdfDictionary dict = (PdfDictionary)obj;
                PdfName type = dict.GetAsName(PdfName.Type);
                if (PdfName.Filespec.Equals(type)) {
                    CheckFileSpec(dict);
                }
            }
        }
    }
}
