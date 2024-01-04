/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
namespace iText.Commons.Bouncycastle.Crypto {
    /// <summary>
    /// This interface represents the wrapper for IDigest that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IDigest {
        /// <summary>
        /// Calls actual
        /// <c>Digest</c>
        /// method for the wrapped IDigest object.
        /// </summary>
        /// <param name="enc2">byte array</param>
        /// <returns>
        /// byte array.
        /// </returns>
        byte[] Digest(byte[] enc2);
        
        /// <summary>
        /// Calls actual
        /// <c>Digest</c>
        /// method for the wrapped IDigest object.
        /// Leaves the digest reset.
        /// </summary>
        /// <returns>
        /// byte array.
        /// </returns>
        byte[] Digest();

        /// <summary>
        /// Gets byte length of wrapped digest algorithm.
        /// </summary>
        /// <returns>digest length</returns>
        int GetDigestLength();
        
        /// <summary>
        /// Calls actual
        /// <c>Update</c>
        /// method for the wrapped IDigest object.
        /// </summary>
        /// <param name="buf">byte array buffer</param>
        /// <param name="off">offset</param>
        /// <param name="len">buffer length</param>
        void Update(byte[] buf, int off, int len);
        
        /// <summary>
        /// Calls actual
        /// <c>Update</c>
        /// method for the wrapped IDigest object.
        /// </summary>
        /// <param name="buf">byte array buffer</param>
        void Update(byte[] buf);

        /// <summary>
        /// Calls actual
        /// <c>Reset</c>
        /// method for the wrapped IDigest object.
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets actual
        /// <c>AlgorithmName</c>
        /// for the wrapped IDigest object.
        /// </summary>
        /// <returns>algorithm name.</returns>
        string GetAlgorithmName();
    }
}
