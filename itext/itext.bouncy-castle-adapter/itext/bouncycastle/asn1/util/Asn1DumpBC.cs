/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Utilities;
using iText.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Util;
using iText.Commons.Utils;

namespace iText.Bouncycastle.Asn1.Util {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Asn1.Utilities.Asn1Dump"/>.
    /// </summary>
    public class Asn1DumpBC : IAsn1Dump {
        private static readonly Asn1DumpBC INSTANCE = new Asn1DumpBC();
        
        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Asn1.Utilities.Asn1Dump"/>.
        /// </summary>
        /// 
        /// <see cref="Org.BouncyCastle.Asn1.Utilities.Asn1Dump"/>
        /// to be wrapped
        /// </param>
        public Asn1DumpBC() {
        }

        /// <summary>Gets wrapper instance.</summary>
        /// <returns>
        /// 
        /// <see cref="Asn1DumpBC"/>
        /// instance.
        /// </returns>
        public static Asn1DumpBC GetInstance() {
            return INSTANCE;
        }
        
        /// <summary><inheritDoc/></summary>
        public virtual String DumpAsString(Object obj, bool b) {
            if (obj is Asn1EncodableBC) {
                return Asn1Dump.DumpAsString(((Asn1EncodableBC)obj).GetEncodable(), b);
            }
            return Asn1Dump.DumpAsString((Asn1Encodable)obj, b);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String DumpAsString(Object obj) {
            if (obj is Asn1EncodableBC) {
                return Asn1Dump.DumpAsString(((Asn1EncodableBC)obj).GetEncodable());
            }
            return Asn1Dump.DumpAsString((Asn1Encodable)obj);
        }
    }
}
