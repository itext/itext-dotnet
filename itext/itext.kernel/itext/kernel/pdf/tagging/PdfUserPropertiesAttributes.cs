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
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Tagging {
    public class PdfUserPropertiesAttributes : PdfStructureAttributes {
        public PdfUserPropertiesAttributes(PdfDictionary attributesDict)
            : base(attributesDict) {
        }

        public PdfUserPropertiesAttributes()
            : base(new PdfDictionary()) {
            GetPdfObject().Put(PdfName.O, PdfName.UserProperties);
            GetPdfObject().Put(PdfName.P, new PdfArray());
        }

        public PdfUserPropertiesAttributes(IList<PdfUserProperty> userProperties)
            : this() {
            PdfArray arr = GetPdfObject().GetAsArray(PdfName.P);
            foreach (PdfUserProperty userProperty in userProperties) {
                arr.Add(userProperty.GetPdfObject());
            }
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserPropertiesAttributes AddUserProperty(PdfUserProperty userProperty
            ) {
            GetPdfObject().GetAsArray(PdfName.P).Add(userProperty.GetPdfObject());
            SetModified();
            return this;
        }

        public virtual PdfUserProperty GetUserProperty(int i) {
            PdfDictionary propDict = GetPdfObject().GetAsArray(PdfName.P).GetAsDictionary(i);
            if (propDict == null) {
                return null;
            }
            return new PdfUserProperty(propDict);
        }

        public virtual iText.Kernel.Pdf.Tagging.PdfUserPropertiesAttributes RemoveUserProperty(int i) {
            GetPdfObject().GetAsArray(PdfName.P).Remove(i);
            return this;
        }

        public virtual int GetNumberOfUserProperties() {
            return GetPdfObject().GetAsArray(PdfName.P).Size();
        }
    }
}
