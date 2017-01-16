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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font.Cmap;
using iText.IO.Util;

namespace iText.IO.Font {
    public class FontCache {
        /// <summary>The path to the font resources.</summary>
        [Obsolete]
        public const String CMAP_RESOURCE_PATH = FontConstants.RESOURCE_PATH + "cmap/";

        private static readonly IDictionary<String, IDictionary<String, Object>> allCidFonts = new Dictionary<String
            , IDictionary<String, Object>>();

        private static readonly IDictionary<String, ICollection<String>> registryNames = new Dictionary<String, ICollection
            <String>>();

        private const String CJK_REGISTRY_FILENAME = "cjk_registry.properties";

        private const String FONTS_PROP = "fonts";

        private const String REGISTRY_PROP = "Registry";

        private const String W_PROP = "W";

        private const String W2_PROP = "W2";

        private static IDictionary<FontCacheKey, FontProgram> fontCache = new ConcurrentDictionary<FontCacheKey, FontProgram
            >();

        static FontCache() {
            try {
                LoadRegistry();
                foreach (String font in registryNames.Get(FONTS_PROP)) {
                    allCidFonts[font] = ReadFontProperties(font);
                }
            }
            catch (Exception) {
            }
        }

        /// <summary>
        /// Checks if the font with the given name and encoding is one
        /// of the predefined CID fonts.
        /// </summary>
        /// <param name="fontName">the font name.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if it is CJKFont.
        /// </returns>
        protected internal static bool IsPredefinedCidFont(String fontName) {
            if (!registryNames.ContainsKey(FONTS_PROP)) {
                return false;
            }
            else {
                if (!registryNames.Get(FONTS_PROP).Contains(fontName)) {
                    return false;
                }
            }
            return true;
        }

        public static String GetCompatibleCidFont(String cmap) {
            foreach (KeyValuePair<String, ICollection<String>> e in registryNames) {
                if (e.Value.Contains(cmap)) {
                    String registry = e.Key;
                    foreach (KeyValuePair<String, IDictionary<String, Object>> e1 in allCidFonts) {
                        if (registry.Equals(e1.Value.Get(REGISTRY_PROP))) {
                            return e1.Key;
                        }
                    }
                }
            }
            return null;
        }

        public static ICollection<String> GetCompatibleCmaps(String fontName) {
            String registry = (String)FontCache.GetAllFonts().Get(fontName).Get(REGISTRY_PROP);
            return registryNames.Get(registry);
        }

        public static IDictionary<String, IDictionary<String, Object>> GetAllPredefinedCidFonts() {
            return allCidFonts;
        }

        [System.ObsoleteAttribute(@"Use GetAllPredefinedCidFonts() instead.")]
        public static IDictionary<String, IDictionary<String, Object>> GetAllFonts() {
            return allCidFonts;
        }

        public static IDictionary<String, ICollection<String>> GetRegistryNames() {
            return registryNames;
        }

        public static CMapCidUni GetCid2UniCmap(String uniMap) {
            CMapCidUni cidUni = new CMapCidUni();
            return ParseCmap(uniMap, cidUni);
        }

        public static CMapUniCid GetUni2CidCmap(String uniMap) {
            CMapUniCid uniCid = new CMapUniCid();
            return ParseCmap(uniMap, uniCid);
        }

        public static CMapByteCid GetByte2CidCmap(String cmap) {
            CMapByteCid uniCid = new CMapByteCid();
            return ParseCmap(cmap, uniCid);
        }

        public static CMapCidByte GetCid2Byte(String cmap) {
            CMapCidByte cidByte = new CMapCidByte();
            return ParseCmap(cmap, cidByte);
        }

        public static FontProgram GetFont(String fontName) {
            return fontCache.Get(FontCacheKey.Create(fontName));
        }

        internal static FontProgram GetFont(FontCacheKey key) {
            return fontCache.Get(key);
        }

        public static FontProgram SaveFont(FontProgram font, String fontName) {
            return SaveFont(font, FontCacheKey.Create(fontName));
        }

        internal static FontProgram SaveFont(FontProgram font, FontCacheKey key) {
            FontProgram fontFound = fontCache.Get(key);
            if (fontFound != null) {
                return fontFound;
            }
            fontCache[key] = font;
            return font;
        }

        /// <exception cref="System.IO.IOException"/>
        private static void LoadRegistry() {
            Stream resource = ResourceUtil.GetResourceStream(FontConstants.CMAP_RESOURCE_PATH + CJK_REGISTRY_FILENAME);
            try {
                Properties p = new Properties();
                p.Load(resource);
                foreach (KeyValuePair<Object, Object> entry in p) {
                    String value = (String)entry.Value;
                    String[] splitValue = iText.IO.Util.StringUtil.Split(value, " ");
                    ICollection<String> set = new HashSet<String>();
                    foreach (String s in splitValue) {
                        if (s.Length != 0) {
                            set.Add(s);
                        }
                    }
                    registryNames[(String)entry.Key] = set;
                }
            }
            finally {
                if (resource != null) {
                    resource.Dispose();
                }
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private static IDictionary<String, Object> ReadFontProperties(String name) {
            Stream resource = ResourceUtil.GetResourceStream(FontConstants.CMAP_RESOURCE_PATH + name + ".properties");
            try {
                Properties p = new Properties();
                p.Load(resource);
                IDictionary<String, Object> fontProperties = new Dictionary<String, Object>();
                foreach (KeyValuePair<Object, Object> entry in p) {
                    fontProperties[(String)entry.Key] = entry.Value;
                }
                fontProperties[W_PROP] = CreateMetric((String)fontProperties.Get(W_PROP));
                fontProperties[W2_PROP] = CreateMetric((String)fontProperties.Get(W2_PROP));
                return fontProperties;
            }
            finally {
                if (resource != null) {
                    resource.Dispose();
                }
            }
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

        private static T ParseCmap<T>(String name, T cmap)
            where T : AbstractCMap {
            try {
                CMapParser.ParseCid(name, cmap, new CMapLocationResource());
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.IOException(iText.IO.IOException.IoException, e);
            }
            return cmap;
        }
    }
}
