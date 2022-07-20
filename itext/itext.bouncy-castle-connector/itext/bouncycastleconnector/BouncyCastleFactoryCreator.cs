using System.IO;
using Microsoft.Extensions.Logging;
using iText.Bouncycastle;
using iText.Bouncycastleconnector.Logs;
using iText.Bouncycastlefips;
using iText.Commons;
using iText.Commons.Bouncycastle;

namespace iText.Bouncycastleconnector {
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
        public static IBouncyCastleFactory GetFactory() {
            return factory;
        }
    }
}
