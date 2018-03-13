using iText.Svg.Renderers;

namespace iText.Svg {
    public class TestUtil {
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
            if (treeNodeOne.GetChildren().Count != treeNodeTwo.GetChildren().Count) {
                return false;
            }
            //Expect empty collection when no children are present
            if (treeNodeOne.GetChildren().IsEmpty()) {
                return true;
            }
            //Iterate over children
            bool iterationResult = true;
            for (int i = 0; i < treeNodeOne.GetChildren().Count; i++) {
                iterationResult = iterationResult && CompareDummyRendererTreesRecursive(treeNodeOne.GetChildren()[i], treeNodeTwo
                    .GetChildren()[i]);
            }
            return iterationResult;
        }
    }
}
