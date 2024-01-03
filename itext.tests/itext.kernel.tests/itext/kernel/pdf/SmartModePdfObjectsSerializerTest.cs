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
using System;
using System.IO;
using System.Text;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class SmartModePdfObjectsSerializerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SmartModeObjectSelfReferencingTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfDictionary dict1 = new PdfDictionary();
            dict1.MakeIndirect(document);
            PdfDictionary dict2 = new PdfDictionary();
            dict2.MakeIndirect(document);
            PdfArray array = new PdfArray();
            array.MakeIndirect(document);
            array.Add(new PdfString(new byte[10000]));
            array.Add(new PdfDictionary(dict2));
            dict1.Put(new PdfName("FirstDict"), array.GetIndirectReference());
            dict2.Put(new PdfName("SecondDict"), dict1.GetIndirectReference());
            SmartModePdfObjectsSerializer serializer = new SmartModePdfObjectsSerializer();
            SerializedObjectContent serializedObject = serializer.SerializeObject(dict1);
            //It is essential to serialize object with huge amount of memory
            StringBuilder stringBytes = new StringBuilder().Append("$D$N/FirstDict$A$S");
            String end = "$D$\\D$\\A$\\D";
            for (int i = 0; i < 10000; i++) {
                stringBytes.Append("\x0");
            }
            stringBytes.Append(end);
            SerializedObjectContent expected = new SerializedObjectContent(stringBytes.ToString().GetBytes(System.Text.Encoding
                .UTF8));
            NUnit.Framework.Assert.AreEqual(expected, serializedObject);
        }
    }
}
