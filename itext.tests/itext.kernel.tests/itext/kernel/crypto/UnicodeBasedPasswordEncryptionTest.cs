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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class UnicodeBasedPasswordEncryptionTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto/UnicodeBasedPasswordEncryptionTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/UnicodeBasedPasswordEncryptionTest/";

        private static IDictionary<String, UnicodeBasedPasswordEncryptionTest.SaslPreparedString> nameToSaslPrepared;

        static UnicodeBasedPasswordEncryptionTest() {
            // values are calculated with com.ibm.icu.text.StringPrep class in icu4j v58.2 lib
            nameToSaslPrepared = new LinkedDictionary<String, UnicodeBasedPasswordEncryptionTest.SaslPreparedString>();
            //الرحيم
            nameToSaslPrepared.Put("arabic01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u0627\u0644\u0631\u062D\u064A\u0645"
                , "\u0627\u0644\u0631\u062D\u064A\u0645"));
            //ال,ر11حيم
            nameToSaslPrepared.Put("arabic02", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u0627\u0644,\u063111\u062D\u064A\u0645"
                , "\u0627\u0644,\u063111\u062D\u064A\u0645"));
            // لـه
            nameToSaslPrepared.Put("arabic03", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u0644\u0640\u0647"
                , "\u0644\u0640\u0647"));
            // ﻻ
            nameToSaslPrepared.Put("arabic04", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\ufefb", "\u0644\u0627"
                ));
            // لا
            nameToSaslPrepared.Put("arabic05", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u0644\u0627"
                , "\u0644\u0627"));
            // शांति    देवनागरी
            nameToSaslPrepared.Put("devanagari01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u0936\u093e\u0902\u0924\u093f    \u0926\u0947\u0935\u0928\u093E\u0917\u0930\u0940"
                , "\u0936\u093E\u0902\u0924\u093F    \u0926\u0947\u0935\u0928\u093E\u0917\u0930\u0940"));
            // की प्राचीनतम
            nameToSaslPrepared.Put("devanagari02", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u0915\u0940 \u092A\u094D\u0930\u093E\u091A\u0940\u0928\u0924\u092E"
                , "\u0915\u0940 \u092A\u094D\u0930\u093E\u091A\u0940\u0928\u0924\u092E"));
            // ਗ੍ਰੰਥ ਸਾਹਿਬ
            nameToSaslPrepared.Put("gurmukhi01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u0A17\u0A4D\u0A30\u0A70\u0A25 \u0A38\u0A3E\u0A39\u0A3F\u0A2C"
                , "\u0A17\u0A4D\u0A30\u0A70\u0A25 \u0A38\u0A3E\u0A39\u0A3F\u0A2C"));
            // ញ្ចូ
            nameToSaslPrepared.Put("khmer01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u1789\u17D2\u1785\u17BC"
                , "\u1789\u17D2\u1785\u17BC"));
            //இலக்கிய நடை கூட மக்களால்
            nameToSaslPrepared.Put("tamil01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u0B87\u0BB2\u0B95\u0BCD\u0B95\u0BBF\u0BAF \u0BA8\u0B9F\u0BC8 \u0B95\u0BC2\u0B9F \u0BAE\u0B95\u0BCD\u0B95\u0BB3\u0BBE\u0BB2\u0BCD"
                , "\u0B87\u0BB2\u0B95\u0BCD\u0B95\u0BBF\u0BAF \u0BA8\u0B9F\u0BC8 \u0B95\u0BC2\u0B9F \u0BAE\u0B95\u0BCD\u0B95\u0BB3\u0BBE\u0BB2\u0BCD"
                ));
            // ประเทศไทย
            nameToSaslPrepared.Put("thai01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u0E1B\u0E23\u0E30\u0E40\u0E17\u0E28\u0E44\u0E17\u0E22"
                , "\u0E1B\u0E23\u0E30\u0E40\u0E17\u0E28\u0E44\u0E17\u0E22"));
            nameToSaslPrepared.Put("unicodeBom01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\uFEFFab\uFEFFc"
                , "abc"));
            nameToSaslPrepared.Put("emoji01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u267B", "\u267B"
                ));
            nameToSaslPrepared.Put("rfc4013Example01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("I\u00ADX"
                , "IX"));
            nameToSaslPrepared.Put("rfc4013Example02", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("user"
                , "user"));
            nameToSaslPrepared.Put("rfc4013Example03", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u00AA"
                , "a"));
            // match rfc4013Example01
            nameToSaslPrepared.Put("rfc4013Example04", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u2168"
                , "IX"));
            nameToSaslPrepared.Put("nonAsciiSpace01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u2008 \u2009 \u200A \u200B"
                , "       "));
            // normalization tests
            nameToSaslPrepared.Put("nfkcNormalization01", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u09C7\u09BE"
                , "\u09CB"));
            nameToSaslPrepared.Put("nfkcNormalization02", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u30AD\u3099\u30AB\u3099"
                , "\u30AE\u30AC"));
            nameToSaslPrepared.Put("nfkcNormalization03", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u3310"
                , "\u30AE\u30AC"));
            nameToSaslPrepared.Put("nfkcNormalization04", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\u1100\u1161\u11A8"
                , "\uAC01"));
            nameToSaslPrepared.Put("nfkcNormalization05", new UnicodeBasedPasswordEncryptionTest.SaslPreparedString("\uF951"
                , "\u964B"));
        }

        /*
        
        // Arabic
        bidirectional check fail:  "\u0627\u0644\u0631\u0651\u064E\u200C\u062D\u0652\u0645\u064E\u0640\u0670\u0646\u0650"
        bidirectional check fail:  "1\u0627\u0644\u0631\u062D\u064A\u06452"
        
        // RFC4013 examples
        bidirectional check fail:  "\u0627\u0031"
        prohibited character fail: "\u0007"
        
        // unassigned code point for Unicode 3.2
        "\uD83E\uDD14"
        "\u038Ba\u038Db\u03A2c\u03CF"
        
        */
        private class SaslPreparedString {
            internal String unicodeInputString;

            internal String preparedString;

            internal SaslPreparedString(String unicodeInputString, String preparedString) {
                this.unicodeInputString = unicodeInputString;
                this.preparedString = preparedString;
            }
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void Aes256EncryptedPdfWithUnicodeBasedPassword() {
            String fileNameTemplate = "unicodePassword_";
            foreach (KeyValuePair<String, UnicodeBasedPasswordEncryptionTest.SaslPreparedString> entry in nameToSaslPrepared
                ) {
                String filename = fileNameTemplate + entry.Key + ".pdf";
                byte[] ownerPassword = entry.Value.preparedString.GetBytes(System.Text.Encoding.UTF8);
                EncryptAes256AndCheck(filename, ownerPassword);
            }
        }

        // TODO after DEVSIX-1220 finished:
        // 1.  Create with both inputString and prepareString.
        // 1.1 Check opening both of these documents with both strings.
        // 2.  Try encrypt document with invalid input string.
        // 3.  Try open encrypted document with password that contains unassigned code points and ensure error is due to wrong password instead of the invalid input string.
        private void EncryptAes256AndCheck(String filename, byte[] ownerPassword) {
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            WriterProperties writerProperties = new WriterProperties().SetStandardEncryption(PdfEncryptionTest.USER, ownerPassword
                , permissions, EncryptionConstants.ENCRYPTION_AES_256).SetPdfVersion(PdfVersion.PDF_2_0);
            PdfWriter writer = new PdfWriter(destinationFolder + filename, writerProperties.AddXmpMetadata());
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetMoreInfo(PdfEncryptionTest.customInfoEntryKey, PdfEncryptionTest.customInfoEntryValue
                );
            PdfPage page = document.AddNewPage();
            PdfEncryptionTest.WriteTextBytesOnPageContent(page, PdfEncryptionTest.pageTextContent);
            page.Flush();
            document.Close();
            PdfEncryptionTest.CheckDecryptedWithPasswordContent(destinationFolder + filename, ownerPassword, PdfEncryptionTest
                .pageTextContent);
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + 
                filename, destinationFolder, "diff_", ownerPassword, ownerPassword);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }
    }
}
