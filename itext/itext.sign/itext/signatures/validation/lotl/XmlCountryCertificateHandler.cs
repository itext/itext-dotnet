/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Text;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal class XmlCountryCertificateHandler : AbstractXmlCertificateHandler {
        private static readonly IList<String> INFORMATION_TAGS = new List<String>();

        private readonly ICollection<String> serviceTypes;

        private StringBuilder information;

        private CountryServiceContext currentServiceContext = null;

        private ServiceChronologicalInfo currentServiceChronologicalInfo = null;

        private AdditionalServiceInformationExtension currentExtension = null;

        static XmlCountryCertificateHandler() {
            INFORMATION_TAGS.Add(XmlTagConstants.SERVICE_TYPE);
            INFORMATION_TAGS.Add(XmlTagConstants.SERVICE_STATUS);
            INFORMATION_TAGS.Add(XmlTagConstants.X509CERTIFICATE);
            INFORMATION_TAGS.Add(XmlTagConstants.SERVICE_STATUS_STARTING_TIME);
            INFORMATION_TAGS.Add(XmlTagConstants.URI);
        }

//\cond DO_NOT_DOCUMENT
        internal XmlCountryCertificateHandler(ICollection<String> serviceTypes) {
            this.serviceTypes = new HashSet<String>(serviceTypes);
        }
//\endcond

        /// <summary><inheritDoc/></summary>
        public override void StartElement(String uri, String localName, String qName, Dictionary<String, String> attributes
            ) {
            if (XmlTagConstants.TSP_SERVICE.Equals(localName)) {
                StartProvider();
            }
            else {
                if (XmlTagConstants.SERVICE_HISTORY_INSTANCE.Equals(localName) || XmlTagConstants.SERVICE_INFORMATION.Equals
                    (localName)) {
                    currentServiceChronologicalInfo = new ServiceChronologicalInfo();
                }
                else {
                    if (XmlTagConstants.ADDITIONAL_INFORMATION_EXTENSION.Equals(localName)) {
                        currentExtension = new AdditionalServiceInformationExtension();
                    }
                    else {
                        if (INFORMATION_TAGS.Contains(localName)) {
                            information = new StringBuilder();
                        }
                    }
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void EndElement(String uri, String localName, String qName) {
            switch (localName) {
                case XmlTagConstants.TSP_SERVICE: {
                    EndProvider();
                    break;
                }

                case XmlTagConstants.X509CERTIFICATE: {
                    AddCertificateToContext(information.ToString());
                    information = null;
                    break;
                }

                case XmlTagConstants.SERVICE_STATUS: {
                    if (currentServiceChronologicalInfo != null) {
                        currentServiceChronologicalInfo.SetServiceStatus(information.ToString());
                    }
                    information = null;
                    break;
                }

                case XmlTagConstants.SERVICE_TYPE: {
                    if (currentServiceContext != null) {
                        if (serviceTypes.IsEmpty() || serviceTypes.Contains(information.ToString())) {
                            currentServiceContext.SetServiceType(information.ToString());
                        }
                        else {
                            // If this service type is not among those which were requested, we should skip such service.
                            currentServiceContext = null;
                        }
                    }
                    information = null;
                    break;
                }

                case XmlTagConstants.SERVICE_STATUS_STARTING_TIME: {
                    if (currentServiceChronologicalInfo != null) {
                        currentServiceChronologicalInfo.SetServiceStatusStartingTime(information.ToString());
                    }
                    information = null;
                    break;
                }

                case XmlTagConstants.SERVICE_INFORMATION:
                case XmlTagConstants.SERVICE_HISTORY_INSTANCE: {
                    if (currentServiceContext != null) {
                        currentServiceContext.AddServiceChronologicalInfo(currentServiceChronologicalInfo);
                    }
                    currentServiceChronologicalInfo = null;
                    break;
                }

                case XmlTagConstants.URI: {
                    if (currentExtension != null) {
                        currentExtension.SetUri(information.ToString());
                    }
                    break;
                }

                case XmlTagConstants.ADDITIONAL_INFORMATION_EXTENSION: {
                    if (currentServiceChronologicalInfo != null) {
                        currentServiceChronologicalInfo.AddExtension(currentExtension);
                    }
                    currentExtension = null;
                    break;
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void Characters(char[] ch, int start, int length) {
            if (information != null) {
                information.Append(ch, start, length);
            }
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void StartProvider() {
            currentServiceContext = new CountryServiceContext();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddCertificateToContext(String certificateString) {
            if (currentServiceContext == null) {
                return;
            }
            IX509Certificate certificate = CertificateUtil.CreateCertificateFromEncodedData(RemoveWhitespacesAndBreakLines
                (certificateString));
            currentServiceContext.AddCertificate(certificate);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void EndProvider() {
            if (currentServiceContext != null) {
                serviceContextList.Add(currentServiceContext);
                currentServiceContext = null;
            }
        }
//\endcond

        private static String RemoveWhitespacesAndBreakLines(String data) {
            return data.Replace(" ", "").Replace("\n", "");
        }
    }
//\endcond
}
