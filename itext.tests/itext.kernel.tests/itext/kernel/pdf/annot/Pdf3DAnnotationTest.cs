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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("UnitTest")]
    public class Pdf3DAnnotationTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SetAndGetDefaultInitialViewTest() {
            Pdf3DAnnotation pdf3DAnnotation = new Pdf3DAnnotation(new PdfDictionary());
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Type, new PdfName("3DView"));
            pdf3DAnnotation.SetDefaultInitialView(dict);
            NUnit.Framework.Assert.AreEqual(dict, pdf3DAnnotation.GetDefaultInitialView());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetActivationDictionaryTest() {
            Pdf3DAnnotation pdf3DAnnotation = new Pdf3DAnnotation(new PdfDictionary());
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.Type, new PdfName("3DView"));
            pdf3DAnnotation.SetActivationDictionary(dict);
            NUnit.Framework.Assert.AreEqual(dict, pdf3DAnnotation.GetActivationDictionary());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndIsInteractiveTest() {
            Pdf3DAnnotation pdf3DAnnotation = new Pdf3DAnnotation(new PdfDictionary());
            bool flag = true;
            pdf3DAnnotation.SetInteractive(flag);
            NUnit.Framework.Assert.AreEqual(flag, pdf3DAnnotation.IsInteractive().GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetViewBoxTest() {
            Pdf3DAnnotation pdf3DAnnotation = new Pdf3DAnnotation(new PdfDictionary());
            Rectangle rect = new Rectangle(10, 10);
            pdf3DAnnotation.SetViewBox(rect);
            bool result = rect.EqualsWithEpsilon(pdf3DAnnotation.GetViewBox());
            NUnit.Framework.Assert.IsTrue(result, "Rectangles are different");
        }
    }
}
