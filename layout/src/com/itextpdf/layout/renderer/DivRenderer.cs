using com.itextpdf.layout.element;

namespace com.itextpdf.layout.renderer
{
	public class DivRenderer : BlockRenderer
	{
		public DivRenderer(Div modelElement)
			: base(modelElement)
		{
		}

		public override IRenderer GetNextRenderer()
		{
			return new com.itextpdf.layout.renderer.DivRenderer((Div)modelElement);
		}
	}
}
