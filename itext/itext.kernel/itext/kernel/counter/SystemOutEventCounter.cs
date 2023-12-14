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
using iText.IO.Util;
using iText.Kernel.Counter.Event;

namespace iText.Kernel.Counter {
    /// <summary>
    /// A
    /// <see cref="EventCounter"/>
    /// implementation that outputs event type to
    /// <see cref="System.Console.Out"/>
    /// </summary>
    public class SystemOutEventCounter : EventCounter {
        /// <summary>
        /// The name of the class for which the ICounter was created
        /// (or iText if no name is available)
        /// </summary>
        protected internal String name;

        public SystemOutEventCounter(String name) {
            this.name = name;
        }

        public SystemOutEventCounter()
            : this("iText") {
        }

        public SystemOutEventCounter(Type cls)
            : this(cls.FullName) {
        }

        protected internal override void OnEvent(IEvent @event, IMetaInfo metaInfo) {
            System.Console.Out.WriteLine(MessageFormatUtil.Format("[{0}] {1} event", name, @event.GetEventType()));
        }
    }
}
