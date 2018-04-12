using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;

namespace iText.Svg.Renderers.Path {
    /// <summary>Interface for IPathShape, which draws the Path-data's d element instructions.</summary>
    public interface IPathShape {
        /// <summary>Draws this instruction to a canvas object.</summary>
        /// <param name="canvas">to which this instruction is drawn</param>
        void Draw(PdfCanvas canvas);

        /// <summary>Sets the map of attributes that this path instruction needs.</summary>
        /// <param name="properties">maps key names to values.</param>
        void SetProperties(IDictionary<String, String> properties);

        /// <param name="coordinates">
        /// an array containing point values for path coordinates
        /// This method Mapps point attributes to their respective values
        /// </param>
        void SetCoordinates(String[] coordinates);

        IDictionary<String, String> GetCoordinates();
    }
}
