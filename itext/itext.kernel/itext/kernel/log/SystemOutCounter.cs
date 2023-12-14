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

namespace iText.Kernel.Log {
    /// <summary>
    /// A
    /// <see cref="ICounter"/>
    /// implementation that outputs information about read and written documents to
    /// <see cref="System.Console.Out"/>
    /// </summary>
    [System.ObsoleteAttribute(@"will be removed in the next major release, please use iText.Kernel.Counter.SystemOutEventCounter instead."
        )]
    public class SystemOutCounter : ICounter {
        /// <summary>
        /// The name of the class for which the ICounter was created
        /// (or iText if no name is available)
        /// </summary>
        protected internal String name;

        public SystemOutCounter(String name) {
            this.name = name;
        }

        public SystemOutCounter()
            : this("iText") {
        }

        public SystemOutCounter(Type cls)
            : this(cls.FullName) {
        }

        public virtual void OnDocumentRead(long size) {
            System.Console.Out.WriteLine(MessageFormatUtil.Format("[{0}] {1} bytes read", name, size));
        }

        public virtual void OnDocumentWritten(long size) {
            System.Console.Out.WriteLine(MessageFormatUtil.Format("[{0}] {1} bytes written", name, size));
        }
    }
}
