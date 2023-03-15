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
using System;

namespace iText.Layout.Tagging {
    public sealed class TaggingHintKey {
        private IAccessibleElement elem;

        private bool isArtifact;

        private bool isFinished;

        private String overriddenRole;

        private bool elementBasedFinishingOnly;

        internal TaggingHintKey(IAccessibleElement elem, bool createdElementBased) {
            this.elem = elem;
            this.elementBasedFinishingOnly = createdElementBased;
        }

        public IAccessibleElement GetAccessibleElement() {
            return elem;
        }

        internal bool IsFinished() {
            return isFinished;
        }

        internal void SetFinished() {
            this.isFinished = true;
        }

        internal bool IsArtifact() {
            return isArtifact;
        }

        internal void SetArtifact() {
            this.isArtifact = true;
        }

        internal String GetOverriddenRole() {
            return overriddenRole;
        }

        internal void SetOverriddenRole(String overriddenRole) {
            this.overriddenRole = overriddenRole;
        }

        internal bool IsElementBasedFinishingOnly() {
            return elementBasedFinishingOnly;
        }
    }
}
