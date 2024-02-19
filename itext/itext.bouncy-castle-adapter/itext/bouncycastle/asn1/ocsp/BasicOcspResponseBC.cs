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
using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastle.Cert;
using iText.Bouncycastle.Cert.Ocsp;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.BasicOcspResponse"/>.
    /// </summary>
    public class BasicOcspResponseBC : Asn1EncodableBC, IBasicOcspResponse {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.BasicOcspResponse"/>.
        /// </summary>
        /// <param name="basicOCSPResponse">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.BasicOcspResponse"/>
        /// to be wrapped
        /// </param>
        public BasicOcspResponseBC(BasicOcspResponse basicOCSPResponse)
            : base(basicOCSPResponse) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.BasicOcspResponse"/>.
        /// </returns>
        public virtual BasicOcspResponse GetBasicOcspResponse() {
            return (BasicOcspResponse)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetProducedAtDate() {
            return GetBasicOcspResponse().TbsResponseData.ProducedAt.ToDateTime();
        }

        /// <summary><inheritDoc/></summary>
        public bool Verify(IX509Certificate cert) {
            try {
                ISigner signature = SignerUtilities.GetSigner(GetBasicOcspResponse().SignatureAlgorithm.Algorithm);
                signature.Init(false, ((PublicKeyBC)cert.GetPublicKey()).GetPublicKey());
                byte[] bs = GetBasicOcspResponse().TbsResponseData.GetDerEncoded();
                signature.BlockUpdate(bs, 0, bs.Length);

                return signature.VerifySignature(GetBasicOcspResponse().GetSignatureOctets());
            }
            catch (InvalidKeyException) {
                return false;
            }
        }

        /// <summary><inheritDoc/></summary>
        public IEnumerable<IX509Certificate> GetCerts() {
            List<IX509Certificate> certificates = new List<IX509Certificate>();
            if (GetBasicOcspResponse().Certs != null) {
                foreach (Asn1Encodable asn1Encodable in GetBasicOcspResponse().Certs) {
                    certificates.Add(new X509CertificateBC(new X509CertificateParser()
                        .ReadCertificate(asn1Encodable.GetEncoded())));
                }
            }
            return certificates;
        }

        /// <summary><inheritDoc/></summary>
        public IX509Certificate[] GetOcspCerts() {
            return ((List<IX509Certificate>)GetCerts()).ToArray();
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GetEncoded() {
            return GetBasicOcspResponse().GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public ISingleResponse[] GetResponses() {
            Asn1Sequence s = GetBasicOcspResponse().TbsResponseData.Responses;
            ISingleResponse[] rs = new ISingleResponse[s.Count];
            for (int i = 0; i != rs.Length; ++i) {
                rs[i] = new SingleResponseBC(SingleResponse.GetInstance(Asn1Sequence.GetInstance(s[i].GetEncoded())));
            }
            return rs;
        }

        /// <summary><inheritDoc/></summary>
        public DateTime GetProducedAt() {
            return GetBasicOcspResponse().TbsResponseData.ProducedAt.ToDateTime();
        }
    }
}
