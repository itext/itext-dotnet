/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Security.Cryptography;
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
using iText.Bouncycastle.Crypto.Modes;
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
using iText.Commons.Bouncycastle.Crypto.Modes;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Openssl;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Bouncycastle.X509;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
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

        private static readonly IBouncyCastleUtil BOUNCY_CASTLE_UTIL = new BouncyCastleUtil();

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
        public virtual String GetDigestAlgorithmOid(String name) {
            try {
                DerObjectIdentifier algorithmIdentifier = DigestUtilities.GetObjectIdentifier(name);
                if (algorithmIdentifier != null) {
                    return algorithmIdentifier.Id;
                }
            } catch (ArgumentException) {
                // Do nothing.
            }
            return null;
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
        public virtual IDerEnumerated CreateASN1Enumerated(IAsn1Encodable i) {
            Asn1EncodableBC encodable = (Asn1EncodableBC) i;
            if (encodable.GetEncodable() is DerEnumerated) {
                return new DerEnumeratedBC((DerEnumerated) encodable.GetEncodable());
            }
            return null;
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
        public virtual IBasicOcspResponse CreateBasicOCSPResponse(byte[] bytes) {
            return new BasicOcspResponseBC(BasicOcspResponse.GetInstance(
                (Asn1Sequence)Asn1Sequence.FromByteArray(bytes)));
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
        public virtual IIssuingDistributionPoint CreateIssuingDistributionPoint(Object point) {
            return new IssuingDistributionPointBC(IssuingDistributionPoint.GetInstance(point is Asn1EncodableBC ?
                ((Asn1EncodableBC) point).GetEncodable() : point));
        }

        /// <summary><inheritDoc/></summary>
        public IIssuingDistributionPoint CreateIssuingDistributionPoint(IDistributionPointName distributionPoint,
            bool onlyContainsUserCerts, bool onlyContainsCACerts, IReasonFlags onlySomeReasons, bool indirectCRL,
            bool onlyContainsAttributeCerts) {
            return new IssuingDistributionPointBC(new IssuingDistributionPoint(distributionPoint == null ? null :
                    ((DistributionPointNameBC) distributionPoint).GetDistributionPointName(), onlyContainsUserCerts,
                onlyContainsCACerts, onlySomeReasons == null ? null :
                    ((ReasonFlagsBC) onlySomeReasons).GetReasonFlags(), indirectCRL, onlyContainsAttributeCerts));
        }

        /// <summary><inheritDoc/></summary>
        public IReasonFlags CreateReasonFlags(int reasons) {
            return new ReasonFlagsBC(new ReasonFlags(reasons));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDistributionPointName CreateDistributionPointName() {
            return DistributionPointNameBC.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public IDistributionPointName CreateDistributionPointName(IGeneralNames generalNames) {
            return new DistributionPointNameBC(new DistributionPointName(((GeneralNamesBC)generalNames).GetGeneralNames()));
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
        
        public virtual ITbsCertificateStructure CreateTBSCertificate(byte[] bytes) {
            return new TbsCertificateStructureBC(TbsCertificateStructure.GetInstance(bytes));
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
            if (encodableBC.GetEncodable() is Asn1GeneralizedTime) {
                return new IAsn1GeneralizedTimeBC((Asn1GeneralizedTime)encodableBC.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerGeneralizedTime CreateASN1GeneralizedTime(DateTime date) {
            return new IAsn1GeneralizedTimeBC(new Asn1GeneralizedTime(date));
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
            String digestOid = GetDigestAlgorithmOid(allowedDigest.ToUpperInvariant());
            return new TimeStampTokenGeneratorBC(pk, certificate, digestOid, policyOid);
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

        public IX500Name CreateX500Name(IAsn1Sequence s)
        {
            return new X509NameBC(X509Name.GetInstance(((Asn1SequenceBC) s).GetAsn1Sequence()));
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
        public virtual IBasicConstraints CreateBasicConstraints(int pathLength) {
            return new BasicConstraintsBC(new BasicConstraints(pathLength));
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
        public virtual IExtendedKeyUsage CreateExtendedKeyUsage(IDerObjectIdentifier[] purposeId) {
            DerObjectIdentifier[] unwrappedPurposeIds = new DerObjectIdentifier[purposeId.Length];
            for (int i = 0; i < purposeId.Length; ++i) {
                unwrappedPurposeIds[i] = ((DerObjectIdentifierBC)purposeId[i]).GetDerObjectIdentifier();
            }
            return new ExtendedKeyUsageBC(new ExtendedKeyUsage(unwrappedPurposeIds));
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
        public virtual ITstInfo CreateTSTInfo(IAsn1Object contentInfo) {
            return new TstInfoBC(TstInfo.GetInstance(((Asn1ObjectBC) contentInfo).GetPrimitive()));
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
            try {
                return new X509CertificateBC(new X509CertificateParser().ReadCertificate(s));
            }
            catch (GeneralSecurityException e) {
                throw new GeneralSecurityExceptionBC(e);
            }
        }
        
        /// <summary><inheritDoc/></summary>
        public IX509Crl CreateX509Crl(Stream input) {
            X509Crl crl = new X509CrlParser().ReadCrl(input);
            if (crl != null) {
                return new X509CrlBC(crl);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public ICollection<IX509Crl> CreateX509Crls(Stream input) {
            try {
                ICollection<IX509Crl> crls = new List<IX509Crl>();
                foreach (X509Crl crl in new X509CrlParser().ReadCrls(input)) {
                    crls.Add(new X509CrlBC(crl));
                }
                return crls;
            } catch (CrlException e) {
                throw new CrlExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public IDigest CreateIDigest(string hashAlgorithm) {
            try {
                return new DigestBC(DigestUtilities.GetDigest(hashAlgorithm));
            } catch (SecurityUtilityException e) {
                throw new SecurityUtilityExceptionBC(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public ICertID CreateCertificateID(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber) {
            return new CertIDBC(new AlgorithmIdentifier(new DerObjectIdentifier(hashAlgorithm), DerNull.Instance), issuerCert, serialNumber);
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
        public bool IsNull(IAsn1Encodable encodable) {
            return ((Asn1EncodableBC)encodable).GetEncodable() == null;
        }

        /// <summary><inheritDoc/></summary>
        public RNGCryptoServiceProvider GetSecureRandom() {
            return new RNGCryptoServiceProvider();
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
        
        /// <summary><inheritDoc/></summary>
        public void IsEncryptionFeatureSupported(int encryptionType, bool withCertificate) {
            // all features supported
        }
        
        /// <summary><inheritDoc/></summary>
        public IPemReader CreatePEMParser(TextReader reader, char[] password) {
            return new PEMParserBC(new PemReader(reader, new BouncyCastlePasswordFinder(password)));
        }

        /// <summary><inheritDoc/></summary>
        public IBouncyCastleUtil GetBouncyCastleUtil() {
            return BOUNCY_CASTLE_UTIL;
        }

        /// <summary><inheritDoc/></summary>
        public string CreateEndDate(IX509Certificate certificate) {
            return certificate.GetEndDateTime();
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GenerateHKDF(byte[] inputKey, byte[] salt, byte[] info) {
            HkdfBytesGenerator hkdfBytesGenerator = new HkdfBytesGenerator(new Sha256Digest());
            HkdfParameters hkdfParameters = new HkdfParameters(inputKey, salt, info);
            hkdfBytesGenerator.Init(hkdfParameters);
            byte[] hkdf = new byte[32];
            hkdfBytesGenerator.GenerateBytes(hkdf, 0, 32);

            return hkdf;
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GenerateHMACSHA256Token(byte[] key, byte[] data) {
            HMACSHA256 mac = new HMACSHA256(key);
            return mac.ComputeHash(data);
        }

        /// <summary><inheritDoc/></summary>
        public byte[] GenerateEncryptedKeyWithAES256NoPad(byte[] key, byte[] kek) {
            IWrapper wrapper = new AesWrapEngine();
            wrapper.Init(true, new KeyParameter(kek));
            return wrapper.Wrap(key, 0, key.Length);
        }
        
        /// <summary><inheritDoc/></summary>
        public byte[] GenerateDecryptedKeyWithAES256NoPad(byte[] key, byte[] kek) {
            IWrapper wrapper = new AesWrapEngine();
            wrapper.Init(false, new KeyParameter(kek));
            return wrapper.Unwrap(key, 0, key.Length);
        }

        /// <summary><inheritDoc/></summary>
        public IGCMBlockCipher CreateGCMBlockCipher() {
            GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
            return new GCMBlockCipherBC(cipher);
        }

        /// <summary><inheritDoc/></summary>
        public RSAParameters? GetRsaParametersFromCertificate(IX509Certificate certificate) {
            AsymmetricKeyParameter asymmetricKeyParameter = ((PublicKeyBC)certificate.GetPublicKey()).GetPublicKey();
            if (asymmetricKeyParameter is RsaKeyParameters) {
                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)asymmetricKeyParameter);
                return rsaParams;
            }
            return null;
        }

        //\cond DO_NOT_DOCUMENT
        internal class BouncyCastlePasswordFinder : IPasswordFinder {
            private readonly char[] password;

            public BouncyCastlePasswordFinder(char[] password) {
                this.password = password;
            }
            
            public char[] GetPassword() {
                return password;
            }
        }
        //\endcond
    }
}
