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
using iText.Commons.Utils;

namespace iText.Signatures.Validation.Lotl {
//\cond DO_NOT_DOCUMENT
    internal class CountryServiceContext : IServiceContext {
        private readonly IList<IX509Certificate> certificates = new List<IX509Certificate>();

        private String serviceType;

        //It is expected that service statuses are ordered starting from the newest one.
        private readonly IList<ServiceChronologicalInfo> serviceChronologicalInfos = new List<ServiceChronologicalInfo
            >();

//\cond DO_NOT_DOCUMENT
        internal CountryServiceContext() {
        }
//\endcond

        // Empty constructor
        /// <summary><inheritDoc/></summary>
        public virtual IList<IX509Certificate> GetCertificates() {
            return new List<IX509Certificate>(certificates);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void AddCertificate(IX509Certificate certificate) {
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
        internal virtual void AddServiceChronologicalInfo(ServiceChronologicalInfo serviceChronologicalInfo) {
            serviceChronologicalInfos.Add(serviceChronologicalInfo);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ServiceChronologicalInfo GetServiceChronologicalInfoByDate(long milliseconds) {
            return GetServiceChronologicalInfoByDate(DateTimeUtil.GetTimeFromMillis(milliseconds));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ServiceChronologicalInfo GetServiceChronologicalInfoByDate(DateTime time) {
            foreach (ServiceChronologicalInfo serviceChronologicalInfo in serviceChronologicalInfos) {
                if (serviceChronologicalInfo.GetServiceStatusStartingTime().Before(time)) {
                    return serviceChronologicalInfo;
                }
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual ServiceChronologicalInfo GetCurrentChronologicalInfo() {
            return serviceChronologicalInfos[0];
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetServiceChronologicalInfosSize() {
            return serviceChronologicalInfos.Count;
        }
//\endcond
    }
//\endcond
}
