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
using System.IO;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfAnnotationMakeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void MakePdfAnnotationTest() {
            PdfDictionary @object = new PdfDictionary();
            PdfAnnotation result = PdfAnnotation.MakeAnnotation(@object);
            NUnit.Framework.Assert.IsNull(result.GetSubtype());
        }

        [NUnit.Framework.Test]
        public virtual void MakePdfTextAnnotationTest() {
            PdfDictionary @object = new PdfDictionary();
            @object.Put(PdfName.Subtype, PdfName.Text);
            PdfAnnotation result = PdfAnnotation.MakeAnnotation(@object);
            NUnit.Framework.Assert.IsTrue(result is PdfTextAnnotation);
        }

        [NUnit.Framework.Test]
        public virtual void MakeIndirectPdfAnnotationTest() {
            PdfDictionary @object = new PdfDictionary();
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                doc.AddNewPage();
                PdfObject indirect = @object.MakeIndirect(doc);
                @object.Put(PdfName.Subtype, PdfName.Text);
                PdfAnnotation result = PdfAnnotation.MakeAnnotation(indirect);
                NUnit.Framework.Assert.IsTrue(result is PdfTextAnnotation);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MakePdfPolyAnnotationTest() {
            PdfDictionary @object = new PdfDictionary();
            @object.Put(PdfName.Subtype, PdfName.Polygon);
            PdfAnnotation result = PdfAnnotation.MakeAnnotation(@object);
            NUnit.Framework.Assert.IsTrue(result is PdfPolyGeomAnnotation);
        }

        [NUnit.Framework.Test]
        public virtual void MakePdfUnknownAnnotationTest() {
            PdfDictionary @object = new PdfDictionary();
            // from DEVSIX-2661
            @object.Put(PdfName.Subtype, new PdfName("BatesN"));
            PdfAnnotation result = PdfAnnotation.MakeAnnotation(@object);
            NUnit.Framework.Assert.IsTrue(result is PdfAnnotation.PdfUnknownAnnotation);
        }
    }
}
