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
using iTextSharp.IO.Font;
using iTextSharp.IO.Font.Cmap;
using iTextSharp.IO.Font.Otf;
using iTextSharp.IO.Log;
using iTextSharp.Kernel;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Font
{
    /// <summary>Note.</summary>
    /// <remarks>Note. For TrueType FontNames.getStyle() is the same to Subfamily(). So, we shouldn't add style to /BaseFont.
    ///     </remarks>
    public class PdfTrueTypeFont : PdfSimpleFont<TrueTypeFont>
    {
        internal PdfTrueTypeFont(TrueTypeFont ttf, String encoding, bool embedded)
            : base()
        {
            SetFontProgram(ttf);
            this.embedded = embedded;
            FontNames fontNames = ttf.GetFontNames();
            if (embedded && !fontNames.AllowEmbedding())
            {
                throw new PdfException("1.cannot.be.embedded.due.to.licensing.restrictions").SetMessageParams
                    (fontNames.GetFontName());
            }
            if ((encoding == null || encoding.Length == 0) && ttf.IsFontSpecific())
            {
                encoding = FontEncoding.FONT_SPECIFIC;
            }
            if (encoding != null && FontEncoding.FONT_SPECIFIC.ToLower(System.Globalization.CultureInfo.InvariantCulture
                ).Equals(encoding.ToLower(System.Globalization.CultureInfo.InvariantCulture)))
            {
                fontEncoding = FontEncoding.CreateFontSpecificEncoding();
            }
            else
            {
                fontEncoding = FontEncoding.CreateFontEncoding(encoding);
            }
        }

        internal PdfTrueTypeFont(PdfDictionary fontDictionary)
            : base(fontDictionary)
        {
            newFont = false;
            CheckFontDictionary(fontDictionary, PdfName.TrueType);
            CMapToUnicode toUni = FontUtil.ProcessToUnicode(fontDictionary.Get(PdfName.ToUnicode
                ));
            fontEncoding = DocFontEncoding.CreateDocFontEncoding(fontDictionary.Get(PdfName.Encoding
                ), toUni);
            fontProgram = DocTrueTypeFont.CreateFontProgram(fontDictionary, fontEncoding);
            embedded = ((IDocFontProgram)fontProgram).GetFontFile() != null;
            subset = false;
        }

        public override Glyph GetGlyph(int unicode)
        {
            if (fontEncoding.CanEncode(unicode))
            {
                Glyph glyph = ((TrueTypeFont)GetFontProgram()).GetGlyph(fontEncoding.GetUnicodeDifference
                    (unicode));
                //TODO TrueType what if font is specific?
                if (glyph == null && (glyph = notdefGlyphs.Get(unicode)) == null)
                {
                    Glyph notdef = ((TrueTypeFont)GetFontProgram()).GetGlyphByCode(0);
                    if (notdef != null)
                    {
                        glyph = new Glyph(((TrueTypeFont)GetFontProgram()).GetGlyphByCode(0), unicode);
                        notdefGlyphs[unicode] = glyph;
                    }
                }
                return glyph;
            }
            return null;
        }

        //TODO make subtype class member and simplify this method
        public override void Flush()
        {
            if (newFont)
            {
                PdfName subtype;
                String fontName;
                if (((TrueTypeFont)GetFontProgram()).IsCff())
                {
                    subtype = PdfName.Type1;
                    fontName = fontProgram.GetFontNames().GetFontName();
                }
                else
                {
                    subtype = PdfName.TrueType;
                    fontName = subset ? CreateSubsetPrefix() + fontProgram.GetFontNames().GetFontName
                        () : fontProgram.GetFontNames().GetFontName();
                }
                FlushFontData(fontName, subtype);
            }
            base.Flush();
        }

        protected internal virtual void AddRangeUni(ICollection<int> longTag)
        {
            if (!subset && (subsetRanges != null || ((TrueTypeFont)GetFontProgram()).GetDirectoryOffset
                () > 0))
            {
                int[] rg = subsetRanges == null && ((TrueTypeFont)GetFontProgram()).GetDirectoryOffset
                    () > 0 ? new int[] { 0, 0xffff } : CompactRanges(subsetRanges);
                IDictionary<int, int[]> usemap = ((TrueTypeFont)GetFontProgram()).GetActiveCmap();
                System.Diagnostics.Debug.Assert(usemap != null);
                foreach (KeyValuePair<int, int[]> e in usemap)
                {
                    int[] v = e.Value;
                    int gi = v[0];
                    if (longTag.Contains(gi))
                    {
                        continue;
                    }
                    int c = (int)e.Key;
                    bool skip = true;
                    for (int k = 0; k < rg.Length; k += 2)
                    {
                        if (c >= rg[k] && c <= rg[k + 1])
                        {
                            skip = false;
                            break;
                        }
                    }
                    if (!skip)
                    {
                        longTag.Add(gi);
                    }
                }
            }
        }

        protected internal override void AddFontStream(PdfDictionary fontDescriptor)
        {
            if (embedded)
            {
                PdfName fontFileName;
                PdfStream fontStream;
                if (fontProgram is IDocFontProgram)
                {
                    fontFileName = ((IDocFontProgram)fontProgram).GetFontFileName();
                    fontStream = ((IDocFontProgram)fontProgram).GetFontFile();
                }
                else
                {
                    if (((TrueTypeFont)GetFontProgram()).IsCff())
                    {
                        fontFileName = PdfName.FontFile3;
                        try
                        {
                            byte[] fontStreamBytes = ((TrueTypeFont)GetFontProgram()).GetFontStreamBytes();
                            fontStream = GetPdfFontStream(fontStreamBytes, new int[] { fontStreamBytes.Length
                                 });
                            fontStream.Put(PdfName.Subtype, new PdfName("Type1C"));
                        }
                        catch (PdfException e)
                        {
                            ILogger logger = LoggerFactory.GetLogger(typeof(iTextSharp.Kernel.Font.PdfTrueTypeFont
                                ));
                            logger.Error(e.Message);
                            fontStream = null;
                        }
                    }
                    else
                    {
                        fontFileName = PdfName.FontFile2;
                        ICollection<int> glyphs = new HashSet<int>();
                        for (int k = 0; k < shortTag.Length; k++)
                        {
                            if (shortTag[k] != 0)
                            {
                                int uni = fontEncoding.GetUnicode(k);
                                Glyph glyph = uni > -1 ? fontProgram.GetGlyph(uni) : fontProgram.GetGlyphByCode(k
                                    );
                                if (glyph != null)
                                {
                                    glyphs.Add(glyph.GetCode());
                                }
                            }
                        }
                        AddRangeUni(glyphs);
                        try
                        {
                            byte[] fontStreamBytes;
                            if (subset || ((TrueTypeFont)GetFontProgram()).GetDirectoryOffset() != 0 || subsetRanges
                                 != null)
                            {
                                //clone glyphs due to possible cache issue
                                fontStreamBytes = ((TrueTypeFont)GetFontProgram()).GetSubset(new HashSet<int>(glyphs
                                    ), subset);
                            }
                            else
                            {
                                fontStreamBytes = ((TrueTypeFont)GetFontProgram()).GetFontStreamBytes();
                            }
                            fontStream = GetPdfFontStream(fontStreamBytes, new int[] { fontStreamBytes.Length
                                 });
                        }
                        catch (PdfException e)
                        {
                            ILogger logger = LoggerFactory.GetLogger(typeof(iTextSharp.Kernel.Font.PdfTrueTypeFont
                                ));
                            logger.Error(e.Message);
                            fontStream = null;
                        }
                    }
                }
                if (fontStream != null)
                {
                    fontDescriptor.Put(fontFileName, fontStream);
                }
            }
        }
    }
}
