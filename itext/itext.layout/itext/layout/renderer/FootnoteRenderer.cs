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
using iText.Commons.Internal.Runtime;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Layout.Tagging;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Renderer for
    /// <see cref="iText.Layout.Properties.Margins.Footnote"/>
    /// representing a footnote placed at the bottom of the page.
    /// </summary>
    public class FootnoteRenderer : BlockRenderer {
        /// <summary>
        /// Creates a
        /// <see cref="FootnoteRenderer"/>
        /// from its corresponding layout object.
        /// </summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Properties.Margins.Footnote"/>
        /// which this object should manage
        /// </param>
        public FootnoteRenderer(Footnote modelElement)
            : base(modelElement) {
        }

        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.FootnoteRenderer), this.GetType());
            return new iText.Layout.Renderer.FootnoteRenderer((Footnote)modelElement);
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            ApplyDefaultStyleToInjectedFootnoteAnchor();
            return base.Layout(layoutContext);
        }

        public override void Draw(DrawContext drawContext) {
            LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
            FootnoteTaggingHelper.RepairFootnoteTagIfNeeded(this, taggingHelper);
            if (!childRenderers.IsEmpty() && !childRenderers[0].GetChildRenderers().IsEmpty()) {
                IRenderer footnoteParagraphContainer = childRenderers[0];
                IRenderer footnoteAnchorContent = footnoteParagraphContainer.GetChildRenderers()[0];
                if (taggingHelper != null && taggingHelper.IsArtifact(this)) {
                    // We remove these properties in case tagging is enabled, but tag is marked as artifact.
                    // We need to do that in order to not create link annotation and destinations,
                    // because annotations need to be tagged. But since this content is artifact, we can't properly tag it.
                    footnoteAnchorContent.SetProperty(Property.LINK_ANNOTATION, null);
                    footnoteAnchorContent.SetProperty(Property.DESTINATION, null);
                }
                FootnoteTaggingHelper.WrapAnchorInsideFootnoteIntoLbl(footnoteAnchorContent, taggingHelper);
            }
            base.Draw(drawContext);
        }

        private void ApplyDefaultStyleToInjectedFootnoteAnchor() {
            Footnote footnote = (Footnote)modelElement;
            if (!FootnotesUtil.IsDefaultStyleNeededForInjectedFootnoteAnchor(footnote)) {
                return;
            }
            UnitValue resolvedFontSize = null;
            if (!footnote.GetChildren().IsEmpty() && footnote.GetChildren()[0] is Paragraph) {
                Paragraph paragraph = (Paragraph)footnote.GetChildren()[0];
                resolvedFontSize = paragraph.GetProperty<UnitValue>(Property.FONT_SIZE);
            }
            if (resolvedFontSize == null) {
                resolvedFontSize = footnote.GetProperty<UnitValue>(Property.FONT_SIZE);
            }
            if (resolvedFontSize == null) {
                // Renderer lookup resolves inheritable properties from parent renderers.
                resolvedFontSize = this.GetProperty<UnitValue>(Property.FONT_SIZE);
            }
            IElement injectedAnchor = FootnotesUtil.GetInjectedFootnoteAnchor(footnote);
            Style defaultStyle = FootnotesUtil.CreateDefaultFootnoteAnchorStyle(resolvedFontSize);
            if (injectedAnchor is Text) {
                ((Text)injectedAnchor).AddStyleIfAbsent(defaultStyle);
            }
            else {
                if (injectedAnchor is Image) {
                    ((Image)injectedAnchor).AddStyleIfAbsent(defaultStyle);
                }
            }
        }
    }
}
