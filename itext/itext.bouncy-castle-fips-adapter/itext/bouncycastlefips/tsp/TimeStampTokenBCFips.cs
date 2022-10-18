using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using iText.Bouncycastlefips.Cert;
using iText.Bouncycastlefips.Crypto;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Operators;
using Org.BouncyCastle.Utilities;
using ContentInfo = Org.BouncyCastle.Asn1.Cms.ContentInfo;

namespace iText.Bouncycastlefips.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Tsp.TimeStampToken"/>.
    /// </summary>
    public class TimeStampTokenBCFips : ITimeStampToken {
        private readonly CmsSignedData tsToken;
        private readonly SignerInformation tsaSignerInfo;
        private readonly TstInfo tstInfo;
        private CertID certID;
        /// <summary>
        /// Creates new wrapper instance for TimeStampToken by specifying its fields:
        /// CmsSignedData, SignerInformation, TstInfo, EssCertID.
        /// </summary>
        public TimeStampTokenBCFips(CmsSignedData tsToken, SignerInformation tsaSignerInfo, 
            TstInfo tstInfo, EssCertID certID) {
            this.tsToken = tsToken;
            this.tsaSignerInfo = tsaSignerInfo;
            this.tstInfo = tstInfo;
            this.certID = new CertID(certID);
        }
        
        /// <summary>
        /// Creates new wrapper instance for TimeStampToken by specifying its fields:
        /// CmsSignedData, SignerInformation, TstInfo, EssCertIDv2.
        /// </summary>
        public TimeStampTokenBCFips(CmsSignedData tsToken, SignerInformation tsaSignerInfo, 
            TstInfo tstInfo, EssCertIDv2 certID) {
            this.tsToken = tsToken;
            this.tsaSignerInfo = tsaSignerInfo;
            this.tstInfo = tstInfo;
            this.certID = new CertID(certID);
        }

        public TimeStampTokenBCFips(ContentInfo contentInfo) {
	        CmsSignedData signedData = new CmsSignedData(contentInfo);
	        tsToken = signedData;
	        if (!tsToken.SignedContent.ContentType.Equals(PkcsObjectIdentifiers.IdCTTstInfo)) {
		        throw new CmsException("ContentInfo object not for a time stamp.");
	        }
	        ICollection<SignerInformation> signers = tsToken.GetSignerInfos().GetAll();
	        if (signers.Count != 1) {
		        throw new ArgumentException("Time-stamp token signed by " + signers.Count +
		                                    " signers, but it must contain just the TSA signature.");
	        }
	        IEnumerator signerEnum = signers.GetEnumerator();
	        signerEnum.MoveNext();
	        tsaSignerInfo = (SignerInformation) signerEnum.Current;
	        ICmsTypedData content = tsToken.SignedContent;
	        MemoryStream bOut = new MemoryStream();
	        content.Write(bOut);
	        tstInfo = TstInfo.GetInstance(Asn1Object.FromByteArray(bOut.ToArray()));
	        Org.BouncyCastle.Asn1.Cms.Attribute attr = tsaSignerInfo.SignedAttributes
		        [PkcsObjectIdentifiers.IdAASigningCertificate];
	        if (attr != null) {
		        if (attr.AttrValues[0] is SigningCertificateV2) {
			        SigningCertificateV2 signCert = SigningCertificateV2.GetInstance(attr.AttrValues[0]);
			        certID = new CertID(EssCertIDv2.GetInstance(signCert.GetCerts()[0]));
		        } else {
			        SigningCertificate signCert = SigningCertificate.GetInstance(attr.AttrValues[0]);
			        certID = new CertID(EssCertID.GetInstance(signCert.GetCerts()[0]));
		        }
	        } else {
		        attr = tsaSignerInfo.SignedAttributes[PkcsObjectIdentifiers.IdAASigningCertificateV2];
		        if (attr == null) {
			        throw new CertificateException("no signing certificate attribute found, time stamp invalid.");
		        }
		        SigningCertificateV2 signCertV2 = SigningCertificateV2.GetInstance(attr.AttrValues[0]);
		        certID = new CertID(EssCertIDv2.GetInstance(signCertV2.GetCerts()[0]));
	        }
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="CmsSignedData"/>.
        /// </returns>
        public virtual CmsSignedData GetCmsSignedData() {
            return tsToken;
        }
        
        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="SignerInformation"/>.
        /// </returns>
        public virtual SignerInformation GetSignerInformation() {
	        return tsaSignerInfo;
        }
        
        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TstInfo"/>.
        /// </returns>
        public virtual TstInfo GetTstInfo() {
	        return tstInfo;
        }
        
        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="CmsSignedData"/>.
        /// </returns>
        public virtual Asn1Encodable GetCertID() {
	        return certID.Cert;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampTokenInfo GetTimeStampInfo() {
            return new TimeStampTokenInfoBCFips(tstInfo);
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return tsToken.ToAsn1Structure().GetEncoded(Asn1Encodable.Der);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Validate(IX509Certificate certificate) {
	        X509Certificate cert = ((X509CertificateBCFips)certificate).GetCertificate();
	        byte[] hash = new IDigestBCFips(certID.GetHashAlgorithmName()).Digest(cert.GetEncoded());

	        if (!Arrays.ConstantTimeAreEqual(certID.GetCertHash(), hash)) {
		        throw new CertificateException("certificate hash does not match certID hash.");
	        }

	        if (certID.IssuerSerial != null) {
		        if (!certID.IssuerSerial.Serial.Value.Equals(cert.SerialNumber)) {
			        throw new CertificateException("certificate serial number does not match certID for signature.");
		        }

		        GeneralName[] names = certID.IssuerSerial.Issuer.GetNames();
		        X500Name principal = cert.IssuerDN;
		        bool found = false;

		        for (int i = 0; i != names.Length; i++) {
			        if (names[i].TagNo == 4 && X500Name.GetInstance(names[i].Name).Equivalent(principal)) {
				        found = true;
				        break;
			        }
		        }

		        if (!found) {
			        throw new CertificateException("certificate name does not match certID for signature. ");
		        }
	        }

	        if (cert.Version != 3) {
		        throw new ArgumentException("Certificate must have an ExtendedKeyUsage extension.");
	        }

	        byte[] ext = cert.GetExtensionValue(X509Extensions.ExtendedKeyUsage);
	        if (ext == null) {
		        throw new CertificateException("Certificate must have an ExtendedKeyUsage extension.");
	        }

	        if (!cert.GetCriticalExtensionOids().Contains(X509Extensions.ExtendedKeyUsage)) {
		        throw new CertificateException("Certificate must have an ExtendedKeyUsage extension marked as critical.");
	        }

	        try {
		        ExtendedKeyUsage extKey = ExtendedKeyUsage.GetInstance(Asn1Object.FromByteArray(ext));

		        if (!extKey.HasKeyPurposeId(KeyPurposeID.IdKPTimeStamping) || extKey.Count != 1) {
			        throw new CertificateException("ExtendedKeyUsage not solely time stamping.");
		        }
	        }
	        catch (IOException) {
		        throw new CertificateException("cannot process ExtendedKeyUsage extension");
	        }

	        cert.CheckValidity(tstInfo.GenTime.ToDateTime());

	        if (!tsaSignerInfo.Verify(new PkixSignerInformationVerifierProvider(cert))) {
		        throw new CertificateException("signature not created by certificate.");
	        }
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
            iText.Bouncycastlefips.Tsp.TimeStampTokenBCFips that = (iText.Bouncycastlefips.Tsp.TimeStampTokenBCFips)o;
            return Object.Equals(tsToken, that.tsToken) && 
                   Object.Equals(tsaSignerInfo, that.tsaSignerInfo) && 
                   Object.Equals(tstInfo, that.tstInfo) && 
                   Object.Equals(certID, that.certID);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
	        return JavaUtil.ArraysHashCode<object>(tsToken, tsaSignerInfo, tstInfo, certID);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
	        return tsToken + " " + tsaSignerInfo + " " + tstInfo + " " + certID;
        }
        
        private class CertID {
	        private EssCertID certID;
	        private EssCertIDv2 certIDv2;

	        internal CertID(EssCertID certID) {
		        this.certID = certID;
		        this.certIDv2 = null;
	        }

	        internal CertID(EssCertIDv2 certID) {
		        this.certIDv2 = certID;
		        this.certID = null;
	        }

	        public string GetHashAlgorithmName() {
		        if (certID != null)
			        return "SHA-1";

		        if (NistObjectIdentifiers.IdSha256.Equals(certIDv2.HashAlgorithm.Algorithm))
			        return "SHA-256";

		        return certIDv2.HashAlgorithm.Algorithm.Id;
	        }

	        public AlgorithmIdentifier GetHashAlgorithm() {
		        return certID != null
			        ?	new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1)
			        :	certIDv2.HashAlgorithm;
	        }

	        public byte[] GetCertHash() {
		        return certID != null ?	certID.GetCertHash() : certIDv2.GetCertHash();
	        }

	        public IssuerSerial IssuerSerial => certID != null ? certID.IssuerSerial : certIDv2.IssuerSerial;

	        public Asn1Encodable Cert => certID != null ? certID.ToAsn1Object() : certIDv2.ToAsn1Object();
	        
	        public override string ToString() {
		        return certID != null ? certID.ToString() : certIDv2.ToString();
	        }
        }
    }
}
