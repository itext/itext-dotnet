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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Signatures {
    /// <summary>
    /// Implementation class for
    /// <see cref="IExternalSignatureContainer"/>.
    /// </summary>
    /// <remarks>
    /// Implementation class for
    /// <see cref="IExternalSignatureContainer"/>.
    /// This external signature container is implemented based on PCS7 standard and
    /// <see cref="PdfPKCS7"/>
    /// class.
    /// </remarks>
    public class PKCS7ExternalSignatureContainer : IExternalSignatureContainer {
        private readonly IX509Certificate[] chain;

        private readonly IPrivateKey privateKey;

        private readonly String hashAlgorithm;

        private IOcspClient ocspClient;

        private ICrlClient crlClient;

        private ITSAClient tsaClient;

        private PdfSigner.CryptoStandard sigType = PdfSigner.CryptoStandard.CMS;

        private SignaturePolicyInfo signaturePolicy;

        /// <summary>Creates an instance of PKCS7ExternalSignatureContainer</summary>
        /// <param name="privateKey">The private key to sign with</param>
        /// <param name="chain">The certificate chain</param>
        /// <param name="hashAlgorithm">The hash algorithm to use</param>
        public PKCS7ExternalSignatureContainer(IPrivateKey privateKey, IX509Certificate[] chain, String hashAlgorithm
            ) {
            this.hashAlgorithm = hashAlgorithm;
            this.chain = chain;
            this.privateKey = privateKey;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual byte[] Sign(Stream data) {
            PdfPKCS7 sgn = new PdfPKCS7((IPrivateKey)null, chain, hashAlgorithm, new BouncyCastleDigest(), false);
            if (signaturePolicy != null) {
                sgn.SetSignaturePolicy(signaturePolicy);
            }
            byte[] hash;
            try {
                hash = DigestAlgorithms.Digest(data, SignUtils.GetMessageDigest(hashAlgorithm));
            }
            catch (System.IO.IOException e) {
                throw new PdfException(e);
            }
            ICollection<byte[]> crlBytes = null;
            int i = 0;
            while (crlClient != null && crlBytes == null && i < chain.Length) {
                crlBytes = crlClient.GetEncoded((IX509Certificate)chain[i++], null);
            }
            IList<byte[]> ocspList = new List<byte[]>();
            if (chain.Length > 1 && ocspClient != null) {
                for (int j = 0; j < chain.Length - 1; ++j) {
                    byte[] ocsp = ocspClient.GetEncoded((IX509Certificate)chain[j], (IX509Certificate)chain[j + 1], null);
                    if (ocsp != null && BouncyCastleFactoryCreator.GetFactory().CreateCertificateStatus().GetGood().Equals(OcspClientBouncyCastle
                        .GetCertificateStatus(ocsp))) {
                        ocspList.Add(ocsp);
                    }
                }
            }
            byte[] sh = sgn.GetAuthenticatedAttributeBytes(hash, sigType, ocspList, crlBytes);
            PrivateKeySignature pkSign = new PrivateKeySignature(privateKey, hashAlgorithm);
            byte[] signData = pkSign.Sign(sh);
            sgn.SetExternalSignatureValue(signData, null, pkSign.GetSignatureAlgorithmName(), pkSign.GetSignatureMechanismParameters
                ());
            return sgn.GetEncodedPKCS7(hash, sigType, tsaClient, ocspList, crlBytes);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual void ModifySigningDictionary(PdfDictionary signDic) {
            signDic.Put(PdfName.Filter, PdfName.Adobe_PPKLite);
            signDic.Put(PdfName.SubFilter, sigType == PdfSigner.CryptoStandard.CADES ? PdfName.ETSI_CAdES_DETACHED : PdfName
                .Adbe_pkcs7_detached);
        }

        /// <summary>Set the OcspClient if you want revocation data collected trough Ocsp to be added to the signature
        ///     </summary>
        /// <param name="ocspClient">the client to be used</param>
        public virtual void SetOcspClient(IOcspClient ocspClient) {
            this.ocspClient = ocspClient;
        }

        /// <summary>Set the CrlClient if you want revocation data collected trough Crl to be added to the signature</summary>
        /// <param name="crlClient">the client to be used</param>
        public virtual void SetCrlClient(ICrlClient crlClient) {
            this.crlClient = crlClient;
        }

        /// <summary>Set the TsaClient if you want a TSA timestamp added to the signature</summary>
        /// <param name="tsaClient">the client to use</param>
        public virtual void SetTsaClient(ITSAClient tsaClient) {
            this.tsaClient = tsaClient;
        }

        /// <summary>Set the signature policy if you want it to be added to the signature</summary>
        /// <param name="signaturePolicy">the signature to be set.</param>
        public virtual void SetSignaturePolicy(SignaturePolicyInfo signaturePolicy) {
            this.signaturePolicy = signaturePolicy;
        }

        /// <summary>
        /// Set a custom signature type, default value
        /// <see cref="CryptoStandard.CMS"/>
        /// </summary>
        /// <param name="sigType">the type  of signature to be created</param>
        public virtual void SetSignatureType(PdfSigner.CryptoStandard sigType) {
            this.sigType = sigType;
        }
    }
}
