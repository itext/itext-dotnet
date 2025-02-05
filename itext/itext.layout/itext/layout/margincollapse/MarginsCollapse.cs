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
namespace iText.Layout.Margincollapse {
//\cond DO_NOT_DOCUMENT
    internal class MarginsCollapse {
        private float maxPositiveMargin = 0;

        private float minNegativeMargin = 0;

//\cond DO_NOT_DOCUMENT
        internal virtual void JoinMargin(float margin) {
            if (maxPositiveMargin < margin) {
                maxPositiveMargin = margin;
            }
            else {
                if (minNegativeMargin > margin) {
                    minNegativeMargin = margin;
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void JoinMargin(MarginsCollapse marginsCollapse) {
            JoinMargin(marginsCollapse.maxPositiveMargin);
            JoinMargin(marginsCollapse.minNegativeMargin);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual float GetCollapsedMarginsSize() {
            return maxPositiveMargin + minNegativeMargin;
        }
//\endcond

        /// <summary>
        /// Creates a "deep copy" of this MarginsCollapse, meaning the object returned by this method will be independent
        /// of the object being cloned.
        /// </summary>
        /// <returns>the copied MarginsCollapse.</returns>
        public virtual MarginsCollapse Clone() {
            return (iText.Layout.Margincollapse.MarginsCollapse) MemberwiseClone();
        }
    }
//\endcond
}
