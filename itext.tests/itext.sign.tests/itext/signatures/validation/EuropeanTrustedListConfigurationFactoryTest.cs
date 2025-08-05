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
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("IntegrationTest")]
    public class EuropeanTrustedListConfigurationFactoryTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetFactory() {
            EuropeanTrustedListConfigurationFactory factory = EuropeanTrustedListConfigurationFactory.GetFactory()();
            NUnit.Framework.Assert.IsNotNull(factory, "Factory should not be null");
            NUnit.Framework.Assert.IsTrue(factory is LoadFromModuleEuropeanTrustedListConfigurationFactory, "Factory should be an instance of LoadFromModuleEuropeanTrustedListConfigurationFactory"
                );
        }

        [NUnit.Framework.Test]
        public virtual void SetFactoryNull() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => {
                EuropeanTrustedListConfigurationFactory.SetFactory(null);
            }
            );
            NUnit.Framework.Assert.IsNotNull(e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SetFactory() {
            EuropeanTrustedListConfigurationFactory factory = EuropeanTrustedListConfigurationFactory.GetFactory()();
            Func<EuropeanTrustedListConfigurationFactory> supplier = () => factory;
            EuropeanTrustedListConfigurationFactory.SetFactory(supplier);
            NUnit.Framework.Assert.IsNotNull(EuropeanTrustedListConfigurationFactory.GetFactory(), "Factory should not be null after setting it with a supplier"
                );
        }
    }
}
