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
namespace iText.Signatures.Validation.Lotl {
    /// <summary>
    /// This class implements the
    /// <see cref="IOnCountryFetchFailureStrategy"/>
    /// interface and provides a strategy
    /// for handling failures when fetching country-specific Lotl (List of Trusted Lists) files.
    /// </summary>
    /// <remarks>
    /// This class implements the
    /// <see cref="IOnCountryFetchFailureStrategy"/>
    /// interface and provides a strategy
    /// for handling failures when fetching country-specific Lotl (List of Trusted Lists) files.
    /// It ignores the failure of the specific country, and converts all report items to INFO status.
    /// This way the country-specific Lotl is not used, the validation report is not invalid, but the items are still
    /// preserved.
    /// </remarks>
    public class IgnoreCountrySpecificCertificates : IOnCountryFetchFailureStrategy {
        /// <summary>
        /// Constructs an instance of
        /// <see cref="IgnoreCountrySpecificCertificates"/>.
        /// </summary>
        public IgnoreCountrySpecificCertificates() {
        }

        //Empty constructor
        /// <summary><inheritDoc/></summary>
        public virtual void OnCountryFetchFailure(CountrySpecificLotlFetcher.Result fetchResult) {
        }
        // we do nothing here, as we ignore the failure of the specific country
    }
}
