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

namespace iText.Signatures.Validation.Lotl.Criteria {
    /// <summary>Criteria List which holds other Criteria or other Criteria Lists.</summary>
    public class CriteriaList : iText.Signatures.Validation.Lotl.Criteria.Criteria {
        private readonly IList<iText.Signatures.Validation.Lotl.Criteria.Criteria> criterias = new List<iText.Signatures.Validation.Lotl.Criteria.Criteria
            >();

        private readonly String assertValue;

        /// <summary>Creates a new instance of a Criteria List with a provided assert value.</summary>
        /// <param name="assertValue">assert value. Possible value are "all", "atLeastOne" and "none".</param>
        public CriteriaList(String assertValue) {
            this.assertValue = assertValue;
        }

        /// <summary>Gets assert value for this Criteria List.</summary>
        /// <returns>assert value</returns>
        public virtual String GetAssertValue() {
            return assertValue;
        }

        /// <summary>
        /// Adds
        /// <see cref="Criteria"/>
        /// to this Criteria List.
        /// </summary>
        /// <param name="criteria">
        /// 
        /// <see cref="Criteria"/>
        /// to be added
        /// </param>
        public virtual void AddCriteria(iText.Signatures.Validation.Lotl.Criteria.Criteria criteria) {
            criterias.Add(criteria);
        }

        /// <summary>Gets Criteria List.</summary>
        /// <returns>Criteria List</returns>
        public virtual IList<iText.Signatures.Validation.Lotl.Criteria.Criteria> GetCriteriaList() {
            return new List<iText.Signatures.Validation.Lotl.Criteria.Criteria>(criterias);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual bool CheckCriteria(IX509Certificate certificate) {
            switch (assertValue) {
                case "all": {
                    foreach (iText.Signatures.Validation.Lotl.Criteria.Criteria criteria in criterias) {
                        if (!criteria.CheckCriteria(certificate)) {
                            return false;
                        }
                    }
                    return true;
                }

                case "atLeastOne": {
                    foreach (iText.Signatures.Validation.Lotl.Criteria.Criteria criteria in criterias) {
                        if (criteria.CheckCriteria(certificate)) {
                            return true;
                        }
                    }
                    return false;
                }

                case "none": {
                    foreach (iText.Signatures.Validation.Lotl.Criteria.Criteria criteria in criterias) {
                        if (criteria.CheckCriteria(certificate)) {
                            return false;
                        }
                    }
                    return true;
                }

                default: {
                    return false;
                }
            }
        }
    }
}
