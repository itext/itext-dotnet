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
using iTextSharp.Kernel;

namespace iTextSharp.Pdfa
{
    /// <summary>Exception that is thrown when the PDF Document doesn't adhere to the PDF/A specification.
    ///     </summary>
    public class PdfAConformanceException : PdfException
    {
        public const String _1ActionsAreNotAllowed = "1.actions.are.not.allowed";

        public const String AllFontsMustBeEmbeddedThisOneIsnt1 = "all.the.fonts.must.be.embedded.this.one.isn.t.1";

        public const String AFormXobjectDictionaryShallNotContainOpiKey = "a.form.xobject.dictionary.shall.not.contain.opi.key";

        public const String AFormXobjectDictionaryShallNotContainPSKey = "a.form.xobject.dictionary.shall.not.contain.PS.key";

        public const String AFormXobjectDictionaryShallNotContainSubtype2KeyWithAValueOfPS
             = "a.form.xobject.dictionary.shall.not.contain.subtype2.key.with.a.value.of.PS";

        public const String AGroupObjectWithAnSKeyWithAValueOfTransparencyShallNotBeIncludedInAFormXobject
             = "a.group.object.with.an.s.key.with.a.value.of.transparency.shall.not.be.included.in.a.form.xobject";

        public const String AGroupObjectWithAnSKeyWithAValueOfTransparencyShallNotBeIncludedInAPageObject
             = "a.group.object.with.an.s.key.with.a.value.of.transparency.shall.not.be.included.in.a.form.xobject";

        public const String AllColourChannelsInTheJpeg2000DataShallHaveTheSameBitDepth = 
            "all.colour.channels.in.the.jpeg2000.data.shall.have.the.same.bit-depth";

        public const String AllHalftonesShallHaveHalftonetype1Or5 = "all.halftones.shall.have.halftonetype.1.or.5";

        public const String AnAnnotationDictionaryShallContainTheFKey = "an.annotation.dictionary.shall.contain.the.f.key";

        public const String AnAnnotationDictionaryShallNotContainAAKey = "an.annotation.dictionary.shall.not.contain.aa.key";

        public const String AnAnnotationDictionaryShallNotContainTheCaKeyWithAValueOtherThan1
             = "an.annotation.dictionary.shall.not.contain.the.ca.key.with.a.value.other.than.1";

        public const String AnExtgstateDictionaryShallNotContainTheHTPKey = "an.extgstate.dictionary.shall.not.contain.the.HTP.key";

        public const String AnExtgstateDictionaryShallNotContainTheTR2KeyWithAValueOtherThanDefault
             = "an.extgstate.dictionary.shall.not.contain.the.TR2.key.with.a.value.other.than.default";

        public const String AnExtgstateDictionaryShallNotContainTheTrKey = "an.extgstate.dictionary.shall.not.contain.the.tr.key";

        public const String AnImageDictionaryShallNotContainAlternatesKey = "an.image.dictionary.shall.not.contain.alternates.key";

        public const String AnImageDictionaryShallNotContainOpiKey = "an.image.dictionary.shall.not.contain.opi.key";

        public const String AnnotationOfType1ShouldHaveContentsKey = "annotation.of.type.1.should.have.contents.key";

        public const String AnnotationType1IsNotPermitted = "annotation.type.1.is.not.permitted";

        public const String AnnotationShallContainKeyF = "annotation.shall.contain.key.F";

        public const String AppearanceDictionaryOfWidgetSubtypeAndBtnFieldTypeShallContainOnlyTheNKeyWithDictionaryValue
             = "appearance.dictionary.of.widget.subtype.and.btn.field.type.shall.contain.only.the.n.key.with.dictionary.value";

        public const String AppearanceDictionaryShallContainOnlyTheNKeyWithStreamValue = 
            "appearance.dictionary.shall.contain.only.the.n.key.with.stream.value";

        public const String BlendModeShallHhaveValueNormalOrCompatible = "blend.mode.shall.have.value.normal.or.compatible";

        public const String CatalogDictionaryShallNotContainAAEntry = "catalog.dictionary.shall.not.contain.aa.entry";

        public const String CatalogDictionaryShallNotContainAlternatepresentationsNamesEntry
             = "catalog.dictionary.shall.not.contain.alternatepresentations.names.entry";

        public const String CatalogDictionaryShallNotContainOCPropertiesKey = "catalog.dictionary.shall.not.contain.the.ocproperties.key";

        public const String CatalogDictionaryShallNotContainRequirementsEntry = "catalog.dictionary.shall.not.contain.a.requirements.entry";

        public const String CatalogShallIncludeMarkInfoDictionaryWithMarkedTrueValue = "catalog.dictionary.shall.include.a.markinfo.dictionary.whose.entry.marked.shall.have.a.value.of.true";

        public const String CatalogShallContainLangEntry = "catalog.dictionary.shall.contain.lang.entry";

        public const String CatalogShallContainMetadataEntry = "catalog.dictionary.shall.contain.metadata.entry";

        public const String ColorSpace1ShallBeDeviceIndependent = "color.space.1.shall.be.device.independent";

        public const String ColorSpace1ShallHave2Components = "color.space.1.shall.have.2.components";

        public const String CryptFilterIsNotPermitted = "crypt.filter.is.not.permitted.inline.image";

        public const String DeprecatedSetStateAndNoOpActionsAreNotAllowed = "deprecated.setstate.and.noop.actions.are.not.allowed";

        public const String DestoutputprofileInThePdfa1OutputintentDictionaryShallBeRgb = 
            "destoutputprofile.in.the.pdfa1.outputintent.dictionary.shall.be.rgb";

        public const String DevicergbAndDevicecmykColorspacesCannotBeUsedBothInOneFile = 
            "devicergb.and.devicecmyk.colorspaces.cannot.be.used.both.in.one.file";

        public const String DevicergbMayBeUsedOnlyIfTheFileHasARgbPdfAOutputIntent = "devicergb.may.be.used.only.if.the.file.has.a.rgb.pdfa.outputIntent";

        public const String DevicergbMayBeUsedOnlyIfTheFileHasARgbPdfAOutputIntentOrDefaultRgbInUsageContext
             = "devicergb.may.be.used.only.if.the.file.has.a.rgb.pdfa.outputIntent.or.defaultrgb.in.usage.context";

        public const String DevicecmykMayBeUsedOnlyIfTheFileHasACmykPdfAOutputIntent = "devicecmyk.may.be.used.only.if.the.file.has.a.cmyk.pdfa.outputIntent";

        public const String DevicecmykMayBeUsedOnlyIfTheFileHasACmykPdfAOutputIntentOrDefaultCmykInUsageContext
             = "devicecmyk.may.be.used.only.if.the.file.has.a.cmyk.pdfa.outputIntent.or.defaultcmyk.in.usage.context";

        public const String DocumentToReadFromShallBeAPdfAConformantFileWithValidXmpMetadata
             = "document.to.read.from.shall.be.a.pdfa.conformant.file.with.valid.xmp.metadata";

        public const String EFKeyOfFileSpecificationDictionaryShallContainDictionaryWithValidFKey
             = "ef.key.of.file.specification.dictionary.shall.contain.dictionary.with.valid.f.key";

        public const String EmbeddedFileShallBeOfPdfMimeType = "embedded.file.shall.be.of.pdf.mime.type";

        public const String EmbeddedFileShallContainParamsKeyWithDictionaryAsValue = "embedded.file.shall.contain.params.key.with.dictionary.as.value";

        public const String EmbeddedFileShallContainParamsKeyWithValidModdateKey = "embedded.file.shall.contain.params.key.with.valid.moddate.key";

        public const String EncryptShallNotBeUsedInTrailerDictionary = "keyword.encrypt.shall.not.be.used.in.the.trailer.dictionary";

        public const String EveryAnnotationShallHaveAtLeastOneAppearanceDictionary = "every.annotation.shall.have.at.least.one.appearance.dictionary";

        public const String ExactlyOneColourSpaceSpecificationShallHaveTheValue0x01InTheApproxField
             = "exactly.one.colour.space.specification.shall.have.the.value.0x01.in.the.approx.field";

        public const String FileSpecificationDictionaryShallContainFKeyUFKeyAndDescKey = 
            "file.specification.dictionary.shall.contain.f.key.uf.key.and.desc.key";

        public const String FileSpecificationDictionaryShallContainOneOfThePredefinedAFRelationshipKeys
             = "file.specification.dictionary.shall.contain.one.of.the.predefined.afrelationship.keys";

        public const String FileSpecificationDictionaryShallNotContainTheEFKey = "file.specification.dictionary.shall.not.contain.the.EF.key";

        public const String GraphicStateStackDepthIsGreaterThan28 = "graphics.state.stack.depth.is.greater.than.28";

        public const String HalftonesShallNotContainHalftonename = "halftones.shall.not.contain.halftonename";

        public const String IfDeviceRgbCmykGrayUsedInFileThatFileShallContainPdfaOutputIntent
             = "if.device.rgb.cmyk.gray.used.in.file.that.file.shall.contain.pdfa.outputintent";

        public const String IfDeviceRgbCmykGrayUsedInFileThatFileShallContainPdfaOutputIntentOrDefaultRgbCmykGrayInUsageContext
             = "if.device.rgb.cmyk.gray.used.in.file.that.file.shall.contain.pdfa.outputintent.orDefaultRgb.Cmyk.Gray.in.usage.context";

        public const String IfOutputintentsArrayHasMoreThanOneEntryWithDestoutputprofileKeyTheSameIndirectObjectShallBeUsedAsTheValueOfThatObject
             = "if.outputintents.array.has.more.than.one.entry.with.destoutputprofile.key.the.same.indirect.object.shall.be.used.as.the.value.of.that.object";

        public const String IfSpecifiedRenderingShallBeOneOfTheFollowingRelativecolorimetricAbsolutecolorimetricPerceptualOrSaturation
             = "if.specified.rendering.shall.be.one.of.the.following.relativecolorimetric.absolutecolorimetric.perceptual.or.saturation";

        public const String IfTheDocumentDoesNotContainAPdfAOutputIntentTransparencyIsForbidden
             = "if.the.document.does.not.contain.a.pdfa.outputintent.transparency.is.forbidden";

        public const String Jpeg2000EnumeratedColourSpace19CIEJabShallNotBeUsed = "jpeg2000.enumerated.colour.space.19.(CIEJab).shall.not.be.used";

        public const String LZWDecodeFilterIsNotPermitted = "lzwdecode.filter.is.not.permitted";

        public const String MimeTypeShallBeSpecifiedUsingTheSubtypeKeyOfTheFileSpecificationStreamDictionary
             = "mime.type.shall.be.specified.using.the.subtype.key.of.the.file.specification.stream.dictionary";

        public const String NamedActionType1IsNotAllowed = "named.action.type.1.not.allowed";

        public const String NameDictionaryShallNotContainTheEmbeddedFilesKey = "name.dictionary.shall.not.contain.the.EmbeddedFiles.key";

        public const String NeedAppearancesFlagOfTheInteractiveFormDictionaryShallEitherNotBePresentedOrShallBeFalse
             = "needappearances.flag.of.the.interactive.form.dictionary.shall.either.not.be.presented.or.shall.be.false";

        public const String NoKeysOtherUr3andDocMdpShallBePresentInPerDict = "no.keys.other.than.UR3.and.DocMDP.shall.be.present.in.a.permissions.dictionary";

        public const String NotIdentityCryptFilterIsNotPermitted = "not.identity.crypt.filter.is.not.permitted";

        public const String OnlyJpxBaselineSetOfFeaturesShallBeUsed = "only.jpx.baseline.set.of.features.shall.be.used";

        public const String OnlyStandardBlendModesShallBeusedForTheValueOfTheBMKeyOnAnExtendedGraphicStateDictionary
             = "only.standard.blend.modes.shall.be.used.for.the.value.of.the.BM.key.in.an.extended.graphic.state.dictionary";

        public const String OptionalContentConfigurationDictionaryShallContainNameEntry = 
            "optional.content.configuration.dictionary.shall.contain.name.entry";

        public const String OrderArrayShallContainReferencesToAllOcgs = "order.array.shall.contain.references.to.all.ocgs";

        public const String OutputIntentColorSpaceShallBeEitherGrayRgbOrCmyk = "output.intent.color.space.shall.be.either.gray.rgb.or.cmyk";

        public const String OverprintModeShallNotBeOneWhenAnICCBasedCMYKColourSpaceIsUsedAndWhenOverprintingIsSetToTrue
             = "overprint.mode.shall.not.be.one.when.an.ICCBased.CMYK.colour.space.is.used.and.when.overprinting.is.set.to.true";

        public const String PageDictionaryShallNotContainAAEntry = "page.dictionary.shall.not.contain.aa.entry";

        public const String PageDictionaryShallNotContainPressstepsEntry = "page.dictionary.shall.not.contain.pressteps.entry";

        public const String PageLess3UnitsNoGreater14400InEitherDirection = "the.page.less.3.units.no.greater.14400.in.either.direction";

        public const String PdfStringIsTooLong = "pdf.string.is.too.long";

        public const String ProfileStreamOfOutputintentShallBeOutputProfilePrtrOrMonitorProfileMntr
             = "profile.stream.of.outputintent.shall.be.output.profile.(prtr).or.monitor.profile.(mntr)";

        public const String RealNumberIsOutOfRange = "real.number.is.out.of.range";

        public const String SigRefDicShallNotContDigestParam = "signature.references.dictionary.shall.not.contain.digestlocation.digestmethod.digestvalue";

        public const String StreamObjDictShallNotContainForFFilterOrFDecodeParams = "stream.object.dictionary.shall.not.contain.the.f.ffilter.or.fdecodeparams.keys";

        public const String TextAnnotationsShouldSetTheNozoomAndNorotateFlagBitsOfTheFKeyTo1
             = "text.annotations.should.set.the.nozoom.and.norotate.flag.bits.of.the.f.key.to.1";

        public const String TheAsKeyShallNotAppearInAnyOptionalContentConfigurationDictionary
             = "the.as.key.shall.not.appear.in.any.optional.content.configuration.dictionary";

        public const String TheBitDepthOfTheJpeg2000DataShallHaveAValueInTheRange1To38 = 
            "the.bit-depth.of.the.jpeg2000.data.shall.have.a.value.in.the.range.1to38";

        public const String TheCatalogDictionaryShallNotContainTheNeedsrenderingKey = "the.catalog.dictionary.shall.not.contain.the.needsrendering.key";

        public const String TheFKeysPrintFlagBitShallBeSetTo1AndItsHiddenInvisibleAndNoviewFlagBitsShallBeSetTo0
             = "the.f.keys.print.flag.bit.shall.be.set.to.1.and.its.hidden.invisible.and.noview.flag.bits.shall.be.set.to.0";

        public const String TheFKeysPrintFlagBitShallBeSetTo1AndItsHiddenInvisibleNoviewAndTogglenoviewFlagBitsShallBeSetTo0
             = "the.f.keys.print.flag.bit.shall.be.set.to.1.and.its.hidden.invisible.noview.and.togglenoview.flag.bits.shall.be.set.to.0";

        public const String TheInteractiveFormDictionaryShallNotContainTheXfaKey = "the.interactive.form.dictionary.shall.not.contain.the.xfa.key";

        public const String TheNumberOfColourChannelsInTheJpeg2000DataShallBe123 = "the.number.of.colour.channels.in.the.jpeg2000.data.shall.be.123";

        public const String TheSmaskKeyIsNotAllowedInExtgstate = "the.smask.key.is.not.allowed.in.extgstate";

        public const String TheSmaskKeyIsNotAllowedInXobjects = "the.smask.key.is.not.allowed.in.xobjects";

        public const String TheValueOfInterpolateKeyShallNotBeTrue = "the.value.of.interpolate.key.shall.not.be.true";

        public const String TheValueOfTheMethEntryInColrBoxShallBe123 = "the.value.of.the.meth.entry.in.colr.box.shall.be.123";

        public const String TintTransformAndAlternateSpaceOfSeparationArraysInTheColorantsOfDeviceNShallBeConsistentWithSameAttributesOfDeviceN
             = "tintTransform.and.alternateSpace.of.separation.arrays.in.the.colorants.of.deviceN.shall.be.consistent.with.same.attributes.of.deviceN";

        public const String TintTransformAndAlternateSpaceShallBeTheSameForTheAllSeparationCSWithTheSameName
             = "tintTransform.and.alternateSpace.shall.be.the.same.for.the.all.separation.cs.with.the.same.name";

        public const String TransparencyIsNotAllowedCAShallBeEqualTo1 = "transparency.is.not.allowed.CA.shall.be.equal.to.1";

        public const String TransparencyIsNotAllowedCaShallBeEqualTo1 = "transparency.is.not.allowed.ca.shall.be.equal.to.1";

        public const String ValueOfNameEntryShallBeUniqueAmongAllOptionalContentConfigurationDictionaries
             = "value.of.name.entry.shall.be.unique.among.all.optional.content.configuration.dictionaries";

        public const String WidgetAnnotationDictionaryOrFieldDictionaryShallNotIncludeAOrAAEntry
             = "widget.annotation.dictionary.or.field.dictionary.shall.not.include.a.or.aa.entry";

        /// <summary>Creates a PdfAConformanceException.</summary>
        /// <param name="message">the error message</param>
        public PdfAConformanceException(String message)
            : base(message)
        {
        }

        /// <summary>Creates a PdfAConformanceException.</summary>
        /// <param name="message">the error message</param>
        /// <param name="object">an object</param>
        public PdfAConformanceException(String message, Object @object)
            : base(message, @object)
        {
        }
    }
}
