/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Datastructures;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Layout.Logs;

namespace iText.Layout.Renderer.Typography {
    public sealed class DefaultTypographyApplier : AbstractTypographyApplier {
        private const String SCRIPT = "script";

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.Typography.DefaultTypographyApplier
            ));

        private static readonly ConcurrentWeakMap<SequenceId, ICollection<String>> IDS_WITH_WARNING = new ConcurrentWeakMap
            <SequenceId, ICollection<String>>();

        private static readonly ConcurrentWeakMap<SequenceId, ICollection<String>> IDS_WITH_INFO = new ConcurrentWeakMap
            <SequenceId, ICollection<String>>();

        public DefaultTypographyApplier() {
        }

        public override bool IsPdfCalligraphInstance() {
            return false;
        }

        public override ICollection<UnicodeScript> GetSupportedScripts() {
            return ScriptInfo.GetSupportedScripts();
        }

        public override ICollection<UnicodeScript> GetSupportedScripts(Object configurator) {
            return ScriptInfo.GetSupportedScripts();
        }

        public override bool ApplyOtfScript(TrueTypeFont font, GlyphLine glyphLine, UnicodeScript? script, Object 
            configurator, SequenceId id, IMetaInfo metaInfo) {
            CheckTypographyRequired(font, script, id);
            return base.ApplyOtfScript(font, glyphLine, script, configurator, id, metaInfo);
        }

        public override bool ApplyKerning(FontProgram fontProgram, GlyphLine text, SequenceId sequenceId, IMetaInfo
             metaInfo) {
            if (fontProgram.HasKernPairs()) {
                LogWarning(sequenceId, "kerning", "kerning enabled");
            }
            return base.ApplyKerning(fontProgram, text, sequenceId, metaInfo);
        }

        public override IList<int> GetPossibleBreaks(String str) {
            return JavaCollectionsUtil.EmptyList<int>();
        }

        public override IDictionary<String, byte[]> LoadShippedFonts() {
            return new Dictionary<String, byte[]>();
        }

        private static void CheckTypographyRequired(TrueTypeFont font, UnicodeScript? script, SequenceId id) {
            if (ScriptInfo.ScriptSupported(script)) {
                ScriptRequirements reqs = ScriptInfo.GetRequirements(script);
                if (!HasWarning(id, script.ToString())) {
                    if (reqs.IsHardCodedHandling()) {
                        LogWarning(id, script.ToString(), SCRIPT, script.ToString(), "which requires special handling.");
                    }
                    else {
                        if (FontHasFeature(font, reqs.GetOtfScriptNames(), reqs.GetRequiredFeatures())) {
                            LogWarning(id, script.ToString(), SCRIPT, script.ToString(), "with required features", reqs.GetRequiredFeatures
                                ().ToString());
                        }
                    }
                    if (!HasInfo(id, script.ToString()) && FontHasFeature(font, reqs.GetOtfScriptNames(), reqs.GetAffectingFeatures
                        ())) {
                        LogInfo(id, script.ToString(), reqs.GetAffectingFeatures().ToString());
                    }
                }
            }
        }

        private static bool FontHasFeature(TrueTypeFont font, ICollection<String> otfScriptNames, ICollection<String
            > features) {
            IDictionary<String, IList<OpenTableLookup>> featuresFound = new Dictionary<String, IList<OpenTableLookup>>
                ();
            font.ExtractFeatures(otfScriptNames, featuresFound);
            foreach (String feature in features) {
                if (featuresFound.ContainsKey(feature)) {
                    return true;
                }
            }
            return false;
        }

        private static bool HasWarning(SequenceId id, String script) {
            return IDS_WITH_WARNING.ContainsKey(id) && IDS_WITH_WARNING.Get(id).Contains(script);
        }

        private static bool HasInfo(SequenceId id, String script) {
            return (IDS_WITH_INFO.ContainsKey(id) && IDS_WITH_INFO.Get(id).Contains(script)) || HasWarning(id, script);
        }

        private static void LogWarning(SequenceId id, String script, params String[] messageParts) {
            if (LOGGER.IsEnabled(LogLevel.Warning)) {
                if (IDS_WITH_WARNING.ContainsKey(id)) {
                    if (IDS_WITH_WARNING.Get(id).Contains(script)) {
                        return;
                    }
                    IDS_WITH_WARNING.Get(id).Add(script);
                }
                else {
                    IDS_WITH_WARNING.Put(id, new HashSet<String>(JavaCollectionsUtil.Singleton(script)));
                }
                StringBuilder message = new StringBuilder();
                foreach (String part in messageParts) {
                    message.Append(part).Append(' ');
                }
                LOGGER.LogWarning(MessageFormatUtil.Format(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_WARNING, message)
                    );
            }
        }

        private static void LogInfo(SequenceId id, String script, String features) {
            if (LOGGER.IsEnabled(LogLevel.Information)) {
                if ((IDS_WITH_WARNING.ContainsKey(id) && IDS_WITH_WARNING.Get(id).Contains(script)) || (IDS_WITH_INFO.ContainsKey
                    (id) && IDS_WITH_INFO.Get(id).Contains(script))) {
                    return;
                }
                if (IDS_WITH_INFO.ContainsKey(id)) {
                    IDS_WITH_INFO.Get(id).Add(script);
                }
                else {
                    IDS_WITH_INFO.Put(id, new HashSet<String>(JavaCollectionsUtil.Singleton(script)));
                }
                LOGGER.LogInformation(MessageFormatUtil.Format(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_INFO, script, 
                    features));
            }
        }
    }
}
