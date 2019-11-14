//Copyright (c) 2006, Adobe Systems Incorporated
//All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//        3. All advertising materials mentioning features or use of this software
//        must display the following acknowledgement:
//        This product includes software developed by the Adobe Systems Incorporated.
//        4. Neither the name of the Adobe Systems Incorporated nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED BY ADOBE SYSTEMS INCORPORATED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL ADOBE SYSTEMS INCORPORATED BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//        http://www.adobe.com/devnet/xmp/library/eula-xmp-library-java.html
using System;
using System.Collections;
using System.Text;

namespace iText.Kernel.XMP.Impl.XPath {
    /// <summary>Representates an XMP XMPPath with segment accessor methods.</summary>
    /// <since>28.02.2006</since>
    public class XMPPath {
        // Bits for XPathStepInfo options.
        /// <summary>Marks a struct field step , also for top level nodes (schema "fields").</summary>
        public const int STRUCT_FIELD_STEP = 0x01;

        /// <summary>Marks a qualifier step.</summary>
        /// <remarks>
        /// Marks a qualifier step.
        /// Note: Order is significant to separate struct/qual from array kinds!
        /// </remarks>
        public const int QUALIFIER_STEP = 0x02;

        // 
        /// <summary>Marks an array index step</summary>
        public const int ARRAY_INDEX_STEP = 0x03;

        public const int ARRAY_LAST_STEP = 0x04;

        public const int QUAL_SELECTOR_STEP = 0x05;

        public const int FIELD_SELECTOR_STEP = 0x06;

        public const int SCHEMA_NODE = unchecked((int)(0x80000000));

        public const int STEP_SCHEMA = 0;

        public const int STEP_ROOT_PROP = 1;

        /// <summary>stores the segments of an XMPPath</summary>
        private IList segments = new ArrayList(5);

        /// <summary>Append a path segment</summary>
        /// <param name="segment">the segment to add</param>
        public virtual void Add(XMPPathSegment segment) {
            segments.Add(segment);
        }

        /// <param name="index">the index of the segment to return</param>
        /// <returns>Returns a path segment.</returns>
        public virtual XMPPathSegment GetSegment(int index) {
            return (XMPPathSegment)segments[index];
        }

        /// <returns>Returns the size of the xmp path.</returns>
        public virtual int Size() {
            return segments.Count;
        }

        /// <summary>Return a single String explaining which certificate was verified, how and why.</summary>
        /// <seealso cref="System.Object.ToString()"/>
        public override String ToString() {
            StringBuilder result = new StringBuilder();
            int index = 1;
            while (index < Size()) {
                result.Append(GetSegment(index));
                if (index < Size() - 1) {
                    int kind = GetSegment(index + 1).GetKind();
                    if (kind == STRUCT_FIELD_STEP || kind == QUALIFIER_STEP) {
                        // all but last and array indices
                        result.Append('/');
                    }
                }
                index++;
            }
            return result.ToString();
        }
    }
}
