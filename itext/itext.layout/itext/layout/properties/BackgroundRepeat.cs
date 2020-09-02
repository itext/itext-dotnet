namespace iText.Layout.Properties {
    /// <summary>Class to hold background-repeat property.</summary>
    public class BackgroundRepeat {
        private readonly bool repeatX;

        private readonly bool repeatY;

        /// <summary>
        /// Creates a new
        /// <see cref="BackgroundRepeat"/>
        /// instance.
        /// </summary>
        /// <param name="repeatX">whether the background repeats in the x dimension.</param>
        /// <param name="repeatY">whether the background repeats in the y dimension.</param>
        public BackgroundRepeat(bool repeatX, bool repeatY) {
            this.repeatX = repeatX;
            this.repeatY = repeatY;
        }

        /// <summary>Is repeatX is true.</summary>
        /// <returns>repeatX value</returns>
        public virtual bool IsRepeatX() {
            return repeatX;
        }

        /// <summary>Is repeatY is true.</summary>
        /// <returns>repeatY value</returns>
        public virtual bool IsRepeatY() {
            return repeatY;
        }
    }
}
