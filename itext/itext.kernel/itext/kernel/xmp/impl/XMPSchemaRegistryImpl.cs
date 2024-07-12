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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using iText.IO.Util;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Kernel.XMP.Properties;

namespace iText.Kernel.XMP.Impl
{
	/// <summary>The schema registry handles the namespaces, aliases and global options for the XMP Toolkit.
	/// 	</summary>
	/// <remarks>
	/// The schema registry handles the namespaces, aliases and global options for the XMP Toolkit. There
	/// is only one single instance used by the toolkit.
	/// </remarks>
	/// <since>27.01.2006</since>
	public sealed class XMPSchemaRegistryImpl : XMPConst, XMPSchemaRegistry
	{
		/// <summary>a map from a namespace URI to its registered prefix</summary>
		private IDictionary namespaceToPrefixMap = new Hashtable();

		/// <summary>a map from a prefix to the associated namespace URI</summary>
		private IDictionary prefixToNamespaceMap = new Hashtable();

		/// <summary>a map of all registered aliases.</summary>
		/// <remarks>
		/// a map of all registered aliases.
		/// The map is a relationship from a qname to an <code>XMPAliasInfo</code>-object.
		/// </remarks>
		private IDictionary aliasMap = new Hashtable();

		/// <summary>The pattern that must not be contained in simple properties</summary>
		private readonly Regex _regex = new Regex("[/*?\\[\\]]");

		/// <summary>
		/// Performs the initialisation of the registry with the default namespaces, aliases and global
		/// options.
		/// </summary>
		public XMPSchemaRegistryImpl()
		{
			try
			{
				RegisterStandardNamespaces();
				RegisterStandardAliases();
			}
			catch (XMPException)
			{
				throw new Exception("The XMPSchemaRegistry cannot be initialized!");
			}
		}

		// ---------------------------------------------------------------------------------------------
		// Namespace Functions
		/// <seealso cref="iText.Kernel.XMP.XMPSchemaRegistry.RegisterNamespace(System.String, System.String)
		/// 	"/>
		public String RegisterNamespace(String namespaceURI, String suggestedPrefix)
		{
			lock (this)
			{
				ParameterAsserts.AssertSchemaNS(namespaceURI);
				ParameterAsserts.AssertPrefix(suggestedPrefix);
				if (suggestedPrefix[suggestedPrefix.Length - 1] != ':')
				{
					suggestedPrefix += ':';
				}
				if (!Utils.IsXMLNameNS(suggestedPrefix.JSubstring(0, suggestedPrefix.Length - 1)))
				{
					throw new XMPException("The prefix is a bad XML name", XMPError.BADXML);
				}
				String registeredPrefix = (String)namespaceToPrefixMap[namespaceURI];
				String registeredNS = (String)prefixToNamespaceMap[suggestedPrefix];
				if (registeredPrefix != null)
				{
					// Return the actual prefix
					return registeredPrefix;
				}
				else
				{
					if (registeredNS != null)
					{
						// the namespace is new, but the prefix is already engaged,
						// we generate a new prefix out of the suggested
						String generatedPrefix = suggestedPrefix;
						for (int i = 1; prefixToNamespaceMap.Contains(generatedPrefix); i++)
						{
							generatedPrefix = suggestedPrefix.JSubstring(0, suggestedPrefix.Length - 1) + "_"
								 + i + "_:";
						}
						suggestedPrefix = generatedPrefix;
					}
					prefixToNamespaceMap[suggestedPrefix] = namespaceURI;
					namespaceToPrefixMap[namespaceURI] = suggestedPrefix;
					// Return the suggested prefix
					return suggestedPrefix;
				}
			}
		}

		/// <seealso cref="iText.Kernel.XMP.XMPSchemaRegistry.DeleteNamespace(System.String)
		/// 	"/>
		public void DeleteNamespace(String namespaceURI)
		{
			lock (this)
			{
				String prefixToDelete = GetNamespacePrefix(namespaceURI);
				if (prefixToDelete != null)
				{
					if (namespaceToPrefixMap.Contains(namespaceURI))
						namespaceToPrefixMap.Remove(namespaceURI);
					if (prefixToNamespaceMap.Contains(prefixToDelete))
						prefixToNamespaceMap.Remove(prefixToDelete);
				}
			}
		}

		/// <seealso cref="iText.Kernel.XMP.XMPSchemaRegistry.GetNamespacePrefix(System.String)
		/// 	"/>
		public String GetNamespacePrefix(String namespaceURI)
		{
			lock (this)
			{
				return (String)namespaceToPrefixMap[namespaceURI];
			}
		}

		/// <seealso cref="iText.Kernel.XMP.XMPSchemaRegistry.GetNamespaceURI(System.String)
		/// 	"/>
		public String GetNamespaceURI(String namespacePrefix)
		{
			lock (this)
			{
				if (namespacePrefix != null && !namespacePrefix.EndsWith(":"))
				{
					namespacePrefix += ":";
				}
				return (String)prefixToNamespaceMap[namespacePrefix];
			}
		}

		/// <seealso cref="iText.Kernel.XMP.XMPSchemaRegistry.GetNamespaces()"/>
		public IDictionary GetNamespaces()
		{
			lock (this)
			{
				return ReadOnlyDictionary.ReadOnly(new Hashtable(namespaceToPrefixMap));
			}
		}

		/// <seealso cref="iText.Kernel.XMP.XMPSchemaRegistry.GetPrefixes()"/>
		public IDictionary GetPrefixes()
		{
			lock (this)
			{
				return ReadOnlyDictionary.ReadOnly(new Hashtable(prefixToNamespaceMap));
			}
		}

		/// <summary>
		/// Register the standard namespaces of schemas and types that are included in the XMP
		/// Specification and some other Adobe private namespaces.
		/// </summary>
		/// <remarks>
		/// Register the standard namespaces of schemas and types that are included in the XMP
		/// Specification and some other Adobe private namespaces.
		/// Note: This method is not lock because only called by the constructor.
		/// </remarks>
		private void RegisterStandardNamespaces()
		{
			// register standard namespaces
			RegisterNamespace(NS_XML, "xml");
			RegisterNamespace(NS_RDF, "rdf");
			RegisterNamespace(NS_DC, "dc");
			RegisterNamespace(NS_IPTCCORE, "Iptc4xmpCore");
			RegisterNamespace(NS_IPTCEXT, "Iptc4xmpExt");
			RegisterNamespace(NS_DICOM, "DICOM");
			RegisterNamespace(NS_PLUS, "plus");
			// register Adobe standard namespaces
			RegisterNamespace(NS_X, "x");
			RegisterNamespace(NS_IX, "iX");
			RegisterNamespace(NS_XMP, "xmp");
			RegisterNamespace(NS_XMP_RIGHTS, "xmpRights");
			RegisterNamespace(NS_XMP_MM, "xmpMM");
			RegisterNamespace(NS_XMP_BJ, "xmpBJ");
			RegisterNamespace(NS_XMP_NOTE, "xmpNote");
			RegisterNamespace(NS_PDF, "pdf");
			RegisterNamespace(NS_PDFX, "pdfx");
			RegisterNamespace(NS_PDFX_ID, "pdfxid");
			RegisterNamespace(NS_PDFA_SCHEMA, "pdfaSchema");
			RegisterNamespace(NS_PDFA_PROPERTY, "pdfaProperty");
			RegisterNamespace(NS_PDFA_TYPE, "pdfaType");
			RegisterNamespace(NS_PDFA_FIELD, "pdfaField");
			RegisterNamespace(NS_PDFA_ID, "pdfaid");
			RegisterNamespace(NS_PDFUA_ID, "pdfuaid");
			RegisterNamespace(NS_PDFA_EXTENSION, "pdfaExtension");
			RegisterNamespace(NS_PHOTOSHOP, "photoshop");
			RegisterNamespace(NS_PSALBUM, "album");
			RegisterNamespace(NS_EXIF, "exif");
			RegisterNamespace(NS_EXIFX, "exifEX");
			RegisterNamespace(NS_EXIF_AUX, "aux");
			RegisterNamespace(NS_TIFF, "tiff");
			RegisterNamespace(NS_PNG, "png");
			RegisterNamespace(NS_JPEG, "jpeg");
			RegisterNamespace(NS_JP2K, "jp2k");
			RegisterNamespace(NS_CAMERARAW, "crs");
			RegisterNamespace(NS_ADOBESTOCKPHOTO, "bmsp");
			RegisterNamespace(NS_CREATOR_ATOM, "creatorAtom");
			RegisterNamespace(NS_ASF, "asf");
			RegisterNamespace(NS_WAV, "wav");
			RegisterNamespace(NS_BWF, "bext");
			RegisterNamespace(NS_RIFFINFO, "riffinfo");
			RegisterNamespace(NS_SCRIPT, "xmpScript");
			RegisterNamespace(NS_TXMP, "txmp");
			RegisterNamespace(NS_SWF, "swf");
			// register Adobe private namespaces
			RegisterNamespace(NS_DM, "xmpDM");
			RegisterNamespace(NS_TRANSIENT, "xmpx");
			// register Adobe standard type namespaces
			RegisterNamespace(TYPE_TEXT, "xmpT");
			RegisterNamespace(TYPE_PAGEDFILE, "xmpTPg");
			RegisterNamespace(TYPE_GRAPHICS, "xmpG");
			RegisterNamespace(TYPE_IMAGE, "xmpGImg");
			RegisterNamespace(TYPE_FONT, "stFnt");
			RegisterNamespace(TYPE_DIMENSIONS, "stDim");
			RegisterNamespace(TYPE_RESOURCEEVENT, "stEvt");
			RegisterNamespace(TYPE_RESOURCEREF, "stRef");
			RegisterNamespace(TYPE_ST_VERSION, "stVer");
			RegisterNamespace(TYPE_ST_JOB, "stJob");
			RegisterNamespace(TYPE_MANIFESTITEM, "stMfs");
			RegisterNamespace(TYPE_IDENTIFIERQUAL, "xmpidq");
		}

		// ---------------------------------------------------------------------------------------------
		// Alias Functions
		/// <seealso cref="iText.Kernel.XMP.XMPSchemaRegistry.ResolveAlias(System.String, System.String)
		/// 	"/>
		public XMPAliasInfo ResolveAlias(String aliasNS, String aliasProp)
		{
			lock (this)
			{
				String aliasPrefix = GetNamespacePrefix(aliasNS);
				if (aliasPrefix == null)
				{
					return null;
				}
				return (XMPAliasInfo)aliasMap[aliasPrefix + aliasProp];
			}
		}

		/// <seealso cref="iText.Kernel.XMP.XMPSchemaRegistry.FindAlias(System.String)" />
		public XMPAliasInfo FindAlias(String qname)
		{
			lock (this)
			{
				return (XMPAliasInfo)aliasMap[qname];
			}
		}

		/// <seealso cref="iText.Kernel.XMP.XMPSchemaRegistry.FindAliases(System.String)"/>
		public XMPAliasInfo[] FindAliases(String aliasNS)
		{
			lock (this)
			{
				String prefix = GetNamespacePrefix(aliasNS);
				IList<XMPAliasInfo> result = new List<XMPAliasInfo>();
				if (prefix != null)
				{
					foreach (Object key in aliasMap.Keys)
					{
						String qname = (String)key;
						if (qname.StartsWith(prefix))
						{
							result.Add(FindAlias(qname));
						}
					}
				}
				return result.ToArray(new XMPAliasInfo[result.Count]);
			}
		}
		//\cond DO_NOT_DOCUMENT
		/// <summary>Associates an alias name with an actual name.</summary>
		/// <remarks>
		/// Associates an alias name with an actual name.
		/// <para>
		/// Define a alias mapping from one namespace/property to another. Both
		/// property names must be simple names. An alias can be a direct mapping,
		/// where the alias and actual have the same data type. It is also possible
		/// to map a simple alias to an item in an array. This can either be to the
		/// first item in the array, or to the 'x-default' item in an alt-text array.
		/// Multiple alias names may map to the same actual, as long as the forms
		/// match. It is a no-op to reregister an alias in an identical fashion.
		/// Note: This method is not locking because only called by registerStandardAliases
		/// which is only called by the constructor.
		/// Note2: The method is only package-private so that it can be tested with unittests
		/// </para>
		/// </remarks>
		/// <param name="aliasNS">
		/// The namespace URI for the alias. Must not be null or the empty
		/// string.
		/// </param>
		/// <param name="aliasProp">
		/// The name of the alias. Must be a simple name, not null or the
		/// empty string and not a general path expression.
		/// </param>
		/// <param name="actualNS">
		/// The namespace URI for the actual. Must not be null or the
		/// empty string.
		/// </param>
		/// <param name="actualProp">
		/// The name of the actual. Must be a simple name, not null or the
		/// empty string and not a general path expression.
		/// </param>
		/// <param name="aliasForm">
		/// Provides options for aliases for simple aliases to array
		/// items. This is needed to know what kind of array to create if
		/// set for the first time via the simple alias. Pass
		/// <code>XMP_NoOptions</code>, the default value, for all
		/// direct aliases regardless of whether the actual data type is
		/// an array or not (see
		/// <see cref="iText.Kernel.XMP.Options.AliasOptions"/>
		/// ).
		/// </param>
		internal void RegisterAlias(String aliasNS, String aliasProp, String actualNS, String
			 actualProp, AliasOptions aliasForm)
		{
			lock (this)
			{
				ParameterAsserts.AssertSchemaNS(aliasNS);
				ParameterAsserts.AssertPropName(aliasProp);
				ParameterAsserts.AssertSchemaNS(actualNS);
				ParameterAsserts.AssertPropName(actualProp);
				// Fix the alias options
				AliasOptions aliasOpts = aliasForm != null ? new AliasOptions(XMPNodeUtils.VerifySetOptions
					(aliasForm.ToPropertyOptions(), null).GetOptions()) : new AliasOptions();
				if (_regex.IsMatch(aliasProp) || _regex.IsMatch(actualProp)) {
					throw new XMPException("Alias and actual property names must be simple", XMPError.BADXPATH);
				}
				// check if both namespaces are registered
				String aliasPrefix = GetNamespacePrefix(aliasNS);
				String actualPrefix = GetNamespacePrefix(actualNS);
				if (aliasPrefix == null)
				{
					throw new XMPException("Alias namespace is not registered", XMPError.BADSCHEMA);
				}
				else
				{
					if (actualPrefix == null)
					{
						throw new XMPException("Actual namespace is not registered", XMPError.BADSCHEMA);
					}
				}
				String key = aliasPrefix + aliasProp;
				// check if alias is already existing
				if (aliasMap.Contains(key))
				{
					throw new XMPException("Alias is already existing", XMPError.BADPARAM);
				}
				else
				{
					if (aliasMap.Contains(actualPrefix + actualProp))
					{
						throw new XMPException("Actual property is already an alias, use the base property"
							, XMPError.BADPARAM);
					}
				}
				XMPAliasInfo aliasInfo = new _XMPAliasInfo_409(actualNS, actualPrefix, actualProp
					, aliasOpts);
				aliasMap[key] = aliasInfo;
			}
		}
		//\endcond	

		private sealed class _XMPAliasInfo_409 : XMPAliasInfo
		{
			public _XMPAliasInfo_409(String actualNS, String actualPrefix, String actualProp, 
				AliasOptions aliasOpts)
			{
				this.actualNS = actualNS;
				this.actualPrefix = actualPrefix;
				this.actualProp = actualProp;
				this.aliasOpts = aliasOpts;
			}

			/// <seealso cref="iText.Kernel.XMP.Properties.XMPAliasInfo.GetNamespace()"/>
			public String GetNamespace()
			{
				return actualNS;
			}

			/// <seealso cref="iText.Kernel.XMP.Properties.XMPAliasInfo.GetPrefix()"/>
			public String GetPrefix()
			{
				return actualPrefix;
			}

			/// <seealso cref="iText.Kernel.XMP.Properties.XMPAliasInfo.GetPropName()"/>
			public String GetPropName()
			{
				return actualProp;
			}

			/// <seealso cref="iText.Kernel.XMP.Properties.XMPAliasInfo.GetAliasForm()"/>
			public AliasOptions GetAliasForm()
			{
				return aliasOpts;
			}

			public override String ToString()
			{
				return actualPrefix + actualProp + " NS(" + actualNS + "), FORM (" + this.GetAliasForm
					() + ")";
			}

			private readonly String actualNS;

			private readonly String actualPrefix;

			private readonly String actualProp;

			private readonly AliasOptions aliasOpts;
		}

		/// <seealso cref="iText.Kernel.XMP.XMPSchemaRegistry.GetAliases()"/>
		public IDictionary GetAliases()
		{
			lock (this)
			{
				return ReadOnlyDictionary.ReadOnly(new Hashtable(aliasMap
					));
			}
		}

		/// <summary>Register the standard aliases.</summary>
		/// <remarks>
		/// Register the standard aliases.
		/// Note: This method is not lock because only called by the constructor.
		/// </remarks>
		private void RegisterStandardAliases()
		{
			AliasOptions aliasToArrayOrdered = new AliasOptions().SetArrayOrdered(true);
			AliasOptions aliasToArrayAltText = new AliasOptions().SetArrayAltText(true);
			// Aliases from XMP to DC.
			RegisterAlias(NS_XMP, "Author", NS_DC, "creator", aliasToArrayOrdered);
			RegisterAlias(NS_XMP, "Authors", NS_DC, "creator", null);
			RegisterAlias(NS_XMP, "Description", NS_DC, "description", null);
			RegisterAlias(NS_XMP, "Format", NS_DC, "format", null);
			RegisterAlias(NS_XMP, "Keywords", NS_DC, "subject", null);
			RegisterAlias(NS_XMP, "Locale", NS_DC, "language", null);
			RegisterAlias(NS_XMP, "Title", NS_DC, "title", null);
			RegisterAlias(NS_XMP_RIGHTS, "Copyright", NS_DC, "rights", null);
			// Aliases from PDF to DC and XMP.
			RegisterAlias(NS_PDF, "Author", NS_DC, "creator", aliasToArrayOrdered);
			RegisterAlias(NS_PDF, "BaseURL", NS_XMP, "BaseURL", null);
			RegisterAlias(NS_PDF, "CreationDate", NS_XMP, "CreateDate", null);
			RegisterAlias(NS_PDF, "Creator", NS_XMP, "CreatorTool", null);
			RegisterAlias(NS_PDF, "ModDate", NS_XMP, "ModifyDate", null);
			RegisterAlias(NS_PDF, "Subject", NS_DC, "description", aliasToArrayAltText);
			RegisterAlias(NS_PDF, "Title", NS_DC, "title", aliasToArrayAltText);
			// Aliases from PHOTOSHOP to DC and XMP.
			RegisterAlias(NS_PHOTOSHOP, "Author", NS_DC, "creator", aliasToArrayOrdered);
			RegisterAlias(NS_PHOTOSHOP, "Caption", NS_DC, "description", aliasToArrayAltText);
			RegisterAlias(NS_PHOTOSHOP, "Copyright", NS_DC, "rights", aliasToArrayAltText);
			RegisterAlias(NS_PHOTOSHOP, "Keywords", NS_DC, "subject", null);
			RegisterAlias(NS_PHOTOSHOP, "Marked", NS_XMP_RIGHTS, "Marked", null);
			RegisterAlias(NS_PHOTOSHOP, "Title", NS_DC, "title", aliasToArrayAltText);
			RegisterAlias(NS_PHOTOSHOP, "WebStatement", NS_XMP_RIGHTS, "WebStatement", null);
			// Aliases from TIFF and EXIF to DC and XMP.
			RegisterAlias(NS_TIFF, "Artist", NS_DC, "creator", aliasToArrayOrdered);
			RegisterAlias(NS_TIFF, "Copyright", NS_DC, "rights", null);
			RegisterAlias(NS_TIFF, "DateTime", NS_XMP, "ModifyDate", null);
			RegisterAlias(NS_TIFF, "ImageDescription", NS_DC, "description", null);
			RegisterAlias(NS_TIFF, "Software", NS_XMP, "CreatorTool", null);
			// Aliases from PNG (Acrobat ImageCapture) to DC and XMP.
			RegisterAlias(NS_PNG, "Author", NS_DC, "creator", aliasToArrayOrdered);
			RegisterAlias(NS_PNG, "Copyright", NS_DC, "rights", aliasToArrayAltText);
			RegisterAlias(NS_PNG, "CreationTime", NS_XMP, "CreateDate", null);
			RegisterAlias(NS_PNG, "Description", NS_DC, "description", aliasToArrayAltText);
			RegisterAlias(NS_PNG, "ModificationTime", NS_XMP, "ModifyDate", null);
			RegisterAlias(NS_PNG, "Software", NS_XMP, "CreatorTool", null);
			RegisterAlias(NS_PNG, "Title", NS_DC, "title", aliasToArrayAltText);
		}
	}

	public class ReadOnlyDictionary : IDictionary {
		#region ReadOnlyDictionary members

		private readonly IDictionary _originalDictionary;

		private ReadOnlyDictionary(IDictionary original) {
			_originalDictionary = original;
		}

		/// <summary>
		/// Return a read only wrapper to an existing dictionary.
		/// Any change to the underlying dictionary will be 
		/// propagated to the read-only wrapper.
		/// </summary>
		public static ReadOnlyDictionary ReadOnly(IDictionary dictionary) {
			return new ReadOnlyDictionary(dictionary);
		}

		private void ReportNotSupported() {
			throw new NotSupportedException("Collection is read-only.");
		}

		#endregion

		#region IDictionary Members

		virtual public bool IsReadOnly {
			get { return true; }
		}

		virtual public IDictionaryEnumerator GetEnumerator() {
			return _originalDictionary.GetEnumerator();
		}

		public object this[object key] {
			get { return _originalDictionary[key]; }
			set { throw new NotSupportedException("Collection is read-only."); }
		}

		virtual public void Remove(object key) {
			ReportNotSupported();
		}

		virtual public bool Contains(object key) {
			return _originalDictionary.Contains(key);
		}

		virtual public void Clear() {
			ReportNotSupported();
		}

		virtual public ICollection Values {
			get {
				// no need to wrap with a read-only thing,
				// as ICollection is always read-only
				return _originalDictionary.Values;
			}
		}

		virtual public void Add(object key, object value) {
			ReportNotSupported();
		}

		virtual public ICollection Keys {
			get {
				// no need to wrap with a read-only thing,
				// as ICollection is always read-only
				return _originalDictionary.Keys;
			}
		}

		virtual public bool IsFixedSize {
			get { return _originalDictionary.IsFixedSize; }
		}

		virtual public bool IsSynchronized {
			get { return _originalDictionary.IsSynchronized; }
		}

		virtual public int Count {
			get { return _originalDictionary.Count; }
		}

		virtual public void CopyTo(Array array, int index) {
			_originalDictionary.CopyTo(array, index);
		}

		virtual public object SyncRoot {
			get { return _originalDictionary.SyncRoot; }
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _originalDictionary.GetEnumerator();
		}

		#endregion
	}
}
