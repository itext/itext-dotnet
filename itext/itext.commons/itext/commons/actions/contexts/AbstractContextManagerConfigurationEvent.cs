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

namespace iText.Commons.Actions.Contexts {
    /// <summary>Class is recommended for internal usage.</summary>
    /// <remarks>Class is recommended for internal usage. Represents system configuration events.</remarks>
    public abstract class AbstractContextManagerConfigurationEvent : AbstractITextConfigurationEvent {
        /// <summary>Creates an instance of context manager configuration event.</summary>
        protected internal AbstractContextManagerConfigurationEvent()
            : base() {
        }

        /// <summary>Registers generic context for products and namespaces which are associated with them.</summary>
        /// <param name="namespaces">namespaces of the products to be registered</param>
        /// <param name="products">the products to be registered</param>
        protected internal virtual void RegisterGenericContext(ICollection<String> namespaces, ICollection<String>
             products) {
            ContextManager.GetInstance().RegisterGenericContext(namespaces, products);
        }

        /// <summary>Unregisters certain namespaces.</summary>
        /// <param name="namespaces">the namespaces to be unregistered</param>
        protected internal virtual void UnregisterContext(ICollection<String> namespaces) {
            ContextManager.GetInstance().UnregisterContext(namespaces);
        }
    }
}
