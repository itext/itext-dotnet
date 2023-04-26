/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System;
using iText.Commons.Exceptions;
using iText.Commons.Utils;

namespace iText.Commons.Actions.Sequence {
    /// <summary>
    /// Util class which is responsible for marking of
    /// <see cref="AbstractIdentifiableElement"/>
    /// with
    /// appropriate
    /// <see cref="SequenceId"/>.
    /// </summary>
    public sealed class SequenceIdManager {
        private SequenceIdManager() {
        }

        /// <summary>
        /// Provides an
        /// <see cref="AbstractIdentifiableElement"/>
        /// with a
        /// <see cref="SequenceId"/>.
        /// </summary>
        /// <remarks>
        /// Provides an
        /// <see cref="AbstractIdentifiableElement"/>
        /// with a
        /// <see cref="SequenceId"/>
        /// . Note that it is
        /// forbidden to override already existing identifier. If try to provide a new one then exception
        /// will be thrown.
        /// </remarks>
        /// <param name="element">is an identifiable element</param>
        /// <param name="sequenceId">is an identifier to set</param>
        public static void SetSequenceId(AbstractIdentifiableElement element, SequenceId sequenceId) {
            lock (element) {
                if (element.GetSequenceId() == null) {
                    element.SetSequenceId(sequenceId);
                }
                else {
                    throw new InvalidOperationException(MessageFormatUtil.Format(CommonsExceptionMessageConstant.ELEMENT_ALREADY_HAS_IDENTIFIER
                        , element.GetSequenceId().GetId(), sequenceId.GetId()));
                }
            }
        }

        /// <summary>Gets an identifier of the element.</summary>
        /// <remarks>Gets an identifier of the element. If it was not provided will return <c>null</c>.</remarks>
        /// <param name="element">is an identifiable element</param>
        /// <returns>the identifier of the element if presented and <c>null</c> otherwise</returns>
        public static SequenceId GetSequenceId(AbstractIdentifiableElement element) {
            return element.GetSequenceId();
        }
    }
}
