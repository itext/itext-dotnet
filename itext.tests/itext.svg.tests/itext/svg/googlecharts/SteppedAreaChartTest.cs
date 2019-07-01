using System;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Googlecharts {
    public class SteppedAreaChartTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/stepped_area_charts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/stepped_area_charts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Stepped_area_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "stepped_area_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Stepped_area2_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "stepped_area2_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Stepped_area3_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "stepped_area3_chart");
        }
    }
}
