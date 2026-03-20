using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Colorspace {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfColorspaceTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetColorspaceNameTest() {
            PdfArray indexedValues = new PdfArray();
            indexedValues.Add(PdfName.Indexed);
            indexedValues.Add(PdfName.DeviceGray);
            indexedValues.Add(new PdfNumber(2));
            PdfString lookup = new PdfString(new byte[] { 0x00, (byte)0xff });
            lookup.SetHexWriting(true);
            indexedValues.Add(lookup);
            PdfSpecialCs.Indexed indexed = new PdfSpecialCs.Indexed(indexedValues);
            NUnit.Framework.Assert.AreEqual("Indexed", indexed.GetColorspaceName().GetValue());
            NUnit.Framework.Assert.AreEqual(typeof(PdfColorspaceTest.TestColorSpace).Name, new PdfColorspaceTest.TestColorSpace
                ().GetColorspaceName().GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void GetNameTest() {
            PdfArray indexedValues = new PdfArray();
            indexedValues.Add(PdfName.Indexed);
            indexedValues.Add(PdfName.DeviceGray);
            indexedValues.Add(new PdfNumber(2));
            PdfString lookup = new PdfString(new byte[] { 0x00, (byte)0xff });
            lookup.SetHexWriting(true);
            indexedValues.Add(lookup);
            PdfSpecialCs.Indexed indexed = new PdfSpecialCs.Indexed(indexedValues);
            NUnit.Framework.Assert.AreEqual("Indexed", indexed.GetName().GetValue());
            NUnit.Framework.Assert.AreEqual(typeof(PdfColorspaceTest.TestColorSpace).Name, new PdfColorspaceTest.TestColorSpace
                ().GetName().GetValue());
        }

        private class TestColorSpace : PdfColorSpace {
            protected internal TestColorSpace()
                : base(new PdfArray()) {
            }

            public override int GetNumberOfComponents() {
                return 0;
            }

            protected internal override bool IsWrappedObjectMustBeIndirect() {
                return false;
            }
        }
    }
}
