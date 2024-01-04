/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Renderer.Typography {
    public sealed class DefaultTypographyApplier : AbstractTypographyApplier {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.Typography.DefaultTypographyApplier
            ));

        public DefaultTypographyApplier() {
        }

        public override bool IsPdfCalligraphInstance() {
            return false;
        }

        public override bool ApplyOtfScript(TrueTypeFont font, GlyphLine glyphLine, UnicodeScript? script, Object 
            configurator, SequenceId id, IMetaInfo metaInfo) {
            LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.ApplyOtfScript(font, glyphLine, script, configurator, id, metaInfo);
        }

        public override ICollection<UnicodeScript> GetSupportedScripts() {
            LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.GetSupportedScripts();
        }

        public override ICollection<UnicodeScript> GetSupportedScripts(Object configurator) {
            LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.GetSupportedScripts(configurator);
        }

        public override bool ApplyKerning(FontProgram fontProgram, GlyphLine text, SequenceId sequenceId, IMetaInfo
             metaInfo) {
            LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.ApplyKerning(fontProgram, text, sequenceId, metaInfo);
        }

        public override byte[] GetBidiLevels(BaseDirection? baseDirection, int[] unicodeIds, SequenceId sequenceId
            , IMetaInfo metaInfo) {
            LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.GetBidiLevels(baseDirection, unicodeIds, sequenceId, metaInfo);
        }

        public override int[] ReorderLine(IList<LineRenderer.RendererGlyph> line, byte[] lineLevels, byte[] levels
            ) {
            LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.ReorderLine(line, lineLevels, levels);
        }

        public override IList<int> GetPossibleBreaks(String str) {
            LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.GetPossibleBreaks(str);
        }

        public override IDictionary<String, byte[]> LoadShippedFonts() {
            LOGGER.LogWarning(iText.IO.Logs.IoLogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.LoadShippedFonts();
        }
    }
}
