/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.IO.Font.Otf;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Font {
    public class PdfType3FontTest : ExtendedITextTest {
        private const float EPS = 1e-4f;

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void AddDifferentGlyphsInConstructorTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            PdfDictionary charProcs = new PdfDictionary();
            charProcs.Put(new PdfName("space"), new PdfStream());
            charProcs.Put(new PdfName("A"), new PdfStream());
            dictionary.Put(PdfName.CharProcs, charProcs);
            dictionary.Put(PdfName.Widths, new PdfArray());
            dictionary.Put(PdfName.ToUnicode, PdfName.IdentityH);
            dictionary.Put(PdfName.Encoding, new PdfName("zapfdingbatsencoding"));
            PdfType3Font type3Font = new _PdfType3Font_65(dictionary);
            NUnit.Framework.Assert.IsNotNull(type3Font.GetFontProgram());
            int spaceGlyphCode = 32;
            Glyph glyph = type3Font.GetFontProgram().GetGlyph(spaceGlyphCode);
            NUnit.Framework.Assert.AreEqual(new Glyph(spaceGlyphCode, 0, new char[] { ' ' }), glyph);
            int AGlyphCode = 65;
            glyph = type3Font.GetFontProgram().GetGlyph(AGlyphCode);
            NUnit.Framework.Assert.AreEqual(new Glyph(AGlyphCode, 0, new char[] { 'A' }), glyph);
        }

        private sealed class _PdfType3Font_65 : PdfType3Font {
            public _PdfType3Font_65(PdfDictionary baseArg1)
                : base(baseArg1) {
            }

            protected internal override PdfDocument GetDocument() {
                return null;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void AddAlreadyExistingGlyphTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            PdfDictionary charProcs = new PdfDictionary();
            charProcs.Put(new PdfName("A"), new PdfStream());
            dictionary.Put(PdfName.CharProcs, charProcs);
            dictionary.Put(PdfName.Widths, new PdfArray());
            PdfType3Font type3Font = new _PdfType3Font_90(dictionary);
            Type3Glyph type3Glyph = type3Font.AddGlyph('A', 1, 2, 3, 5, 8);
            NUnit.Framework.Assert.AreEqual(0, type3Glyph.GetWx(), EPS);
            NUnit.Framework.Assert.AreEqual(0, type3Glyph.GetLlx(), EPS);
            NUnit.Framework.Assert.AreEqual(0, type3Glyph.GetLly(), EPS);
            NUnit.Framework.Assert.AreEqual(0, type3Glyph.GetUrx(), EPS);
            NUnit.Framework.Assert.AreEqual(0, type3Glyph.GetUry(), EPS);
        }

        private sealed class _PdfType3Font_90 : PdfType3Font {
            public _PdfType3Font_90(PdfDictionary baseArg1)
                : base(baseArg1) {
            }

            protected internal override PdfDocument GetDocument() {
                return null;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void SetFontStretchTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            PdfDictionary charProcs = new PdfDictionary();
            dictionary.Put(PdfName.CharProcs, charProcs);
            dictionary.Put(PdfName.Widths, new PdfArray());
            PdfType3Font type3Font = new PdfType3Font(dictionary);
            String fontStretch = "test";
            type3Font.SetFontStretch(fontStretch);
            NUnit.Framework.Assert.IsNotNull(type3Font.fontProgram);
            NUnit.Framework.Assert.IsNotNull(type3Font.fontProgram.GetFontNames());
            NUnit.Framework.Assert.AreEqual(fontStretch, type3Font.fontProgram.GetFontNames().GetFontStretch());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void SetPdfFontFlagsTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            PdfDictionary charProcs = new PdfDictionary();
            dictionary.Put(PdfName.CharProcs, charProcs);
            dictionary.Put(PdfName.Widths, new PdfArray());
            PdfType3Font type3Font = new PdfType3Font(dictionary);
            int randomTestFontFlagsValue = 5;
            type3Font.SetPdfFontFlags(randomTestFontFlagsValue);
            NUnit.Framework.Assert.IsNotNull(type3Font.fontProgram);
            NUnit.Framework.Assert.AreEqual(randomTestFontFlagsValue, type3Font.fontProgram.GetPdfFontFlags());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void GlyphWithUnicodeBiggerThan32CannotBeEncodedTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            PdfDictionary charProcs = new PdfDictionary();
            dictionary.Put(PdfName.CharProcs, charProcs);
            dictionary.Put(PdfName.Widths, new PdfArray());
            PdfType3Font type3Font = new PdfType3Font(dictionary);
            int cannotEncodeAndAUnicodeBiggerThan32TestValue = 333;
            NUnit.Framework.Assert.IsNull(type3Font.GetGlyph(cannotEncodeAndAUnicodeBiggerThan32TestValue));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void ContainsGlyphTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            PdfDictionary charProcs = new PdfDictionary();
            dictionary.Put(PdfName.CharProcs, charProcs);
            dictionary.Put(PdfName.Widths, new PdfArray());
            PdfType3Font type3Font = new _PdfType3Font_160(dictionary);
            NUnit.Framework.Assert.IsFalse(type3Font.ContainsGlyph(333));
            NUnit.Framework.Assert.IsFalse(type3Font.ContainsGlyph(-5));
            NUnit.Framework.Assert.IsFalse(type3Font.ContainsGlyph(32));
            type3Font.AddGlyph(' ', 0, 0, 0, 1, 1);
            NUnit.Framework.Assert.IsTrue(type3Font.ContainsGlyph(32));
            type3Font.AddGlyph('A', 0, 0, 0, 0, 0);
            NUnit.Framework.Assert.IsTrue(type3Font.ContainsGlyph(65));
        }

        private sealed class _PdfType3Font_160 : PdfType3Font {
            public _PdfType3Font_160(PdfDictionary baseArg1)
                : base(baseArg1) {
            }

            protected internal override PdfDocument GetDocument() {
                return null;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void FlushExceptionTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDictionary dictionary = new PdfDictionary();
                dictionary.Put(PdfName.FontMatrix, new PdfArray());
                PdfDictionary charProcs = new PdfDictionary();
                dictionary.Put(PdfName.CharProcs, charProcs);
                dictionary.Put(PdfName.Widths, new PdfArray());
                PdfType3Font type3Font = new PdfType3FontTest.DisableEnsureUnderlyingObjectHasIndirectReference(this, dictionary
                    );
                type3Font.Flush();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfException.NoGlyphsDefinedForType3Font))
;
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void FillFontDescriptorTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            PdfDictionary charProcs = new PdfDictionary();
            dictionary.Put(PdfName.CharProcs, charProcs);
            dictionary.Put(PdfName.Widths, new PdfArray());
            PdfDictionary fontDescriptor = new PdfDictionary();
            String fontStretch = "test";
            fontDescriptor.Put(PdfName.FontStretch, new PdfName(fontStretch));
            dictionary.Put(PdfName.FontDescriptor, fontDescriptor);
            PdfType3Font type3Font = new _PdfType3Font_203(dictionary);
            NUnit.Framework.Assert.IsNotNull(type3Font.fontProgram);
            NUnit.Framework.Assert.IsNotNull(type3Font.fontProgram.GetFontNames());
            NUnit.Framework.Assert.AreEqual(fontStretch, type3Font.fontProgram.GetFontNames().GetFontStretch());
        }

        private sealed class _PdfType3Font_203 : PdfType3Font {
            public _PdfType3Font_203(PdfDictionary baseArg1)
                : base(baseArg1) {
            }

            protected internal override PdfDocument GetDocument() {
                return null;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void NoCharProcsTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            dictionary.Put(PdfName.Widths, new PdfArray());
            NUnit.Framework.Assert.DoesNotThrow(() => new PdfType3Font(dictionary));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void NoEncodingTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            PdfDictionary charProcs = new PdfDictionary();
            dictionary.Put(PdfName.CharProcs, charProcs);
            dictionary.Put(PdfName.Widths, new PdfArray());
            NUnit.Framework.Assert.DoesNotThrow(() => new PdfType3Font(dictionary));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.TYPE3_FONT_INITIALIZATION_ISSUE)]
        public virtual void NoDifferenceTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            PdfDictionary charProcs = new PdfDictionary();
            dictionary.Put(PdfName.CharProcs, charProcs);
            dictionary.Put(PdfName.Widths, new PdfArray());
            PdfDictionary encoding = new PdfDictionary();
            dictionary.Put(PdfName.Encoding, encoding);
            NUnit.Framework.Assert.DoesNotThrow(() => new PdfType3Font(dictionary));
        }

        [NUnit.Framework.Test]
        public virtual void MissingFontMatrixTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.Widths, new PdfArray());
            dictionary.Put(PdfName.ToUnicode, PdfName.IdentityH);
            dictionary.Put(PdfName.Encoding, new PdfName("zapfdingbatsencoding"));
            NUnit.Framework.Assert.That(() =>  {
                new PdfType3Font(dictionary);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(MessageFormatUtil.Format(PdfException.MissingRequiredFieldInFontDictionary, PdfName.FontMatrix)))
;
        }

        [NUnit.Framework.Test]
        public virtual void MissingWidthsTest() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.FontMatrix, new PdfArray());
            dictionary.Put(PdfName.ToUnicode, PdfName.IdentityH);
            dictionary.Put(PdfName.Encoding, new PdfName("zapfdingbatsencoding"));
            NUnit.Framework.Assert.That(() =>  {
                new PdfType3Font(dictionary);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(MessageFormatUtil.Format(PdfException.MissingRequiredFieldInFontDictionary, PdfName.Widths)))
;
        }

        [NUnit.Framework.Test]
        public virtual void NoCharProcGlyphForDifferenceTest() {
            PdfDictionary font = new PdfDictionary();
            font.Put(PdfName.FontMatrix, new PdfArray());
            font.Put(PdfName.Widths, new PdfArray());
            font.Put(PdfName.CharProcs, new PdfDictionary());
            PdfDictionary encoding = new PdfDictionary();
            PdfArray differences = new PdfArray();
            differences.Add(0, new PdfNumber(65));
            differences.Add(1, new PdfName("A"));
            encoding.Put(PdfName.Differences, differences);
            font.Put(PdfName.Encoding, encoding);
            NUnit.Framework.Assert.DoesNotThrow(() => new PdfType3Font(font));
        }

        private class DisableEnsureUnderlyingObjectHasIndirectReference : PdfType3Font {
            internal DisableEnsureUnderlyingObjectHasIndirectReference(PdfType3FontTest _enclosing, PdfDictionary fontDictionary
                )
                : base(fontDictionary) {
                this._enclosing = _enclosing;
            }

            protected internal override void EnsureUnderlyingObjectHasIndirectReference() {
            }

            private readonly PdfType3FontTest _enclosing;
        }
    }
}
