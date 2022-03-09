using System.Threading;
using iText.Commons.Utils;
using iText.Layout.Renderer;

namespace iText.Forms.Fields {
    /// <summary>Class to store meta info that will be used in forms module in static context.</summary>
    public sealed class FormsMetaInfoStaticContainer {
        private static ThreadLocal<MetaInfoContainer> metaInfoForLayout = new ThreadLocal<MetaInfoContainer>();

        private FormsMetaInfoStaticContainer() {
        }

        // Empty constructor.
        /// <summary>Sets meta info related to forms into static context, executes the action and then cleans meta info.
        ///     </summary>
        /// <remarks>
        /// Sets meta info related to forms into static context, executes the action and then cleans meta info.
        /// <para />
        /// Keep in mind that this instance will only be accessible from the same thread.
        /// </remarks>
        /// <param name="metaInfoContainer">instance to be set.</param>
        /// <param name="action">action which will be executed while meta info is set to static context.</param>
        public static void UseMetaInfoDuringTheAction(MetaInfoContainer metaInfoContainer, Action action) {
            // TODO DEVSIX-6368 We want to prevent customer code being run while meta info is in the static context
            try {
                metaInfoForLayout.Value = metaInfoContainer;
                action();
            }
            finally {
                metaInfoForLayout.Value = null;
            }
        }

        /// <summary>Gets meta info which was set previously.</summary>
        /// <remarks>
        /// Gets meta info which was set previously.
        /// <para />
        /// Keep in mind that this operation will return meta info instance which was set previously from the same thread.
        /// </remarks>
        /// <returns>meta info instance.</returns>
        internal static MetaInfoContainer GetMetaInfoForLayout() {
            return metaInfoForLayout.Value;
        }
    }
}
