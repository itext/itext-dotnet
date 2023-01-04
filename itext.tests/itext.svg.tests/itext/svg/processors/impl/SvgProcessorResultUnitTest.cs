/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System;
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Layout.Font;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg.Processors.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgProcessorResultUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ContextParameterCannotBeNullTest() {
            IDictionary<String, ISvgNodeRenderer> namedObjects = new Dictionary<String, ISvgNodeRenderer>();
            ISvgNodeRenderer root = new SvgTagSvgNodeRenderer();
            Exception exception = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new SvgProcessorResult
                (namedObjects, root, null));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.PARAMETER_CANNOT_BE_NULL, exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetFontProviderTest() {
            IDictionary<String, ISvgNodeRenderer> namedObjects = new Dictionary<String, ISvgNodeRenderer>();
            ISvgNodeRenderer root = new SvgTagSvgNodeRenderer();
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            SvgProcessorResult result = new SvgProcessorResult(namedObjects, root, context);
            FontProvider fontProviderFromResult = result.GetFontProvider();
            NUnit.Framework.Assert.IsNotNull(fontProviderFromResult);
            NUnit.Framework.Assert.AreSame(context.GetFontProvider(), fontProviderFromResult);
        }

        [NUnit.Framework.Test]
        public virtual void GetTempFontsTest() {
            IDictionary<String, ISvgNodeRenderer> namedObjects = new Dictionary<String, ISvgNodeRenderer>();
            ISvgNodeRenderer root = new SvgTagSvgNodeRenderer();
            SvgProcessorContext context = new SvgProcessorContext(new SvgConverterProperties());
            FontProgram fp = FontProgramFactory.CreateFont(StandardFonts.HELVETICA);
            context.AddTemporaryFont(fp, PdfEncodings.IDENTITY_H, "");
            SvgProcessorResult result = new SvgProcessorResult(namedObjects, root, context);
            FontSet tempFontsFromResult = result.GetTempFonts();
            NUnit.Framework.Assert.IsNotNull(tempFontsFromResult);
            NUnit.Framework.Assert.AreSame(context.GetTempFonts(), tempFontsFromResult);
        }
    }
}
