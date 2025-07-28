using System;
using System.Collections.Generic;
using iText.Commons.Utils;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Class which stores properties related to LOTL (List of Trusted Lists) fetching and validation process.
    ///     </summary>
    public class LOTLFetchingProperties {
        private readonly HashSet<String> schemaNames = new HashSet<String>();

        private readonly HashSet<String> serviceTypes = new HashSet<String>();

        /// <summary>
        /// Creates an instance of
        /// <see cref="LOTLFetchingProperties"/>.
        /// </summary>
        public LOTLFetchingProperties() {
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
        /// <see cref="LOTLFetchingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LOTLFetchingProperties AddSchemaName(String schemaName) {
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
        /// <see cref="LOTLFetchingProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Signatures.Validation.Lotl.LOTLFetchingProperties AddServiceType(String serviceType) {
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
