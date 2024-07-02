using System;
using iText.Signatures.Validation.V1.Report;

namespace iText.Signatures.Validation.V1 {
//\cond DO_NOT_DOCUMENT
    internal sealed class SafeCalling {
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
            catch (Exception e) {
                report.AddReportItem(reportItemCreator.Invoke(e));
            }
        }

        /// <summary>Adds a report item to the report when an exception is thrown in the action.</summary>
        /// <param name="action">The action to perform</param>
        /// <param name="defaultValue">The value to return when an exception is thrown</param>
        /// <param name="report">The report to add the ReportItem to</param>
        /// <param name="reportItemCreator">A callback to generate a ReportItem</param>
        /// <returns>The returned value from the action</returns>
        /// <typeparam name="T"/>
        public static T OnExceptionLog<T>(Func<T> action, T defaultValue, ValidationReport report, Func<Exception, 
            ReportItem> reportItemCreator) {
            try {
                return action();
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
            catch (Exception e) {
                report.AddReportItem(reportItemCreator.Invoke(e));
            }
        }

        /// <summary>Adds a report item to the report when an exception is thrown in the action.</summary>
        /// <param name="action">The action to perform</param>
        /// <param name="defaultValue">The value to return when an exception is thrown</param>
        /// <param name="report">The report to add the ReportItem to</param>
        /// <param name="reportItemCreator">A callback to generate a ReportItem</param>
        /// <returns>The returned value from the action</returns>
        /// <typeparam name="T"/>
        public static T OnRuntimeExceptionLog<T>(Func<T> action, T defaultValue, ValidationReport report, Func<Exception
            , ReportItem> reportItemCreator) {
            try {
                return action();
            }
            catch (Exception e) {
                report.AddReportItem(reportItemCreator.Invoke(e));
            }
            return defaultValue;
        }
    }
//\endcond
}
