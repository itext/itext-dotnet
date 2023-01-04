/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
