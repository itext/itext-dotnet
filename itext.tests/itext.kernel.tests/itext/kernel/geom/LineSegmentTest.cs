/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using iText.Test;

namespace iText.Kernel.Geom {
    [NUnit.Framework.Category("UnitTest")]
    public class LineSegmentTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ContainsPointNullTest() {
            LineSegment lineSegment = new LineSegment(new Vector(20.3246f, 769.4974f, 1.0f), new Vector(151.22923f, 769.4974f
                , 1.0f));
            NUnit.Framework.Assert.IsFalse(lineSegment.ContainsPoint(null));
        }

        [NUnit.Framework.Test]
        public virtual void ContainsPointTest() {
            Vector pointToCheck = new Vector(20.3246f, 769.4974f, 1.0f);
            LineSegment lineSegment = new LineSegment(pointToCheck, new Vector(151.22923f, 769.4974f, 1.0f));
            NUnit.Framework.Assert.IsTrue(lineSegment.ContainsPoint(pointToCheck));
        }

        [NUnit.Framework.Test]
        public virtual void NotContainsPointLeftTest() {
            Vector pointToCheck = new Vector(100.3246f, 769.4974f, 1.0f);
            LineSegment lineSegment = new LineSegment(new Vector(120.3246f, 769.4974f, 1.0f), new Vector(151.22923f, 769.4974f
                , 1.0f));
            NUnit.Framework.Assert.IsFalse(lineSegment.ContainsPoint(pointToCheck));
        }

        [NUnit.Framework.Test]
        public virtual void NotContainsPointRightTest() {
            Vector pointToCheck = new Vector(160.3246f, 769.4974f, 1.0f);
            LineSegment lineSegment = new LineSegment(new Vector(120.3246f, 769.4974f, 1.0f), new Vector(151.22923f, 769.4974f
                , 1.0f));
            NUnit.Framework.Assert.IsFalse(lineSegment.ContainsPoint(pointToCheck));
        }

        [NUnit.Framework.Test]
        public virtual void ContainsSegmentNullTest() {
            LineSegment lineSegment = new LineSegment(new Vector(100.3246f, 769.4974f, 1.0f), new Vector(151.22923f, 769.4974f
                , 1.0f));
            NUnit.Framework.Assert.IsFalse(lineSegment.ContainsSegment(null));
        }

        [NUnit.Framework.Test]
        public virtual void ContainsSegmentTest() {
            LineSegment lineSegment = new LineSegment(new Vector(100.3246f, 769.4974f, 1.0f), new Vector(151.22923f, 769.4974f
                , 1.0f));
            LineSegment segmentToCheck = new LineSegment(new Vector(110.3246f, 769.4974f, 1.0f), new Vector(140.22923f
                , 769.4974f, 1.0f));
            NUnit.Framework.Assert.IsTrue(lineSegment.ContainsSegment(segmentToCheck));
        }

        [NUnit.Framework.Test]
        public virtual void NotContainsSegmentTest() {
            LineSegment lineSegment = new LineSegment(new Vector(120.3246f, 769.4974f, 1.0f), new Vector(151.22923f, 769.4974f
                , 1.0f));
            LineSegment segmentToCheck = new LineSegment(new Vector(110.3246f, 769.4974f, 1.0f), new Vector(115.22923f
                , 769.4974f, 1.0f));
            NUnit.Framework.Assert.IsFalse(lineSegment.ContainsSegment(segmentToCheck));
        }
    }
}
