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
using System.Collections.Generic;
using iText.Kernel.Pdf;

namespace iText.Signatures {
    /// <summary>
    /// A helper class that tells you more about the type of signature
    /// (certification or approval) and the signature's DMP settings.
    /// </summary>
    public class SignaturePermissions {
        /// <summary>
        /// Class that contains a field lock action and
        /// an array of the fields that are involved.
        /// </summary>
        public class FieldLock {
            /// <summary>Can be /All, /Exclude or /Include</summary>
            internal PdfName action;

            /// <summary>An array of PdfString values with fieldnames</summary>
            internal PdfArray fields;

            /// <summary>Creates a FieldLock instance.</summary>
            /// <param name="action">indicates the set of fields that should be locked</param>
            /// <param name="fields">an array of text strings containing field names</param>
            public FieldLock(SignaturePermissions _enclosing, PdfName action, PdfArray fields) {
                this._enclosing = _enclosing;
                this.action = action;
                this.fields = fields;
            }

            /// <summary>Getter for the field lock action.</summary>
            /// <returns>the action of field lock dictionary</returns>
            public virtual PdfName GetAction() {
                return this.action;
            }

            /// <summary>Getter for the fields involved in the lock action.</summary>
            /// <returns>the fields of field lock dictionary</returns>
            public virtual PdfArray GetFields() {
                return this.fields;
            }

            /// <summary>toString method</summary>
            public override String ToString() {
                return this.action.ToString() + (this.fields == null ? "" : this.fields.ToString());
            }

            private readonly SignaturePermissions _enclosing;
        }

        /// <summary>Is the signature a cerification signature (true) or an approval signature (false)?</summary>
        internal bool certification = false;

        /// <summary>Is form filling allowed by this signature?</summary>
        internal bool fillInAllowed = true;

        /// <summary>Is adding annotations allowed by this signature?</summary>
        internal bool annotationsAllowed = true;

        /// <summary>Does this signature lock specific fields?</summary>
        internal IList<SignaturePermissions.FieldLock> fieldLocks = new List<SignaturePermissions.FieldLock>();

        /// <summary>
        /// Creates an object that can inform you about the type of signature
        /// in a signature dictionary as well as some of the permissions
        /// defined by the signature.
        /// </summary>
        /// <param name="sigDict">the signature dictionary</param>
        /// <param name="previous">the signature permissions</param>
        public SignaturePermissions(PdfDictionary sigDict, SignaturePermissions previous) {
            if (previous != null) {
                annotationsAllowed &= previous.IsAnnotationsAllowed();
                fillInAllowed &= previous.IsFillInAllowed();
                fieldLocks.AddAll(previous.GetFieldLocks());
            }
            PdfArray @ref = sigDict.GetAsArray(PdfName.Reference);
            if (@ref != null) {
                for (int i = 0; i < @ref.Size(); i++) {
                    PdfDictionary dict = @ref.GetAsDictionary(i);
                    PdfDictionary @params = dict.GetAsDictionary(PdfName.TransformParams);
                    if (PdfName.DocMDP.Equals(dict.GetAsName(PdfName.TransformMethod))) {
                        certification = true;
                    }
                    PdfName action = @params.GetAsName(PdfName.Action);
                    if (action != null) {
                        fieldLocks.Add(new SignaturePermissions.FieldLock(this, action, @params.GetAsArray(PdfName.Fields)));
                    }
                    PdfNumber p = @params.GetAsNumber(PdfName.P);
                    if (p == null) {
                        continue;
                    }
                    switch (p.IntValue()) {
                        default: {
                            break;
                        }

                        case 1: {
                            fillInAllowed &= false;
                            goto case 2;
                        }

                        case 2: {
                            annotationsAllowed &= false;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>Getter to find out if the signature is a certification signature.</summary>
        /// <returns>true if the signature is a certification signature, false for an approval signature.</returns>
        public virtual bool IsCertification() {
            return certification;
        }

        /// <summary>Getter to find out if filling out fields is allowed after signing.</summary>
        /// <returns>true if filling out fields is allowed</returns>
        public virtual bool IsFillInAllowed() {
            return fillInAllowed;
        }

        /// <summary>Getter to find out if adding annotations is allowed after signing.</summary>
        /// <returns>true if adding annotations is allowed</returns>
        public virtual bool IsAnnotationsAllowed() {
            return annotationsAllowed;
        }

        /// <summary>Getter for the field lock actions, and fields that are impacted by the action</summary>
        /// <returns>an Array with field names</returns>
        public virtual IList<SignaturePermissions.FieldLock> GetFieldLocks() {
            return fieldLocks;
        }
    }
}
