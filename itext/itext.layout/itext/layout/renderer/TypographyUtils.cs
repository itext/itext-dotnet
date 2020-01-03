/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.Reflection;
using Common.Logging;
using Versions.Attributes;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public sealed class TypographyUtils {
        private static readonly ILog logger = LogManager.GetLogger(typeof(iText.Layout.Renderer.TypographyUtils));

        private const String TYPOGRAPHY_PACKAGE = "iText.Typography.";

        private const String SHAPER = "Shaping.Shaper,iText.Typography";

        private const String BIDI_CHARACTER_MAP = "Bidi.BidiCharacterMap,iText.Typography";

        private const String BIDI_BRACKET_MAP = "Bidi.BidiBracketMap,iText.Typography";

        private const String BIDI_ALGORITHM = "Bidi.BidiAlgorithm,iText.Typography";

        private const String APPLY_OTF_SCRIPT = "ApplyOtfScript";

        private const String APPLY_KERNING = "ApplyKerning";

        private const String GET_SUPPORTED_SCRIPTS = "GetSupportedScripts";

        private const String GET_CHARACTER_TYPES = "GetCharacterTypes";

        private const String GET_BRACKET_TYPES = "GetBracketTypes";

        private const String GET_BRACKET_VALUES = "GetBracketValues";

        private const String GET_PAIRED_BRACKET = "GetPairedBracket";

        private const String GET_LEVELS = "GetLevels";

        private const String COMPUTE_REORDERING = "ComputeReordering";

        private const String INVERSE_REORDERING = "InverseReordering";

        private static readonly ICollection<UnicodeScript> SUPPORTED_SCRIPTS;

        private static readonly bool TYPOGRAPHY_MODULE_INITIALIZED;

        private static IDictionary<String, Type> cachedClasses = new Dictionary<String, Type>();

        private static IDictionary<TypographyUtils.TypographyMethodSignature, MemberInfo> cachedMethods = new Dictionary
            <TypographyUtils.TypographyMethodSignature, MemberInfo>();

        private const String typographyNotFoundException = "Cannot find pdfCalligraph module, which was implicitly required by one of the layout properties";

        static TypographyUtils() {
            bool moduleFound = false;
            try {
                Type type = GetTypographyClass(TYPOGRAPHY_PACKAGE + SHAPER);
                if (type != null) {
                    moduleFound = true;
                }
            }
            catch (TypeLoadException) {
            }
            ICollection<UnicodeScript> supportedScripts = null;
            if (moduleFound) {
                try {
                    supportedScripts = (ICollection<UnicodeScript>)CallMethod(TYPOGRAPHY_PACKAGE + SHAPER, GET_SUPPORTED_SCRIPTS
                        , new Type[] {  });
                }
                catch (Exception e) {
                    supportedScripts = null;
                    logger.Error(e.Message);
                }
            }
            moduleFound = supportedScripts != null;
            if (!moduleFound) {
                cachedClasses.Clear();
                cachedMethods.Clear();
            }
            TYPOGRAPHY_MODULE_INITIALIZED = moduleFound;
            SUPPORTED_SCRIPTS = supportedScripts;
        }

        private TypographyUtils() {
        }

        /// <summary>Checks if layout module can access pdfCalligraph</summary>
        /// <returns><c>true</c> if layout can access pdfCalligraph and <c>false</c> otherwise</returns>
        public static bool IsPdfCalligraphAvailable() {
            return TYPOGRAPHY_MODULE_INITIALIZED;
        }

        internal static void ApplyOtfScript(FontProgram fontProgram, GlyphLine text, UnicodeScript? script, Object
             typographyConfig) {
            if (!TYPOGRAPHY_MODULE_INITIALIZED) {
                logger.Warn(typographyNotFoundException);
            }
            else {
                CallMethod(TYPOGRAPHY_PACKAGE + SHAPER, APPLY_OTF_SCRIPT, new Type[] { typeof(TrueTypeFont), typeof(GlyphLine
                    ), typeof(UnicodeScript?), typeof(Object) }, fontProgram, text, script, typographyConfig);
            }
        }

        internal static void ApplyKerning(FontProgram fontProgram, GlyphLine text) {
            if (!TYPOGRAPHY_MODULE_INITIALIZED) {
                logger.Warn(typographyNotFoundException);
            }
            else {
                CallMethod(TYPOGRAPHY_PACKAGE + SHAPER, APPLY_KERNING, new Type[] { typeof(FontProgram), typeof(GlyphLine)
                     }, fontProgram, text);
            }
        }

        //            Shaper.applyKerning(fontProgram, text);
        internal static byte[] GetBidiLevels(BaseDirection? baseDirection, int[] unicodeIds) {
            if (!TYPOGRAPHY_MODULE_INITIALIZED) {
                logger.Warn(typographyNotFoundException);
            }
            else {
                byte direction;
                switch (baseDirection) {
                    case BaseDirection.LEFT_TO_RIGHT: {
                        direction = 0;
                        break;
                    }

                    case BaseDirection.RIGHT_TO_LEFT: {
                        direction = 1;
                        break;
                    }

                    case BaseDirection.DEFAULT_BIDI:
                    default: {
                        direction = 2;
                        break;
                    }
                }
                int len = unicodeIds.Length;
                byte[] types = (byte[])CallMethod(TYPOGRAPHY_PACKAGE + BIDI_CHARACTER_MAP, GET_CHARACTER_TYPES, new Type[]
                     { typeof(int[]), typeof(int), typeof(int) }, unicodeIds, 0, len);
                //            byte[] types = BidiCharacterMap.getCharacterTypes(unicodeIds, 0, len);
                byte[] pairTypes = (byte[])CallMethod(TYPOGRAPHY_PACKAGE + BIDI_BRACKET_MAP, GET_BRACKET_TYPES, new Type[]
                     { typeof(int[]), typeof(int), typeof(int) }, unicodeIds, 0, len);
                //            byte[] pairTypes = BidiBracketMap.getBracketTypes(unicodeIds, 0, len);
                int[] pairValues = (int[])CallMethod(TYPOGRAPHY_PACKAGE + BIDI_BRACKET_MAP, GET_BRACKET_VALUES, new Type[]
                     { typeof(int[]), typeof(int), typeof(int) }, unicodeIds, 0, len);
                //            int[] pairValues = BidiBracketMap.getBracketValues(unicodeIds, 0, len);
                Object bidiReorder = CallConstructor(TYPOGRAPHY_PACKAGE + BIDI_ALGORITHM, new Type[] { typeof(byte[]), typeof(
                    byte[]), typeof(int[]), typeof(byte) }, types, pairTypes, pairValues, direction);
                //            BidiAlgorithm bidiReorder = new BidiAlgorithm(types, pairTypes, pairValues, direction);
                return (byte[])CallMethod(TYPOGRAPHY_PACKAGE + BIDI_ALGORITHM, GET_LEVELS, bidiReorder, new Type[] { typeof(
                    int[]) }, new int[] { len });
            }
            //            return bidiReorder.getLevels(new int[]{len});
            return null;
        }

        internal static int[] ReorderLine(IList<LineRenderer.RendererGlyph> line, byte[] lineLevels, byte[] levels
            ) {
            if (!TYPOGRAPHY_MODULE_INITIALIZED) {
                logger.Warn(typographyNotFoundException);
            }
            else {
                if (levels == null) {
                    return null;
                }
                int[] reorder = (int[])CallMethod(TYPOGRAPHY_PACKAGE + BIDI_ALGORITHM, COMPUTE_REORDERING, new Type[] { typeof(
                    byte[]) }, lineLevels);
                //            int[] reorder = BidiAlgorithm.computeReordering(lineLevels);
                int[] inverseReorder = (int[])CallMethod(TYPOGRAPHY_PACKAGE + BIDI_ALGORITHM, INVERSE_REORDERING, new Type
                    [] { typeof(int[]) }, reorder);
                //            int[] inverseReorder = BidiAlgorithm.inverseReordering(reorder);
                IList<LineRenderer.RendererGlyph> reorderedLine = new List<LineRenderer.RendererGlyph>(lineLevels.Length);
                for (int i = 0; i < line.Count; i++) {
                    reorderedLine.Add(line[reorder[i]]);
                    // Mirror RTL glyphs
                    if (levels[reorder[i]] % 2 == 1) {
                        if (reorderedLine[i].glyph.HasValidUnicode()) {
                            int unicode = reorderedLine[i].glyph.GetUnicode();
                            int pairedBracket = (int)CallMethod(TYPOGRAPHY_PACKAGE + BIDI_BRACKET_MAP, GET_PAIRED_BRACKET, new Type[] 
                                { typeof(int) }, unicode);
                            //                        int pairedBracket = BidiBracketMap.getPairedBracket(reorderedLine.get(i).glyph.getUnicode());
                            if (pairedBracket != unicode) {
                                PdfFont font = reorderedLine[i].renderer.GetPropertyAsFont(Property.FONT);
                                reorderedLine[i] = new LineRenderer.RendererGlyph(font.GetGlyph(pairedBracket), reorderedLine[i].renderer);
                            }
                        }
                    }
                }
                // fix anchorDelta
                for (int i = 0; i < reorderedLine.Count; i++) {
                    Glyph glyph = reorderedLine[i].glyph;
                    if (glyph.HasPlacement()) {
                        int oldAnchor = reorder[i] + glyph.GetAnchorDelta();
                        int newPos = inverseReorder[oldAnchor];
                        int newAnchorDelta = newPos - i;
                        glyph.SetAnchorDelta((short)newAnchorDelta);
                    }
                }
                line.Clear();
                line.AddAll(reorderedLine);
                return reorder;
            }
            return null;
        }

        internal static ICollection<UnicodeScript> GetSupportedScripts() {
            if (!TYPOGRAPHY_MODULE_INITIALIZED) {
                logger.Warn(typographyNotFoundException);
                return null;
            }
            else {
                return SUPPORTED_SCRIPTS;
            }
        }

        internal static ICollection<UnicodeScript> GetSupportedScripts(Object typographyConfig) {
            if (!TYPOGRAPHY_MODULE_INITIALIZED) {
                logger.Warn(typographyNotFoundException);
                return null;
            }
            else {
                return (ICollection<UnicodeScript>)CallMethod(TYPOGRAPHY_PACKAGE + SHAPER, GET_SUPPORTED_SCRIPTS, (Object)
                    null, new Type[] { typeof(Object) }, typographyConfig);
            }
        }

        private static Object CallMethod(String className, String methodName, Type[] parameterTypes, params Object
            [] args) {
            return CallMethod(className, methodName, (Object)null, parameterTypes, args);
        }

        private static Object CallMethod(String className, String methodName, Object target, Type[] parameterTypes
            , params Object[] args) {
            try {
                MethodInfo method = FindMethod(className, methodName, parameterTypes);
                return method.Invoke(target, args);
            }
            catch (MissingMethodException) {
                logger.Warn(MessageFormatUtil.Format("Cannot find method {0} for class {1}", methodName, className));
            }
            catch (TypeLoadException) {
                logger.Warn(MessageFormatUtil.Format("Cannot find class {0}", className));
            }
            catch (ArgumentException e) {
                logger.Warn(MessageFormatUtil.Format("Illegal arguments passed to {0}#{1} method call: {2}", className, methodName
                    , e.Message));
            }
            catch (Exception e) {
                // Converting checked exceptions to unchecked RuntimeException (java-specific comment).
                //
                // If typography utils throws an exception at this point, we consider it as unrecoverable situation for
                // its callers (layouting methods). Presence of typography module in class path is checked before.
                // It's might be more suitable to wrap checked exceptions at a bit higher level, but we do it here for
                // the sake of convenience.
                //
                // The RuntimeException exception is used instead of, for example, PdfException, because failure here is
                // unexpected and is not connected to PDF documents processing.
                throw new Exception(e.ToString(), e);
            }
            return null;
        }

        private static Object CallConstructor(String className, Type[] parameterTypes, params Object[] args) {
            try {
                ConstructorInfo constructor = FindConstructor(className, parameterTypes);
                return constructor.Invoke(args);
            }
            catch (MissingMethodException) {
                logger.Warn(MessageFormatUtil.Format("Cannot find constructor for class {0}", className));
            }
            catch (TypeLoadException) {
                logger.Warn(MessageFormatUtil.Format("Cannot find class {0}", className));
            }
            catch (Exception exc) {
                // Converting checked exceptions to unchecked RuntimeException (java-specific comment).
                //
                // If typography utils throws an exception at this point, we consider it as unrecoverable situation for
                // its callers (layouting methods). Presence of typography module in class path is checked before.
                // It's might be more suitable to wrap checked exceptions at a bit higher level, but we do it here for
                // the sake of convenience.
                //
                // The RuntimeException exception is used instead of, for example, PdfException, because failure here is
                // unexpected and is not connected to PDF documents processing.
                throw new Exception(exc.ToString(), exc);
            }
            return null;
        }

        private static MethodInfo FindMethod(String className, String methodName, Type[] parameterTypes) {
            TypographyUtils.TypographyMethodSignature tm = new TypographyUtils.TypographyMethodSignature(className, parameterTypes
                , methodName);
            MethodInfo m = (MethodInfo)cachedMethods.Get(tm);
            if (m == null) {
                m = FindClass(className).GetMethod(methodName, parameterTypes);
                cachedMethods.Put(tm, m);
            }
            return m;
        }

        private static ConstructorInfo FindConstructor(String className, Type[] parameterTypes) {
            TypographyUtils.TypographyMethodSignature tc = new TypographyUtils.TypographyMethodSignature(className, parameterTypes
                );
            ConstructorInfo c = (ConstructorInfo)cachedMethods.Get(tc);
            if (c == null) {
                c = FindClass(className).GetConstructor(parameterTypes);
                cachedMethods.Put(tc, c);
            }
            return c;
        }

        private static Type FindClass(String className) {
            Type c = cachedClasses.Get(className);
            if (c == null) {
                c = GetTypographyClass(className);
                cachedClasses.Put(className, c);
            }
            return c;
        }

        private class TypographyMethodSignature {
            protected internal readonly String className;

            protected internal Type[] parameterTypes;

            private readonly String methodName;

            internal TypographyMethodSignature(String className, Type[] parameterTypes)
                : this(className, parameterTypes, null) {
            }

            internal TypographyMethodSignature(String className, Type[] parameterTypes, String methodName) {
                this.methodName = methodName;
                this.className = className;
                this.parameterTypes = parameterTypes;
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                TypographyUtils.TypographyMethodSignature that = (TypographyUtils.TypographyMethodSignature)o;
                if (!className.Equals(that.className)) {
                    return false;
                }
                if (!JavaUtil.ArraysEquals(parameterTypes, that.parameterTypes)) {
                    return false;
                }
                return methodName != null ? methodName.Equals(that.methodName) : that.methodName == null;
            }

            public override int GetHashCode() {
                int result = className.GetHashCode();
                result = 31 * result + JavaUtil.ArraysHashCode(parameterTypes);
                result = 31 * result + (methodName != null ? methodName.GetHashCode() : 0);
                return result;
            }
        }

        private static Type GetTypographyClass(String partialName) {
            String classFullName = null;

            Assembly layoutAssembly = typeof(TypographyUtils).GetAssembly();
            try {
                Attribute customAttribute = layoutAssembly.GetCustomAttribute(typeof(TypographyVersionAttribute));
                if (customAttribute is TypographyVersionAttribute) {
                    string typographyVersion = ((TypographyVersionAttribute) customAttribute).TypographyVersion;
                    string format = "{0}, Version={1}, Culture=neutral, PublicKeyToken=8354ae6d2174ddca";
                    classFullName = String.Format(format, partialName, typographyVersion);
                }
            } catch (Exception ignored) {
            }

            Type type = null;
            if (classFullName != null) {
                String fileLoadExceptionMessage = null;
                try {
                    type = System.Type.GetType(classFullName);
                } catch (FileLoadException fileLoadException) {
                    fileLoadExceptionMessage = fileLoadException.Message;
                }
                if (type == null) {
                    // try to find typography assembly by it's partial name and check if it refers to current version of itext core
                    try {
                        type = System.Type.GetType(partialName);
                    } catch {
                        // ignore
                    }
                    if (type != null) {
                        bool doesReferToCurrentVersionOfCore = false;
                        foreach (AssemblyName assemblyName in type.GetAssembly().GetReferencedAssemblies()) {
                            if ("itext.io".Equals(assemblyName.Name)) {
                                doesReferToCurrentVersionOfCore = assemblyName.Version.Equals(layoutAssembly.GetName().Version);
                                break;
                            }
                        }
                        if (!doesReferToCurrentVersionOfCore) {
                            type = null;
                        }
                    }
                    if (type == null && fileLoadExceptionMessage != null) {
                        ILog logger = LogManager.GetLogger(typeof(TypographyUtils));
                        logger.Error(fileLoadExceptionMessage);
                    }
                }
            }

            return type;
        }
    }
}
