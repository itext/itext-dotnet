using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers.Path.Impl;

namespace iText.Svg.Renderers.Impl {
    public class PathShapeUnitTest {
        [NUnit.Framework.Test]
        public virtual void NullAttributesTest() {
            NUnit.Framework.Assert.That(() =>  {
                new PathShapeUnitTest.DummyShape(this).GetCoordinate(null, "");
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.ATTRIBUTES_NULL));
;
        }

        [NUnit.Framework.Test]
        public virtual void NullCoordinateTest() {
            NUnit.Framework.Assert.That(() =>  {
                IDictionary<String, String> attributes = new Dictionary<String, String>();
                attributes.Put(SvgConstants.Attributes.X, null);
                new PathShapeUnitTest.DummyShape(this).GetCoordinate(attributes, SvgConstants.Attributes.X);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.COORDINATE_VALUE_ABSENT));
;
        }

        [NUnit.Framework.Test]
        public virtual void EmptyCoordinateTest() {
            NUnit.Framework.Assert.That(() =>  {
                IDictionary<String, String> attributes = new Dictionary<String, String>();
                attributes.Put(SvgConstants.Attributes.X, "");
                new PathShapeUnitTest.DummyShape(this).GetCoordinate(attributes, SvgConstants.Attributes.X);
            }
            , NUnit.Framework.Throws.TypeOf<SvgProcessingException>().With.Message.EqualTo(SvgLogMessageConstant.COORDINATE_VALUE_ABSENT));
;
        }

        private class DummyShape : AbstractPathShape {
            public override void Draw(PdfCanvas canvas) {
            }

            public override void SetCoordinates(String[] coordinates) {
            }

            internal DummyShape(PathShapeUnitTest _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly PathShapeUnitTest _enclosing;
        }
    }
}
