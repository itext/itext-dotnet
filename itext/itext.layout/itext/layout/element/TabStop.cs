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
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Properties;

namespace iText.Layout.Element {
    /// <summary>
    /// A TabStop is the closest location on a line of text that the text will jump
    /// to if a
    /// <see cref="Tab"/>
    /// is inserted.
    /// </summary>
    /// <remarks>
    /// A TabStop is the closest location on a line of text that the text will jump
    /// to if a
    /// <see cref="Tab"/>
    /// is inserted. At least one TabStop must be defined on an
    /// element if you want to use
    /// <see cref="Tab">Tabs</see>.
    /// This object can be added to a
    /// <see cref="Paragraph"/>
    /// with the method
    /// <see cref="Paragraph.AddTabStops(TabStop[])"/>.
    /// </remarks>
    public class TabStop {
        // tabPosition here is absolute value
        private float tabPosition;

        private TabAlignment tabAlignment;

        private char? tabAnchor;

        private ILineDrawer tabLeader;

        /// <summary>Creates a TabStop at the appropriate position.</summary>
        /// <param name="tabPosition">a <c>float</c>, measured in absolute points</param>
        public TabStop(float tabPosition)
            : this(tabPosition, TabAlignment.LEFT) {
        }

        /// <summary>
        /// Creates a TabStop at the appropriate position, with a specified tab
        /// alignment.
        /// </summary>
        /// <remarks>
        /// Creates a TabStop at the appropriate position, with a specified tab
        /// alignment. A tab alignment defines the way the textual content should be
        /// positioned with regards to this tab stop.
        /// </remarks>
        /// <param name="tabPosition">a <c>float</c>, measured in absolute points</param>
        /// <param name="tabAlignment">
        /// a
        /// <see cref="iText.Layout.Properties.TabAlignment"/>
        /// value
        /// </param>
        public TabStop(float tabPosition, TabAlignment tabAlignment)
            : this(tabPosition, tabAlignment, null) {
        }

        /// <summary>
        /// Creates a TabStop at the appropriate position, with a specified tab
        /// alignment and an explicitly given line pattern.
        /// </summary>
        /// <remarks>
        /// Creates a TabStop at the appropriate position, with a specified tab
        /// alignment and an explicitly given line pattern. A tab alignment defines
        /// the way the textual content should be positioned with regards to this tab
        /// stop. The line pattern defines a pattern that should be repeated until
        /// the TabStop is reached. If null, the space leading up to the TabStop will
        /// be empty.
        /// </remarks>
        /// <param name="tabPosition">a <c>float</c>, measured in absolute points</param>
        /// <param name="tabAlignment">
        /// a
        /// <see cref="iText.Layout.Properties.TabAlignment"/>
        /// value
        /// </param>
        /// <param name="tabLeader">
        /// the
        /// <see cref="iText.Kernel.Pdf.Canvas.Draw.ILineDrawer"/>
        /// value, a pattern drawing object
        /// </param>
        public TabStop(float tabPosition, TabAlignment tabAlignment, ILineDrawer tabLeader) {
            this.tabPosition = tabPosition;
            this.tabAlignment = tabAlignment;
            this.tabLeader = tabLeader;
            this.tabAnchor = '.';
        }

        /// <summary>Returns the position of a tab stop.</summary>
        /// <returns>tabPosition, measured in absolute points</returns>
        public virtual float GetTabPosition() {
            return tabPosition;
        }

        /// <summary>
        /// Returns the alignment of a tab stop, which defines the way the textual content
        /// should be positioned in regard to this tab stop.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Layout.Properties.TabAlignment"/>
        /// value
        /// </returns>
        public virtual TabAlignment GetTabAlignment() {
            return tabAlignment;
        }

        /// <summary>
        /// Sets the alignment, which defines the way the textual content
        /// should be positioned in regard to this tab stop.
        /// </summary>
        /// <param name="tabAlignment">
        /// a
        /// <see cref="iText.Layout.Properties.TabAlignment"/>
        /// value
        /// </param>
        public virtual void SetTabAlignment(TabAlignment tabAlignment) {
            this.tabAlignment = tabAlignment;
        }

        /// <summary>Returns the anchor of a tab stop.</summary>
        /// <returns>
        /// a
        /// <see cref="char?"/>
        /// value
        /// </returns>
        public virtual char? GetTabAnchor() {
            return tabAnchor;
        }

        /// <summary>Sets the anchor of a tab stop.</summary>
        /// <param name="tabAnchor">
        /// a
        /// <see cref="char?"/>
        /// value
        /// </param>
        public virtual void SetTabAnchor(char? tabAnchor) {
            this.tabAnchor = tabAnchor;
        }

        /// <summary>
        /// Returns the tab leader of a tab stop, which defines a pattern that
        /// should be repeated until the TabStop is reached.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Canvas.Draw.ILineDrawer"/>
        /// value, a pattern drawing object
        /// </returns>
        public virtual ILineDrawer GetTabLeader() {
            return tabLeader;
        }

        /// <summary>
        /// Sets the tab leader of a tab stop, which defines a pattern that
        /// should be repeated until the TabStop is reached.
        /// </summary>
        /// <param name="tabLeader">
        /// a
        /// <see cref="iText.Kernel.Pdf.Canvas.Draw.ILineDrawer"/>
        /// value
        /// </param>
        public virtual void SetTabLeader(ILineDrawer tabLeader) {
            this.tabLeader = tabLeader;
        }
    }
}
