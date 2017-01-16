/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;

namespace iText.IO.Font {
    public class CidFontProperties {
        /// <summary>The path to the font resources.</summary>
        [Obsolete]
        public const String RESOURCE_PATH_CMAP = FontConstants.RESOURCE_PATH + "cmap/";

        private static readonly IDictionary<String, IDictionary<String, Object>> allFonts = new Dictionary<String, 
            IDictionary<String, Object>>();

        private static readonly IDictionary<String, ICollection<String>> registryNames = new Dictionary<String, ICollection
            <String>>();

        static CidFontProperties() {
            try {
                LoadRegistry();
                foreach (String font in registryNames.Get("fonts")) {
                    allFonts[font] = ReadFontProperties(font);
                }
            }
            catch (Exception) {
            }
        }

        /// <summary>Checks if its a valid CJKFont font.</summary>
        /// <param name="fontName">the font name.</param>
        /// <param name="enc">the encoding.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if it is CJKFont.
        /// </returns>
        public static bool IsCidFont(String fontName, String enc) {
            if (!registryNames.ContainsKey("fonts")) {
                return false;
            }
            if (!registryNames.Get("fonts").Contains(fontName)) {
                return false;
            }
            if (enc.Equals(PdfEncodings.IDENTITY_H) || enc.Equals(PdfEncodings.IDENTITY_V)) {
                return true;
            }
            String registry = (String)allFonts.Get(fontName).Get("Registry");
            ICollection<String> encodings = registryNames.Get(registry);
            return encodings != null && encodings.Contains(enc);
        }

        public static String GetCompatibleFont(String enc) {
            foreach (KeyValuePair<String, ICollection<String>> e in registryNames) {
                if (e.Value.Contains(enc)) {
                    String registry = e.Key;
                    foreach (KeyValuePair<String, IDictionary<String, Object>> e1 in allFonts) {
                        if (registry.Equals(e1.Value.Get("Registry"))) {
                            return e1.Key;
                        }
                    }
                }
            }
            return null;
        }

        public static IDictionary<String, IDictionary<String, Object>> GetAllFonts() {
            return allFonts;
        }

        public static IDictionary<String, ICollection<String>> GetRegistryNames() {
            return registryNames;
        }

        /// <exception cref="System.IO.IOException"/>
        private static void LoadRegistry() {
            Stream resource = ResourceUtil.GetResourceStream(FontConstants.CMAP_RESOURCE_PATH + "cjk_registry.properties"
                );
            Properties p = new Properties();
            p.Load(resource);
            resource.Dispose();
            foreach (Object key in p.Keys) {
                String value = p.GetProperty((String)key);
                String[] sp = iText.IO.Util.StringUtil.Split(value, " ");
                ICollection<String> hs = new HashSet<String>();
                foreach (String s in sp) {
                    if (s.Length > 0) {
                        hs.Add(s);
                    }
                }
                registryNames[(String)key] = hs;
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static IDictionary<String, Object> ReadFontProperties(String name) {
            name += ".properties";
            Stream resource = ResourceUtil.GetResourceStream(FontConstants.CMAP_RESOURCE_PATH + name);
            Properties p = new Properties();
            p.Load(resource);
            resource.Dispose();
            IntHashtable W = CreateMetric(p.GetProperty("W"));
            p.Remove("W");
            IntHashtable W2 = CreateMetric(p.GetProperty("W2"));
            p.Remove("W2");
            IDictionary<String, Object> map = new Dictionary<String, Object>();
            foreach (Object obj in p.Keys) {
                map[(String)obj] = p.GetProperty((String)obj);
            }
            map["W"] = W;
            map["W2"] = W2;
            return map;
        }

        private static IntHashtable CreateMetric(String s) {
            IntHashtable h = new IntHashtable();
            StringTokenizer tk = new StringTokenizer(s);
            while (tk.HasMoreTokens()) {
                int n1 = System.Convert.ToInt32(tk.NextToken());
                h.Put(n1, System.Convert.ToInt32(tk.NextToken()));
            }
            return h;
        }
    }
}
