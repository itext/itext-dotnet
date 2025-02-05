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
using iText.Kernel.Pdf.Tagutils;

namespace iText.Pdfua.Checkers.Utils {
    /// <summary>This class keeps track of useful information when validating a PdfUaDocument.</summary>
    /// <remarks>
    /// This class keeps track of useful information when validating a PdfUaDocument.
    /// It also contains some useful utility functions that help with PDF UA validation.
    /// </remarks>
    public class PdfUAValidationContext {
        private readonly PdfDocument pdfDocument;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="PdfUAValidationContext"/>.
        /// </summary>
        /// <param name="pdfDocument">The pdfDocument where the validation is happening.</param>
        public PdfUAValidationContext(PdfDocument pdfDocument) {
            this.pdfDocument = pdfDocument;
        }

        /// <summary>Resolves the node's role to a standard role.</summary>
        /// <param name="node">The node you want to resolve the standard role for.</param>
        /// <returns>The role.</returns>
        public virtual String ResolveToStandardRole(IStructureNode node) {
            if (node == null) {
                return null;
            }
            PdfName originalRole = node.GetRole();
            if (originalRole == null) {
                return null;
            }
            return ResolveToStandardRole(originalRole.GetValue());
        }

        /// <summary>Resolves the  role to a standard role</summary>
        /// <param name="role">The role you want to resolve the standard role for.</param>
        /// <returns>The role.</returns>
        public virtual String ResolveToStandardRole(String role) {
            if (role == null) {
                return null;
            }
            IRoleMappingResolver resolver = pdfDocument.GetTagStructureContext().ResolveMappingToStandardOrDomainSpecificRole
                (role, null);
            if (resolver == null) {
                return role;
            }
            return resolver.GetRole();
        }

        /// <summary>
        /// Checks if a
        /// <see cref="iText.Kernel.Pdf.Tagging.IStructureNode"/>
        /// resolved role's is equal to the provided role.
        /// </summary>
        /// <remarks>
        /// Checks if a
        /// <see cref="iText.Kernel.Pdf.Tagging.IStructureNode"/>
        /// resolved role's is equal to the provided role.
        /// <para />
        /// Note: This  method will not check recursive mapping. So either the node's role is the provided role,
        /// or the standard role is the provided role. So we do not take into account the roles in between the mappings.
        /// </remarks>
        /// <param name="role">The role we want to check against.</param>
        /// <param name="structureNode">The structure node we want to check.</param>
        /// <returns>
        /// The
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfStructElem"/>
        /// if the role matches.
        /// </returns>
        public virtual PdfStructElem GetElementIfRoleMatches(PdfName role, IStructureNode structureNode) {
            if (structureNode == null) {
                return null;
            }
            if (!(structureNode is PdfStructElem)) {
                return null;
            }
            //We can get away with the short code without resolving it. Because we have checks in place
            //that would catch remapped standard roles and cyclic roles.
            if (role.Equals(structureNode.GetRole()) || role.GetValue().Equals(ResolveToStandardRole(structureNode))) {
                return (PdfStructElem)structureNode;
            }
            return null;
        }
    }
}
