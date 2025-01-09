/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
