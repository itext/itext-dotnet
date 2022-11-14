namespace iText.Layout.Properties {
    /// <summary>
    /// The possible values for
    /// <see cref="InlineVerticalAlignment.GetType()"/>.
    /// </summary>
    public enum InlineVerticalAlignmentType {
        BASELINE,
        TEXT_TOP,
        TEXT_BOTTOM,
        SUB,
        SUPER,
        /// <summary>Fixed is used when a length value is given in css.</summary>
        /// <remarks>
        /// Fixed is used when a length value is given in css.
        /// It needs a companion value in
        /// <see cref="InlineVerticalAlignment.SetValue(float)"/>
        /// </remarks>
        FIXED,
        /// <summary>Fixed is used when a percentage value is given in css.</summary>
        /// <remarks>
        /// Fixed is used when a percentage value is given in css.
        /// It needs a companion value in
        /// <see cref="InlineVerticalAlignment.SetValue(float)"/>
        /// </remarks>
        FRACTION,
        MIDDLE,
        TOP,
        BOTTOM
    }
}
