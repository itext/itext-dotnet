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
