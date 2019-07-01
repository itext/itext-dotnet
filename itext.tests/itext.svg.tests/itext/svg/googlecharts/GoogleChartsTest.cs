using System;
using iText.Kernel.Geom;
using iText.Kernel.Utils;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Googlecharts {
    public class GoogleChartsTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/GoogleChartsTests/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/GoogleChartsTests/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void BarChart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "bar_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        public virtual void Annotation_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "annotation_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Area_chart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "area_chart.pdf", sourceFolder + "area_chart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "area_chart.pdf", sourceFolder
                 + "cmp_area_chart.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Bubble_chart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "bubble_chart.pdf", sourceFolder + "bubble_chart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "bubble_chart.pdf", sourceFolder
                 + "cmp_bubble_chart.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG)]
        public virtual void Calendar_chart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "calendar_chart.pdf", sourceFolder + "calendar_chart.svg", 1
                , pageSize);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "calendar_chart.pdf", 
                sourceFolder + "cmp_calendar_chart.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Candlestick_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "candlestick_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Combo_chart() {
            PageSize pageSize = PageSize.A4;
            TestUtils.ConvertSVGtoPDF(destinationFolder + "combo_chart.pdf", sourceFolder + "combo_chart.svg", 1, pageSize
                );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "combo_chart.pdf", sourceFolder
                 + "cmp_combo_chart.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG, Count = 5)]
        public virtual void Diff_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "diff_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Donut_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "donut_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Waterfall_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "waterfall_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Histogram_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "histogram_chart");
        }
    }
}
