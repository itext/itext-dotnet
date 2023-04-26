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
    /// This interface represents the wrapper for ISigner that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface ISigner {
        /// <summary>
        /// Calls actual
        /// <c>InitVerify</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="publicKey">public key</param>
        void InitVerify(IPublicKey publicKey);
        
        /// <summary>
        /// Calls actual
        /// <c>InitVerify</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="publicKey">public key</param>
        void InitSign(IPrivateKey key);
        
        /// <summary>
        /// Calls actual
        /// <c>Update</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="buf">byte array buffer</param>
        /// <param name="off">offset</param>
        /// <param name="len">buffer length</param>
        void Update(byte[] buf, int off, int len);
        
        /// <summary>
        /// Calls actual
        /// <c>Update</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="buf">byte array buffer</param>
        void Update(byte[] digest);
        
        /// <summary>
        /// Calls actual
        /// <c>VerifySignature</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="digest">byte array</param>
        /// <returns>boolean value.</returns>
        bool VerifySignature(byte[] digest);
        
        /// <summary>
        /// Calls actual
        /// <c>GenerateSignature</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <returns>byte array.</returns>
        byte[] GenerateSignature();
        
        /// <summary>
        /// Calls actual
        /// <c>UpdateVerifier</c>
        /// method for the wrapped ISigner object.
        /// </summary>
        /// <param name="digest">byte array</param>
        void UpdateVerifier(byte[] digest);

        /// <summary>
        /// Sets hash and encryption algorithms
        /// for the wrapped ISigner object.
        /// </summary>
        /// <param name="algorithm">digest algorithm</param>
        void SetDigestAlgorithm(string algorithm);
    }
}
