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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Layer {
    /// <summary>
    /// Content typically belongs to a single optional content group,
    /// and is visible when the group is <b>ON</b> and invisible when it is <b>OFF</b>.
    /// </summary>
    /// <remarks>
    /// Content typically belongs to a single optional content group,
    /// and is visible when the group is <b>ON</b> and invisible when it is <b>OFF</b>. To express more
    /// complex visibility policies, content should not declare itself to belong to an optional
    /// content group directly, but rather to an optional content membership dictionary
    /// represented by this class.
    /// <br /><br />
    /// To be able to be wrapped with this
    /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}"/>
    /// the
    /// <see cref="iText.Kernel.Pdf.PdfObject"/>
    /// must be indirect.
    /// </remarks>
    public class PdfLayerMembership : PdfObjectWrapper<PdfDictionary>, IPdfOCG {
        /// <summary>Creates a new, empty membership layer.</summary>
        /// <param name="doc">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// where a new empty membership layer creates
        /// </param>
        public PdfLayerMembership(PdfDocument doc)
            : base(new PdfDictionary()) {
            MakeIndirect(doc);
            GetPdfObject().Put(PdfName.Type, PdfName.OCMD);
        }

        /// <summary>Creates a new PdfLayerMembership instance by its PdfDictionary, which must be an indirect object.
        ///     </summary>
        /// <param name="membershipDictionary">the membership dictionary, must have an indirect reference.</param>
        public PdfLayerMembership(PdfDictionary membershipDictionary)
            : base(membershipDictionary) {
            EnsureObjectIsAddedToDocument(membershipDictionary);
            if (!PdfName.OCMD.Equals(membershipDictionary.GetAsName(PdfName.Type))) {
                throw new ArgumentException("Invalid membershipDictionary.");
            }
        }

        /// <summary>Gets the collection of the layers this layer membership operates with.</summary>
        /// <returns>
        /// list of
        /// <see cref="PdfLayer">layers</see>
        /// this layer membership operates with
        /// </returns>
        public virtual ICollection<PdfLayer> GetLayers() {
            PdfObject layers = GetPdfObject().Get(PdfName.OCGs);
            if (layers is PdfDictionary) {
                IList<PdfLayer> list = new List<PdfLayer>();
                list.Add(new PdfLayer((PdfDictionary)((PdfDictionary)layers).MakeIndirect(GetDocument())));
                return list;
            }
            else {
                if (layers is PdfArray) {
                    IList<PdfLayer> layerList = new List<PdfLayer>();
                    for (int ind = 0; ind < ((PdfArray)layers).Size(); ind++) {
                        layerList.Add(new PdfLayer(((PdfArray)(((PdfArray)layers).MakeIndirect(GetDocument()))).GetAsDictionary(ind
                            )));
                    }
                    return layerList;
                }
            }
            return null;
        }

        /// <summary>Adds a new layer to the current layer membership.</summary>
        /// <param name="layer">the layer to be added</param>
        public virtual void AddLayer(PdfLayer layer) {
            PdfArray layers = GetPdfObject().GetAsArray(PdfName.OCGs);
            if (layers == null) {
                layers = new PdfArray();
                GetPdfObject().Put(PdfName.OCGs, layers);
            }
            layers.Add(layer.GetPdfObject());
            layers.SetModified();
        }

        /// <summary>
        /// Sets the visibility policy for content belonging to this
        /// membership dictionary.
        /// </summary>
        /// <remarks>
        /// Sets the visibility policy for content belonging to this
        /// membership dictionary. Possible values are AllOn, AnyOn, AnyOff and AllOff.
        /// AllOn - Visible only if all of the entries are <b>ON</b>.
        /// AnyOn - Visible if any of the entries are <b>ON</b>.
        /// AnyOff - Visible if any of the entries are <b>OFF</b>.
        /// AllOff - Visible only if all of the entries are <b>OFF</b>.
        /// The default value is AnyOn.
        /// </remarks>
        /// <param name="visibilityPolicy">the visibility policy</param>
        public virtual void SetVisibilityPolicy(PdfName visibilityPolicy) {
            if (visibilityPolicy == null || !PdfName.AllOn.Equals(visibilityPolicy) && !PdfName.AnyOn.Equals(visibilityPolicy
                ) && !PdfName.AnyOff.Equals(visibilityPolicy) && !PdfName.AllOff.Equals(visibilityPolicy)) {
                throw new ArgumentException("Argument: visibilityPolicy");
            }
            GetPdfObject().Put(PdfName.P, visibilityPolicy);
            GetPdfObject().SetModified();
        }

        /// <summary>Gets the visibility policy for content belonging to this optional content membership dictionary.</summary>
        /// <returns>the visibility policy for content belonging to this membership dictionary</returns>
        public virtual PdfName GetVisibilityPolicy() {
            PdfName visibilityPolicy = GetPdfObject().GetAsName(PdfName.P);
            if (visibilityPolicy == null || !visibilityPolicy.Equals(PdfName.AllOn) && !visibilityPolicy.Equals(PdfName
                .AllOff) && !visibilityPolicy.Equals(PdfName.AnyOn) && !visibilityPolicy.Equals(PdfName.AnyOff)) {
                return PdfName.AnyOn;
            }
            return visibilityPolicy;
        }

        /// <summary>
        /// Sets the visibility expression for content belonging to this
        /// membership dictionary.
        /// </summary>
        /// <param name="visibilityExpression">
        /// A (nested) array of which the first value is /And, /Or, or /Not
        /// followed by a series of indirect references to OCGs or other visibility
        /// expressions.
        /// </param>
        public virtual void SetVisibilityExpression(PdfVisibilityExpression visibilityExpression) {
            GetPdfObject().Put(PdfName.VE, visibilityExpression.GetPdfObject());
            GetPdfObject().SetModified();
        }

        /// <summary>Gets the visibility expression for content belonging to this optional content membership dictionary.
        ///     </summary>
        /// <returns>the visibility expression for content belonging to this membership dictionary, if not set return null
        ///     </returns>
        public virtual PdfVisibilityExpression GetVisibilityExpression() {
            PdfArray ve = GetPdfObject().GetAsArray(PdfName.VE);
            return ve != null ? new PdfVisibilityExpression(ve) : null;
        }

        public virtual PdfIndirectReference GetIndirectReference() {
            return GetPdfObject().GetIndirectReference();
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// that owns that layer membership.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// that owns that layer membership
        /// </returns>
        protected internal virtual PdfDocument GetDocument() {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }
    }
}
