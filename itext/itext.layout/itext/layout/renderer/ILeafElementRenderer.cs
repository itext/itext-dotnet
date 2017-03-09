namespace iText.Layout.Renderer {
    public interface ILeafElementRenderer : IRenderer {
        /// <summary>
        /// Gets the maximum offset above the base line that this
        /// <see cref="ILeafElementRenderer"/>
        /// extends to.
        /// </summary>
        /// <returns>
        /// the upwards vertical offset of this
        /// <see cref="ILeafElementRenderer"/>
        /// </returns>
        float GetAscent();

        /// <summary>
        /// Gets the maximum offset below the base line that this
        /// <see cref="ILeafElementRenderer"/>
        /// extends to.
        /// </summary>
        /// <returns>
        /// the downwards vertical offset of this
        /// <see cref="ILeafElementRenderer"/>
        /// </returns>
        float GetDescent();
    }
}
