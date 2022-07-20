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
