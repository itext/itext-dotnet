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

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    internal class XmlCountryCertificateHandler : AbstractXmlCertificateHandler {
        private static readonly IList<String> INFORMATION_TAGS = new List<String>();

        static XmlCountryCertificateHandler() {
            INFORMATION_TAGS.Add(XmlTagConstants.SERVICE_TYPE);
            INFORMATION_TAGS.Add(XmlTagConstants.SERVICE_STATUS);
            INFORMATION_TAGS.Add(XmlTagConstants.X509CERTIFICATE);
            INFORMATION_TAGS.Add(XmlTagConstants.SERVICE_STATUS_STARTING_TIME);
        }

        private readonly IList<IX509Certificate> certificateList = new List<IX509Certificate>();

        private readonly IList<CountryServiceContext> serviceContextList = new List<CountryServiceContext>();

        private StringBuilder information;

        private CountryServiceContext currentServiceContext = null;

        private ServiceStatusInfo currentServiceStatusInfo = null;

//\cond DO_NOT_DOCUMENT
        internal XmlCountryCertificateHandler() {
        }
//\endcond

        //empty constructor
        private static String RemoveWhitespacesAndBreakLines(String data) {
            return data.Replace(" ", "").Replace("\n", "");
        }

        public override void StartElement(String uri, String localName, String qName, Dictionary<String, String> attributes
            ) {
            if (XmlTagConstants.TSP_SERVICE.Equals(localName)) {
                StartProvider();
            }
            else {
                if (XmlTagConstants.SERVICE_HISTORY_INSTANCE.Equals(localName) || XmlTagConstants.SERVICE_INFORMATION.Equals
                    (localName)) {
                    currentServiceStatusInfo = new ServiceStatusInfo();
                }
                else {
                    if (INFORMATION_TAGS.Contains(localName)) {
                        information = new StringBuilder();
                    }
                }
            }
        }

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
                    if (currentServiceContext != null) {
                        currentServiceStatusInfo.SetServiceStatus(information.ToString());
                    }
                    information = null;
                    break;
                }

                case XmlTagConstants.SERVICE_TYPE: {
                    if (currentServiceContext != null) {
                        currentServiceContext.SetServiceType(information.ToString());
                    }
                    information = null;
                    break;
                }

                case XmlTagConstants.SERVICE_STATUS_STARTING_TIME: {
                    if (currentServiceContext != null) {
                        currentServiceStatusInfo.SetServiceStatusStartingTime(information.ToString());
                    }
                    information = null;
                    break;
                }

                case XmlTagConstants.SERVICE_INFORMATION:
                case XmlTagConstants.SERVICE_HISTORY_INSTANCE: {
                    if (currentServiceContext != null) {
                        currentServiceContext.AddNewServiceStatus(currentServiceStatusInfo);
                    }
                    currentServiceStatusInfo = null;
                    break;
                }
            }
        }

        public override void Characters(char[] ch, int start, int length) {
            if (information != null) {
                information.Append(ch, start, length);
            }
        }

//\cond DO_NOT_DOCUMENT
        internal override IServiceContext GetServiceContext(IX509Certificate certificate) {
            foreach (CountryServiceContext context in serviceContextList) {
                if (context.GetCertificates().Contains(certificate)) {
                    return context;
                }
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override IList<IX509Certificate> GetCertificateList() {
            return certificateList;
        }
//\endcond

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
            IX509Certificate certificate = GetCertificateFromEncodedData(RemoveWhitespacesAndBreakLines(certificateString
                ));
            currentServiceContext.AddCertificate(certificate);
            certificateList.Add(certificate);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void EndProvider() {
            serviceContextList.Add(currentServiceContext);
            currentServiceContext = null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override void Clear() {
            certificateList.Clear();
            serviceContextList.Clear();
        }
//\endcond
    }
//\endcond
}
