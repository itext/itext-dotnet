/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
using System.Text;

namespace iText.Layout.Hyphenation {
    /// <summary>Represents a hyphen.</summary>
    public class Hyphen {
        /// <summary>pre break string</summary>
        public String preBreak;

        /// <summary>no break string</summary>
        public String noBreak;

        /// <summary>post break string</summary>
        public String postBreak;

//\cond DO_NOT_DOCUMENT
        /// <summary>Construct a hyphen.</summary>
        /// <param name="pre">break string</param>
        /// <param name="no">break string</param>
        /// <param name="post">break string</param>
        internal Hyphen(String pre, String no, String post) {
            preBreak = pre;
            noBreak = no;
            postBreak = post;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Construct a hyphen.</summary>
        /// <param name="pre">break string</param>
        internal Hyphen(String pre) {
            preBreak = pre;
            noBreak = null;
            postBreak = null;
        }
//\endcond

        /// <summary>
        /// <inheritDoc/>
        /// 
        /// </summary>
        public override String ToString() {
            if (noBreak == null && postBreak == null && preBreak != null && preBreak.Equals("-")) {
                return "-";
            }
            StringBuilder res = new StringBuilder("{");
            res.Append(preBreak);
            res.Append("}{");
            res.Append(postBreak);
            res.Append("}{");
            res.Append(noBreak);
            res.Append('}');
            return res.ToString();
        }
    }
}
