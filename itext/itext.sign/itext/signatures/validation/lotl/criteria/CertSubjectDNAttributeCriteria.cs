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
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Security.Certificates;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;

namespace iText.Signatures.Validation.Lotl.Criteria {
    /// <summary>Class corresponding to CertSubjectDNAttribute criteria in TL.</summary>
    public class CertSubjectDNAttributeCriteria : iText.Signatures.Validation.Lotl.Criteria.Criteria {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private readonly IList<String> requiredAttributeIds = new List<String>();

        /// <summary>
        /// Creates a new instance of
        /// <see cref="CertSubjectDNAttributeCriteria"/>.
        /// </summary>
        public CertSubjectDNAttributeCriteria() {
        }

        // Empty constructor
        /// <summary>Adds required attribute ID into the criteria.</summary>
        /// <param name="requiredAttributeId">required attribute ID</param>
        public virtual void AddRequiredAttributeId(String requiredAttributeId) {
            requiredAttributeIds.Add(requiredAttributeId);
        }

        /// <summary>Gets the required attribute IDs.</summary>
        /// <returns>required attribute IDs</returns>
        public virtual IList<String> GetRequiredAttributeIds() {
            return new List<String>(requiredAttributeIds);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual bool CheckCriteria(IX509Certificate certificate) {
            try {
                IList<String> subjectAttributes = JavaUtil.ArraysToEnumerable(certificate.GetSubjectAttributeTypes()).Select
                    ((asn1Attribute) => asn1Attribute.GetId()).ToList();
                foreach (String requiredAttributeId in requiredAttributeIds) {
                    if (!subjectAttributes.Contains(requiredAttributeId)) {
                        return false;
                    }
                }
                return true;
            }
            catch (CertificateEncodingException) {
                return false;
            }
        }
    }
}
