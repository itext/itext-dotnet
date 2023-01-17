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
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;

namespace iText.Kernel.Crypto.Securityhandler {
    public abstract class SecurityHandler {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Crypto.Securityhandler.SecurityHandler
            ));

        /// <summary>The global encryption key</summary>
        protected internal byte[] mkey = new byte[0];

        /// <summary>The encryption key for a particular object/generation.</summary>
        /// <remarks>
        /// The encryption key for a particular object/generation.
        /// It is recalculated with
        /// <see cref="SetHashKeyForNextObject(int, int)"/>
        /// for every object individually based in its
        /// object/generation.
        /// </remarks>
        protected internal byte[] nextObjectKey;

        /// <summary>
        /// The encryption key length for a particular object/generation
        /// It is recalculated with
        /// <see cref="SetHashKeyForNextObject(int, int)"/>
        /// for every object individually based in its
        /// object/generation.
        /// </summary>
        protected internal int nextObjectKeySize;

        protected internal IIDigest md5;

        /// <summary>Work area to prepare the object/generation bytes</summary>
        protected internal byte[] extra = new byte[5];

        protected internal SecurityHandler() {
            SafeInitMessageDigest();
        }

        /// <summary>
        /// Note: For most of the supported security handlers algorithm to calculate encryption key for particular object
        /// is the same.
        /// </summary>
        /// <param name="objNumber">number of particular object for encryption</param>
        /// <param name="objGeneration">generation of particular object for encryption</param>
        public virtual void SetHashKeyForNextObject(int objNumber, int objGeneration) {
            // added by ujihara
            md5.Reset();
            extra[0] = (byte)objNumber;
            extra[1] = (byte)(objNumber >> 8);
            extra[2] = (byte)(objNumber >> 16);
            extra[3] = (byte)objGeneration;
            extra[4] = (byte)(objGeneration >> 8);
            md5.Update(mkey);
            md5.Update(extra);
            nextObjectKey = md5.Digest();
            nextObjectKeySize = mkey.Length + 5;
            if (nextObjectKeySize > 16) {
                nextObjectKeySize = 16;
            }
        }

        public abstract OutputStreamEncryption GetEncryptionStream(Stream os);

        public abstract IDecryptor GetDecryptor();

        private void SafeInitMessageDigest() {
            try {
                md5 = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("MD5");
                if (FACTORY.IsInApprovedOnlyMode()) {
                    LOGGER.LogWarning(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT);
                }
            }
            catch (Exception e) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_ENCRYPTION, e);
            }
        }
    }
}
