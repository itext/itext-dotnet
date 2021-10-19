using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Tagging {
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
