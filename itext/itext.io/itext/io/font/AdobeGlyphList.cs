/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.IO.Util;

namespace iText.IO.Font {
    public class AdobeGlyphList {
        private static IDictionary<int, String> unicode2names = new Dictionary<int, String>();

        private static IDictionary<String, int?> names2unicode = new Dictionary<String, int?>();

        static AdobeGlyphList() {
            Stream resource = null;
            try {
                resource = ResourceUtil.GetResourceStream(FontResources.ADOBE_GLYPH_LIST);
                if (resource == null) {
                    throw new Exception(FontResources.ADOBE_GLYPH_LIST + " not found as resource.");
                }
                byte[] buf = new byte[1024];
                MemoryStream stream = new MemoryStream();
                while (true) {
                    int size = resource.Read(buf);
                    if (size < 0) {
                        break;
                    }
                    stream.Write(buf, 0, size);
                }
                resource.Dispose();
                resource = null;
                String s = PdfEncodings.ConvertToString(stream.ToArray(), null);
                StringTokenizer tk = new StringTokenizer(s, "\r\n");
                while (tk.HasMoreTokens()) {
                    String line = tk.NextToken();
                    if (line.StartsWith("#")) {
                        continue;
                    }
                    StringTokenizer t2 = new StringTokenizer(line, " ;\r\n\t\f");
                    if (!t2.HasMoreTokens()) {
                        continue;
                    }
                    String name = t2.NextToken();
                    if (!t2.HasMoreTokens()) {
                        continue;
                    }
                    String hex = t2.NextToken();
                    // AdobeGlyphList could contains symbols with marks, e.g.:
                    // resh;05E8
                    // reshhatafpatah;05E8 05B2
                    // So in this case we will just skip this nam
                    if (t2.HasMoreTokens()) {
                        continue;
                    }
                    int num = Convert.ToInt32(hex, 16);
                    unicode2names.Put(num, name);
                    names2unicode.Put(name, num);
                }
            }
            catch (Exception e) {
                System.Console.Error.WriteLine("AdobeGlyphList.txt loading error: " + e.Message);
            }
            finally {
                if (resource != null) {
                    try {
                        resource.Dispose();
                    }
                    catch (Exception) {
                    }
                }
            }
        }

        // empty on purpose
        public static int NameToUnicode(String name) {
            int v = -1;
            if (names2unicode.ContainsKey(name)) {
                v = (int)names2unicode.Get(name);
            }
            if (v == -1 && name.Length == 7 && name.ToLowerInvariant().StartsWith("uni")) {
                try {
                    return Convert.ToInt32(name.Substring(3), 16);
                }
                catch (Exception) {
                }
            }
            return v;
        }

        public static String UnicodeToName(int num) {
            return unicode2names.Get(num);
        }

        public static int GetNameToUnicodeLength() {
            return names2unicode.Count;
        }

        public static int GetUnicodeToNameLength() {
            return unicode2names.Count;
        }
    }
}
