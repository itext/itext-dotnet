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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Crypto;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Filters;

namespace iText.Kernel.Pdf {
    public class PdfOutputStream : HighPrecisionOutputStream<iText.Kernel.Pdf.PdfOutputStream> {
        private static readonly byte[] stream = ByteUtils.GetIsoBytes("stream\n");

        private static readonly byte[] endstream = ByteUtils.GetIsoBytes("\nendstream");

        private static readonly byte[] openDict = ByteUtils.GetIsoBytes("<<");

        private static readonly byte[] closeDict = ByteUtils.GetIsoBytes(">>");

        private static readonly byte[] endIndirect = ByteUtils.GetIsoBytes(" R");

        private static readonly byte[] endIndirectWithZeroGenNr = ByteUtils.GetIsoBytes(" 0 R");

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.PdfOutputStream
            ));

        /// <summary>Document associated with PdfOutputStream.</summary>
        protected internal PdfDocument document = null;

        /// <summary>Contains the business logic for cryptography.</summary>
        protected internal PdfEncryption crypto;

        /// <summary>Create a pdfOutputSteam writing to the passed OutputStream.</summary>
        /// <param name="outputStream">Outputstream to write to.</param>
        public PdfOutputStream(Stream outputStream)
            : base(outputStream) {
        }

        /// <summary>Write a PdfObject to the outputstream.</summary>
        /// <param name="pdfObject">PdfObject to write</param>
        /// <returns>this PdfOutPutStream</returns>
        public virtual iText.Kernel.Pdf.PdfOutputStream Write(PdfObject pdfObject) {
            if (pdfObject.CheckState(PdfObject.MUST_BE_INDIRECT) && document != null) {
                pdfObject.MakeIndirect(document);
                pdfObject = pdfObject.GetIndirectReference();
            }
            if (pdfObject.CheckState(PdfObject.READ_ONLY)) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_WRITE_OBJECT_AFTER_IT_WAS_RELEASED);
            }
            switch (pdfObject.GetObjectType()) {
                case PdfObject.ARRAY: {
                    Write((PdfArray)pdfObject);
                    break;
                }

                case PdfObject.DICTIONARY: {
                    Write((PdfDictionary)pdfObject);
                    break;
                }

                case PdfObject.INDIRECT_REFERENCE: {
                    Write((PdfIndirectReference)pdfObject);
                    break;
                }

                case PdfObject.NAME: {
                    Write((PdfName)pdfObject);
                    break;
                }

                case PdfObject.NULL:
                case PdfObject.BOOLEAN: {
                    Write((PdfPrimitiveObject)pdfObject);
                    break;
                }

                case PdfObject.LITERAL: {
                    Write((PdfLiteral)pdfObject);
                    break;
                }

                case PdfObject.STRING: {
                    Write((PdfString)pdfObject);
                    break;
                }

                case PdfObject.NUMBER: {
                    Write((PdfNumber)pdfObject);
                    break;
                }

                case PdfObject.STREAM: {
                    Write((PdfStream)pdfObject);
                    break;
                }
            }
            return this;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Writes corresponding amount of bytes from a given long</summary>
        /// <param name="bytes">a source of bytes, must be &gt;= 0</param>
        /// <param name="size">expected amount of bytes</param>
        internal virtual void Write(long bytes, int size) {
            System.Diagnostics.Debug.Assert(bytes >= 0);
            while (--size >= 0) {
                Write((byte)(bytes >> 8 * size & 0xff));
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Writes corresponding amount of bytes from a given int</summary>
        /// <param name="bytes">a source of bytes, must be &gt;= 0</param>
        /// <param name="size">expected amount of bytes</param>
        internal virtual void Write(int bytes, int size) {
            //safe convert to long, despite sign.
            Write(bytes & 0xFFFFFFFFL, size);
        }
//\endcond

        private void Write(PdfArray pdfArray) {
            WriteByte('[');
            for (int i = 0; i < pdfArray.Size(); i++) {
                PdfObject value = pdfArray.Get(i, false);
                PdfIndirectReference indirectReference;
                if ((indirectReference = value.GetIndirectReference()) != null) {
                    Write(indirectReference);
                }
                else {
                    Write(value);
                }
                if (i < pdfArray.Size() - 1) {
                    WriteSpace();
                }
            }
            WriteByte(']');
        }

        private void Write(PdfDictionary pdfDictionary) {
            WriteBytes(openDict);
            foreach (PdfName key in pdfDictionary.KeySet()) {
                bool isAlreadyWriteSpace = false;
                Write(key);
                PdfObject value = pdfDictionary.Get(key, false);
                if (value == null) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.INVALID_KEY_VALUE_KEY_0_HAS_NULL_VALUE
                        , key));
                    value = PdfNull.PDF_NULL;
                }
                if ((value.GetObjectType() == PdfObject.NUMBER || value.GetObjectType() == PdfObject.LITERAL || value.GetObjectType
                    () == PdfObject.BOOLEAN || value.GetObjectType() == PdfObject.NULL || value.GetObjectType() == PdfObject
                    .INDIRECT_REFERENCE || value.CheckState(PdfObject.MUST_BE_INDIRECT))) {
                    isAlreadyWriteSpace = true;
                    WriteSpace();
                }
                PdfIndirectReference indirectReference;
                if ((indirectReference = value.GetIndirectReference()) != null) {
                    if (!isAlreadyWriteSpace) {
                        WriteSpace();
                    }
                    Write(indirectReference);
                }
                else {
                    Write(value);
                }
            }
            WriteBytes(closeDict);
        }

        private void Write(PdfIndirectReference indirectReference) {
            if (document != null && !indirectReference.GetDocument().Equals(document)) {
                throw new PdfException(KernelExceptionMessageConstant.PDF_INDIRECT_OBJECT_BELONGS_TO_OTHER_PDF_DOCUMENT);
            }
            if (indirectReference.IsFree()) {
                LOGGER.LogError(iText.IO.Logs.IoLogMessageConstant.FLUSHED_OBJECT_CONTAINS_FREE_REFERENCE);
                Write(PdfNull.PDF_NULL);
            }
            else {
                if (indirectReference.refersTo == null && (indirectReference.CheckState(PdfObject.MODIFIED) || indirectReference
                    .GetReader() == null || !(indirectReference.GetOffset() > 0 || indirectReference.GetIndex() >= 0))) {
                    LOGGER.LogError(iText.IO.Logs.IoLogMessageConstant.FLUSHED_OBJECT_CONTAINS_REFERENCE_WHICH_NOT_REFER_TO_ANY_OBJECT
                        );
                    Write(PdfNull.PDF_NULL);
                }
                else {
                    if (indirectReference.GetGenNumber() == 0) {
                        WriteInteger(indirectReference.GetObjNumber()).WriteBytes(endIndirectWithZeroGenNr);
                    }
                    else {
                        WriteInteger(indirectReference.GetObjNumber()).WriteSpace().WriteInteger(indirectReference.GetGenNumber())
                            .WriteBytes(endIndirect);
                    }
                }
            }
        }

        private void Write(PdfPrimitiveObject pdfPrimitive) {
            WriteBytes(pdfPrimitive.GetInternalContent());
        }

        private void Write(PdfLiteral literal) {
            literal.SetPosition(GetCurrentPos());
            WriteBytes(literal.GetInternalContent());
        }

        private void Write(PdfString pdfString) {
            pdfString.Encrypt(crypto);
            if (pdfString.IsHexWriting()) {
                WriteByte('<');
                WriteBytes(pdfString.GetInternalContent());
                WriteByte('>');
            }
            else {
                WriteByte('(');
                WriteBytes(pdfString.GetInternalContent());
                WriteByte(')');
            }
        }

        private void Write(PdfName name) {
            WriteByte('/');
            WriteBytes(name.GetInternalContent());
        }

        private void Write(PdfNumber pdfNumber) {
            if (pdfNumber.HasContent()) {
                WriteBytes(pdfNumber.GetInternalContent());
            }
            else {
                if (pdfNumber.IsDoubleNumber()) {
                    WriteDouble(pdfNumber.GetValue());
                }
                else {
                    WriteInteger(pdfNumber.IntValue());
                }
            }
        }

        private bool IsNotMetadataPdfStream(PdfStream pdfStream) {
            return pdfStream.GetAsName(PdfName.Type) == null || (pdfStream.GetAsName(PdfName.Type) != null && !pdfStream
                .GetAsName(PdfName.Type).Equals(PdfName.Metadata));
        }

        private bool IsXRefStream(PdfStream pdfStream) {
            return PdfName.XRef.Equals(pdfStream.GetAsName(PdfName.Type));
        }

        private void Write(PdfStream pdfStream) {
            try {
                bool userDefinedCompression = pdfStream.GetCompressionLevel() != CompressionConstants.UNDEFINED_COMPRESSION;
                if (!userDefinedCompression) {
                    int defaultCompressionLevel = document != null ? document.GetWriter().GetCompressionLevel() : CompressionConstants
                        .DEFAULT_COMPRESSION;
                    pdfStream.SetCompressionLevel(defaultCompressionLevel);
                }
                bool toCompress = pdfStream.GetCompressionLevel() != CompressionConstants.NO_COMPRESSION;
                bool allowCompression = !pdfStream.ContainsKey(PdfName.Filter) && IsNotMetadataPdfStream(pdfStream);
                if (pdfStream.GetInputStream() != null) {
                    Stream fout = this;
                    DeflaterOutputStream def = null;
                    OutputStreamEncryption ose = null;
                    if (crypto != null && (!crypto.IsEmbeddedFilesOnly() || document.DoesStreamBelongToEmbeddedFile(pdfStream)
                        )) {
                        fout = ose = crypto.GetEncryptionStream(fout);
                    }
                    if (toCompress && (allowCompression || userDefinedCompression)) {
                        UpdateCompressionFilter(pdfStream);
                        fout = def = new DeflaterOutputStream(fout, pdfStream.GetCompressionLevel(), 0x8000);
                    }
                    this.Write((PdfDictionary)pdfStream);
                    WriteBytes(iText.Kernel.Pdf.PdfOutputStream.stream);
                    long beginStreamContent = GetCurrentPos();
                    byte[] buf = new byte[4192];
                    while (true) {
                        int n = pdfStream.GetInputStream().Read(buf);
                        if (n <= 0) {
                            break;
                        }
                        fout.Write(buf, 0, n);
                    }
                    if (def != null) {
                        def.Finish();
                    }
                    if (ose != null) {
                        ose.Finish();
                    }
                    PdfNumber length = pdfStream.GetAsNumber(PdfName.Length);
                    length.SetValue((int)(GetCurrentPos() - beginStreamContent));
                    pdfStream.UpdateLength(length.IntValue());
                    WriteBytes(iText.Kernel.Pdf.PdfOutputStream.endstream);
                }
                else {
                    //When document is opened in stamping mode the output stream can be uninitialized.
                    //We have to initialize it and write all data from streams input to streams output.
                    if (pdfStream.GetOutputStream() == null && pdfStream.GetIndirectReference().GetReader() != null) {
                        // If new specific compression is set for stream,
                        // then compressed stream should be decoded and written with new compression settings
                        byte[] bytes = pdfStream.GetIndirectReference().GetReader().ReadStreamBytes(pdfStream, false);
                        if (userDefinedCompression) {
                            bytes = DecodeFlateBytes(pdfStream, bytes);
                        }
                        pdfStream.InitOutputStream(new ByteArrayOutputStream(bytes.Length));
                        pdfStream.GetOutputStream().Write(bytes);
                    }
                    System.Diagnostics.Debug.Assert(pdfStream.GetOutputStream() != null, "PdfStream lost OutputStream");
                    ByteArrayOutputStream byteArrayStream;
                    try {
                        if (toCompress && !ContainsFlateFilter(pdfStream) && DecodeParamsArrayNotFlushed(pdfStream) && (allowCompression
                             || userDefinedCompression)) {
                            // compress
                            UpdateCompressionFilter(pdfStream);
                            byteArrayStream = new ByteArrayOutputStream();
                            DeflaterOutputStream zip = new DeflaterOutputStream(byteArrayStream, pdfStream.GetCompressionLevel());
                            if (pdfStream is PdfObjectStream) {
                                PdfObjectStream objectStream = (PdfObjectStream)pdfStream;
                                ((ByteArrayOutputStream)objectStream.GetIndexStream().GetOutputStream()).WriteTo(zip);
                                ((ByteArrayOutputStream)objectStream.GetOutputStream().GetOutputStream()).WriteTo(zip);
                            }
                            else {
                                System.Diagnostics.Debug.Assert(pdfStream.GetOutputStream() != null, "Error in outputStream");
                                ((ByteArrayOutputStream)pdfStream.GetOutputStream().GetOutputStream()).WriteTo(zip);
                            }
                            zip.Finish();
                        }
                        else {
                            if (pdfStream is PdfObjectStream) {
                                PdfObjectStream objectStream = (PdfObjectStream)pdfStream;
                                byteArrayStream = new ByteArrayOutputStream();
                                ((ByteArrayOutputStream)objectStream.GetIndexStream().GetOutputStream()).WriteTo(byteArrayStream);
                                ((ByteArrayOutputStream)objectStream.GetOutputStream().GetOutputStream()).WriteTo(byteArrayStream);
                            }
                            else {
                                System.Diagnostics.Debug.Assert(pdfStream.GetOutputStream() != null, "Error in outputStream");
                                byteArrayStream = (ByteArrayOutputStream)pdfStream.GetOutputStream().GetOutputStream();
                            }
                        }
                        if (CheckEncryption(pdfStream)) {
                            ByteArrayOutputStream encodedStream = new ByteArrayOutputStream();
                            OutputStreamEncryption ose = crypto.GetEncryptionStream(encodedStream);
                            byteArrayStream.WriteTo(ose);
                            ose.Finish();
                            byteArrayStream = encodedStream;
                        }
                    }
                    catch (System.IO.IOException ioe) {
                        throw new PdfException(KernelExceptionMessageConstant.IO_EXCEPTION, ioe);
                    }
                    pdfStream.Put(PdfName.Length, new PdfNumber(byteArrayStream.Length));
                    pdfStream.UpdateLength((int)byteArrayStream.Length);
                    this.Write((PdfDictionary)pdfStream);
                    WriteBytes(iText.Kernel.Pdf.PdfOutputStream.stream);
                    byteArrayStream.WriteTo(this);
                    byteArrayStream.Dispose();
                    WriteBytes(iText.Kernel.Pdf.PdfOutputStream.endstream);
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_WRITE_TO_PDF_STREAM, e, pdfStream);
            }
        }

        protected internal virtual bool CheckEncryption(PdfStream pdfStream) {
            if (crypto == null || (crypto.IsEmbeddedFilesOnly() && !document.DoesStreamBelongToEmbeddedFile(pdfStream)
                )) {
                return false;
            }
            if (IsXRefStream(pdfStream)) {
                // The cross-reference stream shall not be encrypted
                return false;
            }
            PdfObject filter = pdfStream.Get(PdfName.Filter, true);
            if (filter == null) {
                return true;
            }
            if (filter.IsFlushed()) {
                IndirectFilterUtils.ThrowFlushedFilterException(pdfStream);
            }
            if (PdfName.Crypt.Equals(filter)) {
                return false;
            }
            if (filter.GetObjectType() == PdfObject.ARRAY) {
                PdfArray filters = (PdfArray)filter;
                if (filters.IsEmpty()) {
                    return true;
                }
                if (filters.Get(0).IsFlushed()) {
                    IndirectFilterUtils.ThrowFlushedFilterException(pdfStream);
                }
                return !PdfName.Crypt.Equals(filters.Get(0, true));
            }
            return true;
        }

        protected internal virtual bool ContainsFlateFilter(PdfStream pdfStream) {
            PdfObject filter = pdfStream.Get(PdfName.Filter);
            if (filter == null) {
                return false;
            }
            if (filter.IsFlushed()) {
                IndirectFilterUtils.LogFilterWasAlreadyFlushed(LOGGER, pdfStream);
                return true;
            }
            if (filter.GetObjectType() != PdfObject.NAME && filter.GetObjectType() != PdfObject.ARRAY) {
                throw new PdfException(KernelExceptionMessageConstant.FILTER_IS_NOT_A_NAME_OR_ARRAY);
            }
            if (filter.GetObjectType() == PdfObject.NAME) {
                return PdfName.FlateDecode.Equals(filter);
            }
            foreach (PdfObject obj in (PdfArray)filter) {
                if (obj.IsFlushed()) {
                    IndirectFilterUtils.LogFilterWasAlreadyFlushed(LOGGER, pdfStream);
                    return true;
                }
            }
            return ((PdfArray)filter).Contains(PdfName.FlateDecode);
        }

        protected internal virtual void UpdateCompressionFilter(PdfStream pdfStream) {
            PdfObject filter = pdfStream.Get(PdfName.Filter);
            if (filter == null) {
                // Remove if any
                pdfStream.Remove(PdfName.DecodeParms);
                pdfStream.Put(PdfName.Filter, PdfName.FlateDecode);
                return;
            }
            PdfArray filters = new PdfArray();
            filters.Add(PdfName.FlateDecode);
            if (filter is PdfArray) {
                filters.AddAll((PdfArray)filter);
            }
            else {
                filters.Add(filter);
            }
            PdfObject decodeParms = pdfStream.Get(PdfName.DecodeParms);
            if (decodeParms != null) {
                if (decodeParms is PdfDictionary) {
                    PdfArray array = new PdfArray();
                    array.Add(new PdfNull());
                    array.Add(decodeParms);
                    pdfStream.Put(PdfName.DecodeParms, array);
                }
                else {
                    if (decodeParms is PdfArray) {
                        ((PdfArray)decodeParms).Add(0, new PdfNull());
                    }
                    else {
                        throw new PdfException(KernelExceptionMessageConstant.THIS_DECODE_PARAMETER_TYPE_IS_NOT_SUPPORTED).SetMessageParams
                            (decodeParms.GetType().ToString());
                    }
                }
            }
            pdfStream.Put(PdfName.Filter, filters);
        }

        protected internal virtual byte[] DecodeFlateBytes(PdfStream stream, byte[] bytes) {
            PdfObject filterObject = stream.Get(PdfName.Filter);
            if (filterObject == null) {
                return bytes;
            }
            // check if flateDecode filter is on top
            PdfName filterName;
            PdfArray filtersArray = null;
            if (filterObject is PdfName) {
                filterName = (PdfName)filterObject;
            }
            else {
                if (filterObject is PdfArray) {
                    filtersArray = (PdfArray)filterObject;
                    if (filtersArray.IsFlushed()) {
                        IndirectFilterUtils.LogFilterWasAlreadyFlushed(LOGGER, stream);
                        return bytes;
                    }
                    filterName = filtersArray.GetAsName(0);
                }
                else {
                    throw new PdfException(KernelExceptionMessageConstant.FILTER_IS_NOT_A_NAME_OR_ARRAY);
                }
            }
            if (filterName.IsFlushed()) {
                IndirectFilterUtils.LogFilterWasAlreadyFlushed(LOGGER, stream);
                return bytes;
            }
            if (!PdfName.FlateDecode.Equals(filterName)) {
                return bytes;
            }
            // get decode params if present
            PdfDictionary decodeParams;
            PdfArray decodeParamsArray = null;
            PdfObject decodeParamsObject = stream.Get(PdfName.DecodeParms);
            if (decodeParamsObject == null) {
                decodeParams = null;
            }
            else {
                if (decodeParamsObject.IsFlushed()) {
                    IndirectFilterUtils.LogFilterWasAlreadyFlushed(LOGGER, stream);
                    return bytes;
                }
                else {
                    if (decodeParamsObject.GetObjectType() == PdfObject.DICTIONARY) {
                        decodeParams = (PdfDictionary)decodeParamsObject;
                    }
                    else {
                        if (decodeParamsObject.GetObjectType() == PdfObject.ARRAY) {
                            decodeParamsArray = (PdfArray)decodeParamsObject;
                            decodeParams = decodeParamsArray.GetAsDictionary(0);
                        }
                        else {
                            throw new PdfException(KernelExceptionMessageConstant.THIS_DECODE_PARAMETER_TYPE_IS_NOT_SUPPORTED).SetMessageParams
                                (decodeParamsObject.GetType().ToString());
                        }
                    }
                }
            }
            if (decodeParams != null && (decodeParams.IsFlushed() || IsFlushed(decodeParams, PdfName.Predictor) || IsFlushed
                (decodeParams, PdfName.Columns) || IsFlushed(decodeParams, PdfName.Colors) || IsFlushed(decodeParams, 
                PdfName.BitsPerComponent))) {
                IndirectFilterUtils.LogFilterWasAlreadyFlushed(LOGGER, stream);
                return bytes;
            }
            // decode
            byte[] res = FlateDecodeFilter.FlateDecode(bytes, true);
            if (res == null) {
                res = FlateDecodeFilter.FlateDecode(bytes, false);
            }
            bytes = FlateDecodeFilter.DecodePredictor(res, decodeParams);
            //remove filter and decode params
            filterObject = null;
            if (filtersArray != null) {
                filtersArray.Remove(0);
                if (filtersArray.Size() == 1) {
                    filterObject = filtersArray.Get(0);
                }
                else {
                    if (!filtersArray.IsEmpty()) {
                        filterObject = filtersArray;
                    }
                }
            }
            decodeParamsObject = null;
            if (decodeParamsArray != null) {
                decodeParamsArray.Remove(0);
                if (decodeParamsArray.Size() == 1 && decodeParamsArray.Get(0).GetObjectType() != PdfObject.NULL) {
                    decodeParamsObject = decodeParamsArray.Get(0);
                }
                else {
                    if (!decodeParamsArray.IsEmpty()) {
                        decodeParamsObject = decodeParamsArray;
                    }
                }
            }
            if (filterObject == null) {
                stream.Remove(PdfName.Filter);
            }
            else {
                stream.Put(PdfName.Filter, filterObject);
            }
            if (decodeParamsObject == null) {
                stream.Remove(PdfName.DecodeParms);
            }
            else {
                stream.Put(PdfName.DecodeParms, decodeParamsObject);
            }
            return bytes;
        }

        private static bool IsFlushed(PdfDictionary dict, PdfName name) {
            PdfObject obj = dict.Get(name);
            return obj != null && obj.IsFlushed();
        }

        private static bool DecodeParamsArrayNotFlushed(PdfStream pdfStream) {
            PdfArray decodeParams = pdfStream.GetAsArray(PdfName.DecodeParms);
            if (decodeParams == null) {
                return true;
            }
            if (decodeParams.IsFlushed()) {
                IndirectFilterUtils.LogFilterWasAlreadyFlushed(LOGGER, pdfStream);
                return false;
            }
            return true;
        }
    }
}
