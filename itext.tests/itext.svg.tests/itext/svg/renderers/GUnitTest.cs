using System;
using System.Collections.Generic;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg.Renderers {
    public class GUnitTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/gunit/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/gunit/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void MeetTheTeam() {
            IList<Exception> assertionErrorsThrown = new List<Exception>();
            for (int i = 1; i < 6; i++) {
                try {
                    SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "test_00" + i);
                }
                catch (Exception ae) {
                    if (ae.Message.Contains("expected null, but was")) {
                        assertionErrorsThrown.Add(ae);
                    }
                }
            }
            if (assertionErrorsThrown.Count != 0) {
                NUnit.Framework.Assert.Fail("At least one compare file was not identical with the result");
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void ViewboxTest() {
            SvgNodeRendererTestUtility.ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "test_viewbox");
        }
    }
}
