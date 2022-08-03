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
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;
using Org.BouncyCastle.Security;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Security;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Crypto {
    /// <summary>Creates an AES Cipher with CBC and no padding.</summary>
    /// <author>Paulo Soares</author>
    public class AESCipherCBCnoPad {
        private const String CIPHER_WITHOUT_PADDING = "AES/CBC/NoPadding";

        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private static Cipher cipher;

        static AESCipherCBCnoPad() {
            try {
                cipher = Cipher.GetInstance(CIPHER_WITHOUT_PADDING, BOUNCY_CASTLE_FACTORY.CreateProvider());
            }
            catch (SecurityUtilityException e) {
                throw new PdfException(KernelExceptionMessageConstant.ERROR_WHILE_INITIALIZING_AES_CIPHER, e);
            }
            catch (NoSuchPaddingException e) {
                throw new PdfException(KernelExceptionMessageConstant.ERROR_WHILE_INITIALIZING_AES_CIPHER, e);
            }
        }

        /// <summary>Creates a new instance of AESCipher with CBC and no padding</summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        public AESCipherCBCnoPad(bool forEncryption, byte[] key)
            : this(forEncryption, key, new byte[16]) {
        }

        /// <summary>Creates a new instance of AESCipher with CBC and no padding</summary>
        /// <param name="forEncryption">
        /// if true the cipher is initialised for
        /// encryption, if false for decryption
        /// </param>
        /// <param name="key">the key to be used in the cipher</param>
        /// <param name="initVector">initialization vector to be used in cipher</param>
        public AESCipherCBCnoPad(bool forEncryption, byte[] key, byte[] initVector) {
            try {
                cipher.Init(forEncryption ? Cipher.ENCRYPT_MODE : Cipher.DECRYPT_MODE, new SecretKeySpec(key, "AES"), new 
                    IvParameterSpec(initVector));
            }
            catch (InvalidKeyException e) {
                throw new PdfException(KernelExceptionMessageConstant.ERROR_WHILE_INITIALIZING_AES_CIPHER, e);
            }
            catch (InvalidAlgorithmParameterException e) {
                throw new PdfException(KernelExceptionMessageConstant.ERROR_WHILE_INITIALIZING_AES_CIPHER, e);
            }
        }

        public virtual byte[] ProcessBlock(byte[] inp, int inpOff, int inpLen) {
            return cipher.Update(inp, inpOff, inpLen);
        }
    }
}
