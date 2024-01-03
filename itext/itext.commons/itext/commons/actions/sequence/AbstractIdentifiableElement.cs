/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

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
namespace iText.Commons.Actions.Sequence {
    /// <summary>
    /// The class allows to make any element identifiable so that it is possible to store some metadata
    /// for it.
    /// </summary>
    public abstract class AbstractIdentifiableElement {
        private SequenceId sequenceId;

        /// <summary>Obtains an identifier if it was set.</summary>
        /// <returns>identifier</returns>
        internal virtual SequenceId GetSequenceId() {
            return sequenceId;
        }

        /// <summary>Sets an identifier.</summary>
        /// <param name="sequenceId">is a new identifier for the element</param>
        internal virtual void SetSequenceId(SequenceId sequenceId) {
            this.sequenceId = sequenceId;
        }
    }
}
