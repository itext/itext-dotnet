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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Signatures.Validation.Lotl.Criteria;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Class representing Qualifications entry from a country specific Trusted List.</summary>
    public class QualifierExtension {
        private readonly IList<String> qualifiers = new List<String>();

        private CriteriaList criteriaList;

//\cond DO_NOT_DOCUMENT
        internal QualifierExtension() {
        }
//\endcond

        /// <summary>Gets list of qualifiers from this extension.</summary>
        /// <returns>list of qualifiers</returns>
        public virtual IList<String> GetQualifiers() {
            return JavaCollectionsUtil.UnmodifiableList(qualifiers);
        }

        /// <summary>Checks criteria for this Qualifier extension.</summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// for which criteria shall be meet
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if criteria were meet,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool CheckCriteria(IX509Certificate certificate) {
            return criteriaList.CheckCriteria(certificate);
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void SetCriteriaList(CriteriaList criteriaList) {
            this.criteriaList = criteriaList;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual CriteriaList GetCriteriaList() {
            return criteriaList;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddQualifier(String qualifier) {
            this.qualifiers.Add(qualifier);
        }
//\endcond
    }
}
