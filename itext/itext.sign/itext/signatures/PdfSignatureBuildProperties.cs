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
using iText.Kernel.Pdf;

namespace iText.Signatures {
    /// <summary>Dictionary that stores signature build properties.</summary>
    public class PdfSignatureBuildProperties : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Creates new PdfSignatureBuildProperties.</summary>
        public PdfSignatureBuildProperties()
            : base(new PdfDictionary()) {
        }

        /// <summary>Creates new PdfSignatureBuildProperties with preset values.</summary>
        /// <param name="dict">PdfDictionary containing preset values</param>
        public PdfSignatureBuildProperties(PdfDictionary dict)
            : base(dict) {
        }

        /// <summary>
        /// Sets the signatureCreator property in the underlying
        /// <see cref="PdfSignatureApp"/>
        /// dictionary.
        /// </summary>
        /// <param name="name">the signature creator's name to be set</param>
        public virtual void SetSignatureCreator(String name) {
            GetPdfSignatureAppProperty().SetSignatureCreator(name);
        }

        /// <summary>
        /// Gets the
        /// <see cref="PdfSignatureApp"/>
        /// from this dictionary.
        /// </summary>
        /// <remarks>
        /// Gets the
        /// <see cref="PdfSignatureApp"/>
        /// from this dictionary. If it
        /// does not exist, it adds a new
        /// <see cref="PdfSignatureApp"/>
        /// and
        /// returns this instance.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="PdfSignatureApp"/>
        /// </returns>
        private PdfSignatureApp GetPdfSignatureAppProperty() {
            PdfDictionary appPropDic = GetPdfObject().GetAsDictionary(PdfName.App);
            if (appPropDic == null) {
                appPropDic = new PdfDictionary();
                GetPdfObject().Put(PdfName.App, appPropDic);
            }
            return new PdfSignatureApp(appPropDic);
        }

        protected override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
