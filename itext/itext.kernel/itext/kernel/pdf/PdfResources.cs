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
using iText.Kernel.Font;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Colorspace.Shading;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf.Xobject;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// Wrapper class that represent resource dictionary - that define named resources
    /// used by content streams operators.
    /// </summary>
    /// <remarks>
    /// Wrapper class that represent resource dictionary - that define named resources
    /// used by content streams operators. (ISO 32000-1, 7.8.3 Resource Dictionaries)
    /// </remarks>
    public class PdfResources : PdfObjectWrapper<PdfDictionary> {
        private const String F = "F";

        private const String Im = "Im";

        private const String Fm = "Fm";

        private const String Gs = "Gs";

        private const String Pr = "Pr";

        private const String Cs = "Cs";

        private const String P = "P";

        private const String Sh = "Sh";

        private IDictionary<PdfObject, PdfName> resourceToName = new Dictionary<PdfObject, PdfName>();

        private PdfResources.ResourceNameGenerator fontNamesGen = new PdfResources.ResourceNameGenerator(PdfName.Font
            , F);

        private PdfResources.ResourceNameGenerator imageNamesGen = new PdfResources.ResourceNameGenerator(PdfName.
            XObject, Im);

        private PdfResources.ResourceNameGenerator formNamesGen = new PdfResources.ResourceNameGenerator(PdfName.XObject
            , Fm);

        private PdfResources.ResourceNameGenerator egsNamesGen = new PdfResources.ResourceNameGenerator(PdfName.ExtGState
            , Gs);

        private PdfResources.ResourceNameGenerator propNamesGen = new PdfResources.ResourceNameGenerator(PdfName.Properties
            , Pr);

        private PdfResources.ResourceNameGenerator csNamesGen = new PdfResources.ResourceNameGenerator(PdfName.ColorSpace
            , Cs);

        private PdfResources.ResourceNameGenerator patternNamesGen = new PdfResources.ResourceNameGenerator(PdfName
            .Pattern, P);

        private PdfResources.ResourceNameGenerator shadingNamesGen = new PdfResources.ResourceNameGenerator(PdfName
            .Shading, Sh);

        private bool readOnly = false;

        private bool isModified = false;

        /// <summary>Creates new instance from given dictionary.</summary>
        /// <param name="pdfObject">
        /// the
        /// <see cref="PdfDictionary"/>
        /// object from which the resource object will be created.
        /// </param>
        public PdfResources(PdfDictionary pdfObject)
            : base(pdfObject) {
            BuildResources(pdfObject);
        }

        /// <summary>Creates new instance from empty dictionary.</summary>
        public PdfResources()
            : this(new PdfDictionary()) {
        }

        /// <summary>Adds font to resources and registers PdfFont in the document for further flushing.</summary>
        /// <param name="pdfDocument">
        /// a
        /// <see cref="PdfDocument"/>
        /// instance to which the font is added for further flushing
        /// </param>
        /// <param name="font">
        /// a
        /// <see cref="iText.Kernel.Font.PdfFont"/>
        /// instance to be added
        /// </param>
        /// <returns>added font resource name.</returns>
        public virtual PdfName AddFont(PdfDocument pdfDocument, PdfFont font) {
            pdfDocument.AddFont(font);
            return AddResource(font, fontNamesGen);
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// object to the resources.
        /// </summary>
        /// <param name="image">
        /// the
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfImageXObject"/>
        /// to add.
        /// </param>
        /// <returns>added image resource name.</returns>
        public virtual PdfName AddImage(PdfImageXObject image) {
            return AddResource(image, imageNamesGen);
        }

        /// <summary>
        /// Adds
        /// <see cref="PdfStream"/>
        /// to the resources as image.
        /// </summary>
        /// <param name="image">
        /// the
        /// <see cref="PdfStream"/>
        /// to add.
        /// </param>
        /// <returns>added image resources name.</returns>
        public virtual PdfName AddImage(PdfStream image) {
            return AddResource(image, imageNamesGen);
        }

        public virtual PdfImageXObject GetImage(PdfName name) {
            PdfStream image = GetResource(PdfName.XObject).GetAsStream(name);
            return image != null && PdfName.Image.Equals(image.GetAsName(PdfName.Subtype)) ? new PdfImageXObject(image
                ) : null;
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// object to the resources.
        /// </summary>
        /// <param name="form">
        /// the
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// to add.
        /// </param>
        /// <returns>added form resource name.</returns>
        public virtual PdfName AddForm(PdfFormXObject form) {
            return AddResource(form, formNamesGen);
        }

        /// <summary>
        /// Adds
        /// <see cref="PdfStream"/>
        /// to the resources as form.
        /// </summary>
        /// <param name="form">
        /// the
        /// <see cref="PdfStream"/>
        /// to add.
        /// </param>
        /// <returns>added form resources name.</returns>
        public virtual PdfName AddForm(PdfStream form) {
            return AddResource(form, formNamesGen);
        }

        /// <summary>
        /// Adds the given Form XObject to the current instance of
        /// <see cref="PdfResources"/>.
        /// </summary>
        /// <param name="form">Form XObject.</param>
        /// <param name="name">Preferred name for the given Form XObject.</param>
        /// <returns>
        /// the
        /// <see cref="PdfName"/>
        /// of the newly added resource
        /// </returns>
        public virtual PdfName AddForm(PdfFormXObject form, PdfName name) {
            if (GetResourceNames(PdfName.XObject).Contains(name)) {
                name = AddResource(form, formNamesGen);
            }
            else {
                AddResource(form.GetPdfObject(), PdfName.XObject, name);
            }
            return name;
        }

        public virtual PdfFormXObject GetForm(PdfName name) {
            PdfStream form = GetResource(PdfName.XObject).GetAsStream(name);
            return form != null && PdfName.Form.Equals(form.GetAsName(PdfName.Subtype)) ? new PdfFormXObject(form) : null;
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState"/>
        /// object to the resources.
        /// </summary>
        /// <param name="extGState">
        /// the
        /// <see cref="iText.Kernel.Pdf.Extgstate.PdfExtGState"/>
        /// to add.
        /// </param>
        /// <returns>added graphics state parameter dictionary resource name.</returns>
        public virtual PdfName AddExtGState(PdfExtGState extGState) {
            return AddResource(extGState, egsNamesGen);
        }

        /// <summary>
        /// Adds
        /// <see cref="PdfDictionary"/>
        /// to the resources as graphics state parameter dictionary.
        /// </summary>
        /// <param name="extGState">
        /// the
        /// <see cref="PdfDictionary"/>
        /// to add.
        /// </param>
        /// <returns>added graphics state parameter dictionary resources name.</returns>
        public virtual PdfName AddExtGState(PdfDictionary extGState) {
            return AddResource(extGState, egsNamesGen);
        }

        public virtual PdfExtGState GetPdfExtGState(PdfName name) {
            PdfDictionary dic = GetResource(PdfName.ExtGState).GetAsDictionary(name);
            return dic != null ? new PdfExtGState(dic) : null;
        }

        /// <summary>
        /// Adds
        /// <see cref="PdfDictionary"/>
        /// to the resources as properties list.
        /// </summary>
        /// <param name="properties">
        /// the
        /// <see cref="PdfDictionary"/>
        /// to add.
        /// </param>
        /// <returns>added properties list resources name.</returns>
        public virtual PdfName AddProperties(PdfDictionary properties) {
            return AddResource(properties, propNamesGen);
        }

        public virtual PdfObject GetProperties(PdfName name) {
            return GetResourceObject(PdfName.Properties, name);
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// object to the resources.
        /// </summary>
        /// <param name="cs">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfColorSpace"/>
        /// to add.
        /// </param>
        /// <returns>added color space resource name.</returns>
        public virtual PdfName AddColorSpace(PdfColorSpace cs) {
            return AddResource(cs, csNamesGen);
        }

        /// <summary>
        /// Adds
        /// <see cref="PdfObject"/>
        /// to the resources as color space.
        /// </summary>
        /// <param name="colorSpace">
        /// the
        /// <see cref="PdfObject"/>
        /// to add.
        /// </param>
        /// <returns>added color space resources name.</returns>
        public virtual PdfName AddColorSpace(PdfObject colorSpace) {
            return AddResource(colorSpace, csNamesGen);
        }

        public virtual PdfColorSpace GetColorSpace(PdfName name) {
            PdfObject colorSpace = GetResourceObject(PdfName.ColorSpace, name);
            return colorSpace != null ? PdfColorSpace.MakeColorSpace(colorSpace) : null;
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfPattern"/>
        /// object to the resources.
        /// </summary>
        /// <param name="pattern">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.PdfPattern"/>
        /// to add.
        /// </param>
        /// <returns>added pattern resource name.</returns>
        public virtual PdfName AddPattern(PdfPattern pattern) {
            return AddResource(pattern, patternNamesGen);
        }

        /// <summary>
        /// Adds
        /// <see cref="PdfDictionary"/>
        /// to the resources as pattern.
        /// </summary>
        /// <param name="pattern">
        /// the
        /// <see cref="PdfDictionary"/>
        /// to add.
        /// </param>
        /// <returns>added pattern resources name.</returns>
        public virtual PdfName AddPattern(PdfDictionary pattern) {
            return AddResource(pattern, patternNamesGen);
        }

        public virtual PdfPattern GetPattern(PdfName name) {
            PdfObject pattern = GetResourceObject(PdfName.Pattern, name);
            return pattern is PdfDictionary ? PdfPattern.GetPatternInstance((PdfDictionary)pattern) : null;
        }

        /// <summary>
        /// Adds
        /// <see cref="iText.Kernel.Pdf.Colorspace.Shading.AbstractPdfShading"/>
        /// object to the resources.
        /// </summary>
        /// <param name="shading">
        /// the
        /// <see cref="iText.Kernel.Pdf.Colorspace.Shading.AbstractPdfShading"/>
        /// to add.
        /// </param>
        /// <returns>added shading resource name.</returns>
        public virtual PdfName AddShading(AbstractPdfShading shading) {
            return AddResource(shading, shadingNamesGen);
        }

        /// <summary>
        /// Adds
        /// <see cref="PdfDictionary"/>
        /// to the resources as shading dictionary.
        /// </summary>
        /// <param name="shading">
        /// the
        /// <see cref="PdfDictionary"/>
        /// to add.
        /// </param>
        /// <returns>added shading dictionary resources name.</returns>
        public virtual PdfName AddShading(PdfDictionary shading) {
            return AddResource(shading, shadingNamesGen);
        }

        public virtual AbstractPdfShading GetShading(PdfName name) {
            PdfObject shading = GetResourceObject(PdfName.Shading, name);
            return shading is PdfDictionary ? AbstractPdfShading.MakeShading((PdfDictionary)shading) : null;
        }

        protected internal virtual bool IsReadOnly() {
            return readOnly;
        }

        protected internal virtual void SetReadOnly(bool readOnly) {
            this.readOnly = readOnly;
        }

        protected internal virtual bool IsModified() {
            return isModified;
        }

        /// <summary><inheritDoc/></summary>
        public override PdfObjectWrapper<PdfDictionary> SetModified() {
            this.isModified = true;
            return base.SetModified();
        }

        /// <summary>Sets the value of default Gray Color Space (see ISO-320001 Paragraph 8.6.5.6).</summary>
        /// <param name="defaultCs">the color space to set.</param>
        public virtual void SetDefaultGray(PdfColorSpace defaultCs) {
            AddResource(defaultCs.GetPdfObject(), PdfName.ColorSpace, PdfName.DefaultGray);
        }

        /// <summary>Sets the value of default RGB Color Space (see ISO-320001 Paragraph 8.6.5.6).</summary>
        /// <param name="defaultCs">the color space to set.</param>
        public virtual void SetDefaultRgb(PdfColorSpace defaultCs) {
            AddResource(defaultCs.GetPdfObject(), PdfName.ColorSpace, PdfName.DefaultRGB);
        }

        /// <summary>Sets the value of default CMYK Color Space (see ISO-320001 Paragraph 8.6.5.6).</summary>
        /// <param name="defaultCs">the color space to set.</param>
        public virtual void SetDefaultCmyk(PdfColorSpace defaultCs) {
            AddResource(defaultCs.GetPdfObject(), PdfName.ColorSpace, PdfName.DefaultCMYK);
        }

        /// <summary>
        /// Gets the mapped resource name of the
        /// <see cref="PdfObject"/>
        /// under the given wrapper.
        /// </summary>
        /// <remarks>
        /// Gets the mapped resource name of the
        /// <see cref="PdfObject"/>
        /// under the given wrapper.
        /// <br />
        /// <br />
        /// Note: if the name for the object won't be found, then the name of object's Indirect Reference will be searched.
        /// </remarks>
        /// <param name="resource">
        /// the wrapper of the
        /// <see cref="PdfObject"/>
        /// , for which the name will be searched.
        /// </param>
        /// <typeparam name="T">
        /// the type of the underlined
        /// <see cref="PdfObject"/>
        /// in wrapper.
        /// </typeparam>
        /// <returns>
        /// the mapped resource name or
        /// <see langword="null"/>
        /// if object isn't added to resources.
        /// </returns>
        public virtual PdfName GetResourceName<T>(PdfObjectWrapper<T> resource)
            where T : PdfObject {
            return GetResourceName(resource.GetPdfObject());
        }

        /// <summary>
        /// Gets the mapped resource name of the given
        /// <see cref="PdfObject"/>.
        /// </summary>
        /// <remarks>
        /// Gets the mapped resource name of the given
        /// <see cref="PdfObject"/>.
        /// <br />
        /// <br />
        /// Note: if the name for the object won't be found, then the name of object's Indirect Reference will be searched.
        /// </remarks>
        /// <param name="resource">the object, for which the name will be searched.</param>
        /// <returns>
        /// the mapped resource name or
        /// <see langword="null"/>
        /// if object isn't added to resources.
        /// </returns>
        public virtual PdfName GetResourceName(PdfObject resource) {
            PdfName resName = resourceToName.Get(resource);
            if (resName == null) {
                resName = resourceToName.Get(resource.GetIndirectReference());
            }
            return resName;
        }

        /// <summary>Gets the names of all the added resources.</summary>
        /// <returns>the name of all the added resources.</returns>
        public virtual ICollection<PdfName> GetResourceNames() {
            ICollection<PdfName> names = new SortedSet<PdfName>();
            foreach (PdfName resType in GetPdfObject().KeySet()) {
                names.AddAll(GetResourceNames(resType));
            }
            return names;
        }

        /// <summary>Gets the array of predefined procedure set names (see ISO-320001 Paragraph 14.2).</summary>
        /// <remarks>
        /// Gets the array of predefined procedure set names (see ISO-320001 Paragraph 14.2).
        /// Deprecated in PDF 2.0.
        /// </remarks>
        /// <returns>the array of predefined procedure set names.</returns>
        public virtual PdfArray GetProcSet() {
            return GetPdfObject().GetAsArray(PdfName.ProcSet);
        }

        /// <summary>Sets the array of predefined procedure set names (see ISO-320001 Paragraph 14.2).</summary>
        /// <remarks>
        /// Sets the array of predefined procedure set names (see ISO-320001 Paragraph 14.2).
        /// Deprecated in PDF 2.0.
        /// </remarks>
        /// <param name="array">the array of predefined procedure set names to be set.</param>
        public virtual void SetProcSet(PdfArray array) {
            GetPdfObject().Put(PdfName.ProcSet, array);
        }

        /// <summary>Gets the names of all resources of specified type.</summary>
        /// <param name="resType">
        /// the resource type. Should be
        /// <see cref="PdfName.ColorSpace"/>
        /// ,
        /// <see cref="PdfName.ExtGState"/>
        /// ,
        /// <see cref="PdfName.Pattern"/>
        /// ,
        /// <see cref="PdfName.Shading"/>
        /// ,
        /// <see cref="PdfName.XObject"/>
        /// ,
        /// <see cref="PdfName.Font"/>.
        /// </param>
        /// <returns>
        /// set of resources name of corresponding type. May be empty.
        /// Will be empty in case of incorrect resource type.
        /// </returns>
        public virtual ICollection<PdfName> GetResourceNames(PdfName resType) {
            PdfDictionary resourceCategory = GetPdfObject().GetAsDictionary(resType);
            return resourceCategory == null ? JavaCollectionsUtil.EmptySet<PdfName>() : resourceCategory.KeySet();
        }

        /// <summary>
        /// Get the
        /// <see cref="PdfDictionary"/>
        /// object that that contain resources of specified type.
        /// </summary>
        /// <param name="resType">
        /// the resource type. Should be
        /// <see cref="PdfName.ColorSpace"/>
        /// ,
        /// <see cref="PdfName.ExtGState"/>
        /// ,
        /// <see cref="PdfName.Pattern"/>
        /// ,
        /// <see cref="PdfName.Shading"/>
        /// ,
        /// <see cref="PdfName.XObject"/>
        /// ,
        /// <see cref="PdfName.Font"/>.
        /// </param>
        /// <returns>
        /// the
        /// <see cref="PdfDictionary"/>
        /// object containing all resources of specified type,
        /// or
        /// <see langword="null"/>
        /// in case of incorrect resource type.
        /// </returns>
        public virtual PdfDictionary GetResource(PdfName resType) {
            return GetPdfObject().GetAsDictionary(resType);
        }

        /// <summary>
        /// Get the
        /// <see cref="PdfObject"/>
        /// object with specified type and name.
        /// </summary>
        /// <param name="resType">
        /// the resource type. Should be
        /// <see cref="PdfName.ColorSpace"/>
        /// ,
        /// <see cref="PdfName.ExtGState"/>
        /// ,
        /// <see cref="PdfName.Pattern"/>
        /// ,
        /// <see cref="PdfName.Shading"/>
        /// ,
        /// <see cref="PdfName.XObject"/>
        /// ,
        /// <see cref="PdfName.Font"/>.
        /// </param>
        /// <param name="resName">the name of the resource object.</param>
        /// <returns>
        /// the
        /// <see cref="PdfObject"/>
        /// with specified name in the resources of specified type or
        /// <see langword="null"/>
        /// in case of incorrect type or missing resource with such name.
        /// </returns>
        public virtual PdfObject GetResourceObject(PdfName resType, PdfName resName) {
            PdfDictionary resource = GetResource(resType);
            if (resource != null) {
                return resource.Get(resName);
            }
            return null;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual PdfName AddResource<T>(PdfObjectWrapper<T> resource, PdfResources.ResourceNameGenerator nameGen
            )
            where T : PdfObject {
            return AddResource(resource.GetPdfObject(), nameGen);
        }
//\endcond

        protected internal virtual void AddResource(PdfObject resource, PdfName resType, PdfName resName) {
            if (resType.Equals(PdfName.XObject)) {
                CheckAndResolveCircularReferences(resource);
            }
            if (readOnly) {
                SetPdfObject(GetPdfObject().Clone(JavaCollectionsUtil.EmptyList<PdfName>()));
                BuildResources(GetPdfObject());
                isModified = true;
                readOnly = false;
            }
            if (GetPdfObject().ContainsKey(resType) && GetPdfObject().GetAsDictionary(resType).ContainsKey(resName)) {
                return;
            }
            resourceToName.Put(resource, resName);
            PdfDictionary resourceCategory = GetPdfObject().GetAsDictionary(resType);
            if (resourceCategory == null) {
                GetPdfObject().Put(resType, resourceCategory = new PdfDictionary());
            }
            else {
                resourceCategory.SetModified();
            }
            resourceCategory.Put(resName, resource);
            SetModified();
        }

//\cond DO_NOT_DOCUMENT
        internal virtual PdfName AddResource(PdfObject resource, PdfResources.ResourceNameGenerator nameGen) {
            PdfName resName = GetResourceName(resource);
            if (resName == null) {
                resName = nameGen.Generate(this);
                AddResource(resource, nameGen.GetResourceType(), resName);
            }
            return resName;
        }
//\endcond

        protected internal virtual void BuildResources(PdfDictionary dictionary) {
            foreach (PdfName resourceType in dictionary.KeySet()) {
                if (GetPdfObject().Get(resourceType) == null) {
                    GetPdfObject().Put(resourceType, new PdfDictionary());
                }
                PdfDictionary resources = dictionary.GetAsDictionary(resourceType);
                if (resources == null) {
                    continue;
                }
                foreach (PdfName resourceName in resources.KeySet()) {
                    PdfObject resource = resources.Get(resourceName, false);
                    resourceToName.Put(resource, resourceName);
                }
            }
        }

        private void CheckAndResolveCircularReferences(PdfObject pdfObject) {
            // Consider the situation when an XObject references the resources of the first page.
            // We add this XObject to the first page, there is no need to resolve any circular references
            // and then we flush this object and try to add it to the second page.
            // Now there are circular references and we cannot resolve them because the object is flushed
            // and we cannot get resources.
            // On the other hand, this situation may occur any time when object is already flushed and we
            // try to add it to resources and it seems difficult to overcome this without keeping /Resources key value.
            if (pdfObject is PdfDictionary && !pdfObject.IsFlushed()) {
                PdfDictionary pdfXObject = (PdfDictionary)pdfObject;
                PdfObject pdfXObjectResources = pdfXObject.Get(PdfName.Resources);
                if (pdfXObjectResources != null && pdfXObjectResources.GetIndirectReference() != null) {
                    if (pdfXObjectResources.GetIndirectReference().Equals(GetPdfObject().GetIndirectReference())) {
                        PdfObject cloneResources = GetPdfObject().Clone();
                        cloneResources.MakeIndirect(GetPdfObject().GetIndirectReference().GetDocument());
                        pdfXObject.Put(PdfName.Resources, cloneResources.GetIndirectReference());
                    }
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Represents a resource name generator.</summary>
        /// <remarks>
        /// Represents a resource name generator. The generator takes into account
        /// the names of already existing resources thus providing us a unique name.
        /// The name consists of the following parts: prefix (literal) and number.
        /// </remarks>
        internal class ResourceNameGenerator {
            private PdfName resourceType;

            private int counter;

            private String prefix;

            /// <summary>
            /// Constructs an instance of
            /// <see cref="ResourceNameGenerator"/>
            /// class.
            /// </summary>
            /// <param name="resourceType">
            /// Type of resource. Should be
            /// <see cref="PdfName.ColorSpace"/>
            /// ,
            /// <see cref="PdfName.ExtGState"/>
            /// ,
            /// <see cref="PdfName.Pattern"/>
            /// ,
            /// <see cref="PdfName.Shading"/>
            /// ,
            /// <see cref="PdfName.XObject"/>
            /// ,
            /// <see cref="PdfName.Font"/>.
            /// </param>
            /// <param name="prefix">Prefix used for generating names.</param>
            /// <param name="seed">
            /// Seed for the value which is appended to the number each time
            /// new name is generated.
            /// </param>
            public ResourceNameGenerator(PdfName resourceType, String prefix, int seed) {
                this.prefix = prefix;
                this.resourceType = resourceType;
                this.counter = seed;
            }

            /// <summary>
            /// Constructs an instance of
            /// <see cref="ResourceNameGenerator"/>
            /// class.
            /// </summary>
            /// <param name="resourceType">
            /// Type of resource. Should be
            /// <see cref="PdfName.ColorSpace"/>
            /// ,
            /// <see cref="PdfName.ExtGState"/>
            /// ,
            /// <see cref="PdfName.Pattern"/>
            /// ,
            /// <see cref="PdfName.Shading"/>
            /// ,
            /// <see cref="PdfName.XObject"/>
            /// ,
            /// <see cref="PdfName.Font"/>.
            /// </param>
            /// <param name="prefix">Prefix used for generating names.</param>
            public ResourceNameGenerator(PdfName resourceType, String prefix)
                : this(resourceType, prefix, 1) {
            }

            /// <summary>Gets the resource type of generator.</summary>
            /// <returns>
            /// Type of resource. May be
            /// <see cref="PdfName.ColorSpace"/>
            /// ,
            /// <see cref="PdfName.ExtGState"/>
            /// ,
            /// <see cref="PdfName.Pattern"/>
            /// ,
            /// <see cref="PdfName.Shading"/>
            /// ,
            /// <see cref="PdfName.XObject"/>
            /// ,
            /// <see cref="PdfName.Font"/>.
            /// </returns>
            public virtual PdfName GetResourceType() {
                return resourceType;
            }

            /// <summary>Generates new (unique) resource name.</summary>
            /// <param name="resources">
            /// the
            /// <see cref="PdfResources"/>
            /// object for which name will be generated.
            /// </param>
            /// <returns>new (unique) resource name.</returns>
            public virtual PdfName Generate(PdfResources resources) {
                PdfName newName = new PdfName(prefix + counter++);
                PdfDictionary r = resources.GetPdfObject();
                if (r.ContainsKey(resourceType)) {
                    while (r.GetAsDictionary(resourceType).ContainsKey(newName)) {
                        newName = new PdfName(prefix + counter++);
                    }
                }
                return newName;
            }
        }
//\endcond
    }
}
