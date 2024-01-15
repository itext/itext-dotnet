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
