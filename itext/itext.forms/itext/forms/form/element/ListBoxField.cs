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
using iText.Forms.Form;
using iText.Forms.Form.Renderer;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Element {
    /// <summary>A field that represents a control for selecting one or several of the provided options.</summary>
    public class ListBoxField : AbstractSelectField {
        /// <summary>Creates a new list box field.</summary>
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

        protected override IRenderer MakeNewRenderer() {
            return new SelectFieldListBoxRenderer(this);
        }
    }
}
