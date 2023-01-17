/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.IO;
using iText.Kernel.Pdf.Colorspace;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// Specify the colour characteristics of output devices on which the document might be rendered
    /// See ISO 32000-1 14.11.5: Output Intents.
    /// </summary>
    public class PdfOutputIntent : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Creates output intent dictionary.</summary>
        /// <remarks>
        /// Creates output intent dictionary. Null values are allowed to
        /// suppress any key.
        /// By default output intent subtype is GTS_PDFA1, use setter to change it.
        /// </remarks>
        /// <param name="outputConditionIdentifier">
        /// (required) identifying the intended output device or production condition in
        /// human or machine readable form
        /// </param>
        /// <param name="outputCondition">
        /// (optional) identifying the intended output device or production
        /// condition in human-readable form
        /// </param>
        /// <param name="registryName">
        /// identifying the registry in which the condition designated by
        /// <paramref name="outputConditionIdentifier"/>
        /// is defined
        /// </param>
        /// <param name="info">
        /// (required if
        /// <paramref name="outputConditionIdentifier"/>
        /// does not specify a standard
        /// production condition; optional otherwise) containing additional information or
        /// comments about the intended target device or production condition
        /// </param>
        /// <param name="iccStream">ICC profile stream. User is responsible for closing the stream</param>
        public PdfOutputIntent(String outputConditionIdentifier, String outputCondition, String registryName, String
             info, Stream iccStream)
            : base(new PdfDictionary()) {
            SetOutputIntentSubtype(PdfName.GTS_PDFA1);
            GetPdfObject().Put(PdfName.Type, PdfName.OutputIntent);
            if (outputCondition != null) {
                SetOutputCondition(outputCondition);
            }
            if (outputConditionIdentifier != null) {
                SetOutputConditionIdentifier(outputConditionIdentifier);
            }
            if (registryName != null) {
                SetRegistryName(registryName);
            }
            if (info != null) {
                SetInfo(info);
            }
            if (iccStream != null) {
                SetDestOutputProfile(iccStream);
            }
        }

        public PdfOutputIntent(PdfDictionary outputIntentDict)
            : base(outputIntentDict) {
        }

        public virtual PdfStream GetDestOutputProfile() {
            return GetPdfObject().GetAsStream(PdfName.DestOutputProfile);
        }

        public virtual void SetDestOutputProfile(Stream iccStream) {
            PdfStream stream = PdfCieBasedCs.IccBased.GetIccProfileStream(iccStream);
            GetPdfObject().Put(PdfName.DestOutputProfile, stream);
        }

        public virtual PdfString GetInfo() {
            return GetPdfObject().GetAsString(PdfName.Info);
        }

        public virtual void SetInfo(String info) {
            GetPdfObject().Put(PdfName.Info, new PdfString(info));
        }

        public virtual PdfString GetRegistryName() {
            return GetPdfObject().GetAsString(PdfName.RegistryName);
        }

        public virtual void SetRegistryName(String registryName) {
            GetPdfObject().Put(PdfName.RegistryName, new PdfString(registryName));
        }

        public virtual PdfString GetOutputConditionIdentifier() {
            return GetPdfObject().GetAsString(PdfName.OutputConditionIdentifier);
        }

        public virtual void SetOutputConditionIdentifier(String outputConditionIdentifier) {
            GetPdfObject().Put(PdfName.OutputConditionIdentifier, new PdfString(outputConditionIdentifier));
        }

        public virtual PdfString GetOutputCondition() {
            return GetPdfObject().GetAsString(PdfName.OutputCondition);
        }

        public virtual void SetOutputCondition(String outputCondition) {
            GetPdfObject().Put(PdfName.OutputCondition, new PdfString(outputCondition));
        }

        public virtual PdfName GetOutputIntentSubtype() {
            return GetPdfObject().GetAsName(PdfName.S);
        }

        public virtual void SetOutputIntentSubtype(PdfName subtype) {
            GetPdfObject().Put(PdfName.S, subtype);
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return false;
        }
    }
}
