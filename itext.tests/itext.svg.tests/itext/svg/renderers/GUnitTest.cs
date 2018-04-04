using System;
using System.IO;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg.Renderers {
    public class GUnitTest {
        private const String SOURCE_FOLDER = "C:\\Temp\\demo\\resources\\";

        private const String DESTINATION_FOLDER = "C:\\Temp\\demo\\resources\\out\\";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void MeetTheTeam() {
            for (int i = 1; i < 6; i++) {
                SvgNodeRendererTestUtility.Convert(new FileStream(SOURCE_FOLDER + "test_00" + i + ".svg", FileMode.Open, FileAccess.Read
                    ), new FileStream(DESTINATION_FOLDER + "test_00" + i + ".pdf", FileMode.Create));
            }
        }
    }
}
