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
using iText.Layout.Element;

namespace iText.Forms.Form.Element {
    /// <summary>Common interface for HTML form elements.</summary>
    public interface IFormField : IBlockElement {
        /// <summary>
        /// Sets the
        /// <see cref="iText.Forms.Form.FormProperty.FORM_FIELD_VALUE"/>
        /// property.
        /// </summary>
        /// <param name="value">string value of the property to be set.</param>
        /// <returns>
        /// this same
        /// <see cref="IFormField"/>
        /// instance.
        /// </returns>
        IFormField SetValue(String value);

        /// <summary>Set the form field to be interactive and added into Acroform instead of drawing it on a page.</summary>
        /// <param name="interactive">
        /// 
        /// <see langword="true"/>
        /// if the form field element shall be added into Acroform,
        /// <see langword="false"/>
        /// otherwise.
        /// By default, the form field element is not interactive and drawn on a page.
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="IFormField"/>
        /// instance.
        /// </returns>
        IFormField SetInteractive(bool interactive);

        /// <summary>Gets the id.</summary>
        /// <returns>the id.</returns>
        String GetId();
    }
}
