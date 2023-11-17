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
using System.Collections;
using System.IO;
using iText.Bouncycastleconnector.Logs;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Cms;
using iText.Commons.Bouncycastle.Asn1.Esf;
using iText.Commons.Bouncycastle.Asn1.Ess;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.Pkcs;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.Util;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Crypto.Generators;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Openssl;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Bouncycastle.X509;

namespace iText.Bouncycastleconnector {
    /// <summary>
    /// Default bouncy-castle factory which is expected to be used when no other factories can be created.
    /// </summary>
    public class BouncyCastleDefaultFactory : IBouncyCastleFactory {
        
        public virtual String GetAlgorithmOid(String name) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }
        
        public virtual String GetDigestAlgorithmOid(String name) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public virtual String GetAlgorithmName(String oid) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }
        
        public IDerObjectIdentifier CreateASN1ObjectIdentifier(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerObjectIdentifier CreateASN1ObjectIdentifier(string str) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerObjectIdentifier CreateASN1ObjectIdentifierInstance(object @object) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1InputStream CreateASN1InputStream(Stream stream) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1InputStream CreateASN1InputStream(byte[] bytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1OctetString CreateASN1OctetString(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1OctetString CreateASN1OctetString(IAsn1TaggedObject taggedObject, bool b) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1OctetString CreateASN1OctetString(byte[] bytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1OctetString CreateASN1OctetString(IAsn1Object primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Sequence CreateASN1Sequence(object @object) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Sequence CreateASN1Sequence(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Sequence CreateASN1Sequence(byte[] array) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Sequence CreateASN1SequenceInstance(object @object) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerSequence CreateDERSequence(IAsn1EncodableVector encodableVector) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerSequence CreateDERSequence(IAsn1Object primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1TaggedObject CreateASN1TaggedObject(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerInteger CreateASN1Integer(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerInteger CreateASN1Integer(int i) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerInteger CreateASN1Integer(IBigInteger i) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Set CreateASN1Set(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Set CreateASN1Set(object encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Set CreateASN1Set(IAsn1TaggedObject taggedObject, bool b) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Set CreateNullASN1Set() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerOutputStream CreateASN1OutputStream(Stream stream) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerOutputStream CreateASN1OutputStream(Stream outputStream, string asn1Encoding) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerOctetString CreateDEROctetString(byte[] bytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerOctetString CreateDEROctetString(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1EncodableVector CreateASN1EncodableVector() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerNull CreateDERNull() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerTaggedObject CreateDERTaggedObject(int i, IAsn1Object primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerTaggedObject CreateDERTaggedObject(bool b, int i, IAsn1Object primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerSet CreateDERSet(IAsn1EncodableVector encodableVector) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerSet CreateDERSet(IAsn1Object primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerSet CreateDERSet(ISignaturePolicyIdentifier identifier) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerSet CreateDERSet(IRecipientInfo recipientInfo) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerEnumerated CreateASN1Enumerated(int i) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Encoding CreateASN1Encoding() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAttributeTable CreateAttributeTable(IAsn1Set unat) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IPkcsObjectIdentifiers CreatePKCSObjectIdentifiers() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAttribute CreateAttribute(IDerObjectIdentifier attrType, IAsn1Set attrValues) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IContentInfo CreateContentInfo(IAsn1Sequence sequence) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IContentInfo CreateContentInfo(IDerObjectIdentifier objectIdentifier, IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITimeStampToken CreateTimeStampToken(IContentInfo contentInfo) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISigningCertificate CreateSigningCertificate(IAsn1Sequence sequence) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISigningCertificateV2 CreateSigningCertificateV2(IAsn1Sequence sequence) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IBasicOcspResponse CreateBasicOCSPResponse(IAsn1Object primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }
        
        public IBasicOcspResponse CreateBasicOCSPResponse(byte[] bytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IBasicOcspResponse CreateBasicOCSPResponse(object response) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOcspObjectIdentifiers CreateOCSPObjectIdentifiers() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAlgorithmIdentifier CreateAlgorithmIdentifier(IDerObjectIdentifier algorithm) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAlgorithmIdentifier CreateAlgorithmIdentifier(IDerObjectIdentifier algorithm, IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRsassaPssParameters CreateRSASSAPSSParams(IAsn1Encodable encodable)
        {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRsassaPssParameters CreateRSASSAPSSParamsWithMGF1(IDerObjectIdentifier digestAlgoOid, int saltLen, int trailerField)
        {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public string GetProviderName() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICertID CreateCertificateID() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509Extensions CreateExtensions() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509Extensions CreateExtensions(IDictionary objectIdentifier) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOcspReqGenerator CreateOCSPReqBuilder() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISigPolicyQualifierInfo CreateSigPolicyQualifierInfo(IDerObjectIdentifier objectIdentifier, IDerIA5String @string) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerStringBase CreateASN1String(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Object CreateASN1Primitive(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Object CreateASN1Primitive(byte[] array) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOcspResponse CreateOCSPResponse(byte[] bytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOcspResponse CreateOCSPResponse() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOcspResponse CreateOCSPResponse(IOcspResponseStatus respStatus, IResponseBytes responseBytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IResponseBytes CreateResponseBytes(IDerObjectIdentifier asn1ObjectIdentifier, IDerOctetString derOctetString) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOcspResponse CreateOCSPResponse(int respStatus, object ocspRespObject) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOcspResponseStatus CreateOCSPResponseStatus(int status) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOcspResponseStatus CreateOCSPResponseStatus() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICertStatus CreateCertificateStatus() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRevokedCertStatus CreateRevokedStatus(ICertStatus certificateStatus) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRevokedCertStatus CreateRevokedStatus(DateTime date, int i) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerIA5String CreateDERIA5String(IAsn1TaggedObject taggedObject, bool b) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerIA5String CreateDERIA5String(string str) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICrlDistPoint CreateCRLDistPoint(object @object) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDistributionPointName CreateDistributionPointName() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IGeneralNames CreateGeneralNames(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IGeneralName CreateGeneralName() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOtherHashAlgAndValue
            CreateOtherHashAlgAndValue(IAlgorithmIdentifier algorithmIdentifier, IAsn1OctetString octetString) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISignaturePolicyId CreateSignaturePolicyId(IDerObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISignaturePolicyId CreateSignaturePolicyId(IDerObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue,
            params ISigPolicyQualifierInfo[] policyQualifiers) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISignaturePolicyIdentifier CreateSignaturePolicyIdentifier(ISignaturePolicyId policyId) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IEnvelopedData CreateEnvelopedData(IOriginatorInfo originatorInfo, IAsn1Set set,
            IEncryptedContentInfo encryptedContentInfo, IAsn1Set set1) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRecipientInfo CreateRecipientInfo(IKeyTransRecipientInfo keyTransRecipientInfo) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IEncryptedContentInfo CreateEncryptedContentInfo(IDerObjectIdentifier data, IAlgorithmIdentifier algorithmIdentifier,
            IAsn1OctetString octetString) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITbsCertificateStructure CreateTBSCertificate(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IIssuerAndSerialNumber CreateIssuerAndSerialNumber(IX500Name issuer, IBigInteger value) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRecipientIdentifier CreateRecipientIdentifier(IIssuerAndSerialNumber issuerAndSerialNumber) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IKeyTransRecipientInfo CreateKeyTransRecipientInfo(IRecipientIdentifier recipientIdentifier,
            IAlgorithmIdentifier algorithmIdentifier, IAsn1OctetString octetString) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOriginatorInfo CreateNullOriginatorInfo() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICmsEnvelopedData CreateCMSEnvelopedData(byte[] valueBytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITimeStampRequestGenerator CreateTimeStampRequestGenerator() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITimeStampResponse CreateTimeStampResponse(byte[] respBytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public AbstractOcspException CreateAbstractOCSPException(Exception e) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IUnknownCertStatus CreateUnknownStatus() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAsn1Dump CreateASN1Dump() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerBitString CreateASN1BitString(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerGeneralizedTime CreateASN1GeneralizedTime(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDerUtcTime CreateASN1UTCTime(IAsn1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITimeStampTokenGenerator CreateTimeStampTokenGenerator(IPrivateKey pk, IX509Certificate cert, string allowedDigest,
            string policyOid) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITimeStampResponseGenerator CreateTimeStampResponseGenerator(ITimeStampTokenGenerator tokenGenerator, IList algorithms) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITimeStampRequest CreateTimeStampRequest(byte[] bytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX500Name CreateX500Name(IX509Certificate certificate) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX500Name CreateX500Name(string s) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRespID CreateRespID(IX500Name x500Name) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IBasicOcspRespGenerator CreateBasicOCSPRespBuilder(IRespID respID) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOcspRequest CreateOCSPReq(byte[] requestBytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509V2CrlGenerator CreateX509v2CRLBuilder(IX500Name x500Name, DateTime thisUpdate) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509V3CertificateGenerator CreateJcaX509v3CertificateBuilder(IX509Certificate signingCert,
            IBigInteger certSerialNumber, DateTime startDate, DateTime endDate, IX500Name subjectDnName, IPublicKey publicKey) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IBasicConstraints CreateBasicConstraints(bool b) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IKeyUsage CreateKeyUsage() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IKeyUsage CreateKeyUsage(int i) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IKeyPurposeID CreateKeyPurposeId() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IExtendedKeyUsage CreateExtendedKeyUsage(IKeyPurposeID purposeId) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISubjectPublicKeyInfo CreateSubjectPublicKeyInfo(IPublicKey publicKey) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICrlReason CreateCRLReason() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITstInfo CreateTSTInfo(IContentInfo contentInfo) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISingleResponse CreateSingleResp(IBasicOcspResponse basicResp) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509Certificate CreateX509Certificate(Stream s) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509Crl CreateX509Crl(Stream input) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDigest CreateIDigest(string hashAlgorithm) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICertID CreateCertificateID(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX500Name CreateX500NameInstance(IAsn1Encodable issuer) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOcspRequest CreateOCSPReq(ICertID certId, byte[] documentId) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISigner CreateISigner() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509CertificateParser CreateX509CertificateParser() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public AbstractGeneralSecurityException CreateGeneralSecurityException(string exceptionMessage, Exception exception) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public AbstractGeneralSecurityException CreateGeneralSecurityException(string exceptionMessage) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public AbstractGeneralSecurityException CreateGeneralSecurityException() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509Certificate CreateX509Certificate(object element) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IBouncyCastleTestConstantsFactory GetBouncyCastleFactoryTestUtil() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IBigInteger CreateBigInteger() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IBigInteger CreateBigInteger(int i, byte[] array) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IBigInteger CreateBigInteger(string str) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICipher CreateCipher(bool forEncryption, byte[] key, byte[] iv) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICipherCBCnoPad CreateCipherCbCnoPad(bool forEncryption, byte[] key, byte[] iv) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICipherCBCnoPad CreateCipherCbCnoPad(bool forEncryption, byte[] key) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509Crl CreateNullCrl() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRsaKeyPairGenerator CreateRsa2048KeyPairGenerator() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IPemReader CreatePEMParser(TextReader reader, char[] password) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IContentSigner CreateContentSigner(string signatureAlgorithm, IPrivateKey signingKey) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(ISubjectPublicKeyInfo issuerPublicKeyInfo) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISubjectKeyIdentifier CreateSubjectKeyIdentifier(ISubjectPublicKeyInfo subjectPublicKeyInfo) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509Extension CreateExtension(bool b, IDerOctetString octetString) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public bool IsNullExtension(IX509Extension extNonce) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public byte[] CreateCipherBytes(IX509Certificate x509Certificate, byte[] abyte0, IAlgorithmIdentifier algorithmidentifier) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public bool IsInApprovedOnlyMode() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public void IsEncryptionFeatureSupported(int encryptionType, bool withCertificate) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }
        
        public IBouncyCastleUtil GetBouncyCastleUtil() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }
    }
}
