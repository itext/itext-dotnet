using System.Collections.Generic;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>Base interface to customize placing flex items on main flex direction.</summary>
    internal interface IFlexItemMainDirector {
        /// <summary>Apply the direction for placement the items in flex container.</summary>
        /// <param name="lines">
        /// flex lines calculated by
        /// <see cref="FlexUtil"/>.
        /// </param>
        /// <returns>All child renderers in updated order.</returns>
        IList<IRenderer> ApplyDirection(IList<IList<FlexItemInfo>> lines);

        /// <summary>Apply the direction for placement the items in flex line.</summary>
        /// <param name="renderers">
        /// list of renderers or
        /// <see cref="FlexItemInfo"/>.
        /// </param>
        void ApplyDirectionForLine<T>(IList<T> renderers);

        /// <summary>Apply alignment on main flex direction.</summary>
        /// <param name="line">flex line of items to apply alignment to.</param>
        /// <param name="justifyContent">alignment to apply.</param>
        /// <param name="freeSpace">precalculated free space to distribute between flex items in a line.</param>
        void ApplyAlignment(IList<FlexUtil.FlexItemCalculationInfo> line, JustifyContent justifyContent, float freeSpace
            );
    }
}
