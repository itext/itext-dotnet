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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A clickable piece of
    /// <see cref="Text"/>
    /// which contains a
    /// <see cref="iText.Kernel.Pdf.Annot.PdfLinkAnnotation">link annotation dictionary</see>.
    /// </summary>
    /// <remarks>
    /// A clickable piece of
    /// <see cref="Text"/>
    /// which contains a
    /// <see cref="iText.Kernel.Pdf.Annot.PdfLinkAnnotation">link annotation dictionary</see>
    /// . The concept is largely similar to that of the
    /// HTML anchor tag.
    /// </remarks>
    public class Link : Text {
        /// <summary>Creates a Link with a fully constructed link annotation dictionary.</summary>
        /// <param name="text">the textual contents of the link</param>
        /// <param name="linkAnnotation">
        /// a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfLinkAnnotation"/>
        /// </param>
        public Link(String text, PdfLinkAnnotation linkAnnotation)
            : base(text) {
            SetProperty(Property.LINK_ANNOTATION, linkAnnotation);
        }

        /// <summary>Creates a Link which can execute an action.</summary>
        /// <param name="text">the textual contents of the link</param>
        /// <param name="action">
        /// a
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// </param>
        public Link(String text, PdfAction action)
            : this(text, (PdfLinkAnnotation)new PdfLinkAnnotation(new Rectangle(0, 0, 0, 0)).SetAction(action).SetFlags
                (PdfAnnotation.PRINT)) {
        }

        /// <summary>Creates a Link to another location in the document.</summary>
        /// <param name="text">the textual contents of the link</param>
        /// <param name="destination">
        /// a
        /// <see cref="iText.Kernel.Pdf.Navigation.PdfDestination"/>
        /// </param>
        public Link(String text, PdfDestination destination)
            : this(text, (PdfLinkAnnotation)new PdfLinkAnnotation(new Rectangle(0, 0, 0, 0)).SetDestination(destination
                ).SetFlags(PdfAnnotation.PRINT)) {
        }

        /// <summary>Gets the link annotation dictionary associated with this link.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Annot.PdfLinkAnnotation"/>
        /// </returns>
        public virtual PdfLinkAnnotation GetLinkAnnotation() {
            return this.GetProperty<PdfLinkAnnotation>(Property.LINK_ANNOTATION);
        }

        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.LINK);
            }
            return tagProperties;
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new LinkRenderer(this, text);
        }
    }
}
