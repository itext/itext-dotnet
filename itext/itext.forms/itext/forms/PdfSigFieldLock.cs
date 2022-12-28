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
        /// The dictionary whose entries should be added to
        /// the signature field lock dictionary.
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
        /// <param name="permissions">The permissions granted for the document.</param>
        /// <returns>
        /// This
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
        /// Indicates the set of fields that should be locked after the actual
        /// signing of the corresponding signature takes place.
        /// </param>
        /// <param name="fields">Names indicating the fields.</param>
        /// <returns>
        /// This
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
