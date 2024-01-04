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
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Action {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfActionOcgStateTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void PdfActionOcgStateUsageTest() {
            PdfName stateName = PdfName.ON;
            PdfDictionary ocgDict1 = new PdfDictionary();
            ocgDict1.Put(PdfName.Type, PdfName.OCG);
            ocgDict1.Put(PdfName.Name, new PdfName("ocg1"));
            PdfDictionary ocgDict2 = new PdfDictionary();
            ocgDict2.Put(PdfName.Type, PdfName.OCG);
            ocgDict2.Put(PdfName.Name, new PdfName("ocg2"));
            IList<PdfDictionary> dicts = new List<PdfDictionary>();
            dicts.Add(ocgDict1);
            dicts.Add(ocgDict2);
            PdfActionOcgState ocgState = new PdfActionOcgState(stateName, dicts);
            NUnit.Framework.Assert.AreEqual(stateName, ocgState.GetState());
            NUnit.Framework.Assert.AreEqual(dicts, ocgState.GetOcgs());
            IList<PdfObject> states = ocgState.GetObjectList();
            NUnit.Framework.Assert.AreEqual(3, states.Count);
            NUnit.Framework.Assert.AreEqual(stateName, states[0]);
            NUnit.Framework.Assert.AreEqual(ocgDict1, states[1]);
            NUnit.Framework.Assert.AreEqual(ocgDict2, states[2]);
        }
    }
}
