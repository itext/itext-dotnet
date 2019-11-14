/*
* Copyright 2003-2012 by Paulo Soares.
*
* This list of constants was originally released with libtiff
* under the following license:
*
* Copyright (c) 1988-1997 Sam Leffler
* Copyright (c) 1991-1997 Silicon Graphics, Inc.
*
* Permission to use, copy, modify, distribute, and sell this software and
* its documentation for any purpose is hereby granted without fee, provided
* that (i) the above copyright notices and this permission notice appear in
* all copies of the software and related documentation, and (ii) the names of
* Sam Leffler and Silicon Graphics may not be used in any advertising or
* publicity relating to the software without the specific, prior written
* permission of Sam Leffler and Silicon Graphics.
*
* THE SOFTWARE IS PROVIDED "AS-IS" AND WITHOUT WARRANTY OF ANY KIND,
* EXPRESS, IMPLIED OR OTHERWISE, INCLUDING WITHOUT LIMITATION, ANY
* WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE.
*
* IN NO EVENT SHALL SAM LEFFLER OR SILICON GRAPHICS BE LIABLE FOR
* ANY SPECIAL, INCIDENTAL, INDIRECT OR CONSEQUENTIAL DAMAGES OF ANY KIND,
* OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS,
* WHETHER OR NOT ADVISED OF THE POSSIBILITY OF DAMAGE, AND ON ANY THEORY OF
* LIABILITY, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE
* OF THIS SOFTWARE.
*/
namespace iText.IO.Codec {
    /// <summary>A list of constants used in class TIFFImage.</summary>
    public class TIFFConstants {
        /*
        * TIFF Tag Definitions (from tifflib).
        */
        /// <summary>subfile data descriptor</summary>
        public const int TIFFTAG_SUBFILETYPE = 254;

        /// <summary>reduced resolution version</summary>
        public const int FILETYPE_REDUCEDIMAGE = 0x1;

        /// <summary>one page of many</summary>
        public const int FILETYPE_PAGE = 0x2;

        /// <summary>transparency mask</summary>
        public const int FILETYPE_MASK = 0x4;

        /// <summary>+kind of data in subfile</summary>
        public const int TIFFTAG_OSUBFILETYPE = 255;

        /// <summary>full resolution image data</summary>
        public const int OFILETYPE_IMAGE = 1;

        /// <summary>reduced size image data</summary>
        public const int OFILETYPE_REDUCEDIMAGE = 2;

        /// <summary>one page of many</summary>
        public const int OFILETYPE_PAGE = 3;

        /// <summary>image width in pixels</summary>
        public const int TIFFTAG_IMAGEWIDTH = 256;

        /// <summary>image height in pixels</summary>
        public const int TIFFTAG_IMAGELENGTH = 257;

        /// <summary>bits per channel (sample)</summary>
        public const int TIFFTAG_BITSPERSAMPLE = 258;

        /// <summary>data compression technique</summary>
        public const int TIFFTAG_COMPRESSION = 259;

        /// <summary>dump mode</summary>
        public const int COMPRESSION_NONE = 1;

        /// <summary>CCITT modified Huffman RLE</summary>
        public const int COMPRESSION_CCITTRLE = 2;

        /// <summary>CCITT Group 3 fax encoding</summary>
        public const int COMPRESSION_CCITTFAX3 = 3;

        /// <summary>CCITT Group 4 fax encoding</summary>
        public const int COMPRESSION_CCITTFAX4 = 4;

        /// <summary>Lempel-Ziv &amp; Welch</summary>
        public const int COMPRESSION_LZW = 5;

        /// <summary>!6.0 JPEG</summary>
        public const int COMPRESSION_OJPEG = 6;

        /// <summary>%JPEG DCT compression</summary>
        public const int COMPRESSION_JPEG = 7;

        /// <summary>NeXT 2-bit RLE</summary>
        public const int COMPRESSION_NEXT = 32766;

        /// <summary>#1 w/ word alignment</summary>
        public const int COMPRESSION_CCITTRLEW = 32771;

        /// <summary>Macintosh RLE</summary>
        public const int COMPRESSION_PACKBITS = 32773;

        /// <summary>ThunderScan RLE</summary>
        public const int COMPRESSION_THUNDERSCAN = 32809;

        /* codes 32895-32898 are reserved for ANSI IT8 TIFF/IT <dkelly@etsinc.com) */
        /// <summary>IT8 CT w/padding</summary>
        public const int COMPRESSION_IT8CTPAD = 32895;

        /// <summary>IT8 Linework RLE</summary>
        public const int COMPRESSION_IT8LW = 32896;

        /// <summary>IT8 Monochrome picture</summary>
        public const int COMPRESSION_IT8MP = 32897;

        /// <summary>IT8 Binary line art</summary>
        public const int COMPRESSION_IT8BL = 32898;

        /* compression codes 32908-32911 are reserved for Pixar */
        /// <summary>Pixar companded 10bit LZW</summary>
        public const int COMPRESSION_PIXARFILM = 32908;

        /// <summary>Pixar companded 11bit ZIP</summary>
        public const int COMPRESSION_PIXARLOG = 32909;

        /// <summary>Deflate compression</summary>
        public const int COMPRESSION_DEFLATE = 32946;

        /// <summary>Deflate compression, as recognized by Adobe</summary>
        public const int COMPRESSION_ADOBE_DEFLATE = 8;

        /* compression code 32947 is reserved for Oceana Matrix <dev@oceana.com> */
        /// <summary>Kodak DCS encoding</summary>
        public const int COMPRESSION_DCS = 32947;

        /// <summary>ISO JBIG</summary>
        public const int COMPRESSION_JBIG = 34661;

        /// <summary>SGI Log Luminance RLE</summary>
        public const int COMPRESSION_SGILOG = 34676;

        /// <summary>SGI Log 24-bit packed</summary>
        public const int COMPRESSION_SGILOG24 = 34677;

        /// <summary>photometric interpretation</summary>
        public const int TIFFTAG_PHOTOMETRIC = 262;

        /// <summary>min value is white</summary>
        public const int PHOTOMETRIC_MINISWHITE = 0;

        /// <summary>min value is black</summary>
        public const int PHOTOMETRIC_MINISBLACK = 1;

        /// <summary>RGB color model</summary>
        public const int PHOTOMETRIC_RGB = 2;

        /// <summary>color map indexed</summary>
        public const int PHOTOMETRIC_PALETTE = 3;

        /// <summary>$holdout mask</summary>
        public const int PHOTOMETRIC_MASK = 4;

        /// <summary>!color separations</summary>
        public const int PHOTOMETRIC_SEPARATED = 5;

        /// <summary>!CCIR 601</summary>
        public const int PHOTOMETRIC_YCBCR = 6;

        /// <summary>!1976 CIE L*a*b</summary>
        public const int PHOTOMETRIC_CIELAB = 8;

        /// <summary>CIE Log2(L)</summary>
        public const int PHOTOMETRIC_LOGL = 32844;

        /// <summary>CIE Log2(L) (u',v')</summary>
        public const int PHOTOMETRIC_LOGLUV = 32845;

        /// <summary>+thresholding used on data</summary>
        public const int TIFFTAG_THRESHHOLDING = 263;

        /// <summary>b&amp;w art scan</summary>
        public const int THRESHHOLD_BILEVEL = 1;

        /// <summary>or dithered scan</summary>
        public const int THRESHHOLD_HALFTONE = 2;

        /// <summary>usually floyd-steinberg</summary>
        public const int THRESHHOLD_ERRORDIFFUSE = 3;

        /// <summary>+dithering matrix width</summary>
        public const int TIFFTAG_CELLWIDTH = 264;

        /// <summary>+dithering matrix height</summary>
        public const int TIFFTAG_CELLLENGTH = 265;

        /// <summary>data order within a byte</summary>
        public const int TIFFTAG_FILLORDER = 266;

        /// <summary>most significant -&gt; least</summary>
        public const int FILLORDER_MSB2LSB = 1;

        /// <summary>least significant -&gt; most</summary>
        public const int FILLORDER_LSB2MSB = 2;

        /// <summary>name of doc.</summary>
        /// <remarks>name of doc. image is from</remarks>
        public const int TIFFTAG_DOCUMENTNAME = 269;

        /// <summary>info about image</summary>
        public const int TIFFTAG_IMAGEDESCRIPTION = 270;

        /// <summary>scanner manufacturer name</summary>
        public const int TIFFTAG_MAKE = 271;

        /// <summary>scanner model name/number</summary>
        public const int TIFFTAG_MODEL = 272;

        /// <summary>offsets to data strips</summary>
        public const int TIFFTAG_STRIPOFFSETS = 273;

        /// <summary>+image orientation</summary>
        public const int TIFFTAG_ORIENTATION = 274;

        /// <summary>row 0 top, col 0 lhs</summary>
        public const int ORIENTATION_TOPLEFT = 1;

        /// <summary>row 0 top, col 0 rhs</summary>
        public const int ORIENTATION_TOPRIGHT = 2;

        /// <summary>row 0 bottom, col 0 rhs</summary>
        public const int ORIENTATION_BOTRIGHT = 3;

        /// <summary>row 0 bottom, col 0 lhs</summary>
        public const int ORIENTATION_BOTLEFT = 4;

        /// <summary>row 0 lhs, col 0 top</summary>
        public const int ORIENTATION_LEFTTOP = 5;

        /// <summary>row 0 rhs, col 0 top</summary>
        public const int ORIENTATION_RIGHTTOP = 6;

        /// <summary>row 0 rhs, col 0 bottom</summary>
        public const int ORIENTATION_RIGHTBOT = 7;

        /// <summary>row 0 lhs, col 0 bottom</summary>
        public const int ORIENTATION_LEFTBOT = 8;

        /// <summary>samples per pixel</summary>
        public const int TIFFTAG_SAMPLESPERPIXEL = 277;

        /// <summary>rows per strip of data</summary>
        public const int TIFFTAG_ROWSPERSTRIP = 278;

        /// <summary>bytes counts for strips</summary>
        public const int TIFFTAG_STRIPBYTECOUNTS = 279;

        /// <summary>+minimum sample value</summary>
        public const int TIFFTAG_MINSAMPLEVALUE = 280;

        /// <summary>+maximum sample value</summary>
        public const int TIFFTAG_MAXSAMPLEVALUE = 281;

        /// <summary>pixels/resolution in x</summary>
        public const int TIFFTAG_XRESOLUTION = 282;

        /// <summary>pixels/resolution in y</summary>
        public const int TIFFTAG_YRESOLUTION = 283;

        /// <summary>storage organization</summary>
        public const int TIFFTAG_PLANARCONFIG = 284;

        /// <summary>single image plane</summary>
        public const int PLANARCONFIG_CONTIG = 1;

        /// <summary>separate planes of data</summary>
        public const int PLANARCONFIG_SEPARATE = 2;

        /// <summary>page name image is from</summary>
        public const int TIFFTAG_PAGENAME = 285;

        /// <summary>x page offset of image lhs</summary>
        public const int TIFFTAG_XPOSITION = 286;

        /// <summary>y page offset of image lhs</summary>
        public const int TIFFTAG_YPOSITION = 287;

        /// <summary>+byte offset to free block</summary>
        public const int TIFFTAG_FREEOFFSETS = 288;

        /// <summary>+sizes of free blocks</summary>
        public const int TIFFTAG_FREEBYTECOUNTS = 289;

        /// <summary>$gray scale curve accuracy</summary>
        public const int TIFFTAG_GRAYRESPONSEUNIT = 290;

        /// <summary>tenths of a unit</summary>
        public const int GRAYRESPONSEUNIT_10S = 1;

        /// <summary>hundredths of a unit</summary>
        public const int GRAYRESPONSEUNIT_100S = 2;

        /// <summary>thousandths of a unit</summary>
        public const int GRAYRESPONSEUNIT_1000S = 3;

        /// <summary>ten-thousandths of a unit</summary>
        public const int GRAYRESPONSEUNIT_10000S = 4;

        /// <summary>hundred-thousandths</summary>
        public const int GRAYRESPONSEUNIT_100000S = 5;

        /// <summary>$gray scale response curve</summary>
        public const int TIFFTAG_GRAYRESPONSECURVE = 291;

        /// <summary>32 flag bits</summary>
        public const int TIFFTAG_GROUP3OPTIONS = 292;

        /// <summary>2-dimensional coding</summary>
        public const int GROUP3OPT_2DENCODING = 0x1;

        /// <summary>data not compressed</summary>
        public const int GROUP3OPT_UNCOMPRESSED = 0x2;

        /// <summary>fill to byte boundary</summary>
        public const int GROUP3OPT_FILLBITS = 0x4;

        /// <summary>32 flag bits</summary>
        public const int TIFFTAG_GROUP4OPTIONS = 293;

        /// <summary>data not compressed</summary>
        public const int GROUP4OPT_UNCOMPRESSED = 0x2;

        /// <summary>fill to byte boundary</summary>
        public const int GROUP4OPT_FILLBITS = 0x4;

        /// <summary>units of resolutions</summary>
        public const int TIFFTAG_RESOLUTIONUNIT = 296;

        /// <summary>no meaningful units</summary>
        public const int RESUNIT_NONE = 1;

        /// <summary>english</summary>
        public const int RESUNIT_INCH = 2;

        /// <summary>metric</summary>
        public const int RESUNIT_CENTIMETER = 3;

        /// <summary>page numbers of multi-page</summary>
        public const int TIFFTAG_PAGENUMBER = 297;

        /// <summary>$color curve accuracy</summary>
        public const int TIFFTAG_COLORRESPONSEUNIT = 300;

        /// <summary>tenths of a unit</summary>
        public const int COLORRESPONSEUNIT_10S = 1;

        /// <summary>hundredths of a unit</summary>
        public const int COLORRESPONSEUNIT_100S = 2;

        /// <summary>thousandths of a unit</summary>
        public const int COLORRESPONSEUNIT_1000S = 3;

        /// <summary>ten-thousandths of a unit</summary>
        public const int COLORRESPONSEUNIT_10000S = 4;

        /// <summary>hundred-thousandths</summary>
        public const int COLORRESPONSEUNIT_100000S = 5;

        /// <summary>!colorimetry info</summary>
        public const int TIFFTAG_TRANSFERFUNCTION = 301;

        /// <summary>name and release</summary>
        public const int TIFFTAG_SOFTWARE = 305;

        /// <summary>creation date and time</summary>
        public const int TIFFTAG_DATETIME = 306;

        /// <summary>creator of image</summary>
        public const int TIFFTAG_ARTIST = 315;

        /// <summary>machine where created</summary>
        public const int TIFFTAG_HOSTCOMPUTER = 316;

        /// <summary>prediction scheme w/ LZW</summary>
        public const int TIFFTAG_PREDICTOR = 317;

        /// <summary>no predictor</summary>
        public const int PREDICTOR_NONE = 1;

        /// <summary>horizontal differencing</summary>
        public const int PREDICTOR_HORIZONTAL_DIFFERENCING = 2;

        /// <summary>image white point</summary>
        public const int TIFFTAG_WHITEPOINT = 318;

        /// <summary>!primary chromaticities</summary>
        public const int TIFFTAG_PRIMARYCHROMATICITIES = 319;

        /// <summary>RGB map for pallette image</summary>
        public const int TIFFTAG_COLORMAP = 320;

        /// <summary>!highlight+shadow info</summary>
        public const int TIFFTAG_HALFTONEHINTS = 321;

        /// <summary>!rows/data tile</summary>
        public const int TIFFTAG_TILEWIDTH = 322;

        /// <summary>!cols/data tile</summary>
        public const int TIFFTAG_TILELENGTH = 323;

        /// <summary>!offsets to data tiles</summary>
        public const int TIFFTAG_TILEOFFSETS = 324;

        /// <summary>!byte counts for tiles</summary>
        public const int TIFFTAG_TILEBYTECOUNTS = 325;

        /// <summary>lines w/ wrong pixel count</summary>
        public const int TIFFTAG_BADFAXLINES = 326;

        /// <summary>regenerated line info</summary>
        public const int TIFFTAG_CLEANFAXDATA = 327;

        /// <summary>no errors detected</summary>
        public const int CLEANFAXDATA_CLEAN = 0;

        /// <summary>receiver regenerated lines</summary>
        public const int CLEANFAXDATA_REGENERATED = 1;

        /// <summary>uncorrected errors exist</summary>
        public const int CLEANFAXDATA_UNCLEAN = 2;

        /// <summary>max consecutive bad lines</summary>
        public const int TIFFTAG_CONSECUTIVEBADFAXLINES = 328;

        /// <summary>subimage descriptors</summary>
        public const int TIFFTAG_SUBIFD = 330;

        /// <summary>!inks in separated image</summary>
        public const int TIFFTAG_INKSET = 332;

        /// <summary>!cyan-magenta-yellow-black</summary>
        public const int INKSET_CMYK = 1;

        /// <summary>!ascii names of inks</summary>
        public const int TIFFTAG_INKNAMES = 333;

        /// <summary>!number of inks</summary>
        public const int TIFFTAG_NUMBEROFINKS = 334;

        /// <summary>!0% and 100% dot codes</summary>
        public const int TIFFTAG_DOTRANGE = 336;

        /// <summary>!separation target</summary>
        public const int TIFFTAG_TARGETPRINTER = 337;

        /// <summary>!info about extra samples</summary>
        public const int TIFFTAG_EXTRASAMPLES = 338;

        /// <summary>!unspecified data</summary>
        public const int EXTRASAMPLE_UNSPECIFIED = 0;

        /// <summary>!associated alpha data</summary>
        public const int EXTRASAMPLE_ASSOCALPHA = 1;

        /// <summary>!unassociated alpha data</summary>
        public const int EXTRASAMPLE_UNASSALPHA = 2;

        /// <summary>!data sample format</summary>
        public const int TIFFTAG_SAMPLEFORMAT = 339;

        /// <summary>!unsigned integer data</summary>
        public const int SAMPLEFORMAT_UINT = 1;

        /// <summary>!signed integer data</summary>
        public const int SAMPLEFORMAT_INT = 2;

        /// <summary>!IEEE floating point data</summary>
        public const int SAMPLEFORMAT_IEEEFP = 3;

        /// <summary>!untyped data</summary>
        public const int SAMPLEFORMAT_VOID = 4;

        /// <summary>!complex signed int</summary>
        public const int SAMPLEFORMAT_COMPLEXINT = 5;

        /// <summary>!complex ieee floating</summary>
        public const int SAMPLEFORMAT_COMPLEXIEEEFP = 6;

        /// <summary>!variable MinSampleValue</summary>
        public const int TIFFTAG_SMINSAMPLEVALUE = 340;

        /// <summary>!variable MaxSampleValue</summary>
        public const int TIFFTAG_SMAXSAMPLEVALUE = 341;

        /// <summary>%JPEG table stream</summary>
        public const int TIFFTAG_JPEGTABLES = 347;

        /*
        * Tags 512-521 are obsoleted by Technical Note #2
        * which specifies a revised JPEG-in-TIFF scheme.
        */
        /// <summary>!JPEG processing algorithm</summary>
        public const int TIFFTAG_JPEGPROC = 512;

        /// <summary>!baseline sequential</summary>
        public const int JPEGPROC_BASELINE = 1;

        /// <summary>!Huffman coded lossless</summary>
        public const int JPEGPROC_LOSSLESS = 14;

        /// <summary>!pointer to SOI marker</summary>
        public const int TIFFTAG_JPEGIFOFFSET = 513;

        /// <summary>!JFIF stream length</summary>
        public const int TIFFTAG_JPEGIFBYTECOUNT = 514;

        /// <summary>!restart interval length</summary>
        public const int TIFFTAG_JPEGRESTARTINTERVAL = 515;

        /// <summary>!lossless proc predictor</summary>
        public const int TIFFTAG_JPEGLOSSLESSPREDICTORS = 517;

        /// <summary>!lossless point transform</summary>
        public const int TIFFTAG_JPEGPOINTTRANSFORM = 518;

        /// <summary>!Q matrice offsets</summary>
        public const int TIFFTAG_JPEGQTABLES = 519;

        /// <summary>!DCT table offsets</summary>
        public const int TIFFTAG_JPEGDCTABLES = 520;

        /// <summary>!AC coefficient offsets</summary>
        public const int TIFFTAG_JPEGACTABLES = 521;

        /// <summary>!RGB -&gt; YCbCr transform</summary>
        public const int TIFFTAG_YCBCRCOEFFICIENTS = 529;

        /// <summary>!YCbCr subsampling factors</summary>
        public const int TIFFTAG_YCBCRSUBSAMPLING = 530;

        /// <summary>!subsample positioning</summary>
        public const int TIFFTAG_YCBCRPOSITIONING = 531;

        /// <summary>!as in PostScript Level 2</summary>
        public const int YCBCRPOSITION_CENTERED = 1;

        /// <summary>!as in CCIR 601-1</summary>
        public const int YCBCRPOSITION_COSITED = 2;

        /// <summary>!colorimetry info</summary>
        public const int TIFFTAG_REFERENCEBLACKWHITE = 532;

        /* tags 32952-32956 are private tags registered to Island Graphics */
        /// <summary>image reference points</summary>
        public const int TIFFTAG_REFPTS = 32953;

        /// <summary>region-xform tack point</summary>
        public const int TIFFTAG_REGIONTACKPOINT = 32954;

        /// <summary>warp quadrilateral</summary>
        public const int TIFFTAG_REGIONWARPCORNERS = 32955;

        /// <summary>affine transformation mat</summary>
        public const int TIFFTAG_REGIONAFFINE = 32956;

        /* tags 32995-32999 are private tags registered to SGI */
        /// <summary>$use ExtraSamples</summary>
        public const int TIFFTAG_MATTEING = 32995;

        /// <summary>$use SampleFormat</summary>
        public const int TIFFTAG_DATATYPE = 32996;

        /// <summary>z depth of image</summary>
        public const int TIFFTAG_IMAGEDEPTH = 32997;

        /// <summary>z depth/data tile</summary>
        public const int TIFFTAG_TILEDEPTH = 32998;

        /* tags 33300-33309 are private tags registered to Pixar
        * TIFFTAG_PIXAR_IMAGEFULLWIDTH and TIFFTAG_PIXAR_IMAGEFULLLENGTH
        * are set when an image has been cropped out of a larger image.
        * They reflect the size of the original uncropped image.
        * The TIFFTAG_XPOSITION and TIFFTAG_YPOSITION can be used
        * to determine the position of the smaller image in the larger one.
        */
        /// <summary>full image size in x</summary>
        public const int TIFFTAG_PIXAR_IMAGEFULLWIDTH = 33300;

        /// <summary>full image size in y</summary>
        public const int TIFFTAG_PIXAR_IMAGEFULLLENGTH = 33301;

        /* Tags 33302-33306 are used to identify special image modes and data used by Pixar's texture formats. */
        /// <summary>texture map format</summary>
        public const int TIFFTAG_PIXAR_TEXTUREFORMAT = 33302;

        /// <summary>s &amp; t wrap modes</summary>
        public const int TIFFTAG_PIXAR_WRAPMODES = 33303;

        /// <summary>cotan(fov) for env.</summary>
        /// <remarks>cotan(fov) for env. maps</remarks>
        public const int TIFFTAG_PIXAR_FOVCOT = 33304;

        /// <summary>W2S</summary>
        public const int TIFFTAG_PIXAR_MATRIX_WORLDTOSCREEN = 33305;

        /// <summary>W2C</summary>
        public const int TIFFTAG_PIXAR_MATRIX_WORLDTOCAMERA = 33306;

        /// <summary>
        /// device serial number
        /// tag 33405 is a private tag registered to Eastman Kodak
        /// </summary>
        public const int TIFFTAG_WRITERSERIALNUMBER = 33405;

        /// <summary>tag 33432 is listed in the 6.0 spec w/ unknown ownership</summary>
        public const int TIFFTAG_COPYRIGHT = 33432;

        /* copyright string */
        /// <summary>IPTC TAG from RichTIFF specifications</summary>
        public const int TIFFTAG_RICHTIFFIPTC = 33723;

        /* 34016-34029 are reserved for ANSI IT8 TIFF/IT <dkelly@etsinc.com) */
        /// <summary>site name</summary>
        public const int TIFFTAG_IT8SITE = 34016;

        /// <summary>color seq.</summary>
        /// <remarks>color seq. [RGB,CMYK,etc]</remarks>
        public const int TIFFTAG_IT8COLORSEQUENCE = 34017;

        /// <summary>DDES Header</summary>
        public const int TIFFTAG_IT8HEADER = 34018;

        /// <summary>raster scanline padding</summary>
        public const int TIFFTAG_IT8RASTERPADDING = 34019;

        /// <summary># of bits in short run</summary>
        public const int TIFFTAG_IT8BITSPERRUNLENGTH = 34020;

        /// <summary># of bits in long run</summary>
        public const int TIFFTAG_IT8BITSPEREXTENDEDRUNLENGTH = 34021;

        /// <summary>LW colortable</summary>
        public const int TIFFTAG_IT8COLORTABLE = 34022;

        /// <summary>BP/BL image color switch</summary>
        public const int TIFFTAG_IT8IMAGECOLORINDICATOR = 34023;

        /// <summary>BP/BL bg color switch</summary>
        public const int TIFFTAG_IT8BKGCOLORINDICATOR = 34024;

        /// <summary>BP/BL image color value</summary>
        public const int TIFFTAG_IT8IMAGECOLORVALUE = 34025;

        /// <summary>BP/BL bg color value</summary>
        public const int TIFFTAG_IT8BKGCOLORVALUE = 34026;

        /// <summary>MP pixel intensity value</summary>
        public const int TIFFTAG_IT8PIXELINTENSITYRANGE = 34027;

        /// <summary>HC transparency switch</summary>
        public const int TIFFTAG_IT8TRANSPARENCYINDICATOR = 34028;

        /// <summary>color character.</summary>
        /// <remarks>color character. table</remarks>
        public const int TIFFTAG_IT8COLORCHARACTERIZATION = 34029;

        /* tags 34232-34236 are private tags registered to Texas Instruments */
        /// <summary>Sequence Frame Count</summary>
        public const int TIFFTAG_FRAMECOUNT = 34232;

        /// <summary>
        /// ICC profile data
        /// tag 34750 is a private tag registered to Adobe?
        /// </summary>
        public const int TIFFTAG_ICCPROFILE = 34675;

        /// <summary>tag 34377 is private tag registered to Adobe for PhotoShop</summary>
        public const int TIFFTAG_PHOTOSHOP = 34377;

        /// <summary>
        /// JBIG options
        /// tag 34750 is a private tag registered to Pixel Magic
        /// </summary>
        public const int TIFFTAG_JBIGOPTIONS = 34750;

        /* tags 34908-34914 are private tags registered to SGI */
        /// <summary>encoded Class 2 ses.</summary>
        /// <remarks>encoded Class 2 ses. parms</remarks>
        public const int TIFFTAG_FAXRECVPARAMS = 34908;

        /// <summary>received SubAddr string</summary>
        public const int TIFFTAG_FAXSUBADDRESS = 34909;

        /// <summary>receive time (secs)</summary>
        public const int TIFFTAG_FAXRECVTIME = 34910;

        /* tags 37439-37443 are registered to SGI <gregl@sgi.com> */
        /// <summary>Sample value to Nits</summary>
        public const int TIFFTAG_STONITS = 37439;

        /// <summary>
        /// unknown use
        /// tag 34929 is a private tag registered to FedEx
        /// </summary>
        public const int TIFFTAG_FEDEX_EDR = 34929;

        /// <summary>
        /// hue shift correction data
        /// tag 65535 is an undefined tag used by Eastman Kodak
        /// </summary>
        public const int TIFFTAG_DCSHUESHIFTVALUES = 65535;
    }
}
