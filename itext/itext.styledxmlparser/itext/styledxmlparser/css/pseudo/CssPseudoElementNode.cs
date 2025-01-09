/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Pseudo {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Node.IElementNode"/>
    /// implementation for pseudo elements.
    /// </summary>
    public class CssPseudoElementNode : CssContextNode, IElementNode, ICustomElementNode {
        /// <summary>The pseudo element name.</summary>
        private String pseudoElementName;

        /// <summary>The pseudo element tag name.</summary>
        private String pseudoElementTagName;

        /// <summary>
        /// Creates a new
        /// <see cref="CssPseudoElementNode"/>
        /// instance.
        /// </summary>
        /// <param name="parentNode">the parent node</param>
        /// <param name="pseudoElementName">the pseudo element name</param>
        public CssPseudoElementNode(INode parentNode, String pseudoElementName)
            : base(parentNode) {
            this.pseudoElementName = pseudoElementName;
            this.pseudoElementTagName = CssPseudoElementUtil.CreatePseudoElementTagName(pseudoElementName);
        }

        /// <summary>Gets the pseudo element name.</summary>
        /// <returns>the pseudo element name</returns>
        public virtual String GetPseudoElementName() {
            return pseudoElementName;
        }

        /// <summary><inheritDoc/></summary>
        public virtual String Name() {
            return pseudoElementTagName;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IAttributes GetAttributes() {
            return new CssPseudoElementNode.AttributesStub();
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetAttribute(String key) {
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IList<IDictionary<String, String>> GetAdditionalHtmlStyles() {
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void AddAdditionalHtmlStyles(IDictionary<String, String> styles) {
            throw new NotSupportedException();
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetLang() {
            return null;
        }

        /// <summary>
        /// A simple
        /// <see cref="iText.StyledXmlParser.Node.IAttributes"/>
        /// implementation.
        /// </summary>
        private class AttributesStub : IAttributes {
            /// <summary><inheritDoc/></summary>
            public virtual String GetAttribute(String key) {
                return null;
            }

            /// <summary><inheritDoc/></summary>
            public virtual void SetAttribute(String key, String value) {
                throw new NotSupportedException();
            }

            /// <summary><inheritDoc/></summary>
            public virtual int Size() {
                return 0;
            }

            /// <summary><inheritDoc/></summary>
            public virtual IEnumerator<IAttribute> GetEnumerator() {
                return JavaCollectionsUtil.EmptyIterator<IAttribute>();
            }

        /// <summary><inheritDoc/></summary>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        }
    }
}
