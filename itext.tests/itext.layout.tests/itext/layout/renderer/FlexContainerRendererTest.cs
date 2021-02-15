using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    public class FlexContainerRendererTest : ExtendedITextTest {
        private static float EPS = 0.0001F;

        [NUnit.Framework.Test]
        public virtual void WidthNotSetTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            DivRenderer divRenderer = new DivRenderer(new Div());
            flexRenderer.AddChild(divRenderer);
            NUnit.Framework.Assert.AreEqual(0F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(0F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildOneChildTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            DivRenderer divRenderer = new DivRenderer(new Div());
            divRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            flexRenderer.AddChild(divRenderer);
            NUnit.Framework.Assert.AreEqual(50F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(50F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(125F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(125F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenWithBordersMarginsPaddingsTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            divRenderer1.SetProperty(Property.BORDER, new SolidBorder(5));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            divRenderer2.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(10));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            divRenderer3.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(15));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(10));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(165F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(165F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToFlexRendererAndToChildManyChildrenWithBordersMarginsPaddingsTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            divRenderer1.SetProperty(Property.BORDER, new SolidBorder(5));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            divRenderer2.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(10));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            divRenderer3.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(15));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(10));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(50F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(50F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenFlexRendererWithRotationAngleTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.ROTATION_ANGLE, 10f);
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(104.892334F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(104.892334F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenFlexRendererWithMinWidthTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(71));
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(71F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(125F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenFlexRendererWithMinWidthBiggerThanMaxWidthTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.MIN_WIDTH, UnitValue.CreatePointValue(150));
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(150F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(150F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenFlexRendererWithMaxWidthTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(150));
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(125F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(150F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void WidthSetToChildManyChildrenFlexRendererWithMaxWidthLowerThanMinWidthTest() {
            FlexContainerRenderer flexRenderer = new FlexContainerRenderer(new Div());
            flexRenderer.SetProperty(Property.MAX_WIDTH, UnitValue.CreatePointValue(100));
            DivRenderer divRenderer1 = new DivRenderer(new Div());
            divRenderer1.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(50));
            DivRenderer divRenderer2 = new DivRenderer(new Div());
            divRenderer2.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(40));
            DivRenderer divRenderer3 = new DivRenderer(new Div());
            divRenderer3.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(30));
            DivRenderer divRenderer4 = new DivRenderer(new Div());
            divRenderer4.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(5));
            flexRenderer.AddChild(divRenderer1);
            flexRenderer.AddChild(divRenderer2);
            flexRenderer.AddChild(divRenderer3);
            flexRenderer.AddChild(divRenderer4);
            NUnit.Framework.Assert.AreEqual(100F, flexRenderer.GetMinMaxWidth().GetMinWidth(), EPS);
            NUnit.Framework.Assert.AreEqual(100F, flexRenderer.GetMinMaxWidth().GetMaxWidth(), EPS);
        }
    }
}
