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
using System;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Lotl.Criteria {
    /// <summary>Key Usage Criteria implementation from a TL.</summary>
    public class KeyUsageCriteria : iText.Signatures.Validation.Lotl.Criteria.Criteria {
        private readonly bool?[] requiredKeyUsage = new bool?[9];

        /// <summary>
        /// Creates a new instance of
        /// <see cref="KeyUsageCriteria"/>.
        /// </summary>
        public KeyUsageCriteria() {
        }

        // Empty constructor
        /// <summary>Adds required key usage bit by its name and value.</summary>
        /// <param name="name">name of the required key usage bit.</param>
        /// <param name="value">
        /// 
        /// <c>boolean</c>
        /// value for a required key usage bit.
        /// </param>
        public virtual void AddKeyUsageBit(String name, String value) {
            bool booleanValue = "true".Equals(value);
            switch (name) {
                case "digitalSignature": {
                    requiredKeyUsage[0] = booleanValue;
                    break;
                }

                case "nonRepudiation": {
                    requiredKeyUsage[1] = booleanValue;
                    break;
                }

                case "keyEncipherment": {
                    requiredKeyUsage[2] = booleanValue;
                    break;
                }

                case "dataEncipherment": {
                    requiredKeyUsage[3] = booleanValue;
                    break;
                }

                case "keyAgreement": {
                    requiredKeyUsage[4] = booleanValue;
                    break;
                }

                case "keyCertSign": {
                    requiredKeyUsage[5] = booleanValue;
                    break;
                }

                case "crlSign": {
                    requiredKeyUsage[6] = booleanValue;
                    break;
                }

                case "encipherOnly": {
                    requiredKeyUsage[7] = booleanValue;
                    break;
                }

                case "decipherOnly": {
                    requiredKeyUsage[8] = booleanValue;
                    break;
                }
            }
        }

        /// <summary>Gets required key usage bits.</summary>
        /// <returns>required key usage bits</returns>
        public virtual bool?[] GetKeyUsageBits() {
            return requiredKeyUsage;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual bool CheckCriteria(IX509Certificate certificate) {
            bool[] keyUsage = certificate.GetKeyUsage();
            if (keyUsage == null || keyUsage.Length != requiredKeyUsage.Length) {
                return false;
            }
            for (int i = 0; i < keyUsage.Length; ++i) {
                if (requiredKeyUsage[i] == null) {
                    continue;
                }
                if (keyUsage[i] != requiredKeyUsage[i]) {
                    return false;
                }
            }
            return true;
        }
    }
}
