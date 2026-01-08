/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.IO.Resolver.Resource;
using iText.Signatures;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("UnitTest")]
    public class ValidatorChainBuilderTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultClientsTest() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder();
            NUnit.Framework.Assert.AreEqual(typeof(DefaultResourceRetriever), ((CrlClientOnline)builder.GetCrlClient()
                ).GetResourceRetriever().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(DefaultResourceRetriever), ((OcspClientBouncyCastle)builder.GetOcspClient
                ()).GetResourceRetriever().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(DefaultResourceRetriever), builder.GetCertificateRetriever().GetResourceRetriever
                ().GetType());
        }

        [NUnit.Framework.Test]
        public virtual void CustomDeprecatedRetrieverUsedOnlyInCertRetrieverTest() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder().WithResourceRetriever(() => new _IResourceRetriever_56
                ());
            NUnit.Framework.Assert.AreEqual(typeof(DefaultResourceRetriever), ((CrlClientOnline)builder.GetCrlClient()
                ).GetResourceRetriever().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(DefaultResourceRetriever), ((OcspClientBouncyCastle)builder.GetOcspClient
                ()).GetResourceRetriever().GetType());
            NUnit.Framework.Assert.IsNull(builder.GetCertificateRetriever().GetResourceRetriever());
        }

        private sealed class _IResourceRetriever_56 : iText.StyledXmlParser.Resolver.Resource.IResourceRetriever {
            public _IResourceRetriever_56() {
            }

            public Stream GetInputStreamByUrl(Uri url) {
                return null;
            }

            public byte[] GetByteArrayByUrl(Uri url) {
                return new byte[0];
            }
        }

        [NUnit.Framework.Test]
        public virtual void CustomRetrieverUsedInDefaultClientsTest() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder().WithAdvancedResourceRetriever(() => new ValidatorChainBuilderTest.CustomResourceRetriever
                ());
            NUnit.Framework.Assert.AreEqual(typeof(ValidatorChainBuilderTest.CustomResourceRetriever), ((CrlClientOnline
                )builder.GetCrlClient()).GetResourceRetriever().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(ValidatorChainBuilderTest.CustomResourceRetriever), ((OcspClientBouncyCastle
                )builder.GetOcspClient()).GetResourceRetriever().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(ValidatorChainBuilderTest.CustomResourceRetriever), builder.GetCertificateRetriever
                ().GetResourceRetriever().GetType());
        }

        [NUnit.Framework.Test]
        public virtual void CustomRetrieverNotUsedInCustomClientsTest() {
            ValidatorChainBuilder builder = new ValidatorChainBuilder().WithAdvancedResourceRetriever(() => new ValidatorChainBuilderTest.CustomResourceRetriever
                ()).WithCrlClient(() => new CrlClientOnline()).WithOcspClient(() => new OcspClientBouncyCastle()).WithIssuingCertificateRetrieverFactory
                (() => new IssuingCertificateRetriever());
            NUnit.Framework.Assert.AreEqual(typeof(DefaultResourceRetriever), ((CrlClientOnline)builder.GetCrlClient()
                ).GetResourceRetriever().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(DefaultResourceRetriever), ((OcspClientBouncyCastle)builder.GetOcspClient
                ()).GetResourceRetriever().GetType());
            NUnit.Framework.Assert.AreEqual(typeof(DefaultResourceRetriever), builder.GetCertificateRetriever().GetResourceRetriever
                ().GetType());
        }

        private class CustomResourceRetriever : DefaultResourceRetriever {
        }
    }
}
