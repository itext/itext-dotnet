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
using System.IO;
using iText.StyledXmlParser.Resolver.Resource;

namespace iText.Signatures.Validation.Mocks {
    public class MockResourceRetriever : IResourceRetriever {
        private Func<Uri, byte[]> getByteArrayByUrlHandler = (u) => null;

        private Func<Uri, Stream> getInputStreamByUrlHandler = (u) => null;

        public virtual Stream GetInputStreamByUrl(Uri url) {
            return getInputStreamByUrlHandler.Invoke(url);
        }

        public virtual byte[] GetByteArrayByUrl(Uri url) {
            return getByteArrayByUrlHandler.Invoke(url);
        }

        public virtual MockResourceRetriever OnGetInputStreamByUrl(Func<Uri, Stream> handler) {
            getInputStreamByUrlHandler = handler;
            return this;
        }

        public virtual MockResourceRetriever OnGetByteArrayByUrl(Func<Uri, byte[]> handler) {
            getByteArrayByUrlHandler = handler;
            return this;
        }
    }
}
