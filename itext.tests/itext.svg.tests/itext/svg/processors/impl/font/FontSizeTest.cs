/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Processors.Impl.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontSizeTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/processors/impl/font/FontSizeTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/processors/impl/font/FontSizeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void FontSize01Test() {
            String name = "fontSizeTest01";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNKNOWN_ABSOLUTE_METRIC_LENGTH_PARSED
            )]
        public virtual void FontSize02Test() {
            String name = "fontSizeTest02";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }

        [NUnit.Framework.Test]
        public virtual void FontSize03Test() {
            String name = "fontSizeTest03";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }

        [NUnit.Framework.Test]
        public virtual void FontAbsoluteKeywords() {
            String name = "fontAbsoluteKeywords";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }

        [NUnit.Framework.Test]
        public virtual void FontRelativeKeywords() {
            String name = "fontRelativeKeywords";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }

        [NUnit.Framework.Test]
        public virtual void DiffUnitsOfMeasure() {
            // TODO DEVSIX-2884 rem in font-size doesn't support correctly
            String name = "diff_units_of_measure";
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, name);
        }
    }
}
