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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfFontFactoryTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/font/";

        [NUnit.Framework.Test]
        public virtual void StandardFontForceEmbeddedTest() {
            Type1Font fontProgram = (Type1Font)FontProgramFactory.CreateFont(StandardFonts.HELVETICA);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFontFactory.CreateFont(fontProgram
                , PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_EMBED_STANDARD_FONT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void StandardFontPreferEmbeddedTest() {
            Type1Font fontProgram = (Type1Font)FontProgramFactory.CreateFont(StandardFonts.HELVETICA);
            PdfType1Font font = (PdfType1Font)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void StandardFontPreferNotEmbeddedTest() {
            Type1Font fontProgram = (Type1Font)FontProgramFactory.CreateFont(StandardFonts.HELVETICA);
            PdfType1Font font = (PdfType1Font)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void StandardFontForceNotEmbeddedTest() {
            Type1Font fontProgram = (Type1Font)FontProgramFactory.CreateFont(StandardFonts.HELVETICA);
            PdfType1Font font = (PdfType1Font)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void CustomType1FontForceEmbeddedTest() {
            Type1Font fontProgram = new PdfFontFactoryTest.CustomType1FontProgram();
            PdfType1Font font = (PdfType1Font)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void CustomType1FontPreferEmbeddedTest() {
            Type1Font fontProgram = new PdfFontFactoryTest.CustomType1FontProgram();
            PdfType1Font font = (PdfType1Font)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void CustomType1FontPreferNotEmbeddedTest() {
            Type1Font fontProgram = new PdfFontFactoryTest.CustomType1FontProgram();
            PdfType1Font font = (PdfType1Font)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void CustomType1FontForceNotEmbeddedTest() {
            Type1Font fontProgram = new PdfFontFactoryTest.CustomType1FontProgram();
            PdfType1Font font = (PdfType1Font)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramUTF8AllowEmbeddingEncodingForceEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(true);
            PdfTrueTypeFont font = (PdfTrueTypeFont)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramUTF8AllowEmbeddingEncodingPreferEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(true);
            PdfTrueTypeFont font = (PdfTrueTypeFont)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramUTF8AllowEmbeddingEncodingPreferNotEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(true);
            PdfTrueTypeFont font = (PdfTrueTypeFont)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramUTF8AllowEmbeddingEncodingForceNotEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(true);
            PdfTrueTypeFont font = (PdfTrueTypeFont)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramUTF8NotAllowEmbeddingEncodingForceEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(false);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFontFactory.CreateFont(fontProgram
                , PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_BE_EMBEDDED_DUE_TO_LICENSING_RESTRICTIONS
                , "CustomNameCustomStyle"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramUTF8NotAllowEmbeddingEncodingPreferEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(false);
            PdfTrueTypeFont font = (PdfTrueTypeFont)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramUTF8NotAllowEmbeddingEncodingPreferNotEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(false);
            PdfTrueTypeFont font = (PdfTrueTypeFont)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramUTF8NotAllowEmbeddingEncodingForceNotEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(false);
            PdfTrueTypeFont font = (PdfTrueTypeFont)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.UTF8, PdfFontFactory.EmbeddingStrategy
                .FORCE_NOT_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramIdentityHAllowEmbeddingEncodingForceEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(true);
            PdfType0Font font = (PdfType0Font)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramIdentityHAllowEmbeddingEncodingPreferEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(true);
            PdfType0Font font = (PdfType0Font)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy
                .PREFER_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramIdentityHAllowEmbeddingEncodingPreferNotEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(true);
            PdfType0Font font = (PdfType0Font)PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy
                .PREFER_NOT_EMBEDDED);
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramIdentityHAllowEmbeddingEncodingForceNotEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(true);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFontFactory.CreateFont(fontProgram
                , PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_NOT_EMBEDDED));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_CREATE_TYPE_0_FONT_WITH_TRUE_TYPE_FONT_PROGRAM_WITHOUT_EMBEDDING_IT
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramIdentityHNotAllowEmbeddingEncodingForceEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(false);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFontFactory.CreateFont(fontProgram
                , PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_BE_EMBEDDED_DUE_TO_LICENSING_RESTRICTIONS
                , "CustomNameCustomStyle"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramIdentityHNotAllowEmbeddingEncodingPreferEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(false);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFontFactory.CreateFont(fontProgram
                , PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_BE_EMBEDDED_DUE_TO_LICENSING_RESTRICTIONS
                , "CustomNameCustomStyle"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramIdentityHNotAllowEmbeddingEncodingPreferNotEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(false);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFontFactory.CreateFont(fontProgram
                , PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_BE_EMBEDDED_DUE_TO_LICENSING_RESTRICTIONS
                , "CustomNameCustomStyle"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TrueTypeFontProgramIdentityHNotAllowEmbeddingEncodingForceNotEmbeddedTest() {
            TrueTypeFont fontProgram = new PdfFontFactoryTest.CustomTrueTypeFontProgram(false);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFontFactory.CreateFont(fontProgram
                , PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_NOT_EMBEDDED));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.CANNOT_BE_EMBEDDED_DUE_TO_LICENSING_RESTRICTIONS
                , "CustomNameCustomStyle"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void StandardFontCachedWithoutDocumentTest() {
            // this test ensures that method which allows caching into the document does not fail
            // if the document is null
            PdfDocument cacheTo = null;
            PdfType1Font font = (PdfType1Font)PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.UTF8, cacheTo
                );
            NUnit.Framework.Assert.IsNotNull(font);
            NUnit.Framework.Assert.IsFalse(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void CreateFontFromNullDictionaryTest() {
            PdfDictionary dictionary = null;
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFontFactory.CreateFont(dictionary
                ));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CANNOT_CREATE_FONT_FROM_NULL_PDF_DICTIONARY
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CreateFontFromEmptyDictionaryTest() {
            PdfDictionary dictionary = new PdfDictionary();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfFontFactory.CreateFont(dictionary
                ));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DICTIONARY_DOES_NOT_HAVE_SUPPORTED_FONT_DATA
                , e.Message);
        }

        private class CustomType1FontProgram : Type1Font {
            public override bool IsBuiltInFont() {
                return false;
            }
        }

        private class CustomTrueTypeFontProgram : TrueTypeFont {
            public CustomTrueTypeFontProgram(bool allowEmbedding) {
                this.fontNames = new PdfFontFactoryTest.CustomFontNames(allowEmbedding);
            }
        }

        private class CustomFontNames : FontNames {
            public CustomFontNames(bool allowEmbedding) {
                this.SetAllowEmbedding(allowEmbedding);
                this.SetFontName("CustomName");
                this.SetStyle("CustomStyle");
            }
        }
    }
}
