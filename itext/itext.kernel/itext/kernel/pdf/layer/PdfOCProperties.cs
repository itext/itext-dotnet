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
using iText.IO.Font;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Layer {
    /// <summary>
    /// This class represents /OCProperties entry if pdf catalog and manages
    /// the layers of the pdf document.
    /// </summary>
    /// <remarks>
    /// This class represents /OCProperties entry if pdf catalog and manages
    /// the layers of the pdf document.
    /// <para />
    /// To be able to be wrapped with this
    /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}"/>
    /// the
    /// <see cref="iText.Kernel.Pdf.PdfObject"/>
    /// must be indirect.
    /// </remarks>
    public class PdfOCProperties : PdfObjectWrapper<PdfDictionary> {
        internal const String OC_CONFIG_NAME_PATTERN = "OCConfigName";

        private IList<PdfLayer> layers = new List<PdfLayer>();

        /// <summary>Creates a new PdfOCProperties instance.</summary>
        /// <param name="document">the document the optional content belongs to</param>
        public PdfOCProperties(PdfDocument document)
            : this((PdfDictionary)new PdfDictionary().MakeIndirect(document)) {
        }

        /// <summary>
        /// Creates a new PdfOCProperties instance by the dictionary it represents,
        /// the dictionary must be an indirect object.
        /// </summary>
        /// <param name="ocPropertiesDict">the dictionary of optional content properties, must have an indirect reference.
        ///     </param>
        public PdfOCProperties(PdfDictionary ocPropertiesDict)
            : base(ocPropertiesDict) {
            EnsureObjectIsAddedToDocument(ocPropertiesDict);
            ReadLayersFromDictionary();
        }

        /// <summary>
        /// Use this method to set a collection of optional content groups
        /// whose states are intended to follow a "radio button" paradigm.
        /// </summary>
        /// <remarks>
        /// Use this method to set a collection of optional content groups
        /// whose states are intended to follow a "radio button" paradigm.
        /// That is, the state of at most one optional content group
        /// in the array should be ON at a time: if one group is turned
        /// ON, all others must be turned OFF.
        /// </remarks>
        /// <param name="group">the radio group</param>
        public virtual void AddOCGRadioGroup(IList<PdfLayer> group) {
            PdfArray ar = new PdfArray();
            foreach (PdfLayer layer in group) {
                if (layer.GetTitle() == null) {
                    ar.Add(layer.GetPdfObject().GetIndirectReference());
                }
            }
            if (ar.Size() != 0) {
                PdfDictionary d = GetPdfObject().GetAsDictionary(PdfName.D);
                if (d == null) {
                    d = new PdfDictionary();
                    GetPdfObject().Put(PdfName.D, d);
                }
                PdfArray radioButtonGroups = d.GetAsArray(PdfName.RBGroups);
                if (radioButtonGroups == null) {
                    radioButtonGroups = new PdfArray();
                    d.Put(PdfName.RBGroups, radioButtonGroups);
                    d.SetModified();
                }
                else {
                    radioButtonGroups.SetModified();
                }
                radioButtonGroups.Add(ar);
            }
        }

        /// <summary>Fills the underlying PdfDictionary object with the current layers and their settings.</summary>
        /// <remarks>
        /// Fills the underlying PdfDictionary object with the current layers and their settings.
        /// Note that it completely regenerates the dictionary, so your direct changes to the dictionary
        /// will not take any affect.
        /// </remarks>
        /// <returns>the resultant dictionary</returns>
        public virtual PdfObject FillDictionary() {
            return this.FillDictionary(true);
        }

        /// <summary>Fills the underlying PdfDictionary object with the current layers and their settings.</summary>
        /// <remarks>
        /// Fills the underlying PdfDictionary object with the current layers and their settings.
        /// Note that it completely regenerates the dictionary, so your direct changes to the dictionary
        /// will not take any affect.
        /// </remarks>
        /// <param name="removeNonDocumentOcgs">
        /// the flag indicating whether it is necessary
        /// to delete OCGs not from the current document
        /// </param>
        /// <returns>the resultant dictionary</returns>
        public virtual PdfObject FillDictionary(bool removeNonDocumentOcgs) {
            PdfArray gr = new PdfArray();
            foreach (PdfLayer layer in layers) {
                if (layer.GetTitle() == null) {
                    gr.Add(layer.GetIndirectReference());
                }
            }
            GetPdfObject().Put(PdfName.OCGs, gr);
            // Save radio groups.
            PdfArray rbGroups = null;
            PdfDictionary d = GetPdfObject().GetAsDictionary(PdfName.D);
            if (d != null) {
                rbGroups = d.GetAsArray(PdfName.RBGroups);
            }
            d = new PdfDictionary();
            if (rbGroups != null) {
                d.Put(PdfName.RBGroups, rbGroups);
            }
            d.Put(PdfName.Name, new PdfString(CreateUniqueName(), PdfEncodings.UNICODE_BIG));
            GetPdfObject().Put(PdfName.D, d);
            IList<PdfLayer> docOrder = new List<PdfLayer>(layers);
            for (int i = 0; i < docOrder.Count; i++) {
                PdfLayer layer = docOrder[i];
                if (layer.GetParent() != null) {
                    docOrder.Remove(layer);
                    i--;
                }
            }
            PdfArray order = new PdfArray();
            foreach (Object element in docOrder) {
                PdfLayer layer = (PdfLayer)element;
                GetOCGOrder(order, layer);
            }
            d.Put(PdfName.Order, order);
            PdfArray off = new PdfArray();
            foreach (Object element in layers) {
                PdfLayer layer = (PdfLayer)element;
                if (layer.GetTitle() == null && !layer.IsOn()) {
                    off.Add(layer.GetIndirectReference());
                }
            }
            if (off.Size() > 0) {
                d.Put(PdfName.OFF, off);
            }
            else {
                d.Remove(PdfName.OFF);
            }
            PdfArray locked = new PdfArray();
            foreach (PdfLayer layer in layers) {
                if (layer.GetTitle() == null && layer.IsLocked()) {
                    locked.Add(layer.GetIndirectReference());
                }
            }
            if (locked.Size() > 0) {
                d.Put(PdfName.Locked, locked);
            }
            else {
                d.Remove(PdfName.Locked);
            }
            d.Remove(PdfName.AS);
            AddASEvent(PdfName.View, PdfName.Zoom);
            AddASEvent(PdfName.View, PdfName.View);
            AddASEvent(PdfName.Print, PdfName.Print);
            AddASEvent(PdfName.Export, PdfName.Export);
            if (removeNonDocumentOcgs) {
                this.RemoveNotRegisteredOcgs();
            }
            return GetPdfObject();
        }

        public override void Flush() {
            FillDictionary();
            base.Flush();
        }

        /// <summary>Gets the list of all the layers currently registered in the OCProperties.</summary>
        /// <remarks>
        /// Gets the list of all the layers currently registered in the OCProperties.
        /// Note that this is just a new list and modifications to it will not affect anything.
        /// </remarks>
        /// <returns>
        /// list of all the
        /// <see cref="PdfLayer">layers</see>
        /// currently registered in the OCProperties
        /// </returns>
        public virtual IList<PdfLayer> GetLayers() {
            return new List<PdfLayer>(layers);
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>This method registers a new layer in the OCProperties.</summary>
        /// <param name="layer">the new layer</param>
        protected internal virtual void RegisterLayer(PdfLayer layer) {
            if (layer == null) {
                throw new ArgumentException("layer argument is null");
            }
            layers.Add(layer);
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// that owns that OCProperties.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// that owns that OCProperties
        /// </returns>
        protected internal virtual PdfDocument GetDocument() {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }

        /// <summary>
        /// Gets the order of the layers in which they will be displayed in the layer view panel,
        /// including nesting.
        /// </summary>
        private static void GetOCGOrder(PdfArray order, PdfLayer layer) {
            if (!layer.IsOnPanel()) {
                return;
            }
            if (layer.GetTitle() == null) {
                order.Add(layer.GetPdfObject().GetIndirectReference());
            }
            IList<PdfLayer> children = layer.GetChildren();
            if (children == null) {
                return;
            }
            PdfArray kids = new PdfArray();
            if (layer.GetTitle() != null) {
                kids.Add(new PdfString(layer.GetTitle(), PdfEncodings.UNICODE_BIG));
            }
            foreach (PdfLayer child in children) {
                GetOCGOrder(kids, child);
            }
            if (kids.Size() > 0) {
                order.Add(kids);
            }
        }

        private void RemoveNotRegisteredOcgs() {
            PdfDictionary dDict = GetPdfObject().GetAsDictionary(PdfName.D);
            PdfDictionary ocProperties = this.GetDocument().GetCatalog().GetPdfObject().GetAsDictionary(PdfName.OCProperties
                );
            ICollection<PdfIndirectReference> ocgsFromDocument = new HashSet<PdfIndirectReference>();
            if (ocProperties.GetAsArray(PdfName.OCGs) != null) {
                PdfArray ocgs = ocProperties.GetAsArray(PdfName.OCGs);
                foreach (PdfObject ocgObj in ocgs) {
                    if (ocgObj.IsDictionary()) {
                        ocgsFromDocument.Add(ocgObj.GetIndirectReference());
                    }
                }
            }
            // Remove from RBGroups OCGs not presented in the output document (in OCProperties/OCGs)
            PdfArray rbGroups = dDict.GetAsArray(PdfName.RBGroups);
            if (rbGroups != null) {
                foreach (PdfObject rbGroupObj in rbGroups) {
                    PdfArray rbGroup = (PdfArray)rbGroupObj;
                    for (int i = rbGroup.Size() - 1; i > -1; i--) {
                        if (!ocgsFromDocument.Contains(rbGroup.Get(i).GetIndirectReference())) {
                            rbGroup.Remove(i);
                        }
                    }
                }
            }
        }

        /// <summary>Populates the /AS entry in the /D dictionary.</summary>
        private void AddASEvent(PdfName @event, PdfName category) {
            PdfArray arr = new PdfArray();
            foreach (PdfLayer layer in layers) {
                if (layer.GetTitle() == null && !layer.GetPdfObject().IsFlushed()) {
                    PdfDictionary usage = layer.GetPdfObject().GetAsDictionary(PdfName.Usage);
                    if (usage != null && usage.Get(category) != null) {
                        arr.Add(layer.GetPdfObject().GetIndirectReference());
                    }
                }
            }
            if (arr.Size() == 0) {
                return;
            }
            PdfDictionary d = GetPdfObject().GetAsDictionary(PdfName.D);
            PdfArray arras = d.GetAsArray(PdfName.AS);
            if (arras == null) {
                arras = new PdfArray();
                d.Put(PdfName.AS, arras);
            }
            PdfDictionary @as = new PdfDictionary();
            @as.Put(PdfName.Event, @event);
            PdfArray categoryArray = new PdfArray();
            categoryArray.Add(category);
            @as.Put(PdfName.Category, categoryArray);
            @as.Put(PdfName.OCGs, arr);
            arras.Add(@as);
        }

        /// <summary>Reads the layers from the document to be able to modify them in the future.</summary>
        private void ReadLayersFromDictionary() {
            PdfArray ocgs = GetPdfObject().GetAsArray(PdfName.OCGs);
            if (ocgs == null || ocgs.IsEmpty()) {
                return;
            }
            IDictionary<PdfIndirectReference, PdfLayer> layerMap = new SortedDictionary<PdfIndirectReference, PdfLayer
                >();
            for (int ind = 0; ind < ocgs.Size(); ind++) {
                PdfLayer currentLayer = new PdfLayer((PdfDictionary)ocgs.GetAsDictionary(ind).MakeIndirect(GetDocument()));
                // We will set onPanel to true later for the objects present in /D->/Order entry.
                currentLayer.onPanel = false;
                layerMap.Put(currentLayer.GetIndirectReference(), currentLayer);
            }
            PdfDictionary d = GetPdfObject().GetAsDictionary(PdfName.D);
            if (d != null && !d.IsEmpty()) {
                PdfArray off = d.GetAsArray(PdfName.OFF);
                if (off != null) {
                    for (int i = 0; i < off.Size(); i++) {
                        PdfObject offLayer = off.Get(i, false);
                        if (offLayer.IsIndirectReference()) {
                            layerMap.Get((PdfIndirectReference)offLayer).on = false;
                        }
                        else {
                            layerMap.Get(offLayer.GetIndirectReference()).on = false;
                        }
                    }
                }
                PdfArray locked = d.GetAsArray(PdfName.Locked);
                if (locked != null) {
                    for (int i = 0; i < locked.Size(); i++) {
                        PdfObject lockedLayer = locked.Get(i, false);
                        if (lockedLayer.IsIndirectReference()) {
                            layerMap.Get((PdfIndirectReference)lockedLayer).locked = true;
                        }
                        else {
                            layerMap.Get(lockedLayer.GetIndirectReference()).locked = true;
                        }
                    }
                }
                PdfArray orderArray = d.GetAsArray(PdfName.Order);
                if (orderArray != null && !orderArray.IsEmpty()) {
                    ReadOrderFromDictionary(null, orderArray, layerMap);
                }
            }
            // Add the layers which should not be displayed on the panel to the order list
            foreach (PdfLayer layer in layerMap.Values) {
                if (!layer.IsOnPanel()) {
                    layers.Add(layer);
                }
            }
        }

        /// <summary>Reads the /Order in the /D entry and initialized the parent-child hierarchy.</summary>
        private void ReadOrderFromDictionary(PdfLayer parent, PdfArray orderArray, IDictionary<PdfIndirectReference
            , PdfLayer> layerMap) {
            for (int i = 0; i < orderArray.Size(); i++) {
                PdfObject item = orderArray.Get(i);
                if (item.GetObjectType() == PdfObject.DICTIONARY) {
                    PdfLayer layer = layerMap.Get(item.GetIndirectReference());
                    if (layer != null) {
                        layers.Add(layer);
                        layer.onPanel = true;
                        if (parent != null) {
                            parent.AddChild(layer);
                        }
                        if (i + 1 < orderArray.Size() && orderArray.Get(i + 1).GetObjectType() == PdfObject.ARRAY) {
                            PdfArray nextArray = orderArray.GetAsArray(i + 1);
                            if (nextArray.Size() > 0 && nextArray.Get(0).GetObjectType() != PdfObject.STRING) {
                                ReadOrderFromDictionary(layer, orderArray.GetAsArray(i + 1), layerMap);
                                i++;
                            }
                        }
                    }
                }
                else {
                    if (item.GetObjectType() == PdfObject.ARRAY) {
                        PdfArray subArray = (PdfArray)item;
                        if (subArray.IsEmpty()) {
                            continue;
                        }
                        PdfObject firstObj = subArray.Get(0);
                        if (firstObj.GetObjectType() == PdfObject.STRING) {
                            PdfLayer titleLayer = PdfLayer.CreateTitleSilent(((PdfString)firstObj).ToUnicodeString(), GetDocument());
                            titleLayer.onPanel = true;
                            layers.Add(titleLayer);
                            if (parent != null) {
                                parent.AddChild(titleLayer);
                            }
                            ReadOrderFromDictionary(titleLayer, new PdfArray(subArray.SubList(1, subArray.Size())), layerMap);
                        }
                        else {
                            ReadOrderFromDictionary(parent, subArray, layerMap);
                        }
                    }
                }
            }
        }

        private String CreateUniqueName() {
            int uniqueID = 0;
            ICollection<String> usedNames = new HashSet<String>();
            PdfArray configs = GetPdfObject().GetAsArray(PdfName.Configs);
            if (null != configs) {
                for (int i = 0; i < configs.Size(); i++) {
                    PdfDictionary alternateDictionary = configs.GetAsDictionary(i);
                    if (null != alternateDictionary && alternateDictionary.ContainsKey(PdfName.Name)) {
                        usedNames.Add(alternateDictionary.GetAsString(PdfName.Name).ToUnicodeString());
                    }
                }
            }
            while (usedNames.Contains(OC_CONFIG_NAME_PATTERN + uniqueID)) {
                uniqueID++;
            }
            return OC_CONFIG_NAME_PATTERN + uniqueID;
        }
    }
}
