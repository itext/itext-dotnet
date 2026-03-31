/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.IO;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Crypto.Securityhandler {
//\cond DO_NOT_DOCUMENT
    /// <summary>Creates an AES Cipher with CBC and no padding.</summary>
    internal class AESCipherCBCnoPad {
//\cond DO_NOT_DOCUMENT
        internal iText.Kernel.Crypto.AESCipherCBCnoPad aESCipherCBCnoPad;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates a new instance of AESCipher with CBC and no padding</summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        internal AESCipherCBCnoPad(bool forEncryption, byte[] key) {
            aESCipherCBCnoPad = new iText.Kernel.Crypto.AESCipherCBCnoPad(forEncryption, key);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates a new instance of AESCipher with CBC and no padding</summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        /// <param name="initVector">initialization vector to be used in cipher</param>
        internal AESCipherCBCnoPad(bool forEncryption, byte[] key, byte[] initVector) {
            aESCipherCBCnoPad = new iText.Kernel.Crypto.AESCipherCBCnoPad(forEncryption, key, initVector);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Performs a multiple-part encryption or decryption operation (depending on how this cipher was initialized),
        /// processing another data part.
        /// </summary>
        /// <param name="inp">the input buffer</param>
        /// <param name="inpOff">the offset in input where the input starts</param>
        /// <param name="inpLen">the input length</param>
        internal virtual byte[] ProcessBlock(byte[] inp, int inpOff, int inpLen) {
            return aESCipherCBCnoPad.ProcessBlock(inp, inpOff, inpLen);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Finishes a multiple-part encryption or decryption operation, depending on how this cipher was initialized.
        ///     </summary>
        /// <returns>byte array with the result</returns>
        internal virtual byte[] DoFinal() {
            return aESCipherCBCnoPad.DoFinal();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Performs a multiple-part encryption or decryption operation (depending on how this cipher was initialized),
        /// processing the full block with finalizing.
        /// </summary>
        /// <param name="inp">the input buffer</param>
        /// <param name="inpOff">the offset in input where the input starts</param>
        /// <param name="inpLen">the input length</param>
        internal virtual byte[] ProcessFullBlock(byte[] inp, int inpOff, int inpLen) {
            try {
                MemoryStream ba = new MemoryStream();
                byte[] processRes = ProcessBlock(inp, inpOff, inpLen);
                if (processRes != null) {
                    ba.Write(processRes);
                }
                byte[] doFinalRes = DoFinal();
                if (doFinalRes != null) {
                    ba.Write(doFinalRes);
                }
                return ba.ToArray();
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
        }
//\endcond
    }
//\endcond
}
