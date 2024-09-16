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

namespace iText.IO.Font.Otf {
    public class ScriptRecord {
        private String tag;

        private LanguageRecord defaultLanguage;

        private LanguageRecord[] languages;

        /// <summary>Retrieves the tag of the Script Record.</summary>
        /// <returns>tag of record</returns>
        public virtual String GetTag() {
            return tag;
        }

        /// <summary>Sets the tag of the Script Record.</summary>
        /// <param name="tag">tag of record</param>
        public virtual void SetTag(String tag) {
            this.tag = tag;
        }

        /// <summary>Retrieves the default language of the Script Record.</summary>
        /// <returns>default language</returns>
        public virtual LanguageRecord GetDefaultLanguage() {
            return defaultLanguage;
        }

        /// <summary>Sets the default language of the Script Record.</summary>
        /// <param name="defaultLanguage">default language</param>
        public virtual void SetDefaultLanguage(LanguageRecord defaultLanguage) {
            this.defaultLanguage = defaultLanguage;
        }

        /// <summary>Retrieves the languages of the Script Record.</summary>
        /// <returns>languages</returns>
        public virtual LanguageRecord[] GetLanguages() {
            return languages;
        }

        /// <summary>Sets the languages of the Script Record.</summary>
        /// <param name="languages">languages</param>
        public virtual void SetLanguages(LanguageRecord[] languages) {
            this.languages = languages;
        }
    }
}
