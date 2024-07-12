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

namespace iText.Barcodes.Qrcode {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Encapsulates a Character Set ECI, according to "Extended Channel Interpretations" 5.3.1.1
    /// of ISO 18004.
    /// </summary>
    internal sealed class CharacterSetECI {
        private static IDictionary<String, iText.Barcodes.Qrcode.CharacterSetECI> NAME_TO_ECI;

        private static void Initialize() {
            IDictionary<String, iText.Barcodes.Qrcode.CharacterSetECI> n = new Dictionary<String, iText.Barcodes.Qrcode.CharacterSetECI
                >(29);
            AddCharacterSet(0, "Cp437", n);
            AddCharacterSet(1, new String[] { "ISO8859_1", "ISO-8859-1" }, n);
            AddCharacterSet(2, "Cp437", n);
            AddCharacterSet(3, new String[] { "ISO8859_1", "ISO-8859-1" }, n);
            AddCharacterSet(4, new String[] { "ISO8859_2", "ISO-8859-2" }, n);
            AddCharacterSet(5, new String[] { "ISO8859_3", "ISO-8859-3" }, n);
            AddCharacterSet(6, new String[] { "ISO8859_4", "ISO-8859-4" }, n);
            AddCharacterSet(7, new String[] { "ISO8859_5", "ISO-8859-5" }, n);
            AddCharacterSet(8, new String[] { "ISO8859_6", "ISO-8859-6" }, n);
            AddCharacterSet(9, new String[] { "ISO8859_7", "ISO-8859-7" }, n);
            AddCharacterSet(10, new String[] { "ISO8859_8", "ISO-8859-8" }, n);
            AddCharacterSet(11, new String[] { "ISO8859_9", "ISO-8859-9" }, n);
            AddCharacterSet(12, new String[] { "ISO8859_10", "ISO-8859-10" }, n);
            AddCharacterSet(13, new String[] { "ISO8859_11", "ISO-8859-11" }, n);
            AddCharacterSet(15, new String[] { "ISO8859_13", "ISO-8859-13" }, n);
            AddCharacterSet(16, new String[] { "ISO8859_14", "ISO-8859-14" }, n);
            AddCharacterSet(17, new String[] { "ISO8859_15", "ISO-8859-15" }, n);
            AddCharacterSet(18, new String[] { "ISO8859_16", "ISO-8859-16" }, n);
            AddCharacterSet(20, new String[] { "SJIS", "Shift_JIS" }, n);
            NAME_TO_ECI = n;
        }

        private readonly String encodingName;

        private readonly int value;

        private CharacterSetECI(int value, String encodingName) {
            this.encodingName = encodingName;
            this.value = value;
        }

        /// <returns>name of the encoding.</returns>
        public String GetEncodingName() {
            return encodingName;
        }

        /// <returns>the value of the encoding.</returns>
        public int GetValue() {
            return value;
        }

        private static void AddCharacterSet(int value, String encodingName, IDictionary<String, iText.Barcodes.Qrcode.CharacterSetECI
            > n) {
            iText.Barcodes.Qrcode.CharacterSetECI eci = new iText.Barcodes.Qrcode.CharacterSetECI(value, encodingName);
            n.Put(encodingName, eci);
        }

        private static void AddCharacterSet(int value, String[] encodingNames, IDictionary<String, iText.Barcodes.Qrcode.CharacterSetECI
            > n) {
            iText.Barcodes.Qrcode.CharacterSetECI eci = new iText.Barcodes.Qrcode.CharacterSetECI(value, encodingNames
                [0]);
            for (int i = 0; i < encodingNames.Length; i++) {
                n.Put(encodingNames[i], eci);
            }
        }

        /// <param name="name">character set ECI encoding name</param>
        /// <returns>
        /// 
        /// <see cref="CharacterSetECI"/>
        /// representing ECI for character encoding, or null if it is legal
        /// but unsupported
        /// </returns>
        public static iText.Barcodes.Qrcode.CharacterSetECI GetCharacterSetECIByName(String name) {
            if (NAME_TO_ECI == null) {
                Initialize();
            }
            return NAME_TO_ECI.Get(name);
        }
    }
//\endcond
}
