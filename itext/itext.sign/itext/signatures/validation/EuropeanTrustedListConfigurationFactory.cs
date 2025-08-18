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
using iText.Kernel.Exceptions;
using iText.Signatures.Exceptions;

namespace iText.Signatures.Validation {
    /// <summary>Abstract factory class for configuring and retrieving European Trusted List configurations.</summary>
    /// <remarks>
    /// Abstract factory class for configuring and retrieving European Trusted List configurations.
    /// This class provides methods to get and set the factory implementation, as well as abstract
    /// methods to retrieve trusted list-related information.
    /// </remarks>
    public abstract class EuropeanTrustedListConfigurationFactory {
        /// <summary>Supplier for the current factory implementation.</summary>
        /// <remarks>
        /// Supplier for the current factory implementation.
        /// By default, it uses
        /// <see cref="LoadFromModuleEuropeanTrustedListConfigurationFactory"/>.
        /// </remarks>
        private static Func<EuropeanTrustedListConfigurationFactory> configuration = () => {
            try {
                return new LoadFromModuleEuropeanTrustedListConfigurationFactory();
            }
            catch (Exception) {
                throw new PdfException(SignExceptionMessageConstant.EU_RESOURCES_NOT_LOADED);
            }
        }
        ;

        /// <summary>Retrieves the current factory supplier.</summary>
        /// <returns>the current factory supplier</returns>
        public static Func<EuropeanTrustedListConfigurationFactory> GetFactory() {
            return configuration;
        }

        /// <summary>Sets the factory supplier.</summary>
        /// <param name="factory">the new factory supplier to set</param>
        public static void SetFactory(Func<EuropeanTrustedListConfigurationFactory> factory) {
            if (factory == null) {
                throw new ArgumentException("Factory cannot be null");
            }
            configuration = factory;
        }

        /// <summary>Retrieves the URI of the trusted list.</summary>
        /// <returns>the trusted list URI</returns>
        public abstract String GetTrustedListUri();

        /// <summary>Retrieves the currently supported publication of the trusted list.</summary>
        /// <returns>the currently supported publication</returns>
        public abstract String GetCurrentlySupportedPublication();

        /// <summary>Retrieves the list of certificates from the trusted list.</summary>
        /// <returns>a list of certificates</returns>
        public abstract IList<IX509Certificate> GetCertificates();
    }
}
