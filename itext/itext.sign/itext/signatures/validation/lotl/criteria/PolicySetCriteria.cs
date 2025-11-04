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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Validation.Lotl.Criteria {
    /// <summary>Policy Set Criteria implementation from a TL.</summary>
    public class PolicySetCriteria : iText.Signatures.Validation.Lotl.Criteria.Criteria {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private const String POLICY_SET_EXTENSION_ID = "2.5.29.32";

        private readonly IList<String> requiredPolicyIdentifiers = new List<String>();

        /// <summary>
        /// Creates new instance of
        /// <see cref="PolicySetCriteria"/>.
        /// </summary>
        public PolicySetCriteria() {
        }

        // Empty constructor
        /// <summary>Adds required policy id.</summary>
        /// <param name="requiredPolicyId">required policy id</param>
        public virtual void AddRequiredPolicyId(String requiredPolicyId) {
            requiredPolicyIdentifiers.Add(requiredPolicyId);
        }

        /// <summary>Gets the required policy IDs.</summary>
        /// <returns>the required policy IDs</returns>
        public virtual IList<String> GetRequiredPolicyIds() {
            return new List<String>(requiredPolicyIdentifiers);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual bool CheckCriteria(IX509Certificate certificate) {
            byte[] policyExtension = CertificateUtil.GetExtensionValueByOid(certificate, POLICY_SET_EXTENSION_ID);
            if (policyExtension == null) {
                return false;
            }
            try {
                IList<String> policyIds = FACTORY.GetPoliciesIds(policyExtension);
                foreach (String requiredPolicyIdentifier in requiredPolicyIdentifiers) {
                    if (!policyIds.Contains(requiredPolicyIdentifier)) {
                        return false;
                    }
                }
            }
            catch (System.IO.IOException) {
                return false;
            }
            return true;
        }
    }
}
