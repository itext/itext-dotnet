using System;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Googlecharts {
    public class IntervalsChartsTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/intervals_charts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/intervals_charts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_area_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_area_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_backgroundBox_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_backgroundBox_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_box_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_box_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_boxPlot_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_boxPlot_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_boxThick_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_boxThick_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_combining_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_combining_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_line_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_line_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_points_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_points_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_pointsWhiskers_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_pointsWhiskers_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_stick_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_stick_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_sticksHorizontal_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_sticksHorizontal_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Intervals_tailored_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "intervals_tailored_chart");
        }
    }
}
