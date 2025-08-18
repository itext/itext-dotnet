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
using System.IO;
using iText.IO.Resolver.Resource;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal class FromDiskResourceRetriever : IResourceRetriever {
        private readonly String resourcePath;

//\cond DO_NOT_DOCUMENT
        internal FromDiskResourceRetriever(String resourcePath) {
            this.resourcePath = resourcePath;
        }
//\endcond

        public virtual Stream GetInputStreamByUrl(Uri url) {
            throw new NotSupportedException("Not implemented yet");
        }

        public virtual byte[] GetByteArrayByUrl(Uri url) {
            System.Console.Out.WriteLine(url);
            //escape url so it can be used as a complete filename
            String fileName = iText.Commons.Utils.StringUtil.ReplaceAll(url.ToString(), "[^a-zA-Z0-9]", "_");
            String filePath = resourcePath + fileName;
            String path = System.IO.Path.Combine(filePath);
            if (File.Exists(path)) {
                return File.ReadAllBytes(path);
            }
            return null;
        }
    }
//\endcond
}
