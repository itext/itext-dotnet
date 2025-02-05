/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Actions.Processors;
using iText.Commons.Actions.Sequence;

namespace iText.Commons.Actions {
    /// <summary>Abstract class which represents system configuration events.</summary>
    /// <remarks>Abstract class which represents system configuration events. Only for internal usage.</remarks>
    public abstract class AbstractITextConfigurationEvent : AbstractITextEvent {
        /// <summary>
        /// Adds a new
        /// <see cref="iText.Commons.Actions.Processors.ITextProductEventProcessor"/>
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
        /// <remarks>
        /// Gets a processor registered for a product.
        /// <para />
        /// If processor isn't registered and product supports AGPL mode
        /// <see cref="iText.Commons.Actions.Processors.DefaultITextProductEventProcessor"/>
        /// will be obtained otherwise null will be returned.
        /// </remarks>
        /// <param name="productName">is a product for which processor is obtained</param>
        /// <returns>processor for the product</returns>
        protected internal virtual ITextProductEventProcessor GetActiveProcessor(String productName) {
            return ProductEventHandler.INSTANCE.GetActiveProcessor(productName);
        }

        /// <summary>Gets an unmodifiable map of registered processors.</summary>
        /// <returns>all processors</returns>
        protected internal virtual IDictionary<String, ITextProductEventProcessor> GetProcessors() {
            return ProductEventHandler.INSTANCE.GetProcessors();
        }

        /// <summary>Gets events registered for provided identifier.</summary>
        /// <param name="id">is the identifier</param>
        /// <returns>the list of event for identifier</returns>
        protected internal virtual IList<AbstractProductProcessITextEvent> GetEvents(SequenceId id) {
            return ProductEventHandler.INSTANCE.GetEvents(id);
        }

        /// <summary>Registers a new event for provided identifier.</summary>
        /// <param name="id">is the identifier</param>
        /// <param name="event">is the event to register</param>
        protected internal virtual void AddEvent(SequenceId id, AbstractProductProcessITextEvent @event) {
            ProductEventHandler.INSTANCE.AddEvent(id, @event);
        }

        /// <summary>Registers internal namespace.</summary>
        /// <param name="namespace">is the namespace to register</param>
        protected internal virtual void RegisterInternalNamespace(String @namespace) {
            AbstractITextEvent.RegisterNamespace(@namespace);
        }

        /// <summary>Method defines the logic of action processing.</summary>
        protected internal abstract void DoAction();
    }
}
