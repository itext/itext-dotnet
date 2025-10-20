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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Class which stores properties related to LOTL (List of Trusted Lists) fetching and validation process.
    ///     </summary>
    public class LotlFetchingProperties {
        private readonly IOnFailingCountryLotlData onCountryFetchFailureStrategy;

        private ICollection<String> ignoredSchemaNames = new HashSet<String>();

        private HashSet<String> serviceTypes = new HashSet<String>();

        private HashSet<String> schemaNames = new HashSet<String>();

        // Default timeout for invalidating cache is 24 hours in milliseconds.
        private long staleNessInMillis = 24L * 60L * 60L * 1000L;

        private Func<long, long> refreshIntervalCalculator = (stalenessTime) => {
            // This function can be used to set a custom cache refresh timer based on the staleness time.
            // For now, we take 23% of the staleness time as the refresh interval.
            if (stalenessTime <= 0) {
                throw new PdfException(SignExceptionMessageConstant.STALENESS_MUST_BE_POSITIVE);
            }
            double PERCENTAGE = 0.23D;
            return (long)(stalenessTime * PERCENTAGE);
        }
        ;

        /// <summary>
        /// Creates an instance of
        /// <see cref="LotlFetchingProperties"/>.
        /// </summary>
        /// <remarks>
        /// Creates an instance of
        /// <see cref="LotlFetchingProperties"/>.
        /// See
        /// <see cref="IOnFailingCountryLotlData"/>
        /// for more details.
        /// </remarks>
        /// <param name="countryFetchFailureStrategy">strategy to be used when fetching of a country specific LOTL fails
        ///     </param>
        public LotlFetchingProperties(IOnFailingCountryLotlData countryFetchFailureStrategy) {
            this.onCountryFetchFailureStrategy = countryFetchFailureStrategy;
        }

        /// <summary>Adds schema name (usually two letters) of a country which shall be used during LOTL fetching.</summary>
        /// <remarks>
        /// Adds schema name (usually two letters) of a country which shall be used during LOTL fetching.
        /// <para />
        /// This method cannot be used together with
        /// <see cref="SetCountryNamesToIgnore(System.String[])"/>.
        /// <para />
        /// If no schema names are added or ignored, all country specific LOTL files will be used.
        /// <para />
        /// Most of the country names are present in
        /// <see cref="LotlCountryCodeConstants"/>
        /// class.
        /// </remarks>
        /// <param name="countryNames">schema names of countries to use</param>
        /// <returns>
        /// this same
        /// <see cref="LotlFetchingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlFetchingProperties SetCountryNames(params String[] countryNames
            ) {
            if (!ignoredSchemaNames.IsEmpty()) {
                throw new ArgumentException(SignExceptionMessageConstant.EITHER_USE_SCHEMA_NAME_OR_IGNORE_SCHEMA_NAME);
            }
            this.schemaNames = new HashSet<String>(JavaUtil.ArraysAsList(countryNames));
            return this;
        }

        /// <summary>Adds schema name (usually two letters) of a country which shall be ignored during LOTL fetching.</summary>
        /// <remarks>
        /// Adds schema name (usually two letters) of a country which shall be ignored during LOTL fetching.
        /// <para />
        /// This method cannot be used together with
        /// <see cref="SetCountryNames(System.String[])"/>.
        /// <para />
        /// If no schema names are added or ignored, all country specific LOTL files will be used.
        /// <para />
        /// Most of the country names are present in
        /// <see cref="LotlCountryCodeConstants"/>
        /// class.
        /// </remarks>
        /// <param name="countryNamesToIgnore">countries to ignore</param>
        /// <returns>
        /// this same
        /// <see cref="LotlFetchingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlFetchingProperties SetCountryNamesToIgnore(params String
            [] countryNamesToIgnore) {
            if (!schemaNames.IsEmpty()) {
                throw new ArgumentException(SignExceptionMessageConstant.EITHER_USE_SCHEMA_NAME_OR_IGNORE_SCHEMA_NAME);
            }
            this.ignoredSchemaNames = new HashSet<String>(JavaUtil.ArraysAsList(countryNamesToIgnore));
            return this;
        }

        /// <summary>Get the cache staleness threshold value in milliseconds.</summary>
        /// <returns>a set cache staleness in milliseconds.</returns>
        public virtual long GetCacheStalenessInMilliseconds() {
            return staleNessInMillis;
        }

        /// <summary>Sets the allowed staleness of cached EU trusted list entries in milliseconds.</summary>
        /// <remarks>
        /// Sets the allowed staleness of cached EU trusted list entries in milliseconds.
        /// <para />
        /// This value determines how long the cached EU trusted lists certificates will be considered
        /// valid to be used in the signatures validation if they are not updated. The cached entries are attempted
        /// to be updated regularly according to
        /// <see cref="SetRefreshIntervalCalculator(System.Func<long, long>)"/>
        /// configuration.
        /// If the update fails for some reason and the configured staleness threshold for the cached entry is
        /// eventually reached then the
        /// <see cref="IOnFailingCountryLotlData"/>
        /// strategy instance provided in the
        /// <see cref="LotlFetchingProperties(IOnFailingCountryLotlData)"/>
        /// will be invoked.
        /// <para />
        /// The default value is 24 hours (24 * 60 * 60 * 1000 milliseconds).
        /// <para />
        /// You can set this property to positive infinity in order to never consider the certificates stale and to keep
        /// using them in validation even if they are not updated. Consider updating the
        /// <see cref="SetRefreshIntervalCalculator(System.Func<long, long>)"/>
        /// to return static value in this case though.
        /// <para />
        /// See
        /// <see cref="IOnFailingCountryLotlData"/>
        /// for more details.
        /// </remarks>
        /// <param name="stalenessInMillis">the staleness time in milliseconds</param>
        /// <returns>
        /// this same
        /// <see cref="LotlFetchingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlFetchingProperties SetCacheStalenessInMilliseconds(long
             stalenessInMillis) {
            if (stalenessInMillis <= 0) {
                throw new PdfException(SignExceptionMessageConstant.STALENESS_MUST_BE_POSITIVE);
            }
            this.staleNessInMillis = stalenessInMillis;
            return this;
        }

        /// <summary>Gets the calculation function for the cache refresh interval.</summary>
        /// <remarks>
        /// Gets the calculation function for the cache refresh interval.
        /// <para />
        /// This function will be used to determine the refresh interval based on the staleness time.
        /// By default, it takes 23% of the staleness time as the refresh interval.
        /// </remarks>
        /// <returns>
        /// a function that takes the staleness time in milliseconds and returns the refresh interval in
        /// milliseconds.
        /// </returns>
        public virtual Func<long, long> GetRefreshIntervalCalculator() {
            return refreshIntervalCalculator;
        }

        /// <summary>Sets a custom cache refresh timer function.</summary>
        /// <remarks>
        /// Sets a custom cache refresh timer function. This function will be used to determine the refresh interval
        /// based on the staleness time.
        /// <para />
        /// By default, it takes 23% of the staleness time as the refresh interval.
        /// So if the staleness time is 24 hours, the refresh interval will be set to  5.52 hours.
        /// </remarks>
        /// <param name="refreshIntervalCalculator">
        /// a function that takes the staleness time in milliseconds and returns the refresh
        /// interval in milliseconds.
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="LotlFetchingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlFetchingProperties SetRefreshIntervalCalculator(Func<long, long>
             refreshIntervalCalculator) {
            this.refreshIntervalCalculator = refreshIntervalCalculator;
            return this;
        }

        /// <summary>Gets the strategy to be used when fetching a country specific LOTL fails.</summary>
        /// <returns>the strategy to be used when fetching a country specific LOTL fails</returns>
        public virtual IOnFailingCountryLotlData GetOnCountryFetchFailureStrategy() {
            return onCountryFetchFailureStrategy;
        }

        /// <summary>Adds service type identifier which shall be used during country specific LOTL fetching.</summary>
        /// <remarks>
        /// Adds service type identifier which shall be used during country specific LOTL fetching.
        /// <para />
        /// If no service type identifiers are added,
        /// all service types from
        /// <see cref="ServiceTypeIdentifiersConstants"/>
        /// will be used.
        /// <para />
        /// Only values supported by this logic are predefined in
        /// <see cref="ServiceTypeIdentifiersConstants"/>.
        /// </remarks>
        /// <param name="serviceType">
        /// service type identifier as a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="LotlFetchingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlFetchingProperties SetServiceTypes(params String[] serviceType
            ) {
            this.serviceTypes = new HashSet<String>(JavaUtil.ArraysAsList(serviceType));
            return this;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual ICollection<String> GetServiceTypes() {
            return JavaCollectionsUtil.UnmodifiableSet(serviceTypes);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks if the schema should be processed based on the current configuration.</summary>
        /// <param name="countryName">the country name to use</param>
        /// <returns>this instance for method chaining</returns>
        internal virtual bool ShouldProcessCountry(String countryName) {
            if (!schemaNames.IsEmpty() && !ignoredSchemaNames.IsEmpty()) {
                throw new InvalidOperationException(SignExceptionMessageConstant.EITHER_USE_SCHEMA_NAME_OR_IGNORE_SCHEMA_NAME
                    );
            }
            if (schemaNames.IsEmpty() && ignoredSchemaNames.IsEmpty()) {
                // If no specific schema names are set, process all
                return true;
            }
            if (!ignoredSchemaNames.IsEmpty()) {
                // Process if schema is not in ignored list
                return !ignoredSchemaNames.Contains(countryName);
            }
            // Process if schema is in the specified list
            return schemaNames.Contains(countryName);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IList<String> GetSchemaNames() {
            return JavaCollectionsUtil.UnmodifiableList(JavaUtil.ArraysAsList(schemaNames.ToArray(new String[0])));
        }
//\endcond
    }
}
