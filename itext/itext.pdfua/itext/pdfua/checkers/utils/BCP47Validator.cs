/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>This class is a validator for IETF BCP 47 language tag (RFC 5646).</summary>
    [System.ObsoleteAttribute(@"in favor of iText.Kernel.Utils.Checkers.BCP47Validator")]
    public class BCP47Validator {
        private BCP47Validator() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>Validate language tag against RFC 5646.</summary>
        /// <param name="languageTag">language tag string</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if it is a valid tag,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool Validate(String languageTag) {
            return iText.Kernel.Utils.Checkers.BCP47Validator.Validate(languageTag);
        }
    }
}
