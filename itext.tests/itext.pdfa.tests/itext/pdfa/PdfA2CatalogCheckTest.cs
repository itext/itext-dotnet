using System;
using System.IO;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Pdfa {
    public class PdfA2CatalogCheckTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfA2CatalogCheckTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA2CatalogCheckTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CatalogCheck01() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                doc.AddNewPage();
                PdfDictionary ocProperties = new PdfDictionary();
                PdfDictionary d = new PdfDictionary();
                d.Put(PdfName.Name, new PdfString("CustomName"));
                PdfArray configs = new PdfArray();
                PdfDictionary config = new PdfDictionary();
                config.Put(PdfName.Name, new PdfString("CustomName"));
                configs.Add(config);
                ocProperties.Put(PdfName.D, d);
                ocProperties.Put(PdfName.Configs, configs);
                doc.GetCatalog().Put(PdfName.OCProperties, ocProperties);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.ValueOfNameEntryShallBeUniqueAmongAllOptionalContentConfigurationDictionaries));
;
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CatalogCheck02() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                doc.AddNewPage();
                PdfDictionary ocProperties = new PdfDictionary();
                PdfDictionary d = new PdfDictionary();
                d.Put(PdfName.Name, new PdfString("CustomName"));
                PdfArray configs = new PdfArray();
                PdfDictionary config = new PdfDictionary();
                config.Put(PdfName.Name, new PdfString("CustomName1"));
                configs.Add(config);
                config = new PdfDictionary();
                config.Put(PdfName.Name, new PdfString("CustomName1"));
                configs.Add(config);
                ocProperties.Put(PdfName.D, d);
                ocProperties.Put(PdfName.Configs, configs);
                doc.GetCatalog().Put(PdfName.OCProperties, ocProperties);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.ValueOfNameEntryShallBeUniqueAmongAllOptionalContentConfigurationDictionaries));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CatalogCheck03() {
            String outPdf = destinationFolder + "pdfA2b_catalogCheck03.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_catalogCheck03.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary ocProperties = new PdfDictionary();
            PdfDictionary d = new PdfDictionary();
            d.Put(PdfName.Name, new PdfString("CustomName"));
            PdfArray configs = new PdfArray();
            PdfDictionary config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName1"));
            configs.Add(config);
            config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName2"));
            configs.Add(config);
            ocProperties.Put(PdfName.D, d);
            ocProperties.Put(PdfName.Configs, configs);
            doc.GetCatalog().Put(PdfName.OCProperties, ocProperties);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CatalogCheck04() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                doc.AddNewPage();
                PdfDictionary ocProperties = new PdfDictionary();
                PdfDictionary d = new PdfDictionary();
                PdfArray configs = new PdfArray();
                PdfDictionary config = new PdfDictionary();
                config.Put(PdfName.Name, new PdfString("CustomName1"));
                configs.Add(config);
                config = new PdfDictionary();
                config.Put(PdfName.Name, new PdfString("CustomName2"));
                configs.Add(config);
                ocProperties.Put(PdfName.D, d);
                ocProperties.Put(PdfName.Configs, configs);
                doc.GetCatalog().Put(PdfName.OCProperties, ocProperties);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.OptionalContentConfigurationDictionaryShallContainNameEntry));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CatalogCheck05() {
            String outPdf = destinationFolder + "pdfA2b_catalogCheck05.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA2b_catalogCheck05.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            doc.AddNewPage();
            PdfDictionary ocProperties = new PdfDictionary();
            PdfDictionary d = new PdfDictionary();
            d.Put(PdfName.Name, new PdfString("CustomName"));
            PdfArray configs = new PdfArray();
            PdfDictionary config = new PdfDictionary();
            config.Put(PdfName.Name, new PdfString("CustomName1"));
            PdfArray order = new PdfArray();
            PdfDictionary orderItem = new PdfDictionary();
            orderItem.Put(PdfName.Name, new PdfString("CustomName2"));
            order.Add(orderItem);
            PdfDictionary orderItem1 = new PdfDictionary();
            orderItem1.Put(PdfName.Name, new PdfString("CustomName3"));
            order.Add(orderItem1);
            config.Put(PdfName.Order, order);
            PdfArray ocgs = new PdfArray();
            ocgs.Add(orderItem);
            ocgs.Add(orderItem1);
            ocProperties.Put(PdfName.OCGs, ocgs);
            configs.Add(config);
            ocProperties.Put(PdfName.D, d);
            ocProperties.Put(PdfName.Configs, configs);
            doc.GetCatalog().Put(PdfName.OCProperties, ocProperties);
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CatalogCheck06() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                doc.AddNewPage();
                PdfDictionary ocProperties = new PdfDictionary();
                PdfDictionary d = new PdfDictionary();
                d.Put(PdfName.Name, new PdfString("CustomName"));
                PdfArray configs = new PdfArray();
                PdfDictionary config = new PdfDictionary();
                config.Put(PdfName.Name, new PdfString("CustomName1"));
                PdfArray order = new PdfArray();
                PdfDictionary orderItem = new PdfDictionary();
                orderItem.Put(PdfName.Name, new PdfString("CustomName2"));
                order.Add(orderItem);
                PdfDictionary orderItem1 = new PdfDictionary();
                orderItem1.Put(PdfName.Name, new PdfString("CustomName3"));
                order.Add(orderItem1);
                config.Put(PdfName.Order, order);
                PdfArray ocgs = new PdfArray();
                ocgs.Add(orderItem);
                ocProperties.Put(PdfName.OCGs, ocgs);
                configs.Add(config);
                ocProperties.Put(PdfName.D, d);
                ocProperties.Put(PdfName.Configs, configs);
                doc.GetCatalog().Put(PdfName.OCProperties, ocProperties);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.OrderArrayShallContainReferencesToAllOcgs));
;
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CatalogCheck07() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                doc.AddNewPage();
                PdfDictionary ocProperties = new PdfDictionary();
                PdfDictionary d = new PdfDictionary();
                d.Put(PdfName.Name, new PdfString("CustomName"));
                PdfArray configs = new PdfArray();
                PdfDictionary config = new PdfDictionary();
                config.Put(PdfName.Name, new PdfString("CustomName1"));
                PdfArray order = new PdfArray();
                PdfDictionary orderItem = new PdfDictionary();
                orderItem.Put(PdfName.Name, new PdfString("CustomName2"));
                order.Add(orderItem);
                PdfDictionary orderItem1 = new PdfDictionary();
                orderItem1.Put(PdfName.Name, new PdfString("CustomName3"));
                config.Put(PdfName.Order, order);
                PdfArray ocgs = new PdfArray();
                ocgs.Add(orderItem);
                ocgs.Add(orderItem1);
                ocProperties.Put(PdfName.OCGs, ocgs);
                configs.Add(config);
                ocProperties.Put(PdfName.D, d);
                ocProperties.Put(PdfName.Configs, configs);
                doc.GetCatalog().Put(PdfName.OCProperties, ocProperties);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.OrderArrayShallContainReferencesToAllOcgs));
;
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CatalogCheck08() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                doc.AddNewPage();
                PdfDictionary ocProperties = new PdfDictionary();
                PdfDictionary d = new PdfDictionary();
                d.Put(PdfName.Name, new PdfString("CustomName"));
                PdfArray configs = new PdfArray();
                PdfDictionary config = new PdfDictionary();
                config.Put(PdfName.Name, new PdfString("CustomName1"));
                PdfArray order = new PdfArray();
                PdfDictionary orderItem = new PdfDictionary();
                orderItem.Put(PdfName.Name, new PdfString("CustomName2"));
                order.Add(orderItem);
                PdfDictionary orderItem1 = new PdfDictionary();
                orderItem1.Put(PdfName.Name, new PdfString("CustomName3"));
                order.Add(orderItem1);
                config.Put(PdfName.Order, order);
                PdfArray ocgs = new PdfArray();
                PdfDictionary orderItem2 = new PdfDictionary();
                orderItem2.Put(PdfName.Name, new PdfString("CustomName4"));
                ocgs.Add(orderItem2);
                PdfDictionary orderItem3 = new PdfDictionary();
                orderItem3.Put(PdfName.Name, new PdfString("CustomName5"));
                ocgs.Add(orderItem3);
                ocProperties.Put(PdfName.OCGs, ocgs);
                configs.Add(config);
                ocProperties.Put(PdfName.D, d);
                ocProperties.Put(PdfName.Configs, configs);
                doc.GetCatalog().Put(PdfName.OCProperties, ocProperties);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.OrderArrayShallContainReferencesToAllOcgs));
;
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CatalogCheck09() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                doc.AddNewPage();
                PdfDictionary names = new PdfDictionary();
                names.Put(PdfName.AlternatePresentations, new PdfDictionary());
                doc.GetCatalog().Put(PdfName.Names, names);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.CatalogDictionaryShallNotContainAlternatepresentationsNamesEntry));
;
        }

        /// <exception cref="System.IO.FileNotFoundException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CatalogCheck10() {
            NUnit.Framework.Assert.That(() =>  {
                PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
                PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                    , "http://www.color.org", "sRGB IEC61966-2.1", @is));
                doc.AddNewPage();
                doc.GetCatalog().Put(PdfName.Requirements, new PdfArray());
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.CatalogDictionaryShallNotContainRequirementsEntry));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void CompareResult(String outPdf, String cmpPdf) {
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
