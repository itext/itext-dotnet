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
using iText.Commons.Actions;
using iText.Commons.Utils;

namespace iText.Commons.Actions.Contexts {
    /// <summary>The class that retrieves context of its invocation.</summary>
    public class ContextManager {
        private static readonly iText.Commons.Actions.Contexts.ContextManager INSTANCE;

        private readonly SortedDictionary<String, IContext> contextMappings = new SortedDictionary<String, IContext
            >(new ContextManager.LengthComparator());

        static ContextManager() {
            iText.Commons.Actions.Contexts.ContextManager local = new iText.Commons.Actions.Contexts.ContextManager();
            local.RegisterGenericContext(NamespaceConstant.ITEXT_CORE_NAMESPACES, JavaCollectionsUtil.Singleton(ProductNameConstant
                .ITEXT_CORE));
            local.RegisterGenericContext(JavaCollectionsUtil.SingletonList(NamespaceConstant.PDF_HTML), JavaCollectionsUtil
                .Singleton(ProductNameConstant.PDF_HTML));
            local.RegisterGenericContext(JavaCollectionsUtil.SingletonList(NamespaceConstant.PDF_SWEEP), JavaCollectionsUtil
                .Singleton(ProductNameConstant.PDF_SWEEP));
            local.RegisterGenericContext(JavaCollectionsUtil.SingletonList(NamespaceConstant.PDF_OCR_TESSERACT4), JavaCollectionsUtil
                .Singleton(ProductNameConstant.PDF_OCR_TESSERACT4));
            INSTANCE = local;
        }

        internal ContextManager() {
        }

        /// <summary>Gets the singleton instance of this class.</summary>
        /// <returns>
        /// the
        /// <see cref="ContextManager"/>
        /// instance
        /// </returns>
        public static iText.Commons.Actions.Contexts.ContextManager GetInstance() {
            return INSTANCE;
        }

        /// <summary>Gets the context associated with the passed class object.</summary>
        /// <remarks>
        /// Gets the context associated with the passed class object.
        /// The context is determined by class namespace.
        /// </remarks>
        /// <param name="clazz">the class for which the context will be determined.</param>
        /// <returns>
        /// the
        /// <see cref="IContext"/>
        /// associated with the class, or
        /// <see langword="null"/>
        /// if the class is unknown.
        /// </returns>
        public virtual IContext GetContext(Type clazz) {
            return clazz == null ? null : GetContext(clazz.FullName);
        }

        /// <summary>Gets the context associated with the passed class object.</summary>
        /// <remarks>
        /// Gets the context associated with the passed class object.
        /// The context is determined by class namespace.
        /// </remarks>
        /// <param name="className">the class name with the namespace for which the context will be determined.</param>
        /// <returns>
        /// the
        /// <see cref="IContext"/>
        /// associated with the class, or
        /// <see langword="null"/>
        /// if the class is unknown.
        /// </returns>
        public virtual IContext GetContext(String className) {
            return GetNamespaceMapping(GetRecognisedNamespace(className));
        }

        internal virtual String GetRecognisedNamespace(String className) {
            if (className != null) {
                String normalizedClassName = Normalize(className);
                // If both "a" and "a.b" namespaces are registered,
                // iText should consider the context of "a.b" for an "a.b" event,
                // that's why the contexts are sorted by the length of the namespace
                foreach (String @namespace in contextMappings.Keys) {
                    if (normalizedClassName.StartsWith(@namespace)) {
                        return @namespace;
                    }
                }
            }
            return null;
        }

        internal virtual void UnregisterContext(ICollection<String> namespaces) {
            foreach (String @namespace in namespaces) {
                contextMappings.JRemove(Normalize(@namespace));
            }
        }

        private IContext GetNamespaceMapping(String @namespace) {
            if (@namespace != null) {
                return contextMappings.Get(@namespace);
            }
            return null;
        }

        internal virtual void RegisterGenericContext(ICollection<String> namespaces, ICollection<String> products) {
            GenericContext context = new GenericContext(products);
            foreach (String @namespace in namespaces) {
                contextMappings.Put(Normalize(@namespace), context);
            }
        }

        private static String Normalize(String @namespace) {
            // Conversion to lowercase is done to be compatible with possible changes in case of packages/namespaces
            return @namespace.ToLowerInvariant();
        }

        private class LengthComparator : IComparer<String> {
            public virtual int Compare(String o1, String o2) {
                int lengthComparison = JavaUtil.IntegerCompare(o2.Length, o1.Length);
                if (0 == lengthComparison) {
                    return string.CompareOrdinal(o1, o2);
                }
                else {
                    return lengthComparison;
                }
            }
        }
    }
}
