/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
using System.Reflection;

namespace iText.Kernel.Pdf
{
	internal static class PdfNameLoader
	{
		internal static IDictionary<String, PdfName> LoadNames()
		{
			FieldInfo[] fields = typeof(PdfName).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
			IDictionary<String, PdfName> staticNames = new Dictionary<String, PdfName>(fields.Length);
			try {
				for (int fldIdx = 0; fldIdx < fields.Length; ++fldIdx) {
					FieldInfo curFld = fields[fldIdx];
					if (curFld.FieldType.Equals(typeof(PdfName))) {
						PdfName name = (PdfName)curFld.GetValue(null);
						staticNames[name.GetValue()] = name;
					}
				}
			} catch {
				return null;
			}
			return staticNames;
		}
	}
}
