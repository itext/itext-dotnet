/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
