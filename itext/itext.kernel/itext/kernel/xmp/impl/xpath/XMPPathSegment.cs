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

namespace iText.Kernel.XMP.Impl.XPath {
    /// <summary>A segment of a parsed <c>XMPPath</c>.</summary>
    /// <since>23.06.2006</since>
    public class XMPPathSegment {
        /// <summary>name of the path segment</summary>
        private String name;

        /// <summary>kind of the path segment</summary>
        private int kind;

        /// <summary>flag if segment is an alias</summary>
        private bool alias;

        /// <summary>alias form if applicable</summary>
        private int aliasForm;

        /// <summary>Constructor with initial values.</summary>
        /// <param name="name">the name of the segment</param>
        public XMPPathSegment(String name) {
            this.name = name;
        }

        /// <summary>Constructor with initial values.</summary>
        /// <param name="name">the name of the segment</param>
        /// <param name="kind">the kind of the segment</param>
        public XMPPathSegment(String name, int kind) {
            this.name = name;
            this.kind = kind;
        }

        /// <returns>Returns the kind.</returns>
        public virtual int GetKind() {
            return kind;
        }

        /// <param name="kind">The kind to set.</param>
        public virtual void SetKind(int kind) {
            this.kind = kind;
        }

        /// <returns>Returns the name.</returns>
        public virtual String GetName() {
            return name;
        }

        /// <param name="name">The name to set.</param>
        public virtual void SetName(String name) {
            this.name = name;
        }

        /// <param name="alias">the flag to set</param>
        public virtual void SetAlias(bool alias) {
            this.alias = alias;
        }

        /// <returns>Returns the alias.</returns>
        public virtual bool IsAlias() {
            return alias;
        }

        /// <returns>Returns the aliasForm if this segment has been created by an alias.</returns>
        public virtual int GetAliasForm() {
            return aliasForm;
        }

        /// <param name="aliasForm">the aliasForm to set</param>
        public virtual void SetAliasForm(int aliasForm) {
            this.aliasForm = aliasForm;
        }

        /// <seealso cref="System.Object.ToString()"/>
        public override String ToString() {
            switch (kind) {
                case XMPPath.STRUCT_FIELD_STEP:
                case XMPPath.ARRAY_INDEX_STEP:
                case XMPPath.QUALIFIER_STEP:
                case XMPPath.ARRAY_LAST_STEP: {
                    return name;
                }

                case XMPPath.QUAL_SELECTOR_STEP:
                case XMPPath.FIELD_SELECTOR_STEP: {
                    return name;
                }

                default: {
                    // no defined step
                    return name;
                }
            }
        }
    }
}
