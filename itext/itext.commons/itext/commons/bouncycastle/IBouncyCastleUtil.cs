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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Bouncycastle.Cert;

namespace iText.Commons.Bouncycastle {
    /// <summary>
    /// This class contains util methods, which use bouncy-castle objects.
    /// </summary>
    public interface IBouncyCastleUtil {
        /// <summary>
        /// Read <see cref="IX509Certificate"/> objects encoded in PKCS7 format
        /// </summary>
        /// <param name="data"><see cref="Stream"/> containing PKCS7 encoded certificates</param>
        /// <returns><see cref="List<IX509Certificate>"/> of certificates</returns>
        List<IX509Certificate> ReadPkcs7Certs(Stream data);
    }
}
