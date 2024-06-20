/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using iText.IO.Util;
using iText.Kernel.Utils;

namespace iText.Layout.Hyphenation {
    /// <summary>
    /// A SAX document handler to read and parse hyphenation patterns
    /// from a XML file.
    /// <p>
    /// This work was authored by Carlos Villegas (cav@uniscope.co.jp).
    /// </summary>
    public class PatternParser {
        private int currElement;

        private IPatternConsumer consumer;

        private StringBuilder token;

        private ArrayList exception;

        private char hyphenChar;

        private bool hasClasses;

        //\cond DO_NOT_DOCUMENT 
        internal const int ELEM_CLASSES = 1;

        internal const int ELEM_EXCEPTIONS = 2;

        internal const int ELEM_PATTERNS = 3;

        internal const int ELEM_HYPHEN = 4;
        //\endcond
        /// <summary>Construct a pattern parser.</summary>
        private PatternParser() {
            token = new StringBuilder();
            // default
            hyphenChar = '-';
        }

        /// <summary>Construct a pattern parser.</summary>
        /// <param name="consumer">a pattern consumer</param>
        public PatternParser(IPatternConsumer consumer)
            : this() {
            this.consumer = consumer;
        }

        /// <summary>Parses a hyphenation pattern file.</summary>
        /// <param name="filename">the filename</param>
        public virtual void Parse(String filename) {
            Parse(new FileStream(filename, FileMode.Open), filename);
        }

        /// <summary>Parses a hyphenation pattern file.</summary>
        /// <param name="stream">the InputStream for the file</param>
        /// <param name="name">unique key representing country-language combination</param>
        public virtual void Parse(Stream stream, String name) {
            XmlReader reader = XmlProcessorCreator.CreateSafeXmlReader(stream);
            while (reader.Read()) {
                switch (reader.NodeType) {
                    case XmlNodeType.Element: {
                        Hashtable attributes = new Hashtable();
                        string strURI = reader.NamespaceURI;
                        string strName = reader.Name;
                        if (reader.HasAttributes) {
                            for (int i = 0; i < reader.AttributeCount; i++) {
                                reader.MoveToAttribute(i);
                                attributes.Add(reader.Name, reader.Value);
                            }
                        }
                        StartElement(strURI, strName, strName, attributes);
                    }
                        break;
                    case XmlNodeType.EndElement: {
                        string strURI = reader.NamespaceURI;
                        string strName = reader.Name;
                        EndElement(strURI, strName, strName);
                    }
                        break;
                    case XmlNodeType.Text:
                        String text = reader.Value;
                        char[] chars = text.ToCharArray();
                        Characters(chars, 0, chars.Length);
                        break;
                    default:
                        break;
                }
            }
        }

        private String ReadToken(StringBuilder chars) {
            String word;
            bool space = false;
            int i;
            for (i = 0; i < chars.Length; i++) {
                if (TextUtil.IsWhiteSpace(chars[i])) {
                    space = true;
                } else {
                    break;
                }
            }
            if (space) {
                // chars.delete(0,i);
                for (int countr = i; countr < chars.Length; countr++) {
                    chars.SetCharAt(countr - i, chars[countr]);
                }
                chars.Length = chars.Length - i;
                if (token.Length > 0) {
                    word = token.ToString();
                    token.Length = 0;
                    return word;
                }
            }
            space = false;
            for (i = 0; i < chars.Length; i++) {
                if (TextUtil.IsWhiteSpace(chars[i])) {
                    space = true;
                    break;
                }
            }
            token.Append(chars.ToString().JSubstring(0, i));
            // chars.delete(0,i);
            for (int countr_1 = i; countr_1 < chars.Length; countr_1++) {
                chars.SetCharAt(countr_1 - i, chars[countr_1]);
            }
            chars.Length = chars.Length - i;
            if (space) {
                word = token.ToString();
                token.Length = 0;
                return word;
            }
            token.Append(chars);
            return null;
        }

        private static String GetPattern(String word) {
            StringBuilder pat = new StringBuilder();
            int len = word.Length;
            for (int i = 0; i < len; i++) {
                if (!char.IsDigit(word[i])) {
                    pat.Append(word[i]);
                }
            }
            return pat.ToString();
        }

        private ArrayList NormalizeException(ArrayList ex) {
            ArrayList res = new ArrayList();
            for (int i = 0; i < ex.Count; i++) {
                Object item = ex[i];
                if (item is String) {
                    String str = (String) item;
                    StringBuilder buf = new StringBuilder();
                    for (int j = 0; j < str.Length; j++) {
                        char c = str[j];
                        if (c != hyphenChar) {
                            buf.Append(c);
                        } else {
                            res.Add(buf.ToString());
                            buf.Length = 0;
                            char[] h = new char[1];
                            h[0] = hyphenChar;
                            // we use here hyphenChar which is not necessarily
                            // the one to be printed
                            res.Add(new Hyphen(new String(h), null, null));
                        }
                    }
                    if (buf.Length > 0) {
                        res.Add(buf.ToString());
                    }
                } else {
                    res.Add(item);
                }
            }
            return res;
        }

        private String GetExceptionWord(ArrayList ex) {
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < ex.Count; i++) {
                Object item = ex[i];
                if (item is String) {
                    res.Append((String) item);
                } else {
                    if (((Hyphen) item).noBreak != null) {
                        res.Append(((Hyphen) item).noBreak);
                    }
                }
            }
            return res.ToString();
        }

        private static String GetInterletterValues(String pat) {
            StringBuilder il = new StringBuilder();
            // add dummy letter to serve as sentinel
            String word = pat + "a";
            int len = word.Length;
            for (int i = 0; i < len; i++) {
                char c = word[i];
                if (char.IsDigit(c)) {
                    il.Append(c);
                    i++;
                } else {
                    il.Append('0');
                }
            }
            return il.ToString();
        }
    
        //\cond DO_NOT_DOCUMENT 
        protected internal virtual void GetExternalClasses() {
            Parse(ResourceUtil.GetResourceStream(HyphenationConstants.HYPHENATION_DEFAULT_RESOURCE + "External.classes.xml"), "classes.xml");
        }
       //\endcond 

        //
        // ContentHandler methods
        //
        public virtual void StartElement(String uri, String local, String raw, Hashtable attrs) {
            if (local.Equals("hyphen-char")) {
                String h = (String) attrs["value"];
                if (h != null && h.Length == 1) {
                    hyphenChar = h[0];
                }
            } else {
                if (local.Equals("classes")) {
                    currElement = ELEM_CLASSES;
                } else {
                    if (local.Equals("patterns")) {
                        if (!hasClasses) {
                            GetExternalClasses();
                        }
                        currElement = ELEM_PATTERNS;
                    } else {
                        if (local.Equals("exceptions")) {
                            if (!hasClasses) {
                                GetExternalClasses();
                            }
                            currElement = ELEM_EXCEPTIONS;
                            exception = new ArrayList();
                        } else {
                            if (local.Equals("hyphen")) {
                                if (token.Length > 0) {
                                    exception.Add(token.ToString());
                                }
                                exception.Add(new Hyphen((string) attrs["pre"], (string) attrs["no"],
                                    (string) attrs["post"]));
                                currElement = ELEM_HYPHEN;
                            }
                        }
                    }
                }
            }
            token.Length = 0;
        }

        public virtual void EndElement(String uri, String local, String raw) {
            if (token.Length > 0) {
                String word = token.ToString();
                switch (currElement) {
                    case ELEM_CLASSES: {
                        consumer.AddClass(word);
                        break;
                    }

                    case ELEM_EXCEPTIONS: {
                        exception.Add(word);
                        exception = NormalizeException(exception);
                        consumer.AddException(GetExceptionWord(exception), (ArrayList) exception.Clone());
                        break;
                    }

                    case ELEM_PATTERNS: {
                        consumer.AddPattern(GetPattern(word), GetInterletterValues(word));
                        break;
                    }

                    case ELEM_HYPHEN: {
                        // nothing to do
                        break;
                    }

                    default: {
                        break;
                    }
                }
                if (currElement != ELEM_HYPHEN) {
                    token.Length = 0;
                }
            }
            if (currElement == ELEM_CLASSES) {
                hasClasses = true;
            }
            if (currElement == ELEM_HYPHEN) {
                currElement = ELEM_EXCEPTIONS;
            } else {
                currElement = 0;
            }
        }

        public virtual void Characters(char[] ch, int start, int length) {
            StringBuilder chars = new StringBuilder(length);
            chars.Append(ch, start, length);
            String word = ReadToken(chars);
            while (word != null) {
                switch (currElement) {
                    case ELEM_CLASSES: {
                        // System.out.println("\"" + word + "\"");
                        consumer.AddClass(word);
                        break;
                    }

                    case ELEM_EXCEPTIONS: {
                        exception.Add(word);
                        exception = NormalizeException(exception);
                        consumer.AddException(GetExceptionWord(exception), (ArrayList) exception.Clone());
                        exception.Clear();
                        break;
                    }

                    case ELEM_PATTERNS: {
                        consumer.AddPattern(GetPattern(word), GetInterletterValues(word));
                        break;
                    }

                    default: {
                        break;
                    }
                }
                word = ReadToken(chars);
            }
        }
    }
}
