/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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

namespace iText.StyledXmlParser.Jsoup.Helper {
    /// <summary>Simple validation methods.</summary>
    /// <remarks>Simple validation methods. Designed for jsoup internal use</remarks>
    public sealed class Validate {
        private Validate() {
        }

        /// <summary>Validates that the object is not null</summary>
        /// <param name="obj">object to test</param>
        public static void NotNull(Object obj) {
            if (obj == null) {
                throw new ArgumentException("Object must not be null");
            }
        }

        /// <summary>Validates that the object is not null</summary>
        /// <param name="obj">object to test</param>
        /// <param name="msg">message to output if validation fails</param>
        public static void NotNull(Object obj, String msg) {
            if (obj == null) {
                throw new ArgumentException(msg);
            }
        }

        /// <summary>Validates that the value is true</summary>
        /// <param name="val">object to test</param>
        public static void IsTrue(bool val) {
            if (!val) {
                throw new ArgumentException("Must be true");
            }
        }

        /// <summary>Validates that the value is true</summary>
        /// <param name="val">object to test</param>
        /// <param name="msg">message to output if validation fails</param>
        public static void IsTrue(bool val, String msg) {
            if (!val) {
                throw new ArgumentException(msg);
            }
        }

        /// <summary>Validates that the value is false</summary>
        /// <param name="val">object to test</param>
        public static void IsFalse(bool val) {
            if (val) {
                throw new ArgumentException("Must be false");
            }
        }

        /// <summary>Validates that the value is false</summary>
        /// <param name="val">object to test</param>
        /// <param name="msg">message to output if validation fails</param>
        public static void IsFalse(bool val, String msg) {
            if (val) {
                throw new ArgumentException(msg);
            }
        }

        /// <summary>Validates that the array contains no null elements</summary>
        /// <param name="objects">the array to test</param>
        public static void NoNullElements(Object[] objects) {
            NoNullElements(objects, "Array must not contain any null objects");
        }

        /// <summary>Validates that the array contains no null elements</summary>
        /// <param name="objects">the array to test</param>
        /// <param name="msg">message to output if validation fails</param>
        public static void NoNullElements(Object[] objects, String msg) {
            foreach (Object obj in objects) {
                if (obj == null) {
                    throw new ArgumentException(msg);
                }
            }
        }

        /// <summary>Validates that the string is not empty</summary>
        /// <param name="string">the string to test</param>
        public static void NotEmpty(String @string) {
            if (@string == null || @string.Length == 0) {
                throw new ArgumentException("String must not be empty");
            }
        }

        /// <summary>Validates that the string is not empty</summary>
        /// <param name="string">the string to test</param>
        /// <param name="msg">message to output if validation fails</param>
        public static void NotEmpty(String @string, String msg) {
            if (@string == null || @string.Length == 0) {
                throw new ArgumentException(msg);
            }
        }

        /// <summary>Cause a failure.</summary>
        /// <param name="msg">message to output.</param>
        public static void Fail(String msg) {
            throw new ArgumentException(msg);
        }
    }
}
