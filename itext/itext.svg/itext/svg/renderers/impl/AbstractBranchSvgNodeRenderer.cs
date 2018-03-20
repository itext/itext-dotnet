using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// Abstract class that will be the superclass for any element that can function
    /// as a parent.
    /// </summary>
    public abstract class AbstractBranchSvgNodeRenderer : AbstractSvgNodeRenderer {
        /// <summary>
        /// Method that will set properties to be inherited by this branch renderer's
        /// children and will iterate over all children in order to draw them.
        /// </summary>
        /// <param name="context">
        /// the object that knows the place to draw this element and
        /// maintains its state
        /// </param>
        protected internal override void DoDraw(SvgDrawContext context) {
            PdfStream stream = new PdfStream();
            stream.Put(PdfName.Type, PdfName.XObject);
            stream.Put(PdfName.Subtype, PdfName.Form);
            stream.Put(PdfName.BBox, new PdfArray(new Rectangle(1, 1, 1, 1)));
            // required
            PdfFormXObject xObject = (PdfFormXObject)PdfXObject.MakeXObject(stream);
            PdfCanvas newCanvas = new PdfCanvas(xObject, context.GetCurrentCanvas().GetDocument());
            context.PushCanvas(newCanvas);
            foreach (ISvgNodeRenderer child in GetChildren()) {
                child.Draw(context);
            }
            context.PopCanvas();
            context.GetCurrentCanvas().AddXObject(xObject, 1, 0, 0, 1, 0, 0);
        }
    }
}
