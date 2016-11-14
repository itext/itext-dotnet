namespace iText.Layout.Properties {
    public class HeightProperty {
        protected internal HeightPropertyType type;

        protected internal float height;

        public HeightProperty(HeightPropertyType type, float height) {
            this.type = type;
            this.height = height;
        }
    }
}
