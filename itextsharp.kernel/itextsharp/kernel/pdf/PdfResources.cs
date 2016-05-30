/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Pdf.Colorspace;
using iTextSharp.Kernel.Pdf.Extgstate;
using iTextSharp.Kernel.Pdf.Xobject;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfResources : PdfObjectWrapper<PdfDictionary>
	{
		private const String F = "F";

		private const String Im = "Im";

		private const String Fm = "Fm";

		private const String Gs = "Gs";

		private const String Pr = "Pr";

		private const String Cs = "Cs";

		private const String P = "P";

		private const String Sh = "Sh";

		private IDictionary<PdfObject, PdfName> resourceToName = new Dictionary<PdfObject
			, PdfName>();

		private PdfResources.ResourceNameGenerator fontNamesGen = new PdfResources.ResourceNameGenerator
			(PdfName.Font, F);

		private PdfResources.ResourceNameGenerator imageNamesGen = new PdfResources.ResourceNameGenerator
			(PdfName.XObject, Im);

		private PdfResources.ResourceNameGenerator formNamesGen = new PdfResources.ResourceNameGenerator
			(PdfName.XObject, Fm);

		private PdfResources.ResourceNameGenerator egsNamesGen = new PdfResources.ResourceNameGenerator
			(PdfName.ExtGState, Gs);

		private PdfResources.ResourceNameGenerator propNamesGen = new PdfResources.ResourceNameGenerator
			(PdfName.Properties, Pr);

		private PdfResources.ResourceNameGenerator csNamesGen = new PdfResources.ResourceNameGenerator
			(PdfName.ColorSpace, Cs);

		private PdfResources.ResourceNameGenerator patternNamesGen = new PdfResources.ResourceNameGenerator
			(PdfName.Pattern, P);

		private PdfResources.ResourceNameGenerator shadingNamesGen = new PdfResources.ResourceNameGenerator
			(PdfName.Shading, Sh);

		private bool readOnly = false;

		private bool isModified = false;

		public PdfResources(PdfDictionary pdfObject)
			: base(pdfObject)
		{
			BuildResources(pdfObject);
		}

		public PdfResources()
			: this(new PdfDictionary())
		{
		}

		/// <summary>Add font to resources and register PdfFont in the document for further flushing.
		/// 	</summary>
		/// <returns>font resource name.</returns>
		public virtual PdfName AddFont(PdfDocument pdfDocument, PdfFont font)
		{
			pdfDocument.GetDocumentFonts().Add(font);
			return AddResource(font, fontNamesGen);
		}

		public virtual PdfName AddImage(PdfImageXObject image)
		{
			return AddResource(image, imageNamesGen);
		}

		public virtual PdfName AddImage(PdfObject image)
		{
			return AddResource(image, imageNamesGen);
		}

		public virtual PdfName AddForm(PdfFormXObject form)
		{
			return AddResource(form, formNamesGen);
		}

		public virtual PdfName AddForm(PdfObject form)
		{
			return AddResource(form, formNamesGen);
		}

		/// <summary>
		/// Adds the given Form XObject to the current instance of
		/// <see cref="PdfResources"/>
		/// .
		/// </summary>
		/// <param name="form">Form XObject.</param>
		/// <param name="name">Preferred name for the given Form XObject.</param>
		/// <returns>
		/// the
		/// <see cref="PdfName"/>
		/// of the newly added resource
		/// </returns>
		public virtual PdfName AddForm(PdfFormXObject form, PdfName name)
		{
			if (GetResourceNames(PdfName.XObject).Contains(name))
			{
				name = AddResource(form, formNamesGen);
			}
			else
			{
				AddResource(form.GetPdfObject(), PdfName.XObject, name);
			}
			return name;
		}

		public virtual PdfName AddExtGState(PdfExtGState extGState)
		{
			return AddResource(extGState, egsNamesGen);
		}

		public virtual PdfName AddExtGState(PdfObject extGState)
		{
			return AddResource(extGState, egsNamesGen);
		}

		public virtual PdfName AddProperties(PdfObject properties)
		{
			return AddResource(properties, propNamesGen);
		}

		public virtual PdfName AddColorSpace(PdfColorSpace cs)
		{
			return AddResource(cs, csNamesGen);
		}

		public virtual PdfName AddColorSpace(PdfObject colorSpace)
		{
			return AddResource(colorSpace, csNamesGen);
		}

		public virtual PdfName AddPattern(PdfPattern pattern)
		{
			return AddResource(pattern, patternNamesGen);
		}

		public virtual PdfName AddPattern(PdfObject pattern)
		{
			return AddResource(pattern, patternNamesGen);
		}

		public virtual PdfName AddShading(PdfShading shading)
		{
			return AddResource(shading, shadingNamesGen);
		}

		public virtual PdfName AddShading(PdfObject shading)
		{
			return AddResource(shading, shadingNamesGen);
		}

		protected internal virtual bool IsReadOnly()
		{
			return readOnly;
		}

		protected internal virtual void SetReadOnly(bool readOnly)
		{
			this.readOnly = readOnly;
		}

		protected internal virtual bool IsModified()
		{
			return isModified;
		}

		protected internal virtual void SetModified(bool isModified)
		{
			this.isModified = isModified;
		}

		/// <summary>Sets the default color space.</summary>
		/// <param name="defaultCsKey"/>
		/// <param name="defaultCsValue"/>
		/// <exception cref="iTextSharp.Kernel.PdfException"/>
		public virtual void SetDefaultColorSpace(PdfName defaultCsKey, PdfColorSpace defaultCsValue
			)
		{
			AddResource(defaultCsValue.GetPdfObject(), PdfName.ColorSpace, defaultCsKey);
		}

		public virtual void SetDefaultGray(PdfColorSpace defaultCs)
		{
			SetDefaultColorSpace(PdfName.DefaultGray, defaultCs);
		}

		public virtual void SetDefaultRgb(PdfColorSpace defaultCs)
		{
			SetDefaultColorSpace(PdfName.DefaultRGB, defaultCs);
		}

		public virtual void SetDefaultCmyk(PdfColorSpace defaultCs)
		{
			SetDefaultColorSpace(PdfName.DefaultCMYK, defaultCs);
		}

		public virtual PdfName GetResourceName<T>(PdfObjectWrapper<T> resource)
			where T : PdfObject
		{
			return resourceToName.Get(resource.GetPdfObject());
		}

		public virtual PdfName GetResourceName(PdfObject resource)
		{
			PdfName resName = resourceToName.Get(resource);
			if (resName == null)
			{
				resName = resourceToName.Get(resource.GetIndirectReference());
			}
			return resName;
		}

		public virtual ICollection<PdfName> GetResourceNames()
		{
			ICollection<PdfName> names = new SortedSet<PdfName>();
			// TODO: isn't it better to use HashSet? Do we really need certain order?
			foreach (PdfName resType in GetPdfObject().KeySet())
			{
				names.AddAll(GetResourceNames(resType));
			}
			return names;
		}

		public virtual PdfArray GetProcSet()
		{
			return GetPdfObject().GetAsArray(PdfName.ProcSet);
		}

		public virtual void SetProcSet(PdfArray array)
		{
			GetPdfObject().Put(PdfName.ProcSet, array);
		}

		public virtual ICollection<PdfName> GetResourceNames(PdfName resType)
		{
			PdfDictionary resourceCategory = GetPdfObject().GetAsDictionary(resType);
			return resourceCategory == null ? new SortedSet<PdfName>() : resourceCategory.KeySet
				();
		}

		// TODO: TreeSet or HashSet enough?
		public virtual PdfDictionary GetResource(PdfName pdfName)
		{
			return GetPdfObject().GetAsDictionary(pdfName);
		}

		//    public List<PdfDictionary> getFonts(boolean updateFonts) throws IOException {
		//        if (updateFonts) {
		//            getPdfObject().remove(PdfName.Font);
		//            PdfDictionary fMap = getResource(PdfName.Font);
		//            if (fMap != null) {
		//                addFont(fMap.entrySet());
		//            }
		//            PdfDictionary xMap = getResource(PdfName.XObject);
		//            if (xMap != null && !xMap.isEmpty()) {
		//                callXObjectFont(xMap.entrySet(), new HashSet<PdfDictionary>());
		//            }
		//        }
		//        List<PdfDictionary> fonts = new ArrayList<>();
		//        for (PdfObject fontDict : getPdfObject().getAsDictionary(PdfName.Font).values()) {
		//            if (fontDict.isDictionary()) {
		//                fonts.add((PdfDictionary) fontDict);
		//            }
		//        }
		//        return fonts;
		//    }
		protected internal override bool IsWrappedObjectMustBeIndirect()
		{
			return false;
		}

		internal virtual PdfName AddResource<T>(PdfObjectWrapper<T> resource, PdfResources.ResourceNameGenerator
			 nameGen)
			where T : PdfObject
		{
			return AddResource(resource.GetPdfObject(), nameGen);
		}

		protected internal virtual void AddResource(PdfObject resource, PdfName resType, 
			PdfName resName)
		{
			if (resType.Equals(PdfName.XObject))
			{
				CheckAndResolveCircularReferences(resource);
			}
			if (readOnly)
			{
				SetPdfObject(new PdfDictionary(GetPdfObject()));
				BuildResources(GetPdfObject());
				isModified = true;
				readOnly = false;
			}
			if (GetPdfObject().ContainsKey(resType) && GetPdfObject().GetAsDictionary(resType
				).ContainsKey(resName))
			{
				return;
			}
			resourceToName[resource] = resName;
			PdfDictionary resourceCategory = GetPdfObject().GetAsDictionary(resType);
			if (resourceCategory == null)
			{
				GetPdfObject().Put(resType, resourceCategory = new PdfDictionary());
			}
			resourceCategory.Put(resName, resource);
			PdfDictionary resDictionary = (PdfDictionary)GetPdfObject().Get(resType);
			if (resDictionary == null)
			{
				GetPdfObject().Put(resType, resDictionary = new PdfDictionary());
			}
			resDictionary.Put(resName, resource);
		}

		internal virtual PdfName AddResource(PdfObject resource, PdfResources.ResourceNameGenerator
			 nameGen)
		{
			PdfName resName = GetResourceName(resource);
			if (resName == null)
			{
				resName = nameGen.Generate(this);
				AddResource(resource, nameGen.GetResourceType(), resName);
			}
			return resName;
		}

		protected internal virtual void BuildResources(PdfDictionary dictionary)
		{
			foreach (PdfName resourceType in dictionary.KeySet())
			{
				if (GetPdfObject().Get(resourceType) == null)
				{
					GetPdfObject().Put(resourceType, new PdfDictionary());
				}
				PdfDictionary resources = dictionary.GetAsDictionary(resourceType);
				if (resources == null)
				{
					continue;
				}
				foreach (PdfName resourceName in resources.KeySet())
				{
					PdfObject resource = resources.Get(resourceName, false);
					resourceToName[resource] = resourceName;
				}
			}
		}

		//    private void addFont(Collection<PdfObject> entrySet) throws IOException {
		//        for (PdfObject entry : entrySet) {
		//            PdfDictionary fonts = getPdfObject().getAsDictionary(PdfName.Font);
		//            if (entry.isIndirectReference() && !fonts.containsValue(entry)) {
		//                fonts.put((PdfIndirectReference) entry.getValue(),
		//                        PdfFont.createFont((PdfDictionary) ((PdfIndirectReference) entry.getValue()).getRefersTo()));
		//            } else if (entry.getValue().isDictionary()) {
		//                PdfFont font = PdfFont.createFont((PdfDictionary) entry.getValue());
		//                fontsMap.put(font.getPdfObject().getIndirectReference(), font);
		//            }
		//        }
		//    }
		//    private void addFontFromXObject(Set<Map.Entry<PdfName, PdfObject>> entrySet, Set<PdfDictionary> visitedResources) throws IOException {
		//        PdfDictionary xObject = new PdfDictionary(entrySet);
		//        PdfDictionary resources = xObject.getAsDictionary(PdfName.Resources);
		//        if (resources == null)
		//            return;
		//        PdfDictionary font = resources.getAsDictionary(PdfName.Font);
		//
		//        if (font != null) {
		//            addFont(font.values());
		//        }
		//        PdfDictionary xobj = resources.getAsDictionary(PdfName.XObject);
		//        if (xobj != null) {
		//            if (visitedResources.add(xobj)) {
		//                callXObjectFont(xobj.entrySet(), visitedResources);
		//                visitedResources.remove(xobj);
		//            } else {
		//                throw new IOException(IOException.IllegalResourceTree);
		//            }
		//        }
		//    }
		//    private void callXObjectFont(Set<Map.Entry<PdfName, PdfObject>> entrySet, Set<PdfDictionary> visitedResources) throws IOException {
		//        for (Map.Entry<PdfName, PdfObject> entry : entrySet) {
		//            if (entry.getValue().isIndirectReference()) {
		//                if (((PdfIndirectReference) entry.getValue()).getRefersTo().isStream()) {
		//                    addFontFromXObject(((PdfStream) ((PdfIndirectReference) entry.getValue()).getRefersTo()).entrySet(), visitedResources);
		//                }
		//            }
		//        }
		//    }
		private void CheckAndResolveCircularReferences(PdfObject pdfObject)
		{
			// Consider the situation when an XObject references the resources of the first page.
			// We add this XObject to the first page, there is no need to resolve any circular references
			// and then we flush this object and try to add it to the second page.
			// Now there are circular references and we cannot resolve them because the object is flushed
			// and we cannot get resources.
			// On the other hand, this situation may occur any time when object is already flushed and we
			// try to add it to resources and it seems difficult to overcome this without keeping /Resources key value.
			if (pdfObject is PdfDictionary && !pdfObject.IsFlushed())
			{
				PdfDictionary pdfXObject = (PdfDictionary)pdfObject;
				PdfObject pdfXObjectResources = pdfXObject.Get(PdfName.Resources);
				if (pdfXObjectResources != null && pdfXObjectResources.GetIndirectReference() != 
					null)
				{
					if (pdfXObjectResources.GetIndirectReference().Equals(GetPdfObject().GetIndirectReference
						()))
					{
						PdfObject cloneResources = GetPdfObject().Clone();
						cloneResources.MakeIndirect(GetPdfObject().GetIndirectReference().GetDocument());
						pdfXObject.Put(PdfName.Resources, cloneResources.GetIndirectReference());
					}
				}
			}
		}

		/// <summary>Represents a resource name generator.</summary>
		/// <remarks>
		/// Represents a resource name generator. The generator takes into account
		/// the names of already existing resources thus providing us a unique name.
		/// The name consists of the following parts: prefix (literal) and number.
		/// </remarks>
		internal class ResourceNameGenerator
		{
			private PdfName resourceType;

			private int counter;

			private String prefix;

			/// <summary>
			/// Constructs an instance of
			/// <see cref="ResourceNameGenerator"/>
			/// class.
			/// </summary>
			/// <param name="resourceType">
			/// Type of resource (
			/// <see cref="PdfName.XObject"/>
			/// ,
			/// <see cref="PdfName.Font"/>
			/// etc).
			/// </param>
			/// <param name="prefix">Prefix used for generating names.</param>
			/// <param name="seed">
			/// Seed for the value which is appended to the number each time
			/// new name is generated.
			/// </param>
			public ResourceNameGenerator(PdfName resourceType, String prefix, int seed)
			{
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
			/// Type of resource (
			/// <see cref="PdfName.XObject"/>
			/// ,
			/// <see cref="PdfName.Font"/>
			/// etc).
			/// </param>
			/// <param name="prefix">Prefix used for generating names.</param>
			public ResourceNameGenerator(PdfName resourceType, String prefix)
				: this(resourceType, prefix, 1)
			{
			}

			public virtual PdfName GetResourceType()
			{
				return resourceType;
			}

			/// <summary>Generates new (unique) resource name.</summary>
			/// <returns>New (unique) resource name.</returns>
			public virtual PdfName Generate(PdfResources resources)
			{
				PdfName newName = new PdfName(prefix + counter++);
				PdfDictionary r = resources.GetPdfObject();
				if (r.ContainsKey(resourceType))
				{
					while (r.GetAsDictionary(resourceType).ContainsKey(newName))
					{
						newName = new PdfName(prefix + counter++);
					}
				}
				return newName;
			}
		}
	}
}
