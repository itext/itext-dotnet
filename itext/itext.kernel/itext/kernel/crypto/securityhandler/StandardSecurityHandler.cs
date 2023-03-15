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
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    public abstract class StandardSecurityHandler : SecurityHandler {
        protected internal const int PERMS_MASK_1_FOR_REVISION_2 = unchecked((int)(0xffffffc0));

        protected internal const int PERMS_MASK_1_FOR_REVISION_3_OR_GREATER = unchecked((int)(0xfffff0c0));

        protected internal const int PERMS_MASK_2 = unchecked((int)(0xfffffffc));

        protected internal long permissions;

        protected internal bool usedOwnerPassword = true;

        public virtual long GetPermissions() {
            return permissions;
        }

        public virtual bool IsUsedOwnerPassword() {
            return usedOwnerPassword;
        }

        protected internal virtual void SetStandardHandlerDicEntries(PdfDictionary encryptionDictionary, byte[] userKey
            , byte[] ownerKey) {
            encryptionDictionary.Put(PdfName.Filter, PdfName.Standard);
            encryptionDictionary.Put(PdfName.O, new PdfLiteral(StreamUtil.CreateEscapedString(ownerKey)));
            encryptionDictionary.Put(PdfName.U, new PdfLiteral(StreamUtil.CreateEscapedString(userKey)));
            encryptionDictionary.Put(PdfName.P, new PdfNumber(permissions));
        }

        protected internal virtual byte[] GenerateOwnerPasswordIfNullOrEmpty(byte[] ownerPassword) {
            if (ownerPassword == null || ownerPassword.Length == 0) {
                ownerPassword = md5.Digest(PdfEncryption.GenerateNewDocumentId());
            }
            return ownerPassword;
        }

        /// <summary>Gets bytes of String-value without considering encoding.</summary>
        /// <param name="string">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// to get bytes from it
        /// </param>
        /// <returns>byte array</returns>
        protected internal virtual byte[] GetIsoBytes(PdfString @string) {
            return ByteUtils.GetIsoBytes(@string.GetValue());
        }

        protected internal static bool EqualsArray(byte[] ar1, byte[] ar2, int size) {
            for (int k = 0; k < size; ++k) {
                if (ar1[k] != ar2[k]) {
                    return false;
                }
            }
            return true;
        }
    }
}
