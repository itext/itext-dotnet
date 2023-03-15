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
using iText.Test.Attributes;
using NUnit.Framework;

namespace iText.Test {
    /// <summary>
    /// This class is used for testing when logger output should be tested as well.
    /// By default any logger output that is not expected, i.e. marked with <see cref="LogMessageAttribute"/>
    /// will result in crash.
    /// </summary>
    [LogListener]
    public abstract class ExtendedITextTest : ITextTest {
        /// <summary>
        /// This method is called before each test method is executed
        /// </summary>
        [SetUp]
        public virtual void BeforeTestMethodAction() {
        }

        /// <summary>
        /// This method is called after each test method is executed
        /// </summary>
        [TearDown]
        public virtual void AfterTestMethodAction() {
        }
    }
}
