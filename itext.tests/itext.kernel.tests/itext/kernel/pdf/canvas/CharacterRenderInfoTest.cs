/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    [NUnit.Framework.Category("UnitTest")]
    public class CharacterRenderInfoTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void BoundingBoxForRotatedText() {
            TextRenderInfo tri = InitTRI("abc", Math.PI / 2);
            CharacterRenderInfo characterRenderInfo = new CharacterRenderInfo(tri);
            NUnit.Framework.Assert.IsTrue(characterRenderInfo.GetBoundingBox().EqualsWithEpsilon(new Rectangle(-8.616f
                , 0f, 11.1f, 19.344f)));
        }

        private static TextRenderInfo InitTRI(String text, double angle) {
            CanvasGraphicsState gs = new CanvasGraphicsState();
            gs.SetFont(PdfFontFactory.CreateFont());
            gs.SetFontSize(12);
            float[] matrix = new float[6];
            AffineTransform transform = AffineTransform.GetRotateInstance(angle);
            transform.GetMatrix(matrix);
            return new TextRenderInfo(new PdfString(text), gs, new Matrix(matrix[0], matrix[1], matrix[2], matrix[3], 
                matrix[4], matrix[5]), new Stack<CanvasTag>());
        }
    }
}
