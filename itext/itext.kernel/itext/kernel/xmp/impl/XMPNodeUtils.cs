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
using iText.Kernel.XMP;
using iText.Kernel.XMP.Impl.XPath;
using iText.Kernel.XMP.Options;

namespace iText.Kernel.XMP.Impl
{
	/// <summary>Utilities for <c>XMPNode</c>.</summary>
	/// <since>Aug 28, 2006</since>
	public sealed class XMPNodeUtils : XMPConst
	{
		//\cond DO_NOT_DOCUMENT	
		internal const int CLT_NO_VALUES = 0;

		internal const int CLT_SPECIFIC_MATCH = 1;

		internal const int CLT_SINGLE_GENERIC = 2;

		internal const int CLT_MULTIPLE_GENERIC = 3;

		internal const int CLT_XDEFAULT = 4;

		internal const int CLT_FIRST_ITEM = 5;

		/// <summary>Private Constructor</summary>
		private XMPNodeUtils()
		{
		}

		// EMPTY
		/// <summary>Find or create a schema node if <code>createNodes</code> is false and</summary>
		/// <param name="tree">the root of the xmp tree.</param>
		/// <param name="namespaceURI">a namespace</param>
		/// <param name="createNodes">
		/// a flag indicating if the node shall be created if not found.
		/// <em>Note:</em> The namespace must be registered prior to this call.
		/// </param>
		/// <returns>
		/// Returns the schema node if found, <code>null</code> otherwise.
		/// Note: If <code>createNodes</code> is <code>true</code>, it is <b>always</b>
		/// returned a valid node.
		/// </returns>
		internal static XMPNode FindSchemaNode(XMPNode tree, String namespaceURI, bool createNodes
			)
		{
			return FindSchemaNode(tree, namespaceURI, null, createNodes);
		}

		/// <summary>Find or create a schema node if <code>createNodes</code> is true.</summary>
		/// <param name="tree">the root of the xmp tree.</param>
		/// <param name="namespaceURI">a namespace</param>
		/// <param name="suggestedPrefix">If a prefix is suggested, the namespace is allowed to be registered.
		/// 	</param>
		/// <param name="createNodes">
		/// a flag indicating if the node shall be created if not found.
		/// <em>Note:</em> The namespace must be registered prior to this call.
		/// </param>
		/// <returns>
		/// Returns the schema node if found, <code>null</code> otherwise.
		/// Note: If <code>createNodes</code> is <code>true</code>, it is <b>always</b>
		/// returned a valid node.
		/// </returns>
		internal static XMPNode FindSchemaNode(XMPNode tree, String namespaceURI, String 
			suggestedPrefix, bool createNodes)
		{
			System.Diagnostics.Debug.Assert(tree.GetParent() == null);
			// make sure that its the root
			XMPNode schemaNode = tree.FindChildByName(namespaceURI);
			if (schemaNode == null && createNodes)
			{
				schemaNode = new XMPNode(namespaceURI, new PropertyOptions().SetSchemaNode(true));
				schemaNode.SetImplicit(true);
				// only previously registered schema namespaces are allowed in the XMP tree.
				String prefix = XMPMetaFactory.GetSchemaRegistry().GetNamespacePrefix(namespaceURI
					);
				if (prefix == null)
				{
					if (suggestedPrefix != null && suggestedPrefix.Length != 0)
					{
						prefix = XMPMetaFactory.GetSchemaRegistry().RegisterNamespace(namespaceURI, suggestedPrefix
							);
					}
					else
					{
						throw new XMPException("Unregistered schema namespace URI", XMPError.BADSCHEMA);
					}
				}
				schemaNode.SetValue(prefix);
				tree.AddChild(schemaNode);
			}
			return schemaNode;
		}

		/// <summary>Find or create a child node under a given parent node.</summary>
		/// <remarks>
		/// Find or create a child node under a given parent node. If the parent node is no
		/// Returns the found or created child node.
		/// </remarks>
		/// <param name="parent">the parent node</param>
		/// <param name="childName">the node name to find</param>
		/// <param name="createNodes">flag, if new nodes shall be created.</param>
		/// <returns>Returns the found or created node or <code>null</code>.</returns>
		internal static XMPNode FindChildNode(XMPNode parent, String childName, bool createNodes
			)
		{
			if (!parent.GetOptions().IsSchemaNode() && !parent.GetOptions().IsStruct())
			{
				if (!parent.IsImplicit())
				{
					throw new XMPException("Named children only allowed for schemas and structs", XMPError
						.BADXPATH);
				}
				else
				{
					if (parent.GetOptions().IsArray())
					{
						throw new XMPException("Named children not allowed for arrays", XMPError.BADXPATH
							);
					}
					else
					{
						if (createNodes)
						{
							parent.GetOptions().SetStruct(true);
						}
					}
				}
			}
			XMPNode childNode = parent.FindChildByName(childName);
			if (childNode == null && createNodes)
			{
				PropertyOptions options = new PropertyOptions();
				childNode = new XMPNode(childName, options);
				childNode.SetImplicit(true);
				parent.AddChild(childNode);
			}
			System.Diagnostics.Debug.Assert(childNode != null || !createNodes);
			return childNode;
		}

		/// <summary>Follow an expanded path expression to find or create a node.</summary>
		/// <param name="xmpTree">the node to begin the search.</param>
		/// <param name="xpath">the complete xpath</param>
		/// <param name="createNodes">
		/// flag if nodes shall be created
		/// (when called by <code>setProperty()</code>)
		/// </param>
		/// <param name="leafOptions">
		/// the options for the created leaf nodes (only when
		/// <code>createNodes == true</code>).
		/// </param>
		/// <returns>Returns the node if found or created or <code>null</code>.</returns>
		internal static XMPNode FindNode(XMPNode xmpTree, XMPPath xpath, bool createNodes
			, PropertyOptions leafOptions)
		{
			// check if xpath is set.
			if (xpath == null || xpath.Size() == 0)
			{
				throw new XMPException("Empty XMPPath", XMPError.BADXPATH);
			}
			// Root of implicitly created subtree to possible delete it later. 
			// Valid only if leaf is new.
			XMPNode rootImplicitNode = null;
			XMPNode currNode = null;
			// resolve schema step
			currNode = FindSchemaNode(xmpTree, xpath.GetSegment(XMPPath.STEP_SCHEMA).GetName(
				), createNodes);
			if (currNode == null)
			{
				return null;
			}
			else
			{
				if (currNode.IsImplicit())
				{
					currNode.SetImplicit(false);
					// Clear the implicit node bit.
					rootImplicitNode = currNode;
				}
			}
			// Save the top most implicit node.
			// Now follow the remaining steps of the original XMPPath.
			try
			{
				for (int i = 1; i < xpath.Size(); i++)
				{
					currNode = FollowXPathStep(currNode, xpath.GetSegment(i), createNodes);
					if (currNode == null)
					{
						if (createNodes)
						{
							// delete implicitly created nodes
							DeleteNode(rootImplicitNode);
						}
						return null;
					}
					else
					{
						if (currNode.IsImplicit())
						{
							// clear the implicit node flag
							currNode.SetImplicit(false);
							// if node is an ALIAS (can be only in root step, auto-create array 
							// when the path has been resolved from a not simple alias type
							if (i == 1 && xpath.GetSegment(i).IsAlias() && xpath.GetSegment(i).GetAliasForm()
								 != 0)
							{
								currNode.GetOptions().SetOption(xpath.GetSegment(i).GetAliasForm(), true);
							}
							else
							{
								// "CheckImplicitStruct" in C++
								if (i < xpath.Size() - 1 && xpath.GetSegment(i).GetKind() == XMPPath.STRUCT_FIELD_STEP
									 && !currNode.GetOptions().IsCompositeProperty())
								{
									currNode.GetOptions().SetStruct(true);
								}
							}
							if (rootImplicitNode == null)
							{
								rootImplicitNode = currNode;
							}
						}
					}
				}
			}
			catch (XMPException e)
			{
				// Save the top most implicit node.
				// if new notes have been created prior to the error, delete them
				if (rootImplicitNode != null)
				{
					DeleteNode(rootImplicitNode);
				}
				throw;
			}
			if (rootImplicitNode != null)
			{
				// set options only if a node has been successful created
				currNode.GetOptions().MergeWith(leafOptions);
				currNode.SetOptions(currNode.GetOptions());
			}
			return currNode;
		}

		/// <summary>Deletes the the given node and its children from its parent.</summary>
		/// <remarks>
		/// Deletes the the given node and its children from its parent.
		/// Takes care about adjusting the flags.
		/// </remarks>
		/// <param name="node">the top-most node to delete.</param>
		internal static void DeleteNode(XMPNode node)
		{
			XMPNode parent = node.GetParent();
			if (node.GetOptions().IsQualifier())
			{
				// root is qualifier
				parent.RemoveQualifier(node);
			}
			else
			{
				// root is NO qualifier
				parent.RemoveChild(node);
			}
			// delete empty Schema nodes
			if (!parent.HasChildren() && parent.GetOptions().IsSchemaNode())
			{
				parent.GetParent().RemoveChild(parent);
			}
		}

		/// <summary>This is setting the value of a leaf node.</summary>
		/// <param name="node">an XMPNode</param>
		/// <param name="value">a value</param>
		internal static void SetNodeValue(XMPNode node, Object value)
		{
			String strValue = SerializeNodeValue(value);
			if (!(node.GetOptions().IsQualifier() && XML_LANG.Equals(node.GetName())))
			{
				node.SetValue(strValue);
			}
			else
			{
				node.SetValue(iText.Kernel.XMP.Impl.Utils.NormalizeLangValue(strValue));
			}
		}

		/// <summary>Verifies the PropertyOptions for consistancy and updates them as needed.
		/// 	</summary>
		/// <remarks>
		/// Verifies the PropertyOptions for consistancy and updates them as needed.
		/// If options are <code>null</code> they are created with default values.
		/// </remarks>
		/// <param name="options">the <code>PropertyOptions</code></param>
		/// <param name="itemValue">the node value to set</param>
		/// <returns>Returns the updated options.</returns>
		internal static PropertyOptions VerifySetOptions(PropertyOptions options, Object 
			itemValue)
		{
			// create empty and fix existing options
			if (options == null)
			{
				// set default options
				options = new PropertyOptions();
			}
			if (options.IsArrayAltText())
			{
				options.SetArrayAlternate(true);
			}
			if (options.IsArrayAlternate())
			{
				options.SetArrayOrdered(true);
			}
			if (options.IsArrayOrdered())
			{
				options.SetArray(true);
			}
			if (options.IsCompositeProperty() && itemValue != null && itemValue.ToString().Length
				 > 0)
			{
				throw new XMPException("Structs and arrays can't have values", XMPError.BADOPTIONS
					);
			}
			options.AssertConsistency(options.GetOptions());
			return options;
		}

		/// <summary>
		/// Converts the node value to String, apply special conversions for defined
		/// types in XMP.
		/// </summary>
		/// <param name="value">the node value to set</param>
		/// <returns>Returns the String representation of the node value.</returns>
		internal static String SerializeNodeValue(Object value)
		{
			String strValue;
			if (value == null)
			{
				strValue = null;
			}
			else
			{
				if (value is bool?)
				{
					strValue = XMPUtils.ConvertFromBoolean((bool)value);
				}
				else
				{
					if (value is int?)
					{
						strValue = XMPUtils.ConvertFromInteger((int)value);
					}
					else
					{
						if (value is long?)
						{
							strValue = XMPUtils.ConvertFromLong((long)value);
						}
						else
						{
							if (value is double?)
							{
								strValue = XMPUtils.ConvertFromDouble((double)value);
							}
							else
							{
								if (value is XMPDateTime)
								{
									strValue = XMPUtils.ConvertFromDate((XMPDateTime)value);
								}
								else
								{
									if (value is XMPCalendar)
									{
										XMPDateTime dt = XMPDateTimeFactory.CreateFromCalendar((XMPCalendar)value);
										strValue = XMPUtils.ConvertFromDate(dt);
									}
									else
									{
										if (value is byte[])
										{
											strValue = XMPUtils.EncodeBase64((byte[])value);
										}
										else
										{
											strValue = value.ToString();
										}
									}
								}
							}
						}
					}
				}
			}
			return strValue != null ? Utils.RemoveControlChars(strValue) : null;
		}
		//\endcond	

		/// <summary>
		/// After processing by ExpandXPath, a step can be of certain forms described in documentation.
		/// </summary>
		/// <remarks>
		/// After processing by ExpandXPath, a step can be of these forms:
		/// <ul>
		/// <li>qualName - A top level property or struct field.</li>
		/// <li>[index] - An element of an array.</li>
		/// <li>[last()] - The last element of an array.</li>
		/// <li>[qualName="value"] - An element in an array of structs, chosen by a field value.</li>
		/// <li>[?qualName="value"] - An element in an array, chosen by a qualifier value.</li>
		/// <li>?qualName - A general qualifier.</li>
		/// </ul>
		/// Find the appropriate child node, resolving aliases, and optionally creating nodes.
		/// </remarks>
		/// <param name="parentNode">the node to start to start from</param>
		/// <param name="nextStep">the xpath segment</param>
		/// <param name="createNodes"></param>
		/// <returns>returns the found or created XMPPath node</returns>
		private static XMPNode FollowXPathStep(XMPNode parentNode, XMPPathSegment nextStep
			, bool createNodes)
		{
			XMPNode nextNode = null;
			int index = 0;
			int stepKind = nextStep.GetKind();
			if (stepKind == XMPPath.STRUCT_FIELD_STEP)
			{
				nextNode = FindChildNode(parentNode, nextStep.GetName(), createNodes);
			}
			else
			{
				if (stepKind == XMPPath.QUALIFIER_STEP)
				{
					nextNode = FindQualifierNode(parentNode, nextStep.GetName().Substring(1), createNodes
						);
				}
				else
				{
					// This is an array indexing step. First get the index, then get the node.
					if (!parentNode.GetOptions().IsArray())
					{
						throw new XMPException("Indexing applied to non-array", XMPError.BADXPATH);
					}
					if (stepKind == XMPPath.ARRAY_INDEX_STEP)
					{
						index = FindIndexedItem(parentNode, nextStep.GetName(), createNodes);
					}
					else
					{
						if (stepKind == XMPPath.ARRAY_LAST_STEP)
						{
							index = parentNode.GetChildrenLength();
						}
						else
						{
							if (stepKind == XMPPath.FIELD_SELECTOR_STEP)
							{
								String[] result = Utils.SplitNameAndValue(nextStep.GetName());
								String fieldName = result[0];
								String fieldValue = result[1];
								index = LookupFieldSelector(parentNode, fieldName, fieldValue);
							}
							else
							{
								if (stepKind == XMPPath.QUAL_SELECTOR_STEP)
								{
									String[] result = Utils.SplitNameAndValue(nextStep.GetName());
									String qualName = result[0];
									String qualValue = result[1];
									index = LookupQualSelector(parentNode, qualName, qualValue, nextStep.GetAliasForm
										());
								}
								else
								{
									throw new XMPException("Unknown array indexing step in FollowXPathStep", XMPError
										.INTERNALFAILURE);
								}
							}
						}
					}
					if (1 <= index && index <= parentNode.GetChildrenLength())
					{
						nextNode = parentNode.GetChild(index);
					}
				}
			}
			return nextNode;
		}

		/// <summary>Find or create a qualifier node under a given parent node.</summary>
		/// <remarks>
		/// Find or create a qualifier node under a given parent node. Returns a pointer to the
		/// qualifier node, and optionally an iterator for the node's position in
		/// the parent's vector of qualifiers. The iterator is unchanged if no qualifier node (null)
		/// is returned.
		/// <em>Note:</em> On entry, the qualName parameter must not have the leading '?' from the
		/// XMPPath step.
		/// </remarks>
		/// <param name="parent">the parent XMPNode</param>
		/// <param name="qualName">the qualifier name</param>
		/// <param name="createNodes">flag if nodes shall be created</param>
		/// <returns>Returns the qualifier node if found or created, <code>null</code> otherwise.
		/// 	</returns>
		private static XMPNode FindQualifierNode(XMPNode parent, String qualName, bool createNodes
			)
		{
			System.Diagnostics.Debug.Assert(!qualName.StartsWith("?"));
			XMPNode qualNode = parent.FindQualifierByName(qualName);
			if (qualNode == null && createNodes)
			{
				qualNode = new XMPNode(qualName, null);
				qualNode.SetImplicit(true);
				parent.AddQualifier(qualNode);
			}
			return qualNode;
		}

		/// <param name="arrayNode">an array node</param>
		/// <param name="segment">the segment containing the array index</param>
		/// <param name="createNodes">flag if new nodes are allowed to be created.</param>
		/// <returns>Returns the index or index = -1 if not found</returns>
		private static int FindIndexedItem(XMPNode arrayNode, String segment, bool createNodes
			)
		{
			int index = 0;
			try
			{
				segment = segment.JSubstring(1, segment.Length - 1);
				index = System.Convert.ToInt32(segment);
				if (index < 1)
				{
					throw new XMPException("Array index must be larger than zero", XMPError.BADXPATH);
				}
			}
			catch (FormatException)
			{
				throw new XMPException("Array index not digits.", XMPError.BADXPATH);
			}
			if (createNodes && index == arrayNode.GetChildrenLength() + 1)
			{
				// Append a new last + 1 node.
				XMPNode newItem = new XMPNode(ARRAY_ITEM_NAME, null);
				newItem.SetImplicit(true);
				arrayNode.AddChild(newItem);
			}
			return index;
		}

		/// <summary>
		/// Searches for a field selector in a node:
		/// [fieldName="value] - an element in an array of structs, chosen by a field value.
		/// </summary>
		/// <remarks>
		/// Searches for a field selector in a node:
		/// [fieldName="value] - an element in an array of structs, chosen by a field value.
		/// No implicit nodes are created by field selectors.
		/// </remarks>
		/// <param name="arrayNode"/>
		/// <param name="fieldName"/>
		/// <param name="fieldValue"/>
		/// <returns>Returns the index of the field if found, otherwise -1.</returns>
		private static int LookupFieldSelector(XMPNode arrayNode, String fieldName, String
			 fieldValue)
		{
			int result = -1;
			for (int index = 1; index <= arrayNode.GetChildrenLength() && result < 0; index++)
			{
				XMPNode currItem = arrayNode.GetChild(index);
				if (!currItem.GetOptions().IsStruct())
				{
					throw new XMPException("Field selector must be used on array of struct", XMPError
						.BADXPATH);
				}
				for (int f = 1; f <= currItem.GetChildrenLength(); f++)
				{
					XMPNode currField = currItem.GetChild(f);
					if (!fieldName.Equals(currField.GetName()))
					{
						continue;
					}
					if (fieldValue.Equals(currField.GetValue()))
					{
						result = index;
						break;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Searches for a qualifier selector in a node:
		/// [?qualName="value"] - an element in an array, chosen by a qualifier value.
		/// </summary>
		/// <remarks>
		/// Searches for a qualifier selector in a node:
		/// [?qualName="value"] - an element in an array, chosen by a qualifier value.
		/// No implicit nodes are created for qualifier selectors,
		/// except for an alias to an x-default item.
		/// </remarks>
		/// <param name="arrayNode">an array node</param>
		/// <param name="qualName">the qualifier name</param>
		/// <param name="qualValue">the qualifier value</param>
		/// <param name="aliasForm">
		/// in case the qual selector results from an alias,
		/// an x-default node is created if there has not been one.
		/// </param>
		/// <returns>Returns the index of th</returns>
		private static int LookupQualSelector(XMPNode arrayNode, String qualName, String 
			qualValue, int aliasForm)
		{
			if (XML_LANG.Equals(qualName))
			{
				qualValue = iText.Kernel.XMP.Impl.Utils.NormalizeLangValue(qualValue);
				int index = XMPNodeUtils.LookupLanguageItem(arrayNode, qualValue);
				if (index < 0 && (aliasForm & AliasOptions.PROP_ARRAY_ALT_TEXT) > 0)
				{
					XMPNode langNode = new XMPNode(ARRAY_ITEM_NAME, null);
					XMPNode xdefault = new XMPNode(XML_LANG, X_DEFAULT, null);
					langNode.AddQualifier(xdefault);
					arrayNode.AddChild(1, langNode);
					return 1;
				}
				else
				{
					return index;
				}
			}
			else
			{
				for (int index = 1; index < arrayNode.GetChildrenLength(); index++)
				{
					XMPNode currItem = arrayNode.GetChild(index);
					for (IEnumerator it = currItem.IterateQualifier(); it.MoveNext(); )
					{
						XMPNode qualifier = (XMPNode)it.Current;
						if (qualName.Equals(qualifier.GetName()) && qualValue.Equals(qualifier.GetValue()
							))
						{
							return index;
						}
					}
				}
				return -1;
			}
		}

		/// <summary>Make sure the x-default item is first.</summary>
		/// <remarks>
		/// Make sure the x-default item is first. Touch up &quot;single value&quot;
		/// arrays that have a default plus one real language. This case should have
		/// the same value for both items. Older Adobe apps were hardwired to only
		/// use the &quot;x-default&quot; item, so we copy that value to the other
		/// item.
		/// </remarks>
		/// <param name="arrayNode">an alt text array node</param>
		internal static void NormalizeLangArray(XMPNode arrayNode)
		{
			if (!arrayNode.GetOptions().IsArrayAltText())
			{
				return;
			}
			// check if node with x-default qual is first place
			for (int i = 2; i <= arrayNode.GetChildrenLength(); i++)
			{
				XMPNode child = arrayNode.GetChild(i);
				if (child.HasQualifier() && X_DEFAULT.Equals(child.GetQualifier(1).GetValue()))
				{
					// move node to first place
					try
					{
						arrayNode.RemoveChild(i);
						arrayNode.AddChild(1, child);
					}
					catch (XMPException)
					{
						// cannot occur, because same child is removed before
						System.Diagnostics.Debug.Assert(false);
					}
					if (i == 2)
					{
						arrayNode.GetChild(2).SetValue(child.GetValue());
					}
					break;
				}
			}
		}

		/// <summary>See if an array is an alt-text array.</summary>
		/// <remarks>
		/// See if an array is an alt-text array. If so, make sure the x-default item
		/// is first.
		/// </remarks>
		/// <param name="arrayNode">the array node to check if its an alt-text array</param>
		internal static void DetectAltText(XMPNode arrayNode)
		{
			if (arrayNode.GetOptions().IsArrayAlternate() && arrayNode.HasChildren())
			{
				bool isAltText = false;
				for (IEnumerator it = arrayNode.IterateChildren(); it.MoveNext(); )
				{
					XMPNode child = (XMPNode)it.Current;
					if (child.GetOptions().GetHasLanguage())
					{
						isAltText = true;
						break;
					}
				}
				if (isAltText)
				{
					arrayNode.GetOptions().SetArrayAltText(true);
					NormalizeLangArray(arrayNode);
				}
			}
		}

		/// <summary>Appends a language item to an alt text array.</summary>
		/// <param name="arrayNode">the language array</param>
		/// <param name="itemLang">the language of the item</param>
		/// <param name="itemValue">the content of the item</param>
		internal static void AppendLangItem(XMPNode arrayNode, String itemLang, String itemValue
			)
		{
			XMPNode newItem = new XMPNode(ARRAY_ITEM_NAME, itemValue, null);
			XMPNode langQual = new XMPNode(XML_LANG, itemLang, null);
			newItem.AddQualifier(langQual);
			if (!X_DEFAULT.Equals(langQual.GetValue()))
			{
				arrayNode.AddChild(newItem);
			}
			else
			{
				arrayNode.AddChild(1, newItem);
			}
		}

		/// <summary>
		/// Look for an exact match with the specific language.
		/// </summary>
		/// <remarks>
		/// <ul>
		/// <li>Look for an exact match with the specific language.</li>
		/// <li>If a generic language is given, look for partial matches.</li>
		/// <li>Look for an "x-default"-item.</li>
		/// <li>Choose the first item.</li>
		/// </ul>
		/// </remarks>
		/// <param name="arrayNode">the alt text array node</param>
		/// <param name="genericLang">the generic language</param>
		/// <param name="specificLang">the specific language</param>
		/// <returns>
		/// Returns the kind of match as an Integer and the found node in an
		/// array.
		/// </returns>
		internal static Object[] ChooseLocalizedText(XMPNode arrayNode, String genericLang
			, String specificLang)
		{
			// See if the array has the right form. Allow empty alt arrays,
			// that is what parsing returns.
			if (!arrayNode.GetOptions().IsArrayAltText())
			{
				throw new XMPException("Localized text array is not alt-text", XMPError.BADXPATH);
			}
			else
			{
				if (!arrayNode.HasChildren())
				{
					return new Object[] { XMPNodeUtils.CLT_NO_VALUES, null };
				}
			}
			int foundGenericMatches = 0;
			XMPNode resultNode = null;
			XMPNode xDefault = null;
			// Look for the first partial match with the generic language.
			for (IEnumerator it = arrayNode.IterateChildren(); it.MoveNext(); )
			{
				XMPNode currItem = (XMPNode)it.Current;
				// perform some checks on the current item
				if (currItem.GetOptions().IsCompositeProperty())
				{
					throw new XMPException("Alt-text array item is not simple", XMPError.BADXPATH);
				}
				else
				{
					if (!currItem.HasQualifier() || !XML_LANG.Equals(currItem.GetQualifier(1).GetName
						()))
					{
						throw new XMPException("Alt-text array item has no language qualifier", XMPError.
							BADXPATH);
					}
				}
				String currLang = currItem.GetQualifier(1).GetValue();
				// Look for an exact match with the specific language.
				if (specificLang.Equals(currLang))
				{
					return new Object[] { XMPNodeUtils.CLT_SPECIFIC_MATCH, currItem };
				}
				else
				{
					if (genericLang != null && currLang.StartsWith(genericLang))
					{
						if (resultNode == null)
						{
							resultNode = currItem;
						}
						// ! Don't return/break, need to look for other matches.
						foundGenericMatches++;
					}
					else
					{
						if (X_DEFAULT.Equals(currLang))
						{
							xDefault = currItem;
						}
					}
				}
			}
			// evaluate loop
			if (foundGenericMatches == 1)
			{
				return new Object[] { XMPNodeUtils.CLT_SINGLE_GENERIC, resultNode };
			}
			else
			{
				if (foundGenericMatches > 1)
				{
					return new Object[] { XMPNodeUtils.CLT_MULTIPLE_GENERIC, resultNode };
				}
				else
				{
					if (xDefault != null)
					{
						return new Object[] { XMPNodeUtils.CLT_XDEFAULT, xDefault };
					}
					else
					{
						// Everything failed, choose the first item.
						return new Object[] { XMPNodeUtils.CLT_FIRST_ITEM, arrayNode.GetChild(1) };
					}
				}
			}
		}

		/// <summary>Looks for the appropriate language item in a text alternative array.item
		/// 	</summary>
		/// <param name="arrayNode">an array node</param>
		/// <param name="language">the requested language</param>
		/// <returns>Returns the index if the language has been found, -1 otherwise.</returns>
		internal static int LookupLanguageItem(XMPNode arrayNode, String language)
		{
			if (!arrayNode.GetOptions().IsArray())
			{
				throw new XMPException("Language item must be used on array", XMPError.BADXPATH);
			}
			for (int index = 1; index <= arrayNode.GetChildrenLength(); index++)
			{
				XMPNode child = arrayNode.GetChild(index);
				if (!child.HasQualifier() || !XML_LANG.Equals(child.GetQualifier(1).GetName()))
				{
					continue;
				}
				else
				{
					if (language.Equals(child.GetQualifier(1).GetValue()))
					{
						return index;
					}
				}
			}
			return -1;
		}
	}
}
