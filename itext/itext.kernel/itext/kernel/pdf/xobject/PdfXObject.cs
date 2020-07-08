/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using Common.Logging;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Layer;

namespace iText.Kernel.Pdf.Xobject {
    /// <summary>An abstract wrapper for supported types of XObject.</summary>
    /// <seealso cref="PdfFormXObject"/>
    /// <seealso cref="PdfImageXObject"/>
    public abstract class PdfXObject : PdfObjectWrapper<PdfStream> {
        protected internal PdfXObject(PdfStream pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Create
        /// <see cref="PdfFormXObject"/>
        /// or
        /// <see cref="PdfImageXObject"/>
        /// by
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>.
        /// </summary>
        /// <param name="stream">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfStream"/>
        /// with either
        /// <see cref="iText.Kernel.Pdf.PdfName.Form"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName.Image"/>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfName.Subtype"/>
        /// </param>
        /// <returns>
        /// either
        /// <see cref="PdfFormXObject"/>
        /// or
        /// <see cref="PdfImageXObject"/>.
        /// </returns>
        public static iText.Kernel.Pdf.Xobject.PdfXObject MakeXObject(PdfStream stream) {
            if (PdfName.Form.Equals(stream.GetAsName(PdfName.Subtype))) {
                return new PdfFormXObject(stream);
            }
            else {
                if (PdfName.Image.Equals(stream.GetAsName(PdfName.Subtype))) {
                    return new PdfImageXObject(stream);
                }
                else {
                    throw new NotSupportedException(PdfException.UnsupportedXObjectType);
                }
            }
        }

        /// <summary>Sets the layer this XObject belongs to.</summary>
        /// <param name="layer">the layer this XObject belongs to.</param>
        public virtual void SetLayer(IPdfOCG layer) {
            GetPdfObject().Put(PdfName.OC, layer.GetIndirectReference());
        }

        /// <summary>Gets width of XObject.</summary>
        /// <returns>float value.</returns>
        public virtual float GetWidth() {
            throw new NotSupportedException();
        }

        /// <summary>Gets height of XObject.</summary>
        /// <returns>float value.</returns>
        public virtual float GetHeight() {
            throw new NotSupportedException();
        }

        /// <summary>Adds file associated with PDF XObject and identifies the relationship between them.</summary>
        /// <remarks>
        /// Adds file associated with PDF XObject and identifies the relationship between them.
        /// Associated files may be used in Pdf/A-3 and Pdf 2.0 documents.
        /// The method adds file to array value of the AF key in the XObject dictionary.
        /// <para />
        /// For associated files their associated file specification dictionaries shall include the AFRelationship key
        /// </remarks>
        /// <param name="fs">file specification dictionary of associated file</param>
        public virtual void AddAssociatedFile(PdfFileSpec fs) {
            if (null == ((PdfDictionary)fs.GetPdfObject()).Get(PdfName.AFRelationship)) {
                ILog logger = LogManager.GetLogger(typeof(iText.Kernel.Pdf.Xobject.PdfXObject));
                logger.Error(iText.IO.LogMessageConstant.ASSOCIATED_FILE_SPEC_SHALL_INCLUDE_AFRELATIONSHIP);
            }
            PdfArray afArray = GetPdfObject().GetAsArray(PdfName.AF);
            if (afArray == null) {
                afArray = new PdfArray();
                GetPdfObject().Put(PdfName.AF, afArray);
            }
            afArray.Add(fs.GetPdfObject());
        }

        /// <summary>Returns files associated with XObject.</summary>
        /// <param name="create">defines whether AF arrays will be created if it doesn't exist</param>
        /// <returns>associated files array</returns>
        public virtual PdfArray GetAssociatedFiles(bool create) {
            PdfArray afArray = GetPdfObject().GetAsArray(PdfName.AF);
            if (afArray == null && create) {
                afArray = new PdfArray();
                GetPdfObject().Put(PdfName.AF, afArray);
            }
            return afArray;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
