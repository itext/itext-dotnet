/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Pdf.Xobject;

namespace iText.StyledXmlParser.Resolver.Resource {
//\cond DO_NOT_DOCUMENT
    /// <summary>Simple implementation of an image cache.</summary>
    internal class SimpleImageCache {
        /// <summary>The cache mapping a source path to an Image XObject.</summary>
        private IDictionary<String, PdfXObject> cache = new LinkedDictionary<String, PdfXObject>();

        /// <summary>Stores how many times each image is used.</summary>
        private IDictionary<String, int?> imagesFrequency = new LinkedDictionary<String, int?>();

        /// <summary>The capacity of the cache.</summary>
        private int capacity;

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Creates a new
        /// <see cref="SimpleImageCache"/>
        /// instance.
        /// </summary>
        internal SimpleImageCache() {
            this.capacity = 100;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets the size of the cache.</summary>
        /// <returns>the cache size</returns>
        internal virtual int Size() {
            return cache.Count;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Resets the cache.</summary>
        internal virtual void Reset() {
            cache.Clear();
            imagesFrequency.Clear();
        }
//\endcond

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
//\endcond
}
