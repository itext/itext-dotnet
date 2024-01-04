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
