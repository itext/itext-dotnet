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
using System.Collections.Generic;

namespace iText.Forms.Xfa {
    /// <summary>A class to process "classic" fields.</summary>
    internal class AcroFieldsSearch : Xml2Som {
        private IDictionary<String, String> acroShort2LongName;

        /// <summary>Creates a new instance from a Collection with the full names.</summary>
        /// <param name="items">the Collection</param>
        public AcroFieldsSearch(ICollection<String> items) {
            inverseSearch = new Dictionary<String, InverseStore>();
            acroShort2LongName = new Dictionary<String, String>();
            foreach (String itemName in items) {
                String itemShort = GetShortName(itemName);
                acroShort2LongName.Put(itemShort, itemName);
                InverseSearchAdd(inverseSearch, SplitParts(itemShort), itemName);
            }
        }

        /// <summary>Gets the mapping from short names to long names.</summary>
        /// <remarks>
        /// Gets the mapping from short names to long names. A long
        /// name may contain the #subform name part.
        /// </remarks>
        /// <returns>the mapping from short names to long names</returns>
        public virtual IDictionary<String, String> GetAcroShort2LongName() {
            return acroShort2LongName;
        }

        /// <summary>Sets the mapping from short names to long names.</summary>
        /// <remarks>
        /// Sets the mapping from short names to long names. A long
        /// name may contain the #subform name part.
        /// </remarks>
        /// <param name="acroShort2LongName">the mapping from short names to long names</param>
        public virtual void SetAcroShort2LongName(IDictionary<String, String> acroShort2LongName) {
            this.acroShort2LongName = acroShort2LongName;
        }
    }
}
