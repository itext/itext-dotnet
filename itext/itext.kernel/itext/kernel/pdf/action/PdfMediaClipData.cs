/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
