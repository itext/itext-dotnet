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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfFreeTextAnnotationTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetAndGetRotationTest() {
            PdfFreeTextAnnotation freeTextAnnotation = new PdfFreeTextAnnotation(new Rectangle(0, 0, 100, 50), new PdfString
                ("content"));
            freeTextAnnotation.SetRotation(135);
            NUnit.Framework.Assert.AreEqual(135, freeTextAnnotation.GetRotation().IntValue());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetJustificationTest() {
            PdfFreeTextAnnotation freeTextAnnotation = new PdfFreeTextAnnotation(new Rectangle(0, 0, 100, 50), null);
            NUnit.Framework.Assert.AreEqual(PdfFreeTextAnnotation.LEFT_JUSTIFIED, freeTextAnnotation.GetJustification(
                ));
            freeTextAnnotation.SetJustification(PdfFreeTextAnnotation.CENTERED);
            NUnit.Framework.Assert.AreEqual(PdfFreeTextAnnotation.CENTERED, freeTextAnnotation.GetJustification());
        }
    }
}
