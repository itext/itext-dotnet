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
using iText.Kernel.Actions.Events;
using iText.Kernel.Actions.Processors;
using iText.Kernel.Actions.Sequence;

namespace iText.Kernel.Actions {
    public class ProductEventHandlerAccess : IDisposable {
        private ICollection<String> registeredProducts = new HashSet<String>();

        public virtual ITextProductEventProcessor AddProcessor(String productName, ITextProductEventProcessor processor
            ) {
            registeredProducts.Add(productName);
            return ProductEventHandler.INSTANCE.AddProcessor(productName, processor);
        }

        public virtual ITextProductEventProcessor RemoveProcessor(String productName) {
            return ProductEventHandler.INSTANCE.RemoveProcessor(productName);
        }

        public virtual ITextProductEventProcessor GetProcessor(String productName) {
            return ProductEventHandler.INSTANCE.GetProcessor(productName);
        }

        public virtual IList<AbstractITextProductEvent> GetEvents(SequenceId id) {
            return ProductEventHandler.INSTANCE.GetEvents(id);
        }

        public virtual void AddEvent(SequenceId id, AbstractITextProductEvent @event) {
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
