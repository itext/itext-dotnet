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
using System.Collections.Generic;
using iText.Commons.Internal.Runtime;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Tagging;

namespace iText.Layout.Properties.Margins {
    /// <summary>
    /// Class representing a single footnote which is a note placed at the bottom of the page
    /// indicated in the text with superscript numbers (or letters or other symbols).
    /// </summary>
    public class Footnote : AbstractElement<iText.Layout.Properties.Margins.Footnote>, IAccessibleElement {
//\cond DO_NOT_DOCUMENT
        internal readonly IDictionary<int, IElement> anchors = new Dictionary<int, IElement>();
//\endcond

        private IElement footnoteAnchor = null;

        private bool defaultStyleNeededForInjectedFootnoteAnchor = false;

        private DefaultAccessibilityProperties tagProperties;

        /// <summary>
        /// Creates new
        /// <see cref="Footnote"/>
        /// instance with text.
        /// </summary>
        /// <param name="text">the textual contents of the footnote</param>
        public Footnote(String text)
            : this(new Paragraph(text).SetMarginTop(0).SetMarginBottom(0)) {
        }

        /// <summary>
        /// Creates new
        /// <see cref="Footnote"/>
        /// instance.
        /// </summary>
        /// <param name="paragraph">
        /// 
        /// <see cref="iText.Layout.Element.Paragraph"/>
        /// representing the contents of the footnote
        /// </param>
        public Footnote(Paragraph paragraph)
            : base() {
            childElements.Add(paragraph);
            paragraph.SetNeutralRole();
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.NOTE);
            }
            return tagProperties;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets injected footnote anchor element, which is a copy of a footnote anchor in the main content.</summary>
        /// <returns>injected footnote anchor element</returns>
        internal virtual IElement GetInjectedFootnoteAnchor() {
            return footnoteAnchor;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Injects footnote anchor before placing this footnote on the specified page.</summary>
        /// <param name="pageNum">number of the page where this footnote should be placed</param>
        internal virtual void ApplyFootnoteAnchor(int pageNum) {
            if (!this.GetChildren().IsEmpty() && this.GetChildren()[0] is Paragraph) {
                Paragraph paragraph = (Paragraph)this.GetChildren()[0];
                RemoveFootnoteAnchorFromParagraph(paragraph);
                if (this.anchors.ContainsKey(pageNum)) {
                    this.footnoteAnchor = this.anchors.Get(pageNum);
                    paragraph.GetChildren().Add(0, this.footnoteAnchor);
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Injects footnote anchor into this footnote before layout.</summary>
        /// <param name="footnoteAnchor">
        /// 
        /// <see cref="FootnoteAnchor"/>
        /// to link to this footnote
        /// </param>
        internal virtual void ApplyFootnoteAnchor(FootnoteAnchor footnoteAnchor) {
            if (!this.GetChildren().IsEmpty() && this.GetChildren()[0] is Paragraph) {
                Paragraph paragraph = (Paragraph)this.GetChildren()[0];
                RemoveFootnoteAnchorFromParagraph(paragraph);
                InjectFootnoteAnchorIntoParagraph(paragraph, footnoteAnchor);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Resets current
        /// <see cref="FootnoteAnchor"/>
        /// linked to this footnote.
        /// </summary>
        internal virtual void ResetFootnoteAnchor() {
            if (!this.GetChildren().IsEmpty() && this.GetChildren()[0] is Paragraph) {
                Paragraph paragraph = (Paragraph)this.GetChildren()[0];
                RemoveFootnoteAnchorFromParagraph(paragraph);
            }
        }
//\endcond

        private void InjectFootnoteAnchorIntoParagraph(Paragraph paragraph, FootnoteAnchor footnoteAnchor) {
            IElement footnoteAnchorSymbol = CreateAnchorCopy(footnoteAnchor);
            if (footnoteAnchorSymbol == null) {
                return;
            }
            this.defaultStyleNeededForInjectedFootnoteAnchor = footnoteAnchor.IsDefaultStyleNeeded();
            this.footnoteAnchor = footnoteAnchorSymbol;
            paragraph.GetChildren().Add(0, this.footnoteAnchor);
        }

        private IElement CreateAnchorCopy(FootnoteAnchor footnoteAnchor) {
            IElement footnoteAnchorSymbol = footnoteAnchor.GetFootnoteAnchor();
            Style footnoteAnchorStyle = footnoteAnchor.GetFootnoteAnchorLabelStyle();
            if (footnoteAnchorStyle == null) {
                bool isRtl = BaseDirection.RIGHT_TO_LEFT == this.GetProperty<BaseDirection?>(Property.BASE_DIRECTION);
                footnoteAnchorStyle = new Style();
                footnoteAnchorStyle.SetProperty(isRtl ? Property.MARGIN_LEFT : Property.MARGIN_RIGHT, UnitValue.CreatePointValue
                    (5F));
            }
            if (footnoteAnchorSymbol is Text) {
                return new Text((Text)footnoteAnchorSymbol).AddStyle(footnoteAnchorStyle);
            }
            else {
                if (footnoteAnchorSymbol is Image) {
                    return new Image((Image)footnoteAnchorSymbol).AddStyle(footnoteAnchorStyle);
                }
                else {
                    if (footnoteAnchorSymbol is IAbstractElement) {
                        return ((AbstractElement<IElement>)footnoteAnchorSymbol).AddStyle(footnoteAnchorStyle);
                    }
                    else {
                        return footnoteAnchorSymbol;
                    }
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsDefaultStyleNeededForInjectedFootnoteAnchor() {
            return defaultStyleNeededForInjectedFootnoteAnchor;
        }
//\endcond

        private void RemoveFootnoteAnchorFromParagraph(Paragraph paragraph) {
            if (this.footnoteAnchor != null) {
                paragraph.GetChildren().Remove(this.footnoteAnchor);
            }
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected internal override IRenderer MakeNewRenderer() {
            return new FootnoteRenderer(this);
        }
    }
}
