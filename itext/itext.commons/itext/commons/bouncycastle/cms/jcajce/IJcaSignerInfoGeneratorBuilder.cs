/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Operator;

namespace iText.Commons.Bouncycastle.Cms.Jcajce {
    /// <summary>
    /// This interface represents the wrapper for JcaSignerInfoGeneratorBuilder that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IJcaSignerInfoGeneratorBuilder {
        /// <summary>
        /// Calls actual
        /// <c>build</c>
        /// method for the wrapped JcaSignerInfoGeneratorBuilder object.
        /// </summary>
        /// <param name="signer">ContentSigner wrapper</param>
        /// <param name="cert">X509Certificate</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cms.ISignerInfoGenerator"/>
        /// the wrapper for built SignerInfoGenerator object.
        /// </returns>
        ISignerInfoGenerator Build(IContentSigner signer, IX509Certificate cert);
    }
}
