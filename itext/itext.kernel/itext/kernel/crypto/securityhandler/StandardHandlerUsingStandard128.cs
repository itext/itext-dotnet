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
using System;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    public class StandardHandlerUsingStandard128 : StandardHandlerUsingStandard40 {
        public StandardHandlerUsingStandard128(PdfDictionary encryptionDictionary, byte[] userPassword, byte[] ownerPassword
            , int permissions, bool encryptMetadata, bool embeddedFilesOnly, byte[] documentId)
            : base(encryptionDictionary, userPassword, ownerPassword, permissions, encryptMetadata, embeddedFilesOnly, 
                documentId) {
        }

        public StandardHandlerUsingStandard128(PdfDictionary encryptionDictionary, byte[] password, byte[] documentId
            , bool encryptMetadata)
            : base(encryptionDictionary, password, documentId, encryptMetadata) {
        }

        protected internal override void CalculatePermissions(int permissions) {
            permissions |= PERMS_MASK_1_FOR_REVISION_3_OR_GREATER;
            permissions &= PERMS_MASK_2;
            this.permissions = permissions;
        }

        protected internal override byte[] ComputeOwnerKey(byte[] userPad, byte[] ownerPad) {
            byte[] ownerKey = new byte[32];
            byte[] digest = md5.Digest(ownerPad);
            byte[] mkey = new byte[keyLength / 8];
            // only use for the input as many bit as the key consists of
            for (int k = 0; k < 50; ++k) {
                md5.Update(digest, 0, mkey.Length);
                Array.Copy(md5.Digest(), 0, digest, 0, mkey.Length);
            }
            Array.Copy(userPad, 0, ownerKey, 0, 32);
            for (int i = 0; i < 20; ++i) {
                for (int j = 0; j < mkey.Length; ++j) {
                    mkey[j] = (byte)(digest[j] ^ i);
                }
                arcfour.PrepareARCFOURKey(mkey);
                arcfour.EncryptARCFOUR(ownerKey);
            }
            return ownerKey;
        }

        protected internal override void ComputeGlobalEncryptionKey(byte[] userPad, byte[] ownerKey, bool encryptMetadata
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
            // only use the really needed bits as input for the hash
            for (int k = 0; k < 50; ++k) {
                Array.Copy(md5.Digest(digest), 0, digest, 0, mkey.Length);
            }
            Array.Copy(digest, 0, mkey, 0, mkey.Length);
        }

        protected internal override byte[] ComputeUserKey() {
            byte[] userKey = new byte[32];
            md5.Update(pad);
            byte[] digest = md5.Digest(documentId);
            Array.Copy(digest, 0, userKey, 0, 16);
            for (int k = 16; k < 32; ++k) {
                userKey[k] = 0;
            }
            for (int i = 0; i < 20; ++i) {
                for (int j = 0; j < mkey.Length; ++j) {
                    digest[j] = (byte)(mkey[j] ^ i);
                }
                arcfour.PrepareARCFOURKey(digest, 0, mkey.Length);
                arcfour.EncryptARCFOUR(userKey, 0, 16);
            }
            return userKey;
        }

        protected internal override void SetSpecificHandlerDicEntries(PdfDictionary encryptionDictionary, bool encryptMetadata
            , bool embeddedFilesOnly) {
            if (encryptMetadata) {
                encryptionDictionary.Put(PdfName.R, new PdfNumber(3));
                encryptionDictionary.Put(PdfName.V, new PdfNumber(2));
            }
            else {
                encryptionDictionary.Put(PdfName.EncryptMetadata, PdfBoolean.FALSE);
                encryptionDictionary.Put(PdfName.R, new PdfNumber(4));
                encryptionDictionary.Put(PdfName.V, new PdfNumber(4));
                PdfDictionary stdcf = new PdfDictionary();
                stdcf.Put(PdfName.Length, new PdfNumber(16));
                if (embeddedFilesOnly) {
                    stdcf.Put(PdfName.AuthEvent, PdfName.EFOpen);
                    encryptionDictionary.Put(PdfName.EFF, PdfName.StdCF);
                    encryptionDictionary.Put(PdfName.StrF, PdfName.Identity);
                    encryptionDictionary.Put(PdfName.StmF, PdfName.Identity);
                }
                else {
                    stdcf.Put(PdfName.AuthEvent, PdfName.DocOpen);
                    encryptionDictionary.Put(PdfName.StrF, PdfName.StdCF);
                    encryptionDictionary.Put(PdfName.StmF, PdfName.StdCF);
                }
                stdcf.Put(PdfName.CFM, PdfName.V2);
                PdfDictionary cf = new PdfDictionary();
                cf.Put(PdfName.StdCF, stdcf);
                encryptionDictionary.Put(PdfName.CF, cf);
            }
        }

        protected internal override bool IsValidPassword(byte[] uValue, byte[] userKey) {
            return !EqualsArray(uValue, userKey, 16);
        }
    }
}
