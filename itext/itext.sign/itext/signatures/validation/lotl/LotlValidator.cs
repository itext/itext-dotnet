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
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>LotlValidator is responsible for validating the List of Trusted Lists (Lotl) and managing the trusted certificates.
    ///     </summary>
    /// <remarks>
    /// LotlValidator is responsible for validating the List of Trusted Lists (Lotl) and managing the trusted certificates.
    /// It fetches the Lotl, validates it, and retrieves country-specific entries.
    /// </remarks>
    public class LotlValidator {
        public const String LOTL_VALIDATION = "Lotl validation.";

//\cond DO_NOT_DOCUMENT
        internal const String JOURNAL_CERT_NOT_PARSABLE = "One of EU Journal trusted certificates in not parsable. "
             + "It " + "will be ignored.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String LOTL_FETCHING_PROPERTIES_NOT_PROVIDED = "Lotl fetching properties have to be provided in "
             + "order to use Lotl Validator. " + "See \"ValidationChainBuilder#withLotlFetchingProperties\"";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String LOTL_VALIDATION_UNSUCCESSFUL = "Lotl chain validation wasn't successful, trusted " +
             "certificates" + " were not parsed.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNABLE_TO_RETRIEVE_PIVOT = "Unable to retrieve pivot Lotl with {0} url. Lotl validation "
             + "isn't" + " successful.";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNABLE_TO_RETRIEVE_LOTL = "Unable to retrieve main Lotl file. Lotl validation isn't "
             + "successful.";
//\endcond

        private readonly IList<CountryServiceContext> nationalTrustedCertificates = new List<CountryServiceContext
            >();

        private readonly LotlService service;

        /// <summary>Constructs a LotlValidator with the specified ValidatorChainBuilder.</summary>
        /// <param name="service">
        /// 
        /// <see cref="LotlService"/>
        /// from which this instance was created.
        /// </param>
        public LotlValidator(LotlService service) {
            this.service = service;
        }

        /// <summary>Validates the List of Trusted Lists (Lotl) and retrieves national trusted certificates.</summary>
        /// <returns>a ValidationReport containing the results of the validation</returns>
        public virtual ValidationReport Validate() {
            ValidationReport report = new ValidationReport();
            EuropeanLotlFetcher.Result lotl = service.GetLotlBytes();
            if (!lotl.GetLocalReport().GetLogs().IsEmpty()) {
                report.Merge(lotl.GetLocalReport());
                return report;
            }
            EuropeanResourceFetcher.Result europeanResult = service.GetEUJournalCertificates();
            report.Merge(europeanResult.GetLocalReport());
            // get all the data from cache, if it is stale, exception will be thrown
            // locked and pass to methods
            PivotFetcher.Result result = service.GetAndValidatePivotFiles(lotl.GetLotlXml(), europeanResult.GetCertificates
                (), europeanResult.GetCurrentlySupportedPublication());
            report.Merge(result.GetLocalReport());
            if (result.GetLocalReport().GetValidationResult() != ValidationReport.ValidationResult.VALID) {
                return report;
            }
            IList<CountrySpecificLotlFetcher.Result> entries = service.GetCountrySpecificLotlFiles(lotl.GetLotlXml());
            foreach (CountrySpecificLotlFetcher.Result countrySpecificResult in entries) {
                // When cache was loaded without config it still contains all country specific Lotl files.
                // So we need to filter them out if schema names were provided.
                if (!this.service.GetLotlFetchingProperties().ShouldProcessCountry(countrySpecificResult.GetCountrySpecificLotl
                    ().GetSchemeTerritory())) {
                    continue;
                }
                report.Merge(countrySpecificResult.GetLocalReport());
                this.nationalTrustedCertificates.AddAll(LotlTrustedStore.MapIServiceContextToCountry(countrySpecificResult
                    .GetContexts()));
            }
            return report;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Retrieves national trusted certificates.</summary>
        /// <returns>the list of national trusted certificates</returns>
        internal virtual IList<IServiceContext> GetNationalTrustedCertificates() {
            return new List<IServiceContext>(nationalTrustedCertificates);
        }
//\endcond
    }
}
