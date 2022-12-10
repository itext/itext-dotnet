/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using iText.Commons.Bouncycastle.Crypto;
using iText.IO.Source;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
    internal class SmartModePdfObjectsSerializer {
        private IIDigest md5;

        private Dictionary<SerializedObjectContent, PdfIndirectReference> serializedContentToObj = new Dictionary<
            SerializedObjectContent, PdfIndirectReference>();

        internal SmartModePdfObjectsSerializer() {
            try {
                md5 = iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateIDigest("MD5");
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
