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
using Common.Logging;
using iText.IO.Util;

namespace iText.Kernel.Pdf {
    public abstract class PdfPrimitiveObject : PdfObject {
        protected internal byte[] content = null;

        protected internal bool directOnly;

        protected internal PdfPrimitiveObject()
            : base() {
        }

        protected internal PdfPrimitiveObject(bool directOnly)
            : base() {
            this.directOnly = directOnly;
        }

        /// <summary>Initialize PdfPrimitiveObject from the passed bytes.</summary>
        /// <param name="content">byte content, shall not be null.</param>
        protected internal PdfPrimitiveObject(byte[] content)
            : this() {
            System.Diagnostics.Debug.Assert(content != null);
            this.content = content;
        }

        protected internal byte[] GetInternalContent() {
            if (content == null) {
                GenerateContent();
            }
            return content;
        }

        protected internal virtual bool HasContent() {
            return content != null;
        }

        protected internal abstract void GenerateContent();

        public override PdfObject MakeIndirect(PdfDocument document, PdfIndirectReference reference) {
            if (!directOnly) {
                return base.MakeIndirect(document, reference);
            }
            else {
                ILog logger = LogManager.GetLogger(typeof(PdfObject));
                logger.Warn(iText.IO.LogMessageConstant.DIRECTONLY_OBJECT_CANNOT_BE_INDIRECT);
            }
            return this;
        }

        protected internal override PdfObject SetIndirectReference(PdfIndirectReference indirectReference) {
            if (!directOnly) {
                base.SetIndirectReference(indirectReference);
            }
            else {
                ILog logger = LogManager.GetLogger(typeof(PdfObject));
                logger.Warn(iText.IO.LogMessageConstant.DIRECTONLY_OBJECT_CANNOT_BE_INDIRECT);
            }
            return this;
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document) {
            base.CopyContent(from, document);
            iText.Kernel.Pdf.PdfPrimitiveObject @object = (iText.Kernel.Pdf.PdfPrimitiveObject)from;
            if (@object.content != null) {
                content = JavaUtil.ArraysCopyOf(@object.content, @object.content.Length);
            }
        }

        protected internal virtual int CompareContent(iText.Kernel.Pdf.PdfPrimitiveObject o) {
            for (int i = 0; i < Math.Min(content.Length, o.content.Length); i++) {
                if (content[i] > o.content[i]) {
                    return 1;
                }
                if (content[i] < o.content[i]) {
                    return -1;
                }
            }
            return JavaUtil.IntegerCompare(content.Length, o.content.Length);
        }
    }
}
