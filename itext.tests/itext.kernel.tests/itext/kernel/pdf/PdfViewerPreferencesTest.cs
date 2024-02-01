/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfViewerPreferencesTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void PrintScalingTest() {
            PdfViewerPreferences preferences = new PdfViewerPreferences();
            PdfDictionary dictionary = preferences.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            // Set non-appropriate value
            preferences.SetPrintScaling(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA);
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            preferences.SetPrintScaling(PdfViewerPreferences.PdfViewerPreferencesConstants.NONE);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.None, dictionary.Get(PdfName.PrintScaling));
            preferences.SetPrintScaling(PdfViewerPreferences.PdfViewerPreferencesConstants.APP_DEFAULT);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.AppDefault, dictionary.Get(PdfName.PrintScaling));
        }

        [NUnit.Framework.Test]
        public virtual void DuplexTest() {
            PdfViewerPreferences preferences = new PdfViewerPreferences();
            PdfDictionary dictionary = preferences.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            // Set non-appropriate value
            preferences.SetDuplex(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA);
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            preferences.SetDuplex(PdfViewerPreferences.PdfViewerPreferencesConstants.SIMPLEX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.Simplex, dictionary.Get(PdfName.Duplex));
            preferences.SetDuplex(PdfViewerPreferences.PdfViewerPreferencesConstants.DUPLEX_FLIP_LONG_EDGE);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.DuplexFlipLongEdge, dictionary.Get(PdfName.Duplex));
            preferences.SetDuplex(PdfViewerPreferences.PdfViewerPreferencesConstants.DUPLEX_FLIP_SHORT_EDGE);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.DuplexFlipShortEdge, dictionary.Get(PdfName.Duplex));
        }

        [NUnit.Framework.Test]
        public virtual void NonFullScreenPageModeTest() {
            PdfViewerPreferences preferences = new PdfViewerPreferences();
            PdfDictionary dictionary = preferences.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            // Set non-appropriate value
            preferences.SetNonFullScreenPageMode(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA);
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            preferences.SetNonFullScreenPageMode(PdfViewerPreferences.PdfViewerPreferencesConstants.USE_THUMBS);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.UseThumbs, dictionary.Get(PdfName.NonFullScreenPageMode));
            preferences.SetNonFullScreenPageMode(PdfViewerPreferences.PdfViewerPreferencesConstants.USE_NONE);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.UseNone, dictionary.Get(PdfName.NonFullScreenPageMode));
            preferences.SetNonFullScreenPageMode(PdfViewerPreferences.PdfViewerPreferencesConstants.USE_OC);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.UseOC, dictionary.Get(PdfName.NonFullScreenPageMode));
            preferences.SetNonFullScreenPageMode(PdfViewerPreferences.PdfViewerPreferencesConstants.USE_OUTLINES);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.UseOutlines, dictionary.Get(PdfName.NonFullScreenPageMode));
        }

        [NUnit.Framework.Test]
        public virtual void DirectionTest() {
            PdfViewerPreferences preferences = new PdfViewerPreferences();
            PdfDictionary dictionary = preferences.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            // Set non-appropriate value
            preferences.SetDirection(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA);
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            preferences.SetDirection(PdfViewerPreferences.PdfViewerPreferencesConstants.LEFT_TO_RIGHT);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.L2R, dictionary.Get(PdfName.Direction));
            preferences.SetDirection(PdfViewerPreferences.PdfViewerPreferencesConstants.RIGHT_TO_LEFT);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.R2L, dictionary.Get(PdfName.Direction));
        }

        [NUnit.Framework.Test]
        public virtual void ViewAreaTest() {
            PdfViewerPreferences preferences = new PdfViewerPreferences();
            PdfDictionary dictionary = preferences.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            // Set non-appropriate value
            preferences.SetViewArea(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA);
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            preferences.SetViewArea(PdfViewerPreferences.PdfViewerPreferencesConstants.CROP_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.CropBox, dictionary.Get(PdfName.ViewArea));
            preferences.SetViewArea(PdfViewerPreferences.PdfViewerPreferencesConstants.ART_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.ArtBox, dictionary.Get(PdfName.ViewArea));
            preferences.SetViewArea(PdfViewerPreferences.PdfViewerPreferencesConstants.BLEED_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.BleedBox, dictionary.Get(PdfName.ViewArea));
            preferences.SetViewArea(PdfViewerPreferences.PdfViewerPreferencesConstants.MEDIA_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.MediaBox, dictionary.Get(PdfName.ViewArea));
            preferences.SetViewArea(PdfViewerPreferences.PdfViewerPreferencesConstants.TRIM_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.TrimBox, dictionary.Get(PdfName.ViewArea));
        }

        [NUnit.Framework.Test]
        public virtual void ViewClipTest() {
            PdfViewerPreferences preferences = new PdfViewerPreferences();
            PdfDictionary dictionary = preferences.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            // Set non-appropriate value
            preferences.SetViewClip(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA);
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            preferences.SetViewClip(PdfViewerPreferences.PdfViewerPreferencesConstants.CROP_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.CropBox, dictionary.Get(PdfName.ViewClip));
            preferences.SetViewClip(PdfViewerPreferences.PdfViewerPreferencesConstants.ART_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.ArtBox, dictionary.Get(PdfName.ViewClip));
            preferences.SetViewClip(PdfViewerPreferences.PdfViewerPreferencesConstants.BLEED_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.BleedBox, dictionary.Get(PdfName.ViewClip));
            preferences.SetViewClip(PdfViewerPreferences.PdfViewerPreferencesConstants.MEDIA_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.MediaBox, dictionary.Get(PdfName.ViewClip));
            preferences.SetViewClip(PdfViewerPreferences.PdfViewerPreferencesConstants.TRIM_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.TrimBox, dictionary.Get(PdfName.ViewClip));
        }

        [NUnit.Framework.Test]
        public virtual void PrintAreaTest() {
            PdfViewerPreferences preferences = new PdfViewerPreferences();
            PdfDictionary dictionary = preferences.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            // Set non-appropriate value
            preferences.SetPrintArea(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA);
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            preferences.SetPrintArea(PdfViewerPreferences.PdfViewerPreferencesConstants.CROP_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.CropBox, dictionary.Get(PdfName.PrintArea));
            preferences.SetPrintArea(PdfViewerPreferences.PdfViewerPreferencesConstants.ART_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.ArtBox, dictionary.Get(PdfName.PrintArea));
            preferences.SetPrintArea(PdfViewerPreferences.PdfViewerPreferencesConstants.BLEED_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.BleedBox, dictionary.Get(PdfName.PrintArea));
            preferences.SetPrintArea(PdfViewerPreferences.PdfViewerPreferencesConstants.MEDIA_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.MediaBox, dictionary.Get(PdfName.PrintArea));
            preferences.SetPrintArea(PdfViewerPreferences.PdfViewerPreferencesConstants.TRIM_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.TrimBox, dictionary.Get(PdfName.PrintArea));
        }

        [NUnit.Framework.Test]
        public virtual void PrintClipTest() {
            PdfViewerPreferences preferences = new PdfViewerPreferences();
            PdfDictionary dictionary = preferences.GetPdfObject();
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            // Set non-appropriate value
            preferences.SetPrintClip(PdfViewerPreferences.PdfViewerPreferencesConstants.PRINT_AREA);
            NUnit.Framework.Assert.AreEqual(0, dictionary.Size());
            preferences.SetPrintClip(PdfViewerPreferences.PdfViewerPreferencesConstants.CROP_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.CropBox, dictionary.Get(PdfName.PrintClip));
            preferences.SetPrintClip(PdfViewerPreferences.PdfViewerPreferencesConstants.ART_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.ArtBox, dictionary.Get(PdfName.PrintClip));
            preferences.SetPrintClip(PdfViewerPreferences.PdfViewerPreferencesConstants.BLEED_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.BleedBox, dictionary.Get(PdfName.PrintClip));
            preferences.SetPrintClip(PdfViewerPreferences.PdfViewerPreferencesConstants.MEDIA_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.MediaBox, dictionary.Get(PdfName.PrintClip));
            preferences.SetPrintClip(PdfViewerPreferences.PdfViewerPreferencesConstants.TRIM_BOX);
            NUnit.Framework.Assert.AreEqual(1, dictionary.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.TrimBox, dictionary.Get(PdfName.PrintClip));
        }
    }
}
