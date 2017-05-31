using System.Collections.Generic;

namespace iText.Kernel.Pdf.Canvas.Parser.Listener {
    /// <summary>
    /// This is a special interface for
    /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Filter.IEventFilter"/>
    /// that returns a collection of rectangles as result of its work.
    /// </summary>
    public interface ILocationExtractionStrategy : IEventListener {
        /// <summary>Returns the rectangles that have been processed so far.</summary>
        /// <returns>
        /// 
        /// <see>Collection<IPdfTextLocation></see>
        /// instance with the current resultant IPdfTextLocations
        /// </returns>
        ICollection<IPdfTextLocation> GetResultantLocations();
    }
}
