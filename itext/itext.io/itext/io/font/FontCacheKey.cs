using System;

namespace iText.IO.Font {
    internal abstract class FontCacheKey {
        internal static FontCacheKey Create(String fontName) {
            return new FontCacheKey.FontCacheStringKey(fontName);
        }

        internal static FontCacheKey Create(String fontName, int ttcIndex) {
            return new FontCacheKey.FontCacheTtcKey(fontName, ttcIndex);
        }

        internal static FontCacheKey Create(byte[] fontProgram) {
            return new FontCacheKey.FontCacheBytesKey(fontProgram);
        }

        internal static FontCacheKey Create(byte[] fontProgram, int ttcIndex) {
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
                    this.firstFontBytes = fontBytes.Length > maxBytesNum ? iText.IO.Util.JavaUtil.ArraysCopyOf(fontBytes, maxBytesNum
                        ) : fontBytes;
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
                return iText.IO.Util.JavaUtil.ArraysEquals(firstFontBytes, that.firstFontBytes);
            }

            public override int GetHashCode() {
                return hashcode;
            }

            private int CalcHashCode() {
                int result = iText.IO.Util.JavaUtil.ArraysHashCode(firstFontBytes);
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
