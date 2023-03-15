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
using iText.Commons.Bouncycastle.Cert.Jcajce;
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
        public IASN1ObjectIdentifier CreateASN1ObjectIdentifier(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1ObjectIdentifier CreateASN1ObjectIdentifier(string str) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1ObjectIdentifier CreateASN1ObjectIdentifierInstance(object @object) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1InputStream CreateASN1InputStream(Stream stream) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1InputStream CreateASN1InputStream(byte[] bytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1OctetString CreateASN1OctetString(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1OctetString CreateASN1OctetString(IASN1TaggedObject taggedObject, bool b) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1OctetString CreateASN1OctetString(byte[] bytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1OctetString CreateASN1OctetString(IASN1Primitive primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Sequence CreateASN1Sequence(object @object) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Sequence CreateASN1Sequence(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Sequence CreateASN1Sequence(byte[] array) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Sequence CreateASN1SequenceInstance(object @object) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERSequence CreateDERSequence(IASN1EncodableVector encodableVector) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERSequence CreateDERSequence(IASN1Primitive primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1TaggedObject CreateASN1TaggedObject(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Integer CreateASN1Integer(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Integer CreateASN1Integer(int i) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Integer CreateASN1Integer(IBigInteger i) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Set CreateASN1Set(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Set CreateASN1Set(object encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Set CreateASN1Set(IASN1TaggedObject taggedObject, bool b) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Set CreateNullASN1Set() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1OutputStream CreateASN1OutputStream(Stream stream) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1OutputStream CreateASN1OutputStream(Stream outputStream, string asn1Encoding) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDEROctetString CreateDEROctetString(byte[] bytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDEROctetString CreateDEROctetString(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1EncodableVector CreateASN1EncodableVector() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERNull CreateDERNull() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERTaggedObject CreateDERTaggedObject(int i, IASN1Primitive primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERTaggedObject CreateDERTaggedObject(bool b, int i, IASN1Primitive primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERSet CreateDERSet(IASN1EncodableVector encodableVector) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERSet CreateDERSet(IASN1Primitive primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERSet CreateDERSet(ISignaturePolicyIdentifier identifier) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERSet CreateDERSet(IRecipientInfo recipientInfo) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Enumerated CreateASN1Enumerated(int i) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Encoding CreateASN1Encoding() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAttributeTable CreateAttributeTable(IASN1Set unat) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IPKCSObjectIdentifiers CreatePKCSObjectIdentifiers() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAttribute CreateAttribute(IASN1ObjectIdentifier attrType, IASN1Set attrValues) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IContentInfo CreateContentInfo(IASN1Sequence sequence) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IContentInfo CreateContentInfo(IASN1ObjectIdentifier objectIdentifier, IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITimeStampToken CreateTimeStampToken(IContentInfo contentInfo) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISigningCertificate CreateSigningCertificate(IASN1Sequence sequence) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISigningCertificateV2 CreateSigningCertificateV2(IASN1Sequence sequence) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IBasicOCSPResponse CreateBasicOCSPResponse(IASN1Primitive primitive) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IBasicOCSPResponse CreateBasicOCSPResponse(object response) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOCSPObjectIdentifiers CreateOCSPObjectIdentifiers() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAlgorithmIdentifier CreateAlgorithmIdentifier(IASN1ObjectIdentifier algorithm) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IAlgorithmIdentifier CreateAlgorithmIdentifier(IASN1ObjectIdentifier algorithm, IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRsassaPssParameters CreateRSASSAPSSParams(IASN1Encodable encodable)
        {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRsassaPssParameters CreateRSASSAPSSParamsWithMGF1(IASN1ObjectIdentifier digestAlgoOid, int saltLen, int trailerField)
        {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public string GetProviderName() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICertificateID CreateCertificateID() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IExtensions CreateExtensions() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IExtensions CreateExtensions(IDictionary objectIdentifier) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOCSPReqBuilder CreateOCSPReqBuilder() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISigPolicyQualifierInfo CreateSigPolicyQualifierInfo(IASN1ObjectIdentifier objectIdentifier, IDERIA5String @string) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1String CreateASN1String(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Primitive CreateASN1Primitive(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Primitive CreateASN1Primitive(byte[] array) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOCSPResponse CreateOCSPResponse(byte[] bytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOCSPResponse CreateOCSPResponse() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOCSPResponse CreateOCSPResponse(IOCSPResponseStatus respStatus, IResponseBytes responseBytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IResponseBytes CreateResponseBytes(IASN1ObjectIdentifier asn1ObjectIdentifier, IDEROctetString derOctetString) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOCSPResponse CreateOCSPResponse(int respStatus, object ocspRespObject) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOCSPResponseStatus CreateOCSPResponseStatus(int status) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOCSPResponseStatus CreateOCSPResponseStatus() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICertificateStatus CreateCertificateStatus() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRevokedStatus CreateRevokedStatus(ICertificateStatus certificateStatus) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRevokedStatus CreateRevokedStatus(DateTime date, int i) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERIA5String CreateDERIA5String(IASN1TaggedObject taggedObject, bool b) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDERIA5String CreateDERIA5String(string str) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICRLDistPoint CreateCRLDistPoint(object @object) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IDistributionPointName CreateDistributionPointName() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IGeneralNames CreateGeneralNames(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IGeneralName CreateGeneralName() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOtherHashAlgAndValue
            CreateOtherHashAlgAndValue(IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString octetString) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISignaturePolicyId CreateSignaturePolicyId(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISignaturePolicyId CreateSignaturePolicyId(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue,
            params ISigPolicyQualifierInfo[] policyQualifiers) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISignaturePolicyIdentifier CreateSignaturePolicyIdentifier(ISignaturePolicyId policyId) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IEnvelopedData CreateEnvelopedData(IOriginatorInfo originatorInfo, IASN1Set set,
            IEncryptedContentInfo encryptedContentInfo, IASN1Set set1) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRecipientInfo CreateRecipientInfo(IKeyTransRecipientInfo keyTransRecipientInfo) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IEncryptedContentInfo CreateEncryptedContentInfo(IASN1ObjectIdentifier data, IAlgorithmIdentifier algorithmIdentifier,
            IASN1OctetString octetString) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITBSCertificate CreateTBSCertificate(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IIssuerAndSerialNumber CreateIssuerAndSerialNumber(IX500Name issuer, IBigInteger value) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IRecipientIdentifier CreateRecipientIdentifier(IIssuerAndSerialNumber issuerAndSerialNumber) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IKeyTransRecipientInfo CreateKeyTransRecipientInfo(IRecipientIdentifier recipientIdentifier,
            IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString octetString) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOriginatorInfo CreateNullOriginatorInfo() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICMSEnvelopedData CreateCMSEnvelopedData(byte[] valueBytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITimeStampRequestGenerator CreateTimeStampRequestGenerator() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITimeStampResponse CreateTimeStampResponse(byte[] respBytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public AbstractOCSPException CreateAbstractOCSPException(Exception e) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IUnknownStatus CreateUnknownStatus() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1Dump CreateASN1Dump() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1BitString CreateASN1BitString(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1GeneralizedTime CreateASN1GeneralizedTime(IASN1Encodable encodable) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IASN1UTCTime CreateASN1UTCTime(IASN1Encodable encodable) {
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

        public IBasicOCSPRespBuilder CreateBasicOCSPRespBuilder(IRespID respID) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOCSPReq CreateOCSPReq(byte[] requestBytes) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509v2CRLBuilder CreateX509v2CRLBuilder(IX500Name x500Name, DateTime thisUpdate) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IJcaX509v3CertificateBuilder CreateJcaX509v3CertificateBuilder(IX509Certificate signingCert,
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

        public IKeyPurposeId CreateKeyPurposeId() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IExtendedKeyUsage CreateExtendedKeyUsage(IKeyPurposeId purposeId) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISubjectPublicKeyInfo CreateSubjectPublicKeyInfo(IPublicKey publicKey) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICRLReason CreateCRLReason() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ITSTInfo CreateTSTInfo(IContentInfo contentInfo) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ISingleResp CreateSingleResp(IBasicOCSPResponse basicResp) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509Certificate CreateX509Certificate(Stream s) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX509Crl CreateX509Crl(Stream input) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IIDigest CreateIDigest(string hashAlgorithm) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public ICertificateID CreateCertificateID(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IX500Name CreateX500NameInstance(IASN1Encodable issuer) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IOCSPReq CreateOCSPReq(ICertificateID certId, byte[] documentId) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public IISigner CreateISigner() {
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

        public IPEMParser CreatePEMParser(TextReader reader, char[] password) {
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

        public IExtension CreateExtension(bool b, IDEROctetString octetString) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public bool IsNullExtension(IExtension extNonce) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public byte[] CreateCipherBytes(IX509Certificate x509Certificate, byte[] abyte0, IAlgorithmIdentifier algorithmidentifier) {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }

        public bool IsInApprovedOnlyMode() {
            throw new NotSupportedException(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
        }
    }
}
