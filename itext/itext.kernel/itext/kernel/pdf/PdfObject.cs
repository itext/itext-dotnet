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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Exceptions;
using iText.Kernel.Utils;

namespace iText.Kernel.Pdf {
    public abstract class PdfObject {
        public const byte ARRAY = 1;

        public const byte BOOLEAN = 2;

        public const byte DICTIONARY = 3;

        public const byte LITERAL = 4;

        public const byte INDIRECT_REFERENCE = 5;

        public const byte NAME = 6;

        public const byte NULL = 7;

        public const byte NUMBER = 8;

        public const byte STREAM = 9;

        public const byte STRING = 10;

        /// <summary>Indicates if the object has been flushed.</summary>
        protected internal const short FLUSHED = 1;

        /// <summary>Indicates that the indirect reference of the object could be reused or have to be marked as free.
        ///     </summary>
        protected internal const short FREE = 1 << 1;

        /// <summary>Indicates that definition of the indirect reference of the object still not found (e.g. keys in XRefStm).
        ///     </summary>
        protected internal const short READING = 1 << 2;

        /// <summary>Indicates that object changed (is used in append mode).</summary>
        protected internal const short MODIFIED = 1 << 3;

        /// <summary>Indicates that the indirect reference of the object represents ObjectStream from original document.
        ///     </summary>
        /// <remarks>
        /// Indicates that the indirect reference of the object represents ObjectStream from original document.
        /// When PdfReader read ObjectStream reference marked as OriginalObjectStream
        /// to avoid further reusing.
        /// </remarks>
        protected internal const short ORIGINAL_OBJECT_STREAM = 1 << 4;

        /// <summary>For internal usage only.</summary>
        /// <remarks>
        /// For internal usage only. Marks objects that shall be written to the output document.
        /// Option is needed to build the correct PDF objects tree when closing the document.
        /// As a result it avoids writing unused (removed) objects.
        /// </remarks>
        protected internal const short MUST_BE_FLUSHED = 1 << 5;

        /// <summary>Indicates that the object shall be indirect when it is written to the document.</summary>
        /// <remarks>
        /// Indicates that the object shall be indirect when it is written to the document.
        /// It is used to postpone the creation of indirect reference for the objects that shall be indirect,
        /// so it is possible to create such objects without PdfDocument instance.
        /// </remarks>
        protected internal const short MUST_BE_INDIRECT = 1 << 6;

        /// <summary>Indicates that the object is highly sensitive and we do not want to release it even if release() is called.
        ///     </summary>
        /// <remarks>
        /// Indicates that the object is highly sensitive and we do not want to release it even if release() is called.
        /// This flag can be set in stamping mode in object wrapper constructors and is automatically set when setModified
        /// flag is set (we do not want to release changed objects).
        /// The flag is set automatically for some wrappers that need document even in reader mode (FormFields etc).
        /// </remarks>
        protected internal const short FORBID_RELEASE = 1 << 7;

        /// <summary>
        /// Indicates that we do not want this object to be ever written into the resultant document
        /// (because of multiple objects read from the same reference inconsistency).
        /// </summary>
        protected internal const short READ_ONLY = 1 << 8;

        /// <summary>Indicates that this object is not encrypted in the encrypted document.</summary>
        /// <remarks>
        /// Indicates that this object is not encrypted in the encrypted document.
        /// E.g. digital signature dictionary /Contents entry shall not be encrypted.
        /// </remarks>
        protected internal const short UNENCRYPTED = 1 << 9;

        /// <summary>If object is flushed the indirect reference is kept here.</summary>
        protected internal PdfIndirectReference indirectReference = null;

        /// <summary>Indicate same special states of PdfIndirectObject or PdfObject like @see Free, @see Reading, @see Modified.
        ///     </summary>
        private short state;

        /// <summary>Gets object type.</summary>
        /// <returns>object type.</returns>
        public abstract byte GetObjectType();

        /// <summary>Flushes the object to the document.</summary>
        public void Flush() {
            Flush(true);
        }

        /// <summary>Flushes the object to the document.</summary>
        /// <param name="canBeInObjStm">indicates whether object can be placed into object stream.</param>
        public void Flush(bool canBeInObjStm) {
            if (IsFlushed() || GetIndirectReference() == null || GetIndirectReference().IsFree()) {
                // TODO DEVSIX-744: here we should take into account and log the case when object is MustBeIndirect,
                //  but has no indirect reference
                //            Logger logger = LoggerFactory.getLogger(PdfObject.class);
                //            if (isFlushed()) {
                //                logger.warn("Meaningless call, the object has already flushed");
                //            } else if (isIndirect()){
                //                logger.warn("Meaningless call, the object will be transformed into indirect on closing," +
                //                " but at the moment it doesn't have an indirect reference and therefore couldn't be flushed. " +
                //                        "To flush it now call makeIndirect(PdfDocument) method before calling flush() method.");
                //            } else {
                //                logger.warn("Meaningless call, the object is direct object. It will be flushed along with" +
                //                " the indirect object that contains it.");
                //            }
                return;
            }
            try {
                PdfDocument document = GetIndirectReference().GetDocument();
                if (document != null) {
                    if (document.IsAppendMode() && !IsModified()) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(PdfObject));
                        logger.LogInformation(iText.IO.Logs.IoLogMessageConstant.PDF_OBJECT_FLUSHING_NOT_PERFORMED);
                        return;
                    }
                    document.CheckIsoConformance(this, IsoKey.PDF_OBJECT);
                    document.FlushObject(this, canBeInObjStm && GetObjectType() != STREAM && GetObjectType() != INDIRECT_REFERENCE
                         && GetIndirectReference().GetGenNumber() == 0);
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_FLUSH_OBJECT, e, this);
            }
        }

        /// <summary>Gets the indirect reference associated with the object.</summary>
        /// <remarks>
        /// Gets the indirect reference associated with the object.
        /// The indirect reference is used when flushing object to the document.
        /// </remarks>
        /// <returns>indirect reference.</returns>
        public virtual PdfIndirectReference GetIndirectReference() {
            return indirectReference;
        }

        /// <summary>Checks if object is indirect.</summary>
        /// <remarks>
        /// Checks if object is indirect.
        /// <br />
        /// Note:
        /// Return value
        /// <see langword="true"/>
        /// doesn't necessarily mean that indirect reference of this object
        /// is not null at the moment. Object could be marked as indirect and
        /// be transformed to indirect on flushing.
        /// <br />
        /// E.g. all PdfStreams are transformed to indirect objects when they are written, but they don't always
        /// have indirect references at any given moment.
        /// </remarks>
        /// <returns>
        /// returns
        /// <see langword="true"/>
        /// if object is indirect or is to be indirect in the resultant document.
        /// </returns>
        public virtual bool IsIndirect() {
            return indirectReference != null || CheckState(PdfObject.MUST_BE_INDIRECT);
        }

        /// <summary>Marks object to be saved as indirect.</summary>
        /// <param name="document">a document the indirect reference will belong to.</param>
        /// <param name="reference">indirect reference which will be associated with this document</param>
        /// <returns>object itself.</returns>
        public virtual PdfObject MakeIndirect(PdfDocument document, PdfIndirectReference reference) {
            if (document == null || indirectReference != null) {
                return this;
            }
            if (document.GetWriter() == null) {
                throw new PdfException(KernelExceptionMessageConstant.THERE_IS_NO_ASSOCIATE_PDF_WRITER_FOR_MAKING_INDIRECTS
                    );
            }
            if (reference == null) {
                indirectReference = document.CreateNextIndirectReference();
                indirectReference.SetRefersTo(this);
            }
            else {
                reference.SetState(MODIFIED);
                indirectReference = reference;
                indirectReference.SetRefersTo(this);
            }
            SetState(FORBID_RELEASE);
            ClearState(MUST_BE_INDIRECT);
            return this;
        }

        /// <summary>Marks object to be saved as indirect.</summary>
        /// <param name="document">a document the indirect reference will belong to.</param>
        /// <returns>object itself.</returns>
        public virtual PdfObject MakeIndirect(PdfDocument document) {
            return MakeIndirect(document, null);
        }

        /// <summary>Indicates is the object has been flushed or not.</summary>
        /// <returns>true if object has been flushed, otherwise false.</returns>
        public virtual bool IsFlushed() {
            PdfIndirectReference indirectReference = GetIndirectReference();
            return (indirectReference != null && indirectReference.CheckState(FLUSHED));
        }

        /// <summary>Indicates is the object has been set as modified or not.</summary>
        /// <remarks>Indicates is the object has been set as modified or not. Useful for incremental updates (e.g. appendMode).
        ///     </remarks>
        /// <returns>true is object has been set as modified, otherwise false.</returns>
        public virtual bool IsModified() {
            PdfIndirectReference indirectReference = GetIndirectReference();
            return (indirectReference != null && indirectReference.CheckState(MODIFIED));
        }

        /// <summary>Creates clone of the object which belongs to the same document as original object.</summary>
        /// <remarks>
        /// Creates clone of the object which belongs to the same document as original object.
        /// New object shall not be used in other documents.
        /// </remarks>
        /// <returns>cloned object.</returns>
        public virtual PdfObject Clone() {
            return Clone(NullCopyFilter.GetInstance());
        }

        /// <summary>Creates clone of the object which belongs to the same document as original object.</summary>
        /// <remarks>
        /// Creates clone of the object which belongs to the same document as original object.
        /// New object shall not be used in other documents.
        /// </remarks>
        /// <param name="filter">Filter what will be copied or not</param>
        /// <returns>cloned object.</returns>
        public virtual PdfObject Clone(ICopyFilter filter) {
            PdfObject newObject = NewInstance();
            if (indirectReference != null || CheckState(MUST_BE_INDIRECT)) {
                newObject.SetState(MUST_BE_INDIRECT);
            }
            newObject.CopyContent(this, null, filter);
            return newObject;
        }

        /// <summary>Copies object to a specified document.</summary>
        /// <remarks>
        /// Copies object to a specified document.
        /// <br /><br />
        /// NOTE: Works only for objects that are read from document opened in reading mode,
        /// otherwise an exception is thrown.
        /// </remarks>
        /// <param name="document">document to copy object to.</param>
        /// <returns>copied object.</returns>
        public virtual PdfObject CopyTo(PdfDocument document) {
            return CopyTo(document, true, NullCopyFilter.GetInstance());
        }

        /// <summary>Copies object to a specified document.</summary>
        /// <remarks>
        /// Copies object to a specified document.
        /// <br /><br />
        /// NOTE: Works only for objects that are read from document opened in reading mode,
        /// otherwise an exception is thrown.
        /// </remarks>
        /// <param name="document">document to copy object to.</param>
        /// <param name="allowDuplicating">
        /// indicates if to allow copy objects which already have been copied.
        /// If object is associated with any indirect reference and allowDuplicating is
        /// false then already existing reference will be returned instead of copying object.
        /// If allowDuplicating is true then object will be copied and new indirect
        /// reference will be assigned.
        /// </param>
        /// <returns>copied object.</returns>
        public virtual PdfObject CopyTo(PdfDocument document, bool allowDuplicating) {
            return CopyTo(document, allowDuplicating, NullCopyFilter.GetInstance());
        }

        /// <summary>Copies object to a specified document.</summary>
        /// <remarks>
        /// Copies object to a specified document.
        /// <br /><br />
        /// NOTE: Works only for objects that are read from document opened in reading mode,
        /// otherwise an exception is thrown.
        /// </remarks>
        /// <param name="document">document to copy object to.</param>
        /// <param name="copyFilter">
        /// 
        /// <see cref="iText.Kernel.Utils.ICopyFilter"/>
        /// a filter to apply while copying arrays and dictionaries
        /// Use
        /// <see cref="iText.Kernel.Utils.NullCopyFilter"/>
        /// for no filtering
        /// </param>
        /// <returns>copied object.</returns>
        public virtual PdfObject CopyTo(PdfDocument document, ICopyFilter copyFilter) {
            return CopyTo(document, true, copyFilter);
        }

        /// <summary>Copies object to a specified document.</summary>
        /// <remarks>
        /// Copies object to a specified document.
        /// <br /><br />
        /// NOTE: Works only for objects that are read from document opened in reading mode,
        /// otherwise an exception is thrown.
        /// </remarks>
        /// <param name="document">document to copy object to.</param>
        /// <param name="allowDuplicating">
        /// indicates if to allow copy objects which already have been copied.
        /// If object is associated with any indirect reference and allowDuplicating is false
        /// then already existing reference will be returned instead of copying object.
        /// If allowDuplicating is true then object will be copied and new indirect reference
        /// will be assigned.
        /// </param>
        /// <param name="copyFilter">
        /// 
        /// <see cref="iText.Kernel.Utils.ICopyFilter"/>
        /// a filter to apply while copying arrays and dictionaries
        /// Use
        /// <see cref="iText.Kernel.Utils.NullCopyFilter"/>
        /// for no filtering
        /// </param>
        /// <returns>copied object.</returns>
        public virtual PdfObject CopyTo(PdfDocument document, bool allowDuplicating, ICopyFilter copyFilter) {
            if (document == null) {
                throw new PdfException(KernelExceptionMessageConstant.DOCUMENT_FOR_COPY_TO_CANNOT_BE_NULL);
            }
            if (indirectReference != null) {
                // TODO checkState(MUST_BE_INDIRECT) now is always false, because indirectReference != null. See also
                //  DEVSIX-602
                if (indirectReference.GetWriter() != null || CheckState(MUST_BE_INDIRECT)) {
                    throw new PdfException(KernelExceptionMessageConstant.CANNOT_COPY_INDIRECT_OBJECT_FROM_THE_DOCUMENT_THAT_IS_BEING_WRITTEN
                        );
                }
                if (!indirectReference.GetReader().IsOpenedWithFullPermission()) {
                    throw new BadPasswordException(BadPasswordException.PdfReaderNotOpenedWithOwnerPassword);
                }
            }
            return ProcessCopying(document, allowDuplicating, copyFilter);
        }

        /// <summary>
        /// Sets the 'modified' flag to the indirect object, the flag denotes that the object was modified since
        /// the document opening.
        /// </summary>
        /// <remarks>
        /// Sets the 'modified' flag to the indirect object, the flag denotes that the object was modified since
        /// the document opening.
        /// It is recommended to set this flag after changing any PDF object.
        /// <para />
        /// For example flag is used in the append mode (see
        /// <see cref="StampingProperties.UseAppendMode()"/>
        /// ).
        /// In append mode the whole document is preserved as is, and only changes to the document are
        /// appended to the end of the document file. Because of this, only modified objects need to be flushed and are
        /// allowed to be flushed (i.e. to be written).
        /// </remarks>
        /// <returns>
        /// this
        /// <see cref="PdfObject"/>
        /// instance.
        /// </returns>
        public virtual PdfObject SetModified() {
            if (indirectReference != null) {
                indirectReference.SetState(MODIFIED);
                SetState(FORBID_RELEASE);
            }
            return this;
        }

        /// <summary>
        /// Checks if it's forbidden to release this
        /// <see cref="PdfObject"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Checks if it's forbidden to release this
        /// <see cref="PdfObject"/>
        /// instance.
        /// Some objects are vital for the living period of
        /// <see cref="PdfDocument"/>
        /// or may be
        /// prevented from releasing by high-level entities dealing with the objects.
        /// Also it's not possible to release the objects that have been modified.
        /// </remarks>
        /// <returns>true if releasing this object is forbidden, otherwise false</returns>
        public virtual bool IsReleaseForbidden() {
            return CheckState(FORBID_RELEASE);
        }

        public virtual void Release() {
            // In case ForbidRelease flag is set, release will not be performed.
            if (IsReleaseForbidden()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(PdfObject));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.FORBID_RELEASE_IS_SET);
            }
            else {
                if (indirectReference != null && indirectReference.GetReader() != null && !indirectReference.CheckState(FLUSHED
                    )) {
                    indirectReference.refersTo = null;
                    indirectReference = null;
                    SetState(READ_ONLY);
                }
            }
        }

        // TODO DEVSIX-4020. Log reasonless call of method
        /// <summary>
        /// Checks if this <c>PdfObject</c> is of the type
        /// <c>PdfNull</c>.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c></returns>
        public virtual bool IsNull() {
            return GetObjectType() == NULL;
        }

        /// <summary>
        /// Checks if this <c>PdfObject</c> is of the type
        /// <c>PdfBoolean</c>.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c></returns>
        public virtual bool IsBoolean() {
            return GetObjectType() == BOOLEAN;
        }

        /// <summary>
        /// Checks if this <c>PdfObject</c> is of the type
        /// <c>PdfNumber</c>.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c></returns>
        public virtual bool IsNumber() {
            return GetObjectType() == NUMBER;
        }

        /// <summary>
        /// Checks if this <c>PdfObject</c> is of the type
        /// <c>PdfString</c>.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c></returns>
        public virtual bool IsString() {
            return GetObjectType() == STRING;
        }

        /// <summary>
        /// Checks if this <c>PdfObject</c> is of the type
        /// <c>PdfName</c>.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c></returns>
        public virtual bool IsName() {
            return GetObjectType() == NAME;
        }

        /// <summary>
        /// Checks if this <c>PdfObject</c> is of the type
        /// <c>PdfArray</c>.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c></returns>
        public virtual bool IsArray() {
            return GetObjectType() == ARRAY;
        }

        /// <summary>
        /// Checks if this <c>PdfObject</c> is of the type
        /// <c>PdfDictionary</c>.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c></returns>
        public virtual bool IsDictionary() {
            return GetObjectType() == DICTIONARY;
        }

        /// <summary>
        /// Checks if this <c>PdfObject</c> is of the type
        /// <c>PdfStream</c>.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c></returns>
        public virtual bool IsStream() {
            return GetObjectType() == STREAM;
        }

        /// <summary>
        /// Checks if this <c>PdfObject</c> is of the type
        /// <c>PdfIndirectReference</c>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this is an indirect reference,
        /// otherwise <c>false</c>
        /// </returns>
        public virtual bool IsIndirectReference() {
            return GetObjectType() == INDIRECT_REFERENCE;
        }

        protected internal virtual PdfObject SetIndirectReference(PdfIndirectReference indirectReference) {
            this.indirectReference = indirectReference;
            return this;
        }

        /// <summary>
        /// Checks if this <c>PdfObject</c> is of the type
        /// <c>PdfLiteral</c>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this is a literal,
        /// otherwise <c>false</c>
        /// </returns>
        public virtual bool IsLiteral() {
            return GetObjectType() == LITERAL;
        }

        /// <summary>Creates new instance of object.</summary>
        /// <returns>new instance of object.</returns>
        protected internal abstract PdfObject NewInstance();

        /// <summary>Checks state of the flag of current object.</summary>
        /// <param name="state">special flag to check</param>
        /// <returns>true if the state was set.</returns>
        protected internal virtual bool CheckState(short state) {
            return (this.state & state) == state;
        }

        /// <summary>Sets special states of current object.</summary>
        /// <param name="state">special flag of current object</param>
        /// <returns>
        /// this
        /// <see cref="PdfObject"/>
        /// </returns>
        protected internal virtual PdfObject SetState(short state) {
            this.state |= state;
            return this;
        }

        /// <summary>Clear state of the flag of current object.</summary>
        /// <param name="state">special flag state to clear</param>
        /// <returns>
        /// this
        /// <see cref="PdfObject"/>
        /// </returns>
        protected internal virtual PdfObject ClearState(short state) {
            this.state &= (short)~state;
            return this;
        }

        /// <summary>Copies object content from object 'from'.</summary>
        /// <param name="from">object to copy content from.</param>
        /// <param name="document">document to copy object to.</param>
        protected internal virtual void CopyContent(PdfObject from, PdfDocument document) {
            CopyContent(from, document, NullCopyFilter.GetInstance());
        }

        /// <summary>Copies object content from object 'from'.</summary>
        /// <param name="from">object to copy content from.</param>
        /// <param name="document">document to copy object to.</param>
        /// <param name="filter">
        /// 
        /// <see cref="iText.Kernel.Utils.ICopyFilter"/>
        /// a filter that will apply on dictionaries and array
        /// Use
        /// <see cref="iText.Kernel.Utils.NullCopyFilter"/>
        /// for no filtering
        /// </param>
        protected internal virtual void CopyContent(PdfObject from, PdfDocument document, ICopyFilter filter) {
            if (IsFlushed()) {
                throw new PdfException(KernelExceptionMessageConstant.CANNOT_COPY_FLUSHED_OBJECT, this);
            }
        }

        internal static bool EqualContent(PdfObject obj1, PdfObject obj2) {
            PdfObject direct1 = obj1 != null && obj1.IsIndirectReference() ? ((PdfIndirectReference)obj1).GetRefersTo(
                true) : obj1;
            PdfObject direct2 = obj2 != null && obj2.IsIndirectReference() ? ((PdfIndirectReference)obj2).GetRefersTo(
                true) : obj2;
            return direct1 != null && direct1.Equals(direct2);
        }

        /// <summary>
        /// Processes two cases of object copying:
        /// <list type="number">
        /// <item><description>copying to the other document
        /// </description></item>
        /// <item><description>cloning inside of the current document
        /// </description></item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Processes two cases of object copying:
        /// <list type="number">
        /// <item><description>copying to the other document
        /// </description></item>
        /// <item><description>cloning inside of the current document
        /// </description></item>
        /// </list>
        /// <para />
        /// This two cases are distinguished by the state of
        /// <c>document</c>
        /// parameter:
        /// the second case is processed if
        /// <c>document</c>
        /// is
        /// <see langword="null"/>.
        /// </remarks>
        /// <param name="documentTo">if not null: document to copy object to; otherwise indicates that object is to be cloned.
        ///     </param>
        /// <param name="allowDuplicating">
        /// indicates if to allow copy objects which already have been copied.
        /// If object is associated with any indirect reference and allowDuplicating is false then
        /// already existing reference will be returned instead of copying object.
        /// If allowDuplicating is true then object will be copied and new indirect
        /// reference will be assigned.
        /// </param>
        /// <returns>copied object.</returns>
        internal virtual PdfObject ProcessCopying(PdfDocument documentTo, bool allowDuplicating) {
            return ProcessCopying(documentTo, allowDuplicating, NullCopyFilter.GetInstance());
        }

        /// <summary>
        /// Processes two cases of object copying:
        /// <list type="number">
        /// <item><description>copying to the other document
        /// </description></item>
        /// <item><description>cloning inside of the current document
        /// </description></item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Processes two cases of object copying:
        /// <list type="number">
        /// <item><description>copying to the other document
        /// </description></item>
        /// <item><description>cloning inside of the current document
        /// </description></item>
        /// </list>
        /// <para />
        /// This two cases are distinguished by the state of
        /// <c>document</c>
        /// parameter:
        /// the second case is processed if
        /// <c>document</c>
        /// is
        /// <see langword="null"/>.
        /// </remarks>
        /// <param name="documentTo">if not null: document to copy object to; otherwise indicates that object is to be cloned.
        ///     </param>
        /// <param name="allowDuplicating">
        /// indicates if to allow copy objects which already have been copied.
        /// If object is associated with any indirect reference and allowDuplicating is false then
        /// already existing reference will be returned instead of copying object.
        /// If allowDuplicating is true then object will be copied and new indirect reference will
        /// be assigned.
        /// </param>
        /// <param name="filter">filters what will be copies or not</param>
        /// <returns>copied object.</returns>
        internal virtual PdfObject ProcessCopying(PdfDocument documentTo, bool allowDuplicating, ICopyFilter filter
            ) {
            if (documentTo != null) {
                //copyTo case
                PdfWriter writer = documentTo.GetWriter();
                if (writer == null) {
                    throw new PdfException(KernelExceptionMessageConstant.CANNOT_COPY_TO_DOCUMENT_OPENED_IN_READING_MODE);
                }
                return writer.CopyObject(this, documentTo, allowDuplicating, filter);
            }
            else {
                //clone case
                PdfObject obj = this;
                if (obj.IsIndirectReference()) {
                    PdfObject refTo = ((PdfIndirectReference)obj).GetRefersTo();
                    obj = refTo != null ? refTo : obj;
                }
                if (obj.IsIndirect() && !allowDuplicating) {
                    return obj;
                }
                return obj.Clone();
            }
        }
    }
}
