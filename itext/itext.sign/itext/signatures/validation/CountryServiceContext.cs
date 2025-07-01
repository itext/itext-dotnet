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
using iText.Commons.Bouncycastle.Cert;

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    internal class CountryServiceContext : IServiceContext {
        private IList<IX509Certificate> certificates;

        private String serviceType;

        //It is expected that service statuses are ordered starting from the newest one.
        private readonly IList<ServiceStatusInfo> serviceStatusInfos = new List<ServiceStatusInfo>();

//\cond DO_NOT_DOCUMENT
        internal CountryServiceContext() {
        }
//\endcond

        //empty constructor
        public virtual IList<IX509Certificate> GetCertificates() {
            return new List<IX509Certificate>(certificates);
        }

        public virtual void AddCertificate(IX509Certificate certificate) {
            if (certificates == null) {
                certificates = new List<IX509Certificate>();
            }
            certificates.Add(certificate);
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void SetServiceType(String serviceType) {
            this.serviceType = serviceType;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual String GetServiceType() {
            return serviceType;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddNewServiceStatus(ServiceStatusInfo serviceStatusInfo) {
            serviceStatusInfos.Add(serviceStatusInfo);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual String GetServiceStatusByDate(DateTime time) {
            foreach (ServiceStatusInfo serviceStatusInfo in serviceStatusInfos) {
                if (serviceStatusInfo.GetServiceStatusStartingTime().Before(time)) {
                    return serviceStatusInfo.GetServiceStatus();
                }
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ServiceStatusInfo GetCurrentStatusInfo() {
            return serviceStatusInfos[0];
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetServiceStatusInfosSize() {
            return serviceStatusInfos.Count;
        }
//\endcond
    }
//\endcond
}
