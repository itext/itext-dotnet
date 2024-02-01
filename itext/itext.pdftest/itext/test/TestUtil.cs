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
using System.IO;
using NUnit.Framework;

namespace iText.Test {
    public static class TestUtil {
        public static String GetParentProjectDirectory(String testDirectory) {
            DirectoryInfo projectDirectoryInfo = new DirectoryInfo(testDirectory);
            while (!projectDirectoryInfo.Name.Equals("bin", StringComparison.OrdinalIgnoreCase)) {
                projectDirectoryInfo = projectDirectoryInfo.Parent;
            }
            return projectDirectoryInfo.Parent.FullName;
        }

        public static void AreEqual(double[] expected, double[] actual, double margin)
        {
            Assert.That(actual, Is.EqualTo(expected).Within(margin));
        }
        
        public static void AreEqual(float[] expected, float[] actual, float margin)
        {
            Assert.That(actual, Is.EqualTo(expected).Within(margin));
        }
    }
}
