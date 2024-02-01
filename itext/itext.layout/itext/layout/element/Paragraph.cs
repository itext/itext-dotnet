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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Element {
    /// <summary>
    /// A layout element that represents a self-contained block of textual and
    /// graphical information.
    /// </summary>
    /// <remarks>
    /// A layout element that represents a self-contained block of textual and
    /// graphical information.
    /// It is a
    /// <see cref="BlockElement{T}"/>
    /// which essentially acts as a container for
    /// <see cref="ILeafElement">leaf elements</see>.
    /// </remarks>
    public class Paragraph : BlockElement<iText.Layout.Element.Paragraph> {
        protected internal DefaultAccessibilityProperties tagProperties;

        /// <summary>Creates a Paragraph.</summary>
        public Paragraph() {
        }

        /// <summary>Creates a Paragraph, initialized with a piece of text.</summary>
        /// <param name="text">
        /// the initial textual content, as a
        /// <see cref="System.String"/>
        /// </param>
        public Paragraph(String text)
            : this(new Text(text)) {
        }

        /// <summary>Creates a Paragraph, initialized with a piece of text.</summary>
        /// <param name="text">
        /// the initial textual content, as a
        /// <see cref="Text"/>
        /// </param>
        public Paragraph(Text text) {
            Add(text);
        }

        /// <summary>
        /// Adds a piece of text to this
        /// <see cref="Paragraph"/>.
        /// </summary>
        /// <param name="text">
        /// the content to be added, as a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Paragraph"/>
        /// </returns>
        public virtual iText.Layout.Element.Paragraph Add(String text) {
            return Add(new Text(text));
        }

        /// <summary>
        /// Adds a
        /// <see cref="ILeafElement">element</see>
        /// to this
        /// <see cref="Paragraph"/>.
        /// </summary>
        /// <param name="element">
        /// the content to be added, any
        /// <see cref="ILeafElement"/>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Paragraph"/>
        /// </returns>
        public virtual iText.Layout.Element.Paragraph Add(ILeafElement element) {
            childElements.Add(element);
            return this;
        }

        /// <summary>
        /// Adds an
        /// <see cref="IBlockElement">element</see>
        /// to this
        /// <see cref="Paragraph"/>.
        /// </summary>
        /// <param name="element">
        /// the content to be added, any
        /// <see cref="IBlockElement"/>
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Paragraph"/>
        /// </returns>
        public virtual iText.Layout.Element.Paragraph Add(IBlockElement element) {
            childElements.Add(element);
            return this;
        }

        /// <summary>
        /// Adds a
        /// <see cref="System.Collections.IList{E}"/>
        /// of layout elements to this
        /// <see cref="Paragraph"/>.
        /// </summary>
        /// <param name="elements">the content to be added</param>
        /// <typeparam name="T2">
        /// any
        /// <see cref="ILeafElement"/>
        /// </typeparam>
        /// <returns>
        /// this
        /// <see cref="Paragraph"/>
        /// </returns>
        public virtual iText.Layout.Element.Paragraph AddAll<T2>(IList<T2> elements)
            where T2 : ILeafElement {
            foreach (ILeafElement element in elements) {
                Add(element);
            }
            return this;
        }

        /// <summary>
        /// Adds an unspecified amount of tabstop elements as properties to this
        /// <see cref="Paragraph"/>.
        /// </summary>
        /// <param name="tabStops">
        /// the
        /// <see cref="TabStop">tabstop(s)</see>
        /// to be added as properties
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Paragraph"/>
        /// </returns>
        /// <seealso cref="TabStop"/>
        public virtual iText.Layout.Element.Paragraph AddTabStops(params TabStop[] tabStops) {
            AddTabStopsAsProperty(JavaUtil.ArraysAsList(tabStops));
            return this;
        }

        /// <summary>
        /// Adds a
        /// <see cref="System.Collections.IList{E}"/>
        /// of tabstop elements as properties to this
        /// <see cref="Paragraph"/>.
        /// </summary>
        /// <param name="tabStops">
        /// the list of
        /// <see cref="TabStop"/>
        /// s to be added as properties
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Paragraph"/>
        /// </returns>
        /// <seealso cref="TabStop"/>
        public virtual iText.Layout.Element.Paragraph AddTabStops(IList<TabStop> tabStops) {
            AddTabStopsAsProperty(tabStops);
            return this;
        }

        /// <summary>
        /// Removes a tabstop position from the Paragraph, if it is present in the
        /// <see cref="iText.Layout.Properties.Property.TAB_STOPS"/>
        /// property.
        /// </summary>
        /// <param name="tabStopPosition">
        /// the
        /// <see cref="TabStop"/>
        /// position to be removed.
        /// </param>
        /// <returns>this Paragraph</returns>
        /// <seealso cref="TabStop"/>
        public virtual iText.Layout.Element.Paragraph RemoveTabStop(float tabStopPosition) {
            IDictionary<float, TabStop> tabStops = this.GetProperty<IDictionary<float, TabStop>>(Property.TAB_STOPS);
            if (tabStops != null) {
                tabStops.JRemove(tabStopPosition);
            }
            return this;
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
        /// Sets the indent value for the first line of the
        /// <see cref="Paragraph"/>.
        /// </summary>
        /// <param name="indent">
        /// the indent value that must be applied to the first line of
        /// the Paragraph, as a <c>float</c>
        /// </param>
        /// <returns>this Paragraph</returns>
        public virtual iText.Layout.Element.Paragraph SetFirstLineIndent(float indent) {
            SetProperty(Property.FIRST_LINE_INDENT, indent);
            return this;
        }

        /// <summary>
        /// Sets orphans restriction on a
        /// <see cref="Paragraph"/>.
        /// </summary>
        /// <param name="orphansControl">
        /// an instance of
        /// <see cref="iText.Layout.Properties.ParagraphOrphansControl"/>.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Paragraph"/>
        /// instance.
        /// </returns>
        public virtual iText.Layout.Element.Paragraph SetOrphansControl(ParagraphOrphansControl orphansControl) {
            SetProperty(Property.ORPHANS_CONTROL, orphansControl);
            return this;
        }

        /// <summary>
        /// Sets widows restriction on a
        /// <see cref="Paragraph"/>.
        /// </summary>
        /// <param name="widowsControl">
        /// an instance of
        /// <see cref="iText.Layout.Properties.ParagraphWidowsControl"/>.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="Paragraph"/>
        /// instance.
        /// </returns>
        public virtual iText.Layout.Element.Paragraph SetWidowsControl(ParagraphWidowsControl widowsControl) {
            SetProperty(Property.WIDOWS_CONTROL, widowsControl);
            return this;
        }

        /// <summary>
        /// Sets the leading value, using the
        /// <see cref="iText.Layout.Properties.Leading.FIXED"/>
        /// strategy.
        /// </summary>
        /// <param name="leading">the new leading value</param>
        /// <returns>this Paragraph</returns>
        /// <seealso cref="iText.Layout.Properties.Leading"/>
        public virtual iText.Layout.Element.Paragraph SetFixedLeading(float leading) {
            SetProperty(Property.LEADING, new Leading(Leading.FIXED, leading));
            return this;
        }

        /// <summary>
        /// Sets the leading value, using the
        /// <see cref="iText.Layout.Properties.Leading.MULTIPLIED"/>
        /// strategy.
        /// </summary>
        /// <param name="leading">the new leading value</param>
        /// <returns>this Paragraph</returns>
        /// <seealso cref="iText.Layout.Properties.Leading"/>
        public virtual iText.Layout.Element.Paragraph SetMultipliedLeading(float leading) {
            SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, leading));
            return this;
        }

        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.P);
            }
            return tagProperties;
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new ParagraphRenderer(this);
        }

        private void AddTabStopsAsProperty(IList<TabStop> newTabStops) {
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
