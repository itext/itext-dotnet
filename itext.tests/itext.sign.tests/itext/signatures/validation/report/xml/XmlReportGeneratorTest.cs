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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Cms;
using iText.Signatures.Testutils.Report.Xml;
using iText.Signatures.Validation;
using iText.Test;

namespace iText.Signatures.Validation.Report.Xml {
//\cond DO_NOT_DOCUMENT
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    internal class XmlReportGeneratorTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/validation/SignatureValidatorTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private ValidatorChainBuilder builder;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
        }

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            builder = new ValidatorChainBuilder();
        }

        [NUnit.Framework.Test]
        public virtual void BaseXmlReportGenerationTest() {
            using (PdfDocument document = new PdfDocument(new PdfReader(SOURCE_FOLDER + "docWithMultipleSignaturesAndTimeStamp.pdf"
                ))) {
                AdESReportAggregator reportAggregator = new DefaultAdESReportAggregator();
                builder.WithAdESReportAggregator(reportAggregator).BuildSignatureValidator(document).ValidateSignatures();
                XmlReportGenerator reportGenerator = new XmlReportGenerator(new XmlReportOptions());
                StringWriter stringWriter = new StringWriter();
                reportGenerator.Generate(reportAggregator.GetReport(), stringWriter);
                XmlReportTestTool testTool = new XmlReportTestTool(stringWriter.ToString());
                NUnit.Framework.Assert.AreEqual("ValidationReport", testTool.GetDocumentNode().Name);
                // There are 5 signatures, but 3 are timestamps
                NUnit.Framework.Assert.AreEqual(2, testTool.CountElements("//r:SignatureValidationReport"));
                XmlNodeList signatureValueNodes = testTool.ExecuteXpathAsNodeList("//r:SignatureValidationReport//ds:SignatureValue"
                    );
                IList<String> b64ReportedSignatures = new List<String>(signatureValueNodes.Count);
                for (int i = 0; i < signatureValueNodes.Count; i++) {
                    b64ReportedSignatures.Add(signatureValueNodes.Item(i).InnerText);
                }
                SignatureUtil sigUtil = new SignatureUtil(document);
                foreach (String sigName in sigUtil.GetSignatureNames()) {
                    PdfSignature signature = sigUtil.GetSignature(sigName);
                    if (!PdfName.ETSI_RFC3161.Equals(signature.GetSubFilter())) {
                        CMSContainer cms = new CMSContainer(sigUtil.GetSignature(sigName).GetContents().GetValueBytes());
                        String b64signature = Convert.ToBase64String(cms.GetSignerInfo().GetSignatureData());
                        NUnit.Framework.Assert.IsTrue(b64ReportedSignatures.Contains(b64signature));
                    }
                }
                // For each reported signature the certificate is added to the validation objects
                // We don't use something like
                // testTool.countElements("//r:ValidationObject[r:ObjectType=\"urn:etsi:019102:validationObject:certificate\"]");
                // here because it fails in native by not clear reason
                XmlNodeList objectTypesNodes = testTool.ExecuteXpathAsNodeList("//r:ValidationObject//r:ObjectType");
                int requiredObjectTypesCount = 0;
                for (int i = 0; i < objectTypesNodes.Count; i++) {
                    if ("urn:etsi:019102:validationObject:certificate".Equals(objectTypesNodes.Item(i).InnerText)) {
                        ++requiredObjectTypesCount;
                    }
                }
                NUnit.Framework.Assert.AreEqual(2, requiredObjectTypesCount);
                NUnit.Framework.Assert.IsNull(testTool.ValidateXMLSchema());
            }
        }
    }
//\endcond
}
