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
using System.IO;
using iText.Test;

namespace iText.Signatures.Validation {
//\cond DO_NOT_DOCUMENT
    [NUnit.Framework.Category("IntegrationTest")]
    internal class XmlCountryRetrieverTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation" + "/XmlCertificateRetrieverTest/";

        [NUnit.Framework.Test]
        public virtual void ReadLotlCertificatesTest() {
            String xmlPath = SOURCE_FOLDER + "eu-lotl.xml";
            XmlCountryRetriever xmlCountryRetriever = new XmlCountryRetriever();
            IList<XmlCountryRetriever.CountrySpecificLotl> otherCountryList = xmlCountryRetriever.GetAllCountriesLotlFilesLocation
                (iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(xmlPath)));
            NUnit.Framework.Assert.AreEqual(32, otherCountryList.Count);
            foreach (XmlCountryRetriever.CountrySpecificLotl countrySpecificLotl in otherCountryList) {
                NUnit.Framework.Assert.IsNotNull(countrySpecificLotl.GetSchemeTerritory(), "Scheme territory should not be null for country: "
                     + countrySpecificLotl.GetTslLocation());
                NUnit.Framework.Assert.IsNotNull(countrySpecificLotl.GetTslLocation(), "TSL location should not be null for country: "
                     + countrySpecificLotl.GetSchemeTerritory());
            }
        }
    }
//\endcond
}
