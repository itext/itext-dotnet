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
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Asn1;
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
using iText.Bouncycastle.Asn1.X509;
using iText.Bouncycastle.Cert;
using iText.Bouncycastle.Cert.Ocsp;
using iText.Bouncycastle.Cms;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.Crypto.Generators;
using iText.Bouncycastle.Math;
using iText.Bouncycastle.Openssl;
using iText.Bouncycastle.Operator;
using iText.Bouncycastle.Security;
using iText.Bouncycastle.Tsp;
using iText.Bouncycastle.X509;
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
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using ContentInfo = Org.BouncyCastle.Asn1.Cms.ContentInfo;
using ICipher = iText.Commons.Bouncycastle.Crypto.ICipher;
using IDigest = iText.Commons.Bouncycastle.Crypto.IDigest;
using ISigner = iText.Commons.Bouncycastle.Crypto.ISigner;
using IX509Extension = iText.Commons.Bouncycastle.Asn1.X509.IX509Extension;

namespace iText.Bouncycastle {
    /// <summary>
    /// This class implements
    /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
    /// and creates bouncy-castle classes instances.
    /// </summary>
    public class BouncyCastleFactory : IBouncyCastleFactory {
        private static readonly String PROVIDER_NAME = "BC";

        private static readonly BouncyCastleTestConstantsFactory BOUNCY_CASTLE_TEST_CONSTANTS =
            new BouncyCastleTestConstantsFactory();

        /// <summary>
        /// Creates
        /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
        /// for bouncy-castle FIPS module.
        /// </summary>
        public BouncyCastleFactory() {
            // Empty constructor.
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetAlgorithmOid(String name) {
            try {
                return new DefaultSignatureAlgorithmIdentifierFinder().Find(name).Algorithm.Id;
            } catch (ArgumentException) {
                return null;
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetAlgorithmName(String oid) {
            try {
                return SignerUtilities.GetEncodingName(new DerObjectIdentifier(oid));
            } catch (FormatException) {
                return oid;
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier CreateASN1ObjectIdentifier(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerObjectIdentifier) {
                return new DerObjectIdentifierBC((DerObjectIdentifier)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier CreateASN1ObjectIdentifier(String str) {
            return new DerObjectIdentifierBC(str);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier CreateASN1ObjectIdentifierInstance(Object @object) {
            return new DerObjectIdentifierBC(DerObjectIdentifier.GetInstance(@object is Asn1EncodableBC ? ((Asn1EncodableBC
                )@object).GetEncodable() : @object));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1InputStream CreateASN1InputStream(Stream stream) {
            return new Asn1InputStreamBC(stream);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1InputStream CreateASN1InputStream(byte[] bytes) {
            return new Asn1InputStreamBC(bytes);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1OctetString CreateASN1OctetString(IAsn1Object primitive) {
            Asn1ObjectBC primitiveBC = (Asn1ObjectBC)primitive;
            if (primitiveBC.GetPrimitive() is Asn1OctetString) {
                return new Asn1OctetStringBC((Asn1OctetString)primitiveBC.GetPrimitive());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1OctetString CreateASN1OctetString(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is Asn1OctetString) {
                return new Asn1OctetStringBC((Asn1OctetString)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1OctetString CreateASN1OctetString(IAsn1TaggedObject taggedObject, bool b) {
            return new Asn1OctetStringBC(taggedObject, b);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1OctetString CreateASN1OctetString(byte[] bytes) {
            return new Asn1OctetStringBC(Asn1OctetString.GetInstance(bytes));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Sequence CreateASN1Sequence(Object @object) {
            if (@object is Asn1Sequence) {
                return new Asn1SequenceBC((Asn1Sequence)@object);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Sequence CreateASN1Sequence(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is Asn1Sequence) {
                return new Asn1SequenceBC((Asn1Sequence)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Sequence CreateASN1Sequence(byte[] array) {
            return new Asn1SequenceBC((Asn1Sequence)Asn1Sequence.FromByteArray(array));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Sequence CreateASN1SequenceInstance(Object @object) {
            return new Asn1SequenceBC(@object is Asn1EncodableBC ? ((Asn1EncodableBC)@object).GetEncodable() : @object
                );
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSequence CreateDERSequence(IAsn1EncodableVector encodableVector) {
            Asn1EncodableVectorBC vectorBC = (Asn1EncodableVectorBC)encodableVector;
            return new DerSequenceBC(vectorBC.GetEncodableVector());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSequence CreateDERSequence(IAsn1Object primitive) {
            Asn1ObjectBC primitiveBC = (Asn1ObjectBC)primitive;
            return new DerSequenceBC(primitiveBC.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1TaggedObject CreateASN1TaggedObject(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is Asn1TaggedObject) {
                return new Asn1TaggedObjectBC((Asn1TaggedObject)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerInteger CreateASN1Integer(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerInteger) {
                return new DerIntegerBC((DerInteger)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerInteger CreateASN1Integer(int i) {
            return new DerIntegerBC(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerInteger CreateASN1Integer(IBigInteger i) {
            return new DerIntegerBC(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Set CreateASN1Set(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is Asn1Set) {
                return new Asn1SetBC((Asn1Set)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Set CreateASN1Set(Object encodable) {
            return encodable is Asn1Set ? new Asn1SetBC((Asn1Set)encodable) : null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Set CreateASN1Set(IAsn1TaggedObject taggedObject, bool b) {
            Asn1TaggedObjectBC taggedObjectBC = (Asn1TaggedObjectBC)taggedObject;
            return new Asn1SetBC(taggedObjectBC.GetAsn1TaggedObject(), b);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Set CreateNullASN1Set() {
            return new Asn1SetBC(null);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerOutputStream CreateASN1OutputStream(Stream stream) {
            return new DerOutputStreamBC(stream);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerOutputStream CreateASN1OutputStream(Stream outputStream, String asn1Encoding) {
            return new DerOutputStreamBC(Asn1OutputStream.Create(outputStream));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerOctetString CreateDEROctetString(byte[] bytes) {
            return new DerOctetStringBC(bytes);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerOctetString CreateDEROctetString(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerOctetString) {
                return new DerOctetStringBC((DerOctetString)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1EncodableVector CreateASN1EncodableVector() {
            return new Asn1EncodableVectorBC();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerNull CreateDERNull() {
            return DerNullBC.INSTANCE;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerTaggedObject CreateDERTaggedObject(int i, IAsn1Object primitive) {
            Asn1ObjectBC primitiveBC = (Asn1ObjectBC)primitive;
            return new DerTaggedObjectBC(i, primitiveBC.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerTaggedObject CreateDERTaggedObject(bool b, int i, IAsn1Object primitive) {
            Asn1ObjectBC primitiveBC = (Asn1ObjectBC)primitive;
            return new DerTaggedObjectBC(b, i, primitiveBC.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSet CreateDERSet(IAsn1EncodableVector encodableVector) {
            Asn1EncodableVectorBC encodableVectorBC = (Asn1EncodableVectorBC)encodableVector;
            return new DerSetBC(encodableVectorBC.GetEncodableVector());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSet CreateDERSet(IAsn1Object primitive) {
            Asn1ObjectBC primitiveBC = (Asn1ObjectBC)primitive;
            return new DerSetBC(primitiveBC.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSet CreateDERSet(ISignaturePolicyIdentifier identifier) {
            SignaturePolicyIdentifierBC identifierBC = (SignaturePolicyIdentifierBC)identifier;
            return new DerSetBC(identifierBC.GetSignaturePolicyIdentifier());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSet CreateDERSet(IRecipientInfo recipientInfo) {
            RecipientInfoBC recipientInfoBC = (RecipientInfoBC)recipientInfo;
            return new DerSetBC(recipientInfoBC.GetRecipientInfo());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerEnumerated CreateASN1Enumerated(int i) {
            return new DerEnumeratedBC(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Encoding CreateASN1Encoding() {
            return ASN1EncodingBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAttributeTable CreateAttributeTable(IAsn1Set unat) {
            Asn1SetBC asn1SetBC = (Asn1SetBC)unat;
            return new AttributeTableBC(asn1SetBC.GetAsn1Set());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IPkcsObjectIdentifiers CreatePKCSObjectIdentifiers() {
            return PkcsObjectIdentifiersBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAttribute CreateAttribute(IDerObjectIdentifier attrType, IAsn1Set attrValues) {
            DerObjectIdentifierBC attrTypeBc = (DerObjectIdentifierBC)attrType;
            Asn1SetBC attrValuesBc = (Asn1SetBC)attrValues;
            return new AttributeBC(new Org.BouncyCastle.Asn1.Cms.Attribute(attrTypeBc.GetDerObjectIdentifier(), attrValuesBc
                .GetAsn1Set()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IContentInfo CreateContentInfo(IAsn1Sequence sequence) {
            Asn1SequenceBC sequenceBC = (Asn1SequenceBC)sequence;
            return new ContentInfoBC(ContentInfo.GetInstance(sequenceBC.GetAsn1Sequence()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IContentInfo CreateContentInfo(IDerObjectIdentifier objectIdentifier, IAsn1Encodable encodable
            ) {
            return new ContentInfoBC(objectIdentifier, encodable);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISigningCertificate CreateSigningCertificate(IAsn1Sequence sequence) {
            Asn1SequenceBC sequenceBC = (Asn1SequenceBC)sequence;
            return new SigningCertificateBC(SigningCertificate.GetInstance(sequenceBC.GetAsn1Sequence()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISigningCertificateV2 CreateSigningCertificateV2(IAsn1Sequence sequence) {
            Asn1SequenceBC sequenceBC = (Asn1SequenceBC)sequence;
            return new SigningCertificateV2BC(SigningCertificateV2.GetInstance(sequenceBC.GetAsn1Sequence()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOcspResponse CreateBasicOCSPResponse(IAsn1Object primitive) {
            Asn1ObjectBC primitiveBC = (Asn1ObjectBC)primitive;
            return new BasicOcspResponseBC(BasicOcspResponse.GetInstance(primitiveBC.GetPrimitive()));
        }

        /// <summary><inheritDoc/></summary>
        public IBasicOcspResponse CreateBasicOCSPResponse(object response) {
            if (response is BasicOcspResponse) {
                return new BasicOcspResponseBC((BasicOcspResponse) response);
            }
            return null;
            
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspObjectIdentifiers CreateOCSPObjectIdentifiers() {
            return OcspObjectIdentifiersBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier CreateAlgorithmIdentifier(IDerObjectIdentifier algorithm) {
            DerObjectIdentifierBC algorithmBc = (DerObjectIdentifierBC)algorithm;
            return new AlgorithmIdentifierBC(new AlgorithmIdentifier(algorithmBc.GetDerObjectIdentifier(), null));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier CreateAlgorithmIdentifier(IDerObjectIdentifier algorithm, IAsn1Encodable
             parameters) {
            DerObjectIdentifierBC algorithmBc = (DerObjectIdentifierBC)algorithm;
            Asn1EncodableBC encodableBc = (Asn1EncodableBC)parameters;
            return new AlgorithmIdentifierBC(new AlgorithmIdentifier(algorithmBc.GetDerObjectIdentifier(), encodableBc
                .GetEncodable()));
        }
        
        /// <summary><inheritDoc/></summary>
        public virtual IRsassaPssParameters CreateRSASSAPSSParams(IAsn1Encodable encodable) {
            if (encodable == null) {
                throw new ArgumentException("Expected non-null RSASSA-PSS parameter data");
            }
            Asn1EncodableBC encodableBC = (Asn1EncodableBC) encodable;
            return new RsassaPssParametersBC(RsassaPssParameters.GetInstance(encodableBC.GetEncodable()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRsassaPssParameters CreateRSASSAPSSParamsWithMGF1(IDerObjectIdentifier digestAlgoOid, int saltLen,
            int trailerField) {
            DerObjectIdentifier oid = ((DerObjectIdentifierBC) digestAlgoOid).GetDerObjectIdentifier();
            AlgorithmIdentifier digestAlgo = new AlgorithmIdentifier(oid);
            AlgorithmIdentifier mgf = new AlgorithmIdentifier(Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.IdMgf1, digestAlgo);
            var @params = new RsassaPssParameters(digestAlgo, mgf, new DerInteger(saltLen),
                new DerInteger(trailerField));
            return new RsassaPssParametersBC(@params);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetProviderName() {
            return PROVIDER_NAME;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertID CreateCertificateID() {
            return CertIDBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public IX509Extensions CreateExtensions(IDictionary objectIdentifier) {
            IDictionary<DerObjectIdentifier, X509Extension> dictionary = new Dictionary<DerObjectIdentifier, X509Extension>();
            foreach (IDerObjectIdentifier key in objectIdentifier.Keys) {
                dictionary.Add(((DerObjectIdentifierBC)key).GetDerObjectIdentifier(), 
                    ((X509ExtensionBC)objectIdentifier[key]).GetX509Extension());
            }
            return new X509ExtensionsBC(new X509Extensions(dictionary));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509Extensions CreateExtensions() {
            return X509ExtensionsBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspReqGenerator CreateOCSPReqBuilder() {
            return new OCSPReqBuilderBC(new OcspReqGenerator());
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISigPolicyQualifierInfo CreateSigPolicyQualifierInfo(IDerObjectIdentifier objectIdentifier
            , IDerIA5String @string) {
            return new SigPolicyQualifierInfoBC(objectIdentifier, @string);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerStringBase CreateASN1String(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerStringBase) {
                return new DerStringBaseBC((DerStringBase)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Object CreateASN1Primitive(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is Asn1Object) {
                return new Asn1ObjectBC((Asn1Object)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public IOcspResponse CreateOCSPResponse(byte[] bytes) {
            return new OcspResponseBC(OcspResponse.GetInstance(new Asn1InputStream(bytes).ReadObject()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspResponse CreateOCSPResponse() {
            return OcspResponseBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspResponse CreateOCSPResponse(IOcspResponseStatus respStatus, IResponseBytes responseBytes) {
            return new OcspResponseBC(respStatus, responseBytes);
        }

        /// <summary><inheritDoc/></summary>
        public IOcspResponse CreateOCSPResponse(int respStatus, Object response) {
            if (response == null) {
                return new OcspResponseBC(new OcspResponse(new OcspResponseStatus(respStatus), null));
            }
            BasicOcspResponse basicResp = null;
            if (response is IBasicOcspResponse) {
                basicResp = ((BasicOcspResponseBC)response).GetBasicOcspResponse();
                if (basicResp == null) {
                    return new OcspResponseBC(new OcspResponse(new OcspResponseStatus(respStatus), null));
                }
            }
            if (response is BasicOcspResponse) {
                basicResp = (BasicOcspResponse)response;
            }
            if (basicResp == null) {
                throw new OcspExceptionBC(new OcspException("unknown response object"));
            }
            Asn1OctetString octs;
            try {
                octs = new DerOctetString(((BasicOcspResponseBC)response).GetEncoded());
            } catch (Exception e) {
                throw new OcspExceptionBC(new OcspException("can't encode object.", e));
            }
            ResponseBytes rb = new ResponseBytes(OcspObjectIdentifiers.PkixOcspBasic, octs);
            return new OcspResponseBC(new OcspResponse(new OcspResponseStatus(respStatus), rb));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IResponseBytes CreateResponseBytes(IDerObjectIdentifier asn1ObjectIdentifier, IDerOctetString
             derOctetString) {
            return new ResponseBytesBC(asn1ObjectIdentifier, derOctetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspResponseStatus CreateOCSPResponseStatus(int status) {
            return new OcspResponseStatusBC(new OcspResponseStatus(status));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspResponseStatus CreateOCSPResponseStatus() {
            return OcspResponseStatusBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertStatus CreateCertificateStatus() {
            return CertStatusBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public IRevokedCertStatus CreateRevokedStatus(ICertStatus certificateStatus) {
            CertStatus certStatus = ((CertStatusBC) certificateStatus).GetCertStatus();
            if (certStatus != null && certStatus.TagNo == 1) {
                return new RevokedStatusBC(certStatus);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRevokedCertStatus CreateRevokedStatus(DateTime date, int i) {
            return new RevokedStatusBC(date, i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Object CreateASN1Primitive(byte[] array) {
            return new Asn1ObjectBC(array);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerIA5String CreateDERIA5String(IAsn1TaggedObject taggedObject, bool b) {
            return new DerIA5StringBC(((DerIA5String)DerIA5String.GetInstance(((Asn1TaggedObjectBC)taggedObject).GetAsn1TaggedObject
                (), b)));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerIA5String CreateDERIA5String(String str) {
            return new DerIA5StringBC(str);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICrlDistPoint CreateCRLDistPoint(Object @object) {
            return new CrlDistPointBC(CrlDistPoint.GetInstance(@object is Asn1EncodableBC ? ((Asn1EncodableBC)@object)
                .GetEncodable() : @object));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDistributionPointName CreateDistributionPointName() {
            return DistributionPointNameBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IGeneralNames CreateGeneralNames(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is GeneralNames) {
                return new GeneralNamesBC((GeneralNames)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IGeneralName CreateGeneralName() {
            return GeneralNameBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOtherHashAlgAndValue CreateOtherHashAlgAndValue(IAlgorithmIdentifier algorithmIdentifier, 
            IAsn1OctetString octetString) {
            return new OtherHashAlgAndValueBC(algorithmIdentifier, octetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignaturePolicyId CreateSignaturePolicyId(IDerObjectIdentifier objectIdentifier, IOtherHashAlgAndValue
             algAndValue) {
            return new SignaturePolicyIdBC(objectIdentifier, algAndValue);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignaturePolicyId CreateSignaturePolicyId(IDerObjectIdentifier objectIdentifier, IOtherHashAlgAndValue
             algAndValue, params ISigPolicyQualifierInfo[] policyQualifiers) {
            SigPolicyQualifierInfo[] qualifierInfos = new SigPolicyQualifierInfo[policyQualifiers.Length];
            for (int i = 0; i < qualifierInfos.Length; ++i) {
                qualifierInfos[i] = ((SigPolicyQualifierInfoBC)policyQualifiers[i]).GetSigPolicyQualifierInfo();
            }
            return new SignaturePolicyIdBC(objectIdentifier, algAndValue, qualifierInfos);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignaturePolicyIdentifier CreateSignaturePolicyIdentifier(ISignaturePolicyId policyId) {
            return new SignaturePolicyIdentifierBC(policyId);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IEnvelopedData CreateEnvelopedData(IOriginatorInfo originatorInfo, IAsn1Set set, IEncryptedContentInfo
             encryptedContentInfo, IAsn1Set set1) {
            return new EnvelopedDataBC(originatorInfo, set, encryptedContentInfo, set1);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientInfo CreateRecipientInfo(IKeyTransRecipientInfo keyTransRecipientInfo) {
            return new RecipientInfoBC(keyTransRecipientInfo);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IEncryptedContentInfo CreateEncryptedContentInfo(IDerObjectIdentifier data, IAlgorithmIdentifier
             algorithmIdentifier, IAsn1OctetString octetString) {
            return new EncryptedContentInfoBC(data, algorithmIdentifier, octetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITbsCertificateStructure CreateTBSCertificate(IAsn1Encodable encodable) {
            return new TbsCertificateStructureBC(TbsCertificateStructure.GetInstance(((Asn1EncodableBC)encodable).GetEncodable(
                )));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IIssuerAndSerialNumber CreateIssuerAndSerialNumber(IX500Name issuer, IBigInteger value) {
            return new IssuerAndSerialNumberBC(issuer, ((BigIntegerBC)value).GetBigInteger());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientIdentifier CreateRecipientIdentifier(IIssuerAndSerialNumber issuerAndSerialNumber
            ) {
            return new RecipientIdentifierBC(issuerAndSerialNumber);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IKeyTransRecipientInfo CreateKeyTransRecipientInfo(IRecipientIdentifier recipientIdentifier
            , IAlgorithmIdentifier algorithmIdentifier, IAsn1OctetString octetString) {
            return new KeyTransRecipientInfoBC(recipientIdentifier, algorithmIdentifier, octetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOriginatorInfo CreateNullOriginatorInfo() {
            return new OriginatorInfoBC(null);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICmsEnvelopedData CreateCMSEnvelopedData(byte[] bytes) {
            try {
                return new CmsEnvelopedDataBC(new CmsEnvelopedData(bytes));
            }
            catch (CmsException e) {
                throw new CmsExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampRequestGenerator CreateTimeStampRequestGenerator() {
            return new TimeStampRequestGeneratorBC(new TimeStampRequestGenerator());
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampResponse CreateTimeStampResponse(byte[] respBytes) {
            try {
                return new TimeStampResponseBC(new TimeStampResponse(respBytes));
            } catch (TspException e) {
                throw new TSPExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual AbstractOcspException CreateAbstractOCSPException(Exception e) {
            return new OcspExceptionBC(new OcspException(e.Message));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IUnknownCertStatus CreateUnknownStatus() {
            return new UnknownStatusBC();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Dump CreateASN1Dump() {
            return Asn1DumpBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerBitString CreateASN1BitString(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerBitString) {
                return new DerBitStringBC((DerBitString)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerGeneralizedTime CreateASN1GeneralizedTime(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerGeneralizedTime) {
                return new DerGeneralizedTimeBC((DerGeneralizedTime)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerUtcTime CreateASN1UTCTime(IAsn1Encodable encodable) {
            Asn1EncodableBC encodableBC = (Asn1EncodableBC)encodable;
            if (encodableBC.GetEncodable() is DerUtcTime) {
                return new DerUtcTimeBC((DerUtcTime)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampResponseGenerator CreateTimeStampResponseGenerator(ITimeStampTokenGenerator tokenGenerator
            , IList algorithms) {
            return new TimeStampResponseGeneratorBC(tokenGenerator, algorithms);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampRequest CreateTimeStampRequest(byte[] bytes) {
            return new TimeStampRequestBC(new TimeStampRequest(bytes));
        }

        /// <summary><inheritDoc/></summary>
        public ITimeStampTokenGenerator CreateTimeStampTokenGenerator(IPrivateKey pk, IX509Certificate certificate, 
            string allowedDigest, string policyOid) {
            return new TimeStampTokenGeneratorBC(pk, certificate, allowedDigest, policyOid);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX500Name CreateX500Name(IX509Certificate certificate) {
            byte[] tbsCertificate = certificate.GetTbsCertificate();
            if (tbsCertificate.Length != 0) {
                return new X509NameBC(X509Name.GetInstance(TbsCertificateStructure.GetInstance(Asn1Object.FromByteArray(
                    certificate.GetTbsCertificate())).Subject));
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX500Name CreateX500Name(String s) {
            return new X509NameBC(new X509Name(s));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRespID CreateRespID(IX500Name x500Name) {
            return new RespIDBC(x500Name);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOcspRespGenerator CreateBasicOCSPRespBuilder(IRespID respID) {
            return new BasicOcspRespGeneratorBC(respID);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspRequest CreateOCSPReq(byte[] requestBytes) {
            return new OcspReqBC(new OcspReq(requestBytes));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509V2CrlGenerator CreateX509v2CRLBuilder(IX500Name x500Name, DateTime date) {
            return new X509V2CrlGeneratorBC(x500Name, date);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509V3CertificateGenerator CreateJcaX509v3CertificateBuilder(IX509Certificate signingCert, 
            IBigInteger number, DateTime startDate, DateTime endDate, IX500Name subjectDnName, IPublicKey publicKey) {
            return new X509V3CertificateGeneratorBC(signingCert, number, startDate, endDate, subjectDnName, publicKey);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicConstraints CreateBasicConstraints(bool b) {
            return new BasicConstraintsBC(new BasicConstraints(b));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IKeyUsage CreateKeyUsage() {
            return KeyUsageBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IKeyUsage CreateKeyUsage(int i) {
            return new KeyUsageBC(new KeyUsage(i));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IKeyPurposeID CreateKeyPurposeId() {
            return KeyPurposeIDBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IExtendedKeyUsage CreateExtendedKeyUsage(IKeyPurposeID purposeId) {
            return new ExtendedKeyUsageBC(purposeId);
        }
        
        /// <summary><inheritDoc/></summary>
        public virtual ISubjectPublicKeyInfo CreateSubjectPublicKeyInfo(IPublicKey publicKey) {
            return new SubjectPublicKeyInfoBC(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(
                ((PublicKeyBC)publicKey).GetPublicKey()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICrlReason CreateCRLReason() {
            return CrlReasonBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITstInfo CreateTSTInfo(IContentInfo contentInfoTsp) {
            CmsProcessable content = new CmsSignedData(((ContentInfoBC) contentInfoTsp).GetContentInfo()).SignedContent;
            MemoryStream bOut = new MemoryStream();
            content.Write(bOut);
            return new TstInfoBC(TstInfo.GetInstance(Asn1Object.FromByteArray(bOut.ToArray())));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISingleResponse CreateSingleResp(IBasicOcspResponse basicResp) {
            return new SingleResponseBC(basicResp);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509Certificate CreateX509Certificate(object obj) {
            switch (obj) {
                case IX509Certificate _:
                    return (X509CertificateBC) obj;
                case X509Certificate certificate:
                    return new X509CertificateBC(certificate);
                case byte[] encoded:
                    return new X509CertificateBC(new X509CertificateParser().ReadCertificate(encoded));
                default:
                    return null;
            }
        }
        
        /// <summary><inheritDoc/></summary>
        public virtual IX509Certificate CreateX509Certificate(Stream s) {
            return new X509CertificateBC(new X509CertificateParser().ReadCertificate(s));
        }
        
        /// <summary><inheritDoc/></summary>
        public IX509Crl CreateX509Crl(Stream input) {
            return new X509CrlBC(new X509CrlParser().ReadCrl(input));
        }

        /// <summary><inheritDoc/></summary>
        public IDigest CreateIDigest(string hashAlgorithm) {
            return new DigestBC(DigestUtilities.GetDigest(hashAlgorithm));
        }

        /// <summary><inheritDoc/></summary>
        public ICertID CreateCertificateID(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber) {
            return new CertIDBC(hashAlgorithm, issuerCert, serialNumber);
        }
        
        /// <summary><inheritDoc/></summary>
        public IX500Name CreateX500NameInstance(IAsn1Encodable issuer) {
            return new X509NameBC(X509Name.GetInstance(((Asn1EncodableBC)issuer).GetEncodable()));
        }

        /// <summary><inheritDoc/></summary>
        public IOcspRequest CreateOCSPReq(ICertID certId, byte[] documentId) {
            return new OcspReqBC(certId, documentId);
        }
        
        /// <summary><inheritDoc/></summary>
        public ISigner CreateISigner() {
            return new SignerBC(null);
        }
        
        /// <summary><inheritDoc/></summary>
        public IX509CertificateParser CreateX509CertificateParser() {
            return new X509CertificateParserBC(new X509CertificateParser());
        }

        /// <summary><inheritDoc/></summary>
        public AbstractGeneralSecurityException CreateGeneralSecurityException(string exceptionMessage,
            Exception exception) {
            return new GeneralSecurityExceptionBC(exceptionMessage, exception);
        }
        
        /// <summary><inheritDoc/></summary>
        public AbstractGeneralSecurityException CreateGeneralSecurityException(string exceptionMessage) {
            return new GeneralSecurityExceptionBC(new GeneralSecurityException(exceptionMessage));
        }
        
        /// <summary><inheritDoc/></summary>
        public AbstractGeneralSecurityException CreateGeneralSecurityException() {
            return new GeneralSecurityExceptionBC(new GeneralSecurityException());
        }

        /// <summary><inheritDoc/></summary>
    	public IBouncyCastleTestConstantsFactory GetBouncyCastleFactoryTestUtil() {
        	return BOUNCY_CASTLE_TEST_CONSTANTS;
    	}

        /// <summary><inheritDoc/></summary>
        public IBigInteger CreateBigInteger() {
            return BigIntegerBC.GetInstance();
        }
        
        /// <summary><inheritDoc/></summary>
        public IBigInteger CreateBigInteger(int i, byte[] array) {
            return new BigIntegerBC(new BigInteger(i, array));
        }

        /// <summary><inheritDoc/></summary>
        public IBigInteger CreateBigInteger(string str) {
            return new BigIntegerBC(new BigInteger(str));
        }

        /// <summary><inheritDoc/></summary>
        public ICipher CreateCipher(bool forEncryption, byte[] key, byte[] iv) {
            return new CipherBC(forEncryption, key, iv);
        }

        /// <summary><inheritDoc/></summary>
        public ICipherCBCnoPad CreateCipherCbCnoPad(bool forEncryption, byte[] key, byte[] iv) {
            return new CipherCBCnoPadBC(forEncryption, key, iv);
        }
        
        /// <summary><inheritDoc/></summary>
        public ICipherCBCnoPad CreateCipherCbCnoPad(bool forEncryption, byte[] key) {
            return new CipherCBCnoPadBC(forEncryption, key);
        }
        
        /// <summary><inheritDoc/></summary>
        public IX509Crl CreateNullCrl() {
            return new X509CrlBC(null);
        }

        /// <summary><inheritDoc/></summary>
        public ITimeStampToken CreateTimeStampToken(IContentInfo contentInfo) {
            return new TimeStampTokenBC(new TimeStampToken(((ContentInfoBC)contentInfo).GetContentInfo()));
        }

        /// <summary><inheritDoc/></summary>
        public IRsaKeyPairGenerator CreateRsa2048KeyPairGenerator() {
            return new RsaKeyPairGeneratorBC();
        }

        /// <summary><inheritDoc/></summary>
        public IContentSigner CreateContentSigner(string signatureAlgorithm, IPrivateKey signingKey) {
            return new ContentSignerBC(new Asn1SignatureFactory(signatureAlgorithm, 
                (AsymmetricKeyParameter)((PrivateKeyBC) signingKey).GetPrivateKey()));
        }

        /// <summary><inheritDoc/></summary>
        public IAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(ISubjectPublicKeyInfo issuerPublicKeyInfo) {
            return new AuthorityKeyIdentifierBC(new AuthorityKeyIdentifier(
                ((SubjectPublicKeyInfoBC)issuerPublicKeyInfo).GetSubjectPublicKeyInfo()));
        }

        /// <summary><inheritDoc/></summary>
        public ISubjectKeyIdentifier CreateSubjectKeyIdentifier(ISubjectPublicKeyInfo subjectPublicKeyInfo) {
            return new SubjectKeyIdentifierBC(new SubjectKeyIdentifier(
                ((SubjectPublicKeyInfoBC)subjectPublicKeyInfo).GetSubjectPublicKeyInfo()));
        }

        /// <summary><inheritDoc/></summary>
        public bool IsNullExtension(IX509Extension ext) {
            return ((X509ExtensionBC)ext).GetX509Extension() == null;
        }
        
        /// <summary><inheritDoc/></summary>
        public IX509Extension CreateExtension(bool b, IDerOctetString octetString) {
            return new X509ExtensionBC(new X509Extension(b, 
                ((DerOctetStringBC)octetString).GetDerOctetString()));
        }

        /// <summary><inheritDoc/></summary>
        public byte[] CreateCipherBytes(IX509Certificate x509Certificate, byte[] abyte0, IAlgorithmIdentifier algorithmidentifier) {
            IBufferedCipher cipher = CipherUtilities.GetCipher(((AlgorithmIdentifierBC) algorithmidentifier).GetAlgorithmIdentifier().Algorithm);
            cipher.Init(true, ((PublicKeyBC)x509Certificate.GetPublicKey()).GetPublicKey());
            byte[] outp = new byte[10000];
            int len = cipher.DoFinal(abyte0, outp, 0);
            byte[] abyte1 = new byte[len];
            Array.Copy(outp, 0, abyte1, 0, len);
            return abyte1;
        }

        /// <summary><inheritDoc/></summary>
        public bool IsInApprovedOnlyMode() {
            return false;
        }
        
        /// <inheritdoc/>
        public void  IsEncryptionFeatureSupported(int encryptionType, bool withCertificate) {
            // all features supported
        }
        
        /// <summary><inheritDoc/></summary>
        public IPemReader CreatePEMParser(TextReader reader, char[] password) {
            return new PEMParserBC(new PemReader(reader, new BouncyCastlePasswordFinder(password)));
        }

        private class BouncyCastlePasswordFinder : IPasswordFinder {
            private readonly char[] password;

            public BouncyCastlePasswordFinder(char[] password) {
                this.password = password;
            }
            
            public char[] GetPassword() {
                return password;
            }
        }
    }
}
