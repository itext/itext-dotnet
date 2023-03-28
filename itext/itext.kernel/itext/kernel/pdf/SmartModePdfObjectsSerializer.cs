/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Crypto;
using iText.IO.Source;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
    internal class SmartModePdfObjectsSerializer {
        private IIDigest sha512;

        private Dictionary<SerializedObjectContent, PdfIndirectReference> serializedContentToObj = new Dictionary<
            SerializedObjectContent, PdfIndirectReference>();

        internal SmartModePdfObjectsSerializer() {
            try {
                sha512 = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("SHA-512");
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
                ByteBuffer bb = new ByteBuffer();
                int level = 100;
                try {
                    SerObject(obj, bb, level, serializedCache);
                }
                catch (SmartModePdfObjectsSerializer.SelfReferenceException) {
                    return null;
                }
                content = bb.ToByteArray();
            }
            return new SerializedObjectContent(content);
        }

        private void SerObject(PdfObject obj, ByteBuffer bb, int level, IDictionary<PdfIndirectReference, byte[]> 
            serializedCache) {
            if (level <= 0) {
                return;
            }
            if (obj == null) {
                bb.Append("$Lnull");
                return;
            }
            PdfIndirectReference reference = null;
            ByteBuffer savedBb = null;
            if (obj.IsIndirectReference()) {
                reference = (PdfIndirectReference)obj;
                byte[] cached = serializedCache.Get(reference);
                if (cached != null) {
                    bb.Append(cached);
                    return;
                }
                else {
                    if (serializedCache.Keys.Contains(reference)) {
                        //referencing itself
                        throw new SmartModePdfObjectsSerializer.SelfReferenceException();
                    }
                    serializedCache.Put(reference, null);
                    savedBb = bb;
                    bb = new ByteBuffer();
                    obj = reference.GetRefersTo();
                }
            }
            if (obj.IsStream()) {
                SerDic((PdfDictionary)obj, bb, level - 1, serializedCache);
                bb.Append("$B");
                if (level > 0) {
                    bb.Append(sha512.Digest(((PdfStream)obj).GetBytes(false)));
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
                            // TODO specify length for strings, streams, may be names?
                            bb.Append("$S").Append(obj.ToString());
                        }
                        else {
                            if (obj.IsName()) {
                                bb.Append("$N").Append(obj.ToString());
                            }
                            else {
                                // PdfNull case is also here
                                bb.Append("$L").Append(obj.ToString());
                            }
                        }
                    }
                }
            }
            if (savedBb != null) {
                serializedCache.Put(reference, bb.ToByteArray());
                savedBb.Append(bb.GetInternalBuffer(), 0, bb.Size());
            }
        }

        private void SerDic(PdfDictionary dic, ByteBuffer bb, int level, IDictionary<PdfIndirectReference, byte[]>
             serializedCache) {
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

        private void SerArray(PdfArray array, ByteBuffer bb, int level, IDictionary<PdfIndirectReference, byte[]> 
            serializedCache) {
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

        private class SelfReferenceException : Exception {
        }
    }
}
