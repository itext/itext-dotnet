/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Globalization;
using System.Threading;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Test;
using NUnit.Framework;

namespace iText.Layout {
    // This test is present only in c#
    // Also this test in only for windows OS 
    public class NetWorkPathTest : ExtendedITextTest {
        
        [NUnit.Framework.Test]
        public virtual void NetworkPathImageTest() {
            String fullImagePath = @"\\someVeryRandomWords\SomeVeryRandomName.img";
            String startOfMsg = null;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            try {
                Image drawing = new Image(ImageDataFactory.Create(fullImagePath));
            } catch (Exception e) {
                if (e.InnerException != null && e.InnerException.Message.Length > 18)
                    startOfMsg = e.InnerException.Message.Substring(0, 19);
            }
            NUnit.Framework.Assert.IsNotNull(startOfMsg);
            NUnit.Framework.Assert.AreNotEqual("Could not find file", startOfMsg);
        }
        
        [NUnit.Framework.Test]
        [Ignore("Manual run only")]
        public virtual void NetworkPathImageTest02() {
            // TODO This test can work only if shared folder exists on some local network computer.
            // Suggested apporach is to create such folder on your computer and input corresponding names as variables values below.
            String comupterNameAndSharedFolderPath = @"INSERT_YOUR_COMPUTER_NAME"; // e.g. \\DESKTOP-ABCD3TQ\_inbox
            String outPath = "INSERT_OUTPUT_PATH";
            
            String fullImagePath = @"\\" + comupterNameAndSharedFolderPath + @"\img.jpg";
            Image drawing = new Image(ImageDataFactory.Create(fullImagePath));
            Document doc = new Document(new PdfDocument(new PdfWriter(outPath)));
            doc.Add(drawing.SetAutoScale(true));
            doc.Close();
        }
    }
}
