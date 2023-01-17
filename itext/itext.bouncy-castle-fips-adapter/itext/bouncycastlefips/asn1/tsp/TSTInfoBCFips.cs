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
using System;
using Org.BouncyCastle.Asn1.Tsp;
using iText.Bouncycastlefips.Asn1;
using iText.Commons.Bouncycastle.Asn1.Tsp;

namespace iText.Bouncycastlefips.Asn1.Tsp {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Tsp.TstInfo"/>.
    /// </summary>
    public class TSTInfoBCFips : ASN1EncodableBCFips, ITSTInfo {
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TstInfo"/>.
        /// </summary>
        /// <param name="tstInfo">
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TstInfo"/>
        /// to be wrapped
        /// </param>
        public TSTInfoBCFips(TstInfo tstInfo)
            : base(tstInfo) {
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Asn1.Tsp.TstInfo"/>.
        /// </returns>
        public virtual TstInfo GetTstInfo() {
            return (TstInfo)GetEncodable();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IMessageImprint GetMessageImprint() {
            return new MessageImprintBCFips(GetTstInfo().MessageImprint);
        }

        /// <summary><inheritDoc/></summary>
        public virtual DateTime GetGenTime() {
            return GetTstInfo().GenTime.ToDateTime();
        }
    }
}
