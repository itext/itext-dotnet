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
using System.Text;
using System.Xml;
using iText.Commons.Utils;

namespace iText.Signatures.Validation.Report.Xml {
    /// <summary>
    /// This class will convert a
    /// <see cref="PadesValidationReport"/>
    /// to its xml representation.
    /// </summary>
    public class XmlReportGenerator {
        public const String DOC_NS = "http://uri.etsi.org/19102/v1.2.1#";

        public const String DS_NS = "http://www.w3.org/2000/09/xmldsig#";

        public const String XSI_NS = "http://www.w3.org/2001/XMLSchema-instance";

        public const String XS_NS = "http://www.w3.org/2001/XMLSchema";

        private readonly XmlReportOptions options;

        /// <summary>Instantiate a new instance of XmlReportGenerator.</summary>
        /// <param name="options">the conversion options to use</param>
        public XmlReportGenerator(XmlReportOptions options) {
            this.options = options;
        }

        /// <summary>
        /// Generate xlm representation of a
        /// <see cref="PadesValidationReport"/>.
        /// </summary>
        /// <param name="report">the report to transform</param>
        /// <param name="writer">the writer instance to write the resulting xml to</param>
        public virtual void Generate(PadesValidationReport report, TextWriter writer) {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("ValidationReport");
            // Register namespaces in root node.
            root.SetAttribute("xmlns", DOC_NS);
            root.SetAttribute("xmlns:ds", DS_NS);
            root.SetAttribute("xmlns:xsi", XSI_NS);
            root.SetAttribute("xmlns:xs", XS_NS);
            doc.AppendChild(root);
            foreach (SignatureValidationReport signatureValidation in report.GetSignatureValidationReports()) {
                AddSignatureReport(doc, root, signatureValidation);
            }
            XmlNode signatureValidationObjects = root.AppendChild(doc.CreateElement("SignatureValidationObjects"));
            XmlGeneratorCollectableObjectVisitor visitor = new XmlGeneratorCollectableObjectVisitor(doc, signatureValidationObjects
                );
            foreach (CollectableObject @object in report.GetValidationObjects()) {
                @object.Accept(visitor);
            }
            // Convert to pretty printed xml.
            XmlTextWriter xmlWriter = new XmlTextWriter(writer);
            xmlWriter.Formatting = Formatting.Indented;

            doc.Save(xmlWriter);
        }

        private static void AddSignatureReport(XmlDocument doc, XmlElement root, SignatureValidationReport signatureValidation
            ) {
            XmlNode sigValNode = root.AppendChild(doc.CreateElement("SignatureValidationReport"));
            // Create elements with one of the defined namespaces. For the default namespace, there is no namespace prefix.
            // For other namespaces, the correct namespace prefix must be added.
            XmlElement signatureIdentifier = doc.CreateElement("SignatureIdentifier");
            signatureIdentifier.SetAttribute("id", signatureValidation.GetSignatureIdentifier().GetIdentifier().GetId(
                ));
            sigValNode.AppendChild(signatureIdentifier);
            XmlNode digestAlgAndValue = signatureIdentifier.AppendChild(doc.CreateElement("DigestAlgAndValue"
                ));
            // Use setAttributeNS only when the attribute has a different namespace as the element.
            ((XmlElement)digestAlgAndValue.AppendChild(doc.CreateElement("ds", "DigestMethod", DS_NS))).SetAttribute(
                "Algorithm", "", signatureValidation.GetSignatureIdentifier().GetDigestMethodAlgorithm());
            digestAlgAndValue.AppendChild(doc.CreateElement("ds", "DigestValue", DS_NS)).InnerText = signatureValidation
                .GetSignatureIdentifier().GetDigestValue();
            signatureIdentifier.AppendChild(doc.CreateElement("ds", "SignatureValue", DS_NS)).InnerText = signatureValidation
                .GetSignatureIdentifier().GetBase64SignatureValue();
            signatureIdentifier.AppendChild(doc.CreateElement("HashOnly")).InnerText = signatureValidation.GetSignatureIdentifier
                ().IsHashOnly().ToString().ToLower();
            signatureIdentifier.AppendChild(doc.CreateElement("DocHashOnly")).InnerText = signatureValidation
                .GetSignatureIdentifier().IsDocHashOnly().ToString().ToLower();
            XmlNode status = doc.CreateElement("SignatureValidationStatus");
            XmlNode mainIndication = doc.CreateElement("MainIndication");
            mainIndication.InnerText = signatureValidation.GetSignatureValidationStatus().GetMainIndicationAsString();
            status.AppendChild(mainIndication);
            sigValNode.AppendChild(status);
            String subIndication = signatureValidation.GetSignatureValidationStatus().GetSubIndicationAsString();
            if (subIndication != null) {
                XmlNode subIndicationNode = doc.CreateElement("SubIndication");
                subIndicationNode.InnerText = subIndication;
                status.AppendChild(subIndicationNode);
            }
            ICollection<Pair<String, String>> messages = signatureValidation.GetSignatureValidationStatus().GetMessages
                ();
            if (!messages.IsEmpty()) {
                XmlNode associatedValidationReportData = doc.CreateElement("AssociatedValidationReportData");
                status.AppendChild(associatedValidationReportData);
                XmlNode additionalValidationReportData = doc.CreateElement("AdditionalValidationReportData");
                associatedValidationReportData.AppendChild(additionalValidationReportData);
                foreach (Pair<String, String> message in messages) {
                    XmlNode reportData = doc.CreateElement("ReportData");
                    XmlNode type = doc.CreateElement("Type");
                    type.InnerText = message.GetValue();
                    reportData.AppendChild(type);
                    XmlElement value = doc.CreateElement("Value");
                    value.SetAttribute("type", XSI_NS, "xs:string");
                    value.InnerText = message.GetKey();
                    reportData.AppendChild(value);
                    additionalValidationReportData.AppendChild(reportData);
                }
            }
        }
    }
}
