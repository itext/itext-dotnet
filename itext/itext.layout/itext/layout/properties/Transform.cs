using System.Collections.Generic;
using iText.Kernel.Geom;

namespace iText.Layout.Properties {
    public class Transform {
        private IList<Transform.SingleTransform> multipleTransform;

        public Transform(int length) {
            multipleTransform = new List<Transform.SingleTransform>(length);
        }

        public virtual void AddSingleTransform(Transform.SingleTransform singleTransform) {
            multipleTransform.Add(singleTransform);
        }

        private IList<Transform.SingleTransform> GetMultipleTransform() {
            return multipleTransform;
        }

        public static AffineTransform GetAffineTransform(iText.Layout.Properties.Transform t, float width, float height
            ) {
            IList<Transform.SingleTransform> multipleTransform = t.GetMultipleTransform();
            AffineTransform affineTransform = new AffineTransform();
            for (int k = multipleTransform.Count - 1; k >= 0; k--) {
                Transform.SingleTransform transform = multipleTransform[k];
                float[] floats = new float[6];
                for (int i = 0; i < 4; i++) {
                    floats[i] = transform.GetFloats()[i];
                }
                for (int i = 4; i < 6; i++) {
                    floats[i] = transform.GetUnitValues()[i - 4].GetUnitType() == UnitValue.POINT ? transform.GetUnitValues()[
                        i - 4].GetValue() : transform.GetUnitValues()[i - 4].GetValue() / 100 * (i == 4 ? width : height);
                }
                affineTransform.PreConcatenate(new AffineTransform(floats));
            }
            return affineTransform;
        }

        public class SingleTransform {
            private float a;

            private float b;

            private float c;

            private float d;

            private UnitValue tx;

            private UnitValue ty;

            public SingleTransform() {
                this.a = 1;
                this.b = 0;
                this.c = 0;
                this.d = 1;
                this.tx = new UnitValue(UnitValue.POINT, 0);
                this.ty = new UnitValue(UnitValue.POINT, 0);
            }

            public SingleTransform(float a, float b, float c, float d, UnitValue tx, UnitValue ty) {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
                this.tx = tx;
                this.ty = ty;
            }

            private float[] GetFloats() {
                return new float[] { a, b, c, d };
            }

            private UnitValue[] GetUnitValues() {
                return new UnitValue[] { tx, ty };
            }
        }
    }
}
