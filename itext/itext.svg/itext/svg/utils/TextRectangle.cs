using iText.Kernel.Geom;

namespace iText.Svg.Utils {
    /// <summary>A rectangle adapted for working with text elements.</summary>
    public class TextRectangle : Rectangle {
        /// <summary>The y coordinate of the line on which the text is located.</summary>
        private float textBaseLineYCoordinate;

        /// <summary>Create new instance of text rectangle.</summary>
        /// <param name="x">the x coordinate of lower left point</param>
        /// <param name="y">the y coordinate of lower left point</param>
        /// <param name="width">the width value</param>
        /// <param name="height">the height value</param>
        /// <param name="textBaseLineYCoordinate">the y coordinate of the line on which the text is located.</param>
        public TextRectangle(float x, float y, float width, float height, float textBaseLineYCoordinate)
            : base(x, y, width, height) {
            this.textBaseLineYCoordinate = textBaseLineYCoordinate;
        }

        /// <summary>Return the far right point of the rectangle with y on the baseline.</summary>
        /// <returns>the far right baseline point</returns>
        public virtual Point GetTextBaseLineRightPoint() {
            return new Point(GetRight(), textBaseLineYCoordinate);
        }
    }
}
