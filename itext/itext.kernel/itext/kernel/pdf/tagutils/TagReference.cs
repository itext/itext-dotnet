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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    public class TagReference {
        protected internal TagTreePointer tagPointer;

        protected internal int insertIndex;

        protected internal PdfStructElem referencedTag;

        protected internal PdfName role;

        protected internal PdfDictionary properties;

        protected internal TagReference(PdfStructElem referencedTag, TagTreePointer tagPointer, int insertIndex) {
            this.role = referencedTag.GetRole();
            this.referencedTag = referencedTag;
            this.tagPointer = tagPointer;
            this.insertIndex = insertIndex;
        }

        public virtual PdfName GetRole() {
            return role;
        }

        public virtual int CreateNextMcid() {
            return tagPointer.CreateNextMcidForStructElem(referencedTag, insertIndex);
        }

        public virtual iText.Kernel.Pdf.Tagutils.TagReference AddProperty(PdfName name, PdfObject value) {
            if (properties == null) {
                properties = new PdfDictionary();
            }
            properties.Put(name, value);
            return this;
        }

        public virtual iText.Kernel.Pdf.Tagutils.TagReference RemoveProperty(PdfName name) {
            if (properties != null) {
                properties.Remove(name);
            }
            return this;
        }

        public virtual PdfObject GetProperty(PdfName name) {
            if (properties == null) {
                return null;
            }
            return properties.Get(name);
        }

        public virtual PdfDictionary GetProperties() {
            return properties;
        }
    }
}
