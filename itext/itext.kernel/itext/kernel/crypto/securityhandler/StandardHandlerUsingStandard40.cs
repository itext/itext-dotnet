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
using System;
using System.IO;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    public class StandardHandlerUsingStandard40 : StandardSecurityHandler {
        protected internal static readonly byte[] pad = new byte[] { (byte)0x28, (byte)0xBF, (byte)0x4E, (byte)0x5E
            , (byte)0x4E, (byte)0x75, (byte)0x8A, (byte)0x41, (byte)0x64, (byte)0x00, (byte)0x4E, (byte)0x56, (byte
            )0xFF, (byte)0xFA, (byte)0x01, (byte)0x08, (byte)0x2E, (byte)0x2E, (byte)0x00, (byte)0xB6, (byte)0xD0, 
            (byte)0x68, (byte)0x3E, (byte)0x80, (byte)0x2F, (byte)0x0C, (byte)0xA9, (byte)0xFE, (byte)0x64, (byte)
            0x53, (byte)0x69, (byte)0x7A };

        protected internal static readonly byte[] metadataPad = new byte[] { (byte)255, (byte)255, (byte)255, (byte
            )255 };

        protected internal byte[] documentId;

        // stores key length of the main key
        protected internal int keyLength;

        protected internal ARCFOUREncryption arcfour = new ARCFOUREncryption();

        private const int DEFAULT_KEY_LENGTH = 40;

        public StandardHandlerUsingStandard40(PdfDictionary encryptionDictionary, byte[] userPassword, byte[] ownerPassword
            , int permissions, bool encryptMetadata, bool embeddedFilesOnly, byte[] documentId) {
            InitKeyAndFillDictionary(encryptionDictionary, userPassword, ownerPassword, permissions, encryptMetadata, 
                embeddedFilesOnly, documentId);
        }

        public StandardHandlerUsingStandard40(PdfDictionary encryptionDictionary, byte[] password, byte[] documentId
            , bool encryptMetadata) {
            InitKeyAndReadDictionary(encryptionDictionary, password, documentId, encryptMetadata);
        }

        public override OutputStreamEncryption GetEncryptionStream(Stream os) {
            return new OutputStreamStandardEncryption(os, nextObjectKey, 0, nextObjectKeySize);
        }

        public override IDecryptor GetDecryptor() {
            return new StandardDecryptor(nextObjectKey, 0, nextObjectKeySize);
        }

        public virtual byte[] ComputeUserPassword(byte[] ownerPassword, PdfDictionary encryptionDictionary) {
            byte[] ownerKey = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.O));
            byte[] userPad = ComputeOwnerKey(ownerKey, PadPassword(ownerPassword));
            for (int i = 0; i < userPad.Length; i++) {
                bool match = true;
                for (int j = 0; j < userPad.Length - i; j++) {
                    if (userPad[i + j] != pad[j]) {
                        match = false;
                        break;
                    }
                }
                if (!match) {
                    continue;
                }
                byte[] userPassword = new byte[i];
                Array.Copy(userPad, 0, userPassword, 0, i);
                return userPassword;
            }
            return userPad;
        }

        protected internal virtual void CalculatePermissions(int permissions) {
            permissions |= PERMS_MASK_1_FOR_REVISION_2;
            permissions &= PERMS_MASK_2;
            this.permissions = permissions;
        }

        protected internal virtual byte[] ComputeOwnerKey(byte[] userPad, byte[] ownerPad) {
            byte[] ownerKey = new byte[32];
            byte[] digest = md5.Digest(ownerPad);
            arcfour.PrepareARCFOURKey(digest, 0, 5);
            arcfour.EncryptARCFOUR(userPad, ownerKey);
            return ownerKey;
        }

        protected internal virtual void ComputeGlobalEncryptionKey(byte[] userPad, byte[] ownerKey, bool encryptMetadata
            ) {
            mkey = new byte[keyLength / 8];
            // fixed by ujihara in order to follow PDF reference
            md5.Reset();
            md5.Update(userPad);
            md5.Update(ownerKey);
            byte[] ext = new byte[4];
            ext[0] = (byte)permissions;
            ext[1] = (byte)(permissions >> 8);
            ext[2] = (byte)(permissions >> 16);
            ext[3] = (byte)(permissions >> 24);
            md5.Update(ext, 0, 4);
            if (documentId != null) {
                md5.Update(documentId);
            }
            if (!encryptMetadata) {
                md5.Update(metadataPad);
            }
            byte[] digest = new byte[mkey.Length];
            Array.Copy(md5.Digest(), 0, digest, 0, mkey.Length);
            Array.Copy(digest, 0, mkey, 0, mkey.Length);
        }

        protected internal virtual byte[] ComputeUserKey() {
            byte[] userKey = new byte[32];
            arcfour.PrepareARCFOURKey(mkey);
            arcfour.EncryptARCFOUR(pad, userKey);
            return userKey;
        }

        protected internal virtual void SetSpecificHandlerDicEntries(PdfDictionary encryptionDictionary, bool encryptMetadata
            , bool embeddedFilesOnly) {
            encryptionDictionary.Put(PdfName.R, new PdfNumber(2));
            encryptionDictionary.Put(PdfName.V, new PdfNumber(1));
        }

        protected internal virtual bool IsValidPassword(byte[] uValue, byte[] userKey) {
            return !EqualsArray(uValue, userKey, 32);
        }

        private void InitKeyAndFillDictionary(PdfDictionary encryptionDictionary, byte[] userPassword, byte[] ownerPassword
            , int permissions, bool encryptMetadata, bool embeddedFilesOnly, byte[] documentId) {
            ownerPassword = GenerateOwnerPasswordIfNullOrEmpty(ownerPassword);
            CalculatePermissions(permissions);
            this.documentId = documentId;
            keyLength = GetKeyLength(encryptionDictionary);
            // PDF reference 3.5.2 Standard Security Handler, Algorithm 3.3-1
            // If there is no owner password, use the user password instead.
            byte[] userPad = PadPassword(userPassword);
            byte[] ownerPad = PadPassword(ownerPassword);
            byte[] ownerKey = ComputeOwnerKey(userPad, ownerPad);
            ComputeGlobalEncryptionKey(userPad, ownerKey, encryptMetadata);
            byte[] userKey = ComputeUserKey();
            SetStandardHandlerDicEntries(encryptionDictionary, userKey, ownerKey);
            SetSpecificHandlerDicEntries(encryptionDictionary, encryptMetadata, embeddedFilesOnly);
        }

        private void InitKeyAndReadDictionary(PdfDictionary encryptionDictionary, byte[] password, byte[] documentId
            , bool encryptMetadata) {
            byte[] uValue = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.U));
            byte[] oValue = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.O));
            PdfNumber pValue = (PdfNumber)encryptionDictionary.Get(PdfName.P);
            this.permissions = pValue.LongValue();
            this.documentId = documentId;
            keyLength = GetKeyLength(encryptionDictionary);
            byte[] paddedPassword = PadPassword(password);
            CheckPassword(encryptMetadata, uValue, oValue, paddedPassword);
        }

        private void CheckPassword(bool encryptMetadata, byte[] uValue, byte[] oValue, byte[] paddedPassword) {
            // assume password - is owner password
            byte[] userKey;
            byte[] userPad = ComputeOwnerKey(oValue, paddedPassword);
            ComputeGlobalEncryptionKey(userPad, oValue, encryptMetadata);
            userKey = ComputeUserKey();
            // computed user key should be equal to uValue
            if (IsValidPassword(uValue, userKey)) {
                // assume password - is user password
                ComputeGlobalEncryptionKey(paddedPassword, oValue, encryptMetadata);
                userKey = ComputeUserKey();
                // computed user key should be equal to uValue
                if (IsValidPassword(uValue, userKey)) {
                    throw new BadPasswordException(KernelExceptionMessageConstant.BAD_USER_PASSWORD);
                }
                usedOwnerPassword = false;
            }
        }

        private byte[] PadPassword(byte[] password) {
            byte[] userPad = new byte[32];
            if (password == null) {
                Array.Copy(pad, 0, userPad, 0, 32);
            }
            else {
                Array.Copy(password, 0, userPad, 0, Math.Min(password.Length, 32));
                if (password.Length < 32) {
                    Array.Copy(pad, 0, userPad, password.Length, 32 - password.Length);
                }
            }
            return userPad;
        }

        private int GetKeyLength(PdfDictionary encryptionDict) {
            int? keyLength = encryptionDict.GetAsInt(PdfName.Length);
            return keyLength != null ? (int)keyLength : DEFAULT_KEY_LENGTH;
        }
    }
}
