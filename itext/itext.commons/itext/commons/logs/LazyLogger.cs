/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons;

namespace iText.Commons.Logs {
    /// <summary>Represents logger wrapper with lazy log operations for lazy log message constructions.</summary>
    public sealed class LazyLogger {
        private readonly ILogger logger;

        /// <summary>Creates the logger instance with the provided clazz naming.</summary>
        /// <param name="clazz">- the returned logger will be named after clazz</param>
        public LazyLogger(Type clazz)
            : this(ITextLogManager.GetLogger(clazz)) {
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates the logger instance wrapping the provided logger.</summary>
        /// <param name="logger">- the logger to wrap</param>
        internal LazyLogger(ILogger logger) {
            this.logger = logger;
        }
//\endcond

        /// <summary>Logs on error level if it is enabled.</summary>
        /// <param name="messageSupplier">log message supplier</param>
        public void Error(Func<String> messageSupplier) {
            if (IsErrorEnabled()) {
                logger.LogError(messageSupplier());
            }
        }

        /// <summary>Logs on error level if it is enabled.</summary>
        /// <param name="messageSupplier">log message supplier</param>
        /// <param name="exception">exception to log</param>
        public void Error(Func<String> messageSupplier, Exception exception) {
            if (IsErrorEnabled()) {
                logger.LogError(exception, messageSupplier());
            }
        }

        /// <summary>Checks whether error logs would be logged.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if error logs would be logged.
        /// </returns>
        public bool IsErrorEnabled() {
            return logger.IsEnabled(LogLevel.Error);
        }

        /// <summary>Logs on warning level if it is enabled.</summary>
        /// <param name="messageSupplier">log message supplier</param>
        public void Warn(Func<String> messageSupplier) {
            if (IsWarnEnabled()) {
                logger.LogWarning(messageSupplier());
            }
        }

        /// <summary>Logs on warning level if it is enabled.</summary>
        /// <param name="messageSupplier">log message supplier</param>
        /// <param name="exception">exception to log</param>
        public void Warn(Func<String> messageSupplier, Exception exception) {
            if (IsWarnEnabled()) {
                logger.LogWarning(exception, messageSupplier());
            }
        }

        /// <summary>Checks whether warn logs would be logged.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if warn logs would be logged.
        /// </returns>
        public bool IsWarnEnabled() {
            return logger.IsEnabled(LogLevel.Warning);
        }

        /// <summary>Logs on info level if it is enabled.</summary>
        /// <param name="messageSupplier">log message supplier</param>
        public void Info(Func<String> messageSupplier) {
            if (IsInfoEnabled()) {
                logger.LogInformation(messageSupplier());
            }
        }

        /// <summary>Logs on info level if it is enabled.</summary>
        /// <param name="messageSupplier">log message supplier</param>
        /// <param name="exception">exception to log</param>
        public void Info(Func<String> messageSupplier, Exception exception) {
            if (IsInfoEnabled()) {
                logger.LogInformation(exception, messageSupplier());
            }
        }

        /// <summary>Checks whether info logs would be logged.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if info logs would be logged.
        /// </returns>
        public bool IsInfoEnabled() {
            return logger.IsEnabled(LogLevel.Information);
        }

        /// <summary>Logs on debug level if it is enabled.</summary>
        /// <param name="messageSupplier">log message supplier</param>
        public void Debug(Func<String> messageSupplier) {
            if (IsDebugEnabled()) {
                logger.LogDebug(messageSupplier());
            }
        }

        /// <summary>Logs on debug level if it is enabled.</summary>
        /// <param name="messageSupplier">log message supplier</param>
        /// <param name="exception">exception to log</param>
        public void Debug(Func<String> messageSupplier, Exception exception) {
            if (IsDebugEnabled()) {
                logger.LogDebug(exception, messageSupplier());
            }
        }

        /// <summary>Checks whether debug logs would be logged.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if debug logs would be logged.
        /// </returns>
        public bool IsDebugEnabled() {
            return logger.IsEnabled(LogLevel.Debug);
        }

        /// <summary>Logs on trace level if it is enabled.</summary>
        /// <param name="messageSupplier">log message supplier</param>
        public void Trace(Func<String> messageSupplier) {
            if (IsTraceEnabled()) {
                logger.LogTrace(messageSupplier());
            }
        }

        /// <summary>Logs on trace level if it is enabled.</summary>
        /// <param name="messageSupplier">log message supplier</param>
        /// <param name="exception">exception to log</param>
        public void Trace(Func<String> messageSupplier, Exception exception) {
            if (IsTraceEnabled()) {
                logger.LogTrace(exception, messageSupplier());
            }
        }

        /// <summary>Checks whether trace logs would be logged.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if trace logs would be logged.
        /// </returns>
        public bool IsTraceEnabled() {
            return logger.IsEnabled(LogLevel.Trace);
        }
    }
}
