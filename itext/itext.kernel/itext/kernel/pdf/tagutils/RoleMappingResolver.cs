/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
//\cond DO_NOT_DOCUMENT
    internal class RoleMappingResolver : IRoleMappingResolver {
        private PdfName currRole;

        private PdfDictionary roleMap;

//\cond DO_NOT_DOCUMENT
        internal RoleMappingResolver(String role, PdfDocument document) {
            this.currRole = PdfStructTreeRoot.ConvertRoleToPdfName(role);
            this.roleMap = document.GetStructTreeRoot().GetRoleMap();
        }
//\endcond

        public virtual String GetRole() {
            return currRole.GetValue();
        }

        public virtual PdfNamespace GetNamespace() {
            return null;
        }

        public virtual bool CurrentRoleIsStandard() {
            return StandardNamespaces.RoleBelongsToStandardNamespace(currRole.GetValue(), StandardNamespaces.PDF_1_7);
        }

        public virtual bool CurrentRoleShallBeMappedToStandard() {
            return !CurrentRoleIsStandard();
        }

        public virtual bool ResolveNextMapping() {
            PdfName mappedRole = roleMap.GetAsName(currRole);
            if (mappedRole == null) {
                return false;
            }
            currRole = mappedRole;
            return true;
        }
    }
//\endcond
}
