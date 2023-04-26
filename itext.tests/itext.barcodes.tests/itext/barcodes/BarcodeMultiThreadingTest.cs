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
using System.Threading;
using iText.Test;
using NUnit.Framework;

namespace iText.Barcodes
{
    public class BarcodeMultiThreadingTest : ExtendedITextTest {
        [Test]
        public void test() {
            Thread[] threads = new Thread[20];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(DoWork);
                threads[i].Start();
            }

            foreach (Thread thread in threads)
                thread.Join();
        }

        private static void DoWork() {
            BarcodeDataMatrix bc = new BarcodeDataMatrix();
            bc.SetOptions(BarcodeDataMatrix.DM_AUTO);
            bc.SetWidth(10);
            bc.SetHeight(10);
            int result = bc.SetCode("AB01");
            Assert.AreEqual(BarcodeDataMatrix.DM_NO_ERROR, result);
        }
    }
}
