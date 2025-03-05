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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils.Checkers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Validation {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfCheckerTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/validation/PdfCheckerTest/";

        private static readonly Func<String, PdfException> EXCEPTION_SUPPLIER = (msg) => new PdfException(msg);

        [NUnit.Framework.Test]
        public virtual void InvalidTypeSubtypeMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "metadata_ua2.xmp"));
                PdfStream metadata = new PdfStream(bytes);
                catalog.Put(PdfName.Metadata, metadata);
                Pdf20Checker checker = new Pdf20Checker(pdfDocument);
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
        public virtual void NoMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfCheckersUtil.CheckMetadata(catalog
                    .GetPdfObject(), PdfConformance.PDF_UA_2, EXCEPTION_SUPPLIER));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.METADATA_SHALL_BE_PRESENT_IN_THE_CATALOG_DICTIONARY
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void NotStreamMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                catalog.Put(PdfName.Metadata, PdfName.Metadata);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfCheckersUtil.CheckMetadata(catalog
                    .GetPdfObject(), PdfConformance.PDF_UA_2, EXCEPTION_SUPPLIER));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_METADATA_VALUE, e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA)]
        public virtual void BrokenMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                catalog.Put(PdfName.Metadata, new PdfStream(new byte[] { 1, 2, 3 }));
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfCheckersUtil.CheckMetadata(catalog
                    .GetPdfObject(), PdfConformance.PDF_UA_2, EXCEPTION_SUPPLIER));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.INVALID_METADATA_VALUE, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void NoPartInMetadataUA2Test() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "no_version_metadata_ua2.xmp"));
                PdfStream metadata = new PdfStream(bytes);
                catalog.Put(PdfName.Metadata, metadata);
                catalog.Put(PdfName.Type, PdfName.Metadata);
                catalog.Put(PdfName.Subtype, PdfName.XML);
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfCheckersUtil.CheckMetadata(catalog
                    .GetPdfObject(), PdfConformance.PDF_UA_2, EXCEPTION_SUPPLIER));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.XMP_METADATA_HEADER_SHALL_CONTAIN_VERSION_IDENTIFIER_PART
                    , 2, null), e.Message);
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
                    .GetPdfObject(), PdfConformance.PDF_UA_2, EXCEPTION_SUPPLIER));
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
                    .PDF_UA_2, EXCEPTION_SUPPLIER));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ValidLangTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                catalog.SetLang(new PdfString("en-US"));
                Pdf20Checker checker = new Pdf20Checker(pdfDocument);
                NUnit.Framework.Assert.DoesNotThrow(() => checker.CheckLang(catalog));
            }
        }

        [NUnit.Framework.Test]
        public virtual void EmptyLangTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                catalog.SetLang(new PdfString(""));
                Pdf20Checker checker = new Pdf20Checker(pdfDocument);
                NUnit.Framework.Assert.DoesNotThrow(() => checker.CheckLang(catalog));
            }
        }

        [NUnit.Framework.Test]
        public virtual void InvalidLangTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.AddNewPage();
                PdfCatalog catalog = pdfDocument.GetCatalog();
                catalog.SetLang(new PdfString("inva:lid"));
                Pdf20Checker checker = new Pdf20Checker(pdfDocument);
                Exception e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => checker.CheckLang(catalog
                    ));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_SHALL_CONTAIN_VALID_LANG_ENTRY, e.
                    Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void RoleIsNotMappedToStandardNamespaceTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.SetTagged();
                PdfPage page = pdfDocument.AddNewPage();
                PdfStructElem doc = pdfDocument.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDocument, PdfName.Document
                    ));
                PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                doc.SetNamespace(@namespace);
                pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDocument, PdfName.P));
                paragraph.AddKid(new PdfStructElem(pdfDocument, new PdfName("chapter"), page));
                Pdf20Checker checker = new Pdf20Checker(pdfDocument);
                Exception e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => checker.CheckStructureTreeRoot
                    (pdfDocument.GetStructTreeRoot()));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                    , "chapter"), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void RoleWithNamespaceIsNotMappedToStandardNamespaceTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().
                SetPdfVersion(PdfVersion.PDF_2_0)))) {
                pdfDocument.SetTagged();
                PdfPage page = pdfDocument.AddNewPage();
                PdfStructElem doc = pdfDocument.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDocument, PdfName.Document
                    ));
                PdfNamespace namespace20 = new PdfNamespace(StandardNamespaces.PDF_2_0);
                doc.SetNamespace(namespace20);
                PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDocument, PdfName.P));
                PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDocument, new PdfName("chapter"), page));
                PdfNamespace @namespace = new PdfNamespace("http://www.w3.org/1999/xhtml");
                chapter.SetNamespace(@namespace);
                PdfNamespace otherNamespace = new PdfNamespace("http://www.w3.org/2000/svg");
                @namespace.AddNamespaceRoleMapping("chapter", "chapterChild", otherNamespace);
                otherNamespace.AddNamespaceRoleMapping("chapterChild", "chapterGrandchild");
                pdfDocument.GetStructTreeRoot().AddNamespace(namespace20);
                pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                pdfDocument.GetStructTreeRoot().AddNamespace(otherNamespace);
                Pdf20Checker checker = new Pdf20Checker(pdfDocument);
                Exception e = NUnit.Framework.Assert.Catch(typeof(Pdf20ConformanceException), () => checker.CheckStructureTreeRoot
                    (pdfDocument.GetStructTreeRoot()));
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                    , "chapter", @namespace.GetNamespaceName()), e.Message);
            }
        }
    }
}
