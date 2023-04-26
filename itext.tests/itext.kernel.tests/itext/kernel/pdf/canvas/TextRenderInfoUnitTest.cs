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
using iText.Commons.Utils;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    [NUnit.Framework.Category("UnitTest")]
    public class TextRenderInfoUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TextExtractGlyphAverageWidth() {
            String text = "i";
            PdfDictionary fontDict = InitFontDictWithNotSpecifiedWidthOfSpaceChar();
            CanvasGraphicsState gs = new CanvasGraphicsState();
            gs.SetFont(PdfFontFactory.CreateFont(fontDict));
            gs.SetFontSize(12);
            TextRenderInfo testTRI = new TextRenderInfo(new PdfString(text), gs, new Matrix(), new Stack<CanvasTag>());
            float singleSpaceWidth = testTRI.GetSingleSpaceWidth();
            float iWidth = testTRI.GetUnscaledWidth();
            NUnit.Framework.Assert.IsTrue(iWidth < singleSpaceWidth);
            NUnit.Framework.Assert.AreEqual(6.671999931335449, singleSpaceWidth, 0);
        }

        private static PdfDictionary InitFontDictWithNotSpecifiedWidthOfSpaceChar() {
            PdfDictionary fontDict = new PdfDictionary();
            // Initializing all the usual font dictionary entries to make this font more "normal".
            // The entries below should not affect the test.
            fontDict.Put(PdfName.BaseFont, new PdfName("GIXYEW+CMMI10"));
            fontDict.Put(PdfName.Type, PdfName.Font);
            fontDict.Put(PdfName.LastChar, new PdfNumber(122));
            fontDict.Put(PdfName.Subtype, PdfName.Type1);
            // /ToUnicode is omitted for the sake of test brevity.
            // Normally it should be in the font however it's not needed in this test and iText seems to process such font normally.
            // fontDict.put(PdfName.ToUnicode, ...);
            PdfDictionary fontDescriptor = new PdfDictionary();
            fontDescriptor.Put(PdfName.Ascent, new PdfNumber(694));
            fontDescriptor.Put(PdfName.CapHeight, new PdfNumber(683));
            fontDescriptor.Put(PdfName.Descent, new PdfNumber(-194));
            fontDescriptor.Put(PdfName.Flags, new PdfNumber(4));
            fontDescriptor.Put(PdfName.FontBBox, new PdfArray(JavaUtil.ArraysAsList(new PdfObject[] { new PdfNumber(-32
                ), new PdfNumber(-250), new PdfNumber(1048), new PdfNumber(750) })));
            // normally /FontFile should be in the font, however the situation here is the same as for /ToUnicode.
            // fontDescriptor.put(PdfName.FontFile, ...);
            fontDescriptor.Put(PdfName.FontName, new PdfName("GIXYEW+CMMI10"));
            fontDescriptor.Put(PdfName.ItalicAngle, new PdfNumber(-14));
            fontDescriptor.Put(PdfName.StemV, new PdfNumber(72));
            fontDescriptor.Put(PdfName.Type, PdfName.FontDescriptor);
            fontDescriptor.Put(PdfName.XHeight, new PdfNumber(431));
            fontDict.Put(PdfName.FontDescriptor, fontDescriptor);
            // Specifying widths and /FirstChar. /MissingWidth is not specified. Here the result is that font does not specify
            // whitespace character width.
            fontDict.Put(PdfName.FirstChar, new PdfNumber(82));
            String widths = "759 613 584 682 583 944 828 580 682 388 388 388 1000 1000 416 528" + " 429 432 520 465 489 477 576 344 411 520 298 878 600 484 503 446 451 468"
                 + " 361 572 484 715 571 490 465";
            PdfArray widthsArray = new PdfArray();
            foreach (String w in iText.Commons.Utils.StringUtil.Split(widths, " ")) {
                widthsArray.Add(new PdfNumber(Convert.ToInt32(w, System.Globalization.CultureInfo.InvariantCulture)));
            }
            fontDict.Put(PdfName.Widths, widthsArray);
            return fontDict;
        }
    }
}
