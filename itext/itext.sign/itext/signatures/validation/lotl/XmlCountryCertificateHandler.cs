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
using iText.Signatures.Validation.Lotl.Criteria;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal class XmlCountryCertificateHandler : AbstractXmlCertificateHandler {
        private static readonly ICollection<String> INFORMATION_TAGS = new HashSet<String>();

        private readonly ICollection<String> serviceTypes;

        private StringBuilder information;

        private CountryServiceContext currentServiceContext = null;

        private ServiceChronologicalInfo currentServiceChronologicalInfo = null;

        private AdditionalServiceInformationExtension currentServiceExtension = null;

        private QualifierExtension currentQualifierExtension = null;

        private readonly LinkedList<CriteriaList> criteriaListQueue = new LinkedList<CriteriaList>();

        private PolicySetCriteria currentPolicySetCriteria = null;

        private CertSubjectDNAttributeCriteria currentCertSubjectDNAttributeCriteria = null;

        private ExtendedKeyUsageCriteria currentExtendedKeyUsageCriteria = null;

        private KeyUsageCriteria currentKeyUsageCriteria = null;

        private String currentKeyUsageBitName = null;

        static XmlCountryCertificateHandler() {
            INFORMATION_TAGS.Add(XmlTagConstants.SERVICE_TYPE);
            INFORMATION_TAGS.Add(XmlTagConstants.SERVICE_STATUS);
            INFORMATION_TAGS.Add(XmlTagConstants.X509CERTIFICATE);
            INFORMATION_TAGS.Add(XmlTagConstants.SERVICE_STATUS_STARTING_TIME);
            INFORMATION_TAGS.Add(XmlTagConstants.URI);
            INFORMATION_TAGS.Add(XmlTagConstants.IDENTIFIER);
            INFORMATION_TAGS.Add(XmlTagConstants.KEY_PURPOSE_ID);
            INFORMATION_TAGS.Add(XmlTagConstants.KEY_USAGE_BIT);
        }

//\cond DO_NOT_DOCUMENT
        internal XmlCountryCertificateHandler(ICollection<String> serviceTypes) {
            this.serviceTypes = new HashSet<String>(serviceTypes);
        }
//\endcond

        /// <summary><inheritDoc/></summary>
        public override void StartElement(String uri, String localName, String qName, Dictionary<String, String> attributes
            ) {
            if (INFORMATION_TAGS.Contains(localName)) {
                information = new StringBuilder();
            }
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
                        currentServiceExtension = new AdditionalServiceInformationExtension();
                    }
                    else {
                        if (XmlTagConstants.QUALIFICATION_ELEMENT.Equals(localName)) {
                            currentQualifierExtension = new QualifierExtension();
                        }
                        else {
                            if (XmlTagConstants.QUALIFIER.Equals(localName)) {
                                currentQualifierExtension.AddQualifier(attributes.Get(XmlTagConstants.URI_ATTRIBUTE));
                            }
                            else {
                                if (XmlTagConstants.CRITERIA_LIST.Equals(localName)) {
                                    CriteriaList criteriaList = new CriteriaList(attributes.Get(XmlTagConstants.ASSERT));
                                    if (criteriaListQueue.IsEmpty()) {
                                        currentQualifierExtension.SetCriteriaList(criteriaList);
                                    }
                                    else {
                                        criteriaListQueue.Last.Value.AddCriteria(criteriaList);
                                    }
                                    criteriaListQueue.AddLast(criteriaList);
                                }
                                else {
                                    if (XmlTagConstants.POLICY_SET.Equals(localName)) {
                                        currentPolicySetCriteria = new PolicySetCriteria();
                                    }
                                    else {
                                        if (XmlTagConstants.CERT_SUBJECT_DN_ATTRIBUTE.Equals(localName)) {
                                            currentCertSubjectDNAttributeCriteria = new CertSubjectDNAttributeCriteria();
                                        }
                                        else {
                                            if (XmlTagConstants.EXTENDED_KEY_USAGE.Equals(localName)) {
                                                currentExtendedKeyUsageCriteria = new ExtendedKeyUsageCriteria();
                                            }
                                            else {
                                                if (XmlTagConstants.KEY_USAGE.Equals(localName)) {
                                                    currentKeyUsageCriteria = new KeyUsageCriteria();
                                                }
                                                else {
                                                    if (XmlTagConstants.KEY_USAGE_BIT.Equals(localName)) {
                                                        currentKeyUsageBitName = attributes.Get(XmlTagConstants.NAME);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
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
                    if (currentServiceExtension != null) {
                        currentServiceExtension.SetUri(information.ToString());
                    }
                    break;
                }

                case XmlTagConstants.ADDITIONAL_INFORMATION_EXTENSION: {
                    if (currentServiceChronologicalInfo != null) {
                        currentServiceChronologicalInfo.AddServiceExtension(currentServiceExtension);
                    }
                    currentServiceExtension = null;
                    break;
                }

                case XmlTagConstants.QUALIFICATION_ELEMENT: {
                    if (currentServiceChronologicalInfo != null) {
                        currentServiceChronologicalInfo.AddQualifierExtension(currentQualifierExtension);
                    }
                    currentQualifierExtension = null;
                    break;
                }

                case XmlTagConstants.CRITERIA_LIST: {
                    criteriaListQueue.RemoveLast();
                    break;
                }

                case XmlTagConstants.POLICY_SET: {
                    if (criteriaListQueue.Last.Value != null) {
                        criteriaListQueue.Last.Value.AddCriteria(currentPolicySetCriteria);
                    }
                    currentPolicySetCriteria = null;
                    break;
                }

                case XmlTagConstants.CERT_SUBJECT_DN_ATTRIBUTE: {
                    if (criteriaListQueue.Last.Value != null) {
                        criteriaListQueue.Last.Value.AddCriteria(currentCertSubjectDNAttributeCriteria);
                    }
                    currentCertSubjectDNAttributeCriteria = null;
                    break;
                }

                case XmlTagConstants.EXTENDED_KEY_USAGE: {
                    if (criteriaListQueue.Last.Value != null) {
                        criteriaListQueue.Last.Value.AddCriteria(currentExtendedKeyUsageCriteria);
                    }
                    currentExtendedKeyUsageCriteria = null;
                    break;
                }

                case XmlTagConstants.KEY_USAGE: {
                    if (criteriaListQueue.Last.Value != null) {
                        criteriaListQueue.Last.Value.AddCriteria(currentKeyUsageCriteria);
                    }
                    currentKeyUsageCriteria = null;
                    break;
                }

                case XmlTagConstants.IDENTIFIER: {
                    if (currentPolicySetCriteria != null) {
                        currentPolicySetCriteria.AddRequiredPolicyId(information.ToString());
                    }
                    else {
                        if (currentCertSubjectDNAttributeCriteria != null) {
                            currentCertSubjectDNAttributeCriteria.AddRequiredAttributeId(information.ToString());
                        }
                    }
                    break;
                }

                case XmlTagConstants.KEY_PURPOSE_ID: {
                    if (currentExtendedKeyUsageCriteria != null) {
                        currentExtendedKeyUsageCriteria.AddRequiredExtendedKeyUsage(information.ToString());
                    }
                    break;
                }

                case XmlTagConstants.KEY_USAGE_BIT: {
                    if (currentKeyUsageCriteria != null) {
                        currentKeyUsageCriteria.AddKeyUsageBit(currentKeyUsageBitName, information.ToString());
                    }
                    currentKeyUsageBitName = null;
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
