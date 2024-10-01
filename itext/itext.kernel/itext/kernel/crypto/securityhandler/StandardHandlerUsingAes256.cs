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
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    public class StandardHandlerUsingAes256 : StandardSecurityHandler {
        private const int VALIDATION_SALT_OFFSET = 32;

        private const int KEY_SALT_OFFSET = 40;

        private const int SALT_LENGTH = 8;

        protected internal bool encryptMetadata;

        private bool isPdf2;

        public StandardHandlerUsingAes256(PdfDictionary encryptionDictionary, byte[] userPassword, byte[] ownerPassword
            , int permissions, bool encryptMetadata, bool embeddedFilesOnly, PdfVersion version) {
            isPdf2 = version != null && version.CompareTo(PdfVersion.PDF_2_0) >= 0;
            InitKeyAndFillDictionary(encryptionDictionary, userPassword, ownerPassword, permissions, encryptMetadata, 
                embeddedFilesOnly);
        }

        public StandardHandlerUsingAes256(PdfDictionary encryptionDictionary, byte[] password) {
            InitKeyAndReadDictionary(encryptionDictionary, password);
        }

        /// <summary>Checks whether the document-level metadata stream will be encrypted.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the document-level metadata stream shall be encrypted,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsEncryptMetadata() {
            return encryptMetadata;
        }

        public override void SetHashKeyForNextObject(int objNumber, int objGeneration) {
        }

        // in AES256 we don't recalculate nextObjectKey
        public override OutputStreamEncryption GetEncryptionStream(Stream os) {
            return new OutputStreamAesEncryption(os, nextObjectKey, 0, nextObjectKeySize);
        }

        public override IDecryptor GetDecryptor() {
            return new AesDecryptor(nextObjectKey, 0, nextObjectKeySize);
        }

        /// <summary><inheritDoc/></summary>
        public override void SetPermissions(int permissions, PdfDictionary encryptionDictionary) {
            base.SetPermissions(permissions, encryptionDictionary);
            byte[] aes256Perms = GetAes256Perms(permissions, IsEncryptMetadata());
            encryptionDictionary.Put(PdfName.Perms, new PdfLiteral(StreamUtil.CreateEscapedString(aes256Perms)));
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void SetAES256DicEntries(PdfDictionary encryptionDictionary, byte[] oeKey, byte[] ueKey, 
            byte[] aes256Perms, bool encryptMetadata, bool embeddedFilesOnly) {
            int version = 5;
            int rAes256 = 5;
            int rAes256Pdf2 = 6;
            int revision = isPdf2 ? rAes256Pdf2 : rAes256;
            PdfName cryptoFilter = PdfName.AESV3;
            SetEncryptionDictionaryEntries(encryptionDictionary, oeKey, ueKey, aes256Perms, encryptMetadata, embeddedFilesOnly
                , version, revision, cryptoFilter);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetEncryptionDictionaryEntries(PdfDictionary encryptionDictionary, byte[] oeKey, byte
            [] ueKey, byte[] aes256Perms, bool encryptMetadata, bool embeddedFilesOnly, int version, int revision, 
            PdfName cryptoFilter) {
            encryptionDictionary.Put(PdfName.OE, new PdfLiteral(StreamUtil.CreateEscapedString(oeKey)));
            encryptionDictionary.Put(PdfName.UE, new PdfLiteral(StreamUtil.CreateEscapedString(ueKey)));
            encryptionDictionary.Put(PdfName.Perms, new PdfLiteral(StreamUtil.CreateEscapedString(aes256Perms)));
            encryptionDictionary.Put(PdfName.R, new PdfNumber(revision));
            encryptionDictionary.Put(PdfName.V, new PdfNumber(version));
            PdfDictionary stdcf = new PdfDictionary();
            stdcf.Put(PdfName.Length, new PdfNumber(32));
            if (!encryptMetadata) {
                encryptionDictionary.Put(PdfName.EncryptMetadata, PdfBoolean.FALSE);
            }
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
            stdcf.Put(PdfName.CFM, cryptoFilter);
            PdfDictionary cf = new PdfDictionary();
            cf.Put(PdfName.StdCF, stdcf);
            encryptionDictionary.Put(PdfName.CF, cf);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsPdf2(PdfDictionary encryptionDictionary) {
            return encryptionDictionary.GetAsNumber(PdfName.R).GetValue() == 6;
        }
//\endcond

        private void InitKeyAndFillDictionary(PdfDictionary encryptionDictionary, byte[] userPassword, byte[] ownerPassword
            , int permissions, bool encryptMetadata, bool embeddedFilesOnly) {
            ownerPassword = GenerateOwnerPasswordIfNullOrEmpty(ownerPassword);
            permissions |= PERMS_MASK_1_FOR_REVISION_3_OR_GREATER;
            permissions &= PERMS_MASK_2;
            try {
                byte[] userKey;
                byte[] ownerKey;
                byte[] ueKey;
                byte[] oeKey;
                byte[] aes256Perms;
                if (userPassword == null) {
                    userPassword = new byte[0];
                }
                else {
                    if (userPassword.Length > 127) {
                        userPassword = JavaUtil.ArraysCopyOf(userPassword, 127);
                    }
                }
                if (ownerPassword.Length > 127) {
                    ownerPassword = JavaUtil.ArraysCopyOf(ownerPassword, 127);
                }
                // first 8 bytes are validation salt; second 8 bytes are key salt
                byte[] userValAndKeySalt = IVGenerator.GetIV(16);
                byte[] ownerValAndKeySalt = IVGenerator.GetIV(16);
                nextObjectKey = IVGenerator.GetIV(32);
                nextObjectKeySize = 32;
                byte[] hash;
                // Algorithm 8.1
                hash = ComputeHash(userPassword, userValAndKeySalt, 0, 8);
                userKey = JavaUtil.ArraysCopyOf(hash, 48);
                Array.Copy(userValAndKeySalt, 0, userKey, 32, 16);
                // Algorithm 8.2
                hash = ComputeHash(userPassword, userValAndKeySalt, 8, 8);
                AESCipherCBCnoPad ac = new AESCipherCBCnoPad(true, hash);
                ueKey = ac.ProcessBlock(nextObjectKey, 0, nextObjectKey.Length);
                // Algorithm 9.1
                hash = ComputeHash(ownerPassword, ownerValAndKeySalt, 0, 8, userKey);
                ownerKey = JavaUtil.ArraysCopyOf(hash, 48);
                Array.Copy(ownerValAndKeySalt, 0, ownerKey, 32, 16);
                // Algorithm 9.2
                hash = ComputeHash(ownerPassword, ownerValAndKeySalt, 8, 8, userKey);
                ac = new AESCipherCBCnoPad(true, hash);
                oeKey = ac.ProcessBlock(nextObjectKey, 0, nextObjectKey.Length);
                // Algorithm 10
                aes256Perms = GetAes256Perms(permissions, encryptMetadata);
                this.permissions = permissions;
                this.encryptMetadata = encryptMetadata;
                SetStandardHandlerDicEntries(encryptionDictionary, userKey, ownerKey);
                SetAES256DicEntries(encryptionDictionary, oeKey, ueKey, aes256Perms, encryptMetadata, embeddedFilesOnly);
            }
            catch (Exception ex) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, ex);
            }
        }

        private byte[] GetAes256Perms(int permissions, bool encryptMetadata) {
            byte[] aes256Perms;
            AESCipherCBCnoPad ac;
            byte[] permsp = IVGenerator.GetIV(16);
            permsp[0] = (byte)permissions;
            permsp[1] = (byte)(permissions >> 8);
            permsp[2] = (byte)(permissions >> 16);
            permsp[3] = (byte)(permissions >> 24);
            permsp[4] = (byte)(255);
            permsp[5] = (byte)(255);
            permsp[6] = (byte)(255);
            permsp[7] = (byte)(255);
            permsp[8] = encryptMetadata ? (byte)'T' : (byte)'F';
            permsp[9] = (byte)'a';
            permsp[10] = (byte)'d';
            permsp[11] = (byte)'b';
            ac = new AESCipherCBCnoPad(true, nextObjectKey);
            aes256Perms = ac.ProcessBlock(permsp, 0, permsp.Length);
            return aes256Perms;
        }

        private void InitKeyAndReadDictionary(PdfDictionary encryptionDictionary, byte[] password) {
            try {
                if (password == null) {
                    password = new byte[0];
                }
                else {
                    if (password.Length > 127) {
                        password = JavaUtil.ArraysCopyOf(password, 127);
                    }
                }
                isPdf2 = IsPdf2(encryptionDictionary);
                // Truncate user and owner passwords to 48 bytes where the first 32 bytes
                // are a hash value, next 8 bytes are validation salt and final 8 bytes are the key salt
                byte[] oValue = TruncateArray(GetIsoBytes(encryptionDictionary.GetAsString(PdfName.O)));
                byte[] uValue = TruncateArray(GetIsoBytes(encryptionDictionary.GetAsString(PdfName.U)));
                byte[] oeValue = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.OE));
                byte[] ueValue = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.UE));
                byte[] perms = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.Perms));
                PdfNumber pValue = (PdfNumber)encryptionDictionary.Get(PdfName.P);
                this.permissions = pValue.IntValue();
                byte[] hash;
                hash = ComputeHash(password, oValue, VALIDATION_SALT_OFFSET, SALT_LENGTH, uValue);
                usedOwnerPassword = EqualsArray(hash, oValue, 32);
                if (usedOwnerPassword) {
                    hash = ComputeHash(password, oValue, KEY_SALT_OFFSET, SALT_LENGTH, uValue);
                    AESCipherCBCnoPad ac = new AESCipherCBCnoPad(false, hash);
                    nextObjectKey = ac.ProcessBlock(oeValue, 0, oeValue.Length);
                }
                else {
                    hash = ComputeHash(password, uValue, VALIDATION_SALT_OFFSET, SALT_LENGTH);
                    if (!EqualsArray(hash, uValue, 32)) {
                        throw new BadPasswordException(KernelExceptionMessageConstant.BAD_USER_PASSWORD);
                    }
                    hash = ComputeHash(password, uValue, KEY_SALT_OFFSET, SALT_LENGTH);
                    AESCipherCBCnoPad ac = new AESCipherCBCnoPad(false, hash);
                    nextObjectKey = ac.ProcessBlock(ueValue, 0, ueValue.Length);
                }
                nextObjectKeySize = 32;
                AESCipherCBCnoPad ac_1 = new AESCipherCBCnoPad(false, nextObjectKey);
                byte[] decPerms = ac_1.ProcessBlock(perms, 0, perms.Length);
                if (decPerms[9] != (byte)'a' || decPerms[10] != (byte)'d' || decPerms[11] != (byte)'b') {
                    throw new BadPasswordException(KernelExceptionMessageConstant.BAD_USER_PASSWORD);
                }
                int permissionsDecoded = (decPerms[0] & 0xff) | ((decPerms[1] & 0xff) << 8) | ((decPerms[2] & 0xff) << 16)
                     | ((decPerms[3] & 0xff) << 24);
                bool encryptMetadata = decPerms[8] == (byte)'T';
                bool? encryptMetadataEntry = encryptionDictionary.GetAsBool(PdfName.EncryptMetadata);
                if (permissionsDecoded != permissions || encryptMetadataEntry != null && encryptMetadata != encryptMetadataEntry
                    ) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Crypto.Securityhandler.StandardHandlerUsingAes256
                        ));
                    logger.LogError(iText.IO.Logs.IoLogMessageConstant.ENCRYPTION_ENTRIES_P_AND_ENCRYPT_METADATA_NOT_CORRESPOND_PERMS_ENTRY
                        );
                }
                this.permissions = permissionsDecoded;
                this.encryptMetadata = encryptMetadata;
            }
            catch (BadPasswordException ex) {
                throw;
            }
            catch (Exception ex) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, ex);
            }
        }

        private byte[] ComputeHash(byte[] password, byte[] salt, int saltOffset, int saltLen) {
            return ComputeHash(password, salt, saltOffset, saltLen, null);
        }

        private byte[] ComputeHash(byte[] password, byte[] salt, int saltOffset, int saltLen, byte[] userKey) {
            IMessageDigest mdSha256 = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest
                ("SHA-256");
            mdSha256.Update(password);
            mdSha256.Update(salt, saltOffset, saltLen);
            if (userKey != null) {
                mdSha256.Update(userKey);
            }
            byte[] k = mdSha256.Digest();
            if (isPdf2) {
                // See 7.6.4.3.3 "Algorithm 2.B"
                IMessageDigest mdSha384 = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest
                    ("SHA-384");
                IMessageDigest mdSha512 = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest
                    ("SHA-512");
                int userKeyLen = userKey != null ? userKey.Length : 0;
                int passAndUserKeyLen = password.Length + userKeyLen;
                // k1 repetition length
                int k1RepLen;
                int roundNum = 0;
                while (true) {
                    // a)
                    k1RepLen = passAndUserKeyLen + k.Length;
                    byte[] k1 = new byte[k1RepLen * 64];
                    Array.Copy(password, 0, k1, 0, password.Length);
                    Array.Copy(k, 0, k1, password.Length, k.Length);
                    if (userKey != null) {
                        Array.Copy(userKey, 0, k1, password.Length + k.Length, userKeyLen);
                    }
                    for (int i = 1; i < 64; ++i) {
                        Array.Copy(k1, 0, k1, k1RepLen * i, k1RepLen);
                    }
                    // b)
                    AESCipherCBCnoPad cipher = new AESCipherCBCnoPad(true, JavaUtil.ArraysCopyOf(k, 16), JavaUtil.ArraysCopyOfRange
                        (k, 16, 32));
                    byte[] e = cipher.ProcessBlock(k1, 0, k1.Length);
                    // c)
                    IMessageDigest md = null;
                    IBigInteger i_1 = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateBigInteger(1, 
                        JavaUtil.ArraysCopyOf(e, 16));
                    int remainder = i_1.Remainder(iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateBigInteger().ValueOf
                        (3)).GetIntValue();
                    switch (remainder) {
                        case 0: {
                            md = mdSha256;
                            break;
                        }

                        case 1: {
                            md = mdSha384;
                            break;
                        }

                        case 2: {
                            md = mdSha512;
                            break;
                        }
                    }
                    // d)
                    System.Diagnostics.Debug.Assert(md != null);
                    k = md.Digest(e);
                    ++roundNum;
                    if (roundNum > 63) {
                        // e)
                        // interpreting last byte as unsigned integer
                        int condVal = e[e.Length - 1] & 0xFF;
                        if (condVal <= roundNum - 32) {
                            break;
                        }
                    }
                }
                k = k.Length == 32 ? k : JavaUtil.ArraysCopyOf(k, 32);
            }
            return k;
        }

        private byte[] TruncateArray(byte[] array) {
            if (array.Length == 48) {
                return array;
            }
            for (int i = 48; i < array.Length; ++i) {
                if (array[i] != 0) {
                    throw new PdfException(KernelExceptionMessageConstant.BAD_PASSWORD_HASH);
                }
            }
            byte[] truncated = new byte[48];
            Array.Copy(array, 0, truncated, 0, 48);
            return truncated;
        }
    }
}
