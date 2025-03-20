/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
