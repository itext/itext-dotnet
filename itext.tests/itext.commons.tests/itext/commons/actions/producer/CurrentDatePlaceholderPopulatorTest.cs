/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Exceptions;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Commons.Actions.Producer {
    [NUnit.Framework.Category("UnitTest")]
    public class CurrentDatePlaceholderPopulatorTest : ExtendedITextTest {
        private readonly CurrentDatePlaceholderPopulator populator = new CurrentDatePlaceholderPopulator();

        [NUnit.Framework.Test]
        public virtual void NullTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => populator.Populate(null
                , null));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.INVALID_USAGE_FORMAT_REQUIRED
                , "currentDate"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextTest() {
            String result = populator.Populate(null, "'plain text'");
            NUnit.Framework.Assert.AreEqual("plain text", result);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextWithIgnoredBackSlashesTest() {
            String result = populator.Populate(null, "'\\p\\l\\a\\i\\n \\t\\e\\x\\t'");
            NUnit.Framework.Assert.AreEqual("plain text", result);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextWithEscapedBackSlashesTest() {
            String result = populator.Populate(null, "'plain\\\\text'");
            NUnit.Framework.Assert.AreEqual("plain\\text", result);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextWithEscapedApostrophesTest() {
            String result = populator.Populate(null, "'plain\\'text'");
            NUnit.Framework.Assert.AreEqual("plain'text", result);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextSeveralQuotedStringsTest() {
            String result = populator.Populate(null, "'plain'' ''text'");
            NUnit.Framework.Assert.AreEqual("plain text", result);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextWithUnquotedCharactersTest() {
            String result = populator.Populate(null, "'plain text'$$$");
            NUnit.Framework.Assert.AreEqual("plain text$$$", result);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextEndlessQuotationErrorTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => populator.Populate(null
                , "'plain text"));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.PATTERN_CONTAINS_OPEN_QUOTATION, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextMultipleQuotationsEndlessQuotationErrorTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => populator.Populate(null
                , "'plain'' ''text"));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.PATTERN_CONTAINS_OPEN_QUOTATION, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void PlainTextEscapedApostropheEndlessQuotationErrorTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => populator.Populate(null
                , "'plain text\\'"));
            NUnit.Framework.Assert.AreEqual(CommonsExceptionMessageConstant.PATTERN_CONTAINS_OPEN_QUOTATION, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void ValidComponentsTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => populator.Populate(null, "dd MM MMM MMMM yy yyyy HH mm ss"));
        }

        [NUnit.Framework.Test]
        public virtual void ValidComponentsComparisonTest() {
            // the test may potentially fail if you started it at HH:59:59 so that expected result will
            // be generated at the beginning of the next hour.
            DateTime date = DateTimeUtil.GetCurrentUtcTime();
            String result = populator.Populate(null, "dd MM yy yyyy HH");
            String expectedResult = DateTimeUtil.Format(date, "dd MM yy yyyy HH");
            NUnit.Framework.Assert.AreEqual(expectedResult, result);
        }

        [NUnit.Framework.Test]
        public virtual void UnexpectedLetterComponentTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => populator.Populate(null
                , "dd MM tyy yyyy HH"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.PATTERN_CONTAINS_UNEXPECTED_COMPONENT
                , "t"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void UnexpectedLongComponentTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => populator.Populate(null
                , "dd MMMMM yy yyyy HH"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.PATTERN_CONTAINS_UNEXPECTED_COMPONENT
                , "MMMMM"), exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void UnexpectedShortComponentTest() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => populator.Populate(null
                , "dd MM y yyyy HH"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(CommonsExceptionMessageConstant.PATTERN_CONTAINS_UNEXPECTED_COMPONENT
                , "y"), exception.Message);
        }
    }
}
