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
using System.IO;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
    public class ExternalBlankSignatureContainerUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CreateBlankSignTest() {
            IExternalSignatureContainer container = new ExternalBlankSignatureContainer(new PdfDictionary());
            byte[] blankSign = container.Sign(new MemoryStream(new byte[] { 1, 0, 32, 5 }));
            NUnit.Framework.Assert.AreEqual(0, blankSign.Length);
        }

        [NUnit.Framework.Test]
        public virtual void ModifySigningDictionarySignTest() {
            PdfDictionary initDict = new PdfDictionary();
            initDict.Put(new PdfName("test_key"), new PdfName("test_value"));
            IExternalSignatureContainer container = new ExternalBlankSignatureContainer(initDict);
            PdfDictionary addDict = new PdfDictionary();
            addDict.Put(new PdfName("add_key"), new PdfName("add_value"));
            container.ModifySigningDictionary(addDict);
            byte[] blankSign = container.Sign(new MemoryStream(new byte[] { 1, 0, 32, 5 }));
            NUnit.Framework.Assert.AreEqual(0, blankSign.Length);
        }
    }
}
