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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Logs;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Validation;
using iText.Kernel.Validation.Context;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class PdfDocumentUnitTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfDocumentUnitTest/";

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void GetFontWithDirectFontDictionaryTest() {
            PdfDictionary initialFontDict = new PdfDictionary();
            initialFontDict.Put(PdfName.Subtype, PdfName.Type3);
            initialFontDict.Put(PdfName.FontMatrix, new PdfArray(new float[] { 0.001F, 0, 0, 0.001F, 0, 0 }));
            initialFontDict.Put(PdfName.Widths, new PdfArray());
            PdfDictionary encoding = new PdfDictionary();
            initialFontDict.Put(PdfName.Encoding, encoding);
            PdfArray differences = new PdfArray();
            differences.Add(new PdfNumber(AdobeGlyphList.NameToUnicode("a")));
            differences.Add(new PdfName("a"));
            encoding.Put(PdfName.Differences, differences);
            NUnit.Framework.Assert.IsNull(initialFontDict.GetIndirectReference());
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                // prevent no pages exception on close
                doc.AddNewPage();
                PdfType3Font font1 = (PdfType3Font)doc.GetFont(initialFontDict);
                NUnit.Framework.Assert.IsNotNull(font1);
                // prevent no glyphs for type3 font on close
                font1.AddGlyph('a', 0, 0, 0, 0, 0);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesWithOCGDifferentNames() {
            IList<IList<String>> ocgNames = new List<IList<String>>();
            IList<String> ocgNames1 = new List<String>();
            ocgNames1.Add("Name1");
            IList<String> ocgNames2 = new List<String>();
            ocgNames2.Add("Name2");
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames2);
            IList<byte[]> sourceDocuments = PdfDocumentUnitTest.InitSourceDocuments(ocgNames);
            using (PdfDocument outDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                foreach (byte[] docBytes in sourceDocuments) {
                    using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                        for (int i = 1; i <= fromDocument.GetNumberOfPages(); i++) {
                            fromDocument.CopyPagesTo(i, i, outDocument);
                        }
                    }
                }
                IList<String> layerNames = new List<String>();
                layerNames.Add("Name1");
                layerNames.Add("Name2");
                PdfDocumentUnitTest.AssertLayerNames(outDocument, layerNames);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_HAS_CONFLICTING_OCG_NAMES, Count = 3)]
        public virtual void CopyPagesWithOCGSameName() {
            IList<IList<String>> ocgNames = new List<IList<String>>();
            IList<String> ocgNames1 = new List<String>();
            ocgNames1.Add("Name1");
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames1);
            ocgNames.Add(ocgNames1);
            IList<byte[]> sourceDocuments = PdfDocumentUnitTest.InitSourceDocuments(ocgNames);
            using (PdfDocument outDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                foreach (byte[] docBytes in sourceDocuments) {
                    using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                        for (int i = 1; i <= fromDocument.GetNumberOfPages(); i++) {
                            fromDocument.CopyPagesTo(i, i, outDocument);
                        }
                    }
                }
                IList<String> layerNames = new List<String>();
                layerNames.Add("Name1");
                layerNames.Add("Name1_0");
                layerNames.Add("Name1_1");
                layerNames.Add("Name1_2");
                PdfDocumentUnitTest.AssertLayerNames(outDocument, layerNames);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesWithOCGSameObject() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = document.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfDictionary ocg = new PdfDictionary();
                    ocg.Put(PdfName.Type, PdfName.OCG);
                    ocg.Put(PdfName.Name, new PdfString("name1"));
                    ocg.MakeIndirect(document);
                    pdfResource.AddProperties(ocg);
                    PdfPage page2 = document.AddNewPage();
                    PdfResources pdfResource2 = page2.GetResources();
                    pdfResource2.AddProperties(ocg);
                    document.GetCatalog().GetOCProperties(true);
                }
                docBytes = outputStream.ToArray();
            }
            using (PdfDocument outDocument = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                    fromDocument.CopyPagesTo(1, fromDocument.GetNumberOfPages(), outDocument);
                }
                IList<String> layerNames = new List<String>();
                layerNames.Add("name1");
                PdfDocumentUnitTest.AssertLayerNames(outDocument, layerNames);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCG_COPYING_ERROR, LogLevel = LogLevelConstants.ERROR)]
        public virtual void CopyPagesFlushedResources() {
            byte[] docBytes;
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = document.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    PdfDictionary ocg = new PdfDictionary();
                    ocg.Put(PdfName.Type, PdfName.OCG);
                    ocg.Put(PdfName.Name, new PdfString("name1"));
                    ocg.MakeIndirect(document);
                    pdfResource.AddProperties(ocg);
                    pdfResource.MakeIndirect(document);
                    PdfPage page2 = document.AddNewPage();
                    page2.SetResources(pdfResource);
                    document.GetCatalog().GetOCProperties(true);
                }
                docBytes = outputStream.ToArray();
            }
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            using (PdfDocument outDocument = new PdfDocument(writer)) {
                using (PdfDocument fromDocument = new PdfDocument(new PdfReader(new MemoryStream(docBytes)))) {
                    fromDocument.CopyPagesTo(1, 1, outDocument);
                    IList<String> layerNames = new List<String>();
                    layerNames.Add("name1");
                    PdfDocumentUnitTest.AssertLayerNames(outDocument, layerNames);
                    outDocument.FlushCopiedObjects(fromDocument);
                    fromDocument.CopyPagesTo(2, 2, outDocument);
                    NUnit.Framework.Assert.IsNotNull(outDocument.GetCatalog());
                    PdfOCProperties ocProperties = outDocument.GetCatalog().GetOCProperties(false);
                    NUnit.Framework.Assert.IsNotNull(ocProperties);
                    NUnit.Framework.Assert.AreEqual(1, ocProperties.GetLayers().Count);
                    PdfLayer layer = ocProperties.GetLayers()[0];
                    NUnit.Framework.Assert.IsTrue(layer.GetPdfObject().IsFlushed());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetDocumentInfoAlreadyClosedTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "pdfWithMetadata.pdf"));
            pdfDocument.Close();
            NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDocument.GetDocumentInfo());
        }

        [NUnit.Framework.Test]
        public virtual void GetDocumentInfoInitializationTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "pdfWithMetadata.pdf"));
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetDocumentInfo());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void GetPdfAConformanceLevelInitializationTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "pdfWithMetadata.pdf"));
            NUnit.Framework.Assert.IsTrue(pdfDocument.reader.GetPdfConformance().IsPdfAOrUa());
            pdfDocument.Close();
        }

        private static void AssertLayerNames(PdfDocument outDocument, IList<String> layerNames) {
            NUnit.Framework.Assert.IsNotNull(outDocument.GetCatalog());
            PdfOCProperties ocProperties = outDocument.GetCatalog().GetOCProperties(true);
            NUnit.Framework.Assert.IsNotNull(ocProperties);
            NUnit.Framework.Assert.AreEqual(layerNames.Count, ocProperties.GetLayers().Count);
            for (int i = 0; i < layerNames.Count; i++) {
                PdfLayer layer = ocProperties.GetLayers()[i];
                NUnit.Framework.Assert.IsNotNull(layer);
                PdfDocumentUnitTest.AssertLayerNameEqual(layerNames[i], layer);
            }
        }

        private static IList<byte[]> InitSourceDocuments(IList<IList<String>> ocgNames) {
            IList<byte[]> result = new List<byte[]>();
            foreach (IList<String> names in ocgNames) {
                result.Add(PdfDocumentUnitTest.InitDocument(names));
            }
            return result;
        }

        private static byte[] InitDocument(IList<String> names) {
            using (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
                using (PdfDocument document = new PdfDocument(new PdfWriter(outputStream))) {
                    PdfPage page = document.AddNewPage();
                    PdfResources pdfResource = page.GetResources();
                    foreach (String name in names) {
                        PdfDictionary ocg = new PdfDictionary();
                        ocg.Put(PdfName.Type, PdfName.OCG);
                        ocg.Put(PdfName.Name, new PdfString(name));
                        ocg.MakeIndirect(document);
                        pdfResource.AddProperties(ocg);
                    }
                    document.GetCatalog().GetOCProperties(true);
                }
                return outputStream.ToArray();
            }
        }

        private static void AssertLayerNameEqual(String name, PdfLayer layer) {
            PdfDictionary layerDictionary = layer.GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(layerDictionary);
            NUnit.Framework.Assert.IsNotNull(layerDictionary.Get(PdfName.Name));
            String layerNameString = layerDictionary.Get(PdfName.Name).ToString();
            NUnit.Framework.Assert.AreEqual(name, layerNameString);
        }

        [NUnit.Framework.Test]
        public virtual void CannotGetTagStructureForUntaggedDocumentTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc.GetTagStructureContext
                ());
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.MUST_BE_A_TAGGED_DOCUMENT, exception.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CannotAddPageAfterDocumentIsClosedTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.AddNewPage(1);
            pdfDoc.Close();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc.AddNewPage(2));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DOCUMENT_CLOSED_IT_IS_IMPOSSIBLE_TO_EXECUTE_ACTION
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotMovePageToZeroPositionTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.AddNewPage();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => pdfDoc.MovePage
                (1, 0));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.REQUESTED_PAGE_NUMBER_IS_OUT_OF_BOUNDS
                , 0), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotMovePageToNegativePosition() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.AddNewPage();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => pdfDoc.MovePage
                (1, -1));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.REQUESTED_PAGE_NUMBER_IS_OUT_OF_BOUNDS
                , -1), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotMovePageToOneMorePositionThanPagesNumberTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc.AddNewPage();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => pdfDoc.MovePage
                (1, 3));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.REQUESTED_PAGE_NUMBER_IS_OUT_OF_BOUNDS
                , 3), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotAddPageToAnotherDocumentTest() {
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc1.AddNewPage(1);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc2.CheckAndAddPage(1, 
                pdfDoc1.GetPage(1)));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.PAGE_CANNOT_BE_ADDED_TO_DOCUMENT_BECAUSE_IT_BELONGS_TO_ANOTHER_DOCUMENT
                , pdfDoc1, 1, pdfDoc2), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotAddPageToAnotherDocTest() {
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            pdfDoc1.AddNewPage(1);
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc2.CheckAndAddPage(pdfDoc1
                .GetPage(1)));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.PAGE_CANNOT_BE_ADDED_TO_DOCUMENT_BECAUSE_IT_BELONGS_TO_ANOTHER_DOCUMENT
                , pdfDoc1, 1, pdfDoc2), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CannotSetEncryptedPayloadInReadingModeTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "setEncryptedPayloadInReadingModeTest.pdf"
                ));
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc.SetEncryptedPayload(
                null));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_SET_ENCRYPTED_PAYLOAD_TO_DOCUMENT_OPENED_IN_READING_MODE
                , exception.Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void CannotSetEncryptedPayloadToEncryptedDocTest() {
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetStandardEncryption(new byte[] {  }, new byte[] {  }, 1, 1);
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream(), writerProperties));
            PdfFileSpec fs = PdfFileSpec.CreateExternalFileSpec(pdfDoc, SOURCE_FOLDER + "testPath");
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc.SetEncryptedPayload(
                fs));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_SET_ENCRYPTED_PAYLOAD_TO_ENCRYPTED_DOCUMENT
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckEmptyIsoConformanceTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                IValidationContext validationContext = new PdfDocumentValidationContext(doc, doc.GetDocumentFonts());
                NUnit.Framework.Assert.DoesNotThrow(() => doc.CheckIsoConformance(validationContext));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckIsoConformanceTest() {
            using (PdfDocument doc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                ValidationContainer container = new ValidationContainer();
                PdfDocumentUnitTest.CustomValidationChecker checker = new PdfDocumentUnitTest.CustomValidationChecker();
                container.AddChecker(checker);
                doc.GetDiContainer().Register(typeof(ValidationContainer), container);
                NUnit.Framework.Assert.IsFalse(checker.documentValidationPerformed);
                IValidationContext validationContext = new PdfDocumentValidationContext(doc, doc.GetDocumentFonts());
                doc.CheckIsoConformance(validationContext);
                NUnit.Framework.Assert.IsTrue(checker.documentValidationPerformed);
            }
        }

        private class CustomValidationChecker : IValidationChecker {
            public bool documentValidationPerformed = false;

            public virtual void Validate(IValidationContext validationContext) {
                if (validationContext.GetType() == ValidationType.PDF_DOCUMENT) {
                    documentValidationPerformed = true;
                }
            }
        }
    }
}
