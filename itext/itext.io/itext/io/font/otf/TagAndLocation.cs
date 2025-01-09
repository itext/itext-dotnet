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

namespace iText.IO.Font.Otf {
    public class TagAndLocation {
        private String tag;

        private int location;

        /// <summary>Retrieves the tag of the object.</summary>
        /// <returns>tag</returns>
        public virtual String GetTag() {
            return tag;
        }

        /// <summary>Sets the tag of the object.</summary>
        /// <param name="tag">tag</param>
        public virtual void SetTag(String tag) {
            this.tag = tag;
        }

        /// <summary>Retrieves the location of the object.</summary>
        /// <returns>location</returns>
        public virtual int GetLocation() {
            return location;
        }

        /// <summary>Sets the location of the object.</summary>
        /// <param name="location">location</param>
        public virtual void SetLocation(int location) {
            this.location = location;
        }
    }
}
