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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    public class PubSecHandlerUsingAes256 : PubSecHandlerUsingAes128 {
        public PubSecHandlerUsingAes256(PdfDictionary encryptionDictionary, IX509Certificate[] certs, int[] permissions
            , bool encryptMetadata, bool embeddedFilesOnly)
            : base(encryptionDictionary, certs, permissions, encryptMetadata, embeddedFilesOnly) {
        }

        public PubSecHandlerUsingAes256(PdfDictionary encryptionDictionary, IPrivateKey certificateKey, IX509Certificate
             certificate, bool encryptMetadata)
            : base(encryptionDictionary, certificateKey, certificate, encryptMetadata) {
        }

        public override void SetHashKeyForNextObject(int objNumber, int objGeneration) {
        }

        // in AES256 we don't recalculate nextObjectKey
        protected internal override String GetDigestAlgorithm() {
            return "SHA-256";
        }

        protected internal override void InitKey(byte[] globalKey, int keyLength) {
            nextObjectKey = globalKey;
            nextObjectKeySize = 32;
        }

        protected internal override void SetPubSecSpecificHandlerDicEntries(PdfDictionary encryptionDictionary, bool
             encryptMetadata, bool embeddedFilesOnly) {
            encryptionDictionary.Put(PdfName.Filter, PdfName.Adobe_PubSec);
            encryptionDictionary.Put(PdfName.SubFilter, PdfName.Adbe_pkcs7_s5);
            encryptionDictionary.Put(PdfName.V, new PdfNumber(5));
            PdfArray recipients = CreateRecipientsArray();
            PdfDictionary stdcf = new PdfDictionary();
            stdcf.Put(PdfName.Recipients, recipients);
            if (!encryptMetadata) {
                stdcf.Put(PdfName.EncryptMetadata, PdfBoolean.FALSE);
            }
            stdcf.Put(PdfName.CFM, PdfName.AESV3);
            stdcf.Put(PdfName.Length, new PdfNumber(256));
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
