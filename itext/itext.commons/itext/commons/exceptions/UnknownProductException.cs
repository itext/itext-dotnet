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
using System;

namespace iText.Commons.Exceptions {
    /// <summary>An exception notifies that unknown product was found in iText ecosystem.</summary>
    public class UnknownProductException : ITextException {
        /// <summary>Message notifies that event corresponding to unknown product was met.</summary>
        /// <remarks>
        /// Message notifies that event corresponding to unknown product was met. It is a parametrized
        /// message. List of params:
        /// <list type="bullet">
        /// <item><description>the name of unknown product
        /// </description></item>
        /// </list>
        /// </remarks>
        public const String UNKNOWN_PRODUCT = "Product {0} is unknown. Probably you have to register it.";

        /// <summary>Creates a new instance of the exception.</summary>
        /// <param name="message">the detail message</param>
        public UnknownProductException(String message)
            : base(message) {
        }
    }
}
