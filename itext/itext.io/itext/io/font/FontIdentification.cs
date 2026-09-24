/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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

namespace iText.IO.Font {
    /// <summary>Holds identification strings extracted from a font program.</summary>
    public class FontIdentification {
        // name ID 5
        private String ttfVersion;

        // name ID 3
        private String ttfUniqueId;

        // /UniqueID
        private int? type1Xuid;

        // OS/2.panose
        private String panose;

        /// <summary>Returns the TrueType version string.</summary>
        /// <returns>
        /// the version string, or
        /// <see langword="null"/>
        /// when unavailable
        /// </returns>
        public virtual String GetTtfVersion() {
            return ttfVersion;
        }

        /// <summary>Returns the TrueType unique identifier.</summary>
        /// <returns>
        /// the unique identifier, or
        /// <see langword="null"/>
        /// when unavailable
        /// </returns>
        public virtual String GetTtfUniqueId() {
            return ttfUniqueId;
        }

        /// <summary>Returns the Type 1 unique ID.</summary>
        /// <returns>
        /// the identifier, or
        /// <see langword="null"/>
        /// when unavailable
        /// </returns>
        public virtual int? GetType1Xuid() {
            return type1Xuid;
        }

        /// <summary>Returns the PANOSE classification.</summary>
        /// <returns>
        /// the classification string, or
        /// <see langword="null"/>
        /// when unavailable
        /// </returns>
        public virtual String GetPanose() {
            return panose;
        }

        /// <summary>Sets the TrueType version.</summary>
        /// <param name="ttfVersion">the version string</param>
        protected internal virtual void SetTtfVersion(String ttfVersion) {
            this.ttfVersion = ttfVersion;
        }

        /// <summary>Sets the TrueType unique identifier.</summary>
        /// <param name="ttfUniqueId">the identifier</param>
        protected internal virtual void SetTtfUniqueId(String ttfUniqueId) {
            this.ttfUniqueId = ttfUniqueId;
        }

        /// <summary>Sets the Type 1 unique ID.</summary>
        /// <param name="type1Xuid">the unique ID</param>
        protected internal virtual void SetType1Xuid(int? type1Xuid) {
            this.type1Xuid = type1Xuid;
        }

        /// <summary>Sets the PANOSE classification from raw bytes.</summary>
        /// <param name="panose">the PANOSE bytes</param>
        protected internal virtual void SetPanose(byte[] panose) {
            this.panose = iText.Commons.Utils.JavaUtil.GetStringForBytes(panose);
        }

        /// <summary>Sets the PANOSE classification string.</summary>
        /// <param name="panose">the classification string</param>
        protected internal virtual void SetPanose(String panose) {
            this.panose = panose;
        }
    }
}
