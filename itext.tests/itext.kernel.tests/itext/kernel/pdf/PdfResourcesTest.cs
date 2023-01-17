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
