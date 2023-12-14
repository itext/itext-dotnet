/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.StyledXmlParser.Jsoup {
    /// <summary>Signals that a HTTP response returned a mime type that is not supported.</summary>
    public class UnsupportedMimeTypeException : System.IO.IOException {
        private String mimeType;

        private String url;

        public UnsupportedMimeTypeException(String message, String mimeType, String url)
            : base(message) {
            this.mimeType = mimeType;
            this.url = url;
        }

        public virtual String GetMimeType() {
            return mimeType;
        }

        public virtual String GetUrl() {
            return url;
        }

        public override String ToString() {
            return base.ToString() + ". Mimetype=" + mimeType + ", URL=" + url;
        }
    }
}
