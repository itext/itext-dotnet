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
namespace iText.Layout.Margincollapse {
    public class MarginsCollapseInfo {
        private bool ignoreOwnMarginTop;

        private bool ignoreOwnMarginBottom;

        private MarginsCollapse collapseBefore;

        private MarginsCollapse collapseAfter;

        // MarginCollapse instance which contains margin-after of the element without next sibling or parent margins (only element's margin and element's kids)
        private MarginsCollapse ownCollapseAfter;

        private bool isSelfCollapsing;

        // when a parent has a fixed height these fields tells kid how much free space parent has for the margin collapsed with kid
        private float bufferSpaceOnTop;

        private float bufferSpaceOnBottom;

        private float usedBufferSpaceOnTop;

        private float usedBufferSpaceOnBottom;

        private bool clearanceApplied;

        internal MarginsCollapseInfo() {
            this.ignoreOwnMarginTop = false;
            this.ignoreOwnMarginBottom = false;
            this.collapseBefore = new MarginsCollapse();
            this.collapseAfter = new MarginsCollapse();
            this.isSelfCollapsing = true;
            this.bufferSpaceOnTop = 0;
            this.bufferSpaceOnBottom = 0;
            this.usedBufferSpaceOnTop = 0;
            this.usedBufferSpaceOnBottom = 0;
            this.clearanceApplied = false;
        }

        internal MarginsCollapseInfo(bool ignoreOwnMarginTop, bool ignoreOwnMarginBottom, MarginsCollapse collapseBefore
            , MarginsCollapse collapseAfter) {
            this.ignoreOwnMarginTop = ignoreOwnMarginTop;
            this.ignoreOwnMarginBottom = ignoreOwnMarginBottom;
            this.collapseBefore = collapseBefore;
            this.collapseAfter = collapseAfter;
            this.isSelfCollapsing = true;
            this.bufferSpaceOnTop = 0;
            this.bufferSpaceOnBottom = 0;
            this.usedBufferSpaceOnTop = 0;
            this.usedBufferSpaceOnBottom = 0;
            this.clearanceApplied = false;
        }

        public virtual void CopyTo(iText.Layout.Margincollapse.MarginsCollapseInfo destInfo) {
            destInfo.ignoreOwnMarginTop = this.ignoreOwnMarginTop;
            destInfo.ignoreOwnMarginBottom = this.ignoreOwnMarginBottom;
            destInfo.collapseBefore = this.collapseBefore;
            destInfo.collapseAfter = this.collapseAfter;
            destInfo.SetOwnCollapseAfter(ownCollapseAfter);
            destInfo.SetSelfCollapsing(isSelfCollapsing);
            destInfo.SetBufferSpaceOnTop(bufferSpaceOnTop);
            destInfo.SetBufferSpaceOnBottom(bufferSpaceOnBottom);
            destInfo.SetUsedBufferSpaceOnTop(usedBufferSpaceOnTop);
            destInfo.SetUsedBufferSpaceOnBottom(usedBufferSpaceOnBottom);
            destInfo.SetClearanceApplied(clearanceApplied);
        }

        public static iText.Layout.Margincollapse.MarginsCollapseInfo CreateDeepCopy(iText.Layout.Margincollapse.MarginsCollapseInfo
             instance) {
            iText.Layout.Margincollapse.MarginsCollapseInfo copy = new iText.Layout.Margincollapse.MarginsCollapseInfo
                ();
            instance.CopyTo(copy);
            copy.collapseBefore = instance.collapseBefore.Clone();
            copy.collapseAfter = instance.collapseAfter.Clone();
            if (instance.ownCollapseAfter != null) {
                copy.SetOwnCollapseAfter(instance.ownCollapseAfter.Clone());
            }
            return copy;
        }

        public static void UpdateFromCopy(iText.Layout.Margincollapse.MarginsCollapseInfo originalInstance, iText.Layout.Margincollapse.MarginsCollapseInfo
             processedCopy) {
            originalInstance.ignoreOwnMarginTop = processedCopy.ignoreOwnMarginTop;
            originalInstance.ignoreOwnMarginBottom = processedCopy.ignoreOwnMarginBottom;
            originalInstance.collapseBefore.JoinMargin(processedCopy.collapseBefore);
            originalInstance.collapseAfter.JoinMargin(processedCopy.collapseAfter);
            if (processedCopy.GetOwnCollapseAfter() != null) {
                if (originalInstance.GetOwnCollapseAfter() == null) {
                    originalInstance.SetOwnCollapseAfter(new MarginsCollapse());
                }
                originalInstance.GetOwnCollapseAfter().JoinMargin(processedCopy.GetOwnCollapseAfter());
            }
            originalInstance.SetSelfCollapsing(processedCopy.isSelfCollapsing);
            originalInstance.SetBufferSpaceOnTop(processedCopy.bufferSpaceOnTop);
            originalInstance.SetBufferSpaceOnBottom(processedCopy.bufferSpaceOnBottom);
            originalInstance.SetUsedBufferSpaceOnTop(processedCopy.usedBufferSpaceOnTop);
            originalInstance.SetUsedBufferSpaceOnBottom(processedCopy.usedBufferSpaceOnBottom);
            originalInstance.SetClearanceApplied(processedCopy.clearanceApplied);
        }

        internal virtual MarginsCollapse GetCollapseBefore() {
            return this.collapseBefore;
        }

        internal virtual MarginsCollapse GetCollapseAfter() {
            return collapseAfter;
        }

        internal virtual void SetCollapseAfter(MarginsCollapse collapseAfter) {
            this.collapseAfter = collapseAfter;
        }

        internal virtual MarginsCollapse GetOwnCollapseAfter() {
            return ownCollapseAfter;
        }

        internal virtual void SetOwnCollapseAfter(MarginsCollapse marginsCollapse) {
            this.ownCollapseAfter = marginsCollapse;
        }

        internal virtual void SetSelfCollapsing(bool selfCollapsing) {
            isSelfCollapsing = selfCollapsing;
        }

        internal virtual bool IsSelfCollapsing() {
            return isSelfCollapsing;
        }

        internal virtual bool IsIgnoreOwnMarginTop() {
            return ignoreOwnMarginTop;
        }

        internal virtual bool IsIgnoreOwnMarginBottom() {
            return ignoreOwnMarginBottom;
        }

        internal virtual float GetBufferSpaceOnTop() {
            return bufferSpaceOnTop;
        }

        internal virtual void SetBufferSpaceOnTop(float bufferSpaceOnTop) {
            this.bufferSpaceOnTop = bufferSpaceOnTop;
        }

        internal virtual float GetBufferSpaceOnBottom() {
            return bufferSpaceOnBottom;
        }

        internal virtual void SetBufferSpaceOnBottom(float bufferSpaceOnBottom) {
            this.bufferSpaceOnBottom = bufferSpaceOnBottom;
        }

        internal virtual float GetUsedBufferSpaceOnTop() {
            return usedBufferSpaceOnTop;
        }

        internal virtual void SetUsedBufferSpaceOnTop(float usedBufferSpaceOnTop) {
            this.usedBufferSpaceOnTop = usedBufferSpaceOnTop;
        }

        internal virtual float GetUsedBufferSpaceOnBottom() {
            return usedBufferSpaceOnBottom;
        }

        internal virtual void SetUsedBufferSpaceOnBottom(float usedBufferSpaceOnBottom) {
            this.usedBufferSpaceOnBottom = usedBufferSpaceOnBottom;
        }

        internal virtual bool IsClearanceApplied() {
            return clearanceApplied;
        }

        internal virtual void SetClearanceApplied(bool clearanceApplied) {
            this.clearanceApplied = clearanceApplied;
        }
    }
}
