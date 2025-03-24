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
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Pdfua.Checkers;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfua {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfUAMetadataUnitTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAMetadataUnitTest/";

        [NUnit.Framework.Test]
        public virtual void DocumentWithNoTitleInMetadataTest() {
            using (PdfDocument pdfDocument = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "no_title_metadata.xmp"));
                catalog.Put(PdfName.Metadata, new PdfStream(bytes));
                PdfUAMetadataUnitTest.PdfUA1MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA1MetadataChecker(pdfDocument
                    );
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                    catalog));
                NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_DC_TITLE_ENTRY, e.Message
                    );
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithInvalidMetadataVersionTest() {
            PdfDocument pdfDocument = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream()));
            pdfDocument.AddNewPage();
            PdfCatalog catalog = pdfDocument.GetCatalog();
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "invalid_version_metadata.xmp"));
            catalog.Put(PdfName.Metadata, new PdfStream(bytes));
            PdfUAMetadataUnitTest.PdfUA1MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA1MetadataChecker(pdfDocument
                );
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                catalog));
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_UA_VERSION_IDENTIFIER
                , e.Message);
            e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_UA_VERSION_IDENTIFIER
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithNoMetadataVersionTest() {
            using (PdfDocument pdfDocument = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "no_version_metadata.xmp"));
                catalog.Put(PdfName.Metadata, new PdfStream(bytes));
                PdfUAMetadataUnitTest.PdfUA1MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA1MetadataChecker(pdfDocument
                    );
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                    catalog));
                NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_UA_VERSION_IDENTIFIER
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithInvalidMetadataTypeTest() {
            using (PdfDocument pdfDocument = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                catalog.Put(PdfName.Metadata, new PdfDictionary());
                PdfUAMetadataUnitTest.PdfUA1MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA1MetadataChecker(pdfDocument
                    );
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                    catalog));
                NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.DOCUMENT_SHALL_CONTAIN_XMP_METADATA_STREAM, 
                    e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithInvalidPdfVersionTest() {
            PdfUAMetadataUnitTest.PdfDocumentCustomVersion pdfDocument = new PdfUAMetadataUnitTest.PdfDocumentCustomVersion
                (new PdfWriter(new MemoryStream()), new PdfUAConfig(PdfUAConformance.PDF_UA_1, "en-us", "title"));
            pdfDocument.SetPdfVersion(PdfVersion.PDF_2_0);
            pdfDocument.AddNewPage();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.INVALID_PDF_VERSION, e.Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA)]
        public virtual void DocumentWithBrokenMetadataTest() {
            PdfDocument pdfDocument = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream()));
            pdfDocument.AddNewPage();
            PdfCatalog catalog = pdfDocument.GetCatalog();
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "invalid_metadata.xmp"));
            catalog.Put(PdfName.Metadata, new PdfStream(bytes));
            PdfUAMetadataUnitTest.PdfUA1MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA1MetadataChecker(pdfDocument
                );
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                catalog));
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.DOCUMENT_SHALL_CONTAIN_XMP_METADATA_STREAM, 
                e.Message);
            e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.DOCUMENT_SHALL_CONTAIN_XMP_METADATA_STREAM, 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ValidMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "metadata_ua2.xmp"));
                PdfStream metadata = new PdfStream(bytes);
                metadata.Put(PdfName.Type, PdfName.Metadata);
                metadata.Put(PdfName.Subtype, PdfName.XML);
                catalog.Put(PdfName.Metadata, metadata);
                PdfUAMetadataUnitTest.PdfUA2MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA2MetadataChecker(pdfDocument
                    );
                NUnit.Framework.Assert.DoesNotThrow(() => checker.CheckMetadata(catalog));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CatalogNoMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                catalog.Remove(PdfName.Metadata);
                PdfUAMetadataUnitTest.PdfUA2MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA2MetadataChecker(pdfDocument
                    );
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                    catalog));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.METADATA_SHALL_BE_PRESENT_IN_THE_CATALOG_DICTIONARY
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CatalogInvalidMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                catalog.Put(PdfName.Metadata, new PdfString("Error"));
                PdfUAMetadataUnitTest.PdfUA2MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA2MetadataChecker(pdfDocument
                    );
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                    catalog));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_METADATA_VALUE, e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA)]
        public virtual void BrokenMetadataUA2Test() {
            PdfDocument pdfDocument = new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream()));
            pdfDocument.AddNewPage();
            PdfCatalog catalog = pdfDocument.GetCatalog();
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "invalid_metadata_ua2.xmp"));
            PdfStream metadata = new PdfStream(bytes);
            metadata.Put(PdfName.Type, PdfName.Metadata);
            metadata.Put(PdfName.Subtype, PdfName.XML);
            catalog.Put(PdfName.Metadata, metadata);
            PdfUAMetadataUnitTest.PdfUA2MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA2MetadataChecker(pdfDocument
                );
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                catalog));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_METADATA_VALUE, e.Message);
            e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_METADATA_VALUE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithNoPartInMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "metadata_no_part_ua2.xmp"));
                PdfStream metadata = new PdfStream(bytes);
                metadata.Put(PdfName.Type, PdfName.Metadata);
                metadata.Put(PdfName.Subtype, PdfName.XML);
                catalog.Put(PdfName.Metadata, metadata);
                PdfUAMetadataUnitTest.PdfUA2MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA2MetadataChecker(pdfDocument
                    );
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                    catalog));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_PART
                    , 2, null), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithInvalidPartInMetadataUA2Test() {
            PdfDocument pdfDocument = new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream()));
            pdfDocument.AddNewPage();
            PdfCatalog catalog = pdfDocument.GetCatalog();
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "metadata_invalid_part_ua2.xmp"));
            PdfStream metadata = new PdfStream(bytes);
            metadata.Put(PdfName.Type, PdfName.Metadata);
            metadata.Put(PdfName.Subtype, PdfName.XML);
            catalog.Put(PdfName.Metadata, metadata);
            PdfUAMetadataUnitTest.PdfUA2MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA2MetadataChecker(pdfDocument
                );
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                catalog));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_PART
                , 2, 1), e.Message);
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithNoRevInMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "metadata_no_rev_ua2.xmp"));
                PdfStream metadata = new PdfStream(bytes);
                metadata.Put(PdfName.Type, PdfName.Metadata);
                metadata.Put(PdfName.Subtype, PdfName.XML);
                catalog.Put(PdfName.Metadata, metadata);
                PdfUAMetadataUnitTest.PdfUA2MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA2MetadataChecker(pdfDocument
                    );
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                    catalog));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_REV
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithInvalidRevInMetadataUA2Test() {
            PdfDocument pdfDocument = new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream()));
            pdfDocument.AddNewPage();
            PdfCatalog catalog = pdfDocument.GetCatalog();
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "metadata_invalid_rev_ua2.xmp"));
            PdfStream metadata = new PdfStream(bytes);
            metadata.Put(PdfName.Type, PdfName.Metadata);
            metadata.Put(PdfName.Subtype, PdfName.XML);
            catalog.Put(PdfName.Metadata, metadata);
            PdfUAMetadataUnitTest.PdfUA2MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA2MetadataChecker(pdfDocument
                );
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                catalog));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_REV
                , e.Message);
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithInvalidLengthRevInMetadataUA2Test() {
            PdfDocument pdfDocument = new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream()));
            pdfDocument.AddNewPage();
            PdfCatalog catalog = pdfDocument.GetCatalog();
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "metadata_invalid_len_rev_ua2.xmp"
                ));
            PdfStream metadata = new PdfStream(bytes);
            metadata.Put(PdfName.Type, PdfName.Metadata);
            metadata.Put(PdfName.Subtype, PdfName.XML);
            catalog.Put(PdfName.Metadata, metadata);
            PdfUAMetadataUnitTest.PdfUA2MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA2MetadataChecker(pdfDocument
                );
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                catalog));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_REV
                , e.Message);
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDocument.Close());
        }

        [NUnit.Framework.Test]
        public virtual void DocumentWithNoTitleInMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfUA2TestPdfDocument(new PdfWriter(new MemoryStream()))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "no_title_metadata_ua2.xmp"));
                PdfStream metadata = new PdfStream(bytes);
                metadata.Put(PdfName.Type, PdfName.Metadata);
                metadata.Put(PdfName.Subtype, PdfName.XML);
                catalog.Put(PdfName.Metadata, metadata);
                PdfUAMetadataUnitTest.PdfUA2MetadataChecker checker = new PdfUAMetadataUnitTest.PdfUA2MetadataChecker(pdfDocument
                    );
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => checker.CheckMetadata(
                    catalog));
                NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.METADATA_SHALL_CONTAIN_DC_TITLE_ENTRY, e.Message
                    );
            }
        }

        private class PdfUA1MetadataChecker : PdfUA1Checker {
            /// <summary>Creates PdfUA1Checker instance with PDF document which will be validated against PDF/UA-1 standard.
            ///     </summary>
            /// <param name="pdfDocument">the document to validate</param>
            public PdfUA1MetadataChecker(PdfDocument pdfDocument)
                : base(pdfDocument) {
            }

            protected internal override void CheckMetadata(PdfCatalog catalog) {
                base.CheckMetadata(catalog);
            }
        }

        private class PdfUA2MetadataChecker : PdfUA2Checker {
            /// <summary>Creates PdfUA2Checker instance with PDF document which will be validated against PDF/UA-2 standard.
            ///     </summary>
            /// <param name="pdfDocument">the document to validate</param>
            public PdfUA2MetadataChecker(PdfDocument pdfDocument)
                : base(pdfDocument) {
            }

            protected internal override void CheckMetadata(PdfCatalog catalog) {
                base.CheckMetadata(catalog);
            }
        }

        private class PdfDocumentCustomVersion : PdfUADocument {
            public PdfDocumentCustomVersion(PdfWriter writer, PdfUAConfig config)
                : base(writer, config) {
            }

            public virtual void SetPdfVersion(PdfVersion pdfVersion) {
                this.pdfVersion = pdfVersion;
            }
        }
    }
}
