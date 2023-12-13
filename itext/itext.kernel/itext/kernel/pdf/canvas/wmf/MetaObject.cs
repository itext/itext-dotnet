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
namespace iText.Kernel.Pdf.Canvas.Wmf {
    /// <summary>A meta object.</summary>
    public class MetaObject {
        public const int META_NOT_SUPPORTED = 0;

        public const int META_PEN = 1;

        public const int META_BRUSH = 2;

        public const int META_FONT = 3;

        private int type = META_NOT_SUPPORTED;

        /// <summary>Creates a new MetaObject.</summary>
        /// <remarks>Creates a new MetaObject. This constructor doesn't set the type.</remarks>
        public MetaObject() {
        }

        // Empty body
        /// <summary>Creates a MetaObject with a type.</summary>
        /// <param name="type">the type of meta object</param>
        public MetaObject(int type) {
            this.type = type;
        }

        /// <summary>Get the type of this MetaObject.</summary>
        /// <returns>type of MetaObject</returns>
        public virtual int GetObjectType() {
            return type;
        }
    }
}
