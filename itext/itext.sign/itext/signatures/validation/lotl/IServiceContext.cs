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
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Interface for managing service context related to certificates.</summary>
    /// <remarks>
    /// Interface for managing service context related to certificates.
    /// This interface provides methods to retrieve and add certificates.
    /// </remarks>
    public interface IServiceContext {
        /// <summary>Retrieves the list of certificates associated with the service context.</summary>
        /// <returns>
        /// a list of
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// objects
        /// </returns>
        IList<IX509Certificate> GetCertificates();

        /// <summary>Adds a certificate to the service context.</summary>
        /// <param name="certificate">the certificate to be added</param>
        void AddCertificate(IX509Certificate certificate);
    }
}
