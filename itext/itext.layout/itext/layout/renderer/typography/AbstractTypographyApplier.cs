using System;
using System.Collections.Generic;
using iText.Commons.Actions;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Layout.Renderer.Typography {
    public abstract class AbstractTypographyApplier : AbstractITextEvent {
        protected internal AbstractTypographyApplier() {
        }

        // do nothing
        public abstract bool IsPdfCalligraphInstance();

        public virtual ICollection<UnicodeScript> GetSupportedScripts() {
            return null;
        }

        public virtual ICollection<UnicodeScript> GetSupportedScripts(Object configurator) {
            return null;
        }

        public virtual bool ApplyOtfScript(TrueTypeFont font, GlyphLine glyphLine, UnicodeScript? script, Object configurator
            , SequenceId id, IMetaInfo metaInfo) {
            return false;
        }

        public virtual bool ApplyKerning(FontProgram fontProgram, GlyphLine text, SequenceId sequenceId, IMetaInfo
             metaInfo) {
            return false;
        }

        public virtual byte[] GetBidiLevels(BaseDirection? baseDirection, int[] unicodeIds, SequenceId sequenceId, 
            IMetaInfo metaInfo) {
            return null;
        }

        public virtual int[] ReorderLine(IList<LineRenderer.RendererGlyph> line, byte[] lineLevels, byte[] levels) {
            return null;
        }

        public virtual IList<int> GetPossibleBreaks(String str) {
            return null;
        }

        public virtual IDictionary<String, byte[]> LoadShippedFonts() {
            return null;
        }
    }
}
