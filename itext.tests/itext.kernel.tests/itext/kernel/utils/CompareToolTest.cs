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
using iText.Test;

namespace iText.Kernel.Utils {
    public class CompareToolTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/CompareToolTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/utils/CompareToolTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUp() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void CompareToolErrorReportTest01() {
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            String outPdf = sourceFolder + "simple_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_simple_pdf.pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "difference");
            System.Console.Out.WriteLine(result);
            NUnit.Framework.Assert.IsNotNull(result, "CompareTool must return differences found between the files");
            // Comparing the report to the reference one.
            NUnit.Framework.Assert.IsTrue(compareTool.CompareXmls(sourceFolder + "cmp_report01.xml", destinationFolder
                 + "simple_pdf.report.xml"), "CompareTool report differs from the reference one");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void CompareToolErrorReportTest02() {
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            String outPdf = sourceFolder + "tagged_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_tagged_pdf.pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "difference");
            System.Console.Out.WriteLine(result);
            NUnit.Framework.Assert.IsNotNull(result, "CompareTool must return differences found between the files");
            // Comparing the report to the reference one.
            NUnit.Framework.Assert.IsTrue(compareTool.CompareXmls(sourceFolder + "cmp_report02.xml", destinationFolder
                 + "tagged_pdf.report.xml"), "CompareTool report differs from the reference one");
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void CompareToolErrorReportTest03() {
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            String outPdf = sourceFolder + "screenAnnotation.pdf";
            String cmpPdf = sourceFolder + "cmp_screenAnnotation.pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "difference");
            System.Console.Out.WriteLine(result);
            NUnit.Framework.Assert.IsNotNull(result, "CompareTool must return differences found between the files");
            // Comparing the report to the reference one.
            NUnit.Framework.Assert.IsTrue(compareTool.CompareXmls(sourceFolder + "cmp_report03.xml", destinationFolder
                 + "screenAnnotation.report.xml"), "CompareTool report differs from the reference one");
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        [NUnit.Framework.Test]
        public virtual void CompareToolErrorReportTest04() {
            // Test space in name
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            String outPdf = sourceFolder + "simple_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_simple_pdf_with_space .pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "difference");
            System.Console.Out.WriteLine(result);
            NUnit.Framework.Assert.IsNotNull(result, "CompareTool must return differences found between the files");
            // Comparing the report to the reference one.
            NUnit.Framework.Assert.IsTrue(compareTool.CompareXmls(sourceFolder + "cmp_report01.xml", destinationFolder
                 + "simple_pdf.report.xml"), "CompareTool report differs from the reference one");
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void DifferentProducerTest() {
            String expectedMessage = "Document info fail. Expected: \"iText\u00ae <version> \u00a9<copyright years> iText Group NV (iText Software; licensed version)\", actual: \"iText\u00ae <version> \u00a9<copyright years> iText Group NV (AGPL-version)\"";
            String licensed = sourceFolder + "producerLicensed.pdf";
            String agpl = sourceFolder + "producerAGPL.pdf";
            NUnit.Framework.Assert.AreEqual(expectedMessage, new CompareTool().CompareDocumentInfo(agpl, licensed));
        }
    }
}
