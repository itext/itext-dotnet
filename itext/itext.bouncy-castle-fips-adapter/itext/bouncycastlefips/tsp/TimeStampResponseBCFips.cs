/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
using System.Text;
using iText.Bouncycastlefips.Asn1.Cmp;
using iText.Commons.Bouncycastle.Asn1.Cmp;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Utilities;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Tsp.TimeStampResp"/>.
    /// </summary>
    public class TimeStampResponseBCFips : ITimeStampResponse {
        private readonly TimeStampResp timeStampResponse;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TimeStampResp"/>.
        /// </summary>
        /// <param name="timeStampResponse">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TimeStampResp"/>
        /// to be wrapped
        /// </param>
        public TimeStampResponseBCFips(TimeStampResp timeStampResponse) {
            this.timeStampResponse = timeStampResponse;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TimeStampResp"/>.
        /// </returns>
        public virtual TimeStampResp GetTimeStampResponse() {
            return timeStampResponse;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Validate(ITimeStampRequest request) {
            TimeStampReq req = ((TimeStampRequestBCFips)request).GetTimeStampRequest();
            TimeStampTokenBCFips tok = (TimeStampTokenBCFips) GetTimeStampToken();
            if (tok.GetCmsSignedData() != null) {
                TstInfo tstInfo = tok.GetTstInfo();
                if (req.Nonce != null && !req.Nonce.Equals(tstInfo.Nonce)) {
                    throw new CmsException("response contains wrong nonce value.");
                }
                if (timeStampResponse.Status.Status.IntValue != (int) PkiStatus.Granted 
                    && timeStampResponse.Status.Status.IntValue != (int) PkiStatus.GrantedWithMods) {
                    throw new CmsException("time stamp token found in failed request.");
                }
                if (!Arrays.ConstantTimeAreEqual(req.MessageImprint.GetHashedMessage(), tstInfo.MessageImprint.GetHashedMessage())) {
                    throw new CmsException("response for different message imprint digest.");
                }
                if (!tstInfo.MessageImprint.HashAlgorithm.Algorithm.Equals(req.MessageImprint.HashAlgorithm.Algorithm)) {
                    throw new CmsException("response for different message imprint algorithm.");
                }
                Org.BouncyCastle.Asn1.Cms.Attribute scV1 = tok.GetSignerInformation().SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificate];
                Org.BouncyCastle.Asn1.Cms.Attribute scV2 = tok.GetSignerInformation().SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificateV2];
                if (scV1 == null && scV2 == null) {
                    throw new CmsException("no signing certificate attribute present.");
                }
                if (scV1 != null && scV2 != null) {
                    /*
                     * RFC 5035 5.4. If both attributes exist in a single message,
                     * they are independently evaluated. 
                     */
                }
                if (req.ReqPolicy != null && !req.ReqPolicy.Equals(tstInfo.Policy)) {
                    throw new CmsException("TSA policy wrong for request.");
                }
            } else if (timeStampResponse.Status.Status.IntValue == (int) PkiStatus.Granted || 
                      timeStampResponse.Status.Status.IntValue == (int) PkiStatus.GrantedWithMods) {
                throw new CmsException("no time stamp token found and one expected.");
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IPKIFailureInfo GetFailInfo() {
            if (timeStampResponse.Status.FailInfo == null) {
                return new PKIFailureInfoBCFips(null);
            }
            return new PKIFailureInfoBCFips(new PkiFailureInfo(timeStampResponse.Status.FailInfo));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampToken GetTimeStampToken() {
            return timeStampResponse.TimeStampToken == null ? 
                new TimeStampTokenBCFips(null, null, null, (EssCertID)null) :
                new TimeStampTokenBCFips(timeStampResponse.TimeStampToken);
            
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetStatusString() {
            if (timeStampResponse.Status.StatusString == null) {
                return null;
            }
            StringBuilder statusStringBuf = new StringBuilder();
            PkiFreeText text = timeStampResponse.Status.StatusString;
            for (int i = 0; i != text.Count; i++) {
                statusStringBuf.Append(text[i].GetString());
            }
            return statusStringBuf.ToString();
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return timeStampResponse.GetEncoded();
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Tsp.TimeStampResponseBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampResponseBCFips
                )o;
            return Object.Equals(timeStampResponse, that.timeStampResponse);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(timeStampResponse);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return timeStampResponse.ToString();
        }
    }
}
