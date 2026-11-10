/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
