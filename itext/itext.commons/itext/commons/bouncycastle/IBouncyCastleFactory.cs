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

namespace iText.Commons.Bouncycastle {
    /// <summary>
    /// <see cref="IBouncyCastleFactory"/>
    /// contains methods required for bouncy-classes objects creation.
    /// </summary>
    /// <remarks>
    /// <see cref="IBouncyCastleFactory"/>
    /// contains methods required for bouncy-classes objects creation. Implementation will be
    /// selected depending on a bouncy-castle dependency specified by the user.
    /// </remarks>
    public interface IBouncyCastleFactory {
        /// <summary>Cast ASN1 encodable wrapper to the ASN1 object identifier wrapper.</summary>
        /// <param name="encodable">wrapper to be cast</param>
        /// <returns>casted wrapper</returns>
        IASN1ObjectIdentifier CreateASN1ObjectIdentifier(IASN1Encodable encodable);

        /// <summary>
        /// Create ASN1 Object identifier wrapper from
        /// <see cref="System.String"/>.
        /// </summary>
        /// <param name="str">
        /// 
        /// <see cref="System.String"/>
        /// to create object identifier from
        /// </param>
        /// <returns>created object identifier</returns>
        IASN1ObjectIdentifier CreateASN1ObjectIdentifier(String str);

        /// <summary>
        /// Create ASN1 Object identifier wrapper from
        /// <see cref="System.Object"/>
        /// using
        /// <c>getInstance</c>
        /// method call.
        /// </summary>
        /// <param name="object">
        /// 
        /// <see cref="System.Object"/>
        /// to create object identifier from
        /// </param>
        /// <returns>created object identifier</returns>
        IASN1ObjectIdentifier CreateASN1ObjectIdentifierInstance(Object @object);

        /// <summary>
        /// Create ASN1 Input stream wrapper from
        /// <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">
        /// 
        /// <see cref="System.IO.Stream"/>
        /// to create ASN1 Input stream from
        /// </param>
        /// <returns>created ASN1 Input stream</returns>
        IASN1InputStream CreateASN1InputStream(Stream stream);

        /// <summary>
        /// Create ASN1 Input stream wrapper from
        /// <c>byte[]</c>.
        /// </summary>
        /// <param name="bytes">
        /// 
        /// <c>byte[]</c>
        /// to create ASN1 Input stream from
        /// </param>
        /// <returns>created ASN1 Input stream</returns>
        IASN1InputStream CreateASN1InputStream(byte[] bytes);

        /// <summary>Cast ASN1 Encodable wrapper to the ASN1 Octet string wrapper.</summary>
        /// <param name="encodable">to be casted to ASN1 Octet string wrapper</param>
        /// <returns>casted ASN1 Octet string wrapper</returns>
        IASN1OctetString CreateASN1OctetString(IASN1Encodable encodable);

        /// <summary>
        /// Create ASN1 Octet string wrapper from ASN1 Tagged object wrapper and
        /// <c>boolean</c>
        /// parameter.
        /// </summary>
        /// <param name="taggedObject">ASN1 Tagged object wrapper to create ASN1 Octet string wrapper from</param>
        /// <param name="b">boolean to create ASN1 Octet string wrapper</param>
        /// <returns>created ASN1 Octet string wrapper</returns>
        IASN1OctetString CreateASN1OctetString(IASN1TaggedObject taggedObject, bool b);

        /// <summary>
        /// Create ASN1 Octet string wrapper from
        /// <c>byte[]</c>.
        /// </summary>
        /// <param name="bytes">
        /// 
        /// <c>byte[]</c>
        /// to create ASN1 Octet string wrapper from
        /// </param>
        /// <returns>created ASN1 Octet string wrapper</returns>
        IASN1OctetString CreateASN1OctetString(byte[] bytes);

        /// <summary>
        /// Create ASN1 Octet string wrapper from ASN1 Primitive wrapper.
        /// </summary>
        /// <param name="primitive">
        /// ASN1 Primitive wrapper to create ASN1 Octet string wrapper from
        /// </param>
        /// <returns>created ASN1 Octet string wrapper</returns>
        IASN1OctetString CreateASN1OctetString(IASN1Primitive primitive);

        /// <summary>
        /// Cast
        /// <see cref="System.Object"/>
        /// to ASN1 Sequence wrapper.
        /// </summary>
        /// <param name="object">
        /// 
        /// <see cref="System.Object"/>
        /// to be cast. Must be instance of ASN1 Sequence
        /// </param>
        /// <returns>casted ASN1 Sequence wrapper</returns>
        IASN1Sequence CreateASN1Sequence(Object @object);

        /// <summary>Cast ASN1 encodable wrapper to the ASN1 Sequence wrapper.</summary>
        /// <param name="encodable">to be casted to ASN1 Sequence wrapper</param>
        /// <returns>casted ASN1 Sequence wrapper</returns>
        IASN1Sequence CreateASN1Sequence(IASN1Encodable encodable);

        /// <summary>
        /// Create ASN1 Sequence wrapper from
        /// <c>byte[]</c>.
        /// </summary>
        /// <param name="array">
        /// 
        /// <c>byte[]</c>
        /// to create ASN1 Sequence wrapper from
        /// </param>
        /// <returns>created ASN1 Sequence wrapper</returns>
        IASN1Sequence CreateASN1Sequence(byte[] array);

        /// <summary>
        /// Create ASN1 Sequence wrapper from
        /// <see cref="System.Object"/>
        /// using
        /// <c>getInstance</c>
        /// method call.
        /// </summary>
        /// <param name="object">
        /// 
        /// <see cref="System.Object"/>
        /// to create ASN1 Sequence wrapper from
        /// </param>
        /// <returns>created ASN1 Sequence wrapper</returns>
        IASN1Sequence CreateASN1SequenceInstance(Object @object);

        /// <summary>Create DER Sequence wrapper from ASN1 Encodable vector wrapper.</summary>
        /// <param name="encodableVector">ASN1 Encodable vector wrapper to create DER Sequence wrapper from</param>
        /// <returns>created DER Sequence wrapper</returns>
        IDERSequence CreateDERSequence(IASN1EncodableVector encodableVector);

        /// <summary>Create DER Sequence wrapper from ASN1 Primitive wrapper.</summary>
        /// <param name="primitive">ASN1 Primitive wrapper to create DER Sequence wrapper from</param>
        /// <returns>created DER Sequence wrapper</returns>
        IDERSequence CreateDERSequence(IASN1Primitive primitive);

        /// <summary>Create ASN1 Tagged object wrapper from ASN1 Encodable wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable vector to create ASN1 Tagged object wrapper from</param>
        /// <returns>created ASN1 Tagged object wrapper</returns>
        IASN1TaggedObject CreateASN1TaggedObject(IASN1Encodable encodable);

        /// <summary>Cast ASN1 Encodable wrapper to ASN1 Integer wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable wrapper to be cast</param>
        /// <returns>casted ASN1 Integer</returns>
        IASN1Integer CreateASN1Integer(IASN1Encodable encodable);

        /// <summary>
        /// Create ASN1 Integer wrapper from
        /// <c>int</c>.
        /// </summary>
        /// <param name="i">
        /// 
        /// <c>int</c>
        /// to create ASN1 Integer wrapper from
        /// </param>
        /// <returns>created ASN1 Integer wrapper</returns>
        IASN1Integer CreateASN1Integer(int i);

        /// <summary>
        /// Create ASN1 Integer wrapper from
        /// <see cref="iText.Commons.Bouncycastle.Math.IBigInteger"/>.
        /// </summary>
        /// <param name="i">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Math.IBigInteger"/>
        /// to create ASN1 Integer wrapper from
        /// </param>
        /// <returns>created ASN1 Integer wrapper</returns>
        IASN1Integer CreateASN1Integer(IBigInteger i);

        /// <summary>Cast ASN1 Encodable wrapper to ASN1 Set wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable wrapper to be cast</param>
        /// <returns>casted ASN1 Set</returns>
        IASN1Set CreateASN1Set(IASN1Encodable encodable);

        /// <summary>
        /// Create ASN1 Set wrapper from
        /// <see cref="System.Object"/>.
        /// </summary>
        /// <param name="encodable">
        /// 
        /// <see cref="System.Object"/>
        /// to create ASN1 Set wrapper from. Must be instance of ASN1 Set
        /// </param>
        /// <returns>created ASN1 Set wrapper</returns>
        IASN1Set CreateASN1Set(Object encodable);

        /// <summary>
        /// Create ASN1 Set wrapper from ASN1 Tagged object wrapper and
        /// <c>boolean</c>
        /// parameter.
        /// </summary>
        /// <param name="taggedObject">ASN1 Tagged object wrapper to create ASN1 Set wrapper from</param>
        /// <param name="b">boolean to create ASN1 Set wrapper</param>
        /// <returns>created ASN1 Set wrapper</returns>
        IASN1Set CreateASN1Set(IASN1TaggedObject taggedObject, bool b);

        /// <summary>
        /// Create ASN1 Set wrapper which will store
        /// <see langword="null"/>.
        /// </summary>
        /// <returns>
        /// ASN1 Set wrapper with
        /// <see langword="null"/>
        /// value
        /// </returns>
        IASN1Set CreateNullASN1Set();

        /// <summary>
        /// Create ASN1 Output stream wrapper from
        /// <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">
        /// 
        /// <see cref="System.IO.Stream"/>
        /// to create ASN1 Output stream wrapper from
        /// </param>
        /// <returns>created ASN1 Output stream wrapper</returns>
        IASN1OutputStream CreateASN1OutputStream(Stream stream);

        /// <summary>
        /// Create ASN1 Output stream wrapper from
        /// <see cref="System.IO.Stream"/>
        /// and ASN1 Encoding.
        /// </summary>
        /// <param name="outputStream">
        /// 
        /// <see cref="System.IO.Stream"/>
        /// to create ASN1 Output stream wrapper from
        /// </param>
        /// <param name="asn1Encoding">ASN1 Encoding to be used</param>
        /// <returns>created ASN1 Output stream wrapper</returns>
        IASN1OutputStream CreateASN1OutputStream(Stream outputStream, String asn1Encoding);

        /// <summary>
        /// Create DER Octet string wrapper from
        /// <c>byte[]</c>.
        /// </summary>
        /// <param name="bytes">
        /// 
        /// <c>byte[]</c>
        /// to create DER Octet string wrapper from
        /// </param>
        /// <returns>created DER Octet string wrapper</returns>
        IDEROctetString CreateDEROctetString(byte[] bytes);

        /// <summary>Cast ASN1 Encodable wrapper to DER Octet string wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable wrapper to be casted</param>
        /// <returns>DER Octet string wrapper</returns>
        IDEROctetString CreateDEROctetString(IASN1Encodable encodable);

        /// <summary>Create ASN1 Encodable wrapper without parameters.</summary>
        /// <returns>created ASN1 Encodable wrapper</returns>
        IASN1EncodableVector CreateASN1EncodableVector();

        /// <summary>Create DER Null wrapper without parameters.</summary>
        /// <returns>created DER Null wrapper</returns>
        IDERNull CreateDERNull();

        /// <summary>
        /// Create DER Tagged object wrapper from
        /// <c>int</c>
        /// value and ASN1 Primitive wrapper.
        /// </summary>
        /// <param name="i">
        /// 
        /// <c>int</c>
        /// value for DER Tagged object wrapper creation
        /// </param>
        /// <param name="primitive">ASN1 Primitive wrapper to create DER Tagged object wrapper from</param>
        /// <returns>created DER Tagged object wrapper</returns>
        IDERTaggedObject CreateDERTaggedObject(int i, IASN1Primitive primitive);

        /// <summary>
        /// Create DER Tagged object wrapper from
        /// <c>int</c>
        /// value,
        /// <c>boolean</c>
        /// value and ASN1 Primitive wrapper.
        /// </summary>
        /// <param name="b">
        /// 
        /// <c>boolean</c>
        /// value for DER Tagged object wrapper creation
        /// </param>
        /// <param name="i">
        /// 
        /// <c>int</c>
        /// value for DER Tagged object wrapper creation
        /// </param>
        /// <param name="primitive">ASN1 Primitive wrapper to create DER Tagged object wrapper from</param>
        /// <returns>created DER Tagged object wrapper</returns>
        IDERTaggedObject CreateDERTaggedObject(bool b, int i, IASN1Primitive primitive);

        /// <summary>Create DER Set wrapper from ASN1 Encodable vector wrapper.</summary>
        /// <param name="encodableVector">ASN1 Encodable vector wrapper to create DER Set wrapper from</param>
        /// <returns>created DER Set wrapper</returns>
        IDERSet CreateDERSet(IASN1EncodableVector encodableVector);

        /// <summary>Create DER Set wrapper from ASN1 Primitive wrapper.</summary>
        /// <param name="primitive">ASN1 Primitive wrapper to create DER Set wrapper from</param>
        /// <returns>created DER Set wrapper</returns>
        IDERSet CreateDERSet(IASN1Primitive primitive);

        /// <summary>Create DER Set wrapper from signature policy identifier wrapper.</summary>
        /// <param name="identifier">signature policy identifier wrapper to create DER Set wrapper from</param>
        /// <returns>created DER Set wrapper</returns>
        IDERSet CreateDERSet(ISignaturePolicyIdentifier identifier);

        /// <summary>Create DER Set wrapper from recipient info wrapper.</summary>
        /// <param name="recipientInfo">recipient info wrapper to create DER Set wrapper from</param>
        /// <returns>created DER Set wrapper</returns>
        IDERSet CreateDERSet(IRecipientInfo recipientInfo);

        /// <summary>
        /// Create ASN1 Enumerated wrapper from
        /// <c>int</c>
        /// value.
        /// </summary>
        /// <param name="i">
        /// 
        /// <c>int</c>
        /// to create ASN1 Enumerated wrapper from
        /// </param>
        /// <returns>created ASN1 Enumerated wrapper</returns>
        IASN1Enumerated CreateASN1Enumerated(int i);

        /// <summary>Create ASN1 Encoding without parameters.</summary>
        /// <returns>created ASN1 Encoding</returns>
        IASN1Encoding CreateASN1Encoding();

        /// <summary>Create attribute table wrapper from ASN1 Set wrapper.</summary>
        /// <param name="unat">ASN1 Set wrapper to create attribute table wrapper from</param>
        /// <returns>created attribute table wrapper</returns>
        IAttributeTable CreateAttributeTable(IASN1Set unat);

        /// <summary>Create PKCS Object identifiers wrapper without parameters.</summary>
        /// <returns>created PKCS Object identifiers</returns>
        IPKCSObjectIdentifiers CreatePKCSObjectIdentifiers();

        /// <summary>Create attribute wrapper from ASN1 Object identifier wrapper and ASN1 Set wrapper.</summary>
        /// <param name="attrType">ASN1 Object identifier wrapper to create attribute wrapper from</param>
        /// <param name="attrValues">ASN1 Object identifier wrapper to create attribute wrapper from</param>
        /// <returns>created attribute wrapper</returns>
        IAttribute CreateAttribute(IASN1ObjectIdentifier attrType, IASN1Set attrValues);

        /// <summary>Create content info wrapper from ASN1 Sequence wrapper.</summary>
        /// <param name="sequence">ASN1 Sequence wrapper to create content info wrapper from</param>
        /// <returns>created content info wrapper</returns>
        IContentInfo CreateContentInfo(IASN1Sequence sequence);

        /// <summary>Create content info wrapper from ASN1 Object identifier wrapper and ASN1 Encodable wrapper.</summary>
        /// <param name="objectIdentifier">ASN1 Object identifier wrapper to create content info wrapper from</param>
        /// <param name="encodable">ASN1 Encodable wrapper to create content info wrapper from</param>
        /// <returns>created content info wrapper</returns>
        IContentInfo CreateContentInfo(IASN1ObjectIdentifier objectIdentifier, IASN1Encodable encodable);

        /// <summary>Create timestamp token wrapper from content info wrapper.</summary>
        /// <param name="contentInfo">content info wrapper to create timestamp token wrapper from</param>
        /// <returns>created timestamp token wrapper</returns>
        ITimeStampToken CreateTimeStampToken(IContentInfo contentInfo);

        /// <summary>Create signing certificate wrapper from ASN1 Sequence wrapper.</summary>
        /// <param name="sequence">ASN1 Sequence wrapper to create signing certificate wrapper from</param>
        /// <returns>created signing certificate wrapper</returns>
        ISigningCertificate CreateSigningCertificate(IASN1Sequence sequence);

        /// <summary>Create signing certificate version 2 wrapper from ASN1 Sequence wrapper.</summary>
        /// <param name="sequence">ASN1 Sequence wrapper to create signing certificate version 2 wrapper from</param>
        /// <returns>created signing certificate version 2 wrapper</returns>
        ISigningCertificateV2 CreateSigningCertificateV2(IASN1Sequence sequence);

        /// <summary>Create basic OCSP Response wrapper from ASN1 Primitive wrapper.</summary>
        /// <param name="primitive">ASN1 Primitive wrapper to create basic OCSP response wrapper from</param>
        /// <returns>created basic OCSP response wrapper</returns>
        IBasicOCSPResponse CreateBasicOCSPResponse(IASN1Primitive primitive);

        /// <summary>
        /// Create basic OCSP Resp wrapper from
        /// <see cref="System.Object"/>.
        /// </summary>
        /// <param name="response">
        /// 
        /// <see cref="System.Object"/>
        /// to create basic OCSP Resp wrapper from. Must be actual basic OCSP Resp instance
        /// </param>
        /// <returns>created basic OCSP Resp wrapper</returns>
        IBasicOCSPResponse CreateBasicOCSPResponse(Object response);

        /// <summary>Create OCSP Object identifiers wrapper without parameters.</summary>
        /// <returns>created OCSP Object identifiers wrapper</returns>
        IOCSPObjectIdentifiers CreateOCSPObjectIdentifiers();

        /// <summary>Create algorithm identifier wrapper from ASN1 Object identifier wrapper.</summary>
        /// <param name="algorithm">ASN1 Object identifier wrapper to create algorithm identifier wrapper from</param>
        /// <returns>created algorithm identifier wrapper</returns>
        IAlgorithmIdentifier CreateAlgorithmIdentifier(IASN1ObjectIdentifier algorithm);

        /// <summary>Create algorithm identifier wrapper from ASN1 Object identifier wrapper and ASN1 Encodable wrapper.
        ///     </summary>
        /// <param name="algorithm">ASN1 Object identifier wrapper to create algorithm identifier wrapper from</param>
        /// <param name="encodable">ASN1 Encodable wrapper to create algorithm identifier wrapper from</param>
        /// <returns>created algorithm identifier wrapper</returns>
        IAlgorithmIdentifier CreateAlgorithmIdentifier(IASN1ObjectIdentifier algorithm, IASN1Encodable parameters);

        /// <summary>Create a RSASSA-PSS params wrapper from an ASN1 Encodable wrapper.
        ///     </summary>
        /// <param name="encodable"> ASN1 Encodable wrapper to create RSASSA-PSS params wrapper from</param>
        /// <returns>created RSASSA-PSS params wrapper</returns>
        IRsassaPssParameters CreateRSASSAPSSParams(IASN1Encodable encodable);
     
        /// <summary> Create a RSASSA-PSS params wrapper from a digest algorithm OID, a salt length and a trailer field length.
        /// The mask generation function will be set to MGF1, and the same digest algorithm will be used to populate the
        /// MGF parameters.
        ///     </summary>
        /// <param name="digestAlgoOid">  identifier of the digest algorithm to be used both in the MGF and in the signature</param>
        /// <param name="saltLen">        salt length value</param>
        /// <param name="trailerField">   trailer field value</param>
        /// <returns><see cref="IRsassaPssParameters"/> object initialised with the parameters supplied
        IRsassaPssParameters CreateRSASSAPSSParamsWithMGF1(IASN1ObjectIdentifier digestAlgoOid, int saltLen, int trailerField);

        /// <summary>
        /// Get
        /// <see cref="System.String"/>
        /// which represents providers name for this factory.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// which represents providers name
        /// </returns>
        String GetProviderName();

        /// <summary>Create certificate ID wrapper without parameters.</summary>
        /// <returns>created certificate ID wrapper</returns>
        ICertificateID CreateCertificateID();

        /// <summary>Create extensions wrapper without parameters.</summary>
        /// <returns>created extensions wrapper</returns>
        IExtensions CreateExtensions();
        
        /// <summary>
        /// Create extensions wrapper from
        /// <see cref="System.Collections.IDictionary"/>.
        /// </summary>
        /// <returns>created extensions wrapper</returns>
        IExtensions CreateExtensions(IDictionary objectIdentifier);

        /// <summary>Create OCSP Req builder wrapper without parameters.</summary>
        /// <returns>created OCSP Req builder wrapper</returns>
        IOCSPReqBuilder CreateOCSPReqBuilder();

        /// <summary>Create sig policy qualifier info wrapper from ASN1 Object identifier wrapper and DERIA5 String wrapper.
        ///     </summary>
        /// <param name="objectIdentifier">ASN1 Object identifier wrapper to create sig policy qualifier info wrapper from
        ///     </param>
        /// <param name="string">DERIA5 String wrapper to create sig policy qualifier info wrapper from</param>
        /// <returns>created sig policy qualifier info wrapper</returns>
        ISigPolicyQualifierInfo CreateSigPolicyQualifierInfo(IASN1ObjectIdentifier objectIdentifier, IDERIA5String
             @string);

        /// <summary>Cast ASN1 Encodable wrapper to ASN1 String wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable wrapper to be cast</param>
        /// <returns>casted ASN1 String wrapper</returns>
        IASN1String CreateASN1String(IASN1Encodable encodable);

        /// <summary>Cast ASN1 Encodable wrapper to ASN1 Primitive wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable wrapper to be cast</param>
        /// <returns>casted ASN1 Primitive wrapper</returns>
        IASN1Primitive CreateASN1Primitive(IASN1Encodable encodable);

        /// <summary>
        /// Create ASN1 Primitive wrapper from
        /// <c>byte[]</c>
        /// value.
        /// </summary>
        /// <param name="array">
        /// 
        /// <c>byte[]</c>
        /// value to create ASN1 Primitive wrapper from
        /// </param>
        /// <returns>created ASN1 Primitive wrapper</returns>
        IASN1Primitive CreateASN1Primitive(byte[] array);

        /// <summary>
        /// Create OCSP Resp wrapper from
        /// <c>byte[]</c>
        /// value.
        /// </summary>
        /// <param name="bytes">
        /// 
        /// <c>byte[]</c>
        /// value to create OCSP Resp wrapper from
        /// </param>
        /// <returns>created OCSP Resp wrapper</returns>
        IOCSPResponse CreateOCSPResponse(byte[] bytes);

        /// <summary>Create OCSP Resp wrapper without parameters.</summary>
        /// <returns>created OCSP Resp wrapper</returns>
        IOCSPResponse CreateOCSPResponse();

        /// <summary>Create OCSP Response wrapper from OCSP Response status wrapper and response bytes wrapper.</summary>
        /// <param name="respStatus">OCSP Response status wrapper to create OCSP Response wrapper from</param>
        /// <param name="responseBytes">response bytes wrapper to create OCSP Response wrapper from</param>
        /// <returns>created OCSP Response wrapper</returns>
        IOCSPResponse CreateOCSPResponse(IOCSPResponseStatus respStatus, IResponseBytes responseBytes);

        /// <summary>Create response bytes wrapper from ASN1 Object identifier wrapper and DER Octet string wrapper.</summary>
        /// <param name="asn1ObjectIdentifier">ASN1 Object identifier wrapper to create response bytes wrapper from</param>
        /// <param name="derOctetString">DER Octet string wrapper to create response bytes wrapper from</param>
        /// <returns>created response bytes wrapper</returns>
        IResponseBytes CreateResponseBytes(IASN1ObjectIdentifier asn1ObjectIdentifier, IDEROctetString derOctetString
            );
        
        /// <summary>
        /// Create OCSP Response wrapper from
        /// <c>int</c>
        /// value and
        /// <see cref="System.object"/>
        /// </summary>
        /// <param name="respStatus">
        /// 
        /// <c>int</c>
        /// value to create OCSP Response wrapper from
        /// </param>
        /// <param name="ocspRespObject">
        ///
        /// <see cref="System.object"/>
        /// to create OCSP Response wrapper from
        /// </param>
        /// <returns>created OCSP Response wrapper</returns>
        IOCSPResponse CreateOCSPResponse(int respStatus, Object ocspRespObject);

        /// <summary>
        /// Create OCSP Response status wrapper from
        /// <c>int</c>
        /// value.
        /// </summary>
        /// <param name="status">
        /// 
        /// <c>int</c>
        /// value to create OCSP Response status wrapper from
        /// </param>
        /// <returns>created OCSP Response status wrapper</returns>
        IOCSPResponseStatus CreateOCSPResponseStatus(int status);

        /// <summary>Create OCSP Response status wrapper without parameters.</summary>
        /// <returns>created OCSP Response status wrapper</returns>
        IOCSPResponseStatus CreateOCSPResponseStatus();

        /// <summary>Create certificate status wrapper without parameters.</summary>
        /// <returns>created certificate status wrapper</returns>
        ICertificateStatus CreateCertificateStatus();

        /// <summary>Create revoked status wrapper from certificate status wrapper.</summary>
        /// <param name="certificateStatus">certificate status wrapper to create revoked status wrapper from</param>
        /// <returns>created revoked status wrapper</returns>
        IRevokedStatus CreateRevokedStatus(ICertificateStatus certificateStatus);

        /// <summary>
        /// Create revoked status wrapper from
        /// <see cref="System.DateTime"/>
        /// and
        /// <c>int</c>
        /// value.
        /// </summary>
        /// <param name="date">
        /// 
        /// <see cref="System.DateTime"/>
        /// to create revoked status wrapper from
        /// </param>
        /// <param name="i">
        /// 
        /// <c>int</c>
        /// value to create revoked status wrapper from
        /// </param>
        /// <returns>created revoked status wrapper</returns>
        IRevokedStatus CreateRevokedStatus(DateTime date, int i);

        /// <summary>
        /// Create DERIA5 String wrapper from ASN1 Tagged object wrapper and
        /// <c>boolean</c>
        /// value.
        /// </summary>
        /// <param name="taggedObject">ASN1 Tagged object wrapper to create DERIA5 String wrapper from</param>
        /// <param name="b">
        /// 
        /// <c>boolean</c>
        /// value to create DERIA5 String wrapper from
        /// </param>
        /// <returns>created DERIA5 String wrapper</returns>
        IDERIA5String CreateDERIA5String(IASN1TaggedObject taggedObject, bool b);

        /// <summary>
        /// Create DERIA5 String wrapper from
        /// <see cref="System.String"/>
        /// value.
        /// </summary>
        /// <param name="str">
        /// 
        /// <see cref="System.String"/>
        /// value to create DERIA5 String wrapper from
        /// </param>
        /// <returns>created DERIA5 String wrapper</returns>
        IDERIA5String CreateDERIA5String(String str);

        /// <summary>
        /// Create CRL Dist point wrapper from
        /// <see cref="System.Object"/>.
        /// </summary>
        /// <param name="object">
        /// 
        /// <see cref="System.Object"/>
        /// to create CRL Dist point wrapper from
        /// </param>
        /// <returns>created CRL Dist point wrapper</returns>
        ICRLDistPoint CreateCRLDistPoint(Object @object);

        /// <summary>Create distribution point name wrapper without parameters.</summary>
        /// <returns>created distribution point name wrapper</returns>
        IDistributionPointName CreateDistributionPointName();

        /// <summary>Cast ASN1 Encodable wrapper to general names wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable wrapper to be cast</param>
        /// <returns>casted general names wrapper</returns>
        IGeneralNames CreateGeneralNames(IASN1Encodable encodable);

        /// <summary>Create general name wrapper without parameters.</summary>
        /// <returns>created general name wrapper</returns>
        IGeneralName CreateGeneralName();

        /// <summary>Create other hash alg and value wrapper from algorithm identifier wrapper and ASN1 Octet string wrapper.
        ///     </summary>
        /// <param name="algorithmIdentifier">algorithm identifier wrapper to create other hash alg and value wrapper from
        ///     </param>
        /// <param name="octetString">ASN1 Octet string wrapper to create other hash alg and value wrapper from</param>
        /// <returns>created other hash alg and value wrapper</returns>
        IOtherHashAlgAndValue CreateOtherHashAlgAndValue(IAlgorithmIdentifier algorithmIdentifier, IASN1OctetString
             octetString);

        /// <summary>Create signature policy id wrapper from ASN1 Object identifier wrapper and other hash alg and value wrapper.
        ///     </summary>
        /// <param name="objectIdentifier">ASN1 Object identifier wrapper to create signature policy id wrapper from</param>
        /// <param name="algAndValue">other hash alg and value wrapper to create signature policy id wrapper from</param>
        /// <returns>created signature policy id wrapper</returns>
        ISignaturePolicyId CreateSignaturePolicyId(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue
            );

        /// <summary>
        /// Create signature policy id wrapper from ASN1 Object identifier wrapper, other hash alg and value wrapper
        /// and sig policy qualifier info wrappers.
        /// </summary>
        /// <param name="objectIdentifier">ASN1 Object identifier wrapper to create signature policy id wrapper from</param>
        /// <param name="algAndValue">other hash alg and value wrapper to create signature policy id wrapper from</param>
        /// <param name="policyQualifiers">sig policy qualifier info wrappers to create signature policy id wrapper from
        ///     </param>
        /// <returns>created signature policy id wrapper</returns>
        ISignaturePolicyId CreateSignaturePolicyId(IASN1ObjectIdentifier objectIdentifier, IOtherHashAlgAndValue algAndValue
            , params ISigPolicyQualifierInfo[] policyQualifiers);

        /// <summary>Create signature policy identifier wrapper from signature policy id wrapper.</summary>
        /// <param name="policyId">signature policy id wrapper to create signature policy identifier wrapper from</param>
        /// <returns>created signature policy identifier wrapper</returns>
        ISignaturePolicyIdentifier CreateSignaturePolicyIdentifier(ISignaturePolicyId policyId);

        /// <summary>
        /// Create enveloped data wrapper from originator info wrapper, ASN1 Set wrapper,
        /// encrypted content info wrapper and another ASN1 Set wrapper.
        /// </summary>
        /// <param name="originatorInfo">originator info wrapper to create enveloped data wrapper from</param>
        /// <param name="set">ASN1 Set wrapper to create enveloped data wrapper from</param>
        /// <param name="encryptedContentInfo">encrypted content info wrapper to create enveloped data wrapper from</param>
        /// <param name="set1">ASN1 Set wrapper to create enveloped data wrapper from</param>
        /// <returns>created enveloped data wrapper</returns>
        IEnvelopedData CreateEnvelopedData(IOriginatorInfo originatorInfo, IASN1Set set, IEncryptedContentInfo encryptedContentInfo
            , IASN1Set set1);

        /// <summary>Create recipient info wrapper from key trans recipient info wrapper.</summary>
        /// <param name="keyTransRecipientInfo">key trans recipient info wrapper to create recipient info wrapper from
        ///     </param>
        /// <returns>created recipient info wrapper</returns>
        IRecipientInfo CreateRecipientInfo(IKeyTransRecipientInfo keyTransRecipientInfo);

        /// <summary>
        /// Create encrypted content info wrapper from ASN1 Object identifier wrapper,
        /// algorithm identifier wrapper and ASN1 Octet string wrapper.
        /// </summary>
        /// <param name="data">ASN1 Object identifier wrapper to create encrypted content info wrapper from</param>
        /// <param name="algorithmIdentifier">algorithm identifier wrapper to create encrypted content info wrapper from
        ///     </param>
        /// <param name="octetString">ASN1 Octet string wrapper to create encrypted content info wrapper from</param>
        /// <returns>created encrypted content info wrapper</returns>
        IEncryptedContentInfo CreateEncryptedContentInfo(IASN1ObjectIdentifier data, IAlgorithmIdentifier algorithmIdentifier
            , IASN1OctetString octetString);

        /// <summary>Create TBS Certificate wrapper from ASN1 Encodable wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable wrapper to create TBS Certificate wrapper from</param>
        /// <returns>created TBS Certificate wrapper</returns>
        ITBSCertificate CreateTBSCertificate(IASN1Encodable encodable);

        /// <summary>
        /// Create issuer and serial number wrapper from X500 Name wrapper and
        /// <see cref="iText.Commons.Bouncycastle.Math.IBigInteger"/>.
        /// </summary>
        /// <param name="issuer">X500 Name wrapper to create issuer and serial number wrapper from</param>
        /// <param name="value">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Math.IBigInteger"/>
        /// to create issuer and serial number wrapper from
        /// </param>
        /// <returns>created issuer and serial number wrapper</returns>
        IIssuerAndSerialNumber CreateIssuerAndSerialNumber(IX500Name issuer, IBigInteger value);

        /// <summary>Create recipient identifier wrapper from issuer and serial number wrapper.</summary>
        /// <param name="issuerAndSerialNumber">issuer and serial number wrapper to create recipient identifier wrapper from
        ///     </param>
        /// <returns>created recipient identifier wrapper</returns>
        IRecipientIdentifier CreateRecipientIdentifier(IIssuerAndSerialNumber issuerAndSerialNumber);

        /// <summary>
        /// Create key trans recipient info wrapper from recipient identifier wrapper,
        /// algorithm identifier wrapper and ASN1 Octet string wrapper.
        /// </summary>
        /// <param name="recipientIdentifier">recipient identifier wrapper to create key trans recipient info wrapper from
        ///     </param>
        /// <param name="algorithmIdentifier">algorithm identifier wrapper to create key trans recipient info wrapper from
        ///     </param>
        /// <param name="octetString">ASN1 Octet string wrapper to create key trans recipient info wrapper from</param>
        /// <returns>created key trans recipient info wrapper</returns>
        IKeyTransRecipientInfo CreateKeyTransRecipientInfo(IRecipientIdentifier recipientIdentifier, IAlgorithmIdentifier
             algorithmIdentifier, IASN1OctetString octetString);

        /// <summary>
        /// Create originator info wrapper with
        /// <see langword="null"/>
        /// value.
        /// </summary>
        /// <returns>created originator info wrapper</returns>
        IOriginatorInfo CreateNullOriginatorInfo();

        /// <summary>
        /// Create CMS enveloped data from
        /// <c>byte[]</c>
        /// value.
        /// </summary>
        /// <param name="valueBytes">
        /// 
        /// <c>byte[]</c>
        /// value to create CMS enveloped data from
        /// </param>
        /// <returns>created CMS enveloped data</returns>
        ICMSEnvelopedData CreateCMSEnvelopedData(byte[] valueBytes);

        /// <summary>Create timestamp request generator wrapper without parameters.</summary>
        /// <returns>created timestamp request generator wrapper</returns>
        ITimeStampRequestGenerator CreateTimeStampRequestGenerator();

        /// <summary>
        /// Create timestamp response wrapper from
        /// <c>byte[]</c>
        /// value.
        /// </summary>
        /// <param name="respBytes">
        /// 
        /// <c>byte[]</c>
        /// value to create timestamp response wrapper from
        /// </param>
        /// <returns>created timestamp response wrapper</returns>
        ITimeStampResponse CreateTimeStampResponse(byte[] respBytes);

        /// <summary>
        /// Create OCSP Exception wrapper from usual
        /// <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="e">
        /// 
        /// <see cref="System.Exception"/>
        /// to create OCSP Exception wrapper from
        /// </param>
        /// <returns>created OCSP Exception wrapper</returns>
        AbstractOCSPException CreateAbstractOCSPException(Exception e);

        /// <summary>Create unknown status wrapper without parameters.</summary>
        /// <returns>created unknown status wrapper</returns>
        IUnknownStatus CreateUnknownStatus();

        /// <summary>Create ASN1 Dump wrapper without parameters.</summary>
        /// <returns>created ASN1 Dump wrapper</returns>
        IASN1Dump CreateASN1Dump();

        /// <summary>Cast ASN1 Encodable wrapper to ASN1 Bit string wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable wrapper to be cast</param>
        /// <returns>casted ASN1 Bit string wrapper</returns>
        IASN1BitString CreateASN1BitString(IASN1Encodable encodable);

        /// <summary>Cast ASN1 Encodable wrapper to ASN1 Generalized time wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable wrapper to be cast</param>
        /// <returns>casted ASN1 Generalized time wrapper</returns>
        IASN1GeneralizedTime CreateASN1GeneralizedTime(IASN1Encodable encodable);

        /// <summary>Cast ASN1 Encodable wrapper to ASN1 UTC Time wrapper.</summary>
        /// <param name="encodable">ASN1 Encodable wrapper to be cast</param>
        /// <returns>casted ASN1 UTC Time wrapper</returns>
        IASN1UTCTime CreateASN1UTCTime(IASN1Encodable encodable);

        /// <summary>
        /// Create timestamp response generator wrapper from private key wrapper, X509 Certificate wrapper, 
        /// <see cref="System.string"/> 
        /// allowed digest and
        /// <see cref="System.string"/>
        /// policy oid.
        /// </summary>
        /// <param name="pk">private key wrapper to create timestamp response generator wrapper from</param>
        /// <param name="cert">X509 Certificate wrapper to create timestamp response generator wrapper from</param>
        /// <param name="allowedDigest">
        ///
        /// <see cref="System.string"/>
        /// allowed digest to create timestamp response generator wrapper from
        /// </param>
        /// <param name="policyOid">
        ///
        /// <see cref="System.string"/>
        /// policy oid to create timestamp response generator wrapper from
        /// </param>
        /// <returns>created timestamp response generator wrapper</returns>
        ITimeStampTokenGenerator CreateTimeStampTokenGenerator(IPrivateKey pk, IX509Certificate cert,
            string allowedDigest, string policyOid);

        /// <summary>
        /// Create timestamp response generator wrapper from timestamp token generator wrapper and
        /// <see cref="System.Collections.IList"/>
        /// of algorithms.
        /// </summary>
        /// <param name="tokenGenerator">timestamp token generator wrapper to create timestamp response generator wrapper from
        ///     </param>
        /// <param name="algorithms">
        /// 
        /// <see cref="System.Collections.IList"/>
        /// of algorithms to create timestamp response generator wrapper from
        /// </param>
        /// <returns>created timestamp response generator wrapper</returns>
        ITimeStampResponseGenerator CreateTimeStampResponseGenerator(ITimeStampTokenGenerator tokenGenerator, IList algorithms);

        /// <summary>
        /// Create timestamp request wrapper from
        /// <c>byte[]</c>
        /// value.
        /// </summary>
        /// <param name="bytes">
        /// 
        /// <c>byte[]</c>
        /// value to create timestamp request wrapper from
        /// </param>
        /// <returns>created timestamp request wrapper</returns>
        ITimeStampRequest CreateTimeStampRequest(byte[] bytes);

        /// <summary>
        /// Create X500 Name wrapper from
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>.
        /// </summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to create X500 Name wrapper from
        /// </param>
        /// <returns>created X500 Name wrapper</returns>
        IX500Name CreateX500Name(IX509Certificate certificate);

        /// <summary>
        /// Create X500 Name wrapper from
        /// <see cref="System.String"/>.
        /// </summary>
        /// <param name="s">
        /// 
        /// <see cref="System.String"/>
        /// to create X500 Name wrapper from
        /// </param>
        /// <returns>created X500 Name wrapper</returns>
        IX500Name CreateX500Name(String s);

        /// <summary>Create resp ID wrapper from X500 Name wrapper.</summary>
        /// <param name="x500Name">X500 Name wrapper to create resp ID wrapper from</param>
        /// <returns>created resp ID wrapper</returns>
        IRespID CreateRespID(IX500Name x500Name);

        /// <summary>Create basic OCSP Resp builder wrapper from resp ID wrapper.</summary>
        /// <param name="respID">resp ID wrapper to create basic OCSP Resp builder wrapper from</param>
        /// <returns>created basic OCSP Resp builder wrapper</returns>
        IBasicOCSPRespBuilder CreateBasicOCSPRespBuilder(IRespID respID);

        /// <summary>
        /// Create OCSP Req wrapper from
        /// <c>byte[]</c>.
        /// </summary>
        /// <param name="requestBytes">
        /// 
        /// <c>byte[]</c>
        /// to create OCSP Req wrapper from
        /// </param>
        /// <returns>created OCSP Req wrapper</returns>
        IOCSPReq CreateOCSPReq(byte[] requestBytes);

        /// <summary>
        /// Create X509 Version 2 CRL Builder wrapper from X500 Name wrapper and
        /// <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="x500Name">X500 Name wrapper to create X509 Version 2 CRL Builder wrapper from</param>
        /// <param name="thisUpdate">
        /// 
        /// <see cref="System.DateTime"/>
        /// to create X509 Version 2 CRL Builder wrapper from
        /// </param>
        /// <returns>created X509 Version 2 CRL Builder wrapper</returns>
        IX509v2CRLBuilder CreateX509v2CRLBuilder(IX500Name x500Name, DateTime thisUpdate);

        /// <summary>
        /// Create Jca X509 Version 3 certificate builder wrapper from
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// ,
        /// <see cref="iText.Commons.Bouncycastle.Math.IBigInteger"/>
        /// , start
        /// <see cref="System.DateTime"/>
        /// , end
        /// <see cref="System.DateTime"/>
        /// , X500 Name wrapper and
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPublicKey"/>.
        /// </summary>
        /// <param name="signingCert">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to create Jca X509 Version 3 certificate builder wrapper from
        /// </param>
        /// <param name="certSerialNumber">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Math.IBigInteger"/>
        /// to create Jca X509 Version 3 certificate builder wrapper from
        /// </param>
        /// <param name="startDate">
        /// start
        /// <see cref="System.DateTime"/>
        /// to create Jca X509 Version 3 certificate builder wrapper from
        /// </param>
        /// <param name="endDate">
        /// end
        /// <see cref="System.DateTime"/>
        /// to create Jca X509 Version 3 certificate builder wrapper from
        /// </param>
        /// <param name="subjectDnName">X500 Name wrapper to create Jca X509 Version 3 certificate builder wrapper from
        ///     </param>
        /// <param name="publicKey">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPublicKey"/>
        /// to create Jca X509 Version 3 certificate builder wrapper from
        /// </param>
        /// <returns>created Jca X509 Version 3 certificate builder wrapper</returns>
        IJcaX509v3CertificateBuilder CreateJcaX509v3CertificateBuilder(IX509Certificate signingCert, IBigInteger certSerialNumber
            , DateTime startDate, DateTime endDate, IX500Name subjectDnName, IPublicKey publicKey);

        /// <summary>
        /// Create basic constraints wrapper from
        /// <c>boolean</c>
        /// value.
        /// </summary>
        /// <param name="b">
        /// 
        /// <c>boolean</c>
        /// value to create basic constraints wrapper from
        /// </param>
        /// <returns>created basic constraints wrapper</returns>
        IBasicConstraints CreateBasicConstraints(bool b);

        /// <summary>Create key usage wrapper without parameters.</summary>
        /// <returns>created key usage wrapper</returns>
        IKeyUsage CreateKeyUsage();

        /// <summary>
        /// Create key usage wrapper from
        /// <c>int</c>
        /// value.
        /// </summary>
        /// <param name="i">
        /// 
        /// <c>int</c>
        /// value to create key usage wrapper from
        /// </param>
        /// <returns>created key usage wrapper</returns>
        IKeyUsage CreateKeyUsage(int i);

        /// <summary>Create key purpose id wrapper without parameters.</summary>
        /// <returns>created key purpose id wrapper</returns>
        IKeyPurposeId CreateKeyPurposeId();

        /// <summary>Create extended key usage wrapper from key purpose id wrapper.</summary>
        /// <param name="purposeId">key purpose id wrapper to create extended key usage wrapper from</param>
        /// <returns>created extended key usage wrapper</returns>
        IExtendedKeyUsage CreateExtendedKeyUsage(IKeyPurposeId purposeId);

        /// <summary>
        /// Create subject public key info wrapper from public key wrapper
        /// </summary>
        /// <param name="publicKey">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IPublicKey"/>
        /// to create subject public ket info wrapper from
        /// </param>
        /// <returns>created subject public ket info wrapper</returns>
        ISubjectPublicKeyInfo CreateSubjectPublicKeyInfo(IPublicKey publicKey);

        /// <summary>Create CRL Reason wrapper without parameters.</summary>
        /// <returns>created CRL Reason wrapper</returns>
        ICRLReason CreateCRLReason();

        /// <summary>Create TST Info wrapper from content info wrapper.</summary>
        /// <param name="contentInfo">content info wrapper to create TST Info wrapper from</param>
        /// <returns>created TST Info wrapper</returns>
        ITSTInfo CreateTSTInfo(IContentInfo contentInfo);

        /// <summary>Create single resp wrapper from basic OCSP Response wrapper.</summary>
        /// <param name="basicResp">basic OCSP Response wrapper to create single resp wrapper from</param>
        /// <returns>created single resp wrapper</returns>
        ISingleResp CreateSingleResp(IBasicOCSPResponse basicResp);
        
        /// <summary>
        /// Create X509 Certificate wrapper from
        /// <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="s">
        ///
        /// <see cref="System.IO.Stream"/>
        /// to create X509 Certificate wrapper from
        /// </param>
        /// <returns>created X509 Certificate wrapper</returns>
        IX509Certificate CreateX509Certificate(Stream s);
        
        /// <summary>
        /// Create X509 Crl wrapper from
        /// <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="input">
        ///
        /// <see cref="System.IO.Stream"/>
        /// to create X509 Crl wrapper from
        /// </param>
        /// <returns>created X509 Crl wrapper</returns>
        IX509Crl CreateX509Crl(Stream input);
        
        /// <summary>
        /// Create digest wrapper from
        /// <see cref="System.string"/>
        /// hash algorithm.
        /// </summary>
        /// <param name="hashAlgorithm">
        ///
        /// <see cref="System.string"/>
        /// hash algorithm to create digest wrapper from
        /// </param>
        /// <returns>created digest wrapper</returns>
        IIDigest CreateIDigest(string hashAlgorithm);
        
        /// <summary>
        /// Create certificate ID wrapper from
        /// <see cref="System.string"/>
        /// hash algorithm, X509 Certificate wrapper and big integer wrapper.
        /// </summary>
        /// <param name="hashAlgorithm">
        ///
        /// <see cref="System.string"/>
        /// hash algorithm to create certificate ID wrapper from
        /// </param>
        /// <param name="issuerCert">X509 Certificate wrapper to create certificate ID wrapper from</param>
        /// <param name="serialNumber">big integer wrapper to create certificate ID wrapper from</param>
        /// <returns>created certificate ID wrapper</returns>
        ICertificateID CreateCertificateID(string hashAlgorithm, IX509Certificate issuerCert, IBigInteger serialNumber);
        
        /// <summary>Create X500 Name wrapper from ASN1 Encodable wrapper using getInstance call.</summary>
        /// <param name="issuer">ASN1 Encodable wrapper to create X500 Name wrapper from</param>
        /// <returns>created X500 Name wrapper</returns>
        IX500Name CreateX500NameInstance(IASN1Encodable issuer);
        
        /// <summary>
        /// Create OCSP Req wrapper from certificate ID wrapper and
        /// <c>byte[]</c>
        /// document id.
        /// </summary>
        /// <param name="certId">certificate ID wrapper to create OCSP Req wrapper from</param>
        /// <param name="documentId">
        ///
        /// <c>byte[]</c>
        /// document id to create OCSP Req wrapper from
        /// </param>
        /// <returns>created OCSP Req wrapper</returns>
        IOCSPReq CreateOCSPReq(ICertificateID certId, byte[] documentId);
        
        /// <summary>Create signer wrapper without parameters.</summary>
        /// <returns>created signer wrapper</returns>
        IISigner CreateISigner();

        /// <summary>Create X509 Certificate parser wrapper without parameters.</summary>
        /// <returns>created X509 Certificate parser wrapper</returns>
        IX509CertificateParser CreateX509CertificateParser();

        /// <summary>
        /// Create general security exception wrapper from
        /// <see cref="System.string"/>
        /// exception message and
        /// <see cref="System.Exception"/>
        /// exception.
        /// </summary>
        /// <param name="exceptionMessage">
        ///
        /// <see cref="System.string"/>
        /// exception message to create general security exception wrapper from
        /// </param>
        /// <param name="exception">
        ///
        /// <see cref="System.Exception"/>
        /// exception to create general security exception wrapper from
        /// </param>
        /// <returns>created general security exception wrapper</returns>
        AbstractGeneralSecurityException CreateGeneralSecurityException(string exceptionMessage, Exception exception);
        
        /// <summary>
        /// Create general security exception wrapper from
        /// <see cref="System.string"/>
        /// exception message.
        /// </summary>
        /// <param name="exceptionMessage">
        ///
        /// <see cref="System.string"/>
        /// exception message to create general security exception wrapper from
        /// </param>
        /// <returns>created general security exception wrapper</returns>
        AbstractGeneralSecurityException CreateGeneralSecurityException(string exceptionMessage);

        /// <summary>Create general security exception wrapper without parameters.</summary>
        /// <returns>created general security exception wrapper</returns>
        AbstractGeneralSecurityException CreateGeneralSecurityException();

        /// <summary>
        /// Cast
        /// <see cref="System.Object"/>
        /// element to
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>.
        /// </summary>
        /// <param name="element">
        /// 
        /// <see cref="System.Object"/>
        /// to be cast
        /// </param>
        /// <returns>
        /// casted
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// </returns>
        IX509Certificate CreateX509Certificate(Object element);

        /// <summary>
        /// Get
        /// <see cref="IBouncyCastleTestConstantsFactory"/>
        /// corresponding to this
        /// <see cref="IBouncyCastleFactory"/>.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IBouncyCastleTestConstantsFactory"/>
        /// instance
        /// </returns>
        IBouncyCastleTestConstantsFactory GetBouncyCastleFactoryTestUtil();
        
        /// <summary>Create big integer wrapper without parameters.</summary>
        /// <returns>created big integer wrapper</returns>
        IBigInteger CreateBigInteger();
        
        /// <summary>
        /// Create big integer wrapper from
        /// <c>int</c>
        /// value and
        /// <c>byte[]</c>
        /// array.
        /// </summary>
        /// <param name="i">
        ///
        /// <c>int</c>
        /// value to create big integer wrapper from
        /// </param>
        /// <param name="array">
        ///
        /// <c>byte[]</c>
        /// array to create big integer wrapper from
        /// </param>
        /// <returns>created big integer wrapper</returns>
        IBigInteger CreateBigInteger(int i, byte[] array);

        /// <summary>
        /// Create big integer wrapper from
        /// <see cref="System.string"/>
        /// value.
        /// </summary>
        /// <param name="str">
        ///
        /// <see cref="System.string"/>
        /// value to create big integer wrapper from
        /// </param>
        /// <returns>created big integer wrapper</returns>
        IBigInteger CreateBigInteger(string str);
        
        /// <summary>
        /// Create cipher wrapper from
        /// <c>bool</c>
        /// value,
        /// <c>byte[]</c>
        /// key and
        /// <c>byte[]</c>
        /// value.
        /// </summary>
        /// <param name="forEncryption">
        ///
        /// <c>bool</c>
        /// value to create cipher wrapper from
        /// </param>
        /// <param name="key">
        ///
        /// <c>byte[]</c>
        /// key to create cipher wrapper from
        /// </param>
        /// <param name="iv">
        ///
        /// <c>byte[]</c>
        /// value to create cipher wrapper from
        /// </param>
        /// <returns>created cipher wrapper</returns>
        ICipher CreateCipher(bool forEncryption, byte[] key, byte[] iv);

        /// <summary>
        /// Create cipher Cbc no pad wrapper from
        /// <c>bool</c>
        /// value,
        /// <c>byte[]</c>
        /// key and
        /// <c>byte[]</c>
        /// value.
        /// </summary>
        /// <param name="forEncryption">
        ///
        /// <c>bool</c>
        /// value to create cipher Cbc no pad wrapper from
        /// </param>
        /// <param name="key">
        ///
        /// <c>byte[]</c>
        /// key to create cipher Cbc no pad wrapper from
        /// </param>
        /// <param name="iv">
        ///
        /// <c>byte[]</c>
        /// value to create cipher Cbc no pad wrapper from
        /// </param>
        /// <returns>created cipher Cbc no pad wrapper</returns>
        ICipherCBCnoPad CreateCipherCbCnoPad(bool forEncryption, byte[] key, byte[] iv);
        
        /// <summary>
        /// Create cipher Cbc no pad wrapper from
        /// <c>bool</c>
        /// value and 
        /// <c>byte[]</c>
        /// key.
        /// </summary>
        /// <param name="forEncryption">
        ///
        /// <c>bool</c>
        /// value to create cipher Cbc no pad wrapper from
        /// </param>
        /// <param name="key">
        ///
        /// <c>byte[]</c>
        /// key to create cipher Cbc no pad wrapper from
        /// </param>
        /// <returns>created cipher Cbc no pad wrapper</returns>
        ICipherCBCnoPad CreateCipherCbCnoPad(bool forEncryption, byte[] key);

        /// <summary>
        /// Create
        /// <see langword="null"/>
        /// as
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Crl"/>
        /// object.
        /// </summary>
        /// <returns>
        /// 
        /// <see langword="null"/>
        /// as
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Crl"/>
        /// object
        /// </returns>
        IX509Crl CreateNullCrl();

        /// <summary>Create Rsa 2048 key pair generator wrapper without parameters.</summary>
        /// <returns>created Rsa 2048 key pair generator wrapper</returns>
        IRsaKeyPairGenerator CreateRsa2048KeyPairGenerator();

        /// <summary>
        /// Create PEM Parser wrapper from
        /// <see cref="System.IO.TextReader"/>.
        /// </summary>
        /// <param name="reader">
        /// 
        /// <see cref="System.IO.TextReader"/>
        /// to create PEM Parser wrapper from
        /// </param>
        /// <param name="password">
        ///
        /// <c>byte[]</c>
        /// password to read pem file
        /// </param>
        /// <returns>created PEM Parser wrapper</returns>
        IPEMParser CreatePEMParser(TextReader reader, char[] password);
        
        /// <summary>
        /// Create content signer wrapper from
        /// <see cref="System.string"/>
        /// signature algorithm and private key wrapper.
        /// </summary>
        /// <param name="signatureAlgorithm">
        ///
        /// <see cref="System.string"/>
        /// signature algorithm to create content signer wrapper from
        /// </param>
        /// <param name="signingKey">private key wrapper to create content signer wrapper from</param>
        /// <returns>created content signer wrapper</returns>
        IContentSigner CreateContentSigner(string signatureAlgorithm, IPrivateKey signingKey);
        
        /// <summary>Create authority key identifier wrapper from subject public key info wrapper.</summary>
        /// <param name="issuerPublicKeyInfo">
        /// subject public key info wrapper to create authority key identifier wrapper from
        /// </param>
        /// <returns>created authority key identifier wrapper</returns>
        IAuthorityKeyIdentifier CreateAuthorityKeyIdentifier(ISubjectPublicKeyInfo issuerPublicKeyInfo);
        
        /// <summary>
        /// Create subject key identifier from subject public key info wrapper.
        /// </summary>
        /// <param name="subjectPublicKeyInfo">
        /// subject public key info wrapper to create subject key identifier from
        /// </param>
        /// <returns>created subject key identifier</returns>
        ISubjectKeyIdentifier CreateSubjectKeyIdentifier(ISubjectPublicKeyInfo subjectPublicKeyInfo);
        
        /// <summary>
        /// Create extension wrapper from
        /// <c>bool</c>
        /// value and DER Octet string wrapper.
        /// </summary>
        /// <param name="b">
        ///
        /// <c>bool</c>
        /// value to create extension wrapper from
        /// </param>
        /// <param name="octetString">DER Octet string wrapper to create extension wrapper from</param>
        /// <returns>created extension wrapper</returns>
        IExtension CreateExtension(bool b, IDEROctetString octetString);

        /// <summary>
        /// Checks if provided extension wrapper wraps
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="extNonce">extension wrapper to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if provided extension wrapper wraps
        /// <see langword="null"/>
        /// ,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        bool IsNullExtension(IExtension extNonce);
        
        /// <summary>
        /// Create
        /// <c>byte[]</c>
        /// cipher bytes from X509 Certificate wrapper,
        /// <c>byte[]</c>
        /// array and algorithm identifier wrapper.
        /// </summary>
        /// <param name="x509Certificate">X509 Certificate wrapper to create cipher bytes from</param>
        /// <param name="abyte0">
        ///
        /// <c>byte[]</c>
        /// array to create cipher bytes from
        /// </param>
        /// <param name="algorithmidentifier">algorithm identifier wrapper to create cipher bytes from</param>
        /// <returns>created cipher bytes</returns>
        byte[] CreateCipherBytes(IX509Certificate x509Certificate, byte[] abyte0, IAlgorithmIdentifier algorithmidentifier);

        /// <summary>
        /// Check if this bouncy-castle corresponding to this factory is in approved mode.
        /// </summary>
        /// <returns>
        ///
        /// <c>true</c>
        /// if approved mode is enabled,
        /// <c>false</c>
        /// otherwise
        /// </returns>
        bool IsInApprovedOnlyMode();

        /// <summary>
        /// Checks whether an algorithm is supported for encryption by the chosen Bouncy Castle implementation,
        /// throws an exception when not supported.
        /// </summary>
        /// <param name="encryptionType"> the type of encryption
        ///             STANDARD_ENCRYPTION_40 = 2
        ///             STANDARD_ENCRYPTION_128 = 3
        ///             AES_128 = 4
        ///             AES_256 = 5</param>
        /// <param name="withCertificate"> true when used with a certificate, false otherwise</param>        
        void IsEncryptionFeatureSupported(int encryptionType, bool withCertificate);
    }
}
