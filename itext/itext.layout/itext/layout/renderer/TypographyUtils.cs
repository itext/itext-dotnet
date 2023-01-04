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

        internal static void ApplyOtfScript(FontProgram fontProgram, GlyphLine text, UnicodeScript? script, Object
             typographyConfig, SequenceId sequenceId, IMetaInfo metaInfo) {
            applierInstance.ApplyOtfScript((TrueTypeFont)fontProgram, text, script, typographyConfig, sequenceId, metaInfo
                );
        }

        internal static void ApplyKerning(FontProgram fontProgram, GlyphLine text, SequenceId sequenceId, IMetaInfo
             metaInfo) {
            applierInstance.ApplyKerning(fontProgram, text, sequenceId, metaInfo);
        }

        internal static byte[] GetBidiLevels(BaseDirection? baseDirection, int[] unicodeIds, SequenceId sequenceId
            , IMetaInfo metaInfo) {
            return applierInstance.GetBidiLevels(baseDirection, unicodeIds, sequenceId, metaInfo);
        }

        internal static int[] ReorderLine(IList<LineRenderer.RendererGlyph> line, byte[] lineLevels, byte[] levels
            ) {
            return applierInstance.ReorderLine(line, lineLevels, levels);
        }

        internal static IList<int> GetPossibleBreaks(String str) {
            return applierInstance.GetPossibleBreaks(str);
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
                        ILogger logger = ITextLogManager.GetLogger(typeof(TypographyUtils));
                        logger.LogError(fileLoadExceptionMessage);
                    }
                }
            }

            return type;
        }
    }
}
