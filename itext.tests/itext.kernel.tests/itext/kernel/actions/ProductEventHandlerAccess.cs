/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using System.Collections.Generic;
using iText.Events.Sequence;
using iText.Kernel.Actions.Processors;

namespace iText.Kernel.Actions {
    /// <summary>
    /// This class is used for testing purposes to have an access to
    /// <see cref="ProductEventHandler"/>.
    /// </summary>
    /// <remarks>
    /// This class is used for testing purposes to have an access to
    /// <see cref="ProductEventHandler"/>
    /// . Note
    /// that work with it may access further tests because the state of ProductEventHandler is shared
    /// across application. It is strongly recommended to call
    /// <see cref="Close()"/>
    /// method to return ProductEventHandler to initial state.
    /// </remarks>
    public class ProductEventHandlerAccess : IDisposable {
        private ICollection<String> registeredProducts = new HashSet<String>();

        public virtual ITextProductEventProcessor AddProcessor(ITextProductEventProcessor processor) {
            registeredProducts.Add(processor.GetProductName());
            return ProductEventHandler.INSTANCE.AddProcessor(processor);
        }

        public virtual ITextProductEventProcessor RemoveProcessor(String productName) {
            return ProductEventHandler.INSTANCE.RemoveProcessor(productName);
        }

        public virtual IDictionary<String, ITextProductEventProcessor> GetProcessors() {
            return ProductEventHandler.INSTANCE.GetProcessors();
        }

        public virtual IList<AbstractProductProcessITextEvent> GetEvents(SequenceId id) {
            return ProductEventHandler.INSTANCE.GetEvents(id);
        }

        public virtual void AddEvent(SequenceId id, AbstractProductProcessITextEvent @event) {
            ProductEventHandler.INSTANCE.AddEvent(id, @event);
        }

        public virtual void Close() {
            foreach (String product in registeredProducts) {
                RemoveProcessor(product);
            }
        }

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
