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
using iText.Bouncycastlefips.Asn1.Ocsp;
using Org.BouncyCastle.Cert;
using iText.Bouncycastlefips.Asn1.X509;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;

namespace iText.Bouncycastlefips.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for generator for basic OCSP response objects.
    /// </summary>
    public class BasicOcspRespGeneratorBCFips : IBasicOcspRespGenerator {
        private readonly IList list = new ArrayList();
        private X509Extensions responseExtensions;
        private ResponderID responderID;

        /// <summary>
        /// Creates new basic OCSP response objects generator wrapper instance.
        /// </summary>
        /// <param name="list">single responses array</param>
        /// <param name="responseExtensions">X509Extensions</param>
        /// <param name="responderID">resp ID</param>
        public BasicOcspRespGeneratorBCFips(IList list, X509Extensions responseExtensions, ResponderID responderID) {
            this.list = list;
            this.responseExtensions = responseExtensions;
            this.responderID = responderID;
        }

        /// <summary>
        /// Creates new basic OCSP response objects generator wrapper instance.
        /// </summary>
        /// <param name="respID">RespID wrapper to create generator</param>
        public BasicOcspRespGeneratorBCFips(IRespID respID) {
            responderID = ((RespIDBCFips)respID).GetRespID();
        }

        /// <summary>Gets single responses list being wrapped.</summary>
        /// <returns>List field.</returns>
        public virtual IList GetList() {
            return list;
        }
        
        /// <summary>Gets responseExtensions being wrapped.</summary>
        /// <returns>ResponseExtensions field.</returns>
        public virtual X509Extensions GetResponseExtensions() {
            return responseExtensions;
        }
        
        /// <summary>Gets responderID being wrapped.</summary>
        /// <returns>ResponderID field.</returns>
        public virtual ResponderID GetResponderID() {
            return responderID;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOcspRespGenerator SetResponseExtensions(IX509Extensions extensions) {
            responseExtensions = ((X509ExtensionsBCFips)extensions).GetX509Extensions();
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOcspRespGenerator AddResponse(ICertID certID, ICertStatus certificateStatus, 
            DateTime time, DateTime time1, IX509Extensions extensions) {
            list.Add(new SingleResponse(((CertIDBCFips)certID).GetCertificateID(), ((CertStatusBCFips
                )certificateStatus).GetCertStatus(), new DerGeneralizedTime(time), 
                new DerGeneralizedTime(time1), ((X509ExtensionsBCFips)extensions).GetX509Extensions()));
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IBasicOcspResponse Build(IContentSigner signer, IX509Certificate[] wrappersChain,
            DateTime producedAt) {
            X509Certificate[] chain = new X509Certificate[wrappersChain.Length];
            for (int i = 0; i < wrappersChain.Length; ++i) {
                chain[i] = ((X509CertificateBCFips)wrappersChain[i]).GetCertificate();
            }
            ISignatureFactory<AlgorithmIdentifier> signatureCalculator =
                ((ContentSignerBCFips)signer).GetContentSigner();
            if (signatureCalculator == null) {
                throw new ArgumentException("no signature calculator specified");
            }
            AlgorithmIdentifier signingAlgID = signatureCalculator.AlgorithmDetails;
            DerObjectIdentifier signingAlgorithm = signingAlgID.Algorithm;
            Asn1EncodableVector responses = new Asn1EncodableVector();
            foreach (SingleResponse respObj in list) {
                try {
                    responses.Add(respObj);
                } catch (Exception e) {
                    throw new OcspExceptionBCFips("exception creating Request" + e);
                }
            }
            ResponseData tbsResp = new ResponseData(responderID, new DerGeneralizedTime(producedAt),
                new DerSequence(responses), responseExtensions);
            DerBitString bitSig;
            try {
                IStreamCalculator<IBlockResult> streamCalculator = signatureCalculator.CreateCalculator();
                byte[] encoded = tbsResp.GetDerEncoded();
                using (Stream stream = streamCalculator.Stream) {
                    stream.Write(encoded, 0, encoded.Length);
                    bitSig = new DerBitString(streamCalculator.GetResult().Collect());
                }
            } catch (Exception e) {
                throw new OcspExceptionBCFips("exception processing TBSRequest: " + e);
            }
            AlgorithmIdentifier sigAlgId = new AlgorithmIdentifier(signingAlgorithm, DerNull.Instance);
            DerSequence chainSeq = null;
            if (chain.Length > 0) {
                Asn1EncodableVector v = new Asn1EncodableVector();
                try {
                    for (int i = 0; i != chain.Length; i++) {
                        v.Add(X509CertificateStructure.GetInstance(Asn1Object.FromByteArray(chain[i].GetEncoded())));
                    }
                } catch (IOException e) {
                    throw new OcspExceptionBCFips("error processing certs" + e);
                } catch (CertificateEncodingException e) {
                    throw new OcspExceptionBCFips("error encoding certs" + e);
                }
                chainSeq = new DerSequence(v);
            }
            return new BasicOcspResponseBCFips(new BasicOcspResponse(tbsResp, sigAlgId, bitSig, chainSeq));
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
            iText.Bouncycastlefips.Cert.Ocsp.BasicOcspRespGeneratorBCFips that = (iText.Bouncycastlefips.Cert.Ocsp.BasicOcspRespGeneratorBCFips
                )o;
            return Object.Equals(list, that.list) &&
                   Object.Equals(responseExtensions, that.responseExtensions) &&
                   Object.Equals(responderID, that.responderID);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode<object>(list, responseExtensions, responderID);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return list + " " + responseExtensions + " " + responderID;
        }
    }
}
