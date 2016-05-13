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
using iTextSharp.Forms.Fields;
using iTextSharp.IO;
using iTextSharp.IO.Log;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Annot;

namespace iTextSharp.Forms
{
	/// <summary>
	/// A sample implementation of the {#link IPdfPageExtraCopier} interface which
	/// copies only AcroForm fields to a new page.
	/// </summary>
	public class PdfPageFormCopier : IPdfPageExtraCopier
	{
		internal PdfAcroForm formFrom;

		internal PdfAcroForm formTo;

		internal PdfDocument documentFrom;

		internal PdfDocument documentTo;

		public virtual void Copy(PdfPage fromPage, PdfPage toPage)
		{
			if (documentFrom != fromPage.GetDocument())
			{
				documentFrom = fromPage.GetDocument();
				formFrom = PdfAcroForm.GetAcroForm(documentFrom, false);
			}
			if (documentTo != toPage.GetDocument())
			{
				documentTo = toPage.GetDocument();
				formTo = PdfAcroForm.GetAcroForm(documentTo, true);
				if (formFrom != null)
				{
					//duplicate AcroForm dictionary
					IList<PdfName> excludedKeys = new List<PdfName>();
					excludedKeys.Add(PdfName.Fields);
					excludedKeys.Add(PdfName.DR);
					PdfDictionary dict = formFrom.GetPdfObject().CopyTo(documentTo, excludedKeys, false
						);
					formTo.GetPdfObject().MergeDifferent(dict);
				}
			}
			IList<PdfDictionary> usedParents = new List<PdfDictionary>();
			if (formFrom != null)
			{
				IDictionary<String, PdfFormField> fieldsFrom = formFrom.GetFormFields();
				if (fieldsFrom.Count > 0)
				{
					IDictionary<String, PdfFormField> fieldsTo = formTo.GetFormFields();
					IList<PdfAnnotation> annots = toPage.GetAnnotations();
					foreach (PdfAnnotation annot in annots)
					{
						if (annot.GetSubtype().Equals(PdfName.Widget))
						{
							PdfDictionary parent = annot.GetPdfObject().GetAsDictionary(PdfName.Parent);
							if (parent != null)
							{
								PdfString parentName = parent.GetAsString(PdfName.T);
								if (parentName == null)
								{
									continue;
								}
								if (!usedParents.Contains(parent))
								{
									PdfFormField field = PdfFormField.MakeFormField(parent, toPage.GetDocument());
									field.GetKids().Clear();
									formTo.AddField(field, toPage);
									usedParents.Add(parent);
									field.AddKid((PdfWidgetAnnotation)annot);
								}
								else
								{
									parent.GetAsArray(PdfName.Kids).Add(annot.GetPdfObject());
								}
							}
							else
							{
								PdfString annotName = annot.GetPdfObject().GetAsString(PdfName.T);
								String annotNameString = null;
								if (annotName != null)
								{
									annotNameString = annotName.ToUnicodeString();
								}
								if (annotNameString != null && fieldsFrom.ContainsKey(annotNameString))
								{
									PdfFormField field = PdfFormField.MakeFormField(annot.GetPdfObject(), toPage.GetDocument
										());
									if (fieldsTo.ContainsKey(annotNameString))
									{
										field = MergeFieldsWithTheSameName(field, fieldsTo[annotNameString]);
										Logger logger = LoggerFactory.GetLogger(typeof(PdfPageFormCopier));
										logger.Warn(String.Format(LogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, annotNameString
											));
									}
									formTo.AddField(field, null);
								}
							}
						}
					}
				}
			}
		}

		private PdfFormField MergeFieldsWithTheSameName(PdfFormField existingField, PdfFormField
			 newField)
		{
			String fieldName = newField.GetFieldName().ToUnicodeString();
			existingField.GetPdfObject().Remove(PdfName.T);
			PdfFormField mergedField = formTo.GetField(fieldName);
			PdfArray kids = mergedField.GetKids();
			if (kids != null && !kids.IsEmpty())
			{
				mergedField.AddKid(existingField);
				return mergedField;
			}
			newField.GetPdfObject().Remove(PdfName.T);
			mergedField = PdfFormField.CreateEmptyField(documentTo);
			formTo.GetFields().Remove(newField.GetPdfObject());
			mergedField.Put(PdfName.FT, existingField.GetFormType()).Put(PdfName.T, new PdfString
				(fieldName));
			PdfDictionary parent = existingField.GetParent();
			if (parent != null)
			{
				mergedField.Put(PdfName.Parent, parent);
			}
			kids = existingField.GetKids();
			if (kids != null)
			{
				mergedField.Put(PdfName.Kids, kids);
			}
			mergedField.AddKid(existingField).AddKid(newField);
			return mergedField;
		}
	}
}
