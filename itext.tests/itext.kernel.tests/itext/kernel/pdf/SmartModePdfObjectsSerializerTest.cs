using System;
using System.IO;
using System.Text;

namespace iText.Kernel.Pdf {
    public class SmartModePdfObjectsSerializerTest {
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
