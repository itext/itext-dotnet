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
using iText.Commons.Actions.Contexts;

namespace iText.Layout.Renderer {
    /// <summary>Class to store metaInfo that will be used for layout renderers.</summary>
    public class MetaInfoContainer {
        private readonly IMetaInfo metaInfo;

        /// <summary>Creates MetaInfoContainer instance with provided meta info.</summary>
        /// <param name="metaInfo">the meta info</param>
        public MetaInfoContainer(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
        }

        /// <summary>Return the IMetaInfo object.</summary>
        /// <returns>returns IMetaInfo</returns>
        internal virtual IMetaInfo GetMetaInfo() {
            return metaInfo;
        }
    }
}
