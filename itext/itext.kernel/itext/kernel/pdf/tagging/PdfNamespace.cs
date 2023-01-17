/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;

namespace iText.Kernel.Pdf.Tagging {
    /// <summary>A wrapper for namespace dictionaries (ISO 32000-2 section 14.7.4).</summary>
    /// <remarks>
    /// A wrapper for namespace dictionaries (ISO 32000-2 section 14.7.4).
    /// A namespace dictionary defines a namespace within the structure tree.
    /// <para />
    /// This pdf entity is meaningful only for the PDF documents of version <b>2.0 and higher</b>.
    /// </remarks>
    public class PdfNamespace : PdfObjectWrapper<PdfDictionary> {
        /// <summary>
        /// Constructs namespace from the given
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents namespace dictionary.
        /// </summary>
        /// <remarks>
        /// Constructs namespace from the given
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents namespace dictionary.
        /// This method is useful for property reading in reading mode or modifying in stamping mode.
        /// </remarks>
        /// <param name="dictionary">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// that represents namespace in the document.
        /// </param>
        public PdfNamespace(PdfDictionary dictionary)
            : base(dictionary) {
            SetForbidRelease();
        }

        /// <summary>Constructs a namespace defined by the given namespace name.</summary>
        /// <param name="namespaceName">
        /// a
        /// <see cref="System.String"/>
        /// defining the namespace name (conventionally a uniform
        /// resource identifier, or URI).
        /// </param>
        public PdfNamespace(String namespaceName)
            : this(new PdfString(namespaceName)) {
        }

        /// <summary>Constructs a namespace defined by the given namespace name.</summary>
        /// <param name="namespaceName">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// defining the namespace name (conventionally a uniform
        /// resource identifier, or URI).
        /// </param>
        public PdfNamespace(PdfString namespaceName)
            : this(new PdfDictionary()) {
            Put(PdfName.Type, PdfName.Namespace);
            Put(PdfName.NS, namespaceName);
        }

        /// <summary>Sets the string defining the namespace name.</summary>
        /// <param name="namespaceName">
        /// a
        /// <see cref="System.String"/>
        /// defining the namespace name (conventionally a uniform
        /// resource identifier, or URI).
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfNamespace"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace SetNamespaceName(String namespaceName) {
            return SetNamespaceName(new PdfString(namespaceName));
        }

        /// <summary>Sets the string defining the namespace name.</summary>
        /// <param name="namespaceName">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// defining the namespace name (conventionally a uniform
        /// resource identifier, or URI).
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfNamespace"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace SetNamespaceName(PdfString namespaceName) {
            return Put(PdfName.NS, namespaceName);
        }

        /// <summary>Gets the string defining the namespace name.</summary>
        /// <returns>
        /// a
        /// <see cref="System.String"/>
        /// defining the namespace name (conventionally a uniform
        /// resource identifier, or URI).
        /// </returns>
        public virtual String GetNamespaceName() {
            PdfString ns = GetPdfObject().GetAsString(PdfName.NS);
            return ns != null ? ns.ToUnicodeString() : null;
        }

        /// <summary>Sets file specification identifying the schema file, which defines this namespace.</summary>
        /// <param name="fileSpec">
        /// a
        /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>
        /// identifying the schema file.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfNamespace"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace SetSchema(PdfFileSpec fileSpec) {
            return Put(PdfName.Schema, fileSpec.GetPdfObject());
        }

        /// <summary>Gets file specification identifying the schema file, which defines this namespace.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.Filespec.PdfFileSpec"/>
        /// identifying the schema file.
        /// </returns>
        public virtual PdfFileSpec GetSchema() {
            PdfObject schemaObject = GetPdfObject().Get(PdfName.Schema);
            return PdfFileSpec.WrapFileSpecObject(schemaObject);
        }

        /// <summary>
        /// A dictionary that maps the names of structure types used in the namespace to their approximate equivalents in another
        /// namespace.
        /// </summary>
        /// <param name="roleMapNs">
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which is comprised of a set of keys representing structure element types
        /// in the namespace defined within this namespace dictionary. The corresponding value for each of these
        /// keys shall either be a single
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// identifying a structure element type in the default
        /// namespace or an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// where the first value shall be a structure element type name
        /// in a target namespace with the second value being an indirect reference to the target namespace dictionary.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfNamespace"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace SetNamespaceRoleMap(PdfDictionary roleMapNs) {
            return Put(PdfName.RoleMapNS, roleMapNs);
        }

        /// <summary>
        /// A dictionary that maps the names of structure types used in the namespace to their approximate equivalents in another
        /// namespace.
        /// </summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDictionary"/>
        /// which is comprised of a set of keys representing structure element types
        /// in the namespace defined within this namespace dictionary. The corresponding value for each of these
        /// keys shall either be a single
        /// <see cref="iText.Kernel.Pdf.PdfName"/>
        /// identifying a structure element type in the default
        /// namespace or an
        /// <see cref="iText.Kernel.Pdf.PdfArray"/>
        /// where the first value shall be a structure element type name
        /// in a target namespace with the second value being an indirect reference to the target namespace dictionary.
        /// </returns>
        public virtual PdfDictionary GetNamespaceRoleMap() {
            return GetNamespaceRoleMap(false);
        }

        /// <summary>
        /// Adds to the namespace role map (see
        /// <see cref="SetNamespaceRoleMap(iText.Kernel.Pdf.PdfDictionary)"/>
        /// ) a single role mapping to the
        /// default standard structure namespace.
        /// </summary>
        /// <param name="thisNsRole">
        /// a
        /// <see cref="System.String"/>
        /// identifying structure element type in this namespace.
        /// </param>
        /// <param name="defaultNsRole">
        /// a
        /// <see cref="System.String"/>
        /// identifying a structure element type in the default standard structure namespace.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfNamespace"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace AddNamespaceRoleMapping(String thisNsRole, String defaultNsRole
            ) {
            PdfObject prevVal = GetNamespaceRoleMap(true).Put(PdfStructTreeRoot.ConvertRoleToPdfName(thisNsRole), PdfStructTreeRoot
                .ConvertRoleToPdfName(defaultNsRole));
            LogOverwritingOfMappingIfNeeded(thisNsRole, prevVal);
            SetModified();
            return this;
        }

        /// <summary>
        /// Adds to the namespace role map (see
        /// <see cref="SetNamespaceRoleMap(iText.Kernel.Pdf.PdfDictionary)"/>
        /// ) a single role mapping to the
        /// target namespace.
        /// </summary>
        /// <param name="thisNsRole">
        /// a
        /// <see cref="System.String"/>
        /// identifying structure element type in this namespace.
        /// </param>
        /// <param name="targetNsRole">
        /// a
        /// <see cref="System.String"/>
        /// identifying a structure element type in the target namespace.
        /// </param>
        /// <param name="targetNs">
        /// a
        /// <see cref="PdfNamespace"/>
        /// identifying the target namespace.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="PdfNamespace"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Pdf.Tagging.PdfNamespace AddNamespaceRoleMapping(String thisNsRole, String targetNsRole
            , iText.Kernel.Pdf.Tagging.PdfNamespace targetNs) {
            PdfArray targetMapping = new PdfArray();
            targetMapping.Add(PdfStructTreeRoot.ConvertRoleToPdfName(targetNsRole));
            targetMapping.Add(targetNs.GetPdfObject());
            PdfObject prevVal = GetNamespaceRoleMap(true).Put(PdfStructTreeRoot.ConvertRoleToPdfName(thisNsRole), targetMapping
                );
            LogOverwritingOfMappingIfNeeded(thisNsRole, prevVal);
            SetModified();
            return this;
        }

        protected internal override bool IsWrappedObjectMustBeIndirect() {
            return true;
        }

        private iText.Kernel.Pdf.Tagging.PdfNamespace Put(PdfName key, PdfObject value) {
            GetPdfObject().Put(key, value);
            SetModified();
            return this;
        }

        private PdfDictionary GetNamespaceRoleMap(bool createIfNotExist) {
            PdfDictionary roleMapNs = GetPdfObject().GetAsDictionary(PdfName.RoleMapNS);
            if (createIfNotExist && roleMapNs == null) {
                roleMapNs = new PdfDictionary();
                Put(PdfName.RoleMapNS, roleMapNs);
            }
            return roleMapNs;
        }

        private void LogOverwritingOfMappingIfNeeded(String thisNsRole, PdfObject prevVal) {
            if (prevVal != null) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagging.PdfNamespace));
                String nsNameStr = GetNamespaceName();
                if (nsNameStr == null) {
                    nsNameStr = "this";
                }
                logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.MAPPING_IN_NAMESPACE_OVERWRITTEN
                    , thisNsRole, nsNameStr));
            }
        }
    }
}
