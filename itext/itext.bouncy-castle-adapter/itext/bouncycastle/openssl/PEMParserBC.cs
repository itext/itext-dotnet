using System;
using iText.Bouncycastle.Cert;
using iText.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Openssl;
using iText.Commons.Utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace iText.Bouncycastle.Openssl {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Openssl.PEMParser"/>.
    /// </summary>
    public class PEMParserBC : IPEMParser {
        private readonly PemReader parser;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.OpenSsl.OpenSslPemReader"/>.
        /// </summary>
        /// <param name="parser">
        /// 
        /// <see cref="Org.BouncyCastle.OpenSsl.OpenSslPemReader"/>
        /// to be wrapped
        /// </param>
        public PEMParserBC(PemReader parser) {
            this.parser = parser;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.OpenSsl.OpenSslPemReader"/>.
        /// </returns>
        public virtual PemReader GetParser() {
            return parser;
        }

        /// <summary><inheritDoc/></summary>
        public virtual Object ReadObject() {
            Object readObject;
            try {
                readObject = parser.ReadObject();
            }
            catch (PasswordException) {
                return new PrivateKeyBC(null);
            }
            if (readObject is X509Certificate) {
                return new X509CertificateBC((X509Certificate)readObject);
            }
            if (readObject is ICipherParameters) {
                return new PrivateKeyBC((ICipherParameters)readObject);
            }
            return readObject;
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
            iText.Bouncycastle.Openssl.PEMParserBC that = (iText.Bouncycastle.Openssl.PEMParserBC)o;
            return Object.Equals(parser, that.parser);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(parser);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return parser.ToString();
        }
    }
}
