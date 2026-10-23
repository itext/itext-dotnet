/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Digest;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Crypto.Securityhandler {
    public abstract class StandardSecurityHandler : SecurityHandler {
        protected internal const int PERMS_MASK_1_FOR_REVISION_2 = unchecked((int)(0xffffffc0));

        protected internal const int PERMS_MASK_1_FOR_REVISION_3_OR_GREATER = unchecked((int)(0xffffe0c0));

        protected internal const int PERMS_MASK_2 = unchecked((int)(0xfffffffc));

        protected internal int permissions;

        protected internal bool usedOwnerPassword = true;

        public virtual int GetPermissions() {
            return permissions;
        }

        /// <summary>Updates encryption dictionary with the security permissions provided.</summary>
        /// <param name="permissions">new permissions to set</param>
        /// <param name="encryptionDictionary">encryption dictionary to update</param>
        public virtual void SetPermissions(int permissions, PdfDictionary encryptionDictionary) {
            this.permissions = permissions;
            encryptionDictionary.Put(PdfName.P, new PdfNumber(permissions));
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
                try {
                    IMessageDigest sha256 = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest(
                        "SHA-256");
                    ownerPassword = sha256.Digest(PdfEncryption.GenerateNewDocumentId());
                }
                catch (Exception e) {
                    throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
                }
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

        /// <summary>Compares two byte arrays for equality up to a specified size.</summary>
        /// <remarks>
        /// Compares two byte arrays for equality up to a specified size.
        /// <para />
        /// This method is used to compare cryptographic values in a way that avoids timing attacks.
        /// </remarks>
        /// <param name="ar1">the first byte array</param>
        /// <param name="ar2">the second byte array</param>
        /// <param name="size">the number of bytes to compare</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the specified number of bytes in both arrays are equal,
        /// <see langword="false"/>
        /// otherwise.
        /// If any of the arrays is smaller than
        /// <paramref name="size"/>
        /// , the method returns
        /// <see langword="false"/>
        /// </returns>
        protected internal static bool EqualsArray(byte[] ar1, byte[] ar2, int size) {
            if (ar1.Length < size || ar2.Length < size) {
                return false;
            }
            byte[] ba1 = JavaUtil.ArraysCopyOf(ar1, size);
            byte[] ba2 = JavaUtil.ArraysCopyOf(ar2, size);
            return JavaUtil.FixedTimeEquals(ba1, ba2);
        }
    }
}
