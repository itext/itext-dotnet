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
            RegisterHV("UniJIS-UCS2-HW", UCS2_ENCODER);
            RegisterHV("UniJIS2004-UTF16", UTF16_ENCODER);
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
