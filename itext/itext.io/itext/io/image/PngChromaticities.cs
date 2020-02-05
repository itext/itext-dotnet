namespace iText.IO.Image {
    public class PngChromaticities {
        private float xW;

        private float yW;

        private float xR;

        private float yR;

        private float xG;

        private float yG;

        private float xB;

        private float yB;

        public PngChromaticities(float xW, float yW, float xR, float yR, float xG, float yG, float xB, float yB) {
            this.xW = xW;
            this.yW = yW;
            this.xR = xR;
            this.yR = yR;
            this.xG = xG;
            this.yG = yG;
            this.xB = xB;
            this.yB = yB;
        }

        public virtual float GetXW() {
            return xW;
        }

        public virtual float GetYW() {
            return yW;
        }

        public virtual float GetXR() {
            return xR;
        }

        public virtual float GetYR() {
            return yR;
        }

        public virtual float GetXG() {
            return xG;
        }

        public virtual float GetYG() {
            return yG;
        }

        public virtual float GetXB() {
            return xB;
        }

        public virtual float GetYB() {
            return yB;
        }
    }
}
