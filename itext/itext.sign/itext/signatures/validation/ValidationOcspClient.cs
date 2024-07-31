/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Linq;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;
using iText.Signatures;
using iText.Signatures.Validation.Context;

namespace iText.Signatures.Validation {
    /// <summary>OCSP client which is expected to be used in case OCSP responses shall be linked with generation date.
    ///     </summary>
    public class ValidationOcspClient : IOcspClient {
        private readonly IDictionary<IBasicOcspResponse, RevocationDataValidator.OcspResponseValidationInfo> responses
             = new Dictionary<IBasicOcspResponse, RevocationDataValidator.OcspResponseValidationInfo>();

        /// <summary>
        /// Create new
        /// <see cref="ValidationOcspClient"/>
        /// instance.
        /// </summary>
        public ValidationOcspClient() {
        }

        // Empty constructor in order for default one to not be removed if another one is added.
        /// <summary>Add OCSP response which is linked with generation date.</summary>
        /// <param name="response">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// response to be added
        /// </param>
        /// <param name="date">
        /// 
        /// <see cref="System.DateTime"/>
        /// to be linked with the response
        /// </param>
        /// <param name="context">
        /// 
        /// <see cref="iText.Signatures.Validation.Context.TimeBasedContext"/>
        /// time based context which corresponds to generation date
        /// </param>
        public virtual void AddResponse(IBasicOcspResponse response, DateTime date, TimeBasedContext context) {
            responses.Put(response, new RevocationDataValidator.OcspResponseValidationInfo(null, response, date, context
                ));
        }

        /// <summary>Get all the OCSP responses linked with generation dates.</summary>
        /// <returns>all the OCSP responses linked with generation dates</returns>
        public virtual IDictionary<IBasicOcspResponse, RevocationDataValidator.OcspResponseValidationInfo> GetResponses
            () {
            return JavaCollectionsUtil.UnmodifiableMap(responses);
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual byte[] GetEncoded(IX509Certificate checkCert, IX509Certificate issuerCert, String url) {
            if (responses.IsEmpty()) {
                return null;
            }
            try {
                // This method is never actually expected to be used, that's why we just return single latest response.
                return responses.Sorted((r1, r2) => r2.Key.GetProducedAt().CompareTo(r1.Key.GetProducedAt())).ToList()[0].
                    Key.GetEncoded();
            }
            catch (System.IO.IOException) {
                return null;
            }
            catch (Exception) {
                return null;
            }
        }
    }
}
