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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Renderer;

namespace iText.Layout.Tagging {
    /// <summary>This class is used to resolve prohibited relations between parent and child tags.</summary>
    public class ProhibitedTagRelationsResolver {
        private static readonly IDictionary<String, String> updateRules20 = new Dictionary<String, String>();

        private static readonly IDictionary<String, String> updateRules17 = new Dictionary<String, String>();

        private static readonly IList<String> rolesToSkip = JavaUtil.ArraysAsList(StandardRoles.DIV, StandardRoles
            .NONSTRUCT, null);

        private static readonly PdfAllowedTagRelations allowedRelations = new PdfAllowedTagRelations();

        private readonly PdfDocument pdfDocument;

        private readonly IDictionary<String, String> overriddenRoles = new Dictionary<String, String>();

        static ProhibitedTagRelationsResolver() {
            //PDF 1.7 rules
            updateRules17.Put(GenerateKey(StandardRoles.H, StandardRoles.P), StandardRoles.SPAN);
            updateRules17.Put(GenerateKey(StandardRoles.P, StandardRoles.P), StandardRoles.SPAN);
            updateRules17.Put(GenerateKey(StandardRoles.P, StandardRoles.DIV), StandardRoles.SPAN);
            updateRules17.Put(GenerateKey(StandardRoles.TOC, StandardRoles.SPAN), StandardRoles.CAPTION);
            updateRules17.Put(GenerateKey(StandardRoles.TOCI, StandardRoles.SPAN), StandardRoles.LBL);
            //PDF 2.0 rules
            updateRules20.Put(GenerateKey(StandardRoles.H, StandardRoles.P), StandardRoles.SUB);
            updateRules20.Put(GenerateKey(PdfAllowedTagRelations.NUMBERED_HEADER, StandardRoles.P), StandardRoles.SUB);
            updateRules20.Put(GenerateKey(StandardRoles.FORM, StandardRoles.P), StandardRoles.LBL);
            updateRules20.Put(GenerateKey(StandardRoles.FORM, StandardRoles.FORM), StandardRoles.DIV);
            updateRules20.Put(GenerateKey(StandardRoles.FORM, StandardRoles.SPAN), StandardRoles.LBL);
            updateRules20.Put(GenerateKey(StandardRoles.FORM, PdfAllowedTagRelations.NUMBERED_HEADER), StandardRoles.LBL
                );
            updateRules20.Put(GenerateKey(StandardRoles.LBL, StandardRoles.P), StandardRoles.SPAN);
            updateRules20.Put(GenerateKey(StandardRoles.P, StandardRoles.P), StandardRoles.SPAN);
            updateRules20.Put(GenerateKey(StandardRoles.P, StandardRoles.DIV), StandardRoles.SUB);
            updateRules20.Put(GenerateKey(StandardRoles.SPAN, StandardRoles.P), StandardRoles.SPAN);
            updateRules20.Put(GenerateKey(StandardRoles.SPAN, StandardRoles.DIV), StandardRoles.SUB);
            updateRules20.Put(GenerateKey(StandardRoles.SUB, StandardRoles.P), StandardRoles.SPAN);
            updateRules20.Put(GenerateKey(StandardRoles.SUB, StandardRoles.SUB), StandardRoles.SPAN);
            updateRules20.Put(GenerateKey(StandardRoles.SUB, StandardRoles.DIV), StandardRoles.SPAN);
            updateRules20.Put(GenerateKey(StandardRoles.TOC, StandardRoles.SPAN), StandardRoles.CAPTION);
            updateRules20.Put(GenerateKey(StandardRoles.TOCI, StandardRoles.SPAN), StandardRoles.LBL);
            updateRules20.Put(GenerateKey(StandardRoles.DOCUMENT, StandardRoles.SPAN), StandardRoles.P);
        }

        /// <summary>
        /// Creates a new instance of
        /// <see cref="ProhibitedTagRelationsResolver"/>.
        /// </summary>
        /// <param name="pdfDocument">the document to be processed.</param>
        public ProhibitedTagRelationsResolver(PdfDocument pdfDocument) {
            this.pdfDocument = pdfDocument;
        }

        /// <summary>Resolves prohibited relations between parent and child tags.</summary>
        /// <param name="taggingHelper">the tagging helper.</param>
        /// <param name="topRender">the top renderer.</param>
        public virtual void RepairTagStructure(LayoutTaggingHelper taggingHelper, IRenderer topRender) {
            TaggingHintKey currentThk = LayoutTaggingHelper.GetOrCreateHintKey(topRender);
            if (!currentThk.IsAccessible()) {
                return;
            }
            String normalizedParentRole = ResolveToFinalRole(taggingHelper, currentThk, false);
            foreach (IRenderer childRenderer in topRender.GetChildRenderers()) {
                if (childRenderer is AreaBreakRenderer) {
                    continue;
                }
                TaggingHintKey kid = LayoutTaggingHelper.GetOrCreateHintKey(childRenderer);
                if (!kid.IsAccessible()) {
                    continue;
                }
                //To not change the role of non-struct elements
                if (IsKidNonStructElement(kid)) {
                    continue;
                }
                String normalizedKidRole = ResolveToFinalRole(taggingHelper, kid, true);
                String key = GenerateKey(normalizedParentRole, normalizedKidRole);
                ExecuteRoleReplacementRule(kid, key);
            }
        }

        /// <summary>Overwrites tagging rule if it already exists.</summary>
        /// <remarks>Overwrites tagging rule if it already exists. Otherwise, adds the new rule.</remarks>
        /// <param name="parentRole">The parent role.</param>
        /// <param name="childRole">The child role.</param>
        /// <param name="newRole">The new role the child should have.</param>
        public virtual void OverwriteTaggingRule(String parentRole, String childRole, String newRole) {
            overriddenRoles.Put(GenerateKey(parentRole, childRole), newRole);
        }

        private void ExecuteRoleReplacementRule(TaggingHintKey kid, String key) {
            IDictionary<String, String> updateRules = PdfVersion.PDF_2_0.Equals(pdfDocument.GetPdfVersion()) ? updateRules20
                 : updateRules17;
            if (updateRules.ContainsKey(key)) {
                kid.SetOverriddenRole(updateRules.Get(key));
            }
            if (overriddenRoles.ContainsKey(key)) {
                kid.SetOverriddenRole(overriddenRoles.Get(key));
            }
        }

        private static bool IsKidNonStructElement(TaggingHintKey kid) {
            if (kid.GetAccessibleElement() == null) {
                return false;
            }
            if (kid.GetAccessibleElement().GetAccessibilityProperties() == null) {
                return false;
            }
            return StandardRoles.NONSTRUCT.Equals(kid.GetAccessibleElement().GetAccessibilityProperties().GetRole()) ||
                 StandardRoles.NONSTRUCT.Equals(kid.GetOverriddenRole());
        }

        private static String GenerateKey(String parentRole, String childRole) {
            return parentRole + ":" + childRole;
        }

        private String ResolveToFinalRole(LayoutTaggingHelper helper, TaggingHintKey taggingHintKey, bool isKid) {
            String role = taggingHintKey.GetAccessibilityProperties().GetRole();
            if (taggingHintKey.GetOverriddenRole() != null) {
                role = taggingHintKey.GetOverriddenRole();
            }
            role = ResolveToStandardRole(role);
            role = allowedRelations.NormalizeRole(role);
            if (isKid) {
                return role;
            }
            if (rolesToSkip.Contains(role)) {
                return GetParentRole(helper, taggingHintKey, rolesToSkip);
            }
            return role;
        }

        private String GetParentRole(LayoutTaggingHelper helper, TaggingHintKey hintKey, IList<String> rolesToSkip
            ) {
            String currentRole = hintKey.GetAccessibilityProperties().GetRole();
            if (hintKey.GetOverriddenRole() != null) {
                currentRole = hintKey.GetOverriddenRole();
            }
            currentRole = ResolveToStandardRole(currentRole);
            if (!rolesToSkip.Contains(currentRole)) {
                return currentRole;
            }
            TaggingHintKey parent = helper.GetAccessibleParentHint(hintKey);
            if (parent == null) {
                return null;
            }
            return GetParentRole(helper, parent, rolesToSkip);
        }

        private String ResolveToStandardRole(String role) {
            if (role == null) {
                return null;
            }
            TagStructureContext tagStructureContext = pdfDocument.GetTagStructureContext();
            IRoleMappingResolver resolver = tagStructureContext.ResolveMappingToStandardOrDomainSpecificRole(role, tagStructureContext
                .GetDocumentDefaultNamespace());
            if (resolver == null) {
                return role;
            }
            return resolver.GetRole();
        }
    }
}
