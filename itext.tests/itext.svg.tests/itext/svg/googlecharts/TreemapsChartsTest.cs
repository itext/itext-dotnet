using System;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Googlecharts {
    public class TreemapsChartsTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/treemaps_charts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/treemaps_charts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG, Count = 2)]
        public virtual void Treemaps_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "treemaps_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG, Count = 2)]
        public virtual void Treemaps2_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "treemaps2_chart");
        }
    }
}
