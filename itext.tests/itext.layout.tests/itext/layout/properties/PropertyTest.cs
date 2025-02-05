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
using System.Reflection;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Layout.Properties {
    [NUnit.Framework.Category("UnitTest")]
    public class PropertyTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void PropertyUniquenessTest() {
            ICollection<int> fieldValues = new HashSet<int>();
            int maxFieldValue = 1;
            foreach (FieldInfo field in typeof(Property).GetFields()) {
                if (field.FieldType == typeof(int)) {
                    int value = (int)field.GetValue(null);
                    maxFieldValue = Math.Max(maxFieldValue, value);
                    if (fieldValues.Contains(value)) {
                        NUnit.Framework.Assert.Fail(MessageFormatUtil.Format("Multiple fields with same value: {0}", value));
                    }
                    fieldValues.Add(value);
                }
            }
            for (int i = 1; i <= maxFieldValue; i++) {
                if (!fieldValues.Contains(i)) {
                    NUnit.Framework.Assert.Fail(MessageFormatUtil.Format("Missing value: {0}", i));
                }
            }
            System.Console.Out.WriteLine(MessageFormatUtil.Format("Max field value: {0}", maxFieldValue));
        }
    }
}
