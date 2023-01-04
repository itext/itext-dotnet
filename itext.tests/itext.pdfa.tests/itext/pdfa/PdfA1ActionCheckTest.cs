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
using System;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Pdfa.Exceptions;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA1ActionCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        [NUnit.Framework.Test]
        public virtual void ActionCheck01() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary openActions = new PdfDictionary();
            openActions.Put(PdfName.S, PdfName.Launch);
            doc.GetCatalog().Put(PdfName.OpenAction, openActions);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException._0_ACTIONS_ARE_NOT_ALLOWED
                , PdfName.Launch.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ActionCheck02() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary openActions = new PdfDictionary();
            openActions.Put(PdfName.S, PdfName.Hide);
            doc.GetCatalog().Put(PdfName.OpenAction, openActions);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException._0_ACTIONS_ARE_NOT_ALLOWED
                , PdfName.Hide.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ActionCheck03() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary openActions = new PdfDictionary();
            openActions.Put(PdfName.S, PdfName.Sound);
            doc.GetCatalog().Put(PdfName.OpenAction, openActions);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException._0_ACTIONS_ARE_NOT_ALLOWED
                , PdfName.Sound.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ActionCheck04() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary openActions = new PdfDictionary();
            openActions.Put(PdfName.S, PdfName.Movie);
            doc.GetCatalog().Put(PdfName.OpenAction, openActions);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException._0_ACTIONS_ARE_NOT_ALLOWED
                , PdfName.Movie.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ActionCheck05() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary openActions = new PdfDictionary();
            openActions.Put(PdfName.S, PdfName.ResetForm);
            doc.GetCatalog().Put(PdfName.OpenAction, openActions);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException._0_ACTIONS_ARE_NOT_ALLOWED
                , PdfName.ResetForm.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ActionCheck06() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary openActions = new PdfDictionary();
            openActions.Put(PdfName.S, PdfName.ImportData);
            doc.GetCatalog().Put(PdfName.OpenAction, openActions);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException._0_ACTIONS_ARE_NOT_ALLOWED
                , PdfName.ImportData.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ActionCheck07() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary openActions = new PdfDictionary();
            openActions.Put(PdfName.S, PdfName.JavaScript);
            doc.GetCatalog().Put(PdfName.OpenAction, openActions);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException._0_ACTIONS_ARE_NOT_ALLOWED
                , PdfName.JavaScript.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ActionCheck08() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary openActions = new PdfDictionary();
            openActions.Put(PdfName.S, PdfName.Named);
            openActions.Put(PdfName.N, new PdfName("CustomName"));
            doc.GetCatalog().Put(PdfName.OpenAction, openActions);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException.NAMED_ACTION_TYPE_0_IS_NOT_ALLOWED
                , "CustomName"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ActionCheck09() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            page.SetAdditionalAction(PdfName.C, PdfAction.CreateJavaScript("js"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfAConformanceException._0_ACTIONS_ARE_NOT_ALLOWED
                , PdfName.JavaScript.GetValue()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ActionCheck10() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfPage page = doc.AddNewPage();
            PdfDictionary action = new PdfDictionary();
            action.Put(PdfName.S, PdfName.SetState);
            page.SetAdditionalAction(PdfName.C, new PdfAction(action));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.DEPRECATED_SETSTATE_AND_NOOP_ACTIONS_ARE_NOT_ALLOWED
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ActionCheck11() {
            PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            doc.GetCatalog().SetAdditionalAction(PdfName.C, PdfAction.CreateJavaScript("js"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfAConformanceException), () => doc.Close());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceException.A_CATALOG_DICTIONARY_SHALL_NOT_CONTAIN_AA_ENTRY, 
                e.Message);
        }
    }
}
