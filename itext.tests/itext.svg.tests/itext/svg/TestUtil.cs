using System;
using iText.Svg.Renderers;

namespace iText.Svg {
    public class TestUtil {
        [Obsolete]
        public static bool CompareDummyRendererTrees(ISvgNodeRenderer treeOne, ISvgNodeRenderer treeTwo) {
            return CompareDummyRendererTreesRecursive(treeOne, treeTwo);
        }

        private static bool CompareDummyRendererTreesRecursive(ISvgNodeRenderer treeNodeOne, ISvgNodeRenderer treeNodeTwo
            ) {
            //Name
            if (!treeNodeOne.ToString().Equals(treeNodeTwo.ToString())) {
                return false;
            }
            //Nr of children
            if (treeNodeOne is IBranchSvgNodeRenderer && treeNodeTwo is IBranchSvgNodeRenderer) {
                IBranchSvgNodeRenderer one = (IBranchSvgNodeRenderer)treeNodeOne;
                IBranchSvgNodeRenderer two = (IBranchSvgNodeRenderer)treeNodeTwo;
                if (one.GetChildren().Count != two.GetChildren().Count) {
                    return false;
                }
                //Expect empty collection when no children are present
                if (one.GetChildren().IsEmpty()) {
                    return true;
                }
                //Iterate over children
                bool iterationResult = true;
                for (int i = 0; i < one.GetChildren().Count; i++) {
                    iterationResult = iterationResult && CompareDummyRendererTreesRecursive(one.GetChildren()[i], two.GetChildren
                        ()[i]);
                }
                return iterationResult;
            }
            return false;
        }
    }
}
