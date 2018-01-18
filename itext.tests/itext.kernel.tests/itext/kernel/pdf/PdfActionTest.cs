/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.IO.Util;
using iText.Kernel.Pdf.Action;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class PdfActionTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfActionTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfActionTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ActionTest01() {
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + "actionTest01.pdf"), true);
            document.GetCatalog().SetOpenAction(PdfAction.CreateURI("http://itextpdf.com/"));
            document.Close();
            System.Console.Out.WriteLine(MessageFormatUtil.Format("Please open document {0} and make sure that you're automatically redirected to {1} site."
                , destinationFolder + "actionTest01.pdf", "http://itextpdf.com"));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ActionTest02() {
            PdfDocument document = CreateDocument(new PdfWriter(destinationFolder + "actionTest02.pdf"), false);
            document.GetPage(2).SetAdditionalAction(PdfName.O, PdfAction.CreateURI("http://itextpdf.com/"));
            document.Close();
            System.Console.Out.WriteLine(MessageFormatUtil.Format("Please open document {0} at page 2 and make sure that you're automatically redirected to {1} site."
                , destinationFolder + "actionTest02.pdf", "http://itextpdf.com"));
        }

        private PdfDocument CreateDocument(PdfWriter writer, bool flushPages) {
            PdfDocument document = new PdfDocument(writer);
            PdfPage p1 = document.AddNewPage();
            PdfStream str1 = p1.GetFirstContentStream();
            str1.GetOutputStream().WriteString("1 0 0 rg 100 600 100 100 re f\n");
            if (flushPages) {
                p1.Flush();
            }
            PdfPage p2 = document.AddNewPage();
            PdfStream str2 = p2.GetFirstContentStream();
            str2.GetOutputStream().WriteString("0 1 0 rg 100 600 100 100 re f\n");
            if (flushPages) {
                p2.Flush();
            }
            PdfPage p3 = document.AddNewPage();
            PdfStream str3 = p3.GetFirstContentStream();
            str3.GetOutputStream().WriteString("0 0 1 rg 100 600 100 100 re f\n");
            if (flushPages) {
                p3.Flush();
            }
            return document;
        }
    }
}
