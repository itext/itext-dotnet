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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PageResizerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ResizeAppearanceStreamsNullAPTest() {
            PdfAnnotation annotation = new PdfInkAnnotation(new Rectangle(50.0f, 50.0f));
            PageResizer.ResizeAppearanceStreams(annotation, null);
            NUnit.Framework.Assert.IsNull(annotation.GetAppearanceDictionary());
        }

        [NUnit.Framework.Test]
        public virtual void ScalePageBoxNullPageSizeTest() {
            Rectangle originalPageSize = null;
            PageSize newPageSize = new PageSize(25.0f, 25.0f);
            Rectangle box = new Rectangle(10.0f, 10.0f);
            Rectangle scaled = PageResizer.ScalePageBox(originalPageSize, newPageSize, box);
            NUnit.Framework.Assert.AreEqual(box, scaled);
        }

        [NUnit.Framework.Test]
        public virtual void ScalePageBoxNullNewPageSizeTest() {
            Rectangle originalPageSize = new Rectangle(50.0f, 50.0f);
            PageSize newPageSize = null;
            Rectangle box = new Rectangle(10.0f, 10.0f);
            Rectangle scaled = PageResizer.ScalePageBox(originalPageSize, newPageSize, box);
            NUnit.Framework.Assert.AreEqual(box, scaled);
        }

        [NUnit.Framework.Test]
        public virtual void ScalePageBoxNullBoxTest() {
            Rectangle originalPageSize = new Rectangle(50.0f, 50.0f);
            PageSize newPageSize = new PageSize(25.0f, 25.0f);
            Rectangle box = null;
            Rectangle scaled = PageResizer.ScalePageBox(originalPageSize, newPageSize, box);
            NUnit.Framework.Assert.AreEqual(box, scaled);
        }

        [NUnit.Framework.Test]
        public virtual void ScalePageBoxZeroHeightTest() {
            Rectangle originalPageSize = new Rectangle(50.0f, 0.0f);
            PageSize newPageSize = new PageSize(25.0f, 25.0f);
            Rectangle box = new Rectangle(10.0f, 10.0f);
            Rectangle scaled = PageResizer.ScalePageBox(originalPageSize, newPageSize, box);
            NUnit.Framework.Assert.AreEqual(box, scaled);
        }

        [NUnit.Framework.Test]
        public virtual void ScalePageBoxZeroWidthTest() {
            Rectangle originalPageSize = new Rectangle(0.0f, 50.0f);
            PageSize newPageSize = new PageSize(25.0f, 25.0f);
            Rectangle box = new Rectangle(10.0f, 10.0f);
            Rectangle scaled = PageResizer.ScalePageBox(originalPageSize, newPageSize, box);
            NUnit.Framework.Assert.AreEqual(box, scaled);
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringSimpleScaleTest() {
            String input = "/Helv 12 Tf";
            double scale = 0.5;
            String expected = "/Helv 6 Tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringMixedOperatorsAndColorTest() {
            String input = "1 0 0 rg /F1 10 Tf 14 TL";
            double scale = 2;
            String expected = "1 0 0 rg /F1 20 Tf 28 TL";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringEdgeNumericFormsTest() {
            NUnit.Framework.Assert.AreEqual("-1 Ts", PageResizer.ScaleDaString("-.5 Ts", 2.0));
            NUnit.Framework.Assert.AreEqual("1 Ts", PageResizer.ScaleDaString(".5 Ts", 2.0));
            NUnit.Framework.Assert.AreEqual("0.5 Tc", PageResizer.ScaleDaString("5.0000 Tc", 0.1));
            NUnit.Framework.Assert.AreEqual("1 TL", PageResizer.ScaleDaString("1e-1 TL", 10.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringMultipleOperatorGroupsTest() {
            String input = "/F1 10 Tf 5 Tc 2.5 Tw 10 TL /F2 20 Tf -2 Ts";
            double scale = 0.5;
            String expected = "/F1 5 Tf 2.5 Tc 1.25 Tw 5 TL /F2 10 Tf -1 Ts";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringNoOpsTest() {
            double scale = 2.0;
            // Operator with no operands should not change.
            NUnit.Framework.Assert.AreEqual("Tf", PageResizer.ScaleDaString("Tf", scale));
            //Operator with non-numeric operand should not change.
            NUnit.Framework.Assert.AreEqual("/F1 Tf", PageResizer.ScaleDaString("/F1 Tf", scale));
            //String with no operators should not change.
            NUnit.Framework.Assert.AreEqual("foo bar baz", PageResizer.ScaleDaString("foo bar baz", scale));
            //Malformed operator sequence should not change unpredictably.
            NUnit.Framework.Assert.AreEqual("/Helv Tf 12", PageResizer.ScaleDaString("/Helv Tf 12", scale));
            //Whitespace-only string should result in empty.
            NUnit.Framework.Assert.AreEqual("", PageResizer.ScaleDaString("", scale));
            //Numbers without operators should not be scaled.
            NUnit.Framework.Assert.AreEqual("1 2 3", PageResizer.ScaleDaString("1 2 3", scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringWhitespaceNormalizationTest() {
            String input = "  /Helv   12 \t Tf  ";
            double scale = 0.5;
            String expected = "/Helv 6 Tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale), "Whitespace should be normalized."
                );
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringWithIdentityScaleTest() {
            String input = "/Helv 12.5 Tf";
            double scale = 1.0;
            String expected = "/Helv 12.5 Tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringIgnoreOtherOperatorsTest() {
            String input = "100 Tz 12 Tf";
            double scale = 2.0;
            String expected = "100 Tz 24 Tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringNullInputTest() {
            NUnit.Framework.Assert.IsNull(PageResizer.ScaleDaString(null, 2.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringOperatorCaseSensitivityTest() {
            String input = "/Helv 12 tf";
            double scale = 2.0;
            String expected = "/Helv 12 tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDaStringSmallResultingValueTest() {
            String input = "0.0001 Tf";
            double scale = 0.1;
            String expected = "0 Tf";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleDaString(input, scale));
        }

        [NUnit.Framework.Test]
        public virtual void ResizePageWithZeroSizeTest() {
            PageResizer resizer = new PageResizer(new PageSize(0.0F, 0.0F), PageResizer.ResizeType.DEFAULT);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => resizer.Resize(null));
            String expectedMessage = MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_RESIZE_PAGE_WITH_NEGATIVE_OR_INFINITE_SCALE
                , new PageSize(0.0F, 0.0F));
            NUnit.Framework.Assert.AreEqual(expectedMessage, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringWithNullOrEmptyReturnsAsIs() {
            NUnit.Framework.Assert.IsNull(PageResizer.ScaleRcString(null, 2.0));
            NUnit.Framework.Assert.AreEqual("", PageResizer.ScaleRcString("", 2.0));
            NUnit.Framework.Assert.AreEqual("   ", PageResizer.ScaleRcString("   ", 2.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringWithIdentityScaleReturnsAsIs() {
            String input = "font-size: 12.5pt; width: 100px;";
            NUnit.Framework.Assert.AreEqual(input, PageResizer.ScaleRcString(input, 1.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringWithNonScalablePropertiesReturnsAsIs() {
            String input = "color: red; font-weight: bold;";
            NUnit.Framework.Assert.AreEqual(input, PageResizer.ScaleRcString(input, 2.0));
            String input2 = "margin: 10pt 5pt; padding: 10px;";
            NUnit.Framework.Assert.AreEqual(input2, PageResizer.ScaleRcString(input2, 2.0), "Shorthand properties should be ignored."
                );
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringScalesValuesWithAbsoluteUnits() {
            String input = "font-size: 12pt;";
            String expected = "font-size: 6pt;";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleRcString(input, 0.5));
            String multiInput = "width: 1in; height: 2pc; margin-left: 3cm; margin-top: 4mm; border-top-width: 5px;";
            String multiExpected = "width: 0.5in; height: 1pc; margin-left: 1.5cm; margin-top: 2mm; border-top-width: 2.5px;";
            NUnit.Framework.Assert.AreEqual(multiExpected, PageResizer.ScaleRcString(multiInput, 0.5));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringWithRelativeOrNoUnitsReturnsAsIs() {
            String input = "font-size: 1.5em; width: 100%; line-height: 150%;";
            NUnit.Framework.Assert.AreEqual(input, PageResizer.ScaleRcString(input, 2.0));
            String noUnitInput = "font-size: 12";
            NUnit.Framework.Assert.AreEqual(noUnitInput, PageResizer.ScaleRcString(noUnitInput, 2.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringHandlesVariousNumberFormats() {
            NUnit.Framework.Assert.AreEqual("font-size: 20pt;", PageResizer.ScaleRcString("font-size: 10pt;", 2.0), "Integer"
                );
            NUnit.Framework.Assert.AreEqual("font-size: 21pt;", PageResizer.ScaleRcString("font-size: 10.5pt;", 2.0), 
                "Decimal");
            NUnit.Framework.Assert.AreEqual("font-size: 1pt;", PageResizer.ScaleRcString("font-size: .5pt;", 2.0), "Leading dot decimal"
                );
            NUnit.Framework.Assert.AreEqual("width: 50px;", PageResizer.ScaleRcString("width: 1e2px;", 0.5), "Scientific notation"
                );
            NUnit.Framework.Assert.AreEqual("margin-left: -20px;", PageResizer.ScaleRcString("margin-left: -10px;", 2.0
                ), "Negative value");
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringHandlesVariedWhitespace() {
            String input = "font-size:  12pt ; width:50px";
            String expected = "font-size:  24pt ; width:100px";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleRcString(input, 2.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringWithZeroScale_scalesToZero() {
            String input = "font-size: 12pt; width: 100px;";
            String expected = "font-size: 0pt; width: 0px;";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleRcString(input, 0.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringHandlesSmallResultingValues() {
            NUnit.Framework.Assert.AreEqual("font-size: 0.0001pt;", PageResizer.ScaleRcString("font-size: 0.001pt;", 0.1
                ));
            NUnit.Framework.Assert.AreEqual("font-size: 0pt;", PageResizer.ScaleRcString("font-size: 0.001pt;", 0.01));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringWithComplexRichTextScalesCorrectly() {
            String rc = "body{font-family:helvetica,sans-serif;font-size:12.0pt;line-height:14.0pt;margin-top:2.0mm;" 
                + "margin-bottom:2.0mm;text-align:left;}p{margin-top:0.0pt;margin-bottom:0.0pt;}";
            String expected = "body{font-family:helvetica,sans-serif;font-size:18pt;line-height:21pt;margin-top:3mm;" 
                + "margin-bottom:3mm;text-align:left;}p{margin-top:0pt;margin-bottom:0pt;}";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleRcString(rc, 1.5));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringDoesNotScaleRelativeUnitsOnScalableProperties() {
            // em, rem, % should not be scaled
            String input = "font-size: 1.5em; line-height: 150%; letter-spacing: 0.5rem;";
            NUnit.Framework.Assert.AreEqual(input, PageResizer.ScaleRcString(input, 2.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringPreservesWhitespaceBetweenNumberAndUnit() {
            String input = "font-size: 12 pt; width:  10  px; height: 5   cm;";
            String expected = "font-size: 24 pt; width:  20  px; height: 10   cm;";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleRcString(input, 2.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringPropertyNameCaseSensitivity_notScaled() {
            // CSS in XHTML is case-sensitive here; property name with different case should not match
            String input = "Font-Size: 12pt; WIDTH: 100px;";
            NUnit.Framework.Assert.AreEqual(input, PageResizer.ScaleRcString(input, 2.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringIgnoresShorthandBorderWidth() {
            String input = "border: 5px;";
            // Shorthand property is not in the scalable list, should remain unchanged
            NUnit.Framework.Assert.AreEqual(input, PageResizer.ScaleRcString(input, 3.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringScientificNotationNegativeExponent() {
            NUnit.Framework.Assert.AreEqual("width: 0.1in;", PageResizer.ScaleRcString("width: 1e-2in;", 10.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringMixedAbsoluteAndRelativeValues() {
            String input = "font-size: 10pt; line-height: 120%; width: 2cm;";
            String expected = "font-size: 20pt; line-height: 120%; width: 4cm;";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleRcString(input, 2.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcAcrobatExample() {
            String input = "<?xml version=\"1.0\"?><body xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:xfa=\"http://www.xfa.org/schema/xfa-data/1.0/\" xfa:spec=\"2.0.2\"><p style=\"font-size:12pt;padding-left:5pt;padding-top:2pt;margin-bottom:6pt;line-height:15pt;text-align:left;color:#000000;\">This is the first paragraph. It has a specific<span style=\"font-weight:bold; color:#FF0000;\">font size and line height.</span></p><p style=\"font-size:12pt;margin-top:0pt;text-indent:24pt;color:#333333;\">This is the second paragraph. Notice the <b>text-indent</b> property, which creates the indentation.</p></body>";
            String expected = "<?xml version=\"1.0\"?><body xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:xfa=\"http://www.xfa.org/schema/xfa-data/1.0/\" xfa:spec=\"2.0.2\"><p style=\"font-size:6pt;padding-left:2.5pt;padding-top:1pt;margin-bottom:3pt;line-height:7.5pt;text-align:left;color:#000000;\">This is the first paragraph. It has a specific<span style=\"font-weight:bold; color:#FF0000;\">font size and line height.</span></p><p style=\"font-size:6pt;margin-top:0pt;text-indent:12pt;color:#333333;\">This is the second paragraph. Notice the <b>text-indent</b> property, which creates the indentation.</p></body>";
            NUnit.Framework.Assert.AreEqual(expected, PageResizer.ScaleRcString(input, 0.5));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringPreservesWhitespaceBetweenPropertyAndValue() {
            // Test various whitespace patterns are preserved
            String input1 = "font-size:  12pt;";
            String expected1 = "font-size:  24pt;";
            NUnit.Framework.Assert.AreEqual(expected1, PageResizer.ScaleRcString(input1, 2.0), "Multiple spaces after colon should be preserved"
                );
            String input2 = "font-size:\t10pt;";
            String expected2 = "font-size:\t20pt;";
            NUnit.Framework.Assert.AreEqual(expected2, PageResizer.ScaleRcString(input2, 2.0), "Tab after colon should be preserved"
                );
            String input3 = "font-size: 10pt; width:  20px;";
            String expected3 = "font-size: 20pt; width:  40px;";
            NUnit.Framework.Assert.AreEqual(expected3, PageResizer.ScaleRcString(input3, 2.0), "Different whitespace patterns should be preserved independently"
                );
            String input4 = "font-size:10pt;";
            String expected4 = "font-size:20pt;";
            NUnit.Framework.Assert.AreEqual(expected4, PageResizer.ScaleRcString(input4, 2.0), "No space after colon should be preserved"
                );
            String input5 = "margin-left: 5 pt;";
            String expected5 = "margin-left: 10 pt;";
            NUnit.Framework.Assert.AreEqual(expected5, PageResizer.ScaleRcString(input5, 2.0), "Space between number and unit should be preserved"
                );
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringWordBoundaryCheck() {
            String input1 = "font-size-large: 10pt;";
            NUnit.Framework.Assert.AreEqual(input1, PageResizer.ScaleRcString(input1, 2.0), "Property 'font-size-large' should not be treated as 'font-size'"
                );
            String input2 = "width2: 20px;";
            NUnit.Framework.Assert.AreEqual(input2, PageResizer.ScaleRcString(input2, 2.0), "Property 'width2' should not be treated as 'width'"
                );
            String input3 = "my-height: 30pt;";
            NUnit.Framework.Assert.AreEqual(input3, PageResizer.ScaleRcString(input3, 2.0), "Property 'my-height' should not be treated as 'height'"
                );
            String input4 = "margin-top-extra: 15px;";
            NUnit.Framework.Assert.AreEqual(input4, PageResizer.ScaleRcString(input4, 2.0), "Property 'margin-top-extra' should not be treated as 'margin-top'"
                );
            String input5 = "font-size: 12pt; custom-font-size: 10pt;";
            String expected5 = "font-size: 24pt; custom-font-size: 10pt;";
            NUnit.Framework.Assert.AreEqual(expected5, PageResizer.ScaleRcString(input5, 2.0), "Valid 'font-size' should be scaled, but 'custom-font-size' should not"
                );
            String input6 = "width: 100px";
            String expected6 = "width: 200px";
            NUnit.Framework.Assert.AreEqual(expected6, PageResizer.ScaleRcString(input6, 2.0), "Property 'width' at end of string should be scaled"
                );
            String input7 = "font-size: 10pt; font-size-custom: 20pt; height: 50px; height123: 30px;";
            String expected7 = "font-size: 20pt; font-size-custom: 20pt; height: 100px; height123: 30px;";
            NUnit.Framework.Assert.AreEqual(expected7, PageResizer.ScaleRcString(input7, 2.0), "Only exact property name matches should be scaled"
                );
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringWithMalformedNumberReturnsAsIs() {
            // A single dot is not a valid number.
            String input1 = "font-size: .pt;";
            NUnit.Framework.Assert.AreEqual(input1, PageResizer.ScaleRcString(input1, 2.0));
            // A single sign is not a valid number.
            String input2 = "font-size: +pt;";
            NUnit.Framework.Assert.AreEqual(input2, PageResizer.ScaleRcString(input2, 2.0));
            // A dot after a number and sign is not valid.
            String input3 = "font-size: -.pt;";
            NUnit.Framework.Assert.AreEqual(input3, PageResizer.ScaleRcString(input3, 2.0));
            // Exponent without mantissa
            String input4 = "font-size: E1pt;";
            NUnit.Framework.Assert.AreEqual(input4, PageResizer.ScaleRcString(input4, 2.0));
            // Number with exponent but no digits in exponent
            String input5 = "font-size: 1Ept;";
            NUnit.Framework.Assert.AreEqual(input5, PageResizer.ScaleRcString(input5, 2.0));
        }

        [NUnit.Framework.Test]
        public virtual void ScaleRcStringIgnoresComments() {
            String htmlComment = "font-size: 20pt; <!-- font-size: 10pt; -->";
            String expectedHtml = "font-size: 40pt; <!-- font-size: 20pt; -->";
            NUnit.Framework.Assert.AreEqual(expectedHtml, PageResizer.ScaleRcString(htmlComment, 2.0));
            String blockComment = "font-size: 20pt; /** font-size: 10pt; **/";
            String expectedBlock = "font-size: 40pt; /** font-size: 20pt; **/";
            NUnit.Framework.Assert.AreEqual(expectedBlock, PageResizer.ScaleRcString(blockComment, 2.0));
            String combined = "<!-- width: 50px; --> width: 100px; /** height: 40pt; */ height: 80pt;";
            String expectedCombined = "<!-- width: 100px; --> width: 200px; /** height: 80pt; */ height: 160pt;";
            NUnit.Framework.Assert.AreEqual(expectedCombined, PageResizer.ScaleRcString(combined, 2.0));
        }
    }
}
