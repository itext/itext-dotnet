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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;

namespace iText.Kernel.Pdf.Action {
    /// <summary>This class is a wrapper of media clip data dictionary that defines the data for a media object that can be played.
    ///     </summary>
    public class PdfMediaClipData : PdfObjectWrapper<PdfDictionary> {
        private static readonly PdfString TEMPACCESS = new PdfString("TEMPACCESS");

        /// <summary>
        /// Constructs a new
        /// <see cref="PdfMediaClipData"/>
        /// wrapper using an existing dictionary.
        /// </summary>
        /// <param name="pdfObject">the dictionary to construct the wrapper from</param>
        public PdfMediaClipData(PdfDictionary pdfObject)
            : base(pdfObject) {
        }

        /// <summary>
        /// Constructs a new
        /// <see cref="PdfMediaClipData"/>
        /// wrapper around a newly created dictionary.
        /// </summary>
        /// <param name="file">the name of the file to create a media clip for</param>
        /// <param name="fs">a file specification that specifies the actual media data</param>
        /// <param name="mimeType">an ASCII string identifying the type of data</param>
        public PdfMediaClipData(String file, PdfFileSpec fs, String mimeType)
            : this(new PdfDictionary()) {
            PdfDictionary dic = new PdfDictionary();
            MarkObjectAsIndirect(dic);
            dic.Put(PdfName.TF, TEMPACCESS);
            GetPdfObject().Put(PdfName.Type, PdfName.MediaClip);
            GetPdfObject().Put(PdfName.S, PdfName.MCD);
            GetPdfObject().Put(PdfName.N, new PdfString(MessageFormatUtil.Format("Media clip for {0}", file)));
            GetPdfObject().Put(PdfName.CT, new PdfString(mimeType));
            GetPdfObject().Put(PdfName.P, dic);
            GetPdfObject().Put(PdfName.D, fs.GetPdfObject());
        }

        /// <summary>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// </summary>
        /// <remarks>
        /// To manually flush a
        /// <c>PdfObject</c>
        /// behind this wrapper, you have to ensure
        /// that this object is added to the document, i.e. it has an indirect reference.
        /// Basically this means that before flushing you need to explicitly call
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.MakeIndirect(iText.Kernel.Pdf.PdfDocument)"/>.
        /// For example: wrapperInstance.makeIndirect(document).flush();
        /// Note that not every wrapper require this, only those that have such warning in documentation.
        /// </remarks>
        public override void Flush() {
            base.Flush();
        }

        /// <summary><inheritDoc/></summary>
        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
