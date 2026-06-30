/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Layout;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>Class for storing logic shared by break renderers.</summary>
    public abstract class AbstractBreakRenderer : IRenderer {
        protected internal IRenderer parent;

        protected internal IDictionary<int, Object> properties = new Dictionary<int, Object>();

        /// <summary>
        /// Creates new
        /// <see cref="AbstractBreakRenderer"/>
        /// instance.
        /// </summary>
        protected internal AbstractBreakRenderer() {
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool HasProperty(int property) {
            return HasOwnProperty(property) || (parent != null && Property.IsPropertyInherited(property) && parent.HasProperty
                (property));
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool HasOwnProperty(int property) {
            return properties.ContainsKey(property);
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetProperty<T1>(int key) {
            Object property;
            if ((property = properties.Get(key)) != null || properties.ContainsKey(key)) {
                return (T1)property;
            }
            if (parent != null && Property.IsPropertyInherited(key) && (property = parent.GetProperty<T1>(key)) != null
                ) {
                return (T1)property;
            }
            property = this.GetDefaultProperty<T1>(key);
            if (property != null) {
                return (T1)property;
            }
            return (T1)(Object)null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetProperty<T1>(int property, T1 defaultValue) {
            T1 result = this.GetProperty<T1>(property);
            return result != null ? result : defaultValue;
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetOwnProperty<T1>(int property) {
            return (T1)properties.Get(property);
        }

        /// <summary><inheritDoc/></summary>
        public virtual T1 GetDefaultProperty<T1>(int property) {
            return (T1)(Object)null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetProperty(int property, Object value) {
            properties.Put(property, value);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void DeleteOwnProperty(int property) {
            properties.JRemove(property);
        }

        public virtual IRenderer SetParent(IRenderer parent) {
            this.parent = parent;
            return this;
        }

        public virtual IRenderer GetParent() {
            return this.parent;
        }

        public virtual IList<IRenderer> GetChildRenderers() {
            return JavaCollectionsUtil.EmptyList<IRenderer>();
        }

        public virtual bool IsFlushed() {
            return false;
        }

        public abstract void AddChild(IRenderer arg1);

        public abstract void Draw(DrawContext arg1);

        public abstract IPropertyContainer GetModelElement();

        public abstract IRenderer GetNextRenderer();

        public abstract LayoutArea GetOccupiedArea();

        public abstract LayoutResult Layout(LayoutContext arg1);

        public abstract void Move(float arg1, float arg2);
    }
}
