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

namespace iText.Layout {
    /// <summary>
    /// A generic Map-like interface that defines methods for storing and retrieving
    /// objects by an enum key of the
    /// <see cref="iText.Layout.Properties.Property"/>
    /// type.
    /// </summary>
    public interface IPropertyContainer {
        /// <summary>Checks if this entity has the specified property.</summary>
        /// <remarks>
        /// Checks if this entity has the specified property. Compared to
        /// <see cref="HasOwnProperty(int)"/>
        /// ,
        /// this method can check parent's properties, styles, etc, depending on the origin of the instance
        /// </remarks>
        /// <param name="property">the property to be checked</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this instance has given property,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        bool HasProperty(int property);

        /// <summary>Checks if this entity has the specified property, i.e. if it was set to this very element earlier
        ///     </summary>
        /// <param name="property">the property to be checked</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if this instance has given own property,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        bool HasOwnProperty(int property);

        /// <summary>Gets the property from this entity.</summary>
        /// <remarks>
        /// Gets the property from this entity. Compared to
        /// <see cref="GetOwnProperty{T1}(int)"/>
        /// ,
        /// this method can check parent's properties, styles, etc, depending on the origin of the instance
        /// </remarks>
        /// <typeparam name="T1">the return type associated with the property</typeparam>
        /// <param name="property">the property to be retrieved</param>
        /// <returns>
        /// the value of the given property.
        /// <see langword="null"/>
        /// will be returned if the property value was not found
        /// </returns>
        T1 GetProperty<T1>(int property);

        /// <summary>Gets own property from this entity.</summary>
        /// <remarks>
        /// Gets own property from this entity. The property must have been set earlier to this entity.
        /// If the property is not found,
        /// <see langword="null"/>
        /// will be returned.
        /// </remarks>
        /// <typeparam name="T1">the return type associated with the property</typeparam>
        /// <param name="property">the property to be retrieved</param>
        /// <returns>
        /// the value of the given own property.
        /// <see langword="null"/>
        /// will be returned if the property value was not found
        /// </returns>
        T1 GetOwnProperty<T1>(int property);

        /// <summary>Gets the default property from this entity.</summary>
        /// <typeparam name="T1">the return type associated with the property</typeparam>
        /// <param name="property">the property to be retrieved</param>
        /// <returns>
        /// the default property value. If the default property is not defined,
        /// <see langword="null"/>
        /// will be returned
        /// </returns>
        T1 GetDefaultProperty<T1>(int property);

        /// <summary>Sets a property for this entity.</summary>
        /// <param name="property">the property to be set</param>
        /// <param name="value">the value of the property</param>
        void SetProperty(int property, Object value);

        /// <summary>Deletes the own property of this entity.</summary>
        /// <param name="property">the property to be deleted</param>
        void DeleteOwnProperty(int property);
    }
}
