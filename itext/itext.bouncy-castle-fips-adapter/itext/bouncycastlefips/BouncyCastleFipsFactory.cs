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
using iText.Bouncycastlefips.Crypto;
using iText.Bouncycastlefips.Crypto.Generators;
using iText.Bouncycastlefips.Math;
using iText.Bouncycastlefips.Openssl;
using iText.Bouncycastlefips.Operator;
using iText.Bouncycastlefips.Pkcs;
using iText.Bouncycastlefips.Security;
using iText.Bouncycastlefips.Tsp;
using iText.Bouncycastlefips.X509;
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
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Asymmetric;
using Org.BouncyCastle.Crypto.Fips;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Operators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using ContentInfo = Org.BouncyCastle.Asn1.Cms.ContentInfo;
using ICipher = iText.Commons.Bouncycastle.Crypto.ICipher;
using SignedData = Org.BouncyCastle.Asn1.Cms.SignedData;

namespace iText.Bouncycastlefips {
    /// <summary>
    /// This class implements
    /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
    /// and creates bouncy-castle FIPS classes instances.
    /// </summary>
    public class BouncyCastleFipsFactory : IBouncyCastleFactory {
        private static readonly String PROVIDER_NAME = "BCFIPS";
        private static readonly BouncyCastleFipsTestConstantsFactory BOUNCY_CASTLE_FIPS_TEST_CONSTANTS = new BouncyCastleFipsTestConstantsFactory();
        private static readonly String FIPS_MODE_ENVIRONMENT_VARIABLE_NAME = "ITEXT_DOTNET_BOUNCY_CASTLE_FIPS_MODE";
        private static readonly String APPROVED_MODE_VALUE = "approved_mode";

        /// <summary>
        /// Creates
        /// <see cref="iText.Bouncycastlefips.BouncyCastleFipsFactory"/>.
        /// If environment variable "ITEXT_DOTNET_BOUNCY_CASTLE_FIPS_MODE" is set to "approved_mode" value approved only
        /// mode will be enabled in FIPS
        /// </summary>
        public BouncyCastleFipsFactory() {
            string fipsMode = SystemUtil.GetEnvironmentVariable(FIPS_MODE_ENVIRONMENT_VARIABLE_NAME);
            if (fipsMode != null && APPROVED_MODE_VALUE.Equals(fipsMode.ToLowerInvariant())) {
                CryptoServicesRegistrar.SetApprovedOnlyMode(true);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier CreateASN1ObjectIdentifier(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerObjectIdentifier) {
                return new ASN1ObjectIdentifierBCFips((DerObjectIdentifier)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier CreateASN1ObjectIdentifier(String str) {
            return new ASN1ObjectIdentifierBCFips(str);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509v2CRLBuilder CreateX509v2CRLBuilder(IX500Name x500Name, DateTime date) {
            return new X509v2CRLBuilderBCFips(x500Name, date);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1ObjectIdentifier CreateASN1ObjectIdentifierInstance(Object @object) {
            return new ASN1ObjectIdentifierBCFips(DerObjectIdentifier.GetInstance(@object is ASN1EncodableBCFips ? ((ASN1EncodableBCFips
                )@object).GetEncodable() : @object));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1InputStream CreateASN1InputStream(Stream stream) {
            return new ASN1InputStreamBCFips(stream);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1InputStream CreateASN1InputStream(byte[] bytes) {
            return new ASN1InputStreamBCFips(bytes);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1OctetString CreateASN1OctetString(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            if (primitiveBCFips.GetPrimitive() is Asn1OctetString) {
                return new ASN1OctetStringBCFips((Asn1OctetString)primitiveBCFips.GetPrimitive());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1OctetString CreateASN1OctetString(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1OctetString) {
                return new ASN1OctetStringBCFips((Asn1OctetString)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1OctetString CreateASN1OctetString(IASN1TaggedObject taggedObject, bool b) {
            return new ASN1OctetStringBCFips(taggedObject, b);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1OctetString CreateASN1OctetString(byte[] bytes) {
            return new ASN1OctetStringBCFips(Asn1OctetString.GetInstance(bytes));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Sequence CreateASN1Sequence(Object @object) {
            if (@object is Asn1Sequence) {
                return new ASN1SequenceBCFips((Asn1Sequence)@object);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Sequence CreateASN1Sequence(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1Sequence) {
                return new ASN1SequenceBCFips((Asn1Sequence)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Sequence CreateASN1Sequence(byte[] array) {
            return new ASN1SequenceBCFips((Asn1Sequence)Asn1Sequence.FromByteArray(array));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Sequence CreateASN1SequenceInstance(Object @object) {
            return new ASN1SequenceBCFips(@object is ASN1EncodableBCFips ? ((ASN1EncodableBCFips)@object).GetEncodable
                () : @object);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERSequence CreateDERSequence(IASN1EncodableVector encodableVector) {
            ASN1EncodableVectorBCFips vectorBCFips = (ASN1EncodableVectorBCFips)encodableVector;
            return new DERSequenceBCFips(vectorBCFips.GetEncodableVector());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERSequence CreateDERSequence(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            return new DERSequenceBCFips(primitiveBCFips.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1TaggedObject CreateASN1TaggedObject(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1TaggedObject) {
                return new ASN1TaggedObjectBCFips((Asn1TaggedObject)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Integer CreateASN1Integer(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerInteger) {
                return new ASN1IntegerBCFips((DerInteger)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Integer CreateASN1Integer(int i) {
            return new ASN1IntegerBCFips(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Integer CreateASN1Integer(IBigInteger i) {
            return new ASN1IntegerBCFips(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Set CreateASN1Set(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1Set) {
                return new ASN1SetBCFips((Asn1Set)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Set CreateASN1Set(Object encodable) {
            return encodable is Asn1Set ? new ASN1SetBCFips((Asn1Set)encodable) : null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Set CreateASN1Set(IASN1TaggedObject taggedObject, bool b) {
            ASN1TaggedObjectBCFips taggedObjectBCFips = (ASN1TaggedObjectBCFips)taggedObject;
            return new ASN1SetBCFips(taggedObjectBCFips.GetTaggedObject(), b);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Set CreateNullASN1Set() {
            return new ASN1SetBCFips(null);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1OutputStream CreateASN1OutputStream(Stream stream) {
            return new ASN1OutputStreamBCFips(stream);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1OutputStream CreateASN1OutputStream(Stream outputStream, String asn1Encoding) {
            if (Asn1Encodable.Ber.Equals(asn1Encoding)) {
                return new ASN1OutputStreamBCFips(new BerOutputStream(outputStream));
            }
            return new ASN1OutputStreamBCFips(new DerOutputStream(outputStream));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDEROctetString CreateDEROctetString(byte[] bytes) {
            return new DEROctetStringBCFips(bytes);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDEROctetString CreateDEROctetString(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerOctetString) {
                return new DEROctetStringBCFips((DerOctetString)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1EncodableVector CreateASN1EncodableVector() {
            return new ASN1EncodableVectorBCFips();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERNull CreateDERNull() {
            return DERNullBCFips.INSTANCE;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERTaggedObject CreateDERTaggedObject(int i, IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            return new DERTaggedObjectBCFips(i, primitiveBCFips.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERTaggedObject CreateDERTaggedObject(bool b, int i, IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            return new DERTaggedObjectBCFips(b, i, primitiveBCFips.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERSet CreateDERSet(IASN1EncodableVector encodableVector) {
            ASN1EncodableVectorBCFips encodableVectorBCFips = (ASN1EncodableVectorBCFips)encodableVector;
            return new DERSetBCFips(encodableVectorBCFips.GetEncodableVector());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERSet CreateDERSet(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            return new DERSetBCFips(primitiveBCFips.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERSet CreateDERSet(ISignaturePolicyIdentifier identifier) {
            SignaturePolicyIdentifierBCFips identifierBCFips = (SignaturePolicyIdentifierBCFips)identifier;
            return new DERSetBCFips(identifierBCFips.GetSignaturePolicyIdentifier());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERSet CreateDERSet(IRecipientInfo recipientInfo) {
            RecipientInfoBCFips recipientInfoBCFips = (RecipientInfoBCFips)recipientInfo;
            return new DERSetBCFips(recipientInfoBCFips.GetRecipientInfo());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Enumerated CreateASN1Enumerated(int i) {
            return new ASN1EnumeratedBCFips(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Encoding CreateASN1Encoding() {
            return ASN1EncodingBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAttributeTable CreateAttributeTable(IASN1Set unat) {
            ASN1SetBCFips asn1SetBCFips = (ASN1SetBCFips)unat;
            return new AttributeTableBCFips(asn1SetBCFips.GetASN1Set());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IPKCSObjectIdentifiers CreatePKCSObjectIdentifiers() {
            return PKCSObjectIdentifiersBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAttribute CreateAttribute(IASN1ObjectIdentifier attrType, IASN1Set attrValues) {
            ASN1ObjectIdentifierBCFips attrTypeBCFips = (ASN1ObjectIdentifierBCFips)attrType;
            ASN1SetBCFips attrValuesBCFips = (ASN1SetBCFips)attrValues;
            return new AttributeBCFips(new Org.BouncyCastle.Asn1.Cms.Attribute(attrTypeBCFips.GetASN1ObjectIdentifier(
                ), attrValuesBCFips.GetASN1Set()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IContentInfo CreateContentInfo(IASN1Sequence sequence) {
            ASN1SequenceBCFips sequenceBCFips = (ASN1SequenceBCFips)sequence;
            return new ContentInfoBCFips(ContentInfo.GetInstance(sequenceBCFips.GetASN1Sequence()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IContentInfo CreateContentInfo(IASN1ObjectIdentifier objectIdentifier, IASN1Encodable encodable
            ) {
            return new ContentInfoBCFips(objectIdentifier, encodable);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISigningCertificate CreateSigningCertificate(IASN1Sequence sequence) {
            ASN1SequenceBCFips sequenceBCFips = (ASN1SequenceBCFips)sequence;
            return new SigningCertificateBCFips(SigningCertificate.GetInstance(sequenceBCFips.GetASN1Sequence()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISigningCertificateV2 CreateSigningCertificateV2(IASN1Sequence sequence) {
            ASN1SequenceBCFips sequenceBCFips = (ASN1SequenceBCFips)sequence;
            return new SigningCertificateV2BCFips(SigningCertificateV2.GetInstance(sequenceBCFips.GetASN1Sequence()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOCSPResponse CreateBasicOCSPResponse(IASN1Primitive primitive) {
            ASN1PrimitiveBCFips primitiveBCFips = (ASN1PrimitiveBCFips)primitive;
            return new BasicOCSPResponseBCFips(BasicOcspResponse.GetInstance(primitiveBCFips.GetPrimitive()));
        }

        /// <summary><inheritDoc/></summary>
        public IBasicOCSPResponse CreateBasicOCSPResponse(object response)
        {
            if (response is BasicOcspResponse) {
                return new BasicOCSPResponseBCFips((BasicOcspResponse) response);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPObjectIdentifiers CreateOCSPObjectIdentifiers() {
            return OCSPObjectIdentifiersBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier CreateAlgorithmIdentifier(IASN1ObjectIdentifier algorithm) {
            ASN1ObjectIdentifierBCFips algorithmBCFips = (ASN1ObjectIdentifierBCFips)algorithm;
            return new AlgorithmIdentifierBCFips(new AlgorithmIdentifier(algorithmBCFips.GetASN1ObjectIdentifier(), null
                ));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier CreateAlgorithmIdentifier(IASN1ObjectIdentifier algorithm,
            IASN1Encodable parameters) {
            ASN1ObjectIdentifierBCFips algorithmBCFips = (ASN1ObjectIdentifierBCFips) algorithm;
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips) parameters;
            return new AlgorithmIdentifierBCFips(
                new AlgorithmIdentifier(algorithmBCFips.GetASN1ObjectIdentifier(), encodableBCFips.GetEncodable()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRsassaPssParameters CreateRSASSAPSSParams(IASN1Encodable encodable) {
            if (encodable == null) {
                throw new ArgumentException("Expected non-null RSASSA-PSS parameter data");
            }
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips) encodable;
            return new RsassaPssParametersBcFips(RsassaPssParameters.GetInstance(encodableBCFips.GetEncodable()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRsassaPssParameters CreateRSASSAPSSParamsWithMGF1(IASN1ObjectIdentifier digestAlgoOid, int saltLen,
            int trailerField)
        {
            DerObjectIdentifier oid = ((ASN1ObjectIdentifierBCFips)digestAlgoOid).GetASN1ObjectIdentifier();
            AlgorithmIdentifier digestAlgo = new AlgorithmIdentifier(oid);
            AlgorithmIdentifier mgf = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, digestAlgo);
            RsassaPssParameters @params = new RsassaPssParameters(digestAlgo, mgf, new DerInteger(saltLen),
                new DerInteger(trailerField));
            return new RsassaPssParametersBcFips(@params);
        }



        /// <summary><inheritDoc/></summary>
        public virtual String GetProviderName() {
            return PROVIDER_NAME;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertificateID CreateCertificateID() {
            return CertificateIDBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public IExtensions CreateExtensions(IDictionary objectIdentifier) {
            IDictionary dictionary = new Dictionary<DerObjectIdentifier, X509Extension>();
            foreach (IASN1ObjectIdentifier key in objectIdentifier.Keys) {
                dictionary.Add(((ASN1ObjectIdentifierBCFips)key).GetASN1ObjectIdentifier(), 
                    ((ExtensionBCFips)objectIdentifier[key]).GetX509Extension());
            }
            return new ExtensionsBCFips(new X509Extensions(dictionary));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IExtensions CreateExtensions() {
            return ExtensionsBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPReqBuilder CreateOCSPReqBuilder() {
            return new OCSPReqBuilderBCFips();
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISigPolicyQualifierInfo CreateSigPolicyQualifierInfo(IASN1ObjectIdentifier objectIdentifier
            , IDERIA5String @string) {
            return new SigPolicyQualifierInfoBCFips(objectIdentifier, @string);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1String CreateASN1String(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerStringBase) {
                return new ASN1StringBCFips((DerStringBase)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Primitive CreateASN1Primitive(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1Object) {
                return new ASN1PrimitiveBCFips((Asn1Object)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPResponse CreateOCSPResponse(byte[] bytes) {
            return new OCSPResponseBCFips(OcspResponse.GetInstance(new Asn1InputStream(bytes).ReadObject()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPResponse CreateOCSPResponse() {
            return OCSPResponseBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPResponse CreateOCSPResponse(IOCSPResponseStatus respStatus, IResponseBytes responseBytes) {
            return new OCSPResponseBCFips(respStatus, responseBytes);
        }

        /// <summary><inheritDoc/></summary>
        public IOCSPResponse CreateOCSPResponse(int respStatus, Object response) {
            if (response == null) {
                return new OCSPResponseBCFips(new OcspResponse(new OcspResponseStatus(respStatus), null));
            }
            BasicOcspResponse basicResp = null;
            if (response is IBasicOCSPResponse) {
                basicResp = ((BasicOCSPResponseBCFips)response).GetBasicOCSPResponse();
                if (basicResp == null) {
                    return new OCSPResponseBCFips(new OcspResponse(new OcspResponseStatus(respStatus), null));
                }
            }
            if (response is BasicOcspResponse) {
                basicResp = (BasicOcspResponse)response;
            }
            if (basicResp == null) {
                throw new OCSPExceptionBCFips(new Exception("unknown response object"));
            }
            Asn1OctetString octs;
            try {
                octs = new DerOctetString(((BasicOCSPResponseBCFips)response).GetEncoded());
            } catch (Exception e) {
                throw new OCSPExceptionBCFips(new Exception("can't encode object.", e));
            }
            ResponseBytes rb = new ResponseBytes(OcspObjectIdentifiers.PkixOcspBasic, octs);
            return new OCSPResponseBCFips(new OcspResponse(new OcspResponseStatus(respStatus), rb));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IResponseBytes CreateResponseBytes(IASN1ObjectIdentifier asn1ObjectIdentifier, IDEROctetString
             derOctetString) {
            return new ResponseBytesBCFips(asn1ObjectIdentifier, derOctetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPResponseStatus CreateOCSPResponseStatus(int status) {
            return new OCSPResponseStatusBCFips(new OcspResponseStatus(status));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPResponseStatus CreateOCSPResponseStatus() {
            return OCSPResponseStatusBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertificateStatus CreateCertificateStatus() {
            return CertificateStatusBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public IRevokedStatus CreateRevokedStatus(ICertificateStatus certificateStatus) {
            CertStatus certStatus = ((CertificateStatusBCFips) certificateStatus).GetCertificateStatus();
            if (certStatus != null && certStatus.TagNo == 1) {
                return new RevokedStatusBCFips(certStatus);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRevokedStatus CreateRevokedStatus(DateTime date, int i) {
            return new RevokedStatusBCFips(date, i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Primitive CreateASN1Primitive(byte[] array) {
            return new ASN1PrimitiveBCFips(array);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERIA5String CreateDERIA5String(IASN1TaggedObject taggedObject, bool b) {
            return new DERIA5StringBCFips(DerIA5String.GetInstance(((ASN1TaggedObjectBCFips)taggedObject).GetTaggedObject
                (), b));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDERIA5String CreateDERIA5String(String str) {
            return new DERIA5StringBCFips(str);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICRLDistPoint CreateCRLDistPoint(Object @object) {
            return new CRLDistPointBCFips(CrlDistPoint.GetInstance(@object is ASN1EncodableBCFips ? ((ASN1EncodableBCFips
                )@object).GetEncodable() : @object));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDistributionPointName CreateDistributionPointName() {
            return DistributionPointNameBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IGeneralNames CreateGeneralNames(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is GeneralNames) {
                return new GeneralNamesBCFips((GeneralNames)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IGeneralName CreateGeneralName() {
            return GeneralNameBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOtherHashAlgAndValue CreateOtherHashAlgAndValue(IAlgorithmIdentifier algorithmIdentifier, 
            IASN1OctetString octetString) {
            return new OtherHashAlgAndValueBCFips(algorithmIdentifier, octetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignaturePolicyId CreateSignaturePolicyId(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue
             algAndValue) {
            return new SignaturePolicyIdBCFips(objectIdentifier, algAndValue);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignaturePolicyId CreateSignaturePolicyId(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue
             algAndValue, params ISigPolicyQualifierInfo[] policyQualifiers) {
            SigPolicyQualifierInfo[] qualifierInfos = new SigPolicyQualifierInfo[policyQualifiers.Length];
            for (int i = 0; i < qualifierInfos.Length; ++i) {
                qualifierInfos[i] = ((SigPolicyQualifierInfoBCFips)policyQualifiers[i]).GetQualifierInfo();
            }
            return new SignaturePolicyIdBCFips(objectIdentifier, algAndValue, qualifierInfos);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignaturePolicyIdentifier CreateSignaturePolicyIdentifier(ISignaturePolicyId policyId) {
            return new SignaturePolicyIdentifierBCFips(policyId);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IEnvelopedData CreateEnvelopedData(IOriginatorInfo originatorInfo, IASN1Set set, IEncryptedContentInfo
             encryptedContentInfo, IASN1Set set1) {
            return new EnvelopedDataBCFips(originatorInfo, set, encryptedContentInfo, set1);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientInfo CreateRecipientInfo(IKeyTransRecipientInfo keyTransRecipientInfo) {
            return new RecipientInfoBCFips(keyTransRecipientInfo);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IEncryptedContentInfo CreateEncryptedContentInfo(IASN1ObjectIdentifier data, IAlgorithmIdentifier
             algorithmIdentifier, IASN1OctetString octetString) {
            return new EncryptedContentInfoBCFips(data, algorithmIdentifier, octetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITBSCertificate CreateTBSCertificate(IASN1Encodable encodable) {
            return new TBSCertificateBCFips(TbsCertificateStructure.GetInstance(((ASN1EncodableBCFips)encodable).GetEncodable
                ()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IIssuerAndSerialNumber CreateIssuerAndSerialNumber(IX500Name issuer, IBigInteger value) {
            return new IssuerAndSerialNumberBCFips(issuer, ((BigIntegerBCFips)value).GetBigInteger());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientIdentifier CreateRecipientIdentifier(IIssuerAndSerialNumber issuerAndSerialNumber
            ) {
            return new RecipientIdentifierBCFips(issuerAndSerialNumber);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IKeyTransRecipientInfo CreateKeyTransRecipientInfo(IRecipientIdentifier recipientIdentifier
            , IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString octetString) {
            return new KeyTransRecipientInfoBCFips(recipientIdentifier, algorithmIdentifier, octetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOriginatorInfo CreateNullOriginatorInfo() {
            return new OriginatorInfoBCFips(null);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICMSEnvelopedData CreateCMSEnvelopedData(byte[] bytes) {
            try {
                return new CMSEnvelopedDataBCFips(new CmsEnvelopedData(bytes));
            }
            catch (CmsException e) {
                throw new CMSExceptionBCFips(e);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampRequestGenerator CreateTimeStampRequestGenerator() {
            return new TimeStampRequestGeneratorBCFips();
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampResponse CreateTimeStampResponse(byte[] respBytes) {
            Asn1InputStream input = new Asn1InputStream(respBytes);
            return new TimeStampResponseBCFips(TimeStampResp.GetInstance(input.ReadObject()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual AbstractOCSPException CreateAbstractOCSPException(Exception e) {
            return new OCSPExceptionBCFips(e);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IUnknownStatus CreateUnknownStatus() {
            return new UnknownStatusBCFips();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1Dump CreateASN1Dump() {
            return ASN1DumpBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1BitString CreateASN1BitString(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerBitString) {
                return new ASN1BitStringBCFips((DerBitString)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1GeneralizedTime CreateASN1GeneralizedTime(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerGeneralizedTime) {
                return new ASN1GeneralizedTimeBCFips((DerGeneralizedTime)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IASN1UTCTime CreateASN1UTCTime(IASN1Encodable encodable) {
            ASN1EncodableBCFips encodableBCFips = (ASN1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerUtcTime) {
                return new ASN1UTCTimeBCFips((DerUtcTime)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampResponseGenerator CreateTimeStampResponseGenerator(ITimeStampTokenGenerator tokenGenerator
            , IList algorithms) {
            return new TimeStampResponseGeneratorBCFips((TimeStampTokenGeneratorBCFips)tokenGenerator, algorithms);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITimeStampRequest CreateTimeStampRequest(byte[] bytes) {
            return new TimeStampRequestBCFips(TimeStampReq.GetInstance(new Asn1InputStream(bytes).ReadObject()));
        }

        /// <summary><inheritDoc/></summary>
        public ITimeStampTokenGenerator CreateTimeStampTokenGenerator(IPrivateKey pk, IX509Certificate certificate, 
            string allowedDigest, string policyOid) {
            return new TimeStampTokenGeneratorBCFips(pk, certificate, allowedDigest, policyOid);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX500Name CreateX500Name(IX509Certificate certificate) {
            byte[] tbsCertificate = certificate.GetTbsCertificate();
            if (tbsCertificate.Length != 0) {
                return new X500NameBCFips(X500Name.GetInstance(TbsCertificateStructure.GetInstance(Asn1Object.FromByteArray
                    (certificate.GetTbsCertificate())).Subject));
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX500Name CreateX500Name(String s) {
            return new X500NameBCFips(new X500Name(s));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRespID CreateRespID(IX500Name x500Name) {
            return new RespIDBCFips(x500Name);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOCSPRespBuilder CreateBasicOCSPRespBuilder(IRespID respID) {
            return new BasicOCSPRespBuilderBCFips(respID);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOCSPReq CreateOCSPReq(byte[] requestBytes) {
            return new OCSPReqBCFips(OcspRequest.GetInstance(new Asn1InputStream(requestBytes).ReadObject()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IJcaX509v3CertificateBuilder CreateJcaX509v3CertificateBuilder(IX509Certificate signingCert, 
            IBigInteger number, DateTime startDate, DateTime endDate, IX500Name subjectDn, IPublicKey publicKey) {
            return new JcaX509v3CertificateBuilderBCFips(signingCert, number, startDate, endDate, subjectDn, publicKey);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicConstraints CreateBasicConstraints(bool b) {
            return new BasicConstraintsBCFips(new BasicConstraints(b));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IKeyUsage CreateKeyUsage() {
            return KeyUsageBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IKeyUsage CreateKeyUsage(int i) {
            return new KeyUsageBCFips(new KeyUsage(i));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IKeyPurposeId CreateKeyPurposeId() {
            return KeyPurposeIdBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IExtendedKeyUsage CreateExtendedKeyUsage(IKeyPurposeId purposeId) {
            return new ExtendedKeyUsageBCFips(purposeId);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISubjectPublicKeyInfo CreateSubjectPublicKeyInfo(IPublicKey publicKey) {
            return new SubjectPublicKeyInfoBCFips(new SubjectPublicKeyInfo(new AlgorithmIdentifier(
                    PkcsObjectIdentifiers.RsaEncryption, DerNull.Instance), 
                ((PublicKeyBCFips)publicKey).GetPublicKey().GetEncoded()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICRLReason CreateCRLReason() {
            return CRLReasonBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public ITSTInfo CreateTSTInfo(IContentInfo contentInfoTsp) {
            ICmsTypedData content = new CmsSignedData(((ContentInfoBCFips) contentInfoTsp).GetContentInfo()).SignedContent;
            MemoryStream bOut = new MemoryStream();
            content.Write(bOut);
            return new TSTInfoBCFips(TstInfo.GetInstance(Asn1Object.FromByteArray(bOut.ToArray())));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISingleResp CreateSingleResp(IBasicOCSPResponse basicResp) {
            return new SingleRespBCFips(basicResp);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509Certificate CreateX509Certificate(object obj) {
            switch (obj) {
                case IX509Certificate _:
                    return (X509CertificateBCFips) obj;
                case X509Certificate certificate:
                    return new X509CertificateBCFips(certificate);
                case byte[] encoded:
                    return new X509CertificateBCFips(new X509Certificate(X509CertificateStructure.GetInstance(encoded)));
                default:
                    return null;
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509Certificate CreateX509Certificate(Stream s) {
            PushbackStream pushbackStream = new PushbackStream(s);
            int tag = pushbackStream.ReadByte();
            if (tag < 0) {
                return null;
            }
            pushbackStream.Unread(tag);
            if (tag != 0x30) {
                // assume ascii PEM encoded.
                return ReadPemCertificate(pushbackStream);
            }
            return ReadDerCertificate(pushbackStream);
        }

        /// <summary><inheritDoc/></summary>
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

        /// <summary><inheritDoc/></summary>
        public IIDigest CreateIDigest(string hashAlgorithm) {
            return new IDigestBCFips(hashAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public ICertificateID CreateCertificateID(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber) {
            return new CertificateIDBCFips(hashAlgorithm, issuerCert, serialNumber);
        }

        /// <summary><inheritDoc/></summary>
        public IX500Name CreateX500NameInstance(IASN1Encodable issuer) {
            return new X500NameBCFips(X500Name.GetInstance(
                ((ASN1EncodableBCFips) issuer).GetEncodable()));
        }

        /// <summary><inheritDoc/></summary>
        public IOCSPReq CreateOCSPReq(ICertificateID certId, byte[] documentId) {
            return new OCSPReqBCFips(certId, documentId);
        }

        /// <summary><inheritDoc/></summary>
        public IISigner CreateISigner() {
            return new ISignerBCFips(null);
        }

        /// <summary><inheritDoc/></summary>
        public IX509CertificateParser CreateX509CertificateParser() {
            return new X509CertificateParserBCFips();
        }

        /// <summary><inheritDoc/></summary>
        public AbstractGeneralSecurityException CreateGeneralSecurityException(string exceptionMessage,
            Exception exception) {
            return new GeneralSecurityExceptionBCFips(exceptionMessage, exception);
        }

        /// <summary><inheritDoc/></summary>
        public AbstractGeneralSecurityException CreateGeneralSecurityException(string exceptionMessage) {
            return new GeneralSecurityExceptionBCFips(new GeneralSecurityException(exceptionMessage));
        }

        /// <summary><inheritDoc/></summary>
        public AbstractGeneralSecurityException CreateGeneralSecurityException() {
            return new GeneralSecurityExceptionBCFips(new GeneralSecurityException());
        }

        /// <summary><inheritDoc/></summary>
        public IBouncyCastleTestConstantsFactory GetBouncyCastleFactoryTestUtil() {
            return BOUNCY_CASTLE_FIPS_TEST_CONSTANTS;
        }

        /// <summary><inheritDoc/></summary>
        public IBigInteger CreateBigInteger() {
            return BigIntegerBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public IBigInteger CreateBigInteger(int i, byte[] array) {
            return new BigIntegerBCFips(new BigInteger(i, array));
        }

        /// <summary><inheritDoc/></summary>
        public IBigInteger CreateBigInteger(string str) {
            return new BigIntegerBCFips(new BigInteger(str));
        }

        /// <summary><inheritDoc/></summary>
        public ICipher CreateCipher(bool forEncryption, byte[] key, byte[] iv) {
            return new CipherBCFips(forEncryption, key, iv);
        }

        /// <summary><inheritDoc/></summary>
        public ICipherCBCnoPad CreateCipherCbCnoPad(bool forEncryption, byte[] key, byte[] iv) {
            return new CipherCBCnoPadBCFips(forEncryption, key, iv);
        }

        /// <summary><inheritDoc/></summary>
        public ICipherCBCnoPad CreateCipherCbCnoPad(bool forEncryption, byte[] key) {
            return new CipherCBCnoPadBCFips(forEncryption, key);
        }

        /// <summary><inheritDoc/></summary>
        public IX509Crl CreateNullCrl() {
            return new X509CrlBCFips(null);
        }

        /// <summary><inheritDoc/></summary>
        public ITimeStampToken CreateTimeStampToken(IContentInfo contentInfo) {
            return new TimeStampTokenBCFips(((ContentInfoBCFips)contentInfo).GetContentInfo());
        }

        /// <summary><inheritDoc/></summary>
        public IRsaKeyPairGenerator CreateRsa2048KeyPairGenerator() {
            return new RsaKeyPairGeneratorBCFips();
        }

        /// <summary><inheritDoc/></summary>
        public IPEMParser CreatePEMParser(TextReader reader, char[] password) {
            return new PEMParserBCFips(new OpenSslPemReader(reader), password);
        }

        /// <summary><inheritDoc/></summary>
        public IContentSigner CreateContentSigner(string signatureAlgorithm, IPrivateKey signingKey) {
            ISignatureFactory<AlgorithmIdentifier> factory = null;
            ISignatureFactoryService signatureFactoryProvider =
                CryptoServicesRegistrar.CreateService((ICryptoServiceType<ISignatureFactoryService>)
                    ((PrivateKeyBCFips) signingKey).GetPrivateKey(), new SecureRandom());
            switch (signatureAlgorithm.ToUpper()) {
                case "SHA256WITHRSA":
                    factory = new PkixSignatureFactory(signatureFactoryProvider.
                        CreateSignatureFactory(FipsRsa.Pkcs1v15.WithDigest(FipsShs.Sha256)));
                    break;
                case "SHA1WITHRSA":
                    factory = new PkixSignatureFactory(signatureFactoryProvider.
                        CreateSignatureFactory(FipsRsa.Pkcs1v15.WithDigest(FipsShs.Sha1)));
                    break;
                case "SHA1WITHDSA":
                    factory = new PkixSignatureFactory(signatureFactoryProvider.
                        CreateSignatureFactory(FipsDsa.Dsa.WithDigest(FipsShs.Sha1)));
                    break;
                case "SHA1WITHECDSA":
                    factory = new PkixSignatureFactory(signatureFactoryProvider.
                        CreateSignatureFactory(FipsEC.Dsa.WithDigest(FipsShs.Sha1)));
                    break;
            }
            return new ContentSignerBCFips(factory);
        }

        /// <summary><inheritDoc/></summary>
        public IAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(ISubjectPublicKeyInfo issuerPublicKeyInfo) {
            return new AuthorityKeyIdentifierBCFips(new AuthorityKeyIdentifier(
                ((SubjectPublicKeyInfoBCFips)issuerPublicKeyInfo).GetSubjectPublicKeyInfo()));
        }

        /// <summary><inheritDoc/></summary>
        public ISubjectKeyIdentifier CreateSubjectKeyIdentifier(ISubjectPublicKeyInfo subjectPublicKeyInfo) {
            return new SubjectKeyIdentifierBCFips(new SubjectKeyIdentifier(
                ((SubjectPublicKeyInfoBCFips)subjectPublicKeyInfo).GetSubjectPublicKeyInfo()));
        }

        /// <summary><inheritDoc/></summary>
        public bool IsNullExtension(IExtension ext) {
            return ((ExtensionBCFips)ext).GetX509Extension() == null;
        }

        /// <summary><inheritDoc/></summary>
        public IExtension CreateExtension(bool b, IDEROctetString octetString) {
            return new ExtensionBCFips(new X509Extension(b, 
                ((DEROctetStringBCFips)octetString).GetDEROctetString()));
        }

        /// <summary><inheritDoc/></summary>
        public byte[] CreateCipherBytes(IX509Certificate x509Certificate, byte[] abyte0, IAlgorithmIdentifier algorithmidentifier) {
            IKeyWrapper<FipsRsa.OaepWrapParameters> keyWrapper =
                CryptoServicesRegistrar.CreateService((AsymmetricRsaPublicKey)((PublicKeyBCFips)x509Certificate.GetPublicKey()).GetPublicKey(), new SecureRandom())
                    .CreateKeyWrapper(FipsRsa.WrapOaep.WithDigest(FipsShs.Sha1));

            return keyWrapper.Wrap(abyte0).Collect();
        }

        /// <summary><inheritDoc/></summary>
        public bool IsInApprovedOnlyMode() {
            return CryptoServicesRegistrar.IsInApprovedOnlyMode();
        }

        /// <summary><inheritDoc/></summary>
        public SecureRandom GetSecureRandom() {
            byte[] personalizationString = Strings.ToUtf8ByteArray("some personalization string");
            SecureRandom entropySource = new SecureRandom();
            return CryptoServicesRegistrar.CreateService(FipsDrbg.Sha512)
                .FromEntropySource(entropySource,true)
                .SetPersonalizationString(personalizationString).Build(
                    entropySource.GenerateSeed(256 / (2 * 8)), true, 
                    Strings.ToByteArray("number only used once"));
        }
        
        private IX509Certificate ReadPemCertificate(PushbackStream pushbackStream) {
            using (TextReader file = new StreamReader(pushbackStream)) {
                PEMParserBCFips parser = new PEMParserBCFips(new OpenSslPemReader(file), null);
                Object readObject = parser.ReadObject();
                if (readObject is IX509Certificate) {
                    return (IX509Certificate)readObject;
                }
            }
            return new X509CertificateBCFips(null); 
        }

        private IX509Certificate ReadDerCertificate(PushbackStream pushbackStream) {
            Asn1Sequence seq = (Asn1Sequence) new Asn1InputStream(pushbackStream).ReadObject();
            if (seq.Count > 1 && seq[0] is DerObjectIdentifier) {
                if (seq[0].Equals(PkcsObjectIdentifiers.SignedData)) {
                    Asn1Set sData = SignedData.GetInstance(
                        Asn1Sequence.GetInstance((Asn1TaggedObject) seq[1], true)).Certificates;
                    object obj = sData[0];
                    if (obj is Asn1Sequence) {
                        return new X509CertificateBCFips(new X509Certificate(
                            X509CertificateStructure.GetInstance(obj)));
                    }
                }
            }
            return new X509CertificateBCFips(new X509Certificate(X509CertificateStructure.GetInstance(seq)));
        }
    }
}
