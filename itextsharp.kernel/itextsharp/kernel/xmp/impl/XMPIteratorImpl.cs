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
using com.itextpdf.kernel.xmp;
using com.itextpdf.kernel.xmp.impl.xpath;
using com.itextpdf.kernel.xmp.options;
using com.itextpdf.kernel.xmp.properties;

namespace com.itextpdf.kernel.xmp.impl
{
	/// <summary>The <code>XMPIterator</code> implementation.</summary>
	/// <remarks>
	/// The <code>XMPIterator</code> implementation.
	/// Iterates the XMP Tree according to a set of options.
	/// During the iteration the XMPMeta-object must not be changed.
	/// Calls to <code>skipSubtree()</code> / <code>skipSiblings()</code> will affect the iteration.
	/// </remarks>
	/// <since>29.06.2006</since>
	public class XMPIteratorImpl : XMPIterator
	{
		/// <summary>stores the iterator options</summary>
		private IteratorOptions options;

		/// <summary>the base namespace of the property path, will be changed during the iteration
		/// 	</summary>
		private String baseNS = null;

		/// <summary>flag to indicate that skipSiblings() has been called.</summary>
		protected internal bool skipSiblings = false;

		/// <summary>flag to indicate that skipSiblings() has been called.</summary>
		protected internal bool skipSubtree = false;

		/// <summary>the node iterator doing the work</summary>
		private IEnumerator nodeIterator = null;

		/// <summary>Constructor with optionsl initial values.</summary>
		/// <remarks>
		/// Constructor with optionsl initial values. If <code>propName</code> is provided,
		/// <code>schemaNS</code> has also be provided.
		/// </remarks>
		/// <param name="xmp">the iterated metadata object.</param>
		/// <param name="schemaNS">the iteration is reduced to this schema (optional)</param>
		/// <param name="propPath">the iteration is redurce to this property within the <code>schemaNS</code>
		/// 	</param>
		/// <param name="options">
		/// advanced iteration options, see
		/// <see cref="com.itextpdf.kernel.xmp.options.IteratorOptions"/>
		/// </param>
		/// <exception cref="com.itextpdf.kernel.xmp.XMPException">If the node defined by the paramters is not existing.
		/// 	</exception>
		public XMPIteratorImpl(XMPMetaImpl xmp, String schemaNS, String propPath, IteratorOptions
			 options)
		{
			// make sure that options is defined at least with defaults
			this.options = options != null ? options : new IteratorOptions();
			// the start node of the iteration depending on the schema and property filter
			XMPNode startNode = null;
			String initialPath = null;
			bool baseSchema = schemaNS != null && schemaNS.Length > 0;
			bool baseProperty = propPath != null && propPath.Length > 0;
			if (!baseSchema && !baseProperty)
			{
				// complete tree will be iterated
				startNode = xmp.GetRoot();
			}
			else
			{
				if (baseSchema && baseProperty)
				{
					// Schema and property node provided
					XMPPath path = XMPPathParser.ExpandXPath(schemaNS, propPath);
					// base path is the prop path without the property leaf
					XMPPath basePath = new XMPPath();
					for (int i = 0; i < path.Size() - 1; i++)
					{
						basePath.Add(path.GetSegment(i));
					}
					startNode = XMPNodeUtils.FindNode(xmp.GetRoot(), path, false, null);
					baseNS = schemaNS;
					initialPath = basePath.ToString();
				}
				else
				{
					if (baseSchema && !baseProperty)
					{
						// Only Schema provided
						startNode = XMPNodeUtils.FindSchemaNode(xmp.GetRoot(), schemaNS, false);
					}
					else
					{
						// !baseSchema  &&  baseProperty
						// No schema but property provided -> error
						throw new XMPException("Schema namespace URI is required", XMPError.BADSCHEMA);
					}
				}
			}
			// create iterator
			if (startNode != null)
			{
				if (!this.options.IsJustChildren())
				{
					nodeIterator = new XMPIteratorImpl.NodeIterator(this, startNode, initialPath, 1);
				}
				else
				{
					nodeIterator = new XMPIteratorImpl.NodeIteratorChildren(this, startNode, initialPath
						);
				}
			}
			else
			{
				// create null iterator
				nodeIterator = java.util.Collections.EmptyIterator();
			}
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPIterator.SkipSubtree()"/>
		public virtual void SkipSubtree()
		{
			this.skipSubtree = true;
		}

		/// <seealso cref="com.itextpdf.kernel.xmp.XMPIterator.SkipSiblings()"/>
		public virtual void SkipSiblings()
		{
			SkipSubtree();
			this.skipSiblings = true;
		}

		/// <seealso cref="System.Collections.IEnumerator{E}.MoveNext()"/>
		public virtual bool MoveNext()
		{
			return nodeIterator.MoveNext();
		}

		/// <seealso cref="System.Collections.IEnumerator{E}.Current()"/>
		public virtual Object Current
		{
			get
			{
				return nodeIterator.Current;
			}
		}

		/// <seealso cref="System.Collections.IEnumerator{E}.Remove()"/>
		public virtual void Remove()
		{
			throw new NotSupportedException("The XMPIterator does not support remove().");
		}

		/// <returns>Exposes the options for inner class.</returns>
		protected internal virtual IteratorOptions GetOptions()
		{
			return options;
		}

		/// <returns>Exposes the options for inner class.</returns>
		protected internal virtual String GetBaseNS()
		{
			return baseNS;
		}

		/// <param name="baseNS">sets the baseNS from the inner class.</param>
		protected internal virtual void SetBaseNS(String baseNS)
		{
			this.baseNS = baseNS;
		}

		/// <summary>The <code>XMPIterator</code> implementation.</summary>
		/// <remarks>
		/// The <code>XMPIterator</code> implementation.
		/// It first returns the node itself, then recursivly the children and qualifier of the node.
		/// </remarks>
		/// <since>29.06.2006</since>
		private class NodeIterator : IEnumerator
		{
			/// <summary>iteration state</summary>
			protected internal const int ITERATE_NODE = 0;

			/// <summary>iteration state</summary>
			protected internal const int ITERATE_CHILDREN = 1;

			/// <summary>iteration state</summary>
			protected internal const int ITERATE_QUALIFIER = 2;

			/// <summary>the state of the iteration</summary>
			private int state = XMPIteratorImpl.NodeIterator.ITERATE_NODE;

			/// <summary>the currently visited node</summary>
			private XMPNode visitedNode;

			/// <summary>the recursively accumulated path</summary>
			private String path;

			/// <summary>the iterator that goes through the children and qualifier list</summary>
			private IEnumerator childrenIterator = null;

			/// <summary>index of node with parent, only interesting for arrays</summary>
			private int index = 0;

			/// <summary>the iterator for each child</summary>
			private IEnumerator subIterator = java.util.Collections.EmptyIterator();

			/// <summary>the cached <code>PropertyInfo</code> to return</summary>
			private XMPPropertyInfo returnProperty = null;

			/// <summary>Default constructor</summary>
			public NodeIterator(XMPIteratorImpl _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <summary>Constructor for the node iterator.</summary>
			/// <param name="visitedNode">the currently visited node</param>
			/// <param name="parentPath">the accumulated path of the node</param>
			/// <param name="index">the index within the parent node (only for arrays)</param>
			public NodeIterator(XMPIteratorImpl _enclosing, XMPNode visitedNode, String parentPath
				, int index)
			{
				this._enclosing = _enclosing;
				// EMPTY
				this.visitedNode = visitedNode;
				this.state = XMPIteratorImpl.NodeIterator.ITERATE_NODE;
				if (visitedNode.GetOptions().IsSchemaNode())
				{
					this._enclosing.SetBaseNS(visitedNode.GetName());
				}
				// for all but the root node and schema nodes
				this.path = this.AccumulatePath(visitedNode, parentPath, index);
			}

			/// <summary>Prepares the next node to return if not already done.</summary>
			/// <seealso cref="System.Collections.IEnumerator{E}.MoveNext()"/>
			public override bool MoveNext()
			{
				if (this.returnProperty != null)
				{
					// hasNext has been called before
					return true;
				}
				// find next node
				if (this.state == XMPIteratorImpl.NodeIterator.ITERATE_NODE)
				{
					return this.ReportNode();
				}
				else
				{
					if (this.state == XMPIteratorImpl.NodeIterator.ITERATE_CHILDREN)
					{
						if (this.childrenIterator == null)
						{
							this.childrenIterator = this.visitedNode.IterateChildren();
						}
						bool hasNext = this.IterateChildren(this.childrenIterator);
						if (!hasNext && this.visitedNode.HasQualifier() && !this._enclosing.GetOptions().
							IsOmitQualifiers())
						{
							this.state = XMPIteratorImpl.NodeIterator.ITERATE_QUALIFIER;
							this.childrenIterator = null;
							hasNext = this.MoveNext();
						}
						return hasNext;
					}
					else
					{
						if (this.childrenIterator == null)
						{
							this.childrenIterator = this.visitedNode.IterateQualifier();
						}
						return this.IterateChildren(this.childrenIterator);
					}
				}
			}

			/// <summary>Sets the returnProperty as next item or recurses into <code>hasNext()</code>.
			/// 	</summary>
			/// <returns>Returns if there is a next item to return.</returns>
			protected internal virtual bool ReportNode()
			{
				this.state = XMPIteratorImpl.NodeIterator.ITERATE_CHILDREN;
				if (this.visitedNode.GetParent() != null && (!this._enclosing.GetOptions().IsJustLeafnodes
					() || !this.visitedNode.HasChildren()))
				{
					this.returnProperty = this.CreatePropertyInfo(this.visitedNode, this._enclosing.GetBaseNS
						(), this.path);
					return true;
				}
				else
				{
					return this.MoveNext();
				}
			}

			/// <summary>Handles the iteration of the children or qualfier</summary>
			/// <param name="iterator">an iterator</param>
			/// <returns>Returns if there are more elements available.</returns>
			private bool IterateChildren(IEnumerator iterator)
			{
				if (this._enclosing.skipSiblings)
				{
					// setSkipSiblings(false);
					this._enclosing.skipSiblings = false;
					this.subIterator = java.util.Collections.EmptyIterator();
				}
				// create sub iterator for every child,
				// if its the first child visited or the former child is finished
				if ((!this.subIterator.MoveNext()) && iterator.MoveNext())
				{
					XMPNode child = (XMPNode)iterator.Current;
					this.index++;
					this.subIterator = new XMPIteratorImpl.NodeIterator(this, child, this.path, this.
						index);
				}
				if (this.subIterator.MoveNext())
				{
					this.returnProperty = (XMPPropertyInfo)this.subIterator.Current;
					return true;
				}
				else
				{
					return false;
				}
			}

			/// <summary>Calls hasNext() and returnes the prepared node.</summary>
			/// <remarks>
			/// Calls hasNext() and returnes the prepared node. Afterwards its set to null.
			/// The existance of returnProperty indicates if there is a next node, otherwise
			/// an exceptio is thrown.
			/// </remarks>
			/// <seealso cref="System.Collections.IEnumerator{E}.Current()"/>
			public override Object Current
			{
				get
				{
					if (this.MoveNext())
					{
						XMPPropertyInfo result = this.returnProperty;
						this.returnProperty = null;
						return result;
					}
					else
					{
						throw new NoSuchElementException("There are no more nodes to return");
					}
				}
			}

			/// <summary>Not supported.</summary>
			/// <seealso cref="System.Collections.IEnumerator{E}.Remove()"/>
			public override void Remove()
			{
				throw new NotSupportedException();
			}

			/// <param name="currNode">the node that will be added to the path.</param>
			/// <param name="parentPath">the path up to this node.</param>
			/// <param name="currentIndex">the current array index if an arrey is traversed</param>
			/// <returns>Returns the updated path.</returns>
			protected internal virtual String AccumulatePath(XMPNode currNode, String parentPath
				, int currentIndex)
			{
				String separator;
				String segmentName;
				if (currNode.GetParent() == null || currNode.GetOptions().IsSchemaNode())
				{
					return null;
				}
				else
				{
					if (currNode.GetParent().GetOptions().IsArray())
					{
						separator = "";
						segmentName = "[" + com.itextpdf.GetStringValueOf(currentIndex) + "]";
					}
					else
					{
						separator = "/";
						segmentName = currNode.GetName();
					}
				}
				if (parentPath == null || parentPath.Length == 0)
				{
					return segmentName;
				}
				else
				{
					if (this._enclosing.GetOptions().IsJustLeafname())
					{
						return !segmentName.StartsWith("?") ? segmentName : segmentName.Substring(1);
					}
					else
					{
						// qualifier
						return parentPath + separator + segmentName;
					}
				}
			}

			/// <summary>Creates a property info object from an <code>XMPNode</code>.</summary>
			/// <param name="node">an <code>XMPNode</code></param>
			/// <param name="baseNS">the base namespace to report</param>
			/// <param name="path">the full property path</param>
			/// <returns>Returns a <code>XMPProperty</code>-object that serves representation of the node.
			/// 	</returns>
			protected internal virtual XMPPropertyInfo CreatePropertyInfo(XMPNode node, String
				 baseNS, String path)
			{
				String value = node.GetOptions().IsSchemaNode() ? null : node.GetValue();
				return new _XMPPropertyInfo_472(node, baseNS, path, value);
			}

			private sealed class _XMPPropertyInfo_472 : XMPPropertyInfo
			{
				public _XMPPropertyInfo_472(XMPNode node, String baseNS, String path, String value
					)
				{
					this.node = node;
					this.baseNS = baseNS;
					this.path = path;
					this.value = value;
				}

				public String GetNamespace()
				{
					if (!node.GetOptions().IsSchemaNode())
					{
						// determine namespace of leaf node
						QName qname = new QName(node.GetName());
						return XMPMetaFactory.GetSchemaRegistry().GetNamespaceURI(qname.GetPrefix());
					}
					else
					{
						return baseNS;
					}
				}

				public String GetPath()
				{
					return path;
				}

				public String GetValue()
				{
					return value;
				}

				public PropertyOptions GetOptions()
				{
					return node.GetOptions();
				}

				public String GetLanguage()
				{
					// the language is not reported
					return null;
				}

				private readonly XMPNode node;

				private readonly String baseNS;

				private readonly String path;

				private readonly String value;
			}

			/// <returns>the childrenIterator</returns>
			protected internal virtual IEnumerator GetChildrenIterator()
			{
				return this.childrenIterator;
			}

			/// <param name="childrenIterator">the childrenIterator to set</param>
			protected internal virtual void SetChildrenIterator(IEnumerator childrenIterator)
			{
				this.childrenIterator = childrenIterator;
			}

			/// <returns>Returns the returnProperty.</returns>
			protected internal virtual XMPPropertyInfo GetReturnProperty()
			{
				return this.returnProperty;
			}

			/// <param name="returnProperty">the returnProperty to set</param>
			protected internal virtual void SetReturnProperty(XMPPropertyInfo returnProperty)
			{
				this.returnProperty = returnProperty;
			}

			private readonly XMPIteratorImpl _enclosing;
		}

		/// <summary>
		/// This iterator is derived from the default <code>NodeIterator</code>,
		/// and is only used for the option
		/// <see cref="com.itextpdf.kernel.xmp.options.IteratorOptions.JUST_CHILDREN"/>
		/// .
		/// </summary>
		/// <since>02.10.2006</since>
		private class NodeIteratorChildren : XMPIteratorImpl.NodeIterator
		{
			private String parentPath;

			private IEnumerator childrenIterator;

			private int index = 0;

			/// <summary>Constructor</summary>
			/// <param name="parentNode">the node which children shall be iterated.</param>
			/// <param name="parentPath">the full path of the former node without the leaf node.</param>
			public NodeIteratorChildren(XMPIteratorImpl _enclosing, XMPNode parentNode, String
				 parentPath)
				: base(_enclosing)
			{
				this._enclosing = _enclosing;
				if (parentNode.GetOptions().IsSchemaNode())
				{
					this._enclosing.SetBaseNS(parentNode.GetName());
				}
				this.parentPath = this.AccumulatePath(parentNode, parentPath, 1);
				this.childrenIterator = parentNode.IterateChildren();
			}

			/// <summary>Prepares the next node to return if not already done.</summary>
			/// <seealso cref="System.Collections.IEnumerator{E}.MoveNext()"/>
			public override bool MoveNext()
			{
				if (this.GetReturnProperty() != null)
				{
					// hasNext has been called before
					return true;
				}
				else
				{
					if (this._enclosing.skipSiblings)
					{
						return false;
					}
					else
					{
						if (this.childrenIterator.MoveNext())
						{
							XMPNode child = (XMPNode)this.childrenIterator.Current;
							this.index++;
							String path = null;
							if (child.GetOptions().IsSchemaNode())
							{
								this._enclosing.SetBaseNS(child.GetName());
							}
							else
							{
								if (child.GetParent() != null)
								{
									// for all but the root node and schema nodes
									path = this.AccumulatePath(child, this.parentPath, this.index);
								}
							}
							// report next property, skip not-leaf nodes in case options is set
							if (!this._enclosing.GetOptions().IsJustLeafnodes() || !child.HasChildren())
							{
								this.SetReturnProperty(this.CreatePropertyInfo(child, this._enclosing.GetBaseNS()
									, path));
								return true;
							}
							else
							{
								return this.MoveNext();
							}
						}
						else
						{
							return false;
						}
					}
				}
			}

			private readonly XMPIteratorImpl _enclosing;
		}
	}
}
