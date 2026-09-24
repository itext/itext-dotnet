/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Datastructures;

namespace iText.Commons.Logs {
    public class TestLoggerStats {
        private readonly IList<String> traceCalls = new List<String>();

        private readonly IList<Tuple2<String, Exception>> traceWithThrowableCalls = new List<Tuple2<String, Exception
            >>();

        private readonly IList<String> debugCalls = new List<String>();

        private readonly IList<Tuple2<String, Exception>> debugWithThrowableCalls = new List<Tuple2<String, Exception
            >>();

        private readonly IList<String> infoCalls = new List<String>();

        private readonly IList<Tuple2<String, Exception>> infoWithThrowableCalls = new List<Tuple2<String, Exception
            >>();

        private readonly IList<String> warnCalls = new List<String>();

        private readonly IList<Tuple2<String, Exception>> warnWithThrowableCalls = new List<Tuple2<String, Exception
            >>();

        private readonly IList<String> errorCalls = new List<String>();

        private readonly IList<Tuple2<String, Exception>> errorWithThrowableCalls = new List<Tuple2<String, Exception
            >>();

        public TestLoggerStats() {
        }

        // empty constructor
        public virtual void AddTraceCall(String message) {
            traceCalls.Add(message);
        }

        public virtual IList<String> GetTraceCalls() {
            return traceCalls;
        }

        public virtual void AddTraceWithThrowableCall(String message, Exception t) {
            traceWithThrowableCalls.Add(new Tuple2<String, Exception>(message, t));
        }

        public virtual IList<Tuple2<String, Exception>> GetTraceWithThrowableCalls() {
            return traceWithThrowableCalls;
        }

        public virtual void AddDebugCall(String message) {
            debugCalls.Add(message);
        }

        public virtual IList<String> GetDebugCalls() {
            return debugCalls;
        }

        public virtual void AddDebugWithThrowableCall(String message, Exception t) {
            debugWithThrowableCalls.Add(new Tuple2<String, Exception>(message, t));
        }

        public virtual IList<Tuple2<String, Exception>> GetDebugWithThrowableCalls() {
            return debugWithThrowableCalls;
        }

        public virtual void AddInfoCall(String message) {
            infoCalls.Add(message);
        }

        public virtual IList<String> GetInfoCalls() {
            return infoCalls;
        }

        public virtual void AddInfoWithThrowableCall(String message, Exception t) {
            infoWithThrowableCalls.Add(new Tuple2<String, Exception>(message, t));
        }

        public virtual IList<Tuple2<String, Exception>> GetInfoWithThrowableCalls() {
            return infoWithThrowableCalls;
        }

        public virtual void AddWarnCall(String message) {
            warnCalls.Add(message);
        }

        public virtual IList<String> GetWarnCalls() {
            return warnCalls;
        }

        public virtual void AddWarnWithThrowableCall(String message, Exception t) {
            warnWithThrowableCalls.Add(new Tuple2<String, Exception>(message, t));
        }

        public virtual IList<Tuple2<String, Exception>> GetWarnWithThrowableCalls() {
            return warnWithThrowableCalls;
        }

        public virtual void AddErrorCall(String message) {
            errorCalls.Add(message);
        }

        public virtual IList<String> GetErrorCalls() {
            return errorCalls;
        }

        public virtual void AddErrorWithThrowableCall(String message, Exception t) {
            errorWithThrowableCalls.Add(new Tuple2<String, Exception>(message, t));
        }

        public virtual IList<Tuple2<String, Exception>> GetErrorWithThrowableCalls() {
            return errorWithThrowableCalls;
        }

        public virtual int GetTotalInvocationsCount() {
            return traceCalls.Count + traceWithThrowableCalls.Count + debugCalls.Count + debugWithThrowableCalls.Count
                 + infoCalls.Count + infoWithThrowableCalls.Count + warnCalls.Count + warnWithThrowableCalls.Count + errorCalls
                .Count + errorWithThrowableCalls.Count;
        }
    }
}
