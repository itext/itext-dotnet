/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfObjectWrapperUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DirectObjectsCouldntBeWrappedTest01() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDictionary pdfDictionary = new PdfDictionary();
                NUnit.Framework.Assert.IsTrue(null == pdfDictionary.GetIndirectReference());
                // PdfCatalog is one of PdfObjectWrapper implementations. The issue could be reproduced on any of them, not only on this one
                PdfCatalog pdfCatalog = new PdfCatalog(pdfDictionary);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(KernelExceptionMessageConstant.OBJECT_MUST_BE_INDIRECT_TO_WORK_WITH_THIS_WRAPPER))
;
        }
    }
}
