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
using System.Text;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Security;
using iText.IO.Resolver.Resource;
using iText.Kernel.Crypto;

namespace iText.Signatures.Validation.Lotl {
    public class FromDiskResourceRetriever : IResourceRetriever {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private readonly String resourcePath;

        public FromDiskResourceRetriever(String resourcePath) {
            this.resourcePath = resourcePath;
        }

        public virtual Stream GetInputStreamByUrl(Uri url) {
            throw new NotSupportedException("Not implemented yet");
        }

        public virtual byte[] GetByteArrayByUrl(Uri url) {
            //escape url so it can be used as a complete filename
            String urlString = url.ToString();
            urlString = iText.Commons.Utils.StringUtil.ReplaceAll(urlString, " ", "%20");
            String fileName = iText.Commons.Utils.StringUtil.ReplaceAll(urlString.ToString(), "[^a-zA-Z0-9]", "_");
            String fileNameHash = CreateHash(fileName);
            String filePath = resourcePath + fileNameHash;
            if (File.Exists(System.IO.Path.Combine(filePath))) {
                return File.ReadAllBytes(System.IO.Path.Combine(filePath));
            }
            return null;
        }

        private static String CreateHash(String input) {
            try {
                MemoryStream bais = new MemoryStream(input.GetBytes(System.Text.Encoding.UTF8));
                byte[] hash = DigestAlgorithms.Digest(bais, DigestAlgorithms.SHA256);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hash) {
                    char c = (char)('a' + (b & 0x0F) % 26);
                    stringBuilder.Append(c);
                }
                return stringBuilder.ToString();
            }
            catch (AbstractGeneralSecurityException e) {
                throw new Exception(e.Message);
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message);
            }
        }
    }
}
