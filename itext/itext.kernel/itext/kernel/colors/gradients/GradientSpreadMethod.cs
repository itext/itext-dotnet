namespace iText.Kernel.Colors.Gradients {
    /// <summary>Represents possible spreading methods for gradient color outside of the coordinates vector</summary>
    public enum GradientSpreadMethod {
        /// <summary>Pad the corner colors to fill the necessary area</summary>
        PAD,
        /// <summary>Reflect the coloring to fill the necessary area</summary>
        REFLECT,
        /// <summary>Repeat the coloring to fill the necessary area</summary>
        REPEAT,
        /// <summary>No coloring outside of the coordinates vector</summary>
        NONE
    }
}
