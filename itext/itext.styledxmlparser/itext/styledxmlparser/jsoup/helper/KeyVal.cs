/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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

namespace iText.StyledXmlParser.Jsoup.Helper {
    public class KeyVal {
        private String key;

        private String value;

        private Stream stream;

        private String contentType;

        public static iText.StyledXmlParser.Jsoup.Helper.KeyVal Create(String key, String value) {
            return new iText.StyledXmlParser.Jsoup.Helper.KeyVal(key, value);
        }

        public static iText.StyledXmlParser.Jsoup.Helper.KeyVal Create(String key, String filename, Stream stream) {
            return new iText.StyledXmlParser.Jsoup.Helper.KeyVal(key, filename).InputStream(stream);
        }

        private KeyVal(String key, String value) {
            Validate.NotEmpty(key, "Data key must not be empty");
            Validate.NotNull(value, "Data value must not be null");
            this.key = key;
            this.value = value;
        }

        public virtual iText.StyledXmlParser.Jsoup.Helper.KeyVal Key(String key) {
            Validate.NotEmpty(key, "Data key must not be empty");
            this.key = key;
            return this;
        }

        public virtual String Key() {
            return key;
        }

        public virtual iText.StyledXmlParser.Jsoup.Helper.KeyVal Value(String value) {
            Validate.NotNull(value, "Data value must not be null");
            this.value = value;
            return this;
        }

        public virtual String Value() {
            return value;
        }

        public virtual iText.StyledXmlParser.Jsoup.Helper.KeyVal InputStream(Stream inputStream) {
            Validate.NotNull(value, "Data input stream must not be null");
            this.stream = inputStream;
            return this;
        }

        public virtual Stream InputStream() {
            return stream;
        }

        public virtual bool HasInputStream() {
            return stream != null;
        }

        public virtual iText.StyledXmlParser.Jsoup.Helper.KeyVal ContentType(String contentType) {
            Validate.NotEmpty(contentType);
            this.contentType = contentType;
            return this;
        }

        public virtual String ContentType() {
            return contentType;
        }

        public override String ToString() {
            return key + "=" + value;
        }
    }
}
