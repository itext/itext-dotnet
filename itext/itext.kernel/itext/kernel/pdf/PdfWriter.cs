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
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    public class PdfWriter : PdfOutputStream {
        private static readonly byte[] obj = ByteUtils.GetIsoBytes(" obj\n");

        private static readonly byte[] endobj = ByteUtils.GetIsoBytes("\nendobj\n");

        protected internal WriterProperties properties;

        //forewarned is forearmed
        protected internal bool isUserWarnedAboutAcroFormCopying;

        /// <summary>Currently active object stream.</summary>
        /// <remarks>
        /// Currently active object stream.
        /// Objects are written to the object stream if fullCompression set to true.
        /// </remarks>
        internal PdfObjectStream objectStream = null;

        /// <summary>Is used to avoid duplications on object copying.</summary>
        /// <remarks>
        /// Is used to avoid duplications on object copying.
        /// It stores hashes of the indirect reference from the source document and the corresponding
        /// indirect references of the copied objects from the new document.
        /// </remarks>
        private IDictionary<PdfIndirectReference, PdfIndirectReference> copiedObjects = new LinkedDictionary<PdfIndirectReference
            , PdfIndirectReference>();

        /// <summary>Is used in smart mode to serialize and store serialized objects content.</summary>
        private SmartModePdfObjectsSerializer smartModeSerializer = new SmartModePdfObjectsSerializer();

        /// <summary>Create a PdfWriter writing to the passed File and with default writer properties.</summary>
        /// <param name="file">File to write to.</param>
        public PdfWriter(FileInfo file)
            : this(file.FullName) {
        }

        /// <summary>Create a PdfWriter writing to the passed outputstream and with default writer properties.</summary>
        /// <param name="os">Outputstream to write to.</param>
        public PdfWriter(Stream os)
            : this(os, new WriterProperties()) {
        }

        public PdfWriter(Stream os, WriterProperties properties)
            : base(new CountOutputStream(FileUtil.WrapWithBufferedOutputStream(os))) {
            this.properties = properties;
        }

        /// <summary>Create a PdfWriter writing to the passed filename and with default writer properties.</summary>
        /// <param name="filename">filename of the resulting pdf.</param>
        public PdfWriter(String filename)
            : this(filename, new WriterProperties()) {
        }

        /// <summary>Create a PdfWriter writing to the passed filename and using the passed writer properties.</summary>
        /// <param name="filename">filename of the resulting pdf.</param>
        /// <param name="properties">writerproperties to use.</param>
        public PdfWriter(String filename, WriterProperties properties)
            : this(FileUtil.GetBufferedOutputStream(filename), properties) {
        }

        /// <summary>Indicates if to use full compression mode.</summary>
        /// <returns>true if to use full compression, false otherwise.</returns>
        public virtual bool IsFullCompression() {
            return properties.isFullCompression != null ? (bool)properties.isFullCompression : false;
        }

        /// <summary>Gets default compression level for @see PdfStream.</summary>
        /// <remarks>
        /// Gets default compression level for @see PdfStream.
        /// For more details @see
        /// <see cref="iText.IO.Source.DeflaterOutputStream"/>.
        /// </remarks>
        /// <returns>compression level.</returns>
        public virtual int GetCompressionLevel() {
            return properties.compressionLevel;
        }

        /// <summary>Sets default compression level for @see PdfStream.</summary>
        /// <remarks>
        /// Sets default compression level for @see PdfStream.
        /// For more details @see
        /// <see cref="iText.IO.Source.DeflaterOutputStream"/>.
        /// </remarks>
        /// <param name="compressionLevel">compression level.</param>
        /// <returns>
        /// this
        /// <see cref="PdfWriter"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfWriter SetCompressionLevel(int compressionLevel) {
            this.properties.SetCompressionLevel(compressionLevel);
            return this;
        }

        /// <summary>Sets the smart mode.</summary>
        /// <remarks>
        /// Sets the smart mode.
        /// <br />
        /// In smart mode when resources (such as fonts, images,...) are
        /// encountered, a reference to these resources is saved
        /// in a cache, so that they can be reused.
        /// This requires more memory, but reduces the file size
        /// of the resulting PDF document.
        /// </remarks>
        /// <param name="smartMode">True for enabling smart mode.</param>
        /// <returns>
        /// this
        /// <see cref="PdfWriter"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.PdfWriter SetSmartMode(bool smartMode) {
            this.properties.smartMode = smartMode;
            return this;
        }

        protected internal virtual void InitCryptoIfSpecified(PdfVersion version) {
            EncryptionProperties encryptProps = properties.encryptionProperties;
            if (properties.IsStandardEncryptionUsed()) {
                crypto = new PdfEncryption(encryptProps.userPassword, encryptProps.ownerPassword, encryptProps.standardEncryptPermissions
                    , encryptProps.encryptionAlgorithm, ByteUtils.GetIsoBytes(this.document.GetOriginalDocumentId().GetValue
                    ()), version);
            }
            else {
                if (properties.IsPublicKeyEncryptionUsed()) {
                    crypto = new PdfEncryption(encryptProps.publicCertificates, encryptProps.publicKeyEncryptPermissions, encryptProps
                        .encryptionAlgorithm, version);
                }
            }
        }

        /// <summary>Flushes the object.</summary>
        /// <remarks>Flushes the object. Override this method if you want to define custom behaviour for object flushing.
        ///     </remarks>
        /// <param name="pdfObject">object to flush.</param>
        /// <param name="canBeInObjStm">indicates whether object can be placed into object stream.</param>
        protected internal virtual void FlushObject(PdfObject pdfObject, bool canBeInObjStm) {
            PdfIndirectReference indirectReference = pdfObject.GetIndirectReference();
            if (IsFullCompression() && canBeInObjStm) {
                PdfObjectStream objectStream = GetObjectStream();
                objectStream.AddObject(pdfObject);
            }
            else {
                indirectReference.SetOffset(GetCurrentPos());
                WriteToBody(pdfObject);
            }
            indirectReference.SetState(PdfObject.FLUSHED).ClearState(PdfObject.MUST_BE_FLUSHED);
            switch (pdfObject.GetObjectType()) {
                case PdfObject.BOOLEAN:
                case PdfObject.NAME:
                case PdfObject.NULL:
                case PdfObject.NUMBER:
                case PdfObject.STRING: {
                    ((PdfPrimitiveObject)pdfObject).content = null;
                    break;
                }

                case PdfObject.ARRAY: {
                    PdfArray array = ((PdfArray)pdfObject);
                    MarkArrayContentToFlush(array);
                    array.ReleaseContent();
                    break;
                }

                case PdfObject.STREAM:
                case PdfObject.DICTIONARY: {
                    PdfDictionary dictionary = ((PdfDictionary)pdfObject);
                    MarkDictionaryContentToFlush(dictionary);
                    dictionary.ReleaseContent();
                    break;
                }

                case PdfObject.INDIRECT_REFERENCE: {
                    MarkObjectToFlush(((PdfIndirectReference)pdfObject).GetRefersTo(false));
                    break;
                }
            }
        }

        /// <summary>Copies a PdfObject either stand alone or as part of the PdfDocument passed as documentTo.</summary>
        /// <param name="obj">object to copy</param>
        /// <param name="documentTo">optional target document</param>
        /// <param name="allowDuplicating">allow that some objects will become duplicated by this action</param>
        /// <returns>the copies object</returns>
        protected internal virtual PdfObject CopyObject(PdfObject obj, PdfDocument documentTo, bool allowDuplicating
            ) {
            return CopyObject(obj, documentTo, allowDuplicating, NullCopyFilter.GetInstance());
        }

        /// <summary>Copies a PdfObject either stand alone or as part of the PdfDocument passed as documentTo.</summary>
        /// <param name="obj">object to copy</param>
        /// <param name="documentTo">optional target document</param>
        /// <param name="allowDuplicating">allow that some objects will become duplicated by this action</param>
        /// <param name="copyFilter">
        /// 
        /// <see cref="iText.Kernel.Utils.ICopyFilter"/>
        /// a filter to apply while copying arrays and dictionaries
        /// *             Use
        /// <see cref="iText.Kernel.Utils.NullCopyFilter"/>
        /// for no filtering
        /// </param>
        /// <returns>the copies object</returns>
        protected internal virtual PdfObject CopyObject(PdfObject obj, PdfDocument documentTo, bool allowDuplicating
            , ICopyFilter copyFilter) {
            if (obj is PdfIndirectReference) {
                obj = ((PdfIndirectReference)obj).GetRefersTo();
            }
            if (obj == null) {
                obj = PdfNull.PDF_NULL;
            }
            if (CheckTypeOfPdfDictionary(obj, PdfName.Catalog)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(PdfReader));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.MAKE_COPY_OF_CATALOG_DICTIONARY_IS_FORBIDDEN);
                obj = PdfNull.PDF_NULL;
            }
            PdfIndirectReference indirectReference = obj.GetIndirectReference();
            bool tryToFindDuplicate = !allowDuplicating && indirectReference != null;
            if (tryToFindDuplicate) {
                PdfIndirectReference copiedIndirectReference = copiedObjects.Get(indirectReference);
                if (copiedIndirectReference != null) {
                    return copiedIndirectReference.GetRefersTo();
                }
            }
            SerializedObjectContent serializedContent = null;
            if (properties.smartMode && tryToFindDuplicate && !CheckTypeOfPdfDictionary(obj, PdfName.Page) && !CheckTypeOfPdfDictionary
                (obj, PdfName.OCG) && !CheckTypeOfPdfDictionary(obj, PdfName.OCMD)) {
                serializedContent = smartModeSerializer.SerializeObject(obj);
                PdfIndirectReference objectRef = smartModeSerializer.GetSavedSerializedObject(serializedContent);
                if (objectRef != null) {
                    copiedObjects.Put(indirectReference, objectRef);
                    return objectRef.refersTo;
                }
            }
            PdfObject newObject = obj.NewInstance();
            if (indirectReference != null) {
                PdfIndirectReference indRef = newObject.MakeIndirect(documentTo).GetIndirectReference();
                if (serializedContent != null) {
                    smartModeSerializer.SaveSerializedObject(serializedContent, indRef);
                }
                copiedObjects.Put(indirectReference, indRef);
            }
            newObject.CopyContent(obj, documentTo, copyFilter);
            return newObject;
        }

        /// <summary>Writes object to body of PDF document.</summary>
        /// <param name="pdfObj">object to write.</param>
        protected internal virtual void WriteToBody(PdfObject pdfObj) {
            if (crypto != null) {
                crypto.SetHashKeyForNextObject(pdfObj.GetIndirectReference().GetObjNumber(), pdfObj.GetIndirectReference()
                    .GetGenNumber());
            }
            WriteInteger(pdfObj.GetIndirectReference().GetObjNumber()).WriteSpace().WriteInteger(pdfObj.GetIndirectReference
                ().GetGenNumber()).WriteBytes(obj);
            Write(pdfObj);
            WriteBytes(endobj);
        }

        /// <summary>Writes PDF header.</summary>
        protected internal virtual void WriteHeader() {
            WriteByte('%').WriteString(document.GetPdfVersion().ToString()).WriteString("\n%\u00e2\u00e3\u00cf\u00d3\n"
                );
        }

        /// <summary>Flushes all objects which have not been flushed yet.</summary>
        /// <param name="forbiddenToFlush">
        /// a
        /// <see cref="Java.Util.Set{E}"/>
        /// of
        /// <see cref="PdfIndirectReference">references</see>
        /// that are forbidden to be flushed
        /// automatically.
        /// </param>
        protected internal virtual void FlushWaitingObjects(ICollection<PdfIndirectReference> forbiddenToFlush) {
            PdfXrefTable xref = document.GetXref();
            bool needFlush = true;
            while (needFlush) {
                needFlush = false;
                for (int i = 1; i < xref.Size(); i++) {
                    PdfIndirectReference indirectReference = xref.Get(i);
                    if (indirectReference != null && !indirectReference.IsFree() && indirectReference.CheckState(PdfObject.MUST_BE_FLUSHED
                        ) && !forbiddenToFlush.Contains(indirectReference)) {
                        PdfObject obj = indirectReference.GetRefersTo(false);
                        if (obj != null) {
                            obj.Flush();
                            needFlush = true;
                        }
                    }
                }
            }
            if (objectStream != null && objectStream.GetSize() > 0) {
                objectStream.Flush();
                objectStream = null;
            }
        }

        /// <summary>Flushes all modified objects which have not been flushed yet.</summary>
        /// <remarks>Flushes all modified objects which have not been flushed yet. Used in case incremental updates.</remarks>
        /// <param name="forbiddenToFlush">
        /// a
        /// <see cref="Java.Util.Set{E}"/>
        /// of
        /// <see cref="PdfIndirectReference">references</see>
        /// that are forbidden to be flushed
        /// automatically.
        /// </param>
        protected internal virtual void FlushModifiedWaitingObjects(ICollection<PdfIndirectReference> forbiddenToFlush
            ) {
            PdfXrefTable xref = document.GetXref();
            for (int i = 1; i < xref.Size(); i++) {
                PdfIndirectReference indirectReference = xref.Get(i);
                if (null != indirectReference && !indirectReference.IsFree() && !forbiddenToFlush.Contains(indirectReference
                    )) {
                    bool isModified = indirectReference.CheckState(PdfObject.MODIFIED);
                    if (isModified) {
                        PdfObject obj = indirectReference.GetRefersTo(false);
                        if (obj != null) {
                            if (!obj.Equals(objectStream)) {
                                obj.Flush();
                            }
                        }
                    }
                }
            }
            if (objectStream != null && objectStream.GetSize() > 0) {
                objectStream.Flush();
                objectStream = null;
            }
        }

        /// <summary>Gets the current object stream.</summary>
        /// <returns>object stream.</returns>
        internal virtual PdfObjectStream GetObjectStream() {
            if (!IsFullCompression()) {
                return null;
            }
            if (objectStream == null) {
                objectStream = new PdfObjectStream(document);
            }
            else {
                if (objectStream.GetSize() == PdfObjectStream.MAX_OBJ_STREAM_SIZE) {
                    objectStream.Flush();
                    objectStream = new PdfObjectStream(objectStream);
                }
            }
            return objectStream;
        }

        /// <summary>Flush all copied objects.</summary>
        /// <param name="docId">id of the source document</param>
        internal virtual void FlushCopiedObjects(long docId) {
            IList<PdfIndirectReference> remove = new List<PdfIndirectReference>();
            foreach (KeyValuePair<PdfIndirectReference, PdfIndirectReference> copiedObject in copiedObjects) {
                PdfDocument document = copiedObject.Key.GetDocument();
                if (document != null && document.GetDocumentId() == docId) {
                    if (copiedObject.Value.refersTo != null) {
                        copiedObject.Value.refersTo.Flush();
                        remove.Add(copiedObject.Key);
                    }
                }
            }
            foreach (PdfIndirectReference ird in remove) {
                copiedObjects.JRemove(ird);
            }
        }

        private void MarkArrayContentToFlush(PdfArray array) {
            for (int i = 0; i < array.Size(); i++) {
                MarkObjectToFlush(array.Get(i, false));
            }
        }

        private void MarkDictionaryContentToFlush(PdfDictionary dictionary) {
            foreach (PdfObject item in dictionary.Values(false)) {
                MarkObjectToFlush(item);
            }
        }

        private void MarkObjectToFlush(PdfObject pdfObject) {
            if (pdfObject != null) {
                PdfIndirectReference indirectReference = pdfObject.GetIndirectReference();
                if (indirectReference != null) {
                    if (!indirectReference.CheckState(PdfObject.FLUSHED)) {
                        indirectReference.SetState(PdfObject.MUST_BE_FLUSHED);
                    }
                }
                else {
                    if (pdfObject.GetObjectType() == PdfObject.INDIRECT_REFERENCE) {
                        if (!pdfObject.CheckState(PdfObject.FLUSHED)) {
                            pdfObject.SetState(PdfObject.MUST_BE_FLUSHED);
                        }
                    }
                    else {
                        if (pdfObject.GetObjectType() == PdfObject.ARRAY) {
                            MarkArrayContentToFlush((PdfArray)pdfObject);
                        }
                        else {
                            if (pdfObject.GetObjectType() == PdfObject.DICTIONARY) {
                                MarkDictionaryContentToFlush((PdfDictionary)pdfObject);
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckTypeOfPdfDictionary(PdfObject dictionary, PdfName expectedType) {
            return dictionary.IsDictionary() && expectedType.Equals(((PdfDictionary)dictionary).GetAsName(PdfName.Type
                ));
        }
    }
}
