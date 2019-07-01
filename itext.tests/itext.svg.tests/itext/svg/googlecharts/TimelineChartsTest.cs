using System;
using iText.Kernel.Geom;
using iText.Kernel.Utils;
using iText.Svg.Renderers;
using iText.Svg.Utils;
using iText.Test;

namespace iText.Svg.Googlecharts {
    public class TimelineChartsTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/timeline_charts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/timeline_charts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Timeline_advanced_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "timeline_advanced_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Timeline_chart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "timeline_chart.pdf", sourceFolder + "timeline_chart.svg", 1
                , pageSize);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "timeline_chart.pdf", 
                sourceFolder + "cmp_timeline_chart.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Timeline_labeled_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "timeline_labeled_chart");
        }
    }
}
