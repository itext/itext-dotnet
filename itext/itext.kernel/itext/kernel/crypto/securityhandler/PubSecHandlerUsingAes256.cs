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
            encryptionDictionary.Put(PdfName.R, new PdfNumber(5));
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
