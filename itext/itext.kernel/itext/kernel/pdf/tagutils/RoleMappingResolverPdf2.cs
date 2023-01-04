/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    internal class RoleMappingResolverPdf2 : IRoleMappingResolver {
        private PdfName currRole;

        private PdfNamespace currNamespace;

        private PdfNamespace defaultNamespace;

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
}
