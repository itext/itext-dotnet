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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Kernel.Exceptions;

namespace iText.Signatures {
    /// <summary>
    /// An implementation of the CrlClient that handles offline
    /// Certificate Revocation Lists.
    /// </summary>
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
