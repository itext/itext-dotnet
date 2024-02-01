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
