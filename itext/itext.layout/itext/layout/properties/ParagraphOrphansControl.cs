using System;
using Common.Logging;
using iText.IO.Util;
using iText.Layout.Renderer;

namespace iText.Layout.Properties {
    /// <summary>
    /// A specialized class holding configurable parameters related to
    /// <see cref="iText.Layout.Element.Paragraph"/>
    /// 's
    /// orphans restrictions.
    /// </summary>
    /// <remarks>
    /// A specialized class holding configurable parameters related to
    /// <see cref="iText.Layout.Element.Paragraph"/>
    /// 's
    /// orphans restrictions. This class is meant to be used as the value for the
    /// <see cref="Property.ORPHANS_CONTROL"/>
    /// key.
    /// </remarks>
    public class ParagraphOrphansControl {
        private int minOrphans;

        /// <summary>
        /// Creates a
        /// <see cref="ParagraphOrphansControl"/>
        /// instance with a specified orphans limitation.
        /// </summary>
        /// <param name="minOrphans">minimal number of paragraph's lines to remain on an area before an area break.</param>
        public ParagraphOrphansControl(int minOrphans) {
            this.minOrphans = minOrphans;
        }

        /// <summary>Sets parameter that defines orphans restrictions.</summary>
        /// <param name="minOrphans">minimal number of paragraph's lines to remain on an area before an area break.</param>
        /// <returns>
        /// this
        /// <see cref="ParagraphOrphansControl"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.ParagraphOrphansControl SetMinAllowedOrphans(int minOrphans) {
            this.minOrphans = minOrphans;
            return this;
        }

        /// <summary>Gets minimal number of paragraph's lines to remain on an area before a split.</summary>
        /// <returns>minimal number of paragraph's lines to remain on an area before an area break.</returns>
        public virtual int GetMinOrphans() {
            return minOrphans;
        }

        /// <summary>Writes a log message reporting that orphans constraint is violated.</summary>
        /// <remarks>
        /// Writes a log message reporting that orphans constraint is violated.
        /// This method is to be overridden if violation scenarios need to be handled in some other way.
        /// </remarks>
        /// <param name="renderer">a renderer processing orphans</param>
        /// <param name="message">
        /// 
        /// <see cref="System.String"/>
        /// explaining the reason for violation
        /// </param>
        public virtual void HandleViolatedOrphans(ParagraphRenderer renderer, String message) {
            ILog logger = LogManager.GetLogger(typeof(iText.Layout.Properties.ParagraphOrphansControl));
            if (renderer.GetOccupiedArea() != null && renderer.GetLines() != null) {
                int pageNumber = renderer.GetOccupiedArea().GetPageNumber();
                String warnText = MessageFormatUtil.Format(iText.IO.LogMessageConstant.ORPHANS_CONSTRAINT_VIOLATED, pageNumber
                    , minOrphans, renderer.GetLines().Count, message);
                logger.Warn(warnText);
            }
            else {
                logger.Warn(iText.IO.LogMessageConstant.PREMATURE_CALL_OF_HANDLE_VIOLATION_METHOD);
            }
        }
    }
}
