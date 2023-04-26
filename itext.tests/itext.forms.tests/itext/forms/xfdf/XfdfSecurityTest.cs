/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.IO;
using iText.Kernel.Exceptions;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Xfdf {
    [NUnit.Framework.Category("UnitTest")]
    public class XfdfSecurityTest : ExtendedITextTest {
        private const String XFDF_WITH_XXE = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> \n" + "<!DOCTYPE foo [ <!ENTITY xxe SYSTEM \"xxe-data.txt\" > ]>\n"
             + "<xfdf xmlns=\"http://ns.adobe.com/xfdf/\" xml:space=\"preserve\">\n" + "<f href=\"something.pdf\"/>\n"
             + "<fields\n" + "><field name=\"Input field\"\n" + "><value\n" + ">ABCDEFGH&xxe;</value\n" + "></field\n"
             + "></fields\n" + ">\n" + "<ids/>\n" + "</xfdf>";

        [NUnit.Framework.Test]
        public virtual void XxeVulnerabilityXfdfTest() {
            XmlProcessorCreator.SetXmlParserFactory(null);
            using (Stream inputStream = new MemoryStream(XFDF_WITH_XXE.GetBytes(System.Text.Encoding.UTF8))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => XfdfFileUtils.CreateXfdfDocumentFromStream
                    (inputStream));
                NUnit.Framework.Assert.AreEqual(ExceptionTestUtil.GetDoctypeIsDisallowedExceptionMessage(), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void XxeVulnerabilityXfdfCustomXmlParserTest() {
            XmlProcessorCreator.SetXmlParserFactory(new SecurityTestXmlParserFactory());
            using (Stream inputStream = new MemoryStream(XFDF_WITH_XXE.GetBytes(System.Text.Encoding.UTF8))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => XfdfFileUtils.CreateXfdfDocumentFromStream
                    (inputStream));
                NUnit.Framework.Assert.AreEqual("Test message", e.Message);
            }
        }
    }
}
