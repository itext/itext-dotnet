using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.Cms;
using iText.Bouncycastle.Asn1.Esf;
using iText.Bouncycastle.Asn1.Ess;
using iText.Bouncycastle.Asn1.Ocsp;
using iText.Bouncycastle.Asn1.Pcks;
using iText.Bouncycastle.Asn1.Tsp;
using iText.Bouncycastle.Asn1.Util;
using iText.Bouncycastle.Asn1.X500;
using iText.Bouncycastle.Asn1.X509;
using iText.Bouncycastle.Cert;
using iText.Bouncycastle.Cert.Jcajce;
using iText.Bouncycastle.Cert.Ocsp;
using iText.Bouncycastle.Cms;
using iText.Bouncycastle.Cms.Jcajce;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.Crypto.Generators;
using iText.Bouncycastle.Math;
using iText.Bouncycastle.Operator.Jcajce;
using iText.Bouncycastle.Security;
using iText.Bouncycastle.Tsp;
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
using iText.Commons.Bouncycastle.Cms.Jcajce;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Crypto.Generators;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Bouncycastle.Tsp;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using ICertificateID = iText.Commons.Bouncycastle.Cert.Ocsp.ICertificateID;
using ICipher = iText.Commons.Bouncycastle.Crypto.ICipher;
using ISingleResp = iText.Commons.Bouncycastle.Cert.Ocsp.ISingleResp;

namespace iText.Bouncycastle {
    /// <summary>
    /// This class implements
    /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
    /// and creates bouncy-castle classes instances.
    /// </summary>
    public class BouncyCastleFactory : IBouncyCastleFactory {
        private static readonly String PROVIDER_NAME = "BC";
        private static readonly BouncyCastleTestConstantsFactory BOUNCY_CASTLE_TEST_CONSTANTS = new BouncyCastleTestConstantsFactory();

        public virtual IASN1ObjectIdentifier CreateASN1ObjectIdentifier(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerObjectIdentifier) {
                return new ASN1ObjectIdentifierBC((DerObjectIdentifier)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IASN1ObjectIdentifier CreateASN1ObjectIdentifier(String str) {
            return new ASN1ObjectIdentifierBC(str);
        }

        public virtual IASN1ObjectIdentifier CreateASN1ObjectIdentifierInstance(Object @object) {
            return new ASN1ObjectIdentifierBC(DerObjectIdentifier.GetInstance(@object is ASN1EncodableBC ? ((ASN1EncodableBC
                )@object).GetEncodable() : @object));
        }

        public virtual IASN1InputStream CreateASN1InputStream(Stream stream) {
            return new ASN1InputStreamBC(stream);
        }

        public virtual IASN1InputStream CreateASN1InputStream(byte[] bytes) {
            return new ASN1InputStreamBC(bytes);
        }

        public virtual IASN1OctetString CreateASN1OctetString(IASN1Primitive primitive) {
            ASN1PrimitiveBC primitiveBC = (ASN1PrimitiveBC)primitive;
            if (primitiveBC.GetPrimitive() is Asn1OctetString) {
                return new ASN1OctetStringBC((Asn1OctetString)primitiveBC.GetPrimitive());
            }
            return null;
        }

        public virtual IASN1OctetString CreateASN1OctetString(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is Asn1OctetString) {
                return new ASN1OctetStringBC((Asn1OctetString)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IASN1OctetString CreateASN1OctetString(IASN1TaggedObject taggedObject, bool b) {
            return new ASN1OctetStringBC(taggedObject, b);
        }

        public virtual IASN1OctetString CreateASN1OctetString(byte[] bytes) {
            return new ASN1OctetStringBC(Asn1OctetString.GetInstance(bytes));
        }

        public virtual IASN1Sequence CreateASN1Sequence(Object @object) {
            if (@object is Asn1Sequence) {
                return new ASN1SequenceBC((Asn1Sequence)@object);
            }
            return null;
        }

        public virtual IASN1Sequence CreateASN1Sequence(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is Asn1Sequence) {
                return new ASN1SequenceBC((Asn1Sequence)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IASN1Sequence CreateASN1Sequence(byte[] array) {
            return new ASN1SequenceBC((Asn1Sequence)Asn1Sequence.FromByteArray(array));
        }

        public virtual IASN1Sequence CreateASN1SequenceInstance(Object @object) {
            return new ASN1SequenceBC(@object is ASN1EncodableBC ? ((ASN1EncodableBC)@object).GetEncodable() : @object
                );
        }

        public virtual IDERSequence CreateDERSequence(IASN1EncodableVector encodableVector) {
            ASN1EncodableVectorBC vectorBC = (ASN1EncodableVectorBC)encodableVector;
            return new DERSequenceBC(vectorBC.GetEncodableVector());
        }

        public virtual IDERSequence CreateDERSequence(IASN1Primitive primitive) {
            ASN1PrimitiveBC primitiveBC = (ASN1PrimitiveBC)primitive;
            return new DERSequenceBC(primitiveBC.GetPrimitive());
        }

        public virtual IASN1TaggedObject CreateASN1TaggedObject(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is Asn1TaggedObject) {
                return new ASN1TaggedObjectBC((Asn1TaggedObject)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IASN1Integer CreateASN1Integer(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerInteger) {
                return new ASN1IntegerBC((DerInteger)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IASN1Integer CreateASN1Integer(int i) {
            return new ASN1IntegerBC(i);
        }

        public virtual IASN1Integer CreateASN1Integer(IBigInteger i) {
            return new ASN1IntegerBC(i);
        }

        public virtual IASN1Set CreateASN1Set(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is Asn1Set) {
                return new ASN1SetBC((Asn1Set)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IASN1Set CreateASN1Set(Object encodable) {
            return encodable is Asn1Set ? new ASN1SetBC((Asn1Set)encodable) : null;
        }

        public virtual IASN1Set CreateASN1Set(IASN1TaggedObject taggedObject, bool b) {
            ASN1TaggedObjectBC taggedObjectBC = (ASN1TaggedObjectBC)taggedObject;
            return new ASN1SetBC(taggedObjectBC.GetASN1TaggedObject(), b);
        }

        public virtual IASN1Set CreateNullASN1Set() {
            return new ASN1SetBC(null);
        }

        public virtual IASN1OutputStream CreateASN1OutputStream(Stream stream) {
            return new ASN1OutputStreamBC(stream);
        }

        public virtual IASN1OutputStream CreateASN1OutputStream(Stream outputStream, String asn1Encoding) {
            if (Asn1Encodable.Ber.Equals(asn1Encoding)) {
                return new ASN1OutputStreamBC(new BerOutputStream(outputStream));
            }
            return new ASN1OutputStreamBC(new DerOutputStream(outputStream));
        }

        public virtual IDEROctetString CreateDEROctetString(byte[] bytes) {
            return new DEROctetStringBC(bytes);
        }

        public virtual IDEROctetString CreateDEROctetString(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerOctetString) {
                return new DEROctetStringBC((DerOctetString)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IASN1EncodableVector CreateASN1EncodableVector() {
            return new ASN1EncodableVectorBC();
        }

        public virtual IDERNull CreateDERNull() {
            return DERNullBC.INSTANCE;
        }

        public virtual IDERTaggedObject CreateDERTaggedObject(int i, IASN1Primitive primitive) {
            ASN1PrimitiveBC primitiveBC = (ASN1PrimitiveBC)primitive;
            return new DERTaggedObjectBC(i, primitiveBC.GetPrimitive());
        }

        public virtual IDERTaggedObject CreateDERTaggedObject(bool b, int i, IASN1Primitive primitive) {
            ASN1PrimitiveBC primitiveBC = (ASN1PrimitiveBC)primitive;
            return new DERTaggedObjectBC(b, i, primitiveBC.GetPrimitive());
        }

        public virtual IDERSet CreateDERSet(IASN1EncodableVector encodableVector) {
            ASN1EncodableVectorBC encodableVectorBC = (ASN1EncodableVectorBC)encodableVector;
            return new DERSetBC(encodableVectorBC.GetEncodableVector());
        }

        public virtual IDERSet CreateDERSet(IASN1Primitive primitive) {
            ASN1PrimitiveBC primitiveBC = (ASN1PrimitiveBC)primitive;
            return new DERSetBC(primitiveBC.GetPrimitive());
        }

        public virtual IDERSet CreateDERSet(ISignaturePolicyIdentifier identifier) {
            SignaturePolicyIdentifierBC identifierBC = (SignaturePolicyIdentifierBC)identifier;
            return new DERSetBC(identifierBC.GetSignaturePolicyIdentifier());
        }

        public virtual IDERSet CreateDERSet(IRecipientInfo recipientInfo) {
            RecipientInfoBC recipientInfoBC = (RecipientInfoBC)recipientInfo;
            return new DERSetBC(recipientInfoBC.GetRecipientInfo());
        }

        public virtual IASN1Enumerated CreateASN1Enumerated(int i) {
            return new ASN1EnumeratedBC(i);
        }

        public virtual IASN1Encoding CreateASN1Encoding() {
            return ASN1EncodingBC.GetInstance();
        }

        public virtual IAttributeTable CreateAttributeTable(IASN1Set unat) {
            ASN1SetBC asn1SetBC = (ASN1SetBC)unat;
            return new AttributeTableBC(asn1SetBC.GetASN1Set());
        }

        public virtual IPKCSObjectIdentifiers CreatePKCSObjectIdentifiers() {
            return PKCSObjectIdentifiersBC.GetInstance();
        }

        public virtual IAttribute CreateAttribute(IASN1ObjectIdentifier attrType, IASN1Set attrValues) {
            ASN1ObjectIdentifierBC attrTypeBc = (ASN1ObjectIdentifierBC)attrType;
            ASN1SetBC attrValuesBc = (ASN1SetBC)attrValues;
            return new AttributeBC(new Org.BouncyCastle.Asn1.Cms.Attribute(attrTypeBc.GetASN1ObjectIdentifier(), attrValuesBc
                .GetASN1Set()));
        }

        public virtual IContentInfo CreateContentInfo(IASN1Sequence sequence) {
            ASN1SequenceBC sequenceBC = (ASN1SequenceBC)sequence;
            return new ContentInfoBC(ContentInfo.GetInstance(sequenceBC.GetASN1Sequence()));
        }

        public virtual IContentInfo CreateContentInfo(IASN1ObjectIdentifier objectIdentifier, IASN1Encodable encodable
            ) {
            return new ContentInfoBC(objectIdentifier, encodable);
        }

        public virtual ISigningCertificate CreateSigningCertificate(IASN1Sequence sequence) {
            ASN1SequenceBC sequenceBC = (ASN1SequenceBC)sequence;
            return new SigningCertificateBC(SigningCertificate.GetInstance(sequenceBC.GetASN1Sequence()));
        }

        public virtual ISigningCertificateV2 CreateSigningCertificateV2(IASN1Sequence sequence) {
            ASN1SequenceBC sequenceBC = (ASN1SequenceBC)sequence;
            return new SigningCertificateV2BC(SigningCertificateV2.GetInstance(sequenceBC.GetASN1Sequence()));
        }

        public virtual IBasicOCSPResponse CreateBasicOCSPResponse(IASN1Primitive primitive) {
            ASN1PrimitiveBC primitiveBC = (ASN1PrimitiveBC)primitive;
            return new BasicOCSPResponseBC(BasicOcspResponse.GetInstance(primitiveBC.GetPrimitive()));
        }

        public IBasicOCSPResponse CreateBasicOCSPResponse(object response) {
            if (response is BasicOcspResponse) {
                return new BasicOCSPResponseBC((BasicOcspResponse) response);
            }
            return null;
            
        }

        public virtual IOCSPObjectIdentifiers CreateOCSPObjectIdentifiers() {
            return OCSPObjectIdentifiersBC.GetInstance();
        }

        public virtual IAlgorithmIdentifier CreateAlgorithmIdentifier(IASN1ObjectIdentifier algorithm) {
            ASN1ObjectIdentifierBC algorithmBc = (ASN1ObjectIdentifierBC)algorithm;
            return new AlgorithmIdentifierBC(new AlgorithmIdentifier(algorithmBc.GetASN1ObjectIdentifier(), null));
        }

        public virtual IAlgorithmIdentifier CreateAlgorithmIdentifier(IASN1ObjectIdentifier algorithm, IASN1Encodable
             encodable) {
            ASN1ObjectIdentifierBC algorithmBc = (ASN1ObjectIdentifierBC)algorithm;
            ASN1EncodableBC encodableBc = (ASN1EncodableBC)encodable;
            return new AlgorithmIdentifierBC(new AlgorithmIdentifier(algorithmBc.GetASN1ObjectIdentifier(), encodableBc
                .GetEncodable()));
        }

        public virtual String GetProviderName() {
            return PROVIDER_NAME;
        }

        public virtual IJceKeyTransEnvelopedRecipient CreateJceKeyTransEnvelopedRecipient(ICipherParameters privateKey
            ) {
            return new JceKeyTransEnvelopedRecipientBC(new JceKeyTransEnvelopedRecipient(privateKey));
        }

        public virtual IJcaContentVerifierProviderBuilder CreateJcaContentVerifierProviderBuilder() {
            return new JcaContentVerifierProviderBuilderBC(new JcaContentVerifierProviderBuilder());
        }

        public virtual IJcaSimpleSignerInfoVerifierBuilder CreateJcaSimpleSignerInfoVerifierBuilder() {
            return new JcaSimpleSignerInfoVerifierBuilderBC(new JcaSimpleSignerInfoVerifierBuilder());
        }

        public virtual IJcaX509CertificateConverter CreateJcaX509CertificateConverter() {
            return new JcaX509CertificateConverterBC(new JcaX509CertificateConverter());
        }

        public virtual IJcaDigestCalculatorProviderBuilder CreateJcaDigestCalculatorProviderBuilder() {
            return new JcaDigestCalculatorProviderBuilderBC(new JcaDigestCalculatorProviderBuilder());
        }

        public virtual ICertificateID CreateCertificateID() {
            return CertificateIDBC.GetInstance();
        }

        public virtual IX509CertificateHolder CreateX509CertificateHolder(byte[] bytes) {
            return new X509CertificateHolderBC(bytes);
        }

        public virtual IJcaX509CertificateHolder CreateJcaX509CertificateHolder(X509Certificate certificate) {
            return new JcaX509CertificateHolderBC(new JcaX509CertificateHolder(certificate));
        }

        public virtual IExtension CreateExtension(IASN1ObjectIdentifier objectIdentifier, bool critical, IASN1OctetString
             octetString) {
            IDictionary extension = new Hashtable();
            extension.Add(((ASN1ObjectIdentifierBC)objectIdentifier).GetASN1ObjectIdentifier(),
                new X509Extension(critical, ((ASN1OctetStringBC)octetString).GetASN1OctetString()));
            return new ExtensionBC(new X509Extensions(extension));
        }

        public virtual IExtension CreateExtension() {
            return ExtensionBC.GetInstance();
        }

        public virtual IOCSPReqBuilder CreateOCSPReqBuilder() {
            return new OCSPReqBuilderBC(new OCSPReqBuilder());
        }

        public virtual ISigPolicyQualifierInfo CreateSigPolicyQualifierInfo(IASN1ObjectIdentifier objectIdentifier
            , IDERIA5String @string) {
            return new SigPolicyQualifierInfoBC(objectIdentifier, @string);
        }

        public virtual IASN1String CreateASN1String(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerStringBase) {
                return new ASN1StringBC((DerStringBase)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IASN1Primitive CreateASN1Primitive(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is Asn1Object) {
                return new ASN1PrimitiveBC((Asn1Object)encodableBC.GetEncodable());
            }
            return null;
        }

        public IOCSPResponse CreateOCSPResponse(byte[] bytes) {
            return new OCSPResponseBC(OcspResponse.GetInstance(new Asn1InputStream(bytes).ReadObject()));
        }

        public virtual IOCSPResponse CreateOCSPResponse() {
            return OCSPResponseBC.GetInstance();
        }

        public virtual IOCSPResponse CreateOCSPResponse(IOCSPResponseStatus respStatus, IResponseBytes responseBytes) {
            return new OCSPResponseBC(respStatus, responseBytes);
        }

        public IOCSPResponse CreateOCSPResponse(int respStatus, object response) {
            if (response == null) {
                return new OCSPResponseBC(new OcspResponse(new OcspResponseStatus(respStatus), null));
            }
            BasicOcspResponse basicResp = null;
            if (response is IBasicOCSPResponse) {
                basicResp = ((BasicOCSPResponseBC)response).GetBasicOCSPResponse();
                if (basicResp == null) {
                    return new OCSPResponseBC(new OcspResponse(new OcspResponseStatus(respStatus), null));
                }
            }
            if (response is BasicOcspResponse) {
                basicResp = (BasicOcspResponse)response;
            }
            if (basicResp == null) {
                throw new OCSPExceptionBC(new OcspException("unknown response object"));
            }
            Asn1OctetString octs;
            try {
                octs = new DerOctetString(((BasicOcspResponse)response).GetEncoded());
            } catch (Exception e) {
                throw new OCSPExceptionBC(new OcspException("can't encode object.", e));
            }
            ResponseBytes rb = new ResponseBytes(OcspObjectIdentifiers.PkixOcspBasic, octs);
            return new OCSPResponseBC(new OcspResponse(new OcspResponseStatus(respStatus), rb));
        }

        public virtual IResponseBytes CreateResponseBytes(IASN1ObjectIdentifier asn1ObjectIdentifier, IDEROctetString
             derOctetString) {
            return new ResponseBytesBC(asn1ObjectIdentifier, derOctetString);
        }

        public virtual IOCSPResponseStatus CreateOCSPResponseStatus(int status) {
            return new OCSPResponseStatusBC(new OcspResponseStatus(status));
        }

        public virtual IOCSPResponseStatus CreateOCSPResponseStatus() {
            return OCSPResponseStatusBC.GetInstance();
        }

        public virtual ICertificateStatus CreateCertificateStatus() {
            return CertificateStatusBC.GetInstance();
        }

        public IRevokedStatus CreateRevokedStatus(ICertificateStatus certificateStatus) {
            return new RevokedStatusBC(((CertificateStatusBC) certificateStatus).GetCertificateStatus());
        }

        public virtual IRevokedStatus CreateRevokedStatus(DateTime date, int i) {
            return new RevokedStatusBC(date, i);
        }

        public virtual IASN1Primitive CreateASN1Primitive(byte[] array) {
            return new ASN1PrimitiveBC(array);
        }

        public virtual IDERIA5String CreateDERIA5String(IASN1TaggedObject taggedObject, bool b) {
            return new DERIA5StringBC(((DerIA5String)DerIA5String.GetInstance(((ASN1TaggedObjectBC)taggedObject).GetASN1TaggedObject
                (), b)));
        }

        public virtual IDERIA5String CreateDERIA5String(String str) {
            return new DERIA5StringBC(str);
        }

        public virtual ICRLDistPoint CreateCRLDistPoint(Object @object) {
            return new CRLDistPointBC(CrlDistPoint.GetInstance(@object is ASN1EncodableBC ? ((ASN1EncodableBC)@object)
                .GetEncodable() : @object));
        }

        public virtual IDistributionPointName CreateDistributionPointName() {
            return DistributionPointNameBC.GetInstance();
        }

        public virtual IGeneralNames CreateGeneralNames(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is GeneralNames) {
                return new GeneralNamesBC((GeneralNames)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IGeneralName CreateGeneralName() {
            return GeneralNameBC.GetInstance();
        }

        public virtual IOtherHashAlgAndValue CreateOtherHashAlgAndValue(IAlgorithmIdentifier algorithmIdentifier, 
            IASN1OctetString octetString) {
            return new OtherHashAlgAndValueBC(algorithmIdentifier, octetString);
        }

        public virtual ISignaturePolicyId CreateSignaturePolicyId(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue
             algAndValue) {
            return new SignaturePolicyIdBC(objectIdentifier, algAndValue);
        }

        public virtual ISignaturePolicyId CreateSignaturePolicyId(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue
             algAndValue, params ISigPolicyQualifierInfo[] policyQualifiers) {
            SigPolicyQualifierInfo[] qualifierInfos = new SigPolicyQualifierInfo[policyQualifiers.Length];
            for (int i = 0; i < qualifierInfos.Length; ++i) {
                qualifierInfos[i] = ((SigPolicyQualifierInfoBC)policyQualifiers[i]).GetSigPolicyQualifierInfo();
            }
            return new SignaturePolicyIdBC(objectIdentifier, algAndValue, qualifierInfos);
        }

        public virtual ISignaturePolicyIdentifier CreateSignaturePolicyIdentifier(ISignaturePolicyId policyId) {
            return new SignaturePolicyIdentifierBC(policyId);
        }

        public virtual IEnvelopedData CreateEnvelopedData(IOriginatorInfo originatorInfo, IASN1Set set, IEncryptedContentInfo
             encryptedContentInfo, IASN1Set set1) {
            return new EnvelopedDataBC(originatorInfo, set, encryptedContentInfo, set1);
        }

        public virtual IRecipientInfo CreateRecipientInfo(IKeyTransRecipientInfo keyTransRecipientInfo) {
            return new RecipientInfoBC(keyTransRecipientInfo);
        }

        public virtual IEncryptedContentInfo CreateEncryptedContentInfo(IASN1ObjectIdentifier data, IAlgorithmIdentifier
             algorithmIdentifier, IASN1OctetString octetString) {
            return new EncryptedContentInfoBC(data, algorithmIdentifier, octetString);
        }

        public virtual ITBSCertificate CreateTBSCertificate(IASN1Encodable encodable) {
            return new TBSCertificateBC(TbsCertificateStructure.GetInstance(((ASN1EncodableBC)encodable).GetEncodable(
                )));
        }

        public virtual IIssuerAndSerialNumber CreateIssuerAndSerialNumber(IX500Name issuer, IBigInteger value) {
            return new IssuerAndSerialNumberBC(issuer, ((BigIntegerBC)value).GetBigInteger());
        }

        public virtual IRecipientIdentifier CreateRecipientIdentifier(IIssuerAndSerialNumber issuerAndSerialNumber
            ) {
            return new RecipientIdentifierBC(issuerAndSerialNumber);
        }

        public virtual IKeyTransRecipientInfo CreateKeyTransRecipientInfo(IRecipientIdentifier recipientIdentifier
            , IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString octetString) {
            return new KeyTransRecipientInfoBC(recipientIdentifier, algorithmIdentifier, octetString);
        }

        public virtual IOriginatorInfo CreateNullOriginatorInfo() {
            return new OriginatorInfoBC(null);
        }

        public virtual ICMSEnvelopedData CreateCMSEnvelopedData(byte[] bytes) {
            try {
                return new CMSEnvelopedDataBC(new CmsEnvelopedData(bytes));
            }
            catch (CmsException e) {
                throw new CMSExceptionBC(e);
            }
        }

        public virtual ITimeStampRequestGenerator CreateTimeStampRequestGenerator() {
            return new TimeStampRequestGeneratorBC(new TimeStampRequestGenerator());
        }

        public virtual ITimeStampResponse CreateTimeStampResponse(byte[] respBytes) {
            try {
                return new TimeStampResponseBC(new TimeStampResponse(respBytes));
            } catch (TspException e) {
                throw new TSPExceptionBC(e);
            }
        }

        public virtual AbstractOCSPException CreateAbstractOCSPException(Exception e) {
            return new OCSPExceptionBC(new OcspException(e.Message));
        }

        public virtual IUnknownStatus CreateUnknownStatus() {
            return new UnknownStatusBC();
        }

        public virtual IASN1Dump CreateASN1Dump() {
            return ASN1DumpBC.GetInstance();
        }

        public virtual IASN1BitString CreateASN1BitString(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerBitString) {
                return new ASN1BitStringBC((DerBitString)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IASN1GeneralizedTime CreateASN1GeneralizedTime(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerGeneralizedTime) {
                return new ASN1GeneralizedTimeBC((DerGeneralizedTime)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IASN1UTCTime CreateASN1UTCTime(IASN1Encodable encodable) {
            ASN1EncodableBC encodableBC = (ASN1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerUtcTime) {
                return new ASN1UTCTimeBC((DerUtcTime)encodableBC.GetEncodable());
            }
            return null;
        }

        public virtual IJcaCertStore CreateJcaCertStore(IList<X509Certificate> certificates) {
            return new JcaCertStoreBC(new JcaCertStore(certificates));
        }

        public virtual ITimeStampResponseGenerator CreateTimeStampResponseGenerator(ITimeStampTokenGenerator tokenGenerator
            , ICollection<String> algorithms) {
            return new TimeStampResponseGeneratorBC(tokenGenerator, algorithms);
        }

        public virtual ITimeStampRequest CreateTimeStampRequest(byte[] bytes) {
            return new TimeStampRequestBC(new TimeStampRequest(bytes));
        }

        public virtual IJcaContentSignerBuilder CreateJcaContentSignerBuilder(String algorithm) {
            return new JcaContentSignerBuilderBC(new JcaContentSignerBuilder(algorithm));
        }

        public virtual IJcaSignerInfoGeneratorBuilder CreateJcaSignerInfoGeneratorBuilder(IDigestCalculatorProvider
             digestCalcProviderProvider) {
            return new JcaSignerInfoGeneratorBuilderBC(digestCalcProviderProvider);
        }

        public virtual ITimeStampTokenGenerator CreateTimeStampTokenGenerator(ISignerInfoGenerator siGen, IDigestCalculator
             dgCalc, IASN1ObjectIdentifier policy) {
            return new TimeStampTokenGeneratorBC(siGen, dgCalc, policy);
        }

        public virtual IX500Name CreateX500Name(IX509Certificate certificate) {
            byte[] tbsCertificate = certificate.GetTbsCertificate();
            if (tbsCertificate.Length != 0) {
                return new X500NameBC(X509Name.GetInstance(TbsCertificateStructure.GetInstance(Asn1Object.FromByteArray(
                    certificate.GetTbsCertificate())).Subject));
            }
            return null;
        }

        public virtual IX500Name CreateX500Name(String s) {
            return new X500NameBC(new X509Name(s));
        }

        public virtual IRespID CreateRespID(IX500Name x500Name) {
            return new RespIDBC(x500Name);
        }

        public virtual IBasicOCSPRespBuilder CreateBasicOCSPRespBuilder(IRespID respID) {
            return new BasicOCSPRespBuilderBC(respID);
        }

        public virtual IOCSPReq CreateOCSPReq(byte[] requestBytes) {
            return new OCSPReqBC(new OcspReq(requestBytes));
        }

        public virtual IX509v2CRLBuilder CreateX509v2CRLBuilder(IX500Name x500Name, DateTime date) {
            return new X509v2CRLBuilderBC(x500Name, date);
        }

        public virtual IJcaX509v3CertificateBuilder CreateJcaX509v3CertificateBuilder(X509Certificate signingCert, 
            BigInteger certSerialNumber, DateTime startDate, DateTime endDate, IX500Name subjectDnName, AsymmetricKeyParameter
             publicKey) {
            return new JcaX509v3CertificateBuilderBC(signingCert, certSerialNumber, startDate, endDate, subjectDnName, 
                publicKey);
        }

        public virtual IBasicConstraints CreateBasicConstraints(bool b) {
            return new BasicConstraintsBC(new BasicConstraints(b));
        }

        public virtual IKeyUsage CreateKeyUsage() {
            return KeyUsageBC.GetInstance();
        }

        public virtual IKeyUsage CreateKeyUsage(int i) {
            return new KeyUsageBC(new KeyUsage(i));
        }

        public virtual IKeyPurposeId CreateKeyPurposeId() {
            return KeyPurposeIdBC.GetInstance();
        }

        public virtual IExtendedKeyUsage CreateExtendedKeyUsage(IKeyPurposeId purposeId) {
            return new ExtendedKeyUsageBC(purposeId);
        }

        public virtual IX509ExtensionUtils CreateX509ExtensionUtils(IDigestCalculator digestCalculator) {
            return new X509ExtensionUtilsBC(digestCalculator);
        }

        public virtual ISubjectPublicKeyInfo CreateSubjectPublicKeyInfo(Object @object) {
            return new SubjectPublicKeyInfoBC(@object is ASN1EncodableBC ? ((ASN1EncodableBC)@object).GetEncodable() : 
                @object);
        }

        public virtual ICRLReason CreateCRLReason() {
            return CRLReasonBC.GetInstance();
        }

        public virtual ITSTInfo CreateTSTInfo(IContentInfo contentInfoTsp) {
            CmsProcessable content = new CmsSignedData(((ContentInfoBC) contentInfoTsp).GetContentInfo()).SignedContent;
            MemoryStream bOut = new MemoryStream();
            content.Write(bOut);
            return new TSTInfoBC(TstInfo.GetInstance(Asn1Object.FromByteArray(bOut.ToArray())));
        }

        public virtual ISingleResp CreateSingleResp(IBasicOCSPResponse basicResp) {
            return new SingleRespBC(basicResp);
        }

        public virtual IX509Certificate CreateX509Certificate(object obj) {
            switch (obj) {
                case IX509Certificate _:
                    return (X509CertificateBC) obj;
                case X509Certificate certificate:
                    return new X509CertificateBC(certificate);
                default:
                    return null;
            }
        }
        
        public virtual IX509Certificate CreateX509Certificate(Stream s) {
            return new X509CertificateBC(new X509CertificateParser().ReadCertificate(s));
        }
        
        public IX509Crl CreateX509Crl(Stream input) {
            return new X509CrlBC(new X509CrlParser().ReadCrl(input));
        }

        public IIDigest CreateIDigest(string hashAlgorithm) {
            return new IDigestBC(DigestUtilities.GetDigest(hashAlgorithm));
        }

        public ICertificateID CreateCertificateID(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber) {
            return new CertificateIDBC(hashAlgorithm, issuerCert, serialNumber);
        }
        
        public IX500Name CreateX500NameInstance(IASN1Encodable issuer) {
            return new X500NameBC(X509Name.GetInstance(issuer));
        }

        public IOCSPReq CreateOCSPReq(ICertificateID certId, byte[] documentId) {
            return new OCSPReqBC(certId, documentId);
        }
        
        public IISigner CreateISigner() {
            return new ISignerBC(null);
        }
        
        public List<IX509Certificate> ReadAllCerts(byte[] contentsKey) {
            X509CertificateParser cf = new X509CertificateParser();
            List<IX509Certificate> certs = new List<IX509Certificate>();

            foreach (X509Certificate cc in cf.ReadCertificates(contentsKey)) {
                certs.Add(new X509CertificateBC(cc));
            }
            return certs;
        }

        public AbstractGeneralSecurityException CreateGeneralSecurityException(string exceptionMessage,
            Exception exception) {
            return new GeneralSecurityExceptionBC(exceptionMessage, exception);
        }
        
        public AbstractGeneralSecurityException CreateGeneralSecurityException(string exceptionMessage) {
            return new GeneralSecurityExceptionBC(new GeneralSecurityException(exceptionMessage));
        }
        
        public AbstractGeneralSecurityException CreateGeneralSecurityException() {
            return new GeneralSecurityExceptionBC(new GeneralSecurityException());
        }

    	public IBouncyCastleTestConstantsFactory GetBouncyCastleFactoryTestUtil() {
        	return BOUNCY_CASTLE_TEST_CONSTANTS;
    	}

        public IBigInteger CreateBigInteger() {
            return BigIntegerBC.GetInstance();
        }
        
        public IBigInteger CreateBigInteger(int i, byte[] array) {
            return new BigIntegerBC(new BigInteger(i, array));
        }
        
        public ICipher CreateCipher(bool forEncryption, byte[] key, byte[] iv) {
            return new CipherBC(forEncryption, key, iv);
        }
        
        public IX509Crl CreateNullCrl() {
            return new X509CrlBC(null);
        }

        public ITimeStampToken CreateTimeStampToken(IContentInfo contentInfo) {
            return new TimeStampTokenBC(new TimeStampToken(((ContentInfoBC)contentInfo).GetContentInfo()));
        }

        public IRsaKeyPairGenerator CreateRsa2048KeyPairGenerator() {
            return new RsaKeyPairGeneratorBC();
        }
    }
}
