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
using iText.Kernel.Counter.Event;

namespace iText.Kernel.Pdf {
    public class DocumentProperties {
        protected internal IMetaInfo metaInfo = null;

        public DocumentProperties() {
        }

        public DocumentProperties(iText.Kernel.Pdf.DocumentProperties other) {
            this.metaInfo = other.metaInfo;
        }

        /// <summary>Sets document meta info.</summary>
        /// <remarks>
        /// Sets document meta info. This meta info will be passed to the
        /// <see cref="iText.Kernel.Counter.EventCounter"/>
        /// with
        /// <see cref="iText.Kernel.Counter.Event.CoreEvent"/>
        /// and can be used to determine event origin.
        /// </remarks>
        /// <param name="metaInfo">meta info to set</param>
        /// <returns>
        /// this
        /// <see cref="DocumentProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.DocumentProperties SetEventCountingMetaInfo(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
            return this;
        }
    }
}
