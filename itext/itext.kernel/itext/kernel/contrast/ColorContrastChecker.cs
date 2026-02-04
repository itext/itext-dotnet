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
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Validation;
using iText.Kernel.Validation.Context;

namespace iText.Kernel.Contrast {
    /// <summary>
    /// A validation checker that analyzes color contrast in PDF documents to ensure compliance
    /// with Web Content Accessibility Guidelines (WCAG) standards.
    /// </summary>
    /// <remarks>
    /// A validation checker that analyzes color contrast in PDF documents to ensure compliance
    /// with Web Content Accessibility Guidelines (WCAG) standards.
    /// <para />
    /// This checker validates the contrast ratio between text and background colors to ensure
    /// readability for users with visual impairments. It supports both WCAG 2.0 Level AA and
    /// Level AAA conformance levels.
    /// <para />
    /// Features: @see
    /// <see cref="ContrastAnalyzer"/>
    /// for details.
    /// <para />
    /// Current Limitations @see
    /// <see cref="ContrastAnalyzer"/>
    /// for details.
    /// </remarks>
    public class ColorContrastChecker : IValidationChecker {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Contrast.ColorContrastChecker
            ));

        /// <summary>Flag indicating whether to analyze contrast at the individual glyph level.</summary>
        /// <remarks>
        /// Flag indicating whether to analyze contrast at the individual glyph level.
        /// When true, each glyph is analyzed separately for precise contrast checking.
        /// </remarks>
        private readonly bool checkIndividualGlyphs;

        /// <summary>Flag indicating whether to throw an exception when contrast requirements are not met.</summary>
        /// <remarks>
        /// Flag indicating whether to throw an exception when contrast requirements are not met.
        /// When false, warnings are logged instead.
        /// </remarks>
        private readonly bool throwExceptionOnFailure;

        private double minimalPercentualCoverage = 0.1;

        /// <summary>Flag indicating whether to check for WCAG AA compliance.</summary>
        /// <remarks>
        /// Flag indicating whether to check for WCAG AA compliance.
        /// WCAG AA requires a contrast ratio of at least 4.5:1 for normal text and 3:1 for large text.
        /// </remarks>
        private bool checkWcagAA;

        /// <summary>Flag indicating whether to check for WCAG AAA compliance.</summary>
        /// <remarks>
        /// Flag indicating whether to check for WCAG AAA compliance.
        /// WCAG AAA requires a contrast ratio of at least 7:1 for normal text and 4.5:1 for large text.
        /// </remarks>
        private bool checkWcagAAA;

        /// <summary>Creates a new ColorContrastChecker with the specified configuration.</summary>
        /// <param name="checkIndividualGlyphs">
        /// if
        /// <see langword="true"/>
        /// , contrast is checked at the individual glyph level;
        /// if
        /// <see langword="false"/>
        /// , contrast is checked at the text block level.
        /// Individual glyph checking is more precise but may impact performance.
        /// </param>
        /// <param name="throwExceptionOnFailure">
        /// if
        /// <see langword="true"/>
        /// , a
        /// <see cref="iText.Kernel.Exceptions.PdfException"/>
        /// is thrown when contrast
        /// requirements are not met; if
        /// <see langword="false"/>
        /// , warnings are logged instead.
        /// </param>
        public ColorContrastChecker(bool checkIndividualGlyphs, bool throwExceptionOnFailure) {
            this.checkIndividualGlyphs = checkIndividualGlyphs;
            this.throwExceptionOnFailure = throwExceptionOnFailure;
            SetCheckWcagAA(true);
            SetCheckWcagAAA(true);
            SetMinimalPercentualCoverage(0.1);
        }

        /// <summary>
        /// Sets the minimal percentual coverage of text area that must be covered by a background
        /// element for its contrast ratio to be considered in the analysis.
        /// </summary>
        /// <remarks>
        /// Sets the minimal percentual coverage of text area that must be covered by a background
        /// element for its contrast ratio to be considered in the analysis.
        /// <para />
        /// For example, if set to 0.1 (10%), only background elements that cover at least 10% of the
        /// text area will be included in the contrast analysis. This helps filter out insignificant backgrounds
        /// that do not meaningfully affect text readability. Like underlines or small decorative elements.
        /// </remarks>
        /// <param name="minimalPercentualCoverage">the minimal percentual coverage (between 0.0 and 1.0)</param>
        public void SetMinimalPercentualCoverage(double minimalPercentualCoverage) {
            if (minimalPercentualCoverage < 0.0 || minimalPercentualCoverage > 1.0) {
                throw new ArgumentException("Minimal percentual coverage must be a value between 0.0 and 1.0");
            }
            this.minimalPercentualCoverage = minimalPercentualCoverage;
        }

        /// <summary>Sets whether to check for WCAG AA compliance.</summary>
        /// <remarks>
        /// Sets whether to check for WCAG AA compliance.
        /// WCAG AA requires a contrast ratio of at least 4.5:1 for normal text
        /// and 3:1 for large text (18pt+ or 14pt+ bold).
        /// </remarks>
        /// <param name="checkWcagAA">true to enable WCAG AA compliance checking, false to disable</param>
        /// <returns>this ColorContrastChecker instance for method chaining</returns>
        public iText.Kernel.Contrast.ColorContrastChecker SetCheckWcagAA(bool checkWcagAA) {
            this.checkWcagAA = checkWcagAA;
            LogWarningIfBothChecksDisabled();
            return this;
        }

        /// <summary>Sets whether to check for WCAG AAA compliance.</summary>
        /// <remarks>
        /// Sets whether to check for WCAG AAA compliance.
        /// WCAG AAA requires a contrast ratio of at least 7:1 for normal text
        /// and 4.5:1 for large text (18pt+ or 14pt+ bold).
        /// </remarks>
        /// <param name="checkWcagAAA">true to enable WCAG AAA compliance checking, false to disable</param>
        /// <returns>this ColorContrastChecker instance for method chaining</returns>
        public iText.Kernel.Contrast.ColorContrastChecker SetCheckWcagAAA(bool checkWcagAAA) {
            this.checkWcagAAA = checkWcagAAA;
            LogWarningIfBothChecksDisabled();
            return this;
        }

        /// <summary>Validates the given context for color contrast compliance.</summary>
        /// <remarks>
        /// Validates the given context for color contrast compliance.
        /// <para />
        /// This method is called by the validation framework to check color contrast
        /// when a PDF page is being validated. It only processes validation contexts
        /// of type
        /// <see cref="iText.Kernel.Validation.ValidationType.PDF_PAGE"/>.
        /// </remarks>
        /// <param name="validationContext">the validation context containing the PDF page to validate</param>
        public virtual void Validate(IValidationContext validationContext) {
            if (validationContext.GetType() == ValidationType.PDF_PAGE) {
                PdfPageValidationContext pageContext = (PdfPageValidationContext)validationContext;
                CheckContrast(pageContext.GetPage());
            }
        }

        /// <summary>Determines if a PDF object is ready to be flushed to the output stream.</summary>
        /// <remarks>
        /// Determines if a PDF object is ready to be flushed to the output stream.
        /// <para />
        /// This implementation always returns true as color contrast checking does not
        /// impose any restrictions on when objects can be flushed.
        /// </remarks>
        /// <param name="object">the PDF object to check</param>
        /// <returns>
        /// always
        /// <see langword="true"/>
        /// </returns>
        public virtual bool IsPdfObjectReadyToFlush(PdfObject @object) {
            return true;
        }

        /// <summary>Logs a warning if both WCAG AA and AAA compliance checks are disabled.</summary>
        /// <remarks>
        /// Logs a warning if both WCAG AA and AAA compliance checks are disabled.
        /// This helps alert users that no contrast validation will be performed.
        /// </remarks>
        private void LogWarningIfBothChecksDisabled() {
            if (!checkWcagAA && !checkWcagAAA) {
                LOGGER.LogWarning(KernelLogMessageConstant.BOTH_WCAG_AA_AND_AAA_COMPLIANCE_CHECKS_DISABLED);
            }
        }

        /// <summary>Performs color contrast analysis on the specified PDF page.</summary>
        /// <remarks>
        /// Performs color contrast analysis on the specified PDF page.
        /// <para />
        /// This method analyzes all text on the page and checks if it meets the enabled
        /// WCAG compliance levels (AA and/or AAA). For each non-compliant text element,
        /// it either throws a
        /// <see cref="iText.Kernel.Exceptions.PdfException"/>
        /// or logs a warning, depending on the
        /// configuration.
        /// <para />
        /// The method skips processing entirely if both WCAG AA and AAA checks are disabled.
        /// </remarks>
        /// <param name="page">the PDF page to analyze for color contrast compliance</param>
        private void CheckContrast(PdfPage page) {
            if (!checkWcagAA && !checkWcagAAA) {
                // No checks enabled, skip processing
                return;
            }
            IList<ContrastResult> contrastResults = new ContrastAnalyzer(checkIndividualGlyphs).CheckPageContrast(page
                );
            foreach (ContrastResult contrastResult in contrastResults) {
                TextColorInfo textContrastInformation = contrastResult.GetTextRenderInfo();
                foreach (ContrastResult.OverlappingArea overlappingArea in contrastResult.GetOverlappingAreas()) {
                    if (overlappingArea.GetOverlapRatio() < minimalPercentualCoverage) {
                        continue;
                    }
                    // Only check compliance levels that are enabled
                    bool isCompliantAAA = !checkWcagAAA || WCagChecker.IsTextWcagAAACompliant(textContrastInformation.GetFontSize
                        (), overlappingArea.GetContrastRatio());
                    bool isCompliantAA = isCompliantAAA && (!checkWcagAA || WCagChecker.IsTextWcagAACompliant(textContrastInformation
                        .GetFontSize(), overlappingArea.GetContrastRatio()));
                    // Report only if at least one enabled check fails
                    if (!isCompliantAA || !isCompliantAAA) {
                        String message = GenerateMessage(isCompliantAAA, isCompliantAA, contrastResult, overlappingArea.GetContrastRatio
                            ());
                        if (this.throwExceptionOnFailure) {
                            message = "Color contrast check failed: " + message;
                            throw new PdfException(message);
                        }
                        else {
                            LOGGER.LogWarning(message);
                        }
                    }
                }
            }
        }

        private String GenerateMessage(bool isCompliantAAA, bool isCompliantAA, ContrastResult contrastResult, double
             contrastRatio) {
            TextColorInfo textContrastInformation = contrastResult.GetTextRenderInfo();
            StringBuilder message = new StringBuilder();
            message.Append("Page ").Append(contrastResult.GetPageNumber()).Append(": ");
            if (textContrastInformation.GetText() != null) {
                message.Append("Text: '");
                message.Append(textContrastInformation.GetText());
                message.Append("', ");
            }
            if (textContrastInformation.GetParent() != null) {
                message.Append(" parent text: '");
                message.Append(textContrastInformation.GetParent());
                message.Append("' ");
            }
            message.Append("with font size: ").Append(contrastResult.GetTextRenderInfo().GetFontSize()).Append(" pt ");
            message.Append("has contrast ratio: ").Append(FormatFloatWithoutStringFormat(contrastRatio)).Append(". ");
            if (checkWcagAA && !isCompliantAA) {
                message.Append("It is not WCAG AA compliant. ");
            }
            if (checkWcagAAA && !isCompliantAAA) {
                message.Append("It is not WCAG AAA compliant. ");
            }
            return message.ToString();
        }

        private String FormatFloatWithoutStringFormat(double value) {
            //2 decimal places
            long intValue = (long)value;
            long decimalValue = (long)MathematicUtil.Round((value - intValue) * 100);
            if (decimalValue < 10) {
                return intValue + "." + "0" + decimalValue;
            }
            return intValue + "." + decimalValue;
        }
    }
}
