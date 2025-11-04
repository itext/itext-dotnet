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
    /// <summary>Class representing TSPService entry in a country specific Trusted List.</summary>
    public class CountryServiceContext : IServiceContext {
        private readonly IList<IX509Certificate> certificates = new List<IX509Certificate>();

        //It is expected that service statuses are ordered starting from the newest one.
        private readonly IList<ServiceChronologicalInfo> serviceChronologicalInfos = new List<ServiceChronologicalInfo
            >();

        private String serviceType;

//\cond DO_NOT_DOCUMENT
        internal CountryServiceContext() {
        }
//\endcond

        // Empty constructor.
        /// <summary><inheritDoc/></summary>
        public virtual IList<IX509Certificate> GetCertificates() {
            return new List<IX509Certificate>(certificates);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void AddCertificate(IX509Certificate certificate) {
            certificates.Add(certificate);
        }

        /// <summary>
        /// Gets list of
        /// <see cref="ServiceChronologicalInfo"/>
        /// objects corresponding to this country service context.
        /// </summary>
        /// <returns>
        /// list of
        /// <see cref="ServiceChronologicalInfo"/>
        /// objects
        /// </returns>
        public virtual IList<ServiceChronologicalInfo> GetServiceChronologicalInfos() {
            return serviceChronologicalInfos;
        }

        /// <summary>
        /// Gets service type
        /// <see cref="System.String"/>
        /// corresponding to this country service context.
        /// </summary>
        /// <returns>
        /// service type
        /// <see cref="System.String"/>
        /// </returns>
        public virtual String GetServiceType() {
            return serviceType;
        }

        /// <summary>
        /// Gets
        /// <see cref="ServiceChronologicalInfo"/>
        /// corresponding to this country service context and DateTime in milliseconds.
        /// </summary>
        /// <param name="milliseconds">DateTime in milliseconds</param>
        /// <returns>
        /// corresponding
        /// <see cref="ServiceChronologicalInfo"/>
        /// instance
        /// </returns>
        public virtual ServiceChronologicalInfo GetServiceChronologicalInfoByDate(long milliseconds) {
            return GetServiceChronologicalInfoByDate(DateTimeUtil.GetTimeFromMillis(milliseconds));
        }

        /// <summary>
        /// Gets
        /// <see cref="ServiceChronologicalInfo"/>
        /// corresponding to this country service context and
        /// <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="time">
        /// 
        /// <see cref="System.DateTime"/>
        /// date time
        /// </param>
        /// <returns>
        /// corresponding
        /// <see cref="ServiceChronologicalInfo"/>
        /// instance
        /// </returns>
        public virtual ServiceChronologicalInfo GetServiceChronologicalInfoByDate(DateTime time) {
            foreach (ServiceChronologicalInfo serviceChronologicalInfo in serviceChronologicalInfos) {
                if (!time.Before(serviceChronologicalInfo.GetServiceStatusStartingTime())) {
                    return serviceChronologicalInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the latest
        /// <see cref="ServiceChronologicalInfo"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the latest
        /// <see cref="ServiceChronologicalInfo"/>
        /// instance
        /// </returns>
        public virtual ServiceChronologicalInfo GetCurrentChronologicalInfo() {
            return serviceChronologicalInfos[0];
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void SetServiceType(String serviceType) {
            this.serviceType = serviceType;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddServiceChronologicalInfo(ServiceChronologicalInfo serviceChronologicalInfo) {
            serviceChronologicalInfos.Add(serviceChronologicalInfo);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetServiceChronologicalInfosSize() {
            return serviceChronologicalInfos.Count;
        }
//\endcond
    }
}
