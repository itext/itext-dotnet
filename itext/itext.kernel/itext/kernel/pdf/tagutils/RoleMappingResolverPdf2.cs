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
using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
//\cond DO_NOT_DOCUMENT
    internal class RoleMappingResolverPdf2 : IRoleMappingResolver {
        private PdfName currRole;

        private PdfNamespace currNamespace;

        private PdfNamespace defaultNamespace;

//\cond DO_NOT_DOCUMENT
        internal RoleMappingResolverPdf2(String role, PdfNamespace @namespace, PdfDocument document) {
            this.currRole = PdfStructTreeRoot.ConvertRoleToPdfName(role);
            this.currNamespace = @namespace;
            String defaultNsName = StandardNamespaces.GetDefault();
            PdfDictionary defaultNsRoleMap = document.GetStructTreeRoot().GetRoleMap();
            this.defaultNamespace = new PdfNamespace(defaultNsName).SetNamespaceRoleMap(defaultNsRoleMap);
            if (currNamespace == null) {
                currNamespace = defaultNamespace;
            }
        }
//\endcond

        public virtual String GetRole() {
            return currRole.GetValue();
        }

        public virtual PdfNamespace GetNamespace() {
            return currNamespace;
        }

        public virtual bool CurrentRoleIsStandard() {
            String roleStrVal = currRole.GetValue();
            bool stdRole17 = StandardNamespaces.PDF_1_7.Equals(currNamespace.GetNamespaceName()) && StandardNamespaces
                .RoleBelongsToStandardNamespace(roleStrVal, StandardNamespaces.PDF_1_7);
            bool stdRole20 = StandardNamespaces.PDF_2_0.Equals(currNamespace.GetNamespaceName()) && StandardNamespaces
                .RoleBelongsToStandardNamespace(roleStrVal, StandardNamespaces.PDF_2_0);
            return stdRole17 || stdRole20;
        }

        public virtual bool CurrentRoleShallBeMappedToStandard() {
            return !CurrentRoleIsStandard() && !StandardNamespaces.IsKnownDomainSpecificNamespace(currNamespace);
        }

        public virtual bool ResolveNextMapping() {
            PdfObject mapping = null;
            PdfDictionary currNsRoleMap = currNamespace.GetNamespaceRoleMap();
            if (currNsRoleMap != null) {
                mapping = currNsRoleMap.Get(currRole);
            }
            if (mapping == null) {
                return false;
            }
            bool mappingWasResolved = false;
            if (mapping.IsName()) {
                currRole = (PdfName)mapping;
                currNamespace = defaultNamespace;
                mappingWasResolved = true;
            }
            else {
                if (mapping.IsArray()) {
                    PdfName mappedRole = null;
                    PdfDictionary mappedNsDict = null;
                    PdfArray mappingArr = (PdfArray)mapping;
                    if (mappingArr.Size() > 1) {
                        mappedRole = mappingArr.GetAsName(0);
                        mappedNsDict = mappingArr.GetAsDictionary(1);
                    }
                    mappingWasResolved = mappedRole != null && mappedNsDict != null;
                    if (mappingWasResolved) {
                        currRole = mappedRole;
                        currNamespace = new PdfNamespace(mappedNsDict);
                    }
                }
            }
            return mappingWasResolved;
        }
    }
//\endcond
}
