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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Xobject {
    public class PdfTransparencyGroup : PdfObjectWrapper<PdfDictionary> {
        public PdfTransparencyGroup()
            : base(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.S, PdfName.Transparency);
        }

        /// <summary>Determining the initial backdrop against which its stack is composited.</summary>
        /// <param name="isolated">
        /// defines whether the
        /// <see cref="iText.Kernel.Pdf.PdfName.I"/>
        /// flag will be set or removed
        /// </param>
        public virtual void SetIsolated(bool isolated) {
            if (isolated) {
                GetPdfObject().Put(PdfName.I, PdfBoolean.TRUE);
            }
            else {
                GetPdfObject().Remove(PdfName.I);
            }
        }

        /// <summary>Determining whether the objects within the stack are composited with one another or only with the group's backdrop.
        ///     </summary>
        /// <param name="knockout">
        /// defines whether the
        /// <see cref="iText.Kernel.Pdf.PdfName.K"/>
        /// flag will be set or removed
        /// </param>
        public virtual void SetKnockout(bool knockout) {
            if (knockout) {
                GetPdfObject().Put(PdfName.K, PdfBoolean.TRUE);
            }
            else {
                GetPdfObject().Remove(PdfName.K);
            }
        }

        public virtual void SetColorSpace(PdfName colorSpace) {
            GetPdfObject().Put(PdfName.CS, colorSpace);
        }

        public virtual void SetColorSpace(PdfArray colorSpace) {
            GetPdfObject().Put(PdfName.CS, colorSpace);
        }

        public virtual iText.Kernel.Pdf.Xobject.PdfTransparencyGroup Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
