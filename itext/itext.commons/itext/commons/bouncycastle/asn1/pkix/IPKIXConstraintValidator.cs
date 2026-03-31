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
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Commons.Bouncycastle.Asn1.Pkix {
    /// <summary>
    /// This interface represents the wrapper for PKIXConstraintValidator that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IPKIXConstraintValidator {
        /// <summary>
        /// Calls actual
        /// <c>checkPermittedDN</c>
        /// method for the wrapped PKIXConstraintValidator object.
        /// </summary>
        /// <param name="dns">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Sequence"/>
        /// direct name sequence wrapper to be checked
        /// </param>
        void CheckPermittedDN(IAsn1Sequence dns);

        /// <summary>
        /// Calls actual
        /// <c>checkExcludedDN</c>
        /// method for the wrapped PKIXConstraintValidator object.
        /// </summary>
        /// <param name="dns">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.IAsn1Sequence"/>
        /// direct name sequence wrapper to be checked
        /// </param>
        void CheckExcludedDN(IAsn1Sequence dns);

        /// <summary>
        /// Calls actual
        /// <c>checkPermitted</c>
        /// method for the wrapped PKIXConstraintValidator object.
        /// </summary>
        /// <param name="name">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IGeneralName"/>
        /// general name wrapper to be checked
        /// </param>
        void CheckPermitted(IGeneralName name);

        /// <summary>
        /// Calls actual
        /// <c>checkExcluded</c>
        /// method for the wrapped PKIXConstraintValidator object.
        /// </summary>
        /// <param name="name">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IGeneralName"/>
        /// general name wrapper to be checked
        /// </param>
        void CheckExcluded(IGeneralName name);

        /// <summary>
        /// Calls actual
        /// <c>intersectPermittedSubtree</c>
        /// method for the wrapped PKIXConstraintValidator object.
        /// </summary>
        /// <param name="permitted">sequence of GeneralSubtree wrappers</param>
        void IntersectPermittedSubtree(IGeneralSubtree[] permitted);

        /// <summary>
        /// Calls actual
        /// <c>addExcludedSubtree</c>
        /// method for the wrapped PKIXConstraintValidator object.
        /// </summary>
        /// <param name="subtree">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.X509.IGeneralSubtree"/>
        /// wrapper
        /// </param>
        void AddExcludedSubtree(IGeneralSubtree subtree);
    }
}
