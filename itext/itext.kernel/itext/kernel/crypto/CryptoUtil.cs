/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/

using System;
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
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
        public static IASN1OutputStream CreateAsn1OutputStream(Stream outputStream, String asn1Encoding) {
            if (FACTORY.CreateASN1Encoding().GetBer().Equals(asn1Encoding) || 
                FACTORY.CreateASN1Encoding().GetDer().Equals(asn1Encoding)) {
                return FACTORY.CreateASN1OutputStream(outputStream, asn1Encoding);
            }

            throw new NotSupportedException(
                MessageFormatUtil.Format(KernelExceptionMessageConstant.UNSUPPORTED_ASN1_ENCODING, asn1Encoding)
            );
        }


    }
}
