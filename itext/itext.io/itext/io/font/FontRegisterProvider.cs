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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Font.Constants;

namespace iText.IO.Font {
    /// <summary>
    /// If you are using True Type fonts, you can declare the paths of the different ttf- and ttc-files
    /// to this class first and then create fonts in your code using one of the getFont method
    /// without having to enter a path as parameter.
    /// </summary>
    internal class FontRegisterProvider {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.IO.Font.FontRegisterProvider
            ));

        /// <summary>This is a map of postscriptfontnames of fonts and the path of their font file.</summary>
        private readonly IDictionary<String, String> fontNames = new Dictionary<String, String>();

        /// <summary>This is a map of fontfamilies.</summary>
        private readonly IDictionary<String, IList<String>> fontFamilies = new Dictionary<String, IList<String>>();

        /// <summary>Creates new FontRegisterProvider</summary>
        internal FontRegisterProvider() {
            RegisterStandardFonts();
            RegisterStandardFontFamilies();
        }

        /// <summary>Constructs a <c>Font</c>-object.</summary>
        /// <param name="fontName">the name of the font</param>
        /// <param name="style">the style of this font</param>
        /// <returns>the Font constructed based on the parameters</returns>
        internal virtual FontProgram GetFont(String fontName, int style) {
            return GetFont(fontName, style, true);
        }

        /// <summary>Constructs a <c>Font</c>-object.</summary>
        /// <param name="fontName">the name of the font</param>
        /// <param name="style">the style of this font</param>
        /// <param name="cached">
        /// true if the font comes from the cache or is added to
        /// the cache if new, false if the font is always created new
        /// </param>
        /// <returns>the Font constructed based on the parameters</returns>
        internal virtual FontProgram GetFont(String fontName, int style, bool cached) {
            if (fontName == null) {
                return null;
            }
            String lowerCaseFontName = fontName.ToLowerInvariant();
            IList<String> family = !lowerCaseFontName.EqualsIgnoreCase(StandardFonts.TIMES_ROMAN) ? fontFamilies.Get(lowerCaseFontName
                ) : fontFamilies.Get(StandardFontFamilies.TIMES.ToLowerInvariant());
            if (family != null) {
                lock (family) {
                    // some bugs were fixed here by Daniel Marczisovszky
                    int s = style == FontStyles.UNDEFINED ? FontStyles.NORMAL : style;
                    foreach (String f in family) {
                        String lcf = f.ToLowerInvariant();
                        int fs = FontStyles.NORMAL;
                        if (lcf.Contains("bold")) {
                            fs |= FontStyles.BOLD;
                        }
                        if (lcf.Contains("italic") || lcf.Contains("oblique")) {
                            fs |= FontStyles.ITALIC;
                        }
                        if ((s & FontStyles.BOLDITALIC) == fs) {
                            fontName = f;
                            break;
                        }
                    }
                }
            }
            return GetFontProgram(fontName, cached);
        }

        protected internal virtual void RegisterStandardFonts() {
            fontNames.Put(StandardFonts.COURIER.ToLowerInvariant(), StandardFonts.COURIER);
            fontNames.Put(StandardFonts.COURIER_BOLD.ToLowerInvariant(), StandardFonts.COURIER_BOLD);
            fontNames.Put(StandardFonts.COURIER_OBLIQUE.ToLowerInvariant(), StandardFonts.COURIER_OBLIQUE);
            fontNames.Put(StandardFonts.COURIER_BOLDOBLIQUE.ToLowerInvariant(), StandardFonts.COURIER_BOLDOBLIQUE);
            fontNames.Put(StandardFonts.HELVETICA.ToLowerInvariant(), StandardFonts.HELVETICA);
            fontNames.Put(StandardFonts.HELVETICA_BOLD.ToLowerInvariant(), StandardFonts.HELVETICA_BOLD);
            fontNames.Put(StandardFonts.HELVETICA_OBLIQUE.ToLowerInvariant(), StandardFonts.HELVETICA_OBLIQUE);
            fontNames.Put(StandardFonts.HELVETICA_BOLDOBLIQUE.ToLowerInvariant(), StandardFonts.HELVETICA_BOLDOBLIQUE);
            fontNames.Put(StandardFonts.SYMBOL.ToLowerInvariant(), StandardFonts.SYMBOL);
            fontNames.Put(StandardFonts.TIMES_ROMAN.ToLowerInvariant(), StandardFonts.TIMES_ROMAN);
            fontNames.Put(StandardFonts.TIMES_BOLD.ToLowerInvariant(), StandardFonts.TIMES_BOLD);
            fontNames.Put(StandardFonts.TIMES_ITALIC.ToLowerInvariant(), StandardFonts.TIMES_ITALIC);
            fontNames.Put(StandardFonts.TIMES_BOLDITALIC.ToLowerInvariant(), StandardFonts.TIMES_BOLDITALIC);
            fontNames.Put(StandardFonts.ZAPFDINGBATS.ToLowerInvariant(), StandardFonts.ZAPFDINGBATS);
        }

        protected internal virtual void RegisterStandardFontFamilies() {
            IList<String> family;
            family = new List<String>();
            family.Add(StandardFonts.COURIER);
            family.Add(StandardFonts.COURIER_BOLD);
            family.Add(StandardFonts.COURIER_OBLIQUE);
            family.Add(StandardFonts.COURIER_BOLDOBLIQUE);
            fontFamilies.Put(StandardFontFamilies.COURIER.ToLowerInvariant(), family);
            family = new List<String>();
            family.Add(StandardFonts.HELVETICA);
            family.Add(StandardFonts.HELVETICA_BOLD);
            family.Add(StandardFonts.HELVETICA_OBLIQUE);
            family.Add(StandardFonts.HELVETICA_BOLDOBLIQUE);
            fontFamilies.Put(StandardFontFamilies.HELVETICA.ToLowerInvariant(), family);
            family = new List<String>();
            family.Add(StandardFonts.SYMBOL);
            fontFamilies.Put(StandardFontFamilies.SYMBOL.ToLowerInvariant(), family);
            family = new List<String>();
            family.Add(StandardFonts.TIMES_ROMAN);
            family.Add(StandardFonts.TIMES_BOLD);
            family.Add(StandardFonts.TIMES_ITALIC);
            family.Add(StandardFonts.TIMES_BOLDITALIC);
            fontFamilies.Put(StandardFontFamilies.TIMES.ToLowerInvariant(), family);
            family = new List<String>();
            family.Add(StandardFonts.ZAPFDINGBATS);
            fontFamilies.Put(StandardFontFamilies.ZAPFDINGBATS.ToLowerInvariant(), family);
        }

        protected internal virtual FontProgram GetFontProgram(String fontName, bool cached) {
            FontProgram fontProgram = null;
            fontName = fontNames.Get(fontName.ToLowerInvariant());
            if (fontName != null) {
                fontProgram = FontProgramFactory.CreateFont(fontName, cached);
            }
            return fontProgram;
        }

        /// <summary>Register a font by giving explicitly the font family and name.</summary>
        /// <param name="familyName">the font family</param>
        /// <param name="fullName">the font name</param>
        /// <param name="path">the font path</param>
        internal virtual void RegisterFontFamily(String familyName, String fullName, String path) {
            if (path != null) {
                fontNames.Put(fullName, path);
            }
            IList<String> family;
            lock (fontFamilies) {
                family = fontFamilies.Get(familyName);
                if (family == null) {
                    family = new List<String>();
                    fontFamilies.Put(familyName, family);
                }
            }
            lock (family) {
                if (!family.Contains(fullName)) {
                    int fullNameLength = fullName.Length;
                    bool inserted = false;
                    for (int j = 0; j < family.Count; ++j) {
                        if (family[j].Length >= fullNameLength) {
                            family.Add(j, fullName);
                            inserted = true;
                            break;
                        }
                    }
                    if (!inserted) {
                        family.Add(fullName);
                        String newFullName = fullName.ToLowerInvariant();
                        if (newFullName.EndsWith("regular")) {
                            //remove "regular" at the end of the font name
                            newFullName = newFullName.JSubstring(0, newFullName.Length - 7).Trim();
                            //insert this font name at the first position for higher priority
                            family.Add(0, fullName.JSubstring(0, newFullName.Length));
                        }
                    }
                }
            }
        }

        /// <summary>Register a font file, either .ttf or .otf, .afm or a font from TrueType Collection.</summary>
        /// <remarks>
        /// Register a font file, either .ttf or .otf, .afm or a font from TrueType Collection.
        /// If a TrueType Collection is registered, an additional index of the font program can be specified
        /// </remarks>
        /// <param name="path">the path to a ttf- or ttc-file</param>
        internal virtual void RegisterFont(String path) {
            RegisterFont(path, null);
        }

        /// <summary>Register a font file and use an alias for the font contained in it.</summary>
        /// <param name="path">the path to a font file</param>
        /// <param name="alias">the alias you want to use for the font</param>
        internal virtual void RegisterFont(String path, String alias) {
            try {
                if (path.ToLowerInvariant().EndsWith(".ttf") || path.ToLowerInvariant().EndsWith(".otf") || path.ToLowerInvariant
                    ().IndexOf(".ttc,", StringComparison.Ordinal) > 0) {
                    FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor(path);
                    fontNames.Put(descriptor.GetFontNameLowerCase(), path);
                    if (alias != null) {
                        String lcAlias = alias.ToLowerInvariant();
                        fontNames.Put(lcAlias, path);
                        if (lcAlias.EndsWith("regular")) {
                            //do this job to give higher priority to regular fonts in comparison with light, narrow, etc
                            SaveCopyOfRegularFont(lcAlias, path);
                        }
                    }
                    // register all the font names with all the locales
                    foreach (String name in descriptor.GetFullNameAllLangs()) {
                        fontNames.Put(name, path);
                        if (name.EndsWith("regular")) {
                            //do this job to give higher priority to regular fonts in comparison with light, narrow, etc
                            SaveCopyOfRegularFont(name, path);
                        }
                    }
                    if (descriptor.GetFamilyNameEnglishOpenType() != null) {
                        foreach (String fullName in descriptor.GetFullNamesEnglishOpenType()) {
                            RegisterFontFamily(descriptor.GetFamilyNameEnglishOpenType(), fullName, null);
                        }
                    }
                }
                else {
                    if (path.ToLowerInvariant().EndsWith(".ttc")) {
                        TrueTypeCollection ttc = new TrueTypeCollection(path);
                        for (int i = 0; i < ttc.GetTTCSize(); i++) {
                            String fullPath = path + "," + i;
                            if (alias != null) {
                                RegisterFont(fullPath, alias + "," + i);
                            }
                            else {
                                RegisterFont(fullPath);
                            }
                        }
                    }
                    else {
                        if (path.ToLowerInvariant().EndsWith(".afm") || path.ToLowerInvariant().EndsWith(".pfm")) {
                            FontProgramDescriptor descriptor = FontProgramDescriptorFactory.FetchDescriptor(path);
                            RegisterFontFamily(descriptor.GetFamilyNameLowerCase(), descriptor.GetFullNameLowerCase(), null);
                            fontNames.Put(descriptor.GetFontNameLowerCase(), path);
                            fontNames.Put(descriptor.GetFullNameLowerCase(), path);
                        }
                    }
                }
                LOGGER.LogTrace(MessageFormatUtil.Format("Registered {0}", path));
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(e);
            }
        }

        // remove regular and correct last symbol
        // do this job to give higher priority to regular fonts in comparison with light, narrow, etc
        // Don't use this method for not regular fonts!
        internal virtual bool SaveCopyOfRegularFont(String regularFontName, String path) {
            //remove "regular" at the end of the font name
            String alias = regularFontName.JSubstring(0, regularFontName.Length - 7).Trim();
            if (!fontNames.ContainsKey(alias)) {
                fontNames.Put(alias, path);
                return true;
            }
            return false;
        }

        /// <summary>Register all the fonts in a directory.</summary>
        /// <param name="dir">the directory</param>
        /// <returns>the number of fonts registered</returns>
        internal virtual int RegisterFontDirectory(String dir) {
            return RegisterFontDirectory(dir, false);
        }

        /// <summary>Register all the fonts in a directory and possibly its subdirectories.</summary>
        /// <param name="dir">the directory</param>
        /// <param name="scanSubdirectories">recursively scan subdirectories if <c>true</c></param>
        /// <returns>the number of fonts registered</returns>
        internal virtual int RegisterFontDirectory(String dir, bool scanSubdirectories) {
            LOGGER.LogDebug(MessageFormatUtil.Format("Registering directory {0}, looking for fonts", dir));
            int count = 0;
            try {
                String[] files = FileUtil.ListFilesInDirectory(dir, scanSubdirectories);
                if (files == null) {
                    return 0;
                }
                foreach (String file in files) {
                    try {
                        String suffix = file.Length < 4 ? null : file.Substring(file.Length - 4).ToLowerInvariant();
                        if (".afm".Equals(suffix) || ".pfm".Equals(suffix)) {
                            /* Only register Type 1 fonts with matching .pfb files */
                            String pfb = file.JSubstring(0, file.Length - 4) + ".pfb";
                            if (FileUtil.FileExists(pfb)) {
                                RegisterFont(file, null);
                                ++count;
                            }
                        }
                        else {
                            if (".ttf".Equals(suffix) || ".otf".Equals(suffix) || ".ttc".Equals(suffix)) {
                                RegisterFont(file, null);
                                ++count;
                            }
                        }
                    }
                    catch (Exception) {
                    }
                }
            }
            catch (Exception) {
            }
            //empty on purpose
            //empty on purpose
            return count;
        }

        /// <summary>Register fonts in some probable directories.</summary>
        /// <remarks>
        /// Register fonts in some probable directories. It usually works in Windows,
        /// Linux and Solaris.
        /// </remarks>
        /// <returns>the number of fonts registered</returns>
        internal virtual int RegisterSystemFontDirectories() {
            int count = 0;
            String[] withSubDirs = new String[] { FileUtil.GetFontsDir(), "/usr/share/X11/fonts", "/usr/X/lib/X11/fonts"
                , "/usr/openwin/lib/X11/fonts", "/usr/share/fonts", "/usr/X11R6/lib/X11/fonts" };
            foreach (String directory in withSubDirs) {
                count += RegisterFontDirectory(directory, true);
            }
            String[] withoutSubDirs = new String[] { "/Library/Fonts", "/System/Library/Fonts" };
            foreach (String directory in withoutSubDirs) {
                count += RegisterFontDirectory(directory, false);
            }
            return count;
        }

        /// <summary>Gets a set of registered font names.</summary>
        /// <returns>a set of registered fonts</returns>
        internal virtual ICollection<String> GetRegisteredFonts() {
            return fontNames.Keys;
        }

        /// <summary>Gets a set of registered font names.</summary>
        /// <returns>a set of registered font families</returns>
        internal virtual ICollection<String> GetRegisteredFontFamilies() {
            return fontFamilies.Keys;
        }

        /// <summary>Checks if a certain font is registered.</summary>
        /// <param name="fontname">the name of the font that has to be checked.</param>
        /// <returns>true if the font is found</returns>
        internal virtual bool IsRegisteredFont(String fontname) {
            return fontNames.ContainsKey(fontname.ToLowerInvariant());
        }

        public virtual void ClearRegisteredFonts() {
            fontNames.Clear();
            RegisterStandardFonts();
        }

        public virtual void ClearRegisteredFontFamilies() {
            fontFamilies.Clear();
            RegisterStandardFontFamilies();
        }
    }
}
