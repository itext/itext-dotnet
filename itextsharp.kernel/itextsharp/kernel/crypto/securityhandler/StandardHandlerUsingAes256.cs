/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System.IO;
using Org.BouncyCastle.Crypto;
using iTextSharp.IO.Util;
using iTextSharp.Kernel;
using iTextSharp.Kernel.Crypto;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Crypto.Securityhandler
{
    public class StandardHandlerUsingAes256 : StandardSecurityHandler
    {
        private const int VALIDATION_SALT_OFFSET = 32;

        private const int KEY_SALT_OFFSET = 40;

        private const int SALT_LENGTH = 8;

        private const int OU_LENGTH = 48;

        protected internal bool encryptMetadata;

        public StandardHandlerUsingAes256(PdfDictionary encryptionDictionary, byte[] userPassword, byte[] ownerPassword
            , int permissions, bool encryptMetadata, bool embeddedFilesOnly)
        {
            InitKeyAndFillDictionary(encryptionDictionary, userPassword, ownerPassword, permissions, encryptMetadata, 
                embeddedFilesOnly);
        }

        public StandardHandlerUsingAes256(PdfDictionary encryptionDictionary, byte[] password)
        {
            InitKeyAndReadDictionary(encryptionDictionary, password);
        }

        public virtual bool IsEncryptMetadata()
        {
            return encryptMetadata;
        }

        public override void SetHashKeyForNextObject(int objNumber, int objGeneration)
        {
        }

        // in AES256 we don't recalculate nextObjectKey
        public override OutputStreamEncryption GetEncryptionStream(Stream os)
        {
            return new OutputStreamAesEncryption(os, nextObjectKey, 0, nextObjectKeySize);
        }

        public override IDecryptor GetDecryptor()
        {
            return new AesDecryptor(nextObjectKey, 0, nextObjectKeySize);
        }

        private void InitKeyAndFillDictionary(PdfDictionary encryptionDictionary, byte[] userPassword, byte[] ownerPassword
            , int permissions, bool encryptMetadata, bool embeddedFilesOnly)
        {
            ownerPassword = GenerateOwnerPasswordIfNullOrEmpty(ownerPassword);
            permissions |= PERMS_MASK_1_FOR_REVISION_3_OR_GREATER;
            permissions &= PERMS_MASK_2;
            try
            {
                byte[] userKey;
                byte[] ownerKey;
                byte[] ueKey;
                byte[] oeKey;
                byte[] aes256Perms;
                if (userPassword == null)
                {
                    userPassword = new byte[0];
                }
                byte[] uvs = IVGenerator.GetIV(8);
                byte[] uks = IVGenerator.GetIV(8);
                nextObjectKey = IVGenerator.GetIV(32);
                nextObjectKeySize = 32;
                // Algorithm 3.8.1
                IDigest md = Org.BouncyCastle.Security.DigestUtilities.GetDigest("SHA-256");
                md.Update(userPassword, 0, Math.Min(userPassword.Length, 127));
                md.Update(uvs);
                userKey = new byte[48];
                md.Digest(userKey, 0, 32);
                System.Array.Copy(uvs, 0, userKey, 32, 8);
                System.Array.Copy(uks, 0, userKey, 40, 8);
                // Algorithm 3.8.2
                md.Update(userPassword, 0, Math.Min(userPassword.Length, 127));
                md.Update(uks);
                AESCipherCBCnoPad ac = new AESCipherCBCnoPad(true, md.Digest());
                ueKey = ac.ProcessBlock(nextObjectKey, 0, nextObjectKey.Length);
                // Algorithm 3.9.1
                byte[] ovs = IVGenerator.GetIV(8);
                byte[] oks = IVGenerator.GetIV(8);
                md.Update(ownerPassword, 0, Math.Min(ownerPassword.Length, 127));
                md.Update(ovs);
                md.Update(userKey);
                ownerKey = new byte[48];
                md.Digest(ownerKey, 0, 32);
                System.Array.Copy(ovs, 0, ownerKey, 32, 8);
                System.Array.Copy(oks, 0, ownerKey, 40, 8);
                // Algorithm 3.9.2
                md.Update(ownerPassword, 0, Math.Min(ownerPassword.Length, 127));
                md.Update(oks);
                md.Update(userKey);
                ac = new AESCipherCBCnoPad(true, md.Digest());
                oeKey = ac.ProcessBlock(nextObjectKey, 0, nextObjectKey.Length);
                // Algorithm 3.10
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
                this.permissions = permissions;
                this.encryptMetadata = encryptMetadata;
                SetStandardHandlerDicEntries(encryptionDictionary, userKey, ownerKey);
                SetAES256DicEntries(encryptionDictionary, oeKey, ueKey, aes256Perms, encryptMetadata, embeddedFilesOnly);
            }
            catch (Exception ex)
            {
                throw new PdfException(PdfException.PdfEncryption, ex);
            }
        }

        private void SetAES256DicEntries(PdfDictionary encryptionDictionary, byte[] oeKey, byte[] ueKey, byte[] aes256Perms
            , bool encryptMetadata, bool embeddedFilesOnly)
        {
            int aes256Revision = 5;
            encryptionDictionary.Put(PdfName.OE, new PdfLiteral(StreamUtil.CreateEscapedString(oeKey)));
            encryptionDictionary.Put(PdfName.UE, new PdfLiteral(StreamUtil.CreateEscapedString(ueKey)));
            encryptionDictionary.Put(PdfName.Perms, new PdfLiteral(StreamUtil.CreateEscapedString(aes256Perms)));
            encryptionDictionary.Put(PdfName.R, new PdfNumber(aes256Revision));
            encryptionDictionary.Put(PdfName.V, new PdfNumber(aes256Revision));
            PdfDictionary stdcf = new PdfDictionary();
            stdcf.Put(PdfName.Length, new PdfNumber(32));
            if (!encryptMetadata)
            {
                encryptionDictionary.Put(PdfName.EncryptMetadata, PdfBoolean.FALSE);
            }
            if (embeddedFilesOnly)
            {
                stdcf.Put(PdfName.AuthEvent, PdfName.EFOpen);
                encryptionDictionary.Put(PdfName.EFF, PdfName.StdCF);
                encryptionDictionary.Put(PdfName.StrF, PdfName.Identity);
                encryptionDictionary.Put(PdfName.StmF, PdfName.Identity);
            }
            else
            {
                stdcf.Put(PdfName.AuthEvent, PdfName.DocOpen);
                encryptionDictionary.Put(PdfName.StrF, PdfName.StdCF);
                encryptionDictionary.Put(PdfName.StmF, PdfName.StdCF);
            }
            stdcf.Put(PdfName.CFM, PdfName.AESV3);
            PdfDictionary cf = new PdfDictionary();
            cf.Put(PdfName.StdCF, stdcf);
            encryptionDictionary.Put(PdfName.CF, cf);
        }

        private void InitKeyAndReadDictionary(PdfDictionary encryptionDictionary, byte[] password)
        {
            try
            {
                if (password == null)
                {
                    password = new byte[0];
                }
                byte[] oValue = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.O));
                byte[] uValue = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.U));
                byte[] oeValue = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.OE));
                byte[] ueValue = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.UE));
                byte[] perms = GetIsoBytes(encryptionDictionary.GetAsString(PdfName.Perms));
                PdfNumber pValue = (PdfNumber)encryptionDictionary.Get(PdfName.P);
                this.permissions = pValue.LongValue();
                IDigest md = Org.BouncyCastle.Security.DigestUtilities.GetDigest("SHA-256");
                md.Update(password, 0, Math.Min(password.Length, 127));
                md.Update(oValue, VALIDATION_SALT_OFFSET, SALT_LENGTH);
                md.Update(uValue, 0, OU_LENGTH);
                byte[] hash = md.Digest();
                usedOwnerPassword = CompareArray(hash, oValue, 32);
                if (usedOwnerPassword)
                {
                    md.Update(password, 0, Math.Min(password.Length, 127));
                    md.Update(oValue, KEY_SALT_OFFSET, SALT_LENGTH);
                    md.Update(uValue, 0, OU_LENGTH);
                    hash = md.Digest();
                    AESCipherCBCnoPad ac = new AESCipherCBCnoPad(false, hash);
                    nextObjectKey = ac.ProcessBlock(oeValue, 0, oeValue.Length);
                }
                else
                {
                    md.Update(password, 0, Math.Min(password.Length, 127));
                    md.Update(uValue, VALIDATION_SALT_OFFSET, SALT_LENGTH);
                    hash = md.Digest();
                    if (!CompareArray(hash, uValue, 32))
                    {
                        throw new BadPasswordException(PdfException.BadUserPassword);
                    }
                    md.Update(password, 0, Math.Min(password.Length, 127));
                    md.Update(uValue, KEY_SALT_OFFSET, SALT_LENGTH);
                    hash = md.Digest();
                    AESCipherCBCnoPad ac = new AESCipherCBCnoPad(false, hash);
                    nextObjectKey = ac.ProcessBlock(ueValue, 0, ueValue.Length);
                }
                nextObjectKeySize = 32;
                AESCipherCBCnoPad ac_1 = new AESCipherCBCnoPad(false, nextObjectKey);
                byte[] decPerms = ac_1.ProcessBlock(perms, 0, perms.Length);
                if (decPerms[9] != (byte)'a' || decPerms[10] != (byte)'d' || decPerms[11] != (byte)'b')
                {
                    throw new BadPasswordException(PdfException.BadUserPassword);
                }
                permissions = (decPerms[0] & 0xff) | ((decPerms[1] & 0xff) << 8) | ((decPerms[2] & 0xff) << 16) | ((decPerms
                    [2] & 0xff) << 24);
                encryptMetadata = decPerms[8] == (byte)'T';
            }
            catch (BadPasswordException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PdfException(PdfException.PdfEncryption, ex);
            }
        }

        private static bool CompareArray(byte[] a, byte[] b, int len)
        {
            for (int k = 0; k < len; ++k)
            {
                if (a[k] != b[k])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
