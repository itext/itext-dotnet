/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
    /// <summary>Exception class for exceptions in kernel module.</summary>
    public class PdfException : Exception {
        public const String _1IsAnUnknownGraphicsStateDictionary = "{0} is an unknown graphics state dictionary.";

        public const String _1IsNotAnAcceptableValueForTheField2 = "{0} is not an acceptable value for the field {1}.";

        public const String _1IsNotAValidPlaceableWindowsMetafile = "{0} is not a valid placeable windows metafile.";

        [Obsolete]
        public const String AnnotShallHaveReferenceToPage = "Annotation shall have reference to page.";

        public const String AnnotationShallHaveReferenceToPage = "Annotation shall have reference to page.";

        public const String AppendModeRequiresADocumentWithoutErrorsEvenIfRecoveryWasPossible = "Append mode requires a document without errors, even if recovery is possible.";

        public const String AuthenticatedAttributeIsMissingTheDigest = "Authenticated attribute is missing the digest.";

        public const String AvailableSpaceIsNotEnoughForSignature = "Available space is not enough for signature.";

        public const String BadCertificateAndKey = "Bad public key certificate and/or private key.";

        public const String BadUserPassword = "Bad user password. Password is not provided or wrong password provided. Correct password should be passed to PdfReader constructor with properties. See ReaderProperties#setPassword() method.";

        public const String CannotAddKidToTheFlushedElement = "Cannot add kid to the flushed element.";

        public const String CannotAddNonDictionaryExtGStateToResources1 = "Cannot add graphic state to resources. The PdfObject type is {0}, but should be PdfDictionary.";

        public const String CannotAddNonDictionaryPatternToResources1 = "Cannot add pattern to resources. The PdfObject type is {0}, but should be PdfDictionary or PdfStream.";

        public const String CannotAddNonDictionaryPropertiesToResources1 = "Cannot add properties to resources. The PdfObject type is {0}, but should be PdfDictionary.";

        public const String CannotAddNonDictionaryShadingToResources1 = "Cannot add shading to resources. The PdfObject type is {0}, but should be PdfDictionary or PdfStream.";

        public const String CannotAddNonStreamFormToResources1 = "Cannot add form to resources. The PdfObject type is {0}, but should be PdfStream.";

        public const String CannotAddNonStreamImageToResources1 = "Cannot add image to resources. The PdfObject type is {0}, but should be PdfStream.";

        public const String CannotBeEmbeddedDueToLicensingRestrictions = "{0} cannot be embedded due to licensing restrictions.";

        public const String CannotCloseDocument = "Cannot close document.";

        public const String CannotCloseDocumentWithAlreadyFlushedPdfCatalog = "Cannot close document with already flushed PDF Catalog.";

        public const String CannotConvertPdfArrayToDoubleArray = "Cannot convert PdfArray to an array of doubles.";

        public const String CannotConvertPdfArrayToIntArray = "Cannot convert PdfArray to an array of integers.";

        public const String CannotConvertPdfArrayToFloatArray = "Cannot convert PdfArray to an array of floats.";

        public const String CannotConvertPdfArrayToLongArray = "Cannot convert PdfArray to an array of longs.";

        public const String CannotConvertPdfArrayToRectanle = "Cannot convert PdfArray to Rectangle.";

        public const String CannotCopyFlushedObject = "Cannot copy flushed object.";

        public const String CannotCopyFlushedTag = "Cannot copy flushed tag.";

        public const String CannotCopyObjectContent = "Cannot copy object content.";

        public const String CannotCopyIndirectObjectFromTheDocumentThatIsBeingWritten = "Cannot copy indirect object from the document that is being written.";

        public const String CannotCopyToDocumentOpenedInReadingMode = "Cannot copy to document opened in reading mode.";

        public const String CannotCreateLayoutImageByWmfImage = "Cannot create layout image by WmfImage instance. First convert the image into FormXObject and then use the corresponding layout image constructor.";

        public const String CannotCreatePdfImageXObjectByWmfImage = "Cannot create PdfImageXObject instance by WmfImage. Use PdfFormXObject constructor instead.";

        public const String CannotCreatePdfStreamByInputStreamWithoutPdfDocument = "Cannot create pdfstream by InputStream without PdfDocument.";

        public const String CannotGetContentBytes = "Cannot get content bytes.";

        public const String CannotGetPdfStreamBytes = "Cannot get PdfStream bytes.";

        public const String CannotOperateWithFlushedPdfStream = "Cannot operate with the flushed PdfStream.";

        public const String CannotRetrieveMediaBoxAttribute = "Invalid PDF. There is no media box attribute for page or its parents.";

        public const String CannotFindImageDataOrEI = "Cannot find image data or EI.";

        public const String CannotFlushDocumentRootTagBeforeDocumentIsClosed = "Cannot flush document root tag before document is closed.";

        public const String CannotFlushObject = "Cannot flush object.";

        public const String CannotMoveToFlushedKid = "Cannot move to flushed kid.";

        public const String CannotMoveToMarkedContentReference = "Cannot move to marked content reference.";

        public const String CannotMoveToParentCurrentElementIsRoot = "Cannot move to parent current element is root.";

        public const String CannotOpenDocument = "Cannot open document.";

        public const String CannotParseContentStream = "Cannot parse content stream.";

        public const String CannotReadAStreamInOrderToAppendNewBytes = "Cannot read a stream in order to append new bytes.";

        public const String CannotReadPdfObject = "Cannot read PdfObject.";

        public const String CannotRecogniseDocumentFontWithEncoding = "Cannot recognise document font {0} with {1} encoding";

        public const String CannotRemoveDocumentRootTag = "Cannot remove document root tag.";

        public const String CannotRemoveMarkedContentReferenceBecauseItsPageWasAlreadyFlushed = "Cannot remove marked content reference, because its page has been already flushed.";

        public const String CannotRemoveTagBecauseItsParentIsFlushed = "Cannot remove tag, because its parent is flushed.";

        [Obsolete]
        public const String CannotSetDataToPdfstreamWhichWasCreatedByInputstream = "Cannot set data to PdfStream which was created by InputStream.";

        public const String CannotSetDataToPdfstreamWhichWasCreatedByInputStream = "Cannot set data to PdfStream which was created by InputStream.";

        public const String CannotSplitDocumentThatIsBeingWritten = "Cannot split document that is being written.";

        [Obsolete]
        public const String CannotWritePdfStream = "Cannot write pdf stream.";

        public const String CannotWriteToPdfStream = "Cannot write to PdfStream.";

        public const String CannotWriteObjectAfterItWasReleased = "Cannot write object after it was released. In normal situation the object must be read once again before being written.";

        [Obsolete]
        public const String CantDecodePkcs7SigneddataObject = "Cannot decode PKCS#7 SignedData object.";

        public const String CannotDecodePkcs7SigneddataObject = "Cannot decode PKCS#7 SignedData object.";

        [Obsolete]
        public const String CantFindSigningCertificateWithSerial1 = "Cannot find signing certificate with serial {0}.";

        public const String CannotFindSigningCertificateWithSerial1 = "Cannot find signing certificate with serial {0}.";

        public const String CertificateIsNotProvidedDocumentIsEncryptedWithPublicKeyCertificate = "Certificate is not provided. Document is encrypted with public key certificate, it should be passed to PdfReader constructor with properties. See ReaderProperties#setPublicKeySecurityParams() method.";

        public const String CfNotFoundEncryption = "/CF not found (encryption)";

        [Obsolete]
        public const String CodabarCharacterOneIsIllegal = "the character {0} is illegal in codabar.";

        [Obsolete]
        public const String CodabarMustHaveAtLeastAStartAndStopCharacter = "Codabar must have at least start and stop character.";

        public const String CodabarMustHaveAtLeastStartAndStopCharacter = "Codabar must have at least start and stop character.";

        public const String CodabarMustHaveOneAbcdAsStartStopCharacter = "Codabar must have one of 'ABCD' as start/stop character.";

        [Obsolete]
        public const String CodabarStartStopCharacterAreOnlyExtremes = "In codabar, start/stop characters are only allowed at the extremes.";

        [Obsolete]
        public const String ColorNotFound = "Color not found.";

        public const String ColorSpaceNotFound = "ColorSpace not found.";

        public const String ContentStreamMustNotInvokeOperatorsThatSpecifyColorsOrOtherColorRelatedParameters = "Content stream must not invoke operators that specify colors or other color related parameters in the graphics state.";

        public const String DecodeParameterType1IsNotSupported = "Decode parameter type {0} is not supported.";

        public const String DefaultcryptfilterNotFoundEncryption = "/DefaultCryptFilter not found (encryption).";

        public const String DictionaryKey1IsNotAName = "Dictionary key {0} is not a name.";

        [Obsolete]
        public const String DictionaryNotContainFontData = "Dictionary doesn't have font data.";

        public const String DictionaryDoesntHave1FontData = "Dictionary doesn't have {0} font data.";

        public const String DictionaryDoesntHaveSupportedFontData = "Dictionary doesn't have supported font data.";

        public const String DocumentAlreadyPreClosed = "Document has been already pre closed.";

        [Obsolete]
        public const String DocumentClosedImpossibleExecuteAction = "Document was closed. It is impossible to execute action.";

        public const String DocumentClosedItIsImpossibleToExecuteAction = "Document was closed. It is impossible to execute action.";

        public const String DocumentDoesntContainStructTreeRoot = "Document doesn't contain StructTreeRoot.";

        public const String DocumentHasNoPages = "Document has no pages.";

        [Obsolete]
        public const String DocumentHasNoCatalogObject = "Document has no PDF Catalog object.";

        public const String DocumentHasNoPdfCatalogObject = "Document has no PDF Catalog object.";

        [Obsolete]
        public const String DocumentMustBePreclosed = "Document must be preClosed.";

        public const String DocumentMustBePreClosed = "Document must be preClosed.";

        [Obsolete]
        public const String DocumentToCopyToCannotBeNull = "Document for copyTo cannot be null.";

        public const String DocumentForCopyToCannotBeNull = "Document for copyTo cannot be null.";

        public const String ElementCannotFitAnyArea = "Element cannot fit any area.";

        public const String EncryptionCanOnlyBeAddedBeforeOpeningDocument = "Encryption can only be added before opening the document.";

        public const String EndOfContentStreamReachedBeforeEndOfImageData = "End of content stream reached before end of image data.";

        [Obsolete]
        public const String ErrorReadingObjectStream = "Error while reading Object Stream.";

        public const String ErrorWhileReadingObjectStream = "Error while reading Object Stream.";

        public const String FailedToGetTsaResponseFrom1 = "Failed to get TSA response from {0}.";

        public const String FieldFlatteningIsNotSupportedInAppendMode = "Field flattening is not supported in append mode.";

        [Obsolete]
        public const String FieldIsAlreadySigned = "Field has been already signed.";

        public const String FieldAlreadySigned = "Field has been already signed.";

        public const String FieldNamesCannotContainADot = "Field names cannot contain a dot.";

        public const String FieldTypeIsNotASignatureFieldType = "Field type is not a signature field type.";

        public const String Filter1IsNotSupported = "Filter {0} is not supported.";

        [Obsolete]
        public const String FilePosition0CrossReferenceEntryInThisXrefSubsection = "file position {0} cross reference entry in this xref subsection.";

        public const String FilePosition1CrossReferenceEntryInThisXrefSubsection = "file position {0} cross reference entry in this xref subsection.";

        public const String FilterCcittfaxdecodeIsOnlySupportedForImages = "Filter CCITTFaxDecode is only supported for images";

        public const String FilterIsNotANameOrArray = "filter is not a name or array.";

        public const String FlushedPageCannotBeAddedOrInserted = "Flushed page cannot be added or inserted.";

        public const String FontAndSizeMustBeSetBeforeWritingAnyText = "Font and size must be set before writing any text.";

        public const String FontEmbeddingIssue = "Font embedding issue.";

        [Obsolete]
        public const String FontSizeTooSmall = "Font size is too small.";

        public const String FontSizeIsTooSmall = "Font size is too small.";

        public const String FormXObjectMustHaveBbox = "Form XObject must have BBox.";

        public const String FunctionIsNotCompatibleWitColorSpace = "Function is not compatible with ColorSpace.";

        public const String GivenAccessibleElementIsNotConnectedToAnyTag = "Given accessible element is not connected to any tag.";

        public const String IllegalCharacterInAsciihexdecode = "illegal character in ASCIIHexDecode.";

        public const String IllegalCharacterInAscii85decode = "Illegal character in ASCII85Decode.";

        public const String IllegalCharacterInCodabarBarcode = "Illegal character in Codabar Barcode.";

        public const String IllegalLengthValue = "Illegal length value.";

        public const String IllegalPValue = "Illegal P value.";

        public const String IllegalRValue = "Illegal R value.";

        public const String IllegalVValue = "Illegal V value.";

        public const String InAPageLabelThePageNumbersMustBeGreaterOrEqualTo1 = "In a page label the page numbers must be greater or equal to 1.";

        public const String InCodabarStartStopCharactersAreOnlyAllowedAtTheExtremes = "In Codabar, start/stop characters are only allowed at the extremes.";

        public const String InvalidHttpResponse1 = "Invalid http response {0}.";

        public const String InvalidTsa1ResponseCode2 = "Invalid TSA {0} response code {1}.";

        public const String IncorrectNumberOfComponents = "Incorrect number of components.";

        public const String InlineLevelOrIllustrationElementCannotContainKids = "Inline level or illustration element cannot contain kids.";

        public const String InvalidCodewordSize = "Invalid codeword size.";

        public const String InvalidCrossReferenceEntryInThisXrefSubsection = "Invalid cross reference entry in this xref subsection.";

        public const String InvalidIndirectReference1 = "Invalid indirect reference {0}.";

        public const String InvalidMediaBoxValue = "Tne media box object has incorrect values.";

        public const String InvalidPageStructure1 = "Invalid page structure {0}.";

        public const String InvalidPageStructurePagesPagesMustBePdfDictionary = "Invalid page structure. /Pages must be PdfDictionary.";

        public const String InvalidRangeArray = "Invalid range array.";

        public const String InvalidOffsetForObject1 = "Invalid offset for object {0}.";

        public const String InvalidXrefStream = "Invalid xref stream.";

        public const String InvalidXrefTable = "Invalid xref table.";

        public const String IoException = "I/O exception.";

        public const String IoExceptionWhileCreatingFont = "I/O exception while creating Font";

        [Obsolete]
        public const String IsNotAnAcceptableValueForTheField = "{0} is not an acceptable value for the field {1}.";

        [Obsolete]
        public const String IsNotWmfImage = "Not a WMF image.";

        public const String LzwDecoderException = "LZW decoder exception.";

        public const String LzwFlavourNotSupported = "LZW flavour not supported.";

        public const String MacroSegmentIdMustBeGtOrEqZero = "macroSegmentId must be >= 0";

        public const String MacroSegmentIdMustBeGtZero = "macroSegmentId must be > 0";

        public const String MacroSegmentIdMustBeLtMacroSegmentCount = "macroSegmentId must be < macroSemgentCount";

        public const String MustBeATaggedDocument = "Must be a tagged document.";

        public const String NumberOfEntriesInThisXrefSubsectionNotFound = "Number of entries in this xref subsection not found.";

        [Obsolete]
        public const String NameAlreadyExistsInTheNameTree = "Name already exists in the name tree.";

        public const String NoCompatibleEncryptionFound = "No compatible encryption found.";

        public const String NoCryptoDictionaryDefined = "No crypto dictionary defined.";

        public const String NoKidWithSuchRole = "No kid with such role.";

        public const String NoMaxLenPresent = "No /MaxLen has been set even though the Comb flag has been set.";

        public const String NotAPlaceableWindowsMetafile = "Not a placeable windows metafile.";

        public const String NotAValidPkcs7ObjectNotASequence = "Not a valid PKCS#7 object - not a sequence";

        public const String NotAValidPkcs7ObjectNotSignedData = "Not a valid PKCS#7 object - not signed data.";

        public const String NotAWmfImage = "Not a WMF image.";

        public const String NoValidEncryptionMode = "No valid encryption mode.";

        public const String NumberOfBooleansInTheArrayDoesntCorrespondWithTheNumberOfFields = "The number of booleans in the array doesn't correspond with the number of fields.";

        public const String ObjectMustBeIndirectToWorkWithThisWrapper = "Object must be indirect to work with this wrapper.";

        public const String ObjectNumberOfTheFirstObjectInThisXrefSubsectionNotFound = "Object number of the first object in this xref subsection not found.";

        public const String OnlyIdentityCMapsSupportsWithTrueType = "Only Identity CMaps supports with truetype";

        public const String OcspStatusIsRevoked = "OCSP status is revoked.";

        public const String OcspStatusIsUnknown = "OCSP status is unknown.";

        public const String OnlyBmpCanBeWrappedInWmf = "Only BMP can be wrapped in WMF.";

        public const String OperatorEINotFoundAfterEndOfImageData = "Operator EI not found after the end of image data.";

        public const String Page1CannotBeAddedToDocument2BecauseItBelongsToDocument3 = "Page {0} cannot be added to document {1}, because it belongs to document {2}.";

        public const String PageIsNotSetForThePdfTagStructure = "Page is not set for the pdf tag structure.";

        [Obsolete]
        public const String PageWasAlreadyFlushed = "The page has been already flushed.";

        public const String PageAlreadyFlushed = "The page has been already flushed.";

        [Obsolete]
        public const String PageWasAlreadyFlushedUseAddFieldAppearanceToPageMethodBeforePageFlushing = "The page has been already flushed. Use PdfAcroForm#addFieldAppearanceToPage() method before page flushing.";

        public const String PageAlreadyFlushedUseAddFieldAppearanceToPageMethodBeforePageFlushing = "The page has been already flushed. Use PdfAcroForm#addFieldAppearanceToPage() method before page flushing.";

        public const String PdfEncodings = "PdfEncodings exception.";

        public const String PdfEncryption = "PdfEncryption exception.";

        public const String PdfDecryption = "Exception occurred with PDF document decryption. One of the possible reasons is wrong password or wrong public key certificate and private key.";

        public const String PdfDocumentMustBeOpenedInStampingMode = "PdfDocument must be opened in stamping mode.";

        public const String PdfFormXobjectHasInvalidBbox = "PdfFormXObject has invalid BBox.";

        public const String PdfObjectStreamReachMaxSize = "PdfObjectStream reach max size.";

        [Obsolete]
        public const String PdfPageShallHaveContent = "PdfPage shall have content.";

        public const String PdfPagesTreeCouldBeGeneratedOnlyOnce = "PdfPages tree could be generated only once.";

        public const String PdfStartxrefIsNotFollowedByANumber = "PDF startxref is not followed by a number.";

        public const String PdfStartxrefNotFound = "PDF startxref not found.";

        [System.ObsoleteAttribute(@"Will be removed in iText 7.1.0. <p> There is a typo in the name of the constant. Use PdfIndirectObjectBelongsToOtherPdfDocument instead."
            )]
        public const String PdfInderectObjectBelongToOtherPdfDocument = "pdf inderect object belong to other pdf document Copy object to current pdf document.";

        public const String PdfIndirectObjectBelongsToOtherPdfDocument = "Pdf indirect object belongs to other PDF document. Copy object to current pdf document.";

        public const String PdfVersionNotValid = "PDF version is not valid.";

        public const String RequestedPageNumberIsOutOfBounds = "Requested page number {0} is out of bounds.";

        public const String PngFilterUnknown = "PNG filter unknown.";

        public const String ResourcesCannotBeNull = "Resources cannot be null.";

        public const String ResourcesDoNotContainExtgstateEntryUnableToProcessOperator1 = "Resources do not contain ExtGState entry. Unable to process operator {0}.";

        public const String RoleIsNotMappedWithAnyStandardRole = "Role is not mapped with any standard role.";

        public const String ShadingTypeNotFound = "Shading type not found.";

        public const String SignatureWithName1IsNotTheLastItDoesntCoverWholeDocument = "Signature with name {0} is not the last. It doesn't cover the whole document.";

        public const String StdcfNotFoundEncryption = "/StdCF not found (encryption)";

        public const String StructParentIndexNotFoundInTaggedObject = "StructParent index not found in tagged object.";

        public const String StructureElementShallContainParentObject = "StructureElement shall contain parent object.";

        public const String TagCannotBeMovedToTheAnotherDocumentsTagStructure = "Tag cannot be moved to the another document's tag structure.";

        public const String TagFromTheExistingTagStructureIsFlushedCannotAddCopiedPageTags = "Tag from the existing tag structure is flushed. Cannot add copied page tags.";

        public const String TagStructureCopyingFailedItMightBeCorruptedInOneOfTheDocuments = "Tag structure copying failed: it might be corrupted in one of the documents.";

        public const String TagStructureFlushingFailedItMightBeCorrupted = "Tag structure flushing failed: it might be corrupted.";

        public const String TagTreePointerIsInInvalidStateItPointsAtFlushedElementUseMoveToRoot = "TagTreePointer is in invalid state: it points at flushed element. Use TagTreePointer#moveToRoot.";

        public const String TagTreePointerIsInInvalidStateItPointsAtRemovedElementUseMoveToRoot = "TagTreePointer is in invalid state: it points at removed element use TagTreePointer#moveToRoot.";

        public const String TextCannotBeNull = "Text cannot be null.";

        public const String TextIsTooBig = "Text is too big.";

        public const String TextMustBeEven = "The text length must be even.";

        public const String TwoBarcodeMustBeExternally = "The two barcodes must be composed externally.";

        [Obsolete]
        public const String TheNumberOfBooleansInTheArrayDoesntCorrespondWithTheNumberOfFields = "The number of booleans in the array doesn't correspond with the number of fields.";

        public const String ThereAreIllegalCharactersForBarcode128In1 = "There are illegal characters for barcode 128 in {0}.";

        public const String ThereIsNoAssociatePdfWriterForMakingIndirects = "There is no associate PdfWriter for making indirects.";

        public const String ThereIsNoFieldInTheDocumentWithSuchName1 = "There is no field in the document with such name: {0}.";

        public const String ThisPkcs7ObjectHasMultipleSignerinfosOnlyOneIsSupportedAtThisTime = "This PKCS#7 object has multiple SignerInfos. Only one is supported at this time.";

        [Obsolete]
        public const String ThisInstanceOfPdfSignerIsAlreadyClosed = "This instance of PdfSigner has been already closed.";

        public const String ThisInstanceOfPdfSignerAlreadyClosed = "This instance of PdfSigner has been already closed.";

        public const String ToFlushThisWrapperUnderlyingObjectMustBeAddedToDocument = "To manually flush this wrapper, you have to ensure that the object behind this wrapper is added to the document, i.e. it has an indirect reference.";

        public const String Tsa1FailedToReturnTimeStampToken2 = "TSA {0} failed to return time stamp token: {1}.";

        public const String TrailerNotFound = "Trailer not found.";

        public const String TrailerPrevEntryPointsToItsOwnCrossReferenceSection = "Trailer prev entry points to its own cross reference section.";

        public const String UnbalancedBeginEndMarkedContentOperators = "Unbalanced begin/end marked content operators.";

        public const String UnbalancedLayerOperators = "Unbalanced layer operators.";

        public const String UnbalancedSaveRestoreStateOperators = "Unbalanced save restore state operators.";

        public const String UnexpectedCharacter1FoundAfterIDInInlineImage = "Unexpected character {0} found after ID in inline image.";

        public const String UnexpectedCloseBracket = "Unexpected close bracket.";

        public const String UnexpectedColorSpace1 = "Unexpected ColorSpace: {0}.";

        public const String UnexpectedEndOfFile = "Unexpected end of file.";

        public const String UnexpectedGtGt = "unexpected >>.";

        public const String UnexpectedShadingType = "Unexpected shading type.";

        public const String UnknownEncryptionTypeREq1 = "Unknown encryption type R == {0}.";

        public const String UnknownEncryptionTypeVEq1 = "Unknown encryption type V == {0}.";

        public const String UnknownPdfException = "Unknown PdfException.";

        public const String UnknownHashAlgorithm1 = "Unknown hash algorithm: {0}.";

        public const String UnknownKeyAlgorithm1 = "Unknown key algorithm: {0}.";

        [Obsolete]
        public const String UnknownColorFormatMustBeRGBorRRGGBB = "Unknown color format: must be rgb or rrggbb.";

        public const String UnsupportedDefaultColorSpaceName1 = "Unsupported default color space name. Was {0}, but should be DefaultCMYK, DefaultGray or DefaultRGB";

        public const String UnsupportedXObjectType = "Unsupported XObject type.";

        public const String VerificationAlreadyOutput = "Verification already output.";

        public const String WhenAddingObjectReferenceToTheTagTreeItMustBeConnectedToNotFlushedObject = "When adding object reference to the tag tree, it must be connected to not flushed object.";

        public const String WhitePointIsIncorrectlySpecified = "White point is incorrectly specified.";

        public const String WmfImageException = "WMF image exception.";

        public const String WrongFormFieldAddAnnotationToTheField = "Wrong form field. Add annotation to the field.";

        public const String WrongMediaBoxSize1 = "Wrong media box size: {0}.";

        public const String XrefSubsectionNotFound = "xref subsection not found.";

        [Obsolete]
        public const String YouCannotFlushPdfCatalogManually = "You cannot flush PdfCatalog manually.";

        public const String YouHaveToDefineABooleanArrayForThisCollectionSortDictionary = "You have to define a boolean array for this collection sort dictionary.";

        public const String YouMustSetAValueBeforeAddingAPrefix = "You must set a value before adding a prefix.";

        public const String YouNeedASingleBooleanForThisCollectionSortDictionary = "You need a single boolean for this collection sort dictionary.";

        /// <summary>Object for more details</summary>
        protected internal Object @object;

        private IList<Object> messageParams;

        /// <summary>Creates a new instance of PdfException.</summary>
        /// <param name="message">the detail message.</param>
        public PdfException(String message)
            : base(message) {
        }

        /// <summary>Creates a new instance of PdfException.</summary>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public PdfException(Exception cause)
            : this(UnknownPdfException, cause) {
        }

        /// <summary>Creates a new instance of PdfException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="obj">an object for more details.</param>
        public PdfException(String message, Object obj)
            : this(message) {
            this.@object = obj;
        }

        /// <summary>Creates a new instance of PdfException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public PdfException(String message, Exception cause)
            : base(message, cause) {
        }

        /// <summary>Creates a new instance of PdfException.</summary>
        /// <param name="message">the detail message.</param>
        /// <param name="cause">
        /// the cause (which is saved for later retrieval by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        /// <param name="obj">an object for more details.</param>
        public PdfException(String message, Exception cause, Object obj)
            : this(message, cause) {
            this.@object = obj;
        }

        public override String Message {
            get {
                if (messageParams == null || messageParams.Count == 0) {
                    return base.Message;
                }
                else {
                    return MessageFormatUtil.Format(base.Message, GetMessageParams());
                }
            }
        }

        /// <summary>Sets additional params for Exception message.</summary>
        /// <param name="messageParams">additional params.</param>
        /// <returns>object itself.</returns>
        public virtual iText.Kernel.PdfException SetMessageParams(params Object[] messageParams) {
            this.messageParams = new List<Object>();
            this.messageParams.AddAll(messageParams);
            return this;
        }

        /// <summary>Gets additional params for Exception message.</summary>
        protected internal virtual Object[] GetMessageParams() {
            Object[] parameters = new Object[messageParams.Count];
            for (int i = 0; i < messageParams.Count; i++) {
                parameters[i] = messageParams[i];
            }
            return parameters;
        }
    }
}
