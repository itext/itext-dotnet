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
using iText.Commons.Utils;

namespace iText.IO.Font {
    public abstract class FontCacheKey {
        public static FontCacheKey Create(String fontName) {
            return new FontCacheKey.FontCacheStringKey(fontName);
        }

        public static FontCacheKey Create(String fontName, int ttcIndex) {
            return new FontCacheKey.FontCacheTtcKey(fontName, ttcIndex);
        }

        public static FontCacheKey Create(byte[] fontProgram) {
            return new FontCacheKey.FontCacheBytesKey(fontProgram);
        }

        public static FontCacheKey Create(byte[] fontProgram, int ttcIndex) {
            return new FontCacheKey.FontCacheTtcKey(fontProgram, ttcIndex);
        }

        private class FontCacheStringKey : FontCacheKey {
            private String fontName;

            internal FontCacheStringKey(String fontName) {
                this.fontName = fontName;
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                FontCacheKey.FontCacheStringKey that = (FontCacheKey.FontCacheStringKey)o;
                return fontName != null ? fontName.Equals(that.fontName) : that.fontName == null;
            }

            public override int GetHashCode() {
                return fontName != null ? fontName.GetHashCode() : 0;
            }
        }

        private class FontCacheBytesKey : FontCacheKey {
            private byte[] firstFontBytes;

            private int fontLength;

            private int hashcode;

            internal FontCacheBytesKey(byte[] fontBytes) {
                if (fontBytes != null) {
                    int maxBytesNum = 10000;
                    this.firstFontBytes = fontBytes.Length > maxBytesNum ? JavaUtil.ArraysCopyOf(fontBytes, maxBytesNum) : fontBytes;
                    this.fontLength = fontBytes.Length;
                }
                this.hashcode = CalcHashCode();
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                FontCacheKey.FontCacheBytesKey that = (FontCacheKey.FontCacheBytesKey)o;
                if (fontLength != that.fontLength) {
                    return false;
                }
                return JavaUtil.ArraysEquals(firstFontBytes, that.firstFontBytes);
            }

            public override int GetHashCode() {
                return hashcode;
            }

            private int CalcHashCode() {
                int result = JavaUtil.ArraysHashCode(firstFontBytes);
                result = 31 * result + fontLength;
                return result;
            }
        }

        private class FontCacheTtcKey : FontCacheKey {
            private FontCacheKey ttcKey;

            private int ttcIndex;

            internal FontCacheTtcKey(String fontName, int ttcIndex) {
                this.ttcKey = new FontCacheKey.FontCacheStringKey(fontName);
                this.ttcIndex = ttcIndex;
            }

            internal FontCacheTtcKey(byte[] fontBytes, int ttcIndex) {
                this.ttcKey = new FontCacheKey.FontCacheBytesKey(fontBytes);
                this.ttcIndex = ttcIndex;
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o == null || GetType() != o.GetType()) {
                    return false;
                }
                FontCacheKey.FontCacheTtcKey that = (FontCacheKey.FontCacheTtcKey)o;
                if (ttcIndex != that.ttcIndex) {
                    return false;
                }
                return ttcKey.Equals(that.ttcKey);
            }

            public override int GetHashCode() {
                int result = ttcKey.GetHashCode();
                result = 31 * result + ttcIndex;
                return result;
            }
        }
    }
}
