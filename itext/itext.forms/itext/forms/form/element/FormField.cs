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
using iText.Forms.Form;
using iText.Layout.Element;

namespace iText.Forms.Form.Element {
    /// <summary>
    /// Implementation of the
    /// <see cref="iText.Layout.Element.AbstractElement{T}"/>
    /// class for form fields.
    /// </summary>
    /// <typeparam name="T">the generic type of the form field (e.g. input field, button, text area)</typeparam>
    public abstract class FormField<T> : AbstractElement<T>, IFormField
        where T : IFormField {
        /// <summary>The id.</summary>
        private readonly String id;

        /// <summary>
        /// Instantiates a new
        /// <see cref="FormField{T}"/>
        /// instance.
        /// </summary>
        /// <param name="id">the id</param>
        internal FormField(String id) {
            if (id == null || id.Contains(".")) {
                throw new ArgumentException("id should not contain '.'");
            }
            this.id = id;
        }

        /* (non-Javadoc)
        * @see IFormField#getId()
        */
        public virtual String GetId() {
            return id;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.layout.ElementPropertyContainer#getDefaultProperty(int)
        */
        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case FormProperty.FORM_FIELD_FLATTEN: {
                    return (T1)(Object)true;
                }

                case FormProperty.FORM_FIELD_VALUE: {
                    return (T1)(Object)"";
                }

                default: {
                    return base.GetDefaultProperty<T1>(property);
                }
            }
        }
    }
}
