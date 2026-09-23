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

namespace iText.Bouncycastleconnector {
    /// <summary>
    /// A utility class for exception-free initialization of bouncy-castle-dependent
    /// secure random number generator.
    /// </summary>
    /// <remarks>
    /// A utility class for exception-free initialization of bouncy-castle-dependent
    /// secure random number generator. It is meant to be used specifically for
    /// instances initialized as a static field value, because exceptions thrown
    /// during class initialization causes troubles.
    /// </remarks>
    public class BouncyCastleSecureRandomHolder {
        private volatile RNGCryptoServiceProvider RNG = null;

        private Object Lock = new Object();

        /// <summary>
        /// Creates a new
        /// <see cref="BouncyCastleSecureRandomHolder"/>
        /// instance.
        /// </summary>
        public BouncyCastleSecureRandomHolder() {
        }

        // empty constructor
        /// <summary>Gets a secure random number generator instance.</summary>
        /// <remarks>
        /// Gets a secure random number generator instance.
        /// <para />
        /// If bouncy-castle dependency is missing it will throw an exception.
        /// </remarks>
        /// <returns>
        /// the lazily initialized
        /// <see cref="System.Security.Cryptography.RNGCryptoServiceProvider"/>
        /// </returns>
        public virtual RNGCryptoServiceProvider GetSecureRandom() {
            if (RNG == null) {
                lock (Lock) {
                    if (RNG == null) {
                        // can throw an exception if BC is missing
                        RNG = BouncyCastleFactoryCreator.GetFactory().GetSecureRandom();
                    }
                }
            }
            return RNG;
        }
    }
}
