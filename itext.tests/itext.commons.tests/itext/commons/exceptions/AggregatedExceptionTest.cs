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
using iText.Test;

namespace iText.Commons.Exceptions {
    [NUnit.Framework.Category("UnitTest")]
    public class AggregatedExceptionTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AggregatedMessageWithGeneralMessageTest() {
            IList<Exception> exceptions = new List<Exception>();
            exceptions.Add(new Exception("Message 1"));
            exceptions.Add(new Exception("Message 2"));
            exceptions.Add(new AggregatedExceptionTest.CustomException("Message 3"));
            AggregatedException exception = new AggregatedException("General message", exceptions);
            NUnit.Framework.Assert.AreEqual(exceptions, exception.GetAggregatedExceptions());
            NUnit.Framework.Assert.AreEqual("General message:\n" + "0) Message 1\n" + "1) Message 2\n" + "2) Message 3\n"
                , exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void AggregatedMessageWithoutGeneralMessageTest() {
            IList<Exception> exceptions = new List<Exception>();
            exceptions.Add(new Exception("Message 1"));
            exceptions.Add(new Exception("Message 2"));
            exceptions.Add(new AggregatedExceptionTest.CustomException("Message 3"));
            AggregatedException exception = new AggregatedException(exceptions);
            NUnit.Framework.Assert.AreEqual("Aggregated message:\n" + "0) Message 1\n" + "1) Message 2\n" + "2) Message 3\n"
                , exception.Message);
        }

        private sealed class CustomException : Exception {
            public CustomException(String message)
                : base(message) {
            }
        }
    }
}
