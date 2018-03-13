using System.Collections.Generic;

namespace iText.Svg.Renderers {
    /// <summary>
    /// Interface for SvgNodeRenderer, the renderer draws the SVG to its Pdf-canvas passed in
    /// <see cref="iText.Layout.Renderer.DrawContext"/>
    /// ,
    /// applying styling (CSS and attributes).
    /// </summary>
    public interface ISvgNodeRenderer {
        ISvgNodeRenderer GetParent();

        void Draw(SvgDrawContext context);

        void AddChild(ISvgNodeRenderer child);

        IList<ISvgNodeRenderer> GetChildren();
    }
}
