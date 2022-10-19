/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Canvas;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    /// <author>Michael Demey</author>
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfDocumentIdTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfDocumentTestID/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfDocumentTestID/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ChangeIdTest() {
            MemoryStream baos = new MemoryStream();
            String value = "Modified ID 1234";
            WriterProperties writerProperties = new WriterProperties();
            writerProperties.SetModifiedDocumentId(new PdfString((value)));
            PdfWriter writer = new PdfWriter(baos, writerProperties);
            PdfDocument pdfDocument = new PdfDocument(writer);
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            byte[] documentBytes = baos.ToArray();
            baos.Dispose();
            PdfReader reader = new PdfReader(new MemoryStream(documentBytes));
            pdfDocument = new PdfDocument(reader);
            PdfArray idArray = pdfDocument.GetTrailer().GetAsArray(PdfName.ID);
            NUnit.Framework.Assert.IsNotNull(idArray);
            String extractedValue = idArray.GetAsString(1).GetValue();
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(value, extractedValue);
        }

        [NUnit.Framework.Test]
        public virtual void ChangeIdTest02() {
            MemoryStream baos = new MemoryStream();
            IDigest md5;
            try {
                md5 = DigestUtilities.GetDigest("MD5");
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
            PdfString initialId = new PdfString(md5.Digest("Initial ID 56789".GetBytes()));
            PdfWriter writer = new PdfWriter(baos, new WriterProperties().SetInitialDocumentId(initialId));
            PdfDocument pdfDocument = new PdfDocument(writer);
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            byte[] documentBytes = baos.ToArray();
            baos.Dispose();
            PdfReader reader = new PdfReader(new MemoryStream(documentBytes));
            pdfDocument = new PdfDocument(reader);
            PdfArray idArray = pdfDocument.GetTrailer().GetAsArray(PdfName.ID);
            NUnit.Framework.Assert.IsNotNull(idArray);
            PdfString extractedString = idArray.GetAsString(1);
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(initialId, extractedString);
        }

        [NUnit.Framework.Test]
        public virtual void ChangeIdTest03() {
            MemoryStream baosInitial = new MemoryStream();
            MemoryStream baosModified = new MemoryStream();
            IDigest md5;
            try {
                md5 = DigestUtilities.GetDigest("MD5");
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
            PdfString initialId = new PdfString(md5.Digest("Initial ID 56789".GetBytes()));
            PdfString modifiedId = new PdfString("Modified ID 56789");
            PdfWriter writer = new PdfWriter(baosInitial, new WriterProperties().SetInitialDocumentId(initialId).SetModifiedDocumentId
                (modifiedId));
            PdfDocument pdfDocument = new PdfDocument(writer);
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            PdfReader reader = new PdfReader(new RandomAccessSourceFactory().CreateSource(baosInitial.ToArray()), new 
                ReaderProperties());
            pdfDocument = new PdfDocument(reader);
            PdfArray idArray = pdfDocument.GetTrailer().GetAsArray(PdfName.ID);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNotNull(idArray);
            PdfString extractedInitialValue = idArray.GetAsString(0);
            NUnit.Framework.Assert.AreEqual(initialId, extractedInitialValue);
            PdfString extractedModifiedValue = idArray.GetAsString(1);
            NUnit.Framework.Assert.AreEqual(modifiedId, extractedModifiedValue);
            pdfDocument = new PdfDocument(new PdfReader(new RandomAccessSourceFactory().CreateSource(baosInitial.ToArray
                ()), new ReaderProperties()), new PdfWriter(baosModified));
            new PdfCanvas(pdfDocument.AddNewPage()).SaveState().LineTo(100, 100).MoveTo(100, 100).Stroke().RestoreState
                ();
            pdfDocument.Close();
            reader = new PdfReader(new RandomAccessSourceFactory().CreateSource(baosModified.ToArray()), new ReaderProperties
                ());
            pdfDocument = new PdfDocument(reader);
            idArray = pdfDocument.GetTrailer().GetAsArray(PdfName.ID);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNotNull(idArray);
            extractedInitialValue = idArray.GetAsString(0);
            NUnit.Framework.Assert.AreEqual(initialId, extractedInitialValue);
            extractedModifiedValue = idArray.GetAsString(1);
            NUnit.Framework.Assert.AreNotEqual(modifiedId, extractedModifiedValue);
        }

        [NUnit.Framework.Test]
        public virtual void FetchReaderIdTest() {
            MemoryStream baos = new MemoryStream();
            IDigest md5;
            try {
                md5 = DigestUtilities.GetDigest("MD5");
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
            PdfString initialId = new PdfString(md5.Digest("Initial ID 56789".GetBytes()));
            PdfWriter writer = new PdfWriter(baos, new WriterProperties().SetInitialDocumentId(initialId));
            PdfDocument pdfDocument = new PdfDocument(writer);
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            byte[] documentBytes = baos.ToArray();
            baos.Dispose();
            PdfReader reader = new PdfReader(new MemoryStream(documentBytes));
            pdfDocument = new PdfDocument(reader);
            String firstOriginalId = iText.Commons.Utils.JavaUtil.GetStringForBytes(reader.GetOriginalFileId());
            String secondOriginalId = iText.Commons.Utils.JavaUtil.GetStringForBytes(reader.GetOriginalFileId());
            String firstModifiedId = iText.Commons.Utils.JavaUtil.GetStringForBytes(reader.GetModifiedFileId());
            String secondModifiedId = iText.Commons.Utils.JavaUtil.GetStringForBytes(reader.GetModifiedFileId());
            NUnit.Framework.Assert.AreEqual(firstOriginalId, secondOriginalId);
            NUnit.Framework.Assert.AreEqual(firstModifiedId, secondModifiedId);
        }

        [NUnit.Framework.Test]
        public virtual void WriterPropertiesPriorityTest() {
            MemoryStream baos = new MemoryStream();
            IDigest md5;
            try {
                md5 = DigestUtilities.GetDigest("MD5");
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
            PdfString originalId = new PdfString(md5.Digest("Initial ID 01234".GetBytes()));
            PdfString modifiedId = new PdfString(md5.Digest("Modified ID 56789".GetBytes()));
            PdfWriter writer = new PdfWriter(baos, new WriterProperties().SetInitialDocumentId(originalId).SetModifiedDocumentId
                (modifiedId));
            PdfDocument pdfDocument = new PdfDocument(writer);
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            byte[] documentBytes = baos.ToArray();
            baos.Dispose();
            PdfString newOriginalId = new PdfString(md5.Digest("Initial ID 98765".GetBytes()));
            PdfString newModifiedId = new PdfString(md5.Digest("Modified ID 43210".GetBytes()));
            PdfReader reader = new PdfReader(new MemoryStream(documentBytes));
            PdfWriter newWriter = new PdfWriter(new MemoryStream(), new WriterProperties().SetInitialDocumentId(newOriginalId
                ).SetModifiedDocumentId(newModifiedId));
            pdfDocument = new PdfDocument(reader, newWriter);
            String extractedOriginalId = pdfDocument.GetOriginalDocumentId().GetValue();
            String extractedModifiedId = pdfDocument.GetModifiedDocumentId().GetValue();
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(extractedOriginalId, newOriginalId.GetValue());
            NUnit.Framework.Assert.AreEqual(extractedModifiedId, newModifiedId.GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void ReadPdfWithTwoStringIdsTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "pdfWithTwoStringIds.pdf"));
            String originalId = null;
            String modifiedId = null;
            if (pdfDocument.GetOriginalDocumentId() != null) {
                originalId = pdfDocument.GetOriginalDocumentId().GetValue();
            }
            if (pdfDocument.GetModifiedDocumentId() != null) {
                modifiedId = pdfDocument.GetModifiedDocumentId().GetValue();
            }
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNotNull(originalId);
            NUnit.Framework.Assert.IsNotNull(modifiedId);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_IDS_ARE_CORRUPTED)]
        public virtual void ReadPdfWithTwoNumberIdsTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "pdfWithTwoNumberIds.pdf"));
            String originalId = null;
            String modifiedId = null;
            if (pdfDocument.GetOriginalDocumentId() != null) {
                originalId = pdfDocument.GetOriginalDocumentId().GetValue();
            }
            if (pdfDocument.GetModifiedDocumentId() != null) {
                modifiedId = pdfDocument.GetModifiedDocumentId().GetValue();
            }
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(originalId);
            NUnit.Framework.Assert.IsNull(modifiedId);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_IDS_ARE_CORRUPTED)]
        public virtual void ReadPdfWithOneNumberOneStringIdsTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "pdfWithOneNumberOneStringIds.pdf")
                );
            String originalId = null;
            String modifiedId = null;
            if (pdfDocument.GetOriginalDocumentId() != null) {
                originalId = pdfDocument.GetOriginalDocumentId().GetValue();
            }
            if (pdfDocument.GetModifiedDocumentId() != null) {
                modifiedId = pdfDocument.GetModifiedDocumentId().GetValue();
            }
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(originalId);
            NUnit.Framework.Assert.IsNotNull(modifiedId);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_IDS_ARE_CORRUPTED)]
        public virtual void ReadPdfWithOneStringIdValueTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "pdfWithOneStringId.pdf"));
            String originalId = null;
            String modifiedId = null;
            if (pdfDocument.GetOriginalDocumentId() != null) {
                originalId = pdfDocument.GetOriginalDocumentId().GetValue();
            }
            if (pdfDocument.GetModifiedDocumentId() != null) {
                modifiedId = pdfDocument.GetModifiedDocumentId().GetValue();
            }
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(originalId);
            NUnit.Framework.Assert.IsNull(modifiedId);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_IDS_ARE_CORRUPTED)]
        public virtual void ReadPdfWithOneNumberIdValueTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "pdfWithOneNumberId.pdf"));
            String originalId = null;
            String modifiedId = null;
            if (pdfDocument.GetOriginalDocumentId() != null) {
                originalId = pdfDocument.GetOriginalDocumentId().GetValue();
            }
            if (pdfDocument.GetModifiedDocumentId() != null) {
                modifiedId = pdfDocument.GetModifiedDocumentId().GetValue();
            }
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(originalId);
            NUnit.Framework.Assert.IsNull(modifiedId);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_IDS_ARE_CORRUPTED)]
        public virtual void ReadPdfWithNoIdTest() {
            PdfReader reader = new PdfReader(sourceFolder + "pdfWithNoId.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            String originalId = null;
            String modifiedId = null;
            if (pdfDocument.GetOriginalDocumentId() != null) {
                originalId = pdfDocument.GetOriginalDocumentId().GetValue();
            }
            if (pdfDocument.GetModifiedDocumentId() != null) {
                modifiedId = pdfDocument.GetModifiedDocumentId().GetValue();
            }
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(originalId);
            NUnit.Framework.Assert.IsNull(modifiedId);
            NUnit.Framework.Assert.AreEqual(0, reader.GetOriginalFileId().Length);
            NUnit.Framework.Assert.AreEqual(0, reader.GetModifiedFileId().Length);
        }

        [NUnit.Framework.Test]
        public virtual void ReadPdfWithNoIdAndConservativeReadingTest() {
            using (PdfReader reader = new PdfReader(sourceFolder + "pdfWithNoId.pdf").SetStrictnessLevel(PdfReader.StrictnessLevel
                .CONSERVATIVE)) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_IDS_ARE_CORRUPTED, e.Message);
            }
        }
        //    @Test
        //    public void appendModeTest() {
        //        String originalId;
        //        String newOriginalId;
        //        String appendModeNewOriginalId;
        //        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        //        PdfWriter initialWriter = new PdfWriter(baos, new WriterProperties().setInitialDocumentId(originalId));
        //        PdfWriter newWriter = new PdfWriter(baos, new WriterProperties().setInitialDocumentId(newOriginalId));
        //        PdfWriter appendModeWriter = new PdfWriter(baos, new WriterProperties().setInitialDocumentId(appendModeNewOriginalId));
        //
        //
        //    }
        //
        //    @Test
        //    public void encryptionAes128Test() {
        //        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        //        PdfString originalId = new PdfString("Original ID 56789");
        //        PdfWriter initialWriter = new PdfWriter(baos, new WriterProperties().setInitialDocumentId(originalId));
        //
        //        Assert.assertNotEquals();
        //        Assert.assertEquals();
        //
        //    }
    }
}
