using System;

namespace iText.Svg.Renderers.Impl {
    public class PathOperatorSplitTest {
        [NUnit.Framework.Test]
        public virtual void TestNumbersContainingExponent01() {
            String path = "M10,9.999999999999972C203.33333333333334,9.999999999999972,396.6666666666667,1.4210854715202004e-14,590,1.4210854715202004e-14L590,41.666666666666686C396.6666666666667,41.666666666666686,203.33333333333334,51.66666666666664,10,51.66666666666664Z";
            String[] operators = new String[] { "M10,9.999999999999972", "C203.33333333333334,9.999999999999972,396.6666666666667,1.4210854715202004e-14,590,1.4210854715202004e-14"
                , "L590,41.666666666666686", "C396.6666666666667,41.666666666666686,203.33333333333334,51.66666666666664,10,51.66666666666664"
                , "Z" };
            TestSplitting(path, operators);
        }

        private void TestSplitting(String originalStr, String[] expectedSplitting) {
            String[] result = PathSvgNodeRenderer.SplitPathStringIntoOperators(originalStr);
            NUnit.Framework.Assert.AreEqual(expectedSplitting, result);
        }
    }
}
