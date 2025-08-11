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
    /// <summary>Class which stores properties related to Lotl (List of Trusted Lists) fetching and validation process.
    ///     </summary>
    public class LotlFetchingProperties {
        private readonly IOnCountryFetchFailureStrategy onCountryFetchFailureStrategy;

        private ICollection<String> ignoredSchemaNames = new HashSet<String>();

        private HashSet<String> serviceTypes = new HashSet<String>();

        private HashSet<String> schemaNames = new HashSet<String>();

        //default time out for invalidating cache is  24 hours in milliseconds
        private long staleNessInMillis = 24L * 60L * 60L * 1000L;

        private Func<long, long> refreshIntervalCalculator = (stalenessTime) => {
            // This function can be used to set a custom cache refresh timer based on the staleness time.
            // For now, we take 70% of the staleness time as the refresh interval.
            if (stalenessTime <= 0) {
                throw new PdfException(SignExceptionMessageConstant.STALENESS_MUST_BE_POSITIVE);
            }
            double PERCENTAGE = 0.7D;
            return (long)(stalenessTime * PERCENTAGE);
        }
        ;

        /// <summary>
        /// Creates an instance of
        /// <see cref="LotlFetchingProperties"/>.
        /// </summary>
        /// <param name="countryFetchFailureStrategy">strategy to be used when fetching a country specific Lotl fails</param>
        public LotlFetchingProperties(IOnCountryFetchFailureStrategy countryFetchFailureStrategy) {
            this.onCountryFetchFailureStrategy = countryFetchFailureStrategy;
        }

        /// <summary>Adds schema name (usually two letters) of a country which shall be used during Lotl fetching.</summary>
        /// <remarks>
        /// Adds schema name (usually two letters) of a country which shall be used during Lotl fetching.
        /// <para />
        /// If no schema names are added, all country specific Lotl files will be used.
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

        /// <summary>Adds schema name (usually two letters) of a country which shall be ignored during Lotl fetching.</summary>
        /// <remarks>
        /// Adds schema name (usually two letters) of a country which shall be ignored during Lotl fetching.
        /// <para />
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

        /// <summary>Get the cache staleness in milliseconds.</summary>
        /// <returns>a set cache staleness in milliseconds.</returns>
        public virtual long GetCacheStalenessInMilliseconds() {
            return staleNessInMillis;
        }

        /// <summary>Sets the cache staleness in milliseconds.</summary>
        /// <remarks>
        /// Sets the cache staleness in milliseconds.
        /// <para />
        /// This value determines how long the cache will be considered valid before it is refreshed.
        /// If the cache is older than this value, it will be refreshed.
        /// <para />
        /// The default value is 24 hours (24 * 60 * 60 * 1000 milliseconds).
        /// </remarks>
        /// <param name="staleNessInMillis">the staleness time in milliseconds</param>
        /// <returns>
        /// this same
        /// <see cref="LotlFetchingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlFetchingProperties SetCacheStalenessInMilliseconds(long
             staleNessInMillis) {
            if (staleNessInMillis <= 0) {
                throw new PdfException(SignExceptionMessageConstant.STALENESS_MUST_BE_POSITIVE);
            }
            this.staleNessInMillis = staleNessInMillis;
            return this;
        }

        /// <summary>Gets the calculation function for the cache refresh interval.</summary>
        /// <remarks>
        /// Gets the calculation function for the cache refresh interval.
        /// <para />
        /// This function will be used to determine the refresh interval based on the staleness time.
        /// By default, it takes 70% of the staleness time as the refresh interval.
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
        /// By default, it takes 70% of the staleness time as the refresh interval.
        /// So if the staleness time is 24 hours, the refresh interval will be set to 16.8 hours. Which means the cache will
        /// be refreshed every 16.8 hours.
        /// </remarks>
        /// <param name="refreshIntervalCalculator">
        /// a function that takes the staleness time in milliseconds and returns the refresh
        /// interval in milliseconds.
        /// </param>
        public virtual void SetRefreshIntervalCalculator(Func<long, long> refreshIntervalCalculator) {
            this.refreshIntervalCalculator = refreshIntervalCalculator;
        }

        /// <summary>Gets the strategy to be used when fetching a country specific Lotl fails.</summary>
        /// <returns>the strategy to be used when fetching a country specific Lotl fails</returns>
        public virtual IOnCountryFetchFailureStrategy GetOnCountryFetchFailureStrategy() {
            return onCountryFetchFailureStrategy;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual ICollection<String> GetServiceTypes() {
            return JavaCollectionsUtil.UnmodifiableSet(serviceTypes);
        }
//\endcond

        /// <summary>Adds service type identifier which shall be used during country specific Lotl fetching.</summary>
        /// <remarks>
        /// Adds service type identifier which shall be used during country specific Lotl fetching.
        /// <para />
        /// If no service type identifiers are added, all certificates in country specific Lotl files will be used.
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
    }
}
