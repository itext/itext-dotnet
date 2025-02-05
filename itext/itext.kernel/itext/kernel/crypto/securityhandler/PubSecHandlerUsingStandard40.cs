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
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    public class PubSecHandlerUsingStandard40 : PubKeySecurityHandler {
        public PubSecHandlerUsingStandard40(PdfDictionary encryptionDictionary, IX509Certificate[] certs, int[] permissions
            , bool encryptMetadata, bool embeddedFilesOnly) {
            InitKeyAndFillDictionary(encryptionDictionary, certs, permissions, encryptMetadata, embeddedFilesOnly);
        }

        public PubSecHandlerUsingStandard40(PdfDictionary encryptionDictionary, IPrivateKey certificateKey, IX509Certificate
             certificate, bool encryptMetadata) {
            InitKeyAndReadDictionary(encryptionDictionary, certificateKey, certificate, encryptMetadata);
        }

        public override OutputStreamEncryption GetEncryptionStream(Stream os) {
            return new OutputStreamStandardEncryption(os, nextObjectKey, 0, nextObjectKeySize);
        }

        public override IDecryptor GetDecryptor() {
            return new StandardDecryptor(nextObjectKey, 0, nextObjectKeySize);
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
            PdfArray recipients = CreateRecipientsArray();
            encryptionDictionary.Put(PdfName.V, new PdfNumber(1));
            encryptionDictionary.Put(PdfName.SubFilter, PdfName.Adbe_pkcs7_s4);
            encryptionDictionary.Put(PdfName.Recipients, recipients);
        }
    }
}
