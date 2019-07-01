using System;
using iText.Kernel.Geom;
using iText.Kernel.Utils;
using iText.Svg.Renderers;
using iText.Svg.Utils;
using iText.Test;

namespace iText.Svg.Googlecharts {
    public class GanttChartsTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/gantt_charts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/gantt_charts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Gantt_chart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "gantt_chart.pdf", sourceFolder + "gantt_chart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "gantt_chart.pdf", sourceFolder
                 + "cmp_gantt_chart.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Gantt2_chart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "gantt2_chart.pdf", sourceFolder + "gantt2_chart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "gantt2_chart.pdf", sourceFolder
                 + "cmp_gantt2_chart.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Gantt3_chart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "gantt3_chart.pdf", sourceFolder + "gantt3_chart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "gantt3_chart.pdf", sourceFolder
                 + "cmp_gantt3_chart.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Gantt4_chart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "gantt4_chart.pdf", sourceFolder + "gantt4_chart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "gantt4_chart.pdf", sourceFolder
                 + "cmp_gantt4_chart.pdf", destinationFolder, "diff_"));
        }
    }
}
