namespace iText.Layout.Properties {
    /// <summary>
    /// A specialized enum containing potential property values for
    /// <see cref="Property.GRID_FLOW"/>.
    /// </summary>
    public enum GridFlow {
        /// <summary>Defines row flow from left to right of a grid.</summary>
        ROW,
        /// <summary>Defines column flow from top to bottom of a grid.</summary>
        COLUMN,
        /// <summary>
        /// Same as
        /// <c>ROW</c>
        /// but uses dense algorithm for cell placement.
        /// </summary>
        ROW_DENSE,
        /// <summary>
        /// Same as
        /// <c>COLUMN</c>
        /// but uses dense algorithm for cell placement.
        /// </summary>
        COLUMN_DENSE
    }
}
