/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
