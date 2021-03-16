/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using Common.Logging;
using iText.IO.Util;
using iText.Kernel;

namespace iText.Kernel.Actions.Sequence {
    /// <summary>
    /// The class allows to make any element identifiable so that it is possible to store some metadata
    /// for it.
    /// </summary>
    public abstract class AbstractIdentifiableElement {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(AbstractIdentifiableElement));

        private SequenceId sequenceId;

        /// <summary>Obtains an identifier if it was set.</summary>
        /// <returns>identifier</returns>
        public virtual SequenceId GetSequenceId() {
            return sequenceId;
        }

        /// <summary>Sets an identifier.</summary>
        /// <remarks>
        /// Sets an identifier. Note that it is forbidden to override already existing identifier. New
        /// one will be ignored if element already has an identifier.
        /// </remarks>
        /// <param name="sequenceId">is a new identifier for the element</param>
        public virtual void SetSequenceId(SequenceId sequenceId) {
            if (this.sequenceId == null) {
                this.sequenceId = sequenceId;
            }
            else {
                if (LOGGER.IsWarnEnabled) {
                    LOGGER.Warn(MessageFormatUtil.Format(KernelLogMessageConstant.ELEMENT_ALREADY_HAS_AN_IDENTIFIER, this.sequenceId
                        , sequenceId));
                }
            }
        }
    }
}
