/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using System.Xml;
using iText.Kernel.Exceptions;
using iText.Kernel.Utils;

namespace iText.Forms.Xfdf
{
    internal sealed class XfdfFileUtils
    {
        private XfdfFileUtils()
        {
        }

        /// <summary>Creates a new xml-styled document for writing xfdf info.</summary>
        /// <remarks>Creates a new xml-styled document for writing xfdf info.</remarks>
        internal static XmlDocument CreateNewXfdfDocument()
        {
            return new XmlDocument();
        }

        /// <summary>Creates a new xfdf document based on given input stream.</summary>
        /// <param name="inputStream"> the stream containing xfdf info.</param>
        internal static XmlDocument CreateXfdfDocumentFromStream(Stream inputStream)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(XmlProcessorCreator.CreateSafeXmlReader(inputStream));
                return doc;
            }
            catch (Exception e)
            {
                throw new PdfException(e.Message, e);
            }
        }

        /// <summary>Saves the info from output stream to xml-styled document.</summary>
        /// <param name="document"> the document to save info to.</param>
        /// <param name=" outputStream"> the stream containing xfdf info.</param>
        internal static void SaveXfdfDocumentToFile(XmlDocument document, Stream outputStream)
        {
            document.Save(outputStream);
            outputStream.Dispose();
        }
    }
}
