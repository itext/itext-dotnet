/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Reflection;
using Microsoft.Extensions.Logging;
using Versions.Attributes;
using iText.Commons;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Layout.Properties;
using iText.Layout.Renderer.Typography;

namespace iText.Layout.Renderer {
    public sealed class TypographyUtils {
        private const String TYPOGRAPHY_PACKAGE = "iText.Typography.";

        private const String TYPOGRAPHY_APPLIER = "Shaping.TypographyApplier,iText.Typography";

        private const String TYPOGRAPHY_APPLIER_INITIALIZE = "RegisterForLayout";

        private static AbstractTypographyApplier applierInstance;

        static TypographyUtils() {
            try {
                Type type = GetTypographyClass(TYPOGRAPHY_PACKAGE + TYPOGRAPHY_APPLIER);
                if (type != null) {
                    MethodInfo method = type.GetMethod(TYPOGRAPHY_APPLIER_INITIALIZE, new Type[] {  });
                    if (method != null) {
                        method.Invoke(null, new Object[] {  });
                    }
                }
            }
            catch (Exception) {
            }
            // do nothing
            if (applierInstance == null) {
                SetTypographyApplierInstance(new DefaultTypographyApplier());
            }
        }

        private TypographyUtils() {
        }

        /// <summary>
        /// Set
        /// <see cref="iText.Layout.Renderer.Typography.AbstractTypographyApplier"/>
        /// instance to use.
        /// </summary>
        /// <param name="newInstance">the instance to set</param>
        public static void SetTypographyApplierInstance(AbstractTypographyApplier newInstance) {
            applierInstance = newInstance;
        }

        /// <summary>Checks if layout module can access pdfCalligraph</summary>
        /// <returns><c>true</c> if layout can access pdfCalligraph and <c>false</c> otherwise</returns>
        public static bool IsPdfCalligraphAvailable() {
            return applierInstance.IsPdfCalligraphInstance();
        }

        public static ICollection<UnicodeScript> GetSupportedScripts() {
            return applierInstance.GetSupportedScripts();
        }

        public static ICollection<UnicodeScript> GetSupportedScripts(Object typographyConfig) {
            return applierInstance.GetSupportedScripts(typographyConfig);
        }

        public static IDictionary<String, byte[]> LoadShippedFonts() {
            return applierInstance.LoadShippedFonts();
        }

//\cond DO_NOT_DOCUMENT
        internal static void ApplyOtfScript(FontProgram fontProgram, GlyphLine text, UnicodeScript? script, Object
             typographyConfig, SequenceId sequenceId, IMetaInfo metaInfo) {
            applierInstance.ApplyOtfScript((TrueTypeFont)fontProgram, text, script, typographyConfig, sequenceId, metaInfo
                );
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void ApplyKerning(FontProgram fontProgram, GlyphLine text, SequenceId sequenceId, IMetaInfo
             metaInfo) {
            applierInstance.ApplyKerning(fontProgram, text, sequenceId, metaInfo);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static byte[] GetBidiLevels(BaseDirection? baseDirection, int[] unicodeIds, SequenceId sequenceId
            , IMetaInfo metaInfo) {
            return applierInstance.GetBidiLevels(baseDirection, unicodeIds, sequenceId, metaInfo);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static int[] ReorderLine(IList<LineRenderer.RendererGlyph> line, byte[] lineLevels, byte[] levels
            ) {
            return applierInstance.ReorderLine(line, lineLevels, levels);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static IList<int> GetPossibleBreaks(String str) {
            return applierInstance.GetPossibleBreaks(str);
        }
//\endcond

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
                        ILogger logger = ITextLogManager.GetLogger(typeof(TypographyUtils));
                        logger.LogError(fileLoadExceptionMessage);
                    }
                }
            }

            return type;
        }
    }
}
