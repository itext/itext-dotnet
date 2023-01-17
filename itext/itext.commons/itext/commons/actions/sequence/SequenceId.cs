/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.Commons.Utils;

namespace iText.Commons.Actions.Sequence {
    /// <summary>The class represents unique numeric identifier with autoincrement strategy of generation.</summary>
    public sealed class SequenceId {
        private static readonly AtomicLong ID_GENERATOR = new AtomicLong();

        private readonly long id;

        /// <summary>Creates a new instance of identifier.</summary>
        public SequenceId() {
            this.id = ID_GENERATOR.IncrementAndGet();
        }

        /// <summary>Obtains an id.</summary>
        /// <returns>id</returns>
        public long GetId() {
            return id;
        }
    }
}
