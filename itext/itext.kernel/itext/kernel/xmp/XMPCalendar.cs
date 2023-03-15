/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.Kernel.XMP {
	
    public class XMPCalendar {
        private DateTime dateTime;
        private TimeZoneInfo timeZone;

        public XMPCalendar(DateTime dateTime, TimeZoneInfo timeZone) {
            this.dateTime = dateTime;
            this.timeZone = timeZone;
        }

        public XMPCalendar(DateTime dateTime) : this(dateTime, TimeZoneInfo.Local) {
        }

        public XMPCalendar(TimeZoneInfo timeZone)
            : this(DateTime.Now, timeZone) {
        }

        public XMPCalendar() : this(DateTime.Now, TimeZoneInfo.Local) {
        }

        public virtual DateTime GetDateTime() {
            return dateTime;
        }

		public virtual void SetDateTime(DateTime dateTime) {
			this.dateTime = dateTime;
		}

        public virtual void SetTimeZone(TimeZoneInfo timeZone) {
			this.timeZone = timeZone;
		}

		public virtual TimeZoneInfo GetTimeZone() {
            return timeZone;
        }

        public virtual long GetTimeInMillis() {
            return dateTime.Ticks;
        }

		public virtual void SetTimeInMillis(long value) {
			dateTime = new DateTime(value);
		}
    }
}
