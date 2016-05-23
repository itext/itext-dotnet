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
using iTextSharp.Kernel.Xmp;
using iTextSharp.Kernel.Xmp.Impl.XPath;
using iTextSharp.Kernel.Xmp.Options;
using iTextSharp.Kernel.Xmp.Properties;

namespace iTextSharp.Kernel.Xmp.Impl
{
	/// <summary>
	/// Implementation for
	/// <see cref="iTextSharp.Kernel.Xmp.XmpMeta"/>
	/// .
	/// </summary>
	/// <since>17.02.2006</since>
	public class XmpMetaImpl : XmpConst, XmpMeta
	{
		/// <summary>Property values are Strings by default</summary>
		private const int VALUE_STRING = 0;

		private const int VALUE_BOOLEAN = 1;

		private const int VALUE_INTEGER = 2;

		private const int VALUE_LONG = 3;

		private const int VALUE_DOUBLE = 4;

		private const int VALUE_DATE = 5;

		private const int VALUE_CALENDAR = 6;

		private const int VALUE_BASE64 = 7;

		/// <summary>root of the metadata tree</summary>
		private XmpNode tree;

		/// <summary>the xpacket processing instructions content</summary>
		private String packetHeader = null;

		/// <summary>Constructor for an empty metadata object.</summary>
		public XmpMetaImpl()
		{
			// create root node
			tree = new XmpNode(null, null, null);
		}

		/// <summary>Constructor for a cloned metadata tree.</summary>
		/// <param name="tree">
		/// an prefilled metadata tree which fulfills all
		/// <code>XMPNode</code> contracts.
		/// </param>
		public XmpMetaImpl(XmpNode tree)
		{
			this.tree = tree;
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.AppendArrayItem(System.String, System.String, iTextSharp.Kernel.Xmp.Options.PropertyOptions, System.String, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void AppendArrayItem(String schemaNS, String arrayName, PropertyOptions
			 arrayOptions, String itemValue, PropertyOptions itemOptions)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertArrayName(arrayName);
			if (arrayOptions == null)
			{
				arrayOptions = new PropertyOptions();
			}
			if (!arrayOptions.IsOnlyArrayOptions())
			{
				throw new XmpException("Only array form flags allowed for arrayOptions", XmpError
					.BADOPTIONS);
			}
			// Check if array options are set correctly.
			arrayOptions = XmpNodeUtils.VerifySetOptions(arrayOptions, null);
			// Locate or create the array. If it already exists, make sure the array
			// form from the options
			// parameter is compatible with the current state.
			XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNS, arrayName);
			// Just lookup, don't try to create.
			XmpNode arrayNode = XmpNodeUtils.FindNode(tree, arrayPath, false, null);
			if (arrayNode != null)
			{
				// The array exists, make sure the form is compatible. Zero
				// arrayForm means take what exists.
				if (!arrayNode.GetOptions().IsArray())
				{
					throw new XmpException("The named property is not an array", XmpError.BADXPATH);
				}
			}
			else
			{
				// if (arrayOptions != null && !arrayOptions.equalArrayTypes(arrayNode.getOptions()))
				// {
				// throw new XMPException("Mismatch of existing and specified array form", BADOPTIONS);
				// }
				// The array does not exist, try to create it.
				if (arrayOptions.IsArray())
				{
					arrayNode = XmpNodeUtils.FindNode(tree, arrayPath, true, arrayOptions);
					if (arrayNode == null)
					{
						throw new XmpException("Failure creating array node", XmpError.BADXPATH);
					}
				}
				else
				{
					// array options missing
					throw new XmpException("Explicit arrayOptions required to create new array", XmpError
						.BADOPTIONS);
				}
			}
			DoSetArrayItem(arrayNode, ARRAY_LAST_ITEM, itemValue, itemOptions, true);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.AppendArrayItem(System.String, System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void AppendArrayItem(String schemaNS, String arrayName, String itemValue
			)
		{
			AppendArrayItem(schemaNS, arrayName, null, itemValue, null);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.CountArrayItems(System.String, System.String)
		/// 	"/>
		public virtual int CountArrayItems(String schemaNS, String arrayName)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertArrayName(arrayName);
			XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNS, arrayName);
			XmpNode arrayNode = XmpNodeUtils.FindNode(tree, arrayPath, false, null);
			if (arrayNode == null)
			{
				return 0;
			}
			if (arrayNode.GetOptions().IsArray())
			{
				return arrayNode.GetChildrenLength();
			}
			else
			{
				throw new XmpException("The named property is not an array", XmpError.BADXPATH);
			}
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.DeleteArrayItem(System.String, System.String, int)
		/// 	"/>
		public virtual void DeleteArrayItem(String schemaNS, String arrayName, int itemIndex
			)
		{
			try
			{
				ParameterAsserts.AssertSchemaNS(schemaNS);
				ParameterAsserts.AssertArrayName(arrayName);
				String itemPath = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
				DeleteProperty(schemaNS, itemPath);
			}
			catch (XmpException)
			{
			}
		}

		// EMPTY, exceptions are ignored within delete
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.DeleteProperty(System.String, System.String)
		/// 	"/>
		public virtual void DeleteProperty(String schemaNS, String propName)
		{
			try
			{
				ParameterAsserts.AssertSchemaNS(schemaNS);
				ParameterAsserts.AssertPropName(propName);
				XmpPath expPath = XmpPathParser.ExpandXPath(schemaNS, propName);
				XmpNode propNode = XmpNodeUtils.FindNode(tree, expPath, false, null);
				if (propNode != null)
				{
					XmpNodeUtils.DeleteNode(propNode);
				}
			}
			catch (XmpException)
			{
			}
		}

		// EMPTY, exceptions are ignored within delete
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.DeleteQualifier(System.String, System.String, System.String, System.String)
		/// 	"/>
		public virtual void DeleteQualifier(String schemaNS, String propName, String qualNS
			, String qualName)
		{
			try
			{
				// Note: qualNS and qualName are checked inside composeQualfierPath
				ParameterAsserts.AssertSchemaNS(schemaNS);
				ParameterAsserts.AssertPropName(propName);
				String qualPath = propName + XmpPathFactory.ComposeQualifierPath(qualNS, qualName
					);
				DeleteProperty(schemaNS, qualPath);
			}
			catch (XmpException)
			{
			}
		}

		// EMPTY, exceptions within delete are ignored
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.DeleteStructField(System.String, System.String, System.String, System.String)
		/// 	"/>
		public virtual void DeleteStructField(String schemaNS, String structName, String 
			fieldNS, String fieldName)
		{
			try
			{
				// fieldNS and fieldName are checked inside composeStructFieldPath
				ParameterAsserts.AssertSchemaNS(schemaNS);
				ParameterAsserts.AssertStructName(structName);
				String fieldPath = structName + XmpPathFactory.ComposeStructFieldPath(fieldNS, fieldName
					);
				DeleteProperty(schemaNS, fieldPath);
			}
			catch (XmpException)
			{
			}
		}

		// EMPTY, exceptions within delete are ignored
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.DoesPropertyExist(System.String, System.String)
		/// 	"/>
		public virtual bool DoesPropertyExist(String schemaNS, String propName)
		{
			try
			{
				ParameterAsserts.AssertSchemaNS(schemaNS);
				ParameterAsserts.AssertPropName(propName);
				XmpPath expPath = XmpPathParser.ExpandXPath(schemaNS, propName);
				XmpNode propNode = XmpNodeUtils.FindNode(tree, expPath, false, null);
				return propNode != null;
			}
			catch (XmpException)
			{
				return false;
			}
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.DoesArrayItemExist(System.String, System.String, int)
		/// 	"/>
		public virtual bool DoesArrayItemExist(String schemaNS, String arrayName, int itemIndex
			)
		{
			try
			{
				ParameterAsserts.AssertSchemaNS(schemaNS);
				ParameterAsserts.AssertArrayName(arrayName);
				String path = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
				return DoesPropertyExist(schemaNS, path);
			}
			catch (XmpException)
			{
				return false;
			}
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.DoesStructFieldExist(System.String, System.String, System.String, System.String)
		/// 	"/>
		public virtual bool DoesStructFieldExist(String schemaNS, String structName, String
			 fieldNS, String fieldName)
		{
			try
			{
				// fieldNS and fieldName are checked inside composeStructFieldPath()
				ParameterAsserts.AssertSchemaNS(schemaNS);
				ParameterAsserts.AssertStructName(structName);
				String path = XmpPathFactory.ComposeStructFieldPath(fieldNS, fieldName);
				return DoesPropertyExist(schemaNS, structName + path);
			}
			catch (XmpException)
			{
				return false;
			}
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.DoesQualifierExist(System.String, System.String, System.String, System.String)
		/// 	"/>
		public virtual bool DoesQualifierExist(String schemaNS, String propName, String qualNS
			, String qualName)
		{
			try
			{
				// qualNS and qualName are checked inside composeQualifierPath()
				ParameterAsserts.AssertSchemaNS(schemaNS);
				ParameterAsserts.AssertPropName(propName);
				String path = XmpPathFactory.ComposeQualifierPath(qualNS, qualName);
				return DoesPropertyExist(schemaNS, propName + path);
			}
			catch (XmpException)
			{
				return false;
			}
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetArrayItem(System.String, System.String, int)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual XmpProperty GetArrayItem(String schemaNS, String arrayName, int itemIndex
			)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertArrayName(arrayName);
			String itemPath = XmpPathFactory.ComposeArrayItemPath(arrayName, itemIndex);
			return GetProperty(schemaNS, itemPath);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetLocalizedText(System.String, System.String, System.String, System.String)
		/// 	"/>
		public virtual XmpProperty GetLocalizedText(String schemaNS, String altTextName, 
			String genericLang, String specificLang)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertArrayName(altTextName);
			ParameterAsserts.AssertSpecificLang(specificLang);
			genericLang = genericLang != null ? iTextSharp.Kernel.Xmp.Impl.Utils.NormalizeLangValue
				(genericLang) : null;
			specificLang = iTextSharp.Kernel.Xmp.Impl.Utils.NormalizeLangValue(specificLang);
			XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNS, altTextName);
			XmpNode arrayNode = XmpNodeUtils.FindNode(tree, arrayPath, false, null);
			if (arrayNode == null)
			{
				return null;
			}
			Object[] result = XmpNodeUtils.ChooseLocalizedText(arrayNode, genericLang, specificLang
				);
			int match = ((int?)result[0]);
			XmpNode itemNode = (XmpNode)result[1];
			if (match != XmpNodeUtils.CLT_NO_VALUES)
			{
				return new _XmpProperty_428(itemNode);
			}
			else
			{
				return null;
			}
		}

		private sealed class _XmpProperty_428 : XmpProperty
		{
			public _XmpProperty_428(XmpNode itemNode)
			{
				this.itemNode = itemNode;
			}

			public String GetValue()
			{
				return itemNode.GetValue();
			}

			public PropertyOptions GetOptions()
			{
				return itemNode.GetOptions();
			}

			public String GetLanguage()
			{
				return itemNode.GetQualifier(1).GetValue();
			}

			public override String ToString()
			{
				return itemNode.GetValue().ToString();
			}

			private readonly XmpNode itemNode;
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetLocalizedText(System.String, System.String, System.String, System.String, System.String, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetLocalizedText(String schemaNS, String altTextName, String 
			genericLang, String specificLang, String itemValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertArrayName(altTextName);
			ParameterAsserts.AssertSpecificLang(specificLang);
			genericLang = genericLang != null ? iTextSharp.Kernel.Xmp.Impl.Utils.NormalizeLangValue
				(genericLang) : null;
			specificLang = iTextSharp.Kernel.Xmp.Impl.Utils.NormalizeLangValue(specificLang);
			XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNS, altTextName);
			// Find the array node and set the options if it was just created.
			XmpNode arrayNode = XmpNodeUtils.FindNode(tree, arrayPath, true, new PropertyOptions
				(PropertyOptions.ARRAY | PropertyOptions.ARRAY_ORDERED | PropertyOptions.ARRAY_ALTERNATE
				 | PropertyOptions.ARRAY_ALT_TEXT));
			if (arrayNode == null)
			{
				throw new XmpException("Failed to find or create array node", XmpError.BADXPATH);
			}
			else
			{
				if (!arrayNode.GetOptions().IsArrayAltText())
				{
					if (!arrayNode.HasChildren() && arrayNode.GetOptions().IsArrayAlternate())
					{
						arrayNode.GetOptions().SetArrayAltText(true);
					}
					else
					{
						throw new XmpException("Specified property is no alt-text array", XmpError.BADXPATH
							);
					}
				}
			}
			// Make sure the x-default item, if any, is first.
			bool haveXDefault = false;
			XmpNode xdItem = null;
			for (IEnumerator it = arrayNode.IterateChildren(); it.MoveNext(); )
			{
				XmpNode currItem = (XmpNode)it.Current;
				if (!currItem.HasQualifier() || !XmpConst.XML_LANG.Equals(currItem.GetQualifier(1
					).GetName()))
				{
					throw new XmpException("Language qualifier must be first", XmpError.BADXPATH);
				}
				else
				{
					if (XmpConst.X_DEFAULT.Equals(currItem.GetQualifier(1).GetValue()))
					{
						xdItem = currItem;
						haveXDefault = true;
						break;
					}
				}
			}
			// Moves x-default to the beginning of the array
			if (xdItem != null && arrayNode.GetChildrenLength() > 1)
			{
				arrayNode.RemoveChild(xdItem);
				arrayNode.AddChild(1, xdItem);
			}
			// Find the appropriate item.
			// chooseLocalizedText will make sure the array is a language
			// alternative.
			Object[] result = XmpNodeUtils.ChooseLocalizedText(arrayNode, genericLang, specificLang
				);
			int match = ((int?)result[0]);
			XmpNode itemNode = (XmpNode)result[1];
			bool specificXDefault = XmpConst.X_DEFAULT.Equals(specificLang);
			switch (match)
			{
				case XmpNodeUtils.CLT_NO_VALUES:
				{
					// Create the array items for the specificLang and x-default, with
					// x-default first.
					XmpNodeUtils.AppendLangItem(arrayNode, XmpConst.X_DEFAULT, itemValue);
					haveXDefault = true;
					if (!specificXDefault)
					{
						XmpNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
					}
					break;
				}

				case XmpNodeUtils.CLT_SPECIFIC_MATCH:
				{
					if (!specificXDefault)
					{
						// Update the specific item, update x-default if it matches the
						// old value.
						if (haveXDefault && xdItem != itemNode && xdItem != null && xdItem.GetValue().Equals
							(itemNode.GetValue()))
						{
							xdItem.SetValue(itemValue);
						}
						// ! Do this after the x-default check!
						itemNode.SetValue(itemValue);
					}
					else
					{
						// Update all items whose values match the old x-default value.
						System.Diagnostics.Debug.Assert(haveXDefault && xdItem == itemNode);
						for (IEnumerator it_1 = arrayNode.IterateChildren(); it_1.MoveNext(); )
						{
							XmpNode currItem = (XmpNode)it_1.Current;
							if (currItem == xdItem || !currItem.GetValue().Equals(xdItem != null ? xdItem.GetValue
								() : null))
							{
								continue;
							}
							currItem.SetValue(itemValue);
						}
						// And finally do the x-default item.
						if (xdItem != null)
						{
							xdItem.SetValue(itemValue);
						}
					}
					break;
				}

				case XmpNodeUtils.CLT_SINGLE_GENERIC:
				{
					// Update the generic item, update x-default if it matches the old
					// value.
					if (haveXDefault && xdItem != itemNode && xdItem != null && xdItem.GetValue().Equals
						(itemNode.GetValue()))
					{
						xdItem.SetValue(itemValue);
					}
					itemNode.SetValue(itemValue);
					// ! Do this after
					// the x-default
					// check!
					break;
				}

				case XmpNodeUtils.CLT_MULTIPLE_GENERIC:
				{
					// Create the specific language, ignore x-default.
					XmpNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
					if (specificXDefault)
					{
						haveXDefault = true;
					}
					break;
				}

				case XmpNodeUtils.CLT_XDEFAULT:
				{
					// Create the specific language, update x-default if it was the only
					// item.
					if (xdItem != null && arrayNode.GetChildrenLength() == 1)
					{
						xdItem.SetValue(itemValue);
					}
					XmpNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
					break;
				}

				case XmpNodeUtils.CLT_FIRST_ITEM:
				{
					// Create the specific language, don't add an x-default item.
					XmpNodeUtils.AppendLangItem(arrayNode, specificLang, itemValue);
					if (specificXDefault)
					{
						haveXDefault = true;
					}
					break;
				}

				default:
				{
					// does not happen under normal circumstances
					throw new XmpException("Unexpected result from ChooseLocalizedText", XmpError.INTERNALFAILURE
						);
				}
			}
			// Add an x-default at the front if needed.
			if (!haveXDefault && arrayNode.GetChildrenLength() == 1)
			{
				XmpNodeUtils.AppendLangItem(arrayNode, XmpConst.X_DEFAULT, itemValue);
			}
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetLocalizedText(System.String, System.String, System.String, System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetLocalizedText(String schemaNS, String altTextName, String 
			genericLang, String specificLang, String itemValue)
		{
			SetLocalizedText(schemaNS, altTextName, genericLang, specificLang, itemValue, null
				);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetProperty(System.String, System.String)
		/// 	"/>
		public virtual XmpProperty GetProperty(String schemaNS, String propName)
		{
			return GetProperty(schemaNS, propName, VALUE_STRING);
		}

		/// <summary>Returns a property, but the result value can be requested.</summary>
		/// <remarks>
		/// Returns a property, but the result value can be requested. It can be one
		/// of
		/// <see cref="VALUE_STRING"/>
		/// ,
		/// <see cref="VALUE_BOOLEAN"/>
		/// ,
		/// <see cref="VALUE_INTEGER"/>
		/// ,
		/// <see cref="VALUE_LONG"/>
		/// ,
		/// <see cref="VALUE_DOUBLE"/>
		/// ,
		/// <see cref="VALUE_DATE"/>
		/// ,
		/// <see cref="VALUE_CALENDAR"/>
		/// ,
		/// <see cref="VALUE_BASE64"/>
		/// .
		/// </remarks>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetProperty(System.String, System.String)
		/// 	"/>
		/// <param name="schemaNS">a schema namespace</param>
		/// <param name="propName">a property name or path</param>
		/// <param name="valueType">the type of the value, see VALUE_...</param>
		/// <returns>Returns an <code>XMPProperty</code></returns>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException">Collects any exception that occurs.
		/// 	</exception>
		protected internal virtual XmpProperty GetProperty(String schemaNS, String propName
			, int valueType)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertPropName(propName);
			XmpPath expPath = XmpPathParser.ExpandXPath(schemaNS, propName);
			XmpNode propNode = XmpNodeUtils.FindNode(tree, expPath, false, null);
			if (propNode != null)
			{
				if (valueType != VALUE_STRING && propNode.GetOptions().IsCompositeProperty())
				{
					throw new XmpException("Property must be simple when a value type is requested", 
						XmpError.BADXPATH);
				}
				Object value = EvaluateNodeValue(valueType, propNode);
				return new _XmpProperty_703(value, propNode);
			}
			else
			{
				return null;
			}
		}

		private sealed class _XmpProperty_703 : XmpProperty
		{
			public _XmpProperty_703(Object value, XmpNode propNode)
			{
				this.value = value;
				this.propNode = propNode;
			}

			public String GetValue()
			{
				return value != null ? value.ToString() : null;
			}

			public PropertyOptions GetOptions()
			{
				return propNode.GetOptions();
			}

			public String GetLanguage()
			{
				return null;
			}

			public override String ToString()
			{
				return value.ToString();
			}

			private readonly Object value;

			private readonly XmpNode propNode;
		}

		/// <summary>Returns a property, but the result value can be requested.</summary>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetProperty(System.String, System.String)
		/// 	"/>
		/// <param name="schemaNS">a schema namespace</param>
		/// <param name="propName">a property name or path</param>
		/// <param name="valueType">the type of the value, see VALUE_...</param>
		/// <returns>
		/// Returns the node value as an object according to the
		/// <code>valueType</code>.
		/// </returns>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException">Collects any exception that occurs.
		/// 	</exception>
		protected internal virtual Object GetPropertyObject(String schemaNS, String propName
			, int valueType)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertPropName(propName);
			XmpPath expPath = XmpPathParser.ExpandXPath(schemaNS, propName);
			XmpNode propNode = XmpNodeUtils.FindNode(tree, expPath, false, null);
			if (propNode != null)
			{
				if (valueType != VALUE_STRING && propNode.GetOptions().IsCompositeProperty())
				{
					throw new XmpException("Property must be simple when a value type is requested", 
						XmpError.BADXPATH);
				}
				return EvaluateNodeValue(valueType, propNode);
			}
			else
			{
				return null;
			}
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetPropertyBoolean(System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual bool? GetPropertyBoolean(String schemaNS, String propName)
		{
			return (bool?)GetPropertyObject(schemaNS, propName, VALUE_BOOLEAN);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyBoolean(System.String, System.String, bool, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		public virtual void SetPropertyBoolean(String schemaNS, String propName, bool propValue
			, PropertyOptions options)
		{
			SetProperty(schemaNS, propName, propValue ? TRUESTR : FALSESTR, options);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyBoolean(System.String, System.String, bool)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyBoolean(String schemaNS, String propName, bool propValue
			)
		{
			SetProperty(schemaNS, propName, propValue ? TRUESTR : FALSESTR, null);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetPropertyInteger(System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual int? GetPropertyInteger(String schemaNS, String propName)
		{
			return (int?)GetPropertyObject(schemaNS, propName, VALUE_INTEGER);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyInteger(System.String, System.String, int, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyInteger(String schemaNS, String propName, int propValue
			, PropertyOptions options)
		{
			SetProperty(schemaNS, propName, propValue, options);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyInteger(System.String, System.String, int)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyInteger(String schemaNS, String propName, int propValue
			)
		{
			SetProperty(schemaNS, propName, propValue, null);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetPropertyLong(System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual long GetPropertyLong(String schemaNS, String propName)
		{
			return (long)GetPropertyObject(schemaNS, propName, VALUE_LONG);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyLong(System.String, System.String, long, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyLong(String schemaNS, String propName, long propValue
			, PropertyOptions options)
		{
			SetProperty(schemaNS, propName, propValue, options);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyLong(System.String, System.String, long)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyLong(String schemaNS, String propName, long propValue
			)
		{
			SetProperty(schemaNS, propName, propValue, null);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetPropertyDouble(System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual double? GetPropertyDouble(String schemaNS, String propName)
		{
			return (double?)GetPropertyObject(schemaNS, propName, VALUE_DOUBLE);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyDouble(System.String, System.String, double, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyDouble(String schemaNS, String propName, double propValue
			, PropertyOptions options)
		{
			SetProperty(schemaNS, propName, propValue, options);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyDouble(System.String, System.String, double)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyDouble(String schemaNS, String propName, double propValue
			)
		{
			SetProperty(schemaNS, propName, propValue, null);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetPropertyDate(System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual XmpDateTime GetPropertyDate(String schemaNS, String propName)
		{
			return (XmpDateTime)GetPropertyObject(schemaNS, propName, VALUE_DATE);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyDate(System.String, System.String, iTextSharp.Kernel.Xmp.XmpDateTime, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyDate(String schemaNS, String propName, XmpDateTime
			 propValue, PropertyOptions options)
		{
			SetProperty(schemaNS, propName, propValue, options);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyDate(System.String, System.String, iTextSharp.Kernel.Xmp.XmpDateTime)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyDate(String schemaNS, String propName, XmpDateTime
			 propValue)
		{
			SetProperty(schemaNS, propName, propValue, null);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetPropertyCalendar(System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual DateTime GetPropertyCalendar(String schemaNS, String propName)
		{
			return (DateTime)GetPropertyObject(schemaNS, propName, VALUE_CALENDAR);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyCalendar(System.String, System.String, System.DateTime, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyCalendar(String schemaNS, String propName, DateTime
			 propValue, PropertyOptions options)
		{
			SetProperty(schemaNS, propName, propValue, options);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyCalendar(System.String, System.String, System.DateTime)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyCalendar(String schemaNS, String propName, DateTime
			 propValue)
		{
			SetProperty(schemaNS, propName, propValue, null);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetPropertyBase64(System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual byte[] GetPropertyBase64(String schemaNS, String propName)
		{
			return (byte[])GetPropertyObject(schemaNS, propName, VALUE_BASE64);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetPropertyString(System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual String GetPropertyString(String schemaNS, String propName)
		{
			return (String)GetPropertyObject(schemaNS, propName, VALUE_STRING);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyBase64(System.String, System.String, byte[], iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyBase64(String schemaNS, String propName, byte[] propValue
			, PropertyOptions options)
		{
			SetProperty(schemaNS, propName, propValue, options);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetPropertyBase64(System.String, System.String, byte[])
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetPropertyBase64(String schemaNS, String propName, byte[] propValue
			)
		{
			SetProperty(schemaNS, propName, propValue, null);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetQualifier(System.String, System.String, System.String, System.String)
		/// 	"/>
		public virtual XmpProperty GetQualifier(String schemaNS, String propName, String 
			qualNS, String qualName)
		{
			// qualNS and qualName are checked inside composeQualfierPath
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertPropName(propName);
			String qualPath = propName + XmpPathFactory.ComposeQualifierPath(qualNS, qualName
				);
			return GetProperty(schemaNS, qualPath);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetStructField(System.String, System.String, System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual XmpProperty GetStructField(String schemaNS, String structName, String
			 fieldNS, String fieldName)
		{
			// fieldNS and fieldName are checked inside composeStructFieldPath
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertStructName(structName);
			String fieldPath = structName + XmpPathFactory.ComposeStructFieldPath(fieldNS, fieldName
				);
			return GetProperty(schemaNS, fieldPath);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.Iterator()"/>
		public virtual XmpIterator Iterator()
		{
			return Iterator(null, null, null);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.Iterator(iTextSharp.Kernel.Xmp.Options.IteratorOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual XmpIterator Iterator(IteratorOptions options)
		{
			return Iterator(null, null, options);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.Iterator(System.String, System.String, iTextSharp.Kernel.Xmp.Options.IteratorOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual XmpIterator Iterator(String schemaNS, String propName, IteratorOptions
			 options)
		{
			return new XmpIteratorImpl(this, schemaNS, propName, options);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetArrayItem(System.String, System.String, int, System.String, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		public virtual void SetArrayItem(String schemaNS, String arrayName, int itemIndex
			, String itemValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertArrayName(arrayName);
			// Just lookup, don't try to create.
			XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNS, arrayName);
			XmpNode arrayNode = XmpNodeUtils.FindNode(tree, arrayPath, false, null);
			if (arrayNode != null)
			{
				DoSetArrayItem(arrayNode, itemIndex, itemValue, options, false);
			}
			else
			{
				throw new XmpException("Specified array does not exist", XmpError.BADXPATH);
			}
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetArrayItem(System.String, System.String, int, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetArrayItem(String schemaNS, String arrayName, int itemIndex
			, String itemValue)
		{
			SetArrayItem(schemaNS, arrayName, itemIndex, itemValue, null);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.InsertArrayItem(System.String, System.String, int, System.String, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		public virtual void InsertArrayItem(String schemaNS, String arrayName, int itemIndex
			, String itemValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertArrayName(arrayName);
			// Just lookup, don't try to create.
			XmpPath arrayPath = XmpPathParser.ExpandXPath(schemaNS, arrayName);
			XmpNode arrayNode = XmpNodeUtils.FindNode(tree, arrayPath, false, null);
			if (arrayNode != null)
			{
				DoSetArrayItem(arrayNode, itemIndex, itemValue, options, true);
			}
			else
			{
				throw new XmpException("Specified array does not exist", XmpError.BADXPATH);
			}
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.InsertArrayItem(System.String, System.String, int, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void InsertArrayItem(String schemaNS, String arrayName, int itemIndex
			, String itemValue)
		{
			InsertArrayItem(schemaNS, arrayName, itemIndex, itemValue, null);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetProperty(System.String, System.String, System.Object, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		public virtual void SetProperty(String schemaNS, String propName, Object propValue
			, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertPropName(propName);
			options = XmpNodeUtils.VerifySetOptions(options, propValue);
			XmpPath expPath = XmpPathParser.ExpandXPath(schemaNS, propName);
			XmpNode propNode = XmpNodeUtils.FindNode(tree, expPath, true, options);
			if (propNode != null)
			{
				SetNode(propNode, propValue, options, false);
			}
			else
			{
				throw new XmpException("Specified property does not exist", XmpError.BADXPATH);
			}
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetProperty(System.String, System.String, System.Object)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetProperty(String schemaNS, String propName, Object propValue
			)
		{
			SetProperty(schemaNS, propName, propValue, null);
		}

		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetQualifier(System.String, System.String, System.String, System.String, System.String, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		public virtual void SetQualifier(String schemaNS, String propName, String qualNS, 
			String qualName, String qualValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertPropName(propName);
			if (!DoesPropertyExist(schemaNS, propName))
			{
				throw new XmpException("Specified property does not exist!", XmpError.BADXPATH);
			}
			String qualPath = propName + XmpPathFactory.ComposeQualifierPath(qualNS, qualName
				);
			SetProperty(schemaNS, qualPath, qualValue, options);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetQualifier(System.String, System.String, System.String, System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetQualifier(String schemaNS, String propName, String qualNS, 
			String qualName, String qualValue)
		{
			SetQualifier(schemaNS, propName, qualNS, qualName, qualValue, null);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetStructField(System.String, System.String, System.String, System.String, System.String, iTextSharp.Kernel.Xmp.Options.PropertyOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetStructField(String schemaNS, String structName, String fieldNS
			, String fieldName, String fieldValue, PropertyOptions options)
		{
			ParameterAsserts.AssertSchemaNS(schemaNS);
			ParameterAsserts.AssertStructName(structName);
			String fieldPath = structName + XmpPathFactory.ComposeStructFieldPath(fieldNS, fieldName
				);
			SetProperty(schemaNS, fieldPath, fieldValue, options);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetStructField(System.String, System.String, System.String, System.String, System.String)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void SetStructField(String schemaNS, String structName, String fieldNS
			, String fieldName, String fieldValue)
		{
			SetStructField(schemaNS, structName, fieldNS, fieldName, fieldValue, null);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetObjectName()"/>
		public virtual String GetObjectName()
		{
			return tree.GetName() != null ? tree.GetName() : "";
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.SetObjectName(System.String)"/>
		public virtual void SetObjectName(String name)
		{
			tree.SetName(name);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.GetPacketHeader()"/>
		public virtual String GetPacketHeader()
		{
			return packetHeader;
		}

		/// <summary>Sets the packetHeader attributes, only used by the parser.</summary>
		/// <param name="packetHeader">the processing instruction content</param>
		public virtual void SetPacketHeader(String packetHeader)
		{
			this.packetHeader = packetHeader;
		}

		/// <summary>Performs a deep clone of the XMPMeta-object</summary>
		/// <seealso cref="System.Object.Clone()"/>
		public virtual Object Clone()
		{
			XmpNode clonedTree = (XmpNode)tree.Clone();
			return new XmpMetaImpl(clonedTree);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.DumpObject()"/>
		public virtual String DumpObject()
		{
			// renders tree recursively
			return GetRoot().DumpNode(true);
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.Sort()"/>
		public virtual void Sort()
		{
			this.tree.Sort();
		}

		/// <seealso cref="iTextSharp.Kernel.Xmp.XmpMeta.Normalize(iTextSharp.Kernel.Xmp.Options.ParseOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		public virtual void Normalize(ParseOptions options)
		{
			if (options == null)
			{
				options = new ParseOptions();
			}
			XmpNormalizer.Process(this, options);
		}

		/// <returns>Returns the root node of the XMP tree.</returns>
		public virtual XmpNode GetRoot()
		{
			return tree;
		}

		// -------------------------------------------------------------------------------------
		// private
		/// <summary>Locate or create the item node and set the value.</summary>
		/// <remarks>
		/// Locate or create the item node and set the value. Note the index
		/// parameter is one-based! The index can be in the range [1..size + 1] or
		/// "last()", normalize it and check the insert flags. The order of the
		/// normalization checks is important. If the array is empty we end up with
		/// an index and location to set item size + 1.
		/// </remarks>
		/// <param name="arrayNode">an array node</param>
		/// <param name="itemIndex">the index where to insert the item</param>
		/// <param name="itemValue">the item value</param>
		/// <param name="itemOptions">the options for the new item</param>
		/// <param name="insert">insert oder overwrite at index position?</param>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		private void DoSetArrayItem(XmpNode arrayNode, int itemIndex, String itemValue, PropertyOptions
			 itemOptions, bool insert)
		{
			XmpNode itemNode = new XmpNode(ARRAY_ITEM_NAME, null);
			itemOptions = XmpNodeUtils.VerifySetOptions(itemOptions, itemValue);
			// in insert mode the index after the last is allowed,
			// even ARRAY_LAST_ITEM points to the index *after* the last.
			int maxIndex = insert ? arrayNode.GetChildrenLength() + 1 : arrayNode.GetChildrenLength
				();
			if (itemIndex == ARRAY_LAST_ITEM)
			{
				itemIndex = maxIndex;
			}
			if (1 <= itemIndex && itemIndex <= maxIndex)
			{
				if (!insert)
				{
					arrayNode.RemoveChild(itemIndex);
				}
				arrayNode.AddChild(itemIndex, itemNode);
				SetNode(itemNode, itemValue, itemOptions, false);
			}
			else
			{
				throw new XmpException("Array index out of bounds", XmpError.BADINDEX);
			}
		}

		/// <summary>
		/// The internals for setProperty() and related calls, used after the node is
		/// found or created.
		/// </summary>
		/// <param name="node">the newly created node</param>
		/// <param name="value">the node value, can be <code>null</code></param>
		/// <param name="newOptions">options for the new node, must not be <code>null</code>.
		/// 	</param>
		/// <param name="deleteExisting">flag if the existing value is to be overwritten</param>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException">thrown if options and value do not correspond
		/// 	</exception>
		internal virtual void SetNode(XmpNode node, Object value, PropertyOptions newOptions
			, bool deleteExisting)
		{
			if (deleteExisting)
			{
				node.Clear();
			}
			// its checked by setOptions(), if the merged result is a valid options set
			node.GetOptions().MergeWith(newOptions);
			if (!node.GetOptions().IsCompositeProperty())
			{
				// This is setting the value of a leaf node.
				XmpNodeUtils.SetNodeValue(node, value);
			}
			else
			{
				if (value != null && value.ToString().Length > 0)
				{
					throw new XmpException("Composite nodes can't have values", XmpError.BADXPATH);
				}
				node.RemoveChildren();
			}
		}

		/// <summary>
		/// Evaluates a raw node value to the given value type, apply special
		/// conversions for defined types in XMP.
		/// </summary>
		/// <param name="valueType">an int indicating the value type</param>
		/// <param name="propNode">the node containing the value</param>
		/// <returns>Returns a literal value for the node.</returns>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		private Object EvaluateNodeValue(int valueType, XmpNode propNode)
		{
			Object value;
			String rawValue = propNode.GetValue();
			switch (valueType)
			{
				case VALUE_BOOLEAN:
				{
					value = XmpUtils.ConvertToBoolean(rawValue);
					break;
				}

				case VALUE_INTEGER:
				{
					value = XmpUtils.ConvertToInteger(rawValue);
					break;
				}

				case VALUE_LONG:
				{
					value = XmpUtils.ConvertToLong(rawValue);
					break;
				}

				case VALUE_DOUBLE:
				{
					value = XmpUtils.ConvertToDouble(rawValue);
					break;
				}

				case VALUE_DATE:
				{
					value = XmpUtils.ConvertToDate(rawValue);
					break;
				}

				case VALUE_CALENDAR:
				{
					XmpDateTime dt = XmpUtils.ConvertToDate(rawValue);
					value = dt.GetCalendar();
					break;
				}

				case VALUE_BASE64:
				{
					value = XmpUtils.DecodeBase64(rawValue);
					break;
				}

				case VALUE_STRING:
				default:
				{
					// leaf values return empty string instead of null
					// for the other cases the converter methods provides a "null"
					// value.
					// a default value can only occur if this method is made public.
					value = rawValue != null || propNode.GetOptions().IsCompositeProperty() ? rawValue
						 : "";
					break;
				}
			}
			return value;
		}
	}
}
