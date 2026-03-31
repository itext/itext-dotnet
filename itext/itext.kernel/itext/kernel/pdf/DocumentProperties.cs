/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Actions.Contexts;
using iText.Kernel.Exceptions;

namespace iText.Kernel.Pdf {
    /// <summary>
    /// Class with additional properties for
    /// <see cref="PdfDocument"/>
    /// processing.
    /// </summary>
    /// <remarks>
    /// Class with additional properties for
    /// <see cref="PdfDocument"/>
    /// processing.
    /// Needs to be passed at document initialization.
    /// </remarks>
    public class DocumentProperties {
        private readonly IDictionary<Type, Func<Object>> dependencies = new Dictionary<Type, Func<Object>>();

        protected internal IMetaInfo metaInfo = null;

        /// <summary>Default constructor, use provided setters for configuration options.</summary>
        public DocumentProperties() {
        }

        /// <summary>Creates a copy of class instance.</summary>
        /// <param name="other">the base for new class instance</param>
        public DocumentProperties(iText.Kernel.Pdf.DocumentProperties other) {
            this.metaInfo = other.metaInfo;
            this.dependencies.AddAll(other.dependencies);
        }

        /// <summary>Sets document meta info.</summary>
        /// <param name="metaInfo">meta info to set</param>
        /// <returns>
        /// this
        /// <see cref="DocumentProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.DocumentProperties SetEventCountingMetaInfo(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
            return this;
        }

        /// <summary>Checks if the document event counting meta info was already set.</summary>
        /// <returns>true if the document event counting meta info is set, false otherwise.</returns>
        public virtual bool IsEventCountingMetaInfoSet() {
            return this.metaInfo != null;
        }

        /// <summary>Register custom dependency for the document.</summary>
        /// <param name="clazz">Type of the dependency.</param>
        /// <param name="instance">The instance of the dependency.</param>
        /// <returns>
        /// this
        /// <see cref="DocumentProperties"/>
        /// instance
        /// </returns>
        [System.ObsoleteAttribute(@"in favor of RegisterDependency(System.Type{T}, System.Func{T})")]
        public virtual iText.Kernel.Pdf.DocumentProperties RegisterDependency(Type clazz, Object instance) {
            if (instance == null) {
                throw new ArgumentException(KernelExceptionMessageConstant.INSTANCE_SHOULD_NOT_BE_NULL);
            }
            RegisterDependency(clazz, () => instance);
            return this;
        }

        /// <summary>Register custom dependency for the document.</summary>
        /// <param name="clazz">type of the dependency</param>
        /// <param name="instanceSupplier">the instance of the supplier for the dependency</param>
        /// <returns>
        /// this
        /// <see cref="DocumentProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Kernel.Pdf.DocumentProperties RegisterDependency(Type clazz, Func<Object> instanceSupplier
            ) {
            if (clazz == null) {
                throw new ArgumentException(KernelExceptionMessageConstant.TYPE_SHOULD_NOT_BE_NULL);
            }
            if (instanceSupplier == null) {
                throw new ArgumentException(KernelExceptionMessageConstant.INSTANCE_SUPPLIER_SHOULD_NOT_BE_NULL);
            }
            dependencies.Put(clazz, instanceSupplier);
            return this;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Get all registered dependencies classes.</summary>
        /// <returns>the set of registered dependencies classes</returns>
        internal virtual ICollection<Type> GetRegisteredDependenciesClasses() {
            return dependencies.Keys;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Get specific registered dependency supplier.</summary>
        /// <param name="clazz">the dependency class to return supplier for</param>
        /// <returns>
        /// registered supplier for the dependency.
        /// May return {code null} if no dependency supplier has been registered for specified class.
        /// </returns>
        internal virtual Func<Object> GetDependencySupplier(Type clazz) {
            return dependencies.Get(clazz);
        }
//\endcond
    }
}
