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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace iText.Kernel.Crypto.Pdfencryption {
    public class PdfEncryptionTestUtils {
        private readonly String destinationFolder;

        private readonly String sourceFolder;

        public const String PAGE_TEXT_CONTENT = "Hello world!";

        public const String CUSTOM_INFO_ENTRY_KEY = "Custom";

        public const String CUSTOM_INFO_ENTRY_VALUE = "String";

        /// <summary>User password.</summary>
        public static byte[] USER = "Hello".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);

        /// <summary>Owner password.</summary>
        public static byte[] OWNER = "World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);

        public PdfEncryptionTestUtils(String destinationFolder, String sourceFolder) {
            this.destinationFolder = destinationFolder;
            this.sourceFolder = sourceFolder;
        }

        public virtual void CompareEncryptedPdf(String filename) {
            CheckDecryptedWithPasswordContent(destinationFolder + filename, OWNER, PAGE_TEXT_CONTENT);
            CheckDecryptedWithPasswordContent(destinationFolder + filename, USER, PAGE_TEXT_CONTENT);
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare(false);
            String compareResult = compareTool.CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + 
                filename, destinationFolder, "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        public virtual void CheckDecryptedWithPasswordContent(String src, byte[] password, String pageContent) {
            CheckDecryptedWithPasswordContent(src, password, pageContent, false);
        }

        public virtual void CheckDecryptedWithPasswordContent(String src, byte[] password, String pageContent, bool
             expectError) {
            PdfReader reader = CompareTool.CreateOutputReader(src, new ReaderProperties().SetPassword(password));
            PdfDocument document = new PdfDocument(reader);
            PdfPage page = document.GetPage(1);
            bool expectedContentFound = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetStreamBytes(0)).Contains
                (pageContent);
            String actualCustomInfoEntry = document.GetTrailer().GetAsDictionary(PdfName.Info).GetAsString(new PdfName
                (CUSTOM_INFO_ENTRY_KEY)).ToUnicodeString();
            if (!expectError) {
                NUnit.Framework.Assert.IsTrue(expectedContentFound, "Expected content: \n" + pageContent);
                NUnit.Framework.Assert.AreEqual(CUSTOM_INFO_ENTRY_VALUE, actualCustomInfoEntry, "Encrypted custom");
            }
            else {
                NUnit.Framework.Assert.IsFalse(expectedContentFound, "Expected content: \n" + pageContent);
                NUnit.Framework.Assert.AreNotEqual(CUSTOM_INFO_ENTRY_VALUE, actualCustomInfoEntry, "Encrypted custom");
            }
            document.Close();
        }

        public static void WriteTextBytesOnPageContent(PdfPage page, String text) {
            page.GetFirstContentStream().GetOutputStream().WriteBytes(("q\n" + "BT\n" + "36 706 Td\n" + "0 0 Td\n" + "/F1 24 Tf\n"
                 + "(" + text + ")Tj\n" + "0 0 Td\n" + "ET\n" + "Q ").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ));
            page.GetResources().AddFont(page.GetDocument(), PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
        }
    }
}
