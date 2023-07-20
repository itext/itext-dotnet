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
using iText.Commons.Utils;
using iText.IO.Font.Cmap;
using iText.IO.Font.Otf;
using iText.IO.Util;

namespace iText.IO.Font {
    public class CidFont : FontProgram {
        private String fontName;

        private int pdfFontFlags;

        private ICollection<String> compatibleCmaps;

        internal CidFont(String fontName, ICollection<String> cmaps) {
            this.fontName = fontName;
            compatibleCmaps = cmaps;
            fontNames = new FontNames();
            InitializeCidFontNameAndStyle(fontName);
            IDictionary<String, Object> fontDesc = CidFontProperties.GetAllFonts().Get(fontNames.GetFontName());
            if (fontDesc == null) {
                throw new iText.IO.Exceptions.IOException("There is no such predefined font: {0}").SetMessageParams(fontName
                    );
            }
            InitializeCidFontProperties(fontDesc);
        }

        internal CidFont(String fontName, ICollection<String> cmaps, IDictionary<String, Object> fontDescription) {
            InitializeCidFontNameAndStyle(fontName);
            InitializeCidFontProperties(fontDescription);
            compatibleCmaps = cmaps;
        }

        public virtual bool CompatibleWith(String cmap) {
            if (cmap.Equals(PdfEncodings.IDENTITY_H) || cmap.Equals(PdfEncodings.IDENTITY_V)) {
                return true;
            }
            else {
                return compatibleCmaps != null && compatibleCmaps.Contains(cmap);
            }
        }

        public override int GetKerning(Glyph glyph1, Glyph glyph2) {
            return 0;
        }

        public override int GetPdfFontFlags() {
            return pdfFontFlags;
        }

        public override bool IsFontSpecific() {
            return false;
        }

        public override bool IsBuiltWith(String fontName) {
            return Object.Equals(this.fontName, fontName);
        }

        private void InitializeCidFontNameAndStyle(String fontName) {
            String nameBase = TrimFontStyle(fontName);
            if (nameBase.Length < fontName.Length) {
                fontNames.SetFontName(fontName);
                fontNames.SetStyle(fontName.Substring(nameBase.Length));
            }
            else {
                fontNames.SetFontName(fontName);
            }
            fontNames.SetFullName(new String[][] { new String[] { "", "", "", fontNames.GetFontName() } });
        }

        private void InitializeCidFontProperties(IDictionary<String, Object> fontDesc) {
            fontIdentification.SetPanose((String)fontDesc.Get("Panose"));
            fontMetrics.SetItalicAngle(Convert.ToInt32((String)fontDesc.Get("ItalicAngle"), System.Globalization.CultureInfo.InvariantCulture
                ));
            fontMetrics.SetCapHeight(Convert.ToInt32((String)fontDesc.Get("CapHeight"), System.Globalization.CultureInfo.InvariantCulture
                ));
            fontMetrics.SetTypoAscender(Convert.ToInt32((String)fontDesc.Get("Ascent"), System.Globalization.CultureInfo.InvariantCulture
                ));
            fontMetrics.SetTypoDescender(Convert.ToInt32((String)fontDesc.Get("Descent"), System.Globalization.CultureInfo.InvariantCulture
                ));
            fontMetrics.SetStemV(Convert.ToInt32((String)fontDesc.Get("StemV"), System.Globalization.CultureInfo.InvariantCulture
                ));
            pdfFontFlags = Convert.ToInt32((String)fontDesc.Get("Flags"), System.Globalization.CultureInfo.InvariantCulture
                );
            String fontBBox = (String)fontDesc.Get("FontBBox");
            StringTokenizer tk = new StringTokenizer(fontBBox, " []\r\n\t\f");
            int llx = Convert.ToInt32(tk.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
            int lly = Convert.ToInt32(tk.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
            int urx = Convert.ToInt32(tk.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
            int ury = Convert.ToInt32(tk.NextToken(), System.Globalization.CultureInfo.InvariantCulture);
            fontMetrics.UpdateBbox(llx, lly, urx, ury);
            registry = (String)fontDesc.Get("Registry");
            String uniMap = GetCompatibleUniMap(registry);
            if (uniMap != null) {
                IntHashtable metrics = (IntHashtable)fontDesc.Get("W");
                CMapUniCid uni2cid = FontCache.GetUni2CidCmap(uniMap);
                avgWidth = 0;
                foreach (int cp in uni2cid.GetCodePoints()) {
                    int cid = uni2cid.Lookup(cp);
                    int width = metrics.ContainsKey(cid) ? metrics.Get(cid) : DEFAULT_WIDTH;
                    Glyph glyph = new Glyph(cid, width, cp);
                    avgWidth += glyph.GetWidth();
                    codeToGlyph.Put(cid, glyph);
                    unicodeToGlyph.Put(cp, glyph);
                }
                FixSpaceIssue();
                if (codeToGlyph.Count != 0) {
                    avgWidth /= codeToGlyph.Count;
                }
            }
        }

        private static String GetCompatibleUniMap(String registry) {
            String uniMap = "";
            foreach (String name in CidFontProperties.GetRegistryNames().Get(registry + "_Uni")) {
                uniMap = name;
                if (name.EndsWith("H")) {
                    break;
                }
            }
            return uniMap;
        }
    }
}
