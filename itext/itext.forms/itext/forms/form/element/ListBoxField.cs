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
using iText.Forms.Form;
using iText.Forms.Form.Renderer;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Element {
    /// <summary>A field that represents a control for selecting one or several of the provided options.</summary>
    public class ListBoxField : AbstractSelectField {
        /// <summary>Create a new list box field.</summary>
        /// <param name="size">
        /// the size of the list box, which will define the height of visible properties,
        /// shall be greater than zero
        /// </param>
        /// <param name="allowMultipleSelection">
        /// a boolean flag that defines whether multiple options are allowed
        /// to be selected at once
        /// </param>
        /// <param name="id">the id</param>
        public ListBoxField(String id, int size, bool allowMultipleSelection)
            : base(id) {
            SetProperty(FormProperty.FORM_FIELD_SIZE, size);
            SetProperty(FormProperty.FORM_FIELD_MULTIPLE, allowMultipleSelection);
            SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(1));
            SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(1));
            SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(1));
            SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(1));
            // This property allows to show selected item if height is smaller than the size of all items
            SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.HIDDEN);
        }

        /* (non-Javadoc)
        * @see FormField#getDefaultProperty(int)
        */
        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case FormProperty.FORM_FIELD_MULTIPLE: {
                    return (T1)(Object)false;
                }

                case FormProperty.FORM_FIELD_SIZE: {
                    return (T1)(Object)4;
                }

                default: {
                    return base.GetDefaultProperty<T1>(property);
                }
            }
        }

        /// <summary>
        /// Add an option for
        /// <see cref="ListBoxField"/>.
        /// </summary>
        /// <remarks>
        /// Add an option for
        /// <see cref="ListBoxField"/>
        /// . The option is not selected.
        /// </remarks>
        /// <param name="option">string representation of the option.</param>
        /// <returns>
        /// this
        /// <see cref="ListBoxField"/>.
        /// </returns>
        public virtual iText.Forms.Form.Element.ListBoxField AddOption(String option) {
            return AddOption(option, false);
        }

        /// <summary>
        /// Add an option for
        /// <see cref="ListBoxField"/>.
        /// </summary>
        /// <param name="option">string representation of the option.</param>
        /// <param name="selected">
        /// 
        /// <see langword="true"/>
        /// is the option if selected,
        /// <see langword="false"/>
        /// otherwise.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="ListBoxField"/>.
        /// </returns>
        public virtual iText.Forms.Form.Element.ListBoxField AddOption(String option, bool selected) {
            SelectFieldItem item = new SelectFieldItem(option);
            AddOption(item);
            item.GetElement().SetProperty(FormProperty.FORM_FIELD_SELECTED, selected);
            return this;
        }

        /// <summary>Get a list of selected options.</summary>
        /// <returns>a list of display values of selected options.</returns>
        public virtual IList<String> GetSelectedStrings() {
            IList<String> selectedStrings = new List<String>();
            foreach (SelectFieldItem option in options) {
                if (true.Equals(option.GetElement().GetProperty<bool?>(FormProperty.FORM_FIELD_SELECTED))) {
                    selectedStrings.Add(option.GetDisplayValue());
                }
            }
            return selectedStrings;
        }

        protected override IRenderer MakeNewRenderer() {
            return new SelectFieldListBoxRenderer(this);
        }
    }
}
