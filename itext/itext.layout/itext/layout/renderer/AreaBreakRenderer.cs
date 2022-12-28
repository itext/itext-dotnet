/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
