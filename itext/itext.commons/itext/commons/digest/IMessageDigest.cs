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
namespace iText.Commons.Digest {
    /// <summary>
    /// This interface should be implemented to provide applications the functionality of a message digest algorithm.
    /// </summary>
    public interface IMessageDigest {
        /// <summary>
        /// Performs a final update on the digest using the specified array of bytes,
        /// then completes the digest computation.
        /// </summary>
        /// <param name="enc">the input to be updated before the digest is completed</param>
        /// <returns>The array of bytes for the resulting hash value</returns>
        byte[] Digest(byte[] enc);
        
        /// <summary>
        /// Completes the hash computation by performing final operations such as padding.
        /// Leaves the digest reset.
        /// </summary>
        /// <returns>The array of bytes for the resulting hash value</returns>
        byte[] Digest();

        /// <summary>
        /// Gets byte length of wrapped digest algorithm.
        /// </summary>
        /// <returns>The length of the digest in bytes.</returns>
        int GetDigestLength();
        
        /// <summary>
        /// Updates the digest using the specified array of bytes, starting at the specified offset.
        /// </summary>
        /// <param name="buf">byte array buffer</param>
        /// <param name="off">the offset to start from in the array of bytes</param>
        /// <param name="len">the number of bytes to use, starting at offset</param>
        void Update(byte[] buf, int off, int len);
        
        /// <summary>
        /// Updates the digest using the specified array of bytes.
        /// </summary>
        /// <param name="buf">byte array buffer</param>
        void Update(byte[] buf);

        /// <summary>Resets the digest for further use.</summary>
        void Reset();

        /// <summary>
        /// Returns a string that identifies the algorithm, independent of implementation details.
        /// </summary>
        /// <returns>The name of the algorithm.</returns>
        string GetAlgorithmName();
    }
}