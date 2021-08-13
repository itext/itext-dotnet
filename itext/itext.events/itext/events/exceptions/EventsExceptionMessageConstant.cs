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
using System;

namespace iText.Events.Exceptions {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class EventsExceptionMessageConstant {
        /// <summary>Message warns about overriding of the identifier of identifiable element.</summary>
        /// <remarks>
        /// Message warns about overriding of the identifier of identifiable element. List of params:
        /// <list type="bullet">
        /// <item><description>0th is an original element identifier;
        /// </description></item>
        /// <item><description>1st is a new element identifier;
        /// </description></item>
        /// </list>
        /// </remarks>
        public const String ELEMENT_ALREADY_HAS_IDENTIFIER = "Element already has sequence id: {0}, new id {1} " +
             "will be ignored";

        public const String UNKNOWN_ITEXT_EXCEPTION = "Unknown ITextException.";

        private EventsExceptionMessageConstant() {
        }
    }
}
