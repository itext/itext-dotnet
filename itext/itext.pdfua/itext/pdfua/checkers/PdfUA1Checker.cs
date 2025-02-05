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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils.Checkers;
using iText.Kernel.Validation;
using iText.Kernel.Validation.Context;
using iText.Kernel.XMP;
using iText.Layout.Validation.Context;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Checkers.Utils.Headings;
using iText.Pdfua.Checkers.Utils.Tables;
using iText.Pdfua.Exceptions;
using iText.Pdfua.Logs;

namespace iText.Pdfua.Checkers {
    /// <summary>The class defines the requirements of the PDF/UA-1 standard.</summary>
    /// <remarks>
    /// The class defines the requirements of the PDF/UA-1 standard.
    /// <para />
    /// The specification implemented by this class is ISO 14289-1
    /// </remarks>
    public class PdfUA1Checker : IValidationChecker {
        private readonly PdfDocument pdfDocument;

        private readonly TagStructureContext tagStructureContext;

        private readonly HeadingsChecker headingsChecker;

        private readonly PdfUAValidationContext context;

        private bool warnedOnPageFlush = false;

        /// <summary>Creates PdfUA1Checker instance with PDF document which will be validated against PDF/UA-1 standard.
        ///     </summary>
        /// <param name="pdfDocument">the document to validate</param>
        public PdfUA1Checker(PdfDocument pdfDocument) {
            this.pdfDocument = pdfDocument;
            this.tagStructureContext = new TagStructureContext(pdfDocument);
            this.context = new PdfUAValidationContext(pdfDocument);
            this.headingsChecker = new HeadingsChecker(context);
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual void Validate(IValidationContext context) {
            switch (context.GetType()) {
                case ValidationType.PDF_DOCUMENT: {
                    PdfDocumentValidationContext pdfDocContext = (PdfDocumentValidationContext)context;
                    CheckCatalog(pdfDocContext.GetPdfDocument().GetCatalog());
                    CheckStructureTreeRoot(pdfDocContext.GetPdfDocument().GetStructTreeRoot());
                    CheckFonts(pdfDocContext.GetDocumentFonts());
                    XfaCheckUtil.Check(pdfDocContext.GetPdfDocument());
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
                    CheckOnOpeningBeginMarkedContent(bmcContext.GetTagStructureStack(), bmcContext.GetCurrentBmc());
                    break;
                }

                case ValidationType.CANVAS_WRITING_CONTENT: {
                    CanvasWritingContentValidationContext writingContext = (CanvasWritingContentValidationContext)context;
                    CheckOnWritingCanvasToContent(writingContext.GetTagStructureStack());
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
        public virtual bool IsPdfObjectReadyToFlush(PdfObject @object) {
            return true;
        }

        /// <summary>Logs a warn on page flushing that page flushing is disabled in PDF/UA mode.</summary>
        public virtual void WarnOnPageFlush() {
            if (!warnedOnPageFlush) {
                ITextLogManager.GetLogger(typeof(iText.Pdfua.Checkers.PdfUA1Checker)).LogWarning(PdfUALogMessageConstants.
                    PAGE_FLUSHING_DISABLED);
                warnedOnPageFlush = true;
            }
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

        private void CheckText(String str, PdfFont font) {
            int index = FontCheckUtil.CheckGlyphsOfText(str, font, new PdfUA1Checker.UaCharacterChecker());
            if (index != -1) {
                throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                    , str[index]));
            }
        }

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

        private void CheckViewerPreferences(PdfCatalog catalog) {
            PdfDictionary viewerPreferences = catalog.GetPdfObject().GetAsDictionary(PdfName.ViewerPreferences);
            if (viewerPreferences == null) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MISSING_VIEWER_PREFERENCES);
            }
            PdfObject displayDocTitle = viewerPreferences.Get(PdfName.DisplayDocTitle);
            if (!(displayDocTitle is PdfBoolean)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.MISSING_VIEWER_PREFERENCES);
            }
            if (PdfBoolean.FALSE.Equals(displayDocTitle)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.VIEWER_PREFERENCES_IS_FALSE);
            }
        }

        private void CheckOnWritingCanvasToContent(Stack<Tuple2<PdfName, PdfDictionary>> tagStack) {
            if (tagStack.IsEmpty()) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                    );
            }
            bool insideRealContent = IsInsideRealContent(tagStack);
            bool insideArtifact = IsInsideArtifact(tagStack);
            if (insideRealContent && insideArtifact) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.REAL_CONTENT_INSIDE_ARTIFACT_OR_VICE_VERSA
                    );
            }
            else {
                if (!insideRealContent && !insideArtifact) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.CONTENT_IS_NOT_REAL_CONTENT_AND_NOT_ARTIFACT
                        );
                }
            }
        }

        private void CheckOnOpeningBeginMarkedContent(Stack<Tuple2<PdfName, PdfDictionary>> stack, Tuple2<PdfName, 
            PdfDictionary> currentBmc) {
            CheckStandardRoleMapping(currentBmc);
            if (stack.IsEmpty()) {
                return;
            }
            bool isRealContent = IsRealContent(currentBmc);
            bool isArtifact = PdfName.Artifact.Equals(currentBmc.GetFirst());
            if (isArtifact && IsInsideRealContent(stack)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.ARTIFACT_CANT_BE_INSIDE_REAL_CONTENT);
            }
            if (isRealContent && IsInsideArtifact(stack)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.REAL_CONTENT_CANT_BE_INSIDE_ARTIFACT);
            }
        }

        private void CheckStandardRoleMapping(Tuple2<PdfName, PdfDictionary> tag) {
            PdfNamespace @namespace = tagStructureContext.GetDocumentDefaultNamespace();
            String role = tag.GetFirst().GetValue();
            if (!StandardRoles.ARTIFACT.Equals(role) && !tagStructureContext.CheckIfRoleShallBeMappedToStandardRole(role
                , @namespace)) {
                throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.TAG_MAPPING_DOESNT_TERMINATE_WITH_STANDARD_TYPE
                    , role));
            }
        }

        private bool IsInsideArtifact(Stack<Tuple2<PdfName, PdfDictionary>> tagStack) {
            foreach (Tuple2<PdfName, PdfDictionary> tag in tagStack) {
                if (PdfName.Artifact.Equals(tag.GetFirst())) {
                    return true;
                }
            }
            return false;
        }

        private bool IsInsideRealContent(Stack<Tuple2<PdfName, PdfDictionary>> tagStack) {
            foreach (Tuple2<PdfName, PdfDictionary> tag in tagStack) {
                if (IsRealContent(tag)) {
                    return true;
                }
            }
            return false;
        }

        private bool IsRealContent(Tuple2<PdfName, PdfDictionary> tag) {
            if (PdfName.Artifact.Equals(tag.GetFirst())) {
                return false;
            }
            PdfDictionary properties = tag.GetSecond();
            if (properties == null || !properties.ContainsKey(PdfName.MCID)) {
                return false;
            }
            PdfMcr mcr = McrExists(pdfDocument, (int)properties.GetAsInt(PdfName.MCID));
            if (mcr == null) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT
                    );
            }
            return true;
        }

        private PdfMcr McrExists(PdfDocument document, int mcid) {
            int amountOfPages = document.GetNumberOfPages();
            for (int i = 1; i <= amountOfPages; ++i) {
                PdfPage page = document.GetPage(i);
                PdfMcr mcr = document.GetStructTreeRoot().FindMcrByMcid(page.GetPdfObject(), mcid);
                if (mcr != null) {
                    return mcr;
                }
            }
            return null;
        }

        private void CheckCatalog(PdfCatalog catalog) {
            PdfDictionary catalogDict = catalog.GetPdfObject();
            if (!catalogDict.ContainsKey(PdfName.Metadata)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.METADATA_SHALL_BE_PRESENT_IN_THE_CATALOG_DICTIONARY
                    );
            }
            if (!(catalogDict.Get(PdfName.Lang) is PdfString) || !BCP47Validator.Validate(catalogDict.Get(PdfName.Lang
                ).ToString())) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DOCUMENT_SHALL_CONTAIN_VALID_LANG_ENTRY
                    );
            }
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
            tagTreeIterator.AddHandler(new FormulaCheckUtil.FormulaTagHandler(context));
            tagTreeIterator.AddHandler(new NoteCheckUtil.NoteTagHandler(context));
            tagTreeIterator.AddHandler(new HeadingsChecker.HeadingHandler(context));
            tagTreeIterator.AddHandler(new TableCheckUtil.TableHandler(context));
            tagTreeIterator.AddHandler(new AnnotationCheckUtil.AnnotationHandler(context));
            tagTreeIterator.AddHandler(new FormCheckUtil.FormTagHandler(context));
            tagTreeIterator.Traverse();
        }

        private void CheckOCProperties(PdfDictionary ocProperties) {
            if (ocProperties == null) {
                return;
            }
            if (!(ocProperties.Get(PdfName.Configs) is PdfArray)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.OCG_PROPERTIES_CONFIG_SHALL_BE_AN_ARRAY
                    );
            }
            PdfArray configs = ocProperties.GetAsArray(PdfName.Configs);
            if (configs != null && !configs.IsEmpty()) {
                PdfDictionary d = ocProperties.GetAsDictionary(PdfName.D);
                CheckOCGNameAndASKey(d);
                foreach (PdfObject config in configs) {
                    CheckOCGNameAndASKey((PdfDictionary)config);
                }
                PdfArray ocgsArray = ocProperties.GetAsArray(PdfName.OCGs);
                if (ocgsArray != null) {
                    foreach (PdfObject ocg in ocgsArray) {
                        CheckOCGNameAndASKey((PdfDictionary)ocg);
                    }
                }
            }
        }

        private void CheckOCGNameAndASKey(PdfDictionary dict) {
            if (dict == null) {
                return;
            }
            if (dict.Get(PdfName.AS) != null) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.OCG_SHALL_NOT_CONTAIN_AS_ENTRY);
            }
            if (!(dict.Get(PdfName.Name) is PdfString) || (String.IsNullOrEmpty(((PdfString)dict.Get(PdfName.Name)).ToString
                ()))) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.NAME_ENTRY_IS_MISSING_OR_EMPTY_IN_OCG);
            }
        }

        private void CheckFonts(ICollection<PdfFont> fontsInDocument) {
            ICollection<String> fontNamesThatAreNotEmbedded = new HashSet<String>();
            foreach (PdfFont font in fontsInDocument) {
                if (!font.IsEmbedded()) {
                    fontNamesThatAreNotEmbedded.Add(font.GetFontProgram().GetFontNames().GetFontName());
                }
            }
            if (!fontNamesThatAreNotEmbedded.IsEmpty()) {
                throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.FONT_SHOULD_BE_EMBEDDED
                    , String.Join(", ", fontNamesThatAreNotEmbedded)));
            }
        }

        private void CheckCrypto(PdfDictionary encryptionDictionary) {
            if (encryptionDictionary != null) {
                if (!(encryptionDictionary.Get(PdfName.P) is PdfNumber)) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.P_VALUE_IS_ABSENT_IN_ENCRYPTION_DICTIONARY
                        );
                }
                int permissions = ((PdfNumber)encryptionDictionary.Get(PdfName.P)).IntValue();
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

        private sealed class UaCharacterChecker : FontCheckUtil.CharacterChecker {
            public bool Check(int ch, PdfFont font) {
                if (font.ContainsGlyph(ch)) {
                    return !font.GetGlyph(ch).HasValidUnicode();
                }
                else {
                    return true;
                }
            }
        }
    }
}
