/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Bouncycastle;
using iText.Bouncycastleconnector.Logs;
using iText.Bouncycastlefips;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Utils;

namespace iText.Bouncycastleconnector {
    // Android-Conversion-Skip-Line (BC FIPS isn't supported on Android)
    /// <summary>
    /// This class provides the ability to create
    /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
    /// instance.
    /// </summary>
    /// <remarks>
    /// This class provides the ability to create
    /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
    /// instance.
    /// User chooses which bouncy-castle will be created by specifying dependency.
    /// Bouncy-castle dependency must be added in order to use this class.
    /// </remarks>
    public sealed class BouncyCastleFactoryCreator {
        private static IBouncyCastleFactory factory;

        private static IDictionary<String, Func<IBouncyCastleFactory>> factories = new LinkedDictionary<String, Func
            <IBouncyCastleFactory>>();

        private const String FACTORY_ENVIRONMENT_VARIABLE_NAME = "ITEXT_BOUNCY_CASTLE_FACTORY_NAME";

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Bouncycastleconnector.BouncyCastleFactoryCreator
            ));

        static BouncyCastleFactoryCreator() {
            PopulateFactoriesMap();
            String factoryName = SystemUtil.GetEnvironmentVariable(FACTORY_ENVIRONMENT_VARIABLE_NAME);
            Func<IBouncyCastleFactory> systemVariableFactoryCreator = factories.Get(factoryName);
            if (systemVariableFactoryCreator != null) {
                TryCreateFactory(systemVariableFactoryCreator);
            }
            foreach (Func<IBouncyCastleFactory> factorySupplier in factories.Values) {
                if (factory != null) {
                    break;
                }
                TryCreateFactory(factorySupplier);
            }
            if (factory == null) {
                LOGGER.LogError(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
                factory = new BouncyCastleDefaultFactory();
            }
        }

        private BouncyCastleFactoryCreator() {
        }

        // Empty constructor.
        /// <summary>
        /// Sets
        /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
        /// instance, which will be used for bouncy-castle classes creation.
        /// </summary>
        /// <param name="newFactory">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
        /// instance to be set.
        /// </param>
        public static void SetFactory(IBouncyCastleFactory newFactory) {
            factory = newFactory;
        }

        /// <summary>
        /// Returns
        /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
        /// instance for bouncy-castle classes creation.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
        /// implementation.
        /// </returns>
        public static IBouncyCastleFactory GetFactory() {
            return factory;
        }

        private static void TryCreateFactory(Func<IBouncyCastleFactory> factoryCreator) {
            try {
                CreateFactory(factoryCreator);
            }
            catch (FileNotFoundException) {
            }
        }

        // Do nothing if factory cannot be created.
        private static void CreateFactory(Func<IBouncyCastleFactory> factoryCreator) {
            factory = factoryCreator();
        }

        private static void PopulateFactoriesMap() {
            factories.Put("bouncy-castle", () => new BouncyCastleFactory());
            factories.Put("bouncy-castle-fips", () => new BouncyCastleFipsFactory());
        }
        // Android-Conversion-Skip-Line (BC FIPS isn't supported on Android)
    }
}
