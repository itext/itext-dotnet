/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using Org.BouncyCastle.Asn1.X509;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.X509;

namespace iText.Bouncycastle.Asn1.X509 {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralNames"/>.
    /// </summary>
    public class GeneralNamesBC : ASN1EncodableBC, IGeneralNames {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralNames"/>.
        /// </summary>
        /// <param name="generalNames">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralNames"/>
        /// to be wrapped
        /// </param>
        public GeneralNamesBC(GeneralNames generalNames)
            : base(generalNames) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.X509.GeneralNames"/>.
        /// </returns>
        public virtual GeneralNames GetGeneralNames() {
            return (GeneralNames)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IGeneralName[] GetNames() {
            GeneralName[] generalNames = GetGeneralNames().GetNames();
            IGeneralName[] generalNamesBC = new IGeneralName[generalNames.Length];
            for (int i = 0; i < generalNames.Length; ++i) {
                generalNamesBC[i] = new GeneralNameBC(generalNames[i]);
            }
            return generalNamesBC;
        }
    }
}
