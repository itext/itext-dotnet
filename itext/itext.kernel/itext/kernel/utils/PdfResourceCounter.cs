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
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Utils {
    /// <summary>
    /// This class can be used to count the number of bytes needed when copying
    /// pages from an existing PDF into a newly created PDF.
    /// </summary>
    public class PdfResourceCounter {
        /// <summary>A map of the resources that are already taken into account</summary>
        private IDictionary<int, PdfObject> resources;

        /// <summary>
        /// Creates a PdfResourceCounter instance to be used to count the resources
        /// needed for either a page (in this case pass a page dictionary) or the
        /// trailer (root and info dictionary) of a PDF file.
        /// </summary>
        /// <param name="obj">the object we want to examine</param>
        public PdfResourceCounter(PdfObject obj) {
            resources = new Dictionary<int, PdfObject>();
            Process(obj);
        }

        /// <summary>Processes an object.</summary>
        /// <remarks>
        /// Processes an object. If the object is indirect, it is added to the
        /// list of resources. If not, it is just processed.
        /// </remarks>
        /// <param name="obj">the object to process</param>
        protected internal void Process(PdfObject obj) {
            PdfIndirectReference @ref = obj.GetIndirectReference();
            if (@ref == null) {
                LoopOver(obj);
            }
            else {
                if (!resources.ContainsKey(@ref.GetObjNumber())) {
                    resources.Put(@ref.GetObjNumber(), obj);
                    LoopOver(obj);
                }
            }
        }

        /// <summary>
        /// In case an object is an array, a dictionary or a stream,
        /// we need to loop over the entries and process them one by one.
        /// </summary>
        /// <param name="obj">the object to examine</param>
        protected internal void LoopOver(PdfObject obj) {
            switch (obj.GetObjectType()) {
                case PdfObject.ARRAY: {
                    PdfArray array = (PdfArray)obj;
                    for (int i = 0; i < array.Size(); i++) {
                        Process(array.Get(i));
                    }
                    break;
                }

                case PdfObject.DICTIONARY:
                case PdfObject.STREAM: {
                    PdfDictionary dict = (PdfDictionary)obj;
                    if (PdfName.Pages.Equals(dict.Get(PdfName.Type))) {
                        break;
                    }
                    foreach (PdfName name in dict.KeySet()) {
                        Process(dict.Get(name));
                    }
                    break;
                }
            }
        }

        /// <summary>Returns a map with the resources.</summary>
        /// <returns>the resources</returns>
        public virtual IDictionary<int, PdfObject> GetResources() {
            return resources;
        }

        /// <summary>
        /// Returns the resources needed for the object that was used to create
        /// this PdfResourceCounter.
        /// </summary>
        /// <remarks>
        /// Returns the resources needed for the object that was used to create
        /// this PdfResourceCounter. If you pass a Map with resources that were
        /// already used by other objects, these objects will not be taken into
        /// account.
        /// </remarks>
        /// <param name="res">The resources that can be excluded when counting the bytes.</param>
        /// <returns>The number of bytes needed for an object.</returns>
        public virtual long GetLength(IDictionary<int, PdfObject> res) {
            long length = 0;
            foreach (int @ref in resources.Keys) {
                if (res != null && res.ContainsKey(@ref)) {
                    continue;
                }
                PdfOutputStream os = new PdfOutputStream(new IdleOutputStream());
                os.Write(resources.Get(@ref).Clone());
                length += os.GetCurrentPos();
            }
            return length;
        }
    }
}
