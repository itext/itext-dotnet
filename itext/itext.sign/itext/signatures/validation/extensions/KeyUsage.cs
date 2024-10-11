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
namespace iText.Signatures.Validation.Extensions {
    /// <summary>Enum representing possible "Key Usage" extension values.</summary>
    public enum KeyUsage {
        /// <summary>"Digital Signature" key usage value</summary>
        DIGITAL_SIGNATURE,
        /// <summary>"Non Repudiation" key usage value</summary>
        NON_REPUDIATION,
        /// <summary>"Key Encipherment" key usage value</summary>
        KEY_ENCIPHERMENT,
        /// <summary>"Data Encipherment" key usage value</summary>
        DATA_ENCIPHERMENT,
        /// <summary>"Key Agreement" key usage value</summary>
        KEY_AGREEMENT,
        /// <summary>"Key Cert Sign" key usage value</summary>
        KEY_CERT_SIGN,
        /// <summary>"CRL Sign" key usage value</summary>
        CRL_SIGN,
        /// <summary>"Encipher Only" key usage value</summary>
        ENCIPHER_ONLY,
        /// <summary>"Decipher Only" key usage value</summary>
        DECIPHER_ONLY
    }
}
