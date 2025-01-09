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
using System.Collections.Generic;
using iText.Test;

namespace iText.StyledXmlParser.Css.Media {
    [NUnit.Framework.Category("UnitTest")]
    public class MediaQueryTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void MatchTest() {
            MediaDeviceDescription deviceDescription = new MediaDeviceDescription("all");
            MediaQuery query = MediaQueryParser.ParseMediaQuery("not all and (min-width: 600px)");
            IList<MediaQuery> queries = MediaQueryParser.ParseMediaQueries("not all and (min-width: 600px), not all and (min-width: 500px)"
                );
            NUnit.Framework.Assert.IsTrue(query.Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(queries[0].Matches(deviceDescription));
            NUnit.Framework.Assert.IsTrue(queries[1].Matches(deviceDescription));
        }
    }
}
