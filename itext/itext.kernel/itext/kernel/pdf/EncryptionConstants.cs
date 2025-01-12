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
namespace iText.Kernel.Pdf {
    /// <summary>
    /// Encryption constants for
    /// <see cref="WriterProperties.SetStandardEncryption(byte[], byte[], int, int)"/>.
    /// </summary>
    public sealed class EncryptionConstants {
        private EncryptionConstants() {
        }

        // Empty constructor
        /// <summary>Type of encryption.</summary>
        /// <remarks>Type of encryption. RC4 encryption algorithm will be used with the key length of 40 bits.</remarks>
        public const int STANDARD_ENCRYPTION_40 = 0;

        /// <summary>Type of encryption.</summary>
        /// <remarks>Type of encryption. RC4 encryption algorithm will be used with the key length of 128 bits.</remarks>
        public const int STANDARD_ENCRYPTION_128 = 1;

        /// <summary>Type of encryption.</summary>
        /// <remarks>Type of encryption. AES encryption algorithm will be used with the key length of 128 bits.</remarks>
        public const int ENCRYPTION_AES_128 = 2;

        /// <summary>Type of encryption.</summary>
        /// <remarks>Type of encryption. AES encryption algorithm will be used with the key length of 256 bits.</remarks>
        public const int ENCRYPTION_AES_256 = 3;

        /// <summary>Type of encryption.</summary>
        /// <remarks>Type of encryption. Advanced Encryption Standard-Galois/Counter Mode (AES-GCM) encryption algorithm.
        ///     </remarks>
        public const int ENCRYPTION_AES_GCM = 4;

        /// <summary>Add this to the mode to keep the metadata in clear text.</summary>
        public const int DO_NOT_ENCRYPT_METADATA = 8;

        /// <summary>Add this to the mode to encrypt only the embedded files.</summary>
        public const int EMBEDDED_FILES_ONLY = 24;

        /// <summary>The operation is permitted when the document is opened with the user password.</summary>
        public const int ALLOW_PRINTING = 4 + 2048;

        /// <summary>The operation is permitted when the document is opened with the user password.</summary>
        public const int ALLOW_MODIFY_CONTENTS = 8;

        /// <summary>The operation is permitted when the document is opened with the user password.</summary>
        public const int ALLOW_COPY = 16;

        /// <summary>The operation is permitted when the document is opened with the user password.</summary>
        public const int ALLOW_MODIFY_ANNOTATIONS = 32;

        /// <summary>The operation is permitted when the document is opened with the user password.</summary>
        public const int ALLOW_FILL_IN = 256;

        /// <summary>The operation is permitted when the document is opened with the user password.</summary>
        public const int ALLOW_SCREENREADERS = 512;

        /// <summary>The operation is permitted when the document is opened with the user password.</summary>
        public const int ALLOW_ASSEMBLY = 1024;

        /// <summary>The operation is permitted when the document is opened with the user password.</summary>
        public const int ALLOW_DEGRADED_PRINTING = 4;

//\cond DO_NOT_DOCUMENT
        /// <summary>Mask to separate the encryption type from the encryption mode.</summary>
        internal const int ENCRYPTION_MASK = 7;
//\endcond
    }
}
