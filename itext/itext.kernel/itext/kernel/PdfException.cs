/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System.Collections.Generic;
using iText.IO.Util;

namespace iText.Kernel {
    public class PdfException : Exception {
        public const String _1IsAnUnknownGraphicsStateDictionary = "{0} is.an.unknown.graphics.state.dictionary";

        public const String _1IsNotAValidPlaceableWindowsMetafile = "{0} is.not.a.valid.placeable.windows.metafile";

        public const String AnnotShallHaveReferenceToPage = "annot.shall.have.reference.to.page";

        public const String AppendModeRequiresADocumentWithoutErrorsEvenIfRecoveryWasPossible = "append.mode.requires.a.document.without.errors.even.if.recovery.was.possible";

        public const String AuthenticatedAttributeIsMissingTheDigest = "authenticated.attribute.is.missing.the.digest";

        public const String AvailableSpaceIsNotEnoughForSignature = "available.space.is.not.enough.for.signature";

        public const String BadCertificateAndKey = "Bad public key certificate and/or private key.";

        public const String BadUserPassword = "Bad user password. Password is not provided or provided wrong password. Correct password should be passed to PdfReader constructor with properties. See ReaderProperties.setPassword method.";

        public const String CannotAddKidToTheFlushedElement = "cannot.add.kid.to.the.flushed.element";

        public const String CannotAddNonDictionaryExtGStateToResources1 = "Cannot add graphic state to resources. The PdfObject type is {0}, but should be PdfDictionary";

        public const String CannotAddNonDictionaryPatternToResources1 = "Cannot add pattern to resources. The PdfObject type is {0}, but should be PdfDictionary or PdfStream";

        public const String CannotAddNonDictionaryPropertiesToResources1 = "Cannot add properties to resources. The PdfObject type is {0}, but should be PdfDictionary";

        public const String CannotAddNonDictionaryShadingToResources1 = "Cannot add shading to resources. The PdfObject type is {0}, but should be PdfDictionary or PdfStream";

        public const String CannotAddNonStreamFormToResources1 = "Cannot add form to resources. The PdfObject type is {0}, but should be PdfStream";

        public const String CannotAddNonStreamImageToResources1 = "Cannot add image to resources. The PdfObject type is {0}, but should be PdfStream";

        public const String CannotCloseDocument = "cannot.close.document";

        public const String CannotCloseDocumentWithAlreadyFlushedPdfCatalog = "cannot.close.document.with.already.flushed.pdf.catalog";

        public const String CannotConvertPdfArrayToRectanle = "cannot.convert.pdfarray.to.rectangle";

        public const String CannotCopyFlushedObject = "cannot.copy.flushed.object";

        public const String CannotCopyFlushedTag = "cannot.copy.flushed.tag";

        public const String CannotCopyObjectContent = "cannot.copy.object.content";

        public const String CannotCopyIndirectObjectFromTheDocumentThatIsBeingWritten = "cannot.copy.indirect.object.from.the.document.that.is.being.written";

        public const String CannotCopyToDocumentOpenedInReadingMode = "cannot.copy.to.document.opened.in.reading.mode";

        public const String CannotCreateLayoutImageByWmfImage = "Cannot create layout image by WmfImage instance. First convert the image into FormXObject and then use the corresponding layout image constructor";

        public const String CannotCreatePdfImageXObjectByWmfImage = "Cannot create PdfImageXObject instance by WmfImage. Use PdfFormXObject constructor instead.";

        public const String CannotCreatePdfStreamByInputStreamWithoutPdfDocument = "cannot.create.pdfstream.by.inputstream.without.pdfdocument";

        public const String CannotGetContentBytes = "cannot.get.content.bytes";

        public const String CannotGetPdfStreamBytes = "cannot.get.pdfstream.bytes";

        public const String CannotRetrieveMediaBoxAttribute = "Invalid pdf. There is no media box attribute for page or its parents.";

        public const String CannotFindImageDataOrEI = "cannot.find.image.data.or.EI";

        public const String CannotFlushDocumentRootTagBeforeDocumentIsClosed = "cannot.flush.document.root.tag.before.document.is.closed";

        public const String CannotFlushObject = "cannot.flush.object";

        public const String CannotMoveToFlushedKid = "cannot.move.to.flushed.kid";

        public const String CannotMoveToMarkedContentReference = "cannot.move.to.marked.content.reference";

        public const String CannotMoveToParentCurrentElementIsRoot = "cannot.move.to.parent.current.element.is.root";

        public const String CannotOpenDocument = "cannot.open.document";

        public const String CannotParseContentStream = "could.not.parse.content.stream";

        public const String CannotReadAStreamInOrderToAppendNewBytes = "cannot.read.a.stream.in.order.to.append.new.bytes.reason {0}";

        public const String CannotReadPdfObject = "cannot.read.pdf.object";

        public const String CannotRemoveDocumentRootTag = "cannot.remove.document.root.tag";

        public const String CannotRemoveMarkedContentReferenceBecauseItsPageWasAlreadyFlushed = "cannot.remove.marked.content.reference.because.its.page.was.already.flushed";

        public const String CannotRemoveTagBecauseItsParentIsFlushed = "cannot.remove.tag.because.its.parent.is.flushed";

        public const String CannotSetDataToPdfstreamWhichWasCreatedByInputstream = "cannot.set.data.to.pdfstream.which.was.created.by.inputstream";

        public const String CannotSplitDocumentThatIsBeingWritten = "cannot.split.document.that.is.being.written";

        public const String CannotWritePdfStream = "cannot.write.pdf.stream";

        public const String CannotWriteObjectAfterItWasReleased = "Cannot write object after it was released. In normal situation the object must be read once again before being written";

        public const String CantDecodePkcs7SigneddataObject = "can.t.decode.pkcs7signeddata.object";

        public const String CantFindSigningCertificateWithSerial1 = "can.t.find.signing.certificate.with.serial {0}";

        public const String CertificateIsNotProvidedDocumentIsEncryptedWithPublicKeyCertificate = "Certificate is not provided. Document is encrypted with public key certificate, it should be passed to PdfReader constructor with properties. See ReaderProperties.setPublicKeySecurityParams method.";

        public const String CfNotFoundEncryption = "cf.not.found.encryption";

        public const String CodabarCharacterOneIsIllegal = "the.character {0} is.illegal.in.codabar";

        public const String CodabarMustHaveAtLeastAStartAndStopCharacter = "codabar.must.have.at.least.a.start.and.stop.character";

        public const String CodabarMustHaveOneAbcdAsStartStopCharacter = "codabar.must.have.one.of.abcd.as.start.stop.character";

        public const String CodabarStartStopCharacterAreOnlyExtremes = "in.codabar.start.stop.characters.are.only.allowed.at.the.extremes";

        public const String ColorNotFound = "color.not.found";

        public const String ColorSpaceNotFound = "color.space.not.found";

        public const String ContentStreamMustNotInvokeOperatorsThatSpecifyColorsOrOtherColorRelatedParameters = "content.stream.must.not.invoke.operators.that.specify.colors.or.other.color.related.parameters.in.the.graphics.state";

        public const String DecodeParameterType1IsNotSupported = "decode.parameter.type {0} is.not.supported";

        public const String DefaultcryptfilterNotFoundEncryption = "defaultcryptfilter.not.found.encryption";

        public const String DictionaryKey1IsNotAName = "dictionary.key {0} is.not.a.name";

        public const String DictionaryNotContainFontData = "dict.not.contain.font.data";

        public const String DocumentAlreadyPreClosed = "document.already.pre.closed";

        public const String DocumentClosedImpossibleExecuteAction = "document.was.closed.it.is.impossible.execute.action";

        public const String DocumentDoesntContainStructTreeRoot = "document.doesn't.contain.structtreeroot";

        public const String DocumentHasNoPages = "document.has.no.pages";

        public const String DocumentHasNoCatalogObject = "document.has.no.catalog.object";

        public const String DocumentMustBePreclosed = "document.must.be.preclosed";

        public const String DocumentToCopyToCannotBeNull = "document.to.copy.to.cannot.be.null";

        public const String ElementCannotFitAnyArea = "element.cannot.fit.any.area";

        public const String EncryptionCanOnlyBeAddedBeforeOpeningDocument = "encryption.can.only.be.added.before.opening.the.document";

        public const String EndOfContentStreamReachedBeforeEndOfImageData = "end.of.content.stream.reached.before.end.of.image.data";

        public const String ErrorReadingObjectStream = "error.reading.objstm";

        public const String FailedToGetTsaResponseFrom1 = "failed.to.get.tsa.response.from {0}";

        public const String FieldFlatteningIsNotSupportedInAppendMode = "field.flattening.is.not.supported.in.append.mode";

        public const String FieldIsAlreadySigned = "field.flattening.is.not.supported.in.append.mode";

        public const String FieldNamesCannotContainADot = "field.names.cannot.contain.a.dot";

        public const String FieldTypeIsNotASignatureFieldType = "the.field.type.is.not.a.signature.field.type";

        public const String Filter1IsNotSupported = "filter {0} is.not.supported";

        public const String FilePosition0CrossReferenceEntryInThisXrefSubsection = "file.position {0} cross.reference.entry.in.this.xref.subsection";

        public const String FilterCcittfaxdecodeIsOnlySupportedForImages = "filter.ccittfaxdecode.is.only.supported.for.images";

        public const String FilterIsNotANameOrArray = "filter.is.not.a.name.or.array";

        public const String FlushedPageCannotBeAddedOrInserted = "flushed.page.cannot.be.added.or.inserted";

        public const String FontAndSizeMustBeSetBeforeWritingAnyText = "font.and.size.must.be.set.before.writing.any.text";

        public const String FontEmbeddingIssue = "font.embedding.issue";

        public const String FontSizeTooSmall = "font.size.too.small";

        public const String FormXObjectMustHaveBbox = "form.xobject.must.have.bbox";

        public const String FunctionIsNotCompatibleWitColorSpace = "function.is.not.compatible.with.color.space";

        public const String GivenAccessibleElementIsNotConnectedToAnyTag = "given.accessible.element.is.not.connected.to.any.tag";

        public const String IllegalCharacterInAsciihexdecode = "illegal.character.in.asciihexdecode";

        public const String IllegalCharacterInAscii85decode = "illegal.character.in.ascii85decode";

        public const String IllegalLengthValue = "illegal.length.value";

        public const String IllegalPValue = "illegal.p.value";

        public const String IllegalRValue = "illegal.r.value";

        public const String IllegalVValue = "illegal.v.value";

        public const String InAPageLabelThePageNumbersMustBeGreaterOrEqualTo1 = "in.a.page.label.the.page.numbers.must.be.greater.or.equal.to.1";

        public const String InvalidHttpResponse1 = "invalid.http.response {0}";

        public const String InvalidTsa1ResponseCode2 = "invalid.tsa {0} response.code {1}";

        public const String IncorrectNumberOfComponents = "incorrect.number.of.components";

        public const String InlineLevelOrIllustrationElementCannotContainKids = "inline.level.or.illustration.element.cannot.contain.kids";

        public const String InvalidCodewordSize = "invalid.codeword.size";

        public const String InvalidCrossReferenceEntryInThisXrefSubsection = "invalid.cross.reference.entry.in.this.xref.subsection";

        public const String InvalidIndirectReference1 = "invalid.indirect.reference {0}";

        public const String InvalidMediaBoxValue = "Tne media box object has incorrect values";

        public const String InvalidPageStructure1 = "invalid.page.structure {0}";

        public const String InvalidPageStructurePagesPagesMustBePdfDictionary = "invalid.page.structure.pages.must.be.pdfdictionary";

        public const String InvalidRangeArray = "invalid.range.array";

        public const String InvalidOffsetForObject1 = "invalid.offset.for.object {0}";

        public const String InvalidXrefStream = "invalid.xref.stream";

        public const String InvalidXrefTable = "invalid.xref.table";

        public const String IoException = "io.exception";

        public const String IsNotAnAcceptableValueForTheField = "{0}.is.not.an.acceptable.value.for.the.field.{1}";

        public const String IsNotWmfImage = "is.not.wmf.image";

        public const String LzwDecoderException = "lzw.decoder.exception";

        public const String LzwFlavourNotSupported = "lzw.flavour.not.supported";

        public const String MacroSegmentIdMustBeGtOrEqZero = "macrosegmentid.must.be.gt.eq.0";

        public const String MacroSegmentIdMustBeGtZero = "macrosegmentid.must.be.gt.0";

        public const String MacroSegmentIdMustBeLtMacroSegmentCount = "macrosegmentid.must.be.lt.macrosegmentcount";

        public const String MustBeATaggedDocument = "must.be.a.tagged.document";

        public const String NumberOfEntriesInThisXrefSubsectionNotFound = "number.of.entries.in.this.xref.subsection.not.found";

        public const String NameAlreadyExistsInTheNameTree = "name.already.exist.in.the.name.tree";

        public const String NoCompatibleEncryptionFound = "no.compatible.encryption.found";

        public const String NoCryptoDictionaryDefined = "no.crypto.dictionary.defined";

        public const String NoKidWithSuchRole = "no.kid.with.such.role";

        public const String NotAPlaceableWindowsMetafile = "not.a.placeable.windows.metafile";

        public const String NotAValidPkcs7ObjectNotASequence = "not.a.valid.pkcs.7.object.not.a.sequence";

        public const String NotAValidPkcs7ObjectNotSignedData = "not.a.valid.pkcs.7.object.not.signed.data";

        public const String NoValidEncryptionMode = "no.valid.encryption.mode";

        public const String ObjectMustBeIndirectToWorkWithThisWrapper = "object.must.be.indirect.to.work.with.this.wrapper";

        public const String ObjectNumberOfTheFirstObjectInThisXrefSubsectionNotFound = "object.number.of.the.first.object.in.this.xref.subsection.not.found";

        public const String OcspStatusIsRevoked = "ocsp.status.is.revoked";

        public const String OcspStatusIsUnknown = "ocsp.status.is.unknown";

        public const String OnlyBmpCanBeWrappedInWmf = "only.bmp.can.be.wrapped.in.wmf";

        public const String OperatorEINotFoundAfterEndOfImageData = "operator.EI.not.found.after.end.of.image.data";

        public const String Page1CannotBeAddedToDocument2BecauseItBelongsToDocument3 = "page {0} cannot.be.added.to.document {1} because.it.belongs.to.document {2}";

        public const String PageIsNotSetForThePdfTagStructure = "page.is.not.set.for.the.pdf.tag.structure";

        public const String PageWasAlreadyFlushed = "the.page.was.already.flushed";

        public const String PageWasAlreadyFlushedUseAddFieldAppearanceToPageMethodBeforePageFlushing = "the.page.was.already.flushed.use.add.field.appearance.to.page.method.before.page.flushing";

        public const String PdfEncodings = "pdf.encodings";

        public const String PdfEncryption = "pdf.encryption";

        public const String PdfDecryption = "Exception occurred with pdf document decryption. One of the possible reasons is wrong password or wrong public key certificate and private key.";

        public const String PdfDocumentMustBeOpenedInStampingMode = "pdf.document.must.be.opened.in.stamping.mode";

        public const String PdfFormXobjectHasInvalidBbox = "pdf.form.xobject.has.invalid.bbox";

        public const String PdfObjectStreamReachMaxSize = "pdf.object.stream.reach.max.size";

        public const String PdfPageShallHaveContent = "pdf.page.shall.have.content";

        public const String PdfPagesTreeCouldBeGeneratedOnlyOnce = "pdf.pages.tree.could.be.generated.only.once";

        public const String PdfStartxrefIsNotFollowedByANumber = "pdf.startxref.is.not.followed.by.a.number";

        public const String PdfStartxrefNotFound = "pdf.startxref.not.found";

        [System.ObsoleteAttribute(@"Will be removed in iText 7.1.0. There is a typo in the name of the constant. Use PdfIndirectObjectBelongToOtherPdfDocument instead."
            )]
        public const String PdfInderectObjectBelongToOtherPdfDocument = "pdf.inderect.object.belong.to.other.pdf.document.Copy.object.to.current.pdf.document";

        public const String PdfIndirectObjectBelongToOtherPdfDocument = "pdf.indirect.object.belong.to.other.pdf.document.Copy.object.to.current.pdf.document";

        public const String PdfVersionNotValid = "pdf.version.not.valid";

        public const String PngFilterUnknown = "png.filter.unknown";

        public const String ResourcesCannotBeNull = "resources.cannot.be.null";

        public const String ResourcesDoNotContainExtgstateEntryUnableToProcessOperator1 = "resources.do.not.contain.extgstate.entry.unable.to.process.operator {0}";

        public const String RoleIsNotMappedWithAnyStandardRole = "role.is.not.mapped.with.any.standard.role";

        public const String ShadingTypeNotFound = "shading.type.not.found";

        public const String SignatureWithName1IsNotTheLastItDoesntCoverWholeDocument = "signature.with.name.1.is.not.the.last.it.doesnt.cover.whole.document";

        public const String StdcfNotFoundEncryption = "stdcf.not.found.encryption";

        public const String StructParentIndexNotFoundInTaggedObject = "struct.parent.index.not.found.in.tagged.object";

        public const String StructureElementShallContainParentObject = "structure.element.shall.contain.parent.object";

        public const String TagCannotBeMovedToTheAnotherDocumentsTagStructure = "tag.cannot.be.moved.to.the.another.documents.tag.structure";

        public const String TagFromTheExistingTagStructureIsFlushedCannotAddCopiedPageTags = "tag.from.the.existing.tag.structure.is.flushed.cannot.add.copied.page.tags";

        public const String TagStructureCopyingFailedItMightBeCorruptedInOneOfTheDocuments = "Tag structure copying failed: it might be corrupted in one of the documents.";

        public const String TagStructureFlushingFailedItMightBeCorrupted = "Tag structure flushing failed: it might be corrupted.";

        public const String TagTreePointerIsInInvalidStateItPointsAtFlushedElementUseMoveToRoot = "tagtreepointer.is.in.invalid.state.it.points.at.flushed.element.use.movetoroot";

        public const String TagTreePointerIsInInvalidStateItPointsAtRemovedElementUseMoveToRoot = "tagtreepointer.is.in.invalid.state.it.points.at.removed.element.use.movetoroot";

        public const String TextCannotBeNull = "text.cannot.be.null";

        public const String TextIsTooBig = "text.is.too.big";

        public const String TextMustBeEven = "the.text.length.must.be.even";

        public const String TwoBarcodeMustBeExternally = "the.two.barcodes.must.be.composed.externally";

        public const String TheNumberOfBooleansInTheArrayDoesntCorrespondWithTheNumberOfFields = "the.number.of.booleans.in.the.array.doesn.t.correspond.with.the.number.of.fields";

        public const String ThereAreIllegalCharactersForBarcode128In1 = "there.are.illegal.characters.for.barcode.128.in {0}";

        public const String ThereIsNoAssociatePdfWriterForMakingIndirects = "there.is.no.associate.pdf.writer.for.making.indirects";

        public const String ThereIsNoFieldInTheDocumentWithSuchName1 = "there.is.no.field.in.the.document.with.such.name {0}";

        public const String ThisPkcs7ObjectHasMultipleSignerinfosOnlyOneIsSupportedAtThisTime = "this.pkcs.7.object.has.multiple.signerinfos.only.one.is.supported.at.this.time";

        public const String ThisInstanceOfPdfSignerIsAlreadyClosed = "this.instance.of.PdfSigner.is.already.closed";

        public const String Tsa1FailedToReturnTimeStampToken2 = "tsa {0} failed.to.return.time.stamp.token {1}";

        public const String TrailerNotFound = "trailer.not.found";

        public const String TrailerPrevEntryPointsToItsOwnCrossReferenceSection = "trailer.prev.entry.points.to.its.own.cross.reference.section";

        public const String UnbalancedBeginEndMarkedContentOperators = "unbalanced.begin.end.marked.content.operators";

        public const String UnbalancedLayerOperators = "unbalanced.layer.operators";

        public const String UnbalancedSaveRestoreStateOperators = "unbalanced.save.restore.state.operators";

        public const String UnexpectedCharacter1FoundAfterIDInInlineImage = "unexpected.character.1.found.after.ID.in.inline.image";

        public const String UnexpectedCloseBracket = "unexpected.close.bracket";

        public const String UnexpectedColorSpace1 = "unexpected.color.space {0}";

        public const String UnexpectedEndOfFile = "unexpected.end.of.file";

        public const String UnexpectedGtGt = "unexpected.gt.gt";

        public const String UnexpectedShadingType = "unexpected.shading.type";

        public const String UnknownEncryptionTypeREq1 = "unknown.encryption.type.r.eq {0}";

        public const String UnknownEncryptionTypeVEq1 = "unknown.encryption.type.v.eq {0}";

        public const String UnknownPdfException = "unknown.pdf.exception";

        public const String UnknownHashAlgorithm1 = "unknown.hash.algorithm {0}";

        public const String UnknownKeyAlgorithm1 = "unknown.key.algorithm {0}";

        public const String UnknownColorFormatMustBeRGBorRRGGBB = "unknown.color.format.must.be.rgb.or.rrggbb";

        public const String UnsupportedXObjectType = "Unsupported XObject type";

        public const String VerificationAlreadyOutput = "verification.already.output";

        public const String WhenAddingObjectReferenceToTheTagTreeItMustBeConnectedToNotFlushedObject = "when.adding.object.reference.to.the.tag.tree.it.must.be.connected.to.not.flushed.object";

        public const String WhitePointIsIncorrectlySpecified = "white.point.is.incorrectly.specified";

        public const String WmfImageException = "wmf.image.exception";

        public const String WrongFormFieldAddAnnotationToTheField = "wrong.form.field.add.annotation.to.the.field";

        public const String WrongMediaBoxSize1 = "Wrong media box size: {0}";

        public const String XrefSubsectionNotFound = "xref.subsection.not.found";

        public const String YouCannotFlushPdfCatalogManually = "you.cannot.flush.pdf.catalog.manually";

        public const String YouHaveToDefineABooleanArrayForThisCollectionSortDictionary = "you.have.to.define.a.boolean.array.for.this.collection.sort.dictionary";

        public const String YouMustSetAValueBeforeAddingAPrefix = "you.must.set.a.value.before.adding.a.prefix";

        public const String YouNeedASingleBooleanForThisCollectionSortDictionary = "you.need.a.single.boolean.for.this.collection.sort.dictionary";

        protected internal Object @object;

        private IList<Object> messageParams;

        public PdfException(String message)
            : base(message) {
        }

        public PdfException(Exception cause)
            : this(UnknownPdfException, cause) {
        }

        public PdfException(String message, Object @object)
            : this(message) {
            this.@object = @object;
        }

        public PdfException(String message, Exception cause)
            : base(message, cause) {
        }

        public PdfException(String message, Exception cause, Object @object)
            : this(message, cause) {
            this.@object = @object;
        }

        public override String Message {
            get {
                if (messageParams == null || messageParams.Count == 0) {
                    return base.Message;
                }
                else {
                    return String.Format(base.Message, GetMessageParams());
                }
            }
        }

        public virtual iText.Kernel.PdfException SetMessageParams(params Object[] messageParams) {
            this.messageParams = new List<Object>();
            this.messageParams.AddAll(messageParams);
            return this;
        }

        protected internal virtual Object[] GetMessageParams() {
            Object[] parameters = new Object[messageParams.Count];
            for (int i = 0; i < messageParams.Count; i++) {
                parameters[i] = messageParams[i];
            }
            return parameters;
        }
    }
}
