using System;
using System.Collections.Generic;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal class AdditionalServiceInformationExtension {
        private static readonly ICollection<String> invalidScopes = new HashSet<String>();

        static AdditionalServiceInformationExtension() {
            invalidScopes.Add("http://uri.etsi.org/TrstSvc/TrustedList/SvcInfoExt/ForWebSiteAuthentication");
        }

        private String uri;

//\cond DO_NOT_DOCUMENT
        internal AdditionalServiceInformationExtension() {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        // Empty constructor.
        internal AdditionalServiceInformationExtension(String uri) {
            this.uri = uri;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual String GetUri() {
            return uri;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetUri(String uri) {
            this.uri = uri;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsScopeValid() {
            return !invalidScopes.Contains(uri);
        }
//\endcond
    }
//\endcond
}
