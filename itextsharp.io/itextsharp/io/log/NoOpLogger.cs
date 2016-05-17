using System;
/*
 * $Id$
 *
 * This file is part of the iText (R) project. Copyright (c) 1998-2016 iText Group NV
 * BVBA Authors: Bruno Lowagie, Paulo Soares, et al.
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Affero General Public License version 3 as published by
 * the Free Software Foundation with the addition of the following permission
 * added to Section 15 as permitted in Section 7(a): FOR ANY PART OF THE COVERED
 * WORK IN WHICH THE COPYRIGHT IS OWNED BY ITEXT GROUP, ITEXT GROUP DISCLAIMS THE WARRANTY
 * OF NON INFRINGEMENT OF THIRD PARTY RIGHTS.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
 * details. You should have received a copy of the GNU Affero General Public
 * License along with this program; if not, see http://www.gnu.org/licenses or
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
 * Boston, MA, 02110-1301 USA, or download the license from the following URL:
 * http://itextpdf.com/terms-of-use/
 *
 * The interactive user interfaces in modified source and object code versions
 * of this program must display Appropriate Legal Notices, as required under
 * Section 5 of the GNU Affero General Public License.
 *
 * In accordance with Section 7(b) of the GNU Affero General Public License, a
 * covered work must retain the producer line in every PDF that is created or
 * manipulated using iText.
 *
 * You can be released from the requirements of the license by purchasing a
 * commercial license. Buying such a license is mandatory as soon as you develop
 * commercial activities involving the iText software without disclosing the
 * source code of your own applications. These activities include: offering paid
 * services to customers as an ASP, serving PDFs on the fly in a web
 * application, shipping iText with a closed source product.
 *
 * For more information, please contact iText Software Corp. at this address:
 * sales@itextpdf.com
 */
namespace iTextSharp.IO.Log
{

    /// <summary>
    /// The no-operation logger, it does nothing with the received logging
    /// statements. And returns false by default for <see cref="IsDebugEnabled"/>,
    /// <see cref="IsErrorEnabled"/>, <see cref="IsInfoEnabled"/>, <see cref="IsTraceEnabled"/> and
    /// <see cref="IsWarnEnabled"/>.
    /// </summary>
    /// <author>redlab_b</author>
    public sealed class NoOpLogger : ILogger {

        /* (non-Javadoc)
         * @see com.itextpdf.text.log.ILogger#getLogger(java.lang.Class)
         */
        public ILogger GetLogger(Type name) {
            return this;
        }

        /* (non-Javadoc)
         * @see com.itextpdf.text.log.ILogger#warn(java.lang.String)
         */
        public void Warn(String message) {
        }

        public bool IsWarnEnabled()
        {
            return false;
        }

        /* (non-Javadoc)
         * @see com.itextpdf.text.log.ILogger#trace(java.lang.String)
         */
        public void Trace(String message) {
        }

        public bool IsTraceEnabled()
        {
            return false;
        }

        /* (non-Javadoc)
         * @see com.itextpdf.text.log.ILogger#debug(java.lang.String)
         */
        public void Debug(String message) {
        }

        public bool IsDebugEnabled()
        {
            return false;
        }

        /* (non-Javadoc)
         * @see com.itextpdf.text.log.ILogger#info(java.lang.String)
         */
        public void Info(String message) {
        }

        public bool IsInfoEnabled()
        {
            return false;
        }

        /* (non-Javadoc)
         * @see com.itextpdf.text.log.ILogger#error(java.lang.String, java.lang.Exception)
         */
        public void Error(String message, Exception e) {
        }

        public bool IsErrorEnabled()
        {
            return false;
        }

        /* (non-Javadoc)
         * @see com.itextpdf.text.log.ILogger#error(java.lang.String)
         */
        public void Error(String message) {
        }

        /* (non-Javadoc)
         * @see com.itextpdf.text.log.ILogger#getLogger(java.lang.String)
         */
        public ILogger GetLogger(String name) {
            return this;
        }
    }
}
