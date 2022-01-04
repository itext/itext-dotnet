/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using iText.Kernel.Pdf;

namespace iText.Signatures {
    /// <summary>Dictionary that stores signature build properties.</summary>
    /// <author>Kwinten Pisman</author>
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
