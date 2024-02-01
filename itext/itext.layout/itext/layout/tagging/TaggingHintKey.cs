/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
    /// <summary>
    /// TaggingHintKey instances are created in the scope of
    /// <see cref="iText.Layout.Renderer.RootRenderer.AddChild(iText.Layout.Renderer.IRenderer)"/>
    /// to preserve logical order of layout elements from model elements.
    /// </summary>
    public sealed class TaggingHintKey {
        private IAccessibleElement elem;

        private bool isArtifact;

        private bool isFinished;

        private String overriddenRole;

        private bool elementBasedFinishingOnly;

        /// <summary>
        /// Instantiate a new
        /// <see cref="TaggingHintKey"/>
        /// instance.
        /// </summary>
        /// <param name="elem">element this hint key will be created for.</param>
        /// <param name="createdElementBased">
        /// 
        /// <see langword="true"/>
        /// if element implements
        /// <see cref="iText.Layout.Element.IElement"/>.
        /// </param>
        internal TaggingHintKey(IAccessibleElement elem, bool createdElementBased) {
            this.elem = elem;
            this.elementBasedFinishingOnly = createdElementBased;
        }

        /// <summary>Get accessible element.</summary>
        /// <returns>the accessible element.</returns>
        public IAccessibleElement GetAccessibleElement() {
            return elem;
        }

        /// <summary>Retrieve hint key finished flag.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if hint key is finished,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        internal bool IsFinished() {
            return isFinished;
        }

        /// <summary>Set finished flag for hint key instance.</summary>
        internal void SetFinished() {
            this.isFinished = true;
        }

        /// <summary>Retrieve information whether this hint key is artifact or not.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if hint key corresponds to artifact,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        internal bool IsArtifact() {
            return isArtifact;
        }

        /// <summary>Specify that hint key instance corresponds to artifact.</summary>
        internal void SetArtifact() {
            this.isArtifact = true;
        }

        /// <summary>Get overridden role.</summary>
        /// <returns>the overridden role.</returns>
        internal String GetOverriddenRole() {
            return overriddenRole;
        }

        /// <summary>Set the overridden role.</summary>
        /// <param name="overriddenRole">overridden role.</param>
        internal void SetOverriddenRole(String overriddenRole) {
            this.overriddenRole = overriddenRole;
        }

        /// <summary>
        /// Retrieve information whether the element backed by this hint key implements
        /// <see cref="iText.Layout.Element.IElement"/>.
        /// </summary>
        /// <returns>
        /// 
        /// <c/>
        /// true if element implements
        /// <see cref="iText.Layout.Element.IElement"/>.
        /// </returns>
        internal bool IsElementBasedFinishingOnly() {
            return elementBasedFinishingOnly;
        }
    }
}
