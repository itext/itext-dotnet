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
using System;
using iText.Kernel.Pdf;

namespace iText.Forms {
    /// <summary>A signature field lock dictionary.</summary>
    /// <remarks>
    /// A signature field lock dictionary. Specifies a set of form
    /// fields that shall be locked when this signature field is
    /// signed.
    /// </remarks>
    public class PdfSigFieldLock : PdfObjectWrapper<PdfDictionary> {
        /// <summary>
        /// Creates an instance of
        /// <see cref="PdfSigFieldLock"/>.
        /// </summary>
        public PdfSigFieldLock()
            : this(new PdfDictionary()) {
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="PdfSigFieldLock"/>.
        /// </summary>
        /// <param name="dict">
        /// the dictionary whose entries should be added to
        /// the signature field lock dictionary
        /// </param>
        public PdfSigFieldLock(PdfDictionary dict)
            : base(dict) {
            GetPdfObject().Put(PdfName.Type, PdfName.SigFieldLock);
        }

        /// <summary>
        /// Sets the permissions granted for the document when the corresponding signature
        /// field is signed.
        /// </summary>
        /// <remarks>
        /// Sets the permissions granted for the document when the corresponding signature
        /// field is signed. See
        /// <see cref="LockPermissions"/>
        /// for getting more info.
        /// </remarks>
        /// <param name="permissions">the permissions granted for the document</param>
        /// <returns>
        /// this
        /// <see cref="PdfSigFieldLock"/>
        /// object.
        /// </returns>
        public virtual iText.Forms.PdfSigFieldLock SetDocumentPermissions(PdfSigFieldLock.LockPermissions permissions
            ) {
            GetPdfObject().Put(PdfName.P, GetLockPermission(permissions));
            return this;
        }

        /// <summary>Sets signature lock for specific fields in the document.</summary>
        /// <param name="action">
        /// indicates the set of fields that should be locked after the actual
        /// signing of the corresponding signature takes place
        /// </param>
        /// <param name="fields">names indicating the fields</param>
        /// <returns>
        /// this
        /// <see cref="PdfSigFieldLock"/>
        /// object.
        /// </returns>
        public virtual iText.Forms.PdfSigFieldLock SetFieldLock(PdfSigFieldLock.LockAction action, params String[]
             fields) {
            PdfArray fieldsArray = new PdfArray();
            foreach (String field in fields) {
                fieldsArray.Add(new PdfString(field));
            }
            GetPdfObject().Put(PdfName.Action, GetLockActionValue(action));
            GetPdfObject().Put(PdfName.Fields, fieldsArray);
            return this;
        }

        /// <summary>
        /// Returns the specified action of a signature field lock as
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// value.
        /// </summary>
        /// <param name="action">
        /// the action as
        /// <see cref="LockAction"/>
        /// </param>
        /// <returns>
        /// the specified action of a signature field lock as
        /// <see cref="iText.Kernel.Pdf.PdfName"/>.
        /// </returns>
        public static PdfName GetLockActionValue(PdfSigFieldLock.LockAction action) {
            switch (action) {
                case PdfSigFieldLock.LockAction.ALL: {
                    return PdfName.All;
                }

                case PdfSigFieldLock.LockAction.INCLUDE: {
                    return PdfName.Include;
                }

                case PdfSigFieldLock.LockAction.EXCLUDE: {
                    return PdfName.Exclude;
                }

                default: {
                    return PdfName.All;
                }
            }
        }

        /// <summary>
        /// Returns the specified level of access permissions granted for the document as
        /// <see cref="iText.Kernel.Pdf.PdfNumber"/>
        /// value.
        /// </summary>
        /// <param name="permissions">
        /// the level of access permissions as
        /// <see cref="LockPermissions"/>
        /// </param>
        /// <returns>
        /// the specified level of access permissions as
        /// <see cref="iText.Kernel.Pdf.PdfNumber"/>.
        /// </returns>
        public static PdfNumber GetLockPermission(PdfSigFieldLock.LockPermissions permissions) {
            switch (permissions) {
                case PdfSigFieldLock.LockPermissions.NO_CHANGES_ALLOWED: {
                    return new PdfNumber(1);
                }

                case PdfSigFieldLock.LockPermissions.FORM_FILLING: {
                    return new PdfNumber(2);
                }

                case PdfSigFieldLock.LockPermissions.FORM_FILLING_AND_ANNOTATION: {
                    return new PdfNumber(3);
                }

                default: {
                    return new PdfNumber(0);
                }
            }
        }

        /// <summary>Enumerates the different actions of a signature field lock.</summary>
        /// <remarks>
        /// Enumerates the different actions of a signature field lock.
        /// Indicates the set of fields that should be locked when the
        /// corresponding signature field is signed:
        /// <list type="bullet">
        /// <item><description>all the fields in the document,
        /// </description></item>
        /// <item><description>all the fields specified in the /Fields array,
        /// </description></item>
        /// <item><description>all the fields except those specified in the /Fields array.
        /// </description></item>
        /// </list>
        /// </remarks>
        public enum LockAction {
            ALL,
            INCLUDE,
            EXCLUDE
        }

        /// <summary>
        /// Enumerates the different levels of access permissions granted for
        /// the document when the corresponding signature field is signed:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="NO_CHANGES_ALLOWED"/>
        /// - no changes to the document are
        /// permitted; any change to the document invalidates the signature,
        /// </description></item>
        /// <item><description>
        /// <see cref="FORM_FILLING"/>
        /// - permitted changes are filling in forms,
        /// instantiating page templates, and signing; other changes invalidate
        /// the signature,
        /// </description></item>
        /// <item><description>
        /// <see cref="FORM_FILLING_AND_ANNOTATION"/>
        /// - permitted changes are the
        /// same as for the previous, as well as annotation creation, deletion,
        /// and modification; other changes invalidate the signature.
        /// </summary>
        /// <remarks>
        /// Enumerates the different levels of access permissions granted for
        /// the document when the corresponding signature field is signed:
        /// <list type="bullet">
        /// <item><description>
        /// <see cref="NO_CHANGES_ALLOWED"/>
        /// - no changes to the document are
        /// permitted; any change to the document invalidates the signature,
        /// </description></item>
        /// <item><description>
        /// <see cref="FORM_FILLING"/>
        /// - permitted changes are filling in forms,
        /// instantiating page templates, and signing; other changes invalidate
        /// the signature,
        /// </description></item>
        /// <item><description>
        /// <see cref="FORM_FILLING_AND_ANNOTATION"/>
        /// - permitted changes are the
        /// same as for the previous, as well as annotation creation, deletion,
        /// and modification; other changes invalidate the signature.
        /// </description></item>
        /// </list>
        /// </remarks>
        public enum LockPermissions {
            NO_CHANGES_ALLOWED,
            FORM_FILLING,
            FORM_FILLING_AND_ANNOTATION
        }

        protected override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }
    }
}
