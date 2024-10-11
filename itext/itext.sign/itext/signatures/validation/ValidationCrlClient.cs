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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Validation.Context;

namespace iText.Signatures.Validation {
    /// <summary>CRL client which is expected to be used in case CRL responses shall be linked with generation date.
    ///     </summary>
    public class ValidationCrlClient : ICrlClient {
        private readonly IDictionary<IX509Crl, RevocationDataValidator.CrlValidationInfo> crls = new Dictionary<IX509Crl
            , RevocationDataValidator.CrlValidationInfo>();

        /// <summary>
        /// Create new
        /// <see cref="ValidationCrlClient"/>
        /// instance.
        /// </summary>
        public ValidationCrlClient() {
        }

        // Empty constructor in order for default one to not be removed if another one is added.
        /// <summary>Add CRL response which is linked with generation date.</summary>
        /// <param name="response">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Crl"/>
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
        public virtual void AddCrl(IX509Crl response, DateTime date, TimeBasedContext context) {
            // We need to have these data stored in Map in order to replace duplicates.
            crls.Put(response, new RevocationDataValidator.CrlValidationInfo(response, date, context));
        }

        /// <summary>Get all the CRL responses linked with generation dates.</summary>
        /// <returns>all the CRL responses linked with generation dates</returns>
        public virtual IDictionary<IX509Crl, RevocationDataValidator.CrlValidationInfo> GetCrls() {
            return JavaCollectionsUtil.UnmodifiableMap(crls);
        }

        /// <summary>
        /// <inheritDoc/>.
        /// </summary>
        public virtual ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
            ICollection<byte[]> byteResponses = new List<byte[]>();
            foreach (IX509Crl response in crls.Keys) {
                try {
                    byteResponses.Add(response.GetEncoded());
                }
                catch (AbstractCrlException) {
                }
                catch (Exception) {
                }
            }
            // Do nothing.
            return byteResponses;
        }
    }
}
