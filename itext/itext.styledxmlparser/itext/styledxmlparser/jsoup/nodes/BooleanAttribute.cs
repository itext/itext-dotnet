/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>A boolean attribute that is written out without any value.</summary>
    public class BooleanAttribute : iText.StyledXmlParser.Jsoup.Nodes.Attribute {
        /// <summary>Create a new boolean attribute from unencoded (raw) key.</summary>
        /// <param name="key">attribute key</param>
        public BooleanAttribute(String key)
            : base(key, "") {
        }

        protected internal override bool IsBooleanAttribute() {
            return true;
        }
    }
}
