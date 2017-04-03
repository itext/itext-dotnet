/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.Layout.Properties;

namespace iText.Layout.Renderer
{
    /// <summary>
    /// Helper class for easier acces for float and int properties.
    /// </summary>
    public static class AbstractRendererExtensions
    {

        /// <summary>Returns a property with a certain key, as a floating point value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="float?"/>
        /// </returns>
        public static float? GetPropertyAsFloat(this AbstractRenderer renderer, int property)
        {
            Object value = renderer.GetProperty<Object>(property);
            return (value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal)
                ? Convert.ToSingle(value) : (float?)null;
        }

        /// <summary>Returns a property with a certain key, as a floating point value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="Property">enum value</see>
        /// </param>
        /// <param name="defaultValue">default value to be returned if property is not found</param>
        /// <returns>
        /// a
        /// <see cref="float?"/>
        /// </returns>
        public static float? GetPropertyAsFloat(this AbstractRenderer renderer, int property, float? defaultValue)
        {

            Object value = renderer.GetProperty<Object>(property, defaultValue);
            return (value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal)
                ? Convert.ToSingle(value) : (float?)null;
        }

        /// <summary>Returns a property with a certain key, as an integer value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="int?"/>
        /// </returns>
        public static int? GetPropertyAsInteger(this AbstractRenderer renderer, int property)
        {
            Object value = renderer.GetProperty<Object>(property);
            return (value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal)
                ? Convert.ToInt32(value) : (int?)null;
        }
    }
}