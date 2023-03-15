/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Actions.Processors;
using iText.Commons.Actions.Sequence;

namespace iText.Kernel.Actions {
    /// <summary>This class is used for testing purposes to have an access to ProductEventHandler.</summary>
    /// <remarks>
    /// This class is used for testing purposes to have an access to ProductEventHandler. Note
    /// that work with it may access further tests because the state of ProductEventHandler is shared
    /// across application. It is strongly recommended to call
    /// <see cref="Close()"/>
    /// method to return ProductEventHandler to initial state.
    /// </remarks>
    public class ProductEventHandlerAccess : AbstractITextConfigurationEvent, IDisposable {
        private readonly ICollection<String> registeredProducts = new HashSet<String>();

        public virtual ITextProductEventProcessor PublicAddProcessor(ITextProductEventProcessor processor) {
            registeredProducts.Add(processor.GetProductName());
            return base.AddProcessor(processor);
        }

        public virtual ITextProductEventProcessor PublicRemoveProcessor(String productName) {
            return base.RemoveProcessor(productName);
        }

        public virtual IDictionary<String, ITextProductEventProcessor> PublicGetProcessors() {
            return base.GetProcessors();
        }

        public virtual IList<AbstractProductProcessITextEvent> PublicGetEvents(SequenceId id) {
            return base.GetEvents(id);
        }

        public virtual void PublicAddEvent(SequenceId id, AbstractProductProcessITextEvent @event) {
            base.AddEvent(id, @event);
        }

        protected override void DoAction() {
            throw new InvalidOperationException();
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
