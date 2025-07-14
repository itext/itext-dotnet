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
