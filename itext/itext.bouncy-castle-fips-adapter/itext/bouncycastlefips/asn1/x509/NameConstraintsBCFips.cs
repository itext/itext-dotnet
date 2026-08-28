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
using Org.BouncyCastle.Asn1.X509;
using iText.Commons.Bouncycastle.Asn1.X509;
using Org.BouncyCastle.Asn1;

namespace iText.Bouncycastlefips.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.NameConstraints"/>
    /// </summary>
    public class NameConstraintsBCFips : Asn1EncodableBCFips, INameConstraints {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.NameConstraints"/>.
        /// </summary>
        /// <param name="nameConstraints">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.NameConstraints"/>
        /// to be wrapped
        /// </param>
        public NameConstraintsBCFips(NameConstraints nameConstraints)
            : base(nameConstraints) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.NameConstraints"/>.
        /// </returns>
        public virtual NameConstraints GetNameConstraints() {
            return (NameConstraints)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IGeneralSubtree[] GetPermittedSubtrees() {
            Asn1Sequence permittedSubtreesSequence = GetNameConstraints().PermittedSubtrees;
            if (permittedSubtreesSequence == null) {
                return new IGeneralSubtree[0];
            }

            IGeneralSubtree[] permittedSubtress = new IGeneralSubtree[permittedSubtreesSequence.Count];
            for (int i = 0; i < permittedSubtreesSequence.Count; ++i) {
                permittedSubtress[i] = new GeneralSubtreeBCFips(GeneralSubtree.GetInstance(permittedSubtreesSequence[i]));
            }
            return permittedSubtress;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IGeneralSubtree[] GetExcludedSubtrees() {
            Asn1Sequence excludedSubtreesSequence = GetNameConstraints().ExcludedSubtrees;
            if (excludedSubtreesSequence == null) {
                return new IGeneralSubtree[0];
            }

            IGeneralSubtree[] excludedSubtrees = new IGeneralSubtree[excludedSubtreesSequence.Count];
            for (int i = 0; i < excludedSubtreesSequence.Count; ++i) {
                excludedSubtrees[i] = new GeneralSubtreeBCFips(GeneralSubtree.GetInstance(excludedSubtreesSequence[i]));
            }
            return excludedSubtrees;
        }
    }
}
