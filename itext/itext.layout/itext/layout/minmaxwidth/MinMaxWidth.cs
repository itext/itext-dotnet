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

namespace iText.Layout.Minmaxwidth {
    public class MinMaxWidth {
        private float childrenMinWidth;

        private float childrenMaxWidth;

        private float additionalWidth;

        public MinMaxWidth()
            : this(0) {
        }

        public MinMaxWidth(float additionalWidth)
            : this(0, 0, additionalWidth) {
        }

        public MinMaxWidth(float childrenMinWidth, float childrenMaxWidth, float additionalWidth) {
            this.childrenMinWidth = childrenMinWidth;
            this.childrenMaxWidth = childrenMaxWidth;
            this.additionalWidth = additionalWidth;
        }

        public virtual float GetChildrenMinWidth() {
            return childrenMinWidth;
        }

        public virtual void SetChildrenMinWidth(float childrenMinWidth) {
            this.childrenMinWidth = childrenMinWidth;
        }

        public virtual float GetChildrenMaxWidth() {
            return childrenMaxWidth;
        }

        public virtual void SetChildrenMaxWidth(float childrenMaxWidth) {
            this.childrenMaxWidth = childrenMaxWidth;
        }

        public virtual float GetAdditionalWidth() {
            return additionalWidth;
        }

        public virtual void SetAdditionalWidth(float additionalWidth) {
            this.additionalWidth = additionalWidth;
        }

        public virtual float GetMaxWidth() {
            return Math.Min(childrenMaxWidth + additionalWidth, MinMaxWidthUtils.GetInfWidth());
        }

        public virtual float GetMinWidth() {
            return Math.Min(childrenMinWidth + additionalWidth, GetMaxWidth());
        }

        public override String ToString() {
            return "min=" + (childrenMinWidth + additionalWidth) + ", max=" + (childrenMaxWidth + additionalWidth);
        }
    }
}
