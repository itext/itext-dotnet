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
                CMapCidUni cid2Uni = FontCache.GetCid2UniCmap(uniMap);
                avgWidth = 0;
                foreach (int cid in cid2Uni.GetCids()) {
                    int uni = cid2Uni.Lookup(cid);
                    int width = metrics.ContainsKey(cid) ? metrics.Get(cid) : DEFAULT_WIDTH;
                    Glyph glyph = new Glyph(cid, width, uni);
                    avgWidth += glyph.GetWidth();
                    codeToGlyph.Put(cid, glyph);
                    unicodeToGlyph.Put(uni, glyph);
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
