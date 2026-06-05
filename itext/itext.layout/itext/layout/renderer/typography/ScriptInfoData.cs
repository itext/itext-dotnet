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
using System.Collections.Generic;
using System.ComponentModel;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;

namespace iText.Layout.Renderer.Typography {
//\cond DO_NOT_DOCUMENT
    internal class ScriptInfoData {
        private readonly IDictionary<UnicodeScript, ScriptRequirements> SCRIPT_REQ_FEATURE;

        private readonly ICollection<UnicodeScript> SUPPORTED_SCRIPTS;

//\cond DO_NOT_DOCUMENT
        internal ScriptInfoData(int initialSize) {
            this.SCRIPT_REQ_FEATURE = new Dictionary<UnicodeScript, ScriptRequirements>(initialSize);
            this.SUPPORTED_SCRIPTS = new HashSet<UnicodeScript>();
        }
//\endcond

        public virtual ICollection<UnicodeScript> GetSupportedScripts() {
            return JavaCollectionsUtil.UnmodifiableCollection<UnicodeScript>(this.SUPPORTED_SCRIPTS);
        }

//\cond DO_NOT_DOCUMENT
        internal virtual ScriptRequirements Get(UnicodeScript? script) {
            return SCRIPT_REQ_FEATURE.Get(script.Value);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool ScriptSupported(UnicodeScript? script) {
            return SUPPORTED_SCRIPTS.Contains(script.Value);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddRequirements(UnicodeScript? script, ScriptRequirements requirements) {
            SCRIPT_REQ_FEATURE.Put(script.Value, requirements);
            if (requirements.IsSupported()) {
                SUPPORTED_SCRIPTS.Add(script.Value);
            }
        }
//\endcond
    }
//\endcond
}
