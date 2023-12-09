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
using System.Collections.Generic;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers {
    /// <summary>The class defines the requirements of the PDF/UA-1 standard.</summary>
    /// <remarks>
    /// The class defines the requirements of the PDF/UA-1 standard.
    /// <para />
    /// The specification implemented by this class is ISO 14289-1
    /// </remarks>
    public class PdfUA1Checker : IValidationChecker {
        private readonly PdfDocument pdfDocument;

        /// <summary>Creates PdfUA1Checker instance with PDF document which will be validated against PDF/UA-1 standard.
        ///     </summary>
        /// <param name="pdfDocument">the document to validate</param>
        public PdfUA1Checker(PdfDocument pdfDocument) {
            this.pdfDocument = pdfDocument;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void ValidateDocument(ValidationContext validationContext) {
            CheckCatalog(validationContext.GetPdfDocument().GetCatalog());
            CheckFonts(validationContext.GetFonts());
        }

        /// <summary><inheritDoc/></summary>
        public virtual void ValidateObject(Object obj, IsoKey key, PdfResources resources, PdfStream contentStream
            , Object extra) {
            switch (key) {
                case IsoKey.CANVAS_WRITING_CONTENT: {
                    CheckOnWritingCanvasToContent(obj);
                    break;
                }

                case IsoKey.CANVAS_BEGIN_MARKED_CONTENT: {
                    CheckOnOpeningBeginMarkedContent(obj, extra);
                    break;
                }
            }
        }

        private void CheckOnWritingCanvasToContent(Object data) {
            Stack<Tuple2<PdfName, PdfDictionary>> tagStack = GetTagStack(data);
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

        private Stack<Tuple2<PdfName, PdfDictionary>> GetTagStack(Object data) {
            return (Stack<Tuple2<PdfName, PdfDictionary>>)data;
        }

        private void CheckOnOpeningBeginMarkedContent(Object obj, Object extra) {
            Stack<Tuple2<PdfName, PdfDictionary>> stack = GetTagStack(obj);
            if (stack.IsEmpty()) {
                return;
            }
            Tuple2<PdfName, PdfDictionary> currentBmc = (Tuple2<PdfName, PdfDictionary>)extra;
            bool isRealContent = IsRealContent(currentBmc);
            bool isArtifact = PdfName.Artifact.Equals(currentBmc.GetFirst());
            if (isArtifact && IsInsideRealContent(stack)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.ARTIFACT_CANT_BE_INSIDE_REAL_CONTENT);
            }
            if (isRealContent && IsInsideArtifact(stack)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.REAL_CONTENT_CANT_BE_INSIDE_ARTIFACT);
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
            PdfMcr mcr = this.pdfDocument.GetStructTreeRoot().FindMcrByMcid(pdfDocument, (int)properties.GetAsInt(PdfName
                .MCID));
            if (mcr == null) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT
                    );
            }
            return true;
        }

        private void CheckCatalog(PdfCatalog catalog) {
            PdfDictionary catalogDict = catalog.GetPdfObject();
            if (!catalogDict.ContainsKey(PdfName.Metadata)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.METADATA_SHALL_BE_PRESENT_IN_THE_CATALOG_DICTIONARY
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
    }
}
