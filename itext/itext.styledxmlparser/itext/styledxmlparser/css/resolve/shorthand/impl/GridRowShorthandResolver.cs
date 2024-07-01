using iText.StyledXmlParser.Css;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for grid-row shorthand.
    /// </summary>
    public class GridRowShorthandResolver : GridItemShorthandResolver {
        /// <summary>Creates a shorthand resolver for grid-row property</summary>
        public GridRowShorthandResolver()
            : base(CommonCssConstants.GRID_ROW) {
        }
    }
}
