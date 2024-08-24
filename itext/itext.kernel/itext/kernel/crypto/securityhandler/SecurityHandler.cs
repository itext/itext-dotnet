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
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
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

        protected internal IDigest md5;

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
        
        /// <summary>
        /// Gets encryption key for a particular object/generation.
        /// </summary>
        /// <returns>encryption key for a particular object/generation.</returns>
        public byte[] GetNextObjectKey() {
            return JavaUtil.ArraysCopyOf(nextObjectKey, nextObjectKey.Length);
        }

        /// <summary>
        /// Gets global encryption key.
        /// </summary>
        /// <returns>global encryption key.</returns>
        public byte[] GetMkey() {
            return JavaUtil.ArraysCopyOf(mkey, mkey.Length);
        }

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
