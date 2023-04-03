using iText.Forms.Form.Renderer;
using iText.Kernel.Geom;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Renderer.Checkboximpl {
    /// <summary>This interface is used to draw a checkBox icon.</summary>
    public interface ICheckBoxRenderingStrategy {
        /// <summary>Draws a check box icon.</summary>
        /// <param name="drawContext">the draw context</param>
        /// <param name="checkBoxRenderer">the checkBox renderer</param>
        /// <param name="rectangle">the rectangle where the icon should be drawn</param>
        void DrawCheckBoxContent(DrawContext drawContext, CheckBoxRenderer checkBoxRenderer, Rectangle rectangle);
    }
}
