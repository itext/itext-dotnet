/*
$Id$

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
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Tagging;

namespace iTextSharp.Kernel.Pdf.Tagutils
{
	public class AccessibilityProperties
	{
		protected internal String language;

		protected internal String actualText;

		protected internal String alternateDescription;

		protected internal String expansion;

		protected internal IList<PdfDictionary> attributesList = new List<PdfDictionary>(
			);

		public virtual String GetLanguage()
		{
			return language;
		}

		public virtual AccessibilityProperties SetLanguage(String language)
		{
			this.language = language;
			return this;
		}

		public virtual String GetActualText()
		{
			return actualText;
		}

		public virtual AccessibilityProperties SetActualText(String actualText)
		{
			this.actualText = actualText;
			return this;
		}

		public virtual String GetAlternateDescription()
		{
			return alternateDescription;
		}

		public virtual AccessibilityProperties SetAlternateDescription(String alternateDescription
			)
		{
			this.alternateDescription = alternateDescription;
			return this;
		}

		public virtual String GetExpansion()
		{
			return expansion;
		}

		public virtual AccessibilityProperties SetExpansion(String expansion)
		{
			this.expansion = expansion;
			return this;
		}

		public virtual AccessibilityProperties AddAttributes(PdfDictionary attributes)
		{
			attributesList.Add(attributes);
			return this;
		}

		public virtual AccessibilityProperties ClearAttributes()
		{
			attributesList.Clear();
			return this;
		}

		public virtual IList<PdfDictionary> GetAttributesList()
		{
			return attributesList;
		}

		internal virtual void SetToStructElem(PdfStructElem elem)
		{
			if (GetActualText() != null)
			{
				elem.SetActualText(new PdfString(GetActualText()));
			}
			if (GetAlternateDescription() != null)
			{
				elem.SetAlt(new PdfString(GetAlternateDescription()));
			}
			if (GetExpansion() != null)
			{
				elem.SetE(new PdfString(GetExpansion()));
			}
			if (GetLanguage() != null)
			{
				elem.SetLang(new PdfString(GetLanguage()));
			}
			IList<PdfDictionary> newAttributesList = GetAttributesList();
			if (newAttributesList.Count > 0)
			{
				PdfObject attributesObject = elem.GetAttributes(false);
				PdfObject combinedAttributes = CombineAttributesList(attributesObject, newAttributesList
					, elem.GetPdfObject().GetAsNumber(PdfName.R));
				elem.SetAttributes(combinedAttributes);
			}
		}

		protected internal virtual PdfObject CombineAttributesList(PdfObject attributesObject
			, IList<PdfDictionary> newAttributesList, PdfNumber revision)
		{
			PdfObject combinedAttributes;
			if (attributesObject is PdfDictionary)
			{
				PdfArray combinedAttributesArray = new PdfArray();
				combinedAttributesArray.Add(attributesObject);
				AddNewAttributesToAttributesArray(newAttributesList, revision, combinedAttributesArray
					);
				combinedAttributes = combinedAttributesArray;
			}
			else
			{
				if (attributesObject is PdfArray)
				{
					PdfArray combinedAttributesArray = (PdfArray)attributesObject;
					AddNewAttributesToAttributesArray(newAttributesList, revision, combinedAttributesArray
						);
					combinedAttributes = combinedAttributesArray;
				}
				else
				{
					if (newAttributesList.Count == 1)
					{
						combinedAttributes = newAttributesList[0];
					}
					else
					{
						combinedAttributes = new PdfArray();
						AddNewAttributesToAttributesArray(newAttributesList, revision, (PdfArray)combinedAttributes
							);
					}
				}
			}
			return combinedAttributes;
		}

		protected internal virtual void AddNewAttributesToAttributesArray(IList<PdfDictionary
			> newAttributesList, PdfNumber revision, PdfArray attributesArray)
		{
			if (revision != null)
			{
				foreach (PdfDictionary attributes in newAttributesList)
				{
					attributesArray.Add(attributes);
					attributesArray.Add(revision);
				}
			}
			else
			{
				foreach (PdfDictionary newAttribute in newAttributesList)
				{
					attributesArray.Add(newAttribute);
				}
			}
		}
	}
}
