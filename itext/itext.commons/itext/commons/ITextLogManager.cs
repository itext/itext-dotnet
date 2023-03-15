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
using Microsoft.Extensions.Logging;

namespace iText.Commons {
    /// <summary>
    /// iText static log manager working with the <c>Microsoft.Extensions.Logging</c> framework. Use it to add iText
    /// logs to your application. Call <see cref = "SetLoggerFactory" /> to set up a logger factory that will
    /// receive iText log messages.
    /// </summary>
    public static class ITextLogManager {
        private static ILoggerFactory _loggerFactory;

        static ITextLogManager() {
            _loggerFactory = new LoggerFactory();
        }

        /// <summary>
        /// Sets the implementation of <see cref="Microsoft.Extensions.Logging.ILoggerFactory"/> to be used for creating <see cref="Microsoft.Extensions.Logging.ILogger"/>
        /// objects.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public static void SetLoggerFactory(ILoggerFactory factory) {
            _loggerFactory = factory;
        }

        /// <summary>
        /// Gets an instance of the used logger factory.
        /// </summary>
        /// <returns>The factory.</returns>
        public static ILoggerFactory GetLoggerFactory() {
            return _loggerFactory;
        }

        /// <summary>
        /// Creates a new <see cref="Microsoft.Extensions.Logging.ILogger"/> instance using the full name of the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ILogger GetLogger(Type type) {
            return _loggerFactory.CreateLogger(type);
        }
    }
}
