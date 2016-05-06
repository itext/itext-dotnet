//Copyright (c) 2006, Adobe Systems Incorporated
//All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//        3. All advertising materials mentioning features or use of this software
//        must display the following acknowledgement:
//        This product includes software developed by the Adobe Systems Incorporated.
//        4. Neither the name of the Adobe Systems Incorporated nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED BY ADOBE SYSTEMS INCORPORATED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL ADOBE SYSTEMS INCORPORATED BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//        http://www.adobe.com/devnet/xmp/library/eula-xmp-library-java.html
using System;
using System.Globalization;
using com.itextpdf.kernel.xmp;

namespace com.itextpdf.kernel.xmp.impl
{
	/// <summary>The implementation of <code>XMPDateTime</code>.</summary>
	/// <remarks>
	/// The implementation of <code>XMPDateTime</code>. Internally a <code>calendar</code> is used
	/// plus an additional nano seconds field, because <code>Calendar</code> supports only milli
	/// seconds. The <code>nanoSeconds</code> convers only the resolution beyond a milli second.
	/// </remarks>
	/// <since>16.02.2006</since>
	public class XMPDateTimeImpl : XMPDateTime
	{
		private int year = 0;

		private int month = 0;

		private int day = 0;

		private int hour = 0;

		private int minute = 0;

		private int second = 0;

		/// <summary>Use NO time zone as default</summary>
		private TimeZone timeZone = null;

		/// <summary>The nano seconds take micro and nano seconds, while the milli seconds are in the calendar.
		/// 	</summary>
		private int nanoSeconds;

		private bool hasDate = false;

		private bool hasTime = false;

		private bool hasTimeZone = false;

		/// <summary>
		/// Creates an <code>XMPDateTime</code>-instance with the current time in the default time
		/// zone.
		/// </summary>
		public XMPDateTimeImpl()
		{
		}

		/// <summary>Creates an <code>XMPDateTime</code>-instance from a calendar.</summary>
		/// <param name="calendar">a <code>Calendar</code></param>
		public XMPDateTimeImpl(Calendar calendar)
		{
			// EMPTY
			// extract the date and timezone from the calendar provided
			DateTime date = calendar.GetTime();
			TimeZone zone = calendar.GetTimeZone();
			// put that date into a calendar the pretty much represents ISO8601
			// I use US because it is close to the "locale" for the ISO8601 spec
			GregorianCalendar intCalendar = (GregorianCalendar)Calendar.GetInstance(Locale.US
				);
			intCalendar.SetGregorianChange(new DateTime(long.MinValue));
			intCalendar.SetTimeZone(zone);
			intCalendar.SetTime(date);
			this.year = intCalendar.Get(Calendar.YEAR);
			this.month = intCalendar.Get(Calendar.MONTH) + 1;
			// cal is from 0..12
			this.day = intCalendar.Get(Calendar.DAY_OF_MONTH);
			this.hour = intCalendar.Get(Calendar.HOUR_OF_DAY);
			this.minute = intCalendar.Get(Calendar.MINUTE);
			this.second = intCalendar.Get(Calendar.SECOND);
			this.nanoSeconds = intCalendar.Get(Calendar.MILLISECOND) * 1000000;
			this.timeZone = intCalendar.GetTimeZone();
			// object contains all date components
			hasDate = hasTime = hasTimeZone = true;
		}

		/// <summary>
		/// Creates an <code>XMPDateTime</code>-instance from
		/// a <code>Date</code> and a <code>TimeZone</code>.
		/// </summary>
		/// <param name="date">a date describing an absolute point in time</param>
		/// <param name="timeZone">a TimeZone how to interpret the date</param>
		public XMPDateTimeImpl(DateTime date, TimeZone timeZone)
		{
			GregorianCalendar calendar = new GregorianCalendar(timeZone);
			calendar.SetTime(date);
			this.year = calendar.Get(Calendar.YEAR);
			this.month = calendar.Get(Calendar.MONTH) + 1;
			// cal is from 0..12
			this.day = calendar.Get(Calendar.DAY_OF_MONTH);
			this.hour = calendar.Get(Calendar.HOUR_OF_DAY);
			this.minute = calendar.Get(Calendar.MINUTE);
			this.second = calendar.Get(Calendar.SECOND);
			this.nanoSeconds = calendar.Get(Calendar.MILLISECOND) * 1000000;
			this.timeZone = timeZone;
			// object contains all date components
			hasDate = hasTime = hasTimeZone = true;
		}

		/// <summary>Creates an <code>XMPDateTime</code>-instance from an ISO 8601 string.</summary>
		/// <param name="strValue">an ISO 8601 string</param>
		/// <exception cref="com.itextpdf.kernel.xmp.XMPException">If the string is a non-conform ISO 8601 string, an exception is thrown
		/// 	</exception>
		public XMPDateTimeImpl(String strValue)
		{
			ISO8601Converter.Parse(strValue, this);
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.GetYear()"/>
		public virtual int GetYear()
		{
			return year;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.SetYear(int)"/>
		public virtual void SetYear(int year)
		{
			this.year = Math.Min(Math.Abs(year), 9999);
			this.hasDate = true;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.GetMonth()"/>
		public virtual int GetMonth()
		{
			return month;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.SetMonth(int)"/>
		public virtual void SetMonth(int month)
		{
			if (month < 1)
			{
				this.month = 1;
			}
			else
			{
				if (month > 12)
				{
					this.month = 12;
				}
				else
				{
					this.month = month;
				}
			}
			this.hasDate = true;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.GetDay()"/>
		public virtual int GetDay()
		{
			return day;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.SetDay(int)"/>
		public virtual void SetDay(int day)
		{
			if (day < 1)
			{
				this.day = 1;
			}
			else
			{
				if (day > 31)
				{
					this.day = 31;
				}
				else
				{
					this.day = day;
				}
			}
			this.hasDate = true;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.GetHour()"/>
		public virtual int GetHour()
		{
			return hour;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.SetHour(int)"/>
		public virtual void SetHour(int hour)
		{
			this.hour = Math.Min(Math.Abs(hour), 23);
			this.hasTime = true;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.GetMinute()"/>
		public virtual int GetMinute()
		{
			return minute;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.SetMinute(int)"/>
		public virtual void SetMinute(int minute)
		{
			this.minute = Math.Min(Math.Abs(minute), 59);
			this.hasTime = true;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.GetSecond()"/>
		public virtual int GetSecond()
		{
			return second;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.SetSecond(int)"/>
		public virtual void SetSecond(int second)
		{
			this.second = Math.Min(Math.Abs(second), 59);
			this.hasTime = true;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.GetNanoSecond()"/>
		public virtual int GetNanoSecond()
		{
			return nanoSeconds;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.SetNanoSecond(int)"/>
		public virtual void SetNanoSecond(int nanoSecond)
		{
			this.nanoSeconds = nanoSecond;
			this.hasTime = true;
		}

		/// <seealso cref="java.lang.Comparable{T}.CompareTo(System.Object)"/>
		public virtual int CompareTo(Object dt)
		{
			long d = GetCalendar().GetTimeInMillis() - ((XMPDateTime)dt).GetCalendar().GetTimeInMillis
				();
			if (d != 0)
			{
				return (int)Math.Signum(d);
			}
			else
			{
				// if millis are equal, compare nanoseconds
				d = nanoSeconds - ((XMPDateTime)dt).GetNanoSecond();
				return (int)Math.Signum(d);
			}
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.GetTimeZone()"/>
		public virtual TimeZone GetTimeZone()
		{
			return timeZone;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.SetTimeZone(java.util.TimeZone)
		/// 	"/>
		public virtual void SetTimeZone(TimeZone timeZone)
		{
			this.timeZone = timeZone;
			this.hasTime = true;
			this.hasTimeZone = true;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.HasDate()"/>
		public virtual bool HasDate()
		{
			return this.hasDate;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.HasTime()"/>
		public virtual bool HasTime()
		{
			return this.hasTime;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.HasTimeZone()"/>
		public virtual bool HasTimeZone()
		{
			return this.hasTimeZone;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.GetCalendar()"/>
		public virtual Calendar GetCalendar()
		{
			GregorianCalendar calendar = (GregorianCalendar)Calendar.GetInstance(Locale.US);
			calendar.SetGregorianChange(new DateTime(long.MinValue));
			if (hasTimeZone)
			{
				calendar.SetTimeZone(timeZone);
			}
			calendar.Set(Calendar.YEAR, year);
			calendar.Set(Calendar.MONTH, month - 1);
			calendar.Set(Calendar.DAY_OF_MONTH, day);
			calendar.Set(Calendar.HOUR_OF_DAY, hour);
			calendar.Set(Calendar.MINUTE, minute);
			calendar.Set(Calendar.SECOND, second);
			calendar.Set(Calendar.MILLISECOND, nanoSeconds / 1000000);
			return calendar;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPDateTime.GetISO8601String()"/>
		public virtual String GetISO8601String()
		{
			return ISO8601Converter.Render(this);
		}

		/// <returns>Returns the ISO string representation.</returns>
		public override String ToString()
		{
			return GetISO8601String();
		}
	}
}
