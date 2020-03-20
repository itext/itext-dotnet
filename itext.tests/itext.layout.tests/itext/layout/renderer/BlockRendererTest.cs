using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    public class BlockRendererTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ApplyMinHeightForSpecificDimensionsCausingFloatPrecisionError() {
            float divHeight = 42.55f;
            Div div = new Div();
            div.SetHeight(UnitValue.CreatePointValue(divHeight));
            float occupiedHeight = 17.981995f;
            float leftHeight = 24.567993f;
            NUnit.Framework.Assert.IsTrue(occupiedHeight + leftHeight < divHeight);
            BlockRenderer blockRenderer = (BlockRenderer)div.CreateRendererSubTree();
            blockRenderer.occupiedArea = new LayoutArea(1, new Rectangle(0, 267.9681f, 0, occupiedHeight));
            AbstractRenderer renderer = blockRenderer.ApplyMinHeight(OverflowPropertyValue.FIT, new Rectangle(0, 243.40012f
                , 0, leftHeight));
            NUnit.Framework.Assert.IsNull(renderer);
        }
    }
}
