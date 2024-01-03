/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Signatures {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfSignatureBuildPropertiesUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetSignatureCreatorTest() {
            PdfSignatureBuildProperties properties = new PdfSignatureBuildProperties();
            PdfDictionary appPropDic = new PdfDictionary();
            properties.GetPdfObject().Put(PdfName.App, appPropDic);
            properties.SetSignatureCreator("sign_creator");
            PdfObject appDict = properties.GetPdfObject().Get(PdfName.App);
            NUnit.Framework.Assert.IsTrue(appDict is PdfDictionary);
            PdfObject name = ((PdfDictionary)appDict).Get(PdfName.Name);
            NUnit.Framework.Assert.IsTrue(name is PdfName);
            NUnit.Framework.Assert.AreEqual("sign_creator", ((PdfName)name).GetValue());
        }
    }
}
