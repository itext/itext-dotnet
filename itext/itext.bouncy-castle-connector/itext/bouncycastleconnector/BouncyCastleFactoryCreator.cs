/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Bouncycastle;
using iText.Bouncycastleconnector.Logs;
using iText.Bouncycastlefips;
using iText.Commons;
using iText.Commons.Bouncycastle;

namespace iText.Bouncycastleconnector {
    /// <summary>
    /// This class provides the ability to create
    /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
    /// instance
    /// to create bouncy-castle or bouncy-castle FIPS classes instances.
    /// </summary>
    /// <remarks>
    /// This class provides the ability to create
    /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
    /// instance
    /// to create bouncy-castle or bouncy-castle FIPS classes instances. User chooses which
    /// bouncy-castle will be used by specifying dependency, so either bouncy-castle or
    /// bouncy-castle-fips dependency must be added in order to use this class.
    /// </remarks>
    public sealed class BouncyCastleFactoryCreator {
        private static IBouncyCastleFactory factory;

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Bouncycastleconnector.BouncyCastleFactoryCreator
            ));

        static BouncyCastleFactoryCreator() {
            try {
                factory = new BouncyCastleFactory();
            }
            catch (FileNotFoundException) {
                try {
                    factory = new BouncyCastleFipsFactory();
                }
                catch (FileNotFoundException) {
                    LOGGER.LogError(BouncyCastleLogMessageConstant.BOUNCY_CASTLE_DEPENDENCY_MUST_PRESENT);
                }
            }
        }

        private BouncyCastleFactoryCreator() {
        }

        // Empty constructor.
        /// <summary>
        /// Returns
        /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
        /// instance to create bouncy-castle or bouncy-castle FIPS
        /// classes instances depending on specified dependency.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.IBouncyCastleFactory"/>
        /// appropriate implementation.
        /// </returns>
        public static IBouncyCastleFactory GetFactory() {
            return factory;
        }
    }
}
