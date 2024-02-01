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
using System.Collections.Generic;
using System.IO;
using iText.IO.Source;
using iText.Kernel.Pdf.Extgstate;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfResourcesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ResourcesTest1() {
            PdfDocument document = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfPage page = document.AddNewPage();
            PdfExtGState egs1 = new PdfExtGState();
            PdfExtGState egs2 = new PdfExtGState();
            PdfResources resources = page.GetResources();
            PdfName n1 = resources.AddExtGState(egs1);
            NUnit.Framework.Assert.AreEqual("Gs1", n1.GetValue());
            PdfName n2 = resources.AddExtGState(egs2);
            NUnit.Framework.Assert.AreEqual("Gs2", n2.GetValue());
            n1 = resources.AddExtGState(egs1);
            NUnit.Framework.Assert.AreEqual("Gs1", n1.GetValue());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ResourcesTest2() {
            MemoryStream baos = new MemoryStream();
            PdfDocument document = new PdfDocument(new PdfWriter(baos));
            PdfPage page = document.AddNewPage();
            PdfExtGState egs1 = new PdfExtGState();
            PdfExtGState egs2 = new PdfExtGState();
            PdfResources resources = page.GetResources();
            resources.AddExtGState(egs1);
            resources.AddExtGState(egs2);
            document.Close();
            PdfReader reader = new PdfReader(new MemoryStream(baos.ToArray()));
            document = new PdfDocument(reader, new PdfWriter(new ByteArrayOutputStream()));
            page = document.GetPage(1);
            resources = page.GetResources();
            ICollection<PdfName> names = resources.GetResourceNames();
            NUnit.Framework.Assert.AreEqual(2, names.Count);
            String[] expectedNames = new String[] { "Gs1", "Gs2" };
            int i = 0;
            foreach (PdfName name in names) {
                NUnit.Framework.Assert.AreEqual(expectedNames[i++], name.GetValue());
            }
            PdfExtGState egs3 = new PdfExtGState();
            PdfName n3 = resources.AddExtGState(egs3);
            NUnit.Framework.Assert.AreEqual("Gs3", n3.GetValue());
            PdfDictionary egsResources = page.GetPdfObject().GetAsDictionary(PdfName.Resources).GetAsDictionary(PdfName
                .ExtGState);
            PdfDictionary e1 = egsResources.GetAsDictionary(new PdfName("Gs1"));
            PdfName n1 = resources.AddExtGState(e1);
            NUnit.Framework.Assert.AreEqual("Gs1", n1.GetValue());
            PdfDictionary e2 = egsResources.GetAsDictionary(new PdfName("Gs2"));
            PdfName n2 = resources.AddExtGState(e2);
            NUnit.Framework.Assert.AreEqual("Gs2", n2.GetValue());
            PdfDictionary e4 = (PdfDictionary)e2.Clone();
            PdfName n4 = resources.AddExtGState(e4);
            NUnit.Framework.Assert.AreEqual("Gs4", n4.GetValue());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void GetNonExistentResourcesCategory() {
            PdfResources resources = new PdfResources();
            ICollection<PdfName> unknownResCategory = resources.GetResourceNames(new PdfName("UnknownResCategory"));
            // Assert returned value is properly functioning
            PdfName randomResName = new PdfName("NonExistentResourceName");
            NUnit.Framework.Assert.IsFalse(unknownResCategory.Contains(randomResName));
            NUnit.Framework.Assert.IsFalse(unknownResCategory.Remove(randomResName));
            NUnit.Framework.Assert.IsTrue(unknownResCategory.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void ResourcesCategoryDictionarySetModifiedTest() {
            PdfDictionary pdfDict = new PdfDictionary();
            pdfDict.Put(PdfName.ExtGState, new PdfDictionary());
            PdfResources resources = new PdfResources(pdfDict);
            PdfObject resourceCategoryDict = resources.GetPdfObject().Get(PdfName.ExtGState);
            resourceCategoryDict.SetIndirectReference(new PdfIndirectReference(null, 1));
            NUnit.Framework.Assert.IsFalse(resourceCategoryDict.IsModified());
            resources.AddExtGState(new PdfExtGState());
            // Check that when changing an existing resource category dictionary, the flag PdfObject.MODIFIED will be set for it
            NUnit.Framework.Assert.IsTrue(resourceCategoryDict.IsModified());
        }
    }
}
