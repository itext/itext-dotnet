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
