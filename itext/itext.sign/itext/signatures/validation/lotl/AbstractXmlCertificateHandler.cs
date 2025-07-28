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
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures.Validation.Lotl.Xml;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal abstract class AbstractXmlCertificateHandler : IDefaultXmlHandler {
//\cond DO_NOT_DOCUMENT
        internal readonly IList<IServiceContext> serviceContextList = new List<IServiceContext>();
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IList<IServiceContext> GetServiceContexts() {
            return serviceContextList;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IList<IX509Certificate> GetCertificateList() {
            IList<IX509Certificate> certificateList = new List<IX509Certificate>();
            foreach (IServiceContext context in serviceContextList) {
                certificateList.AddAll(context.GetCertificates());
            }
            return certificateList;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void Clear() {
            serviceContextList.Clear();
        }
//\endcond

        public abstract void Characters(char[] arg1, int arg2, int arg3);

        public abstract void EndElement(String arg1, String arg2, String arg3);

        public abstract void StartElement(String arg1, String arg2, String arg3, Dictionary<String, String> arg4);
    }
//\endcond
}
