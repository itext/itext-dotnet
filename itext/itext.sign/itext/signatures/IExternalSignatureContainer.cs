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
using System.IO;
using iText.Kernel.Pdf;

namespace iText.Signatures {
    /// <summary>Interface to sign a document.</summary>
    /// <remarks>Interface to sign a document. The signing is fully done externally, including the container composition.
    ///     </remarks>
    public interface IExternalSignatureContainer {
        /// <summary>Produces the container with the signature.</summary>
        /// <param name="data">the data to sign</param>
        /// <returns>a container with the signature and other objects, like CRL and OCSP. The container will generally be a PKCS7 one.
        ///     </returns>
        byte[] Sign(Stream data);

        /// <summary>Modifies the signature dictionary to suit the container.</summary>
        /// <remarks>
        /// Modifies the signature dictionary to suit the container. At least the keys
        /// <see cref="iText.Kernel.Pdf.PdfName.Filter"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfName.SubFilter"/>
        /// will have to be set.
        /// </remarks>
        /// <param name="signDic">the signature dictionary</param>
        void ModifySigningDictionary(PdfDictionary signDic);
    }
}
