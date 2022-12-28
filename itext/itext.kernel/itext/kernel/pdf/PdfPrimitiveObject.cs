/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Utils;

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
                ILogger logger = ITextLogManager.GetLogger(typeof(PdfObject));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.DIRECTONLY_OBJECT_CANNOT_BE_INDIRECT);
            }
            return this;
        }

        protected internal override PdfObject SetIndirectReference(PdfIndirectReference indirectReference) {
            if (!directOnly) {
                base.SetIndirectReference(indirectReference);
            }
            else {
                ILogger logger = ITextLogManager.GetLogger(typeof(PdfObject));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.DIRECTONLY_OBJECT_CANNOT_BE_INDIRECT);
            }
            return this;
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document, ICopyFilter copyFilter) {
            base.CopyContent(from, document, copyFilter);
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
