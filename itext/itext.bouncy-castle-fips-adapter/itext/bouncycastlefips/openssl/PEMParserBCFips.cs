/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Asymmetric;
using iText.Bouncycastlefips.Cert;
using iText.Bouncycastlefips.Crypto;
using iText.Commons.Bouncycastle.Openssl;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Cert;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Operators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;

namespace iText.Bouncycastlefips.Openssl {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.OpenSsl.OpenSslPemReader"/>.
    /// </summary>
    public class PEMParserBCFips : IPemReader {
        private readonly OpenSslPemReader parser;

        private readonly char[] password;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.OpenSsl.OpenSslPemReader"/>.
        /// </summary>
        /// <param name="parser">
        /// 
        /// <see cref="Org.BouncyCastle.OpenSsl.OpenSslPemReader"/>
        /// to be wrapped
        /// </param>
        public PEMParserBCFips(OpenSslPemReader parser) {
            this.parser = parser;
        }
        
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.OpenSsl.OpenSslPemReader"/>.
        /// </summary>
        /// <param name="parser">
        /// 
        /// <see cref="Org.BouncyCastle.OpenSsl.OpenSslPemReader"/>
        /// to be wrapped
        /// </param>
        /// <param name="password">
        /// password to use during decryption
        /// </param>
        public PEMParserBCFips(OpenSslPemReader parser, char[] password) : this(parser) {
            this.password = password;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.OpenSsl.OpenSslPemReader"/>.
        /// </returns>
        public virtual OpenSslPemReader GetParser() {
            return parser;
        }

        /// <summary><inheritDoc/></summary>
        public virtual Object ReadObject() {
            Object readObject = parser.ReadObject();
            if (readObject is X509Certificate) {
                return new X509CertificateBCFips((X509Certificate)readObject);
            }

            if (readObject is PrivateKeyInfo) {
                PrivateKeyInfo privateKeyInfo = (PrivateKeyInfo)readObject;
                return new PrivateKeyBCFips(AsymmetricKeyFactory.CreatePrivateKey(privateKeyInfo.GetEncoded()));
            }
            if (readObject is Pkcs8EncryptedPrivateKeyInfo) {
                if (password == null) {
                    return new PrivateKeyBCFips(null);
                }
                return new PrivateKeyBCFips(GeneratePrivateKey((Pkcs8EncryptedPrivateKeyInfo)readObject, password));
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
            iText.Bouncycastlefips.Openssl.PEMParserBCFips that = (iText.Bouncycastlefips.Openssl.PEMParserBCFips)o;
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
        
        private static IAsymmetricPrivateKey GeneratePrivateKey(
            Pkcs8EncryptedPrivateKeyInfo encryptedPrivateKeyInfo, char[] password) {
            PrivateKeyInfo privateKeyInfo = encryptedPrivateKeyInfo.DecryptPrivateKeyInfo(
                new PkixPbeDecryptorProviderBuilder().Build(password));
            return AsymmetricKeyFactory.CreatePrivateKey(privateKeyInfo.GetEncoded());
        }
    }
}
