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
using iText.Commons.Utils;
using iText.Kernel.Pdf.Xobject;

namespace iText.StyledXmlParser.Resolver.Resource {
    /// <summary>Simple implementation of an image cache.</summary>
    internal class SimpleImageCache {
        /// <summary>The cache mapping a source path to an Image XObject.</summary>
        private IDictionary<String, PdfXObject> cache = new LinkedDictionary<String, PdfXObject>();

        /// <summary>Stores how many times each image is used.</summary>
        private IDictionary<String, int?> imagesFrequency = new LinkedDictionary<String, int?>();

        /// <summary>The capacity of the cache.</summary>
        private int capacity;

        /// <summary>
        /// Creates a new
        /// <see cref="SimpleImageCache"/>
        /// instance.
        /// </summary>
        internal SimpleImageCache() {
            this.capacity = 100;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="SimpleImageCache"/>
        /// instance.
        /// </summary>
        /// <param name="capacity">the capacity</param>
        internal SimpleImageCache(int capacity) {
            if (capacity < 1) {
                throw new ArgumentException("capacity");
            }
            this.capacity = capacity;
        }

        /// <summary>Adds an image to the cache.</summary>
        /// <param name="src">the source path</param>
        /// <param name="imageXObject">the image XObject to be cached</param>
        internal virtual void PutImage(String src, PdfXObject imageXObject) {
            if (cache.ContainsKey(src)) {
                return;
            }
            EnsureCapacity();
            cache.Put(src, imageXObject);
        }

        /// <summary>Gets an image from the cache.</summary>
        /// <param name="src">the source path</param>
        /// <returns>the image XObject</returns>
        internal virtual PdfXObject GetImage(String src) {
            int? frequency = imagesFrequency.Get(src);
            if (frequency != null) {
                imagesFrequency.Put(src, frequency + 1);
            }
            else {
                imagesFrequency.Put(src, 1);
            }
            return cache.Get(src);
        }

        /// <summary>Gets the size of the cache.</summary>
        /// <returns>the cache size</returns>
        internal virtual int Size() {
            return cache.Count;
        }

        /// <summary>Resets the cache.</summary>
        internal virtual void Reset() {
            cache.Clear();
            imagesFrequency.Clear();
        }

        /// <summary>
        /// Ensures the capacity of the cache by removing the least important images
        /// (based on the number of times an image is used).
        /// </summary>
        private void EnsureCapacity() {
            if (cache.Count >= capacity) {
                String mostUnpopularImg = null;
                int minFrequency = int.MaxValue;
                // the keySet method preserves the LinkedList order.
                foreach (String imgSrc in cache.Keys) {
                    int? imgFrequency = imagesFrequency.Get(imgSrc);
                    if (imgFrequency == null || imgFrequency < minFrequency) {
                        mostUnpopularImg = imgSrc;
                        if (imgFrequency == null) {
                            break;
                        }
                        else {
                            minFrequency = (int)imgFrequency;
                        }
                    }
                }
                cache.JRemove(mostUnpopularImg);
            }
        }
    }
}
