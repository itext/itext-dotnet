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
using System.Collections;
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;

namespace iText.Signatures.Validation.Lotl.Criteria {
    /// <summary>Extended Key Usage Criteria implementation from a TL.</summary>
    public class ExtendedKeyUsageCriteria : iText.Signatures.Validation.Lotl.Criteria.Criteria {
        private readonly IList<String> requiredExtendedKeyUsages = new List<String>();

        /// <summary>
        /// Creates new instance of
        /// <see cref="ExtendedKeyUsageCriteria"/>.
        /// </summary>
        public ExtendedKeyUsageCriteria() {
        }

        // Empty constructor
        /// <summary>Adds required extended key usage.</summary>
        /// <param name="requiredExtendedKeyUsage">
        /// 
        /// <see cref="System.String"/>
        /// required extended key usage.
        /// </param>
        public virtual void AddRequiredExtendedKeyUsage(String requiredExtendedKeyUsage) {
            requiredExtendedKeyUsages.Add(requiredExtendedKeyUsage);
        }

        /// <summary>Gets the required extended key usages.</summary>
        /// <returns>the required extended key usages</returns>
        public virtual IList<String> GetRequiredExtendedKeyUsages() {
            return new List<String>(requiredExtendedKeyUsages);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual bool CheckCriteria(IX509Certificate certificate) {
            try {
                IList extendedKeyUsage = certificate.GetExtendedKeyUsage();
                foreach (String requiredExtendedKeyUsage in requiredExtendedKeyUsages) {
                    if (!extendedKeyUsage.Contains(requiredExtendedKeyUsage)) {
                        return false;
                    }
                }
            }
            catch (AbstractCertificateParsingException) {
                return false;
            }
            return true;
        }
    }
}
