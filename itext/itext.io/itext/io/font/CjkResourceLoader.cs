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
using iText.IO.Exceptions;
using iText.IO.Font.Cmap;
using iText.IO.Util;

namespace iText.IO.Font {
    /// <summary>This class is responsible for loading and handling CJK fonts and CMaps from font-asian package.</summary>
    public sealed class CjkResourceLoader {
        private static readonly IDictionary<String, IDictionary<String, Object>> allCidFonts = new LinkedDictionary
            <String, IDictionary<String, Object>>();

        private static readonly IDictionary<String, ICollection<String>> registryNames = new Dictionary<String, ICollection
            <String>>();

        private const String CJK_REGISTRY_FILENAME = "cjk_registry.properties";

        private const String FONTS_PROP = "fonts";

        private const String REGISTRY_PROP = "Registry";

        private const String W_PROP = "W";

        private const String W2_PROP = "W2";

        private static CMapLocationResource cmapLocation;

        private CjkResourceLoader() {
        }

        static CjkResourceLoader() {
            iText.IO.Font.CjkResourceLoader.SetCmapLocation(new CMapLocationResource());
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
        public static bool IsPredefinedCidFont(String fontName) {
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

        /// <summary>Finds a CJK font family which is compatible to the given CMap.</summary>
        /// <param name="cmap">a name of the CMap for which compatible font is searched.</param>
        /// <returns>a CJK font name if there's known compatible font for the given cmap name, or null otherwise.</returns>
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

        /// <summary>
        /// Finds all CMap names that belong to the same registry to which a given
        /// font belongs.
        /// </summary>
        /// <param name="fontName">a name of the font for which CMap's are searched.</param>
        /// <returns>a set of CMap names corresponding to the given font.</returns>
        public static ICollection<String> GetCompatibleCmaps(String fontName) {
            IDictionary<String, Object> cidFonts = iText.IO.Font.CjkResourceLoader.GetAllPredefinedCidFonts().Get(fontName
                );
            if (cidFonts == null) {
                return null;
            }
            String registry = (String)cidFonts.Get(REGISTRY_PROP);
            return registryNames.Get(registry);
        }

        /// <summary>Get all loaded predefined CID fonts.</summary>
        /// <returns>predefined CID fonts.</returns>
        public static IDictionary<String, IDictionary<String, Object>> GetAllPredefinedCidFonts() {
            return allCidFonts;
        }

        /// <summary>Get all loaded CJK registry names mapped to a set of compatible cmaps.</summary>
        /// <returns>CJK registry names mapped to a set of compatible cmaps.</returns>
        public static IDictionary<String, ICollection<String>> GetRegistryNames() {
            return registryNames;
        }

        /// <summary>Parses CMap with a given name producing it in a form of cid to unicode mapping.</summary>
        /// <param name="uniMap">a CMap name. It is expected that CMap identified by this name defines unicode to cid mapping.
        ///     </param>
        /// <returns>
        /// an object for convenient mapping from cid to unicode. If no CMap was found for provided name
        /// an exception is thrown.
        /// </returns>
        public static CMapCidUni GetCid2UniCmap(String uniMap) {
            CMapCidUni cidUni = new CMapCidUni();
            return ParseCmap(uniMap, cidUni);
        }

        /// <summary>Parses CMap with a given name producing it in a form of unicode to cid mapping.</summary>
        /// <param name="uniMap">a CMap name. It is expected that CMap identified by this name defines unicode to cid mapping.
        ///     </param>
        /// <returns>
        /// an object for convenient mapping from unicode to cid. If no CMap was found for provided name
        /// an exception is thrown.
        /// </returns>
        public static CMapUniCid GetUni2CidCmap(String uniMap) {
            return ParseCmap(uniMap, new CMapUniCid());
        }

        /// <summary>Parses CMap with a given name producing it in a form of byte to cid mapping.</summary>
        /// <param name="cmap">a CMap name. It is expected that CMap identified by this name defines byte to cid mapping.
        ///     </param>
        /// <returns>
        /// an object for convenient mapping from byte to cid. If no CMap was found for provided name
        /// an exception is thrown.
        /// </returns>
        public static CMapByteCid GetByte2CidCmap(String cmap) {
            CMapByteCid uniCid = new CMapByteCid();
            return ParseCmap(cmap, uniCid);
        }

        /// <summary>Parses CMap with a given name producing it in a form of cid to code point mapping.</summary>
        /// <param name="cmap">a CMap name. It is expected that CMap identified by this name defines code point to cid mapping.
        ///     </param>
        /// <returns>
        /// an object for convenient mapping from cid to code point. If no CMap was found for provided name
        /// an exception is thrown.
        /// </returns>
        public static CMapCidToCodepoint GetCidToCodepointCmap(String cmap) {
            CMapCidToCodepoint cidByte = new CMapCidToCodepoint();
            return ParseCmap(cmap, cidByte);
        }

        /// <summary>Parses CMap with a given name producing it in a form of code point to cid mapping.</summary>
        /// <param name="uniMap">a CMap name. It is expected that CMap identified by this name defines code point to cid mapping.
        ///     </param>
        /// <returns>
        /// an object for convenient mapping from code point to cid. If no CMap was found for provided name
        /// an exception is thrown.
        /// </returns>
        public static CMapCodepointToCid GetCodepointToCidCmap(String uniMap) {
            return ParseCmap(uniMap, new CMapCodepointToCid());
        }

        internal static void SetCmapLocation(CMapLocationResource cmapLocation) {
            iText.IO.Font.CjkResourceLoader.cmapLocation = cmapLocation;
            try {
                LoadRegistry();
            }
            catch (Exception) {
            }
        }

        private static void LoadRegistry() {
            registryNames.Clear();
            allCidFonts.Clear();
            Stream resource = ResourceUtil.GetResourceStream(cmapLocation.GetLocationPath() + CJK_REGISTRY_FILENAME);
            try {
                Properties p = new Properties();
                p.Load(resource);
                foreach (KeyValuePair<Object, Object> entry in p) {
                    String value = (String)entry.Value;
                    String[] splitValue = iText.Commons.Utils.StringUtil.Split(value, " ");
                    ICollection<String> set = new HashSet<String>();
                    foreach (String s in splitValue) {
                        if (s.Length != 0) {
                            set.Add(s);
                        }
                    }
                    registryNames.Put((String)entry.Key, set);
                }
            }
            finally {
                if (resource != null) {
                    resource.Dispose();
                }
            }
            foreach (String font in registryNames.Get(FONTS_PROP)) {
                allCidFonts.Put(font, ReadFontProperties(font));
            }
        }

        private static IDictionary<String, Object> ReadFontProperties(String name) {
            Stream resource = ResourceUtil.GetResourceStream(cmapLocation.GetLocationPath() + name + ".properties");
            try {
                Properties p = new Properties();
                p.Load(resource);
                IDictionary<String, Object> fontProperties = new Dictionary<String, Object>();
                foreach (KeyValuePair<Object, Object> entry in p) {
                    fontProperties.Put((String)entry.Key, entry.Value);
                }
                fontProperties.Put(W_PROP, CreateMetric((String)fontProperties.Get(W_PROP)));
                fontProperties.Put(W2_PROP, CreateMetric((String)fontProperties.Get(W2_PROP)));
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
                int n1 = Convert.ToInt32(tk.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
                h.Put(n1, Convert.ToInt32(tk.NextToken(), System.Globalization.CultureInfo.InvariantCulture));
            }
            return h;
        }

        private static T ParseCmap<T>(String name, T cmap)
            where T : AbstractCMap {
            try {
                CMapParser.ParseCid(name, cmap, cmapLocation);
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.IO_EXCEPTION, e);
            }
            return cmap;
        }
    }
}
