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
using System.IO;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    /// <summary>Standard security handler with Advanced Encryption Standard-Galois/Counter Mode (AES-GCM) encryption algorithm.
    ///     </summary>
    public class StandardHandlerUsingAesGcm : StandardHandlerUsingAes256 {
        protected internal byte[] noncePart = null;

        protected internal int inObjectNonceCounter = 0;

        /// <summary>
        /// Creates new
        /// <see cref="StandardHandlerUsingAesGcm"/>
        /// instance for encryption.
        /// </summary>
        /// <param name="encryptionDictionary">document's encryption dictionary</param>
        /// <param name="userPassword">user password</param>
        /// <param name="ownerPassword">owner password</param>
        /// <param name="permissions">access permissions</param>
        /// <param name="encryptMetadata">indicates whether the document-level metadata stream shall be encrypted</param>
        /// <param name="embeddedFilesOnly">indicates whether embedded files shall be encrypted in an otherwise unencrypted document
        ///     </param>
        public StandardHandlerUsingAesGcm(PdfDictionary encryptionDictionary, byte[] userPassword, byte[] ownerPassword
            , int permissions, bool encryptMetadata, bool embeddedFilesOnly)
            : base(encryptionDictionary, userPassword, ownerPassword, permissions, encryptMetadata, embeddedFilesOnly, 
                PdfVersion.PDF_2_0) {
        }

        /// <summary>
        /// Creates new
        /// <see cref="StandardHandlerUsingAesGcm"/>
        /// instance for decryption.
        /// </summary>
        /// <param name="encryptionDictionary">document's encryption dictionary</param>
        /// <param name="password">owner or user password to decrypt the document</param>
        public StandardHandlerUsingAesGcm(PdfDictionary encryptionDictionary, byte[] password)
            : base(encryptionDictionary, password) {
        }

        public override void SetHashKeyForNextObject(int objNumber, int objGeneration) {
            // Make sure the same IV is never used twice in the same file. We do this by turning the objId/objGen into a
            // 5-byte nonce (with generation restricted to 1 byte instead of 2) plus an in-object 2-byte counter that
            // increments each time a new string is encrypted within the same object. The remaining 5 bytes will be
            // generated randomly using a strong PRNG.
            // This is very different from the situation with AES-CBC, where randomness is paramount. GCM uses a variation
            // of counter mode, so making sure the IV is unique is more important than randomness.
            this.inObjectNonceCounter = 0;
            this.noncePart = new byte[] { 0, 0, (byte)(objGeneration), (byte)((int)(((uint)objNumber) >> 24)), (byte)(
                (int)(((uint)objNumber) >> 16)), (byte)((int)(((uint)objNumber) >> 8)), (byte)(objNumber) };
        }

        public override OutputStreamEncryption GetEncryptionStream(Stream os) {
            int ctr = inObjectNonceCounter;
            noncePart[0] = (byte)((int)(((uint)ctr) >> 8));
            noncePart[1] = (byte)ctr;
            return new OutputStreamAesGcmEncryption(os, nextObjectKey, noncePart);
        }

        public override IDecryptor GetDecryptor() {
            return new AesGcmDecryptor(nextObjectKey, 0, nextObjectKeySize);
        }

//\cond DO_NOT_DOCUMENT
        internal override void SetAES256DicEntries(PdfDictionary encryptionDictionary, byte[] oeKey, byte[] ueKey, 
            byte[] aes256Perms, bool encryptMetadata, bool embeddedFilesOnly) {
            int version = 6;
            int revision = 7;
            PdfName cryptoFilter = PdfName.AESV4;
            SetEncryptionDictionaryEntries(encryptionDictionary, oeKey, ueKey, aes256Perms, encryptMetadata, embeddedFilesOnly
                , version, revision, cryptoFilter);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override bool IsPdf2(PdfDictionary encryptionDictionary) {
            return true;
        }
//\endcond

        /// <summary><inheritDoc/></summary>
        protected internal override void InitMd5MessageDigest() {
        }
        //Do nothing to not initialize md5 message digest, since it's not used by AES-GCM handler
    }
}
