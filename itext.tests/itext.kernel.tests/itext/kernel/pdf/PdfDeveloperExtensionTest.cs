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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfDeveloperExtensionTest : ExtendedITextTest {
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

        [NUnit.Framework.Test]
        public virtual void RemoveSingleValuedExtensionTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos))) {
                pdfDoc.GetCatalog().AddDeveloperExtension(SIMPLE_EXTENSION_L5);
                pdfDoc.GetCatalog().RemoveDeveloperExtension(SIMPLE_EXTENSION_L5);
            }
            AssertNoExtensionWithPrefix(baos.ToArray(), SIMPLE_EXTENSION_L5.GetPrefix());
        }

        [NUnit.Framework.Test]
        public virtual void RemoveMultivaluedExtensionTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos))) {
                pdfDoc.GetCatalog().AddDeveloperExtension(MULTI_EXTENSION_1);
                pdfDoc.GetCatalog().AddDeveloperExtension(MULTI_EXTENSION_2);
                pdfDoc.GetCatalog().RemoveDeveloperExtension(MULTI_EXTENSION_2);
            }
            AssertMultiExtension(baos.ToArray(), MULTI_EXTENSION_1.GetPrefix(), JavaUtil.ArraysAsList(MULTI_EXTENSION_1
                .GetExtensionLevel()));
        }

        private void AssertSimpleExtension(byte[] docData, PdfName prefix, int expectedLevel) {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(new MemoryStream(docData)))) {
                PdfDictionary extDict = pdfDoc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Extensions).GetAsDictionary
                    (prefix);
                NUnit.Framework.Assert.AreEqual(expectedLevel, extDict.GetAsNumber(PdfName.ExtensionLevel).IntValue());
            }
        }

        private void AssertNoExtensionWithPrefix(byte[] docData, PdfName prefix) {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(new MemoryStream(docData)))) {
                PdfDictionary extDict = pdfDoc.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Extensions).GetAsDictionary
                    (prefix);
                NUnit.Framework.Assert.IsNull(extDict);
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
