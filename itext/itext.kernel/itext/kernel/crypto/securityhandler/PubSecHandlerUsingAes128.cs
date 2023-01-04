/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    public class PubSecHandlerUsingAes128 : PubKeySecurityHandler {
        private static readonly byte[] salt = new byte[] { (byte)0x73, (byte)0x41, (byte)0x6c, (byte)0x54 };

        public PubSecHandlerUsingAes128(PdfDictionary encryptionDictionary, IX509Certificate[] certs, int[] permissions
            , bool encryptMetadata, bool embeddedFilesOnly) {
            InitKeyAndFillDictionary(encryptionDictionary, certs, permissions, encryptMetadata, embeddedFilesOnly);
        }

        public PubSecHandlerUsingAes128(PdfDictionary encryptionDictionary, IPrivateKey certificateKey, IX509Certificate
             certificate, bool encryptMetadata) {
            InitKeyAndReadDictionary(encryptionDictionary, certificateKey, certificate, encryptMetadata);
        }

        public override OutputStreamEncryption GetEncryptionStream(Stream os) {
            return new OutputStreamAesEncryption(os, nextObjectKey, 0, nextObjectKeySize);
        }

        public override IDecryptor GetDecryptor() {
            return new AesDecryptor(nextObjectKey, 0, nextObjectKeySize);
        }

        public override void SetHashKeyForNextObject(int objNumber, int objGeneration) {
            // added by ujihara
            md5.Reset();
            extra[0] = (byte)objNumber;
            extra[1] = (byte)(objNumber >> 8);
            extra[2] = (byte)(objNumber >> 16);
            extra[3] = (byte)objGeneration;
            extra[4] = (byte)(objGeneration >> 8);
            md5.Update(mkey);
            md5.Update(extra);
            md5.Update(salt);
            nextObjectKey = md5.Digest();
            nextObjectKeySize = mkey.Length + 5;
            if (nextObjectKeySize > 16) {
                nextObjectKeySize = 16;
            }
        }

        protected internal override String GetDigestAlgorithm() {
            return "SHA-1";
        }

        protected internal override void InitKey(byte[] globalKey, int keyLength) {
            mkey = new byte[keyLength / 8];
            Array.Copy(globalKey, 0, mkey, 0, mkey.Length);
        }

        protected internal override void SetPubSecSpecificHandlerDicEntries(PdfDictionary encryptionDictionary, bool
             encryptMetadata, bool embeddedFilesOnly) {
            encryptionDictionary.Put(PdfName.Filter, PdfName.Adobe_PubSec);
            encryptionDictionary.Put(PdfName.SubFilter, PdfName.Adbe_pkcs7_s5);
            encryptionDictionary.Put(PdfName.R, new PdfNumber(4));
            encryptionDictionary.Put(PdfName.V, new PdfNumber(4));
            PdfArray recipients = CreateRecipientsArray();
            PdfDictionary stdcf = new PdfDictionary();
            stdcf.Put(PdfName.Recipients, recipients);
            if (!encryptMetadata) {
                stdcf.Put(PdfName.EncryptMetadata, PdfBoolean.FALSE);
            }
            stdcf.Put(PdfName.CFM, PdfName.AESV2);
            stdcf.Put(PdfName.Length, new PdfNumber(128));
            PdfDictionary cf = new PdfDictionary();
            cf.Put(PdfName.DefaultCryptFilter, stdcf);
            encryptionDictionary.Put(PdfName.CF, cf);
            if (embeddedFilesOnly) {
                encryptionDictionary.Put(PdfName.EFF, PdfName.DefaultCryptFilter);
                encryptionDictionary.Put(PdfName.StrF, PdfName.Identity);
                encryptionDictionary.Put(PdfName.StmF, PdfName.Identity);
            }
            else {
                encryptionDictionary.Put(PdfName.StrF, PdfName.DefaultCryptFilter);
                encryptionDictionary.Put(PdfName.StmF, PdfName.DefaultCryptFilter);
            }
        }
    }
}
