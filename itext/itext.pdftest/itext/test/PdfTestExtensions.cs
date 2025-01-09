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
using System.Linq;
using System.Reflection;

namespace iText.Test {
    internal static class PdfTestExtensions {
        public static Attribute GetCustomAttribute(this Type classType, Type attributeType) {
#if !NETSTANDARD2_0
            return Attribute.GetCustomAttribute(classType, attributeType);
#else
            return classType.GetTypeInfo().GetCustomAttribute(attributeType);
#endif
        }

        public static Assembly GetAssembly(this Type type) {
#if !NETSTANDARD2_0
            return type.Assembly;
#else
            return type.GetTypeInfo().Assembly;
#endif
        }

#if NETSTANDARD2_0
        public static object[] GetCustomAttributes(this Type type, Type attributeType, bool inherit) {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
        }
#endif
    }
}
