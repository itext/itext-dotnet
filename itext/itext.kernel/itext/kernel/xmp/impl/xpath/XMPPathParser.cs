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
using iText.Kernel.XMP;
using iText.Kernel.XMP.Impl;
using iText.Kernel.XMP.Properties;

namespace iText.Kernel.XMP.Impl.XPath {
    /// <summary>Parser for XMP XPaths.</summary>
    /// <since>01.03.2006</since>
    public sealed class XMPPathParser {
        /// <summary>Private constructor</summary>
        private XMPPathParser() {
        }

        // empty
        /// <summary>
        /// Split an XMPPath expression apart at the conceptual steps, adding the
        /// root namespace prefix to the first property component.
        /// </summary>
        /// <remarks>
        /// Split an XMPPath expression apart at the conceptual steps, adding the
        /// root namespace prefix to the first property component. The schema URI is
        /// put in the first (0th) slot in the expanded XMPPath. Check if the top
        /// level component is an alias, but don't resolve it.
        /// <para />
        /// In the most verbose case steps are separated by '/', and each step can be
        /// of these forms:
        /// <list type="table">
        /// <item><term>prefix:name
        /// </term>
        /// <description> A top level property or struct field.
        /// </description></item>
        /// <item><term>[index]
        /// </term>
        /// <description> An element of an array.
        /// </description></item>
        /// <item><term>[last()]
        /// </term>
        /// <description> The last element of an array.
        /// </description></item>
        /// <item><term>[fieldName=&amp;quot;value&amp;quot;]
        /// </term>
        /// <description> An element in an array of structs, chosen by a field value.
        /// </description></item>
        /// <item><term>[@xml:lang=&amp;quot;value&amp;quot;]
        /// </term>
        /// <description> An element in an alt-text array, chosen by the xml:lang qualifier.
        /// </description></item>
        /// <item><term>[?qualName=&amp;quot;value&amp;quot;]
        /// </term>
        /// <description> An element in an array, chosen by a qualifier value.
        /// </description></item>
        /// <item><term>@xml:lang
        /// </term>
        /// <description> An xml:lang qualifier.
        /// </description></item>
        /// <item><term>?qualName
        /// </term>
        /// <description> A general qualifier.
        /// </description></item>
        /// </list>
        /// <para />
        /// The logic is complicated though by shorthand for arrays, the separating
        /// '/' and leading '*' are optional. These are all equivalent: array/*[2]
        /// array/[2] array*[2] array[2] All of these are broken into the 2 steps
        /// "array" and "[2]".
        /// <para />
        /// The value portion in the array selector forms is a string quoted by '''
        /// or '"'. The value may contain any character including a doubled quoting
        /// character. The value may be empty.
        /// <para />
        /// The syntax isn't checked, but an XML name begins with a letter or '_',
        /// and contains letters, digits, '.', '-', '_', and a bunch of special
        /// non-ASCII Unicode characters. An XML qualified name is a pair of names
        /// separated by a colon.
        /// </remarks>
        /// <param name="schemaNS">schema namespace</param>
        /// <param name="path">property name</param>
        /// <returns>Returns the expandet XMPPath.</returns>
        public static XMPPath ExpandXPath(String schemaNS, String path) {
            if (schemaNS == null || path == null) {
                throw new XMPException("Parameter must not be null", XMPError.BADPARAM);
            }
            XMPPath expandedXPath = new XMPPath();
            PathPosition pos = new PathPosition();
            pos.path = path;
            // Pull out the first component and do some special processing on it: add the schema
            // namespace prefix and and see if it is an alias. The start must be a "qualName".
            ParseRootNode(schemaNS, pos, expandedXPath);
            // Now continue to process the rest of the XMPPath string.
            while (pos.stepEnd < path.Length) {
                pos.stepBegin = pos.stepEnd;
                SkipPathDelimiter(path, pos);
                pos.stepEnd = pos.stepBegin;
                XMPPathSegment segment;
                if (path[pos.stepBegin] != '[') {
                    // A struct field or qualifier.
                    segment = ParseStructSegment(pos);
                }
                else {
                    // One of the array forms.
                    segment = ParseIndexSegment(pos);
                }
                if (segment.GetKind() == XMPPath.STRUCT_FIELD_STEP) {
                    if (segment.GetName()[0] == '@') {
                        segment.SetName("?" + segment.GetName().Substring(1));
                        if (!"?xml:lang".Equals(segment.GetName())) {
                            throw new XMPException("Only xml:lang allowed with '@'", XMPError.BADXPATH);
                        }
                    }
                    if (segment.GetName()[0] == '?') {
                        pos.nameStart++;
                        segment.SetKind(XMPPath.QUALIFIER_STEP);
                    }
                    VerifyQualName(pos.path.JSubstring(pos.nameStart, pos.nameEnd));
                }
                else {
                    if (segment.GetKind() == XMPPath.FIELD_SELECTOR_STEP) {
                        if (segment.GetName()[1] == '@') {
                            segment.SetName("[?" + segment.GetName().Substring(2));
                            if (!segment.GetName().StartsWith("[?xml:lang=")) {
                                throw new XMPException("Only xml:lang allowed with '@'", XMPError.BADXPATH);
                            }
                        }
                        if (segment.GetName()[1] == '?') {
                            pos.nameStart++;
                            segment.SetKind(XMPPath.QUAL_SELECTOR_STEP);
                            VerifyQualName(pos.path.JSubstring(pos.nameStart, pos.nameEnd));
                        }
                    }
                }
                expandedXPath.Add(segment);
            }
            return expandedXPath;
        }

        /// <param name="path"/>
        /// <param name="pos"/>
        private static void SkipPathDelimiter(String path, PathPosition pos) {
            if (path[pos.stepBegin] == '/') {
                // skip slash
                pos.stepBegin++;
                // added for Java
                if (pos.stepBegin >= path.Length) {
                    throw new XMPException("Empty XMPPath segment", XMPError.BADXPATH);
                }
            }
            if (path[pos.stepBegin] == '*') {
                // skip asterisk
                pos.stepBegin++;
                if (pos.stepBegin >= path.Length || path[pos.stepBegin] != '[') {
                    throw new XMPException("Missing '[' after '*'", XMPError.BADXPATH);
                }
            }
        }

        /// <summary>Parses a struct segment</summary>
        /// <param name="pos">the current position in the path</param>
        /// <returns>Retusn the segment or an errror</returns>
        private static XMPPathSegment ParseStructSegment(PathPosition pos) {
            pos.nameStart = pos.stepBegin;
            while (pos.stepEnd < pos.path.Length && "/[*".IndexOf(pos.path[pos.stepEnd]) < 0) {
                pos.stepEnd++;
            }
            pos.nameEnd = pos.stepEnd;
            if (pos.stepEnd == pos.stepBegin) {
                throw new XMPException("Empty XMPPath segment", XMPError.BADXPATH);
            }
            // ! Touch up later, also changing '@' to '?'.
            XMPPathSegment segment = new XMPPathSegment(pos.path.JSubstring(pos.stepBegin, pos.stepEnd), XMPPath.STRUCT_FIELD_STEP
                );
            return segment;
        }

        /// <summary>Parses an array index segment.</summary>
        /// <param name="pos">the xmp path</param>
        /// <returns>Returns the segment or an error</returns>
        private static XMPPathSegment ParseIndexSegment(PathPosition pos) {
            XMPPathSegment segment;
            pos.stepEnd++;
            // Look at the character after the leading '['.
            if ('0' <= pos.path[pos.stepEnd] && pos.path[pos.stepEnd] <= '9') {
                // A numeric (decimal integer) array index.
                while (pos.stepEnd < pos.path.Length && '0' <= pos.path[pos.stepEnd] && pos.path[pos.stepEnd] <= '9') {
                    pos.stepEnd++;
                }
                segment = new XMPPathSegment(null, XMPPath.ARRAY_INDEX_STEP);
            }
            else {
                // Could be "[last()]" or one of the selector forms. Find the ']' or '='.
                while (pos.stepEnd < pos.path.Length && pos.path[pos.stepEnd] != ']' && pos.path[pos.stepEnd] != '=') {
                    pos.stepEnd++;
                }
                if (pos.stepEnd >= pos.path.Length) {
                    throw new XMPException("Missing ']' or '=' for array index", XMPError.BADXPATH);
                }
                if (pos.path[pos.stepEnd] == ']') {
                    if (!"[last()".Equals(pos.path.JSubstring(pos.stepBegin, pos.stepEnd))) {
                        throw new XMPException("Invalid non-numeric array index", XMPError.BADXPATH);
                    }
                    segment = new XMPPathSegment(null, XMPPath.ARRAY_LAST_STEP);
                }
                else {
                    pos.nameStart = pos.stepBegin + 1;
                    pos.nameEnd = pos.stepEnd;
                    pos.stepEnd++;
                    // Absorb the '=', remember the quote.
                    char quote = pos.path[pos.stepEnd];
                    if (quote != '\'' && quote != '"') {
                        throw new XMPException("Invalid quote in array selector", XMPError.BADXPATH);
                    }
                    pos.stepEnd++;
                    // Absorb the leading quote.
                    while (pos.stepEnd < pos.path.Length) {
                        if (pos.path[pos.stepEnd] == quote) {
                            // check for escaped quote
                            if (pos.stepEnd + 1 >= pos.path.Length || pos.path[pos.stepEnd + 1] != quote) {
                                break;
                            }
                            pos.stepEnd++;
                        }
                        pos.stepEnd++;
                    }
                    if (pos.stepEnd >= pos.path.Length) {
                        throw new XMPException("No terminating quote for array selector", XMPError.BADXPATH);
                    }
                    pos.stepEnd++;
                    // Absorb the trailing quote.
                    // ! Touch up later, also changing '@' to '?'.
                    segment = new XMPPathSegment(null, XMPPath.FIELD_SELECTOR_STEP);
                }
            }
            if (pos.stepEnd >= pos.path.Length || pos.path[pos.stepEnd] != ']') {
                throw new XMPException("Missing ']' for array index", XMPError.BADXPATH);
            }
            pos.stepEnd++;
            segment.SetName(pos.path.JSubstring(pos.stepBegin, pos.stepEnd));
            return segment;
        }

        /// <summary>
        /// Parses the root node of an XMP Path, checks if namespace and prefix fit together
        /// and resolve the property to the base property if it is an alias.
        /// </summary>
        /// <param name="schemaNS">the root namespace</param>
        /// <param name="pos">the parsing position helper</param>
        /// <param name="expandedXPath">the path to contribute to</param>
        private static void ParseRootNode(String schemaNS, PathPosition pos, XMPPath expandedXPath) {
            while (pos.stepEnd < pos.path.Length && "/[*".IndexOf(pos.path[pos.stepEnd]) < 0) {
                pos.stepEnd++;
            }
            if (pos.stepEnd == pos.stepBegin) {
                throw new XMPException("Empty initial XMPPath step", XMPError.BADXPATH);
            }
            String rootProp = VerifyXPathRoot(schemaNS, pos.path.JSubstring(pos.stepBegin, pos.stepEnd));
            XMPAliasInfo aliasInfo = XMPMetaFactory.GetSchemaRegistry().FindAlias(rootProp);
            if (aliasInfo == null) {
                // add schema xpath step
                expandedXPath.Add(new XMPPathSegment(schemaNS, XMPPath.SCHEMA_NODE));
                XMPPathSegment rootStep = new XMPPathSegment(rootProp, XMPPath.STRUCT_FIELD_STEP);
                expandedXPath.Add(rootStep);
            }
            else {
                // add schema xpath step and base step of alias
                expandedXPath.Add(new XMPPathSegment(aliasInfo.GetNamespace(), XMPPath.SCHEMA_NODE));
                XMPPathSegment rootStep = new XMPPathSegment(VerifyXPathRoot(aliasInfo.GetNamespace(), aliasInfo.GetPropName
                    ()), XMPPath.STRUCT_FIELD_STEP);
                rootStep.SetAlias(true);
                rootStep.SetAliasForm(aliasInfo.GetAliasForm().GetOptions());
                expandedXPath.Add(rootStep);
                if (aliasInfo.GetAliasForm().IsArrayAltText()) {
                    XMPPathSegment qualSelectorStep = new XMPPathSegment("[?xml:lang='x-default']", XMPPath.QUAL_SELECTOR_STEP
                        );
                    qualSelectorStep.SetAlias(true);
                    qualSelectorStep.SetAliasForm(aliasInfo.GetAliasForm().GetOptions());
                    expandedXPath.Add(qualSelectorStep);
                }
                else {
                    if (aliasInfo.GetAliasForm().IsArray()) {
                        XMPPathSegment indexStep = new XMPPathSegment("[1]", XMPPath.ARRAY_INDEX_STEP);
                        indexStep.SetAlias(true);
                        indexStep.SetAliasForm(aliasInfo.GetAliasForm().GetOptions());
                        expandedXPath.Add(indexStep);
                    }
                }
            }
        }

        /// <summary>
        /// Verifies whether the qualifier name is not XML conformant or the
        /// namespace prefix has not been registered.
        /// </summary>
        /// <param name="qualName">a qualifier name</param>
        private static void VerifyQualName(String qualName) {
            int colonPos = qualName.IndexOf(':');
            if (colonPos > 0) {
                String prefix = qualName.JSubstring(0, colonPos);
                if (Utils.IsXMLNameNS(prefix)) {
                    String regURI = XMPMetaFactory.GetSchemaRegistry().GetNamespaceURI(prefix);
                    if (regURI != null) {
                        return;
                    }
                    throw new XMPException("Unknown namespace prefix for qualified name", XMPError.BADXPATH);
                }
            }
            throw new XMPException("Ill-formed qualified name", XMPError.BADXPATH);
        }

        /// <summary>Verify if an XML name is conformant.</summary>
        /// <param name="name">an XML name</param>
        private static void VerifySimpleXMLName(String name) {
            if (!Utils.IsXMLName(name)) {
                throw new XMPException("Bad XML name", XMPError.BADXPATH);
            }
        }

        /// <summary>Set up the first 2 components of the expanded XMPPath.</summary>
        /// <remarks>
        /// Set up the first 2 components of the expanded XMPPath. Normalizes the various cases of using
        /// the full schema URI and/or a qualified root property name. Returns true for normal
        /// processing. If allowUnknownSchemaNS is true and the schema namespace is not registered, false
        /// is returned. If allowUnknownSchemaNS is false and the schema namespace is not registered, an
        /// exception is thrown
        /// <para />
        /// (Should someday check the full syntax:)
        /// </remarks>
        /// <param name="schemaNS">schema namespace</param>
        /// <param name="rootProp">the root xpath segment</param>
        /// <returns>Returns root QName.</returns>
        private static String VerifyXPathRoot(String schemaNS, String rootProp) {
            // Do some basic checks on the URI and name. Try to lookup the URI. See if the name is
            // qualified.
            if (schemaNS == null || schemaNS.Length == 0) {
                throw new XMPException("Schema namespace URI is required", XMPError.BADSCHEMA);
            }
            if ((rootProp[0] == '?') || (rootProp[0] == '@')) {
                throw new XMPException("Top level name must not be a qualifier", XMPError.BADXPATH);
            }
            if (rootProp.IndexOf('/') >= 0 || rootProp.IndexOf('[') >= 0) {
                throw new XMPException("Top level name must be simple", XMPError.BADXPATH);
            }
            String prefix = XMPMetaFactory.GetSchemaRegistry().GetNamespacePrefix(schemaNS);
            if (prefix == null) {
                throw new XMPException("Unregistered schema namespace URI", XMPError.BADSCHEMA);
            }
            // Verify the various URI and prefix combinations. Initialize the
            // expanded XMPPath.
            int colonPos = rootProp.IndexOf(':');
            if (colonPos < 0) {
                // The propName is unqualified, use the schemaURI and associated
                // prefix.
                VerifySimpleXMLName(rootProp);
                // Verify the part before any colon
                return prefix + rootProp;
            }
            else {
                // The propName is qualified. Make sure the prefix is legit. Use the associated URI and
                // qualified name.
                // Verify the part before any colon
                VerifySimpleXMLName(rootProp.JSubstring(0, colonPos));
                VerifySimpleXMLName(rootProp.Substring(colonPos));
                prefix = rootProp.JSubstring(0, colonPos + 1);
                String regPrefix = XMPMetaFactory.GetSchemaRegistry().GetNamespacePrefix(schemaNS);
                if (regPrefix == null) {
                    throw new XMPException("Unknown schema namespace prefix", XMPError.BADSCHEMA);
                }
                if (!prefix.Equals(regPrefix)) {
                    throw new XMPException("Schema namespace URI and prefix mismatch", XMPError.BADSCHEMA);
                }
                return rootProp;
            }
        }
    }

//\cond DO_NOT_DOCUMENT
    /// <summary>This objects contains all needed char positions to parse.</summary>
    internal class PathPosition {
        /// <summary>the complete path</summary>
        public String path = null;

//\cond DO_NOT_DOCUMENT
        /// <summary>the start of a segment name</summary>
        internal int nameStart = 0;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>the end of a segment name</summary>
        internal int nameEnd = 0;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>the begin of a step</summary>
        internal int stepBegin = 0;
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>the end of a step</summary>
        internal int stepEnd = 0;
//\endcond
    }
//\endcond
}
