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
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>This class fetches the European Union Journal certificates from the trusted list configuration.</summary>
    /// <remarks>
    /// This class fetches the European Union Journal certificates from the trusted list configuration.
    /// It reads the PEM certificates and returns them in a structured result.
    /// </remarks>
    public class EuropeanResourceFetcher {
        /// <summary>Default constructor for EuropeanResourceFetcher.</summary>
        /// <remarks>
        /// Default constructor for EuropeanResourceFetcher.
        /// Initializes the fetcher without any specific configuration.
        /// </remarks>
        public EuropeanResourceFetcher() {
        }

        // Default constructor
        /// <summary>Fetches the European Union Journal certificates.</summary>
        /// <returns>a Result object containing a list of certificates and any report items</returns>
        public virtual EuropeanResourceFetcher.Result GetEUJournalCertificates() {
            EuropeanResourceFetcher.Result result = new EuropeanResourceFetcher.Result();
            EuropeanTrustedListConfigurationFactory factory = EuropeanTrustedListConfigurationFactory.GetFactory()();
            result.SetCurrentlySupportedPublication(factory.GetCurrentlySupportedPublication());
            SafeCalling.OnExceptionLog(() => result.SetCertificates(factory.GetCertificates()), result.GetLocalReport(
                ), (e) => new ReportItem(LotlValidator.LOTL_VALIDATION, LotlValidator.JOURNAL_CERT_NOT_PARSABLE, e, ReportItem.ReportItemStatus
                .INFO));
            return result;
        }

        /// <summary>Represents the result of fetching European Union Journal certificates.</summary>
        /// <remarks>
        /// Represents the result of fetching European Union Journal certificates.
        /// Contains a list of report items and a list of certificates.
        /// </remarks>
        public class Result {
            private readonly ValidationReport localReport;

            private IList<IX509Certificate> certificates;

            private String currentlySupportedPublication;

            /// <summary>
            /// Create a new Instance of
            /// <see cref="Result"/>.
            /// </summary>
            public Result() {
                this.localReport = new ValidationReport();
                certificates = new List<IX509Certificate>();
            }

            /// <summary>Gets the list of report items.</summary>
            /// <returns>a ValidationReport object containing report items</returns>
            public virtual ValidationReport GetLocalReport() {
                return localReport;
            }

            /// <summary>Gets the list of certificates.</summary>
            /// <returns>a list of Certificate objects</returns>
            public virtual IList<IX509Certificate> GetCertificates() {
                return certificates;
            }

            /// <summary>Gets string constant representing currently used Official Journal publication.</summary>
            /// <returns>
            /// 
            /// <see cref="System.String"/>
            /// constant representing currently used Official Journal publication
            /// </returns>
            public virtual String GetCurrentlySupportedPublication() {
                return currentlySupportedPublication;
            }

            /// <summary>Sets the list of certificates.</summary>
            /// <param name="certificates">a list of Certificate objects to set</param>
            public virtual void SetCertificates(IList<IX509Certificate> certificates) {
                this.certificates = certificates;
            }

            /// <summary>Sets string constant representing currently used Official Journal publication.</summary>
            /// <param name="currentlySuppostedPublication">
            /// 
            /// <see cref="System.String"/>
            /// constant representing currently used Official Journal publication
            /// </param>
            public virtual void SetCurrentlySupportedPublication(String currentlySuppostedPublication) {
                this.currentlySupportedPublication = currentlySuppostedPublication;
            }
        }
    }
}
