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

namespace iText.Kernel.Counter {
    /// <summary>
    /// Factory that can be registered in
    /// <see cref="EventCounterHandler"/>
    /// and creates a counter for every reader or writer class.
    /// </summary>
    /// <remarks>
    /// Factory that can be registered in
    /// <see cref="EventCounterHandler"/>
    /// and creates a counter for every reader or writer class.
    /// <para />
    /// You can implement your own counter factory and register it like this:
    /// <c>EventCounterHandler.getInstance().register(new SystemOutEventCounterFactory());</c>
    /// <para />
    /// <see cref="SystemOutEventCounterFactory"/>
    /// is just an example of
    /// <see cref="IEventCounterFactory"/>
    /// implementation.
    /// It creates
    /// <see cref="SystemOutEventCounter"/>
    /// that writes info about files being read and written to the
    /// <see cref="System.Console.Out"/>
    /// <para />
    /// This functionality can be used to create metrics in a SaaS context.
    /// </remarks>
    public interface IEventCounterFactory {
        EventCounter GetCounter(Type cls);
    }
}
