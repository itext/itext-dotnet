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
