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
using iText.Signatures.Validation.Report;

namespace iText.Signatures.Validation {
    /// <summary>Utility class to handle exceptions and generate validation report items instead.</summary>
    public sealed class SafeCalling {
        private SafeCalling() {
        }

        /// <summary>Adds a report item to the report when an exception is thrown in the action.</summary>
        /// <param name="action">The action to perform</param>
        /// <param name="report">The report to add the ReportItem to</param>
        /// <param name="reportItemCreator">A callback to generate a ReportItem</param>
        public static void OnExceptionLog(Action action, ValidationReport report, Func<Exception, ReportItem> reportItemCreator
            ) {
            try {
                action();
            }
            catch (SafeCallingAvoidantException e) {
                throw;
            }
            catch (Exception e) {
                report.AddReportItem(reportItemCreator.Invoke(e));
            }
        }

        /// <summary>Adds a report item to the report when an exception is thrown in the action.</summary>
        /// <param name="action">The action to perform</param>
        /// <param name="defaultValue">The value to return when an exception is thrown</param>
        /// <param name="report">The report to add the ReportItem to</param>
        /// <param name="reportItemCreator">A callback to generate a ReportItem</param>
        /// <typeparam name="T">type of return value</typeparam>
        /// <returns>The returned value from the action</returns>
        public static T OnExceptionLog<T>(Func<T> action, T defaultValue, ValidationReport report, Func<Exception, 
            ReportItem> reportItemCreator) {
            try {
                return action();
            }
            catch (SafeCallingAvoidantException e) {
                throw;
            }
            catch (Exception e) {
                report.AddReportItem(reportItemCreator.Invoke(e));
            }
            return defaultValue;
        }

        /// <summary>Adds a report item to the report when an exception is thrown in the action.</summary>
        /// <param name="action">The action to perform</param>
        /// <param name="report">The report to add the ReportItem to</param>
        /// <param name="reportItemCreator">A callback to generate a ReportItem</param>
        public static void OnRuntimeExceptionLog(Action action, ValidationReport report, Func<Exception, ReportItem
            > reportItemCreator) {
            try {
                action();
            }
            catch (SafeCallingAvoidantException e) {
                throw;
            }
            catch (Exception e) {
                report.AddReportItem(reportItemCreator.Invoke(e));
            }
        }

        /// <summary>Adds a report item to the report when an exception is thrown in the action.</summary>
        /// <param name="action">The action to perform</param>
        /// <param name="defaultValue">The value to return when an exception is thrown</param>
        /// <param name="report">The report to add the ReportItem to</param>
        /// <param name="reportItemCreator">A callback to generate a ReportItem</param>
        /// <typeparam name="T">type of return value</typeparam>
        /// <returns>The returned value from the action</returns>
        public static T OnRuntimeExceptionLog<T>(Func<T> action, T defaultValue, ValidationReport report, Func<Exception
            , ReportItem> reportItemCreator) {
            try {
                return action();
            }
            catch (SafeCallingAvoidantException e) {
                throw;
            }
            catch (Exception e) {
                report.AddReportItem(reportItemCreator.Invoke(e));
            }
            return defaultValue;
        }
    }
}
