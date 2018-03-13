namespace iText.Svg.Css {
    /// <summary>
    /// Context necessary for evaluating certain Css statements whose final values depends on other statements
    /// e.g.
    /// </summary>
    /// <remarks>
    /// Context necessary for evaluating certain Css statements whose final values depends on other statements
    /// e.g. relative font-size statements.
    /// </remarks>
    public class CssContext {
        private float rootFontSize;

        public virtual float GetRootFontSize() {
            return rootFontSize;
        }
    }
}
