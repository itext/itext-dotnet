using System;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Asn1.Ocsp;
using iText.Bouncycastlefips.Cert;
using iText.Bouncycastlefips.Cert.Ocsp;
using iText.Bouncycastlefips.Crypto;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Asymmetric;
using Org.BouncyCastle.Crypto.Fips;

namespace iText.Bouncycastlefips.Asn1.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Ocsp.BasicOcspResponse"/>.
    /// </summary>
    public class BasicOCSPResponseBCFips : ASN1EncodableBCFips, IBasicOCSPResponse {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.BasicOcspResponse"/>.
        /// </summary>
        /// <param name="basicOCSPResponse">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.BasicOcspResponse"/>
        /// to be wrapped
        /// </param>
        public BasicOCSPResponseBCFips(BasicOcspResponse basicOCSPResponse)
            : base(basicOCSPResponse) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Ocsp.BasicOcspResponse"/>.
        /// </returns>
        public virtual BasicOcspResponse GetBasicOCSPResponse() {
            return (BasicOcspResponse)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetProducedAtDate() {
            return GetBasicOCSPResponse().TbsResponseData.ProducedAt.ToDateTime();
        }
        
        /// <summary><inheritDoc/></summary>
        public bool Verify(IX509Certificate cert) {
            IVerifierFactoryService verifierFactoryProvider =
                CryptoServicesRegistrar.CreateService((AsymmetricRsaPublicKey) 
                    ((PublicKeyBCFips)cert.GetPublicKey()).GetPublicKey());
            IVerifierFactory<FipsRsa.SignatureParameters> rsaVer =
                verifierFactoryProvider.CreateVerifierFactory(
                    FipsRsa.Pkcs1v15.WithDigest(FipsShs.Sha256));
            IStreamCalculator<IVerifier> verCalc = rsaVer.CreateCalculator();
            using (Stream verStream = verCalc.Stream) {
                verStream.Write(GetBasicOCSPResponse().TbsResponseData.GetDerEncoded(), 
                    0, GetBasicOCSPResponse().TbsResponseData.GetDerEncoded().Length);
            }
            return verCalc.GetResult().IsVerified(GetBasicOCSPResponse().GetSignatureOctets());
        }

        /// <summary><inheritDoc/></summary>
        public IEnumerable<IX509Certificate> GetCerts() {
            List<IX509Certificate> certificates = new List<IX509Certificate>();
            foreach (Asn1Object asn1Object in GetBasicOCSPResponse().Certs) {
                certificates.Add(new X509CertificateBCFips(new X509Certificate(asn1Object.GetEncoded())));
            }
            return certificates;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GetEncoded() {
            return GetBasicOCSPResponse().GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public ISingleResp[] GetResponses() {
            Asn1Sequence s = GetBasicOCSPResponse().TbsResponseData.Responses;
            ISingleResp[] rs = new ISingleResp[s.Count];
            for (int i = 0; i != rs.Length; ++i) {
                rs[i] = new SingleRespBCFips(SingleResponse.GetInstance(Asn1Sequence.GetInstance(s[i].GetEncoded())));
            }
            return rs;
        }
    }
}
