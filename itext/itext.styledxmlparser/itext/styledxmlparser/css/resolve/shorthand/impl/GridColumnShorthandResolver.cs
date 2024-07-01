using iText.StyledXmlParser.Css;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for grid-column shorthand.
    /// </summary>
    public class GridColumnShorthandResolver : GridItemShorthandResolver {
        /// <summary>Creates a shorthand resolver for grid-column property</summary>
        public GridColumnShorthandResolver()
            : base(CommonCssConstants.GRID_COLUMN) {
        }
    }
}
