using System;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Googlecharts {
    public class SankeyChartsTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/googlecharts/sankey_charts/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/googlecharts/sankey_charts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Sankey_borders_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "sankey_borders_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Sankey_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "sankey_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPEDTAG, Count = 46)]
        public virtual void Sankey_colored_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "sankey_colored_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Sankey_fonts_chart() {
            // TODO DEVSIX-2905
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "sankey_fonts_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Sankey_multilevel_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "sankey_multilevel_chart");
        }

        /// <exception cref="iText.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void Sankey_nodes_chart() {
            ConvertAndCompareVisually(sourceFolder, destinationFolder, "sankey_nodes_chart");
        }
    }
}
