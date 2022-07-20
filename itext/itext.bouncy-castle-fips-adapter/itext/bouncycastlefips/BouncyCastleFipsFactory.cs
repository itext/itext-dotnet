using System;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.Ess;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Math;
using iText.Bouncycastlefips.Asn1;
using iText.Bouncycastlefips.Asn1.Cms;
using iText.Bouncycastlefips.Asn1.Esf;
using iText.Bouncycastlefips.Asn1.Ess;
using iText.Bouncycastlefips.Asn1.Ocsp;
using iText.Bouncycastlefips.Asn1.Pcks;
using iText.Bouncycastlefips.Asn1.Tsp;
using iText.Bouncycastlefips.Asn1.Util;
using iText.Bouncycastlefips.Asn1.X500;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Bouncycastlefips.Cert;
using iText.Bouncycastlefips.Cert.Jcajce;
using iText.Bouncycastlefips.Cert.Ocsp;
using iText.Bouncycastlefips.Cms;
using iText.Bouncycastlefips.Cms.Jcajce;
using iText.Bouncycastlefips.Crypto;
using iText.Bouncycastlefips.Operator.Jcajce;
using iText.Bouncycastlefips.Tsp;
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
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Operator.Jcajce;
using iText.Commons.Bouncycastle.Tsp;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Utilities.IO;
using ContentInfo = Org.BouncyCastle.Asn1.Cms.ContentInfo;
using SignedData = Org.BouncyCastle.Asn1.Cms.SignedData;

namespace iText.Bouncycastlefips {
    /// <summary>
    /// This class implements
    /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
    /// and creates bouncy-castle FIPS classes instances.
    /// </summary>
    public class BouncyCastleFipsFactory : IBouncyCastleFactory {
        private static readonly String PROVIDER_NAME = new BouncyCastleFipsProvider().GetName();

        public virtual IASN1ObjectIdentifier CreateASN1ObjectIdentifier(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerObjectIdentifier) {
                return new ASN1ObjectIdentifierBCFips((DerObjectIdentifier)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IASN1ObjectIdentifier CreateASN1ObjectIdentifier(String str) {
            return new ASN1ObjectIdentifierBCFips(str);
        }

        public virtual IASN1ObjectIdentifier CreateASN1ObjectIdentifierInstance(Object @object) {
            return new ASN1ObjectIdentifierBCFips(DerObjectIdentifier.GetInstance(@object is ASN1EncodableBCFips ? ((ASN1EncodableBCFips
                )@object).GetEncodable() : @object));
        }

        public virtual IASN1InputStream CreateASN1InputStream(Stream stream) {
            return new ASN1InputStreamBCFips(stream);
        }

        public virtual IASN1InputStream CreateASN1InputStream(byte[] bytes) {
            return new ASN1InputStreamBCFips(bytes);
        }

        public virtual IASN1OctetString CreateASN1OctetString(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            if (primitiveBCFips.GetPrimitive() is Asn1OctetString) {
                return new ASN1OctetStringBCFips((Asn1OctetString)primitiveBCFips.GetPrimitive());
            }
            return null;
        }

        public virtual IASN1OctetString CreateASN1OctetString(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1OctetString) {
                return new ASN1OctetStringBCFips((Asn1OctetString)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IASN1OctetString CreateASN1OctetString(IASN1TaggedObject taggedObject, bool b) {
            return new ASN1OctetStringBCFips(taggedObject, b);
        }

        public virtual IASN1OctetString CreateASN1OctetString(byte[] bytes) {
            return new ASN1OctetStringBCFips(Asn1OctetString.GetInstance(bytes));
        }

        public virtual IASN1Sequence CreateASN1Sequence(Object @object) {
            if (@object is Asn1Sequence) {
                return new ASN1SequenceBCFips((Asn1Sequence)@object);
            }
            return null;
        }

        public virtual IASN1Sequence CreateASN1Sequence(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1Sequence) {
                return new ASN1SequenceBCFips((Asn1Sequence)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IASN1Sequence CreateASN1Sequence(byte[] array) {
            return new ASN1SequenceBCFips((Asn1Sequence)Asn1Sequence.FromByteArray(array));
        }

        public virtual IASN1Sequence CreateASN1SequenceInstance(Object @object) {
            return new ASN1SequenceBCFips(@object is ASN1EncodableBCFips ? ((ASN1EncodableBCFips)@object).GetEncodable
                () : @object);
        }

        public virtual IDERSequence CreateDERSequence(IASN1EncodableVector encodableVector) {
            ASN1EncodableVectorBCFips vectorBCFips = (ASN1EncodableVectorBCFips)encodableVector;
            return new DERSequenceBCFips(vectorBCFips.GetEncodableVector());
        }

        public virtual IDERSequence CreateDERSequence(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            return new DERSequenceBCFips(primitiveBCFips.GetPrimitive());
        }

        public virtual IASN1TaggedObject CreateASN1TaggedObject(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1TaggedObject) {
                return new ASN1TaggedObjectBCFips((Asn1TaggedObject)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IASN1Integer CreateASN1Integer(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerInteger) {
                return new ASN1IntegerBCFips((DerInteger)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IASN1Integer CreateASN1Integer(int i) {
            return new ASN1IntegerBCFips(i);
        }

        public virtual IASN1Integer CreateASN1Integer(IBigInteger i) {
            return new ASN1IntegerBCFips(i);
        }

        public virtual IASN1Set CreateASN1Set(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1Set) {
                return new ASN1SetBCFips((Asn1Set)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IASN1Set CreateASN1Set(Object encodable) {
            return encodable is Asn1Set ? new ASN1SetBCFips((Asn1Set)encodable) : null;
        }

        public virtual IASN1Set CreateASN1Set(IASN1TaggedObject taggedObject, bool b) {
            ASN1TaggedObjectBCFips taggedObjectBCFips = (ASN1TaggedObjectBCFips)taggedObject;
            return new ASN1SetBCFips(taggedObjectBCFips.GetTaggedObject(), b);
        }

        public virtual IASN1Set CreateNullASN1Set() {
            return new ASN1SetBCFips(null);
        }

        public virtual IASN1OutputStream CreateASN1OutputStream(Stream stream) {
            return new ASN1OutputStreamBCFips(stream);
        }

        public virtual IASN1OutputStream CreateASN1OutputStream(Stream outputStream, String asn1Encoding) {
            if (asn1Encoding.Equals("DER")) {
                return new ASN1OutputStreamBCFips(new DEROutputStream(outputStream));
            }
            else {
                return new ASN1OutputStreamBCFips((asn1Encoding.Equals("DL") ? new DLOutputStream(outputStream) : new DerOutputStream
                    (outputStream)));
            }
        }

        public virtual IDEROctetString CreateDEROctetString(byte[] bytes) {
            return new DEROctetStringBCFips(bytes);
        }

        public virtual IDEROctetString CreateDEROctetString(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerOctetString) {
                return new DEROctetStringBCFips((DerOctetString)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IASN1EncodableVector CreateASN1EncodableVector() {
            return new ASN1EncodableVectorBCFips();
        }

        public virtual IDERNull CreateDERNull() {
            return DERNullBCFips.INSTANCE;
        }

        public virtual IDERTaggedObject CreateDERTaggedObject(int i, IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            return new DERTaggedObjectBCFips(i, primitiveBCFips.GetPrimitive());
        }

        public virtual IDERTaggedObject CreateDERTaggedObject(bool b, int i, IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            return new DERTaggedObjectBCFips(b, i, primitiveBCFips.GetPrimitive());
        }

        public virtual IDERSet CreateDERSet(IASN1EncodableVector encodableVector) {
            ASN1EncodableVectorBCFips encodableVectorBCFips = (ASN1EncodableVectorBCFips)encodableVector;
            return new DERSetBCFips(encodableVectorBCFips.GetEncodableVector());
        }

        public virtual IDERSet CreateDERSet(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            return new DERSetBCFips(primitiveBCFips.GetPrimitive());
        }

        public virtual IDERSet CreateDERSet(ISignaturePolicyIdentifier identifier) {
            SignaturePolicyIdentifierBCFips identifierBCFips = (SignaturePolicyIdentifierBCFips)identifier;
            return new DERSetBCFips(identifierBCFips.GetSignaturePolicyIdentifier());
        }

        public virtual IDERSet CreateDERSet(IRecipientInfo recipientInfo) {
            RecipientInfoBCFips recipientInfoBCFips = (RecipientInfoBCFips)recipientInfo;
            return new DERSetBCFips(recipientInfoBCFips.GetRecipientInfo());
        }

        public virtual IASN1Enumerated CreateASN1Enumerated(int i) {
            return new ASN1EnumeratedBCFips(i);
        }

        public virtual IASN1Encoding CreateASN1Encoding() {
            return ASN1EncodingBCFips.GetInstance();
        }

        public virtual IAttributeTable CreateAttributeTable(IASN1Set unat) {
            ASN1SetBCFips asn1SetBCFips = (ASN1SetBCFips)unat;
            return new AttributeTableBCFips(asn1SetBCFips.GetASN1Set());
        }

        public virtual IPKCSObjectIdentifiers CreatePKCSObjectIdentifiers() {
            return PKCSObjectIdentifiersBCFips.GetInstance();
        }

        public virtual IAttribute CreateAttribute(IASN1ObjectIdentifier attrType, IASN1Set attrValues) {
            ASN1ObjectIdentifierBCFips attrTypeBCFips = (ASN1ObjectIdentifierBCFips)attrType;
            ASN1SetBCFips attrValuesBCFips = (ASN1SetBCFips)attrValues;
            return new AttributeBCFips(new Org.BouncyCastle.Asn1.Cms.Attribute(attrTypeBCFips.GetASN1ObjectIdentifier(
                ), attrValuesBCFips.GetASN1Set()));
        }

        public virtual IContentInfo CreateContentInfo(IASN1Sequence sequence) {
            ASN1SequenceBCFips sequenceBCFips = (ASN1SequenceBCFips)sequence;
            return new ContentInfoBCFips(ContentInfo.GetInstance(sequenceBCFips.GetASN1Sequence()));
        }

        public virtual IContentInfo CreateContentInfo(IASN1ObjectIdentifier objectIdentifier, IASN1Encodable encodable
            ) {
            return new ContentInfoBCFips(objectIdentifier, encodable);
        }

        public virtual ISigningCertificate CreateSigningCertificate(IASN1Sequence sequence) {
            ASN1SequenceBCFips sequenceBCFips = (ASN1SequenceBCFips)sequence;
            return new SigningCertificateBCFips(SigningCertificate.GetInstance(sequenceBCFips.GetASN1Sequence()));
        }

        public virtual ISigningCertificateV2 CreateSigningCertificateV2(IASN1Sequence sequence) {
            ASN1SequenceBCFips sequenceBCFips = (ASN1SequenceBCFips)sequence;
            return new SigningCertificateV2BCFips(SigningCertificateV2.GetInstance(sequenceBCFips.GetASN1Sequence()));
        }

        public virtual IBasicOCSPResponse CreateBasicOCSPResponse(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            return new BasicOCSPResponseBCFips(BasicOcspResponse.GetInstance(primitiveBCFips.GetPrimitive()));
        }

        public virtual IOCSPObjectIdentifiers CreateOCSPObjectIdentifiers() {
            return OCSPObjectIdentifiersBCFips.GetInstance();
        }

        public virtual IAlgorithmIdentifier CreateAlgorithmIdentifier(IASN1ObjectIdentifier algorithm) {
            ASN1ObjectIdentifierBCFips algorithmBCFips = (ASN1ObjectIdentifierBCFips)algorithm;
            return new AlgorithmIdentifierBCFips(new AlgorithmIdentifier(algorithmBCFips.GetASN1ObjectIdentifier(), null
                ));
        }

        public virtual IAlgorithmIdentifier CreateAlgorithmIdentifier(IASN1ObjectIdentifier algorithm, IASN1Encodable
             encodable) {
            ASN1ObjectIdentifierBCFips algorithmBCFips = (ASN1ObjectIdentifierBCFips)algorithm;
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            return new AlgorithmIdentifierBCFips(new AlgorithmIdentifier(algorithmBCFips.GetASN1ObjectIdentifier(), encodableBCFips
                .GetEncodable()));
        }

        public virtual Java.Security.Provider CreateProvider() {
            return new BouncyCastleFipsProvider();
        }

        public virtual String GetProviderName() {
            return PROVIDER_NAME;
        }

        public virtual IJceKeyTransEnvelopedRecipient CreateJceKeyTransEnvelopedRecipient(ICipherParameters privateKey
            ) {
            return new JceKeyTransEnvelopedRecipientBCFips(new JceKeyTransEnvelopedRecipient(privateKey));
        }

        public virtual IJcaContentVerifierProviderBuilder CreateJcaContentVerifierProviderBuilder() {
            return new JcaContentVerifierProviderBuilderBCFips(new JcaContentVerifierProviderBuilder());
        }

        public virtual IJcaSimpleSignerInfoVerifierBuilder CreateJcaSimpleSignerInfoVerifierBuilder() {
            return new JcaSimpleSignerInfoVerifierBuilderBCFips(new JcaSimpleSignerInfoVerifierBuilder());
        }

        public virtual IJcaX509CertificateConverter CreateJcaX509CertificateConverter() {
            return new JcaX509CertificateConverterBCFips(new JcaX509CertificateConverter());
        }

        public virtual IJcaDigestCalculatorProviderBuilder CreateJcaDigestCalculatorProviderBuilder() {
            return new JcaDigestCalculatorProviderBuilderBCFips(new JcaDigestCalculatorProviderBuilder());
        }

        public virtual ICertificateID CreateCertificateID() {
            return CertificateIDBCFips.GetInstance();
        }

        public virtual IX509CertificateHolder CreateX509CertificateHolder(byte[] bytes) {
            return new X509CertificateHolderBCFips(bytes);
        }

        public virtual IJcaX509CertificateHolder CreateJcaX509CertificateHolder(X509Certificate certificate) {
            return new JcaX509CertificateHolderBCFips(new JcaX509CertificateHolder(certificate));
        }

        public virtual IExtension CreateExtension(IASN1ObjectIdentifier objectIdentifier, bool critical, IASN1OctetString
             octetString) {
            return new ExtensionBCFips(objectIdentifier, critical, octetString);
        }

        public virtual IExtension CreateExtension() {
            return ExtensionBCFips.GetInstance();
        }

        public virtual IExtensions CreateExtensions(IExtension extension) {
            return new ExtensionsBCFips(extension);
        }

        public virtual IExtensions CreateNullExtensions() {
            return new ExtensionsBCFips((Extensions)null);
        }

        public virtual IOCSPReqBuilder CreateOCSPReqBuilder() {
            return new OCSPReqBuilderBCFips(new OCSPReqBuilder());
        }

        public virtual ISigPolicyQualifiers CreateSigPolicyQualifiers(params ISigPolicyQualifierInfo[] qualifierInfosBCFips
            ) {
            SigPolicyQualifierInfo[] qualifierInfos = new SigPolicyQualifierInfo[qualifierInfosBCFips.Length];
            for (int i = 0; i < qualifierInfos.Length; ++i) {
                qualifierInfos[i] = ((SigPolicyQualifierInfoBCFips)qualifierInfosBCFips[i]).GetQualifierInfo();
            }
            return new SigPolicyQualifiersBCFips(qualifierInfos);
        }

        public virtual ISigPolicyQualifierInfo CreateSigPolicyQualifierInfo(IASN1ObjectIdentifier objectIdentifier
            , IDERIA5String @string) {
            return new SigPolicyQualifierInfoBCFips(objectIdentifier, @string);
        }

        public virtual IASN1String CreateASN1String(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerStringBase) {
                return new ASN1StringBCFips((DerStringBase)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IASN1Primitive CreateASN1Primitive(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1Object) {
                return new ASN1PrimitiveBCFips((Asn1Object)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IOCSPResp CreateOCSPResp(IOCSPResponse ocspResponse) {
            return new OCSPRespBCFips(ocspResponse);
        }

        public virtual IOCSPResp CreateOCSPResp(byte[] bytes) {
            return new OCSPRespBCFips(new OcspResp(bytes));
        }

        public virtual IOCSPResp CreateOCSPResp() {
            return OCSPRespBCFips.GetInstance();
        }

        public virtual IOCSPResponse CreateOCSPResponse(IOCSPResponseStatus respStatus, IResponseBytes responseBytes
            ) {
            return new OCSPResponseBCFips(respStatus, responseBytes);
        }

        public virtual IResponseBytes CreateResponseBytes(IASN1ObjectIdentifier asn1ObjectIdentifier, IDEROctetString
             derOctetString) {
            return new ResponseBytesBCFips(asn1ObjectIdentifier, derOctetString);
        }

        public virtual IOCSPRespBuilder CreateOCSPRespBuilderInstance() {
            return OCSPRespBuilderBCFips.GetInstance();
        }

        public virtual IOCSPRespBuilder CreateOCSPRespBuilder() {
            return new OCSPRespBuilderBCFips(new OCSPRespGenerator());
        }

        public virtual IOCSPResponseStatus CreateOCSPResponseStatus(int status) {
            return new OCSPResponseStatusBCFips(new OcspResponseStatus(status));
        }

        public virtual IOCSPResponseStatus CreateOCSPResponseStatus() {
            return OCSPResponseStatusBCFips.GetInstance();
        }

        public virtual ICertificateStatus CreateCertificateStatus() {
            return CertificateStatusBCFips.GetInstance();
        }

        public virtual IRevokedStatus CreateRevokedStatus(ICertificateStatus certificateStatus) {
            CertificateStatusBCFips certificateStatusBCFips = (CertificateStatusBCFips)certificateStatus;
            if (certificateStatusBCFips.GetCertificateStatus() is RevokedStatus) {
                return new RevokedStatusBCFips((RevokedStatus)certificateStatusBCFips.GetCertificateStatus());
            }
            return null;
        }

        public virtual IRevokedStatus CreateRevokedStatus(DateTime date, int i) {
            return new RevokedStatusBCFips(new RevokedStatus(date, i));
        }

        public virtual IASN1Primitive CreateASN1Primitive(byte[] array) {
            return new ASN1PrimitiveBCFips(array);
        }

        public virtual IDERIA5String CreateDERIA5String(IASN1TaggedObject taggedObject, bool b) {
            return new DERIA5StringBCFips(DerIA5String.GetInstance(((ASN1TaggedObjectBCFips)taggedObject).GetTaggedObject
                (), b));
        }

        public virtual IDERIA5String CreateDERIA5String(String str) {
            return new DERIA5StringBCFips(str);
        }

        public virtual ICRLDistPoint CreateCRLDistPoint(Object @object) {
            return new CRLDistPointBCFips(CrlDistPoint.GetInstance(@object is ASN1EncodableBCFips ? ((ASN1EncodableBCFips
                )@object).GetEncodable() : @object));
        }

        public virtual IDistributionPointName CreateDistributionPointName() {
            return DistributionPointNameBCFips.GetInstance();
        }

        public virtual IGeneralNames CreateGeneralNames(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is GeneralNames) {
                return new GeneralNamesBCFips((GeneralNames)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IGeneralName CreateGeneralName() {
            return GeneralNameBCFips.GetInstance();
        }

        public virtual IOtherHashAlgAndValue CreateOtherHashAlgAndValue(IAlgorithmIdentifier algorithmIdentifier, 
            IASN1OctetString octetString) {
            return new OtherHashAlgAndValueBCFips(algorithmIdentifier, octetString);
        }

        public virtual ISignaturePolicyId CreateSignaturePolicyId(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue
             algAndValue) {
            return new SignaturePolicyIdBCFips(objectIdentifier, algAndValue);
        }

        public virtual ISignaturePolicyId CreateSignaturePolicyId(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue
             algAndValue, ISigPolicyQualifiers policyQualifiers) {
            return new SignaturePolicyIdBCFips(objectIdentifier, algAndValue, policyQualifiers);
        }

        public virtual ISignaturePolicyIdentifier CreateSignaturePolicyIdentifier(ISignaturePolicyId policyId) {
            return new SignaturePolicyIdentifierBCFips(policyId);
        }

        public virtual IEnvelopedData CreateEnvelopedData(IOriginatorInfo originatorInfo, IASN1Set set, IEncryptedContentInfo
             encryptedContentInfo, IASN1Set set1) {
            return new EnvelopedDataBCFips(originatorInfo, set, encryptedContentInfo, set1);
        }

        public virtual IRecipientInfo CreateRecipientInfo(IKeyTransRecipientInfo keyTransRecipientInfo) {
            return new RecipientInfoBCFips(keyTransRecipientInfo);
        }

        public virtual IEncryptedContentInfo CreateEncryptedContentInfo(IASN1ObjectIdentifier data, IAlgorithmIdentifier
             algorithmIdentifier, IASN1OctetString octetString) {
            return new EncryptedContentInfoBCFips(data, algorithmIdentifier, octetString);
        }

        public virtual ITBSCertificate CreateTBSCertificate(IASN1Encodable encodable) {
            return new TBSCertificateBCFips(TbsCertificateStructure.GetInstance(((ASN1EncodableBCFips)encodable).GetEncodable
                ()));
        }

        public virtual IIssuerAndSerialNumber CreateIssuerAndSerialNumber(IX500Name issuer, BigInteger value) {
            return new IssuerAndSerialNumberBCFips(issuer, value);
        }

        public virtual IRecipientIdentifier CreateRecipientIdentifier(IIssuerAndSerialNumber issuerAndSerialNumber
            ) {
            return new RecipientIdentifierBCFips(issuerAndSerialNumber);
        }

        public virtual IKeyTransRecipientInfo CreateKeyTransRecipientInfo(IRecipientIdentifier recipientIdentifier
            , IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString octetString) {
            return new KeyTransRecipientInfoBCFips(recipientIdentifier, algorithmIdentifier, octetString);
        }

        public virtual IOriginatorInfo CreateNullOriginatorInfo() {
            return new OriginatorInfoBCFips(null);
        }

        public virtual ICMSEnvelopedData CreateCMSEnvelopedData(byte[] bytes) {
            try {
                return new CMSEnvelopedDataBCFips(new CMSEnvelopedData(bytes));
            }
            catch (CmsException e) {
                throw new CMSExceptionBCFips(e);
            }
        }

        public virtual ITimeStampRequestGenerator CreateTimeStampRequestGenerator() {
            return new TimeStampRequestGeneratorBCFips(new TimeStampRequestGenerator());
        }

        public virtual ITimeStampResponse CreateTimeStampResponse(byte[] respBytes) {
            try {
                return new TimeStampResponseBCFips(new TimeStampResponse(respBytes));
            }
            catch (TspException e) {
                throw new TSPExceptionBCFips(e);
            }
        }

        public virtual AbstractOCSPException CreateAbstractOCSPException(Exception e) {
            return new OCSPExceptionBCFips(new OcspException(e.Message));
        }

        public virtual IUnknownStatus CreateUnknownStatus() {
            return new UnknownStatusBCFips(new UnknownStatus());
        }

        public virtual IASN1Dump CreateASN1Dump() {
            return ASN1DumpBCFips.GetInstance();
        }

        public virtual IASN1BitString CreateASN1BitString(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerBitString) {
                return new ASN1BitStringBCFips((DerBitString)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IASN1GeneralizedTime CreateASN1GeneralizedTime(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerGeneralizedTime) {
                return new ASN1GeneralizedTimeBCFips((DerGeneralizedTime)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IASN1UTCTime CreateASN1UTCTime(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerUtcTime) {
                return new ASN1UTCTimeBCFips((DerUtcTime)encodableBCFips.GetEncodable());
            }
            return null;
        }

        public virtual IJcaCertStore CreateJcaCertStore(IList<X509Certificate> certificates) {
            return new JcaCertStoreBCFips(new JcaCertStore(certificates));
        }

        public virtual ITimeStampResponseGenerator CreateTimeStampResponseGenerator(ITimeStampTokenGenerator tokenGenerator
            , ICollection<String> algorithms) {
            return new TimeStampResponseGeneratorBCFips(tokenGenerator, algorithms);
        }

        public virtual ITimeStampRequest CreateTimeStampRequest(byte[] bytes) {
            return new TimeStampRequestBCFips(new TimeStampRequest(bytes));
        }

        public virtual IJcaContentSignerBuilder CreateJcaContentSignerBuilder(String algorithm) {
            return new JcaContentSignerBuilderBCFips(new JcaContentSignerBuilder(algorithm));
        }

        public virtual IJcaSignerInfoGeneratorBuilder CreateJcaSignerInfoGeneratorBuilder(IDigestCalculatorProvider
             digestCalcProviderProvider) {
            return new JcaSignerInfoGeneratorBuilderBCFips(digestCalcProviderProvider);
        }

        public virtual ITimeStampTokenGenerator CreateTimeStampTokenGenerator(ISignerInfoGenerator siGen, IDigestCalculator
             dgCalc, IASN1ObjectIdentifier policy) {
            return new TimeStampTokenGeneratorBCFips(siGen, dgCalc, policy);
        }

        public virtual IX500Name CreateX500Name(X509Certificate certificate) {
            return new X500NameBCFips(X509Name.GetInstance(TbsCertificateStructure.GetInstance(Asn1Object.FromByteArray
                (certificate.GetTbsCertificate())).GetSubject()));
        }

        public virtual IX500Name CreateX500Name(String s) {
            return new X500NameBCFips(new X509Name(s));
        }

        public virtual IRespID CreateRespID(IX500Name x500Name) {
            return new RespIDBCFips(x500Name);
        }

        public virtual IBasicOCSPRespBuilder CreateBasicOCSPRespBuilder(IRespID respID) {
            return new BasicOCSPRespBuilderBCFips(respID);
        }

        public virtual IOCSPReq CreateOCSPReq(byte[] requestBytes) {
            return new OCSPReqBCFips(new OcspReq(requestBytes));
        }

        public virtual IX509v2CRLBuilder CreateX509v2CRLBuilder(IX500Name x500Name, DateTime date) {
            return new X509v2CRLBuilderBCFips(x500Name, date);
        }

        public virtual IJcaX509v3CertificateBuilder CreateJcaX509v3CertificateBuilder(X509Certificate signingCert, 
            BigInteger certSerialNumber, DateTime startDate, DateTime endDate, IX500Name subjectDnName, AsymmetricKeyParameter
             publicKey) {
            return new JcaX509v3CertificateBuilderBCFips(signingCert, certSerialNumber, startDate, endDate, subjectDnName
                , publicKey);
        }

        public virtual IBasicConstraints CreateBasicConstraints(bool b) {
            return new BasicConstraintsBCFips(new BasicConstraints(b));
        }

        public virtual IKeyUsage CreateKeyUsage() {
            return KeyUsageBCFips.GetInstance();
        }

        public virtual IKeyUsage CreateKeyUsage(int i) {
            return new KeyUsageBCFips(new KeyUsage(i));
        }

        public virtual IKeyPurposeId CreateKeyPurposeId() {
            return KeyPurposeIdBCFips.GetInstance();
        }

        public virtual IExtendedKeyUsage CreateExtendedKeyUsage(IKeyPurposeId purposeId) {
            return new ExtendedKeyUsageBCFips(purposeId);
        }

        public virtual IX509ExtensionUtils CreateX509ExtensionUtils(IDigestCalculator digestCalculator) {
            return new X509ExtensionUtilsBCFips(digestCalculator);
        }

        public virtual ISubjectPublicKeyInfo CreateSubjectPublicKeyInfo(Object @object) {
            return new SubjectPublicKeyInfoBCFips(@object is ASN1EncodableBCFips ? ((ASN1EncodableBCFips)@object).GetEncodable
                () : @object);
        }

        public virtual ICRLReason CreateCRLReason() {
            return CRLReasonBCFips.GetInstance();
        }

        public ITSTInfo CreateTSTInfo(IContentInfo contentInfoTsp) {
            ICmsTypedData content = new CmsSignedData(((ContentInfoBCFips) contentInfoTsp).GetContentInfo()).SignedContent;
            MemoryStream bOut = new MemoryStream();
            content.Write(bOut);
            return new TSTInfoBCFips(TstInfo.GetInstance(Asn1Object.FromByteArray(bOut.ToArray())));
        }

        public virtual ISingleResp CreateSingleResp(IBasicOCSPResponse basicResp) {
            return new SingleRespBCFips(basicResp);
        }

        public virtual IX509Certificate CreateX509Certificate(object obj) {
            switch (obj) {
                case IX509Certificate _:
                    return (X509CertificateBCFips) obj;
                case X509Certificate certificate:
                    return new X509CertificateBCFips(certificate);
                default:
                    return null;
            }
        }
        
        public IX509Crl CreateX509Crl(Stream input) {
            PushbackStream pushbackStream = new PushbackStream(input);
            int tag = pushbackStream.ReadByte();

            if (tag < 0) {
                return new X509CrlBCFips(null);
            }
            
            pushbackStream.Unread(tag);
            
            Asn1InputStream asn1 = new Asn1InputStream(pushbackStream);

            Asn1Sequence seq = (Asn1Sequence)asn1.ReadObject();

            if (seq.Count > 1 && seq[0] is DerObjectIdentifier) {
                if (seq[0].Equals(PkcsObjectIdentifiers.SignedData)) {
                    Asn1Set sCrlData = SignedData.GetInstance(
                        Asn1Sequence.GetInstance((Asn1TaggedObject) seq[1], true)).CRLs;
                    return new X509CrlBCFips(new X509Crl(CertificateList.GetInstance(sCrlData[0])));
                }
            }
            return new X509CrlBCFips(new X509Crl(CertificateList.GetInstance(seq)));
        }
        
        public IIDigest CreateIDigest(string hashAlgorithm) {
            return new IDigestBCFips(hashAlgorithm);
        }
        
        public ICertificateID CreateCertificateID(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber) {
            return new CertificateIDBCFips(hashAlgorithm, issuerCert, serialNumber);
        }
        
        public IX500Name CreateX500NameInstance(IASN1Encodable issuer) {
            return new X500NameBCFips(X500Name.GetInstance(
                ((ASN1EncodableBCFips) issuer).GetEncodable()));
        }
        
        public IOCSPReq CreateOCSPReq(ICertificateID certId, byte[] documentId) {
            return new OCSPReqBCFips(certId, documentId);
        }
        
        public IISigner CreateISigner() {
            return new ISignerBCFips(null);
        }
        
        public List<IX509Certificate> ReadAllCerts(byte[] contentsKey) {
            List<IX509Certificate> certs = new List<IX509Certificate>();
            X509Certificate cert;
            while ((cert = ReadCertificate(new MemoryStream(contentsKey))) != null) {
                certs.Add(new X509CertificateBCFips(cert));
            }
            return certs;
        }
        
        private static X509Certificate ReadCertificate(MemoryStream stream) {
            PushbackStream pushbackStream = new PushbackStream(stream);
            int tag = pushbackStream.ReadByte();
            if (tag < 0) {
                return null;
            }
            pushbackStream.Unread(tag);
            Asn1Sequence seq = (Asn1Sequence)(new Asn1InputStream(pushbackStream).ReadObject());
            if (seq.Count > 1 && seq[0] is DerObjectIdentifier) {
                if (seq[0].Equals(PkcsObjectIdentifiers.SignedData)) {
                    Asn1Set sData = SignedData.GetInstance(
                        Asn1Sequence.GetInstance((Asn1TaggedObject) seq[1], true)).Certificates;
                    object obj = sData[0];
                    if (obj is Asn1Sequence) {
                        return new X509Certificate(X509CertificateStructure.GetInstance(obj));
                    }
                }
            }
            return new X509Certificate(X509CertificateStructure.GetInstance(seq));
        }
    }
}
