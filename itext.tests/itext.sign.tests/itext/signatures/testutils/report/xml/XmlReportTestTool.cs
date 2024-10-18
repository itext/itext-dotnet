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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using iText.Signatures.Validation.Report.Xml;

namespace iText.Signatures.Testutils.Report.Xml {
    public class XmlReportTestTool {
        private static readonly String XSDROOT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/report/xml/";

        private readonly XmlDocument xml;

        private readonly String report;
        private XPathNavigator navigator;
        private XmlNamespaceManager manager;

        public XmlReportTestTool(String report) {
            this.report = report;
            xml = new XmlDocument();   
            xml.LoadXml(report);

            navigator = xml.CreateNavigator();
            manager = new XmlNamespaceManager(navigator.NameTable);
            manager.AddNamespace("r", XmlReportGenerator.DOC_NS);
            manager.AddNamespace("ds", XmlReportGenerator.DS_NS);
            manager.AddNamespace("xs", XmlReportGenerator.XS_NS);
            manager.AddNamespace("xsi", XmlReportGenerator.XSI_NS);
        }

        public virtual XmlElement GetDocumentNode() {
            return xml.DocumentElement;
        }

        public virtual int CountElements(String xPathQuery) {
            return xml.SelectNodes(xPathQuery, manager).Count;
        }

        public virtual String GetElementContent(String xPathQuery) {
            return xml.SelectSingleNode(xPathQuery, manager).InnerText;            
        }

        public virtual XmlNodeList ExecuteXpathAsNodeList(String xPathQuery) {
            return xml.SelectNodes(xPathQuery, manager);
        }

        public virtual XmlNode ExecuteXpathAsNode(String xPathQuery) {
            return xml.SelectSingleNode(xPathQuery, manager);            
        }

        public virtual String ExecuteXpathAsString(String xPathQuery) {
            return (String)navigator.Evaluate(xPathQuery, manager);
        }

        public virtual double? ExecuteXpathAsNumber(String xPathQuery) {
            return (double?)navigator.Evaluate(xPathQuery, manager);
        }

        public virtual bool? ExecuteXpathAsBoolean(String xPathQuery) {
            return (bool?)navigator.Evaluate(xPathQuery, manager);
        }

        public virtual String ValidateXMLSchema() {
            List<string> files = new List<string>();
            files.AddAll(new[] {
                XSDROOT + "xml.xsd",
                XSDROOT + "XMLSchema.xsd",
                XSDROOT + "XAdES.xsd",
                XSDROOT + "ts_119612v020201_201601xsd.xsd",
                XSDROOT + "1910202xmlSchema.xsd",
                XSDROOT + "xmldsig-core-schema.xsd"
            });

            var schemas = files.Select(f => XmlSchema.Read(XmlReader.Create(f, 
                new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse }), (s, e) => {
            }));

            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
            foreach (var s in schemas) {
                xmlSchemaSet.Add(s);
            }

            XmlReaderSettings validationSettings = new XmlReaderSettings();
            validationSettings.ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.AllowXmlAttributes;
            validationSettings.ValidationType = ValidationType.Schema;
            validationSettings.Schemas.Add(xmlSchemaSet);
            validationSettings.Schemas.Compile();
            StringBuilder log = new StringBuilder();

            validationSettings.ValidationEventHandler += (s, e) => {
                log.Append("***\n");
                log.Append("\tPosition:").Append(e.Exception.LineNumber).Append(':').Append(e.Exception.LinePosition).Append('\n');
                log.Append("\tSeverity:").Append(e.Severity).Append('\n');
                log.Append("\tMessage :").Append(e.Message).Append('\n');              
            };
            
            XmlReader reader = XmlReader.Create(new StringReader(report), validationSettings);
            while (reader.Read()) {}
            
            String message = log.ToString();
            if (String.IsNullOrEmpty(message)) {
                return null;
            }
            return message;
        }
    }
}
