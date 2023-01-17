/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace iText.Commons.Utils {
    /// <summary>
    /// Summary description for Properties.
    /// </summary>
    public class Properties {
        private Dictionary<Object, Object> _col;
        private const String whiteSpaceChars = " \t\r\n\f";
        private const String keyValueSeparators = "=: \t\r\n\f";
        private const String strictKeyValueSeparators = "=:";

        public Properties() {
            _col = new Dictionary<Object, Object>();
        }

        public virtual String Remove(String key) {
            Object retval;
            _col.TryGetValue(key, out retval);
            _col.Remove(key);
            return (String) retval;
        }

        public virtual Dictionary<Object, Object>.Enumerator GetEnumerator() {
            return _col.GetEnumerator();
        }

        public virtual bool ContainsKey(String key) {
            return _col.ContainsKey(key);
        }

        public virtual void Add(String key, String value) {
            _col[key] = value;
        }

        public virtual void AddAll(Properties col) {
            foreach (String itm in col.Keys) {
                _col[itm] = col.GetProperty(itm);
            }
        }

        public virtual int Count {
            get { return _col.Count; }
        }

        public virtual String GetProperty(String key) {
            Object retval;
            _col.TryGetValue(key, out retval);
            return (String) retval;
        }

        public virtual void SetProperty(String key, String value) {
            _col[key] = value;
        }

        public virtual Dictionary<Object, Object>.KeyCollection Keys {
            get { return _col.Keys; }
        }

        public virtual void Clear() {
            _col.Clear();
        }

        public virtual void Load(Stream inStream) {
            if (inStream == null) return;
            StreamReader inp = new StreamReader(inStream, EncodingUtil.GetEncoding(1252));
            while (true) {
                // Get next line
                String line = inp.ReadLine();
                if (line == null)
                    return;

                if (line.Length > 0) {
                    // Find start of key
                    int len = line.Length;
                    int keyStart;
                    for (keyStart = 0; keyStart < len; keyStart++)
                        if (whiteSpaceChars.IndexOf(line[keyStart]) == -1)
                            break;

                    // Blank lines are ignored
                    if (keyStart == len)
                        continue;

                    // Continue lines that end in slashes if they are not comments
                    char firstChar = line[keyStart];
                    if ((firstChar != '#') && (firstChar != '!')) {
                        while (ContinueLine(line)) {
                            String nextLine = inp.ReadLine();
                            if (nextLine == null)
                                nextLine = "";
                            String loppedLine = line.Substring(0, len - 1);
                            // Advance beyond whitespace on new line
                            int startIndex;
                            for (startIndex = 0; startIndex < nextLine.Length; startIndex++)
                                if (whiteSpaceChars.IndexOf(nextLine[startIndex]) == -1)
                                    break;
                            nextLine = nextLine.Substring(startIndex, nextLine.Length - startIndex);
                            line = loppedLine + nextLine;
                            len = line.Length;
                        }

                        // Find separation between key and value
                        int separatorIndex;
                        for (separatorIndex = keyStart; separatorIndex < len; separatorIndex++) {
                            char currentChar = line[separatorIndex];
                            if (currentChar == '\\')
                                separatorIndex++;
                            else if (keyValueSeparators.IndexOf(currentChar) != -1)
                                break;
                        }

                        // Skip over whitespace after key if any
                        int valueIndex;
                        for (valueIndex = separatorIndex; valueIndex < len; valueIndex++)
                            if (whiteSpaceChars.IndexOf(line[valueIndex]) == -1)
                                break;

                        // Skip over one non whitespace key value separators if any
                        if (valueIndex < len)
                            if (strictKeyValueSeparators.IndexOf(line[valueIndex]) != -1)
                                valueIndex++;

                        // Skip over white space after other separators if any
                        while (valueIndex < len) {
                            if (whiteSpaceChars.IndexOf(line[valueIndex]) == -1)
                                break;
                            valueIndex++;
                        }
                        String key = line.Substring(keyStart, separatorIndex - keyStart);
                        String value = (separatorIndex < len) ? line.Substring(valueIndex, len - valueIndex) : "";

                        // Convert then store key and value
                        key = LoadConvert(key);
                        value = LoadConvert(value);
                        Add(key, value);
                    }
                }
            }
        }

        /*
        * Converts encoded &#92;uxxxx to unicode chars
        * and changes special saved chars to their original forms
        */

        private String LoadConvert(String theString) {
            char aChar;
            int len = theString.Length;
            StringBuilder outBuffer = new StringBuilder(len);

            for (int x = 0; x < len;) {
                aChar = theString[x++];
                if (aChar == '\\') {
                    aChar = theString[x++];
                    if (aChar == 'u') {
                        // Read the xxxx
                        int value = 0;
                        for (int i = 0; i < 4; i++) {
                            aChar = theString[x++];
                            switch (aChar) {
                                case '0':
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                case '6':
                                case '7':
                                case '8':
                                case '9':
                                    value = (value << 4) + aChar - '0';
                                    break;
                                case 'a':
                                case 'b':
                                case 'c':
                                case 'd':
                                case 'e':
                                case 'f':
                                    value = (value << 4) + 10 + aChar - 'a';
                                    break;
                                case 'A':
                                case 'B':
                                case 'C':
                                case 'D':
                                case 'E':
                                case 'F':
                                    value = (value << 4) + 10 + aChar - 'A';
                                    break;
                                default:
                                    throw new ArgumentException(
                                        "Malformed \\uxxxx encoding.");
                            }
                        }
                        outBuffer.Append((char) value);
                    } else {
                        if (aChar == 't') aChar = '\t';
                        else if (aChar == 'r') aChar = '\r';
                        else if (aChar == 'n') aChar = '\n';
                        else if (aChar == 'f') aChar = '\f';
                        outBuffer.Append(aChar);
                    }
                } else
                    outBuffer.Append(aChar);
            }
            return outBuffer.ToString();
        }

        private bool ContinueLine(String line) {
            int slashCount = 0;
            int index = line.Length - 1;
            while ((index >= 0) && (line[index--] == '\\'))
                slashCount++;
            return (slashCount%2 == 1);
        }
    }
}
