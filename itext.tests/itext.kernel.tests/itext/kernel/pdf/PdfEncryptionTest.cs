using System;
using System.IO;
using iTextSharp.IO.Font;
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Kernel.Pdf {
    public class PdfEncryptionTest : ExtendedITextTest {
        /// <summary>User password.</summary>
        public static byte[] USER = "Hello".GetBytes();

        /// <summary>Owner password.</summary>
        public static byte[] OWNER = "World".GetBytes();

        internal const String author = "Alexander Chingarev";

        internal const String creator = "iText 6";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itextsharp/kernel/pdf/PdfEncryptionTest/";

        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/../../resources/itextsharp/kernel/pdf/PdfEncryptionTest/";

        [NUnit.Framework.TestFixtureSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iTextSharp.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordStandard128() {
            String filename = "encryptWithPasswordStandard128.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iTextSharp.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordStandard40() {
            String filename = "encryptWithPasswordStandard40.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iTextSharp.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordStandard128NoCompression() {
            String filename = "encryptWithPasswordStandard128NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iTextSharp.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordStandard40NoCompression() {
            String filename = "encryptWithPasswordStandard40NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iTextSharp.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordAes128() {
            String filename = "encryptWithPasswordAes128.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iTextSharp.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordAes256() {
            String filename = "encryptWithPasswordAes256.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iTextSharp.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordAes128NoCompression() {
            String filename = "encryptWithPasswordAes128NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iTextSharp.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordAes256NoCompression() {
            String filename = "encryptWithPasswordAes256NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="iTextSharp.Kernel.XMP.XMPException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        public virtual void EncryptWithPassword(String filename, int encryptionType, int compression) {
            String outFileName = destinationFolder + filename;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            PdfWriter writer = new PdfWriter(new FileStream(outFileName, FileMode.Create), new WriterProperties().SetStandardEncryption
                (USER, OWNER, permissions, encryptionType).AddXmpMetadata());
            writer.SetCompressionLevel(compression);
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetAuthor(author).SetCreator(creator);
            PdfPage page = document.AddNewPage();
            page.GetFirstContentStream().GetOutputStream().WriteBytes(("q\n" + "BT\n" + "36 706 Td\n" + "0 0 Td\n" + "/F1 24 Tf\n"
                 + "(Hello world!)Tj\n" + "0 0 Td\n" + "ET\n" + "Q ").GetBytes());
            page.GetResources().AddFont(document, PdfFontFactory.CreateFont(FontConstants.HELVETICA));
            page.Flush();
            document.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
            CheckDecryptedContent(filename, OWNER, "(Hello world!)");
            CheckDecryptedContent(filename, USER, "(Hello world!)");
            CheckDocumentStamping(filename, OWNER);
            CheckDocumentAppending(filename, OWNER);
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual void CheckDecryptedContent(String filename, byte[] password, String pageContent) {
            String src = destinationFolder + filename;
            PdfReader reader = new PdfReader(src, new ReaderProperties().SetPassword(password));
            PdfDocument document = new PdfDocument(reader);
            PdfPage page = document.GetPage(1);
            NUnit.Framework.Assert.IsTrue(iTextSharp.IO.Util.JavaUtil.GetStringForBytes(page.GetStreamBytes(0)).Contains
                (pageContent), "Expected content: \n" + pageContent);
            NUnit.Framework.Assert.AreEqual(author, document.GetDocumentInfo().GetAuthor(), "Encrypted author");
            NUnit.Framework.Assert.AreEqual(creator, document.GetDocumentInfo().GetCreator(), "Encrypted creator");
            document.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        public virtual void CheckDocumentStamping(String filename, byte[] password) {
            String srcFileName = destinationFolder + filename;
            String outFileName = destinationFolder + "stamped_" + filename;
            PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(password));
            PdfDocument document = new PdfDocument(reader, new PdfWriter(outFileName));
            document.Close();
            CompareTool compareTool = new CompareTool();
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        public virtual void CheckDocumentAppending(String filename, byte[] password) {
            String srcFileName = destinationFolder + filename;
            String outFileName = destinationFolder + "appended_" + filename;
            PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(password));
            PdfDocument document = new PdfDocument(reader, new PdfWriter(outFileName), new StampingProperties().UseAppendMode
                ());
            PdfPage newPage = document.AddNewPage();
            newPage.Put(PdfName.Default, new PdfString("Hello world string"));
            document.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_appended_" + filename
                , destinationFolder, "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }
    }
}
