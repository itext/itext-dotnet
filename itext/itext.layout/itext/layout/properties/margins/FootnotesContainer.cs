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
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Class representing container to store
    /// <see cref="Footnote"/>
    /// instances.
    /// </summary>
    internal class FootnotesContainer : BlockElement<iText.Layout.Properties.Margins.FootnotesContainer> {
        private readonly int pageNumber;

        private readonly IDictionary<IPropertyContainer, TaggingHintKey> footnoteTaggingHints = new Dictionary<IPropertyContainer
            , TaggingHintKey>();

        protected internal DefaultAccessibilityProperties tagProperties;

        /// <summary>
        /// Creates new
        /// <see cref="FootnotesContainer"/>
        /// instance.
        /// </summary>
        /// <param name="pageNum">number of the page to which this container will be added</param>
        public FootnotesContainer(int pageNum) {
            this.pageNumber = pageNum;
            this.SetNeutralRole();
        }

        /// <summary>
        /// Adds
        /// <see cref="Footnote"/>
        /// to this container.
        /// </summary>
        /// <param name="footnote">
        /// 
        /// <see cref="Footnote"/>
        /// to add
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="FootnotesContainer"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.FootnotesContainer Add(Footnote footnote, TaggingHintKey taggingHint
            ) {
            this.childElements.Add(footnote);
            footnoteTaggingHints.Put(footnote, taggingHint);
            return this;
        }

        /// <summary>Adds footnoted from another FootnotesContainer.</summary>
        /// <param name="otherContainer">the other FootnotesContainer to add the footnotes from</param>
        public virtual void AddFootnotesFromOtherContainer(iText.Layout.Properties.Margins.FootnotesContainer otherContainer
            ) {
            foreach (IElement childElement in otherContainer.childElements) {
                if (childElement is Footnote) {
                    Add((Footnote)childElement, otherContainer.footnoteTaggingHints.Get(childElement));
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override IRenderer CreateRendererSubTree() {
            IRenderer rendererRoot = GetRenderer();
            foreach (IElement child in childElements) {
                if (child is Footnote) {
                    Footnote footnote = (Footnote)child;
                    footnote.ApplyFootnoteAnchor(this.pageNumber);
                }
                IRenderer childRenderer = child.CreateRendererSubTree();
                if (footnoteTaggingHints.ContainsKey(child)) {
                    childRenderer.SetProperty(Property.TAGGING_HINT_KEY, footnoteTaggingHints.Get(child));
                }
                rendererRoot.AddChild(childRenderer);
            }
            return rendererRoot;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.P);
            }
            return tagProperties;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected internal override IRenderer MakeNewRenderer() {
            return new FootnotesContainerRenderer(this);
        }
    }
//\endcond
}
