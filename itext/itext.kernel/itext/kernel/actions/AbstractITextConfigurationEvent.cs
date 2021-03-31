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
    /// <summary>Class is recommended for internal usage.</summary>
    /// <remarks>Class is recommended for internal usage. Represents system configuration events.</remarks>
    public abstract class AbstractITextConfigurationEvent : ITextEvent {
        private const String INTERNAL_PACKAGE = "iText";

        private const String ONLY_FOR_INTERNAL_USE = "AbstractTextConfigurationEvent is only for internal usage.";

        /// <summary>Creates an instance of configuration event.</summary>
        public AbstractITextConfigurationEvent()
            : base() {
            // TODO: DEVSIX-4958 if needed we can create some wrapper mechanism to allow creation
            // of ITextConfigurationEvent in Pdf2Data
            if (!this.GetType().FullName.StartsWith(INTERNAL_PACKAGE)) {
                throw new NotSupportedException(ONLY_FOR_INTERNAL_USE);
            }
        }

        /// <summary>
        /// Add a new
        /// <see cref="iText.Kernel.Actions.Processors.ITextProductEventProcessor"/>
        /// for a product.
        /// </summary>
        /// <param name="processor">is a new processor</param>
        /// <returns>a replaced processor for the product</returns>
        protected internal virtual ITextProductEventProcessor AddProcessor(ITextProductEventProcessor processor) {
            return ProductEventHandler.INSTANCE.AddProcessor(processor);
        }

        /// <summary>Removes a processor registered for a product.</summary>
        /// <param name="productName">is a product for which processor is removed</param>
        /// <returns>removed processor</returns>
        protected internal virtual ITextProductEventProcessor RemoveProcessor(String productName) {
            return ProductEventHandler.INSTANCE.RemoveProcessor(productName);
        }

        /// <summary>Gets a processor registered for a product.</summary>
        /// <param name="productName">is a product for which processor is obtained</param>
        /// <returns>processor for the product</returns>
        protected internal virtual ITextProductEventProcessor GetProcessor(String productName) {
            return ProductEventHandler.INSTANCE.GetProcessor(productName);
        }

        /// <summary>Gets an unmodifiable map of registered processors.</summary>
        /// <returns>all processors</returns>
        protected internal virtual IDictionary<String, ITextProductEventProcessor> GetProcessors() {
            return ProductEventHandler.INSTANCE.GetProcessors();
        }

        /// <summary>Gets events registered for provided identifier.</summary>
        /// <param name="id">is the identifier</param>
        /// <returns>the list of event for identifier</returns>
        protected internal virtual IList<AbstractITextProductEvent> GetEvents(SequenceId id) {
            return ProductEventHandler.INSTANCE.GetEvents(id);
        }

        /// <summary>Registers a new event for provided identifier.</summary>
        /// <param name="id">is the identifier</param>
        /// <param name="event">is the event to register</param>
        protected internal virtual void AddEvent(SequenceId id, AbstractITextProductEvent @event) {
            ProductEventHandler.INSTANCE.AddEvent(id, @event);
        }

        /// <summary>Method defines the logic of action processing.</summary>
        protected internal abstract void DoAction();

        public abstract String GetEventType();

        public abstract String GetProductName();
    }
}
