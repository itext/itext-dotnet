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
using iText.IO.Font;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Layer {
    /// <summary>
    /// An optional content group is a dictionary representing a collection of graphics
    /// that can be made visible or invisible dynamically by users of viewer applications.
    /// </summary>
    /// <remarks>
    /// An optional content group is a dictionary representing a collection of graphics
    /// that can be made visible or invisible dynamically by users of viewer applications.
    /// In iText they are referenced as layers.
    /// <br /><br />
    /// To be able to be wrapped with this
    /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}"/>
    /// the
    /// <see cref="iText.Kernel.Pdf.PdfObject"/>
    /// must be indirect.
    /// </remarks>
    public class PdfLayer : PdfObjectWrapper<PdfDictionary>, IPdfOCG {
        /// <summary>Used for titling group of objects but not actually grouping them.</summary>
        protected internal String title;

        protected internal bool on = true;

        protected internal bool onPanel = true;

        protected internal bool locked = false;

        protected internal iText.Kernel.Pdf.Layer.PdfLayer parent;

        protected internal IList<iText.Kernel.Pdf.Layer.PdfLayer> children;

        /// <summary>Creates a new layer by existing dictionary, which must be an indirect object.</summary>
        /// <param name="layerDictionary">the layer dictionary, must have an indirect reference.</param>
        public PdfLayer(PdfDictionary layerDictionary)
            : base(layerDictionary) {
            SetForbidRelease();
            EnsureObjectIsAddedToDocument(layerDictionary);
        }

        /// <summary>Creates a new layer by its name and document.</summary>
        /// <param name="name">the layer name</param>
        /// <param name="document">the PdfDocument which the layer belongs to</param>
        public PdfLayer(String name, PdfDocument document)
            : this(document) {
            SetName(name);
            document.GetCatalog().GetOCProperties(true).RegisterLayer(this);
        }

        private PdfLayer(PdfDocument document)
            : base(new PdfDictionary()) {
            MakeIndirect(document);
            GetPdfObject().Put(PdfName.Type, PdfName.OCG);
        }

        /// <summary>Creates a title layer.</summary>
        /// <remarks>
        /// Creates a title layer. A title layer is not really a layer but a collection of layers
        /// under the same title heading.
        /// </remarks>
        /// <param name="title">the title text</param>
        /// <param name="document">the <c>PdfDocument</c></param>
        /// <returns>the title layer</returns>
        public static iText.Kernel.Pdf.Layer.PdfLayer CreateTitle(String title, PdfDocument document) {
            iText.Kernel.Pdf.Layer.PdfLayer layer = CreateTitleSilent(title, document);
            document.GetCatalog().GetOCProperties(true).RegisterLayer(layer);
            return layer;
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
        /// <param name="document">the <c>PdfDocument</c></param>
        /// <param name="group">the radio group</param>
        public static void AddOCGRadioGroup(PdfDocument document, IList<iText.Kernel.Pdf.Layer.PdfLayer> group) {
            document.GetCatalog().GetOCProperties(true).AddOCGRadioGroup(group);
        }

        /// <summary>Adds a child layer.</summary>
        /// <remarks>Adds a child layer. Nested layers can only have one parent.</remarks>
        /// <param name="childLayer">the child layer</param>
        public virtual void AddChild(iText.Kernel.Pdf.Layer.PdfLayer childLayer) {
            if (childLayer.parent != null) {
                throw new ArgumentException("Illegal argument: childLayer");
            }
            childLayer.parent = this;
            if (children == null) {
                children = new List<iText.Kernel.Pdf.Layer.PdfLayer>();
            }
            children.Add(childLayer);
        }

        /// <summary>Gets the parent of this layer, be it a title layer, or a usual one.</summary>
        /// <returns>the parent of the layer, or null if it has no parent</returns>
        public virtual iText.Kernel.Pdf.Layer.PdfLayer GetParent() {
            return parent;
        }

        /// <summary>Sets the name of the layer to be displayed in the Layers panel.</summary>
        /// <param name="name">the name of the layer.</param>
        public virtual void SetName(String name) {
            GetPdfObject().Put(PdfName.Name, new PdfString(name, PdfEncodings.UNICODE_BIG));
            GetPdfObject().SetModified();
        }

        /// <summary>Gets the initial visibility of the layer when the document is opened.</summary>
        /// <returns>the initial visibility of the layer</returns>
        public virtual bool IsOn() {
            return on;
        }

        /// <summary>Sets the initial visibility of the layer when the document is opened.</summary>
        /// <param name="on">the initial visibility of the layer</param>
        public virtual void SetOn(bool on) {
            if (this.on != on) {
                FetchOCProperties().SetModified();
            }
            this.on = on;
        }

        /// <summary>Gets whether the layer is currently locked or not.</summary>
        /// <remarks>
        /// Gets whether the layer is currently locked or not. If the layer is locked,
        /// it will not be possible to change its state (on/off) in a viewer.
        /// </remarks>
        /// <returns>true if the layer is currently locked, false otherwise.</returns>
        public virtual bool IsLocked() {
            return locked;
        }

        /// <summary>Use this method to lock an optional content group.</summary>
        /// <remarks>
        /// Use this method to lock an optional content group.
        /// The state of a locked group cannot be changed through the user interface
        /// of a viewer application. Producers can use this entry to prevent the visibility
        /// of content that depends on these groups from being changed by users.
        /// </remarks>
        /// <param name="locked">sets whether the layer is currently locked or not</param>
        public virtual void SetLocked(bool locked) {
            if (this.IsLocked() != locked) {
                FetchOCProperties().SetModified();
            }
            this.locked = locked;
        }

        /// <summary>Gets the layer visibility in Acrobat's layer panel</summary>
        /// <returns>the layer visibility in Acrobat's layer panel</returns>
        public virtual bool IsOnPanel() {
            return onPanel;
        }

        /// <summary>Sets the visibility of the layer in Acrobat's layer panel.</summary>
        /// <remarks>
        /// Sets the visibility of the layer in Acrobat's layer panel. If <c>false</c>
        /// the layer cannot be directly manipulated by the user. Note that any children layers will
        /// also be absent from the panel.
        /// </remarks>
        /// <param name="onPanel">the visibility of the layer in Acrobat's layer panel</param>
        public virtual void SetOnPanel(bool onPanel) {
            if (this.on != onPanel) {
                FetchOCProperties().SetModified();
            }
            this.onPanel = onPanel;
        }

        /// <summary>Gets a collection of current intents specified for this layer.</summary>
        /// <remarks>
        /// Gets a collection of current intents specified for this layer.
        /// The default value is
        /// <see cref="iText.Kernel.Pdf.PdfName.View"/>
        /// , so it will be the only element of the
        /// resultant collection if no intents are currently specified.
        /// </remarks>
        /// <returns>the collection of intents.</returns>
        public virtual ICollection<PdfName> GetIntents() {
            PdfObject intent = GetPdfObject().Get(PdfName.Intent);
            if (intent is PdfName) {
                return JavaCollectionsUtil.SingletonList((PdfName)intent);
            }
            else {
                if (intent is PdfArray) {
                    PdfArray intentArr = (PdfArray)intent;
                    ICollection<PdfName> intentsCollection = new List<PdfName>(intentArr.Size());
                    foreach (PdfObject i in intentArr) {
                        if (i is PdfName) {
                            intentsCollection.Add((PdfName)i);
                        }
                    }
                    return intentsCollection;
                }
            }
            return JavaCollectionsUtil.SingletonList(PdfName.View);
        }

        /// <summary>Sets the intents of the layer.</summary>
        /// <param name="intents">the list of intents.</param>
        public virtual void SetIntents(IList<PdfName> intents) {
            if (intents == null || intents.Count == 0) {
                GetPdfObject().Remove(PdfName.Intent);
            }
            else {
                if (intents.Count == 1) {
                    GetPdfObject().Put(PdfName.Intent, intents[0]);
                }
                else {
                    // intents.size() > 1
                    PdfArray array = new PdfArray();
                    foreach (PdfName intent in intents) {
                        array.Add(intent);
                    }
                    GetPdfObject().Put(PdfName.Intent, array);
                }
            }
            GetPdfObject().SetModified();
        }

        /// <summary>
        /// Used by the creating application to store application-specific
        /// data associated with this optional content group.
        /// </summary>
        /// <param name="creator">a text string specifying the application that created the group</param>
        /// <param name="subtype">
        /// a string defining the type of content controlled by the group. Suggested
        /// values include but are not limited to <b>Artwork</b>, for graphic-design or publishing
        /// applications, and <b>Technical</b>, for technical designs such as building plans or
        /// schematics
        /// </param>
        public virtual void SetCreatorInfo(String creator, String subtype) {
            PdfDictionary usage = GetUsage();
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.Creator, new PdfString(creator, PdfEncodings.UNICODE_BIG));
            dic.Put(PdfName.Subtype, new PdfName(subtype));
            usage.Put(PdfName.CreatorInfo, dic);
            usage.SetModified();
        }

        /// <summary>
        /// Specifies the language of the content controlled by this
        /// optional content group
        /// </summary>
        /// <param name="lang">
        /// a language string which specifies a language and possibly a locale
        /// (for example, <b>es-MX</b> represents Mexican Spanish)
        /// </param>
        /// <param name="preferred">
        /// used by viewer applications when there is a partial match but no exact
        /// match between the system language and the language strings in all usage dictionaries
        /// </param>
        public virtual void SetLanguage(String lang, bool preferred) {
            PdfDictionary usage = GetUsage();
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.Lang, new PdfString(lang, PdfEncodings.UNICODE_BIG));
            if (preferred) {
                dic.Put(PdfName.Preferred, PdfName.ON);
            }
            usage.Put(PdfName.Language, dic);
            usage.SetModified();
        }

        /// <summary>
        /// Specifies the recommended state for content in this
        /// group when the document (or part of it) is saved by a viewer application to a format
        /// that does not support optional content (for example, an earlier version of
        /// PDF or a raster image format).
        /// </summary>
        /// <param name="export">the export state</param>
        public virtual void SetExport(bool export) {
            PdfDictionary usage = GetUsage();
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.ExportState, export ? PdfName.ON : PdfName.OFF);
            usage.Put(PdfName.Export, dic);
            usage.SetModified();
        }

        /// <summary>
        /// Specifies a range of magnifications at which the content
        /// in this optional content group is best viewed.
        /// </summary>
        /// <param name="min">
        /// the minimum recommended magnification factors at which the group
        /// should be ON. A negative value will set the default to 0
        /// </param>
        /// <param name="max">
        /// the maximum recommended magnification factor at which the group
        /// should be ON. A negative value will set the largest possible magnification supported by the
        /// viewer application
        /// </param>
        public virtual void SetZoom(float min, float max) {
            if (min <= 0 && max < 0) {
                return;
            }
            PdfDictionary usage = GetUsage();
            PdfDictionary dic = new PdfDictionary();
            if (min > 0) {
                dic.Put(PdfName.min, new PdfNumber(min));
            }
            if (max >= 0) {
                dic.Put(PdfName.max, new PdfNumber(max));
            }
            usage.Put(PdfName.Zoom, dic);
            usage.SetModified();
        }

        /// <summary>
        /// Specifies that the content in this group is intended for
        /// use in printing
        /// </summary>
        /// <param name="subtype">
        /// a name specifying the kind of content controlled by the group;
        /// for example, <b>Trapping</b>, <b>PrintersMarks</b> and <b>Watermark</b>
        /// </param>
        /// <param name="printState">
        /// indicates that the group should be
        /// set to that state when the document is printed from a viewer application
        /// </param>
        public virtual void SetPrint(String subtype, bool printState) {
            PdfDictionary usage = GetUsage();
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.Subtype, new PdfName(subtype));
            dic.Put(PdfName.PrintState, printState ? PdfName.ON : PdfName.OFF);
            usage.Put(PdfName.Print, dic);
            usage.SetModified();
        }

        /// <summary>
        /// Indicates that the group should be set to that state when the
        /// document is opened in a viewer application.
        /// </summary>
        /// <param name="view">the view state</param>
        public virtual void SetView(bool view) {
            PdfDictionary usage = GetUsage();
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.ViewState, view ? PdfName.ON : PdfName.OFF);
            usage.Put(PdfName.View, dic);
            usage.SetModified();
        }

        /// <summary>
        /// Specifies one or more users for whom this optional content group
        /// is primarily intended.
        /// </summary>
        /// <param name="type">a name that can be Ind (individual), Ttl (title), or Org (organization).</param>
        /// <param name="names">
        /// one or more text strings representing
        /// the name(s) of the individual, position or organization
        /// </param>
        public virtual void SetUser(String type, params String[] names) {
            if (type == null || !"Ind".Equals(type) && !"Ttl".Equals(type) && !"Org".Equals(type)) {
                throw new ArgumentException("Illegal type argument");
            }
            if (names == null || names.Length == 0) {
                throw new ArgumentException("Illegal names argument");
            }
            PdfDictionary usage = GetUsage();
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.Type, new PdfName(type));
            if (names.Length == 1) {
                dic.Put(PdfName.Name, new PdfString(names[0], PdfEncodings.UNICODE_BIG));
            }
            else {
                PdfArray namesArray = new PdfArray();
                foreach (String name in names) {
                    namesArray.Add(new PdfString(name, PdfEncodings.UNICODE_BIG));
                }
                dic.Put(PdfName.Name, namesArray);
            }
            usage.Put(PdfName.User, dic);
            usage.SetModified();
        }

        /// <summary>Indicates that the group contains a pagination artifact.</summary>
        /// <param name="pe">
        /// one of the following names: "HF" (Header Footer),
        /// "FG" (Foreground), "BG" (Background), or "L" (Logo).
        /// </param>
        public virtual void SetPageElement(String pe) {
            PdfDictionary usage = GetUsage();
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.Subtype, new PdfName(pe));
            usage.Put(PdfName.PageElement, dic);
            usage.SetModified();
        }

        /// <summary>Gets the indirect reference to the current layer object.</summary>
        /// <returns>the indirect reference to the object representing the layer</returns>
        public virtual PdfIndirectReference GetIndirectReference() {
            return GetPdfObject().GetIndirectReference();
        }

        /// <summary>Gets the title of the layer if it is a title layer, or null if it is a usual layer.</summary>
        /// <returns>the title of the layer if it is a title layer, or null if it is a usual layer</returns>
        public virtual String GetTitle() {
            return title;
        }

        /// <summary>Gets the list of the current child layers of the layer.</summary>
        /// <remarks>
        /// Gets the list of the current child layers of the layer.
        /// BE CAREFUL! Do not try to add a child layer using the resultant child list,
        /// use #addChild method instead.
        /// </remarks>
        /// <returns>the list of the current child layers, null if the layer has no children.</returns>
        public virtual IList<iText.Kernel.Pdf.Layer.PdfLayer> GetChildren() {
            return children == null ? null : new List<iText.Kernel.Pdf.Layer.PdfLayer>(children);
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// that owns that layer.
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// that owns that layer
        /// </returns>
        protected internal virtual PdfDocument GetDocument() {
            return GetPdfObject().GetIndirectReference().GetDocument();
        }

        /// <summary>Creates a title layer without registering it in PdfOCProperties.</summary>
        /// <param name="title">the title of the layer</param>
        /// <param name="document">the document this title layer belongs to</param>
        /// <returns>the created layer</returns>
        protected internal static iText.Kernel.Pdf.Layer.PdfLayer CreateTitleSilent(String title, PdfDocument document
            ) {
            if (title == null) {
                throw new ArgumentException("Invalid title argument");
            }
            iText.Kernel.Pdf.Layer.PdfLayer layer = new iText.Kernel.Pdf.Layer.PdfLayer(document);
            layer.title = title;
            return layer;
        }

        /// <summary>Gets the /Usage dictionary, creating a new one if necessary.</summary>
        /// <returns>the /Usage dictionary</returns>
        protected internal virtual PdfDictionary GetUsage() {
            PdfDictionary usage = GetPdfObject().GetAsDictionary(PdfName.Usage);
            if (usage == null) {
                usage = new PdfDictionary();
                GetPdfObject().Put(PdfName.Usage, usage);
            }
            return usage;
        }

        private PdfOCProperties FetchOCProperties() {
            return GetDocument().GetCatalog().GetOCProperties(true);
        }
    }
}
