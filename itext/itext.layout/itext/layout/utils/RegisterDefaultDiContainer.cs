using iText.Commons.Utils;

namespace iText.Layout.Utils {
    /// <summary>Registers a default instance for a dependency injection container for the layout module.</summary>
    public class RegisterDefaultDiContainer {
        static RegisterDefaultDiContainer() {
            DIContainer.RegisterDefault(typeof(LayoutInfiniteLoopResolver), () => new LayoutInfiniteLoopResolver());
        }

        /// <summary>
        /// Creates an instance of
        /// <see cref="iText.Kernel.Utils.RegisterDefaultDiContainer"/>.
        /// </summary>
        public RegisterDefaultDiContainer() {
        }
        // Empty constructor but should be public as we need it for automatic class loading
        // sharp
    }
}
