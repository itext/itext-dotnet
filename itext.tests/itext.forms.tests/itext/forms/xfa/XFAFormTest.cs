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
using System.Xml;
using System.Xml.Linq;
using iText.Forms;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using NUnit.Framework;

namespace iText.Forms.Xfa {
    public class XFAFormTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/xfa/XFAFormTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/xfa/XFAFormTest/";

        public static readonly String XML = sourceFolder + "xfa.xml";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CreateEmptyXFAFormTest01() {
            String outFileName = destinationFolder + "createEmptyXFAFormTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_createEmptyXFAFormTest01.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(outFileName));
            XfaForm xfa = new XfaForm(doc);
            XfaForm.SetXfaForm(xfa, doc);
            doc.AddNewPage();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateEmptyXFAFormTest02() {
            String outFileName = destinationFolder + "createEmptyXFAFormTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_createEmptyXFAFormTest02.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(outFileName));
            XfaForm xfa = new XfaForm();
            XfaForm.SetXfaForm(xfa, doc);
            doc.AddNewPage();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void CreateXFAFormTest() {
            String outFileName = destinationFolder + "createXFAFormTest.pdf";
            String cmpFileName = sourceFolder + "cmp_createXFAFormTest.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(outFileName));
            XfaForm xfa = new XfaForm(new FileStream(XML, FileMode.Open, FileAccess.Read));
            xfa.Write(doc);
            doc.AddNewPage();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void ReadXFAFormTest() {
            String inFileName = sourceFolder + "formTemplate.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            Assert.DoesNotThrow(() => PdfAcroForm.GetAcroForm(pdfDocument, true));
        }

        [NUnit.Framework.Test]
        public virtual void FindFieldName() {
            String inFileName = sourceFolder + "TextField1.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);
            XfaForm xfaForm = acroForm.GetXfaForm();
            xfaForm.FindFieldName("TextField1");
            String secondRun = xfaForm.FindFieldName("TextField1");
            NUnit.Framework.Assert.IsNotNull(secondRun);
        }

        [NUnit.Framework.Test]
        public virtual void FindFieldNameWithoutDataSet() {
            String inFileName = sourceFolder + "TextField1_empty.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(inFileName));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);
            XfaForm xfaForm = acroForm.GetXfaForm();
            String name = xfaForm.FindFieldName("TextField1");
            NUnit.Framework.Assert.IsNull(name);
        }

        [NUnit.Framework.Test]
        public virtual void ExtractXFADataTest() {
            String src = sourceFolder + "xfaFormWithDataSet.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(src));
            XfaForm xfa = new XfaForm(pdfDocument);
            XElement node = (XElement) xfa.FindDatasetsNode("Number1");
            NUnit.Framework.Assert.IsNotNull(node);
            NUnit.Framework.Assert.AreEqual("Number1", node.Name.LocalName);
        }
    }
}
