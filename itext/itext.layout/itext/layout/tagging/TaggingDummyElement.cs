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
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Tagging {
    /// <summary>
    /// Instances of the class are used for
    /// <see cref="TaggingHintKey"/>
    /// which don't have model element
    /// e.g. TR or THEAD in the table.
    /// </summary>
    /// <remarks>
    /// Instances of the class are used for
    /// <see cref="TaggingHintKey"/>
    /// which don't have model element
    /// e.g. TR or THEAD in the table. Nobody will call
    /// <see cref="LayoutTaggingHelper.FinishTaggingHint(iText.Layout.IPropertyContainer)"/>
    /// for them, it is why they should be handled separately.
    /// </remarks>
    public class TaggingDummyElement : IAccessibleElement, IPropertyContainer {
        private DefaultAccessibilityProperties properties;

        private Object id;

        /// <summary>
        /// Instantiate a new
        /// <see cref="TaggingDummyElement"/>
        /// instance.
        /// </summary>
        /// <param name="role">the role.</param>
        public TaggingDummyElement(String role) {
            this.properties = new DefaultAccessibilityProperties(role);
        }

        /// <summary><inheritDoc/></summary>
        public virtual AccessibilityProperties GetAccessibilityProperties() {
            return properties;
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetProperty<T1>(int property) {
            if (property == Property.TAGGING_HINT_KEY) {
                return (T1)id;
            }
            return (T1)(Object)null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetProperty(int property, Object value) {
            if (property == Property.TAGGING_HINT_KEY) {
                this.id = value;
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool HasProperty(int property) {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool HasOwnProperty(int property) {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetOwnProperty<T1>(int property) {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetDefaultProperty<T1>(int property) {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public virtual void DeleteOwnProperty(int property) {
            throw new NotSupportedException();
        }
    }
}
