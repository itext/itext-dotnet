using System;
using iText.Kernel.Geom;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>An interface containing a method to simplify working with SVG text elements.</summary>
    /// <remarks>
    /// An interface containing a method to simplify working with SVG text elements.
    /// Must be removed in update 7.3 as the methods of this interface will be moved to
    /// <see cref="ISvgTextNodeRenderer"/>
    /// </remarks>
    [Obsolete]
    public interface ISvgTextNodeHelper {
        /// <summary>Return the bounding rectangle of the text element.</summary>
        /// <param name="context">
        /// current
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>
        /// </param>
        /// <param name="basePoint">end point of previous text element</param>
        /// <returns>
        /// created instance of
        /// <see cref="iText.Svg.Utils.TextRectangle"/>
        /// </returns>
        TextRectangle GetTextRectangle(SvgDrawContext context, Point basePoint);
        // TODO DEVSIX-3814 This method should be moved to ISvgTextNodeRenderer in 7.2 and this class should be removed
    }
}
