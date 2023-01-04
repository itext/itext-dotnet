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
