using System;
using iText.Kernel.Geom;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>
    /// Instances of this interface represent a piece of text,
    /// somewhere on a page in a pdf document.
    /// </summary>
    public interface IPdfTextLocation {
        /// <summary>Get the visual rectangle in which the text is located</summary>
        /// <returns/>
        Rectangle GetRectangle();

        /// <summary>Get the text</summary>
        /// <returns/>
        String GetText();

        /// <summary>Get the page number of the page on which the text is located</summary>
        /// <returns>the page number, or 0 if no page number was set</returns>
        int GetPageNumber();
    }
}
