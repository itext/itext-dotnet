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
using iText.Commons.Bouncycastle.Asn1;

namespace iText.Commons.Bouncycastle.Asn1.Pkcs {
    /// <summary>
    /// This interface represents the wrapper for PKCSObjectIdentifiers that provides the ability
    /// to switch between bouncy-castle and bouncy-castle FIPS implementations.
    /// </summary>
    public interface IPKCSObjectIdentifiers {
        /// <summary>
        /// Gets
        /// <c>id_aa_signatureTimeStampToken</c>
        /// constant for the wrapped PKCSObjectIdentifiers.
        /// </summary>
        /// <returns>PKCSObjectIdentifiers.id_aa_signatureTimeStampToken wrapper.</returns>
        IASN1ObjectIdentifier GetIdAaSignatureTimeStampToken();

        /// <summary>
        /// Gets
        /// <c>id_aa_ets_sigPolicyId</c>
        /// constant for the wrapped PKCSObjectIdentifiers.
        /// </summary>
        /// <returns>PKCSObjectIdentifiers.id_aa_ets_sigPolicyId wrapper.</returns>
        IASN1ObjectIdentifier GetIdAaEtsSigPolicyId();

        /// <summary>
        /// Gets
        /// <c>id_spq_ets_uri</c>
        /// constant for the wrapped PKCSObjectIdentifiers.
        /// </summary>
        /// <returns>PKCSObjectIdentifiers.id_spq_ets_uri wrapper.</returns>
        IASN1ObjectIdentifier GetIdSpqEtsUri();

        /// <summary>
        /// Gets
        /// <c>envelopedData</c>
        /// constant for the wrapped PKCSObjectIdentifiers.
        /// </summary>
        /// <returns>PKCSObjectIdentifiers.envelopedData wrapper.</returns>
        IASN1ObjectIdentifier GetEnvelopedData();

        /// <summary>
        /// Gets
        /// <c>data</c>
        /// constant for the wrapped PKCSObjectIdentifiers.
        /// </summary>
        /// <returns>PKCSObjectIdentifiers.data wrapper.</returns>
        IASN1ObjectIdentifier GetData();
    }
}
