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
    public class LanguageRecord {
        private String tag;

        private int featureRequired;

        private int[] features;

        /// <summary>Retrieves the tag of the language record.</summary>
        /// <returns>tag of record</returns>
        public virtual String GetTag() {
            return tag;
        }

        /// <summary>Sets the tag of the language record.</summary>
        /// <param name="tag">tag of record</param>
        public virtual void SetTag(String tag) {
            this.tag = tag;
        }

        /// <summary>Retrieves the feature required of the language record.</summary>
        /// <returns>feature required</returns>
        public virtual int GetFeatureRequired() {
            return featureRequired;
        }

        /// <summary>Sets the feature required of the language record.</summary>
        /// <param name="featureRequired">feature required</param>
        public virtual void SetFeatureRequired(int featureRequired) {
            this.featureRequired = featureRequired;
        }

        /// <summary>Retrieves the features of the language record.</summary>
        /// <returns>features</returns>
        public virtual int[] GetFeatures() {
            return features;
        }

        /// <summary>Sets the features of the language record.</summary>
        /// <param name="features">features</param>
        public virtual void SetFeatures(int[] features) {
            this.features = features;
        }
    }
}
