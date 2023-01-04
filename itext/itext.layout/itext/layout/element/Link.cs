/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
