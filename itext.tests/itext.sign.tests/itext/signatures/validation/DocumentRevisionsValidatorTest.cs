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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class DocumentRevisionsValidatorTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/DocumentRevisionsValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.Test]
        public virtual void MultipleRevisionsDocument() {
            DocumentRevisionsValidator validator = new DocumentRevisionsValidator();
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "multipleRevisionsDocument.pdf"
                ))) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport;
                using (Stream inputStream = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [0])) {
                    using (PdfDocument previousDocument = new PdfDocument(new PdfReader(inputStream))) {
                        validationReport = validator.ValidateRevision(document, previousDocument, documentRevisions[1]);
                    }
                }
                // Between these two revisions DSS is added, which is allowed, but also timestamp is added, which is not yet allowed.
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem1 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem1.GetCheckName());
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.NOT_ALLOWED_CATALOG_CHANGES, reportItem1.GetMessage
                    ());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem1.GetStatus());
                using (Stream inputStream_1 = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [1])) {
                    using (PdfDocument previousDocument_1 = new PdfDocument(new PdfReader(inputStream_1))) {
                        validationReport = validator.ValidateRevision(document, previousDocument_1, documentRevisions[2]);
                    }
                }
                // Between these two revisions only DSS is updated, which is allowed.
                NUnit.Framework.Assert.AreEqual(0, validationReport.GetFailures().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void HugeDocumentTest() {
            DocumentRevisionsValidator validator = new DocumentRevisionsValidator();
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "hugeDocument.pdf"))) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport;
                using (Stream inputStream = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [0])) {
                    using (PdfDocument previousDocument = new PdfDocument(new PdfReader(inputStream))) {
                        validationReport = validator.ValidateRevision(document, previousDocument, documentRevisions[1]);
                    }
                }
                NUnit.Framework.Assert.AreEqual(0, validationReport.GetFailures().Count);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ExtensionsModificationsTest() {
            DocumentRevisionsValidator validator = new DocumentRevisionsValidator();
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "extensionsModifications.pdf")
                )) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport;
                using (Stream inputStream = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [0])) {
                    using (PdfDocument previousDocument = new PdfDocument(new PdfReader(inputStream))) {
                        validationReport = validator.ValidateRevision(document, previousDocument, documentRevisions[1]);
                    }
                }
                NUnit.Framework.Assert.AreEqual(0, validationReport.GetFailures().Count);
                using (Stream inputStream_1 = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [1])) {
                    using (PdfDocument previousDocument_1 = new PdfDocument(new PdfReader(inputStream_1))) {
                        validationReport = validator.ValidateRevision(document, previousDocument_1, documentRevisions[2]);
                    }
                }
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem1 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem1.GetCheckName());
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(DocumentRevisionsValidator.DEVELOPER_EXTENSION_REMOVED
                    , PdfName.ESIC), reportItem1.GetMessage());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem1.GetStatus());
                using (Stream inputStream_2 = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [2])) {
                    using (PdfDocument previousDocument_2 = new PdfDocument(new PdfReader(inputStream_2))) {
                        validationReport = validator.ValidateRevision(document, previousDocument_2, documentRevisions[3]);
                    }
                }
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem2 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem2.GetCheckName());
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(DocumentRevisionsValidator.EXTENSION_LEVEL_DECREASED
                    , PdfName.ESIC), reportItem2.GetMessage());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem2.GetStatus());
                using (Stream inputStream_3 = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [3])) {
                    using (PdfDocument previousDocument_3 = new PdfDocument(new PdfReader(inputStream_3))) {
                        validationReport = validator.ValidateRevision(document, previousDocument_3, documentRevisions[4]);
                    }
                }
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem3 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem3.GetCheckName());
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(DocumentRevisionsValidator.DEVELOPER_EXTENSION_REMOVED
                    , PdfName.ESIC), reportItem3.GetMessage());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem3.GetStatus());
                using (Stream inputStream_4 = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [4])) {
                    using (PdfDocument previousDocument_4 = new PdfDocument(new PdfReader(inputStream_4))) {
                        validationReport = validator.ValidateRevision(document, previousDocument_4, documentRevisions[5]);
                    }
                }
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem4 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem4.GetCheckName());
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.EXTENSIONS_REMOVED, reportItem4.GetMessage());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem4.GetStatus());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CompletelyInvalidDocumentTest() {
            DocumentRevisionsValidator validator = new DocumentRevisionsValidator();
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "completelyInvalidDocument.pdf"
                ))) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport;
                using (Stream inputStream = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [0])) {
                    using (PdfDocument previousDocument = new PdfDocument(new PdfReader(inputStream))) {
                        validationReport = validator.ValidateRevision(document, previousDocument, documentRevisions[1]);
                    }
                }
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem.GetCheckName());
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.NOT_ALLOWED_CATALOG_CHANGES, reportItem.GetMessage
                    ());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem.GetStatus());
            }
        }

        [NUnit.Framework.Test]
        public virtual void MakePagesEntryDirectAndIndirectTest() {
            DocumentRevisionsValidator validator = new DocumentRevisionsValidator();
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "makePagesDirect.pdf"))) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport;
                using (Stream inputStream = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [0])) {
                    using (PdfDocument previousDocument = new PdfDocument(new PdfReader(inputStream))) {
                        validationReport = validator.ValidateRevision(document, previousDocument, documentRevisions[1]);
                    }
                }
                // Adobe Acrobat doesn't complain about such change. We consider this incorrect.
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem1 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem1.GetCheckName());
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.NOT_ALLOWED_CATALOG_CHANGES, reportItem1.GetMessage
                    ());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem1.GetStatus());
                using (Stream inputStream_1 = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [1])) {
                    using (PdfDocument previousDocument_1 = new PdfDocument(new PdfReader(inputStream_1))) {
                        validationReport = validator.ValidateRevision(document, previousDocument_1, documentRevisions[2]);
                    }
                }
                // Adobe Acrobat doesn't complain about such change. We consider this incorrect.
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem2 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem2.GetCheckName());
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.NOT_ALLOWED_CATALOG_CHANGES, reportItem2.GetMessage
                    ());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem2.GetStatus());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RandomEntryAddedTest() {
            DocumentRevisionsValidator validator = new DocumentRevisionsValidator();
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "randomEntryAdded.pdf"))) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport;
                using (Stream inputStream = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [0])) {
                    using (PdfDocument previousDocument = new PdfDocument(new PdfReader(inputStream))) {
                        validationReport = validator.ValidateRevision(document, previousDocument, documentRevisions[1]);
                    }
                }
                // Adobe Acrobat doesn't complain about such change. We consider this incorrect.
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem1 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem1.GetCheckName());
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.NOT_ALLOWED_CATALOG_CHANGES, reportItem1.GetMessage
                    ());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem1.GetStatus());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RandomEntryWithoutUsageTest() {
            DocumentRevisionsValidator validator = new DocumentRevisionsValidator();
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "randomEntryWithoutUsage.pdf")
                )) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport;
                using (Stream inputStream = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [0])) {
                    using (PdfDocument previousDocument = new PdfDocument(new PdfReader(inputStream))) {
                        validationReport = validator.ValidateRevision(document, previousDocument, documentRevisions[1]);
                    }
                }
                // Adobe Acrobat doesn't complain about such change. We consider this incorrect.
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem1 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem1.GetCheckName());
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(DocumentRevisionsValidator.UNEXPECTED_ENTRY_IN_XREF
                    , 16), reportItem1.GetMessage());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem1.GetStatus());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ChangeExistingFontTest() {
            DocumentRevisionsValidator validator = new DocumentRevisionsValidator();
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "changeExistingFont.pdf"))) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport;
                using (Stream inputStream = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [0])) {
                    using (PdfDocument previousDocument = new PdfDocument(new PdfReader(inputStream))) {
                        validationReport = validator.ValidateRevision(document, previousDocument, documentRevisions[1]);
                    }
                }
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem1 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem1.GetCheckName());
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.NOT_ALLOWED_CATALOG_CHANGES, reportItem1.GetMessage
                    ());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem1.GetStatus());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ChangeExistingFontAndAddAsDssTest() {
            DocumentRevisionsValidator validator = new DocumentRevisionsValidator();
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "changeExistingFontAndAddAsDss.pdf"
                ))) {
                PdfRevisionsReader revisionsReader = new PdfRevisionsReader(document.GetReader());
                IList<DocumentRevision> documentRevisions = revisionsReader.GetAllRevisions();
                ValidationReport validationReport;
                using (Stream inputStream = DocumentRevisionsValidator.CreateInputStreamFromRevision(document, documentRevisions
                    [0])) {
                    using (PdfDocument previousDocument = new PdfDocument(new PdfReader(inputStream))) {
                        validationReport = validator.ValidateRevision(document, previousDocument, documentRevisions[1]);
                    }
                }
                // Adobe Acrobat doesn't complain about such change. We consider this incorrect.
                NUnit.Framework.Assert.AreEqual(1, validationReport.GetFailures().Count);
                ReportItem reportItem1 = validationReport.GetFailures()[0];
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.DOC_MDP_CHECK, reportItem1.GetCheckName());
                NUnit.Framework.Assert.AreEqual(DocumentRevisionsValidator.NOT_ALLOWED_CATALOG_CHANGES, reportItem1.GetMessage
                    ());
                NUnit.Framework.Assert.AreEqual(ReportItem.ReportItemStatus.INVALID, reportItem1.GetStatus());
            }
        }
    }
}
