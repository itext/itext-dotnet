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
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Tagging {
    [NUnit.Framework.Category("UnitTest")]
    public class StructureTreeCopierUnitTest : ExtendedITextTest {
        private static readonly IDictionary<PdfName, PdfObject> td = new Dictionary<PdfName, PdfObject>();

        private static readonly IDictionary<PdfName, PdfObject> tr = new Dictionary<PdfName, PdfObject>();

        private static readonly IDictionary<PdfName, PdfObject> th = new Dictionary<PdfName, PdfObject>();

        static StructureTreeCopierUnitTest() {
            td.Put(PdfName.S, PdfName.TD);
            tr.Put(PdfName.S, PdfName.TR);
            th.Put(PdfName.S, PdfName.TH);
        }

        [NUnit.Framework.Test]
        public virtual void ShouldTableElementBeCopiedTdTrTest() {
            PdfDictionary obj = new PdfDictionary(td);
            PdfDictionary parent = new PdfDictionary(tr);
            NUnit.Framework.Assert.IsTrue(StructureTreeCopier.ShouldTableElementBeCopied(obj, parent));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldTableElementBeCopiedThTrTest() {
            PdfDictionary obj = new PdfDictionary(th);
            PdfDictionary parent = new PdfDictionary(tr);
            NUnit.Framework.Assert.IsTrue(StructureTreeCopier.ShouldTableElementBeCopied(obj, parent));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldTableElementBeCopiedTdTdTest() {
            PdfDictionary obj = new PdfDictionary(td);
            PdfDictionary parent = new PdfDictionary(td);
            NUnit.Framework.Assert.IsFalse(StructureTreeCopier.ShouldTableElementBeCopied(obj, parent));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldTableElementBeCopiedTrTdTest() {
            PdfDictionary obj = new PdfDictionary(tr);
            PdfDictionary parent = new PdfDictionary(td);
            NUnit.Framework.Assert.IsFalse(StructureTreeCopier.ShouldTableElementBeCopied(obj, parent));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldTableElementBeCopiedTrTrTest() {
            PdfDictionary obj = new PdfDictionary(tr);
            PdfDictionary parent = new PdfDictionary(tr);
            NUnit.Framework.Assert.IsFalse(StructureTreeCopier.ShouldTableElementBeCopied(obj, parent));
        }

        [NUnit.Framework.Test]
        public virtual void ShouldTableElementBeCopiedThThTest() {
            PdfDictionary obj = new PdfDictionary(th);
            PdfDictionary parent = new PdfDictionary(th);
            NUnit.Framework.Assert.IsFalse(StructureTreeCopier.ShouldTableElementBeCopied(obj, parent));
        }
    }
}
