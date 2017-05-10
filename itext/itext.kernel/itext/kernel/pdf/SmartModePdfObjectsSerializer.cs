using System;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto;
using iText.Kernel;

namespace iText.Kernel.Pdf {
    internal class SmartModePdfObjectsSerializer {
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

        public virtual void SaveSerializedObject(SerializedObjectContent objectKey, PdfIndirectReference reference
            ) {
            serializedContentToObj.Put(objectKey, reference);
        }

        public virtual PdfIndirectReference GetSavedSerializedObject(SerializedObjectContent serializedContent) {
            if (serializedContent != null) {
                return serializedContentToObj.Get(serializedContent);
            }
            else {
                return null;
            }
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
            PdfDocument.IndirectRefDescription indRefKey = null;
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
                    md5.Reset();
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
            PdfName[] keys = new PdfName[dic.KeySet().Count];
            keys = dic.KeySet().ToArray(keys);
            iText.IO.Util.JavaUtil.Sort(keys);
            foreach (Object key in keys) {
                if (key.Equals(PdfName.P) && (dic.Get((PdfName)key).IsIndirectReference() || dic.Get((PdfName)key).IsDictionary
                    ()) || key.Equals(PdfName.Parent)) {
                    // ignore recursive call
                    continue;
                }
                SerObject((PdfObject)key, bb, level, serializedCache);
                SerObject(dic.Get((PdfName)key, false), bb, level, serializedCache);
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
    }
}
