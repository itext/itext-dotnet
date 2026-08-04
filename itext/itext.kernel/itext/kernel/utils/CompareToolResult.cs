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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Kernel.Utils.Objectpathitems;

namespace iText.Kernel.Utils {
    /// <summary>Class containing results of the comparison of two pdf documents.</summary>
    public sealed class CompareToolResult {
        // LinkedHashMap to retain order. HashMap has different order in Java6/7 and Java8
        private readonly IDictionary<ObjectPath, String> differences = new LinkedDictionary<ObjectPath, String>();

        private int messageLimit = 1;

        /// <summary>Creates new empty instance of CompareToolResult with given limit of difference messages.</summary>
        /// <param name="messageLimit">maximum number of difference messages to be handled by this CompareToolResult.</param>
        public CompareToolResult(int messageLimit) {
            this.messageLimit = messageLimit;
        }

        /// <summary>Verifies if documents are considered equal after comparison.</summary>
        /// <returns>true if documents are equal, false otherwise.</returns>
        public bool IsOk() {
            return differences.IsEmpty();
        }

        /// <summary>Returns number of differences between two documents detected during comparison.</summary>
        /// <returns>number of differences.</returns>
        public int GetErrorCount() {
            return differences.Count;
        }

        /// <summary>Converts this CompareToolResult into text form.</summary>
        /// <returns>text report on the differences between two documents.</returns>
        public String GetReport() {
            StringBuilder sb = new StringBuilder();
            bool firstEntry = true;
            foreach (KeyValuePair<ObjectPath, String> entry in differences) {
                if (!firstEntry) {
                    sb.Append("-----------------------------").Append("\n");
                }
                ObjectPath diffPath = entry.Key;
                sb.Append(entry.Value).Append("\n").Append(diffPath.ToString()).Append("\n");
                firstEntry = false;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns map with
        /// <see cref="iText.Kernel.Utils.Objectpathitems.ObjectPath"/>
        /// as keys and difference descriptions as values.
        /// </summary>
        /// <returns>differences map which could be used to find in the document the objects that are different.</returns>
        public IDictionary<ObjectPath, String> GetDifferences() {
            return differences;
        }

        /// <summary>Converts this CompareToolResult into xml form.</summary>
        /// <param name="stream">output stream to which xml report will be written.</param>
        public void WriteReportToXml(Stream stream) {
            XmlDocument xmlReport = XmlUtils.InitNewXmlDocument();
            XmlElement root = xmlReport.CreateElement("report");
            XmlElement errors = xmlReport.CreateElement("errors");
            errors.SetAttribute("count", differences.Count.ToString());
            root.AppendChild(errors);
            foreach (KeyValuePair<ObjectPath, String> entry in differences) {
                XmlNode errorNode = xmlReport.CreateElement("error");
                XmlNode message = xmlReport.CreateElement("message");
                message.AppendChild(xmlReport.CreateTextNode(entry.Value));
                XmlNode path = entry.Key.ToXmlNode(xmlReport);
                errorNode.AppendChild(message);
                errorNode.AppendChild(path);
                errors.AppendChild(errorNode);
            }
            xmlReport.AppendChild(root);
            XmlUtils.WriteXmlDocToStream(xmlReport, stream);
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks whether maximum number of difference messages to be handled by this CompareToolResult is reached.
        ///     </summary>
        /// <returns>true if limit of difference messages is reached, false otherwise.</returns>
        internal bool IsMessageLimitReached() {
            return differences.Count >= messageLimit;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Returns set limit of difference messages.</summary>
        /// <returns>message limit.</returns>
        internal int GetMessageLimit() {
            return messageLimit;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Adds an error message for the
        /// <see cref="iText.Kernel.Utils.Objectpathitems.ObjectPath"/>.
        /// </summary>
        /// <param name="path">
        /// 
        /// <see cref="iText.Kernel.Utils.Objectpathitems.ObjectPath"/>
        /// for the two corresponding objects in the compared documents
        /// </param>
        /// <param name="message">an error message</param>
        internal void AddError(ObjectPath path, String message) {
            if (differences.Count < messageLimit) {
                differences.Put(new ObjectPath(path), message);
            }
        }
//\endcond
    }
}
