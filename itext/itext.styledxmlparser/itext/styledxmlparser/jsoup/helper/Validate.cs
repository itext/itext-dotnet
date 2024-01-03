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

        /// <summary>Validates that the string is not null and is not empty</summary>
        /// <param name="string">the string to test</param>
        public static void NotEmpty(String @string) {
            if (@string == null || @string.Length == 0) {
                throw new ArgumentException("String must not be empty");
            }
        }

        /// <summary>Validates that the string is not null and is not empty</summary>
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
