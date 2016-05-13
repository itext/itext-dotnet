/*
$Id: 70357d33d9f2db0f4bce8dc1b80dd3131b1cfb74 $

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
using iTextSharp.IO.Util;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Tagging;

namespace iTextSharp.Kernel.Pdf.Tagutils
{
	internal class BackedAccessibleProperties : AccessibilityProperties
	{
		private const long serialVersionUID = 4080083623525383278L;

		private PdfStructElem backingElem;

		internal BackedAccessibleProperties(PdfStructElem backingElem)
		{
			this.backingElem = backingElem;
		}

		public override String GetLanguage()
		{
			return backingElem.GetLang().GetValue();
		}

		public override AccessibilityProperties SetLanguage(String language)
		{
			backingElem.SetLang(new PdfString(language));
			return this;
		}

		public override String GetActualText()
		{
			return backingElem.GetActualText().GetValue();
		}

		public override AccessibilityProperties SetActualText(String actualText)
		{
			backingElem.SetActualText(new PdfString(actualText));
			return this;
		}

		public override String GetAlternateDescription()
		{
			return backingElem.GetAlt().GetValue();
		}

		public override AccessibilityProperties SetAlternateDescription(String alternateDescription
			)
		{
			backingElem.SetAlt(new PdfString(alternateDescription));
			return this;
		}

		public override String GetExpansion()
		{
			return backingElem.GetE().GetValue();
		}

		public override AccessibilityProperties SetExpansion(String expansion)
		{
			backingElem.SetE(new PdfString(expansion));
			return this;
		}

		public override AccessibilityProperties AddAttributes(PdfDictionary attributes)
		{
			PdfObject attributesObject = backingElem.GetAttributes(false);
			PdfObject combinedAttributes = CombineAttributesList(attributesObject, JavaCollectionsUtil
				.SingletonList(attributes), backingElem.GetPdfObject().GetAsNumber(PdfName.R));
			backingElem.SetAttributes(combinedAttributes);
			return this;
		}

		public override AccessibilityProperties ClearAttributes()
		{
			backingElem.GetPdfObject().Remove(PdfName.A);
			return this;
		}

		public override IList<PdfDictionary> GetAttributesList()
		{
			List<PdfDictionary> attributesList = new List<PdfDictionary>();
			PdfObject elemAttributesObj = backingElem.GetAttributes(false);
			if (elemAttributesObj != null)
			{
				if (elemAttributesObj.IsDictionary())
				{
					attributesList.Add((PdfDictionary)elemAttributesObj);
				}
				else
				{
					if (elemAttributesObj.IsArray())
					{
						PdfArray attributesArray = (PdfArray)elemAttributesObj;
						foreach (PdfObject attributeObj in attributesArray)
						{
							if (attributeObj.IsDictionary())
							{
								attributesList.Add((PdfDictionary)attributeObj);
							}
						}
					}
				}
			}
			return attributesList;
		}

		internal override void SetToStructElem(PdfStructElem elem)
		{
		}
		// ignore, because all attributes are directly set to the structElem
	}
}
