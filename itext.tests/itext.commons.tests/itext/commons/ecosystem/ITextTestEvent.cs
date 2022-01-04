/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Sequence;

namespace iText.Commons.Ecosystem {
    public class ITextTestEvent : AbstractProductProcessITextEvent {
        private readonly String eventType;

        public ITextTestEvent(SequenceId sequenceId, IMetaInfo metaInfo, String eventType, String productName)
            : base(sequenceId, new ProductData("", productName, "", 2000, 2100), metaInfo, EventConfirmationType.ON_CLOSE
                ) {
            this.eventType = eventType;
        }

        public ITextTestEvent(SequenceId sequenceId, ProductData productData, IMetaInfo metaInfo, String eventType
            , EventConfirmationType confirmationType)
            : base(sequenceId, productData, metaInfo, confirmationType) {
            this.eventType = eventType;
        }

        public ITextTestEvent(SequenceId sequenceId, ProductData productData, IMetaInfo metaInfo, String eventType
            )
            : this(sequenceId, productData, metaInfo, eventType, EventConfirmationType.ON_CLOSE) {
        }

        public override String GetEventType() {
            return eventType;
        }
    }
}
