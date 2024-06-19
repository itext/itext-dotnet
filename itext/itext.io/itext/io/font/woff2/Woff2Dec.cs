// Copyright 2016 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// This is part of java port of project hosted at https://github.com/google/woff2
using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Codec.Brotli.Dec;
using iText.IO.Exceptions;

namespace iText.IO.Font.Woff2 {
//\cond DO_NOT_DOCUMENT
    // Library for converting WOFF2 format font files to their TTF versions.
    internal class Woff2Dec {
        // simple glyph flags
        private const int kGlyfOnCurve = 1 << 0;

        private const int kGlyfXShort = 1 << 1;

        private const int kGlyfYShort = 1 << 2;

        private const int kGlyfRepeat = 1 << 3;

        private const int kGlyfThisXIsSame = 1 << 4;

        private const int kGlyfThisYIsSame = 1 << 5;

        // composite glyph flags
        // See CompositeGlyph.java in sfntly for full definitions
        private const int FLAG_ARG_1_AND_2_ARE_WORDS = 1 << 0;

        private const int FLAG_WE_HAVE_A_SCALE = 1 << 3;

        private const int FLAG_MORE_COMPONENTS = 1 << 5;

        private const int FLAG_WE_HAVE_AN_X_AND_Y_SCALE = 1 << 6;

        private const int FLAG_WE_HAVE_A_TWO_BY_TWO = 1 << 7;

        private const int FLAG_WE_HAVE_INSTRUCTIONS = 1 << 8;

        private const int kEndPtsOfContoursOffset = 10;

        // 98% of Google Fonts have no glyph above 5k bytes
        // Largest glyph ever observed was 72k bytes
        private const int kDefaultGlyphBuf = 5120;

        // Over 14k test fonts the max compression ratio seen to date was ~20.
        // >100 suggests you wrote a bad uncompressed size.
        private const float kMaxPlausibleCompressionRatio = 100.0f;

        // metadata for a TTC font entry
        private class TtcFont {
            public int flavor;

            public int dst_offset;

            public int header_checksum;

            public short[] table_indices;
        }

        private class Woff2Header {
            public int flavor;

            public int header_version;

            public short num_tables;

            public int compressed_offset;

            public int compressed_length;

            public int uncompressed_size;

            public Woff2Common.Table[] tables;

            // num_tables unique tables
            public Woff2Dec.TtcFont[] ttc_fonts;
            // metadata to help rebuild font
        }

        /// <summary>Accumulates data we may need to reconstruct a single font.</summary>
        /// <remarks>
        /// Accumulates data we may need to reconstruct a single font. One per font
        /// created for a TTC.
        /// </remarks>
        private class Woff2FontInfo {
            public short num_glyphs;

            public short index_format;

            public short num_hmetrics;

            public short[] x_mins;

            public IDictionary<int, int?> table_entry_by_tag = new Dictionary<int, int?>();
        }

        // Accumulates metadata as we rebuild the font
        private class RebuildMetadata {
//\cond DO_NOT_DOCUMENT
            internal int header_checksum;
//\endcond

//\cond DO_NOT_DOCUMENT
            // set by writeHeaders
            internal Woff2Dec.Woff2FontInfo[] font_infos;
//\endcond

//\cond DO_NOT_DOCUMENT
            // checksums for tables that have been written.
            // (tag, src_offset) => checksum. Need both because 0-length loca.
            internal IDictionary<Woff2Dec.TableChecksumInfo, int?> checksums = new Dictionary<Woff2Dec.TableChecksumInfo
                , int?>();
//\endcond
        }

        private class TableChecksumInfo {
            public int tag;

            public int offset;

            public TableChecksumInfo(int tag, int offset) {
                this.tag = tag;
                this.offset = offset;
            }

            public override int GetHashCode() {
                return tag.GetHashCode() * 13 + offset.GetHashCode();
            }

            public override bool Equals(Object o) {
                if (this == o) {
                    return true;
                }
                if (o is Woff2Dec.TableChecksumInfo) {
                    Woff2Dec.TableChecksumInfo info = (Woff2Dec.TableChecksumInfo)o;
                    return tag == info.tag && offset == info.offset;
                }
                return false;
            }
        }

        private static int WithSign(int flag, int baseval) {
            // Precondition: 0 <= baseval < 65536 (to avoid integer overflow)
            return (flag & 1) != 0 ? baseval : -baseval;
        }

        private static int TripletDecode(byte[] data, int flags_in_offset, int in_offset, int in_size, int n_points
            , Woff2Common.Point[] result) {
            int x = 0;
            int y = 0;
            if (n_points > in_size) {
                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_GLYPH_FAILED);
            }
            int triplet_index = 0;
            for (int i = 0; i < n_points; ++i) {
                int flag = JavaUnsignedUtil.AsU8(data[i + flags_in_offset]);
                bool on_curve = (flag >> 7) == 0;
                flag &= 0x7f;
                int n_data_bytes;
                if (flag < 84) {
                    n_data_bytes = 1;
                }
                else {
                    if (flag < 120) {
                        n_data_bytes = 2;
                    }
                    else {
                        if (flag < 124) {
                            n_data_bytes = 3;
                        }
                        else {
                            n_data_bytes = 4;
                        }
                    }
                }
                if (triplet_index + n_data_bytes > in_size || triplet_index + n_data_bytes < triplet_index) {
                    throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_GLYPH_FAILED);
                }
                int dx;
                int dy;
                if (flag < 10) {
                    dx = 0;
                    dy = WithSign(flag, ((flag & 14) << 7) + JavaUnsignedUtil.AsU8(data[in_offset + triplet_index]));
                }
                else {
                    if (flag < 20) {
                        dx = WithSign(flag, (((flag - 10) & 14) << 7) + JavaUnsignedUtil.AsU8(data[in_offset + triplet_index]));
                        dy = 0;
                    }
                    else {
                        if (flag < 84) {
                            int b0 = flag - 20;
                            int b1 = JavaUnsignedUtil.AsU8(data[in_offset + triplet_index]);
                            dx = WithSign(flag, 1 + (b0 & 0x30) + (b1 >> 4));
                            dy = WithSign(flag >> 1, 1 + ((b0 & 0x0c) << 2) + (b1 & 0x0f));
                        }
                        else {
                            if (flag < 120) {
                                int b0 = flag - 84;
                                dx = WithSign(flag, 1 + ((b0 / 12) << 8) + JavaUnsignedUtil.AsU8(data[in_offset + triplet_index]));
                                dy = WithSign(flag >> 1, 1 + (((b0 % 12) >> 2) << 8) + JavaUnsignedUtil.AsU8(data[in_offset + triplet_index
                                     + 1]));
                            }
                            else {
                                if (flag < 124) {
                                    int b2 = JavaUnsignedUtil.AsU8(data[in_offset + triplet_index + 1]);
                                    dx = WithSign(flag, (JavaUnsignedUtil.AsU8(data[in_offset + triplet_index]) << 4) + (b2 >> 4));
                                    dy = WithSign(flag >> 1, ((b2 & 0x0f) << 8) + JavaUnsignedUtil.AsU8(data[in_offset + triplet_index + 2]));
                                }
                                else {
                                    dx = WithSign(flag, (JavaUnsignedUtil.AsU8(data[in_offset + triplet_index]) << 8) + JavaUnsignedUtil.AsU8(
                                        data[in_offset + triplet_index + 1]));
                                    dy = WithSign(flag >> 1, (JavaUnsignedUtil.AsU8(data[in_offset + triplet_index + 2]) << 8) + JavaUnsignedUtil.AsU8
                                        (data[in_offset + triplet_index + 3]));
                                }
                            }
                        }
                    }
                }
                triplet_index += n_data_bytes;
                // Possible overflow but coordinate values are not security sensitive
                x += dx;
                y += dy;
                result[i] = new Woff2Common.Point(x, y, on_curve);
            }
            return triplet_index;
        }

        // This function stores just the point data. On entry, dst points to the
        // beginning of a simple glyph. Returns total glyph size on success.
        private static int StorePoints(int n_points, Woff2Common.Point[] points, int n_contours, int instruction_length
            , byte[] dst, int dst_size) {
            // I believe that n_contours < 65536, in which case this is safe. However, a
            // comment and/or an assert would be good.
            int flag_offset = kEndPtsOfContoursOffset + 2 * n_contours + 2 + instruction_length;
            int last_flag = -1;
            int repeat_count = 0;
            int last_x = 0;
            int last_y = 0;
            int x_bytes = 0;
            int y_bytes = 0;
            for (int i = 0; i < n_points; ++i) {
                Woff2Common.Point point = points[i];
                int flag = point.on_curve ? kGlyfOnCurve : 0;
                int dx = point.x - last_x;
                int dy = point.y - last_y;
                if (dx == 0) {
                    flag |= kGlyfThisXIsSame;
                }
                else {
                    if (dx > -256 && dx < 256) {
                        flag |= kGlyfXShort | (dx > 0 ? kGlyfThisXIsSame : 0);
                        x_bytes += 1;
                    }
                    else {
                        x_bytes += 2;
                    }
                }
                if (dy == 0) {
                    flag |= kGlyfThisYIsSame;
                }
                else {
                    if (dy > -256 && dy < 256) {
                        flag |= kGlyfYShort | (dy > 0 ? kGlyfThisYIsSame : 0);
                        y_bytes += 1;
                    }
                    else {
                        y_bytes += 2;
                    }
                }
                if (flag == last_flag && repeat_count != 255) {
                    dst[flag_offset - 1] |= kGlyfRepeat;
                    repeat_count++;
                }
                else {
                    if (repeat_count != 0) {
                        if (flag_offset >= dst_size) {
                            throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_POINT_FAILED);
                        }
                        dst[flag_offset++] = (byte)repeat_count;
                    }
                    if (flag_offset >= dst_size) {
                        throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_POINT_FAILED);
                    }
                    dst[flag_offset++] = (byte)flag;
                    repeat_count = 0;
                }
                last_x = point.x;
                last_y = point.y;
                last_flag = flag;
            }
            if (repeat_count != 0) {
                if (flag_offset >= dst_size) {
                    throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_POINT_FAILED);
                }
                dst[flag_offset++] = (byte)repeat_count;
            }
            int xy_bytes = x_bytes + y_bytes;
            if (xy_bytes < x_bytes || flag_offset + xy_bytes < flag_offset || flag_offset + xy_bytes > dst_size) {
                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_POINT_FAILED);
            }
            int x_offset = flag_offset;
            int y_offset = flag_offset + x_bytes;
            last_x = 0;
            last_y = 0;
            for (int i = 0; i < n_points; ++i) {
                int dx = points[i].x - last_x;
                if (dx == 0) {
                }
                else {
                    // pass
                    if (dx > -256 && dx < 256) {
                        dst[x_offset++] = (byte)Math.Abs(dx);
                    }
                    else {
                        // will always fit for valid input, but overflow is harmless
                        x_offset = StoreBytes.StoreU16(dst, x_offset, dx);
                    }
                }
                last_x += dx;
                int dy = points[i].y - last_y;
                if (dy == 0) {
                }
                else {
                    // pass
                    if (dy > -256 && dy < 256) {
                        dst[y_offset++] = (byte)Math.Abs(dy);
                    }
                    else {
                        y_offset = StoreBytes.StoreU16(dst, y_offset, dy);
                    }
                }
                last_y += dy;
            }
            int glyph_size = y_offset;
            return glyph_size;
        }

        // Compute the bounding box of the coordinates, and store into a glyf buffer.
        // A precondition is that there are at least 10 bytes available.
        // dst should point to the beginning of a 'glyf' record.
        private static void ComputeBbox(int n_points, Woff2Common.Point[] points, byte[] dst) {
            int x_min = 0;
            int y_min = 0;
            int x_max = 0;
            int y_max = 0;
            if (n_points > 0) {
                x_min = points[0].x;
                x_max = points[0].x;
                y_min = points[0].y;
                y_max = points[0].y;
            }
            for (int i = 1; i < n_points; ++i) {
                int x = points[i].x;
                int y = points[i].y;
                x_min = Math.Min(x, x_min);
                x_max = Math.Max(x, x_max);
                y_min = Math.Min(y, y_min);
                y_max = Math.Max(y, y_max);
            }
            int offset = 2;
            offset = StoreBytes.StoreU16(dst, offset, x_min);
            offset = StoreBytes.StoreU16(dst, offset, y_min);
            offset = StoreBytes.StoreU16(dst, offset, x_max);
            offset = StoreBytes.StoreU16(dst, offset, y_max);
        }

        private static Woff2Dec.CompositeGlyphInfo SizeOfComposite(Buffer composite_stream) {
            //In c++ code the composite_stream is transferred by value so we need to recreate it in oder to not mess it up
            composite_stream = new Buffer(composite_stream);
            int start_offset = composite_stream.GetOffset();
            bool we_have_instructions = false;
            int flags = FLAG_MORE_COMPONENTS;
            while ((flags & FLAG_MORE_COMPONENTS) != 0) {
                flags = JavaUnsignedUtil.AsU16(composite_stream.ReadShort());
                we_have_instructions |= (flags & FLAG_WE_HAVE_INSTRUCTIONS) != 0;
                int arg_size = 2;
                // glyph index
                if ((flags & FLAG_ARG_1_AND_2_ARE_WORDS) != 0) {
                    arg_size += 4;
                }
                else {
                    arg_size += 2;
                }
                if ((flags & FLAG_WE_HAVE_A_SCALE) != 0) {
                    arg_size += 2;
                }
                else {
                    if ((flags & FLAG_WE_HAVE_AN_X_AND_Y_SCALE) != 0) {
                        arg_size += 4;
                    }
                    else {
                        if ((flags & FLAG_WE_HAVE_A_TWO_BY_TWO) != 0) {
                            arg_size += 8;
                        }
                    }
                }
                composite_stream.Skip(arg_size);
            }
            int size = composite_stream.GetOffset() - start_offset;
            bool have_instructions = we_have_instructions;
            return new Woff2Dec.CompositeGlyphInfo(size, have_instructions);
        }

        private class CompositeGlyphInfo {
            public int size;

            public bool have_instructions;

            public CompositeGlyphInfo(int size, bool have_instructions) {
                this.size = size;
                this.have_instructions = have_instructions;
            }
        }

        private static void Pad4(Woff2Out @out) {
            byte[] zeroes = new byte[] { 0, 0, 0 };
            if (@out.Size() + 3 < @out.Size()) {
                throw new FontCompressionException(IoExceptionMessageConstant.PADDING_OVERFLOW);
            }
            int pad_bytes = Round.Round4(@out.Size()) - @out.Size();
            if (pad_bytes > 0) {
                @out.Write(zeroes, 0, pad_bytes);
            }
        }

        // Build TrueType loca table. Returns loca_checksum
        private static int StoreLoca(int[] loca_values, int index_format, Woff2Out @out) {
            long loca_size = loca_values.Length;
            long offset_size = index_format != 0 ? 4 : 2;
            if ((loca_size << 2) >> 2 != loca_size) {
                throw new FontCompressionException(IoExceptionMessageConstant.LOCA_SIZE_OVERFLOW);
            }
            byte[] loca_content = new byte[(int)(loca_size * offset_size)];
            int offset = 0;
            for (int i = 0; i < loca_values.Length; ++i) {
                int value = loca_values[i];
                if (index_format != 0) {
                    offset = StoreBytes.StoreU32(loca_content, offset, value);
                }
                else {
                    offset = StoreBytes.StoreU16(loca_content, offset, value >> 1);
                }
            }
            int checksum = Woff2Common.ComputeULongSum(loca_content, 0, loca_content.Length);
            @out.Write(loca_content, 0, loca_content.Length);
            return checksum;
        }

        // Reconstruct entire glyf table based on transformed original
        private static Woff2Dec.Checksums ReconstructGlyf(byte[] data, int data_offset, Woff2Common.Table glyf_table
            , int glyph_checksum, Woff2Common.Table loca_table, int loca_checksum, Woff2Dec.Woff2FontInfo info, Woff2Out
             @out) {
            int kNumSubStreams = 7;
            Buffer file = new Buffer(data, data_offset, glyf_table.transform_length);
            List<Woff2Dec.StreamInfo> substreams = new List<Woff2Dec.StreamInfo>(kNumSubStreams);
            int glyf_start = @out.Size();
            //read and ignore version
            file.ReadInt();
            info.num_glyphs = file.ReadShort();
            info.index_format = file.ReadShort();
            int offset = (2 + kNumSubStreams) * 4;
            if (offset > glyf_table.transform_length) {
                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_GLYF_TABLE_FAILED);
            }
            // Invariant from here on: data_size >= offset
            for (int i = 0; i < kNumSubStreams; ++i) {
                int substream_size;
                substream_size = file.ReadInt();
                if (substream_size > glyf_table.transform_length - offset) {
                    throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_GLYF_TABLE_FAILED);
                }
                substreams.Add(new Woff2Dec.StreamInfo(data_offset + offset, substream_size));
                offset += substream_size;
            }
            Buffer n_contour_stream = new Buffer(data, substreams[0].offset, substreams[0].length);
            Buffer n_points_stream = new Buffer(data, substreams[1].offset, substreams[1].length);
            Buffer flag_stream = new Buffer(data, substreams[2].offset, substreams[2].length);
            Buffer glyph_stream = new Buffer(data, substreams[3].offset, substreams[3].length);
            Buffer composite_stream = new Buffer(data, substreams[4].offset, substreams[4].length);
            Buffer bbox_stream = new Buffer(data, substreams[5].offset, substreams[5].length);
            Buffer instruction_stream = new Buffer(data, substreams[6].offset, substreams[6].length);
            int[] loca_values = new int[JavaUnsignedUtil.AsU16(info.num_glyphs) + 1];
            List<int> n_points_vec = new List<int>();
            Woff2Common.Point[] points = new Woff2Common.Point[0];
            int points_size = 0;
            int bbox_bitmap_offset = bbox_stream.GetInitialOffset();
            // Safe because num_glyphs is bounded
            int bitmap_length = ((JavaUnsignedUtil.AsU16(info.num_glyphs) + 31) >> 5) << 2;
            bbox_stream.Skip(bitmap_length);
            // Temp buffer for glyph's.
            int glyph_buf_size = kDefaultGlyphBuf;
            byte[] glyph_buf = new byte[glyph_buf_size];
            info.x_mins = new short[JavaUnsignedUtil.AsU16(info.num_glyphs)];
            for (int i = 0; i < JavaUnsignedUtil.AsU16(info.num_glyphs); ++i) {
                int glyph_size = 0;
                int n_contours = 0;
                bool have_bbox = false;
                byte[] bitmap = new byte[bitmap_length];
                Array.Copy(data, bbox_bitmap_offset, bitmap, 0, bitmap_length);
                if ((data[bbox_bitmap_offset + (i >> 3)] & (0x80 >> (i & 7))) != 0) {
                    have_bbox = true;
                }
                n_contours = JavaUnsignedUtil.AsU16(n_contour_stream.ReadShort());
                if (n_contours == 0xffff) {
                    // composite glyph
                    bool have_instructions = false;
                    int instruction_size = 0;
                    if (!have_bbox) {
                        // composite glyphs must have an explicit bbox
                        throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_GLYF_TABLE_FAILED);
                    }
                    int composite_size;
                    Woff2Dec.CompositeGlyphInfo compositeGlyphInfo = SizeOfComposite(composite_stream);
                    have_instructions = compositeGlyphInfo.have_instructions;
                    composite_size = compositeGlyphInfo.size;
                    if (have_instructions) {
                        instruction_size = VariableLength.Read255UShort(glyph_stream);
                    }
                    int size_needed = 12 + composite_size + instruction_size;
                    if (glyph_buf_size < size_needed) {
                        glyph_buf = new byte[size_needed];
                        glyph_buf_size = size_needed;
                    }
                    glyph_size = StoreBytes.StoreU16(glyph_buf, glyph_size, n_contours);
                    bbox_stream.Read(glyph_buf, glyph_size, 8);
                    glyph_size += 8;
                    composite_stream.Read(glyph_buf, glyph_size, composite_size);
                    glyph_size += composite_size;
                    if (have_instructions) {
                        glyph_size = StoreBytes.StoreU16(glyph_buf, glyph_size, instruction_size);
                        instruction_stream.Read(glyph_buf, glyph_size, instruction_size);
                        glyph_size += instruction_size;
                    }
                }
                else {
                    if (n_contours > 0) {
                        // simple glyph
                        n_points_vec.Clear();
                        int total_n_points = 0;
                        int n_points_contour;
                        //Read numberOfContours 255UInt16 values from the nPoints stream. Each of these is the number of points of that contour.
                        //Convert this into the endPtsOfContours[] array by computing the cumulative sum, then subtracting one.
                        //For example, if the values in the stream are [2, 4], then the endPtsOfContours array is [1, 5].
                        //Also, the sum of all the values in the array is the total number of points in the glyph, nPoints. In the example given, the value of nPoints is 6.
                        for (int j = 0; j < n_contours; ++j) {
                            n_points_contour = VariableLength.Read255UShort(n_points_stream);
                            n_points_vec.Add(n_points_contour);
                            if (total_n_points + n_points_contour < total_n_points) {
                                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_GLYF_TABLE_FAILED);
                            }
                            total_n_points += n_points_contour;
                        }
                        int flag_size = total_n_points;
                        if (flag_size > flag_stream.GetLength() - flag_stream.GetOffset()) {
                            throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_GLYF_TABLE_FAILED);
                        }
                        int flags_buf_offset = flag_stream.GetInitialOffset() + flag_stream.GetOffset();
                        int triplet_buf_offset = glyph_stream.GetInitialOffset() + glyph_stream.GetOffset();
                        int triplet_size = glyph_stream.GetLength() - glyph_stream.GetOffset();
                        int triplet_bytes_consumed = 0;
                        if (points_size < total_n_points) {
                            points_size = total_n_points;
                            points = new Woff2Common.Point[points_size];
                        }
                        triplet_bytes_consumed = TripletDecode(data, flags_buf_offset, triplet_buf_offset, triplet_size, total_n_points
                            , points);
                        //Read nPoints UInt8 values from the flags stream. Each corresponds to one point in the reconstructed glyph outline.
                        //The interpretation of the flag byte is described in details in subclause 5.2.
                        flag_stream.Skip(flag_size);
                        glyph_stream.Skip(triplet_bytes_consumed);
                        int instruction_size;
                        instruction_size = VariableLength.Read255UShort(glyph_stream);
                        if (total_n_points >= (1 << 27) || instruction_size >= (1 << 30)) {
                            throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_GLYF_TABLE_FAILED);
                        }
                        int size_needed = 12 + 2 * n_contours + 5 * total_n_points + instruction_size;
                        if (glyph_buf_size < size_needed) {
                            glyph_buf = new byte[size_needed];
                            glyph_buf_size = size_needed;
                        }
                        glyph_size = StoreBytes.StoreU16(glyph_buf, glyph_size, n_contours);
                        if (have_bbox) {
                            bbox_stream.Read(glyph_buf, glyph_size, 8);
                        }
                        else {
                            ComputeBbox(total_n_points, points, glyph_buf);
                        }
                        glyph_size = kEndPtsOfContoursOffset;
                        int end_point = -1;
                        for (int contour_ix = 0; contour_ix < n_contours; ++contour_ix) {
                            end_point += n_points_vec[contour_ix];
                            if (end_point >= 65536) {
                                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_GLYF_TABLE_FAILED);
                            }
                            glyph_size = StoreBytes.StoreU16(glyph_buf, glyph_size, end_point);
                        }
                        glyph_size = StoreBytes.StoreU16(glyph_buf, glyph_size, instruction_size);
                        instruction_stream.Read(glyph_buf, glyph_size, instruction_size);
                        glyph_size += instruction_size;
                        glyph_size = StorePoints(total_n_points, points, n_contours, instruction_size, glyph_buf, glyph_buf_size);
                    }
                }
                loca_values[i] = @out.Size() - glyf_start;
                @out.Write(glyph_buf, 0, glyph_size);
                Pad4(@out);
                glyph_checksum += Woff2Common.ComputeULongSum(glyph_buf, 0, glyph_size);
                // We may need x_min to reconstruct 'hmtx'
                if (n_contours > 0) {
                    Buffer x_min_buf = new Buffer(glyph_buf, 2, 2);
                    info.x_mins[i] = x_min_buf.ReadShort();
                }
            }
            // glyf_table dst_offset was set by reconstructFont
            glyf_table.dst_length = @out.Size() - glyf_table.dst_offset;
            loca_table.dst_offset = @out.Size();
            // loca[n] will be equal the length of the glyph data ('glyf') table
            loca_values[JavaUnsignedUtil.AsU16(info.num_glyphs)] = glyf_table.dst_length;
            loca_checksum = StoreLoca(loca_values, info.index_format, @out);
            loca_table.dst_length = @out.Size() - loca_table.dst_offset;
            return new Woff2Dec.Checksums(loca_checksum, glyph_checksum);
        }

        private class Checksums {
            public int loca_checksum;

            public int glyph_checksum;

            public Checksums(int loca_checksum, int glyph_checksum) {
                this.loca_checksum = loca_checksum;
                this.glyph_checksum = glyph_checksum;
            }
        }

        private class StreamInfo {
            public int offset;

            public int length;

            public StreamInfo(int offset, int length) {
                this.offset = offset;
                this.length = length;
            }
        }

        private static Woff2Common.Table FindTable(List<Woff2Common.Table> tables, int tag) {
            foreach (Woff2Common.Table table in tables) {
                if (table.tag == tag) {
                    return table;
                }
            }
            return null;
        }

        // Get numberOfHMetrics, https://www.microsoft.com/typography/otspec/hhea.htm
        private static short ReadNumHMetrics(byte[] data, int offset, int data_length) {
            // Skip 34 to reach 'hhea' numberOfHMetrics
            Buffer buffer = new Buffer(data, offset, data_length);
            buffer.Skip(34);
            return buffer.ReadShort();
        }

        private static int ReconstructTransformedHmtx(byte[] transformed_buf, int transformed_offset, int transformed_size
            , int num_glyphs, int num_hmetrics, short[] x_mins, Woff2Out @out) {
            //uint16
            //uint16
            Buffer hmtx_buff_in = new Buffer(transformed_buf, transformed_offset, transformed_size);
            int hmtx_flags = JavaUnsignedUtil.AsU8(hmtx_buff_in.ReadByte());
            short[] advance_widths;
            short[] lsbs;
            bool has_proportional_lsbs = (hmtx_flags & 1) == 0;
            bool has_monospace_lsbs = (hmtx_flags & 2) == 0;
            // you say you transformed but there is little evidence of it
            if (has_proportional_lsbs && has_monospace_lsbs) {
                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_HMTX_TABLE_FAILED);
            }
            if (x_mins == null || x_mins.Length != num_glyphs) {
                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_HMTX_TABLE_FAILED);
            }
            // num_glyphs 0 is OK if there is no 'glyf' but cannot then xform 'hmtx'.
            if (num_hmetrics > num_glyphs) {
                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_HMTX_TABLE_FAILED);
            }
            // https://www.microsoft.com/typography/otspec/hmtx.htm
            // "...only one entry need be in the array, but that entry is required."
            if (num_hmetrics < 1) {
                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_HMTX_TABLE_FAILED);
            }
            advance_widths = new short[num_hmetrics];
            for (int i = 0; i < num_hmetrics; i++) {
                short advance_width;
                advance_width = hmtx_buff_in.ReadShort();
                //u16, but it doesn't meter since we only store them
                advance_widths[i] = advance_width;
            }
            lsbs = new short[num_glyphs];
            for (int i = 0; i < num_hmetrics; i++) {
                short lsb;
                if (has_proportional_lsbs) {
                    lsb = hmtx_buff_in.ReadShort();
                }
                else {
                    //u16, but it doesn't meter since we only store them
                    lsb = x_mins[i];
                }
                lsbs[i] = lsb;
            }
            for (int i = num_hmetrics; i < num_glyphs; i++) {
                short lsb;
                if (has_monospace_lsbs) {
                    lsb = hmtx_buff_in.ReadShort();
                }
                else {
                    //u16, but it doesn't meter since we only store them
                    lsb = x_mins[i];
                }
                lsbs[i] = lsb;
            }
            // bake me a shiny new hmtx table
            int hmtx_output_size = 2 * num_glyphs + 2 * num_hmetrics;
            byte[] hmtx_table = new byte[hmtx_output_size];
            int dst_offset = 0;
            for (int i = 0; i < num_glyphs; i++) {
                if (i < num_hmetrics) {
                    dst_offset = StoreBytes.StoreU16(hmtx_table, dst_offset, advance_widths[i]);
                }
                dst_offset = StoreBytes.StoreU16(hmtx_table, dst_offset, lsbs[i]);
            }
            int checksum = Woff2Common.ComputeULongSum(hmtx_table, 0, hmtx_output_size);
            @out.Write(hmtx_table, 0, hmtx_output_size);
            return checksum;
        }

        private static void Woff2Uncompress(byte[] dst_buf, int dst_offset, int dst_length, byte[] src_buf, int src_offset
            , int src_length) {
            int remain = dst_length;
            try {
                BrotliInputStream stream = new BrotliInputStream(new MemoryStream(src_buf, src_offset, src_length));
                while (remain > 0) {
                    int read = stream.JRead(dst_buf, dst_offset, dst_length);
                    if (read < 0) {
                        throw new FontCompressionException(IoExceptionMessageConstant.BROTLI_DECODING_FAILED);
                    }
                    remain -= read;
                }
                //check that we read stream fully
                if (stream.ReadByte() != -1) {
                    throw new FontCompressionException(IoExceptionMessageConstant.BROTLI_DECODING_FAILED);
                }
            }
            catch (System.IO.IOException) {
                throw new FontCompressionException(IoExceptionMessageConstant.BROTLI_DECODING_FAILED);
            }
            if (remain != 0) {
                throw new FontCompressionException(IoExceptionMessageConstant.BROTLI_DECODING_FAILED);
            }
        }

        private static void ReadTableDirectory(Buffer file, Woff2Common.Table[] tables, int num_tables) {
            int src_offset = 0;
            for (int i = 0; i < num_tables; ++i) {
                Woff2Common.Table table = new Woff2Common.Table();
                tables[i] = table;
                int flag_byte = JavaUnsignedUtil.AsU8(file.ReadByte());
                int tag;
                if ((flag_byte & 0x3f) == 0x3f) {
                    tag = file.ReadInt();
                }
                else {
                    tag = TableTags.kKnownTags[flag_byte & 0x3f];
                }
                int flags = 0;
                int xform_version = ((flag_byte >> 6) & 0x03);
                // 0 means xform for glyph/loca, non-0 for others
                if (tag == TableTags.kGlyfTableTag || tag == TableTags.kLocaTableTag) {
                    if (xform_version == 0) {
                        flags |= Woff2Common.kWoff2FlagsTransform;
                    }
                }
                else {
                    if (xform_version != 0) {
                        flags |= Woff2Common.kWoff2FlagsTransform;
                    }
                }
                flags |= xform_version;
                int dst_length = VariableLength.ReadBase128(file);
                int transform_length = dst_length;
                if ((flags & Woff2Common.kWoff2FlagsTransform) != 0) {
                    transform_length = VariableLength.ReadBase128(file);
                    if (tag == TableTags.kLocaTableTag && transform_length != 0) {
                        throw new FontCompressionException(IoExceptionMessageConstant.READ_TABLE_DIRECTORY_FAILED);
                    }
                }
                if (src_offset + transform_length < src_offset) {
                    throw new FontCompressionException(IoExceptionMessageConstant.READ_TABLE_DIRECTORY_FAILED);
                }
                table.src_offset = src_offset;
                table.src_length = transform_length;
                src_offset += transform_length;
                table.tag = tag;
                table.flags = flags;
                table.transform_length = transform_length;
                table.dst_length = dst_length;
            }
        }

        // Writes a single Offset Table entry
        private static int StoreOffsetTable(byte[] result, int offset, int flavor, int num_tables) {
            offset = StoreBytes.StoreU32(result, offset, flavor);
            // sfnt version
            offset = StoreBytes.StoreU16(result, offset, num_tables);
            // num_tables
            int max_pow2 = 0;
            while (1 << (max_pow2 + 1) <= num_tables) {
                max_pow2++;
            }
            int output_search_range = (1 << max_pow2) << 4;
            offset = StoreBytes.StoreU16(result, offset, output_search_range);
            // searchRange
            offset = StoreBytes.StoreU16(result, offset, max_pow2);
            // entrySelector
            // rangeShift
            offset = StoreBytes.StoreU16(result, offset, (num_tables << 4) - output_search_range);
            return offset;
        }

        private static int StoreTableEntry(byte[] result, int offset, int tag) {
            offset = StoreBytes.StoreU32(result, offset, tag);
            offset = StoreBytes.StoreU32(result, offset, 0);
            offset = StoreBytes.StoreU32(result, offset, 0);
            offset = StoreBytes.StoreU32(result, offset, 0);
            return offset;
        }

        // First table goes after all the headers, table directory, etc
        private static int ComputeOffsetToFirstTable(Woff2Dec.Woff2Header hdr) {
            int offset = Woff2Common.kSfntHeaderSize + Woff2Common.kSfntEntrySize * hdr.num_tables;
            if (hdr.header_version != 0) {
                offset = Woff2Common.CollectionHeaderSize(hdr.header_version, hdr.ttc_fonts.Length) + Woff2Common.kSfntHeaderSize
                     * hdr.ttc_fonts.Length;
                foreach (Woff2Dec.TtcFont ttc_font in hdr.ttc_fonts) {
                    offset += Woff2Common.kSfntEntrySize * ttc_font.table_indices.Length;
                }
            }
            return offset;
        }

        private static List<Woff2Common.Table> Tables(Woff2Dec.Woff2Header hdr, int font_index) {
            List<Woff2Common.Table> tables = new List<Woff2Common.Table>();
            if (hdr.header_version != 0) {
                foreach (short index in hdr.ttc_fonts[font_index].table_indices) {
                    tables.Add(hdr.tables[JavaUnsignedUtil.AsU16(index)]);
                }
            }
            else {
                tables.AddAll(JavaUtil.ArraysAsList(hdr.tables));
            }
            return tables;
        }

        private static void ReconstructFont(byte[] transformed_buf, int transformed_buf_offset, int transformed_buf_size
            , Woff2Dec.RebuildMetadata metadata, Woff2Dec.Woff2Header hdr, int font_index, Woff2Out @out) {
            int dest_offset = @out.Size();
            byte[] table_entry = new byte[12];
            Woff2Dec.Woff2FontInfo info = metadata.font_infos[font_index];
            List<Woff2Common.Table> tables = Tables(hdr, font_index);
            // 'glyf' without 'loca' doesn't make sense
            if ((FindTable(tables, TableTags.kGlyfTableTag) == null) == (FindTable(tables, TableTags.kLocaTableTag) !=
                 null)) {
                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_TABLE_DIRECTORY_FAILED);
            }
            int font_checksum = metadata.header_checksum;
            if (hdr.header_version != 0) {
                font_checksum = hdr.ttc_fonts[font_index].header_checksum;
            }
            int loca_checksum = 0;
            for (int i = 0; i < tables.Count; i++) {
                Woff2Common.Table table = tables[i];
                Woff2Dec.TableChecksumInfo checksum_key = new Woff2Dec.TableChecksumInfo(table.tag, table.src_offset);
                bool reused = metadata.checksums.ContainsKey(checksum_key);
                if (font_index == 0 && reused) {
                    throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_TABLE_DIRECTORY_FAILED);
                }
                if (((long)table.src_offset) + table.src_length > transformed_buf_size) {
                    throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_TABLE_DIRECTORY_FAILED);
                }
                if (table.tag == TableTags.kHheaTableTag) {
                    info.num_hmetrics = ReadNumHMetrics(transformed_buf, transformed_buf_offset + table.src_offset, table.src_length
                        );
                }
                int checksum = 0;
                if (!reused) {
                    if ((table.flags & Woff2Common.kWoff2FlagsTransform) != Woff2Common.kWoff2FlagsTransform) {
                        if (table.tag == TableTags.kHeadTableTag) {
                            if (table.src_length < 12) {
                                throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_TABLE_DIRECTORY_FAILED);
                            }
                            // checkSumAdjustment = 0
                            StoreBytes.StoreU32(transformed_buf, transformed_buf_offset + table.src_offset + 8, 0);
                        }
                        table.dst_offset = dest_offset;
                        checksum = Woff2Common.ComputeULongSum(transformed_buf, transformed_buf_offset + table.src_offset, table.src_length
                            );
                        @out.Write(transformed_buf, transformed_buf_offset + table.src_offset, table.src_length);
                    }
                    else {
                        if (table.tag == TableTags.kGlyfTableTag) {
                            table.dst_offset = dest_offset;
                            Woff2Common.Table loca_table = FindTable(tables, TableTags.kLocaTableTag);
                            Woff2Dec.Checksums resultChecksum = ReconstructGlyf(transformed_buf, transformed_buf_offset + table.src_offset
                                , table, checksum, loca_table, loca_checksum, info, @out);
                            checksum = resultChecksum.glyph_checksum;
                            loca_checksum = resultChecksum.loca_checksum;
                        }
                        else {
                            if (table.tag == TableTags.kLocaTableTag) {
                                // All the work was done by reconstructGlyf. We already know checksum.
                                checksum = loca_checksum;
                            }
                            else {
                                if (table.tag == TableTags.kHmtxTableTag) {
                                    table.dst_offset = dest_offset;
                                    // Tables are sorted so all the info we need has been gathered.
                                    checksum = ReconstructTransformedHmtx(transformed_buf, transformed_buf_offset + table.src_offset, table.src_length
                                        , JavaUnsignedUtil.AsU16(info.num_glyphs), JavaUnsignedUtil.AsU16(info.num_hmetrics), info.x_mins, @out
                                        );
                                }
                                else {
                                    throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_TABLE_DIRECTORY_FAILED);
                                }
                            }
                        }
                    }
                    // transform unknown
                    metadata.checksums.Put(checksum_key, checksum);
                }
                else {
                    checksum = metadata.checksums.Get(checksum_key).Value;
                }
                font_checksum += checksum;
                // update the table entry with real values.
                StoreBytes.StoreU32(table_entry, 0, checksum);
                StoreBytes.StoreU32(table_entry, 4, table.dst_offset);
                StoreBytes.StoreU32(table_entry, 8, table.dst_length);
                @out.Write(table_entry, 0, info.table_entry_by_tag.Get(table.tag).Value + 4, 12);
                // We replaced 0's. Update overall checksum.
                font_checksum += Woff2Common.ComputeULongSum(table_entry, 0, 12);
                Pad4(@out);
                if (((long)table.dst_offset) + table.dst_length > @out.Size()) {
                    throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_TABLE_DIRECTORY_FAILED);
                }
                dest_offset = @out.Size();
            }
            // Update 'head' checkSumAdjustment. We already set it to 0 and summed font.
            Woff2Common.Table head_table = FindTable(tables, TableTags.kHeadTableTag);
            if (head_table != null) {
                if (head_table.dst_length < 12) {
                    throw new FontCompressionException(IoExceptionMessageConstant.RECONSTRUCT_TABLE_DIRECTORY_FAILED);
                }
                byte[] checksum_adjustment = new byte[4];
                StoreBytes.StoreU32(checksum_adjustment, 0, (int)(0xB1B0AFBA - font_checksum));
                @out.Write(checksum_adjustment, 0, head_table.dst_offset + 8, 4);
            }
        }

        private static void ReadWoff2Header(byte[] data, int length, Woff2Dec.Woff2Header hdr) {
            Buffer file = new Buffer(data, 0, length);
            int signature;
            signature = file.ReadInt();
            if (signature != Woff2Common.kWoff2Signature) {
                throw new FontCompressionException(IoExceptionMessageConstant.INCORRECT_SIGNATURE);
            }
            hdr.flavor = file.ReadInt();
            int reported_length = file.ReadInt();
            System.Diagnostics.Debug.Assert(reported_length > 0);
            if (length != reported_length) {
                throw new FontCompressionException(IoExceptionMessageConstant.READ_HEADER_FAILED);
            }
            hdr.num_tables = file.ReadShort();
            if (hdr.num_tables == 0) {
                throw new FontCompressionException(IoExceptionMessageConstant.READ_HEADER_FAILED);
            }
            // We don't care about these fields of the header:
            //   uint16_t reserved
            //   uint32_t total_sfnt_size, we don't believe this, will compute later
            file.Skip(6);
            hdr.compressed_length = file.ReadInt();
            System.Diagnostics.Debug.Assert(hdr.compressed_length >= 0);
            // We don't care about these fields of the header:
            //   uint16_t major_version, minor_version
            file.Skip(2 * 2);
            int meta_offset;
            int meta_length;
            int meta_length_orig;
            meta_offset = file.ReadInt();
            System.Diagnostics.Debug.Assert(meta_offset >= 0);
            meta_length = file.ReadInt();
            System.Diagnostics.Debug.Assert(meta_length >= 0);
            meta_length_orig = file.ReadInt();
            System.Diagnostics.Debug.Assert(meta_length_orig >= 0);
            if (meta_offset != 0) {
                if (meta_offset >= length || length - meta_offset < meta_length) {
                    throw new FontCompressionException(IoExceptionMessageConstant.READ_HEADER_FAILED);
                }
            }
            int priv_offset;
            int priv_length;
            priv_offset = file.ReadInt();
            System.Diagnostics.Debug.Assert(priv_offset >= 0);
            priv_length = file.ReadInt();
            System.Diagnostics.Debug.Assert(priv_length >= 0);
            if (priv_offset != 0) {
                if (priv_offset >= length || length - priv_offset < priv_length) {
                    throw new FontCompressionException(IoExceptionMessageConstant.READ_HEADER_FAILED);
                }
            }
            hdr.tables = new Woff2Common.Table[hdr.num_tables];
            ReadTableDirectory(file, hdr.tables, hdr.num_tables);
            // Before we sort for output the last table end is the uncompressed size.
            Woff2Common.Table last_table = hdr.tables[hdr.tables.Length - 1];
            hdr.uncompressed_size = last_table.src_offset + last_table.src_length;
            System.Diagnostics.Debug.Assert(hdr.uncompressed_size > 0);
            if (hdr.uncompressed_size < last_table.src_offset) {
                throw new FontCompressionException(IoExceptionMessageConstant.READ_HEADER_FAILED);
            }
            hdr.header_version = 0;
            if (hdr.flavor == Woff2Common.kTtcFontFlavor) {
                hdr.header_version = file.ReadInt();
                if (hdr.header_version != 0x00010000 && hdr.header_version != 0x00020000) {
                    throw new FontCompressionException(IoExceptionMessageConstant.READ_COLLECTION_HEADER_FAILED);
                }
                int num_fonts;
                num_fonts = VariableLength.Read255UShort(file);
                hdr.ttc_fonts = new Woff2Dec.TtcFont[num_fonts];
                for (int i = 0; i < num_fonts; i++) {
                    Woff2Dec.TtcFont ttc_font = new Woff2Dec.TtcFont();
                    hdr.ttc_fonts[i] = ttc_font;
                    int num_tables;
                    num_tables = VariableLength.Read255UShort(file);
                    ttc_font.flavor = file.ReadInt();
                    ttc_font.table_indices = new short[num_tables];
                    Woff2Common.Table glyf_table = null;
                    Woff2Common.Table loca_table = null;
                    for (int j = 0; j < num_tables; j++) {
                        int table_idx;
                        table_idx = VariableLength.Read255UShort(file);
                        if (table_idx >= hdr.tables.Length) {
                            throw new FontCompressionException(IoExceptionMessageConstant.READ_COLLECTION_HEADER_FAILED);
                        }
                        ttc_font.table_indices[j] = (short)table_idx;
                        Woff2Common.Table table = hdr.tables[table_idx];
                        if (table.tag == TableTags.kLocaTableTag) {
                            loca_table = table;
                        }
                        if (table.tag == TableTags.kGlyfTableTag) {
                            glyf_table = table;
                        }
                    }
                    if ((glyf_table == null) != (loca_table == null)) {
                        throw new FontCompressionException(IoExceptionMessageConstant.READ_COLLECTION_HEADER_FAILED);
                    }
                }
            }
            hdr.compressed_offset = file.GetOffset();
            int src_offset = Round.Round4(hdr.compressed_offset + hdr.compressed_length);
            if (src_offset > length) {
                throw new FontCompressionException(IoExceptionMessageConstant.READ_HEADER_FAILED);
            }
            if (meta_offset != 0) {
                if (src_offset != meta_offset) {
                    throw new FontCompressionException(IoExceptionMessageConstant.READ_HEADER_FAILED);
                }
                src_offset = Round.Round4(meta_offset + meta_length);
            }
            if (priv_offset != 0) {
                if (src_offset != priv_offset) {
                    throw new FontCompressionException(IoExceptionMessageConstant.READ_HEADER_FAILED);
                }
                src_offset = Round.Round4(priv_offset + priv_length);
            }
            if (src_offset != Round.Round4(length)) {
                throw new FontCompressionException(IoExceptionMessageConstant.READ_HEADER_FAILED);
            }
        }

        // Write everything before the actual table data
        private static void WriteHeaders(byte[] data, int length, Woff2Dec.RebuildMetadata metadata, Woff2Dec.Woff2Header
             hdr, Woff2Out @out) {
            byte[] output = new byte[ComputeOffsetToFirstTable(hdr)];
            // Re-order tables in output (OTSpec) order
            IList<Woff2Common.Table> sorted_tables = new List<Woff2Common.Table>(JavaUtil.ArraysAsList(hdr.tables));
            if (hdr.header_version != 0) {
                // collection font; we have to sort the table offset vector in each font
                foreach (Woff2Dec.TtcFont ttc_font in hdr.ttc_fonts) {
                    IDictionary<int, short> sorted_index_by_tag = new SortedDictionary<int, short>();
                    foreach (short table_index in ttc_font.table_indices) {
                        sorted_index_by_tag.Put(hdr.tables[table_index].tag, table_index);
                    }
                    short index = 0;
                    foreach (KeyValuePair<int, short> i in sorted_index_by_tag) {
                        ttc_font.table_indices[index++] = i.Value;
                    }
                }
            }
            else {
                // non-collection font; we can just sort the tables
                JavaCollectionsUtil.Sort(sorted_tables);
            }
            // Start building the font
            int offset = 0;
            if (hdr.header_version != 0) {
                // TTC header
                offset = StoreBytes.StoreU32(output, offset, hdr.flavor);
                // TAG TTCTag
                offset = StoreBytes.StoreU32(output, offset, hdr.header_version);
                // FIXED Version
                offset = StoreBytes.StoreU32(output, offset, hdr.ttc_fonts.Length);
                // ULONG numFonts
                // Space for ULONG OffsetTable[numFonts] (zeroed initially)
                int offset_table = offset;
                // keep start of offset table for later
                for (int i = 0; i < hdr.ttc_fonts.Length; ++i) {
                    offset = StoreBytes.StoreU32(output, offset, 0);
                }
                // will fill real values in later
                // space for DSIG fields for header v2
                if (hdr.header_version == 0x00020000) {
                    offset = StoreBytes.StoreU32(output, offset, 0);
                    // ULONG ulDsigTag
                    offset = StoreBytes.StoreU32(output, offset, 0);
                    // ULONG ulDsigLength
                    offset = StoreBytes.StoreU32(output, offset, 0);
                }
                // ULONG ulDsigOffset
                // write Offset Tables and store the location of each in TTC Header
                metadata.font_infos = new Woff2Dec.Woff2FontInfo[hdr.ttc_fonts.Length];
                for (int i = 0; i < hdr.ttc_fonts.Length; ++i) {
                    Woff2Dec.TtcFont ttc_font = hdr.ttc_fonts[i];
                    // write Offset Table location into TTC Header
                    offset_table = StoreBytes.StoreU32(output, offset_table, offset);
                    // write the actual offset table so our header doesn't lie
                    ttc_font.dst_offset = offset;
                    offset = StoreOffsetTable(output, offset, ttc_font.flavor, ttc_font.table_indices.Length);
                    metadata.font_infos[i] = new Woff2Dec.Woff2FontInfo();
                    foreach (short table_index in ttc_font.table_indices) {
                        int tag = hdr.tables[table_index].tag;
                        metadata.font_infos[i].table_entry_by_tag.Put(tag, offset);
                        offset = StoreTableEntry(output, offset, tag);
                    }
                    ttc_font.header_checksum = Woff2Common.ComputeULongSum(output, ttc_font.dst_offset, offset - ttc_font.dst_offset
                        );
                }
            }
            else {
                metadata.font_infos = new Woff2Dec.Woff2FontInfo[1];
                offset = StoreOffsetTable(output, offset, hdr.flavor, hdr.num_tables);
                metadata.font_infos[0] = new Woff2Dec.Woff2FontInfo();
                for (int i = 0; i < hdr.num_tables; ++i) {
                    metadata.font_infos[0].table_entry_by_tag.Put(sorted_tables[i].tag, offset);
                    offset = StoreTableEntry(output, offset, sorted_tables[i].tag);
                }
            }
            @out.Write(output, 0, output.Length);
            metadata.header_checksum = Woff2Common.ComputeULongSum(output, 0, output.Length);
        }

        // Compute the size of the final uncompressed font, or throws exception on error.
        public static int ComputeWoff2FinalSize(byte[] data, int length) {
            Buffer file = new Buffer(data, 0, length);
            file.Skip(16);
            return file.ReadInt();
        }

        // Decompresses the font into out. Returns true on success.
        // Works even if WOFF2Header totalSfntSize is wrong.
        // Please prefer this API.
        public static void ConvertWoff2ToTtf(byte[] data, int length, Woff2Out @out) {
            Woff2Dec.RebuildMetadata metadata = new Woff2Dec.RebuildMetadata();
            Woff2Dec.Woff2Header hdr = new Woff2Dec.Woff2Header();
            ReadWoff2Header(data, length, hdr);
            WriteHeaders(data, length, metadata, hdr, @out);
            float compression_ratio = (float)hdr.uncompressed_size / length;
            if (compression_ratio > kMaxPlausibleCompressionRatio) {
                throw new FontCompressionException(MessageFormatUtil.Format("Implausible compression ratio {0}", compression_ratio
                    ));
            }
            byte[] uncompressed_buf = new byte[hdr.uncompressed_size];
            Woff2Uncompress(uncompressed_buf, 0, hdr.uncompressed_size, data, hdr.compressed_offset, hdr.compressed_length
                );
            for (int i = 0; i < metadata.font_infos.Length; i++) {
                ReconstructFont(uncompressed_buf, 0, hdr.uncompressed_size, metadata, hdr, i, @out);
            }
        }
    }
//\endcond
}
