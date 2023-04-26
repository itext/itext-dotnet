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
using System.Collections.Generic;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Renderer for the
    /// <see cref="iText.Layout.Element.AreaBreak"/>
    /// layout element.
    /// </summary>
    /// <remarks>
    /// Renderer for the
    /// <see cref="iText.Layout.Element.AreaBreak"/>
    /// layout element. Will terminate the
    /// current content area and initialize a new one.
    /// </remarks>
    public class AreaBreakRenderer : IRenderer {
        protected internal AreaBreak areaBreak;

        /// <summary>Creates an AreaBreakRenderer.</summary>
        /// <param name="areaBreak">
        /// the
        /// <see cref="iText.Layout.Element.AreaBreak"/>
        /// that will be rendered by this object
        /// </param>
        public AreaBreakRenderer(AreaBreak areaBreak) {
            this.areaBreak = areaBreak;
        }

        /// <summary>
        /// Throws an UnsupportedOperationException because instances of this
        /// class are only used for terminating the current content area.
        /// </summary>
        /// <param name="renderer">
        /// 
        /// <inheritDoc/>
        /// </param>
        public virtual void AddChild(IRenderer renderer) {
            throw new NotSupportedException();
        }

        public virtual LayoutResult Layout(LayoutContext layoutContext) {
            return new LayoutResult(LayoutResult.NOTHING, null, null, null, this).SetAreaBreak(areaBreak);
        }

        /// <summary>
        /// Throws an UnsupportedOperationException because instances of this
        /// class are only used for terminating the current content area.
        /// </summary>
        /// <param name="drawContext">
        /// 
        /// <inheritDoc/>
        /// </param>
        public virtual void Draw(DrawContext drawContext) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Throws an UnsupportedOperationException because instances of this
        /// class are only used for terminating the current content area.
        /// </summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual LayoutArea GetOccupiedArea() {
            throw new NotSupportedException();
        }

        public virtual bool HasProperty(int property) {
            return false;
        }

        public virtual bool HasOwnProperty(int property) {
            return false;
        }

        public virtual T1 GetProperty<T1>(int key) {
            return (T1)(Object)null;
        }

        public virtual T1 GetOwnProperty<T1>(int property) {
            return (T1)(Object)null;
        }

        public virtual T1 GetDefaultProperty<T1>(int property) {
            return (T1)(Object)null;
        }

        /// <summary>
        /// Throws an UnsupportedOperationException because instances of this
        /// class are only used for terminating the current content area.
        /// </summary>
        /// <param name="property">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="defaultValue">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <typeparam name="T1">
        /// 
        /// <inheritDoc/>
        /// </typeparam>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual T1 GetProperty<T1>(int property, T1 defaultValue) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Throws an UnsupportedOperationException because instances of this
        /// class are only used for terminating the current content area.
        /// </summary>
        /// <param name="property">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="value">
        /// 
        /// <inheritDoc/>
        /// </param>
        public virtual void SetProperty(int property, Object value) {
            throw new NotSupportedException();
        }

        public virtual void DeleteOwnProperty(int property) {
        }

        public virtual IRenderer SetParent(IRenderer parent) {
            return this;
        }

        public virtual IPropertyContainer GetModelElement() {
            return null;
        }

        public virtual IRenderer GetParent() {
            return null;
        }

        public virtual IList<IRenderer> GetChildRenderers() {
            return null;
        }

        public virtual bool IsFlushed() {
            return false;
        }

        /// <summary>
        /// Throws an UnsupportedOperationException because instances of this
        /// class are only used for terminating the current content area.
        /// </summary>
        /// <param name="dx">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="dy">
        /// 
        /// <inheritDoc/>
        /// </param>
        public virtual void Move(float dx, float dy) {
            throw new NotSupportedException();
        }

        public virtual IRenderer GetNextRenderer() {
            return null;
        }
    }
}
