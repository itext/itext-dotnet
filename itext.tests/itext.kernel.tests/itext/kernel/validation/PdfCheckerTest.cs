/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Utils.Checkers;
using iText.Test;

namespace iText.Kernel.Validation {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfCheckerTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/validation/PdfCheckerTest/";

        [NUnit.Framework.Test]
        public virtual void InvalidTypeSubtypeMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "metadata_ua2.xmp"));
                PdfStream metadata = new PdfStream(bytes);
                catalog.Put(PdfName.Metadata, metadata);
                Pdf20Checker checker = new Pdf20Checker();
                Exception e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => checker.CheckMetadata(
                    catalog));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.METADATA_STREAM_REQUIRES_METADATA_TYPE_AND_XML_SUBTYPE
                    , e.Message);
                metadata.Put(PdfName.Type, PdfName.XML);
                e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => checker.CheckMetadata(catalog));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.METADATA_STREAM_REQUIRES_METADATA_TYPE_AND_XML_SUBTYPE
                    , e.Message);
                metadata.Put(PdfName.Type, PdfName.Metadata);
                e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => checker.CheckMetadata(catalog));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.METADATA_STREAM_REQUIRES_METADATA_TYPE_AND_XML_SUBTYPE
                    , e.Message);
                metadata.Put(PdfName.Type, PdfName.Metadata);
                metadata.Put(PdfName.Subtype, PdfName.Metadata);
                e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => checker.CheckMetadata(catalog));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.METADATA_STREAM_REQUIRES_METADATA_TYPE_AND_XML_SUBTYPE
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoRevInMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "no_revision_metadata_ua2.xmp"));
                PdfStream metadata = new PdfStream(bytes);
                catalog.Put(PdfName.Metadata, metadata);
                catalog.Put(PdfName.Type, PdfName.Metadata);
                catalog.Put(PdfName.Subtype, PdfName.XML);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfCheckersUtil.CheckMetadata(catalog
                    .GetPdfObject(), PdfConformance.PDF_UA_2));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_REV
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ValidMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "metadata_ua2.xmp"));
                PdfStream metadata = new PdfStream(bytes);
                catalog.Put(PdfName.Metadata, metadata);
                catalog.Put(PdfName.Type, PdfName.Metadata);
                catalog.Put(PdfName.Subtype, PdfName.XML);
                NUnit.Framework.Assert.DoesNotThrow(() => PdfCheckersUtil.CheckMetadata(catalog.GetPdfObject(), PdfConformance
                    .PDF_UA_2));
            }
        }
    }
}
