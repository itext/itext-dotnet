namespace iText.Kernel.Colors {
    /// <summary>
    /// Class containing predefined
    /// <see cref="DeviceRgb"/>
    /// colors.
    /// Color space specific classes should be used for the advanced handling of colors.
    /// The most common ones are
    /// <see cref="DeviceGray"/>
    /// ,
    /// <see cref="DeviceCmyk"/>
    /// and
    /// <see cref="DeviceRgb"/>
    /// .
    /// </summary>
    public class ColorConstants {
        /// <summary>Predefined black DeviceRgb color</summary>
        public static readonly Color BLACK = DeviceRgb.BLACK;

        /// <summary>Predefined blue  DeviceRgb color</summary>
        public static readonly Color BLUE = DeviceRgb.BLUE;

        /// <summary>Predefined cyan DeviceRgb color</summary>
        public static readonly Color CYAN = new DeviceRgb(0, 255, 255);

        /// <summary>Predefined dark gray DeviceRgb color</summary>
        public static readonly Color DARK_GRAY = new DeviceRgb(64, 64, 64);

        /// <summary>Predefined gray DeviceRgb color</summary>
        public static readonly Color GRAY = new DeviceRgb(128, 128, 128);

        /// <summary>Predefined green DeviceRgb color</summary>
        public static readonly Color GREEN = DeviceRgb.GREEN;

        /// <summary>Predefined light gray DeviceRgb color</summary>
        public static readonly Color LIGHT_GRAY = new DeviceRgb(192, 192, 192);

        /// <summary>Predefined magenta DeviceRgb color</summary>
        public static readonly Color MAGENTA = new DeviceRgb(255, 0, 255);

        /// <summary>Predefined orange DeviceRgb color</summary>
        public static readonly Color ORANGE = new DeviceRgb(255, 200, 0);

        /// <summary>Predefined pink DeviceRgb color</summary>
        public static readonly Color PINK = new DeviceRgb(255, 175, 175);

        /// <summary>Predefined red DeviceRgb color</summary>
        public static readonly Color RED = DeviceRgb.RED;

        /// <summary>Predefined white DeviceRgb color</summary>
        public static readonly Color WHITE = DeviceRgb.WHITE;

        /// <summary>Predefined yellow DeviceRgb color</summary>
        public static readonly Color YELLOW = new DeviceRgb(255, 255, 0);
    }
}
