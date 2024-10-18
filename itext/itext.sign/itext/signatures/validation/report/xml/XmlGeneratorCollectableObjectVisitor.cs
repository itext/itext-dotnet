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
using System.Xml;

namespace iText.Signatures.Validation.Report.Xml {
//\cond DO_NOT_DOCUMENT
    internal class XmlGeneratorCollectableObjectVisitor : CollectableObjectVisitor {
        private readonly XmlDocument doc;

        private readonly XmlNode parent;

        public XmlGeneratorCollectableObjectVisitor(XmlDocument doc, XmlNode parent) {
            this.doc = doc;
            this.parent = parent;
        }

        public virtual void Visit(CertificateWrapper certificateWrapper) {
            XmlNode validationObject = CreateValidationObjectElement(certificateWrapper.GetIdentifier(), "urn:etsi:019102:validationObject:certificate"
                );
            XmlNode representation = doc.CreateElement("ValidationObjectRepresentation");
            XmlNode b64 = doc.CreateElement("base64");
            b64.InnerText = certificateWrapper.GetBase64ASN1Structure();
            representation.AppendChild(b64);
            validationObject.AppendChild(representation);
            parent.AppendChild(validationObject);
        }

        public virtual void Visit(POEValidationReport poeValidationReport) {
        }

        // Will be completed later
        private XmlNode CreateValidationObjectElement(Identifier identifier, String typeName) {
            XmlElement validationObject = doc.CreateElement("ValidationObject");
            validationObject.SetAttribute("id", identifier.GetId());
            XmlNode objectType = doc.CreateElement("ObjectType");
            objectType.InnerText = typeName;
            validationObject.AppendChild(objectType);
            return validationObject;
        }
    }
//\endcond
}
