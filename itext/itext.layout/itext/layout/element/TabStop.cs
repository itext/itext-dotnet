/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

        public virtual float GetTabPosition() {
            return tabPosition;
        }

        public virtual TabAlignment GetTabAlignment() {
            return tabAlignment;
        }

        public virtual void SetTabAlignment(TabAlignment tabAlignment) {
            this.tabAlignment = tabAlignment;
        }

        public virtual char? GetTabAnchor() {
            return tabAnchor;
        }

        public virtual void SetTabAnchor(char? tabAnchor) {
            this.tabAnchor = tabAnchor;
        }

        public virtual ILineDrawer GetTabLeader() {
            return tabLeader;
        }

        public virtual void SetTabLeader(ILineDrawer tabLeader) {
            this.tabLeader = tabLeader;
        }
    }
}
