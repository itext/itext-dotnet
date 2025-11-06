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
using System.Collections.Generic;
using iText.Commons.Datastructures;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("UnitTest")]
    public class DocumentRevisionPdfObjectComparatorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestUnsupportedDecodingTypeDoesNotThrowException() {
            PdfStream stream1 = new PdfStream();
            stream1.SetData(new byte[] { 0x01, 0x02, 0x03 });
            stream1.Put(PdfName.Filter, PdfName.JBIG2Decode);
            PdfStream stream2 = new PdfStream();
            stream2.SetData(new byte[] { 0x01, 0x02, 0x03 });
            stream2.Put(PdfName.Filter, PdfName.JBIG2Decode);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                DocumentRevisionPdfObjectComparator.ComparePdfObjects(stream1, stream2, new Tuple2<ICollection<PdfIndirectReference
                    >, ICollection<PdfIndirectReference>>(new HashSet<PdfIndirectReference>(), new HashSet<PdfIndirectReference
                    >()));
            }
            );
        }
    }
}
