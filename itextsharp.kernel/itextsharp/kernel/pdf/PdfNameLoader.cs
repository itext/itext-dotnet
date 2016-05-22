using System;
using System.Collections.Generic;
using System.Reflection;

namespace iTextSharp.Kernel.Pdf
{
	internal sealed class PdfNameLoader
	{
		internal static IDictionary<String, PdfName> LoadPdfNames()
		{
			FieldInfo[] fields = typeof(PdfName).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
			IDictionary<String, PdfName> staticNames = new Dictionary<String, PdfName>(fields.Length);
			try {
				for (int fldIdx = 0; fldIdx < fields.Length; ++fldIdx) {
					FieldInfo curFld = fields[fldIdx];
					if (curFld.FieldType.Equals(typeof(PdfName))) {
						PdfName name = (PdfName)curFld.GetValue(null);
						staticNames.Add(name.GetValue(), name);
					}
				}
			} 
			catch {}
			return staticNames;
		}
	}
}
