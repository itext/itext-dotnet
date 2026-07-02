namespace iText.Kernel.Colors.Gradients {
    /// <summary>This resolver is used during the layout process to prevent infinite loops.</summary>
    public class GradientPropertiesResolver {
        /// <summary>Default max color stops.</summary>
        public const int DEFAULT_MAX_COLOR_STOPS = 10_000;

        private readonly int maxColorStops;

        /// <summary>
        /// Creates default instance of
        /// <see cref="GradientPropertiesResolver"/>.
        /// </summary>
        public GradientPropertiesResolver() {
            maxColorStops = DEFAULT_MAX_COLOR_STOPS;
        }

        /// <summary>
        /// Creates
        /// <see cref="GradientPropertiesResolver"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Creates
        /// <see cref="GradientPropertiesResolver"/>
        /// instance.
        /// <para />
        /// This resolver is used for gradient creation.
        /// </remarks>
        /// <param name="maxColorStops">
        /// max color stops for repeat and reflect
        /// (see
        /// <see cref="GetMaxColorStops()"/>
        /// </param>
        public GradientPropertiesResolver(int maxColorStops) {
            this.maxColorStops = maxColorStops;
        }

        /// <summary>Gets maximum color stops for repeat and reflect spreading.</summary>
        /// <remarks>
        /// Gets maximum color stops for repeat and reflect spreading.
        /// <para />
        /// This property defines the maximum amount of color stops to be created for
        /// repeat and reflect spreading of colors in gradients.
        /// </remarks>
        /// <returns>maximum color stops for repeat and reflect</returns>
        public virtual int GetMaxColorStops() {
            return maxColorStops;
        }
    }
}
