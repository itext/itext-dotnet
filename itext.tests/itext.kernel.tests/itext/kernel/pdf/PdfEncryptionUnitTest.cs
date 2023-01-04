/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
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
