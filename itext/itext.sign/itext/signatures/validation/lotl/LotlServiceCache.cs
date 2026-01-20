/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Generic;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Interface for caching LOTL (List of Trusted Lists) service results.</summary>
    /// <remarks>
    /// Interface for caching LOTL (List of Trusted Lists) service results.
    /// It provides methods to set and get various LOTL-related data, including
    /// European LOTL, country-specific LOTLs, and pivot results.
    /// <para />
    /// Notice: If you do your own implementation of this interface,
    /// you should ensure that the cache is thread-safe and can handle concurrent access.
    /// This is important because LOTL data can be accessed and modified by multiple threads
    /// simultaneously.
    /// You should also ensure that all the values are set atomically using
    /// <see cref="SetAllValues(Result, Result, Result, System.Collections.Generic.IDictionary{K, V})"/>
    /// method
    /// to maintain consistency, So that you are not using outdated pivot results or country-specific LOTLs with a changed
    /// European LOTL.
    /// </remarks>
    public interface LotlServiceCache {
        /// <summary>
        /// Sets all values related to LOTL, including European LOTL, EU Journal certificates,
        /// pivot results, and country-specific LOTLs.
        /// </summary>
        /// <remarks>
        /// Sets all values related to LOTL, including European LOTL, EU Journal certificates,
        /// pivot results, and country-specific LOTLs. This extra method is used for syncronized
        /// updates to the cache, ensuring that all related data is set at once. This is useful
        /// in multithreaded environments where you want to ensure that all related data is consistent.
        /// </remarks>
        /// <param name="lotlXml">the European LOTL result</param>
        /// <param name="europeanResourceFetcherEUJournalCertificates">the EU Journal certificates</param>
        /// <param name="result">the pivot fetcher result</param>
        /// <param name="countrySpecificResult">a map of country-specific LOTL results</param>
        void SetAllValues(EuropeanLotlFetcher.Result lotlXml, EuropeanResourceFetcher.Result europeanResourceFetcherEUJournalCertificates
            , PivotFetcher.Result result, IDictionary<String, CountrySpecificLotlFetcher.Result> countrySpecificResult
            );

        /// <summary>Gets the European LOTL result.</summary>
        /// <returns>the European LOTL result</returns>
        PivotFetcher.Result GetPivotResult();

        /// <summary>Sets the pivot result.</summary>
        /// <param name="newResult">the new pivot result to set</param>
        void SetPivotResult(PivotFetcher.Result newResult);

        /// <summary>Gets the country-specific LOTL results.</summary>
        /// <returns>a map of country-specific LOTL results</returns>
        IDictionary<String, CountrySpecificLotlFetcher.Result> GetCountrySpecificLotls();

        /// <summary>Sets the country-specific LOTL result for a specific country.</summary>
        /// <param name="countrySpecificLotlResult">the country-specific LOTL result to set</param>
        void SetCountrySpecificLotlResult(CountrySpecificLotlFetcher.Result countrySpecificLotlResult);

        /// <summary>Gets the European LOTL result.</summary>
        /// <returns>the European LOTL result</returns>
        EuropeanLotlFetcher.Result GetLotlResult();

        /// <summary>Sets the European LOTL result.</summary>
        /// <param name="data">the European LOTL result to set</param>
        void SetLotlResult(EuropeanLotlFetcher.Result data);

        /// <summary>Sets the result of the European Resource Fetcher.</summary>
        /// <remarks>
        /// Sets the result of the European Resource Fetcher.
        /// This method is used to update the cache with the result
        /// </remarks>
        /// <param name="result">the result of the European Resource Fetcher</param>
        void SetEuropeanResourceFetcherResult(EuropeanResourceFetcher.Result result);

        /// <summary>Gets the result of the European Resource Fetcher.</summary>
        /// <returns>the result of the European Resource Fetcher</returns>
        EuropeanResourceFetcher.Result GetEUJournalCertificates();
    }
}
