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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using iText.Commons.Actions;
using iText.Commons.Actions.Contexts;
using iText.Test;

namespace iText.IO.Font
{
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontProgramMultithreadedTest : ExtendedITextTest
    {
        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSans.ttf";

        private static readonly TrueTypeFont fp = (TrueTypeFont) FontProgramFactory.CreateFont(FONT);

        [NUnit.Framework.Test]
        public virtual void FontSubsetTest()
        {
            Test[] tests = new Test[6];
            Thread[] threads = new Thread[6];
            for (int i = 0; i < tests.Length; i++)
            {
                tests[i] = new Test();
                threads[i] = tests[i].DoWork();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            foreach (Test test in tests)
            {
                NUnit.Framework.Assert.False(test.exceptionCaught, "Exception during font subsetting");
                NUnit.Framework.Assert.AreEqual(2956, test.subsetSize);
            }
        }

        private class Test
        {
            public bool exceptionCaught = false;
            public int subsetSize = 0;

            public Thread DoWork()
            {
                Thread t = new Thread(Do);
                t.Start();

                return t;
            }

            private void Do()
            {
                for (int i = 0; i < 10; i++)
                {
                    byte[] bytes = null;
                    try
                    {
                        bytes = fp.Subset(new HashSet<int>(), true).GetSecond();
                    }
                    catch (Exception e)
                    {
                        exceptionCaught = true;
                    }
                    subsetSize = bytes == null ? 0 : bytes.Length;
                }
            }
        }
    }
}
