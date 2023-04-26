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
using iText.Forms;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms.Xfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class XfaSecurityTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/xfa/XfaSecurityTest/";

        private static readonly String DTD_EXCEPTION_MESSAGE = ExceptionTestUtil.GetDoctypeIsDisallowedExceptionMessage
            ();

        private const String XFA_WITH_DTD_XML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + "<!DOCTYPE foo>\n"
             + "\n" + "<xdp:xdp xmlns:xdp=\"http://ns.adobe.com/xdp/\" timeStamp=\"2018-03-08T12:50:19Z\"\n" + "         uuid=\"36ac5111-55c5-4172-b0c1-0cbd783e2fcf\">\n"
             + "</xdp:xdp>\n";

        [NUnit.Framework.SetUp]
        public virtual void ResetXmlParserFactoryToDefault() {
            XmlProcessorCreator.SetXmlParserFactory(null);
        }

        [NUnit.Framework.Test]
        public virtual void XfaExternalFileTest() {
            XfaSecurityExceptionTest(SOURCE_FOLDER + "xfaExternalFile.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void XfaExternalConnectionTest() {
            XfaSecurityExceptionTest(SOURCE_FOLDER + "xfaExternalConnection.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void XfaInternalEntityTest() {
            XfaSecurityExceptionTest(SOURCE_FOLDER + "xfaInternalEntity.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void XfaExternalFileCustomFactoryTest() {
            String inFileName = SOURCE_FOLDER + "xfaExternalFile.pdf";
            XmlProcessorCreator.SetXmlParserFactory(new SecurityTestXmlParserFactory());
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inFileName), new PdfWriter(new MemoryStream()))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfAcroForm.GetAcroForm(pdfDoc, true
                    ));
                NUnit.Framework.Assert.AreEqual(ExceptionTestUtil.GetXxeTestMessage(), e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfaExternalFileXfaFormTest() {
            String inFileName = SOURCE_FOLDER + "xfaExternalFile.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inFileName))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new XfaForm(pdfDoc));
                NUnit.Framework.Assert.AreEqual(DTD_EXCEPTION_MESSAGE, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void XfaWithDtdXfaFormTest() {
            using (Stream inputStream = new MemoryStream(XFA_WITH_DTD_XML.GetBytes(System.Text.Encoding.UTF8))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new XfaForm(inputStream));
                NUnit.Framework.Assert.AreEqual(DTD_EXCEPTION_MESSAGE, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void FillXfaFormTest() {
            using (Stream inputStream = new MemoryStream(XFA_WITH_DTD_XML.GetBytes(System.Text.Encoding.UTF8))) {
                XfaForm form = new XfaForm();
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => form.FillXfaForm(inputStream, true)
                    );
                NUnit.Framework.Assert.AreEqual(DTD_EXCEPTION_MESSAGE, e.Message);
            }
        }

        private void XfaSecurityExceptionTest(String inputFileName) {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(inputFileName), new PdfWriter(new MemoryStream()
                ))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfAcroForm.GetAcroForm(pdfDoc, true
                    ));
                NUnit.Framework.Assert.AreEqual(DTD_EXCEPTION_MESSAGE, e.Message);
            }
        }
    }
}
