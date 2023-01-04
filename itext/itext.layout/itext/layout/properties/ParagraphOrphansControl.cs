/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
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
            ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Properties.ParagraphOrphansControl));
            if (renderer.GetOccupiedArea() != null && renderer.GetLines() != null) {
                int pageNumber = renderer.GetOccupiedArea().GetPageNumber();
                String warnText = MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.ORPHANS_CONSTRAINT_VIOLATED, 
                    pageNumber, minOrphans, renderer.GetLines().Count, message);
                logger.LogWarning(warnText);
            }
            else {
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.PREMATURE_CALL_OF_HANDLE_VIOLATION_METHOD);
            }
        }
    }
}
