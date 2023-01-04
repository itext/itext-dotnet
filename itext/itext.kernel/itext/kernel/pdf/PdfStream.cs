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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    /// <summary>Representation of a stream as described in the PDF Specification.</summary>
    public class PdfStream : PdfDictionary {
        protected internal int compressionLevel;

        // Output stream associated with PDF stream.
        protected internal PdfOutputStream outputStream;

        private Stream inputStream;

        private long offset;

        private int length = -1;

        /// <summary>
        /// Constructs a
        /// <c>PdfStream</c>
        /// -object.
        /// </summary>
        /// <param name="bytes">
        /// initial content of
        /// <see cref="PdfOutputStream"/>.
        /// </param>
        /// <param name="compressionLevel">the compression level (0 = best speed, 9 = best compression, -1 is default)
        ///     </param>
        public PdfStream(byte[] bytes, int compressionLevel)
            : base() {
            SetState(MUST_BE_INDIRECT);
            this.compressionLevel = compressionLevel;
            if (bytes != null && bytes.Length > 0) {
                this.outputStream = new PdfOutputStream(new ByteArrayOutputStream(bytes.Length));
                this.outputStream.WriteBytes(bytes);
            }
            else {
                this.outputStream = new PdfOutputStream(new ByteArrayOutputStream());
            }
        }

        /// <summary>Creates a PdfStream instance.</summary>
        /// <param name="bytes">bytes to write to the PdfStream</param>
        public PdfStream(byte[] bytes)
            : this(bytes, CompressionConstants.UNDEFINED_COMPRESSION) {
        }

        /// <summary>Creates an efficient stream.</summary>
        /// <remarks>
        /// Creates an efficient stream. No temporary array is ever created. The
        /// <c>InputStream</c>
        /// is totally consumed but is not closed. The general usage is:
        /// <br />
        /// <pre>
        /// PdfDocument document = ?;
        /// InputStream in = ?;
        /// PdfStream stream = new PdfStream(document, in, PdfOutputStream.DEFAULT_COMPRESSION);
        /// ?
        /// stream.flush();
        /// in.close();
        /// </pre>
        /// </remarks>
        /// <param name="doc">
        /// the
        /// <see cref="PdfDocument">pdf document</see>
        /// in which this stream lies
        /// </param>
        /// <param name="inputStream">the data to write to this stream</param>
        /// <param name="compressionLevel">the compression level (0 = best speed, 9 = best compression, -1 is default)
        ///     </param>
        public PdfStream(PdfDocument doc, Stream inputStream, int compressionLevel)
            : base() {
            if (doc == null) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_CREATE_PDFSTREAM_BY_INPUT_STREAM_WITHOUT_PDF_DOCUMENT
                    );
            }
            MakeIndirect(doc);
            if (inputStream == null) {
                throw new ArgumentException("The input stream in PdfStream constructor can not be null.");
            }
            this.inputStream = inputStream;
            this.compressionLevel = compressionLevel;
            Put(PdfName.Length, new PdfNumber(-1).MakeIndirect(doc));
        }

        /// <summary>Creates an efficient stream.</summary>
        /// <remarks>
        /// Creates an efficient stream. No temporary array is ever created. The
        /// <c>InputStream</c>
        /// is totally consumed but is not closed. The general usage is:
        /// <br />
        /// <pre>
        /// PdfDocument document = ?;
        /// InputStream in = ?;
        /// PdfStream stream = new PdfStream(document, in);
        /// stream.flush();
        /// in.close();
        /// </pre>
        /// </remarks>
        /// <param name="doc">
        /// the
        /// <see cref="PdfDocument">pdf document</see>
        /// in which this stream lies
        /// </param>
        /// <param name="inputStream">the data to write to this stream</param>
        public PdfStream(PdfDocument doc, Stream inputStream)
            : this(doc, inputStream, CompressionConstants.UNDEFINED_COMPRESSION) {
        }

        /// <summary>
        /// Constructs a
        /// <c>PdfStream</c>
        /// -object.
        /// </summary>
        /// <param name="compressionLevel">the compression level (0 = best speed, 9 = best compression, -1 is default)
        ///     </param>
        public PdfStream(int compressionLevel)
            : this(null, compressionLevel) {
        }

        /// <summary>Creates an empty PdfStream instance.</summary>
        public PdfStream()
            : this((byte[])null) {
        }

        protected internal PdfStream(Stream outputStream) {
            this.outputStream = new PdfOutputStream(outputStream);
            this.compressionLevel = CompressionConstants.UNDEFINED_COMPRESSION;
            SetState(MUST_BE_INDIRECT);
        }

        //NOTE This constructor only for PdfReader.
        internal PdfStream(long offset, PdfDictionary keys)
            : base() {
            this.compressionLevel = CompressionConstants.UNDEFINED_COMPRESSION;
            this.offset = offset;
            PutAll(keys);
            PdfNumber length = GetAsNumber(PdfName.Length);
            if (length == null) {
                this.length = 0;
            }
            else {
                this.length = length.IntValue();
            }
        }

        /// <summary>Gets output stream.</summary>
        /// <returns>output stream</returns>
        public virtual PdfOutputStream GetOutputStream() {
            return outputStream;
        }

        /// <summary>Gets compression level of this PdfStream.</summary>
        /// <remarks>
        /// Gets compression level of this PdfStream.
        /// For more details @see
        /// <see cref="iText.IO.Source.DeflaterOutputStream"/>.
        /// </remarks>
        /// <returns>compression level.</returns>
        public virtual int GetCompressionLevel() {
            return compressionLevel;
        }

        /// <summary>Sets compression level of this PdfStream.</summary>
        /// <remarks>
        /// Sets compression level of this PdfStream.
        /// For more details @see
        /// <see cref="iText.IO.Source.DeflaterOutputStream"/>.
        /// </remarks>
        /// <param name="compressionLevel">the compression level (0 = best speed, 9 = best compression, -1 is default)
        ///     </param>
        public virtual void SetCompressionLevel(int compressionLevel) {
            this.compressionLevel = compressionLevel;
        }

        public override byte GetObjectType() {
            return STREAM;
        }

        public virtual int GetLength() {
            return length;
        }

        /// <summary>Gets decoded stream bytes.</summary>
        /// <remarks>
        /// Gets decoded stream bytes.
        /// Note,
        /// <see cref="PdfName.DCTDecode"/>
        /// and
        /// <see cref="PdfName.JPXDecode"/>
        /// filters will be ignored.
        /// </remarks>
        /// <returns>
        /// byte content of the
        /// <c>PdfStream</c>
        /// . Byte content will be
        /// <see langword="null"/>
        /// ,
        /// if the
        /// <c>PdfStream</c>
        /// was created by
        /// <c>InputStream</c>.
        /// </returns>
        public virtual byte[] GetBytes() {
            return GetBytes(true);
        }

        /// <summary>Gets stream bytes.</summary>
        /// <remarks>
        /// Gets stream bytes.
        /// Note,
        /// <see cref="PdfName.DCTDecode"/>
        /// and
        /// <see cref="PdfName.JPXDecode"/>
        /// filters will be ignored.
        /// </remarks>
        /// <param name="decoded">true if to get decoded stream bytes, otherwise false.</param>
        /// <returns>
        /// byte content of the
        /// <c>PdfStream</c>
        /// . Byte content will be
        /// <see langword="null"/>
        /// ,
        /// if the
        /// <c>PdfStream</c>
        /// was created by
        /// <c>InputStream</c>.
        /// </returns>
        public virtual byte[] GetBytes(bool decoded) {
            if (IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_OPERATE_WITH_FLUSHED_PDF_STREAM);
            }
            if (inputStream != null) {
                ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfStream)).LogWarning("PdfStream was created by InputStream."
                     + "getBytes() always returns null in this case");
                return null;
            }
            byte[] bytes = null;
            if (outputStream != null && outputStream.GetOutputStream() != null) {
                System.Diagnostics.Debug.Assert(outputStream.GetOutputStream() is ByteArrayOutputStream, "Invalid OutputStream: ByteArrayByteArrayOutputStream expected"
                    );
                try {
                    outputStream.GetOutputStream().Flush();
                    bytes = ((ByteArrayOutputStream)outputStream.GetOutputStream()).ToArray();
                    if (decoded && ContainsKey(PdfName.Filter)) {
                        bytes = PdfReader.DecodeBytes(bytes, this);
                    }
                }
                catch (System.IO.IOException ioe) {
                    throw new PdfException(KernelExceptionMessageConstant.CANNOT_GET_PDF_STREAM_BYTES, ioe, this);
                }
            }
            else {
                if (GetIndirectReference() != null) {
                    // This logic makes sense only for the case when PdfStream was created by reader and in this
                    // case PdfStream instance always has indirect reference and is never in the MustBeIndirect state
                    PdfReader reader = GetIndirectReference().GetReader();
                    if (reader != null) {
                        try {
                            bytes = reader.ReadStreamBytes(this, decoded);
                        }
                        catch (System.IO.IOException ioe) {
                            throw new PdfException(KernelExceptionMessageConstant.CANNOT_GET_PDF_STREAM_BYTES, ioe, this);
                        }
                    }
                }
            }
            return bytes;
        }

        /// <summary>Sets <c>bytes</c> as stream's content.</summary>
        /// <remarks>
        /// Sets <c>bytes</c> as stream's content.
        /// Could not be used with streams which were created by <c>InputStream</c>.
        /// </remarks>
        /// <param name="bytes">new content for stream; if <c>null</c> then stream's content will be discarded</param>
        public virtual void SetData(byte[] bytes) {
            SetData(bytes, false);
        }

        /// <summary>Sets or appends <c>bytes</c> to stream content.</summary>
        /// <remarks>
        /// Sets or appends <c>bytes</c> to stream content.
        /// Could not be used with streams which were created by <c>InputStream</c>.
        /// </remarks>
        /// <param name="bytes">
        /// New content for stream. These bytes are considered to be a raw data (i.e. not encoded/compressed/encrypted)
        /// and if it's not true, the corresponding filters shall be set to the PdfStream object manually.
        /// Data compression generally should be configured via
        /// <see cref="SetCompressionLevel(int)"/>
        /// and
        /// is handled on stream writing to the output document.
        /// If <c>null</c> and <c>append</c> is false then stream's content will be discarded.
        /// </param>
        /// <param name="append">
        /// If set to true then <c>bytes</c> will be appended to the end,
        /// rather then replace original content. The original content will be decoded if needed.
        /// </param>
        public virtual void SetData(byte[] bytes, bool append) {
            if (IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_OPERATE_WITH_FLUSHED_PDF_STREAM);
            }
            if (inputStream != null) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_SET_DATA_TO_PDF_STREAM_WHICH_WAS_CREATED_BY_INPUT_STREAM
                    );
            }
            bool outputStreamIsUninitialized = outputStream == null;
            if (outputStreamIsUninitialized) {
                outputStream = new PdfOutputStream(new ByteArrayOutputStream());
            }
            if (append) {
                if (outputStreamIsUninitialized && GetIndirectReference() != null && GetIndirectReference().GetReader() !=
                     null || !outputStreamIsUninitialized && ContainsKey(PdfName.Filter)) {
                    // here is the same as in the getBytes() method: this logic makes sense only when stream is created
                    // by reader and in this case indirect reference won't be null and stream is not in the MustBeIndirect state.
                    byte[] oldBytes;
                    try {
                        oldBytes = GetBytes();
                    }
                    catch (PdfException ex) {
                        throw new PdfException(KernelExceptionMessageConstant.CANNOT_READ_A_STREAM_IN_ORDER_TO_APPEND_NEW_BYTES, ex
                            );
                    }
                    outputStream.AssignBytes(oldBytes, oldBytes.Length);
                }
                if (bytes != null) {
                    outputStream.WriteBytes(bytes);
                }
            }
            else {
                if (bytes != null) {
                    outputStream.AssignBytes(bytes, bytes.Length);
                }
                else {
                    outputStream.Reset();
                }
            }
            offset = 0;
            // Bytes that are set shall be not encoded, and moreover the existing bytes in cases of the appending are decoded,
            // therefore all filters shall be removed. Compression will be handled on stream flushing.
            Remove(PdfName.Filter);
            Remove(PdfName.DecodeParms);
        }

        protected internal override PdfObject NewInstance() {
            return new iText.Kernel.Pdf.PdfStream();
        }

        protected internal virtual long GetOffset() {
            return offset;
        }

        /// <summary>Update length manually in case its correction.</summary>
        /// <param name="length">the new length</param>
        /// <seealso cref="PdfReader.CheckPdfStreamLength(PdfStream)"/>
        protected internal virtual void UpdateLength(int length) {
            this.length = length;
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document) {
            CopyContent(from, document, NullCopyFilter.GetInstance());
        }

        protected internal override void CopyContent(PdfObject from, PdfDocument document, ICopyFilter copyFilter) {
            base.CopyContent(from, document, copyFilter);
            iText.Kernel.Pdf.PdfStream stream = (iText.Kernel.Pdf.PdfStream)from;
            System.Diagnostics.Debug.Assert(inputStream == null, "Try to copy the PdfStream that has been just created."
                );
            byte[] bytes = stream.GetBytes(false);
            try {
                outputStream.Write(bytes);
            }
            catch (System.IO.IOException ioe) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_COPY_OBJECT_CONTENT, ioe, stream);
            }
        }

        protected internal virtual void InitOutputStream(Stream stream) {
            if (GetOutputStream() == null && inputStream == null) {
                outputStream = new PdfOutputStream(stream != null ? stream : new ByteArrayOutputStream());
            }
        }

        /// <summary>Release content of PdfStream.</summary>
        protected internal override void ReleaseContent() {
            base.ReleaseContent();
            try {
                if (outputStream != null) {
                    outputStream.Dispose();
                    outputStream = null;
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.IO_EXCEPTION, e);
            }
        }

        protected internal virtual Stream GetInputStream() {
            return inputStream;
        }
    }
}
