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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Crypto {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that it's API and functionality may be changed in future.
    /// </summary>
    public static class CryptoUtil {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
        
        public static IX509Certificate ReadPublicCertificate(Stream s) {
            return FACTORY.CreateX509Certificate(s);
        }

        /// <summary>
        /// Creates <see cref="DerOutputStream"/> instance which can be an implementation of one of the ASN1 encodings
        /// writing logic. This method also asserts for unexpected ASN1 encodings.
        /// </summary>
        /// <param name="outputStream">the underlying stream</param>
        /// <param name="asn1Encoding">ASN1 encoding that will be used for writing. Only DER and BER are allowed as values.</param>
        /// <returns>a <see cref="DerOutputStream"/> instance. Exact stream implementation is chosen based on passed encoding.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public static IDerOutputStream CreateAsn1OutputStream(Stream outputStream, String asn1Encoding) {
            if (FACTORY.CreateASN1Encoding().GetBer().Equals(asn1Encoding) || 
                FACTORY.CreateASN1Encoding().GetDer().Equals(asn1Encoding)) {
                return FACTORY.CreateASN1OutputStream(outputStream, asn1Encoding);
            }

            throw new NotSupportedException(
                MessageFormatUtil.Format(KernelExceptionMessageConstant.UNSUPPORTED_ASN1_ENCODING, asn1Encoding)
            );
        }
        
        internal static IMessageDigest GetMessageDigest(String hashAlgorithm) {
            return FACTORY.CreateIDigest(hashAlgorithm);
        }
    }
}
