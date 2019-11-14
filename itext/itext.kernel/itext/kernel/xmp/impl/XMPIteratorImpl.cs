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
using iText.IO.Util;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Impl.XPath;
using iText.Kernel.XMP.Options;
using iText.Kernel.XMP.Properties;

namespace iText.Kernel.XMP.Impl
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
		private static readonly IList EmptyList = new ArrayList();

		/// <summary>stores the iterator options</summary>
		private readonly IteratorOptions options;

		/// <summary>the base namespace of the property path, will be changed during the iteration
		/// 	</summary>
		private String baseNS = null;

		/// <summary>flag to indicate that skipSiblings() has been called.</summary>
		protected internal bool skipSiblings = false;

		/// <summary>flag to indicate that skipSiblings() has been called.</summary>
		protected internal bool skipSubtree = false;

		/// <summary>the node iterator doing the work</summary>
		private readonly IEnumerator nodeIterator = null;

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
		/// <see cref="iText.Kernel.XMP.Options.IteratorOptions"/>
		/// </param>
		public XMPIteratorImpl(XMPMetaImpl xmp, String schemaNS, String propPath, IteratorOptions
			 options)
		{
			// make sure that options is defined at least with defaults
			this.options = options ?? new IteratorOptions();

			// the start node of the iteration depending on the schema and property filter
			XMPNode startNode;
			string initialPath = null;
			bool baseSchema = !String.IsNullOrEmpty(schemaNS);
			bool baseProperty = !String.IsNullOrEmpty(propPath);

			if (!baseSchema && !baseProperty) {
				// complete tree will be iterated
				startNode = xmp.GetRoot();
			}
			else if (baseSchema && baseProperty) {
				// Schema and property node provided
				XMPPath path = XMPPathParser.ExpandXPath(schemaNS, propPath);

				// base path is the prop path without the property leaf
				XMPPath basePath = new XMPPath();
				for (int i = 0; i < path.Size() - 1; i++) {
					basePath.Add(path.GetSegment(i));
				}

				startNode = XMPNodeUtils.FindNode(xmp.GetRoot(), path, false, null);
				this.baseNS = schemaNS;
				initialPath = basePath.ToString();
			}
			else if (baseSchema && !baseProperty) {
				// Only Schema provided
				startNode = XMPNodeUtils.FindSchemaNode(xmp.GetRoot(), schemaNS, false);
			}
			else // !baseSchema  &&  baseProperty
			{
				// No schema but property provided -> error
				throw new XMPException("Schema namespace URI is required", XMPError.BADSCHEMA);
			}


			// create iterator
			if (startNode != null) {
				this.nodeIterator = (!this.options.IsJustChildren())
					? new NodeIterator(this, startNode, initialPath, 1)
					: new NodeIteratorChildren(this, startNode, initialPath);
			}
			else {
				// create null iterator
				this.nodeIterator = EmptyList.GetEnumerator();
			}
		}

		/// <seealso cref="iText.Kernel.XMP.XMPIterator.SkipSubtree()"/>
		public virtual void SkipSubtree()
		{
			this.skipSubtree = true;
		}

		/// <seealso cref="iText.Kernel.XMP.XMPIterator.SkipSiblings()"/>
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

		public virtual void Reset() {
			nodeIterator.Reset();
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
		private class NodeIterator : IEnumerator {
			/// <summary>
			/// iteration state </summary>
			private const int ITERATE_NODE = 0;

			/// <summary>
			/// iteration state </summary>
			private const int ITERATE_CHILDREN = 1;

			/// <summary>
			/// iteration state </summary>
			private const int ITERATE_QUALIFIER = 2;

			private static readonly IList EmptyList = new ArrayList();

			private readonly XMPIteratorImpl outerInstance;

			/// <summary>
			/// the recursively accumulated path </summary>
			private readonly string path;

			/// <summary>
			/// the currently visited node </summary>
			private readonly XMPNode visitedNode;

			/// <summary>
			/// the iterator that goes through the children and qualifier list </summary>
			private IEnumerator childrenIterator;

			/// <summary>
			/// index of node with parent, only interesting for arrays </summary>
			private int index;

			/// <summary>
			/// the cached <code>PropertyInfo</code> to return </summary>
			private XMPPropertyInfo returnProperty;

			/// <summary>
			/// the state of the iteration </summary>
			private int state = ITERATE_NODE;

			/// <summary>
			/// the iterator for each child </summary>
			private IEnumerator subIterator = EmptyList.GetEnumerator();

			/// <summary>
			/// Constructor for the node iterator. </summary>
			/// <param name="visitedNode"> the currently visited node </param>
			/// <param name="parentPath"> the accumulated path of the node </param>
			/// <param name="index"> the index within the parent node (only for arrays) </param>
			public NodeIterator(XMPIteratorImpl outerInstance, XMPNode visitedNode, string parentPath, int index) {
				this.outerInstance = outerInstance;
				this.visitedNode = visitedNode;
				this.state = ITERATE_NODE;
				if (visitedNode.GetOptions().IsSchemaNode()) {
					outerInstance.SetBaseNS(visitedNode.GetName());
				}

				// for all but the root node and schema nodes
				this.path = AccumulatePath(visitedNode, parentPath, index);
			}

			/// <returns> the childrenIterator </returns>
			protected internal virtual IEnumerator GetChildrenIterator() {
				return childrenIterator;
			}


			/// <returns> Returns the returnProperty. </returns>
			protected internal virtual XMPPropertyInfo GetReturnProperty() {
				return returnProperty;
			}

			protected internal virtual void SetReturnProperty(XMPPropertyInfo value) {
				returnProperty = value;
			}

			public virtual Object Current {
				get { return returnProperty; }
			}

			/// <summary>
			/// Prepares the next node to return if not already done. 
			/// </summary>
			/// <seealso cref="IEnumerator.MoveNext"/>
			public virtual bool MoveNext() {
				// find next node
				if (state == ITERATE_NODE) {
					return ReportNode();
				}
				if (state == ITERATE_CHILDREN) {
					if (childrenIterator == null) {
						childrenIterator = visitedNode.IterateChildren();
					}

					bool hasNext = IterateChildren(childrenIterator);

					if (!hasNext && visitedNode.HasQualifier() && !outerInstance.GetOptions().IsOmitQualifiers()) {
						state = ITERATE_QUALIFIER;
						childrenIterator = null;
						hasNext = MoveNext();
					}
					return hasNext;
				}
				if (childrenIterator == null) {
					childrenIterator = visitedNode.IterateQualifier();
				}

				return IterateChildren(childrenIterator);
			}

			public virtual void Reset() {
				throw new NotSupportedException();
			}

			/// <summary>
			/// Sets the returnProperty as next item or recurses into <code>hasNext()</code>. </summary>
			/// <returns> Returns if there is a next item to return.  </returns>
			protected internal virtual bool ReportNode() {
				state = ITERATE_CHILDREN;
				if (visitedNode.GetParent() != null &&
					(!outerInstance.GetOptions().IsJustLeafnodes() || !visitedNode.HasChildren())) {
					returnProperty = CreatePropertyInfo(visitedNode, outerInstance.GetBaseNS(), path);
					return true;
				}
				return MoveNext();
			}

			/// <summary>
			/// Handles the iteration of the children or qualfier </summary>
			/// <param name="iterator"> an iterator </param>
			/// <returns> Returns if there are more elements available. </returns>
			private bool IterateChildren(IEnumerator iterator) {
				if (outerInstance.skipSiblings) {
					// setSkipSiblings(false);
					outerInstance.skipSiblings = false;
					subIterator = EmptyList.GetEnumerator();
				}

				// create sub iterator for every child,
				// if its the first child visited or the former child is finished 
				bool subIteratorMoveNext = subIterator.MoveNext();
				if (!subIteratorMoveNext && iterator.MoveNext()) {
					XMPNode child = (XMPNode) iterator.Current;
					index++;
					subIterator = new NodeIterator(outerInstance, child, path, index);
				}
				if (subIteratorMoveNext) {
					returnProperty = (XMPPropertyInfo) subIterator.Current;
					return true;
				}
				return false;
			}
				
			/// <param name="currNode"> the node that will be added to the path. </param>
			/// <param name="parentPath"> the path up to this node. </param>
			/// <param name="currentIndex"> the current array index if an arrey is traversed </param>
			/// <returns> Returns the updated path. </returns>
			protected internal virtual String AccumulatePath(XMPNode currNode, string parentPath, int currentIndex) {
				String separator;
				String segmentName;
				if (currNode.GetParent() == null || currNode.GetOptions().IsSchemaNode()) {
					return null;
				}
				if (currNode.GetParent().GetOptions().IsArray()) {
					separator = "";
					segmentName = "[" + Convert.ToString(currentIndex) + "]";
				}
				else {
					separator = "/";
					segmentName = currNode.GetName();
				}


				if (String.IsNullOrEmpty(parentPath)) {
					return segmentName;
				}
				if (outerInstance.GetOptions().IsJustLeafname()) {
					return !segmentName.StartsWith("?") ? segmentName : segmentName.Substring(1); // qualifier
				}
				return parentPath + separator + segmentName;
			}

			/// <summary>
			/// Creates a property info object from an <code>XMPNode</code>. </summary>
			/// <param name="node"> an <code>XMPNode</code> </param>
			/// <param name="baseNs"> the base namespace to report </param>
			/// <param name="path"> the full property path </param>
			/// <returns> Returns a <code>XMPProperty</code>-object that serves representation of the node. </returns>
			protected internal virtual XMPPropertyInfo CreatePropertyInfo(XMPNode node, String baseNS, String path) {
				String value = node.GetOptions().IsSchemaNode() ? null : node.GetValue();
				return new XMPPropertyInfoImpl(node, baseNS, path, value);
			}

			private class XMPPropertyInfoImpl : XMPPropertyInfo {
				
				private readonly string baseNs;
				private readonly XMPNode node;
				private readonly string path;
				private readonly string value;

				public XMPPropertyInfoImpl(XMPNode node, string baseNs, string path, string value) {
					this.node = node;
					this.baseNs = baseNs;
					this.path = path;
					this.value = value;
				}

				public virtual string GetNamespace() {
					if (!node.GetOptions().IsSchemaNode()) {
						// determine namespace of leaf node
						QName qname = new QName(node.GetName());
						return XMPMetaFactory.GetSchemaRegistry().GetNamespaceURI(qname.GetPrefix());
					}
					return baseNs;
				}

				public virtual string GetPath() {
					return path;
				}

				public virtual string GetValue() {
					return value;
				}

				public virtual PropertyOptions GetOptions() {
					return node.GetOptions();
				}

				public virtual string GetLanguage() {
					// the language is not reported
					return null;
				}
			}
		}

		/// <summary>
		/// This iterator is derived from the default <code>NodeIterator</code>,
		/// and is only used for the option <seealso cref="IteratorOptions.JUST_CHILDREN"/>.
		/// 
		/// @since 02.10.2006
		/// </summary>
		private class NodeIteratorChildren : NodeIterator {
			private readonly IEnumerator childrenIterator;
			private readonly XMPIteratorImpl outerInstance;

			private readonly string parentPath;
			private int index;


			/// <summary>
			/// Constructor </summary>
			/// <param name="parentNode"> the node which children shall be iterated. </param>
			/// <param name="parentPath"> the full path of the former node without the leaf node. </param>
			public NodeIteratorChildren(XMPIteratorImpl outerInstance, XMPNode parentNode, string parentPath)
				: base(outerInstance, parentNode, parentPath, 0) {
				this.outerInstance = outerInstance;
				if (parentNode.GetOptions().IsSchemaNode()) {
					outerInstance.SetBaseNS(parentNode.GetName());
				}
				this.parentPath = AccumulatePath(parentNode, parentPath, 1);
				this.childrenIterator = parentNode.IterateChildren();
			}


			/// <summary>
			/// Prepares the next node to return if not already done. 
			/// </summary>
			/// <seealso cref="IEnumerator.MoveNext"/>
			public override bool MoveNext() {
				if (outerInstance.skipSiblings) {
					return false;
				}
				if (childrenIterator.MoveNext()) {
					XMPNode child = (XMPNode) childrenIterator.Current;
					if (child != null) {
						index++;
						string path = null;
						if (child.GetOptions().IsSchemaNode()) {
							outerInstance.SetBaseNS(child.GetName());
						}
						else if (child.GetParent() != null) {
							// for all but the root node and schema nodes
							path = AccumulatePath(child, parentPath, index);
						}

						// report next property, skip not-leaf nodes in case options is set
						if (!outerInstance.GetOptions().IsJustLeafnodes() || !child.HasChildren()) {
							SetReturnProperty(CreatePropertyInfo(child, outerInstance.GetBaseNS(), path));
							return true;
						}
					}
					return MoveNext();
				}
				return false;
			}
		}
	}
}
