using System;
using System.Collections;
using iText.Bouncycastle.Asn1;
using iText.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;
using Org.BouncyCastle.Ocsp;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;

namespace iText.Bouncycastle.Cert.Ocsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>.
    /// </summary>
    public class OCSPReqBC : IOCSPReq {
        private readonly OcspReq ocspReq;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>.
        /// </summary>
        /// <param name="ocspReq">
        /// 
        /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>
        /// to be wrapped
        /// </param>
        public OCSPReqBC(OcspReq ocspReq) {
            this.ocspReq = ocspReq;
        }

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>.
        /// </summary>
        /// <param name="certId">
        /// CertID wrapper
        /// </param>
        /// <param name="documentId">
        /// byte array
        /// </param>
        public OCSPReqBC(ICertificateID certId, byte[] documentId) {
            OcspReqGenerator gen = new OcspReqGenerator();
            gen.AddRequest(new CertificateID(((CertificateIDBC) certId).GetCertificateID()));

            // create details for nonce extension
            IDictionary extensions = new Hashtable();
            DerOctetString derOctetString = new DerOctetString(new DerOctetString(documentId).GetEncoded());
            extensions[OcspObjectIdentifiers.PkixOcspNonce] = new X509Extension(false, derOctetString);

            gen.SetRequestExtensions(new X509Extensions(extensions));
            ocspReq = gen.Generate();
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Ocsp.OcspReq"/>.
        /// </returns>
        public virtual OcspReq GetOcspReq() {
            return ocspReq;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded() {
            return ocspReq.GetEncoded();
        }

        /// <summary><inheritDoc/></summary>
        public IReq[] GetRequestList() {
            Req[] reqs = ocspReq.GetRequestList();
            IReq[] wrappedReqs = new IReq[reqs.Length];
            for (int i = 0; i < reqs.Length; ++i) {
                wrappedReqs[i] = new ReqBC(reqs[i]);
            }
            return wrappedReqs;
        }

        /// <summary><inheritDoc/></summary>
        public IExtension GetExtension(IASN1ObjectIdentifier objectIdentifier) {
            return new ExtensionBC(ocspReq.RequestExtensions.GetExtension(
                ((ASN1ObjectIdentifierBC) objectIdentifier).GetASN1ObjectIdentifier()));
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
            iText.Bouncycastle.Cert.Ocsp.OCSPReqBC ocspReqBC = (iText.Bouncycastle.Cert.Ocsp.OCSPReqBC)o;
            return Object.Equals(ocspReq, ocspReqBC.ocspReq);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(ocspReq);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return ocspReq.ToString();
        }
    }
}
