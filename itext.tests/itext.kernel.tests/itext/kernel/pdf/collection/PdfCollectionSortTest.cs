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
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Collection {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfCollectionSortTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void OneKeyConstructorTest() {
            String key = "testKey";
            PdfCollectionSort sort = new PdfCollectionSort(key);
            NUnit.Framework.Assert.AreEqual(key, sort.GetPdfObject().GetAsName(PdfName.S).GetValue());
        }

        [NUnit.Framework.Test]
        public virtual void MultipleKeysConstructorTest() {
            String[] keys = new String[] { "testKey1", "testKey2", "testKey3" };
            PdfCollectionSort sort = new PdfCollectionSort(keys);
            for (int i = 0; i < keys.Length; i++) {
                NUnit.Framework.Assert.AreEqual(keys[i], sort.GetPdfObject().GetAsArray(PdfName.S).GetAsName(i).GetValue()
                    );
            }
        }

        [NUnit.Framework.Test]
        public virtual void SortOrderForOneKeyTest() {
            String key = "testKey";
            bool testAscending = true;
            PdfCollectionSort sort = new PdfCollectionSort(key);
            NUnit.Framework.Assert.IsNull(sort.GetPdfObject().Get(PdfName.A));
            sort.SetSortOrder(testAscending);
            NUnit.Framework.Assert.IsTrue(sort.GetPdfObject().GetAsBool(PdfName.A));
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectSortOrderForOneKeyTest() {
            String key = "testKey";
            bool[] testAscendings = new bool[] { true, false };
            PdfCollectionSort sort = new PdfCollectionSort(key);
            // this line will throw an exception as number of parameters of setSortOrder()
            // method should be exactly the same as number of keys of PdfCollectionSort
            // here we have one key but two params
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => sort.SetSortOrder(testAscendings));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.YOU_NEED_A_SINGLE_BOOLEAN_FOR_THIS_COLLECTION_SORT_DICTIONARY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SortOrderForMultipleKeysTest() {
            String[] keys = new String[] { "testKey1", "testKey2", "testKey3" };
            bool[] testAscendings = new bool[] { true, false, true };
            PdfCollectionSort sort = new PdfCollectionSort(keys);
            sort.SetSortOrder(testAscendings);
            for (int i = 0; i < testAscendings.Length; i++) {
                NUnit.Framework.Assert.AreEqual(testAscendings[i], sort.GetPdfObject().GetAsArray(PdfName.A).GetAsBoolean(
                    i).GetValue());
            }
        }

        [NUnit.Framework.Test]
        public virtual void SingleSortOrderForMultipleKeysTest() {
            String[] keys = new String[] { "testKey1", "testKey2", "testKey3" };
            bool testAscending = true;
            PdfCollectionSort sort = new PdfCollectionSort(keys);
            // this line will throw an exception as number of parameters of setSortOrder()
            // method should be exactly the same as number of keys of PdfCollectionSort
            // here we have three keys but one param
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => sort.SetSortOrder(testAscending));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.YOU_HAVE_TO_DEFINE_A_BOOLEAN_ARRAY_FOR_THIS_COLLECTION_SORT_DICTIONARY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectMultipleSortOrderForMultipleKeysTest() {
            String[] keys = new String[] { "testKey1", "testKey2", "testKey3" };
            bool[] testAscendings = new bool[] { true, false };
            PdfCollectionSort sort = new PdfCollectionSort(keys);
            // this line will throw an exception as number of parameters of setSortOrder()
            // method should be exactly the same as number of keys of PdfCollectionSort
            // here we have three keys but two params
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => sort.SetSortOrder(testAscendings));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.NUMBER_OF_BOOLEANS_IN_THE_ARRAY_DOES_NOT_CORRESPOND_WITH_THE_NUMBER_OF_FIELDS
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void IsWrappedObjectMustBeIndirectTest() {
            String key = "testKey";
            NUnit.Framework.Assert.IsFalse(new PdfCollectionSort(key).IsWrappedObjectMustBeIndirect());
        }
    }
}
