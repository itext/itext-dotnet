/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfStringTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfStringTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfStringTest/";

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestPdfDocumentInfoStringEncoding01() {
            String fileName = "testPdfDocumentInfoStringEncoding01.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(destinationFolder + fileName, new WriterProperties
                ().SetCompressionLevel(CompressionConstants.NO_COMPRESSION)));
            pdfDocument.AddNewPage();
            String author = "Алексей";
            String title = "Заголовок";
            String subject = "Тема";
            String keywords = "Ключевые слова";
            String creator = "English text";
            pdfDocument.GetDocumentInfo().SetAuthor(author);
            pdfDocument.GetDocumentInfo().SetTitle(title);
            pdfDocument.GetDocumentInfo().SetSubject(subject);
            pdfDocument.GetDocumentInfo().SetKeywords(keywords);
            pdfDocument.GetDocumentInfo().SetCreator(creator);
            pdfDocument.Close();
            PdfDocument readDoc = new PdfDocument(new PdfReader(destinationFolder + fileName));
            NUnit.Framework.Assert.AreEqual(author, readDoc.GetDocumentInfo().GetAuthor());
            NUnit.Framework.Assert.AreEqual(title, readDoc.GetDocumentInfo().GetTitle());
            NUnit.Framework.Assert.AreEqual(subject, readDoc.GetDocumentInfo().GetSubject());
            NUnit.Framework.Assert.AreEqual(keywords, readDoc.GetDocumentInfo().GetKeywords());
            NUnit.Framework.Assert.AreEqual(creator, readDoc.GetDocumentInfo().GetCreator());
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestUnicodeString() {
            String unicode = "Привет!";
            PdfString @string = new PdfString(unicode);
            NUnit.Framework.Assert.AreNotEqual(unicode, @string.ToUnicodeString());
        }
    }
}
