using System.Collections.Generic;
using iText.Test;

namespace iText.StyledXmlParser.Css.Media {
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
