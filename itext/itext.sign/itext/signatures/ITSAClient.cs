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
using iText.Commons.Bouncycastle.Crypto;

namespace iText.Signatures {
    /// <summary>Time Stamp Authority client (caller) interface.</summary>
    /// <remarks>
    /// Time Stamp Authority client (caller) interface.
    /// <para />
    /// Interface used by the PdfPKCS7 digital signature builder to call
    /// Time Stamp Authority providing RFC 3161 compliant time stamp token.
    /// </remarks>
    public interface ITSAClient {
        /// <summary>Get the time stamp estimated token size.</summary>
        /// <remarks>
        /// Get the time stamp estimated token size.
        /// Implementation must return value large enough to accommodate the
        /// entire token returned by
        /// <see cref="GetTimeStampToken(byte[])"/>
        /// prior
        /// to actual
        /// <see cref="GetTimeStampToken(byte[])"/>
        /// call.
        /// </remarks>
        /// <returns>an estimate of the token size</returns>
        int GetTokenSizeEstimate();

        /// <summary>
        /// Returns the
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IIDigest"/>
        /// to digest the data imprint
        /// </summary>
        /// <returns>
        /// The
        /// <see cref="iText.Commons.Bouncycastle.Crypto.IIDigest"/>
        /// object.
        /// </returns>
        IIDigest GetMessageDigest();

        /// <summary>Returns RFC 3161 timeStampToken.</summary>
        /// <param name="imprint">byte[] - data imprint to be time-stamped</param>
        /// <returns>byte[] - encoded, TSA signed data of the timeStampToken</returns>
        byte[] GetTimeStampToken(byte[] imprint);
    }
}
