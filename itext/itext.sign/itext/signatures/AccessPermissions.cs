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
namespace iText.Signatures {
    /// <summary>Access permissions value to be set to certification signature as a part of DocMDP configuration.</summary>
    public enum AccessPermissions {
        /// <summary>Unspecified access permissions value which makes signature "approval" rather than "certification".
        ///     </summary>
        UNSPECIFIED,
        /// <summary>Access permissions level 1 which indicates that no changes are permitted except for DSS and DTS creation.
        ///     </summary>
        NO_CHANGES_PERMITTED,
        /// <summary>
        /// Access permissions level 2 which indicates that permitted changes, with addition to level 1, are:
        /// filling in forms, instantiating page templates, and signing.
        /// </summary>
        FORM_FIELDS_MODIFICATION,
        /// <summary>
        /// Access permissions level 3 which indicates that permitted changes, with addition to level 2, are:
        /// annotation creation, deletion and modification.
        /// </summary>
        ANNOTATION_MODIFICATION
    }
}
