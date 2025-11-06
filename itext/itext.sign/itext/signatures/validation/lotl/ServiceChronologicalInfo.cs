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
using System.Globalization;
using System.Linq;

namespace iText.Signatures.Validation.Lotl {
    /// <summary>Class representing ServiceHistory entry in a country specific Trusted List.</summary>
    public class ServiceChronologicalInfo {
//\cond DO_NOT_DOCUMENT
        internal const String GRANTED = "http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/granted";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String GRANTED_NATIONALLY = "http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/recognisedatnationallevel";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String ACCREDITED = "http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/accredited";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SET_BY_NATIONAL_LAW = "http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/setbynationallaw";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String UNDER_SUPERVISION = "http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/undersupervision";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const String SUPERVISION_IN_CESSATION = "http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/supervisionincessation";
//\endcond

        private static readonly ICollection<String> VALID_STATUSES = new HashSet<String>();

        private readonly IList<AdditionalServiceInformationExtension> serviceExtensions = new List<AdditionalServiceInformationExtension
            >();

        private readonly IList<QualifierExtension> qualifierExtensions = new List<QualifierExtension>();

        private readonly String statusStartDateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

        private String serviceStatus;

        //Local time is used here because it is required to use UTC in a trusted lists, so no offset shall be presented.
        private DateTime serviceStatusStartingTime;

        static ServiceChronologicalInfo() {
            VALID_STATUSES.Add(GRANTED);
            VALID_STATUSES.Add(GRANTED_NATIONALLY);
            VALID_STATUSES.Add(ACCREDITED);
            VALID_STATUSES.Add(SET_BY_NATIONAL_LAW);
            VALID_STATUSES.Add(UNDER_SUPERVISION);
            VALID_STATUSES.Add(SUPERVISION_IN_CESSATION);
        }

//\cond DO_NOT_DOCUMENT
        internal ServiceChronologicalInfo() {
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal ServiceChronologicalInfo(String serviceStatus, DateTime serviceStatusStartingTime) {
            this.serviceStatus = serviceStatus;
            this.serviceStatusStartingTime = serviceStatusStartingTime;
        }
//\endcond

        /// <summary>Gets service status corresponding to this Service Chronological Info instance.</summary>
        /// <returns>service status</returns>
        public virtual String GetServiceStatus() {
            return serviceStatus;
        }

        /// <summary>Gets service status starting time corresponding to this Service Chronological Info instance.</summary>
        /// <returns>service status starting time</returns>
        public virtual DateTime GetServiceStatusStartingTime() {
            return serviceStatusStartingTime;
        }

        /// <summary>
        /// Gets list of
        /// <see cref="AdditionalServiceInformationExtension"/>
        /// corresponding to this Service Chronological Info.
        /// </summary>
        /// <returns>
        /// list of
        /// <see cref="AdditionalServiceInformationExtension"/>
        /// </returns>
        public virtual IList<AdditionalServiceInformationExtension> GetServiceExtensions() {
            return serviceExtensions;
        }

        /// <summary>
        /// Gets list of
        /// <see cref="QualifierExtension"/>
        /// corresponding to this Service Chronological Info.
        /// </summary>
        /// <returns>
        /// list of
        /// <see cref="QualifierExtension"/>
        /// </returns>
        public virtual IList<QualifierExtension> GetQualifierExtensions() {
            return qualifierExtensions;
        }

//\cond DO_NOT_DOCUMENT
        internal static bool IsStatusValid(String status) {
            return VALID_STATUSES.Contains(status);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetServiceStatus(String serviceStatus) {
            this.serviceStatus = serviceStatus;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetServiceStatusStartingTime(String timeString) {
            DateTime.TryParseExact(timeString, statusStartDateFormat, null,
                            DateTimeStyles.None, out this.serviceStatusStartingTime);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetServiceStatusStartingTime(DateTime serviceStatusStartingTime) {
            this.serviceStatusStartingTime = serviceStatusStartingTime;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddServiceExtension(AdditionalServiceInformationExtension extension) {
            serviceExtensions.Add(extension);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void AddQualifierExtension(QualifierExtension qualifierExtension) {
            qualifierExtensions.Add(qualifierExtension);
        }
//\endcond
    }
}