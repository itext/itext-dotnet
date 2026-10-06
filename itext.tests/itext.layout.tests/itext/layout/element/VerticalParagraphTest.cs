/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Renderer.Typography;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout.Element {
    [NUnit.Framework.Category("UnitTest")]
    public class VerticalParagraphTest : ExtendedITextTest {
        private static readonly String CJK_FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/BioRhymeExpanded-Regular.ttf";

        [NUnit.Framework.TearDown]
        public virtual void Cleanup() {
            TypographyUtils.SetTypographyApplierInstance(new DefaultTypographyApplier());
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.UNSUPPORTED_PROPERTY, LogLevel = LogLevelConstants.WARN)]
        public virtual void SettingUnsupportedPropertiesMustLogWarning() {
            VerticalParagraph verticalParagraph = new VerticalParagraph(false);
            Leading leading = new Leading(Leading.MULTIPLIED, 3f);
            verticalParagraph.SetProperty(Property.FLOAT, FloatPropertyValue.LEFT);
            verticalParagraph.SetProperty(Property.LEADING, leading);
            verticalParagraph.GetRenderer().SetParent(new VerticalParagraphTest.TestRenderer());
            // Keep sonar happy; the real test is through the log messages, so we just assert true here.
            NUnit.Framework.Assert.AreEqual(leading, verticalParagraph.GetProperty<Leading>(Property.LEADING));
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.UNSUPPORTED_PROPERTY, LogLevel = LogLevelConstants.WARN)]
        public virtual void UnsupportedInheritedPropertiesMustLogWarning() {
            VerticalParagraph verticalParagraph = new VerticalParagraph(false);
            VerticalParagraphTest.TestRenderer parentRenderer = new VerticalParagraphTest.TestRenderer();
            parentRenderer.SetProperty(Property.TEXT_ANCHOR, TextAnchor.END);
            IRenderer verticalParagraphRenderer = verticalParagraph.GetRenderer();
            verticalParagraphRenderer.SetParent(parentRenderer);
            // Actual test is done through the log messages, so we just assert here to keep sonar happy.
            NUnit.Framework.Assert.IsNull(verticalParagraph.GetProperty<TextAnchor?>(Property.TEXT_ANCHOR));
        }

        [NonParallelizable]
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_WARNING)]
        public virtual void VerticalTextShouldUseDefaultTypographyApplierWhenTypographyAvailable() {
            PdfFont font = PdfFontFactory.CreateFont(CJK_FONT);
            VerticalParagraphTest.TestTypographyApplier testApplier = new VerticalParagraphTest.TestTypographyApplier(
                true);
            TypographyUtils.SetTypographyApplierInstance(testApplier);
            Document dummyDocument = new Document(new PdfDocument(new PdfWriter(new MemoryStream())));
            VerticalParagraph p = new VerticalParagraph("Hello world!", false);
            p.Add("\u0E2D\u0E32\u0E01\u0E32\u0E28");
            p.SetFont(font);
            p.SetProperty(Property.TYPOGRAPHY_CONFIG, true);
            p.SetProperty(Property.FONT_KERNING, FontKerning.YES);
            dummyDocument.Add(p);
            dummyDocument.Close();
            NUnit.Framework.Assert.IsFalse(testApplier.called, "Default typography applier should be used for vertical text"
                );
        }

        [NonParallelizable]
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.TYPOGRAPHY_NOT_FOUND_WARNING)]
        public virtual void VerticalTextShouldUseDefaultTypographyApplierWhenTypographyNotAvailable() {
            PdfFont font = PdfFontFactory.CreateFont(CJK_FONT);
            VerticalParagraphTest.TestTypographyApplier testApplier = new VerticalParagraphTest.TestTypographyApplier(
                false);
            TypographyUtils.SetTypographyApplierInstance(testApplier);
            Document dummyDocument = new Document(new PdfDocument(new PdfWriter(new MemoryStream())));
            VerticalParagraph p = new VerticalParagraph("Hello world!", false);
            p.Add("\u0E2D\u0E32\u0E01\u0E32\u0E28");
            p.SetFont(font);
            p.SetProperty(Property.TYPOGRAPHY_CONFIG, true);
            p.SetProperty(Property.FONT_KERNING, FontKerning.YES);
            dummyDocument.Add(p);
            dummyDocument.Close();
            NUnit.Framework.Assert.IsFalse(testApplier.called, "Default typography applier should be used for vertical text"
                );
        }

        private class TestRenderer : AbstractRenderer {
            public TestRenderer()
                : base() {
            }

            public override LayoutResult Layout(LayoutContext layoutContext) {
                return null;
            }

            public override IRenderer GetNextRenderer() {
                return null;
            }
        }

        private class TestTypographyApplier : AbstractTypographyApplier {
            public readonly bool emulatePdfCalligraphInstance;

            public bool called = false;

            public TestTypographyApplier(bool emulatePdfCalligraphInstance)
                : base() {
                this.emulatePdfCalligraphInstance = emulatePdfCalligraphInstance;
            }

            public override bool IsPdfCalligraphInstance() {
                //This one is not counted
                return emulatePdfCalligraphInstance;
            }

            public override ICollection<UnicodeScript> GetSupportedScripts() {
                called = true;
                return JavaCollectionsUtil.EmptyList<UnicodeScript>();
            }

            public override ICollection<UnicodeScript> GetSupportedScripts(Object configurator) {
                called = true;
                return base.GetSupportedScripts(configurator);
            }

            public override bool ApplyOtfScript(TrueTypeFont font, GlyphLine glyphLine, UnicodeScript? script, Object 
                configurator, SequenceId id, IMetaInfo metaInfo) {
                called = true;
                return base.ApplyOtfScript(font, glyphLine, script, configurator, id, metaInfo);
            }

            public override bool ApplyKerning(FontProgram fontProgram, GlyphLine text, SequenceId sequenceId, IMetaInfo
                 metaInfo) {
                called = true;
                return base.ApplyKerning(fontProgram, text, sequenceId, metaInfo);
            }

            public override byte[] GetBidiLevels(BaseDirection? baseDirection, int[] unicodeIds, SequenceId sequenceId
                , IMetaInfo metaInfo) {
                called = true;
                return base.GetBidiLevels(baseDirection, unicodeIds, sequenceId, metaInfo);
            }

            public override int[] ReorderLine(IList<LineRenderer.RendererGlyph> line, byte[] lineLevels, byte[] levels
                ) {
                called = true;
                return base.ReorderLine(line, lineLevels, levels);
            }

            public override IList<int> GetPossibleBreaks(String str) {
                called = true;
                return base.GetPossibleBreaks(str);
            }

            public override IDictionary<String, byte[]> LoadShippedFonts() {
                called = true;
                return base.LoadShippedFonts();
            }
        }
    }
}
