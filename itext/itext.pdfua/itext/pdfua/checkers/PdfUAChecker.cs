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
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils.Checkers;
using iText.Kernel.Validation;
using iText.Pdfua.Exceptions;
using iText.Pdfua.Logs;

namespace iText.Pdfua.Checkers {
    /// <summary>An abstract class that will run through all necessary checks defined in the different PDF/UA standards.
    ///     </summary>
    /// <remarks>
    /// An abstract class that will run through all necessary checks defined in the different PDF/UA standards. A number of
    /// common checks are executed in this class, while standard-dependent specifications are implemented in the available
    /// subclasses. The standard that is followed is the series of ISO 14289 specifications, currently generations 1 and 2.
    /// <para />
    /// While it is possible to subclass this method and implement its abstract methods in client code, this is not
    /// encouraged and will have little effect. It is not possible to plug custom implementations into iText, because
    /// iText should always refuse to create non-compliant PDF/UA, which would be possible with client code implementations.
    /// Any future generations of the PDF/UA standard and its derivatives will get their own implementation in the iText -
    /// pdfua project.
    /// </remarks>
    public abstract class PdfUAChecker : IValidationChecker {
//\cond DO_NOT_DOCUMENT
        internal static readonly Func<String, PdfException> EXCEPTION_SUPPLIER = (msg) => new PdfUAConformanceException
            (msg);
//\endcond

        private bool warnedOnPageFlush = false;

        /// <summary>
        /// Creates new
        /// <see cref="PdfUAChecker"/>
        /// instance.
        /// </summary>
        protected internal PdfUAChecker() {
        }

        // Empty constructor.
        /// <summary>Logs a warn on page flushing that page flushing is disabled in PDF/UA mode.</summary>
        public virtual void WarnOnPageFlush() {
            if (!warnedOnPageFlush) {
                ITextLogManager.GetLogger(typeof(iText.Pdfua.Checkers.PdfUAChecker)).LogWarning(PdfUALogMessageConstants.PAGE_FLUSHING_DISABLED
                    );
                warnedOnPageFlush = true;
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Checks that the default natural language for content and text strings is specified using the
        /// <c>Lang</c>
        /// entry, with a nonempty value, in the document catalog dictionary.
        /// </summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary
        /// </param>
        internal virtual void CheckLang(PdfCatalog catalog) {
            PdfDictionary catalogDict = catalog.GetPdfObject();
            PdfObject lang = catalogDict.Get(PdfName.Lang);
            if (!(lang is PdfString)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.CATALOG_SHOULD_CONTAIN_LANG_ENTRY);
            }
            if (String.IsNullOrEmpty(((PdfString)lang).GetValue())) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.DOCUMENT_SHALL_CONTAIN_VALID_LANG_ENTRY
                    );
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Checks that the
        /// <c>ViewerPreferences</c>
        /// dictionary of the document catalog dictionary is present and contains
        /// at least the
        /// <c>DisplayDocTitle</c>
        /// key with a value of
        /// <see langword="true"/>
        /// , as defined in
        /// ISO 32000-1:2008, 12.2, Table 150 or ISO 32000-2:2020, Table 147.
        /// </summary>
        /// <param name="catalog">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfCatalog"/>
        /// document catalog dictionary
        /// </param>
        internal virtual void CheckViewerPreferences(PdfCatalog catalog) {
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Checks that all optional content configuration dictionaries in the file, including the default one, shall contain
        /// a Name entry (see ISO 32000-2:2020, Table 96, or ISO 32000-1:2008, 8.11.2.1, Table 98) whose value is a non-empty
        /// text string when document contains a Configs entry in the OCProperties entry of the document catalog dictionary
        /// (see ISO 32000-2:2020, Table 29, or ISO 32000-1:2008, 7.7.2, Table 28), and the Configs entry contains at least
        /// one optional content configuration dictionary.
        /// </summary>
        /// <remarks>
        /// Checks that all optional content configuration dictionaries in the file, including the default one, shall contain
        /// a Name entry (see ISO 32000-2:2020, Table 96, or ISO 32000-1:2008, 8.11.2.1, Table 98) whose value is a non-empty
        /// text string when document contains a Configs entry in the OCProperties entry of the document catalog dictionary
        /// (see ISO 32000-2:2020, Table 29, or ISO 32000-1:2008, 7.7.2, Table 28), and the Configs entry contains at least
        /// one optional content configuration dictionary.
        /// <para />
        /// Also checks that the AS key does not appear in any optional content configuration dictionary.
        /// </remarks>
        /// <param name="ocProperties">OCProperties entry of the Catalog dictionary</param>
        internal virtual void CheckOCProperties(PdfDictionary ocProperties) {
            if (ocProperties == null) {
                return;
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks if content marked as Artifact resides in Artifact content, but real content does not.</summary>
        /// <param name="stack">the tag structure stack</param>
        /// <param name="currentBmc">the current BMC</param>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to check
        /// </param>
        internal virtual void CheckLogicalStructureInBMC(Stack<Tuple2<PdfName, PdfDictionary>> stack, Tuple2<PdfName
            , PdfDictionary> currentBmc, PdfDocument document) {
            if (stack.IsEmpty()) {
                return;
            }
            bool isRealContent = IsRealContent(currentBmc, document);
            bool isArtifact = PdfName.Artifact.Equals(currentBmc.GetFirst());
            if (isArtifact && IsInsideRealContent(stack, document)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.ARTIFACT_CANT_BE_INSIDE_REAL_CONTENT);
            }
            if (isRealContent && IsInsideArtifact(stack)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.REAL_CONTENT_CANT_BE_INSIDE_ARTIFACT);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks if content is neither marked as Artifact nor tagged as real content.</summary>
        /// <param name="tagStack">tag structure stack</param>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to check
        /// </param>
        internal virtual void CheckContentInCanvas(Stack<Tuple2<PdfName, PdfDictionary>> tagStack, PdfDocument document
            ) {
            if (tagStack.IsEmpty()) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                    );
            }
            bool insideRealContent = IsInsideRealContent(tagStack, document);
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Checks that font programs for all fonts used for rendering within a conforming file, as determined by whether at
        /// least one of its glyphs is referenced from one or more content streams, are embedded within that file, as defined
        /// in ISO 32000-2:2020, 9.9 and ISO 32000-1:2008, 9.9.
        /// </summary>
        /// <remarks>
        /// Checks that font programs for all fonts used for rendering within a conforming file, as determined by whether at
        /// least one of its glyphs is referenced from one or more content streams, are embedded within that file, as defined
        /// in ISO 32000-2:2020, 9.9 and ISO 32000-1:2008, 9.9.
        /// <para />
        /// Checks character encodings rules as defined in ISO 14289-2, 8.4.5.7 and ISO 14289-1, 7.21.6.
        /// </remarks>
        /// <param name="fontsInDocument">collection of fonts used in the document</param>
        internal virtual void CheckFonts(ICollection<PdfFont> fontsInDocument) {
            ICollection<String> fontNamesThatAreNotEmbedded = new HashSet<String>();
            foreach (PdfFont font in fontsInDocument) {
                if (!font.IsEmbedded()) {
                    fontNamesThatAreNotEmbedded.Add(font.GetFontProgram().GetFontNames().GetFontName());
                    continue;
                }
                if (font is PdfTrueTypeFont) {
                    PdfTrueTypeFont trueTypeFont = (PdfTrueTypeFont)font;
                    int flags = trueTypeFont.GetFontProgram().GetPdfFontFlags();
                    bool symbolic = PdfCheckersUtil.CheckFlag(flags, FontDescriptorFlags.SYMBOLIC) && !PdfCheckersUtil.CheckFlag
                        (flags, FontDescriptorFlags.NONSYMBOLIC);
                    if (symbolic) {
                        CheckSymbolicTrueTypeFont(trueTypeFont);
                    }
                    else {
                        CheckNonSymbolicTrueTypeFont(trueTypeFont);
                    }
                }
            }
            if (!fontNamesThatAreNotEmbedded.IsEmpty()) {
                throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.FONT_SHOULD_BE_EMBEDDED
                    , String.Join(", ", fontNamesThatAreNotEmbedded)));
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks cmap entries present in the embedded TrueType font program of the non-symbolic TrueType font.
        ///     </summary>
        /// <param name="fontProgram">the embedded TrueType font program to check</param>
        internal abstract void CheckNonSymbolicCmapSubtable(TrueTypeFont fontProgram);
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks cmap entries present in the embedded TrueType font program of the symbolic TrueType font.</summary>
        /// <param name="fontProgram">the embedded TrueType font program to check</param>
        internal abstract void CheckSymbolicCmapSubtable(TrueTypeFont fontProgram);
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks that embedded fonts define all glyphs referenced for rendering within the conforming file.
        ///     </summary>
        /// <param name="str">the text to check</param>
        /// <param name="font">the font to check</param>
        internal virtual void CheckText(String str, PdfFont font) {
            int index = FontCheckUtil.CheckGlyphsOfText(str, font, new PdfUAChecker.UaCharacterChecker());
            if (index != -1) {
                throw new PdfUAConformanceException(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.GLYPH_IS_NOT_DEFINED_OR_WITHOUT_UNICODE
                    , str[index]));
            }
        }
//\endcond

        private static void CheckOCGNameAndASKey(PdfDictionary dict) {
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

        private static bool IsInsideArtifact(Stack<Tuple2<PdfName, PdfDictionary>> tagStack) {
            foreach (Tuple2<PdfName, PdfDictionary> tag in tagStack) {
                if (PdfName.Artifact.Equals(tag.GetFirst())) {
                    return true;
                }
            }
            return false;
        }

        private static bool IsInsideRealContent(Stack<Tuple2<PdfName, PdfDictionary>> tagStack, PdfDocument document
            ) {
            foreach (Tuple2<PdfName, PdfDictionary> tag in tagStack) {
                if (IsRealContent(tag, document)) {
                    return true;
                }
            }
            return false;
        }

        private static bool IsRealContent(Tuple2<PdfName, PdfDictionary> tag, PdfDocument document) {
            if (PdfName.Artifact.Equals(tag.GetFirst())) {
                return false;
            }
            PdfDictionary properties = tag.GetSecond();
            if (properties == null || !properties.ContainsKey(PdfName.MCID)) {
                return false;
            }
            PdfMcr mcr = McrExists(document, (int)properties.GetAsInt(PdfName.MCID));
            if (mcr == null) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.CONTENT_WITH_MCID_BUT_MCID_NOT_FOUND_IN_STRUCT_TREE_ROOT
                    );
            }
            return true;
        }

        private static PdfMcr McrExists(PdfDocument document, int mcid) {
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

        private void CheckNonSymbolicTrueTypeFont(PdfTrueTypeFont trueTypeFont) {
            TrueTypeFont fontProgram = (TrueTypeFont)trueTypeFont.GetFontProgram();
            CheckNonSymbolicCmapSubtable(fontProgram);
            String encoding = trueTypeFont.GetFontEncoding().GetBaseEncoding();
            // Non-symbolic TTF will always have the dictionary value in the Encoding key of the Font dictionary in itext.
            if (!PdfEncodings.WINANSI.Equals(encoding) && !PdfEncodings.MACROMAN.Equals(encoding)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.NON_SYMBOLIC_TTF_SHALL_SPECIFY_MAC_ROMAN_OR_WIN_ANSI_ENCODING
                    );
            }
            if (trueTypeFont.GetFontEncoding().HasDifferences() && !fontProgram.IsCmapPresent(3, 1)) {
                // If font has differences array, itext ensures that all the glyph names in the Differences array are listed
                // in the Adobe Glyph List.
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.NON_SYMBOLIC_TTF_SHALL_NOT_DEFINE_DIFFERENCES
                    );
            }
        }

        private void CheckSymbolicTrueTypeFont(PdfTrueTypeFont trueTypeFont) {
            if (trueTypeFont.GetPdfObject().ContainsKey(PdfName.Encoding)) {
                throw new PdfUAConformanceException(PdfUAExceptionMessageConstants.SYMBOLIC_TTF_SHALL_NOT_CONTAIN_ENCODING
                    );
            }
            TrueTypeFont fontProgram = (TrueTypeFont)trueTypeFont.GetFontProgram();
            CheckSymbolicCmapSubtable(fontProgram);
        }

        private sealed class UaCharacterChecker : FontCheckUtil.CharacterChecker {
            /// <summary>
            /// Creates new
            /// <see cref="UaCharacterChecker"/>
            /// instance.
            /// </summary>
            public UaCharacterChecker() {
            }

            // Empty constructor.
            public bool Check(int ch, PdfFont font) {
                if (font.ContainsGlyph(ch)) {
                    return !font.GetGlyph(ch).HasValidUnicode();
                }
                else {
                    return true;
                }
            }
        }

        public abstract bool IsPdfObjectReadyToFlush(PdfObject arg1);

        public abstract void Validate(IValidationContext arg1);
    }
}
