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

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Class which stores properties related to LOTL (List of Trusted Lists) fetching and validation process.
    ///     </summary>
    public class LotlFetchingProperties {
        private readonly HashSet<String> schemaNames = new HashSet<String>();

        private readonly HashSet<String> serviceTypes = new HashSet<String>();

        /// <summary>
        /// Creates an instance of
        /// <see cref="LotlFetchingProperties"/>.
        /// </summary>
        public LotlFetchingProperties() {
        }

        // Empty constructor.
        /// <summary>Adds schema name (usually two letters) of a country which shall be used during LOTL fetching.</summary>
        /// <remarks>
        /// Adds schema name (usually two letters) of a country which shall be used during LOTL fetching.
        /// <para />
        /// If no schema names are added, all country specific LOTL files will be used.
        /// </remarks>
        /// <param name="schemaName">
        /// country schema name as a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="LotlFetchingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LotlFetchingProperties AddSchemaName(String schemaName) {
            schemaNames.Add(schemaName);
            return this;
        }

        /// <summary>Adds service type identifier which shall be used during country specific LOTL fetching.</summary>
        /// <remarks>
        /// Adds service type identifier which shall be used during country specific LOTL fetching.
        /// <para />
        /// If no service type identifiers are added, all certificates in country specific LOTL files will be used.
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
        public virtual iText.Signatures.Validation.Lotl.LotlFetchingProperties AddServiceType(String serviceType) {
            serviceTypes.Add(serviceType);
            return this;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual ICollection<String> GetSchemaNames() {
            return JavaCollectionsUtil.UnmodifiableSet<String>(schemaNames);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ICollection<String> GetServiceTypes() {
            return JavaCollectionsUtil.UnmodifiableSet<String>(serviceTypes);
        }
//\endcond
    }
}
