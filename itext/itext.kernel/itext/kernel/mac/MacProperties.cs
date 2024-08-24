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
namespace iText.Kernel.Mac {
    /// <summary>Class which contains configurable properties for MAC integrity protection mechanism.</summary>
    public class MacProperties {
        private readonly MacProperties.MacDigestAlgorithm macDigestAlgorithm;

        private readonly MacProperties.MacAlgorithm macAlgorithm;

        private readonly MacProperties.KeyWrappingAlgorithm keyWrappingAlgorithm;

        /// <summary>
        /// Creates
        /// <see cref="MacProperties"/>
        /// class containing provided
        /// <see cref="MacDigestAlgorithm"/>.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="MacProperties"/>
        /// class containing provided
        /// <see cref="MacDigestAlgorithm"/>.
        /// For other properties default values are used.
        /// </remarks>
        /// <param name="macDigestAlgorithm">
        /// 
        /// <see cref="MacDigestAlgorithm"/>
        /// to be used in MAC integrity protection algorithm
        /// </param>
        public MacProperties(MacProperties.MacDigestAlgorithm macDigestAlgorithm)
            : this(macDigestAlgorithm, MacProperties.MacAlgorithm.HMAC_WITH_SHA_256, MacProperties.KeyWrappingAlgorithm
                .AES_256_NO_PADD) {
        }

        /// <summary>
        /// Creates
        /// <see cref="MacProperties"/>
        /// class containing provided properties.
        /// </summary>
        /// <param name="macDigestAlgorithm">
        /// 
        /// <see cref="MacDigestAlgorithm"/>
        /// to be used in MAC integrity protection algorithm
        /// </param>
        /// <param name="macAlgorithm">
        /// 
        /// <see cref="MacAlgorithm"/>
        /// to be used in MAC integrity protection algorithm
        /// </param>
        /// <param name="keyWrappingAlgorithm">
        /// 
        /// <see cref="KeyWrappingAlgorithm"/>
        /// to be used in MAC integrity protection algorithm
        /// </param>
        public MacProperties(MacProperties.MacDigestAlgorithm macDigestAlgorithm, MacProperties.MacAlgorithm macAlgorithm
            , MacProperties.KeyWrappingAlgorithm keyWrappingAlgorithm) {
            this.macDigestAlgorithm = macDigestAlgorithm;
            this.macAlgorithm = macAlgorithm;
            this.keyWrappingAlgorithm = keyWrappingAlgorithm;
        }

        /// <summary>
        /// Gets
        /// <see cref="MacDigestAlgorithm"/>
        /// to be used in MAC integrity protection algorithm.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="MacDigestAlgorithm"/>
        /// to be used in MAC integrity protection algorithm
        /// </returns>
        public virtual MacProperties.MacDigestAlgorithm GetMacDigestAlgorithm() {
            return macDigestAlgorithm;
        }

        /// <summary>
        /// Gets
        /// <see cref="MacAlgorithm"/>
        /// to be used in MAC integrity protection algorithm.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="MacAlgorithm"/>
        /// to be used in MAC integrity protection algorithm
        /// </returns>
        public virtual MacProperties.MacAlgorithm GetMacAlgorithm() {
            return macAlgorithm;
        }

        /// <summary>
        /// Gets
        /// <see cref="KeyWrappingAlgorithm"/>
        /// to be used in MAC integrity protection algorithm.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="KeyWrappingAlgorithm"/>
        /// to be used in MAC integrity protection algorithm
        /// </returns>
        public virtual MacProperties.KeyWrappingAlgorithm GetKeyWrappingAlgorithm() {
            return keyWrappingAlgorithm;
        }

        /// <summary>Message digest algorithms, which can be used in MAC integrity protection algorithm.</summary>
        public enum MacDigestAlgorithm {
            SHA_256,
            SHA_384,
            SHA_512,
            SHA3_256,
            SHA3_384,
            SHA3_512
        }

        /// <summary>MAC algorithms, which can be used during integrity protection operation.</summary>
        public enum MacAlgorithm {
            HMAC_WITH_SHA_256
        }

        /// <summary>Key wrapping algorithms, which can be used in MAC integrity protection algorithm.</summary>
        public enum KeyWrappingAlgorithm {
            AES_256_NO_PADD
        }
    }
}
