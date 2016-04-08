/*
$Id: a69e5f7d07a4a83e0687685143d975c3cec0537c $

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

namespace com.itextpdf.io
{
	public class IOException : Exception
	{
		public const String _1BitSamplesAreNotSupportedForHorizontalDifferencingPredictor
			 = "{0} bit.samples.are.not.supported.for.horizontal.differencing.predictor";

		public const String _1CorruptedJfifMarker = "{0} corrupted.jfif.marker";

		public const String _1IsNotAValidJpegFile = "{0} is.not.a.valid.jpeg.file";

		public const String _1MustHave8BitsPerComponent = "{0} must.have.8.bits.per.component";

		public const String _1UnsupportedJpegMarker2 = "{0} unsupported.jpeg.marker {1}";

		public const String _1IsNotAnAfmOrPfmFontFile = "{0} is.not.an.afm.or.pfm.font.file";

		public const String _1NotFoundAsFileOrResource = "{0} not found as file or resource.";

		public const String AllFillBitsPrecedingEolCodeMustBe0 = "all.fill.bits.preceding.eol.code.must.be.0";

		public const String BadEndiannessTagNot0x4949Or0x4d4d = "bad.endianness.tag.not.0x4949.or.0x4d4d";

		public const String BadMagicNumberShouldBe42 = "bad.magic.number.should.be.42";

		public const String BitsPerComponentMustBe1_2_4or8 = "bits.per.component.must.be.1.2.4.or.8";

		public const String BitsPerSample1IsNotSupported = "bits.per.sample {0} is.not.supported";

		public const String BmpImageException = "bmp.image.exception";

		public const String BytesCanBeAssignedToByteArrayOutputStreamOnly = "bytes.can.be.assigned.to.bytearrayoutputstream.only";

		public const String BytesCanBeResetInByteArrayOutputStreamOnly = "bytes.can.be.reset.in.bytearrayoutputstream.only";

		public const String CannotGetTiffImageColor = "cannot.get.tiff.image.color";

		public const String CannotFind1Frame = "cannot.find {0} frame";

		public const String CannotHandleBoxSizesHigherThan2_32 = "cannot.handle.box.sizes.higher.than.2.32";

		public const String CannotInflateTiffImage = "cannot.inflate.tiff.image";

		public const String CannotReadTiffImage = "cannot.read.tiff.image";

		public const String CannotWriteByte = "cannot.write.byte";

		public const String CannotWriteBytes = "cannot.write.bytes";

		public const String CannotWriteFloatNumber = "cannot.write.float.number";

		public const String CannotWriteIntNumber = "cannot.write.int.number";

		public const String CcittCompressionTypeMustBeCcittg4Ccittg3_1dOrCcittg3_2d = "ccitt.compression.type.must.be.ccittg4.ccittg3.1d.or.ccittg3.2d";

		public const String ComponentsMustBe1_3Or4 = "components.must.be.1.3.or.4";

		public const String Compression1IsNotSupported = "compression {0} is.not.supported";

		public const String ColorDepthIsNotSupported = "the.color.depth {0} is.not.supported";

		public const String ColorSpaceIsNotSupported = "the.color.space {0} is.not.supported";

		public const String CompressionJpegIsOnlySupportedWithASingleStripThisImageHas1Strips
			 = "compression.jpeg.is.only.supported.with.a.single.strip.this.image.has {0} strips";

		public const String DirectoryNumberTooLarge = "directory.number.too.large";

		public const String EolCodeWordEncounteredInBlackRun = "eol.code.word.encountered.in.black.run";

		public const String EolCodeWordEncounteredInWhiteRun = "eol.code.word.encountered.in.white.run";

		public const String ErrorAtFilePointer1 = "error.at.file.pointer {0}";

		public const String ErrorReadingString = "error.reading.string";

		public const String ErrorWithJpMarker = "error.with.jp.marker";

		public const String ExpectedFtypMarker = "expected.ftyp.marker";

		public const String ExpectedIhdrMarker = "expected.ihdr.marker";

		public const String ExpectedJpMarker = "expected.jp.marker";

		public const String ExpectedJp2hMarker = "expected.jp2h.marker";

		public const String ExtraSamplesAreNotSupported = "extra.samples.are.not.supported";

		public const String FdfStartxrefNotFound = "fdf.startxref.not.found";

		public const String FirstScanlineMustBe1dEncoded = "first.scanline.must.be.1d.encoded";

		public const String FontFile1NotFound = "font.file {0} not.found";

		public const String ImageFormatCannotBeRecognized = "image.format.cannot.be.recognized";

		public const String GifImageException = "gif.image.exception";

		public const String GtNotExpected = "gt.not.expected";

		public const String GifSignatureNotFound = "gif.signature.not.found";

		public const String IllegalValueForPredictorInTiffFile = "illegal.value.for.predictor.in.tiff.file";

		public const String Font1IsNotRecognized = "font {0} is.not.recognized";

		public const String FontIsNotRecognized = "font.is.not.recognized";

		public const String ImageCanNotBeAnImageMask = "image.can.not.be.an.image.mask";

		public const String ImageMaskCannotContainAnotherImageMask = "image.mask.cannot.contain.another.image.mask";

		public const String ImageMaskIsNotAMaskDidYouDoMakeMask = "image.mask.is.not.a.mask.did.you.do.makemask";

		public const String IncompletePalette = "incomplete.palette";

		public const String InvalidTTCFile = "{0} is.not.a.valid.ttc.file";

		public const String InvalidBmpFileCompression = "invalid.bmp.file.compression";

		public const String InvalidCodeEncountered = "invalid.code.encountered";

		public const String InvalidCodeEncounteredWhileDecoding2dGroup3CompressedData = "invalid.code.encountered.while.decoding.2d.group.3.compressed.data";

		public const String InvalidCodeEncounteredWhileDecoding2dGroup4CompressedData = "invalid.code.encountered.while.decoding.2d.group.4.compressed.data";

		public const String InvalidIccProfile = "invalid.icc.profile";

		public const String InvalidJpeg2000File = "invalid.jpeg2000.file";

		public const String InvalidMagicValueForBmpFile = "invalid.magic.value.for.bmp.file";

		public const String IoException = "io.exception";

		public const String Jbig2ImageException = "jbig2.image.exception";

		public const String JpegImageException = "jpeg.image.exception";

		public const String Jpeg2000ImageException = "jpeg2000.image.exception";

		public const String MissingTagSForOjpegCompression = "missing.tag.s.for.ojpeg.compression";

		public const String NValueIsNotSupported = "N.value.1.is.not.supported";

		public const String PageNumberMustBeGtEq1 = "page.number.must.be.gt.eq {0}";

		public const String PdfEncodings = "pdf.encodings";

		public const String PdfHeaderNotFound = "pdf.header.not.found";

		public const String PdfStartxrefNotFound = "pdf.startxref.not.found";

		public const String Photometric1IsNotSupported = "photometric.1.is.not.supported";

		public const String PlanarImagesAreNotSupported = "planar.images.are.not.supported";

		public const String PngFilterUnknown = "png.filter.unknown";

		public const String PngImageException = "png.image.exception";

		public const String PrematureEofWhileReadingJpg = "premature.eof.while.reading.jpg";

		public const String ScanlineMustBeginWithEolCodeWord = "scanline.must.begin.with.eol.code.word";

		public const String Tiff50StyleLzwCodesAreNotSupported = "tiff.5.0.style.lzw.codes.are.not.supported";

		public const String TiffFillOrderTagMustBeEither1Or2 = "tiff.fill.order.tag.must.be.either.1.or.2";

		public const String TiffImageException = "tiff.image.exception";

		public const String TTCIndexDoesNotExistInFile = "ttc.index.doesn't.exist.in.ttc.file";

		public const String TilesAreNotSupported = "tiles.are.not.supported";

		public const String TransparencyLengthMustBeEqualTo2WithCcittImages = "transparency.length.must.be.equal.to.2.with.ccitt.images";

		public const String UnexpectedCloseBracket = "unexpected.close.bracket";

		public const String UnexpectedGtGt = "unexpected.gt.gt";

		public const String UnknownCompressionType1 = "unknown.compression.type {0}";

		public const String UnknownIOException = "unknown.io.exception";

		public const String UnsupportedBoxSizeEqEq0 = "unsupported.box.size.eq.eq.0";

		public const String WrongNumberOfComponentsInIccProfile = "icc.profile.contains {0} components.the.image.data.contains {2} components";

		protected internal Object obj;

		private IList<Object> messageParams;

		public IOException(String message)
			: base(message)
		{
		}

		public IOException(Exception cause)
			: this(UnknownIOException, cause)
		{
		}

		public IOException(String message, Object obj)
			: this(message)
		{
			this.obj = obj;
		}

		public IOException(String message, Exception cause)
			: base(message, cause)
		{
		}

		public IOException(String message, Exception cause, Object obj)
			: this(message, cause)
		{
			this.obj = obj;
		}

		public override String Message
		{
			get
			{
				if (messageParams == null || messageParams.Count == 0)
				{
					return base.Message;
				}
				else
				{
					Object[] parameters = new Object[messageParams.Count];
					for (int i = 0; i < messageParams.Count; i++)
					{
						parameters[i] = messageParams[i];
					}
					return String.Format(base.Message, parameters);
				}
			}
		}

		public virtual com.itextpdf.io.IOException SetMessageParams(params Object[] messageParams
			)
		{
			this.messageParams = new List<Object>();
			com.itextpdf.io.util.JavaUtil.CollectionsAddAll(this.messageParams, messageParams
				);
			return this;
		}
	}
}
