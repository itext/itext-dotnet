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
using iText.IO.Exceptions;

namespace iText.IO.Font.Woff2 {
    /// <summary>Fixed memory block for woff2 out.</summary>
    internal class Woff2MemoryOut : Woff2Out {
        private byte[] buf_;

        private int buf_size_;

        private int offset_;

        public Woff2MemoryOut(byte[] buf_, int buf_size_) {
            this.buf_ = buf_;
            this.buf_size_ = buf_size_;
            this.offset_ = 0;
        }

        public virtual void Write(byte[] buf, int buff_offset, int n) {
            Write(buf, buff_offset, offset_, n);
        }

        public virtual void Write(byte[] buf, int buff_offset, int offset, int n) {
            if (offset > buf_size_ || n > buf_size_ - offset) {
                throw new FontCompressionException(FontCompressionException.WRITE_FAILED);
            }
            Array.Copy(buf, buff_offset, buf_, offset, n);
            offset_ = Math.Max(offset_, offset + n);
        }

        public virtual int Size() {
            return offset_;
        }
    }
}
