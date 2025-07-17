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

namespace iText.Signatures.Validation {

    internal class ServiceStatusInfo {
        private String serviceStatus;

        //Local time is used here because it is required to use UTC in a trusted lists, so no offset shall be presented.
        private DateTime serviceStatusStartingTime;

        private readonly String statusStartDateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

        internal const String GRANTED = "http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/granted";
        internal const String GRANTED_NATIONALLY =
            "http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/recognisedatnationallevel";
        internal const String WITHDRAWN = "http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/withdrawn";
        internal const String ACCREDITED = "http://uri.etsi.org/TrstSvc/TrustedList/Svcstatus/accredited";
        private static readonly HashSet<String> validStatuses = new HashSet<String>();

        static ServiceStatusInfo()
        {
            validStatuses.Add(GRANTED);
            validStatuses.Add(GRANTED_NATIONALLY);
            validStatuses.Add(ACCREDITED);
        }

        internal ServiceStatusInfo() {
        }

        internal ServiceStatusInfo(String serviceStatus, DateTime serviceStatusStartingTime) {
            this.serviceStatus = serviceStatus;
            this.serviceStatusStartingTime = serviceStatusStartingTime;
        }

        internal virtual void SetServiceStatus(String serviceStatus) {
            this.serviceStatus = serviceStatus;
        }

        internal virtual String GetServiceStatus() {
            return serviceStatus;
        }

        internal virtual void SetServiceStatusStartingTime(String timeString) {
            DateTime.TryParseExact(timeString, statusStartDateFormat, null,
                                      DateTimeStyles.None, out this.serviceStatusStartingTime);
        }

        internal virtual void SetServiceStatusStartingTime(DateTime serviceStatusStartingTime) {
            this.serviceStatusStartingTime = serviceStatusStartingTime;
        }

        internal virtual DateTime GetServiceStatusStartingTime() {
            return serviceStatusStartingTime;
        }

        internal static Boolean IsStatusValid(String status)
        {
            return validStatuses.Contains(status);
        }
    }
}
