using System;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Googlecharts {
    public class TrendlinesChartsTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/trendlines_charts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/trendlines_charts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Trendlines_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "trendlines_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Trendlines2_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "trendlines2_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Trendlines3_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "trendlines3_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Trendlines4_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "trendlines4_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Trendlines5_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "trendlines5_chart");
        }
    }
}
