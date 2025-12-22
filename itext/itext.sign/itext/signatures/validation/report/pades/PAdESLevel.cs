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
namespace iText.Signatures.Validation.Report.Pades {
    /// <summary>
    /// This enumeration holds all possible PAdES levels plus none and indeterminate, needed for when
    /// none if the levels is reached or a signature is invalid.
    /// </summary>
    public enum PAdESLevel {
        /// <summary>None of the levels criteria where met</summary>
        NONE,
        /// <summary>Unable to establish the PAdES level</summary>
        INDETERMINATE,
        /// <summary>
        /// B-B level provides requirements for the incorporation of signed and some unsigned attributes when the
        /// signature is generated.
        /// </summary>
        B_B,
        /// <summary>
        /// B-T level provides requirements for the generation and inclusion, for an existing signature, of a trusted token
        /// proving that the signature itself actually existed at a certain date and time.
        /// </summary>
        B_T,
        /// <summary>
        /// B-LT level provides requirements for the incorporation of all the material required for validating the signature
        /// in the signature document.
        /// </summary>
        /// <remarks>
        /// B-LT level provides requirements for the incorporation of all the material required for validating the signature
        /// in the signature document. This level aims to tackle the long term availability of the validation material.
        /// </remarks>
        B_LT,
        /// <summary>
        /// B-LTA level provides requirements for the incorporation of electronic timestamps that allow validation of the
        /// signature long time after its generation.
        /// </summary>
        /// <remarks>
        /// B-LTA level provides requirements for the incorporation of electronic timestamps that allow validation of the
        /// signature long time after its generation. This level aims to tackle the long term availability and integrity of
        /// the validation material.
        /// </remarks>
        B_LTA
    }
}
