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
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Validation;
using iText.Kernel.Validation.Context;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Contrast {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ColorContrastCheckerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestSetMinimalPercentualCoverageValid() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                checker.SetMinimalPercentualCoverage(0.5);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestSetMinimalPercentualCoverageZero() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                checker.SetMinimalPercentualCoverage(0.0);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestSetMinimalPercentualCoverageOne() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                checker.SetMinimalPercentualCoverage(1.0);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestSetMinimalPercentualCoverageNegative() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => checker.SetMinimalPercentualCoverage
                (-0.1));
            NUnit.Framework.Assert.AreEqual("Minimal percentual coverage must be a value between 0.0 and 1.0", exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestSetMinimalPercentualCoverageGreaterThanOne() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => checker.SetMinimalPercentualCoverage
                (1.1));
            NUnit.Framework.Assert.AreEqual("Minimal percentual coverage must be a value between 0.0 and 1.0", exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestSetCheckWcagAATrue() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            ColorContrastChecker result = checker.SetCheckWcagAA(true);
            NUnit.Framework.Assert.AreSame(checker, result);
        }

        [NUnit.Framework.Test]
        public virtual void TestSetCheckWcagAAFalse() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            ColorContrastChecker result = checker.SetCheckWcagAA(false);
            NUnit.Framework.Assert.AreSame(checker, result);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.BOTH_WCAG_AA_AND_AAA_COMPLIANCE_CHECKS_DISABLED, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void TestSetCheckWcagAAFalseLogsWarningWhenBothDisabled() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                checker.SetCheckWcagAAA(false);
                checker.SetCheckWcagAA(false);
            }
            );
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.BOTH_WCAG_AA_AND_AAA_COMPLIANCE_CHECKS_DISABLED, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void TestSetCheckWcagAAAFalseLogsWarningWhenBothDisabled() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                checker.SetCheckWcagAAA(false);
                checker.SetCheckWcagAA(false);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestIsPdfObjectReadyToFlush() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            NUnit.Framework.Assert.IsTrue(checker.IsPdfObjectReadyToFlush(null));
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithNonPdfPageContext() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            IValidationContext context = new _IValidationContext_142();
            NUnit.Framework.Assert.DoesNotThrow(() => {
                checker.Validate(context);
            }
            );
        }

        private sealed class _IValidationContext_142 : IValidationContext {
            public _IValidationContext_142() {
            }

            public ValidationType GetType() {
                return ValidationType.PDF_DOCUMENT;
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithCompliantBlackTextOnWhiteBackground() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception - black on white is compliant
            NUnit.Framework.Assert.DoesNotThrow(() => {
                checker.Validate(context);
            }
            );
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithNonCompliantTextThrowsException() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Create low contrast: light gray text on white background
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetColor(ColorConstants.LIGHT_GRAY, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, true);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => checker.Validate(context));
            NUnit.Framework.Assert.IsTrue(exception.Message.Contains("Color contrast check failed"));
            NUnit.Framework.Assert.IsTrue(exception.Message.Contains("Page 1"));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage("Page 1: Text: 'T', ", LogLevel = LogLevelConstants.WARN)]
        public virtual void TestValidateWithNonCompliantTextLogsWarning() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Create low contrast: light gray text on white background
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetColor(ColorConstants.LIGHT_GRAY, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("T");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(true, false);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should log warning but not throw exception
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithWcagAAOnlyEnabled() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            checker.SetCheckWcagAAA(false);
            checker.SetCheckWcagAA(true);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithWcagAAAOnlyEnabled() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            checker.SetCheckWcagAA(false);
            checker.SetCheckWcagAAA(true);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.BOTH_WCAG_AA_AND_AAA_COMPLIANCE_CHECKS_DISABLED, LogLevel = LogLevelConstants
            .WARN)]
        public virtual void TestValidateWithBothWcagChecksDisabled() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Create low contrast text
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetColor(ColorConstants.CYAN, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, true);
            checker.SetCheckWcagAA(false);
            checker.SetCheckWcagAAA(false);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception because checks are disabled
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithIndividualGlyphsEnabled() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("ABC");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(true, false);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithLargeText() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Large text (20pt) has different WCAG requirements
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 20);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithTextOnColoredBackground() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Draw colored background
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.BLUE);
            canvas.Rectangle(50, 50, 200, 100);
            canvas.Fill();
            // Draw white text on blue background
            canvas.BeginText();
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception - white on blue should be compliant
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithMinimalCoverageFiltering() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            checker.SetMinimalPercentualCoverage(0.9);
            // Very high threshold
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestMethodChaining() {
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            ColorContrastChecker result = checker.SetCheckWcagAA(true).SetCheckWcagAAA(false);
            NUnit.Framework.Assert.AreSame(checker, result);
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithTextWithoutParent() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Create text without parent context
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("X");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(true, false);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithAACompliantButNotAAA() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Create a scenario where AA passes but AAA fails
            // Using gray on white which has ~4.5:1 ratio
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.LIGHT_GRAY);
            canvas.Rectangle(50, 50, 200, 100);
            canvas.Fill();
            canvas.BeginText();
            canvas.SetColor(ColorConstants.BLACK, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 80);
            canvas.ShowText("Test");
            canvas.EndText();
            // Enable only AAA check
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            checker.SetCheckWcagAA(false);
            checker.SetCheckWcagAAA(true);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception with logging mode
            NUnit.Framework.Assert.DoesNotThrow(() => {
                checker.Validate(context);
            }
            );
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithMultipleBackgrounds() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Draw multiple overlapping backgrounds
            PdfCanvas canvas = new PdfCanvas(page);
            // First background
            canvas.SetFillColor(ColorConstants.RED);
            canvas.Rectangle(50, 50, 150, 100);
            canvas.Fill();
            // Second overlapping background
            canvas.SetFillColor(ColorConstants.BLUE);
            canvas.Rectangle(100, 50, 150, 100);
            canvas.Fill();
            // Draw text over overlapping area
            canvas.BeginText();
            canvas.SetColor(ColorConstants.WHITE, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(120, 80);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithSmallFontAndLowContrast() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Small font with insufficient contrast
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetColor(ColorConstants.LIGHT_GRAY, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 8);
            // Small font
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, true);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should throw exception for low contrast with small font
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => checker.Validate(context));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithLargeFontAndModerateContrast() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Large font (>18pt) has more lenient requirements
            PdfCanvas canvas = new PdfCanvas(page);
            // Background
            canvas.SetFillColor(ColorConstants.LIGHT_GRAY);
            canvas.Rectangle(50, 50, 200, 100);
            canvas.Fill();
            // Large text
            canvas.BeginText();
            canvas.SetColor(ColorConstants.BLACK, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 24);
            canvas.MoveText(100, 80);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw exception - large text has lower requirements
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateWithZeroCoverageThreshold() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            checker.SetMinimalPercentualCoverage(0.0);
            // Include all backgrounds
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            checker.Validate(context);
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateMessageIncludesParentText() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Create low contrast text that will have parent context
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetColor(ColorConstants.LIGHT_GRAY, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(true, true);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => checker.Validate(context));
            System.Console.Out.WriteLine(e.Message);
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("parent text: 'Test'"));
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateAAFailsButAAADisabled() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Create text that fails AA but AAA is disabled
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetColor(ColorConstants.LIGHT_GRAY, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            ColorContrastChecker checker = new ColorContrastChecker(false, true);
            checker.SetCheckWcagAAA(false);
            // Disable AAA
            checker.SetCheckWcagAA(true);
            // Enable AA
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => checker.Validate(context));
            NUnit.Framework.Assert.IsTrue(exception.Message.Contains("WCAG AA compliant"));
            NUnit.Framework.Assert.IsFalse(exception.Message.Contains("WCAG AAA compliant"));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateAAAFailsButAADisabled() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            ValidationContainer container = new ValidationContainer();
            ColorContrastChecker contrastChecker = new ColorContrastChecker(false, true);
            contrastChecker.SetCheckWcagAA(false);
            contrastChecker.SetCheckWcagAAA(true);
            container.AddChecker(contrastChecker);
            pdfDoc.GetDiContainer().Register(typeof(ValidationContainer), container);
            PdfPage page = pdfDoc.AddNewPage();
            // Create text that passes AA but fails AAA
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.LIGHT_GRAY);
            canvas.Rectangle(50, 50, 200, 100);
            canvas.Fill();
            canvas.BeginText();
            canvas.SetColor(ColorConstants.CYAN, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 80);
            canvas.ShowText("Test");
            canvas.EndText();
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc.Close());
            NUnit.Framework.Assert.IsTrue(exception.Message.Contains("WCAG AAA compliant"));
            NUnit.Framework.Assert.IsFalse(exception.Message.Contains("WCAG AA compliant"));
        }

        [NUnit.Framework.Test]
        public virtual void TestValidateBothAAAndAAAFail() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            ValidationContainer container = new ValidationContainer();
            container.AddChecker(new ColorContrastChecker(false, true));
            pdfDoc.GetDiContainer().Register(typeof(ValidationContainer), container);
            PdfPage page = pdfDoc.AddNewPage();
            // Create very low contrast that fails both
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetColor(ColorConstants.LIGHT_GRAY, true);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc.Close());
            NUnit.Framework.Assert.IsTrue(exception.Message.Contains("WCAG AA compliant"));
            NUnit.Framework.Assert.IsTrue(exception.Message.Contains("WCAG AAA compliant"));
        }

        [NUnit.Framework.Test]
        public virtual void TestConstructorSetsDefaultValues() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = pdfDoc.AddNewPage();
            // Create compliant text
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
            canvas.MoveText(100, 100);
            canvas.ShowText("Test");
            canvas.EndText();
            // Constructor should set both WCAG checks to true by default
            ColorContrastChecker checker = new ColorContrastChecker(false, false);
            PdfPageValidationContext context = new PdfPageValidationContext(page);
            // Should not throw - defaults allow compliant text
            checker.Validate(context);
            pdfDoc.Close();
        }
    }
}
