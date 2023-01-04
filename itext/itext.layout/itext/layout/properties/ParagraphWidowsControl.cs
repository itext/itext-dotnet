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
    /// widows restrictions.
    /// </summary>
    /// <remarks>
    /// A specialized class holding configurable parameters related to
    /// <see cref="iText.Layout.Element.Paragraph"/>
    /// 's
    /// widows restrictions. This class is meant to be used as the value for the
    /// <see cref="Property.WIDOWS_CONTROL"/>
    /// key.
    /// </remarks>
    public class ParagraphWidowsControl {
        private int minWidows;

        private int maxLinesToMove;

        private bool overflowOnWidowsViolation;

        /// <summary>
        /// Creates a
        /// <see cref="ParagraphWidowsControl"/>
        /// instance with specified widows restrictions.
        /// </summary>
        /// <param name="minWidows">minimal number of paragraph's lines to be overflowed to the next area.</param>
        /// <param name="maxLinesToMove">
        /// a number of lines that are allowed to be moved to the next area
        /// in order to fix widows constraint violation.
        /// </param>
        /// <param name="overflowParagraphOnViolation">
        /// defines whether the entire paragraph should be pushed to the next area
        /// if widows constraint is violated and cannot be automatically fixed.
        /// </param>
        public ParagraphWidowsControl(int minWidows, int maxLinesToMove, bool overflowParagraphOnViolation) {
            this.minWidows = minWidows;
            this.maxLinesToMove = maxLinesToMove;
            overflowOnWidowsViolation = overflowParagraphOnViolation;
        }

        /// <summary>Sets parameters that define widows restrictions and conditions of handling cases of widows constraint violation.
        ///     </summary>
        /// <param name="minWidows">minimal number of paragraph's lines to be overflowed to the next area.</param>
        /// <param name="maxLinesToMove">
        /// a number of lines that are allowed to be moved to the next area
        /// in order to fix widows constraint violation.
        /// </param>
        /// <param name="overflowParagraphOnViolation">
        /// defines whether paragraph should be completely pushed to the next area
        /// if widows constraint is violated and cannot be automatically fixed.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="ParagraphWidowsControl"/>
        /// instance.
        /// </returns>
        public virtual iText.Layout.Properties.ParagraphWidowsControl SetMinAllowedWidows(int minWidows, int maxLinesToMove
            , bool overflowParagraphOnViolation) {
            this.minWidows = minWidows;
            this.maxLinesToMove = maxLinesToMove;
            overflowOnWidowsViolation = overflowParagraphOnViolation;
            return this;
        }

        /// <summary>Gets minimal number of paragraph's lines to be overflowed to the next area.</summary>
        /// <returns>minimal number of paragraph's lines to be overflowed to the next area</returns>
        public virtual int GetMinWidows() {
            return minWidows;
        }

        /// <summary>
        /// Gets a number of lines that are allowed to be moved to the next area in order to fix
        /// widows constraint violation.
        /// </summary>
        /// <returns>a number of lines that are allowed to be moved to the next are</returns>
        public virtual int GetMaxLinesToMove() {
            return maxLinesToMove;
        }

        /// <summary>
        /// Indicates whether paragraph should be completely pushed to the next area if widows constraint is violated and
        /// cannot be automatically fixed.
        /// </summary>
        /// <returns>
        /// true if paragraph should be completely pushed to the next area if widows constraint is violated and
        /// cannot be automatically fixed, otherwise - false
        /// </returns>
        public virtual bool IsOverflowOnWidowsViolation() {
            return overflowOnWidowsViolation;
        }

        /// <summary>Writes a log message reporting that widows constraint is violated and cannot be automatically fixed.
        ///     </summary>
        /// <remarks>
        /// Writes a log message reporting that widows constraint is violated and cannot be automatically fixed.
        /// This method is to be overridden if violation scenarios need to be handled in some other way.
        /// </remarks>
        /// <param name="widowsRenderer">a renderer processing widows</param>
        /// <param name="message">
        /// 
        /// <see cref="System.String"/>
        /// explaining the reason for violation
        /// </param>
        public virtual void HandleViolatedWidows(ParagraphRenderer widowsRenderer, String message) {
            ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Properties.ParagraphWidowsControl));
            if (widowsRenderer.GetOccupiedArea() != null && widowsRenderer.GetLines() != null) {
                int pageNumber = widowsRenderer.GetOccupiedArea().GetPageNumber();
                String warnText = MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.WIDOWS_CONSTRAINT_VIOLATED, 
                    pageNumber, minWidows, widowsRenderer.GetLines().Count, message);
                logger.LogWarning(warnText);
            }
            else {
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.PREMATURE_CALL_OF_HANDLE_VIOLATION_METHOD);
            }
        }
    }
}
