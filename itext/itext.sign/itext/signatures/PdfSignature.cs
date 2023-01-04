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
using iText.IO.Font;
using iText.Kernel.Pdf;

namespace iText.Signatures {
    /// <summary>Represents the signature dictionary.</summary>
    /// <author>Paulo Soares</author>
    public class PdfSignature : PdfObjectWrapper<PdfDictionary> {
        /// <summary>Creates new PdfSignature.</summary>
        public PdfSignature()
            : base(new PdfDictionary()) {
            Put(PdfName.Type, PdfName.Sig);
        }

        /// <summary>Creates new PdfSignature.</summary>
        /// <param name="filter">PdfName of the signature handler to use when validating this signature</param>
        /// <param name="subFilter">PdfName that describes the encoding of the signature</param>
        public PdfSignature(PdfName filter, PdfName subFilter)
            : this() {
            Put(PdfName.Filter, filter);
            Put(PdfName.SubFilter, subFilter);
        }

        public PdfSignature(PdfDictionary sigDictionary)
            : base(sigDictionary) {
            PdfString contents = GetPdfObject().GetAsString(PdfName.Contents);
            if (contents != null) {
                contents.MarkAsUnencryptedObject();
            }
        }

        /// <summary>A name that describes the encoding of the signature value and key information in the signature dictionary.
        ///     </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// which usually has a value either
        /// <see cref="iText.Kernel.Pdf.PdfName.Adbe_pkcs7_detached"/>
        /// or
        /// <see cref="iText.Kernel.Pdf.PdfName.ETSI_CAdES_DETACHED"/>.
        /// </returns>
        public virtual PdfName GetSubFilter() {
            return GetPdfObject().GetAsName(PdfName.SubFilter);
        }

        /// <summary>
        /// The type of PDF object that the wrapped dictionary describes; if present, shall be
        /// <see cref="iText.Kernel.Pdf.PdfName.Sig"/>
        /// for a signature
        /// dictionary or
        /// <see cref="iText.Kernel.Pdf.PdfName.DocTimeStamp"/>
        /// for a timestamp signature dictionary.
        /// </summary>
        /// <remarks>
        /// The type of PDF object that the wrapped dictionary describes; if present, shall be
        /// <see cref="iText.Kernel.Pdf.PdfName.Sig"/>
        /// for a signature
        /// dictionary or
        /// <see cref="iText.Kernel.Pdf.PdfName.DocTimeStamp"/>
        /// for a timestamp signature dictionary. Shall be not null if it's value
        /// is
        /// <see cref="iText.Kernel.Pdf.PdfName.DocTimeStamp"/>
        /// . The default value is:
        /// <see cref="iText.Kernel.Pdf.PdfName.Sig"/>.
        /// </remarks>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// that identifies type of the wrapped dictionary,
        /// returns null if it is not explicitly specified.
        /// </returns>
        public virtual PdfName GetType() {
            return GetPdfObject().GetAsName(PdfName.Type);
        }

        /// <summary>Sets the /ByteRange.</summary>
        /// <param name="range">
        /// an array of pairs of integers that specifies the byte range used in the digest calculation.
        /// A pair consists of the starting byte offset and the length
        /// </param>
        public virtual void SetByteRange(int[] range) {
            PdfArray array = new PdfArray();
            for (int k = 0; k < range.Length; ++k) {
                array.Add(new PdfNumber(range[k]));
            }
            Put(PdfName.ByteRange, array);
        }

        /// <summary>Gets the /ByteRange.</summary>
        /// <returns>
        /// an array of pairs of integers that specifies the byte range used in the digest calculation.
        /// A pair consists of the starting byte offset and the length.
        /// </returns>
        public virtual PdfArray GetByteRange() {
            return GetPdfObject().GetAsArray(PdfName.ByteRange);
        }

        /// <summary>Sets the /Contents value to the specified byte[].</summary>
        /// <param name="contents">a byte[] representing the digest</param>
        public virtual void SetContents(byte[] contents) {
            PdfString contentsString = new PdfString(contents).SetHexWriting(true);
            contentsString.MarkAsUnencryptedObject();
            Put(PdfName.Contents, contentsString);
        }

        /// <summary>Gets the /Contents entry value.</summary>
        /// <remarks>
        /// Gets the /Contents entry value.
        /// See ISO 32000-1 12.8.1, Table 252 – Entries in a signature dictionary.
        /// </remarks>
        /// <returns>the signature content</returns>
        public virtual PdfString GetContents() {
            return GetPdfObject().GetAsString(PdfName.Contents);
        }

        /// <summary>Sets the /Cert value of this signature.</summary>
        /// <param name="cert">the byte[] representing the certificate chain</param>
        public virtual void SetCert(byte[] cert) {
            Put(PdfName.Cert, new PdfString(cert));
        }

        /// <summary>Gets the /Cert entry value of this signature.</summary>
        /// <remarks>
        /// Gets the /Cert entry value of this signature.
        /// See ISO 32000-1 12.8.1, Table 252 – Entries in a signature dictionary.
        /// </remarks>
        /// <returns>the signature cert</returns>
        public virtual PdfString GetCert() {
            return GetPdfObject().GetAsString(PdfName.Cert);
        }

        /// <summary>Gets the /Cert entry value of this signature.</summary>
        /// <remarks>
        /// Gets the /Cert entry value of this signature.
        /// /Cert entry required when SubFilter is adbe.x509.rsa_sha1. May be array or byte string.
        /// </remarks>
        /// <returns>the signature cert value</returns>
        public virtual PdfObject GetCertObject() {
            PdfString certAsStr = GetPdfObject().GetAsString(PdfName.Cert);
            PdfArray certAsArray = GetPdfObject().GetAsArray(PdfName.Cert);
            if (certAsStr != null) {
                return certAsStr;
            }
            else {
                return certAsArray;
            }
        }

        /// <summary>Sets the /Name of the person signing the document.</summary>
        /// <param name="name">name of the person signing the document</param>
        public virtual void SetName(String name) {
            Put(PdfName.Name, new PdfString(name, PdfEncodings.UNICODE_BIG));
        }

        /// <summary>gets the /Name of the person signing the document.</summary>
        /// <returns>name of the person signing the document.</returns>
        public virtual String GetName() {
            PdfString nameStr = GetPdfObject().GetAsString(PdfName.Name);
            PdfName nameName = GetPdfObject().GetAsName(PdfName.Name);
            if (nameStr != null) {
                return nameStr.ToUnicodeString();
            }
            else {
                return nameName != null ? nameName.GetValue() : null;
            }
        }

        /// <summary>Sets the /M value.</summary>
        /// <remarks>Sets the /M value. Should only be used if the time of signing is not available in the signature.</remarks>
        /// <param name="date">time of signing</param>
        public virtual void SetDate(PdfDate date) {
            Put(PdfName.M, date.GetPdfObject());
        }

        /// <summary>Gets the /M value.</summary>
        /// <remarks>Gets the /M value. Should only be used if the time of signing is not available in the signature.</remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// which denotes time of signing.
        /// </returns>
        public virtual PdfString GetDate() {
            return GetPdfObject().GetAsString(PdfName.M);
        }

        /// <summary>Sets the /Location value.</summary>
        /// <param name="location">physical location of signing</param>
        public virtual void SetLocation(String location) {
            Put(PdfName.Location, new PdfString(location, PdfEncodings.UNICODE_BIG));
        }

        /// <summary>Gets the /Location entry value.</summary>
        /// <returns>physical location of signing.</returns>
        public virtual String GetLocation() {
            PdfString locationStr = GetPdfObject().GetAsString(PdfName.Location);
            return locationStr != null ? locationStr.ToUnicodeString() : null;
        }

        /// <summary>Sets the /Reason value.</summary>
        /// <param name="reason">reason for signing</param>
        public virtual void SetReason(String reason) {
            Put(PdfName.Reason, new PdfString(reason, PdfEncodings.UNICODE_BIG));
        }

        public virtual String GetReason() {
            PdfString reasonStr = GetPdfObject().GetAsString(PdfName.Reason);
            return reasonStr != null ? reasonStr.ToUnicodeString() : null;
        }

        /// <summary>
        /// Sets the signature creator name in the
        /// <see cref="PdfSignatureBuildProperties"/>
        /// dictionary.
        /// </summary>
        /// <param name="signatureCreator">name of the signature creator</param>
        public virtual void SetSignatureCreator(String signatureCreator) {
            if (signatureCreator != null) {
                GetPdfSignatureBuildProperties().SetSignatureCreator(signatureCreator);
            }
        }

        /// <summary>Sets the /ContactInfo value.</summary>
        /// <param name="contactInfo">information to contact the person who signed this document</param>
        public virtual void SetContact(String contactInfo) {
            Put(PdfName.ContactInfo, new PdfString(contactInfo, PdfEncodings.UNICODE_BIG));
        }

        public virtual iText.Signatures.PdfSignature Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        protected override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        /// <summary>
        /// Gets the
        /// <see cref="PdfSignatureBuildProperties"/>
        /// instance if it exists, if
        /// not it adds a new one and returns this.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="PdfSignatureBuildProperties"/>
        /// </returns>
        private PdfSignatureBuildProperties GetPdfSignatureBuildProperties() {
            PdfDictionary buildPropDict = GetPdfObject().GetAsDictionary(PdfName.Prop_Build);
            if (buildPropDict == null) {
                buildPropDict = new PdfDictionary();
                Put(PdfName.Prop_Build, buildPropDict);
            }
            return new PdfSignatureBuildProperties(buildPropDict);
        }
    }
}
