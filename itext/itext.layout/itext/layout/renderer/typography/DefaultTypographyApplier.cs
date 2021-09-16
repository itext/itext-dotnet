using System;
using System.Collections.Generic;
using Common.Logging;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Renderer.Typography {
    public sealed class DefaultTypographyApplier : AbstractTypographyApplier {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Layout.Renderer.Typography.DefaultTypographyApplier
            ));

        public DefaultTypographyApplier() {
        }

        public override bool IsPdfCalligraphInstance() {
            return false;
        }

        public override bool ApplyOtfScript(TrueTypeFont font, GlyphLine glyphLine, UnicodeScript? script, Object 
            configurator, SequenceId id, IMetaInfo metaInfo) {
            LOGGER.Warn(iText.IO.LogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.ApplyOtfScript(font, glyphLine, script, configurator, id, metaInfo);
        }

        public override ICollection<UnicodeScript> GetSupportedScripts() {
            LOGGER.Warn(iText.IO.LogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.GetSupportedScripts();
        }

        public override ICollection<UnicodeScript> GetSupportedScripts(Object configurator) {
            LOGGER.Warn(iText.IO.LogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.GetSupportedScripts(configurator);
        }

        public override bool ApplyKerning(FontProgram fontProgram, GlyphLine text, SequenceId sequenceId, IMetaInfo
             metaInfo) {
            LOGGER.Warn(iText.IO.LogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.ApplyKerning(fontProgram, text, sequenceId, metaInfo);
        }

        public override byte[] GetBidiLevels(BaseDirection? baseDirection, int[] unicodeIds, SequenceId sequenceId
            , IMetaInfo metaInfo) {
            LOGGER.Warn(iText.IO.LogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.GetBidiLevels(baseDirection, unicodeIds, sequenceId, metaInfo);
        }

        public override int[] ReorderLine(IList<LineRenderer.RendererGlyph> line, byte[] lineLevels, byte[] levels
            ) {
            LOGGER.Warn(iText.IO.LogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.ReorderLine(line, lineLevels, levels);
        }

        public override IList<int> GetPossibleBreaks(String str) {
            LOGGER.Warn(iText.IO.LogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.GetPossibleBreaks(str);
        }

        public override IDictionary<String, byte[]> LoadShippedFonts() {
            LOGGER.Warn(iText.IO.LogMessageConstant.TYPOGRAPHY_NOT_FOUND);
            return base.LoadShippedFonts();
        }
    }
}
