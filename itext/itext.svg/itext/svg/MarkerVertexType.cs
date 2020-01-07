using System;

namespace iText.Svg {
    /// <summary>
    /// Defines a property of markable elements (&lt;path&gt;, &lt;line&gt;, &lt;polyline&gt; or
    /// &lt;polygon&gt;) which is used to determine at which verticies a marker should be drawn.
    /// </summary>
    public sealed class MarkerVertexType {
        /// <summary>Specifies that marker will be drawn only at the first vertex of element.</summary>
        public static readonly iText.Svg.MarkerVertexType MARKER_START = new iText.Svg.MarkerVertexType(SvgConstants.Attributes
            .MARKER_START);

        /// <summary>Specifies that marker will be drawn at every vertex except the first and last.</summary>
        public static readonly iText.Svg.MarkerVertexType MARKER_MID = new iText.Svg.MarkerVertexType(SvgConstants.Attributes
            .MARKER_MID);

        /// <summary>Specifies that marker will be drawn only at the last vertex of element.</summary>
        public static readonly iText.Svg.MarkerVertexType MARKER_END = new iText.Svg.MarkerVertexType(SvgConstants.Attributes
            .MARKER_END);

        private readonly String name;

        private MarkerVertexType(String s) {
            this.name = s;
        }

        public override String ToString() {
            return this.name;
        }
    }
}
