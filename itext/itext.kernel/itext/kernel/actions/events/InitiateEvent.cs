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
using iText.Kernel.Counter.Event;

namespace iText.Kernel.Actions.Events {
    /// <summary>The event identifies that product usage was started.</summary>
    public class InitiateEvent : AbstractITextProductEvent {
        private const String INITIATE_TYPE = "initiate-product-session-event";

        private readonly String productName;

        /// <summary>Creates a new instance of the event.</summary>
        /// <param name="productName">is a product name</param>
        /// <param name="metaInfo">is an auxiliary meta data</param>
        public InitiateEvent(String productName, IMetaInfo metaInfo)
            : base(metaInfo) {
            this.productName = productName;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override String GetEventType() {
            return INITIATE_TYPE;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override String GetProductName() {
            return productName;
        }
    }
}
