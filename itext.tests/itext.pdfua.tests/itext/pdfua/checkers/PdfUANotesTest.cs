using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUANotesTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUANotesTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return JavaUtil.ArraysAsList(PdfUAConformance.PDF_UA_1, PdfUAConformance.PDF_UA_2);
        }

        private static String GetRoleBasedOnConformance(PdfUAConformance pdfUAConformance) {
            return pdfUAConformance == PdfUAConformance.PDF_UA_1 ? StandardRoles.NOTE : StandardRoles.FENOTE;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddNoteForUA2AndFENoteForUA1Test(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_68(pdfUAConformance));
            String message = NUnit.Framework.Assert.Catch(typeof(PdfException), () => 
                        // It doesn't matter what we call here.
                        
                        // Test fails on document creation and verapdf validation isn't triggered anyway.
                        framework.AssertVeraPdfFail("addFENoteWithoutReferences", pdfUAConformance)).Message;
            String expectedExceptionMessage = pdfUAConformance == PdfUAConformance.PDF_UA_1 ? MessageFormatUtil.Format
                (KernelExceptionMessageConstant.ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE, StandardRoles.FENOTE) : MessageFormatUtil
                .Format(KernelExceptionMessageConstant.ROLE_IN_NAMESPACE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE, StandardRoles
                .NOTE, StandardNamespaces.PDF_2_0);
            NUnit.Framework.Assert.AreEqual(expectedExceptionMessage, message);
        }

        private sealed class _Generator_68 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_68(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("FENote");
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(pdfUAConformance == PdfUAConformance.PDF_UA_1 ? StandardRoles.FENOTE
                     : StandardRoles.NOTE);
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddFENoteWithoutReferencesTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_99(pdfUAConformance));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("addFENoteWithoutReferences", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("addFENoteWithoutReferences", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_99 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_99(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("FENote");
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddFENoteWithValidNoteTypeTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_125(pdfUAConformance));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("addFENoteWithValidNoteTypeTest", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("addFENoteWithValidNoteTypeTest", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_125 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_125(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("FENote");
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                PdfDictionary attribute = new PdfDictionary();
                attribute.Put(PdfName.O, PdfName.FENote);
                attribute.Put(PdfName.NoteType, PdfName.Endnote);
                note.GetAccessibilityProperties().AddAttributes(new PdfStructureAttributes(attribute));
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddFENoteWithInvalidNoteTypeTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_157(pdfUAConformance));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("addFENoteWithInvalidNoteTypeTest", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("addFENoteWithInvalidNoteTypeTest", PdfUAExceptionMessageConstants.INCORRECT_NOTE_TYPE_VALUE
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_157 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_157(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("FENote");
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                PdfDictionary attribute = new PdfDictionary();
                attribute.Put(PdfName.O, PdfName.FENote);
                attribute.Put(PdfName.NoteType, PdfName.End);
                note.GetAccessibilityProperties().AddAttributes(new PdfStructureAttributes(attribute));
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Count = 4, Ignore
             = true)]
        public virtual void RealContentDoesntHaveReferenceTest(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                pdfDocument.AddNewPage();
                PdfAnnotation annotation = new PdfTextAnnotation(new Rectangle(100, 100)).SetContents("Real content");
                pdfDocument.GetPage(1).AddAnnotation(annotation);
            }
            );
            framework.AddSuppliers(new _Generator_196(pdfUAConformance));
            framework.AddAfterGenerationHook((pdfDocument) => {
                TagTreePointer pointer = new TagTreePointer(pdfDocument);
                pointer.MoveToKid(GetRoleBasedOnConformance(pdfUAConformance));
                TagTreePointer feNotePointer = new TagTreePointer(pointer);
                feNotePointer.ApplyProperties(new DefaultAccessibilityProperties(pointer.GetRole()).AddRef(pointer.MoveToRoot
                    ().MoveToKid(StandardRoles.ANNOT)));
                pointer.MoveToRoot();
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("realContentDoesntHaveReferenceTest", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("realContentDoesntHaveReferenceTest", PdfUAExceptionMessageConstants.CONTENT_NOT_REFERENCING_FE_NOTE
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_196 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_196(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("FENote");
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Count = 4, Ignore
             = true)]
        public virtual void NoteDoesntHaveReferenceTest(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                pdfDocument.AddNewPage();
                PdfAnnotation annotation = new PdfTextAnnotation(new Rectangle(100, 100)).SetContents("Real content");
                pdfDocument.GetPage(1).AddAnnotation(annotation);
            }
            );
            framework.AddSuppliers(new _Generator_240(pdfUAConformance));
            framework.AddAfterGenerationHook((pdfDocument) => {
                TagTreePointer pointer = new TagTreePointer(pdfDocument);
                pointer.MoveToKid(StandardRoles.ANNOT);
                TagTreePointer realContentPointer = new TagTreePointer(pointer);
                realContentPointer.ApplyProperties(new DefaultAccessibilityProperties(pointer.GetRole()).AddRef(pointer.MoveToRoot
                    ().MoveToKid(GetRoleBasedOnConformance(pdfUAConformance))));
                pointer.MoveToRoot();
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("noteDoesntHaveReferenceTest", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("noteDoesntHaveReferenceTest", PdfUAExceptionMessageConstants.FE_NOTE_NOT_REFERENCING_CONTENT
                        , pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_240 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_240(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("FENote");
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Count = 4, Ignore
             = true)]
        public virtual void FeNoteWithValidReferencesTest(PdfUAConformance pdfUAConformance) {
            framework.AddBeforeGenerationHook((pdfDocument) => {
                pdfDocument.AddNewPage();
                PdfAnnotation annotation = new PdfTextAnnotation(new Rectangle(100, 100)).SetContents("Real content");
                pdfDocument.GetPage(1).AddAnnotation(annotation);
            }
            );
            framework.AddSuppliers(new _Generator_284(pdfUAConformance));
            framework.AddAfterGenerationHook((pdfDocument) => {
                TagTreePointer pointer = new TagTreePointer(pdfDocument);
                pointer.MoveToKid(StandardRoles.ANNOT);
                TagTreePointer realContentPointer = new TagTreePointer(pointer);
                realContentPointer.ApplyProperties(new DefaultAccessibilityProperties(pointer.GetRole()).AddRef(pointer.MoveToRoot
                    ().MoveToKid(GetRoleBasedOnConformance(pdfUAConformance))));
                TagTreePointer notePointer = new TagTreePointer(pointer);
                notePointer.ApplyProperties(new DefaultAccessibilityProperties(pointer.GetRole()).AddRef(pointer.MoveToRoot
                    ().MoveToKid(StandardRoles.ANNOT)));
                pointer.MoveToRoot();
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("feNoteWithValidReferencesTest", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("feNoteWithValidReferencesTest", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_284 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_284(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("FENote");
                PdfFont font;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddNoteWithoutIdTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_326(pdfUAConformance));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("noteWithoutID", PdfUAExceptionMessageConstants.NOTE_TAG_SHALL_HAVE_ID_ENTRY, pdfUAConformance
                    );
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("noteWithoutID", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_326 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_326(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.NAME_ALREADY_EXISTS_IN_THE_NAME_TREE, Ignore = true)]
        public virtual void AddTwoNotesWithSameIdTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_354(pdfUAConformance), new _Generator_370(pdfUAConformance));
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("twoNotesWithSameId", MessageFormatUtil.Format(PdfUAExceptionMessageConstants.NON_UNIQUE_ID_ENTRY_IN_STRUCT_TREE_ROOT
                    , "123"), false, pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("twoNotesWithSameId", pdfUAConformance);
                }
            }
        }

        private sealed class _Generator_354 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_354(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note 1");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                note.GetAccessibilityProperties().SetStructureElementIdString("123");
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        private sealed class _Generator_370 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_370(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note 2");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                note.GetAccessibilityProperties().SetStructureElementIdString("123");
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddNoteWithValidIdTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_398(pdfUAConformance));
            framework.AssertBothValid("noteWithValidID", pdfUAConformance);
        }

        private sealed class _Generator_398 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_398(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                note.GetAccessibilityProperties().SetStructureElementIdString("123");
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddTwoNotesWithDifferentIdTest(PdfUAConformance pdfUAConformance) {
            framework.AddSuppliers(new _Generator_421(pdfUAConformance), new _Generator_437(pdfUAConformance));
            framework.AssertBothValid("twoNotesWithDifferentId", pdfUAConformance);
        }

        private sealed class _Generator_421 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_421(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note 1");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                note.GetAccessibilityProperties().SetStructureElementIdString("123");
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }

        private sealed class _Generator_437 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_437(PdfUAConformance pdfUAConformance) {
                this.pdfUAConformance = pdfUAConformance;
            }

            public IBlockElement Generate() {
                Paragraph note = new Paragraph("note 2");
                PdfFont font = null;
                try {
                    font = PdfFontFactory.CreateFont(PdfUANotesTest.FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                        .FORCE_EMBEDDED);
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                note.SetFont(font);
                note.GetAccessibilityProperties().SetRole(PdfUANotesTest.GetRoleBasedOnConformance(pdfUAConformance));
                note.GetAccessibilityProperties().SetStructureElementIdString("234");
                return note;
            }

            private readonly PdfUAConformance pdfUAConformance;
        }
    }
}
