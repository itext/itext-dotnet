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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Test;

namespace iText.Pdfua.Checkers.Utils {
    [NUnit.Framework.Category("UnitTest")]
    [Obsolete]
    public class AnnotationCheckUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestIsAnnotationVisible() {
            NUnit.Framework.Assert.IsTrue(AnnotationCheckUtil.IsAnnotationVisible(new PdfDictionary()));
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationHandler() {
            AnnotationCheckUtil.AnnotationHandler handler = new AnnotationCheckUtil.AnnotationHandler(new PdfUAValidationContext
                (null));
            NUnit.Framework.Assert.IsNotNull(handler);
            NUnit.Framework.Assert.IsFalse(handler.Accept(null));
            NUnit.Framework.Assert.IsTrue(handler.Accept(new PdfMcrNumber(new PdfNumber(2), null)));
            NUnit.Framework.Assert.DoesNotThrow(() => handler.ProcessElement(null));
        }
    }
}
