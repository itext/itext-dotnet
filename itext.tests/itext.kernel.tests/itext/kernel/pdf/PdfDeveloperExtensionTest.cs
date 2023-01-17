/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfDeveloperExtensionTest {
        public static readonly PdfDeveloperExtension SIMPLE_EXTENSION_L3 = new PdfDeveloperExtension(new PdfName("Test"
            ), PdfName.Pdf_Version_1_7, 3);

        public static readonly PdfDeveloperExtension SIMPLE_EXTENSION_L5 = new PdfDeveloperExtension(new PdfName("Test"
            ), PdfName.Pdf_Version_1_7, 5);

        public static readonly PdfDeveloperExtension MULTI_EXTENSION_1 = new PdfDeveloperExtension(new PdfName("Test"
            ), PdfName.Pdf_Version_2_0, 1, "https://example.com", ":2022", true);

        public static readonly PdfDeveloperExtension MULTI_EXTENSION_2 = new PdfDeveloperExtension(new PdfName("Test"
            ), PdfName.Pdf_Version_2_0, 2, "https://example.com", ":2022", true);

        [NUnit.Framework.Test]
        public virtual void AddSingleValuedExtensionTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos))) {
                pdfDoc.GetCatalog().AddDeveloperExtension(SIMPLE_EXTENSION_L3);
            }
            AssertSimpleExtension(baos.ToArray(), SIMPLE_EXTENSION_L3.GetPrefix(), SIMPLE_EXTENSION_L3.GetExtensionLevel
                ());
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleValuedExtensionOverrideTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos))) {
                pdfDoc.GetCatalog().AddDeveloperExtension(SIMPLE_EXTENSION_L3);
                pdfDoc.GetCatalog().AddDeveloperExtension(SIMPLE_EXTENSION_L5);
            }
            AssertSimpleExtension(baos.ToArray(), SIMPLE_EXTENSION_L5.GetPrefix(), SIMPLE_EXTENSION_L5.GetExtensionLevel
                ());
        }

        [NUnit.Framework.Test]
        public virtual void AddSingleValuedExtensionNoOverrideTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos))) {
                pdfDoc.GetCatalog().AddDeveloperExtension(SIMPLE_EXTENSION_L5);
                pdfDoc.GetCatalog().AddDeveloperExtension(SIMPLE_EXTENSION_L3);
            }
            AssertSimpleExtension(baos.ToArray(), SIMPLE_EXTENSION_L5.GetPrefix(), SIMPLE_EXTENSION_L5.GetExtensionLevel
                ());
        }

        [NUnit.Framework.Test]
        public virtual void AddMultivaluedExtensionTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos))) {
                pdfDoc.GetCatalog().AddDeveloperExtension(MULTI_EXTENSION_1);
            }
            AssertMultiExtension(baos.ToArray(), MULTI_EXTENSION_1.GetPrefix(), JavaCollectionsUtil.SingletonList(MULTI_EXTENSION_1
                .GetExtensionLevel()));
        }

        [NUnit.Framework.Test]
        public virtual void AddMultivaluedExtensionNoDuplicateTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos))) {
                pdfDoc.GetCatalog().AddDeveloperExtension(MULTI_EXTENSION_1);
                pdfDoc.GetCatalog().AddDeveloperExtension(MULTI_EXTENSION_1);
            }
            AssertMultiExtension(baos.ToArray(), MULTI_EXTENSION_1.GetPrefix(), JavaCollectionsUtil.SingletonList(MULTI_EXTENSION_1
                .GetExtensionLevel()));
        }

        [NUnit.Framework.Test]
        public virtual void AddMultivaluedExtensionNoOverrideTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos))) {
                pdfDoc.GetCatalog().AddDeveloperExtension(MULTI_EXTENSION_1);
                pdfDoc.GetCatalog().AddDeveloperExtension(MULTI_EXTENSION_2);
            }
            AssertMultiExtension(baos.ToArray(), MULTI_EXTENSION_1.GetPrefix(), JavaUtil.ArraysAsList(MULTI_EXTENSION_1
                .GetExtensionLevel(), MULTI_EXTENSION_2.GetExtensionLevel()));
        }

        private void AssertSimpleExtension(byte[] docData, PdfName prefix, int expectedLevel) {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(new MemoryStream(docData)))) {
                PdfDictionary extDict = pdfDoc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Extensions).GetAsDictionary
                    (prefix);
                NUnit.Framework.Assert.AreEqual(expectedLevel, extDict.GetAsNumber(PdfName.ExtensionLevel).IntValue());
            }
        }

        private void AssertMultiExtension(byte[] docData, PdfName prefix, ICollection<int> expectedLevels) {
            ICollection<int> seen = new HashSet<int>();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(new MemoryStream(docData)))) {
                PdfArray exts = pdfDoc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Extensions).GetAsArray(prefix);
                for (int i = 0; i < exts.Size(); i++) {
                    int level = exts.GetAsDictionary(i).GetAsInt(PdfName.ExtensionLevel).Value;
                    NUnit.Framework.Assert.IsTrue(expectedLevels.Contains(level), "Level " + level + " is not in expected level list"
                        );
                    NUnit.Framework.Assert.IsFalse(seen.Contains(level), "Level " + level + " appears multiple times");
                    seen.Add(level);
                }
            }
        }
    }
}
