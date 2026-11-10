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
using System.Security.Cryptography;

namespace iText.Kernel.Crypto {
    /// <summary>
    /// A utility class for exception-free initialization of
    /// secure random number generator.
    /// </summary>
    /// <remarks>
    /// A utility class for exception-free initialization of
    /// secure random number generator. It is meant to be used specifically for
    /// instances initialized as a static field value, because exceptions thrown
    /// during class initialization causes troubles.
    /// </remarks>
    public class SecureRandomHolder {
        private volatile RNGCryptoServiceProvider Rng = null;

        private Object Lock = new Object();

        /// <summary>
        /// Creates a new
        /// <see cref="SecureRandomHolder"/>
        /// instance.
        /// </summary>
        public SecureRandomHolder() {
        }

        // empty constructor
        /// <summary>Gets a secure random number generator instance.</summary>
        /// <remarks>
        /// Gets a secure random number generator instance.
        /// </remarks>
        /// <returns>
        /// the lazily initialized
        /// <see cref="System.Security.Cryptography.RNGCryptoServiceProvider"/>
        /// </returns>
        public virtual RNGCryptoServiceProvider GetSecureRandom() {
            if (Rng == null) {
                lock (Lock) {
                    if (Rng == null) {
                        Rng = new RNGCryptoServiceProvider();
                    }
                }
            }
            return Rng;
        }
    }
}
