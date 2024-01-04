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
using iText.Commons.Actions.Processors;
using iText.Test;

namespace iText.Commons.Actions {
    [NUnit.Framework.Category("UnitTest")]
    public class ProductProcessorFactoryKeeperTest : ExtendedITextTest {
        [NUnit.Framework.TearDown]
        public virtual void AfterEach() {
            ProductProcessorFactoryKeeper.RestoreDefaultProductProcessorFactory();
        }

        [NUnit.Framework.Test]
        public virtual void GettingDefaultFactoryFromKeeper() {
            IProductProcessorFactory productProcessorFactory = ProductProcessorFactoryKeeper.GetProductProcessorFactory
                ();
            NUnit.Framework.Assert.IsTrue(productProcessorFactory is DefaultProductProcessorFactory);
        }

        [NUnit.Framework.Test]
        public virtual void RestoringDefaultFactory() {
            ProductProcessorFactoryKeeper.SetProductProcessorFactory(new UnderAgplProductProcessorFactory());
            NUnit.Framework.Assert.IsTrue(ProductProcessorFactoryKeeper.GetProductProcessorFactory() is UnderAgplProductProcessorFactory
                );
            ProductProcessorFactoryKeeper.RestoreDefaultProductProcessorFactory();
            NUnit.Framework.Assert.IsTrue(ProductProcessorFactoryKeeper.GetProductProcessorFactory() is DefaultProductProcessorFactory
                );
        }
    }
}
