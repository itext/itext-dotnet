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
using System.Globalization;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Impl.XPath;
using iText.Kernel.XMP.Options;
using iText.Kernel.XMP.Properties;

namespace iText.Kernel.XMP.Impl
{
	/// <since>Aug 18, 2006</since>
	public class XMPNormalizer
	{
		/// <summary>caches the correct dc-property array forms</summary>
		private static IDictionary dcArrayForms;

		static XMPNormalizer()
		{
			InitDCArrays();
		}

		/// <summary>Hidden constructor</summary>
		private XMPNormalizer()
		{
		}

		//\cond DO_NOT_DOCUMENT	
		// EMPTY
		/// <summary>Normalizes a raw parsed XMPMeta-Object</summary>
		/// <param name="xmp">the raw metadata object</param>
		/// <param name="options">the parsing options</param>
		/// <returns>Returns the normalized metadata object</returns>
		internal static XMPMeta Process(XMPMetaImpl xmp, ParseOptions options)
		{
			XMPNode tree = xmp.GetRoot();
			TouchUpDataModel(xmp);
			MoveExplicitAliases(tree, options);
			TweakOldXMP(tree);
			DeleteEmptySchemas(tree);
			return xmp;
		}
		//\endcond
		
		/// <summary>
		/// Tweak old XMP: Move an instance ID from rdf:about to the
		/// <em>xmpMM:InstanceID</em> property.
		/// </summary>
		/// <remarks>
		/// Tweak old XMP: Move an instance ID from rdf:about to the
		/// <em>xmpMM:InstanceID</em> property. An old instance ID usually looks
		/// like &quot;uuid:bac965c4-9d87-11d9-9a30-000d936b79c4&quot;, plus InDesign
		/// 3.0 wrote them like &quot;bac965c4-9d87-11d9-9a30-000d936b79c4&quot;. If
		/// the name looks like a UUID simply move it to <em>xmpMM:InstanceID</em>,
		/// don't worry about any existing <em>xmpMM:InstanceID</em>. Both will
		/// only be present when a newer file with the <em>xmpMM:InstanceID</em>
		/// property is updated by an old app that uses <em>rdf:about</em>.
		/// </remarks>
		/// <param name="tree">the root of the metadata tree</param>
		private static void TweakOldXMP(XMPNode tree)
		{
			if (tree.GetName() != null && tree.GetName().Length >= Utils.UUID_LENGTH)
			{
				String nameStr = tree.GetName().ToLower();
				if (nameStr.StartsWith("uuid:"))
				{
					nameStr = nameStr.Substring(5);
				}
				if (Utils.CheckUUIDFormat(nameStr))
				{
					// move UUID to xmpMM:InstanceID and remove it from the root node
					XMPPath path = XMPPathParser.ExpandXPath(XMPConst.NS_XMP_MM, "InstanceID");
					XMPNode idNode = XMPNodeUtils.FindNode(tree, path, true, null);
					if (idNode != null)
					{
						idNode.SetOptions(null);
						// Clobber any existing xmpMM:InstanceID.
						idNode.SetValue("uuid:" + nameStr);
						idNode.RemoveChildren();
						idNode.RemoveQualifiers();
						tree.SetName(null);
					}
					else
					{
						throw new XMPException("Failure creating xmpMM:InstanceID", XMPError.INTERNALFAILURE
							);
					}
				}
			}
		}

		/// <summary>Visit all schemas to do general fixes and handle special cases.</summary>
		/// <param name="xmp">the metadata object implementation</param>
		private static void TouchUpDataModel(XMPMetaImpl xmp)
		{
			// make sure the DC schema is existing, because it might be needed within the normalization
			// if not touched it will be removed by removeEmptySchemas
			XMPNodeUtils.FindSchemaNode(xmp.GetRoot(), XMPConst.NS_DC, true);
			// Do the special case fixes within each schema.
			for (IEnumerator it = xmp.GetRoot().IterateChildren(); it.MoveNext(); )
			{
				XMPNode currSchema = (XMPNode)it.Current;
				if (XMPConst.NS_DC.Equals(currSchema.GetName()))
				{
					NormalizeDCArrays(currSchema);
				}
				else
				{
					if (XMPConst.NS_EXIF.Equals(currSchema.GetName()))
					{
						// Do a special case fix for exif:GPSTimeStamp.
						FixGPSTimeStamp(currSchema);
						XMPNode arrayNode = XMPNodeUtils.FindChildNode(currSchema, "exif:UserComment", false
							);
						if (arrayNode != null)
						{
							RepairAltText(arrayNode);
						}
					}
					else
					{
						if (XMPConst.NS_DM.Equals(currSchema.GetName()))
						{
							// Do a special case migration of xmpDM:copyright to
							// dc:rights['x-default'].
							XMPNode dmCopyright = XMPNodeUtils.FindChildNode(currSchema, "xmpDM:copyright", false
								);
							if (dmCopyright != null)
							{
								MigrateAudioCopyright(xmp, dmCopyright);
							}
						}
						else
						{
							if (XMPConst.NS_XMP_RIGHTS.Equals(currSchema.GetName()))
							{
								XMPNode arrayNode = XMPNodeUtils.FindChildNode(currSchema, "xmpRights:UsageTerms"
									, false);
								if (arrayNode != null)
								{
									RepairAltText(arrayNode);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Undo the denormalization performed by the XMP used in Acrobat 5.<br />
		/// If a Dublin Core array had only one item, it was serialized as a simple
		/// property.
		/// </summary>
		/// <remarks>
		/// Undo the denormalization performed by the XMP used in Acrobat 5.<br />
		/// If a Dublin Core array had only one item, it was serialized as a simple
		/// property. <br />
		/// The <code>xml:lang</code> attribute was dropped from an
		/// <code>alt-text</code> item if the language was <code>x-default</code>.
		/// </remarks>
		/// <param name="dcSchema">the DC schema node</param>
		private static void NormalizeDCArrays(XMPNode dcSchema)
		{
			for (int i = 1; i <= dcSchema.GetChildrenLength(); i++)
			{
				XMPNode currProp = dcSchema.GetChild(i);
				PropertyOptions arrayForm = (PropertyOptions)dcArrayForms[currProp.GetName()];
				if (arrayForm == null)
				{
					continue;
				}
				else
				{
					if (currProp.GetOptions().IsSimple())
					{
						// create a new array and add the current property as child, 
						// if it was formerly simple 
						XMPNode newArray = new XMPNode(currProp.GetName(), arrayForm);
						currProp.SetName(XMPConst.ARRAY_ITEM_NAME);
						newArray.AddChild(currProp);
						dcSchema.ReplaceChild(i, newArray);
						// fix language alternatives
						if (arrayForm.IsArrayAltText() && !currProp.GetOptions().GetHasLanguage())
						{
							XMPNode newLang = new XMPNode(XMPConst.XML_LANG, XMPConst.X_DEFAULT, null);
							currProp.AddQualifier(newLang);
						}
					}
					else
					{
						// clear array options and add corrected array form if it has been an array before
						currProp.GetOptions().SetOption(PropertyOptions.ARRAY | PropertyOptions.ARRAY_ORDERED
							 | PropertyOptions.ARRAY_ALTERNATE | PropertyOptions.ARRAY_ALT_TEXT, false);
						currProp.GetOptions().MergeWith(arrayForm);
						if (arrayForm.IsArrayAltText())
						{
							// applying for "dc:description", "dc:rights", "dc:title"
							RepairAltText(currProp);
						}
					}
				}
			}
		}

		/// <summary>Make sure that the array is well-formed AltText.</summary>
		/// <remarks>
		/// Make sure that the array is well-formed AltText. Each item must be simple
		/// and have an "xml:lang" qualifier. If repairs are needed, keep simple
		/// non-empty items by adding the "xml:lang" with value "x-repair".
		/// </remarks>
		/// <param name="arrayNode">the property node of the array to repair.</param>
		private static void RepairAltText(XMPNode arrayNode)
		{
			if (arrayNode == null || !arrayNode.GetOptions().IsArray()) {
				// Already OK or not even an array.
				return;
			}

			// fix options
			arrayNode.GetOptions().SetArrayOrdered(true);
			arrayNode.GetOptions().SetArrayAlternate(true);
			arrayNode.GetOptions().SetArrayAltText(true);
			ArrayList currChildsToRemove = new ArrayList();
			IEnumerator it = arrayNode.IterateChildren();
			while (it.MoveNext()) {
				XMPNode currChild = (XMPNode) it.Current;
				if (currChild == null)
					continue;
				if (currChild.GetOptions().IsCompositeProperty()) {
					// Delete non-simple children.
					currChildsToRemove.Add(currChild);
				}
				else if (!currChild.GetOptions().GetHasLanguage()) {
					string childValue = currChild.GetValue();
					if (String.IsNullOrEmpty(childValue)) {
						// Delete empty valued children that have no xml:lang.
						currChildsToRemove.Add(currChild);
					}
					else {
						// Add an xml:lang qualifier with the value "x-repair".
						XMPNode repairLang = new XMPNode(XMPConst.XML_LANG, "x-repair", null);
						currChild.AddQualifier(repairLang);
					}
				}
			}
			foreach (object o in currChildsToRemove) {
				arrayNode.GetChildren().Remove(o);
			}
		}

		/// <summary>Visit all of the top level nodes looking for aliases.</summary>
		/// <remarks>
		/// Visit all of the top level nodes looking for aliases. If there is
		/// no base, transplant the alias subtree. If there is a base and strict
		/// aliasing is on, make sure the alias and base subtrees match.
		/// </remarks>
		/// <param name="tree">the root of the metadata tree</param>
		/// <param name="options">th parsing options</param>
		private static void MoveExplicitAliases(XMPNode tree, ParseOptions options)
		{
			if (!tree.GetHasAliases()) {
				return;
			}
			tree.SetHasAliases(false);

			bool strictAliasing = options.GetStrictAliasing();
			IEnumerator schemaIt = tree.GetUnmodifiableChildren().GetEnumerator();
			while (schemaIt.MoveNext()) {
				XMPNode currSchema = (XMPNode) schemaIt.Current;
				if (currSchema == null)
					continue;
				if (!currSchema.GetHasAliases()) {
					continue;
				}

				List<XMPNode> currPropsToRemove = new List<XMPNode>();
				IEnumerator propertyIt = currSchema.IterateChildren();
				while (propertyIt.MoveNext()) {
					XMPNode currProp = (XMPNode) propertyIt.Current;
					if (currProp == null)
						continue;

					if (!currProp.IsAlias()) {
						continue;
					}

					currProp.SetAlias(false);

					// Find the base path, look for the base schema and root node.
					XMPAliasInfo info = XMPMetaFactory.GetSchemaRegistry().FindAlias(currProp.GetName());
					if (info != null) {
						// find or create schema
						XMPNode baseSchema = XMPNodeUtils.FindSchemaNode(tree, info.GetNamespace(), null, true);
						baseSchema.SetImplicit(false);

						XMPNode baseNode = XMPNodeUtils.FindChildNode(baseSchema, info.GetPrefix() + info.GetPropName(), false);
						if (baseNode == null) {
							if (info.GetAliasForm().IsSimple()) {
								// A top-to-top alias, transplant the property.
								// change the alias property name to the base name
								string qname = info.GetPrefix() + info.GetPropName();
								currProp.SetName(qname);
								baseSchema.AddChild(currProp);
							}
							else {
								// An alias to an array item, 
								// create the array and transplant the property.
								baseNode = new XMPNode(info.GetPrefix() + info.GetPropName(),
									info.GetAliasForm().ToPropertyOptions());
								baseSchema.AddChild(baseNode);
								TransplantArrayItemAlias(currProp, baseNode);
							}
							currPropsToRemove.Add(currProp);
						}
						else if (info.GetAliasForm().IsSimple()) {
							// The base node does exist and this is a top-to-top alias.
							// Check for conflicts if strict aliasing is on. 
							// Remove and delete the alias subtree.
							if (strictAliasing) {
								CompareAliasedSubtrees(currProp, baseNode, true);
							}
							currPropsToRemove.Add(currProp);
						}
						else {
							// This is an alias to an array item and the array exists.
							// Look for the aliased item.
							// Then transplant or check & delete as appropriate.

							XMPNode itemNode = null;
							if (info.GetAliasForm().IsArrayAltText()) {
								int xdIndex = XMPNodeUtils.LookupLanguageItem(baseNode, XMPConst.X_DEFAULT);
								if (xdIndex != -1) {
									itemNode = baseNode.GetChild(xdIndex);
								}
							}
							else if (baseNode.HasChildren()) {
								itemNode = baseNode.GetChild(1);
							}

							if (itemNode == null) {
								TransplantArrayItemAlias(currProp, baseNode);
							}
							else {
								if (strictAliasing) {
									CompareAliasedSubtrees(currProp, itemNode, true);
								}
							}
							currPropsToRemove.Add(currProp);
						}
					}
				}
				foreach (XMPNode o in currPropsToRemove)
					currSchema.RemoveChild(o);
				currPropsToRemove.Clear();
				currSchema.SetHasAliases(false);
			}
		}

		/// <summary>
		/// Moves an alias node of array form to another schema into an array </summary>
		/// <param name="childNode"> the node to be moved </param>
		/// <param name="baseArray"> the base array for the array item </param>
		private static void TransplantArrayItemAlias(XMPNode childNode, XMPNode baseArray) {
			if (baseArray.GetOptions().IsArrayAltText()) {
				if (childNode.GetOptions().GetHasLanguage()) {
					throw new XMPException("Alias to x-default already has a language qualifier",
						XMPError.BADXMP);
				}

				XMPNode langQual = new XMPNode(XMPConst.XML_LANG, XMPConst.X_DEFAULT, null);
				childNode.AddQualifier(langQual);
			}

			childNode.SetName(XMPConst.ARRAY_ITEM_NAME);
			baseArray.AddChild(childNode);
		}


		/// <summary>Fixes the GPS Timestamp in EXIF.</summary>
		/// <param name="exifSchema">the EXIF schema node</param>
		private static void FixGPSTimeStamp(XMPNode exifSchema)
		{
			// Note: if dates are not found the convert-methods throws an exceptions,
			// 		 and this methods returns.
			XMPNode gpsDateTime = XMPNodeUtils.FindChildNode(exifSchema, "exif:GPSTimeStamp", false);
			if (gpsDateTime == null) {
				return;
			}

			try {
				XMPDateTime binGpsStamp = XMPUtils.ConvertToDate(gpsDateTime.GetValue());
				if (binGpsStamp.GetYear() != 0 || binGpsStamp.GetMonth() != 0 || binGpsStamp.GetDay() != 0) {
					return;
				}

				XMPNode otherDate = XMPNodeUtils.FindChildNode(exifSchema, "exif:DateTimeOriginal", false);
				otherDate = otherDate ?? XMPNodeUtils.FindChildNode(exifSchema, "exif:DateTimeDigitized", false);

				XMPDateTime binOtherDate = XMPUtils.ConvertToDate(otherDate.GetValue());
				XMPCalendar cal = binGpsStamp.GetCalendar();
				DateTime dt = new DateTime(binOtherDate.GetYear(), binOtherDate.GetMonth(), binOtherDate.GetDay(),
					cal.GetDateTime().Hour, cal.GetDateTime().Minute, cal.GetDateTime().Second, cal.GetDateTime().Millisecond);
				cal.SetDateTime(dt);
				binGpsStamp = new XMPDateTimeImpl(cal);
				gpsDateTime.SetValue(XMPUtils.ConvertFromDate(binGpsStamp));
			}
			catch (XMPException) {
			}
		}

		/// <summary>Remove all empty schemas from the metadata tree that were generated during the rdf parsing.
		/// 	</summary>
		/// <param name="tree">the root of the metadata tree</param>
		private static void DeleteEmptySchemas(XMPNode tree)
		{
			// Delete empty schema nodes. Do this last, other cleanup can make empty
			// schema.
			List<XMPNode> schemasToRemove = new List<XMPNode>();
			foreach (XMPNode schema in tree.GetChildren()) {
				if (!schema.HasChildren()) {
					schemasToRemove.Add(schema);
				}
			}
			foreach (XMPNode xmpNode in schemasToRemove) {
				tree.GetChildren().Remove(xmpNode);
			}
		}

		/// <summary>The outermost call is special.</summary>
		/// <remarks>
		/// The outermost call is special. The names almost certainly differ. The
		/// qualifiers (and hence options) will differ for an alias to the x-default
		/// item of a langAlt array.
		/// </remarks>
		/// <param name="aliasNode">the alias node</param>
		/// <param name="baseNode">the base node of the alias</param>
		/// <param name="outerCall">marks the outer call of the recursion</param>
		private static void CompareAliasedSubtrees(XMPNode aliasNode, XMPNode baseNode, bool
			 outerCall)
		{
			if (!aliasNode.GetValue().Equals(baseNode.GetValue())
				|| aliasNode.GetChildrenLength() != baseNode.GetChildrenLength()) {
				throw new XMPException("Mismatch between alias and base nodes", XMPError.BADXMP);
			}

			if (!outerCall &&
				(!aliasNode.GetName().Equals(baseNode.GetName()) || !aliasNode.GetOptions().Equals(baseNode.GetOptions()) ||
					aliasNode.GetQualifierLength() != baseNode.GetQualifierLength())) {
				throw new XMPException("Mismatch between alias and base nodes", XMPError.BADXMP);
			}

			for (IEnumerator an = aliasNode.IterateChildren(), bn = baseNode.IterateChildren();
				an.MoveNext() && bn.MoveNext();) {
				XMPNode aliasChild = (XMPNode) an.Current;
				XMPNode baseChild = (XMPNode) bn.Current;
				CompareAliasedSubtrees(aliasChild, baseChild, false);
			}


			for (IEnumerator an = aliasNode.IterateQualifier(), bn = baseNode.IterateQualifier();
				an.MoveNext() && bn.MoveNext();) {
				XMPNode aliasQual = (XMPNode) an.Current;
				XMPNode baseQual = (XMPNode) bn.Current;
				CompareAliasedSubtrees(aliasQual, baseQual, false);
			}
		}

		/// <summary>
		/// The initial support for WAV files mapped a legacy ID3 audio copyright
		/// into a new xmpDM:copyright property.
		/// </summary>
		/// <remarks>
		/// The initial support for WAV files mapped a legacy ID3 audio copyright
		/// into a new xmpDM:copyright property. This is special case code to migrate
		/// that into dc:rights['x-default']. The rules:
		/// <pre>
		/// 1. If there is no dc:rights array, or an empty array -
		/// Create one with dc:rights['x-default'] set from double linefeed and xmpDM:copyright.
		/// 2. If there is a dc:rights array but it has no x-default item -
		/// Create an x-default item as a copy of the first item then apply rule #3.
		/// 3. If there is a dc:rights array with an x-default item,
		/// Look for a double linefeed in the value.
		/// A. If no double linefeed, compare the x-default value to the xmpDM:copyright value.
		/// A1. If they match then leave the x-default value alone.
		/// A2. Otherwise, append a double linefeed and
		/// the xmpDM:copyright value to the x-default value.
		/// B. If there is a double linefeed, compare the trailing text to the xmpDM:copyright value.
		/// B1. If they match then leave the x-default value alone.
		/// B2. Otherwise, replace the trailing x-default text with the xmpDM:copyright value.
		/// 4. In all cases, delete the xmpDM:copyright property.
		/// </pre>
		/// </remarks>
		/// <param name="xmp">the metadata object</param>
		/// <param name="dmCopyright">the "dm:copyright"-property</param>
		private static void MigrateAudioCopyright(XMPMeta xmp, XMPNode dmCopyright)
		{
			try
			{
				XMPNode dcSchema = XMPNodeUtils.FindSchemaNode(((XMPMetaImpl)xmp).GetRoot(), XMPConst
					.NS_DC, true);
				String dmValue = dmCopyright.GetValue();
				String doubleLF = "\n\n";
				XMPNode dcRightsArray = XMPNodeUtils.FindChildNode(dcSchema, "dc:rights", false);
				if (dcRightsArray == null || !dcRightsArray.HasChildren())
				{
					// 1. No dc:rights array, create from double linefeed and xmpDM:copyright.
					dmValue = doubleLF + dmValue;
					xmp.SetLocalizedText(XMPConst.NS_DC, "rights", "", XMPConst.X_DEFAULT, dmValue, null
						);
				}
				else
				{
					int xdIndex = XMPNodeUtils.LookupLanguageItem(dcRightsArray, XMPConst.X_DEFAULT);
					if (xdIndex < 0)
					{
						// 2. No x-default item, create from the first item.
						String firstValue = dcRightsArray.GetChild(1).GetValue();
						xmp.SetLocalizedText(XMPConst.NS_DC, "rights", "", XMPConst.X_DEFAULT, firstValue
							, null);
						xdIndex = XMPNodeUtils.LookupLanguageItem(dcRightsArray, XMPConst.X_DEFAULT);
					}
					// 3. Look for a double linefeed in the x-default value.
					XMPNode defaultNode = dcRightsArray.GetChild(xdIndex);
					String defaultValue = defaultNode.GetValue();
					int lfPos = defaultValue.IndexOf(doubleLF);
					if (lfPos < 0)
					{
						// 3A. No double LF, compare whole values.
						if (!dmValue.Equals(defaultValue))
						{
							// 3A2. Append the xmpDM:copyright to the x-default
							// item.
							defaultNode.SetValue(defaultValue + doubleLF + dmValue);
						}
					}
					else
					{
						// 3B. Has double LF, compare the tail.
						if (!defaultValue.Substring(lfPos + 2).Equals(dmValue))
						{
							// 3B2. Replace the x-default tail.
							defaultNode.SetValue(defaultValue.JSubstring(0, lfPos + 2) + dmValue);
						}
					}
				}
				// 4. Get rid of the xmpDM:copyright.
				dmCopyright.GetParent().RemoveChild(dmCopyright);
			}
			catch (XMPException)
			{
			}
		}

		// Don't let failures (like a bad dc:rights form) stop other
		// cleanup.
		/// <summary>
		/// Initializes the map that contains the known arrays, that are fixed by
		/// <see cref="NormalizeDCArrays(XMPNode)"/>
		/// .
		/// </summary>
		private static void InitDCArrays()
		{
			dcArrayForms = new Hashtable();
			// Properties supposed to be a "Bag".
			PropertyOptions bagForm = new PropertyOptions();
			bagForm.SetArray(true);
			dcArrayForms["dc:contributor"] = bagForm;
			dcArrayForms["dc:language"] = bagForm;
			dcArrayForms["dc:publisher"] = bagForm;
			dcArrayForms["dc:relation"] = bagForm;
			dcArrayForms["dc:subject"] = bagForm;
			dcArrayForms["dc:type"] = bagForm;
			// Properties supposed to be a "Seq".
			PropertyOptions seqForm = new PropertyOptions();
			seqForm.SetArray(true);
			seqForm.SetArrayOrdered(true);
			dcArrayForms["dc:creator"] = seqForm;
			dcArrayForms["dc:date"] = seqForm;
			// Properties supposed to be an "Alt" in alternative-text form.
			PropertyOptions altTextForm = new PropertyOptions();
			altTextForm.SetArray(true);
			altTextForm.SetArrayOrdered(true);
			altTextForm.SetArrayAlternate(true);
			altTextForm.SetArrayAltText(true);
			dcArrayForms["dc:description"] = altTextForm;
			dcArrayForms["dc:rights"] = altTextForm;
			dcArrayForms["dc:title"] = altTextForm;
		}
	}
}
