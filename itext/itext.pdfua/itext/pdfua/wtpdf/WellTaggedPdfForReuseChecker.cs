/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils.Checkers;
using iText.Kernel.Validation;
using iText.Kernel.Validation.Context;
using iText.Kernel.XMP;
using iText.Layout.Validation.Context;
using iText.Pdfua.Checkers.Utils.Tables;
using iText.Pdfua.Checkers.Utils.Ua2;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Wtpdf {
    /// <summary>
    /// <see cref="WellTaggedPdfForReuseChecker"/>
    /// class performs validation of a PDF document against WTPDF For Reuse standard.
    /// </summary>
    public class WellTaggedPdfForReuseChecker : WellTaggedPdfForAccessibilityChecker {
        /// <summary>
        /// Creates
        /// <see cref="WellTaggedPdfForReuseChecker"/>
        /// instance which will be validated against WTPDF For Reuse standard.
        /// </summary>
        /// <param name="pdfDocument">the document to validate</param>
        public WellTaggedPdfForReuseChecker(PdfDocument pdfDocument)
            : base(pdfDocument) {
        }

        public override void Validate(IValidationContext context) {
            switch (context.GetType()) {
                case ValidationType.PDF_DOCUMENT: {
                    ValidatePdfDocument((PdfDocumentValidationContext)context);
                    break;
                }

                case ValidationType.FONT: {
                    FontValidationContext fontContext = (FontValidationContext)context;
                    CheckText(fontContext.GetText(), fontContext.GetFont());
                    break;
                }

                case ValidationType.CANVAS_BEGIN_MARKED_CONTENT: {
                    ValidateCanvasBmc((CanvasBmcValidationContext)context);
                    break;
                }

                case ValidationType.CANVAS_WRITING_CONTENT: {
                    ValidateCanvasWriting((CanvasWritingContentValidationContext)context);
                    break;
                }

                case ValidationType.LAYOUT: {
                    ValidateLayout((LayoutValidationContext)context);
                    break;
                }

                case ValidationType.DESTINATION_ADDITION: {
                    ValidateDestinationAddition((PdfDestinationAdditionContext)context);
                    break;
                }

                case ValidationType.PDF_OBJECT: {
                    PdfObjectValidationContext validationContext = (PdfObjectValidationContext)context;
                    CheckPdfObject(validationContext.GetObject());
                    break;
                }

                case ValidationType.ANNOTATION: {
                    ValidateAnnotation((PdfAnnotationContext)context);
                    break;
                }

                default: {
                    break;
                }
            }
        }

        /// <summary>Validates document catalog dictionary against PDF/UA-2 standard.</summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary to check
        /// </param>
        protected internal override void CheckCatalog(PdfCatalog catalog) {
            CheckLang(catalog);
            CheckMetadata(catalog);
            CheckFormFieldsAndAnnotations(catalog);
            PdfUA2EmbeddedFilesChecker.CheckEmbeddedFiles(catalog);
        }

        /// <summary>Validates all annotations and form fields present in the document against PDF/UA-2 standard.</summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// to check form fields present in the acroform
        /// </param>
        protected internal override void CheckFormFieldsAndAnnotations(PdfCatalog catalog) {
            PdfUA2FormChecker formChecker = new PdfUA2FormChecker(GetUAValidationContext());
            formChecker.CheckFormFields(catalog.GetPdfObject().GetAsDictionary(PdfName.AcroForm));
            formChecker.CheckWidgetAnnotations(GetPdfDocument());
            PdfUA2LinkChecker.CheckLinkAnnotations(GetPdfDocument());
            new WellTaggedPdfForReuseAnnotationChecker().CheckAllAnnotations(GetPdfDocument());
        }

        protected internal override TagTreeIterator CreateTagTreeIterator(PdfStructTreeRoot structTreeRoot) {
            TagTreeIterator tagTreeIterator = new TagTreeIterator(structTreeRoot);
            tagTreeIterator.AddHandler(new PdfUA2HeadingsChecker.PdfUA2HeadingHandler(GetUAValidationContext()));
            tagTreeIterator.AddHandler(new TableCheckUtil.TableHandler(GetUAValidationContext()));
            tagTreeIterator.AddHandler(new PdfUA2FormChecker.PdfUA2FormTagHandler(GetUAValidationContext()));
            tagTreeIterator.AddHandler(new WellTaggedPdfForReuseAnnotationChecker.WellTaggedPdfForReuseAnnotationHandler
                (GetUAValidationContext()));
            tagTreeIterator.AddHandler(new PdfUA2ListChecker.PdfUA2ListHandler(GetUAValidationContext()));
            tagTreeIterator.AddHandler(new PdfUA2NotesChecker.PdfUA2NotesHandler(GetUAValidationContext()));
            tagTreeIterator.AddHandler(new PdfUA2TableOfContentsChecker.PdfUA2TableOfContentsHandler(GetUAValidationContext
                ()));
            tagTreeIterator.AddHandler(new PdfUA2FormulaChecker.PdfUA2FormulaTagHandler(GetUAValidationContext()));
            tagTreeIterator.AddHandler(new PdfUA2LinkChecker.PdfUA2LinkAnnotationHandler(GetUAValidationContext(), GetPdfDocument
                ()));
            return tagTreeIterator;
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
        /// a metadata stream as defined in ISO 32000-2:2020.
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
        protected internal override void CheckMetadata(PdfCatalog catalog) {
            PdfCheckersUtil.CheckMetadata(catalog.GetPdfObject(), PdfConformance.WELL_TAGGED_PDF_FOR_REUSE, (msg) => new 
                PdfUAConformanceException(msg));
            try {
                XMPMeta metadata = catalog.GetDocument().GetXmpMetadata();
                if (metadata.GetProperty(XMPConst.NS_DC, XMPConst.TITLE) == null) {
                    throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_DC_TITLE_ENTRY);
                }
            }
            catch (XMPException e) {
                throw new PdfUAConformanceException(e.Message, e);
            }
        }

        private void ValidatePdfDocument(PdfDocumentValidationContext pdfDocContext) {
            CheckCatalog(pdfDocContext.GetPdfDocument().GetCatalog());
            CheckStructureTreeRoot(pdfDocContext.GetPdfDocument().GetStructTreeRoot());
            CheckFonts(pdfDocContext.GetDocumentFonts());
            new PdfUA2DestinationsChecker(pdfDocContext.GetPdfDocument()).CheckDestinations();
            PdfUA2XfaChecker.Check(pdfDocContext.GetPdfDocument());
        }

        private void ValidateCanvasBmc(CanvasBmcValidationContext bmcContext) {
            CheckLogicalStructureInBMC(bmcContext.GetTagStructureStack(), bmcContext.GetCurrentBmc(), GetPdfDocument()
                );
        }

        private void ValidateCanvasWriting(CanvasWritingContentValidationContext writingContext) {
            CheckContentInCanvas(writingContext.GetTagStructureStack(), GetPdfDocument());
        }

        private void ValidateLayout(LayoutValidationContext layoutContext) {
            new WellTaggedPdfForReuseLayoutChecker(GetUAValidationContext()).CheckRenderer(layoutContext.GetRenderer()
                );
            new PdfUA2HeadingsChecker(GetUAValidationContext()).CheckLayoutElement(layoutContext.GetRenderer());
        }

        private void ValidateDestinationAddition(PdfDestinationAdditionContext ctx) {
            new PdfUA2DestinationsChecker(ctx, GetPdfDocument()).CheckDestinationsOnCreation();
        }

        private void ValidateAnnotation(PdfAnnotationContext annotationContext) {
            new WellTaggedPdfForReuseAnnotationChecker().CheckSingleAnnotation(annotationContext.GetAnnotation(), GetUAValidationContext
                ());
        }
    }
}
