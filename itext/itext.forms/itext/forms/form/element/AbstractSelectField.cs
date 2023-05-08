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
using iText.Forms.Form;
using iText.Layout.Element;

namespace iText.Forms.Form.Element {
    /// <summary>An abstract class for fields that represents a control for selecting one or several of the provided options.
    ///     </summary>
    public abstract class AbstractSelectField : FormField<iText.Forms.Form.Element.AbstractSelectField> {
        protected internal IList<SelectFieldItem> options = new List<SelectFieldItem>();

        protected internal AbstractSelectField(String id)
            : base(id) {
        }

        /// <summary>Add a container with options.</summary>
        /// <remarks>Add a container with options. This might be a container for options group.</remarks>
        /// <param name="optionElement">a container with options.</param>
        [System.ObsoleteAttribute(@"starting from 8.0.1.")]
        public virtual void AddOption(IBlockElement optionElement) {
            String value = TryAndExtractText(optionElement);
            AddOption(new SelectFieldItem(value, optionElement));
        }

        /// <summary>Add an option to the element.</summary>
        /// <param name="option">
        /// a
        /// <see cref="SelectFieldItem"/>.
        /// </param>
        public virtual void AddOption(SelectFieldItem option) {
            options.Add(option);
        }

        /// <summary>Add an option to the element.</summary>
        /// <param name="option">
        /// a
        /// <see cref="SelectFieldItem"/>.
        /// </param>
        /// <param name="selected">
        /// 
        /// <see langword="true"/>
        /// is the option if selected,
        /// <see langword="false"/>
        /// otherwise.
        /// </param>
        public virtual void AddOption(SelectFieldItem option, bool selected) {
            option.GetElement().SetProperty(FormProperty.FORM_FIELD_SELECTED, selected);
            options.Add(option);
        }

        /// <summary>
        /// Get a list of
        /// <see cref="SelectFieldItem"/>.
        /// </summary>
        /// <returns>a list of options.</returns>
        public virtual IList<SelectFieldItem> GetItems() {
            return options;
        }

        /// <summary>Gets the total amount of options available.</summary>
        /// <returns>the number of options in the element.</returns>
        public virtual int OptionsCount() {
            return this.GetItems().Count;
        }

        /// <summary>Checks if the element has any options.</summary>
        /// <returns>true if the element has options, false otherwise.</returns>
        public virtual bool HasOptions() {
            return OptionsCount() > 0;
        }

        /// <summary>
        /// Get an option
        /// <see cref="SelectFieldItem"/>
        /// by its string value.
        /// </summary>
        /// <param name="value">string value to find an option by.</param>
        /// <returns>
        /// a
        /// <see cref="SelectFieldItem"/>.
        /// </returns>
        public virtual SelectFieldItem GetOption(String value) {
            foreach (SelectFieldItem option in options) {
                if (option.GetExportValue().Equals(value)) {
                    return option;
                }
            }
            return null;
        }

        /// <summary>Gets a list of containers with option(s).</summary>
        /// <remarks>Gets a list of containers with option(s). Every container might be a container for options group.
        ///     </remarks>
        /// <returns>a list of containers with options.</returns>
        [System.ObsoleteAttribute(@"starting from 8.0.1.")]
        public virtual IList<IBlockElement> GetOptions() {
            IList<IBlockElement> blockElements = new List<IBlockElement>();
            foreach (SelectFieldItem option in options) {
                blockElements.Add(option.GetElement());
            }
            return blockElements;
        }

        /// <summary>Checks if the field has options with export and display values.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the field has options with export and display values,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool HasExportAndDisplayValues() {
            foreach (SelectFieldItem option in options) {
                if (option.HasExportAndDisplayValues()) {
                    return true;
                }
            }
            return false;
        }

        private String TryAndExtractText(IBlockElement optionElement) {
            String label = optionElement.GetProperty<String>(FormProperty.FORM_FIELD_LABEL);
            if (label != null) {
                return label;
            }
            foreach (IElement child in optionElement.GetChildren()) {
                if (child is Text) {
                    return ((Text)child).GetText();
                }
                else {
                    if (child is IBlockElement) {
                        return TryAndExtractText((IBlockElement)child);
                    }
                }
            }
            return "";
        }
    }
}
