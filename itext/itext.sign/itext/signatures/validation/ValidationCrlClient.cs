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
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Validation.Context;
using iText.Signatures.Validation.Dataorigin;

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
        [System.ObsoleteAttribute(@"use AddCrl(iText.Commons.Bouncycastle.Cert.IX509Crl, System.DateTime, iText.Signatures.Validation.Context.TimeBasedContext, iText.Signatures.Validation.Dataorigin.RevocationDataOrigin?) instead"
            )]
        public virtual void AddCrl(IX509Crl response, DateTime date, TimeBasedContext context) {
            AddCrl(response, date, context, RevocationDataOrigin.OTHER);
        }

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
        /// <param name="responseOrigin">
        /// 
        /// <see cref="iText.Signatures.Validation.Dataorigin.RevocationDataOrigin?"/>
        /// representing an origin from which CRL comes from
        /// </param>
        public virtual void AddCrl(IX509Crl response, DateTime date, TimeBasedContext context, RevocationDataOrigin?
             responseOrigin) {
            RevocationDataValidator.CrlValidationInfo validationInfo = crls.Get(response);
            if (validationInfo != null) {
                // If CRL is already there, we don't need to update response origin.
                // But we do need to update so-called trusted generation date.
                // Consider such update as following: we've encountered the same CRL in the document,
                // but now it's covered with a timestamp. So now we know, if was generated at a certain point in time,
                // and we can use this information during the validation.
                // This of course only works, because the CRL is exactly the same.
                if (validationInfo.trustedGenerationDate.After(date)) {
                    // We found better data, so update.
                    validationInfo.trustedGenerationDate = date;
                    validationInfo.timeBasedContext = context;
                }
                if ((int)(validationInfo.responseOrigin) > (int)(responseOrigin)) {
                    // We found better response origin, so update. It's considered better in terms of PAdES compliance.
                    validationInfo.responseOrigin = responseOrigin;
                }
            }
            else {
                crls.Put(response, new RevocationDataValidator.CrlValidationInfo(response, date, context, responseOrigin));
            }
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
