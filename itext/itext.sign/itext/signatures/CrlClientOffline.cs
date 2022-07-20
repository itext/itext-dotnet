/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Kernel.Exceptions;

namespace iText.Signatures {
    /// <summary>
    /// An implementation of the CrlClient that handles offline
    /// Certificate Revocation Lists.
    /// </summary>
    /// <author>Paulo Soares</author>
    public class CrlClientOffline : ICrlClient {
        /// <summary>The CRL as a byte array.</summary>
        private IList<byte[]> crls = new List<byte[]>();

        /// <summary>
        /// Creates an instance of a CrlClient in case you
        /// have a local cache of the Certificate Revocation List.
        /// </summary>
        /// <param name="crlEncoded">the CRL bytes</param>
        public CrlClientOffline(byte[] crlEncoded) {
            crls.Add(crlEncoded);
        }

        /// <summary>
        /// Creates an instance of a CrlClient in case you
        /// have a local cache of the Certificate Revocation List.
        /// </summary>
        /// <param name="crl">a CRL object</param>
        public CrlClientOffline(IX509Crl crl) {
            try {
                crls.Add(((IX509Crl)crl).GetEncoded());
            }
            catch (Exception ex) {
                throw new PdfException(ex);
            }
        }

        /// <summary>Returns the CRL bytes (the parameters are ignored).</summary>
        /// <seealso cref="ICrlClient.GetEncoded(iText.Commons.Bouncycastle.Cert.IX509Certificate, System.String)"/>
        public virtual ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url) {
            return crls;
        }
    }
}
