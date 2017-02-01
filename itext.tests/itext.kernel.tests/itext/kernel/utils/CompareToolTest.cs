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
    }
}
