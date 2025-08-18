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
using iText.Commons.Utils;
using iText.Signatures.Exceptions;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>
    /// This class implements the
    /// <see cref="IOnFailingCountryLotlData"/>
    /// interface and provides a strategy
    /// for handling failures when fetching country-specific trusted list .
    /// </summary>
    /// <remarks>
    /// This class implements the
    /// <see cref="IOnFailingCountryLotlData"/>
    /// interface and provides a strategy
    /// for handling failures when fetching country-specific trusted list .
    /// <para />
    /// It throws an
    /// <see cref="InvalidLotlDataException"/>
    /// if the specific country fetch or the trusted lists validation fails.
    /// </remarks>
    public class ThrowExceptionOnFailingCountryData : IOnFailingCountryLotlData {
        /// <summary>
        /// Creates an instance of
        /// <see cref="ThrowExceptionOnFailingCountryData"/>.
        /// </summary>
        public ThrowExceptionOnFailingCountryData() {
        }

        // Default constructor
        /// <summary><inheritDoc/></summary>
        public virtual void OnCountryFailure(CountrySpecificLotlFetcher.Result fetchResult) {
            CountrySpecificLotl country = fetchResult.GetCountrySpecificLotl();
            throw new InvalidLotlDataException(MessageFormatUtil.Format(SignExceptionMessageConstant.FAILED_TO_FETCH_LOTL_FOR_COUNTRY
                , country.GetSchemeTerritory(), country.GetTslLocation(), fetchResult.GetLocalReport()), fetchResult.GetLocalReport
                ());
        }
    }
}
