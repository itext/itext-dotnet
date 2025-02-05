/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// A structure to store each part of a SOM name and link it to the next part
    /// beginning from the lower hierarchy.
    /// </summary>
    internal class InverseStore {
        protected internal IList<String> part = new List<String>();

        protected internal IList<Object> follow = new List<Object>();

        /// <summary>
        /// Gets the full name by traversing the hierarchy using only the
        /// index 0.
        /// </summary>
        /// <returns>the full name</returns>
        public virtual String GetDefaultName() {
            InverseStore store = this;
            while (true) {
                Object obj = store.follow[0];
                if (obj is String) {
                    return (String)obj;
                }
                store = (InverseStore)obj;
            }
        }

        /// <summary>Search the current node for a similar name.</summary>
        /// <remarks>
        /// Search the current node for a similar name. A similar name starts
        /// with the same name but has a different index. For example, "detail[3]"
        /// is similar to "detail[9]". The main use is to discard names that
        /// correspond to out of bounds records.
        /// </remarks>
        /// <param name="name">the name to search</param>
        /// <returns><c>true</c> if a similitude was found</returns>
        public virtual bool IsSimilar(String name) {
            int idx = name.IndexOf('[');
            name = name.JSubstring(0, idx + 1);
            for (int k = 0; k < part.Count; ++k) {
                if (part[k].StartsWith(name)) {
                    return true;
                }
            }
            return false;
        }
    }
//\endcond
}
