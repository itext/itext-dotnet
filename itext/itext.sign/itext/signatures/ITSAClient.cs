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
using iText.Commons.Digest;

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
        /// <see cref="iText.Commons.Digest.IMessageDigest"/>
        /// to digest the data imprint
        /// </summary>
        /// <returns>
        /// The
        /// <see cref="iText.Commons.Digest.IMessageDigest"/>
        /// object.
        /// </returns>
        IMessageDigest GetMessageDigest();

        /// <summary>Returns RFC 3161 timeStampToken.</summary>
        /// <param name="imprint">byte[] - data imprint to be time-stamped</param>
        /// <returns>byte[] - encoded, TSA signed data of the timeStampToken</returns>
        byte[] GetTimeStampToken(byte[] imprint);
    }
}
