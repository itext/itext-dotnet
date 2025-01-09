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
using iText.Kernel.Pdf.Tagutils;

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

        private TagTreePointer tagPointer;

//\cond DO_NOT_DOCUMENT
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
//\endcond

        /// <summary>Get accessible element.</summary>
        /// <returns>the accessible element.</returns>
        public IAccessibleElement GetAccessibleElement() {
            return elem;
        }

        /// <summary>Gets the TagTreePointer.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.Tagutils.TagTreePointer"/>
        /// or null if there is no associated one yet.
        /// </returns>
        public TagTreePointer GetTagPointer() {
            return tagPointer;
        }

        /// <summary>Sets the TagTreePointer.</summary>
        /// <param name="tag">the TagTreePointer to set.</param>
        public void SetTagPointer(TagTreePointer tag) {
            this.tagPointer = tag;
        }

//\cond DO_NOT_DOCUMENT
        internal AccessibilityProperties GetAccessibilityProperties() {
            if (elem == null) {
                return null;
            }
            return elem.GetAccessibilityProperties();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set finished flag for hint key instance.</summary>
        internal void SetFinished() {
            this.isFinished = true;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Specify that hint key instance corresponds to artifact.</summary>
        internal void SetArtifact() {
            this.isArtifact = true;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Get overridden role.</summary>
        /// <returns>the overridden role.</returns>
        internal String GetOverriddenRole() {
            return overriddenRole;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Set the overridden role.</summary>
        /// <param name="overriddenRole">overridden role.</param>
        internal void SetOverriddenRole(String overriddenRole) {
            this.overriddenRole = overriddenRole;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond
    }
}
