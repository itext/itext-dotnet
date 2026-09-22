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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;

namespace iText.Layout.Element {
    /// <summary>An abstract class that represents a paragraph of text in a document.</summary>
    /// <remarks>
    /// An abstract class that represents a paragraph of text in a document. It provides methods for adding text
    /// and other elements to the paragraph, as well as managing properties such as leading, indentation, and margins.
    /// </remarks>
    /// <typeparam name="T">the type of the concrete subclass extending this abstract class</typeparam>
    public abstract class AbstractParagraph<T> : BlockElement<T>
        where T : iText.Layout.Element.AbstractParagraph<T> {
        protected internal DefaultAccessibilityProperties tagProperties;

        /// <summary>
        /// Creates a new instance of
        /// <see cref="AbstractParagraph{T}"/>.
        /// </summary>
        /// <remarks>
        /// Creates a new instance of
        /// <see cref="AbstractParagraph{T}"/>
        /// . This constructor is protected to allow subclassing.
        /// </remarks>
        protected internal AbstractParagraph()
            : base() {
        }

        /// <summary>
        /// Adds a piece of text to this
        /// <see cref="AbstractParagraph{T}"/>.
        /// </summary>
        /// <param name="text">
        /// the content to be added, as a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual T Add(String text) {
            return Add(new Text(text));
        }

        /// <summary>
        /// Adds a
        /// <see cref="ILeafElement"/>
        /// element to this
        /// <see cref="AbstractParagraph{T}"/>.
        /// </summary>
        /// <param name="element">
        /// the content to be added, any
        /// <see cref="ILeafElement"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual T Add(ILeafElement element) {
            childElements.Add(element);
            return (T)this;
        }

        /// <summary>
        /// Adds a
        /// <see cref="AbstractParagraph{T}"/>
        /// element.
        /// </summary>
        /// <param name="element">
        /// the content to be added, any
        /// <see cref="AbstractParagraph{T}"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual T Add(iText.Layout.Element.AbstractParagraph element) {
            childElements.Add(element);
            return (T)this;
        }

        /// <summary>
        /// Adds a
        /// <see cref="System.Collections.IList{E}"/>
        /// of layout elements to this
        /// <see cref="AbstractParagraph{T}"/>.
        /// </summary>
        /// <param name="elements">the content to be added</param>
        /// <typeparam name="T2">
        /// any
        /// <see cref="ILeafElement"/>
        /// </typeparam>
        /// <returns>this Element</returns>
        public virtual T AddAll<T2>(IList<T2> elements)
            where T2 : ILeafElement {
            foreach (ILeafElement element in elements) {
                Add(element);
            }
            return (T)this;
        }

        /// <summary>
        /// Sets the indent value for the first line of the
        /// <see cref="AbstractParagraph{T}"/>.
        /// </summary>
        /// <param name="indent">
        /// the indent value that must be applied to the first line of
        /// the
        /// <see cref="AbstractParagraph{T}"/>
        /// , as a <c>float</c>
        /// </param>
        /// <returns>this Element</returns>
        public virtual T SetFirstLineIndent(float indent) {
            SetProperty(Property.FIRST_LINE_INDENT, indent);
            return (T)this;
        }

        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case Property.LEADING: {
                    return (T1)(Object)new Leading(Leading.MULTIPLIED, childElements.Count == 1 && childElements[0] is Image ? 
                        1 : 1.35f);
                }

                case Property.FIRST_LINE_INDENT: {
                    return (T1)(Object)0f;
                }

                case Property.MARGIN_TOP:
                case Property.MARGIN_BOTTOM: {
                    return (T1)(Object)UnitValue.CreatePointValue(4f);
                }

                case Property.TAB_DEFAULT: {
                    return (T1)(Object)50f;
                }

                default: {
                    return base.GetDefaultProperty<T1>(property);
                }
            }
        }

        /// <summary>
        /// Sets orphans restriction on a
        /// <see cref="AbstractParagraph{T}"/>.
        /// </summary>
        /// <param name="orphansControl">
        /// an instance of
        /// <see cref="iText.Layout.Properties.ParagraphOrphansControl"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual T SetOrphansControl(ParagraphOrphansControl orphansControl) {
            SetProperty(Property.ORPHANS_CONTROL, orphansControl);
            return (T)this;
        }

        /// <summary>
        /// Sets widows restriction on a
        /// <see cref="AbstractParagraph{T}"/>.
        /// </summary>
        /// <param name="widowsControl">
        /// an instance of
        /// <see cref="iText.Layout.Properties.ParagraphWidowsControl"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual T SetWidowsControl(ParagraphWidowsControl widowsControl) {
            SetProperty(Property.WIDOWS_CONTROL, widowsControl);
            return (T)this;
        }

        /// <summary><inheritDoc/></summary>
        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.P);
            }
            return tagProperties;
        }

        /// <summary>Returns a map of unsupported properties.</summary>
        /// <remarks>
        /// Returns a map of unsupported properties. The map is empty by default,
        /// but can be overridden by subclasses to specify unsupported properties.
        /// </remarks>
        /// <returns>a map of unsupported properties, where the key is the property ID and the value is the property name
        ///     </returns>
        public virtual IDictionary<int, String> GetUnsupportedProperties() {
            return JavaCollectionsUtil.EmptyMap<int, String>();
        }

        /// <summary>
        /// Adds a list of
        /// <see cref="TabStop"/>
        /// objects to the paragraph's properties.
        /// </summary>
        /// <param name="newTabStops">
        /// the list of
        /// <see cref="TabStop"/>
        /// objects to be added
        /// </param>
        protected internal virtual void AddTabStopsAsProperty(IList<TabStop> newTabStops) {
            IDictionary<float, TabStop> tabStops = this.GetProperty<IDictionary<float, TabStop>>(Property.TAB_STOPS);
            if (tabStops == null) {
                tabStops = new SortedDictionary<float, TabStop>();
                SetProperty(Property.TAB_STOPS, tabStops);
            }
            foreach (TabStop tabStop in newTabStops) {
                tabStops.Put(tabStop.GetTabPosition(), tabStop);
            }
        }
    }
}
