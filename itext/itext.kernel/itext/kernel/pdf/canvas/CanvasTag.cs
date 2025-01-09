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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Canvas {
    /// <summary>This class represents a single tag on a single piece of marked content.</summary>
    /// <remarks>
    /// This class represents a single tag on a single piece of marked content.
    /// <para />
    /// In Tagged PDF, a tag is the basic structure unit for marking content. The tag
    /// structure and hierarchy is largely comparable to HTML. As in HTML, every tag
    /// type has a name, defined here in the <c>role</c> attribute. The tagging
    /// mechanism in Tagged PDF is extensible, so PDF creators can choose to create
    /// custom tags.
    /// </remarks>
    public class CanvasTag {
        /// <summary>The type of the tag.</summary>
        protected internal PdfName role;

        /// <summary>The properties of the tag.</summary>
        protected internal PdfDictionary properties;

        /// <summary>
        /// Creates a tag that is referenced to the document's tag structure (i.e.
        /// logical structure).
        /// </summary>
        /// <param name="role">the type of tag</param>
        public CanvasTag(PdfName role) {
            this.role = role;
        }

        /// <summary>
        /// Creates a tag that is referenced to the document's tag structure (i.e.
        /// logical structure).
        /// </summary>
        /// <param name="role">the type of tag</param>
        /// <param name="mcid">marked content id which serves as a reference to the document's logical structure</param>
        public CanvasTag(PdfName role, int mcid) {
            this.role = role;
            AddProperty(PdfName.MCID, new PdfNumber(mcid));
        }

        /// <summary>
        /// Creates a tag that is referenced to the document's tag structure (i.e.
        /// logical structure).
        /// </summary>
        /// <param name="mcr">
        /// the
        /// <see cref="iText.Kernel.Pdf.Tagging.PdfMcr">Marked Content Reference</see>
        /// wrapper object
        /// </param>
        public CanvasTag(PdfMcr mcr)
            : this(mcr.GetRole(), mcr.GetMcid()) {
        }

        /// <summary>Get the role of the tag.</summary>
        /// <returns>the role of the tag as a PdfName</returns>
        public virtual PdfName GetRole() {
            return role;
        }

        /// <summary>Get the marked content id of the tag.</summary>
        /// <returns>marked content id</returns>
        public virtual int GetMcid() {
            int mcid = -1;
            if (properties != null) {
                mcid = (int)properties.GetAsInt(PdfName.MCID);
            }
            if (mcid == -1) {
                throw new InvalidOperationException("CanvasTag has no MCID");
            }
            return mcid;
        }

        /// <summary>Determine if an MCID is available</summary>
        /// <returns>true if the MCID is available, false otherwise</returns>
        public virtual bool HasMcid() {
            return properties != null && properties.ContainsKey(PdfName.MCID);
        }

        /// <summary>
        /// Sets a dictionary of properties to the
        /// <see cref="CanvasTag">tag</see>
        /// 's properties.
        /// </summary>
        /// <remarks>
        /// Sets a dictionary of properties to the
        /// <see cref="CanvasTag">tag</see>
        /// 's properties. All existing properties (if any) will be lost.
        /// </remarks>
        /// <param name="properties">a dictionary</param>
        /// <returns>
        /// current
        /// <see cref="CanvasTag"/>
        /// </returns>
        public virtual iText.Kernel.Pdf.Canvas.CanvasTag SetProperties(PdfDictionary properties) {
            this.properties = properties;
            return this;
        }

        /// <summary>
        /// Adds a single property to the
        /// <see cref="CanvasTag">tag</see>
        /// 's properties.
        /// </summary>
        /// <param name="name">a key</param>
        /// <param name="value">the value for the key</param>
        /// <returns>
        /// current
        /// <see cref="CanvasTag"/>
        /// </returns>
        public virtual iText.Kernel.Pdf.Canvas.CanvasTag AddProperty(PdfName name, PdfObject value) {
            EnsurePropertiesInit();
            properties.Put(name, value);
            return this;
        }

        /// <summary>
        /// Removes a single property from the
        /// <see cref="CanvasTag">tag</see>
        /// 's properties.
        /// </summary>
        /// <param name="name">the key of the key-value pair to be removed</param>
        /// <returns>
        /// current
        /// <see cref="CanvasTag"/>
        /// </returns>
        public virtual iText.Kernel.Pdf.Canvas.CanvasTag RemoveProperty(PdfName name) {
            if (properties != null) {
                properties.Remove(name);
            }
            return this;
        }

        /// <summary>
        /// Gets a property from the
        /// <see cref="CanvasTag">tag</see>
        /// 's properties dictionary.
        /// </summary>
        /// <param name="name">the key of the key-value pair to be retrieved</param>
        /// <returns>the value corresponding to the key</returns>
        public virtual PdfObject GetProperty(PdfName name) {
            if (properties == null) {
                return null;
            }
            return properties.Get(name);
        }

        /// <summary>Get the properties of the tag.</summary>
        /// <returns>properties of the tag</returns>
        public virtual PdfDictionary GetProperties() {
            return properties;
        }

        /// <summary>Gets value of /ActualText property.</summary>
        /// <returns>
        /// actual text value or
        /// <see langword="null"/>
        /// if actual text is not defined
        /// </returns>
        public virtual String GetActualText() {
            return GetPropertyAsString(PdfName.ActualText);
        }

        public virtual String GetExpansionText() {
            return GetPropertyAsString(PdfName.E);
        }

        private String GetPropertyAsString(PdfName name) {
            PdfString text = null;
            if (properties != null) {
                text = properties.GetAsString(name);
            }
            String result = null;
            if (text != null) {
                result = text.ToUnicodeString();
            }
            return result;
        }

        private void EnsurePropertiesInit() {
            if (properties == null) {
                properties = new PdfDictionary();
            }
        }
    }
}
