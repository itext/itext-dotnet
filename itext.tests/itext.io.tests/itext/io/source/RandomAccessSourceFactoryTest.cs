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
using System.IO;
using iText.IO.Exceptions;
using iText.Test;

namespace iText.IO.Source {
    [NUnit.Framework.Category("UnitTest")]
    public class RandomAccessSourceFactoryTest : ExtendedITextTest {
        private static readonly String SOURCE_FILE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/source/RAF.txt";

        [NUnit.Framework.Test]
        public virtual void ReadRASInputStreamClosedTest() {
            String fileName = SOURCE_FILE;
            using (Stream pdfStream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                IRandomAccessSource randomAccessSource = new RandomAccessSourceFactory().ExtractOrCreateSource(pdfStream);
                RASInputStream rasInputStream = new RASInputStream(randomAccessSource);
                IRandomAccessSource extractedRandomAccessSource = new RandomAccessSourceFactory().ExtractOrCreateSource(rasInputStream
                    );
                extractedRandomAccessSource.Close();
                Exception e = NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => rasInputStream.Read());
                NUnit.Framework.Assert.AreEqual(IoExceptionMessage.ALREADY_CLOSED, e.Message);
                e = NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => randomAccessSource.Get(0));
                NUnit.Framework.Assert.AreEqual(IoExceptionMessage.ALREADY_CLOSED, e.Message);
                e = NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => randomAccessSource.Get(0, new byte
                    [10], 0, 10));
                NUnit.Framework.Assert.AreEqual(IoExceptionMessage.ALREADY_CLOSED, e.Message);
                e = NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => randomAccessSource.Length());
                NUnit.Framework.Assert.AreEqual(IoExceptionMessage.ALREADY_CLOSED, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadRASInputStreamTest() {
            String fileName = SOURCE_FILE;
            using (Stream pdfStream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                IRandomAccessSource randomAccessSource = new RandomAccessSourceFactory().ExtractOrCreateSource(pdfStream);
                RASInputStream rasInputStream = new RASInputStream(randomAccessSource);
                IRandomAccessSource extractedRandomAccessSource = new RandomAccessSourceFactory().ExtractOrCreateSource(rasInputStream
                    );
                NUnit.Framework.Assert.AreEqual(72, rasInputStream.Read());
                NUnit.Framework.Assert.AreEqual(72, extractedRandomAccessSource.Get(0));
                NUnit.Framework.Assert.AreEqual(extractedRandomAccessSource, rasInputStream.GetSource());
            }
        }
    }
}
