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
    /// <summary>Interface for handling the failure of fetching a country-specific List of Trusted Lists (Lotl).</summary>
    /// <remarks>
    /// Interface for handling the failure of fetching a country-specific List of Trusted Lists (Lotl).
    /// Implementations can define custom strategies for dealing with such failures.
    /// <para />
    /// We provide 2 default implementations out of the box:
    /// -
    /// <see cref="RemoveOnFailingCountryData"/>
    /// - which does nothing and won't
    /// throw an exception, but will not add any country-specific certificates to the trust store.
    /// -
    /// <see cref="ThrowExceptionOnFailingCountryData"/>
    /// - which will throw an exception
    /// if the fetching of a country-specific Lotl fails so that the validation process can be halted.
    /// </remarks>
    public interface IOnCountryFetchFailureStrategy {
        /// <summary>This method is called when the fetching of a country-specific Lotl fails.</summary>
        /// <remarks>
        /// This method is called when the fetching of a country-specific Lotl fails.
        /// It allows for custom handling of the failure.
        /// <para />
        /// If the implementation does not throw an exception, the validation process will continue, and the certificates
        /// from the
        /// <c>CountrySpecificLotlFetcher.Result</c>
        /// will not be added to the trust store.
        /// </remarks>
        /// <param name="fetchResult">the result of the fetch attempt, which may contain error details</param>
        void OnCountryFetchFailure(CountrySpecificLotlFetcher.Result fetchResult);
    }
}
