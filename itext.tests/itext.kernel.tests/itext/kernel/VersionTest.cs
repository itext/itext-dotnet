/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using iText.Test;

namespace iText.Kernel {
    public class VersionTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseCurrentVersionTest() {
            iText.Kernel.Version instance = iText.Kernel.Version.GetInstance();
            // expected values
            String release = instance.GetRelease();
            String major = "7";
            String minor = iText.IO.Util.StringUtil.Split(release, "\\.")[1];
            String[] parseResults = iText.Kernel.Version.ParseVersionString(release);
            NUnit.Framework.Assert.AreEqual(2, parseResults.Length);
            NUnit.Framework.Assert.AreEqual(major, parseResults[0]);
            NUnit.Framework.Assert.AreEqual(minor, parseResults[1]);
        }

        [NUnit.Framework.Test]
        public virtual void ParseCustomCorrectVersionTest() {
            iText.Kernel.Version customVersion = new iText.Kernel.Version(new VersionInfo("iText®", "7.5.1-SNAPSHOT", 
                "iText® 7.5.1-SNAPSHOT ©2000-2090 iText Group NV (AGPL-version)", null), false);
            // expected values
            String major = "7";
            String minor = "5";
            String[] parseResults = iText.Kernel.Version.ParseVersionString(customVersion.GetRelease());
            NUnit.Framework.Assert.AreEqual(2, parseResults.Length);
            NUnit.Framework.Assert.AreEqual(major, parseResults[0]);
            NUnit.Framework.Assert.AreEqual(minor, parseResults[1]);
        }

        [NUnit.Framework.Test]
        public virtual void ParseVersionIncorrectMajorTest() {
            NUnit.Framework.Assert.That(() =>  {
                // the line below is expected to produce an exception
                String[] parseResults = iText.Kernel.Version.ParseVersionString("a.9.11");
            }
            , NUnit.Framework.Throws.InstanceOf<LicenseVersionException>().With.Message.EqualTo(LicenseVersionException.MAJOR_VERSION_IS_NOT_NUMERIC))
;
        }

        [NUnit.Framework.Test]
        public virtual void ParseVersionIncorrectMinorTest() {
            NUnit.Framework.Assert.That(() =>  {
                // the line below is expected to produce an exception
                iText.Kernel.Version.ParseVersionString("1.a.11");
            }
            , NUnit.Framework.Throws.InstanceOf<LicenseVersionException>().With.Message.EqualTo(LicenseVersionException.MINOR_VERSION_IS_NOT_NUMERIC))
;
        }

        [NUnit.Framework.Test]
        public virtual void IsVersionNumericPositiveIntegerTest() {
            NUnit.Framework.Assert.IsTrue(iText.Kernel.Version.IsVersionNumeric("7"));
        }

        [NUnit.Framework.Test]
        public virtual void IsVersionNumericNegativeIntegerTest() {
            NUnit.Framework.Assert.IsFalse(iText.Kernel.Version.IsVersionNumeric("-7"));
        }

        [NUnit.Framework.Test]
        public virtual void IsVersionNumericPositiveFloatTest() {
            NUnit.Framework.Assert.IsFalse(iText.Kernel.Version.IsVersionNumeric("5.973"));
        }

        [NUnit.Framework.Test]
        public virtual void IsVersionNumericNegativeFloatTest() {
            NUnit.Framework.Assert.IsFalse(iText.Kernel.Version.IsVersionNumeric("-5.973"));
        }

        [NUnit.Framework.Test]
        public virtual void IsVersionNumericLetterTest() {
            NUnit.Framework.Assert.IsFalse(iText.Kernel.Version.IsVersionNumeric("a"));
        }

        [NUnit.Framework.Test]
        public virtual void IsAGPLVersionTest() {
            NUnit.Framework.Assert.IsTrue(iText.Kernel.Version.IsAGPLVersion());
        }

        [NUnit.Framework.Test]
        public virtual void IsAGPLTrueTest() {
            iText.Kernel.Version customVersion = new iText.Kernel.Version(new VersionInfo("iText®", "7.5.1-SNAPSHOT", 
                "iText® 7.5.1-SNAPSHOT ©2000-2090 iText Group NV (AGPL-version)", null), false);
            NUnit.Framework.Assert.IsTrue(customVersion.IsAGPL());
        }

        [NUnit.Framework.Test]
        public virtual void IsAGPLFalseTest() {
            iText.Kernel.Version customVersion = new iText.Kernel.Version(new VersionInfo("iText®", "7.5.1-SNAPSHOT", 
                "iText® 7.5.1-SNAPSHOT ©2000-2090 iText Group NV", null), false);
            NUnit.Framework.Assert.IsFalse(customVersion.IsAGPL());
        }

        [NUnit.Framework.Test]
        public virtual void IsExpiredTest() {
            NUnit.Framework.Assert.IsFalse(iText.Kernel.Version.IsExpired());
        }

        [NUnit.Framework.Test]
        public virtual void GetInstanceTest() {
            iText.Kernel.Version instance = iText.Kernel.Version.GetInstance();
            CheckVersionInstance(instance);
        }

        [NUnit.Framework.Test]
        public virtual void CustomVersionCorrectTest() {
            iText.Kernel.Version customVersion = new iText.Kernel.Version(new VersionInfo("iText®", "7.5.1-SNAPSHOT", 
                "iText® 7.5.1-SNAPSHOT ©2000-2090 iText Group NV", null), false);
            CheckVersionInstance(customVersion);
        }

        [NUnit.Framework.Test]
        public virtual void CustomVersionIncorrectMajorTest() {
            iText.Kernel.Version customVersion = new iText.Kernel.Version(new VersionInfo("iText®", "8.5.1-SNAPSHOT", 
                "iText® 8.5.1-SNAPSHOT ©2000-2090 iText Group NV", null), false);
            NUnit.Framework.Assert.IsFalse(CheckVersion(customVersion.GetVersion()));
        }

        [NUnit.Framework.Test]
        public virtual void CustomVersionIncorrectMinorTest() {
            iText.Kernel.Version customVersion = new iText.Kernel.Version(new VersionInfo("iText®", "7.a.1-SNAPSHOT", 
                "iText® 7.a.1-SNAPSHOT ©2000-2090 iText Group NV", null), false);
            NUnit.Framework.Assert.IsFalse(CheckVersion(customVersion.GetVersion()));
        }

        [NUnit.Framework.Test]
        public virtual void CustomVersionIncorrectPatchTest() {
            iText.Kernel.Version customVersion = new iText.Kernel.Version(new VersionInfo("iText®", "7.50.a-SNAPSHOT", 
                "iText® 7.50.a-SNAPSHOT ©2000-2090 iText Group NV", null), false);
            NUnit.Framework.Assert.IsFalse(CheckVersion(customVersion.GetVersion()));
        }

        private static void CheckVersionInstance(iText.Kernel.Version instance) {
            String product = instance.GetProduct();
            String release = instance.GetRelease();
            String version = instance.GetVersion();
            String key = instance.GetKey();
            VersionInfo info = instance.GetInfo();
            NUnit.Framework.Assert.AreEqual(product, info.GetProduct());
            NUnit.Framework.Assert.AreEqual("iText®", product);
            NUnit.Framework.Assert.AreEqual(release, info.GetRelease());
            NUnit.Framework.Assert.IsTrue(release.Matches("[7]\\.[0-9]+\\.[0-9]+(-SNAPSHOT)?$"));
            NUnit.Framework.Assert.AreEqual(version, info.GetVersion());
            NUnit.Framework.Assert.IsTrue(CheckVersion(version));
            NUnit.Framework.Assert.IsNull(key);
        }

        private static bool CheckVersion(String version) {
            String regexp = "iText\\u00ae [7]\\.[0-9]+\\.[0-9]+(-SNAPSHOT)? \\u00a92000-20([2-9][0-9]) " + "iText Group NV( \\(AGPL-version\\))?";
            return version.Matches(regexp);
        }
    }
}
