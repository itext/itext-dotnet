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
using iText.Bouncycastlefips.Cert.Ocsp;
using iText.Bouncycastlefips.Cms;
using iText.Bouncycastlefips.Crypto;
using iText.Bouncycastlefips.Crypto.Generators;
using iText.Bouncycastlefips.Math;
using iText.Bouncycastlefips.Openssl;
using iText.Bouncycastlefips.Operator;
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
        public virtual IDerObjectIdentifier CreateASN1ObjectIdentifier(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerObjectIdentifier) {
                return new DerObjectIdentifierBCFips((DerObjectIdentifier)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier CreateASN1ObjectIdentifier(String str) {
            return new DerObjectIdentifierBCFips(str);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509V2CrlGenerator CreateX509v2CRLBuilder(IX500Name x500Name, DateTime date) {
            return new X509V2CrlGeneratorBCFips(x500Name, date);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerObjectIdentifier CreateASN1ObjectIdentifierInstance(Object @object) {
            return new DerObjectIdentifierBCFips(DerObjectIdentifier.GetInstance(@object is Asn1EncodableBCFips ? ((Asn1EncodableBCFips
                )@object).GetEncodable() : @object));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1InputStream CreateASN1InputStream(Stream stream) {
            return new Asn1InputStreamBCFips(stream);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1InputStream CreateASN1InputStream(byte[] bytes) {
            return new Asn1InputStreamBCFips(bytes);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1OctetString CreateASN1OctetString(IAsn1Object primitive) {
            Asn1ObjectBCFips primitiveBCFips = (Asn1ObjectBCFips)primitive;
            if (primitiveBCFips.GetPrimitive() is Asn1OctetString) {
                return new Asn1OctetStringBCFips((Asn1OctetString)primitiveBCFips.GetPrimitive());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1OctetString CreateASN1OctetString(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1OctetString) {
                return new Asn1OctetStringBCFips((Asn1OctetString)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1OctetString CreateASN1OctetString(IAsn1TaggedObject taggedObject, bool b) {
            return new Asn1OctetStringBCFips(taggedObject, b);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1OctetString CreateASN1OctetString(byte[] bytes) {
            return new Asn1OctetStringBCFips(Asn1OctetString.GetInstance(bytes));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Sequence CreateASN1Sequence(Object @object) {
            if (@object is Asn1Sequence) {
                return new Asn1SequenceBCFips((Asn1Sequence)@object);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Sequence CreateASN1Sequence(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1Sequence) {
                return new Asn1SequenceBCFips((Asn1Sequence)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Sequence CreateASN1Sequence(byte[] array) {
            return new Asn1SequenceBCFips((Asn1Sequence)Asn1Sequence.FromByteArray(array));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Sequence CreateASN1SequenceInstance(Object @object) {
            return new Asn1SequenceBCFips(@object is Asn1EncodableBCFips ? ((Asn1EncodableBCFips)@object).GetEncodable
                () : @object);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSequence CreateDERSequence(IAsn1EncodableVector encodableVector) {
            Asn1EncodableVectorBCFips vectorBCFips = (Asn1EncodableVectorBCFips)encodableVector;
            return new DerSequenceBCFips(vectorBCFips.GetEncodableVector());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSequence CreateDERSequence(IAsn1Object primitive) {
            Asn1ObjectBCFips primitiveBCFips = (Asn1ObjectBCFips)primitive;
            return new DerSequenceBCFips(primitiveBCFips.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1TaggedObject CreateASN1TaggedObject(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1TaggedObject) {
                return new Asn1TaggedObjectBCFips((Asn1TaggedObject)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerInteger CreateASN1Integer(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerInteger) {
                return new DerIntegerBCFips((DerInteger)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerInteger CreateASN1Integer(int i) {
            return new DerIntegerBCFips(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerInteger CreateASN1Integer(IBigInteger i) {
            return new DerIntegerBCFips(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Set CreateASN1Set(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1Set) {
                return new Asn1SetBCFips((Asn1Set)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Set CreateASN1Set(Object encodable) {
            return encodable is Asn1Set ? new Asn1SetBCFips((Asn1Set)encodable) : null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Set CreateASN1Set(IAsn1TaggedObject taggedObject, bool b) {
            Asn1TaggedObjectBCFips taggedObjectBCFips = (Asn1TaggedObjectBCFips)taggedObject;
            return new Asn1SetBCFips(taggedObjectBCFips.GetTaggedObject(), b);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Set CreateNullASN1Set() {
            return new Asn1SetBCFips(null);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerOutputStream CreateASN1OutputStream(Stream stream) {
            return new DerOutputStreamBCFips(stream);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerOutputStream CreateASN1OutputStream(Stream outputStream, String asn1Encoding) {
            if (Asn1Encodable.Ber.Equals(asn1Encoding)) {
                return new DerOutputStreamBCFips(new BerOutputStream(outputStream));
            }
            return new DerOutputStreamBCFips(new DerOutputStream(outputStream));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerOctetString CreateDEROctetString(byte[] bytes) {
            return new DerOctetStringBCFips(bytes);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerOctetString CreateDEROctetString(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerOctetString) {
                return new DerOctetStringBCFips((DerOctetString)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1EncodableVector CreateASN1EncodableVector() {
            return new Asn1EncodableVectorBCFips();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerNull CreateDERNull() {
            return DerNullBCFips.INSTANCE;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerTaggedObject CreateDERTaggedObject(int i, IAsn1Object primitive) {
            Asn1ObjectBCFips primitiveBCFips = (Asn1ObjectBCFips)primitive;
            return new DerTaggedObjectBCFips(i, primitiveBCFips.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerTaggedObject CreateDERTaggedObject(bool b, int i, IAsn1Object primitive) {
            Asn1ObjectBCFips primitiveBCFips = (Asn1ObjectBCFips)primitive;
            return new DerTaggedObjectBCFips(b, i, primitiveBCFips.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSet CreateDERSet(IAsn1EncodableVector encodableVector) {
            Asn1EncodableVectorBCFips encodableVectorBCFips = (Asn1EncodableVectorBCFips)encodableVector;
            return new DerSetBCFips(encodableVectorBCFips.GetEncodableVector());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSet CreateDERSet(IAsn1Object primitive) {
            Asn1ObjectBCFips primitiveBCFips = (Asn1ObjectBCFips)primitive;
            return new DerSetBCFips(primitiveBCFips.GetPrimitive());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSet CreateDERSet(ISignaturePolicyIdentifier identifier) {
            SignaturePolicyIdentifierBCFips identifierBCFips = (SignaturePolicyIdentifierBCFips)identifier;
            return new DerSetBCFips(identifierBCFips.GetSignaturePolicyIdentifier());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerSet CreateDERSet(IRecipientInfo recipientInfo) {
            RecipientInfoBCFips recipientInfoBCFips = (RecipientInfoBCFips)recipientInfo;
            return new DerSetBCFips(recipientInfoBCFips.GetRecipientInfo());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerEnumerated CreateASN1Enumerated(int i) {
            return new DerEnumeratedBCFips(i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Encoding CreateASN1Encoding() {
            return ASN1EncodingBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAttributeTable CreateAttributeTable(IAsn1Set unat) {
            Asn1SetBCFips asn1SetBCFips = (Asn1SetBCFips)unat;
            return new AttributeTableBCFips(asn1SetBCFips.GetAsn1Set());
        }

        /// <summary><inheritDoc/></summary>
        public virtual IPkcsObjectIdentifiers CreatePKCSObjectIdentifiers() {
            return PkcsObjectIdentifiersBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAttribute CreateAttribute(IDerObjectIdentifier attrType, IAsn1Set attrValues) {
            DerObjectIdentifierBCFips attrTypeBCFips = (DerObjectIdentifierBCFips)attrType;
            Asn1SetBCFips attrValuesBCFips = (Asn1SetBCFips)attrValues;
            return new AttributeBCFips(new Org.BouncyCastle.Asn1.Cms.Attribute(attrTypeBCFips.GetDerObjectIdentifier(
                ), attrValuesBCFips.GetAsn1Set()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IContentInfo CreateContentInfo(IAsn1Sequence sequence) {
            Asn1SequenceBCFips sequenceBCFips = (Asn1SequenceBCFips)sequence;
            return new ContentInfoBCFips(ContentInfo.GetInstance(sequenceBCFips.GetAsn1Sequence()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IContentInfo CreateContentInfo(IDerObjectIdentifier objectIdentifier, IAsn1Encodable encodable
            ) {
            return new ContentInfoBCFips(objectIdentifier, encodable);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISigningCertificate CreateSigningCertificate(IAsn1Sequence sequence) {
            Asn1SequenceBCFips sequenceBCFips = (Asn1SequenceBCFips)sequence;
            return new SigningCertificateBCFips(SigningCertificate.GetInstance(sequenceBCFips.GetAsn1Sequence()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISigningCertificateV2 CreateSigningCertificateV2(IAsn1Sequence sequence) {
            Asn1SequenceBCFips sequenceBCFips = (Asn1SequenceBCFips)sequence;
            return new SigningCertificateV2BCFips(SigningCertificateV2.GetInstance(sequenceBCFips.GetAsn1Sequence()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOcspResponse CreateBasicOCSPResponse(IAsn1Object primitive) {
            Asn1ObjectBCFips primitiveBCFips = (Asn1ObjectBCFips)primitive;
            return new BasicOcspResponseBCFips(BasicOcspResponse.GetInstance(primitiveBCFips.GetPrimitive()));
        }

        /// <summary><inheritDoc/></summary>
        public IBasicOcspResponse CreateBasicOCSPResponse(object response)
        {
            if (response is BasicOcspResponse) {
                return new BasicOcspResponseBCFips((BasicOcspResponse) response);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspObjectIdentifiers CreateOCSPObjectIdentifiers() {
            return OcspObjectIdentifiersBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier CreateAlgorithmIdentifier(IDerObjectIdentifier algorithm) {
            DerObjectIdentifierBCFips algorithmBCFips = (DerObjectIdentifierBCFips)algorithm;
            return new AlgorithmIdentifierBCFips(new AlgorithmIdentifier(algorithmBCFips.GetDerObjectIdentifier(), null
                ));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAlgorithmIdentifier CreateAlgorithmIdentifier(IDerObjectIdentifier algorithm,
            IAsn1Encodable parameters) {
            DerObjectIdentifierBCFips algorithmBCFips = (DerObjectIdentifierBCFips) algorithm;
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips) parameters;
            return new AlgorithmIdentifierBCFips(
                new AlgorithmIdentifier(algorithmBCFips.GetDerObjectIdentifier(), encodableBCFips.GetEncodable()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRsassaPssParameters CreateRSASSAPSSParams(IAsn1Encodable encodable) {
            if (encodable == null) {
                throw new ArgumentException("Expected non-null RSASSA-PSS parameter data");
            }
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips) encodable;
            return new RsassaPssParametersBCFips(RsassaPssParameters.GetInstance(encodableBCFips.GetEncodable()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRsassaPssParameters CreateRSASSAPSSParamsWithMGF1(IDerObjectIdentifier digestAlgoOid, int saltLen,
            int trailerField)
        {
            DerObjectIdentifier oid = ((DerObjectIdentifierBCFips)digestAlgoOid).GetDerObjectIdentifier();
            AlgorithmIdentifier digestAlgo = new AlgorithmIdentifier(oid);
            AlgorithmIdentifier mgf = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, digestAlgo);
            RsassaPssParameters @params = new RsassaPssParameters(digestAlgo, mgf, new DerInteger(saltLen),
                new DerInteger(trailerField));
            return new RsassaPssParametersBCFips(@params);
        }



        /// <summary><inheritDoc/></summary>
        public virtual String GetProviderName() {
            return PROVIDER_NAME;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertID CreateCertificateID() {
            return CertIDBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public IX509Extensions CreateExtensions(IDictionary objectIdentifier) {
            IDictionary dictionary = new Dictionary<DerObjectIdentifier, X509Extension>();
            foreach (IDerObjectIdentifier key in objectIdentifier.Keys) {
                dictionary.Add(((DerObjectIdentifierBCFips)key).GetDerObjectIdentifier(), 
                    ((X509ExtensionBCFips)objectIdentifier[key]).GetX509Extension());
            }
            return new X509ExtensionsBCFips(new X509Extensions(dictionary));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509Extensions CreateExtensions() {
            return X509ExtensionsBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspReqGenerator CreateOCSPReqBuilder() {
            return new OCSPReqBuilderBCFips();
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISigPolicyQualifierInfo CreateSigPolicyQualifierInfo(IDerObjectIdentifier objectIdentifier
            , IDerIA5String @string) {
            return new SigPolicyQualifierInfoBCFips(objectIdentifier, @string);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerStringBase CreateASN1String(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerStringBase) {
                return new DerStringBaseBCFips((DerStringBase)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Object CreateASN1Primitive(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is Asn1Object) {
                return new Asn1ObjectBCFips((Asn1Object)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspResponse CreateOCSPResponse(byte[] bytes) {
            return new OcspResponseBCFips(OcspResponse.GetInstance(new Asn1InputStream(bytes).ReadObject()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspResponse CreateOCSPResponse() {
            return OcspResponseBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspResponse CreateOCSPResponse(IOcspResponseStatus respStatus, IResponseBytes responseBytes) {
            return new OcspResponseBCFips(respStatus, responseBytes);
        }

        /// <summary><inheritDoc/></summary>
        public IOcspResponse CreateOCSPResponse(int respStatus, Object response) {
            if (response == null) {
                return new OcspResponseBCFips(new OcspResponse(new OcspResponseStatus(respStatus), null));
            }
            BasicOcspResponse basicResp = null;
            if (response is IBasicOcspResponse) {
                basicResp = ((BasicOcspResponseBCFips)response).GetBasicOcspResponse();
                if (basicResp == null) {
                    return new OcspResponseBCFips(new OcspResponse(new OcspResponseStatus(respStatus), null));
                }
            }
            if (response is BasicOcspResponse) {
                basicResp = (BasicOcspResponse)response;
            }
            if (basicResp == null) {
                throw new OcspExceptionBCFips(new Exception("unknown response object"));
            }
            Asn1OctetString octs;
            try {
                octs = new DerOctetString(((BasicOcspResponseBCFips)response).GetEncoded());
            } catch (Exception e) {
                throw new OcspExceptionBCFips(new Exception("can't encode object.", e));
            }
            ResponseBytes rb = new ResponseBytes(OcspObjectIdentifiers.PkixOcspBasic, octs);
            return new OcspResponseBCFips(new OcspResponse(new OcspResponseStatus(respStatus), rb));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IResponseBytes CreateResponseBytes(IDerObjectIdentifier asn1ObjectIdentifier, IDerOctetString
             derOctetString) {
            return new ResponseBytesBCFips(asn1ObjectIdentifier, derOctetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspResponseStatus CreateOCSPResponseStatus(int status) {
            return new OcspResponseStatusBCFips(new OcspResponseStatus(status));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspResponseStatus CreateOCSPResponseStatus() {
            return OcspResponseStatusBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICertStatus CreateCertificateStatus() {
            return CertStatusBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public IRevokedCertStatus CreateRevokedStatus(ICertStatus certificateStatus) {
            CertStatus certStatus = ((CertStatusBCFips) certificateStatus).GetCertStatus();
            if (certStatus != null && certStatus.TagNo == 1) {
                return new RevokedStatusBCFips(certStatus);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRevokedCertStatus CreateRevokedStatus(DateTime date, int i) {
            return new RevokedStatusBCFips(date, i);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Object CreateASN1Primitive(byte[] array) {
            return new Asn1ObjectBCFips(array);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerIA5String CreateDERIA5String(IAsn1TaggedObject taggedObject, bool b) {
            return new DerIA5StringBCFips(DerIA5String.GetInstance(((Asn1TaggedObjectBCFips)taggedObject).GetTaggedObject
                (), b));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerIA5String CreateDERIA5String(String str) {
            return new DerIA5StringBCFips(str);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICrlDistPoint CreateCRLDistPoint(Object @object) {
            return new CrlDistPointBCFips(CrlDistPoint.GetInstance(@object is Asn1EncodableBCFips ? ((Asn1EncodableBCFips
                )@object).GetEncodable() : @object));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDistributionPointName CreateDistributionPointName() {
            return DistributionPointNameBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IGeneralNames CreateGeneralNames(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
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
            IAsn1OctetString octetString) {
            return new OtherHashAlgAndValueBCFips(algorithmIdentifier, octetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignaturePolicyId CreateSignaturePolicyId(IDerObjectIdentifier objectIdentifier, IOtherHashAlgAndValue
             algAndValue) {
            return new SignaturePolicyIdBCFips(objectIdentifier, algAndValue);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignaturePolicyId CreateSignaturePolicyId(IDerObjectIdentifier objectIdentifier, IOtherHashAlgAndValue
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
        public virtual IEnvelopedData CreateEnvelopedData(IOriginatorInfo originatorInfo, IAsn1Set set, IEncryptedContentInfo
             encryptedContentInfo, IAsn1Set set1) {
            return new EnvelopedDataBCFips(originatorInfo, set, encryptedContentInfo, set1);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IRecipientInfo CreateRecipientInfo(IKeyTransRecipientInfo keyTransRecipientInfo) {
            return new RecipientInfoBCFips(keyTransRecipientInfo);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IEncryptedContentInfo CreateEncryptedContentInfo(IDerObjectIdentifier data, IAlgorithmIdentifier
             algorithmIdentifier, IAsn1OctetString octetString) {
            return new EncryptedContentInfoBCFips(data, algorithmIdentifier, octetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ITbsCertificateStructure CreateTBSCertificate(IAsn1Encodable encodable) {
            return new TbsCertificateStructureBCFips(TbsCertificateStructure.GetInstance(((Asn1EncodableBCFips)encodable).GetEncodable
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
            , IAlgorithmIdentifier algorithmIdentifier, IAsn1OctetString octetString) {
            return new KeyTransRecipientInfoBCFips(recipientIdentifier, algorithmIdentifier, octetString);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOriginatorInfo CreateNullOriginatorInfo() {
            return new OriginatorInfoBCFips(null);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICmsEnvelopedData CreateCMSEnvelopedData(byte[] bytes) {
            try {
                return new CmsEnvelopedDataBCFips(new CmsEnvelopedData(bytes));
            }
            catch (CmsException e) {
                throw new CmsExceptionBCFips(e);
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
        public virtual AbstractOcspException CreateAbstractOCSPException(Exception e) {
            return new OcspExceptionBCFips(e);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IUnknownCertStatus CreateUnknownStatus() {
            return new UnknownStatusBCFips();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAsn1Dump CreateASN1Dump() {
            return Asn1DumpBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerBitString CreateASN1BitString(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerBitString) {
                return new DerBitStringBCFips((DerBitString)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerGeneralizedTime CreateASN1GeneralizedTime(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerGeneralizedTime) {
                return new DerGeneralizedTimeBCFips((DerGeneralizedTime)encodableBCFips.GetEncodable());
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDerUtcTime CreateASN1UTCTime(IAsn1Encodable encodable) {
            Asn1EncodableBCFips encodableBCFips = (Asn1EncodableBCFips)encodable;
            if (encodableBCFips.GetEncodable() is DerUtcTime) {
                return new DerUtcTimeBCFips((DerUtcTime)encodableBCFips.GetEncodable());
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
        public virtual IBasicOcspRespGenerator CreateBasicOCSPRespBuilder(IRespID respID) {
            return new BasicOcspRespGeneratorBCFips(respID);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IOcspRequest CreateOCSPReq(byte[] requestBytes) {
            return new OcspRequestBCFips(OcspRequest.GetInstance(new Asn1InputStream(requestBytes).ReadObject()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IX509V3CertificateGenerator CreateJcaX509v3CertificateBuilder(IX509Certificate signingCert, 
            IBigInteger number, DateTime startDate, DateTime endDate, IX500Name subjectDn, IPublicKey publicKey) {
            return new X509V3CertificateGeneratorBCFips(signingCert, number, startDate, endDate, subjectDn, publicKey);
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
        public virtual IKeyPurposeID CreateKeyPurposeId() {
            return KeyPurposeIDBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IExtendedKeyUsage CreateExtendedKeyUsage(IKeyPurposeID purposeId) {
            return new ExtendedKeyUsageBCFips(purposeId);
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISubjectPublicKeyInfo CreateSubjectPublicKeyInfo(IPublicKey publicKey) {
            return new SubjectPublicKeyInfoBCFips(new SubjectPublicKeyInfo(new AlgorithmIdentifier(
                    PkcsObjectIdentifiers.RsaEncryption, DerNull.Instance), 
                ((PublicKeyBCFips)publicKey).GetPublicKey().GetEncoded()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ICrlReason CreateCRLReason() {
            return CrlReasonBCFips.GetInstance();
        }

        /// <summary><inheritDoc/></summary>
        public ITstInfo CreateTSTInfo(IContentInfo contentInfoTsp) {
            ICmsTypedData content = new CmsSignedData(((ContentInfoBCFips) contentInfoTsp).GetContentInfo()).SignedContent;
            MemoryStream bOut = new MemoryStream();
            content.Write(bOut);
            return new TstInfoBCFips(TstInfo.GetInstance(Asn1Object.FromByteArray(bOut.ToArray())));
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISingleResponse CreateSingleResp(IBasicOcspResponse basicResp) {
            return new SingleResponseBCFips(basicResp);
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
        public IDigest CreateIDigest(string hashAlgorithm) {
            return new DigestBCFips(hashAlgorithm);
        }

        /// <summary><inheritDoc/></summary>
        public ICertID CreateCertificateID(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber) {
            return new CertIDBCFips(hashAlgorithm, issuerCert, serialNumber);
        }

        /// <summary><inheritDoc/></summary>
        public IX500Name CreateX500NameInstance(IAsn1Encodable issuer) {
            return new X500NameBCFips(X500Name.GetInstance(
                ((Asn1EncodableBCFips) issuer).GetEncodable()));
        }

        /// <summary><inheritDoc/></summary>
        public IOcspRequest CreateOCSPReq(ICertID certId, byte[] documentId) {
            return new OcspRequestBCFips(certId, documentId);
        }

        /// <summary><inheritDoc/></summary>
        public ISigner CreateISigner() {
            return new SignerBCFips(null);
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
        public IPemReader CreatePEMParser(TextReader reader, char[] password) {
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
        public bool IsNullExtension(IX509Extension ext) {
            return ((X509ExtensionBCFips)ext).GetX509Extension() == null;
        }

        /// <summary><inheritDoc/></summary>
        public IX509Extension CreateExtension(bool b, IDerOctetString octetString) {
            return new X509ExtensionBCFips(new X509Extension(b, 
                ((DerOctetStringBCFips)octetString).GetDerOctetString()));
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
        
        /// <inheritdoc/>
        public void IsEncryptionFeatureSupported(int encryptionType, bool withCertificate) {
            if (withCertificate) {
                throw new UnsupportedEncryptionFeatureException(
                    UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS);
            }            
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
