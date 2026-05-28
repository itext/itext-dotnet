using System.Collections.Generic;
using System.ComponentModel;
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
