using System;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto;
using iText.Kernel;

namespace iText.Kernel.Pdf {
    internal class SmartModePdfObjectsSerializer {
        [System.NonSerialized]
        private IDigest md5;

        private Dictionary<SerializedObjectContent, PdfIndirectReference> serializedContentToObj = new Dictionary<
            SerializedObjectContent, PdfIndirectReference>();

        internal SmartModePdfObjectsSerializer() {
            try {
                md5 = Org.BouncyCastle.Security.DigestUtilities.GetDigest("MD5");
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        public virtual void SaveSerializedObject(SerializedObjectContent serializedContent, PdfIndirectReference objectReference
            ) {
            serializedContentToObj.Put(serializedContent, objectReference);
        }

        public virtual PdfIndirectReference GetSavedSerializedObject(SerializedObjectContent serializedContent) {
            if (serializedContent != null) {
                return serializedContentToObj.Get(serializedContent);
            }
            return null;
        }

        public virtual SerializedObjectContent SerializeObject(PdfObject obj) {
            if (!obj.IsStream() && !obj.IsDictionary()) {
                return null;
            }
            PdfIndirectReference indRef = obj.GetIndirectReference();
            System.Diagnostics.Debug.Assert(indRef != null);
            IDictionary<PdfIndirectReference, byte[]> serializedCache = indRef.GetDocument().serializedObjectsCache;
            byte[] content = serializedCache.Get(indRef);
            if (content == null) {
                ByteBufferOutputStream bb = new ByteBufferOutputStream();
                int level = 100;
                SerObject(obj, bb, level, serializedCache);
                content = bb.ToByteArray();
            }
            return new SerializedObjectContent(content);
        }

        private void SerObject(PdfObject obj, ByteBufferOutputStream bb, int level, IDictionary<PdfIndirectReference
            , byte[]> serializedCache) {
            if (level <= 0) {
                return;
            }
            if (obj == null) {
                bb.Append("$Lnull");
                return;
            }
            PdfIndirectReference reference = null;
            ByteBufferOutputStream savedBb = null;
            if (obj.IsIndirectReference()) {
                reference = (PdfIndirectReference)obj;
                byte[] cached = serializedCache.Get(reference);
                if (cached != null) {
                    bb.Append(cached);
                    return;
                }
                else {
                    savedBb = bb;
                    bb = new ByteBufferOutputStream();
                    obj = reference.GetRefersTo();
                }
            }
            if (obj.IsStream()) {
                SerDic((PdfDictionary)obj, bb, level - 1, serializedCache);
                bb.Append("$B");
                if (level > 0) {
                    bb.Append(md5.Digest(((PdfStream)obj).GetBytes(false)));
                }
            }
            else {
                if (obj.IsDictionary()) {
                    SerDic((PdfDictionary)obj, bb, level - 1, serializedCache);
                }
                else {
                    if (obj.IsArray()) {
                        SerArray((PdfArray)obj, bb, level - 1, serializedCache);
                    }
                    else {
                        if (obj.IsString()) {
                            bb.Append("$S").Append(obj.ToString());
                        }
                        else {
                            // TODO specify length for strings, streams, may be names?
                            if (obj.IsName()) {
                                bb.Append("$N").Append(obj.ToString());
                            }
                            else {
                                bb.Append("$L").Append(obj.ToString());
                            }
                        }
                    }
                }
            }
            // PdfNull case is also here
            if (savedBb != null) {
                // TODO getBuffer? won't it contain garbage also?
                serializedCache.Put(reference, bb.GetBuffer());
                savedBb.Append(bb);
            }
        }

        private void SerDic(PdfDictionary dic, ByteBufferOutputStream bb, int level, IDictionary<PdfIndirectReference
            , byte[]> serializedCache) {
            bb.Append("$D");
            if (level <= 0) {
                return;
            }
            foreach (PdfName key in dic.KeySet()) {
                if (IsKeyRefersBack(dic, key)) {
                    continue;
                }
                SerObject(key, bb, level, serializedCache);
                SerObject(dic.Get(key, false), bb, level, serializedCache);
            }
            bb.Append("$\\D");
        }

        private void SerArray(PdfArray array, ByteBufferOutputStream bb, int level, IDictionary<PdfIndirectReference
            , byte[]> serializedCache) {
            bb.Append("$A");
            if (level <= 0) {
                return;
            }
            for (int k = 0; k < array.Size(); ++k) {
                SerObject(array.Get(k, false), bb, level, serializedCache);
            }
            bb.Append("$\\A");
        }

        private bool IsKeyRefersBack(PdfDictionary dic, PdfName key) {
            // TODO review this method?
            // ignore recursive call
            return key.Equals(PdfName.P) && (dic.Get(key).IsIndirectReference() || dic.Get(key).IsDictionary()) || key
                .Equals(PdfName.Parent);
        }
    }
}
