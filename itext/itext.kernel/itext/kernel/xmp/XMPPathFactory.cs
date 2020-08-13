//Copyright (c) 2006, Adobe Systems Incorporated
//All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//        3. All advertising materials mentioning features or use of this software
//        must display the following acknowledgement:
//        This product includes software developed by the Adobe Systems Incorporated.
//        4. Neither the name of the Adobe Systems Incorporated nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED BY ADOBE SYSTEMS INCORPORATED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL ADOBE SYSTEMS INCORPORATED BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//        http://www.adobe.com/devnet/xmp/library/eula-xmp-library-java.html
using System;
using iText.Kernel.XMP.Impl;
using iText.Kernel.XMP.Impl.XPath;

namespace iText.Kernel.XMP {
    /// <summary>Utility services for the metadata object.</summary>
    /// <remarks>
    /// Utility services for the metadata object. It has only public static functions, you cannot create
    /// an object. These are all functions that layer cleanly on top of the kernel XMP toolkit.
    /// <para />
    /// These functions provide support for composing path expressions to deeply nested properties. The
    /// functions <c>XMPMeta</c> such as <c>getProperty()</c>,
    /// <c>getArrayItem()</c> and <c>getStructField()</c> provide easy access to top
    /// level simple properties, items in top level arrays, and fields of top level structs. They do not
    /// provide convenient access to more complex things like fields several levels deep in a complex
    /// struct, or fields within an array of structs, or items of an array that is a field of a struct.
    /// These functions can also be used to compose paths to top level array items or struct fields so
    /// that you can use the binary accessors like <c>getPropertyAsInteger()</c>.
    /// <para />
    /// You can use these functions is to compose a complete path expression, or all but the last
    /// component. Suppose you have a property that is an array of integers within a struct. You can
    /// access one of the array items like this:
    /// <blockquote>
    /// <pre>
    /// String path = XMPPathFactory.composeStructFieldPath (schemaNS, &amp;quot;Struct&amp;quot;, fieldNS,
    /// &amp;quot;Array&amp;quot;);
    /// String path += XMPPathFactory.composeArrayItemPath (schemaNS, &amp;quot;Array&amp;quot; index);
    /// PropertyInteger result = xmpObj.getPropertyAsInteger(schemaNS, path);
    /// </pre>
    /// </blockquote> You could also use this code if you want the string form of the integer:
    /// <blockquote>
    /// <pre>
    /// String path = XMPPathFactory.composeStructFieldPath (schemaNS, &amp;quot;Struct&amp;quot;, fieldNS,
    /// &amp;quot;Array&amp;quot;);
    /// PropertyText xmpObj.getArrayItem (schemaNS, path, index);
    /// </pre>
    /// </blockquote>
    /// <para />
    /// <em>Note:</em> It might look confusing that the schemaNS is passed in all of the calls above.
    /// This is because the XMP toolkit keeps the top level &amp;quot;schema&amp;quot; namespace separate from
    /// the rest of the path expression.
    /// <em>Note:</em> These methods are much simpler than in the C++-API, they don't check the given
    /// path or array indices.
    /// </remarks>
    /// <since>25.01.2006</since>
    public sealed class XMPPathFactory {
        /// <summary>Private constructor</summary>
        private XMPPathFactory() {
        }

        // EMPTY
        /// <summary>Compose the path expression for an item in an array.</summary>
        /// <param name="arrayName">
        /// The name of the array. May be a general path expression, must not be
        /// <c>null</c> or the empty string.
        /// </param>
        /// <param name="itemIndex">
        /// The index of the desired item. Arrays in XMP are indexed from 1.
        /// 0 and below means last array item and renders as <c>[last()]</c>.
        /// </param>
        /// <returns>
        /// Returns the composed path basing on fullPath. This will be of the form
        /// <tt>ns:arrayName[i]</tt>, where &amp;quot;ns&amp;quot; is the prefix for schemaNS and
        /// &amp;quot;i&amp;quot; is the decimal representation of itemIndex.
        /// </returns>
        public static String ComposeArrayItemPath(String arrayName, int itemIndex) {
            if (itemIndex > 0) {
                return arrayName + '[' + itemIndex + ']';
            }
            else {
                if (itemIndex == XMPConst.ARRAY_LAST_ITEM) {
                    return arrayName + "[last()]";
                }
                else {
                    throw new XMPException("Array index must be larger than zero", XMPError.BADINDEX);
                }
            }
        }

        /// <summary>Compose the path expression for a field in a struct.</summary>
        /// <remarks>
        /// Compose the path expression for a field in a struct. The result can be added to the
        /// path of
        /// </remarks>
        /// <param name="fieldNS">
        /// The namespace URI for the field. Must not be <c>null</c> or the empty
        /// string.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field. Must be a simple XML name, must not be
        /// <c>null</c> or the empty string.
        /// </param>
        /// <returns>
        /// Returns the composed path. This will be of the form
        /// <tt>ns:structName/fNS:fieldName</tt>, where &amp;quot;ns&amp;quot; is the prefix for
        /// schemaNS and &amp;quot;fNS&amp;quot; is the prefix for fieldNS.
        /// </returns>
        public static String ComposeStructFieldPath(String fieldNS, String fieldName) {
            AssertFieldNS(fieldNS);
            AssertFieldName(fieldName);
            XMPPath fieldPath = XMPPathParser.ExpandXPath(fieldNS, fieldName);
            if (fieldPath.Size() != 2) {
                throw new XMPException("The field name must be simple", XMPError.BADXPATH);
            }
            return '/' + fieldPath.GetSegment(XMPPath.STEP_ROOT_PROP).GetName();
        }

        /// <summary>Compose the path expression for a qualifier.</summary>
        /// <param name="qualNS">
        /// The namespace URI for the qualifier. May be <c>null</c> or the empty
        /// string if the qualifier is in the XML empty namespace.
        /// </param>
        /// <param name="qualName">
        /// The name of the qualifier. Must be a simple XML name, must not be
        /// <c>null</c> or the empty string.
        /// </param>
        /// <returns>
        /// Returns the composed path. This will be of the form
        /// <tt>ns:propName/?qNS:qualName</tt>, where &amp;quot;ns&amp;quot; is the prefix for
        /// schemaNS and &amp;quot;qNS&amp;quot; is the prefix for qualNS.
        /// </returns>
        public static String ComposeQualifierPath(String qualNS, String qualName) {
            AssertQualNS(qualNS);
            AssertQualName(qualName);
            XMPPath qualPath = XMPPathParser.ExpandXPath(qualNS, qualName);
            if (qualPath.Size() != 2) {
                throw new XMPException("The qualifier name must be simple", XMPError.BADXPATH);
            }
            return "/?" + qualPath.GetSegment(XMPPath.STEP_ROOT_PROP).GetName();
        }

        /// <summary>Compose the path expression to select an alternate item by language.</summary>
        /// <remarks>
        /// Compose the path expression to select an alternate item by language. The
        /// path syntax allows two forms of &amp;quot;content addressing&amp;quot; that may
        /// be used to select an item in an array of alternatives. The form used in
        /// ComposeLangSelector lets you select an item in an alt-text array based on
        /// the value of its <tt>xml:lang</tt> qualifier. The other form of content
        /// addressing is shown in ComposeFieldSelector. \note ComposeLangSelector
        /// does not supplant SetLocalizedText or GetLocalizedText. They should
        /// generally be used, as they provide extra logic to choose the appropriate
        /// language and maintain consistency with the 'x-default' value.
        /// ComposeLangSelector gives you an path expression that is explicitly and
        /// only for the language given in the langName parameter.
        /// </remarks>
        /// <param name="arrayName">
        /// The name of the array. May be a general path expression, must
        /// not be <c>null</c> or the empty string.
        /// </param>
        /// <param name="langName">The RFC 3066 code for the desired language.</param>
        /// <returns>
        /// Returns the composed path. This will be of the form
        /// <tt>ns:arrayName[@xml:lang='langName']</tt>, where
        /// &amp;quot;ns&amp;quot; is the prefix for schemaNS.
        /// </returns>
        public static String ComposeLangSelector(String arrayName, String langName) {
            return arrayName + "[?xml:lang=\"" + iText.Kernel.XMP.Impl.Utils.NormalizeLangValue(langName) + "\"]";
        }

        /// <summary>Compose the path expression to select an alternate item by a field's value.</summary>
        /// <remarks>
        /// Compose the path expression to select an alternate item by a field's value. The path syntax
        /// allows two forms of &amp;quot;content addressing&amp;quot; that may be used to select an item in an
        /// array of alternatives. The form used in ComposeFieldSelector lets you select an item in an
        /// array of structs based on the value of one of the fields in the structs. The other form of
        /// content addressing is shown in ComposeLangSelector. For example, consider a simple struct
        /// that has two fields, the name of a city and the URI of an FTP site in that city. Use this to
        /// create an array of download alternatives. You can show the user a popup built from the values
        /// of the city fields. You can then get the corresponding URI as follows:
        /// <blockquote>
        /// <pre>
        /// String path = composeFieldSelector ( schemaNS, &amp;quot;Downloads&amp;quot;, fieldNS,
        /// &amp;quot;City&amp;quot;, chosenCity );
        /// XMPProperty prop = xmpObj.getStructField ( schemaNS, path, fieldNS, &amp;quot;URI&amp;quot; );
        /// </pre>
        /// </blockquote>
        /// </remarks>
        /// <param name="arrayName">
        /// The name of the array. May be a general path expression, must not be
        /// <c>null</c> or the empty string.
        /// </param>
        /// <param name="fieldNS">
        /// The namespace URI for the field used as the selector. Must not be
        /// <c>null</c> or the empty string.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field used as the selector. Must be a simple XML name, must
        /// not be <c>null</c> or the empty string. It must be the name of a field that is
        /// itself simple.
        /// </param>
        /// <param name="fieldValue">The desired value of the field.</param>
        /// <returns>
        /// Returns the composed path. This will be of the form
        /// <tt>ns:arrayName[fNS:fieldName='fieldValue']</tt>, where &amp;quot;ns&amp;quot; is the
        /// prefix for schemaNS and &amp;quot;fNS&amp;quot; is the prefix for fieldNS.
        /// </returns>
        public static String ComposeFieldSelector(String arrayName, String fieldNS, String fieldName, String fieldValue
            ) {
            XMPPath fieldPath = XMPPathParser.ExpandXPath(fieldNS, fieldName);
            if (fieldPath.Size() != 2) {
                throw new XMPException("The fieldName name must be simple", XMPError.BADXPATH);
            }
            return arrayName + '[' + fieldPath.GetSegment(XMPPath.STEP_ROOT_PROP).GetName() + "=\"" + fieldValue + "\"]";
        }

        /// <summary>ParameterAsserts that a qualifier namespace is set.</summary>
        /// <param name="qualNS">a qualifier namespace</param>
        private static void AssertQualNS(String qualNS) {
            if (qualNS == null || qualNS.Length == 0) {
                throw new XMPException("Empty qualifier namespace URI", XMPError.BADSCHEMA);
            }
        }

        /// <summary>ParameterAsserts that a qualifier name is set.</summary>
        /// <param name="qualName">a qualifier name or path</param>
        private static void AssertQualName(String qualName) {
            if (qualName == null || qualName.Length == 0) {
                throw new XMPException("Empty qualifier name", XMPError.BADXPATH);
            }
        }

        /// <summary>ParameterAsserts that a struct field namespace is set.</summary>
        /// <param name="fieldNS">a struct field namespace</param>
        private static void AssertFieldNS(String fieldNS) {
            if (fieldNS == null || fieldNS.Length == 0) {
                throw new XMPException("Empty field namespace URI", XMPError.BADSCHEMA);
            }
        }

        /// <summary>ParameterAsserts that a struct field name is set.</summary>
        /// <param name="fieldName">a struct field name or path</param>
        private static void AssertFieldName(String fieldName) {
            if (fieldName == null || fieldName.Length == 0) {
                throw new XMPException("Empty f name", XMPError.BADXPATH);
            }
        }
    }
}
