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
using System;
using System.Collections.Generic;
using System.Linq;
using iText.IO.Font;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("UnitTest")]
    public class OpenTypeFontTableReaderTest : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/OpenTypeFontTableReaderTest/";

        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/sharedFontsResourceFiles/";

        private static readonly String CJK_JP_BOLD_PATH = FONTS_FOLDER + "NotoSansCJKjp-Bold.otf";

        private static readonly String SANS_MYANMAR_REGULAR_PATH = RESOURCE_FOLDER + "NotoSansMyanmar-Regular.ttf";

        [NUnit.Framework.Test]
        public virtual void TestFetchLangSysByTag() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(SANS_MYANMAR_REGULAR_PATH);
            GlyphSubstitutionTableReader gsub = fontProgram.GetGsubTable();
            ScriptRecord mym2 = gsub.GetScriptRecords()[0];
            NUnit.Framework.Assert.AreEqual("mym2", mym2.GetTag());
            // default LangSys has no tag
            NUnit.Framework.Assert.AreEqual("", gsub.GetLanguageRecord("mym2").GetTag());
            NUnit.Framework.Assert.AreEqual(LanguageTags.SGAW_KAREN, gsub.GetLanguageRecord("mym2", LanguageTags.SGAW_KAREN
                ).GetTag());
            NUnit.Framework.Assert.AreEqual(LanguageTags.MON, gsub.GetLanguageRecord("mym2", LanguageTags.MON).GetTag(
                ));
            NUnit.Framework.Assert.IsNull(gsub.GetLanguageRecord(null));
            NUnit.Framework.Assert.IsNull(gsub.GetLanguageRecord("mym3"));
            NUnit.Framework.Assert.IsNull(gsub.GetLanguageRecord("mym3", LanguageTags.SGAW_KAREN));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetLookupsArray() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(SANS_MYANMAR_REGULAR_PATH);
            GlyphSubstitutionTableReader gsub = fontProgram.GetGsubTable();
            FeatureRecord firstRecord = new FeatureRecord();
            firstRecord.SetLookups(new int[] { 5, 2 });
            firstRecord.SetTag("1");
            FeatureRecord secondRecord = new FeatureRecord();
            secondRecord.SetLookups(new int[] { 4, 10 });
            secondRecord.SetTag("2");
            FeatureRecord[] records = new FeatureRecord[] { firstRecord, secondRecord };
            int[] lookupsLocations = gsub.GetLookups(firstRecord).Select((record) => record.subTableLocations[0]).ToArray
                ();
            int[] expected = new int[] { 142610, 142436 };
            NUnit.Framework.Assert.AreEqual(expected, lookupsLocations);
            lookupsLocations = gsub.GetLookups(records).Select((record) => record.subTableLocations[0]).ToArray();
            expected = new int[] { 142436, 142538, 142610, 143908 };
            NUnit.Framework.Assert.AreEqual(expected, lookupsLocations);
        }

        [NUnit.Framework.Test]
        public virtual void GetNegativeIdxTest() {
            GlyphPositioningTableReader gposTableReader = GetGPosTableReader(SANS_MYANMAR_REGULAR_PATH);
            GposLookupType1 lookup = (GposLookupType1)gposTableReader.GetLookupTable(-1);
            NUnit.Framework.Assert.IsNull(lookup);
        }

        [NUnit.Framework.Test]
        public virtual void GetFeatureRecordsTest() {
            GlyphPositioningTableReader gposTableReader = GetGPosTableReader(SANS_MYANMAR_REGULAR_PATH);
            IList<FeatureRecord> lookup = gposTableReader.GetFeatureRecords();
            NUnit.Framework.Assert.AreEqual(3, lookup.Count);
        }

        [NUnit.Framework.Test]
        public virtual void GetFeaturesNullTest() {
            GlyphPositioningTableReader gposTableReader = GetGPosTableReader(SANS_MYANMAR_REGULAR_PATH);
            String[] scripts = new String[0];
            IList<FeatureRecord> lookup = gposTableReader.GetFeatures(scripts, "null");
            NUnit.Framework.Assert.IsNull(lookup);
        }

        [NUnit.Framework.Test]
        public virtual void GetRequiredFeaturesNullTest() {
            GlyphPositioningTableReader gposTableReader = GetGPosTableReader(SANS_MYANMAR_REGULAR_PATH);
            String[] scripts = new String[1];
            scripts[0] = "test";
            FeatureRecord requiredFeature = gposTableReader.GetRequiredFeature(scripts, "null");
            NUnit.Framework.Assert.IsNull(requiredFeature);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultLangTest() {
            GlyphPositioningTableReader gposTableReader = GetGPosTableReader(CJK_JP_BOLD_PATH);
            String[] scripts = new String[7];
            scripts[0] = "DFLT";
            IList<FeatureRecord> featureRecords = gposTableReader.GetFeatures(scripts, "");
            NUnit.Framework.Assert.AreEqual(8, featureRecords.Count);
        }

        [NUnit.Framework.Test]
        public virtual void NullStringArrayScriptsTest() {
            GlyphPositioningTableReader gposTableReader = GetGPosTableReader(CJK_JP_BOLD_PATH);
            String[] scripts = new String[7];
            IList<FeatureRecord> featureRecords = gposTableReader.GetFeatures(scripts, "");
            NUnit.Framework.Assert.AreEqual(8, featureRecords.Count);
        }

        [NUnit.Framework.Test]
        public virtual void NonDefTest() {
            GlyphPositioningTableReader gposTableReader = GetGPosTableReader(SANS_MYANMAR_REGULAR_PATH);
            String[] scripts = new String[7];
            scripts[2] = "DFLT";
            IList<FeatureRecord> featureRecords = gposTableReader.GetFeatures(scripts, "");
            NUnit.Framework.Assert.AreEqual(3, featureRecords.Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestFetchLangSysByTag2() {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(SANS_MYANMAR_REGULAR_PATH);
            GlyphSubstitutionTableReader gsub = fontProgram.GetGsubTable();
            NUnit.Framework.Assert.IsNull(gsub.GetLanguageRecord("mym2", LanguageTags.ARABIC));
        }

        [NUnit.Framework.Test]
        public virtual void SpecificEqualsNullTest() {
            GlyphPositioningTableReader gPosTableReader = GetGPosTableReader(CJK_JP_BOLD_PATH);
            IList<FeatureRecord> test = new List<FeatureRecord>();
            test.Add(new FeatureRecord());
            IList<FeatureRecord> specificFeatures = gPosTableReader.GetSpecificFeatures(test, null);
            NUnit.Framework.Assert.AreEqual(test, specificFeatures);
        }

        [NUnit.Framework.Test]
        public virtual void SpecificFeaturesTest() {
            GlyphPositioningTableReader gPosTableReader = GetGPosTableReader(CJK_JP_BOLD_PATH);
            String[] specific = new String[1];
            IList<FeatureRecord> test = new List<FeatureRecord>();
            test.Add(new FeatureRecord());
            IList<FeatureRecord> specificFeatures = gPosTableReader.GetSpecificFeatures(test, specific);
            NUnit.Framework.Assert.AreEqual(test, specificFeatures);
        }

        private GlyphPositioningTableReader GetGPosTableReader(String fontPath) {
            TrueTypeFont fontProgram = (TrueTypeFont)FontProgramFactory.CreateFont(fontPath);
            return fontProgram.GetGposTable();
        }
    }
}
