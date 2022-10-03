using System;
using System.Collections.Generic;
using iText.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.X509;
using iText.Commons.Utils;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="X509CertificateParser"/>.
    /// </summary>
    public class X509CertificateParserBC : IX509CertificateParser {
        private readonly X509CertificateParser certificateParser;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="X509CertificateParser"/>.
        /// </summary>
        /// <param name="certificateParser">
        /// <see cref="X509CertificateParser"/>
        /// to be wrapped
        /// </param>
        public X509CertificateParserBC(X509CertificateParser certificateParser) {
            this.certificateParser = certificateParser;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="X509CertificateParser"/>.
        /// </returns>
        public X509CertificateParser GetCertificateParser() {
            return certificateParser;
        }

        /// <summary><inheritDoc/></summary>
        public List<IX509Certificate> ReadAllCerts(byte[] contentsKey) {
            List<IX509Certificate> certs = new List<IX509Certificate>();

            foreach (X509Certificate cc in certificateParser.ReadCertificates(contentsKey)) {
                certs.Add(new X509CertificateBC(cc));
            }
            return certs;
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
            X509CertificateParserBC that = (X509CertificateParserBC)o;
            return Object.Equals(certificateParser, that.certificateParser);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(certificateParser);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return certificateParser.ToString();
        }
    }
}