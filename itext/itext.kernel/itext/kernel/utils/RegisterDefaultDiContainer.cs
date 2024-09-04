using iText.Commons.Utils;
using iText.Kernel.DI.Pagetree;

namespace iText.Kernel.Utils {
    /// <summary>Registers a default instance for a dependency injection container for the kernel module.</summary>
    public class RegisterDefaultDiContainer {
        private const int DEFAULT_PAGE_TREE_LIST_FACTORY_MAX_SAFE_ENTRIES = 50_000;

        /// <summary>
        /// Creates an instance of
        /// <see cref="RegisterDefaultDiContainer"/>.
        /// </summary>
        public RegisterDefaultDiContainer() {
        }

        static RegisterDefaultDiContainer() {
            // Empty constructor but should be public as we need it for automatic class loading
            // sharp
            DIContainer.RegisterDefault(typeof(IPageTreeListFactory), () => new DefaultPageTreeListFactory(DEFAULT_PAGE_TREE_LIST_FACTORY_MAX_SAFE_ENTRIES
                ));
        }
    }
}
