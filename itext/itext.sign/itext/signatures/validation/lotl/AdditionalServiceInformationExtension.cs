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

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Wrapper class for additional service information extension.</summary>
    public class AdditionalServiceInformationExtension {
//\cond DO_NOT_DOCUMENT
        internal const String FOR_E_SIGNATURES = "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForeSignatures";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String FOR_E_SEALS = "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForeSeals";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String FOR_WSA = "http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForWebSiteAuthentication";
//\endcond

        private static readonly ICollection<String> INVALID_SCOPES = new HashSet<String>();

        private String uri;

        static AdditionalServiceInformationExtension() {
            INVALID_SCOPES.Add(FOR_WSA);
        }

        /// <summary>
        /// Creates empty instance of
        /// <see cref="AdditionalServiceInformationExtension"/>.
        /// </summary>
        public AdditionalServiceInformationExtension() {
        }

//\cond DO_NOT_DOCUMENT
        // Empty constructor.
        internal AdditionalServiceInformationExtension(String uri) {
            this.uri = uri;
        }
//\endcond

        /// <summary>
        /// Gets URI representing a value of
        /// <see cref="AdditionalServiceInformationExtension"/>.
        /// </summary>
        /// <returns>
        /// URI representing a value of
        /// <see cref="AdditionalServiceInformationExtension"/>
        /// </returns>
        public virtual String GetUri() {
            return uri;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void SetUri(String uri) {
            this.uri = uri;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsScopeValid() {
            return !INVALID_SCOPES.Contains(uri);
        }
//\endcond
    }
}
