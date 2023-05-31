using System;
using System.Collections.Generic;

namespace iText.IO.Font.Cmap {
    public sealed class StandardCMapCharsets {
        private static readonly IDictionary<String, CMapCharsetEncoder> encoders = new Dictionary<String, CMapCharsetEncoder
            >();

        private static readonly CMapCharsetEncoder UTF16_ENCODER = new CMapCharsetEncoder(System.Text.Encoding.BigEndianUnicode
            );

        private static readonly CMapCharsetEncoder UCS2_ENCODER = new CMapCharsetEncoder(System.Text.Encoding.BigEndianUnicode
            , true);

        private StandardCMapCharsets() {
        }

        private static void RegisterHV(String cmapPrefix, CMapCharsetEncoder encoder) {
            encoders.Put(cmapPrefix + "-H", encoder);
            encoders.Put(cmapPrefix + "-V", encoder);
        }

        static StandardCMapCharsets() {
            RegisterEncoder();
        }

        private static void RegisterEncoder() {
            // Register encoders for all standard non-identity CMaps in PDF
            // Simplified Chinese
            RegisterHV("UniGB-UCS2", UCS2_ENCODER);
            RegisterHV("UniGB-UTF16", UTF16_ENCODER);
            // Traditional Chinese
            RegisterHV("UniCNS-UCS2", UCS2_ENCODER);
            RegisterHV("UniCNS-UTF16", UTF16_ENCODER);
            // Japanese
            RegisterHV("UniJIS-UCS2", UCS2_ENCODER);
            RegisterHV("UniJIS-UTF16", UTF16_ENCODER);
            // Korean
            RegisterHV("UniKS-UCS2", UCS2_ENCODER);
            RegisterHV("UniKS-UTF16", UTF16_ENCODER);
        }

        public static CMapCharsetEncoder GetEncoder(String stdCmapName) {
            return encoders.Get(stdCmapName);
        }

        /// <summary>Charset encoders are disabled.</summary>
        public static void DisableCharsetEncoders() {
            encoders.Clear();
        }

        /// <summary>Charset encoders are enabled (default).</summary>
        public static void EnableCharsetEncoders() {
            if (encoders.Count == 0) {
                RegisterEncoder();
            }
        }
    }
}
