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
using System.Diagnostics;
using System.Xml;
using iText.IO.Util;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;

namespace iText.Kernel.XMP.Impl
{
	/// <summary>Parser for "normal" XML serialisation of RDF.</summary>
	/// <since>14.07.2006</since>
	public class ParseRdf : XMPConst
	{
		public const int RDFTERM_OTHER = 0;

		/// <summary>Start of coreSyntaxTerms.</summary>
		public const int RDFTERM_RDF = 1;

		public const int RDFTERM_ID = 2;

		public const int RDFTERM_ABOUT = 3;

		public const int RDFTERM_PARSE_TYPE = 4;

		public const int RDFTERM_RESOURCE = 5;

		public const int RDFTERM_NODE_ID = 6;

		/// <summary>End of coreSyntaxTerms</summary>
		public const int RDFTERM_DATATYPE = 7;

		/// <summary>Start of additions for syntax Terms.</summary>
		public const int RDFTERM_DESCRIPTION = 8;

		/// <summary>End of of additions for syntaxTerms.</summary>
		public const int RDFTERM_LI = 9;

		/// <summary>Start of oldTerms.</summary>
		public const int RDFTERM_ABOUT_EACH = 10;

		public const int RDFTERM_ABOUT_EACH_PREFIX = 11;

		/// <summary>End of oldTerms.</summary>
		public const int RDFTERM_BAG_ID = 12;

		public const int RDFTERM_FIRST_CORE = RDFTERM_RDF;

		public const int RDFTERM_LAST_CORE = RDFTERM_DATATYPE;

		/// <summary>! Yes, the syntax terms include the core terms.</summary>
		public const int RDFTERM_FIRST_SYNTAX = RDFTERM_FIRST_CORE;

		public const int RDFTERM_LAST_SYNTAX = RDFTERM_LI;

		public const int RDFTERM_FIRST_OLD = RDFTERM_ABOUT_EACH;

		public const int RDFTERM_LAST_OLD = RDFTERM_BAG_ID;

		/// <summary>this prefix is used for default namespaces</summary>
		public const String DEFAULT_PREFIX = "_dflt";

		//\cond DO_NOT_DOCUMENT		
		/// <summary>The main parsing method.</summary>
		/// <remarks>
		/// The main parsing method. The XML tree is walked through from the root node and and XMP tree
		/// is created. This is a raw parse, the normalisation of the XMP tree happens outside.
		/// </remarks>
		/// <param name="xmlRoot">the XML root node</param>
		/// <returns>Returns an XMP metadata object (not normalized)</returns>
		internal static XMPMetaImpl Parse(XmlNode xmlRoot)
		{
			XMPMetaImpl xmp = new XMPMetaImpl();
			Rdf_RDF(xmp, xmlRoot);
			return xmp;
		}

		/// <summary>
		/// Each of these parsing methods is responsible for recognizing an RDF
		/// syntax production and adding the appropriate structure to the XMP tree.
		/// </summary>
		/// <remarks>
		/// Each of these parsing methods is responsible for recognizing an RDF
		/// syntax production and adding the appropriate structure to the XMP tree.
		/// They simply return for success, failures will throw an exception.
		/// </remarks>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="rdfRdfNode">the top-level xml node</param>
		internal static void Rdf_RDF(XMPMetaImpl xmp, XmlNode rdfRdfNode)
		{
			if (rdfRdfNode.Attributes != null && rdfRdfNode.Attributes.Count > 0) {
				Rdf_NodeElementList(xmp, xmp.GetRoot(), rdfRdfNode);
			} else {
				throw new XMPException("Invalid attributes of rdf:RDF element", XMPError.BADRDF);
			}
		}
		//\endcond	

		/// <summary>
		/// 7.2.10 nodeElementList<br />
		/// ws* ( nodeElement ws* )
		/// Note: this method is only called from the rdf:RDF-node (top level)
		/// </summary>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="rdfRdfNode">the top-level xml node</param>
		private static void Rdf_NodeElementList(XMPMetaImpl xmp, XMPNode xmpParent, XmlNode 
			rdfRdfNode)
		{
			for (int i = 0; i < rdfRdfNode.ChildNodes.Count; i++) {
				XmlNode child = rdfRdfNode.ChildNodes[i];
				// filter whitespaces (and all text nodes)
				if (!IsWhitespaceNode(child)) {
					Rdf_NodeElement(xmp, xmpParent, child, true);
				}
			}
		}

		/// <summary>
		/// 7.2.5 nodeElementURIs
		/// anyURI - ( coreSyntaxTerms | rdf:li | oldTerms )
		/// 7.2.11 nodeElement
		/// start-element ( URI == nodeElementURIs,
		/// attributes == set ( ( idAttr | nodeIdAttr | aboutAttr )?, propertyAttr* ) )
		/// propertyEltList
		/// end-element()
		/// A node element URI is rdf:Description or anything else that is not an RDF
		/// term.
		/// </summary>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="xmlNode">the currently processed XML node</param>
		/// <param name="isTopLevel">Flag if the node is a top-level node</param>
		private static void Rdf_NodeElement(XMPMetaImpl xmp, XMPNode xmpParent, XmlNode xmlNode
			, bool isTopLevel)
		{
			int nodeTerm = GetRdfTermKind(xmlNode);
			if (nodeTerm != RDFTERM_DESCRIPTION && nodeTerm != RDFTERM_OTHER)
			{
				throw new XMPException("Node element must be rdf:Description or typed node", XMPError.BADRDF
					);
			}
			else
			{
				if (isTopLevel && nodeTerm == RDFTERM_OTHER)
				{
					throw new XMPException("Top level typed node not allowed", XMPError.BADXMP);
				}
				else
				{
					Rdf_NodeElementAttrs(xmp, xmpParent, xmlNode, isTopLevel);
					Rdf_PropertyElementList(xmp, xmpParent, xmlNode, isTopLevel);
				}
			}
		}

		/// <summary>
		/// 7.2.7 propertyAttributeURIs
		/// anyURI - ( coreSyntaxTerms | rdf:Description | rdf:li | oldTerms )
		/// 7.2.11 nodeElement
		/// start-element ( URI == nodeElementURIs,
		/// attributes == set ( ( idAttr | nodeIdAttr | aboutAttr )?, propertyAttr* ) )
		/// propertyEltList
		/// end-element()
		/// Process the attribute list for an RDF node element.
		/// </summary>
		/// <remarks>
		/// 7.2.7 propertyAttributeURIs
		/// anyURI - ( coreSyntaxTerms | rdf:Description | rdf:li | oldTerms )
		/// 7.2.11 nodeElement
		/// start-element ( URI == nodeElementURIs,
		/// attributes == set ( ( idAttr | nodeIdAttr | aboutAttr )?, propertyAttr* ) )
		/// propertyEltList
		/// end-element()
		/// Process the attribute list for an RDF node element. A property attribute URI is
		/// anything other than an RDF term. The rdf:ID and rdf:nodeID attributes are simply ignored,
		/// as are rdf:about attributes on inner nodes.
		/// </remarks>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="xmlNode">the currently processed XML node</param>
		/// <param name="isTopLevel">Flag if the node is a top-level node</param>
		private static void Rdf_NodeElementAttrs(XMPMetaImpl xmp, XMPNode xmpParent, XmlNode
			 xmlNode, bool isTopLevel)
		{
			// Used to detect attributes that are mutually exclusive.
			int exclusiveAttrs = 0;
			if (xmlNode == null || xmlNode.Attributes == null)
				return;

			for (int i = 0; i < xmlNode.Attributes.Count; i++) {
				XmlNode attribute = xmlNode.Attributes[i];

				// quick hack, ns declarations do not appear in C++
				// ignore "ID" without namespace
				if ("xmlns".Equals(attribute.Prefix) || (attribute.Prefix == null && "xmlns".Equals(attribute.Name))) {
					continue;
				}

				int attrTerm = GetRdfTermKind(attribute);

				switch (attrTerm) {
					case RDFTERM_ID:
					case RDFTERM_NODE_ID:
					case RDFTERM_ABOUT:
						if (exclusiveAttrs > 0) {
							throw new XMPException("Mutally exclusive about, ID, nodeID attributes",
								XMPError.BADRDF);
						}

						exclusiveAttrs++;

						if (isTopLevel && (attrTerm == RDFTERM_ABOUT)) {
							// This is the rdf:about attribute on a top level node. Set
							// the XMP tree name if
							// it doesn't have a name yet. Make sure this name matches
							// the XMP tree name.
							if (!string.IsNullOrEmpty(xmpParent.GetName())) {
								if (!xmpParent.GetName().Equals(attribute.Value)) {
									throw new XMPException("Mismatched top level rdf:about values",
										XMPError.BADXMP);
								}
							}
							else {
								xmpParent.SetName(attribute.Value);
							}
						}
						break;

					case RDFTERM_OTHER:
						AddChildNode(xmp, xmpParent, attribute, attribute.Value, isTopLevel);
						break;

					default:
						throw new XMPException("Invalid nodeElement attribute", XMPError.BADRDF);
				}
			}
		}

		/// <summary>
		/// 7.2.13 propertyEltList
		/// ws* ( propertyElt ws* )
		/// </summary>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="xmlParent">the currently processed XML node</param>
		/// <param name="isTopLevel">Flag if the node is a top-level node</param>
		private static void Rdf_PropertyElementList(XMPMetaImpl xmp, XMPNode xmpParent, XmlNode
			 xmlParent, bool isTopLevel)
		{
			for (int i = 0; i < xmlParent.ChildNodes.Count; i++) {
				XmlNode currChild = xmlParent.ChildNodes[i];
				if (IsWhitespaceNode(currChild)) {
					continue;
				}
				if (currChild.NodeType != XmlNodeType.Element) {
					throw new XMPException("Expected property element node not found", XMPError.BADRDF);
				}
				Rdf_PropertyElement(xmp, xmpParent, currChild, isTopLevel);
			}
		}

		/// <summary>
		/// 7.2.14 propertyElt
		/// resourcePropertyElt | literalPropertyElt | parseTypeLiteralPropertyElt |
		/// parseTypeResourcePropertyElt | parseTypeCollectionPropertyElt |
		/// parseTypeOtherPropertyElt | emptyPropertyElt
		/// 7.2.15 resourcePropertyElt
		/// start-element ( URI == propertyElementURIs, attributes == set ( idAttr? ) )
		/// ws* nodeElement ws
		/// end-element()
		/// 7.2.16 literalPropertyElt
		/// start-element (
		/// URI == propertyElementURIs, attributes == set ( idAttr?, datatypeAttr?) )
		/// text()
		/// end-element()
		/// 7.2.17 parseTypeLiteralPropertyElt
		/// start-element (
		/// URI == propertyElementURIs, attributes == set ( idAttr?, parseLiteral ) )
		/// literal
		/// end-element()
		/// 7.2.18 parseTypeResourcePropertyElt
		/// start-element (
		/// URI == propertyElementURIs, attributes == set ( idAttr?, parseResource ) )
		/// propertyEltList
		/// end-element()
		/// 7.2.19 parseTypeCollectionPropertyElt
		/// start-element (
		/// URI == propertyElementURIs, attributes == set ( idAttr?, parseCollection ) )
		/// nodeElementList
		/// end-element()
		/// 7.2.20 parseTypeOtherPropertyElt
		/// start-element ( URI == propertyElementURIs, attributes == set ( idAttr?, parseOther ) )
		/// propertyEltList
		/// end-element()
		/// 7.2.21 emptyPropertyElt
		/// start-element ( URI == propertyElementURIs,
		/// attributes == set ( idAttr?, ( resourceAttr | nodeIdAttr )?, propertyAttr* ) )
		/// end-element()
		/// The various property element forms are not distinguished by the XML element name,
		/// but by their attributes for the most part.
		/// </summary>
		/// <remarks>
		/// 7.2.14 propertyElt
		/// resourcePropertyElt | literalPropertyElt | parseTypeLiteralPropertyElt |
		/// parseTypeResourcePropertyElt | parseTypeCollectionPropertyElt |
		/// parseTypeOtherPropertyElt | emptyPropertyElt
		/// 7.2.15 resourcePropertyElt
		/// start-element ( URI == propertyElementURIs, attributes == set ( idAttr? ) )
		/// ws* nodeElement ws
		/// end-element()
		/// 7.2.16 literalPropertyElt
		/// start-element (
		/// URI == propertyElementURIs, attributes == set ( idAttr?, datatypeAttr?) )
		/// text()
		/// end-element()
		/// 7.2.17 parseTypeLiteralPropertyElt
		/// start-element (
		/// URI == propertyElementURIs, attributes == set ( idAttr?, parseLiteral ) )
		/// literal
		/// end-element()
		/// 7.2.18 parseTypeResourcePropertyElt
		/// start-element (
		/// URI == propertyElementURIs, attributes == set ( idAttr?, parseResource ) )
		/// propertyEltList
		/// end-element()
		/// 7.2.19 parseTypeCollectionPropertyElt
		/// start-element (
		/// URI == propertyElementURIs, attributes == set ( idAttr?, parseCollection ) )
		/// nodeElementList
		/// end-element()
		/// 7.2.20 parseTypeOtherPropertyElt
		/// start-element ( URI == propertyElementURIs, attributes == set ( idAttr?, parseOther ) )
		/// propertyEltList
		/// end-element()
		/// 7.2.21 emptyPropertyElt
		/// start-element ( URI == propertyElementURIs,
		/// attributes == set ( idAttr?, ( resourceAttr | nodeIdAttr )?, propertyAttr* ) )
		/// end-element()
		/// The various property element forms are not distinguished by the XML element name,
		/// but by their attributes for the most part. The exceptions are resourcePropertyElt and
		/// literalPropertyElt. They are distinguished by their XML element content.
		/// NOTE: The RDF syntax does not explicitly include the xml:lang attribute although it can
		/// appear in many of these. We have to allow for it in the attibute counts below.
		/// </remarks>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="xmlNode">the currently processed XML node</param>
		/// <param name="isTopLevel">Flag if the node is a top-level node</param>
		private static void Rdf_PropertyElement(XMPMetaImpl xmp, XMPNode xmpParent, XmlNode 
			xmlNode, bool isTopLevel)
		{
			int nodeTerm = GetRdfTermKind(xmlNode);
			if (!IsPropertyElementName(nodeTerm)) {
				throw new XMPException("Invalid property element name", XMPError.BADRDF);
			}

			// remove the namespace-definitions from the list
			XmlAttributeCollection attributes = xmlNode.Attributes;
			if (attributes == null)
				return;
			IList nsAttrs = null;
			for (int i = 0; i < attributes.Count; i++) {
				XmlNode attribute = attributes[i];
				if ("xmlns".Equals(attribute.Prefix) || (attribute.Prefix == null && "xmlns".Equals(attribute.Name))) {
					if (nsAttrs == null) {
						nsAttrs = new ArrayList();
					}
					nsAttrs.Add(attribute.Name);
				}
			}
			if (nsAttrs != null) {
				for (IEnumerator it = nsAttrs.GetEnumerator(); it.MoveNext();) {
					string ns = (string) it.Current;
					attributes.RemoveNamedItem(ns);
				}
			}


			if (attributes.Count > 3) {
				// Only an emptyPropertyElt can have more than 3 attributes.
				Rdf_EmptyPropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
			}
			else {
				// Look through the attributes for one that isn't rdf:ID or xml:lang, 
				// it will usually tell what we should be dealing with. 
				// The called routines must verify their specific syntax!

				for (int i = 0; i < attributes.Count; i++) {
					XmlNode attribute = attributes[i];
					string attrLocal = attribute.LocalName;
					string attrNs = attribute.NamespaceURI;
					string attrValue = attribute.Value;
					if (!(XML_LANG.Equals(attribute.Name) && !("ID".Equals(attrLocal) && NS_RDF.Equals(attrNs)))) {
						if ("datatype".Equals(attrLocal) && NS_RDF.Equals(attrNs)) {
							Rdf_LiteralPropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
						}
						else if (!("parseType".Equals(attrLocal) && NS_RDF.Equals(attrNs))) {
							Rdf_EmptyPropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
						}
						else if ("Literal".Equals(attrValue)) {
							Rdf_ParseTypeLiteralPropertyElement();
						}
						else if ("Resource".Equals(attrValue)) {
							Rdf_ParseTypeResourcePropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
						}
						else if ("Collection".Equals(attrValue)) {
							Rdf_ParseTypeCollectionPropertyElement();
						}
						else {
							Rdf_ParseTypeOtherPropertyElement();
						}

						return;
					}
				}

				// Only rdf:ID and xml:lang, could be a resourcePropertyElt, a literalPropertyElt, 
				// or an emptyPropertyElt. Look at the child XML nodes to decide which.

				if (xmlNode.HasChildNodes) {
					for (int i = 0; i < xmlNode.ChildNodes.Count; i++) {
						XmlNode currChild = xmlNode.ChildNodes[i];
						if (currChild.NodeType != XmlNodeType.Text) {
							Rdf_ResourcePropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
							return;
						}
					}

					Rdf_LiteralPropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
				}
				else {
					Rdf_EmptyPropertyElement(xmp, xmpParent, xmlNode, isTopLevel);
				}
			}
		}

		/// <summary>
		/// 7.2.15 resourcePropertyElt
		/// start-element ( URI == propertyElementURIs, attributes == set ( idAttr? ) )
		/// ws* nodeElement ws
		/// end-element()
		/// This handles structs using an rdf:Description node,
		/// arrays using rdf:Bag/Seq/Alt, and typedNodes.
		/// </summary>
		/// <remarks>
		/// 7.2.15 resourcePropertyElt
		/// start-element ( URI == propertyElementURIs, attributes == set ( idAttr? ) )
		/// ws* nodeElement ws
		/// end-element()
		/// This handles structs using an rdf:Description node,
		/// arrays using rdf:Bag/Seq/Alt, and typedNodes. It also catches and cleans up qualified
		/// properties written with rdf:Description and rdf:value.
		/// </remarks>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="xmlNode">the currently processed XML node</param>
		/// <param name="isTopLevel">Flag if the node is a top-level node</param>
		private static void Rdf_ResourcePropertyElement(XMPMetaImpl xmp, XMPNode xmpParent
			, XmlNode xmlNode, bool isTopLevel)
		{
			if (isTopLevel && "iX:changes".Equals(xmlNode.Name)) {
				// Strip old "punchcard" chaff which has on the prefix "iX:".
				return;
			}

			XMPNode newCompound = AddChildNode(xmp, xmpParent, xmlNode, "", isTopLevel);

			// walk through the attributes
			if (xmlNode.Attributes != null) {
				for (int i = 0; i < xmlNode.Attributes.Count; i++) {
					XmlNode attribute = xmlNode.Attributes[i];
					if ("xmlns".Equals(attribute.Prefix) || (attribute.Prefix == null && "xmlns".Equals(attribute.Name))) {
						continue;
					}

					string attrLocal = attribute.LocalName;
					string attrNs = attribute.NamespaceURI;
					if (XML_LANG.Equals(attribute.Name)) {
						AddQualifierNode(newCompound, XML_LANG, attribute.Value);
					}
					else if ("ID".Equals(attrLocal) && NS_RDF.Equals(attrNs)) {
						continue; // Ignore all rdf:ID attributes.
					}
					throw new XMPException("Invalid attribute for resource property element", XMPError.BADRDF);
				}
			}

			// walk through the children

			bool found = false;
			for (int i = 0; i < xmlNode.ChildNodes.Count; i++) {
				XmlNode currChild = xmlNode.ChildNodes[i];
				if (!IsWhitespaceNode(currChild)) {
					if (currChild.NodeType == XmlNodeType.Element && !found) {
						bool isRdf = NS_RDF.Equals(currChild.NamespaceURI);
						string childLocal = currChild.LocalName;

						if (isRdf && "Bag".Equals(childLocal)) {
							newCompound.GetOptions().SetArray(true);
						}
						else if (isRdf && "Seq".Equals(childLocal)) {
							newCompound.GetOptions().SetArray(true);
							newCompound.GetOptions().SetArrayOrdered(true);
						}
						else if (isRdf && "Alt".Equals(childLocal)) {
							newCompound.GetOptions().SetArray(true);
							newCompound.GetOptions().SetArrayOrdered(true);
							newCompound.GetOptions().SetArrayAlternate(true);
						}
						else {
							newCompound.GetOptions().SetStruct(true);
							if (!isRdf && !"Description".Equals(childLocal)) {
								string typeName = currChild.NamespaceURI;
								if (typeName == null) {
									throw new XMPException("All XML elements must be in a namespace",
										XMPError.BADXMP);
								}
								typeName += ':' + childLocal;
								AddQualifierNode(newCompound, "rdf:type", typeName);
							}
						}

						Rdf_NodeElement(xmp, newCompound, currChild, false);

						if (newCompound.GetHasValueChild()) {
							FixupQualifiedNode(newCompound);
						}
						else if (newCompound.GetOptions().IsArrayAlternate()) {
							XMPNodeUtils.DetectAltText(newCompound);
						}

						found = true;
					}
					else if (found) {
						// found second child element
						throw new XMPException("Invalid child of resource property element", XMPError.BADRDF);
					}
					else {
						throw new XMPException("Children of resource property element must be XML elements",
							XMPError.BADRDF);
					}
				}
			}

			if (!found) {
				// didn't found any child elements
				throw new XMPException("Missing child of resource property element", XMPError.BADRDF);
			}
		}

		/// <summary>
		/// 7.2.16 literalPropertyElt
		/// start-element ( URI == propertyElementURIs,
		/// attributes == set ( idAttr?, datatypeAttr?) )
		/// text()
		/// end-element()
		/// Add a leaf node with the text value and qualifiers for the attributes.
		/// </summary>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="xmlNode">the currently processed XML node</param>
		/// <param name="isTopLevel">Flag if the node is a top-level node</param>
		private static void Rdf_LiteralPropertyElement(XMPMetaImpl xmp, XMPNode xmpParent
			, XmlNode xmlNode, bool isTopLevel)
		{
			XMPNode newChild = AddChildNode(xmp, xmpParent, xmlNode, null, isTopLevel);
			if (xmlNode.Attributes != null) {
				for (int i = 0; i < xmlNode.Attributes.Count; i++) {
					XmlNode attribute = xmlNode.Attributes[i];
					if ("xmlns".Equals(attribute.Prefix) || (attribute.Prefix == null && "xmlns".Equals(attribute.Name))) {
						continue;
					}

					String attrNs = attribute.NamespaceURI;
					String attrLocal = attribute.LocalName;
					if (XML_LANG.Equals(attribute.Name)) {
						AddQualifierNode(newChild, XML_LANG, attribute.Value);
					} else if (NS_RDF.Equals(attrNs) && ("ID".Equals(attrLocal) || "datatype".Equals(attrLocal))) {
						continue; // Ignore all rdf:ID and rdf:datatype attributes.
					} else
						throw new XMPException("Invalid attribute for literal property element", XMPError.BADRDF);
				}
			}
			String textValue = "";
			for (int i = 0; i < xmlNode.ChildNodes.Count; i++) {
				XmlNode child = xmlNode.ChildNodes[i];
				if (child.NodeType == XmlNodeType.Text) {
					textValue += child.Value;
				}
				else {
					throw new XMPException("Invalid child of literal property element", XMPError.BADRDF);
				}
			}
			newChild.SetValue(textValue);
		}

		/// <summary>
		/// 7.2.17 parseTypeLiteralPropertyElt
		/// start-element ( URI == propertyElementURIs,
		/// attributes == set ( idAttr?, parseLiteral ) )
		/// literal
		/// end-element()
		/// </summary>
		private static void Rdf_ParseTypeLiteralPropertyElement()
		{
			throw new XMPException("ParseTypeLiteral property element not allowed", XMPError.BADXMP);
		}

		/// <summary>
		/// 7.2.18 parseTypeResourcePropertyElt
		/// start-element ( URI == propertyElementURIs,
		/// attributes == set ( idAttr?, parseResource ) )
		/// propertyEltList
		/// end-element()
		/// Add a new struct node with a qualifier for the possible rdf:ID attribute.
		/// </summary>
		/// <remarks>
		/// 7.2.18 parseTypeResourcePropertyElt
		/// start-element ( URI == propertyElementURIs,
		/// attributes == set ( idAttr?, parseResource ) )
		/// propertyEltList
		/// end-element()
		/// Add a new struct node with a qualifier for the possible rdf:ID attribute.
		/// Then process the XML child nodes to get the struct fields.
		/// </remarks>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="xmlNode">the currently processed XML node</param>
		/// <param name="isTopLevel">Flag if the node is a top-level node</param>
		private static void Rdf_ParseTypeResourcePropertyElement(XMPMetaImpl xmp, XMPNode
			 xmpParent, XmlNode xmlNode, bool isTopLevel)
		{
			XMPNode newStruct = AddChildNode(xmp, xmpParent, xmlNode, "", isTopLevel);
			newStruct.GetOptions().SetStruct(true);

			if (xmlNode.Attributes != null) {
				for (int i = 0; i < xmlNode.Attributes.Count; i++) {
					XmlNode attribute = xmlNode.Attributes[i];
					if ("xmlns".Equals(attribute.Prefix) || (attribute.Prefix == null && "xmlns".Equals(attribute.Name))) {
						continue;
					}

					String attrLocal = attribute.LocalName;
					String attrNs = attribute.NamespaceURI;
					if (XML_LANG.Equals(attribute.Name)) {
						AddQualifierNode(newStruct, XML_LANG, attribute.Value);
					}
					else if (NS_RDF.Equals(attrNs) && ("ID".Equals(attrLocal) || "parseType".Equals(attrLocal))) {
						continue; // The caller ensured the value is "Resource".
						// Ignore all rdf:ID attributes.
					}
					throw new XMPException("Invalid attribute for ParseTypeResource property element",
						XMPError.BADRDF);
				}
			}

			Rdf_PropertyElementList(xmp, newStruct, xmlNode, false);

			if (newStruct.GetHasValueChild()) {
				FixupQualifiedNode(newStruct);
			}
		}

		/// <summary>
		/// 7.2.19 parseTypeCollectionPropertyElt
		/// start-element ( URI == propertyElementURIs,
		/// attributes == set ( idAttr?, parseCollection ) )
		/// nodeElementList
		/// end-element()
		/// </summary>
		private static void Rdf_ParseTypeCollectionPropertyElement()
		{
			throw new XMPException("ParseTypeCollection property element not allowed", XMPError.BADXMP
				);
		}

		/// <summary>
		/// 7.2.20 parseTypeOtherPropertyElt
		/// start-element ( URI == propertyElementURIs, attributes == set ( idAttr?, parseOther ) )
		/// propertyEltList
		/// end-element()
		/// </summary>
		private static void Rdf_ParseTypeOtherPropertyElement()
		{
			throw new XMPException("ParseTypeOther property element not allowed", XMPError.BADXMP);
		}

		/// <summary>
		/// 7.2.21 emptyPropertyElt
		/// start-element ( URI == propertyElementURIs,
		/// attributes == set (
		/// idAttr?, ( resourceAttr | nodeIdAttr )?, propertyAttr* ) )
		/// end-element()
		/// </summary>
		/// <remarks>
		/// 7.2.21 emptyPropertyElt
		/// start-element ( URI == propertyElementURIs,
		/// attributes == set (
		/// idAttr?, ( resourceAttr | nodeIdAttr )?, propertyAttr* ) )
		/// end-element()
		/// <br/>
		/// &lt;ns:Prop1/&gt;  &lt;!-- a simple property with an empty value --&gt;<br/>
		/// &lt;ns:Prop2 rdf:resource="http: *www.adobe.com/"/&gt; &lt;!-- a URI value --&gt;<br/>
		/// &lt;ns:Prop3 rdf:value="..." ns:Qual="..."/&gt; &lt;!-- a simple qualified property --&gt;<br/>
		/// &lt;ns:Prop4 ns:Field1="..." ns:Field2="..."/&gt; &lt;!-- a struct with simple fields --&gt;<br/>
		/// An emptyPropertyElt is an element with no contained content, just a possibly empty set of
		/// attributes. 
		/// An emptyPropertyElt is an element with no contained content, just a possibly empty set of
		/// attributes. An emptyPropertyElt can represent three special cases of simple XMP properties: a
		/// simple property with an empty value (ns:Prop1), a simple property whose value is a URI
		/// (ns:Prop2), or a simple property with simple qualifiers (ns:Prop3).
		/// An emptyPropertyElt can also represent an XMP struct whose fields are all simple and
		/// unqualified (ns:Prop4).
		/// <para/>
		/// It is an error to use both rdf:value and rdf:resource - that can lead to invalid  RDF in the
		/// verbose form written using a literalPropertyElt.
		/// <para/>
		/// The XMP mapping for an emptyPropertyElt is a bit different from generic RDF, partly for
		/// design reasons and partly for historical reasons. The XMP mapping rules are:
		/// <list type="number">
		/// <item><description> If there is an rdf:value attribute then this is a simple property 
		/// with a text value.
		/// All other attributes are qualifiers.</description></item>
		/// <item><description> If there is an rdf:resource attribute then this is a simple property
		/// with a URI value.
		/// All other attributes are qualifiers.</description></item>
		/// <item><description> If there are no attributes other than xml:lang, rdf:ID, or rdf:nodeID
		/// then this is a simple
		/// property with an empty value.</description></item>
		/// <item><description> Otherwise this is a struct, the attributes other than xml:lang, rdf:ID,
		/// or rdf:nodeID are fields.</description></item>
		/// </list>
		/// </remarks>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="xmlNode">the currently processed XML node</param>
		/// <param name="isTopLevel">Flag if the node is a top-level node</param>
		private static void Rdf_EmptyPropertyElement(XMPMetaImpl xmp, XMPNode xmpParent, 
			XmlNode xmlNode, bool isTopLevel)
		{
			bool hasPropertyAttrs = false;
			bool hasResourceAttr = false;
			bool hasNodeIdAttr = false;
			bool hasValueAttr = false;

			XmlNode valueNode = null; // ! Can come from rdf:value or rdf:resource.

			if (xmlNode.HasChildNodes) {
				throw new XMPException("Nested content not allowed with rdf:resource or property attributes",
					XMPError.BADRDF);
			}

			// First figure out what XMP this maps to and remember the XML node for a simple value.
			if (xmlNode.Attributes != null) {
				for (int i = 0; i < xmlNode.Attributes.Count; i++) {
					XmlNode attribute = xmlNode.Attributes[i];
					if ("xmlns".Equals(attribute.Prefix) || (attribute.Prefix == null && "xmlns".Equals(attribute.Name))) {
						continue;
					}

					int attrTerm = GetRdfTermKind(attribute);

					switch (attrTerm) {
						case RDFTERM_ID:
							// Nothing to do.
							break;

						case RDFTERM_RESOURCE:
							if (hasNodeIdAttr) {
								throw new XMPException(
									"Empty property element can't have both rdf:resource and rdf:nodeID",
									XMPError.BADRDF);
							}
							if (hasValueAttr) {
								throw new XMPException(
									"Empty property element can't have both rdf:value and rdf:resource",
									XMPError.BADXMP);
							}

							hasResourceAttr = true;
							if (!hasValueAttr) {
								valueNode = attribute;
							}
							break;

						case RDFTERM_NODE_ID:
							if (hasResourceAttr) {
								throw new XMPException(
									"Empty property element can't have both rdf:resource and rdf:nodeID",
									XMPError.BADRDF);
							}
							hasNodeIdAttr = true;
							break;

						case RDFTERM_OTHER:
							if ("value".Equals(attribute.LocalName) && NS_RDF.Equals(attribute.NamespaceURI)) {
								if (hasResourceAttr) {
									throw new XMPException(
										"Empty property element can't have both rdf:value and rdf:resource",
										XMPError.BADXMP);
								}
								hasValueAttr = true;
								valueNode = attribute;
							}
							else if (!XML_LANG.Equals(attribute.Name)) {
								hasPropertyAttrs = true;
							}
							break;

						default:
							throw new XMPException("Unrecognized attribute of empty property element",
								XMPError.BADRDF);
					}
				}
			}

			// Create the right kind of child node and visit the attributes again 
			// to add the fields or qualifiers.
			// ! Because of implementation vagaries, 
			//   the xmpParent is the tree root for top level properties.
			// ! The schema is found, created if necessary, by addChildNode.

			XMPNode childNode = AddChildNode(xmp, xmpParent, xmlNode, "", isTopLevel);
			bool childIsStruct = false;

			if (hasValueAttr || hasResourceAttr) {
				childNode.SetValue(valueNode != null ? valueNode.Value : "");
				if (!hasValueAttr) {
					// ! Might have both rdf:value and rdf:resource.
					childNode.GetOptions().SetURI(true);
				}
			}
			else if (hasPropertyAttrs) {
				childNode.GetOptions().SetStruct(true);
				childIsStruct = true;
			}

			if (xmlNode.Attributes != null) {
				for (int i = 0; i < xmlNode.Attributes.Count; i++) {
					XmlNode attribute = xmlNode.Attributes[i];
					if (attribute == valueNode || "xmlns".Equals(attribute.Prefix) ||
						(attribute.Prefix == null && "xmlns".Equals(attribute.Name))) {
						continue; // Skip the rdf:value or rdf:resource attribute holding the value.
					}

					int attrTerm = GetRdfTermKind(attribute);

					switch (attrTerm) {
						case RDFTERM_ID:
						case RDFTERM_NODE_ID:
							break; // Ignore all rdf:ID and rdf:nodeID attributes.
						case RDFTERM_RESOURCE:
							AddQualifierNode(childNode, "rdf:resource", attribute.Value);
							break;

						case RDFTERM_OTHER:
							if (!childIsStruct) {
								AddQualifierNode(childNode, attribute.Name, attribute.Value);
							}
							else if (XML_LANG.Equals(attribute.Name)) {
								AddQualifierNode(childNode, XML_LANG, attribute.Value);
							}
							else {
								AddChildNode(xmp, childNode, attribute, attribute.Value, false);
							}
							break;

						default:
							throw new XMPException("Unrecognized attribute of empty property element",
								XMPError.BADRDF);
					}
				}
			}
		}

		/// <summary>Adds a child node.</summary>
		/// <param name="xmp">the xmp metadata object that is generated</param>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="xmlNode">the currently processed XML node</param>
		/// <param name="value">Node value</param>
		/// <param name="isTopLevel">Flag if the node is a top-level node</param>
		/// <returns>Returns the newly created child node.</returns>
		private static XMPNode AddChildNode(XMPMetaImpl xmp, XMPNode xmpParent, XmlNode xmlNode
			, String value, bool isTopLevel)
		{
			XMPSchemaRegistry registry = XMPMetaFactory.GetSchemaRegistry();
			String ns = xmlNode.NamespaceURI;
			String childName;
			if (ns != null) {
				if (NS_DC_DEPRECATED.Equals(ns)) {
					// Fix a legacy DC namespace
					ns = NS_DC;
				}

				string prefix = registry.GetNamespacePrefix(ns);
				if (prefix == null) {
					prefix = xmlNode.Prefix ?? DEFAULT_PREFIX;
					prefix = registry.RegisterNamespace(ns, prefix);
				}
				childName = prefix + xmlNode.LocalName;
			}
			else {
				throw new XMPException("XML namespace required for all elements and attributes",
					XMPError.BADRDF);
			}


			// create schema node if not already there
			PropertyOptions childOptions = new PropertyOptions();
			bool isAlias = false;
			if (isTopLevel) {
				// Lookup the schema node, adjust the XMP parent pointer.
				// Incoming parent must be the tree root.
				XMPNode schemaNode = XMPNodeUtils.FindSchemaNode(xmp.GetRoot(), ns, DEFAULT_PREFIX, true);
				schemaNode.SetImplicit(false); // Clear the implicit node bit.
				// need runtime check for proper 32 bit code.
				xmpParent = schemaNode;

				// If this is an alias set the alias flag in the node 
				// and the hasAliases flag in the tree.
				if (registry.FindAlias(childName) != null) {
					isAlias = true;
					xmp.GetRoot().SetHasAliases(true);
					schemaNode.SetHasAliases(true);
				}
			}


			// Make sure that this is not a duplicate of a named node.
			bool isArrayItem = "rdf:li".Equals(childName);
			bool isValueNode = "rdf:value".Equals(childName);

			// Create XMP node and so some checks
			XMPNode newChild = new XMPNode(childName, value, childOptions);
			newChild.SetAlias(isAlias);

			// Add the new child to the XMP parent node, a value node first.
			if (!isValueNode) {
				xmpParent.AddChild(newChild);
			}
			else {
				xmpParent.AddChild(1, newChild);
			}


			if (isValueNode) {
				if (isTopLevel || !xmpParent.GetOptions().IsStruct()) {
					throw new XMPException("Misplaced rdf:value element", XMPError.BADRDF);
				}
				xmpParent.SetHasValueChild(true);
			}

			if (isArrayItem) {
				if (!xmpParent.GetOptions().IsArray()) {
					throw new XMPException("Misplaced rdf:li element", XMPError.BADRDF);
				}
				newChild.SetName(ARRAY_ITEM_NAME);
			}

			return newChild;
		}

		/// <summary>Adds a qualifier node.</summary>
		/// <param name="xmpParent">the parent xmp node</param>
		/// <param name="name">
		/// the name of the qualifier which has to be
		/// QName including the <b>default prefix</b>
		/// </param>
		/// <param name="value">the value of the qualifier</param>
		/// <returns>Returns the newly created child node.</returns>
		private static XMPNode AddQualifierNode(XMPNode xmpParent, String name, String value
			)
		{
			bool isLang = XML_LANG.Equals(name);
			XMPNode newQual = null;
			// normalize value of language qualifiers
			newQual = new XMPNode(name, isLang ? iText.Kernel.XMP.Impl.Utils.NormalizeLangValue
				(value) : value, null);
			xmpParent.AddQualifier(newQual);
			return newQual;
		}

		/// <summary>The parent is an RDF pseudo-struct containing an rdf:value field.</summary>
		/// <remarks>
		/// The parent is an RDF pseudo-struct containing an rdf:value field. Fix the
		/// XMP data model. The rdf:value node must be the first child, the other
		/// children are qualifiers. The form, value, and children of the rdf:value
		/// node are the real ones. The rdf:value node's qualifiers must be added to
		/// the others.
		/// </remarks>
		/// <param name="xmpParent">the parent xmp node</param>
		private static void FixupQualifiedNode(XMPNode xmpParent)
		{
			System.Diagnostics.Debug.Assert(xmpParent.GetOptions().IsStruct() && xmpParent.HasChildren
				());
			XMPNode valueNode = xmpParent.GetChild(1);
			System.Diagnostics.Debug.Assert("rdf:value".Equals(valueNode.GetName()));
			// Move the qualifiers on the value node to the parent. 
			// Make sure an xml:lang qualifier stays at the front.
			// Check for duplicate names between the value node's qualifiers and the parent's children. 
			// The parent's children are about to become qualifiers. Check here, between the groups. 
			// Intra-group duplicates are caught by XMPNode#addChild(...).
			if (valueNode.GetOptions().GetHasLanguage())
			{
				if (xmpParent.GetOptions().GetHasLanguage())
				{
					throw new XMPException("Redundant xml:lang for rdf:value element", XMPError.BADXMP);
				}
				XMPNode langQual = valueNode.GetQualifier(1);
				valueNode.RemoveQualifier(langQual);
				xmpParent.AddQualifier(langQual);
			}
			// Start the remaining copy after the xml:lang qualifier.		
			for (int i = 1; i <= valueNode.GetQualifierLength(); i++)
			{
				XMPNode qualifier = valueNode.GetQualifier(i);
				xmpParent.AddQualifier(qualifier);
			}
			// Change the parent's other children into qualifiers. 
			// This loop starts at 1, child 0 is the rdf:value node.
			for (int i_1 = 2; i_1 <= xmpParent.GetChildrenLength(); i_1++)
			{
				XMPNode qualifier = xmpParent.GetChild(i_1);
				xmpParent.AddQualifier(qualifier);
			}
			// Move the options and value last, other checks need the parent's original options. 
			// Move the value node's children to be the parent's children.
			System.Diagnostics.Debug.Assert(xmpParent.GetOptions().IsStruct() || xmpParent.GetHasValueChild
				());
			xmpParent.SetHasValueChild(false);
			xmpParent.GetOptions().SetStruct(false);
			xmpParent.GetOptions().MergeWith(valueNode.GetOptions());
			xmpParent.SetValue(valueNode.GetValue());
			xmpParent.RemoveChildren();
			for (IEnumerator it = valueNode.IterateChildren(); it.MoveNext(); )
			{
				XMPNode child = (XMPNode)it.Current;
				xmpParent.AddChild(child);
			}
		}

		/// <summary>Checks if the node is a white space.</summary>
		/// <param name="node">an XML-node</param>
		/// <returns>
		/// Returns whether the node is a whitespace node,
		/// i.e. a text node that contains only whitespaces.
		/// </returns>
		private static bool IsWhitespaceNode(XmlNode node)
		{
			if (node.NodeType != XmlNodeType.Text) {
				return false;
			}

			string value = node.Value;
			for (int i = 0; i < value.Length; i++) {
				if (!TextUtil.IsWhiteSpace(value[i])) {
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 7.2.6 propertyElementURIs
		/// anyURI - ( coreSyntaxTerms | rdf:Description | oldTerms )
		/// </summary>
		/// <param name="term">the term id</param>
		/// <returns>Return true if the term is a property element name.</returns>
		private static bool IsPropertyElementName(int term)
		{
			if (term == RDFTERM_DESCRIPTION || IsOldTerm(term))
			{
				return false;
			}
			else
			{
				return (!IsCoreSyntaxTerm(term));
			}
		}

		/// <summary>
		/// 7.2.4 oldTerms<br />
		/// rdf:aboutEach | rdf:aboutEachPrefix | rdf:bagID
		/// </summary>
		/// <param name="term">the term id</param>
		/// <returns>Returns true if the term is an old term.</returns>
		private static bool IsOldTerm(int term)
		{
			return RDFTERM_FIRST_OLD <= term && term <= RDFTERM_LAST_OLD;
		}

		/// <summary>
		/// 7.2.2 coreSyntaxTerms<br />
		/// rdf:RDF | rdf:ID | rdf:about | rdf:parseType | rdf:resource | rdf:nodeID |
		/// rdf:datatype
		/// </summary>
		/// <param name="term">the term id</param>
		/// <returns>Return true if the term is a core syntax term</returns>
		private static bool IsCoreSyntaxTerm(int term)
		{
			return RDFTERM_FIRST_CORE <= term && term <= RDFTERM_LAST_CORE;
		}

		/// <summary>Determines the ID for a certain RDF Term.</summary>
		/// <remarks>
		/// Determines the ID for a certain RDF Term.
		/// Arranged to hopefully minimize the parse time for large XMP.
		/// </remarks>
		/// <param name="node">an XML node</param>
		/// <returns>Returns the term ID.</returns>
		private static int GetRdfTermKind(XmlNode node)
		{
			string localName = node.LocalName;
			string @namespace = node.NamespaceURI;

			if (@namespace == null && ("about".Equals(localName) || "ID".Equals(localName)) && (node is XmlAttribute) &&
				NS_RDF.Equals(((XmlAttribute) node).OwnerElement.NamespaceURI)) {
				@namespace = NS_RDF;
			}

			if (NS_RDF.Equals(@namespace)) {
				if ("li".Equals(localName)) {
					return RDFTERM_LI;
				}
				if ("parseType".Equals(localName)) {
					return RDFTERM_PARSE_TYPE;
				}
				if ("Description".Equals(localName)) {
					return RDFTERM_DESCRIPTION;
				}
				if ("about".Equals(localName)) {
					return RDFTERM_ABOUT;
				}
				if ("resource".Equals(localName)) {
					return RDFTERM_RESOURCE;
				}
				if ("RDF".Equals(localName)) {
					return RDFTERM_RDF;
				}
				if ("ID".Equals(localName)) {
					return RDFTERM_ID;
				}
				if ("nodeID".Equals(localName)) {
					return RDFTERM_NODE_ID;
				}
				if ("datatype".Equals(localName)) {
					return RDFTERM_DATATYPE;
				}
				if ("aboutEach".Equals(localName)) {
					return RDFTERM_ABOUT_EACH;
				}
				if ("aboutEachPrefix".Equals(localName)) {
					return RDFTERM_ABOUT_EACH_PREFIX;
				}
				if ("bagID".Equals(localName)) {
					return RDFTERM_BAG_ID;
				}
			}

			return RDFTERM_OTHER;
		}
	}
}
