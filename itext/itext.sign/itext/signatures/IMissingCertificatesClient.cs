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
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures {
    /// <summary>Interface client to support Certificate Chain with Missing Certificates.</summary>
    public interface IMissingCertificatesClient {
        /// <summary>Retrieves missing certificates in chain using Authority Information Access (AIA) Extension.</summary>
        /// <param name="chain">certificate chain to restore with at least signing certificate.</param>
        /// <returns>
        /// full chain of trust or maximum chain that could be restored in case missing certificates cannot be
        /// retrieved from AIA extension.
        /// </returns>
        IX509Certificate[] RetrieveMissingCertificates(IX509Certificate[] chain);
    }
}
