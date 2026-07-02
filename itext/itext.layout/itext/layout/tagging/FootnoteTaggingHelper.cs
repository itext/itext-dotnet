/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Layout;
using iText.Layout.Renderer;

namespace iText.Layout.Tagging {
    /// <summary>
    /// The class is a helper which is used to correctly create structure
    /// tree for Footnote elements.
    /// </summary>
    public sealed class FootnoteTaggingHelper {
        private FootnoteTaggingHelper() {
            throw new InvalidOperationException("Utility class");
        }

        /// <summary>
        /// Adjusts the tag roles of the
        /// <see cref="iText.Layout.Properties.Margins.Footnote"/>
        /// element when required by the PdfVersion targeted.
        /// </summary>
        /// <param name="hintOwner">
        /// the
        /// <see cref="iText.Layout.Renderer.FootnoteRenderer"/>
        /// to repair
        /// </param>
        /// <param name="taggingHelper">
        /// the
        /// <see cref="LayoutTaggingHelper"/>
        /// instance to use
        /// </param>
        public static void RepairFootnoteTagIfNeeded(FootnoteRenderer hintOwner, LayoutTaggingHelper taggingHelper
            ) {
            TaggingHintKey hint = LayoutTaggingHelper.GetOrCreateHintKey(hintOwner);
            if (taggingHelper != null && hint.IsAccessible()) {
                PdfVersion targetVersion = taggingHelper.GetPdfDocument().GetTagStructureContext().GetTagStructureTargetVersion
                    ();
                if (targetVersion.CompareTo(PdfVersion.PDF_2_0) >= 0 && StandardRoles.NOTE.Equals(hint.GetAccessibleElement
                    ().GetAccessibilityProperties().GetRole())) {
                    hint.SetOverriddenRole(StandardRoles.FENOTE);
                }
                else {
                    if (hint.GetAccessibleElement().GetAccessibilityProperties().GetStructureElementId() == null) {
                        hint.GetAccessibleElement().GetAccessibilityProperties().SetStructureElementIdString(taggingHelper.CreateStructureElementId
                            ("footnote_"));
                    }
                }
            }
        }

        /// <summary>Wraps the FootnoteAnchor content element with a dummy element.</summary>
        /// <param name="footnoteAnchorContent">the FootnoteAnchor content element to wrap.</param>
        /// <param name="taggingHelper">
        /// the
        /// <see cref="LayoutTaggingHelper"/>
        /// instance to use
        /// </param>
        public static void WrapAnchorInsideFootnoteIntoLbl(IPropertyContainer footnoteAnchorContent, LayoutTaggingHelper
             taggingHelper) {
            if (taggingHelper != null) {
                TaggingHintKey footnoteAnchorContentHint = LayoutTaggingHelper.GetHintKey(footnoteAnchorContent);
                TaggingDummyElement lblParentForFootnoteAnchorContent = new TaggingDummyElement(StandardRoles.LBL);
                TaggingHintKey lblHint = LayoutTaggingHelper.GetOrCreateHintKey(lblParentForFootnoteAnchorContent);
                taggingHelper.ReplaceKidHint(footnoteAnchorContentHint, JavaCollectionsUtil.SingletonList(lblHint));
                taggingHelper.AddKidsHint(lblHint, JavaCollectionsUtil.SingletonList(footnoteAnchorContentHint));
            }
        }

        /// <summary>Adjusts the tag roles when required by the PdfVersion targeted.</summary>
        /// <param name="hintOwner">
        /// the
        /// <see cref="iText.Layout.Renderer.FootnoteAnchorRenderer"/>
        /// to repair
        /// </param>
        /// <param name="taggingHelper">
        /// the
        /// <see cref="LayoutTaggingHelper"/>
        /// instance to use
        /// </param>
        public static void RepairFootnoteAnchorTagIfNeeded(FootnoteAnchorRenderer hintOwner, LayoutTaggingHelper taggingHelper
            ) {
            TaggingHintKey hint = LayoutTaggingHelper.GetOrCreateHintKey(hintOwner);
            if (taggingHelper != null && hint.IsAccessible()) {
                PdfVersion targetVersion = taggingHelper.GetPdfDocument().GetTagStructureContext().GetTagStructureTargetVersion
                    ();
                if (targetVersion.CompareTo(PdfVersion.PDF_2_0) >= 0 && StandardRoles.REFERENCE.Equals(hint.GetAccessibleElement
                    ().GetAccessibilityProperties().GetRole())) {
                    hint.SetOverriddenRole(StandardRoles.LBL);
                }
            }
        }
    }
}
