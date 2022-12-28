/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace iText.Commons.Actions {
    /// <summary>Abstract class which defines events only for internal usage.</summary>
    public abstract class AbstractITextEvent : IEvent {
        private const String ONLY_FOR_INTERNAL_USE = "AbstractITextEvent is only for internal usage.";

        private static readonly IDictionary<String, Object> INTERNAL_PACKAGES = new ConcurrentDictionary<String, Object
            >();

        static AbstractITextEvent() {
            RegisterNamespace(NamespaceConstant.ITEXT);
        }

        /// <summary>Creates an instance of abstract iText event.</summary>
        /// <remarks>Creates an instance of abstract iText event. Only for internal usage.</remarks>
        protected internal AbstractITextEvent() {
            bool isUnknown = true;
            foreach (String @namespace in INTERNAL_PACKAGES.Keys) {
                if (this.GetType().FullName.StartsWith(@namespace)) {
                    isUnknown = false;
                    break;
                }
            }
            if (isUnknown) {
                throw new NotSupportedException(ONLY_FOR_INTERNAL_USE);
            }
        }

        internal static void RegisterNamespace(String @namespace) {
            INTERNAL_PACKAGES.Put(@namespace + ".", new Object());
        }
    }
}
