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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Pdf.Action;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// Defines the most common properties that most
    /// <see cref="IElement"/>
    /// implementations
    /// share.
    /// </summary>
    /// <typeparam name="T">the type of the implementation</typeparam>
    public abstract class AbstractElement<T> : ElementPropertyContainer<T>, IAbstractElement
        where T : IElement {
        protected internal IRenderer nextRenderer;

        protected internal IList<IElement> childElements = new List<IElement>();

        protected internal ICollection<Style> styles;

        public virtual IRenderer GetRenderer() {
            if (nextRenderer != null) {
                IRenderer renderer = nextRenderer;
                nextRenderer = nextRenderer.GetNextRenderer();
                return renderer;
            }
            return MakeNewRenderer();
        }

        public virtual void SetNextRenderer(IRenderer renderer) {
            this.nextRenderer = renderer;
        }

        public virtual IRenderer CreateRendererSubTree() {
            IRenderer rendererRoot = GetRenderer();
            foreach (IElement child in childElements) {
                rendererRoot.AddChild(child.CreateRendererSubTree());
            }
            return rendererRoot;
        }

        public override bool HasProperty(int property) {
            bool hasProperty = base.HasProperty(property);
            if (styles != null && styles.Count > 0 && !hasProperty) {
                foreach (Style style in styles) {
                    if (style.HasProperty(property)) {
                        hasProperty = true;
                        break;
                    }
                }
            }
            return hasProperty;
        }

        public override T1 GetProperty<T1>(int property) {
            Object result = base.GetProperty<T1>(property);
            if (styles != null && styles.Count > 0 && result == null && !base.HasProperty(property)) {
                foreach (Style style in styles) {
                    T1 foundInStyle = style.GetProperty<T1>(property);
                    if (foundInStyle != null || style.HasProperty(property)) {
                        result = foundInStyle;
                    }
                }
            }
            return (T1)result;
        }

        /// <summary>Add a new style to this element.</summary>
        /// <remarks>
        /// Add a new style to this element. A style can be used as an effective way
        /// to define multiple equal properties to several elements, however its properties have
        /// lower priority than properties, directly set on
        /// <see cref="iText.Layout.ElementPropertyContainer{T}"/>
        /// Note that if several Style objects are added, iText checks them one by one
        /// in the order in which they were added and returns the property's value from
        /// the last Style object, which contains this property. So, if there are two Style
        /// objects added: the first has set width of 100 points and the second of 200 points,
        /// iText will get 200 points as width value.
        /// </remarks>
        /// <param name="style">the style to be added</param>
        /// <returns>this element</returns>
        public virtual T AddStyle(Style style) {
            if (style == null) {
                throw new ArgumentException("Style can not be null.");
            }
            if (styles == null) {
                styles = new LinkedHashSet<Style>();
            }
            styles.Add(style);
            return (T)(Object)this;
        }

        /// <summary>Gets the child elements of this elements</summary>
        /// <returns>a list of children</returns>
        public virtual IList<IElement> GetChildren() {
            return childElements;
        }

        /// <summary>Returns <c>true</c> if this list contains no elements.</summary>
        /// <returns><c>true</c> if this list contains no elements</returns>
        public virtual bool IsEmpty() {
            return 0 == childElements.Count;
        }

        /// <summary>Sets an action on this Element.</summary>
        /// <remarks>
        /// Sets an action on this Element. An action is a general PDF concept that
        /// signifies anything that makes the document interactive, e.g. a hyperlink
        /// or a button.
        /// </remarks>
        /// <param name="action">
        /// the
        /// <see cref="iText.Kernel.Pdf.Action.PdfAction"/>
        /// that should be performed
        /// </param>
        /// <returns>this Element</returns>
        public virtual T SetAction(PdfAction action) {
            SetProperty(Property.ACTION, action);
            return (T)(Object)this;
        }

        /// <summary>Explicitly sets the page number this element should be put on.</summary>
        /// <remarks>
        /// Explicitly sets the page number this element should be put on. The location
        /// on the page will be the same as if it were added at the end of the document,
        /// but it will be located on the specified page.
        /// <para />
        /// This method should be used very carefully in client code.
        /// </remarks>
        /// <param name="pageNumber">the page number of the page this element should be placed on</param>
        /// <returns>this Element</returns>
        public virtual T SetPageNumber(int pageNumber) {
            SetProperty(Property.PAGE_NUMBER, pageNumber);
            return (T)(Object)this;
        }

        /// <summary>Creates new renderer instance.</summary>
        /// <returns>
        /// new
        /// <see cref="iText.Layout.Renderer.IRenderer"/>
        /// </returns>
        protected internal abstract IRenderer MakeNewRenderer();
    }
}
