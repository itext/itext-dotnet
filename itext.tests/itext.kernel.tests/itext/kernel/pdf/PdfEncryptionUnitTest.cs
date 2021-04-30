using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfEncryptionUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ReadEncryptEmbeddedFilesOnlyFromPdfDocumentCorrectEntryTest() {
            PdfDictionary cryptDictionary = new PdfDictionary();
            cryptDictionary.Put(PdfName.EFF, PdfName.StdCF);
            cryptDictionary.Put(PdfName.StmF, PdfName.Identity);
            cryptDictionary.Put(PdfName.StrF, PdfName.Identity);
            PdfDictionary cfDictionary = new PdfDictionary();
            cfDictionary.Put(PdfName.StdCF, new PdfDictionary());
            cryptDictionary.Put(PdfName.CF, cfDictionary);
            NUnit.Framework.Assert.IsTrue(PdfEncryption.ReadEmbeddedFilesOnlyFromEncryptDictionary(cryptDictionary));
        }

        [NUnit.Framework.Test]
        public virtual void ReadEncryptEmbeddedFilesOnlyFromPdfDocumentIncorrectEffTest() {
            PdfDictionary cryptDictionary = new PdfDictionary();
            cryptDictionary.Put(PdfName.EFF, PdfName.Identity);
            cryptDictionary.Put(PdfName.StmF, PdfName.Identity);
            cryptDictionary.Put(PdfName.StrF, PdfName.Identity);
            PdfDictionary cfDictionary = new PdfDictionary();
            cfDictionary.Put(PdfName.StdCF, new PdfDictionary());
            cryptDictionary.Put(PdfName.CF, cfDictionary);
            NUnit.Framework.Assert.IsFalse(PdfEncryption.ReadEmbeddedFilesOnlyFromEncryptDictionary(cryptDictionary));
        }

        [NUnit.Framework.Test]
        public virtual void ReadEncryptEmbeddedFilesOnlyFromPdfDocumentIncorrectStmFTest() {
            PdfDictionary cryptDictionary = new PdfDictionary();
            cryptDictionary.Put(PdfName.EFF, PdfName.StdCF);
            cryptDictionary.Put(PdfName.StmF, PdfName.StdCF);
            cryptDictionary.Put(PdfName.StrF, PdfName.Identity);
            PdfDictionary cfDictionary = new PdfDictionary();
            cfDictionary.Put(PdfName.StdCF, new PdfDictionary());
            cryptDictionary.Put(PdfName.CF, cfDictionary);
            NUnit.Framework.Assert.IsFalse(PdfEncryption.ReadEmbeddedFilesOnlyFromEncryptDictionary(cryptDictionary));
        }

        [NUnit.Framework.Test]
        public virtual void ReadEncryptEmbeddedFilesOnlyFromPdfDocumentIncorrectStrFTest() {
            PdfDictionary cryptDictionary = new PdfDictionary();
            cryptDictionary.Put(PdfName.EFF, PdfName.StdCF);
            cryptDictionary.Put(PdfName.StmF, PdfName.Identity);
            cryptDictionary.Put(PdfName.StrF, PdfName.StdCF);
            PdfDictionary cfDictionary = new PdfDictionary();
            cfDictionary.Put(PdfName.StdCF, new PdfDictionary());
            cryptDictionary.Put(PdfName.CF, cfDictionary);
            NUnit.Framework.Assert.IsFalse(PdfEncryption.ReadEmbeddedFilesOnlyFromEncryptDictionary(cryptDictionary));
        }

        [NUnit.Framework.Test]
        public virtual void ReadEncryptEmbeddedFilesOnlyFromPdfDocumentIncorrectCfTest() {
            PdfDictionary cryptDictionary = new PdfDictionary();
            cryptDictionary.Put(PdfName.EFF, PdfName.StdCF);
            cryptDictionary.Put(PdfName.StmF, PdfName.Identity);
            cryptDictionary.Put(PdfName.StrF, PdfName.Identity);
            PdfDictionary cfDictionary = new PdfDictionary();
            cfDictionary.Put(PdfName.DefaultCryptFilter, new PdfDictionary());
            cryptDictionary.Put(PdfName.CF, cfDictionary);
            NUnit.Framework.Assert.IsFalse(PdfEncryption.ReadEmbeddedFilesOnlyFromEncryptDictionary(cryptDictionary));
        }
    }
}
