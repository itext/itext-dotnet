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
using iText.Pdfua.Checkers.Utils.Ua2;
using iText.Pdfua.Exceptions;
using iText.Pdfua.Logs;

namespace iText.Pdfua.Checkers {
    /// <summary>
    /// The class defines the requirements of the PDF/UA-2 standard and contains
    /// method implementations from the abstract
    /// <see cref="PdfUAChecker"/>
    /// class.
    /// </summary>
    /// <remarks>
    /// The class defines the requirements of the PDF/UA-2 standard and contains
    /// method implementations from the abstract
    /// <see cref="PdfUAChecker"/>
    /// class.
    /// <para />
    /// The specification implemented by this class is ISO 14289-2.
    /// </remarks>
    public class PdfUA2Checker : PdfUAChecker {
        private readonly PdfDocument pdfDocument;

        private readonly PdfUAValidationContext context;

        private readonly PdfUA2CanvasTextChecker textChecker = new PdfUA2CanvasTextChecker();

        /// <summary>
        /// Creates
        /// <see cref="PdfUA2Checker"/>
        /// instance with PDF document which will be validated against PDF/UA-2 standard.
        /// </summary>
        /// <param name="pdfDocument">the document to validate</param>
        public PdfUA2Checker(PdfDocument pdfDocument)
            : base() {
            this.pdfDocument = pdfDocument;
            this.context = new PdfUAValidationContext(this.pdfDocument);
        }

        public override void Validate(IValidationContext context) {
            switch (context.GetType()) {
                case ValidationType.PDF_DOCUMENT: {
                    PdfDocumentValidationContext pdfDocContext = (PdfDocumentValidationContext)context;
                    CheckCatalog(pdfDocContext.GetPdfDocument().GetCatalog());
                    CheckStructureTreeRoot(pdfDocContext.GetPdfDocument().GetStructTreeRoot());
                    CheckFonts(pdfDocContext.GetDocumentFonts());
                    new PdfUA2DestinationsChecker(pdfDocContext.GetPdfDocument()).CheckDestinations();
                    PdfUA2XfaChecker.Check(pdfDocContext.GetPdfDocument());
                    textChecker.CheckCollectedContexts(pdfDocContext.GetPdfDocument());
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
                    new PdfUA2HeadingsChecker(this.context).CheckLayoutElement(layoutContext.GetRenderer());
                    break;
                }

                case ValidationType.DESTINATION_ADDITION: {
                    PdfDestinationAdditionContext destinationAdditionContext = (PdfDestinationAdditionContext)context;
                    new PdfUA2DestinationsChecker(destinationAdditionContext, pdfDocument).CheckDestinationsOnCreation();
                    break;
                }

                case ValidationType.PDF_OBJECT: {
                    PdfObjectValidationContext validationContext = (PdfObjectValidationContext)context;
                    CheckPdfObject(validationContext.GetObject());
                    break;
                }

                case ValidationType.ANNOTATION: {
                    PdfAnnotationContext annotationContext = (PdfAnnotationContext)context;
                    PdfUA2AnnotationChecker.CheckAnnotation(annotationContext.GetAnnotation(), this.context);
                    break;
                }

                case ValidationType.CANVAS_TEXT_ADDITION: {
                    CanvasTextAdditionContext canvasTextAdditionContext = (CanvasTextAdditionContext)context;
                    textChecker.CollectTextAdditionContext(canvasTextAdditionContext);
                    break;
                }
            }
        }

        public override bool IsPdfObjectReadyToFlush(PdfObject @object) {
            return false;
        }

        /// <summary>
        /// Checks that the
        /// <c>Catalog</c>
        /// dictionary of a conforming file contains the
        /// <c>Metadata</c>
        /// key whose value is
        /// a metadata stream as defined in ISO 32000-2:2020.
        /// </summary>
        /// <remarks>
        /// Checks that the
        /// <c>Catalog</c>
        /// dictionary of a conforming file contains the
        /// <c>Metadata</c>
        /// key whose value is
        /// a metadata stream as defined in ISO 32000-2:2020. Also checks that the value of
        /// <c>pdfuaid:part</c>
        /// is 2 for
        /// conforming PDF files and validates required
        /// <c>pdfuaid:rev</c>
        /// value.
        /// <para />
        /// Checks that the
        /// <c>Metadata</c>
        /// stream as specified in ISO 32000-2:2020, 14.3 in the document catalog dictionary
        /// includes a
        /// <c>dc: title</c>
        /// entry reflecting the title of the document.
        /// </remarks>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary
        /// </param>
        protected internal virtual void CheckMetadata(PdfCatalog catalog) {
            PdfCheckersUtil.CheckMetadata(catalog.GetPdfObject(), PdfConformance.PDF_UA_2, EXCEPTION_SUPPLIER);
            try {
                XMPMeta metadata = catalog.GetDocument().GetXmpMetadata();
                if (metadata.GetProperty(XMPConst.NS_DC, XMPConst.TITLE) == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_DC_TITLE_ENTRY);
                }
            }
            catch (XMPException e) {
                throw new PdfUAConformanceException(e.Message);
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// For all non-symbolic TrueType fonts used for rendering, the embedded TrueType font program shall contain
        /// at least the Microsoft Unicode (3, 1 – Platform ID = 3, Encoding ID = 1),
        /// or the Macintosh Roman (1, 0 – Platform ID = 1, Encoding ID = 0) “cmap” subtable.
        /// </summary>
        /// <param name="fontProgram">the embedded TrueType font program to check</param>
        internal override void CheckNonSymbolicCmapSubtable(TrueTypeFont fontProgram) {
            if (!fontProgram.IsCmapPresent(3, 1) && !fontProgram.IsCmapPresent(1, 0)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.NON_SYMBOLIC_TTF_SHALL_CONTAIN_MAC_ROMAN_OR_MICROSOFT_UNI_CMAP
                    );
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks cmap entries present in the embedded TrueType font program of the symbolic TrueType font.</summary>
        /// <remarks>
        /// Checks cmap entries present in the embedded TrueType font program of the symbolic TrueType font.
        /// <para />
        /// The “cmap” subtable in the embedded font program shall either contain the Microsoft Symbol
        /// (3, 0 – Platform ID = 3, Encoding ID = 0) or the Mac Roman (1, 0 – Platform ID = 1, Encoding ID = 1) encoding.
        /// </remarks>
        /// <param name="fontProgram">the embedded TrueType font program to check</param>
        internal override void CheckSymbolicCmapSubtable(TrueTypeFont fontProgram) {
            if (!fontProgram.IsCmapPresent(3, 0) && !fontProgram.IsCmapPresent(1, 0)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.SYMBOLIC_TTF_SHALL_CONTAIN_MAC_ROMAN_OR_MICROSOFT_SYMBOL_CMAP
                    );
            }
        }
//\endcond

        private void CheckPdfObject(PdfObject obj) {
            switch (obj.GetObjectType()) {
                case PdfObject.STRING: {
                    PdfUA2StringChecker.CheckPdfString((PdfString)obj);
                    break;
                }

                case PdfObject.ARRAY: {
                    CheckArrayRecursively((PdfArray)obj);
                    break;
                }

                case PdfObject.DICTIONARY:
                case PdfObject.STREAM: {
                    CheckDictionaryRecursively((PdfDictionary)obj);
                    break;
                }
            }
        }

        private void CheckArrayRecursively(PdfArray array) {
            for (int i = 0; i < array.Size(); i++) {
                PdfObject @object = array.Get(i, false);
                if (@object != null && !@object.IsIndirect()) {
                    CheckPdfObject(@object);
                }
            }
        }

        private void CheckDictionaryRecursively(PdfDictionary dictionary) {
            foreach (PdfName name in dictionary.KeySet()) {
                PdfObject @object = dictionary.Get(name, false);
                if (@object != null && !@object.IsIndirect()) {
                    CheckPdfObject(@object);
                }
            }
        }

        /// <summary>Validates document catalog dictionary against PDF/UA-2 standard.</summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary to check
        /// </param>
        private void CheckCatalog(PdfCatalog catalog) {
            CheckLang(catalog);
            CheckMetadata(catalog);
            CheckViewerPreferences(catalog);
            CheckOCProperties(catalog.GetPdfObject().GetAsDictionary(PdfName.OCProperties));
            CheckFormFieldsAndAnnotations(catalog);
            PdfUA2EmbeddedFilesChecker.CheckEmbeddedFiles(catalog);
        }

        /// <summary>Validates all annotations and form fields present in the document against PDF/UA-2 standard.</summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// to check form fields present in the acroform
        /// </param>
        private void CheckFormFieldsAndAnnotations(PdfCatalog catalog) {
            PdfUA2FormChecker formChecker = new PdfUA2FormChecker(context);
            formChecker.CheckFormFields(catalog.GetPdfObject().GetAsDictionary(PdfName.AcroForm));
            formChecker.CheckWidgetAnnotations(this.pdfDocument);
            PdfUA2LinkChecker.CheckLinkAnnotations(this.pdfDocument);
            PdfUA2AnnotationChecker.CheckAnnotations(this.pdfDocument);
        }

        /// <summary>Validates structure tree root dictionary against PDF/UA-2 standard.</summary>
        /// <remarks>
        /// Validates structure tree root dictionary against PDF/UA-2 standard.
        /// <para />
        /// Additionally, checks that within a given explicitly provided namespace, structure types are not role mapped to
        /// other structure types in the same namespace. In the StructTreeRoot RoleMap there is no explicitly provided
        /// namespace, that's why it is not checked.
        /// </remarks>
        /// <param name="structTreeRoot">
        /// 
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructTreeRoot"/>
        /// structure tree root dictionary to check
        /// </param>
        private void CheckStructureTreeRoot(PdfStructTreeRoot structTreeRoot) {
            IList<PdfNamespace> namespaces = structTreeRoot.GetNamespaces();
            foreach (PdfNamespace @namespace in namespaces) {
                PdfDictionary roleMap = @namespace.GetNamespaceRoleMap();
                if (roleMap != null) {
                    foreach (KeyValuePair<PdfName, PdfObject> entry in roleMap.EntrySet()) {
                        String role = entry.Key.GetValue();
                        IRoleMappingResolver roleMappingResolver = pdfDocument.GetTagStructureContext().GetRoleMappingResolver(role
                            , @namespace);
                        int i = 0;
                        // Reasonably large arbitrary number that will help to avoid a possible infinite loop.
                        int maxIters = 100;
                        while (roleMappingResolver.ResolveNextMapping()) {
                            if (++i > maxIters) {
                                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Pdfua.Checkers.PdfUA2Checker));
                                logger.LogError(MessageFormatUtil.Format(PdfUALogMessageConstants.CANNOT_RESOLVE_ROLE_IN_NAMESPACE_TOO_MUCH_TRANSITIVE_MAPPINGS
                                    , role, @namespace));
                                break;
                            }
                            PdfNamespace roleMapToNamespace = roleMappingResolver.GetNamespace();
                            if (@namespace.GetNamespaceName().Equals(roleMapToNamespace.GetNamespaceName())) {
                                throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.STRUCTURE_TYPE_IS_ROLE_MAPPED_TO_OTHER_STRUCTURE_TYPE_IN_THE_SAME_NAMESPACE
                                    , @namespace.GetNamespaceName(), role));
                            }
                        }
                    }
                }
            }
            TagTreeIterator tagTreeIterator = new TagTreeIterator(structTreeRoot);
            tagTreeIterator.AddHandler(new GraphicsCheckUtil.GraphicsHandler(context));
            tagTreeIterator.AddHandler(new PdfUA2HeadingsChecker.PdfUA2HeadingHandler(context));
            tagTreeIterator.AddHandler(new TableCheckUtil.TableHandler(context));
            tagTreeIterator.AddHandler(new PdfUA2FormChecker.PdfUA2FormTagHandler(context));
            tagTreeIterator.AddHandler(new PdfUA2AnnotationChecker.PdfUA2AnnotationHandler(context));
            tagTreeIterator.AddHandler(new PdfUA2ListChecker.PdfUA2ListHandler(context));
            tagTreeIterator.AddHandler(new PdfUA2NotesChecker.PdfUA2NotesHandler(context));
            tagTreeIterator.AddHandler(new PdfUA2TableOfContentsChecker.PdfUA2TableOfContentsHandler(context));
            tagTreeIterator.AddHandler(new PdfUA2FormulaChecker.PdfUA2FormulaTagHandler(context));
            tagTreeIterator.AddHandler(new PdfUA2LinkChecker.PdfUA2LinkAnnotationHandler(context, pdfDocument));
            tagTreeIterator.Traverse();
        }
    }
}
