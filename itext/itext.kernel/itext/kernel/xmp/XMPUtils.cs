// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================
using System;
using iText.Kernel.XMP.Impl;
using iText.Kernel.XMP.Options;

namespace iText.Kernel.XMP {
    /// <summary>Utility methods for XMP.</summary>
    /// <remarks>
    /// Utility methods for XMP. I included only those that are different from the
    /// Java default conversion utilities.
    /// </remarks>
    /// <since>21.02.2006</since>
    public sealed class XMPUtils {
        /// <summary>Private constructor</summary>
        private XMPUtils() {
        }

        // EMPTY
        /// <summary>Create a single edit string from an array of strings.</summary>
        /// <param name="xmp">The XMP object containing the array to be catenated.</param>
        /// <param name="schemaNS">
        /// The schema namespace URI for the array. Must not be null or
        /// the empty string.
        /// </param>
        /// <param name="arrayName">
        /// The name of the array. May be a general path expression, must
        /// not be null or the empty string. Each item in the array must
        /// be a simple string value.
        /// </param>
        /// <param name="separator">
        /// The string to be used to separate the items in the catenated
        /// string. Defaults to &amp;quot;; &amp;quot;, ASCII semicolon and space
        /// (U+003B, U+0020).
        /// </param>
        /// <param name="quotes">
        /// The characters to be used as quotes around array items that
        /// contain a separator. Defaults to '&amp;quot;'
        /// </param>
        /// <param name="allowCommas">Option flag to control the catenation.</param>
        /// <returns>Returns the string containing the catenated array items.</returns>
        public static String CatenateArrayItems(XMPMeta xmp, String schemaNS, String arrayName, String separator, 
            String quotes, bool allowCommas) {
            return XMPUtilsImpl.CatenateArrayItems(xmp, schemaNS, arrayName, separator, quotes, allowCommas);
        }

        /// <summary>Separate a single edit string into an array of strings.</summary>
        /// <param name="xmp">The XMP object containing the array to be updated.</param>
        /// <param name="schemaNS">
        /// The schema namespace URI for the array. Must not be null or
        /// the empty string.
        /// </param>
        /// <param name="arrayName">
        /// The name of the array. May be a general path expression, must
        /// not be null or the empty string. Each item in the array must
        /// be a simple string value.
        /// </param>
        /// <param name="catedStr">The string to be separated into the array items.</param>
        /// <param name="arrayOptions">Option flags to control the separation.</param>
        /// <param name="preserveCommas">Flag if commas shall be preserved</param>
        public static void SeparateArrayItems(XMPMeta xmp, String schemaNS, String arrayName, String catedStr, PropertyOptions
             arrayOptions, bool preserveCommas) {
            XMPUtilsImpl.SeparateArrayItems(xmp, schemaNS, arrayName, catedStr, arrayOptions, preserveCommas);
        }

        /// <summary>Remove multiple properties from an XMP object.</summary>
        /// <remarks>
        /// Remove multiple properties from an XMP object.
        /// RemoveProperties was created to support the File Info dialog's Delete
        /// button, and has been been generalized somewhat from those specific needs.
        /// It operates in one of three main modes depending on the schemaNS and
        /// propName parameters:
        /// <list type="bullet">
        /// <item><description> Non-empty <c>schemaNS</c> and <c>propName</c> - The named property is
        /// removed if it is an external property, or if the
        /// flag <c>doAllProperties</c> option is true. It does not matter whether the
        /// named property is an actual property or an alias.
        /// </description></item>
        /// <item><description> Non-empty <c>schemaNS</c> and empty <c>propName</c> - The all external
        /// properties in the named schema are removed. Internal properties are also
        /// removed if the flag <c>doAllProperties</c> option is set. In addition,
        /// aliases from the named schema will be removed if the flag <c>includeAliases</c>
        /// option is set.
        /// </description></item>
        /// <item><description> Empty <c>schemaNS</c> and empty <c>propName</c> - All external properties in
        /// all schema are removed. Internal properties are also removed if the
        /// flag <c>doAllProperties</c> option is passed. Aliases are implicitly handled
        /// because the associated actuals are internal if the alias is.
        /// </description></item>
        /// </list>
        /// It is an error to pass an empty <c>schemaNS</c> and non-empty <c>propName</c>.
        /// </remarks>
        /// <param name="xmp">The XMP object containing the properties to be removed.</param>
        /// <param name="schemaNS">
        /// Optional schema namespace URI for the properties to be
        /// removed.
        /// </param>
        /// <param name="propName">Optional path expression for the property to be removed.</param>
        /// <param name="doAllProperties">
        /// Option flag to control the deletion: do internal properties in
        /// addition to external properties.
        /// </param>
        /// <param name="includeAliases">
        /// Option flag to control the deletion:
        /// Include aliases in the "named schema" case above.
        /// <em>Note:</em> Currently not supported.
        /// </param>
        public static void RemoveProperties(XMPMeta xmp, String schemaNS, String propName, bool doAllProperties, bool
             includeAliases) {
            XMPUtilsImpl.RemoveProperties(xmp, schemaNS, propName, doAllProperties, includeAliases);
        }

        /// <summary>Alias without the new option <c>deleteEmptyValues</c>.</summary>
        /// <param name="source">The source XMP object.</param>
        /// <param name="dest">The destination XMP object.</param>
        /// <param name="doAllProperties">Do internal properties in addition to external properties.</param>
        /// <param name="replaceOldValues">Replace the values of existing properties.</param>
        public static void AppendProperties(XMPMeta source, XMPMeta dest, bool doAllProperties, bool replaceOldValues
            ) {
            AppendProperties(source, dest, doAllProperties, replaceOldValues, false);
        }

        /// <summary>Append properties from one XMP object to another.</summary>
        /// <remarks>
        /// Append properties from one XMP object to another.
        /// <para />XMPUtils#appendProperties was created to support the File Info dialog's Append button, and
        /// has been been generalized somewhat from those specific needs. It appends information from one
        /// XMP object (source) to another (dest). The default operation is to append only external
        /// properties that do not already exist in the destination. The flag
        /// <c>doAllProperties</c> can be used to operate on all properties, external and internal.
        /// The flag <c>replaceOldValues</c> option can be used to replace the values
        /// of existing properties. The notion of external
        /// versus internal applies only to top level properties. The keep-or-replace-old notion applies
        /// within structs and arrays as described below.
        /// <list type="bullet">
        /// <item><description>If <c>replaceOldValues</c> is true then the processing is restricted to the top
        /// level properties. The processed properties from the source (according to
        /// <c>doAllProperties</c>) are propagated to the destination,
        /// replacing any existing values.Properties in the destination that are not in the source
        /// are left alone.
        /// </description></item>
        /// <item><description>If <c>replaceOldValues</c> is not passed then the processing is more complicated.
        /// Top level properties are added to the destination if they do not already exist.
        /// If they do exist but differ in form (simple/struct/array) then the destination is left alone.
        /// If the forms match, simple properties are left unchanged while structs and arrays are merged.
        /// </description></item>
        /// <item><description>If <c>deleteEmptyValues</c> is passed then an empty value in the source XMP causes
        /// the corresponding destination XMP property to be deleted. The default is to treat empty
        /// values the same as non-empty values. An empty value is any of a simple empty string, an array
        /// with no items, or a struct with no fields. Qualifiers are ignored.
        /// </description></item>
        /// </list>
        /// <para />The detailed behavior is defined by the following pseudo-code:
        /// <blockquote>
        /// <pre>
        /// appendProperties ( sourceXMP, destXMP, doAllProperties,
        /// replaceOldValues, deleteEmptyValues ):
        /// for all source schema (top level namespaces):
        /// for all top level properties in sourceSchema:
        /// if doAllProperties or prop is external:
        /// appendSubtree ( sourceNode, destSchema, replaceOldValues, deleteEmptyValues )
        /// appendSubtree ( sourceNode, destParent, replaceOldValues, deleteEmptyValues ):
        /// if deleteEmptyValues and source value is empty:
        /// delete the corresponding child from destParent
        /// else if sourceNode not in destParent (by name):
        /// copy sourceNode's subtree to destParent
        /// else if replaceOld:
        /// delete subtree from destParent
        /// copy sourceNode's subtree to destParent
        /// else:
        /// // Already exists in dest and not replacing, merge structs and arrays
        /// if sourceNode and destNode forms differ:
        /// return, leave the destNode alone
        /// else if form is a struct:
        /// for each field in sourceNode:
        /// AppendSubtree ( sourceNode.field, destNode, replaceOldValues )
        /// else if form is an alt-text array:
        /// copy new items by "xml:lang" value into the destination
        /// else if form is an array:
        /// copy new items by value into the destination, ignoring order and duplicates
        /// </pre>
        /// </blockquote>
        /// <para /><em>Note:</em> appendProperties can be expensive if replaceOldValues is not passed and
        /// the XMP contains large arrays. The array item checking described above is n-squared.
        /// Each source item is checked to see if it already exists in the destination,
        /// without regard to order or duplicates.
        /// <para />Simple items are compared by value and "xml:lang" qualifier, other qualifiers are ignored.
        /// Structs are recursively compared by field names, without regard to field order. Arrays are
        /// compared by recursively comparing all items.
        /// </remarks>
        /// <param name="source">The source XMP object.</param>
        /// <param name="dest">The destination XMP object.</param>
        /// <param name="doAllProperties">Do internal properties in addition to external properties.</param>
        /// <param name="replaceOldValues">Replace the values of existing properties.</param>
        /// <param name="deleteEmptyValues">Delete destination values if source property is empty.</param>
        public static void AppendProperties(XMPMeta source, XMPMeta dest, bool doAllProperties, bool replaceOldValues
            , bool deleteEmptyValues) {
            XMPUtilsImpl.AppendProperties(source, dest, doAllProperties, replaceOldValues, deleteEmptyValues);
        }

        /// <summary>Convert from string to Boolean.</summary>
        /// <param name="value">The string representation of the Boolean.</param>
        /// <returns>
        /// The appropriate boolean value for the string. The checked values
        /// for <c>true</c> and <c>false</c> are:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="XMPConst.TRUESTR"/>
        /// and
        /// <see cref="XMPConst.FALSESTR"/>
        /// </description></item>
        /// <item><description>&amp;quot;t&amp;quot; and &amp;quot;f&amp;quot;
        /// </description></item>
        /// <item><description>&amp;quot;on&amp;quot; and &amp;quot;off&amp;quot;
        /// </description></item>
        /// <item><description>&amp;quot;yes&amp;quot; and &amp;quot;no&amp;quot;
        /// </description></item>
        /// <item><description>&amp;quot;value &lt;&gt; 0&amp;quot; and &amp;quot;value == 0&amp;quot;
        /// </description></item>
        /// </list>
        /// </returns>
        public static bool ConvertToBoolean(String value) {
            if (value == null || value.Length == 0) {
                throw new XMPException("Empty convert-string", XMPError.BADVALUE);
            }
            value = value.ToLowerInvariant();
            try {
                // First try interpretation as Integer (anything not 0 is true)
                return Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture) != 0;
            }
            catch (FormatException) {
                return "true".Equals(value) || "t".Equals(value) || "on".Equals(value) || "yes".Equals(value);
            }
        }

        /// <summary>Convert from boolean to string.</summary>
        /// <param name="value">a boolean value</param>
        /// <returns>
        /// The XMP string representation of the boolean. The values used are
        /// given by the constnts
        /// <see cref="XMPConst.TRUESTR"/>
        /// and
        /// <see cref="XMPConst.FALSESTR"/>.
        /// </returns>
        public static String ConvertFromBoolean(bool value) {
            return value ? XMPConst.TRUESTR : XMPConst.FALSESTR;
        }

        /// <summary>Converts a string value to an <c>int</c>.</summary>
        /// <param name="rawValue">the string value</param>
        /// <returns>Returns an int.</returns>
        public static int ConvertToInteger(String rawValue) {
            try {
                if (rawValue == null || rawValue.Length == 0) {
                    throw new XMPException("Empty convert-string", XMPError.BADVALUE);
                }
                if (rawValue.StartsWith("0x")) {
                    return Convert.ToInt32(rawValue.Substring(2), 16);
                }
                else {
                    return Convert.ToInt32(rawValue, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (FormatException) {
                throw new XMPException("Invalid integer string", XMPError.BADVALUE);
            }
        }

        /// <summary>Convert from int to string.</summary>
        /// <param name="value">an int value</param>
        /// <returns>The string representation of the int.</returns>
        public static String ConvertFromInteger(int value) {
            return value.ToString();
        }

        /// <summary>Converts a string value to a <c>long</c>.</summary>
        /// <param name="rawValue">the string value</param>
        /// <returns>Returns a long.</returns>
        public static long ConvertToLong(String rawValue) {
            try {
                if (rawValue == null || rawValue.Length == 0) {
                    throw new XMPException("Empty convert-string", XMPError.BADVALUE);
                }
                if (rawValue.StartsWith("0x")) {
                    return Convert.ToInt64(rawValue.Substring(2), 16);
                }
                else {
                    return Convert.ToInt64(rawValue, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (FormatException) {
                throw new XMPException("Invalid long string", XMPError.BADVALUE);
            }
        }

        /// <summary>Convert from long to string.</summary>
        /// <param name="value">a long value</param>
        /// <returns>The string representation of the long.</returns>
        public static String ConvertFromLong(long value) {
            return value.ToString();
        }

        /// <summary>Converts a string value to a <c>double</c>.</summary>
        /// <param name="rawValue">the string value</param>
        /// <returns>Returns a double.</returns>
        public static double ConvertToDouble(String rawValue) {
            try {
                if (rawValue == null || rawValue.Length == 0) {
                    throw new XMPException("Empty convert-string", XMPError.BADVALUE);
                }
                else {
                    return Double.Parse(rawValue, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (FormatException) {
                throw new XMPException("Invalid double string", XMPError.BADVALUE);
            }
        }

        /// <summary>Convert from long to string.</summary>
        /// <param name="value">a long value</param>
        /// <returns>The string representation of the long.</returns>
        public static String ConvertFromDouble(double value) {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>Converts a string value to an <c>XMPDateTime</c>.</summary>
        /// <param name="rawValue">the string value</param>
        /// <returns>Returns an <c>XMPDateTime</c>-object.</returns>
        public static XMPDateTime ConvertToDate(String rawValue) {
            if (rawValue == null || rawValue.Length == 0) {
                throw new XMPException("Empty convert-string", XMPError.BADVALUE);
            }
            else {
                return ISO8601Converter.Parse(rawValue);
            }
        }

        /// <summary>Convert from <c>XMPDateTime</c> to string.</summary>
        /// <param name="value">an <c>XMPDateTime</c></param>
        /// <returns>The string representation of the long.</returns>
        public static String ConvertFromDate(XMPDateTime value) {
            return ISO8601Converter.Render(value);
        }

        /// <summary>Convert from a byte array to a base64 encoded string.</summary>
        /// <param name="buffer">the byte array to be converted</param>
        /// <returns>Returns the base64 string.</returns>
        public static String EncodeBase64(byte[] buffer) {
            return iText.Commons.Utils.JavaUtil.GetStringForBytes(Base64.Encode(buffer));
        }

        /// <summary>Decode from Base64 encoded string to raw data.</summary>
        /// <param name="base64String">a base64 encoded string</param>
        /// <returns>Returns a byte array containg the decoded string.</returns>
        public static byte[] DecodeBase64(String base64String) {
            try {
                return Base64.Decode(base64String.GetBytes());
            }
            catch (Exception e) {
                throw new XMPException("Invalid base64 string", XMPError.BADVALUE, e);
            }
        }
    }
}
